using CommonClass;
using CommonClass.Game;
using CommonClassLibrary;
using SanguoshaServer.Game;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using static CommonClass.Game.CardUseStruct;
using static CommonClass.Game.Player;
using static SanguoshaServer.Package.FunctionCard;

namespace SanguoshaServer.Package
{
    public class ClassicStander : GeneralPackage
    {
        public ClassicStander() : base("ClassicStander")
        {
            skills = new List<Skill>
            {
                new Jijiang(),
                new WushengJX(),
                new Yijue(),
                new YijueInvalid(),
                new PaoxiaoJX(),
                new Tishen(),
                new LongdanJX(),
                new Yajiao(),
                new TieqiJX(),
                new TieqiJXInvalid(),
                new JizhiJX(),
                new JizhiMax(),
                new QicaiJX(),
                new QicaiFix(),
                new GuanxingJX(),
                new KongchengJX(),
                new KongchengJXP(),
                new BiyueJX(),
                new Liyu(),
                new JianxiongJX(),
                new Hujia(),
                new FankuiJX(),
                new GuicaiJX(),
                new GanglieJX(),
                new Qingjian(),
                new QingjianMax(),
                new TuxiJx(),
                new LuoyiJX(),
                new LuoyiDamageJX(),
            };
            skill_cards = new List<FunctionCard>
            {
                new JijiangCard(),
                new YijueCard(),
                new HujiaCard(),
                new QingjianCard(),
            };
        }
    }

    public class Jijiang : ZeroCardViewAsSkill
    {
        public Jijiang() : base("$jijiang") { }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            if (!Slash.IsAvailable(room, player) || player.HasFlag(string.Format("jijiang_{0}", room.GetRoomState().GetCurrentResponseID())))
                return false;

            foreach (Player p in room.GetOtherPlayers(player))
                if (p.Kingdom == "shu") return true;

            return false;
        }

        public override bool IsEnabledAtResponse(Room room, Player player, string pattern)
        {
            if (pattern != "Slash" || player.HasFlag(string.Format("jijiang_{0}", room.GetRoomState().GetCurrentResponseID())))
                return false;

            foreach (Player p in room.GetOtherPlayers(player))
                if (p.Kingdom == "shu") return true;

            return false;
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard("JijiangCard") { Skill = "$jijiang", Mute = true };
        }
    }

    public class JijiangCard : SkillCard
    {
        public JijiangCard() : base("JijiangCard")
        {
        }

        public override void OnUse(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            LogMessage log = new LogMessage("$jijiang-slash")
            {
                From = player.Name,
                To = new List<string>(),
                Card_str = RoomLogic.CardToString(room, card_use.Card)
            };
            log.SetTos(card_use.To);
            room.SendLog(log);

            foreach (Player p in card_use.To)
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, p.Name);

            player.SetFlags(string.Format("jijiang_{0}", room.GetRoomState().GetCurrentResponseID()));
            room.BroadcastSkillInvoke("$jijiang", player, card_use.Card.SkillPosition);
            room.NotifySkillInvoked(player, "$jijiang");

            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (p.Kingdom == "shu")
                {
                    WrappedCard card = room.AskForCard(player, "jijiang", "Slash", string.Format("@jijiang-target:{0},{1}", player.Name, card_use.To[0].Name), null, HandlingMethod.MethodUse);
                    if (card != null)
                    {
                        CardUseStruct use = new CardUseStruct(card, player, card_use.To)
                        {
                            Reason = CardUseReason.CARD_USE_REASON_PLAY
                        };
                        room.UseCard(use);
                        return;
                    }
                }
            }
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            if (room.GetRoomState().GetCurrentCardUseReason() == CardUseReason.CARD_USE_REASON_PLAY || room.GetRoomState().GetCurrentCardUseReason() == CardUseReason.CARD_USE_REASON_RESPONSE_USE)
            {
                WrappedCard slash = new WrappedCard("Slash");
                FunctionCard fcard = Engine.GetFunctionCard("Slash");
                return fcard.TargetFilter(room, targets, to_select, Self, slash);
            }

            return false;
        }

        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            if (room.GetRoomState().GetCurrentCardUseReason() == CardUseReason.CARD_USE_REASON_PLAY || room.GetRoomState().GetCurrentCardUseReason() == CardUseReason.CARD_USE_REASON_RESPONSE_USE)
                return targets.Count > 0;

            return true;
        }

        public override WrappedCard Validate(Room room, CardUseStruct use)
        {
            if (use.Reason == CardUseReason.CARD_USE_REASON_PLAY)
            {
                return use.Card;
            }
            else
            {
                Player player = use.From;
                LogMessage log = new LogMessage("$jijiang-slash")
                {
                    From = player.Name,
                    To = new List<string>(),
                    Card_str = RoomLogic.CardToString(room, use.Card)
                };
                log.SetTos(use.To);
                room.SendLog(log);

                foreach (Player p in use.To)
                    room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, p.Name);

                player.SetFlags(string.Format("jijiang_{0}", room.GetRoomState().GetCurrentResponseID()));
                room.BroadcastSkillInvoke("$jijiang", player, use.Card.SkillPosition);
                room.NotifySkillInvoked(player, "$jijiang");

                foreach (Player p in room.GetOtherPlayers(player))
                {
                    if (p.Kingdom == "shu")
                    {
                        WrappedCard card = room.AskForCard(player, "jijiang", "Slash", string.Format("@jijiang-target:{0},{1}", player.Name, use.To[0].Name), null, HandlingMethod.MethodUse);
                        if (card != null) return card;
                    }
                }

                return null;
            }
        }

        public override WrappedCard ValidateInResponse(Room room, Player player, WrappedCard card)
        {
            LogMessage log = new LogMessage("#UseCard")
            {
                From = player.Name,
                To = new List<string>(),
                Card_str = RoomLogic.CardToString(room, card)
            };
            room.SendLog(log);

            room.BroadcastSkillInvoke("$jijiang", player, card.SkillPosition);
            room.NotifySkillInvoked(player, "$jijiang");

            HandlingMethod method = room.GetRoomState().GetCurrentCardUseReason() == CardUseReason.CARD_USE_REASON_RESPONSE_USE ? HandlingMethod.MethodUse : HandlingMethod.MethodResponse;
            player.SetFlags(string.Format("jijiang_{0}", room.GetRoomState().GetCurrentResponseID()));
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (p.Kingdom == "shu")
                {
                    WrappedCard slash = room.AskForCard(player, "$jijiang", "Slash", "@jijiang:" + player.Name, null, method);
                    if (slash != null) return slash;
                }
            }

            return null;
        }
    }

    public class WushengJX : OneCardViewAsSkill
    {
        public WushengJX() : base("wusheng_jx")
        {
            response_or_use = true;
            skill_type = SkillType.Attack;
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return Slash.IsAvailable(room, player);
        }
        public override bool IsEnabledAtResponse(Room room, Player player, string pattern)
        {
            return pattern == "Slash";
        }
        public override bool ViewFilter(Room room, WrappedCard card, Player player)
        {
            if (!WrappedCard.IsRed(RoomLogic.GetCardSuit(room, card)))
                return false;

            if (room.GetRoomState().GetCurrentCardUseReason() == CardUseReason.CARD_USE_REASON_PLAY)
            {
                WrappedCard slash = new WrappedCard("Slash");
                slash.AddSubCard(card);
                return Slash.IsAvailable(room, player, slash);
            }
            return true;
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard slash = new WrappedCard("Slash")
            {
                Skill = Name
            };
            slash.AddSubCard(card);
            if (card.Suit == WrappedCard.CardSuit.Diamond)
                slash.DistanceLimited = false;
            slash = RoomLogic.ParseUseCard(room, slash);
            return slash;
        }
    }

    public class Yijue : TriggerSkill
    {
        public Yijue() : base("yijue")
        {
            view_as_skill = new YijueVS();
            events = new List<TriggerEvent> { TriggerEvent.DamageCaused, TriggerEvent.EventPhaseChanging };
            skill_type = SkillType.Attack;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
            {
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (p.GetMark("@yijue") > 0)
                    {
                        p.SetMark("@yijue", 0);
                        RoomLogic.RemovePlayerCardLimitation(p, "use,response", ".|.|.|hand$0");
                    }
                }
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.DamageCaused && data is DamageStruct damage && damage.To.GetMark("@yijue") > 0 &&
                player.HasFlag("yijue_" + damage.To.Name) && damage.Card != null && damage.Card.Name.Contains("Slash")
                && damage.Card.Suit == WrappedCard.CardSuit.Heart && damage.ByUser)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name, true);
            DamageStruct damage = (DamageStruct)data;
            LogMessage log = new LogMessage
            {
                Type = "$yijue",
                From = player.Name,
                To = new List<string> { damage.To.Name },
                Arg = damage.Damage.ToString(),
                Arg2 = (++damage.Damage).ToString()
            };
            room.SendLog(log);
            data = damage;

            return false;
        }
    }

    public class YijueVS : OneCardViewAsSkill
    {
        public YijueVS() : base("yijue")
        {
            filter_pattern = "..!";
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed("YijueCard");
        }

        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard yijue = new WrappedCard("YijueCard") { Skill = Name };
            yijue.AddSubCard(card);
            return yijue;
        }
    }

    public class YijueCard : SkillCard
    {
        public YijueCard() : base("YijueCard") { will_throw = true; }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select != Self && !to_select.IsKongcheng();
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            Player target = card_use.To[0];

            int id = room.AskForCardShow(target, player, Name, "yijue");
            room.ShowCard(target, id, "yijue");

            if (WrappedCard.IsBlack(room.GetCard(id).Suit))
            {
                string pattern = ".|.|.|hand$0";
                RoomLogic.SetPlayerCardLimitation(target, "use,response", pattern);
                room.SetPlayerMark(target, "@yijue", 1);
                player.SetFlags("yijue_" + target.Name);
            }
            else
            {
                room.ObtainCard(player, id);
                if (room.AskForSkillInvoke(player, "yijue", target))
                {
                    RecoverStruct recover = new RecoverStruct
                    {
                        Recover = 1,
                        Who = player
                    };
                    room.Recover(target, recover, true);
                }
            }
        }
    }

    public class YijueInvalid : InvalidSkill
    {
        public YijueInvalid() : base("#yijue-invalid") { }

        public override bool Invalid(Room room, Player player, string skill)
        {
            if (player.GetMark("@yijue") > 0 && Engine.GetSkill(skill) != null && Engine.GetSkill(skill).SkillFrequency != Frequency.Compulsory)
                return true;

            return false;
        }
    }

    public class PaoxiaoJX : TargetModSkill
    {
        public PaoxiaoJX() : base("paoxiao_jx")
        {
        }
        public override int GetResidueNum(Room room, Player from, WrappedCard card)
        {
            if (RoomLogic.PlayerHasSkill(room, from, Name) && Engine.MatchExpPattern(room, pattern, from, card))
                return 1000;
            else
                return 0;
        }
    }

    public class Tishen : TriggerSkill
    {
        public Tishen() : base("tishen")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseEnd, TriggerEvent.TargetConfirmed, TriggerEvent.CardFinished, TriggerEvent.Damaged, TriggerEvent.EventPhaseStart };
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.RoundStart && player.GetMark(Name) > 0)
            {
                player.SetMark(Name, 0);
                room.RemovePlayerStringMark(player, Name);
            }
            else if (triggerEvent == TriggerEvent.TargetConfirmed && player.GetMark(Name) > 0 && data is CardUseStruct use && use.Card != null
                && use.Card.Name.Contains("Slash"))
            {
                use.Card.SetFlags(string.Format("{0}_{1}", Name, player.Name));
            }
            else if (triggerEvent == TriggerEvent.Damaged && player.GetMark(Name) > 0
                && data is DamageStruct damage && damage.Card != null && damage.Card.HasFlag(string.Format("{0}_{1}", Name, player.Name)))
            {
                damage.Card.SetFlags(string.Format("-{0}_{1}", Name, player.Name));
            }
            else if (triggerEvent == TriggerEvent.CardFinished && data is CardUseStruct _use && _use.Card != null && _use.Card.Name.Contains("Slash"))
            {
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (p.GetMark(Name) > 0 && _use.Card.HasFlag(string.Format("{0}_{1}", Name, p.Name)))
                    {
                        List<int> ids = new List<int>();
                        foreach (int id in _use.Card.SubCards)
                            if (room.GetCardPlace(id) == Place.PlaceTable)
                                ids.Add(id);

                        room.ObtainCard(p, ids, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_RECYCLE, p.Name, Name, string.Empty));
                    }
                }
            }
        }

        public override bool Triggerable(Player target, Room room)
        {
            return base.Triggerable(target, room) && target.Phase == PlayerPhase.Play;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            return room.AskForSkillInvoke(player, Name, data, info.SkillPosition) ? info : new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            List<int> ids = new List<int>();
            foreach (int id in player.GetCards("he"))
            {
                FunctionCard fcard = Engine.GetFunctionCard(room.GetCard(id).Name);
                if (fcard is TrickCard || fcard is Horse)
                    ids.Add(id);
            }
            room.ThrowCard(ref ids, player, player);

            room.SetPlayerStringMark(player, Name, string.Empty);
            player.SetMark(Name, 1);
            return false;
        }
    }

    public class LongdanJX : OneCardViewAsSkill
    {
        public LongdanJX() : base("longdan_jx")
        {
            response_or_use = true;
        }
        public override bool ViewFilter(Room room, WrappedCard card, Player player)
        {
            switch (room.GetRoomState().GetCurrentCardUseReason())
            {
                case CardUseReason.CARD_USE_REASON_PLAY:
                    return card.Name == "Jink";
                case CardUseReason.CARD_USE_REASON_RESPONSE:
                case CardUseReason.CARD_USE_REASON_RESPONSE_USE:
                    string pattern = room.GetRoomState().GetCurrentCardUsePattern();
                    pattern = Engine.GetPattern(pattern).GetPatternString();
                    if (pattern == "Slash")
                        return card.Name == "Jink";
                    else if (pattern == "Jink")
                    {
                        FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                        return fcard is Slash;
                    }
                    break;
                default:
                    return false;
            }

            return false;
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return Slash.IsAvailable(room, player);
        }
        public override bool IsEnabledAtResponse(Room room, Player player, string pattern)
        {
            pattern = Engine.GetPattern(pattern).GetPatternString();
            return pattern == "Jink" || pattern == "Slash";
        }
        public override WrappedCard ViewAs(Room room, WrappedCard originalCard, Player player)
        {
            FunctionCard fcard = Engine.GetFunctionCard(originalCard.Name);
            if (fcard is Slash)
            {
                WrappedCard jink = new WrappedCard("Jink");
                jink.AddSubCard(originalCard);
                jink.Skill = Name;
                jink = RoomLogic.ParseUseCard(room, jink);
                return jink;
            }
            else if (fcard is Jink)
            {
                WrappedCard slash = new WrappedCard("Slash");
                slash.AddSubCard(originalCard);
                slash.Skill = Name;
                slash = RoomLogic.ParseUseCard(room, slash);
                return slash;
            }
            else
                return null;
        }
    }

    public class Yajiao : TriggerSkill
    {
        public Yajiao() : base("yajiao")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardUsed, TriggerEvent.CardResponded };
            skill_type = SkillType.Replenish;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && player.Phase == PlayerPhase.NotActive)
            {
                WrappedCard card = null;
                if (triggerEvent == TriggerEvent.CardUsed && data is CardUseStruct use && use.IsHandcard && !RoomLogic.IsVirtualCard(room, use.Card))
                    card = use.Card;
                else if (triggerEvent == TriggerEvent.CardResponded && data is CardResponseStruct response
                    && response.Handcard && !RoomLogic.IsVirtualCard(room, response.Card))
                    card = response.Card;

                if (card != null && !(Engine.GetFunctionCard(card.Name) is SkillCard)) return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(player, Name, data, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            WrappedCard card = null;
            if (triggerEvent == TriggerEvent.CardUsed && data is CardUseStruct use)
                card = use.Card;
            else if (triggerEvent == TriggerEvent.CardResponded && data is CardResponseStruct response)
                card = response.Card;

            List<int> ids = room.GetNCards(1, true);
            room.MoveCardTo(room.GetCard(ids[0]), null, Place.PlaceTable, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_TURNOVER, player.Name));

            bool discard = true;
            if ((Engine.GetFunctionCard(card.Name) is EquipCard && Engine.GetFunctionCard(room.GetCard(ids[0]).Name) is EquipCard)
                || (Engine.GetFunctionCard(card.Name) is BasicCard && Engine.GetFunctionCard(room.GetCard(ids[0]).Name) is BasicCard)
                || (Engine.GetFunctionCard(card.Name) is TrickCard && Engine.GetFunctionCard(room.GetCard(ids[0]).Name) is TrickCard))
                discard = false;

            if (room.GetCardPlace(ids[0]) == Place.PlaceTable)
            {
                Player p = room.AskForPlayerChosen(player, room.GetAlivePlayers(), Name, "@yajiao", false, false, info.SkillPosition);
                room.ObtainCard(p, ids, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_PREVIEWGIVE, player.Name, p.Name, Name, string.Empty));
                if (discard)
                    room.AskForDiscard(player, Name, 1, 1, false, true, "@yajiao-disacard", false, info.SkillPosition);
            }

            return false;
        }
    }

    public class TieqiJX : TriggerSkill
    {
        public TieqiJX() : base("tieqi_jx")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseChanging, TriggerEvent.TargetChosen };
            frequency = Frequency.Frequent;
            skill_type = SkillType.Attack;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
            {
                foreach (Player p in room.Players)
                    if (p.GetMark("@tieqi_jx") > 0)
                        p.SetMark("@tieqi_jx", 0);
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.TargetChosen && base.Triggerable(player, room) && data is CardUseStruct use && use.Card != null)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is Slash)
                    return new TriggerStruct(Name, player, use.To);
            }
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player skill_target, ref object data, Player player, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(player, Name, skill_target, info.SkillPosition))
            {
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player skill_target, ref object data, Player machao, TriggerStruct info)
        {
            CardUseStruct use = (CardUseStruct)data;
            DoTieqi(room, skill_target, machao, ref use, info);
            data = use;
            return false;
        }

        private void DoTieqi(Room room, Player target, Player source, ref CardUseStruct use, TriggerStruct info)
        {
            int index = use.To.IndexOf(target);

            JudgeStruct judge = new JudgeStruct
            {
                Reason = Name,
                Who = source
            };
            target.SetFlags("TieqiTarget"); //for AI
            room.Judge(ref judge);
            target.SetFlags("-TieqiTarget");

            Thread.Sleep(400);

            string suit = WrappedCard.GetSuitString(judge.Card.Suit);
            LogMessage l = new LogMessage
            {
                Type = "#jtieqijudge",
                From = source.Name,
                Arg = Name,
                Arg2 = WrappedCard.GetSuitString(judge.Card.Suit)
            };
            room.SendLog(l);
            target.SetTag("tieqi_judge", judge.Card.Id);

            if (room.AskForCard(target, Name,
                string.Format("..{0}", suit.Substring(0, 1).ToUpper()),
                string.Format("@tieqi-discard:{0}:{1}_char", suit, suit), judge.Card) == null)
            {
                LogMessage log = new LogMessage
                {
                    Type = "#NoJink",
                    From = target.Name
                };
                room.SendLog(log);

                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, source, Name, info.SkillPosition);
                room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);

                bool done = false;
                for (int i = 0; i < use.EffectCount.Count; i++)
                {
                    EffctCount effect = use.EffectCount[i];
                    if (effect.Index == index)
                    {
                        effect.Count = 0;
                        done = true;
                        use.EffectCount[i] = effect;
                    }
                }
                if (!done)
                {
                    EffctCount effect = new EffctCount(source, target, 0)
                    {
                        Index = index
                    };
                    use.EffectCount.Add(effect);
                }
                Thread.Sleep(500);
            }
            else
            {
                LogMessage log = new LogMessage
                {
                    Type = "#jtieqidis",
                    From = target.Name,
                    Arg = Name,
                    Arg2 = suit
                };
                room.SendLog(log);
            }
        }
    }

    public class TieqiJXInvalid : InvalidSkill
    {
        public TieqiJXInvalid() : base("#tieqi_jx-invalid")
        {
        }

        public override bool Invalid(Room room, Player player, string skill)
        {
            Skill s = Engine.GetSkill(skill);
            return s != null && s.SkillFrequency != Frequency.Compulsory && player.GetMark("@tieqi_jx") > 0;     
        }
    }


    public class JizhiJX : TriggerSkill
    {
        public JizhiJX() : base("jizhi_jx")
        {
            frequency = Frequency.Frequent;
            events= new List<TriggerEvent> { TriggerEvent.CardUsed, TriggerEvent.EventPhaseChanging };
            skill_type = SkillType.Replenish;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive && player.GetMark(Name) > 0)
            {
                room.RemovePlayerStringMark(player, Name);
                player.SetMark(Name, 0);
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.CardUsed && base.Triggerable(player, room) && data is CardUseStruct use && use.Card != null)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard.IsNDTrick())
                {
                    if (!RoomLogic.IsVirtualCard(room, use.Card) || use.Card.SubCards.Count == 0)
                        return new TriggerStruct(Name, player);
                }
            }
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(player, Name, data, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<int> card_ids = room.GetNCards(1, true);
            CardsMoveStruct move = new CardsMoveStruct(card_ids, player, Place.PlaceHand,
                new CardMoveReason(CardMoveReason.MoveReason.S_REASON_DRAW, player.Name, player.Name, Name, string.Empty));
            card_ids = room.MoveCardsAtomic(new List<CardsMoveStruct> { move }, false);
            if (player.Phase == PlayerPhase.Play && card_ids.Count == 1 && room.GetCardPlace(card_ids[0]) == Place.PlaceHand && room.GetCardOwner(card_ids[0]) == player)
            {
                List<int> ids = room.AskForExchange(player, Name, 1, 0, "@jizhi", string.Empty, card_ids[0].ToString(), info.SkillPosition);
                if (ids.Count > 0)
                {
                    room.ThrowCard(ref ids, player, player, Name);
                    player.AddMark(Name, ids.Count);
                    room.SetPlayerStringMark(player, Name, player.GetMark(Name).ToString());
                }
            }

            return false;
        }
    }

    public class JizhiMax : MaxCardsSkill
    {
        public JizhiMax() : base("#jizhi-max") { }

        public override int GetExtra(Room room, Player target)
        {
            return target.GetMark("jizhi_jx");
        }
    }

    public class QicaiJX : TargetModSkill
    {
        public QicaiJX() : base("qicai_jx")
        {
            pattern = "TrickCard";
            skill_type = SkillType.Wizzard;
        }
        public override bool GetDistanceLimit(Room room, Player from, Player to, WrappedCard card)
        {
            if (!Engine.MatchExpPattern(room, pattern, from, card))
                return false;

            if (RoomLogic.PlayerHasSkill(room, from, Name))
                return true;
            else
                return false;
        }
    }

    public class QicaiFix : FixCardSkill
    {
        public QicaiFix() : base("#qicai-fix") { }

        public override bool IsCardFixed(Room room, Player from, Player to, string flags, FunctionCard.HandlingMethod method)
        {
            if (to != null && from != null && from != to && (flags == "t" || flags == "d" || flags == "o")
                && method == FunctionCard.HandlingMethod.MethodDiscard && RoomLogic.PlayerHasSkill(room, to, "qicai_jx"))
                return true;

            return false;
        }
    }

    public class GuanxingJX : PhaseChangeSkill
    {
        public GuanxingJX() : base("guanxing_jx")
        {
            frequency = Frequency.Frequent;
            skill_type = SkillType.Wizzard;
        }
        public override bool CanPreShow() => false;
        public override bool Triggerable(Player target, Room room)
        {
            return base.Triggerable(target, room) && (target.Phase == PlayerPhase.Finish && target.HasFlag(Name) || target.Phase == PlayerPhase.Start);
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(player, Name, null, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }
                return new TriggerStruct();
        }
        public override bool OnPhaseChange(Room room, Player zhuge, TriggerStruct info)
        {
            List<int> guanxing = room.GetNCards(GetGuanxingNum(room, zhuge), false);
            LogMessage log = new LogMessage
            {
                Type = "$ViewDrawPile",
                From = zhuge.Name,
                Card_str = string.Join("+", JsonUntity.IntList2StringList(guanxing))
            };
            room.SendLog(log, zhuge);
            log.Type = "$ViewDrawPile2";
            log.Arg = guanxing.Count.ToString();
            log.Card_str = null;
            room.SendLog(log, new List<Player> { zhuge });
            
            AskForMoveCardsStruct result = room.AskForMoveCards(zhuge, guanxing, new List<int>(), true, Name, 0, guanxing.Count, false, true, new List<int>(), info.SkillPosition);
            List<int> top_cards = result.Top, bottom_cards = result.Bottom;
            log = new LogMessage
            {
                Type = "#GuanxingResult",
                From = zhuge.Name,
                Arg = top_cards.Count.ToString(),
                Arg2 = bottom_cards.Count.ToString()
            };
            room.SendLog(log);

            if (top_cards.Count > 0)
            {
                LogMessage log1 = new LogMessage
                {
                    Type = "$GuanxingTop",
                    From = zhuge.Name,
                    Card_str = string.Join("+", JsonUntity.IntList2StringList(top_cards))
                };
                room.SendLog(log1, zhuge);
            }
            if (bottom_cards.Count > 0)
            {
                LogMessage log1 = new LogMessage
                {
                    Type = "$GuanxingBottom",
                    From = zhuge.Name,
                    Card_str = string.Join("+", JsonUntity.IntList2StringList(bottom_cards))
                };
                room.SendLog(log, zhuge);
            }

            room.ReturnToDrawPile(top_cards, false, zhuge);
            room.ReturnToDrawPile(bottom_cards, true, zhuge);

            if (top_cards.Count == 0)
                zhuge.SetFlags(Name);

            return false;
        }
        private int GetGuanxingNum(Room room, Player zhuge)
        {
            return Math.Min(5, room.AliveCount());
        }
    }

    public class KongchengJX : TriggerSkill
    {
        public KongchengJX() : base("kongcheng_jx")
        {
            events.Add(TriggerEvent.CardsMoveOneTime);
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Defense;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is CardsMoveOneTimeStruct move && move.From != null && move.From_places.Contains(Place.PlaceHand) && RoomLogic.PlayerHasSkill(room, move.From, Name)
                && move.From.IsKongcheng())
                return new TriggerStruct(Name, move.From);

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
            return false;
        }
    }

    public class KongchengJXP : ProhibitSkill
    {
        public KongchengJXP() : base("#kongcheng_jx-prohibit") { }

        public override bool IsProhibited(Room room, Player from, Player to, WrappedCard card, List<Player> others = null)
        {
            if (to != null && card != null && (card.Name == "Duel" || card.Name.Contains("Slash")) && RoomLogic.PlayerHasSkill(room, to, Name))
                return true;

            return false;
        }
    }

    public class BiyueJX : PhaseChangeSkill
    {
        public BiyueJX() : base("biyue_jx")
        {
            frequency = Frequency.Frequent;
            skill_type = SkillType.Replenish;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (!base.Triggerable(player, room)) return new TriggerStruct();
            if (player.Phase == PlayerPhase.Finish) return new TriggerStruct(Name, player);
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(player, Name, null, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool OnPhaseChange(Room room, Player player, TriggerStruct info)
        {
            room.DrawCards(player, player.IsKongcheng() ? 2 : 1, Name);
            return false;
        }
    }

    public class Liyu : TriggerSkill
    {
        public Liyu() : base("liyu")
        {
            events.Add(TriggerEvent.Damage);
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is DamageStruct damage && damage.To.Alive && damage.Card != null && damage.Card.Name.Contains("Slash") && RoomLogic.PlayerHasSkill(room, player, Name)
                && !damage.To.IsAllNude() && !damage.Transfer && damage.ByUser && !damage.Chain && RoomLogic.CanGetCard(room, player, damage.To, "hej"))
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is DamageStruct damage && room.AskForSkillInvoke(player, Name, damage.To, info.SkillPosition))
                return info;

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is DamageStruct damage)
            {
                int id = room.AskForCardChosen(player, damage.To, "hej", Name, false, FunctionCard.HandlingMethod.MethodGet);
                Debug.Assert(id > -1, "liyu get card error");
                room.ObtainCard(player, id);
                bool duel = Engine.GetFunctionCard(room.GetCard(id).Name) is EquipCard;
                if (duel)
                {
                    WrappedCard Duel = new WrappedCard("Duel") { Skill = "_liyu" };
                    List<Player> targets = new List<Player>();
                    foreach (Player p in room.GetOtherPlayers(damage.To))
                    {
                        if (p == player || p == damage.To) continue;
                        if (RoomLogic.IsProhibited(room, player, p, Duel) == null)
                            targets.Add(p);
                    }

                    if (targets.Count > 0)
                    {
                        Player victim = room.AskForPlayerChosen(damage.To, targets, Name, "@liyu-duel:" + player.Name);
                        if (victim != null)
                            room.UseCard(new CardUseStruct(Duel, player, victim));
                    }
                }
                else
                    room.DrawCards(damage.To, 1, Name);
            }

            return false;
        }
    }

    public class JianxiongJX : MasochismSkill
    {
        public JianxiongJX() : base("jianxiong_jx")
        {
            frequency = Frequency.Frequent;
            skill_type = SkillType.Replenish;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(player, Name, data, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }
        public override void OnDamaged(Room room, Player target, DamageStruct damage, TriggerStruct info)
        {
            if (damage.Card != null)
            {
                WrappedCard card = damage.Card;
                List<int> table_cardids = room.GetCardIdsOnTable(card);
                List<int> ids = card.SubCards;

                if (table_cardids.Count != 0 && ids.SequenceEqual(table_cardids))
                    room.ObtainCard(target, damage.Card);
            }
            room.DrawCards(target, 1, Name);
        }
    }

    public class Hujia : ZeroCardViewAsSkill
    {
        public Hujia() : base("$hujia") { }

        public override bool IsEnabledAtResponse(Room room, Player player, string pattern)
        {
            if (pattern != "Jink" || player.HasFlag(string.Format("hujia_{0}", room.GetRoomState().GetCurrentResponseID())))
                return false;

            foreach (Player p in room.GetOtherPlayers(player))
                if (p.Kingdom == "wei") return true;

            return false;
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard("HujiaCard") { Skill = Name };
        }
    }

    public class HujiaCard : SkillCard
    {
        public HujiaCard() : base("HujiaCard")
        {
        }
        public override WrappedCard ValidateInResponse(Room room, Player player, WrappedCard card)
        {
            LogMessage log = new LogMessage("#UseCard")
            {
                From = player.Name,
                To = new List<string>(),
                Card_str = RoomLogic.CardToString(room, card)
            };
            room.SendLog(log);

            room.BroadcastSkillInvoke("$hujia", player, card.SkillPosition);
            room.NotifySkillInvoked(player, "$hujia");

            HandlingMethod method = room.GetRoomState().GetCurrentCardUseReason() == CardUseReason.CARD_USE_REASON_RESPONSE_USE ? HandlingMethod.MethodUse : HandlingMethod.MethodResponse;
            player.SetFlags(string.Format("hujia_{0}", room.GetRoomState().GetCurrentResponseID()));
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (p.Kingdom == "wei")
                {
                    WrappedCard slash = room.AskForCard(player, "$hujia", "Jink", "@hujia:" + player.Name, null,
                        room.GetRoomState().GetCurrentCardUseReason() == CardUseReason.CARD_USE_REASON_RESPONSE ? HandlingMethod.MethodResponse : HandlingMethod.MethodUse);
                    if (slash != null) return slash;
                }
            }

            return null;
        }
    }

    class FankuiJX : MasochismSkill
    {
        public FankuiJX() : base("fankui_jx")
        {
            skill_type = SkillType.Wizzard;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is DamageStruct damage)
            {
                Player from = damage.From;
                if (from != null && RoomLogic.CanGetCard(room, player, from, "he"))
                {
                    TriggerStruct trigger = new TriggerStruct(Name, player)
                    {
                        Times = damage.Damage
                    };
                    return trigger;
                }
            }

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player simayi, ref object data, Player ask_who, TriggerStruct info)
        {
            DamageStruct damage = (DamageStruct)data;
            if (!damage.From.IsNude() && room.AskForSkillInvoke(simayi, Name, data, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, simayi, info.SkillPosition);
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, simayi.Name, damage.From.Name);
                return info;
            }
            return new TriggerStruct();
        }
        public override void OnDamaged(Room room, Player simayi, DamageStruct damage, TriggerStruct info)
        {
            int card_id = room.AskForCardChosen(simayi, damage.From, "he", Name, false, FunctionCard.HandlingMethod.MethodGet);
            CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_EXTRACTION, simayi.Name);
            room.ObtainCard(simayi, room.GetCard(card_id), reason, false);
        }

    }

    public class GuicaiJX : TriggerSkill
    {
        public GuicaiJX() : base("guicai_jx")
        {
            events.Add(TriggerEvent.AskForRetrial);
            skill_type = SkillType.Wizzard;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (!base.Triggerable(player, room)) return new TriggerStruct();
            if (player.IsNude() && player.GetHandPile().Count == 0)
                return new TriggerStruct();
            return new TriggerStruct(Name, player);
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            JudgeStruct judge = (JudgeStruct)data;

            List<string> prompt_list = new List<string> { "@guicai_jx-card", judge.Who.Name, string.Empty, Name, judge.Reason };
            string prompt = string.Join(":", prompt_list);

            room.SetTag(Name, data);
            WrappedCard card = room.AskForCard(player, Name, "..", prompt, data, HandlingMethod.MethodResponse, judge.Who, true);
            room.RemoveTag(Name);
            if (card != null)
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                room.Retrial(card, player, ref judge, Name, false, info.SkillPosition);
                data = judge;
                return info;
            }


            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            JudgeStruct judge = (JudgeStruct)data;
            room.UpdateJudgeResult(ref judge);
            data = judge;
            return false;
        }
    }

    class GanglieJX : MasochismSkill
    {
        public GanglieJX() : base("ganglie_jx")
        {
            skill_type = SkillType.Defense;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(player, Name, data, info.SkillPosition) && data is DamageStruct damage)
            {
                if (damage.From != null)
                    room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, damage.From.Name);
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }
        public override void OnDamaged(Room room, Player xiahou, DamageStruct damage, TriggerStruct info)
        {
            Player from = damage.From;

            JudgeStruct judge = new JudgeStruct
            {
                Negative = false,
                Reason = Name,
                PlayAnimation = true,
                Who = xiahou
            };

            room.Judge(ref judge);
            if (from != null && from.Alive)
            {
                bool red = WrappedCard.IsRed(judge.Card.Suit);
                if (red)
                    room.Damage(new DamageStruct(Name, xiahou, from));
                else if (!from.IsNude())
                {
                    int id = room.AskForCardChosen(xiahou, from, "he", Name, false, HandlingMethod.MethodDiscard);
                    room.ThrowCard(id, from, xiahou);
                }
            }
        }
    }

    public class Qingjian : TriggerSkill
    {
        public Qingjian() : base("qingjian")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardsMoveOneTime, TriggerEvent.EventPhaseChanging };
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
            {
                if (player.GetMark(Name) > 0)
                {
                    player.SetMark(Name, 0);
                    room.RemovePlayerStringMark(player, Name);
                }

                foreach (Player p in room.GetAlivePlayers())
                    player.SetFlags("-qingjian");
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.To != null
                && base.Triggerable(move.To, room) && move.To.Phase != PlayerPhase.Draw && move.To_place == Place.PlaceHand && !move.To.HasFlag(Name))
            {
                return new TriggerStruct(Name, move.To);
            }
            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardsMoveOneTimeStruct move && move.Card_ids.Count > 0)
            {
                foreach (int id in move.Card_ids)
                {
                    if (room.GetCardPlace(id) == Place.PlaceHand && room.GetCardOwner(id) == ask_who)
                        room.SetCardFlag(id, Name);
                }

                WrappedCard card = room.AskForUseCard(ask_who, "@@qingjian", "@qingjian", -1, HandlingMethod.MethodUse, true, info.SkillPosition);
                foreach (int id in move.Card_ids)
                {
                    room.SetCardFlag(id, "-qingjian");
                }
                if (card != null && room.ContainsTag(Name))
                    return info;
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            ask_who.SetFlags(Name);
            CardUseStruct use = (CardUseStruct)room.GetTag(Name);
            room.RemoveTag(Name);
            List<int> ids = new List<int>(use.Card.SubCards);
            bool basic = false;
            bool equip = false;
            bool trick = false;
            foreach (int id in ids)
            {
                FunctionCard card = Engine.GetFunctionCard(room.GetCard(id).Name);
                if (card is BasicCard) basic = true;
                if (card is EquipCard) equip = true;
                if (card is TrickCard) trick = true;
            }
            room.ObtainCard(use.To[0], ids, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_GIVE, ask_who.Name, use.To[0].Name, Name, string.Empty));
            if (room.Current != null)
            {
                if (basic) room.Current.AddMark(Name, 1);
                if (equip) room.Current.AddMark(Name, 1);
                if (trick) room.Current.AddMark(Name, 1);
                room.SetPlayerStringMark(room.Current, Name, room.Current.GetMark(Name).ToString());
            }

            return false;
        }
    }

    public class QingjianVS : ViewAsSkill
    {
        public QingjianVS() : base("qingjian") { response_pattern = "@@qingjian"; }
        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return to_select.HasFlag(Name);
        }

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count > 0)
            {
                WrappedCard card = new WrappedCard("QingjianCard");
                card.AddSubCards(cards);
                return card;
            }

            return null;
        }
    }

    public class QingjianCard : SkillCard
    {
        public QingjianCard() : base("QingjianCard")
        {
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select != Self;
        }

        public override void OnUse(Room room, CardUseStruct card_use)
        {
            room.NotifySkillInvoked(card_use.From, "qingjian");
            room.BroadcastSkillInvoke("qingjian", card_use.From, card_use.Card.SkillPosition);
            room.ShowCards(card_use.From, new List<int>(card_use.Card.SubCards), "qingjian");
            room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, card_use.From.Name, card_use.To[0].Name);
            room.SetTag("qingjian", card_use);
        }
    }



    public class QingjianMax : MaxCardsSkill
    {
        public QingjianMax() : base("#qingjian-max") { }
        public override int GetExtra(Room room, Player target)
        {
            return target.GetMark("qingjian");
        }
    }

    public class TuxiJx : DrawCardsSkill
    {
        public TuxiJx() : base("tuxi_jx")
        {
            skill_type = SkillType.Attack;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetOtherPlayers(player))
                if (RoomLogic.CanGetCard(room, player, p, "h") && !p.IsKongcheng())
                    targets.Add(p);

            if (data is int count && count > 0 && targets.Count > 0)
            {
                List<Player> victims = room.AskForPlayersChosen(player, targets, Name, 0, count, "@tuxi:::" + count.ToString(), true, info.SkillPosition);
                if (victims.Count > 0)
                {
                    room.SetTag(Name, victims);
                    room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                    count -= victims.Count;
                    data = count;
                    return info;
                }
            }
            return new TriggerStruct();
        }
        public override int GetDrawNum(Room room, Player player, int n)
        {
            List<Player> victims = (List<Player>)room.GetTag(Name);
            List<int> ids = new List<int>();
            foreach (Player p in victims)
                ids.Add(room.AskForCardChosen(player, p, "h", Name, false, HandlingMethod.MethodGet));

            room.ObtainCard(player, ids, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_EXTRACTION, player.Name, Name, string.Empty), false);
            return n;
        }
    }

    public class LuoyiJX : TriggerSkill
    {
        public LuoyiJX() : base("luoyi_jx")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart, TriggerEvent.PreCardUsed };
            skill_type = SkillType.Attack;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.PreCardUsed && player != null && player.Alive && player.GetMark("@luoyi") > 0 && data is CardUseStruct use)
            {
                if (use.Card != null)
                {
                    FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                    if (fcard is Slash || fcard is Duel)
                        room.SetCardFlag(use.Card, Name);
                }
            }
            else if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.RoundStart && player.GetMark("@luoyi") > 0)
                room.SetPlayerMark(player, "@luoyi", 0);
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && base.Triggerable(player, room) && player.Phase == PlayerPhase.Draw)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(player, Name, data, info.SkillPosition))
            {
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);

                List<int> ids = room.GetNCards(3);
                bool check = false;
                foreach (int id in ids)
                {
                    WrappedCard card = room.GetCard(id);
                    FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                    room.MoveCardTo(card, null, Place.PlaceTable, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_TURNOVER, player.Name));
                    if (fcard is BasicCard || fcard is Weapon || fcard is Duel)
                        check = true;
                }
                if (check && room.AskForSkillInvoke(player, Name, "@luoyi-get", info.SkillPosition))
                {
                    room.ObtainCard(player, ids, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_RECYCLE, player.Name, Name, string.Empty));
                    return info;
                }
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SetPlayerMark(player, "@luoyi", 1);
            return true;
        }
    }

    class LuoyiDamageJX : TriggerSkill
    {
        public LuoyiDamageJX() : base("luoyi_jx-damage")
        {
            events.Add(TriggerEvent.DamageCaused);
            frequency = Frequency.Compulsory;
            global = true;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player != null && player.Alive && player.GetMark("@luoyi") > 0 && data is DamageStruct damage)
            {
                if (damage.Card != null && damage.Card.HasFlag("luoyi_jx") && !damage.Chain && !damage.Transfer && damage.ByUser)
                    return new TriggerStruct(Name, player);
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
            room.BroadcastSkillInvoke("luoyi_jx", "male", 1, gsk.General, gsk.SkinId);
            DamageStruct damage = (DamageStruct)data;
            LogMessage log = new LogMessage
            {
                Type = "#LuoyiBuff",
                From = player.Name,
                To = new List<string> { damage.To.Name },
                Arg = damage.Damage.ToString(),
                Arg2 = (++damage.Damage).ToString()
            };
            room.SendLog(log);

            data = damage;

            return false;
        }
    }
}