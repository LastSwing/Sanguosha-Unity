using CommonClass;
using CommonClass.Game;
using CommonClassLibrary;
using SanguoshaServer.Game;
using System.Collections.Generic;
using System.Threading;
using static CommonClass.Game.DamageStruct;
using static CommonClass.Game.Player;
using static SanguoshaServer.Game.FunctionCard;

namespace SanguoshaServer.Scenario
{
    public abstract class GameScenario
    {
        public string Name => mode_name;

        protected string mode_name;
        protected string lord;
        protected List<string> loyalists, rebels, renegades;
        protected GameRule rule;
        protected bool random_seat;
        
        public abstract void Assign(Room room);
        public abstract void PrepareForStart(Room room, ref List<Player> room_players, ref List<int> game_cards, ref List<int> m_drawPile);
        public abstract List<string> GetWinners(Room room);
        public virtual RoomThread GetThread(Room room)
        {
            return new RoomThread(room, rule);
        }
        public abstract bool IsFriendWith(Room room, Player player, Player other);
        public abstract bool WillBeFriendWith(Room room, Player player, Player other);

    }

    public abstract class GameRule : TriggerSkill
    {
        public GameRule(string name) : base(name)
        {
            AddTrigger();
            AddRuleSkill();
        }

        protected virtual void AddTrigger()
        {
            events = new List<TriggerEvent>
            {
                TriggerEvent.GameStart, TriggerEvent.TurnStart, TriggerEvent.EventPhaseProceeding, TriggerEvent.EventPhaseEnd, TriggerEvent.EventPhaseChanging,
                TriggerEvent.PreCardUsed, TriggerEvent.CardUsed, TriggerEvent.CardFinished, TriggerEvent.CardEffect, TriggerEvent.CardEffected,
                TriggerEvent.PostHpReduced, TriggerEvent.EventLoseSkill, TriggerEvent.EventAcquireSkill, TriggerEvent.AskForPeaches,
                TriggerEvent.AskForPeachesDone, TriggerEvent.BuryVictim, TriggerEvent.BeforeGameOverJudge, TriggerEvent.GameOverJudge,
                TriggerEvent.SlashHit, TriggerEvent.SlashEffected, TriggerEvent.SlashProceed, TriggerEvent.ConfirmDamage, TriggerEvent.DamageDone, TriggerEvent.DamageComplete,
                TriggerEvent.StartJudge, TriggerEvent.FinishRetrial, TriggerEvent.FinishJudge,
                TriggerEvent.ChoiceMade, TriggerEvent.GeneralShown, TriggerEvent.BeforeCardsMove, TriggerEvent.Death, TriggerEvent.CardsMoveOneTime
            };
        }

        protected abstract void AddRuleSkill();

        public override int GetPriority() => 0;
        public abstract string GetWinner(Room room);
        public override bool Triggerable(Player player, Room room)
        {
            return true;
        }
        //handle trigger
        protected virtual void OnGameStart(Room room, ref object data)
        {

            foreach (Player player in room.Players) {
                foreach (string skill_name in player.GetSkills()) {
                    Skill skill = Engine.GetSkill(skill_name);
                    if (skill.SkillFrequency == Frequency.Limited && !string.IsNullOrEmpty(skill.LimitMark))
                    {
                        player.SetMark(skill.LimitMark, 1);
                        List<string> arg = new List<string> { player.Name, skill.LimitMark, "1" };
                        room.DoNotify(room.GetClient(player), CommandType.S_COMMAND_SET_MARK, arg);
                    }
                }
            }
            room.SetTag("FirstRound", true);
            room.DrawCards(room.Players, 4);

            if (room.Setting.LuckCard)
                room.AskForLuckCard(3);

            //游戏开始动画
            room.DoBroadcastNotify(CommandType.S_COMMAND_GAME_START, new List<string>());
            Thread.Sleep(2000);
        }
        protected virtual void OnTurnStart(Room room, Player target, ref object data)
        {
            Player player = room.Current;
            LogMessage log = new LogMessage
            {
                Type = "$AppendSeparator"
            };
            room.SendLog(log);
            //player.AddMark("Global_TurnCount");      回合不该如此计算
            player.AddHistory("Analeptic", 0);         //clear Analeptic

            if (!player.FaceUp)
            {
                room.TurnOver(player);
            }
            else if (player.Alive)
                room.Play(player);
        }
        protected virtual void OnPhaseProceed(Room room, Player player, ref object data)
        {
            switch (player.Phase)
            {
                case PlayerPhase.Judge:
                    {
                        List <int> tricks = new List<int>(player.JudgingArea);
                        while (tricks.Count > 0 && player.Alive)
                        {
                            WrappedCard trick = room.GetCard(tricks[tricks.Count - 1]);
                            tricks.RemoveAt(tricks.Count - 1);
                            bool on_effect = room.CardEffect(trick, null, player);
                            if (!on_effect)
                            {
                                FunctionCard card = Engine.GetFunctionCard(trick.Name);
                                card.OnNullified(room, player, trick);
                            }
                        }
                        break;
                    }
                case PlayerPhase.Draw:
                    {
                        int num = (int)data;
                        if (num > 0)
                            room.DrawCards(player, num);
                        room.RoomThread.Trigger(TriggerEvent.AfterDrawNCards, room, player, ref data);

                        break;
                    }
                case PlayerPhase.Play:
                    {
                        while (player.Alive)
                        {
                            room.Activate(player, out CardUseStruct card_use);
                            if (card_use.Card != null)
                                room.UseCard(card_use);
                            else
                                break;
                        }
                        break;
                    }
                case PlayerPhase.Discard:
                    {
                        int discard_num = player.HandcardNum - RoomLogic.GetMaxCards(room, player);
                        if (discard_num > 0)
                            room.AskForDiscard(player, "gamerule", discard_num, discard_num);
                        
                        break;
                    }
                default:
                    break;
            }
        }
        protected virtual void OnPhaseEnd(Room room, Player player, ref object data)
        {
            if (player.Phase == PlayerPhase.Play)
                player.AddHistory(".");
            if (player.Phase == PlayerPhase.Finish)
            {
                player.AddHistory("Analeptic", 0);     //clear Analeptic
                foreach (Player p in room.GetAllPlayers())
                    p.SetMark("multi_kill_count", 0);
            }
        }

        protected virtual void OnPhaseChanging(Room room, Player player, ref object data)
        {
            PhaseChangeStruct change = (PhaseChangeStruct)data;
            if (change.To == PlayerPhase.NotActive)
            {
                player.SetFlags(".");
                RoomLogic.ClearPlayerCardLimitation(player, true);
                foreach (Player p in room.GetAllPlayers()) {
                    if (p.GetMark("drank") > 0)
                    {
                        LogMessage log = new LogMessage
                        {
                            Type = "#UnsetDrankEndOfTurn",
                            From = p.Name
                        };
                        room.SendLog(log);

                        room.SetPlayerMark(p, "drank", 0);
                    }
                }
                if (room.ContainsTag("ImperialOrderInvoke") && (bool)room.GetTag("ImperialOrderInvoke"))
                {
                    room.SetTag("ImperialOrderInvoke", false);
                    LogMessage log = new LogMessage
                    {
                        Type = "#ImperialOrderEffect",
                        From = player.Name,
                        Arg = "imperial_order"
                    };
                    room.SendLog(log);
                    WrappedCard io = (WrappedCard)room.GetTag("ImperialOrderCard");
                    if (io != null)
                    {
                        FunctionCard fcard = Engine.GetFunctionCard(io.Name);
                        foreach (Player p in room.GetAllPlayers()) {
                            if (!p.HasShownOneGeneral() && Engine.IsProhibited(room, null, p, io) == null) // from is null!
                                room.CardEffect(io, null, p);
                        }
                    }
                }
            }
            else if (change.To == PlayerPhase.Play)
            {
                player.AddHistory(".");
            }
            else if (change.To == PlayerPhase.Start)
            {
                if (!player.General1Showed && Engine.GetGeneral(player.ActualGeneral1).IsLord())
                    room.ShowGeneral(player);
            }
        }
        protected virtual void OnPreCardUsed(Room room, Player player, ref object data)
        {
            if (data is CardUseStruct card_use)
            {
                if (card_use.From.HasFlag("Global_ForbidSurrender"))
                {
                    card_use.From.SetFlags("-Global_ForbidSurrender");
                    room.DoNotify(room.GetClient(card_use.From), CommandType.S_COMMAND_ENABLE_SURRENDER, new List<string> { true.ToString() });
                }

                room.BroadcastSkillInvoke(card_use.From, card_use.Card);
                if (!string.IsNullOrEmpty(card_use.Card.Skill) && card_use.Card.GetSkillName() == card_use.Card.Skill
                    && card_use.IsOwnerUse && RoomLogic.PlayerHasSkill(room, card_use.From, card_use.Card.Skill))
                    room.NotifySkillInvoked(card_use.From, card_use.Card.Skill);
            }
        }
        protected virtual void OnCardUsed(Room room, Player player, ref object data)
        {
            if (data is CardUseStruct card_use)
            {
                RoomThread thread = room.RoomThread;

                FunctionCard fcard = Engine.GetFunctionCard(card_use.Card.Name);
                WrappedCard card = card_use.Card;
                if (fcard.HasPreact)
                {
                    fcard.DoPreAction(room, card);
                    data = card_use;
                }

                List<Player> targets = card_use.To;
                List<CardUseStruct> use_list = room.ContainsTag("card_proceeing") ?
                    (List<CardUseStruct>)room.GetTag("card_proceeing") : new List<CardUseStruct>();                    //for serval purpose, such as AI
                use_list.Add(card_use);
                room.SetTag("card_proceeing", use_list);

                if (card_use.From != null)
                {
                    thread.Trigger(TriggerEvent.TargetChoosing, room, card_use.From, ref data);
                    CardUseStruct new_use = (CardUseStruct)data;
                    targets = new_use.To;
                }

                if (card_use.From != null && targets.Count > 0)
                {
                    List<Player> targets_copy = new List<Player>(targets);
                    foreach (Player to in targets_copy) {
                        if (targets.Contains(to))
                        {
                            thread.Trigger(TriggerEvent.TargetConfirming, room, to, ref data);
                            CardUseStruct new_use = (CardUseStruct)data;
                            targets = new_use.To;
                            if (targets.Count == 0) break;
                        }
                    }
                }

                card_use = (CardUseStruct)data;
                if (card_use.From != null && card_use.To.Count > 0)
                {
                    thread.Trigger(TriggerEvent.TargetChosen, room, card_use.From, ref data);
                    foreach (Player p in card_use.To)
                        thread.Trigger(TriggerEvent.TargetConfirmed, room, p, ref data);
                }
                card_use = (CardUseStruct)data;
                if (card_use.NullifiedList != null)
                    room.SetTag("CardUseNullifiedList", card_use.NullifiedList);
                fcard.Use(room, card_use);
            }
        }
        protected virtual void OnAskforPeach(Room room, Player player, ref object data)
        {
            DyingStruct dying = (DyingStruct)data;

            bool askforpeach = true;
            Client client = room.GetClient(player);

            if (!client.GetPlayers().Contains(dying.Who))
            {
                List<Player> players = room.GetAllPlayers();
                int index = players.IndexOf(player);
                for (int i = 0; i < index; i++)
                {
                    Player p = players[i];
                    if (client.GetPlayers().Contains(p))
                    {
                        askforpeach = false;
                        break;
                    }
                }
            }
            else if (dying.Who != player)
                askforpeach = false;


            if (askforpeach)
            {
                while (dying.Who.Hp <= 0 && dying.Who.Alive)
                {
                    CardUseStruct use = room.AskForSinglePeach(player, dying.Who);
                    if (use.Card == null)
                        break;
                    else
                        room.UseCard(use, false);
                }
            }
        }
        protected virtual void OnDeath(Room room, Player player, ref object data)
        {
            CheckBigKingdoms(room);
        }
        protected virtual void OnBuryVictim(Room room, Player player, ref object data)
        {
            DeathStruct death = (DeathStruct)data;
            room.BuryPlayer(player);

            if (room.ContainsTag("SkipNormalDeathProcess") && (bool)room.GetTag("SkipNormalDeathProcess"))
                return;

            Player killer = death.Damage.From ?? null;
            if (killer != null)
            {
                killer.SetMark("multi_kill_count", killer.GetMark("multi_kill_count") + 1);
                int kill_count = killer.GetMark("multi_kill_count");
                if (kill_count > 1 && kill_count < 8)
                    room.SetEmotion(killer, string.Format("multi_kill%1",kill_count));
                else if (kill_count > 7)
                    room.SetEmotion(killer, "zylove");
                RewardAndPunish(room, killer, player);
            }

            if (Engine.GetGeneral(player.General1).IsLord() && player == death.Who)
            {
                foreach (Player p in room.GetOtherPlayers(player, true)) {
                    if (p.Kingdom == player.Kingdom)
                    {
                        p.Role = "careerist";
                        if (p.HasShownOneGeneral())
                        {
                            room.BroadcastProperty(p, "Role");
                        }
                        else
                        {
                            room.NotifyProperty(room.GetClient(p), p, "Role");
                        }
                    }
                }
                CheckBigKingdoms(room);
            }
        }

        private void RewardAndPunish(Room room, Player killer, Player victim)
        {
            if (!killer.Alive || !killer.HasShownOneGeneral())
                return;


            if (!RoomLogic.IsFriendWith(room, killer, victim))
            {
                int n = 1;
                foreach (Player p in room.GetOtherPlayers(victim))
                {
                    if (RoomLogic.IsFriendWith(room, victim, p))
                        ++n;
                }
                room.DrawCards(killer, n);
            }
            else
                room.ThrowAllHandCardsAndEquips(killer);
        }


        public virtual void CheckBigKingdoms(Room room)
        {
            room.DoBroadcastNotify(CommandType.S_COMMAND_LOG_EVENT, new List<string> {
                GameEventType.S_GAME_EVENT_BIG_KINGDOM.ToString(),
                JsonUntity.Object2Json(RoomLogic.GetBigKingdoms(room))});
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player target, TriggerStruct trigger_info)
        {
            if (room.ContainsTag("SkipGameRule") && (bool)room.GetTag("SkipGameRule"))
            {
                room.RemoveTag("SkipGameRule");
                return false;
            }

            // Handle global events
            if (player == null)
            {
                if (triggerEvent == TriggerEvent.GameStart)
                {
                    OnGameStart(room, ref data);
                }

                if (triggerEvent != TriggerEvent.BeforeCardsMove && triggerEvent != TriggerEvent.CardsMoveOneTime)
                    return false;
            }

            switch (triggerEvent)
            {
                case TriggerEvent.TurnStart:
                        OnTurnStart(room, player, ref data);
                        break;
                case TriggerEvent.EventPhaseProceeding:
                        OnPhaseProceed(room, player, ref data);
                        break;
                case TriggerEvent.EventPhaseEnd:
                        OnPhaseEnd(room, player, ref data);
                        break;
                case TriggerEvent.EventPhaseChanging:
                        OnPhaseChanging(room, player, ref data);
                        break;
                case TriggerEvent.PreCardUsed:
                        OnPreCardUsed(room, player, ref data);
                        break;
                case TriggerEvent.CardUsed:
                        OnCardUsed(room, player, ref data);
                        break;
                case TriggerEvent.CardFinished:
                        CardUseStruct use = (CardUseStruct)data;
                        room.ClearCardFlag(use.Card);

                        List<CardUseStruct> use_list = (List<CardUseStruct>)room.GetTag("card_proceeing");                    //remove when finished
                        if (use_list.Count != 0) use_list.RemoveAt(use_list.Count - 1);
                        room.SetTag("card_proceeing", use_list);
                        room.RemoveTag("targets" + RoomLogic.CardToString(room, use.Card));

                        if (Engine.GetFunctionCard(use.Card.Name).IsNDTrick())
                            room.RemoveTag(RoomLogic.CardToString(room, use.Card) + "HegnullificationTargets");

                        foreach (Client p in room.Clients)
                            room.DoNotify(p, CommandType.S_COMMAND_NULLIFICATION_ASKED, new List<string> { "." });
                        if (Engine.GetFunctionCard(use.Card.Name) is Slash)
                            use.From.RemoveTag("Jink_" + RoomLogic.CardToString(room, use.Card));

                        break;
                case TriggerEvent.EventAcquireSkill:
                case TriggerEvent.EventLoseSkill:
                    InfoStruct info = (InfoStruct)data;
                        string skill_name = info.Info;
                        Skill skill = Engine.GetSkill(skill_name);
                        bool refilter = skill is FilterSkill;

                        if (!refilter && skill is TriggerSkill)
                        {
                            TriggerSkill trigger = (TriggerSkill)skill;
                        ViewAsSkill vsskill = trigger.ViewAsSkill;
                            if (vsskill != null && (vsskill is FilterSkill))
                                refilter = true;
                        }

                        if (refilter)
                            room.FilterCards(player, player.GetCards("he"), triggerEvent == TriggerEvent.EventLoseSkill);

                        break;
                case TriggerEvent.PostHpReduced:
                        if (player.Hp > 0 || player.HasFlag("Global_Dying")) // newest GameRule -- a player cannot enter dying when it is dying.
                            break;
                    if (data is DamageStruct damage)
                    {
                        room.EnterDying(player, damage);
                    }
                    else
                        room.EnterDying(player, new DamageStruct());

                    break;
                case TriggerEvent.AskForPeaches:
                        OnAskforPeach(room, player, ref data);
                        break;
                case TriggerEvent.AskForPeachesDone:
                    {
                        if (player.Hp <= 0 && player.Alive)
                        {
                            DyingStruct dying = (DyingStruct)data;
                            room.KillPlayer(player, dying.Damage);
                        }

                        break;
                    }
                case TriggerEvent.ConfirmDamage:
                    {
                        damage = (DamageStruct)data;
                        if (damage.Card != null && damage.To.GetMark("SlashIsDrank") > 0)
                        {
                            LogMessage log = new LogMessage
                            {
                                Type = "#AnalepticBuff",
                                From = damage.From.Name,
                                To = new List<string> { damage.To.Name },
                                Arg = damage.Damage.ToString()
                            };

                            damage.Damage += damage.To.GetMark("SlashIsDrank");
                            damage.To.SetMark("SlashIsDrank", 0);

                            log.Arg2 = damage.Damage.ToString();

                            room.SendLog(log);

                            data = damage;
                        }

                        break;
                    }
                case TriggerEvent.DamageDone:
                    {
                        damage = (DamageStruct)data;
                        if (damage.From != null && !damage.From.Alive)
                            damage.From = null;
                        data = damage;
                        room.SendDamageLog(damage);

                        room.ApplyDamage(player, damage);
                        if (damage.Nature != DamageNature.Normal && player.Chained && !damage.Chain)
                        {
                            int n = room.ContainsTag("is_chained") ? (int)room.GetTag("is_chained") : 0;
                            n++;
                            room.SetTag("is_chained", n);
                        }
                        room.RoomThread.Trigger(TriggerEvent.PostHpReduced, room, player, ref data);

                        break;
                    }
                case TriggerEvent.DamageComplete:
                    {
                        damage = (DamageStruct)data;
                        if (damage.Prevented)
                            return false;
                        if (damage.Nature != DamageNature.Normal && player.Chained)
                        {
                            room.ChainedRemoveOnDamageDone(player);
                            //player.Chained = false;
                            //room.BroadcastProperty(player, "Chained");
                        }
                        if (room.ContainsTag("is_chained") && (int)room.GetTag("is_chained") > 0)
                        {
                            if (damage.Nature != DamageNature.Normal && !damage.Chain)
                            {
                                // iron chain effect
                                int n = (int)room.GetTag("is_chained");
                                n--;
                                room.SetTag("is_chained", n);
                                List<Player> chained_players = new List<Player>();
                                if (!room.Current.Alive)
                                    chained_players = room.GetOtherPlayers(room.Current);
                                else
                                    chained_players = room.GetAllPlayers();
                                foreach (Player chained_player in chained_players) {
                                    if (chained_player.Chained)
                                    {
                                        Thread.Sleep(500);
                                        LogMessage log = new LogMessage
                                        {
                                            Type = "#IronChainDamage",
                                            From = chained_player.Name
                                        };
                                        room.SendLog(log);

                                        DamageStruct chain_damage = damage;
                                        chain_damage.To = chained_player;
                                        chain_damage.Chain = true;
                                        chain_damage.Transfer = false;
                                        chain_damage.TransferReason = null;

                                        room.Damage(chain_damage);
                                    }
                                }
                            }
                        }
                        foreach (Player p in room.GetAllPlayers()) {
                            if (p.HasFlag("Global_DFDebut"))
                            {
                                p.SetFlags("-Global_DFDebut");
                                room.RoomThread.Trigger(TriggerEvent.DFDebut, room, p);
                            }
                        }
                        break;
                    }
                case TriggerEvent.CardEffect:
                    {
                        if (data is CardEffectStruct effect)
                        {
                            if (Engine.GetFunctionCard(effect.Card.Name) is DelayedTrick)
                            {
                                CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_DELAYTRICK_EFFECT,
                                    effect.To.Name, effect.Card.Skill, effect.Card.Name)
                                {
                                    CardString = RoomLogic.CardToString(room, effect.Card)
                                };

                                room.MoveCardTo(effect.Card, effect.To, Place.PlaceTable, reason, true);
                                Thread.Sleep(500);
                            }
                        }
                        break;
                    }
                case TriggerEvent.CardEffected:
                    {
                        if (data is CardEffectStruct effect)
                        {
                            FunctionCard fcard = Engine.GetFunctionCard(effect.Card.Name);
                            if (!(fcard is Slash) && effect.Nullified)
                            {
                                LogMessage log = new LogMessage
                                {
                                    Type = "#Cardnullified",
                                    From = effect.To.Name,
                                    Arg = effect.Card.Name
                                };
                                room.SendLog(log);

                                return true;
                            }
                            else if (fcard.TypeID == CardType.TypeTrick && room.IsCanceled(effect))
                            {
                                effect.To.SetFlags("Global_NonSkillnullify");
                                return true;
                            }
                            object _effect = effect;
                            room.RoomThread.Trigger(TriggerEvent.CardEffectConfirmed, room, effect.To, ref _effect);

                            room.SetTag("Global_CardEffected", _effect);                                         //for AI 
                            if (effect.To.Alive || fcard.IsKindOf("Slash"))
                                fcard.OnEffect(room, effect);

                            room.RemoveTag("Global_CardEffected");
                        }

                        break;
                    }
                case TriggerEvent.SlashEffected:
                    {
                        SlashEffectStruct effect = (SlashEffectStruct)data;
                        if (effect.Nullified)
                        {
                            LogMessage log = new LogMessage
                            {
                                Type = "#Cardnullified",
                                From = effect.To.Name,
                                Arg = effect.Slash.Name
                            };
                            room.SendLog(log);

                            return true;
                        }

                        if (effect.Jink_num > 0)
                            room.RoomThread.Trigger(TriggerEvent.SlashProceed, room, effect.From, ref data);
                        else
                            room.SlashResult(effect, null);
                        break;
                    }
                case TriggerEvent.SlashProceed:
                    {
                        SlashEffectStruct effect = (SlashEffectStruct)data;
                        string slasher = effect.From.Name;
                        if (!effect.To.Alive)
                            break;
                        if (effect.Jink_num == 1)
                        {
                            CardResponseStruct resp = room.AskForCard(effect.To, "Slash", "jink", "slash-jink:" + slasher, data, HandlingMethod.MethodUse, null, effect.From, false, false);
                            room.SlashResult(effect, room.IsJinkEffected(effect.To, resp) ? resp.Card : null);
                        }
                        else
                        {
                            WrappedCard jink = new WrappedCard("Dummy");
                            for (int i = effect.Jink_num; i > 0; i--)
                            {
                                string prompt = string.Format("@multi-jink{0}:{1}::{2}" , i == effect.Jink_num ? "-start" : string.Empty, slasher, i);
                                CardResponseStruct resp = room.AskForCard(effect.To, "Slash", "jink", prompt, data, HandlingMethod.MethodUse, null, effect.From, false, false);
                                if (!room.IsJinkEffected(effect.To, resp))
                                {
                                    //delete jink;
                                    room.SlashResult(effect, null);
                                    return false;
                                }
                                else
                                {
                                    jink.AddSubCard(resp.Card);
                                }
                            }
                            room.SlashResult(effect, jink);
                        }

                        break;
                    }
                case TriggerEvent.SlashHit:
                    {
                        SlashEffectStruct effect = (SlashEffectStruct)data;
                        if (effect.Drank > 0) effect.To.SetMark("SlashIsDrank", effect.Drank);
                        room.Damage(new DamageStruct(effect.Slash, effect.From, effect.To, 1, effect.Nature));

                        break;
                    }
                case TriggerEvent.BeforeGameOverJudge:
                    {
                        if (!player.General1Showed)
                            room.ShowGeneral(player, true, false, false);
                        if (!player.General2Showed)
                            room.ShowGeneral(player, false, false, false);
                        break;
                    }
                case TriggerEvent.GameOverJudge:
                    {
                        string winner = GetWinner(room);
                        if (!string.IsNullOrEmpty(winner))
                        {
                            room.GameOver(winner);
                            return true;
                        }

                        break;
                    }
                case TriggerEvent.BuryVictim:
                    {
                        OnBuryVictim(room, player, ref data);
                        break;
                    }
                case TriggerEvent.StartJudge:
                    {
                        int card_id = room.GetNCards(1)[0];
                        JudgeStruct judge_struct = (JudgeStruct)data;
                        judge_struct.Card = room.GetCard(card_id);

                        LogMessage log = new LogMessage
                        {
                            Type = "$InitialJudge",
                            From = judge_struct.Who.Name,
                            Card_str = card_id.ToString()
                        };
                        room.SendLog(log);

                        room.MoveCardTo(judge_struct.Card, null, judge_struct.Who, Place.PlaceJudge,
                            new CardMoveReason(CardMoveReason.MoveReason.S_REASON_JUDGE, judge_struct.Who.Name, null, null, judge_struct.Reason), true);

                        Thread.Sleep(500);
                        bool effected = judge_struct.Good == Engine.MatchExpPattern(room, judge_struct.Pattern, judge_struct.Who, judge_struct.Card);
                        judge_struct.UpdateResult(effected);
                        data = judge_struct;
                        break;
                    }
                case TriggerEvent.FinishRetrial:
                    {
                        JudgeStruct judge = (JudgeStruct)data;
                        LogMessage log = new LogMessage
                        {
                            Type = "$JudgeResult",
                            From = player.Name,
                            Card_str = RoomLogic.CardToString(room, judge.Card)
                        };
                        room.SendLog(log);

                        //Thread.Sleep(500);
                        if (judge.PlayAnimation)
                        {
                            room.SendJudgeResult(judge);
                            Thread.Sleep(800);
                        }

                        break;
                    }
                case TriggerEvent.FinishJudge:
                    {
                        JudgeStruct judge = (JudgeStruct)data;

                        if (room.GetCardPlace(judge.Card.Id) == Place.PlaceJudge)
                        {
                            CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_JUDGEDONE, judge.Who.Name, null, judge.Reason);
                            room.MoveCardTo(judge.Card, judge.Who, null, Place.DiscardPile, reason, true);
                        }

                        break;
                    }
                case TriggerEvent.ChoiceMade:
                    {
                        foreach (Player p in room.AlivePlayers) {
                            List<string> flags = new List<string>(p.Flags);
                            foreach (string flag in flags) {
                                if (flag.StartsWith("Global_") && flag.EndsWith("Failed"))
                                    p.SetFlags( "-" + flag);
                            }
                        }
                        break;
                    }
                case TriggerEvent.GeneralShown:
                    {
                        string winner = GetWinner(room);
                        if (!string.IsNullOrEmpty(winner))
                        {
                            room.GameOver(winner); // if all hasShownGenreal, and they are all friend, game over.
                            return true;
                        }
                        if (!room.ContainsTag("TheFirstToShowRewarded"))
                        {
                            room.SetTag("TheFirstToShowRewarded", true);
                            room.SetPlayerMark(player, "@pioneer", 1);
                            room.AttachSkillToPlayer(player, "pioneer");
                        }
                        if (player.Alive && player.HasShownAllGenerals())
                        {
                            if (player.GetMark("CompanionEffect") > 0)
                            {
                                room.RemovePlayerMark(player, "CompanionEffect");
                                room.DoSuperLightbox(player, string.Empty, "companion");
                                room.SetPlayerMark(player, "@companion", 1);
                                room.AttachSkillToPlayer(player, "companion");
                            }
                            if (player.GetMark("HalfMaxHpLeft") > 0)
                            {
                                room.RemovePlayerMark(player, "HalfMaxHpLeft");
                                room.SetPlayerMark(player, "@megatama", 1);
                                room.AttachSkillToPlayer(player, "megatama");
                            }
                        }
                        CheckBigKingdoms(room);
                        break;
                    }
                case TriggerEvent.BeforeCardsMove:
                    {
                        if (data is CardsMoveOneTimeStruct move)
                        {
                            bool should_find_io = false;
                            if (move.To_place == Place.DiscardPile)
                            {
                                if (move.Reason.Reason != CardMoveReason.MoveReason.S_REASON_USE)
                                {
                                    should_find_io = true; // not use
                                }
                                else if (move.Card_ids.Count > 1)
                                {
                                    should_find_io = true; // use card isn't IO
                                }
                                else
                                {
                                    WrappedCard card = room.GetCard(move.Card_ids[0]);
                                    if (card.Name == "ImperialOrder" && !card.HasFlag("imperial_order_normal_use"))
                                        should_find_io = true; // use card isn't IO
                                }
                            }
                            if (should_find_io)
                            {
                                foreach (int id in move.Card_ids) {
                                    WrappedCard card = room.GetCard(id);
                                    if (card.Name == "ImperialOrder")
                                    {
                                        room.MoveCardTo(card, null, Place.PlaceTable, true);
                                        room.AddToPile(room.Players[0], "#imperial_order", card, false);
                                        LogMessage log = new LogMessage
                                        {
                                            Type = "#RemoveImperialOrder",
                                            Arg = "imperial_order"
                                        };
                                        room.SendLog(log);
                                        room.SetTag("ImperialOrderInvoke", true);
                                        room.SetTag("ImperialOrderCard", card);
                                        int i = move.Card_ids.IndexOf(id);
                                        move.From_places.RemoveAt(i);
                                        move.Open.RemoveAt(i);
                                        move.From_pile_names.RemoveAt(i);
                                        move.Card_ids.Remove(id);
                                        data = move;
                                        break;
                                    }
                                }
                            }
                        }
                        break;
                    }
                case TriggerEvent.Death:
                    {
                        OnDeath(room, player, ref data);
                        break;
                    }
                case TriggerEvent.CardsMoveOneTime:
                    {
                        if (data is CardsMoveOneTimeStruct move)
                        {
                            if (move.From != null && move.From_places.Contains(Place.PlaceEquip))
                            {
                                foreach (int id in move.Card_ids) {
                                    WrappedCard card = room.GetCard(id);
                                    if (card.Name == "JadeSeal")
                                    {
                                        CheckBigKingdoms(room);
                                        break;
                                    }
                                }
                            }

                            if (move.To != null && move.To_place == Place.PlaceEquip)
                            {
                                foreach (int id in move.Card_ids) {
                                    WrappedCard card = room.GetCard(id);
                                    if (card.Name == "JadeSeal")
                                    {
                                        CheckBigKingdoms(room);
                                        break;
                                    }
                                }
                            }
                        }

                        break;
                    }
                default:
                    break;
            }

            return false;
        }
    }
}
