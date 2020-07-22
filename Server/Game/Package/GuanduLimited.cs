using CommonClass;
using CommonClass.Game;
using CommonClassLibrary;
using SanguoshaServer.Game;
using System;
using System.Collections.Generic;
using static CommonClass.Game.Player;

namespace SanguoshaServer.Package
{
    public class GuanduLimited : GeneralPackage
    {
        public GuanduLimited() : base("GuanduLimited")
        {
            skills = new List<Skill> {
                new Shicai(),
                new Fushi(),
                new Chenggong(),
                new Zhezhu(),
                new Xiying(),
                new Shushou(),
                new Cangchu(),
                new Liangying(),
                new Beizhan(),
                new BeizhanProhibit(),
                new Gangzhi(),
                new Yuanlue(),
                new Fenglue(),
                new Moushi(),
                new MoushiDraw(),
                new Choulue(),
                new Polu(),
                new HengjiangJX(),
                new HengjiangJXDraw(),
                new HengjiangJXFail(),
                new Liehou(),
                new Qigong(),
                new QigongTag(),
            };
            skill_cards = new List<FunctionCard>
            {
                new ShicaiCard(),
                new ZhezhuCard(),
                new ShushouCard(),
                new YuanlueCard(),
                new MoushiCard(),
                new ShenduanCard(),
                new LiehouCard(),
            };
            related_skills = new Dictionary<string, List<string>>
            {
                { "moushi", new List<string> { "#moushi-draw" } },
                { "hengjiang_jx", new List<string> { "#hengjiang_jx-draw", "#hengjiang_jx-fail" } },
                { "qigong", new List<string> { "#qigong-target" } },
            };
        }
    }

    //许攸
    public class ShicaiCard : SkillCard
    {
        public static string ClassName = "ShicaiCard";
        public ShicaiCard() : base(ClassName)
        {
            target_fixed = true;
            will_throw = true;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            card_use.From.SetMark("shicai", 1);
            int id = room.GetNCards(1, false)[0];
            card_use.From.SetMark("shicai_id", id);
            room.DrawCards(card_use.From, 1, "shicai");
        }
    }

    public class ShicaiCardVS : OneCardViewAsSkill
    {
        public ShicaiCardVS() : base("shicai")
        {
            filter_pattern = "..!";
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return player.GetMark(Name) == 0;
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {

            WrappedCard c = new WrappedCard(ShicaiCard.ClassName) { Skill = Name };
            c.AddSubCard(card);
            return c;
        }
    }

    public class Shicai : TriggerSkill
    {
        public Shicai() : base("shicai")
        {
            events = new List<TriggerEvent> { TriggerEvent.DrawPileChanged, TriggerEvent.CardsMoveOneTime, TriggerEvent.EventPhaseStart, TriggerEvent.EventPhaseChanging };
            view_as_skill = new ShicaiCardVS();
            skill_type = SkillType.Alter;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            bool update_pile = false;
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Play && base.Triggerable(player, room))
            {
                update_pile = true;
                player.SetFlags(Name);
                player.SetMark(Name, 0);
            }
            else if (triggerEvent == TriggerEvent.DrawPileChanged)
            {
                update_pile = true;
            }
            else if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move &&
                move.From != null && base.Triggerable(move.From, room) && move.From.GetMark(Name) > 0)
            {
                int id = move.From.GetMark("shicai_id");
                if (move.Card_ids.Contains(id))
                    move.From.SetMark(Name, 0);
            }
            else if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.From == PlayerPhase.Play)
            {
                player.SetFlags("-shicai");
                player.Piles[Name] = new List<int>();

                List<string> args = new List<string>
                {
                    player.Name,
                    Name,
                    string.Empty
                };
                room.DoNotify(room.GetClient(player), CommandType.S_COMMAND_UPDATE_PRIVATE_PILE, args);
            }

            if (update_pile)
            {
                foreach (Player xuyou in RoomLogic.FindPlayersBySkillName(room, Name))
                {
                    if (xuyou.Phase == PlayerPhase.Play && xuyou.HasFlag(Name))
                    {
                        List<int> ids = room.GetNCards(1, false);
                        xuyou.Piles[Name] = new List<int>(ids);
                        List<string> args = new List<string>
                        {
                            xuyou.Name,
                            Name,
                            JsonUntity.Object2Json(ids)
                        };
                        room.DoNotify(room.GetClient(xuyou), CommandType.S_COMMAND_UPDATE_PRIVATE_PILE, args);
                    }
                }
            }
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class Fushi : TriggerSkill
    {
        public Fushi() : base("fushi")
        {
            events.Add(TriggerEvent.BuryVictim);
            frequency = Frequency.Compulsory;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<Player> xuyous = RoomLogic.FindPlayersBySkillName(room, Name);
            int weis = 0, quns = 0;
            foreach (Player p in room.GetAlivePlayers())
            {
                if (p == player) continue;
                if (p.Kingdom == "wei") weis++;
                if (p.Kingdom == "qun") quns++;
            }

            foreach (Player xuyou in xuyous)
            {
                if (weis > quns)
                {
                    room.HandleAcquireDetachSkills(xuyou, "chenggong", true);
                    room.HandleAcquireDetachSkills(xuyou, "-zhezhu");
                }
                else if (quns > weis)
                {
                    room.HandleAcquireDetachSkills(xuyou, "zhezhu", true);
                    room.HandleAcquireDetachSkills(xuyou, "-chenggong");
                }
                else
                {
                    room.HandleAcquireDetachSkills(xuyou, "-zhezhu|-chenggong", true);
                }
            }
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class Chenggong : TriggerSkill
    {
        public Chenggong() : base("chenggong")
        {
            events.Add(TriggerEvent.TargetChosen);
            skill_type = SkillType.Replenish;
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> result = new List<TriggerStruct>();
            if (data is CardUseStruct use && use.Card != null && use.To.Count >= 2)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is SkillCard) return result;
                foreach (Player xuyou in RoomLogic.FindPlayersBySkillName(room, Name))
                {
                    result.Add(new TriggerStruct(Name, xuyou));
                }
            }

            return result;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use && room.AskForSkillInvoke(ask_who, Name, player, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.DrawCards(player, new DrawCardStruct(1, ask_who, Name));
            return false;
        }
    }

    public class ZhezhuCard : SkillCard
    {
        public static string ClassName = "ZhezhuCard";
        public ZhezhuCard() : base(ClassName)
        {
            target_fixed = true;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            List<Player> lords = new List<Player>();
            foreach (Player p in room.GetAlivePlayers())
                if (p.GetRoleEnum() == Player.PlayerRole.Lord)
                    lords.Add(p);

            room.SortByActionOrder(ref lords);
            foreach (Player lord in lords)
            {
                if (!lord.IsNude())
                    room.ObtainCard(card_use.From, room.AskForCardChosen(card_use.From, lord, "he", "zhezhu", false, HandlingMethod.MethodGet), false);
                else
                    room.DrawCards(card_use.From, 1, "zhezhu");
            }

            foreach (Player lord in lords)
            {
                if (lord.Alive)
                {
                    room.SetTag("zhezhu_target", lord);
                    List<int> ids = room.AskForExchange(card_use.From, "zhezhu", 1, 1, "@zhezhu:" + lord.Name, string.Empty, "..", card_use.Card.SkillPosition);
                    room.RemoveTag("zhezhu_target");
                    room.ObtainCard(lord, ref ids, new CardMoveReason(MoveReason.S_REASON_GIVE, card_use.From.Name, lord.Name, "zhezhu", string.Empty), false);
                }
            }
        }
    }

    public class Zhezhu : ZeroCardViewAsSkill
    {
        public Zhezhu() : base("zhezhu")
        {
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(ZhezhuCard.ClassName);
        }
        public override WrappedCard ViewAs(Room room, Player player)
        {
            WrappedCard c = new WrappedCard(ZhezhuCard.ClassName) { Skill = Name };
            return c;
        }
    }

    //高览
    public class Xiying : TriggerSkill
    {
        public Xiying() : base("xiying")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseChanging, TriggerEvent.EventPhaseStart };
            skill_type = SkillType.Attack;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == Player.PlayerPhase.NotActive)
            {
                foreach (Player p in room.GetAllPlayers(true))
                {
                    if (p.GetMark(Name) > 0)
                    {
                        RoomLogic.RemovePlayerCardLimitation(p, Name);
                        p.SetMark(Name, 0);
                        room.RemovePlayerStringMark(p, "no_handcards");
                    }
                }
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && base.Triggerable(player, room) && player.Phase == Player.PlayerPhase.Play)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            player.SetFlags(Name);
            bool invoke = room.AskForDiscard(player, Name, 1, 1, true, true, "@xiying-invoke", true, info.SkillPosition);
            if (invoke)
            {
                foreach (Player p in room.GetOtherPlayers(player))
                    room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, p.Name);

                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (!room.AskForDiscard(p, Name, 1, 1, true, true, "@xiying-discard"))
                {
                    p.SetMark(Name, 1);
                    room.SetPlayerStringMark(p, "no_handcards", string.Empty);
                    RoomLogic.SetPlayerCardLimitation(p, Name, "use,response", ".", true);
                }
            }

            return false;
        }
    }

    //淳于琼
    public class Cangchu : TriggerSkill
    {
        public Cangchu() : base("cangchu")
        {
            events = new List<TriggerEvent> { TriggerEvent.GameStart, TriggerEvent.Damaged };
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player != null && base.Triggerable(player, room))
            {
                if (triggerEvent == TriggerEvent.Damaged && player.GetMark("@gd_liang") > 0 && data is DamageStruct damage && damage.Nature == DamageStruct.DamageNature.Fire)
                {
                    return new TriggerStruct(Name, player);
                }
                else if (triggerEvent == TriggerEvent.GameStart)
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name, true);
            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
            if (triggerEvent == TriggerEvent.GameStart)
            {
                room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                room.SetPlayerMark(player, "@gd_liang", 3);
            }
            else if (data is DamageStruct damage)
            {
                room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);
                if (player.GetMark("@gd_liang") - damage.Damage <= 0)
                    player.SetFlags("lose_all_liang");

                room.RemovePlayerMark(player, "@gd_liang", damage.Damage);
            }

            return false;
        }
    }

    public class ShushouVS : OneCardViewAsSkill
    {
        public ShushouVS() : base("shushou")
        {
            response_pattern = "@@shushou";
        }
        public override bool ViewFilter(Room room, WrappedCard to_select, Player player)
        {
            return player.GetPile("#shushou").Contains(to_select.Id);
        }

        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard ss = new WrappedCard(ShushouCard.ClassName);
            ss.AddSubCard(card);
            return ss;
        }
    }

    public class ShushouCard : SkillCard
    {
        public static string ClassName = "ShushouCard";
        public ShushouCard() : base(ClassName)
        {
            will_throw = false;
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select.HasFlag("shushou") && to_select != Self;
        }

        public override void OnUse(Room room, CardUseStruct card_use)
        {
            room.SetTag("shushou_target", card_use.To[0]);

            ResultStruct result = card_use.From.Result;
            result.Assist += card_use.Card.SubCards.Count;
            card_use.From.Result = result;
        }
    }
    public class Shushou : TriggerSkill
    {
        public Shushou() : base("shushou")
        {
            events.Add(TriggerEvent.EventPhaseStart);
            view_as_skill = new ShushouVS();
            skill_type = SkillType.Replenish;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && player.Phase == Player.PlayerPhase.Discard)
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
            room.DrawCards(player, player.GetMark("@gd_liang") + 1, Name);
            List<Player> friends = new List<Player>(), targets = new List<Player>();
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (p.Camp == player.Camp)
                {
                    friends.Add(p);
                    p.SetFlags(Name);
                }
            }

            List<int> cards = player.GetCards("he");
            player.PileChange("#shushou", cards);

            List<CardsMoveStruct> moves = new List<CardsMoveStruct>();
            while (cards.Count > 0 && friends.Count > 0)
            {
                WrappedCard card = room.AskForUseCard(player, "@@shushou", "@shushou", null, -1, FunctionCard.HandlingMethod.MethodNone, true, info.SkillPosition);
                if (card != null)
                {
                    Player target = (Player)room.GetTag("shushou_target");
                    room.RemoveTag("shushou_target");
                    target.SetFlags("-" + Name);
                    friends.Remove(target);
                    targets.Add(target);
                    CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_GIVE, player.Name, target.Name, Name, string.Empty);
                    CardsMoveStruct move = new CardsMoveStruct(new List<int>(card.SubCards), target, Place.PlaceHand, reason);
                    moves.Add(move);

                    player.PileChange("#shushou", card.SubCards, false);
                    cards.RemoveAll(t => card.SubCards.Contains(t));

                    List<string> args = new List<string> { JsonUntity.Object2Json(card.SubCards), "@attribute:" + target.Name };
                    room.DoNotify(room.GetClient(player), CommandType.S_COMMAND_UPDATE_CARD_FOOTNAME, args);
                }
                else
                    break;
            }

            player.PileChange("#shushou", cards, false);
            foreach (Player p in room.Players)
                p.SetFlags("-shushou");

            if (moves.Count > 0)
            {
                LogMessage l = new LogMessage
                {
                    Type = "#ChoosePlayerWithSkill",
                    From = player.Name,
                    Arg = Name
                };
                l.SetTos(targets);
                room.SendLog(l);

                room.MoveCardsAtomic(moves, false);
            }

            return false;
        }
    }

    public class Liangying : TriggerSkill
    {
        public Liangying() : base("liangying")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseProceeding, TriggerEvent.Damaged };
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Wizzard;
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.Damaged && base.Triggerable(player, room) && player.HasFlag("lose_all_liang"))
                triggers.Add(new TriggerStruct(Name, player));
            else if (triggerEvent == TriggerEvent.EventPhaseProceeding && player.Phase == Player.PlayerPhase.Draw && data is int i && i >= 0)
            {
                List<Player> chunyqs = RoomLogic.FindPlayersBySkillName(room, Name);
                foreach (Player p in chunyqs)
                {
                    if (p.IsSameCamp(player) && p.GetMark("@gd_liang") > 0)
                        triggers.Add(new TriggerStruct(Name, p));
                }
            }

            return triggers;
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(ask_who, Name, true);
            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, ask_who, Name, info.SkillPosition);
            if (triggerEvent == TriggerEvent.Damaged && data is DamageStruct damage)
            {
                room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);
                player.SetFlags("-lose_all_liang");
                room.LoseMaxHp(ask_who);
                foreach (Player p in room.GetAlivePlayers())
                    if (!ask_who.IsSameCamp(p))
                        room.DrawCards(p, new DrawCardStruct(2, damage.From, Name));
            }
            else if (data is int i)
            {
                room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                i++;
                data = i;
            }

            return false;
        }
    }

    //审配
    public class Gangzhi : TriggerSkill
    {
        public Gangzhi() : base("gangzhi")
        {
            events.Add(TriggerEvent.DamageForseen);
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Wizzard;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is DamageStruct damage && base.Triggerable(player, room) && damage.From != player)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(ask_who, Name, true);
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            if (data is DamageStruct damage)
            {

                LogMessage log = new LogMessage
                {
                    Type = "#gangzhi",
                    From = ask_who.Name,
                    Arg = Name,
                    Arg2 = damage.Damage.ToString()
                };
                room.SendLog(log);

                room.LoseHp(ask_who, damage.Damage);
                return true;
            }

            return false;
        }
    }

    public class Beizhan : PhaseChangeSkill
    {
        public Beizhan() : base("beizhan")
        {
            skill_type = SkillType.Replenish;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (player.GetMark(Name) > 0 && player.Phase == Player.PlayerPhase.Start)
            {
                player.SetMark(Name, 0);
                int max_cout = 0;
                foreach (Player p in room.GetAlivePlayers())
                    if (p.HandcardNum > max_cout)
                        max_cout = p.HandcardNum;

                if (player.HandcardNum == max_cout)
                {
                    LogMessage log = new LogMessage
                    {
                        Type = "#beizhan",
                        From = player.Name,
                        Arg = Name
                    };
                    room.SendLog(log);

                    player.SetFlags(Name);
                }
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && player.Phase == Player.PlayerPhase.Finish)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player to = room.AskForPlayerChosen(player, room.GetAlivePlayers(), Name, "@beizhan", true, true, info.SkillPosition);
            if (to != null)
            {
                room.SetTag("beizhan_target", to);
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool OnPhaseChange(Room room, Player player, TriggerStruct info)
        {
            Player to = (Player)room.GetTag("beizhan_target");
            room.RemoveTag("beizhan_target");
            if (to != null)
            {
                to.SetMark(Name, 1);
                if (to.HandcardNum < to.MaxHp)
                    room.DrawCards(to, new DrawCardStruct(to.MaxHp - to.HandcardNum, player, Name));
            }

            return false;
        }
    }

    public class BeizhanProhibit : ProhibitSkill
    {
        public BeizhanProhibit() : base("#beizhan-prohibit")
        {
        }
        public override bool IsProhibited(Room room, Player from, Player to, WrappedCard card, List<Player> others = null)
        {
            if (from != null && from.HasFlag("beizhan") && card != null)
            {
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                return !(fcard is SkillCard) && to != null && to != from;
            }
            return false;
        }
    }

    public class YuanlueCard : SkillCard
    {
        public static string ClassName = "YuanlueCard";
        public YuanlueCard() : base(ClassName)
        {
            will_throw = false;
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select != Self;
        }

        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            return card.SubCards.Count == 1 && targets.Count == 1;
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            ResultStruct result = card_use.From.Result;
            result.Assist += card_use.Card.SubCards.Count;
            card_use.From.Result = result;

            Player target = card_use.To[0];
            CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_GIVE, card_use.From.Name, target.Name, "yuanlue", string.Empty);
            List<int> ids = new List<int>(card_use.Card.SubCards);
            room.ObtainCard(target, ref ids, reason, true);

            int card_id = card_use.Card.SubCards[0];
            if (room.GetCardOwner(card_id) == target && room.GetCardPlace(card_id) == Player.Place.PlaceHand)
            {
                WrappedCard card = room.GetCard(card_id);
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                if (fcard.IsAvailable(room, target, card))
                {
                    WrappedCard use = room.AskForUseCard(target, card_id.ToString(), "@yuanlue:" + card_use.From.Name, null);
                    if (use != null && card_use.From.Alive)
                        room.DrawCards(card_use.From, 1, "yuanlue");
                }
            }
        }
    }

    public class Yuanlue : OneCardViewAsSkill
    {
        public Yuanlue() : base("yuanlue")
        {
            filter_pattern = "^EquipCard";
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(YuanlueCard.ClassName);
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard c = new WrappedCard(YuanlueCard.ClassName) { Skill = Name };
            c.AddSubCard(card);
            return c;
        }
    }

    //荀谌
    public class Fenglue : TriggerSkill
    {
        public Fenglue() : base("fenglue")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart, TriggerEvent.Pindian };
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (!base.Triggerable(player, room)) return new TriggerStruct();

            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Play && !player.IsKongcheng())
            {
                bool can_invoke = false;
                List<Player> other_players = room.GetOtherPlayers(player);
                foreach (Player p in other_players)
                {
                    if (RoomLogic.CanBePindianBy(room, p, player))
                    {
                        can_invoke = true;
                        break;
                    }
                }

                return can_invoke ? new TriggerStruct(Name, player) : new TriggerStruct();
            }
            else if (triggerEvent == TriggerEvent.Pindian && data is PindianStruct pd && pd.Reason == Name && pd.From == player && pd.Tos[0].Alive)
            {
                int card_id = pd.From_card.Id;
                if (room.GetCardPlace(card_id) == Place.PlaceTable)
                    return new TriggerStruct(Name, player);
            }
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart)
            {
                List<Player> targets = new List<Player>();
                foreach (Player p in room.GetOtherPlayers(player))
                {
                    if (RoomLogic.CanBePindianBy(room, p, player))
                        targets.Add(p);
                }
                Player victim = room.AskForPlayerChosen(player, targets, Name, "@fenglue", true, true, info.SkillOwner);
                if (victim != null)
                {
                    room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                    PindianStruct pd = room.PindianSelect(player, victim, Name);
                    room.SetTag("shuangren_pd", pd);
                    return info;
                }
            }
            else if (data is PindianStruct pd)
            {
                room.SetTag(Name, pd.Tos[0]);
                bool invoke = room.AskForSkillInvoke(player, Name, "@fenglue-give:" + pd.Tos[0].Name, info.SkillPosition);
                room.RemoveTag(Name);
                if (invoke)
                {
                    //room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                    return info;
                }
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && room.GetTag("shuangren_pd") is PindianStruct pd)
            {
                room.RemoveTag("shuangren_pd");
                Player target = pd.Tos[0];
                room.Pindian(ref pd);
                if (pd.Success)
                {
                    List<int> ids = new List<int>();
                    if (!target.IsKongcheng())
                    {
                        if (target.GetCardCount(false) > 1)
                            ids.AddRange(room.AskForExchange(target, Name, 1, 1, "@fenglue-hand:" + player.Name, string.Empty, ".", string.Empty));
                        else
                            ids.AddRange(target.GetCards("h"));
                    }

                    if (target.HasEquip())
                    {
                        if (target.GetEquips().Count > 1)
                            ids.AddRange(room.AskForExchange(target, Name, 1, 1, "@fenglue-equip:" + player.Name, string.Empty, ".|.|.|equipped", string.Empty));
                        else
                            ids.AddRange(target.GetEquips());
                    }

                    if (target.JudgingArea.Count > 0)
                    {
                        if (target.JudgingArea.Count > 1)
                            ids.Add(room.AskForCardChosen(target, target, "j", Name));
                        else
                            ids.AddRange(target.JudgingArea);
                    }

                    room.ObtainCard(player, ref ids, new CardMoveReason(MoveReason.S_REASON_GIVE, target.Name, player.Name, Name, string.Empty), false);
                }
                else if (!player.IsNude())
                {
                    List<int> ids = room.AskForExchange(player, Name, 1, 1, "@fenglue-fail:" + target.Name, string.Empty, "..", info.SkillPosition);
                    room.ObtainCard(target, ref ids, new CardMoveReason(MoveReason.S_REASON_GIVE, player.Name, target.Name, Name, string.Empty), false);
                }
            }
            else if (data is PindianStruct pindian)
            {
                int card_id = pindian.From_card.Id;
                if (room.GetCardPlace(card_id) == Place.PlaceTable && pindian.Tos[0].Alive)
                    room.ObtainCard(pindian.Tos[0], card_id);
            }

            return false;
        }
    }

    public class MoushiCard : SkillCard
    {
        public static string ClassName = "MoushiCard";
        public MoushiCard() : base(ClassName)
        {
            will_throw = false;
        }
        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            return card.SubCards.Count == 1 && targets.Count == 1;
        }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select != Self;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, "moushi", card_use.Card.SkillPosition);
            room.BroadcastSkillInvoke("moushi", "male", 1, gsk.General, gsk.SkinId);
            Player target = card_use.To[0];
            CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_GIVE, player.Name, target.Name, "moushi", string.Empty);
            List<int> ids = new List<int>(card_use.Card.SubCards);
            room.ObtainCard(target,ref ids, reason, false);

            room.SetPlayerStringMark(target, "moushi", string.Empty);
            room.SetPlayerMark(target, "moushi", 1);
            string key = string.Format("moushi_{0}", target.Name);
            List<Player> _targets = room.ContainsTag(key) ? (List<Player>)room.GetTag(key) : new List<Player>();
            _targets.Add(player);
            room.SetTag(key, _targets);

            ResultStruct result = card_use.From.Result;
            result.Assist++;
            card_use.From.Result = result;
        }
    }

    public class MoushiVS : OneCardViewAsSkill
    {
        public MoushiVS() : base("moushi")
        {
            filter_pattern = ".";
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(MoushiCard.ClassName);
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard c = new WrappedCard(MoushiCard.ClassName) { Skill = Name, Mute = true };
            c.AddSubCard(card);
            return c;
        }
    }

    public class Moushi : TriggerSkill
    {
        public Moushi() : base("moushi")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseChanging, TriggerEvent.Damage };
            view_as_skill = new MoushiVS();
            skill_type = SkillType.Wizzard;
            priority.Add(TriggerEvent.EventPhaseChanging, 3);
            priority.Add(TriggerEvent.Damage, 1);
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (player.GetMark("moushi") == 0) return;

            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change)
            {
                if (change.From == PlayerPhase.Play)
                {
                    string key = string.Format("moushi_{0}", player.Name);
                    room.RemoveTag(key);
                    room.RemoveTag("moushi_targets");
                    room.SetPlayerMark(player, Name, 0);
                    room.RemovePlayerStringMark(player, Name);
                }
                else if (change.To == PlayerPhase.Play)
                {
                    room.RemoveTag("moushi_targets");
                }
            }
            else if (triggerEvent == TriggerEvent.Damage && player.Phase == PlayerPhase.Play && data is DamageStruct damage)
            {
                List<Player> damages = room.ContainsTag("moushi_targets") ? (List<Player>)room.GetTag("moushi_targets") : new List<Player>();
                if (!damages.Contains(damage.To))
                    damages.Add(damage.To);

                room.SetTag("moushi_targets", damages);
            }
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class MoushiDraw : TriggerSkill
    {
        public MoushiDraw() : base("#moushi-draw")
        {
            events.Add(TriggerEvent.Damage);
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Replenish;
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (player.GetMark("moushi") > 0 && player.Phase == Player.PlayerPhase.Play && data is DamageStruct damage)
            {
                List<Player> damages = room.ContainsTag("moushi_targets") ? (List<Player>)room.GetTag("moushi_targets") : new List<Player>();
                if (!damages.Contains(damage.To))
                {
                    string key = string.Format("moushi_{0}", player.Name);
                    List<Player> targets = room.ContainsTag(key) ? (List<Player>)room.GetTag(key) : new List<Player>();
                    foreach (Player p in targets)
                        if (p.Alive)
                            triggers.Add(new TriggerStruct(Name, p));
                }
            }

            return triggers;
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            ResultStruct result = ask_who.Result;
            result.Assist++;
            ask_who.Result = result;

            room.SendCompulsoryTriggerLog(ask_who, "moushi", true);
            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, "moushi", info.SkillPosition);
            room.BroadcastSkillInvoke("moushi", "male", 2, gsk.General, gsk.SkinId);
            room.DrawCards(ask_who, 1, "moushi");

            return false;
        }
    }

    //刘晔
    public class Polu : TriggerSkill
    {
        public Polu() : base("polu")
        {
            events = new List<TriggerEvent> { TriggerEvent.Damaged, TriggerEvent.TurnStart, TriggerEvent.GameStart };
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Alter;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.GameStart && base.Triggerable(player, room))
            {
                foreach (int id in Engine.GetEngineCards())
                {
                    WrappedCard real_card = Engine.GetRealCard(id);
                    if (real_card.Name == Catapult.ClassName && room.GetCard(id) == null)
                    {
                        room.AddNewCard(id);
                        break;
                    }
                }
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room))
            {
                if (triggerEvent == TriggerEvent.TurnStart && !player.HasWeapon("Catapult"))
                    return new TriggerStruct(Name, player);
                else if (triggerEvent == TriggerEvent.Damaged && !player.HasWeapon("Catapult"))
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name, true);
            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
            if (triggerEvent == TriggerEvent.TurnStart)
            {
                int catapult = -1;
                foreach (int id in room.RoomCards)
                {
                    WrappedCard card = room.GetCard(id);
                    if (card.Name == "Catapult")
                    {
                        catapult = id;
                        break;
                    }
                }
                if (catapult == -1) return false;

                room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);

                int equipped_id = -1;
                if (player.GetWeapon())
                    equipped_id = player.Weapon.Key;
                List<CardsMoveStruct> exchangeMove = new List<CardsMoveStruct>();
                if (equipped_id != -1)
                {
                    CardsMoveStruct move1 = new CardsMoveStruct(new List<int> { equipped_id }, player, Player.Place.PlaceTable,
                        new CardMoveReason(MoveReason.S_REASON_CHANGE_EQUIP, player.Name));
                    exchangeMove.Add(move1);
                    room.MoveCardsAtomic(exchangeMove, true);
                }
                CardsMoveStruct move2 = new CardsMoveStruct(new List<int> { catapult }, room.GetCardOwner(catapult), player, room.GetCardPlace(catapult),
                                      Place.PlaceEquip, new CardMoveReason(MoveReason.S_REASON_PUT, player.Name, Name, string.Empty));
                exchangeMove.Add(move2);
                room.MoveCardsAtomic(exchangeMove, true);

                LogMessage log = new LogMessage
                {
                    From = player.Name,
                    Type = "$Install",
                    Card_str = catapult.ToString()
                };
                room.SendLog(log);

                if (equipped_id != -1)
                {
                    if (room.GetCardPlace(equipped_id) == Place.PlaceTable)
                    {
                        CardsMoveStruct move3 = new CardsMoveStruct(new List<int> { equipped_id }, null, Player.Place.DiscardPile,
                           new CardMoveReason(MoveReason.S_REASON_CHANGE_EQUIP, player.Name));
                        room.MoveCardsAtomic(new List<CardsMoveStruct> { move3 }, true);
                    }
                }
            }
            else if (data is DamageStruct damage)
            {
                room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                room.DrawCards(player, 1, Name);
            }

            return false;
        }
    }

    public class ChoulueVS : ZeroCardViewAsSkill
    {
        public ChoulueVS() : base("choulue")
        {
            response_pattern = "@@choulue";
        }
        public override WrappedCard ViewAs(Room room, Player player)
        {
            WrappedCard card = new WrappedCard(player.GetTag(Name).ToString())
            {
                Skill = Name
            };
            return card;
        }
    }

    public class Choulue : TriggerSkill
    {
        public Choulue() : base("choulue")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart, TriggerEvent.Damaged };
            view_as_skill = new ChoulueVS();
            skill_type = SkillType.Attack;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (base.Triggerable(player, room) && triggerEvent == TriggerEvent.Damaged && data is DamageStruct damage && damage.Card != null)
            {
                FunctionCard fcard = Engine.GetFunctionCard(damage.Card.Name);
                if (!(fcard is DelayedTrick) && !(fcard is SkillCard))
                    player.SetTag(Name, damage.Card.Name);
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Play)
            {
                foreach (Player p in room.GetOtherPlayers(player))
                    if (!p.IsNude())
                        return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (!p.IsNude())
                    targets.Add(p);
            }

            Player victim = room.AskForPlayerChosen(player, targets, Name, "@choulue", true, true, info.SkillPosition);
            if (victim != null)
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                room.SetTag("choulue_target", victim);
                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player target = (Player)room.GetTag("choulue_target");
            room.RemoveTag("choulue_target");
            if (!target.IsNude())
            {
                room.SetTag("choulue_source", player);
                List<int> ids = room.AskForExchange(target, Name, 1, 0, "@choulue-give:" + player.Name, string.Empty, "..", string.Empty);
                room.RemoveTag("choulue_source");
                if (ids.Count > 0)
                {
                    ResultStruct result = target.Result;
                    result.Assist++;
                    target.Result = result;

                    room.ObtainCard(player, ref ids, new CardMoveReason(MoveReason.S_REASON_GIVE, target.Name, player.Name, Name, string.Empty), false);
                    if (player.ContainsTag(Name) && player.GetTag(Name) is string card_name && !string.IsNullOrEmpty(card_name))
                    {
                        WrappedCard card = new WrappedCard(card_name) { Skill = "_choulue" };
                        FunctionCard fcard = Engine.GetFunctionCard(card_name);
                        if (fcard != null && fcard.IsAvailable(room, player, card))
                        {
                            room.AskForUseCard(player, "@@choulue", "@choulue-use:::" + card_name, null, -1, FunctionCard.HandlingMethod.MethodUse, false, info.SkillPosition);
                        }
                    }
                }
            }

            return false;
        }
    }

    public class HengjiangJX : MasochismSkill
    {
        public HengjiangJX() : base("hengjiang_jx")
        {
            skill_type = SkillType.Defense;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room))
            {
                Player current = room.Current;
                if (current != null && current.Alive && current.Phase != Player.PlayerPhase.NotActive && data is DamageStruct damage)
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
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player current = room.Current;
            if (current != null && room.AskForSkillInvoke(player, Name, current, info.SkillPosition))
            {
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, current.Name);
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }
        public override void OnDamaged(Room room, Player target, DamageStruct damage, TriggerStruct info)
        {
            Player current = room.Current;
            if (current == null) return;
            room.AddPlayerMark(current, "@hengjiang");
            target.AddMark("HengjiangInvoke");
        }
    }
    public class HengjiangJXDraw : TriggerSkill
    {
        public HengjiangJXDraw() : base("#hengjiang_jx-draw")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardsMoveOneTime, TriggerEvent.EventPhaseChanging };
            frequency = Frequency.Compulsory;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == Player.PlayerPhase.NotActive)
            {
                List<Player> zangbas = new List<Player>();
                foreach (Player p in room.GetAllPlayers())
                {
                    if (p.GetMark("HengjiangInvoke") > 0)
                        zangbas.Add(p);
                }
                if (zangbas.Count > 0 && player.GetMark("@hengjiang") > 0 && !player.HasFlag("HengjiangDiscarded"))
                {
                    LogMessage log = new LogMessage
                    {
                        Type = "#HengjiangDraw",
                        From = player.Name,
                        To = new List<string>(),
                        Arg = "hengjiang"
                    };
                    foreach (Player p in zangbas)
                        log.To.Add(p.Name);
                    room.SendLog(log);
                }
            }
            else if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.From != null && move.From.Phase == PlayerPhase.Discard
                && (move.Reason.Reason & MoveReason.S_MASK_BASIC_REASON) == MoveReason.S_REASON_DISCARD && move.From.GetMark("@hengjiang") > 0)
            {
                move.From.SetFlags("HengjiangDiscarded");
            }
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> skill_list = new List<TriggerStruct>();

            if (triggerEvent == TriggerEvent.EventPhaseChanging && player != null && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
            {
                List<Player> zangbas = new List<Player>();
                foreach (Player p in room.GetAllPlayers())
                {
                    if (p.GetMark("HengjiangInvoke") > 0)
                        zangbas.Add(p);
                }
                if (zangbas.Count > 0 && player.GetMark("@hengjiang") > 0 && !player.HasFlag("HengjiangDiscarded"))
                    foreach (Player zangba in zangbas)
                        skill_list.Add(new TriggerStruct(Name, zangba));
            }
            return skill_list;
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.DrawCards(ask_who, ask_who.GetMark("HengjiangInvoke"), "hengjiang_jx");
            return false;
        }
    }
    public class HengjiangJXFail : TriggerSkill
    {
        public HengjiangJXFail() : base("#hengjiang_jx-fail")
        {
            events.Add(TriggerEvent.EventPhaseChanging);
        }
        public override int GetPriority() => -1;
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
            {
                if (player.GetMark("@hengjiang") > 0)
                {
                    room.SetPlayerMark(player, "@hengjiang", 0);
                }
                foreach (Player p in room.GetAllPlayers())
                    if (p.GetMark("HengjiangInvoke") > 0)
                        p.SetMark("HengjiangInvoke", 0);
            }
            return new List<TriggerStruct>();
        }
    }

    public class Liehou : ZeroCardViewAsSkill
    {
        public Liehou() : base("liehou")
        {
            skill_type = SkillType.Wizzard;
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(LiehouCard.ClassName);
        }
        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard(LiehouCard.ClassName) { Skill = Name };
        }
    }

    public class LiehouCard : SkillCard
    {
        public static string ClassName = "LiehouCard";
        public LiehouCard() : base(ClassName)
        {
        }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return to_select != Self && !to_select.IsKongcheng() && RoomLogic.InMyAttackRange(room, Self, to_select);
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From, target = card_use.To[0];
            List<int> ids = room.AskForExchange(target, "liehou", 1, 1, "@liehou-give:" + player.Name, string.Empty, ".", null);
            if (ids.Count > 0)
            {
                room.ObtainCard(player, ref ids, new CardMoveReason(MoveReason.S_REASON_GIVE, target.Name, player.Name, "liehou", string.Empty), false);
                if (player.Alive && !player.IsKongcheng())
                {
                    List<Player> targets = new List<Player>();
                    foreach (Player p in room.GetOtherPlayers(target))
                    {
                        if (p != player && RoomLogic.InMyAttackRange(room, player, p))
                            targets.Add(p);
                    }

                    List<int> hands = player.GetCards("h");
                    if (targets.Count > 0)
                    {
                        target.SetFlags("liehou");
                        bool give = !room.AskForYiji(player, hands, "liehou", false, false, false, 1, targets, null,
                            "@liehou", null, false, card_use.Card.SkillPosition);
                        target.SetFlags("-liehou");
                        if (!give)
                        {
                            Shuffle.shuffle(ref hands);
                            Shuffle.shuffle(ref targets);
                            List<int> to_give = new List<int> { hands[0] };
                            room.ObtainCard(targets[0], ref to_give, new CardMoveReason(MoveReason.S_REASON_GIVE, player.Name, targets[0].Name, "liehou", string.Empty), false);
                        }
                    }
                }
            }
        }
    }

    public class Qigong : TriggerSkill
    {
        public Qigong() : base("qigong")
        {
            events = new List<TriggerEvent> { TriggerEvent.SlashMissed, TriggerEvent.TargetChosen };
            skill_type = SkillType.Attack;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.TargetChosen && data is CardUseStruct use && use.Pattern == "Slash:qigong" && use.Card.Name.Contains(Slash.ClassName))
            {
                for (int i = 0; i < use.EffectCount.Count; i++)
                {
                    CardBasicEffect effect = use.EffectCount[i];
                    effect.Effect2 = 0;

                    LogMessage log = new LogMessage
                    {
                        Type = "#NoJink",
                        From = effect.To.Name
                    };
                    room.SendLog(log);
                }

                data = use;
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.SlashMissed && base.Triggerable(player, room))
            {
                return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            SlashEffectStruct effect = (SlashEffectStruct)data;
            player.SetFlags("slashTargetFix");
            player.SetFlags("slashTargetFixToOne");
            effect.To.SetFlags("SlashAssignee");
            WrappedCard slash = room.AskForUseCard(player, "Slash:qigong", "@qigong:" + effect.To.Name, null, -1, FunctionCard.HandlingMethod.MethodUse, false);
            if (slash == null)
            {
                player.SetFlags("-slashTargetFix");
                player.SetFlags("-slashTargetFixToOne");
                effect.To.SetFlags("-SlashAssignee");
            }

            return false;
        }
    }

    public class QigongTag : TargetModSkill
    {
        public QigongTag() : base("#qigong-target", false) { }
        public override bool GetDistanceLimit(Room room, Player from, Player to, WrappedCard card, CardUseStruct.CardUseReason reason, string pattern)
        {
            if (reason == CardUseStruct.CardUseReason.CARD_USE_REASON_RESPONSE_USE
                && (room.GetRoomState().GetCurrentResponseSkill() == "qigong" || pattern == "Slash:qigong"))
                return true;

            return false;
        }
    }
}
