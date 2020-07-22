using CommonClass;
using CommonClass.Game;
using CommonClassLibrary;
using SanguoshaServer.Game;
using System;
using System.Collections.Generic;
using System.Configuration;
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
                new WushengTar(),
                new Yijue(),
                new YijueInvalid(),
                new PaoxiaoJX(),
                new PaoxiaoJXTar(),
                //new PaoxiaoRecord(),
                new Tishen(),
                //new TishenGet(),
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
                new Jijie(),
                new Jiyuan(),

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
                new YijiJX(),
                new LuoshenJX(),
                new LuoshenMax(),

                new ZhihengJX(),
                new Jiuyuan(),
                new Fenwei(),
                new KejiJX(),
                new Qinxue(),
                new Botu(),
                new BotuClear(),
                new Gongxin(),
                new KurouJX(),
                new Zhaxiang(),
                new ZhaxiangTM(),
                new GuoseJX(),
                new JieyinJX(),
                new QianxunJX(),
                new QianxunClear(),
                new Lianying(),
            };
            skill_cards = new List<FunctionCard>
            {
                new JijiangCard(),
                new YijueCard(),
                new HujiaCard(),
                new QingjianCard(),
                new YijiJCard(),
                new ZhihengJCard(),
                new KurouJCard(),
                new GongxinCard(),
                new GuoseCard(),
                new JieyinJCard(),
                new JijieCard(),
            };
            
            related_skills = new Dictionary<string, List<string>>
            {
                { "wusheng_jx", new List<string> { "#wusheng-target" } },
                //{ "paoxiao_jx", new List<string> { "#paoxiao-record" } },
                { "paoxiao_jx", new List<string> { "#paoxiao_jx-tar" } },
                { "kongcheng_jx", new List<string> { "#kongcheng_jx-prohibit" } },
                //{ "tishen", new List<string> { "#tishen-get" } },
                { "zhaxiang", new List<string> { "#zhaxiang-tm" } },
                { "qianxun_jx", new List<string> { "#qianxun_jx-clear" } },
                { "botu", new List<string> { "#botu" } },
            };
        }
    }

    public class Jijiang : ZeroCardViewAsSkill
    {
        public Jijiang() : base("jijiang") { lord_skill = true; }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            if (!Slash.IsAvailable(room, player) || player.HasFlag(string.Format("jijiang_activate_{0}", room.GetRoomState().GlobalActivateID)))
                return false;

            foreach (Player p in room.GetOtherPlayers(player))
                if (p.Kingdom == "shu") return true;

            return false;
        }

        public override bool IsEnabledAtResponse(Room room, Player player, string pattern)
        {
            if (pattern != Slash.ClassName || player.HasFlag(string.Format("jijiang_{0}", room.GetRoomState().GetCurrentResponseID())))
                return false;

            foreach (Player p in room.GetOtherPlayers(player))
                if (p.Kingdom == "shu") return true;

            return false;
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard(JijiangCard.ClassName) { Skill = Name, Mute = true };
        }
    }

    public class JijiangCard : SkillCard
    {
        public static string ClassName = "JijiangCard";
        public JijiangCard() : base(ClassName)
        {
        }
        
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            if (room.GetRoomState().GetCurrentCardUseReason() == CardUseReason.CARD_USE_REASON_PLAY || room.GetRoomState().GetCurrentCardUseReason() == CardUseReason.CARD_USE_REASON_RESPONSE_USE)
            {
                WrappedCard slash = new WrappedCard(Slash.ClassName);
                FunctionCard fcard = Slash.Instance;
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
            Player player = use.From;
            if (use.Reason == CardUseReason.CARD_USE_REASON_PLAY)
                player.SetFlags(string.Format("jijiang_activate_{0}", room.GetRoomState().GlobalActivateID));
            else
                player.SetFlags(string.Format("jijiang_{0}", room.GetRoomState().GetCurrentResponseID()));

            LogMessage log = new LogMessage("$jijiang-slash")
            {
                From = player.Name,
                To = new List<string>(),
                Card_str = RoomLogic.CardToString(room, use.Card)
            };
            log.SetTos(use.To);
            room.SendLog(log);

            List<string> targets = new List<string>();
            foreach (Player p in use.To)
            {
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, p.Name);
                targets.Add(p.Name);
            }

            room.BroadcastSkillInvoke("jijiang", player, use.Card.SkillPosition);
            room.NotifySkillInvoked(player, "jijiang");
            
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (p.Kingdom == "shu")
                {
                    CardEffectStruct effect = new CardEffectStruct
                    {
                        Card = use.Card,
                        From = player,
                        To = p
                    };
                    WrappedCard card = room.AskForCard(p, "jijiang", Slash.ClassName, string.Format("@jijiang-target:{0}:{1}", player.Name, string.Join("+", targets)),
                        effect, HandlingMethod.MethodResponse);
                    if (card != null)
                    {
                        ResultStruct result = p.Result;
                        result.Assist++;
                        p.Result = result;

                        Thread.Sleep(500);
                        return card;
                    }
                }
            }

            return null;
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

            room.BroadcastSkillInvoke("jijiang", player, card.SkillPosition);
            room.NotifySkillInvoked(player, "jijiang");

            HandlingMethod method = room.GetRoomState().GetCurrentCardUseReason() == CardUseReason.CARD_USE_REASON_RESPONSE_USE ? HandlingMethod.MethodUse : HandlingMethod.MethodResponse;
            player.SetFlags(string.Format("jijiang_{0}", room.GetRoomState().GetCurrentResponseID()));
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (p.Kingdom == "shu")
                {
                    CardEffectStruct effect = new CardEffectStruct
                    {
                        Card = card,
                        From = player,
                        To = p
                    };
                    WrappedCard slash = room.AskForCard(p, "jijiang", Slash.ClassName, "@jijiang:" + player.Name, effect, method);
                    if (slash != null)
                    {
                        ResultStruct result = p.Result;
                        result.Assist++;
                        p.Result = result;

                        Thread.Sleep(500);
                        return slash;
                    }
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
            skill_type = SkillType.Alter;
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return Slash.IsAvailable(room, player);
        }
        public override bool IsEnabledAtResponse(Room room, Player player, string pattern)
        {
            return Engine.GetPattern(pattern).GetPatternString() == Slash.ClassName;
        }
        public override bool ViewFilter(Room room, WrappedCard card, Player player)
        {
            if (!WrappedCard.IsRed(RoomLogic.GetCardSuit(room, card)))
                return false;

            if (room.GetRoomState().GetCurrentCardUseReason() == CardUseReason.CARD_USE_REASON_PLAY)
            {
                WrappedCard slash = new WrappedCard(Slash.ClassName);
                slash.AddSubCard(card);
                return Slash.IsAvailable(room, player, slash);
            }
            return true;
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard slash = new WrappedCard(Slash.ClassName)
            {
                Skill = Name
            };
            slash.AddSubCard(card);
            slash = RoomLogic.ParseUseCard(room, slash);
            return slash;
        }
    }

    public class WushengTar : TargetModSkill
    {
        public WushengTar() : base("#wusheng-target") { }
        public override bool GetDistanceLimit(Room room, Player from, Player to, WrappedCard card, CardUseReason reason, string pattern)
        {
            if (RoomLogic.PlayerHasSkill(room, from, "wusheng_jx") && RoomLogic.GetCardSuit(room, card) == WrappedCard.CardSuit.Diamond)
                return true;

            return false;
        }
        public override void GetEffectIndex(Room room, Player player, WrappedCard card, ModType type, ref int index, ref string skill_name, ref string general_name, ref int skin_id)
        {
            index = -2;
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
                        room.SetPlayerMark(p, "@yijue", 0);
                        RoomLogic.RemovePlayerCardLimitation(p, Name);
                    }
                }
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.DamageCaused && data is DamageStruct damage && damage.To.GetMark("@yijue") > 0 &&
                player.HasFlag("yijue_" + damage.To.Name) && damage.Card != null && damage.Card.Name.Contains(Slash.ClassName)
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
        public static string ClassName = "YijueCard";
        public YijueCard() : base(ClassName) { will_throw = true; }
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
                RoomLogic.SetPlayerCardLimitation(target, "yijue", "use,response", pattern);
                room.SetPlayerMark(target, "@yijue", 1);
                player.SetFlags("yijue_" + target.Name);
            }
            else
            {
                room.ObtainCard(player, id);
                target.SetFlags("yijue");
                bool invoke = room.AskForSkillInvoke(player, "yijue", target);
                target.SetFlags("-yijue");
                if (invoke)
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
            Skill s = Engine.GetSkill(skill);
            if (s == null || s.Attached_lord_skill) return false;
            if (player.HasEquip(skill)) return false;
            if (player.GetMark("@yijue") > 0 && s.SkillFrequency != Frequency.Compulsory && s.SkillFrequency != Frequency.Wake)
                return true;

            return false;
        }
    }
    /*
    public class PaoxiaoJX : TargetModSkill
    {
        public PaoxiaoJX() : base("paoxiao_jx")
        {
            skill_type = SkillType.Attack;
        }
        public override int GetResidueNum(Room room, Player from, WrappedCard card)
        {
            if (RoomLogic.PlayerHasSkill(room, from, Name) && Engine.MatchExpPattern(room, pattern, from, card))
                return 1000;
            else
                return 0;
        }
        public override bool GetDistanceLimit(Room room, Player from, Player to, WrappedCard card, CardUseReason reason, string pattern)
        {
            return from.HasFlag(Name);
        }
        public override void GetEffectIndex(Room room, Player player, WrappedCard card, ModType type, ref int index, ref string skill_name, ref string general_name, ref int skin_id)
        {
            if (type == ModType.DistanceLimit)
                index = -2;
        }
    }

    public class PaoxiaoRecord : TriggerSkill
    {
        public PaoxiaoRecord() : base("#paoxiao-record")
        {
            events.Add(TriggerEvent.CardUsedAnnounced);
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (data is CardUseStruct use && use.Card != null && use.Card.Name.Contains(Slash.ClassName)
                && RoomLogic.PlayerHasSkill(room, player, "paoxiao_jx") && player.Phase == PlayerPhase.Play)
                player.SetFlags("paoxiao_jx");
        }
    }

    public class Tishen : TriggerSkill
    {
        public Tishen() : base("tishen")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseEnd, TriggerEvent.TargetConfirmed, TriggerEvent.Damaged, TriggerEvent.EventPhaseStart };
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.RoundStart && player.GetMark(Name) > 0)
            {
                player.SetMark(Name, 0);
                room.RemovePlayerStringMark(player, Name);
            }
            else if (triggerEvent == TriggerEvent.TargetConfirmed && player.GetMark(Name) > 0 && data is CardUseStruct use && use.Card != null
                && use.Card.Name.Contains(Slash.ClassName) && use.Card.SubCards.Count > 0)
            {
                use.Card.SetFlags(string.Format("{0}_{1}", Name, player.Name));
            }
            else if (triggerEvent == TriggerEvent.Damaged && player.GetMark(Name) > 0
                && data is DamageStruct damage && damage.Card != null && damage.Card.HasFlag(string.Format("{0}_{1}", Name, player.Name)))
            {
                damage.Card.SetFlags(string.Format("-{0}_{1}", Name, player.Name));
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseEnd && base.Triggerable(player, room) && player.Phase == PlayerPhase.Play)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
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
            room.ThrowCard(ref ids, player);

            room.SetPlayerStringMark(player, Name, string.Empty);
            player.SetMark(Name, 1);
            return false;
        }
    }
    
    public class TishenGet : TriggerSkill
    {
        public TishenGet() : base("#tishen-get")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardFinished };
            frequency = Frequency.Compulsory;
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (data is CardUseStruct _use && _use.Card != null && _use.Card.Name.Contains(Slash.ClassName))
            {
                List<int> ids = new List<int>(room.GetSubCards(_use.Card));
                if (ids.Count > 0 && ids.SequenceEqual(_use.Card.SubCards))
                {
                    bool check = true;
                    foreach (int id in _use.Card.SubCards)
                    {
                        if (room.GetCardPlace(id) != Place.DiscardPile)
                        {
                            check = false;
                            break;
                        }
                    }

                    if (check)
                    {
                        foreach (Player p in room.GetAlivePlayers())
                        {
                            if (p.GetMark("tishen") > 0 && _use.Card.HasFlag(string.Format("{0}_{1}", "tishen", p.Name)))
                                triggers.Add(new TriggerStruct(Name, p));
                        }
                    }
                }
            }

            return triggers;
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct _use)
            {
                List<int> ids = new List<int>(room.GetSubCards(_use.Card));
                if (ids.Count > 0 && ids.SequenceEqual(_use.Card.SubCards))
                {
                    bool check = true;
                    foreach (int id in _use.Card.SubCards)
                    {
                        if (room.GetCardPlace(id) != Place.DiscardPile)
                        {
                            check = false;
                            break;
                        }
                    }

                    if (check)
                    {
                        room.RemoveSubCards(_use.Card);
                        room.ObtainCard(ask_who, ref ids, new CardMoveReason(MoveReason.S_REASON_RECYCLE, ask_who.Name, "tishen", string.Empty));
                    }
                }
            }

            return false;
        }
    }
    */

    public class PaoxiaoJXTar : TargetModSkill
    {
        public PaoxiaoJXTar() : base("#paoxiao_jx-tar")
        {
        }
        public override int GetResidueNum(Room room, Player from, WrappedCard card)
        {
            if (RoomLogic.PlayerHasSkill(room, from, Name) && Engine.MatchExpPattern(room, pattern, from, card))
                return 1000;
            else
                return 0;
        }

        public override void GetEffectIndex(Room room, Player player, WrappedCard card, ref int index, ref string skill_name, ref string general_name, ref int skin_id)
        {
            index = 1;
        }
    }

    public class PaoxiaoJX : TriggerSkill
    {
        public PaoxiaoJX() : base("paoxiao_jx")
        {
            events = new List<TriggerEvent> { TriggerEvent.SlashMissed, TriggerEvent.DamageCaused, TriggerEvent.EventPhaseChanging };
            skill_type = SkillType.Attack;
            frequency = Frequency.Compulsory;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.SlashMissed && base.Triggerable(player, room) && room.Current == player)
            {
                player.AddMark(Name);
                room.SetPlayerStringMark(player, Name, player.GetMark(Name).ToString());
            }
            else if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive && player.GetMark(Name) > 0)
            {
                player.SetMark(Name, 0);
                room.RemovePlayerStringMark(player, Name);
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.DamageCaused && data is DamageStruct damage && damage.Card != null && damage.Card.Name.Contains(Slash.ClassName)
                && player.Alive && player.GetMark(Name) > 0)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
            room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
            room.SendCompulsoryTriggerLog(player, Name, true);

            DamageStruct damage = (DamageStruct)data;
            damage.Damage += player.GetMark(Name);
            player.SetMark(Name, 0);
            room.RemovePlayerStringMark(player, Name);
            data = damage;

            LogMessage log = new LogMessage
            {
                Type = "#AddDamage",
                From = player.Name,
                To = new List<string> { damage.To.Name },
                Arg = Name,
                Arg2 = (damage.Damage).ToString()
            };
            room.SendLog(log);

            return false;
        }
    }

    public class Tishen : TriggerSkill
    {
        public Tishen() : base("tishen")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart };
            limit_mark = "@tishen";
            frequency = Frequency.Limited;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.RoundStart && base.Triggerable(player, room)
                && player.GetMark(limit_mark) > 0 && player.IsWounded())
            {
                return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            return room.AskForSkillInvoke(player, Name, data, info.SkillPosition) ? info : new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            room.SetPlayerMark(player, limit_mark, 0);
            int count = player.MaxHp - player.Hp;
            RecoverStruct recover = new RecoverStruct
            {
                Recover = count,
                Who = player
            };
            room.Recover(player, recover, true);
            if (player.Alive) room.DrawCards(player, count, Name);

            return false;
        }
    }
    public class LongdanJX : OneCardViewAsSkill
    {
        public LongdanJX() : base("longdan_jx")
        {
            skill_type = SkillType.Alter;
            response_or_use = true;
        }
        public override bool ViewFilter(Room room, WrappedCard card, Player player)
        {
            switch (room.GetRoomState().GetCurrentCardUseReason())
            {
                case CardUseReason.CARD_USE_REASON_PLAY:
                    {
                        if (Slash.IsAvailable(room, player) && card.Name == Jink.ClassName)
                            return true;
                        else if (card.Name == Peach.ClassName)
                        {
                            WrappedCard ana = new WrappedCard(Analeptic.ClassName);
                            ana.AddSubCard(card.Id);
                            ana = RoomLogic.ParseUseCard(room, ana);
                            return Analeptic.IsAnalepticAvailable(room, player, ana);
                        }
                        else if (card.Name == Analeptic.ClassName && player.IsWounded())
                        {
                            WrappedCard ana = new WrappedCard(Peach.ClassName);
                            ana.AddSubCard(card.Id);
                            ana = RoomLogic.ParseUseCard(room, ana);
                            return Peach.Instance.IsAvailable(room, player, ana);
                        }
                    }
                    break;
                case CardUseReason.CARD_USE_REASON_RESPONSE:
                case CardUseReason.CARD_USE_REASON_RESPONSE_USE:
                    string pattern = room.GetRoomState().GetCurrentCardUsePattern();
                    pattern = Engine.GetPattern(pattern).GetPatternString();
                    if (pattern == Slash.ClassName)
                        return card.Name == Jink.ClassName;
                    else if (pattern == Jink.ClassName)
                    {
                        FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                        return fcard is Slash;
                    }
                    else if (pattern == Peach.ClassName)
                    {
                        FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                        return fcard is Analeptic;
                    }
                    else if (pattern == "Peach,Analeptic")
                    {
                        FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                        return fcard is Peach || fcard is Analeptic;
                    }
                    else if (pattern == Analeptic.ClassName)
                    {
                        FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                        return fcard is Peach;
                    }
                    break;
                default:
                    return false;
            }

            return false;
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return true;
        }
        public override bool IsEnabledAtResponse(Room room, Player player, string pattern)
        {
            pattern = Engine.GetPattern(pattern).GetPatternString();
            return pattern == Jink.ClassName || pattern == Slash.ClassName;
        }
        public override WrappedCard ViewAs(Room room, WrappedCard originalCard, Player player)
        {
            FunctionCard fcard = Engine.GetFunctionCard(originalCard.Name);
            if (fcard is Slash)
            {
                WrappedCard jink = new WrappedCard(Jink.ClassName);
                jink.AddSubCard(originalCard);
                jink.Skill = Name;
                jink = RoomLogic.ParseUseCard(room, jink);
                return jink;
            }
            else if (fcard is Jink)
            {
                WrappedCard slash = new WrappedCard(Slash.ClassName);
                slash.AddSubCard(originalCard);
                slash.Skill = Name;
                slash = RoomLogic.ParseUseCard(room, slash);
                return slash;
            }
            else if (fcard is Peach)
            {
                WrappedCard ana = new WrappedCard(Analeptic.ClassName);
                ana.AddSubCard(originalCard);
                ana.Skill = Name;
                ana = RoomLogic.ParseUseCard(room, ana);
                return ana;
            }
            else if (fcard is Analeptic)
            {
                WrappedCard ana = new WrappedCard(Peach.ClassName);
                ana.AddSubCard(originalCard);
                ana.Skill = Name;
                ana = RoomLogic.ParseUseCard(room, ana);
                return ana;
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
                if (triggerEvent == TriggerEvent.CardUsed && data is CardUseStruct use && use.IsHandcard)
                    card = use.Card;
                else if (triggerEvent == TriggerEvent.CardResponded && data is CardResponseStruct response
                    && response.Handcard)
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
            room.MoveCardTo(room.GetCard(ids[0]), null, Place.PlaceTable, new CardMoveReason(MoveReason.S_REASON_TURNOVER, player.Name));

            bool discard = true;
            if ((Engine.GetFunctionCard(card.Name) is EquipCard && Engine.GetFunctionCard(room.GetCard(ids[0]).Name) is EquipCard)
                || (Engine.GetFunctionCard(card.Name) is BasicCard && Engine.GetFunctionCard(room.GetCard(ids[0]).Name) is BasicCard)
                || (Engine.GetFunctionCard(card.Name) is TrickCard && Engine.GetFunctionCard(room.GetCard(ids[0]).Name) is TrickCard))
                discard = false;

            if (room.GetCardPlace(ids[0]) == Place.PlaceTable)
            {
                if (!discard)
                {
                    Player p = room.AskForPlayerChosen(player, room.GetAlivePlayers(), Name, "@yajiao:::" + room.GetCard(ids[0]).Name, false, false, info.SkillPosition);
                    room.ObtainCard(p, ref ids, new CardMoveReason(MoveReason.S_REASON_PREVIEWGIVE, player.Name, p.Name, Name, string.Empty));
                }
                else
                {
                    List<Player> targets = new List<Player>();
                    foreach (Player p in room.GetAlivePlayers())
                    {
                        if (!p.IsAllNude() && (p == player || RoomLogic.InMyAttackRange(room, player, p)) && RoomLogic.CanDiscard(room, player, p, "hej"))
                            targets.Add(p);
                    }

                    if (targets.Count > 0)
                    {
                        player.SetFlags(Name);
                        Player target = room.AskForPlayerChosen(player, targets, Name, "@yajiao-disacard", false, false, info.SkillPosition);
                        player.SetFlags("-yajiao");
                        if (target != null)
                        {
                            room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, target.Name);
                            int id = room.AskForCardChosen(player, target, "hej", Name, false, HandlingMethod.MethodDiscard);
                            room.ThrowCard(id, room.GetCardPlace(id) == Place.PlaceDelayedTrick ? null : target, player);
                        }
                    }
                }
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
                {
                    if (p.GetMark(Name) > 0)
                    {
                        p.SetMark(Name, 0);
                        room.RemovePlayerStringMark(p, Name);
                        room.FilterCards(p, p.GetCards("he"), true);
                    }
                }
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
            skill_target.SetFlags("TieqiTarget"); //for AI
            bool invoke = room.AskForSkillInvoke(player, Name, skill_target, info.SkillPosition);
            skill_target.SetFlags("-TieqiTarget");
            if (invoke)
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
            room.SetPlayerStringMark(target, Name, string.Empty);
            target.SetMark(Name, 1);
            room.FilterCards(target, target.GetCards("he"), true);

            JudgeStruct judge = new JudgeStruct
            {
                Reason = Name,
                Who = source
            };
            target.SetFlags("TieqiTarget"); //for AI
            room.Judge(ref judge);
            target.SetFlags("-TieqiTarget");

            Thread.Sleep(400);

            string suit = WrappedCard.GetSuitString(judge.JudgeSuit);
            LogMessage l = new LogMessage
            {
                Type = "#tieqijudge",
                From = source.Name,
                Arg = Name,
                Arg2 = WrappedCard.GetSuitString(judge.JudgeSuit)
            };
            room.SendLog(l);
            target.SetTag("tieqi_judge", judge.Card.Id);

            if (room.AskForCard(target, "tieqi",
                string.Format("..{0}", suit.Substring(0, 1).ToUpper()),
                string.Format("@tieqi-discard:::{0}", string.Format("<color={0}>{1}</color>", WrappedCard.IsBlack(judge.JudgeSuit) ? "black" : "red", WrappedCard.GetSuitIcon(judge.JudgeSuit))),
                judge.Card) == null)
            {
                LogMessage log = new LogMessage
                {
                    Type = "#NoJink",
                    From = target.Name
                };
                room.SendLog(log);

                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, source, Name, info.SkillPosition);
                room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);

                int index = 0;
                for (int i = 0; i < use.EffectCount.Count; i++)
                {
                    CardBasicEffect effect = use.EffectCount[i];
                    if (effect.To == target)
                    {
                        index++;
                        if (index == info.Times)
                        {
                            effect.Effect2 = 0;
                            use.EffectCount[i] = effect;
                            break;
                        }
                    }
                }

                Thread.Sleep(500);
            }
            else
            {
                LogMessage log = new LogMessage
                {
                    Type = "#tieqidis",
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
            if (player.HasEquip(skill)) return false;
            Skill s = Engine.GetSkill(skill);
            return s != null && !s.Attached_lord_skill && s.SkillFrequency != Frequency.Compulsory && s.SkillFrequency != Frequency.Wake && player.GetMark("tieqi_jx") > 0;
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
            if (triggerEvent == TriggerEvent.CardUsed && base.Triggerable(player, room) && data is CardUseStruct use && use.Card != null && (!use.Card.IsVirtualCard() || use.Card.SubCards.Count == 0))
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is TrickCard)
                    return new TriggerStruct(Name, player);
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
                new CardMoveReason(MoveReason.S_REASON_DRAW, player.Name, player.Name, Name, string.Empty));
            card_ids = room.MoveCardsAtomic(new List<CardsMoveStruct> { move }, false);
            if (player.Phase == PlayerPhase.Play && card_ids.Count == 1 && room.GetCardPlace(card_ids[0]) == Place.PlaceHand
                && room.GetCardOwner(card_ids[0]) == player && Engine.GetFunctionCard(room.GetCard(card_ids[0]).Name) is BasicCard)
            {
                List<int> ids = room.AskForExchange(player, Name, 1, 0, "@jizhi", string.Empty, card_ids[0].ToString(), info.SkillPosition);
                if (ids.Count > 0)
                {
                    room.ThrowCard(ref ids, player, null, Name);
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
        public override bool GetDistanceLimit(Room room, Player from, Player to, WrappedCard card, CardUseReason reason, string pattern)
        {
            if (RoomLogic.PlayerHasSkill(room, from, Name))
                return true;
            else
                return false;
        }
    }

    public class QicaiFix : FixCardSkill
    {
        public QicaiFix() : base("#qicai-fix") { }

        public override bool IsCardFixed(Room room, Player from, Player to, string flags, HandlingMethod method)
        {
            if (to != null && from != null && from != to && (flags == "t" || flags == "a")
                && method == HandlingMethod.MethodDiscard && RoomLogic.PlayerHasSkill(room, to, "qicai_jx"))
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
            return room.AliveCount() <= 3 ? 3 : 5;
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
            if (data is CardsMoveOneTimeStruct move && move.From != null && move.From_places.Contains(Place.PlaceHand) && base.Triggerable(move.From, room) && move.From.IsKongcheng())
                return new TriggerStruct(Name, move.From);

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(ask_who, Name);
            room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
            return false;
        }
    }

    public class KongchengJXP : ProhibitSkill
    {
        public KongchengJXP() : base("#kongcheng_jx-prohibit") { }

        public override bool IsProhibited(Room room, Player from, Player to, WrappedCard card, List<Player> others = null)
        {
            if (to != null && card != null && (card.Name == Duel.ClassName || card.Name.Contains(Slash.ClassName))
                && RoomLogic.PlayerHasSkill(room, to, Name) && to.IsKongcheng())
                return true;

            return false;
        }
    }

    public class Jijie : ZeroCardViewAsSkill
    {
        public Jijie() : base("jijie")
        {
            skill_type = SkillType.Replenish;
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(JijieCard.ClassName);
        }
        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard(JijieCard.ClassName) { Skill = Name };
        }
    }

    public class JijieCard : SkillCard
    {
        public static string ClassName = "JijieCard";
        public JijieCard() : base(ClassName)
        {
            target_fixed = true;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player guojia = card_use.From;
            List<int> yiji_cards = new List<int> { room.DrawPile[room.DrawPile.Count - 1] };
            room.RemoveFromDrawPile(yiji_cards);

            guojia.PileChange("#jijie", yiji_cards);
            if (!room.AskForYiji(guojia, yiji_cards, "jijie", true, false, true, -1, room.GetOtherPlayers(guojia), null, "@jijie", "#jijie", false, card_use.Card.SkillPosition))
            {
                CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_GOTBACK, guojia.Name, "jijie", string.Empty);
                room.ObtainCard(guojia, ref yiji_cards, reason, false);
            }
            guojia.Piles["#jijie"].Clear();
        }
    }

    public class Jiyuan : TriggerSkill
    {
        public Jiyuan() : base("jiyuan")
        {
            skill_type = SkillType.Replenish;
            events = new List<TriggerEvent> { TriggerEvent.CardsMoveOneTime, TriggerEvent.Dying };
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.Dying)
            {
                foreach (Player p in RoomLogic.FindPlayersBySkillName(room, Name))
                    triggers.Add(new TriggerStruct(Name, p));
            }
            else if (data is CardsMoveOneTimeStruct move && move.To != null && move.To_place == Place.PlaceHand && move.To.Alive
                && (move.Reason.Reason == MoveReason.S_REASON_PREVIEWGIVE || move.Reason.Reason == MoveReason.S_REASON_GIVE)
                && !string.IsNullOrEmpty(move.Reason.PlayerId))
            {
                Player yiji = room.FindPlayer(move.Reason.PlayerId);
                if (base.Triggerable(yiji, room) && yiji != move.To)
                    triggers.Add(new TriggerStruct(Name, yiji));
            }

            return triggers;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player target = player;
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move)
                target = move.To;

            target.SetFlags(Name);
            bool invoke = room.AskForSkillInvoke(ask_who, Name, target, info.SkillPosition);
            target.SetFlags("-jiyuan");
            if (invoke)
            {
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, ask_who.Name, target.Name);
                room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player target = player;
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move)
                target = move.To;

            room.DrawCards(target, new DrawCardStruct(1, ask_who, Name));

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
            if (data is DamageStruct damage && damage.To.Alive && damage.Card != null && damage.Card.Name.Contains(Slash.ClassName) && RoomLogic.PlayerHasSkill(room, player, Name)
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
                int id = room.AskForCardChosen(player, damage.To, "hej", Name, false, HandlingMethod.MethodGet);
                Debug.Assert(id > -1, "liyu get card error");
                room.ObtainCard(player, id);
                bool duel = Engine.GetFunctionCard(room.GetCard(id).Name) is EquipCard;
                if (duel)
                {
                    WrappedCard _Duel = new WrappedCard(Duel.ClassName) { Skill = "_liyu" };
                    List<Player> targets = new List<Player>();
                    foreach (Player p in room.GetOtherPlayers(damage.To))
                    {
                        if (p == player || p == damage.To) continue;
                        if (RoomLogic.IsProhibited(room, player, p, _Duel) == null)
                            targets.Add(p);
                    }

                    if (targets.Count > 0)
                    {
                        Player victim = room.AskForPlayerChosen(damage.To, targets, Name, "@liyu-duel:" + player.Name);
                        if (victim != null)
                            room.UseCard(new CardUseStruct(_Duel, player, victim));
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
                if (Engine.GetFunctionCard(card.Name).TypeID != CardType.TypeSkill)
                {
                    List<int> table_cardids = room.GetCardIdsOnTable(room.GetSubCards(card));
                    List<int> ids = new List<int>(card.SubCards);
                    if (table_cardids.Count > 0 && ids.SequenceEqual(table_cardids))
                    {
                        bool check = true;
                        foreach (int id in card.SubCards)
                        {
                            if (room.GetCardPlace(id) != Place.PlaceTable)
                            {
                                check = false;
                                break;
                            }
                        }

                        if (check)
                        {
                            room.RemoveSubCards(card);
                            room.ObtainCard(target, damage.Card);
                        }
                    }
                }
            }
            if (target.Alive)
                room.DrawCards(target, 1, Name);
        }
    }

    public class Hujia : ZeroCardViewAsSkill
    {
        public Hujia() : base("hujia") { skill_type = SkillType.Defense; lord_skill = true; }
        public override bool IsEnabledAtPlay(Room room, Player player) => false;
        public override bool IsEnabledAtResponse(Room room, Player player, string pattern)
        {
            if (pattern != Jink.ClassName || player.HasFlag(string.Format("hujia_{0}", room.GetRoomState().GetCurrentResponseID())))
                return false;

            foreach (Player p in room.GetOtherPlayers(player))
                if (p.Kingdom == "wei") return true;

            return false;
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard(HujiaCard.ClassName) { Skill = Name };
        }
    }

    public class HujiaCard : SkillCard
    {
        public static string ClassName = "HujiaCard";
        public HujiaCard() : base(ClassName)
        {
            target_fixed = true;
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

            room.BroadcastSkillInvoke("hujia", player, card.SkillPosition);
            room.NotifySkillInvoked(player, "hujia");

            HandlingMethod method = room.GetRoomState().GetCurrentCardUseReason() == CardUseReason.CARD_USE_REASON_RESPONSE_USE ? HandlingMethod.MethodUse : HandlingMethod.MethodResponse;
            player.SetFlags(string.Format("hujia_{0}", room.GetRoomState().GetCurrentResponseID()));

            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (p.Kingdom == "wei")
                {
                    CardEffectStruct effect = new CardEffectStruct
                    {
                        Card = card,
                        From = player,
                        To = p
                    };
                    WrappedCard slash = room.AskForCard(p, "hujia", Jink.ClassName, "@hujia:" + player.Name, effect,
                        room.GetRoomState().GetCurrentCardUseReason() == CardUseReason.CARD_USE_REASON_RESPONSE ? HandlingMethod.MethodResponse : HandlingMethod.MethodUse);
                    if (slash != null)
                    {
                        ResultStruct result = p.Result;
                        result.Assist++;
                        p.Result = result;

                        Thread.Sleep(500);
                        return slash;
                    }
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
            int card_id = room.AskForCardChosen(simayi, damage.From, "he", Name, false, HandlingMethod.MethodGet);
            CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_EXTRACTION, simayi.Name);
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
                bool red = WrappedCard.IsRed(judge.JudgeSuit);
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
            view_as_skill = new QingjianVS();
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
                WrappedCard card = room.AskForUseCard(ask_who, "@@qingjian", "@qingjian", null, -1, HandlingMethod.MethodUse, true, info.SkillPosition);
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
            room.ObtainCard(use.To[0], ref ids, new CardMoveReason(MoveReason.S_REASON_GIVE, ask_who.Name, use.To[0].Name, Name, string.Empty));
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
            //return room.GetCardPlace(to_select.Id) == Place.PlaceHand;
            return room.GetCardOwner(to_select.Id) == player;
        }

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count > 0)
            {
                WrappedCard card = new WrappedCard(QingjianCard.ClassName);
                card.AddSubCards(cards);
                return card;
            }

            return null;
        }
    }

    public class QingjianCard : SkillCard
    {
        public static string ClassName = "QingjianCard";
        public QingjianCard() : base(ClassName)
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

            ResultStruct result = card_use.From.Result;
            result.Assist += card_use.Card.SubCards.Count;
            card_use.From.Result = result;
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
            room.RemoveTag(Name);
            List<CardsMoveStruct> moves = new List<CardsMoveStruct>();

            foreach (Player p in victims)
            {
                int id = room.AskForCardChosen(player, p, "h", Name, false, HandlingMethod.MethodGet);
                CardsMoveStruct move2 = new CardsMoveStruct
                {
                    Card_ids = new List<int> { id },
                    To = player.Name,
                    To_place = Place.PlaceHand,
                    Reason = new CardMoveReason(MoveReason.S_REASON_EXTRACTION, player.Name, p.Name, Name, string.Empty)
                };
                moves.Add(move2);
            }
            room.MoveCardsAtomic(moves, false);
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

                List<int> ids = room.GetNCards(3), get = new List<int>(); ;
                foreach (int id in ids)
                {
                    WrappedCard card = room.GetCard(id);
                    FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                    room.MoveCardTo(card, null, Place.PlaceTable, new CardMoveReason(MoveReason.S_REASON_TURNOVER, player.Name));
                    if (fcard is BasicCard || fcard is Weapon || fcard is Duel)
                        get.Add(id);
                }
                if (get.Count > 0)
                {
                    player.SetTag(Name, get);
                    bool invoke = room.AskForSkillInvoke(player, Name, "@luoyi-get", info.SkillPosition);
                    player.RemoveTag(Name);
                    if (invoke)
                    {
                        room.ObtainCard(player, ref get, new CardMoveReason(MoveReason.S_REASON_RECYCLE, player.Name, Name, string.Empty));
                        return info;
                    }
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
                Type = "#AddDamage",
                From = player.Name,
                To = new List<string> { damage.To.Name },
                Arg = Name,
                Arg2 = (++damage.Damage).ToString()
            };
            room.SendLog(log);

            data = damage;

            return false;
        }
    }

    public class YijiJX : MasochismSkill
    {
        public YijiJX() : base("yiji_jx")
        {
            view_as_skill = new YijiVS();
            frequency = Frequency.Frequent;
            skill_type = SkillType.Masochism;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is DamageStruct damage)
            {
                TriggerStruct trigger = new TriggerStruct(Name, player)
                {
                    Times = damage.Damage
                };
                return trigger;
            }

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(player, Name, data, info.SkillPosition))
            {
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                return info;
            }

            return new TriggerStruct();
        }
        public override void OnDamaged(Room room, Player guojia, DamageStruct damage, TriggerStruct info)
        {
            room.DrawCards(guojia, 2, Name);
            int count = Math.Min(2, guojia.HandcardNum);
            while (count > 0)
            {
                guojia.SetMark(Name, count);
                WrappedCard card = room.AskForUseCard(guojia, "@@yiji_jx", "@yiji-give:::" + count.ToString(), null, -1, HandlingMethod.MethodUse, true, info.SkillPosition);
                guojia.SetMark(Name, 0);

                if (card != null)
                    count -= card.SubCards.Count;
                else
                    break;
            }
        }
    }

    public class YijiVS : ViewAsSkill
    {
        public YijiVS() : base("yiji_jx")
        {
            response_pattern = "@@yiji_jx";
        }
        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return selected.Count < player.GetMark(Name) && room.GetCardPlace(to_select.Id) == Place.PlaceHand;
        }

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count <= player.GetMark(Name) && cards.Count > 0)
            {
                WrappedCard card = new WrappedCard(YijiJCard.ClassName) { Skill = Name };
                card.AddSubCards(cards);
                return card;
            }

            return null;
        }
    }

    public class YijiJCard : SkillCard
    {
        public static string ClassName = "YijiJCard";
        public YijiJCard() : base(ClassName) { }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && Self != to_select;
        }

        public override void OnUse(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            Player target = card_use.To[0];

            ResultStruct result = card_use.From.Result;
            result.Assist += card_use.Card.SubCards.Count;
            card_use.From.Result = result;

            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, card_use.Card.SkillPosition);
            room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
            room.NotifySkillInvoked(player, "yiji_jx");
            room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, target.Name);
            List<int> ids = new List<int>(card_use.Card.SubCards);
            room.ObtainCard(target, ref ids, new CardMoveReason(MoveReason.S_REASON_GIVE, player.Name, target.Name, "yiji_jx", string.Empty), false);
        }
    }
    public class LuoshenJX : TriggerSkill
    {
        public LuoshenJX() : base("luoshen_jx")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart, TriggerEvent.CardsMoveOneTime, TriggerEvent.EventPhaseChanging };
            frequency = Frequency.Frequent;
            skill_type = SkillType.Replenish;
        }
        public override bool CanPreShow() => false;

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
            {
                foreach (int id in player.GetCards("h"))
                    room.SetCardFlag(id, "-luoshen_jx");
            }
            else if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.From != null && move.From_places.Contains(Place.PlaceHand))
            {
                foreach (int id in move.Card_ids)
                    if (room.GetCard(id).HasFlag(Name))
                        room.SetCardFlag(id, "-luoshen_jx");
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Start && base.Triggerable(player, room))
                return new TriggerStruct(Name, player);

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
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player zhenji, ref object data, Player ask_who, TriggerStruct info)
        {
            JudgeStruct judge;
            do
            {
                judge = new JudgeStruct
                {
                    Pattern = ".|black",
                    Good = true,
                    Reason = Name,
                    PlayAnimation = false,
                    Who = zhenji,
                    Time_consuming = true
                };
                room.Judge(ref judge);

                if (judge.IsBad())
                    Thread.Sleep(1000);
                else if (zhenji.Alive)
                {
                    room.ObtainCard(zhenji, judge.Card, new CardMoveReason(MoveReason.S_REASON_GOTBACK, zhenji.Name));
                    if (room.GetCardPlace(judge.Card.Id) == Place.PlaceHand && room.GetCardOwner(judge.Card.Id) == zhenji)
                        room.SetCardFlag(judge.Card.Id, Name);
                }
            }
            while (judge.IsGood() && zhenji.Alive && room.AskForSkillInvoke(zhenji, Name, null, info.SkillPosition));

            return false;
        }
    }

    public class LuoshenMax : MaxCardsSkill
    {
        public LuoshenMax() : base("#luoshen-max") { }

        public override bool Ingnore(Room room, Player player, int card_id)
        {
            return room.GetCard(card_id).HasFlag("luoshen_jx");
        }
    }

    public class ZhihengJX : ViewAsSkill
    {
        public ZhihengJX() : base("zhiheng_jx")
        {
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return RoomLogic.CanDiscard(room, player, player, "he") && !player.HasUsed("ZhihengJCard");
        }
        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return !RoomLogic.IsJilei(room, player, to_select);
        }
        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count == 0)
                return null;

            WrappedCard zhiheng_card = new WrappedCard("ZhihengJCard")
            {
                Skill = Name,
                ShowSkill = Name
            };
            zhiheng_card.AddSubCards(cards);
            return zhiheng_card;
        }
    }

    public class ZhihengJCard : SkillCard
    {
        public static string ClassName = "ZhihengJCard";
        public ZhihengJCard() : base(ClassName)
        {
            target_fixed = true;
        }
        public override void OnCardAnnounce(Room room, CardUseStruct use, bool ignore_rule)
        {
            if (use.From.IsLastHandCard(use.Card, true))
                use.Card.UserString = "1";
            base.OnCardAnnounce(room, use, ignore_rule);
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            if (card_use.From.Alive)
            {
                int count = card_use.Card.SubCards.Count;
                if (card_use.Card.UserString == "1") count++;
                room.DrawCards(card_use.From, count, "zhiheng_jx");
            }
        }
    }

    public class Jiuyuan : TriggerSkill
    {
        public Jiuyuan() : base("jiuyuan")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardUsed };
            skill_type = SkillType.Recover;
            lord_skill = true;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is CardUseStruct use && use.Card.Name == Peach.ClassName && player.Kingdom == "wu" && use.To.Contains(player))
            {
                List<Player> lords = RoomLogic.FindPlayersBySkillName(room, Name), tos = new List<Player>();
                foreach (Player p in lords)
                    if (player.Hp > p.Hp)
                        tos.Add(p);

                if (tos.Count > 0)
                    return new TriggerStruct(Name, player, tos);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player target, ref object data, Player player, TriggerStruct info)
        {
            if (data is CardUseStruct use && player.Kingdom == "wu" && use.To.Contains(player) && player.Hp > target.Hp && target.Alive && room.AskForSkillInvoke(player, Name, target))
            {
                ResultStruct result = player.Result;
                result.Assist++;
                player.Result = result;

                room.BroadcastSkillInvoke(Name, target);
                room.CancelTarget(ref use, player);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player target, ref object data, Player player, TriggerStruct info)
        {
            RecoverStruct recover = new RecoverStruct
            {
                Who = player,
                Recover = 1
            };
            room.Recover(target, recover);

            room.DrawCards(player, 1, Name);

            return false;
        }
    }

        /*
        public class Jiuyuan : TriggerSkill
        {
            public Jiuyuan() : base("jiuyuan")
            {
                events = new List<TriggerEvent> { TriggerEvent.TargetConfirmed, TriggerEvent.PreHpRecover };
                frequency = Frequency.Compulsory;
                skill_type = SkillType.Recover;
            }

            public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
            {
                if (triggerEvent == TriggerEvent.TargetConfirmed && data is CardUseStruct use && base.Triggerable(player, room) && player.HasFlag("Global_Dying")
                    && player != use.From && use.Card.Name == Peach.ClassName && use.From != null && use.From.Kingdom == "wu")
                    use.Card.SetFlags(Name);
            }

            public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
            {
                if (triggerEvent == TriggerEvent.PreHpRecover && data is RecoverStruct recover && recover.Card != null && recover.Card.HasFlag(Name))
                    return new TriggerStruct(Name, player);

                return new TriggerStruct();
            }

            public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
            {
                if (data is RecoverStruct recover)
                {
                    room.SendCompulsoryTriggerLog(player, Name, true);
                    room.BroadcastSkillInvoke(Name, player, info.SkillPosition);

                    LogMessage log = new LogMessage("#JiuyuanExtraRecover")
                    {
                        From = player.Name,
                        To = new List<string> { recover.Who.Name },
                        Arg = Name
                    };
                    room.SendLog(log);

                    recover.Recover++;
                    data = recover;
                }

                return false;
            }
        }
        */
    public class Fenwei : TriggerSkill
    {
        public Fenwei() : base("fenwei")
        {
            events.Add(TriggerEvent.TargetChosen);
            frequency = Frequency.Limited;
            limit_mark = "@fenwei";
            skill_type = SkillType.Defense;
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> result = new List<TriggerStruct>();
            if (data is CardUseStruct use && use.To.Count > 1 && Engine.GetFunctionCard(use.Card.Name) is TrickCard)
            {
                List<Player> gannings = RoomLogic.FindPlayersBySkillName(room, Name);
                foreach (Player p in gannings)
                    if (p.GetMark("@fenwei") > 0)
                        result.Add(new TriggerStruct(Name, p));
            }

            return result;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use)
            {
                room.SetTag(Name, data);
                List<Player> players = room.AskForPlayersChosen(ask_who, new List<Player>(use.To), Name, 0, use.To.Count, "@fenwei:::" + use.Card.Name, true, info.SkillPosition);
                room.RemoveTag(Name);
                if (players.Count > 0)
                {
                    room.SetTag(Name, players);
                    room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                    room.SetPlayerMark(ask_who, "@fenwei", 0);
                    room.DoSuperLightbox(ask_who, info.SkillPosition, Name);
                    return info;
                }
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<Player> players = (List<Player>)room.GetTag(Name);
            room.RemoveTag(Name);
            if (data is CardUseStruct use)
            {
                for (int i = 0; i < use.EffectCount.Count; i++)
                {
                    CardBasicEffect effect = use.EffectCount[i];
                    if (players.Contains(effect.To))
                    {
                        effect.Nullified = true;
                        use.EffectCount[i] = effect;
                    }
                }

                data = use;
            }

            return false;
        }
    }

    public class KejiJX : TriggerSkill
    {
        public KejiJX() : base("keji_jx")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseChanging, TriggerEvent.PreCardUsed, TriggerEvent.CardResponded };

        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {

            if (player != null && player.Alive && player.Phase == PlayerPhase.Play && (triggerEvent == TriggerEvent.PreCardUsed || triggerEvent == TriggerEvent.CardResponded))
            {
                WrappedCard card = null;
                if (triggerEvent == TriggerEvent.PreCardUsed && data is CardUseStruct use)
                    card = use.Card;
                else if (triggerEvent == TriggerEvent.CardResponded && data is CardResponseStruct resp)
                    card = resp.Card;
                if (card.Name.Contains(Slash.ClassName)) player.SetFlags("KejiSlashInPlayPhase");
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change
                    && !player.HasFlag("KejiSlashInPlayPhase") && change.To == PlayerPhase.Discard)
                return new TriggerStruct(Name, player);

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
            room.SkipPhase(player, PlayerPhase.Discard, true);
            return false;
        }
    }

    public class Qinxue : PhaseChangeSkill
    {
        public Qinxue() : base("qinxue")
        {
            frequency = Frequency.Wake;
        }

        public override bool Triggerable(Player target, Room room)
        {
            int count = 3 + target.Hp;
            if (room.Players.Count >= 7) count = 2 + target.Hp;
            return target.GetMark(Name) == 0 && base.Triggerable(target, room) && target.Phase == PlayerPhase.Start && target.HandcardNum >= count;
        }

        public override bool OnPhaseChange(Room room, Player player, TriggerStruct info)
        {
            room.SetPlayerMark(player, Name, 1);
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            room.DoSuperLightbox(player, info.SkillPosition, Name);
            room.LoseMaxHp(player);
            if (player.Alive)
                room.HandleAcquireDetachSkills(player, "gongxin", true);
            return false;
        }
    }
    
    public class Gongxin : ZeroCardViewAsSkill
    {
        public Gongxin() : base("gongxin")
        {
            skill_type = SkillType.Attack;
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(GongxinCard.ClassName);
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard(GongxinCard.ClassName) { Skill = Name };
        }
    }

    public class GongxinCard : SkillCard
    {
        public static string ClassName = "GongxinCard";
        public GongxinCard() : base(ClassName) { }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select != Self && !to_select.IsKongcheng();
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            Player target = card_use.To[0];
            List<int> ids = new List<int>();
            foreach (int id in target.GetCards("h"))
                if (room.GetCard(id).Suit == WrappedCard.CardSuit.Heart)
                    ids.Add(id);

            target.SetFlags("gongxin_target");
            int result = room.DoGongxin(player, target, target.GetCards("h"), ids, "gongxin", "@gongxin:" + target.Name, card_use.Card.SkillPosition);
            target.SetFlags("-gongxin_target");

            if (result != -1)
            {
                string choice = room.AskForChoice(player, "gongxin", "discard+piletop", null, result);
                if (choice == "disacard")
                    room.ThrowCard(result, target, player);
                else
                    room.MoveCardTo(room.GetCard(result), null, Place.DrawPile, true);
            }
        }
    }

    public class Botu : TriggerSkill
    {
        public Botu() : base("botu")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardUsedAnnounced, TriggerEvent.EventPhaseChanging };
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (player != null && room.Current == player && triggerEvent == TriggerEvent.CardUsedAnnounced && data is CardUseStruct use && use.Card != null && base.Triggerable(player, room))
            {
                int suit = (int)RoomLogic.GetCardSuit(room, use.Card);
                List<int> suits = player.ContainsTag(Name + "Suit") ? (List<int>)player.GetTag(Name + "Suit") : new List<int>();
                if (suit < 4 && !suits.Contains(suit))
                {
                    suits.Add(suit);
                    player.SetTag(Name + "Suit", suits);
                }
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && base.Triggerable(player, room) && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
            {
                List<int> suits = player.ContainsTag(Name + "Suit") ? (List<int>)player.GetTag(Name + "Suit") : new List<int>();
                if (suits.Count == 4)
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(ask_who, Name, null, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            LogMessage l = new LogMessage
            {
                Type = "#Fangquan",
                To = new List<string> { ask_who.Name }
            };
            room.SendLog(l);
            room.GainAnExtraTurn(ask_who);
            return false;
        }
    }

    public class BotuClear : TriggerSkill
    {
        public BotuClear() : base("#botu")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseChanging };
        }

        public override int GetPriority() => 2;

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive && player.ContainsTag("botuSuit"))
                player.RemoveTag("botuSuit");
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class KurouJCard : SkillCard
    {
        public static string ClassName = "KurouJCard";
        public KurouJCard() : base(ClassName)
        {
            target_fixed = true;
            will_throw = true;
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            room.LoseHp(card_use.From);
        }
    }
    public class KurouJX : OneCardViewAsSkill
    {
        public KurouJX() : base("kurou_jx")
        {
            skill_type = SkillType.Masochism;
            filter_pattern = "..!";
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return player.Hp > 0 && !player.HasUsed("KurouJCard");
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard kr = new WrappedCard("KurouJCard")
            {
                Skill = Name,
                ShowSkill = Name
            };
            kr.AddSubCard(card);
            return kr;
        }
    }


    public class Zhaxiang : TriggerSkill
    {
        public Zhaxiang() : base("zhaxiang")
        {
            events = new List<TriggerEvent> { TriggerEvent.HpLost, TriggerEvent.EventPhaseChanging, TriggerEvent.TargetChosen };
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Attack;
        }

        public override void GetEffectIndex(Room room, Player player, WrappedCard card, ref int index, ref string skill_name, ref string general_name, ref int skin_id)
        {
            index = 2;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive && player.GetMark(Name) > 0)
            {
                player.SetMark(Name, 0);
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.HpLost && base.Triggerable(player, room) && data is int count)
            {
                TriggerStruct trigger = new TriggerStruct(Name, player)
                {
                    Times = count
                };
                return trigger;
            }
            else if (triggerEvent == TriggerEvent.TargetChosen && data is CardUseStruct use && use.Card.Name.Contains(Slash.ClassName)
                && WrappedCard.IsRed(use.Card.Suit) && player.GetMark(Name) > 0)
                return new TriggerStruct(Name, player, use.To);

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(ask_who, Name);
            if (triggerEvent == TriggerEvent.HpLost)
            {
                GeneralSkin general = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                room.BroadcastSkillInvoke(Name, "male", 1, general.General, general.SkinId);
                room.DrawCards(player, 3, Name);
                if (room.Current == player)
                    player.AddMark(Name);
            }
            else if (triggerEvent == TriggerEvent.TargetChosen && data is CardUseStruct use)
            {
                GeneralSkin general = RoomLogic.GetGeneralSkin(room, ask_who, Name, info.SkillPosition);
                room.BroadcastSkillInvoke(Name, "male", 2, general.General, general.SkinId);

                LogMessage log = new LogMessage
                {
                    Type = "#NoJink",
                    From = player.Name
                };
                room.SendLog(log);

                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, ask_who, Name, info.SkillPosition);
                room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);

                int index = 0;
                for (int i = 0; i < use.EffectCount.Count; i++)
                {
                    CardBasicEffect effect = use.EffectCount[i];
                    if (effect.To == player)
                    {
                        index++;
                        if (index == info.Times)
                        {
                            effect.Effect2 = 0;
                            use.EffectCount[i] = effect;
                            data = use;
                            break;
                        }
                    }
                }
                Thread.Sleep(500);
            }

            return false;
        }
    }

    public class ZhaxiangTM : TargetModSkill
    {
        public ZhaxiangTM() : base("#zhaxiang-tm", false)
        {
        }
        public override int GetResidueNum(Room room, Player from, WrappedCard card)
        {
            return Engine.MatchExpPattern(room, pattern, from, card) ? from.GetMark("zhaxiang") : 0;
        }

        public override bool GetDistanceLimit(Room room, Player from, Player to, WrappedCard card, CardUseReason reason, string pattern)
        {
            return from.GetMark("zhaxiang") > 0 && WrappedCard.IsRed(RoomLogic.GetCardSuit(room, card));
        }
    }

    public class GuoseJX : OneCardViewAsSkill
    {
        public GuoseJX() : base("guose_jx")
        {
            filter_pattern = "..D";
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(GuoseCard.ClassName);
        }

        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard guose = new WrappedCard(GuoseCard.ClassName)
            {
                Mute = true,
                Skill = Name
            };
            guose.AddSubCard(card);
            return guose;
        }

        public override void GetEffectIndex(Room room, Player player, WrappedCard card, ref int index, ref string skill_name, ref string general_name, ref int skin_id)
        {
            index = 1;
        }
    }

    public class GuoseCard : SkillCard
    {
        public static string ClassName = "GuoseCard";
        public GuoseCard() : base(ClassName) { will_throw = true; }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            if (targets.Count > 0 || to_select == Self) return false;
            FunctionCard indu = Engine.GetFunctionCard(Indulgence.ClassName);
            WrappedCard wrapped = new WrappedCard(Indulgence.ClassName) { Skill = "guose_jx" };
            wrapped.AddSubCards(card.SubCards);
            wrapped = RoomLogic.ParseUseCard(room, wrapped);
            return RoomLogic.PlayerContainsTrick(room, to_select, Indulgence.ClassName) || indu.TargetFilter(room, targets, to_select, Self, wrapped);
        }

        public override WrappedCard Validate(Room room, CardUseStruct use)
        {
            if (!RoomLogic.PlayerContainsTrick(room, use.To[0], Indulgence.ClassName))
            {
                WrappedCard wrapped = new WrappedCard(Indulgence.ClassName) { Skill = "guose_jx" };
                wrapped.AddSubCards(use.Card.SubCards);
                wrapped = RoomLogic.ParseUseCard(room, wrapped);
                room.DrawCards(use.From, 1, "guose_jx");
                return wrapped;
            }
            else
            {
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, use.From, "guose_jx", use.Card.SkillPosition);
                room.BroadcastSkillInvoke("guose_jx", "male", 2, gsk.General, gsk.SkinId);

                room.DrawCards(use.From, 1, "guose_jx");
                return use.Card;
            }
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            Player target = card_use.To[0];

            List<int> ids = new List<int>(target.JudgingArea);
            foreach (int id in ids)
                if (room.GetCard(id).Name == Indulgence.ClassName)
                    room.ThrowCard(id, target, player);
        }
    }

    public class JieyinJX : OneCardViewAsSkill
    {
        public JieyinJX() : base("jieyin_jx") { skill_type = SkillType.Recover; }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(JieyinJCard.ClassName);
        }
        public override bool ViewFilter(Room room, WrappedCard to_select, Player player)
        {
            return Engine.GetFunctionCard(to_select.Name) is EquipCard || (room.GetCardPlace(to_select.Id) == Place.PlaceHand && RoomLogic.CanDiscard(room, player, player, to_select.Id));
        }

        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard jieyin = new WrappedCard(JieyinJCard.ClassName) { Skill = Name };
            jieyin.AddSubCard(card);
            return jieyin;
        }
    }

    public class JieyinJCard : SkillCard
    {
        public static string ClassName = "JieyinJCard";
        public JieyinJCard() : base(ClassName)
        {
            will_throw = false;
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            if (targets.Count > 0 || !to_select.IsMale() || to_select == Self) return false;
            bool hand = room.GetCardPlace(card.GetEffectiveId()) == Place.PlaceHand;

            WrappedCard equip_card = room.GetCard(card.GetEffectiveId());
            FunctionCard fcard = Engine.GetFunctionCard(equip_card.Name);

            if (fcard is EquipCard equip)
            {
                int equip_index = (int)equip.EquipLocation();
                return (to_select.GetEquip(equip_index) < 0 && RoomLogic.CanPutEquip(to_select, equip_card))
                    || (hand && RoomLogic.CanDiscard(room, Self, Self, card.GetEffectiveId()));
            }

            return true;
        }

        public override void OnUse(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From, target = card_use.To[0];

            WrappedCard equip_card = room.GetCard(card_use.Card.GetEffectiveId());
            FunctionCard fcard = Engine.GetFunctionCard(equip_card.Name);
            bool hand = room.GetCardPlace(card_use.Card.GetEffectiveId()) == Place.PlaceHand;

            if (hand)
            {
                if (fcard is EquipCard equip)
                {
                    int equip_index = (int)equip.EquipLocation();
                    if (!RoomLogic.CanDiscard(room, player, player, card_use.Card.GetEffectiveId()))
                        hand = false;
                    else if (target.GetEquip(equip_index) < 0 && RoomLogic.CanPutEquip(target, equip_card))
                        hand = room.AskForChoice(player, "jieyin_jx", "discard+put", new List<string> { "@to-player:" + target.Name }, target) == "discard";
                }
            }

            if (hand)
            {
                CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_THROW, card_use.From.Name, null,
                        card_use.Card.Skill, null)
                {
                    Card = card_use.Card,
                    General = RoomLogic.GetGeneralSkin(room, player, card_use.Card.Skill, card_use.Card.SkillPosition)
                };
                room.RecordSubCards(card_use.Card);
                room.MoveCardTo(card_use.Card, card_use.From, null, Place.PlaceTable, reason, true);

                List<int> table_cardids = room.GetCardIdsOnTable(room.GetSubCards(card_use.Card));
                if (table_cardids.Count > 0)
                {
                    CardsMoveStruct move = new CardsMoveStruct(table_cardids, player, null, Place.PlaceTable, Place.DiscardPile, reason);
                    room.MoveCardsAtomic(new List<CardsMoveStruct> { move }, true);
                    room.RemoveSubCards(card_use.Card);
                }
            }
            else
            {
                room.MoveCardTo(card_use.Card, player, target, Place.PlaceEquip, new CardMoveReason(MoveReason.S_REASON_PUT, player.Name, target.Name, "jieyin_jx", string.Empty));
                LogMessage log = new LogMessage
                {
                    Type = "$ZhijianEquip",
                    From = target.Name,
                    Card_str = card_use.Card.GetEffectiveId().ToString()
                };
                room.SendLog(log);
            }
            
            base.OnUse(room, card_use);
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            Player target = card_use.To[0];
            if (player.Hp != target.Hp)
            {
                Player heal = null, draw = null;
                if (player.Hp > target.Hp)
                {
                    heal = target;
                    draw = player;
                }
                else
                {
                    heal = player;
                    draw = target;
                }

                RecoverStruct recover = new RecoverStruct
                {
                    Who = player,
                    Recover = 1
                };
                room.Recover(heal, recover, true);
                room.DrawCards(draw, new DrawCardStruct(1, player, "jieyin_jx"));
            }
            /*
            else
            {
                RecoverStruct recover = new RecoverStruct
                {
                    Who = player,
                    Recover = 1
                };
                if (player.IsWounded())
                    room.Recover(player, recover, true);
                if (target.IsWounded())
                    room.Recover(target, recover, true);
                room.DrawCards(player, new DrawCardStruct(1, player, "jieyin_jx"));
                room.DrawCards(target, new DrawCardStruct(1, player, "jieyin_jx"));
            }
            */
        }
    }


    public class QianxunJX : TriggerSkill
    {
        public QianxunJX() : base("qianxun_jx")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardEffectConfirmed, TriggerEvent.EventPhaseChanging };
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
            {
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (p.GetPile(Name).Count > 0)
                    {
                        List<int> ids = p.GetPile(Name);
                        room.ObtainCard(p, ref ids, new CardMoveReason(MoveReason.S_REASON_EXCHANGE_FROM_PILE, p.Name, Name, string.Empty), false);
                    }
                }
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.CardEffectConfirmed && base.Triggerable(player, room) && !player.IsKongcheng()
                && data is CardEffectStruct effect && !effect.Multiple && effect.From != player)
            {
                FunctionCard fcard = Engine.GetFunctionCard(effect.Card.Name);
                if (fcard is TrickCard) return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (!player.IsKongcheng() && room.AskForSkillInvoke(player, Name, data, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.AddToPile(player, Name, player.GetCards("h"), false);
            return false;
        }
    }

    public class QianxunClear : DetachEffectSkill
    {
        public QianxunClear() : base("qianxun_jx", "qianxun_jx") { }
    }

    public class Lianying : TriggerSkill
    {
        public Lianying() : base("lianying")
        {
            events.Add(TriggerEvent.CardsMoveOneTime);
            skill_type = SkillType.Replenish;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is CardsMoveOneTimeStruct move && move.From != null && base.Triggerable(move.From, room)
                && move.From_places.Contains(Place.PlaceHand) && move.From.IsKongcheng())
                return new TriggerStruct(Name, move.From);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player p, ref object data, Player luxun, TriggerStruct info)
        {
            if (data is CardsMoveOneTimeStruct move)
            {
                int count = 0;
                foreach (Place place in move.From_places)
                    if (place == Place.PlaceHand) count++;

                int max = Math.Min(count, room.AliveCount());
                List<Player> players = room.AskForPlayersChosen(luxun, room.GetAlivePlayers(), Name, 0, max, "@lianying:::" + max.ToString(), true, info.SkillPosition);
                if (players.Count > 0)
                {
                    room.SetTag(Name, players);
                    room.BroadcastSkillInvoke(Name, luxun, info.SkillPosition);
                    return info;
                }
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player target, ref object data, Player luxun, TriggerStruct info)
        {
            List<Player> players = (List<Player>)room.GetTag(Name);
            room.RemoveTag(Name);
            room.SortByActionOrder(ref players);
            foreach (Player p in players)
                room.DrawCards(p, new DrawCardStruct(1, luxun, Name));

            return false;
        }
    }
}