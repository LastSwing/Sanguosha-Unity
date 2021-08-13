using CommonClass;
using CommonClass.Game;
using CommonClassLibrary;
using SanguoshaServer.Game;
using System;
using System.Collections.Generic;
using System.Threading;
using static CommonClass.Game.CardUseStruct;
using static CommonClass.Game.Player;
using static SanguoshaServer.Package.FunctionCard;

namespace SanguoshaServer.Package
{
    public class HegemonySpecial : GeneralPackage
    {
        public HegemonySpecial() : base("HegemonySpecial")
        {
            skills = new List<Skill>
            {
                new Daoshu(),
                new Weicheng(),
                new Quanjin(),
                new Zaoyun(),
                new ZaoyunDistance(),

                new Tunchu(),
                new TunchuAdd(),
                new TunchuProhibit(),
                new Shuliang(),
                new Qiao(),
                new Chengshang(),
                new JujianHegemony(),

                new Yuanyu(),
                new Guishu(),
                new WukuHegemony(),
                new ZhidaoHegemony(),
                new ZhidaoHegemonyPro(),
                new ZhidaoDistance(),
                new JiliYbhHegemony(),
                new KuangcaiHegemony(),
                new KuangcaiHegemonyMax(),
                new KuangcaiHegemonyTar(),
                new ShejianHegemony(),

                new Dujin(),
                new Zhente(),
                new Zhiwei(),

            };
            skill_cards = new List<FunctionCard>
            {
                new GuishuCard(),
                new DaoshuCard(),
                new JujianHCard(),
                new QuanjinCard(),
                new ZaoyunCard(),
            };
            related_skills = new Dictionary<string, List<string>>
            {
                { "tunchu", new List<string> { "#tunchu-add", "#tunchu-prohibit" } },
                { "zhidao_hegemony", new List<string> { "#zhidao_hegemony", "#zhidao-distance" } },
                { "zaoyun", new List<string> { "#zaoyun" } },
                { "kuangcai_hegemony", new List<string> { "#kuangcai_hegemony", "#kuangcai_hegemony-max" } },
            };
        }
    }

    //jianggan
    public class Weicheng : TriggerSkill
    {
        public Weicheng() : base("weicheng")
        {
            events.Add(TriggerEvent.CardsMoveOneTime);
            skill_type = SkillType.Replenish;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is CardsMoveOneTimeStruct move && move.To != null && move.From != null && base.Triggerable(move.From, room)
                && move.From_places.Contains(Place.PlaceHand) && (move.Reason.Reason == MoveReason.S_REASON_GIVE
                || move.Reason.Reason == MoveReason.S_REASON_EXTRACTION) && move.From.HandcardNum < move.From.Hp)
                return new TriggerStruct(Name, move.From);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player p, ref object data, Player player, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(player, Name, data, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player p, ref object data, Player player, TriggerStruct info)
        {
            room.DrawCards(player, 1, Name);
            return false;
        }
    }
    public class Daoshu : TriggerSkill
    {
        public Daoshu() : base("daoshu")
        {
            events.Add(TriggerEvent.EventPhaseChanging);
            skill_type = SkillType.Wizzard;
            view_as_skill = new DaoshuVS();
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (data is PhaseChangeStruct change && change.From == PlayerPhase.Play && player.HasFlag(Name))
                player.SetFlags("-daoshu");
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }
    public class DaoshuVS : ZeroCardViewAsSkill
    {
        public DaoshuVS() : base("daoshu")
        {
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasFlag(Name);
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            WrappedCard card = new WrappedCard(DaoshuCard.ClassName)
            {
                Skill = Name,
                ShowSkill = Name
            };
            return card;
        }
    }

    public class DaoshuCard : SkillCard
    {
        public static string ClassName = "DaoshuCard";
        private readonly List<string> suits = new List<string> { "spade", "heart", "club", "diamond" };
        public DaoshuCard() : base(ClassName)
        {
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return Self != to_select && targets.Count == 0 && !to_select.IsKongcheng() && RoomLogic.CanGetCard(room, Self, to_select, "h");
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            Player target = card_use.To[0];
            if (target.Alive && !target.IsKongcheng() && RoomLogic.CanGetCard(room, player, target, "h"))
            {
                target.SetFlags("daoshu_target");
                string suit = room.AskForChoice(player, "daoshu", string.Join("+", suits));
                target.SetFlags("-daoshu_target");

                int id = room.AskForCardChosen(player, target, "h", "daoshu", false, HandlingMethod.MethodGet);
                room.ObtainCard(player, id);

                if (WrappedCard.GetSuitString(room.GetCard(id).Suit) == suit)
                {
                    room.Damage(new DamageStruct("daoshu", player, target));
                }
                else
                {
                    suit = WrappedCard.GetSuitString(room.GetCard(id).Suit);
                    player.SetFlags("daoshu");
                    List<int> ids = new List<int>();
                    CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_GIVE, player.Name, target.Name, "daoshu", string.Empty);
                    foreach (int card_id in player.GetCards("h"))
                    {
                        if (WrappedCard.GetSuitString(room.GetCard(card_id).Suit) != suit)
                            ids.Add(card_id);
                    }

                    if (ids.Count == 0) room.ShowAllCards(player, null, "daoshu");
                    else if (ids.Count == 1)
                    {
                        room.ObtainCard(target, ref ids, reason, true);
                    }
                    else
                    {
                        List<int> to_give = room.AskForExchange(player, "daoshu", 1, 1, "@daoshu-give:" + target.Name, string.Empty, string.Format(".|^{0}|.|hand", suit), card_use.Card.SkillPosition);
                        if (to_give.Count == 1)
                            room.ObtainCard(target, ref to_give, reason, true);
                        else
                        {
                            List<int> give = new List<int> { ids[0] };
                            room.ObtainCard(target, ref give, reason, true);
                        }
                    }
                }
            }
        }
    }

    //dongzhao
    public class Quanjin : TriggerSkill
    {
        public Quanjin() : base("quanjin")
        {
            view_as_skill = new QuanjinVS();
            skill_type = SkillType.Wizzard;
            events = new List<TriggerEvent> { TriggerEvent.Damaged, TriggerEvent.EventPhaseChanging };
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.Damaged && player.Alive && room.Current != null && room.Current.Phase == PlayerPhase.Play)
                player.SetFlags(Name);
            else if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.From == PlayerPhase.Play)
            {
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (p.HasFlag(Name)) p.SetFlags("-quanjin");
                }
            }
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }
    public class QuanjinVS : OneCardViewAsSkill
    {
        public QuanjinVS() : base("quanjin") { filter_pattern = "."; }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(QuanjinCard.ClassName) && !player.IsKongcheng();
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard jieyue = new WrappedCard(QuanjinCard.ClassName) { Skill = Name, ShowSkill = Name };
            jieyue.AddSubCard(card);
            return jieyue;
        }
    }

    public class QuanjinCard : SkillCard
    {
        public static string ClassName = "QuanjinCard";
        public QuanjinCard() : base(ClassName) { will_throw = false; }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return to_select != Self && targets.Count == 0 && to_select.HasFlag("quanjin");
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            Player target = card_use.To[0];

            List<int> ids = new List<int>(card_use.Card.SubCards);
            room.ObtainCard(target, ref ids, new CardMoveReason(MoveReason.S_REASON_GIVE, player.Name, target.Name, "quanjin", string.Empty), false);

            if (player.Alive && target.Alive)
            {
                target.SetFlags("imperialorder_target");        //这个flag表明该玩家为敕令的固定目标，且会自动在ViewAsSkill生成随机敕令牌后清除
                player.SetTag("order_reason", "quanjin");
                WrappedCard card = room.AskForUseCard(player, "@@imperialorder!", "@quanjin-target:" + target.Name, null, -1, HandlingMethod.MethodUse);
                if (card == null)
                {
                    string card_name = player.ContainsTag("imperialorder_select") ? ((string)player.GetTag("imperialorder_select")).Split('+')[0] : string.Empty;
                    if (string.IsNullOrEmpty(card_name))
                        card = ImperialOrderVS.GetImperialOrders(room, player)[0];
                    else
                        card = new WrappedCard(card_name);

                    CardUseStruct use = new CardUseStruct(card, player, target);
                    room.UseCard(use);
                }
                player.RemoveTag("imperialorder_select");

                if (!player.ContainsTag("ImperialOrder") || !(bool)player.GetTag("ImperialOrder"))
                {
                    if (player.Alive && player.HandcardNum < 5)
                    {
                        int max = 0;
                        foreach (Player p in room.GetAlivePlayers())
                        {
                            if (p.HandcardNum > max) max = p.HandcardNum;
                        }
                        if (player.HandcardNum < max)
                        {
                            max = Math.Min(5, max + 1);
                            int count = max - player.HandcardNum;
                            room.DrawCards(player, count, "quanjin");
                        }
                    }
                }
                else if (player.Alive)
                    room.DrawCards(player, 1, Name);
            }
        }
    }

    public class Zaoyun : ViewAsSkill
    {
        public Zaoyun() : base("zaoyun")
        {
            skill_type = SkillType.Attack;
        }

        public override bool IsEnabledAtPlay(Room room, Player player) => !player.HasUsed(ZaoyunCard.ClassName);

        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return RoomLogic.CanDiscard(room, player, player, to_select.Id) && room.GetCardPlace(to_select.Id) == Place.PlaceHand;
        }

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count > 0)
            {
                WrappedCard zy = new WrappedCard(ZaoyunCard.ClassName) { Skill = Name, ShowSkill = Name };
                zy.AddSubCards(cards);
                return zy;
            }
            return null;
        }
    }

    public class ZaoyunCard : SkillCard
    {
        public static string ClassName = "ZaoyunCard";
        public ZaoyunCard() : base(ClassName)
        {
            will_throw = true;
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            if (targets.Count == 0 && to_select != Self && !RoomLogic.WillBeFriendWith(room, Self, to_select))
            {
                int distance = RoomLogic.DistanceTo(room, Self, to_select);
                return distance > 1 && distance - 1 == card.SubCards.Count;
            }
            return false;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From, target = card_use.To[0];
            player.SetFlags("zaoyun_from");
            target.SetFlags("zaoyun");
            room.Damage(new DamageStruct("zaoyun", player, target));
        }
    }

    public class ZaoyunDistance : DistanceSkill
    {
        public ZaoyunDistance() : base("#zaoyun")
        {
        }
        public override int GetFixed(Room room, Player from, Player to)
        {
            if (from.HasFlag("zaoyun_from") && to.HasFlag("zaoyun"))
                return 1;
            else
                return 0;
        }
    }

    //lifeng
    public class Tunchu : DrawCardsSkill
    {
        public Tunchu() : base("tunchu")
        {
            skill_type = SkillType.Replenish;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && player.Phase == PlayerPhase.Draw && player.GetPile("commissariat").Count == 0)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(player, Name, null, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                player.SetTag(Name, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }
        public override int GetDrawNum(Room room, Player player, int n)
        {
            player.SetFlags(Name);
            return n + 2;
        }
    }

    public class TunchuAdd : TriggerSkill
    {
        public TunchuAdd() : base("#tunchu-add")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventLoseSkill, TriggerEvent.AfterDrawNCards };
            frequency = Frequency.Compulsory;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventLoseSkill && data is InfoStruct info && info.Info == "tunchu")
                room.ClearOnePrivatePile(player, "commissariat");
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.AfterDrawNCards && player != null && player.Alive && player.HasFlag("tunchu"))
            {
                if (player.IsKongcheng())
                {
                    player.SetFlags("-tunchu");
                    return new TriggerStruct();
                }
                TriggerStruct trigger = new TriggerStruct(Name, player)
                {
                    SkillPosition = (string)player.GetTag("tunchu")
                };
                return trigger;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            player.SetFlags("-tunchu");
            player.RemoveTag("tunchu");

            List<int> ids = room.AskForExchange(player, "tunchu", player.HandcardNum, 0, "@tunchu", string.Empty, ".|.|.|hand", info.SkillPosition);
            if (ids.Count > 0) room.AddToPile(player, "commissariat", ids);

            return false;
        }
    }
    

    public class TunchuProhibit : ProhibitSkill
    {
        public TunchuProhibit() : base("#tunchu-prohibit")
        {
        }
        public override bool IsProhibited(Room room, Player from, Player to, WrappedCard card, List<Player> others = null)
        {
            if (from != null && from.GetPile("commissariat").Count > 0 && card != null && card.Name.Contains("Slash"))
            {
                CardUseReason reason = room.GetRoomState().GetCurrentCardUseReason();
                return reason == CardUseReason.CARD_USE_REASON_PLAY || reason == CardUseReason.CARD_USE_REASON_RESPONSE_USE;
            }

            return false;
        }
    }

    public class Shuliang : TriggerSkill
    {
        public Shuliang() : base("shuliang")
        {
            events.Add(TriggerEvent.EventPhaseStart);
            skill_type = SkillType.Replenish;
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> skill_list = new List<TriggerStruct>();
            if (player.Alive && player.Phase == PlayerPhase.Finish && player.HandcardNum < player.Hp)
            {
                List<Player> lifengs = RoomLogic.FindPlayersBySkillName(room, Name);
                foreach (Player p in lifengs)
                {
                    if (p.GetPile("commissariat").Count > 0)
                    {
                        TriggerStruct trigger = new TriggerStruct(Name, p);
                        skill_list.Add(trigger);
                    }
                }
            }
            return skill_list;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player lifeng, TriggerStruct info)
        {
            if (player.Alive && player.Phase == PlayerPhase.Finish && player.HandcardNum < player.Hp && lifeng.GetPile("commissariat").Count > 0)
            {
                List<int> ids = room.AskForExchange(lifeng, Name, 1, 0, "@shuliang:" + player.Name, "commissariat", string.Empty, info.SkillPosition);
                if (ids.Count > 0)
                {
                    CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_REMOVE_FROM_PILE, string.Empty, Name, Name);
                    room.ThrowCard(ref ids, reason, null);
                    room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, lifeng.Name, player.Name);
                    room.BroadcastSkillInvoke(Name, lifeng, info.SkillPosition);
                    return info;
                }
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (player.Alive)
                room.DrawCards(player, new DrawCardStruct(2, ask_who, Name));
            return false;
        }
    }

    //zongyu
    public class Qiao : TriggerSkill
    {
        public Qiao() : base("qiao")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseChanging, TriggerEvent.TargetConfirmed };
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
                foreach (Player p in room.GetAlivePlayers())
                    if (p.GetMark(Name) > 0) p.SetMark(Name, 0);
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.TargetConfirmed && base.Triggerable(player, room) && data is CardUseStruct use && use.From != null && use.From != player && use.From.Alive
                && !RoomLogic.WillBeFriendWith(room, player, use.From) && !use.From.IsAllNude() && RoomLogic.CanDiscard(room, player, use.From, "he") && player.GetMark(Name) < 2)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (!(fcard is SkillCard))
                    return new TriggerStruct(Name, player);
            }
            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use && room.AskForSkillInvoke(player, Name, use.From, info.SkillPosition))
            {
                player.AddMark(Name);
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, use.From.Name);
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use)
            {
                List<int> ids = new List<int> { room.AskForCardChosen(player, use.From, "he", Name, false, FunctionCard.HandlingMethod.MethodDiscard) };
                room.ThrowCard(ref ids, new CardMoveReason(MoveReason.S_REASON_DISMANTLE, player.Name, use.From.Name, Name, string.Empty), use.From, player);
                if (player.Alive && !player.IsNude() && RoomLogic.CanDiscard(room, player, player, "he"))
                    room.AskForDiscard(player, Name, 1, 1, false, true, "@qiao", false, info.SkillPosition);
            }

            return false;
        }
    }

    public class Chengshang : TriggerSkill
    {
        public Chengshang() : base("chengshang")
        {
            events= new List<TriggerEvent> { TriggerEvent.CardFinished, TriggerEvent.Damage };
            skill_type = SkillType.Replenish;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.Damage && data is DamageStruct damage && player.Phase == PlayerPhase.Play && base.Triggerable(player, room)
                && !player.HasFlag(Name) && damage.Card != null && damage.Card.Suit != WrappedCard.CardSuit.NoSuit && damage.Card.Number != 0)
                damage.Card.SetFlags(Name);
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is CardUseStruct use && use.Card.Suit != WrappedCard.CardSuit.NoSuit && use.Card.Number != 0 && !player.HasFlag(Name)
                && player.Phase == PlayerPhase.Play && !use.Card.HasFlag(Name))
            {
                bool diff = false;
                foreach (Player p in use.To)
                {
                    if (!RoomLogic.WillBeFriendWith(room, player, p))
                    {
                        diff = true;
                        break;
                    }
                }
                if (diff)
                {
                    foreach (int id in room.DrawPile)
                    {
                        WrappedCard card = room.GetCard(id);
                        if (card.Suit == use.Card.Suit && card.Number == use.Card.Number)
                        {
                            return new TriggerStruct(Name, player);
                        }
                    }
                }
            }
            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(player, Name, data, info.SkillPosition))
            {
                player.SetFlags(Name);
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use)
            {
                List<int> ids = new List<int>();
                foreach (int id in room.DrawPile)
                {
                    WrappedCard card = room.GetCard(id);
                    if (card.Suit == use.Card.Suit && card.Number == use.Card.Number)
                    {
                        ids.Add(id);
                    }
                }
                if (ids.Count > 0)
                {
                    foreach (int id in ids)
                    {
                        room.MoveCardTo(room.GetCard(id), player, Place.PlaceTable, new CardMoveReason(MoveReason.S_REASON_TURNOVER, player.Name, Name, null), false);
                        Thread.Sleep(200);
                    }
                    CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_GOTBACK, player.Name, Name, string.Empty);
                    room.ObtainCard(player, ref ids, reason, true);
                }
            }
            return false;
        }
    }

    //xushu
    public class JujianHCard : SkillCard
    {
        public static string ClassName = "JujianHCard";
        public JujianHCard() : base(ClassName)
        {
        }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && RoomLogic.WillBeFriendWith(room, to_select, Self);
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From, target = card_use.To[0];

            List<string> choicelist = new List<string> { "draw" };
            if (target.IsWounded())
                choicelist.Add("recover");
            string choice = room.AskForChoice(target, "jujian_hegemony", string.Join("+", choicelist));

            if (choice == "draw")
                room.DrawCards(target, new DrawCardStruct(2, player, "jujian_hegemony"));
            else if (choice == "recover")
            {
                RecoverStruct recover = new RecoverStruct
                {
                    Recover = 1,
                    Who = player
                };
                room.Recover(target, recover, true);
            }

            if (player.Alive && player.GetMark("jujian_hegemony") == 0 && target.Alive && RoomLogic.CanTransform(target) && room.AskForSkillInvoke(target, "transform"))
            {
                player.SetMark("jujian_hegemony", 1);
                room.TransformDeputyGeneral(target);
            }
        }
    }

    public class JujianHVS : OneCardViewAsSkill
    {
        public JujianHVS() : base("jujian_hegemony")
        {
            filter_pattern = "^BasicCard!";
            response_pattern = "@@jujian_hegemony";
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard jujianCard = new WrappedCard(JujianHCard.ClassName) { Skill = Name, ShowSkill = Name };
            jujianCard.AddSubCard(card);
            return jujianCard;
        }
    }

    public class JujianHegemony : PhaseChangeSkill
    {
        public JujianHegemony() : base("jujian_hegemony")
        {
            view_as_skill = new JujianHVS();
            relate_to_place = "deputy";
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player.Phase == PlayerPhase.Finish && base.Triggerable(player, room))
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.AskForUseCard(player, "@@jujian_hegemony", "@jujian", null, -1, HandlingMethod.MethodUse, true, info.SkillPosition);
            return new TriggerStruct();
        }

        public override bool OnPhaseChange(Room room, Player player, TriggerStruct info)
        {
            return false;
        }
    }

    //himiko
    public class Guishu : TriggerSkill
    {
        public Guishu() : base("guishu")
        {
            events.Add(TriggerEvent.EventPhaseChanging);
            skill_type = SkillType.Alter;
            view_as_skill = new GuishuVS();
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive && player.GetMark(Name) > 0)
                player.SetMark(Name, 0);
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }
    public class GuishuVS : ViewAsSkill
    {
        public GuishuVS() : base("guishu")
        {
            response_or_use = true;
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return true;
        }
        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return to_select.Suit == WrappedCard.CardSuit.Spade && selected.Count == 0 && room.GetCardPlace(to_select.Id) != Place.PlaceEquip;
        }

        public override List<WrappedCard> GetGuhuoCards(Room room, List<WrappedCard> cards, Player player)
        {
            List<WrappedCard> result = new List<WrappedCard>();
            if (cards.Count == 1)
            {
                foreach (FunctionCard fcard in room.AvailableFunctionCards)
                {
                    if (fcard.Name == BefriendAttacking.ClassName && (player.GetMark(Name) == 0 || player.GetMark(Name) == 1))
                    {

                        WrappedCard ba = new WrappedCard(BefriendAttacking.ClassName)
                        {
                            Skill = Name,
                            ShowSkill = Name,
                        };
                        ba.AddSubCards(cards);
                        result.Add(ba);
                    }

                    if (fcard.Name == KnownBoth.ClassName && (player.GetMark(Name) == 0 || player.GetMark(Name) == 2))
                    {
                        WrappedCard kb = new WrappedCard(KnownBoth.ClassName)
                        {
                            Skill = Name,
                            ShowSkill = Name,
                        };
                        kb.AddSubCards(cards);
                        result.Add(kb);
                    }
                }
            }

            return result;
        }

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count == 1 && cards[0].IsVirtualCard())
            {
                WrappedCard gs = new WrappedCard(GuishuCard.ClassName)
                {
                    Skill = Name,
                    ShowSkill = Name,
                    UserString = cards[0].Name
                };
                gs.AddSubCard(cards[0]);
                return gs;
            }

            return null;
        }
    }

    public class GuishuCard : SkillCard
    {
        public static string ClassName = "GuishuCard";
        public GuishuCard() : base(ClassName)
        {
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            WrappedCard ba = new WrappedCard(card.UserString);
            ba.AddSubCard(card);
            ba = RoomLogic.ParseUseCard(room, ba);
            FunctionCard bcard = Engine.GetFunctionCard(card.UserString);

            return bcard.TargetFilter(room, targets, to_select, Self, ba);
        }

        public override WrappedCard Validate(Room room, CardUseStruct use)
        {
            Player player = use.From;
            WrappedCard ba = new WrappedCard(use.Card.UserString)
            {
                Skill = "guishu",
                ShowSkill = "guishu",
                SkillPosition = use.Card.SkillPosition
            };
            ba.AddSubCard(use.Card);
            ba = RoomLogic.ParseUseCard(room, ba);

            if (ba.Name == BefriendAttacking.ClassName)
                player.SetMark("guishu", 2);
            else
                player.SetMark("guishu", 1);

            return ba;
        }
    }

    public class Yuanyu : TriggerSkill
    {
        public Yuanyu() : base("yuanyu")
        {
            events.Add(TriggerEvent.DamageInflicted);
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Defense;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is DamageStruct damage && damage.From != null && damage.From.Alive && damage.From != damage.To
                && room.GetNextAlive(damage.From) != player && room.GetNextAlive(player) != damage.From)
                return new TriggerStruct(Name, player);
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            bool invoke = RoomLogic.PlayerHasShownSkill(room, player, Name) ? true : room.AskForSkillInvoke(player, Name, data, info.SkillPosition);
            if (invoke)
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name, true);
            DamageStruct damage = (DamageStruct)data;
            LogMessage log = new LogMessage
            {
                Type = "#ReduceDamage",
                From = player.Name,
                Arg = Name,
                Arg2 = (--damage.Damage).ToString()
            };
            room.SendLog(log);

            if (damage.Damage < 1)
                return true;
            data = damage;

            return false;
        }
    }

    public class WukuHegemony : TriggerSkill
    {
        public WukuHegemony() : base("wuku_hegemony")
        {
            events.Add(TriggerEvent.CardUsed);
            frequency = Frequency.Compulsory;
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (data is CardUseStruct use)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is EquipCard)
                {
                    foreach (Player p in RoomLogic.FindPlayersBySkillName(room, Name))
                        if (!RoomLogic.WillBeFriendWith(room, p, player) && p.GetMark("wuku") < 2)
                            triggers.Add(new TriggerStruct(Name, p));
                }
            }
            return triggers;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (RoomLogic.PlayerHasShownSkill(room, ask_who, this) || room.AskForSkillInvoke(ask_who, Name, data, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(ask_who, Name);
            ask_who.AddMark("wuku");
            room.SetPlayerStringMark(ask_who, "wuku", ask_who.GetMark("wuku").ToString());
            return false;
        }
    }

    //yanbaihu
    public class ZhidaoHegemony : TriggerSkill
    {
        public ZhidaoHegemony() : base("zhidao_hegemony")
        {
            events = new List<TriggerEvent> { TriggerEvent.Damage, TriggerEvent.EventPhaseChanging, TriggerEvent.EventPhaseStart };
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Attack;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.Damage && player.Phase == PlayerPhase.Play && data is DamageStruct damage && damage.To.HasFlag(Name))
                player.AddMark(Name);
            else if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.From == PlayerPhase.Play && player.GetMark(Name) > 0)
                player.SetMark(Name, 0);

        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.Damage && data is DamageStruct damage && player.GetMark(Name) == 1 && damage.To.Alive && !damage.To.IsAllNude()
                 && damage.To.HasFlag(Name) && base.Triggerable(player, room) && RoomLogic.CanGetCard(room, player, damage.To, "hej") && damage.To != player)
            {
                return new TriggerStruct(Name, player);
            }
            else if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Play && base.Triggerable(player, room))
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart)
            {
                bool option = !RoomLogic.PlayerHasShownSkill(room, player, Name);
                string prompt = option ? "@zhidao-hegemony" : "@zhidao-hegemony2";
                Player target = room.AskForPlayerChosen(player, room.GetOtherPlayers(player), Name, prompt, option, true, info.SkillPosition);
                if (target != null)
                {
                    room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                    room.SetTag(Name, target);
                }
                else
                    return new TriggerStruct();
            }

            return info;
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is DamageStruct damage)
            {
                Player target = damage.To;
                room.SendCompulsoryTriggerLog(player, Name);
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, target.Name);
                List<int> ids = new List<int>();
                if (!target.IsKongcheng() && RoomLogic.CanGetCard(room, player, target, "h"))
                {
                    int id = room.AskForCardChosen(player, target, "h", Name, false, HandlingMethod.MethodGet);
                    ids.Add(id);
                }
                if (target.HasEquip() && RoomLogic.CanGetCard(room, player, target, "e"))
                {
                    int id = room.AskForCardChosen(player, target, "e", Name, false, HandlingMethod.MethodGet);
                    ids.Add(id);
                }
                if (target.JudgingArea.Count > 0 && RoomLogic.CanGetCard(room, player, target, "j"))
                {
                    int id = room.AskForCardChosen(player, target, "j", Name, false, HandlingMethod.MethodGet);
                    ids.Add(id);
                }
                if (ids.Count > 0)
                    room.ObtainCard(player, ref ids, new CardMoveReason(MoveReason.S_REASON_EXTRACTION, player.Name, target.Name, Name, string.Empty), false);

            }
            else if (room.GetTag(Name) is Player target)
            {
                room.RemoveTag(Name);
                player.SetFlags(Name);
                target.SetFlags(Name);
            }
            return false;
        }
    }

    public class ZhidaoHegemonyPro : ProhibitSkill
    {
        public ZhidaoHegemonyPro() : base("#zhidao_hegemony") { }

        public override bool IsProhibited(Room room, Player from, Player to, WrappedCard card, List<Player> others = null)
        {
            if (from != null && Engine.GetFunctionCard(card.Name).TypeID != CardType.TypeSkill && from.HasFlag("zhidao_hegemony") && from.Phase != PlayerPhase.NotActive)
                return from != to && to != null && !to.HasFlag("zhidao_hegemony");

            return false;
        }
    }

    public class ZhidaoDistance : DistanceSkill
    {
        public ZhidaoDistance() : base("#zhidao-distance")
        {
        }
        public override int GetFixed(Room room, Player from, Player to)
        {
            if (from.HasFlag("zhidao_hegemony") && from.Phase != PlayerPhase.NotActive && to.HasFlag("zhidao_hegemony"))
                return 1;
            else
                return 0;
        }
    }

    public class JiliYbhHegemony : TriggerSkill
    {
        public JiliYbhHegemony() : base("jili_ybh_hegemony")
        {
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Wizzard;
            events = new List<TriggerEvent> { TriggerEvent.DamageInflicted, TriggerEvent.CardFinished, TriggerEvent.Damaged, TriggerEvent.EventPhaseChanging };
            relate_to_place = "deputy";
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.Damaged && player.Alive)
                player.AddMark(Name);
            else if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
                foreach (Player p in room.GetAlivePlayers())
                    if (p.GetMark(Name) > 0) p.SetMark(Name, 0);
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.CardFinished && data is CardUseStruct use && use.To.Count == 1 && base.Triggerable(use.To[0], room) && use.Card.Name != Jink.ClassName
                && use.From != null && use.From.Alive && WrappedCard.IsRed(use.Card.Suit))
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is BasicCard || (fcard is TrickCard && !(fcard is DelayedTrick)))
                {
                    WrappedCard card = new WrappedCard(use.Card.Name);
                    if (fcard.IsAvailable(room, use.From, card) && (fcard.TargetFixed(card) || RoomLogic.IsProhibited(room, use.From, use.To[0], card) == null))
                        return new TriggerStruct(Name, use.To[0]);
                }
            }
            else if (triggerEvent == TriggerEvent.DamageInflicted && base.Triggerable(player, room) && player.GetMark(Name) == 1)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.DamageInflicted && (RoomLogic.PlayerHasShownSkill(room, player, Name) || room.AskForSkillInvoke(player, Name, "@jili-remove", info.SkillPosition)))
                return info;
            else if (triggerEvent == TriggerEvent.CardFinished && data is CardUseStruct use
                && (RoomLogic.PlayerHasShownSkill(room, ask_who, Name) || room.AskForSkillInvoke(ask_who, Name, string.Format("@jili-use:{0}::{1}", use.From.Name, use.Card.Name), info.SkillPosition)))
                return info;

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(ask_who, Name);
            room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
            if (triggerEvent == TriggerEvent.DamageInflicted)
            {
                room.RemoveGeneral(ask_who, false);
                return true;
            }
            else if (data is CardUseStruct use)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                WrappedCard card = new WrappedCard(use.Card.Name);
                room.UseCard(new CardUseStruct(card, use.From, fcard.TargetFixed(card) ? new List<Player>() : new List<Player> { ask_who }));
            }
            return false;
        }
    }

    public class KuangcaiHegemony : TriggerSkill
    {
        public KuangcaiHegemony() : base("kuangcai_hegemony")
        {
            frequency = Frequency.Compulsory;
            events = new List<TriggerEvent> { TriggerEvent.DamageDone, TriggerEvent.CardUsedAnnounced };
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardUsedAnnounced && data is CardUseStruct use && player.Phase != PlayerPhase.NotActive && !player.HasFlag(Name))
                player.SetFlags("kuangcai_use");
            else if (triggerEvent == TriggerEvent.DamageDone && data is DamageStruct damage && damage.From != null && damage.From.Phase != PlayerPhase.NotActive)
                damage.From.SetFlags(Name);
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data) => new List<TriggerStruct>();
    }

    //miheng
    public class KuangcaiHegemonyTar : TargetModSkill
    {
        public KuangcaiHegemonyTar() : base("#kuangcai_hegemony", true)
        {
            pattern = "BasicCard#TrickCard";
        }

        public override int GetResidueNum(Room room, Player from, WrappedCard card)
        {
            if (RoomLogic.PlayerHasShownSkill(room, from, "kuangcai_hegemony"))
                return 1000;
            else
                return 0;
        }

        public override bool GetDistanceLimit(Room room, Player from, Player to, WrappedCard card, CardUseReason reason, string pattern)
        {
            if (to != null && RoomLogic.PlayerHasShownSkill(room, from, "kuangcai_hegemony") && from.Phase != PlayerPhase.NotActive)
                return true;

            return false;
        }
    }

    public class KuangcaiHegemonyMax : MaxCardsSkill
    {
        public KuangcaiHegemonyMax() : base("#kuangcai_hegemony-max") { }
        public override int GetExtra(Room room, Player target)
        {
            return RoomLogic.PlayerHasShownSkill(room, target, "kuangcai_hegemony") && !target.HasFlag("kuangcai_hegemony") && target.HasFlag("kuangcai_use") ? -1 : 0;
        }
    }

    public class ShejianHegemony : TriggerSkill
    {
        public ShejianHegemony() : base("shejian_hegemony")
        {
            events.Add(TriggerEvent.TargetConfirmed);
            skill_type = SkillType.Attack;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is CardUseStruct use && use.To.Count == 1 && base.Triggerable(player, room) && !player.IsKongcheng() && use.From != null && use.From.Alive
                && use.From != player && RoomLogic.CanDiscard(room, player, player, "h"))
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is BasicCard || fcard is TrickCard)
                    return new TriggerStruct(Name, player);
            }
            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use && room.AskForSkillInvoke(player, Name, "@shejian_hegemony:" + use.From.Name, info.SkillPosition))
            {
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, use.From.Name);
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);

                List<int> ids = room.ForceToDiscard(player, player.GetCards("h"), player.GetCardCount(false), true);
                room.ThrowCard(ref ids, new CardMoveReason(MoveReason.S_REASON_THROW, player.Name, Name, string.Empty), player, null, Name);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use && use.From.Alive)
                room.Damage(new DamageStruct(Name, player, use.From));
            return false;
        }
    }

    //lincao
    public class Dujin : DrawCardsSkill
    {
        public Dujin() : base("dujin")
        {
            skill_type = SkillType.Replenish;
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
        public override int GetDrawNum(Room room, Player player, int n)
        {
            int m = player.GetEquips().Count / 2 + 1;
            return n + m;
        }
    }


    public class Zhente : TriggerSkill
    {
        public Zhente() : base("zhente")
        {
            events= new List<TriggerEvent> { TriggerEvent.EventPhaseChanging, TriggerEvent.TargetConfirmed, TriggerEvent.Death };
            skill_type = SkillType.Defense;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            bool clear = false;
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
            {
                foreach (Player p in room.GetAlivePlayers())
                    if (p.HasFlag(Name))
                        p.SetFlags("-zhente");

                if (player.HasFlag("ZhenteTarget"))
                    clear = true;
            }

            if (triggerEvent == TriggerEvent.Death && player.HasFlag("ZhenteTarget"))
                clear = true;

            if (clear)
            {
                RoomLogic.RemovePlayerCardLimitation(player, Name);
                room.SetPlayerMark(player, "@qianxi_black", 0);
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.TargetConfirmed && base.Triggerable(player, room) && data is CardUseStruct use && use.To.Contains(player)
                && !player.HasFlag(Name) && use.From != null && use.From != player && use.From.Alive)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if ((fcard is BasicCard || fcard.IsNDTrick()) && WrappedCard.IsBlack(use.Card.Suit))
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(player, Name, data, info.SkillPosition))
            {
                player.SetFlags(Name);
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player liushan, ref object data, Player ask_who, TriggerStruct info)
        {
            CardUseStruct use = (CardUseStruct)data;

            int index = 0, i;
            for (i = 0; i < use.EffectCount.Count; i++)
            {
                CardBasicEffect effect = use.EffectCount[i];
                if (effect.To == liushan)
                {
                    index++;
                    if (index == info.Times)
                    {
                        use.From.SetTag(Name, i);
                        break;
                    }
                }
            }

            if (room.AskForChoice(use.From, Name, "use+nulli", new List<string> { "@to-player:" + liushan.Name }, data) == "nulli")
            {
                CardBasicEffect effect = use.EffectCount[i];
                effect.Nullified = true;

                data = use;
            }
            else
            {
                string color = "black";
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, liushan.Name, use.From.Name);

                string pattern = string.Format(".|{0}|.|hand$0", color);
                use.From.SetFlags("ZhenteTarget");
                room.AddPlayerMark(use.From, "@qianxi_" + color);
                RoomLogic.SetPlayerCardLimitation(use.From, Name, "use", pattern, false);

                LogMessage log = new LogMessage
                {
                    Type = "#NoColor",
                    From = use.From.Name,
                    Arg = "no_suit_" + color,
                    Arg2 = Name
                };
                room.SendLog(log);
            }

            use.From.RemoveTag(Name);

            return false;
        }
    }

    public class Zhiwei : TriggerSkill
    {
        public Zhiwei() : base("zhiwei")
        {
            events = new List<TriggerEvent> { TriggerEvent.GeneralShown, TriggerEvent.Damage, TriggerEvent.Damaged, TriggerEvent.Death,
                TriggerEvent.CardsMoveOneTime, TriggerEvent.EventPhaseEnd };
            frequency = Frequency.Compulsory;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.From != null && move.From.Phase == PlayerPhase.Discard
                && move.From == room.Current && (move.Reason.Reason & MoveReason.S_MASK_BASIC_REASON) == MoveReason.S_REASON_DISCARD && base.Triggerable(move.From, room))
            {
                if (move.From.ContainsTag(Name) && move.From.GetTag(Name) is string target_name)
                {
                    Player target = room.FindPlayer(target_name);
                    if (target != null)
                    {
                        List<int> guzhengToGet = move.From.ContainsTag("zhiwei_give") ? (List<int>)move.From.GetTag("zhiwei_give") : new List<int>();
                        foreach (int card_id in move.Card_ids)
                        {
                            if (!guzhengToGet.Contains(card_id))
                                guzhengToGet.Add(card_id);
                        }

                        move.From.SetTag("zhiwei_give", guzhengToGet);
                    }
                }
            }
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.GeneralShown)
            {
                if (base.Triggerable(player, room) && !player.ContainsTag(Name))
                {
                    bool head = (bool)data;
                    if (head && RoomLogic.InPlayerHeadSkills(player, Name))
                    {
                        TriggerStruct trigger = new TriggerStruct(Name, player)
                        {
                            SkillPosition = "head"
                        };
                        triggers.Add(trigger);
                    }
                    else if (!head && RoomLogic.InPlayerDeputykills(player, Name))
                    {
                        TriggerStruct trigger = new TriggerStruct(Name, player)
                        {
                            SkillPosition = "deputy"
                        };
                        triggers.Add(trigger);
                    }
                }
            }
            else if ((triggerEvent == TriggerEvent.Damage || triggerEvent == TriggerEvent.Damaged) && player.ContainsTag("zhiwei_from")
                && player.GetTag("zhiwei_from") is List<string> froms)
            {
                foreach (string name in froms)
                {
                    Player p = room.FindPlayer(name);
                    if (p != null && RoomLogic.PlayerHasShownSkill(room, p, Name))
                        triggers.Add(new TriggerStruct(Name, p));
                }
            }
            else if (triggerEvent == TriggerEvent.Death && player.ContainsTag("zhiwei_from")
                && player.GetTag("zhiwei_from") is List<string> _froms)
            {
                foreach (string name in _froms)
                {
                    Player p = room.FindPlayer(name);
                    if (p != null)
                        triggers.Add(new TriggerStruct(Name, p));
                }
            }
            else if (triggerEvent == TriggerEvent.EventPhaseEnd && player.Phase == PlayerPhase.Discard && player.ContainsTag("zhiwei_give"))
                triggers.Add(new TriggerStruct(Name, player));

            return triggers;
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.GeneralShown)
            {
                Player target = room.AskForPlayerChosen(player, room.GetOtherPlayers(player), Name, "@zhiwei-choose", false, true, info.SkillPosition);
                player.SetTag(Name, target.Name);
                List<string> zhiwei = new List<string>();
                if (target.ContainsTag("zhiwei_from")) zhiwei = (List<string>)target.GetTag("zhiwei_from");
                if (!zhiwei.Contains(player.Name)) zhiwei.Add(player.Name);
                target.SetTag("zhiwei_from", zhiwei);
            }
            else if (triggerEvent == TriggerEvent.Damaged && !ask_who.IsKongcheng())
            {
                List<int> ids = new List<int>();
                foreach (int id in ask_who.GetCards("h"))
                {
                    if (RoomLogic.CanDiscard(room, ask_who, ask_who, id))
                        ids.Add(id);
                }

                if (ids.Count > 0)
                {
                    Shuffle.shuffle(ref ids);
                    List<int> discard = new List<int> { ids[0] };
                    room.ThrowCard(ref discard, new CardMoveReason(MoveReason.S_REASON_THROW, ask_who.Name, Name, string.Empty), ask_who, ask_who);
                }
            }
            else if (triggerEvent == TriggerEvent.Damage)
            {
                room.DrawCards(ask_who, 1, Name);
            }
            else if (triggerEvent == TriggerEvent.Death)
            {
                ask_who.RemoveTag(Name);
                if (ask_who.HasShownAllGenerals() && RoomLogic.PlayerHasShownSkill(room, ask_who, Name))
                    room.HideGeneral(ask_who, RoomLogic.InPlayerHeadSkills(ask_who, Name));
            }
            else if (triggerEvent == TriggerEvent.EventPhaseEnd && ask_who.ContainsTag(Name) && ask_who.GetTag(Name) is string target_name)
            {
                Player target = room.FindPlayer(target_name);
                if (base.Triggerable(player, room) && target != null && player.GetTag("zhiwei_give") is List<int> guzhengToGet)
                {
                    guzhengToGet.RemoveAll(t => room.GetCardPlace(t) != Place.DiscardPile);
                    if (guzhengToGet.Count > 0)
                    {
                        room.ObtainCard(target, ref guzhengToGet, new CardMoveReason(MoveReason.S_REASON_RECYCLE, player.Name, target.Name, Name, string.Empty));
                    }
                }
                player.RemoveTag("zhiwei_give");
            }

            return false;
        }
    }
}