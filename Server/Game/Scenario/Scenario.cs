using CommonClass;
using CommonClass.Game;
using CommonClassLibrary;
using SanguoshaServer.AI;
using SanguoshaServer.Game;
using SanguoshaServer.Package;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using static CommonClass.Game.DamageStruct;
using static CommonClass.Game.Player;
using static SanguoshaServer.Package.FunctionCard;

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
        protected List<Skill> skills = new List<Skill>();
        protected Dictionary<Player, List<string>> reserved = new Dictionary<Player, List<string>>();
        public abstract void Assign(Room room);
        public abstract void PrepareForStart(Room room, ref List<Player> room_players, ref List<int> game_cards, ref List<int> m_drawPile);
        public abstract void PrepareForPlayers(Room room);
        public abstract void OnChooseGeneralReply(Room room, Interactivity client);
        public virtual RoomThread GetThread(Room room)
        {
            return new RoomThread(room, rule);
        }
        public virtual void SeatReAdjust(Room room, ref List<Player> players)
        {
        }
        public List<Skill> Skills => skills;
        public abstract bool IsFriendWith(Room room, Player player, Player other);
        public abstract bool WillBeFriendWith(Room room, Player player, Player other, string show_skill = null);
        public abstract TrustedAI GetAI(Room room, Player player);

        public virtual bool IsFull(Room room)
        {
            return room.Clients.Count >= room.Setting.PlayerNum;
        }

        public virtual Player Marshal(Room room, Client client, Player player)
        {
            Player _player = new Player();
            _player.Copy(player);
            if (player.ClientId == client.UserID)
            {
                _player.ActualGeneral1 = player.ActualGeneral1;
                _player.ActualGeneral2 = player.ActualGeneral2;
                _player.General1 = player.General1;
                _player.General2 = player.General2;
                _player.Role = player.Role;
                _player.Kingdom = player.Kingdom;
                _player.HeadSkills = player.HeadSkills;
                _player.DeputySkills = player.DeputySkills;
                _player.HeadSkinId = player.HeadSkinId;
                _player.DeputySkinId = player.DeputySkinId;
                _player.Piles = player.Piles;
                foreach (string mark in player.Marks.Keys)
                    _player.SetMark(mark, player.GetMark(mark));
                foreach (string mark in player.StringMarks.Keys)
                    if (mark != "spirit" && mark != "huashen")
                        _player.SetMark(mark, player.GetMark(mark));

                if (player.ContainsTag("spirit"))
                    _player.SetTag("spirit", player.GetTag("spirit"));

                if (player.ContainsTag("huashen"))
                    _player.SetTag("huashen", player.GetTag("huashen"));

                _player.SetFlags("marshal");
                _player.Status = "online";
            }
            else
            {
                List<string> invisebale_marks = new List<string>();
                if (player.General1Showed)
                {
                    _player.General1 = player.General1;
                    _player.HeadSkills = player.HeadSkills;
                    _player.HeadSkinId = player.HeadSkinId;
                }
                else
                {
                    _player.General1 = "anjiang";
                    foreach (string skill_name in player.HeadSkills.Keys)
                    {
                        Skill skill = Engine.GetSkill(skill_name);
                        if (!string.IsNullOrEmpty(skill.LimitMark))
                            invisebale_marks.Add(skill.LimitMark);
                    }
                }
                if (!string.IsNullOrEmpty(player.General2))
                {
                    if (player.General2Showed)
                    {
                        _player.General2 = player.General2;
                        _player.DeputySkills = player.DeputySkills;
                        _player.DeputySkinId = player.DeputySkinId;
                    }
                    else
                    {
                        _player.General2 = "anjiang";
                        foreach (string skill_name in player.DeputySkills.Keys)
                        {
                            Skill skill = Engine.GetSkill(skill_name);
                            if (!string.IsNullOrEmpty(skill.LimitMark))
                                invisebale_marks.Add(skill.LimitMark);
                        }
                    }
                    foreach (string mark in player.StringMarks.Keys)
                        _player.SetMark(mark, player.GetMark(mark));
                }

                if (player.HasShownOneGeneral())
                    _player.Kingdom = player.Kingdom;
                else
                    _player.Kingdom = "god";

                if (room.Setting.GameMode == "Hegemony" && player.HasShownOneGeneral() || player.RoleShown)
                    _player.Role = player.Role;

                foreach (string mark in player.Marks.Keys)
                {
                    if (mark.StartsWith("@") && !invisebale_marks.Contains(mark))
                        _player.SetMark(mark, player.GetMark(mark));
                }

                foreach (string pile in player.Piles.Keys)
                {
                    bool open = false;
                    foreach (Player p in room.Players)
                    {
                        if (p.ClientId == client.UserID && player.GetPileOpener(pile).Contains(p.Name))
                        {
                            open = true;
                            break;
                        }
                    }

                    if (open)
                    {
                        _player.PileChange(pile, player.GetPile(pile));
                    }
                    else
                    {
                        List<int> ids = new List<int>();
                        for (int i = 0; i < player.GetPile(pile).Count; i++)
                            ids.Add(-1);

                        _player.PileChange(pile, ids);
                    }
                }
            }

            return _player;
        }

        public abstract List<Interactivity> CheckSurrendAvailable(Room room);
        public abstract string GetPreWinner(Room room, Client surrender_client);
    }

    public abstract class GameRule : TriggerSkill
    {
        public GameRule(string name) : base(name)
        {
            events = new List<TriggerEvent>
            {
                TriggerEvent.GameStart, TriggerEvent.TurnStart, TriggerEvent.EventPhaseProceeding, TriggerEvent.EventPhaseEnd, TriggerEvent.EventPhaseChanging,
                TriggerEvent.PreCardUsed, TriggerEvent.CardUsed, TriggerEvent.CardFinished, TriggerEvent.CardEffect, TriggerEvent.CardEffected,
                TriggerEvent.PostHpReduced, TriggerEvent.EventLoseSkill, TriggerEvent.EventAcquireSkill, TriggerEvent.AskForPeaches,
                TriggerEvent.AskForPeachesDone, TriggerEvent.BuryVictim, TriggerEvent.BeforeGameOverJudge, TriggerEvent.GameOverJudge,
                TriggerEvent.SlashHit, TriggerEvent.SlashEffected, TriggerEvent.SlashProceed, TriggerEvent.ConfirmDamage, TriggerEvent.DamageDone, TriggerEvent.DamageComplete,
                TriggerEvent.StartJudge, TriggerEvent.JudgeResult, TriggerEvent.FinishJudge,
                TriggerEvent.ChoiceMade, TriggerEvent.GeneralShown, TriggerEvent.BeforeCardsMove, TriggerEvent.Death, TriggerEvent.CardsMoveOneTime
            };
        }
        
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
            foreach (Player p in room.Players)
                room.DrawCards(p, 4, "gamerule");

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
            player.AddHistory(Analeptic.ClassName, 0);         //clear Analeptic

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
                            room.DrawCards(player, num, "gamerule");
                        room.RoomThread.Trigger(TriggerEvent.AfterDrawNCards, room, player, ref data);

                        break;
                    }
                case PlayerPhase.Play:
                    {
                        bool add_index = true;
                        while (player.Alive && string.IsNullOrEmpty(room.PreWinner))
                        {
                            room.Activate(player, out CardUseStruct card_use, add_index);
                            if (card_use.Card != null)
                                add_index = room.UseCard(card_use);
                            else
                                break;
                        }
                        break;
                    }
                case PlayerPhase.Discard:
                    {
                        List<int> handcards = new List<int>();
                        foreach (int id in player.GetCards("h"))
                            if (!Engine.IgnoreHandCard(room, player, id))
                                handcards.Add(id);

                        int discard_num = handcards.Count - RoomLogic.GetMaxCards(room, player);
                        if (discard_num > 0)
                            room.AskForDiscard(player, handcards, "gamerule", discard_num, discard_num);
                        
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
                player.AddHistory(Analeptic.ClassName, 0);     //clear Analeptic
                foreach (Player p in room.GetAllPlayers())
                    p.SetMark("multi_kill_count", 0);
            }
        }

        protected virtual void OnPhaseChanging(Room room, Player player, ref object data)
        {
            PhaseChangeStruct change = (PhaseChangeStruct)data;
            if (change.To == PlayerPhase.NotActive)
            {
                if (player.GetMark("TurnPlayed") == 0) player.SetMark("TurnPlayed", 1);
                foreach (Player p in room.GetAlivePlayers())
                    p.SetFlags(".");

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
                if (room.ContainsTag("EdictInvoke") && (bool)room.GetTag("EdictInvoke"))
                {
                    room.SetTag("EdictInvoke", false);
                    LogMessage log = new LogMessage
                    {
                        Type = "#EdictEffect",
                        From = player.Name,
                        Arg = Edict.ClassName
                    };
                    room.SendLog(log);
                    WrappedCard io = (WrappedCard)room.GetTag("EdictCard");
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
                if (!player.General1Showed && Engine.GetGeneral(player.ActualGeneral1, room.Setting.GameMode).IsLord())
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
                    fcard.DoPreAction(room, player, card);
                    data = card_use;
                }
                
                room.AddUseList(card_use);
                card_use.EffectCount = new List<CardBasicEffect>();
                foreach (Player p in card_use.To)
                    card_use.EffectCount.Add(fcard.FillCardBasicEffct(room, p));
                data = card_use;

                if (card_use.From != null)
                {
                    thread.Trigger(TriggerEvent.TargetChoosing, room, card_use.From, ref data);
                    CardUseStruct new_use = (CardUseStruct)data;
                }

                card_use = (CardUseStruct)data;
                if (card_use.From != null && card_use.To.Count > 0)
                {
                    foreach (CardBasicEffect effect in card_use.EffectCount)
                        effect.Triggered = false;

                    while (card_use.EffectCount.Count > 0)
                    {
                        bool check = true;
                        int count = card_use.EffectCount.Count;
                        for (int i = 0; i < count; i++)
                        {
                            CardBasicEffect effect = card_use.EffectCount[i];
                            if (!effect.Triggered)
                            {
                                check = false;
                                thread.Trigger(TriggerEvent.TargetConfirming, room, effect.To, ref data);
                                effect.Triggered = true;
                                break;
                            }
                        }

                        if (check)
                            break;

                        card_use = (CardUseStruct)data;
                    }
                }

                card_use = (CardUseStruct)data;
                
                if (card_use.From != null && card_use.To.Count > 0)
                {
                    thread.Trigger(TriggerEvent.TargetChosen, room, card_use.From, ref data);                        
                    for (int i = 0; i < card_use.EffectCount.Count; i++)
                    {
                        CardBasicEffect effect = card_use.EffectCount[i];
                        effect.Triggered = false;
                        thread.Trigger(TriggerEvent.TargetConfirmed, room, effect.To, ref data);
                        effect.Triggered = true;
                    }
                }

                card_use = (CardUseStruct)data;
                fcard.Use(room, card_use);
            }
        }
        protected virtual void OnAskforPeach(Room room, Player player, ref object data)
        {
            DyingStruct dying = (DyingStruct)data;

            bool askforpeach = true;
            Interactivity client = room.GetInteractivity(player);
            List<Player> controlls = new List<Player> { player };
            if (client != null)
                controlls = room.GetPlayers(client.ClientId);

            if (!controlls.Contains(dying.Who))
            {
                List<Player> players = room.GetAllPlayers();
                int index = players.IndexOf(player);
                for (int i = 0; i < index; i++)
                {
                    Player p = players[i];
                    if (controlls.Contains(p))
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
                    CardUseStruct use = room.AskForSinglePeach(player, dying.Who, dying);
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
        protected abstract void OnBuryVictim(Room room, Player player, ref object data);
        protected virtual void RewardAndPunish(Room room, Player killer, Player victim)
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
                room.DrawCards(killer, n, "gamerule");
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
            if (room.SkipGameRule)
            {
                room.SkipGameRule = false;
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
                    //room.ClearCardFlag(use.Card);
                    use.Card.ClearFlags();                  //RoomCard会在其移动后自动清除flag
                    room.RemoveSubCards(use.Card);

                    //以askforcard形式使用的卡牌没有onUse的trigger，但有finish
                    if (use.Reason != CardUseStruct.CardUseReason.CARD_USE_REASON_RESPONSE)
                    {
                        room.RemoveUseOnFinish();
                    }

                    if (Engine.GetFunctionCard(use.Card.Name).IsNDTrick()) room.RemoveHegNullification(use.Card);

                    foreach (Client p in room.Clients)
                        room.DoNotify(p, CommandType.S_COMMAND_NULLIFICATION_ASKED, new List<string> { "." });

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

                    CheckBigKingdoms(room);
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
                        break;
                    }
                case TriggerEvent.DamageDone:
                    {
                        damage = (DamageStruct)data;
                        if (damage.From != null && !damage.From.Alive) damage.From = null;
                        room.SendDamageLog(damage);

                        if (damage.Nature != DamageNature.Normal && player.Chained && !damage.Chain && !damage.ChainStarter)
                            damage.ChainStarter = true;

                        data = damage;

                        bool reduce = !room.ApplyDamage(player, damage);

                        if (reduce)
                            room.RoomThread.Trigger(TriggerEvent.PostHpReduced, room, player, ref data);

                        break;
                    }
                case TriggerEvent.DamageComplete:
                    {
                        damage = (DamageStruct)data;
                        if (damage.Prevented)
                            return false;
                        /*
                        if (damage.Nature != DamageNature.Normal && player.Chained)
                        {
                            room.ChainedRemoveOnDamageDone(player, damage);
                        }
                        */
                        if (damage.Nature != DamageNature.Normal && !damage.Chain && damage.ChainStarter)      // iron chain effect
                        {
                            List<Player> chained_players = new List<Player>();
                            if (!room.Current.Alive)
                                chained_players = room.GetOtherPlayers(room.Current);
                            else
                                chained_players = room.GetAllPlayers();
                            chained_players.Remove(damage.To);
                            foreach (Player chained_player in chained_players)
                            {
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
                                CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_DELAYTRICK_EFFECT,
                                    effect.To.Name, effect.Card.Skill, effect.Card.Name)
                                {
                                    Card = effect.Card
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
                            if (!(fcard is Slash) && effect.BasicEffect.Nullified)
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

                            if (effect.To.Alive || fcard is Slash)
                                fcard.OnEffect(room, effect);
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
                        string pattern = string.IsNullOrEmpty(effect.RespondPattern) ? Jink.ClassName : effect.RespondPattern;
                        if (!effect.To.Alive)
                            break;
                        if (effect.Jink_num == 1)
                        {
                            CardResponseStruct resp = room.AskForCard(effect.To, Slash.ClassName, pattern, string.Format("slash-jink:{0}::{1}", slasher, effect.Slash.Name),
                                data, HandlingMethod.MethodUse, null, effect.From, false, false);
                            room.SlashResult(effect, room.IsJinkEffected(effect.To, resp) ? resp.Card : null);
                        }
                        else
                        {
                            WrappedCard jink = new WrappedCard(DummyCard.ClassName);
                            for (int i = effect.Jink_num; i > 0; i--)
                            {
                                string prompt = string.Format("@multi-jink{0}:{1}::{2}:{3}" , i == effect.Jink_num ? "-start" : string.Empty, slasher, i, effect.Slash.Name);
                                CardResponseStruct resp = room.AskForCard(effect.To, Slash.ClassName, pattern, prompt, data, HandlingMethod.MethodUse, null, effect.From, false, false);

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
                        if (effect.Drank > 0)
                        {
                            LogMessage log = new LogMessage
                            {
                                Type = "#AnalepticBuff",
                                From = effect.From.Name,
                                To = new List<string> { effect.To.Name },
                                Arg = (1 + effect.ExDamage).ToString(),
                                Arg2 = (1 + effect.ExDamage + effect.Drank).ToString()
                            };

                            room.SendLog(log);
                        }
                        DamageStruct slash_damage = new DamageStruct(effect.Slash, effect.From, effect.To, 1 + effect.ExDamage + effect.Drank, effect.Nature)
                        {
                            Drank = effect.Drank > 0
                        };
                        room.Damage(slash_damage);
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
                            new CardMoveReason(MoveReason.S_REASON_JUDGE, judge_struct.Who.Name, null, null, judge_struct.Reason), true);

                        Thread.Sleep(500);
                        bool effected = judge_struct.Good == Engine.MatchExpPattern(room, judge_struct.Pattern, judge_struct.Who, judge_struct.Card);
                        judge_struct.UpdateResult(effected);
                        data = judge_struct;
                        break;
                    }
                case TriggerEvent.JudgeResult:
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
                            CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_JUDGEDONE, judge.Who.Name, null, judge.Reason);
                            room.MoveCardTo(judge.Card, judge.Who, null, Place.DiscardPile, reason, true);
                        }

                        break;
                    }
                case TriggerEvent.ChoiceMade:
                    {
                        foreach (Player p in room.GetAlivePlayers()) {
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
                                if (move.Reason.Reason != MoveReason.S_REASON_USE)
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
                                    if (card.Name == Edict.ClassName && !card.HasFlag("edict_normal_use"))
                                        should_find_io = true; // use card isn't IO
                                }
                            }
                            if (should_find_io)
                            {
                                foreach (int id in move.Card_ids) {
                                    WrappedCard card = room.GetCard(id);
                                    if (card.Name == Edict.ClassName)
                                    {
                                        room.MoveCardTo(card, null, Place.PlaceTable, true);
                                        room.AddToPile(room.Players[0], "#edict", card, false);
                                        LogMessage log = new LogMessage
                                        {
                                            Type = "#RemoveEdict",
                                            Arg = Edict.ClassName
                                        };
                                        room.SendLog(log);
                                        room.SetTag("EdictInvoke", true);
                                        room.SetTag("EdictCard", card);
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
                                    if (card.Name == JadeSeal.ClassName)
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
                                    if (card.Name == JadeSeal.ClassName)
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
