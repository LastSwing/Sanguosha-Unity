using CommonClass;
using CommonClass.Game;
using CommonClassLibrary;
using SanguoshaServer.Game;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using static CommonClass.Game.CardUseStruct;
using static CommonClass.Game.Player;
using static SanguoshaServer.Package.FunctionCard;

namespace SanguoshaServer.Package
{
    public class Myth : GeneralPackage
    {
        public Myth() : base("Myth")
        {
            skills = new List<Skill>
            {
                new Wushen(),
                new WushenTar(),
                new Wuhun(),
                new Qinyin(),
                new QinyinClear(),
                new Yeyan(),
                new Qixing(),
                new QixingClear(),
                new Kuangfeng(),
                new KuangfengDamage(),
                new Dawu(),
                new DawuDamage(),
                new Guixin(),
                new Juejing(),
                new JuejingMax(),
                new Longhun(),
                new Wuqian(),
                new Wumou(),
                new Kuangbao(),
                new Shenfen(),
                new Renjie(),
                new Lianpo(),
                new Baiyin(),
                new Jilue(),
                new Longnu(),
                new LongnuTar(),
                new Jieying(),
                new JieyingMax(),
                new Poxi(),
                new PoxiMax(),
                new JieyingGN(),
                new JieyingGNMax(),
                new JieyingGNTar(),
                new JieyingDraw(),
                new JieyingDetach(),
                new Junlue(),
                new Cuike(),
                new Zhanhuo(),
                new Shenfu(),
                new Qixian(),
                new Chuyuan(),
                //new ChuyuanMax(),
                new Dengji(),
                new Tianxing(),
                new Duorui(),
                new DuoruiInvalid(),
                new Zhiti(),
                new ZhitiMax(),
            };
            skill_cards = new List<FunctionCard>
            {
                new YeyanCard(),
                new KuangfengCard(),
                new DawuCard(),
                new WuqianCard(),
                new ShenfenCard(),
                new JilueCard(),
                new QiangxiJXCard(),
                new PoxiCard(),
                new ZhanhuoCard(),
            };
            related_skills = new Dictionary<string, List<string>>
            {
                { "wushen", new List<string>{ "#wushen-target" } },
                { "qinyin", new List<string>{ "#qinyin-clear" } },
                { "kuangfeng", new List<string>{ "#kuangfeng" } },
                { "dawu", new List<string>{ "#dawu" } },
                { "shuangxiong_jx", new List<string>{ "#shuangxiong_jx-get" } },
                { "juejing", new List<string>{ "#juejing-max" } },
                { "hushen", new List<string>{ "#huashen-clear" } },
                { "canyuan", new List<string>{ "#canyuan" } },
                { "longnu", new List<string>{ "#longnu-tar" } },
                { "jieying", new List<string>{ "#jieying-max" } },
                { "poxi", new List<string>{ "#poxi" } },
                { "jieying_gn", new List<string>{ "#jieying_gn-max", "#jieying_gn-tar", "#jieying-draw", "#jieying_gn-clear" } },
                { "qixing", new List<string>{ "#qixing-clear" } },
                //{ "chuyuan", new List<string>{ "#chuyuan" } },
                { "duorui", new List<string>{ "#duorui" } },
                { "zhiti", new List<string>{ "#zhiti" } },
            };
        }
    }

    public class WushenFilter : FilterSkill
    {
        public WushenFilter() : base("wushen")
        {
        }

        public override bool ViewFilter(Room room, WrappedCard to_select, Player player)
        {
            return to_select.Suit == WrappedCard.CardSuit.Heart && room.GetCardPlace(to_select.Id) == Place.PlaceHand;
        }
        public override void ViewAs(Room room, ref RoomCard card, Player player)
        {
            card.ChangeName(Slash.ClassName);
        }
    }

    public class Wushen : TriggerSkill
    {
        public Wushen() : base("wushen")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardUsedAnnounced, TriggerEvent.CardResponded, TriggerEvent.TargetChosen };
            frequency = Frequency.Compulsory;
            view_as_skill = new WushenFilter();
            skill_type = SkillType.Alter;
        }
        public override bool CanPreShow() => true;
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.CardUsedAnnounced && data is CardUseStruct use && use.Card.Name == Slash.ClassName && use.Card.Skill == Name)
                return new TriggerStruct(Name, player);
            else if (triggerEvent == TriggerEvent.CardResponded && data is CardResponseStruct resp && resp.Card.Name == Slash.ClassName && resp.Card.Skill == Name)
                return new TriggerStruct(Name, player);
            else if (triggerEvent == TriggerEvent.TargetChosen && base.Triggerable(player, room) && data is CardUseStruct _use && _use.Card != null)
            {
                FunctionCard fcard = Engine.GetFunctionCard(_use.Card.Name);
                if (fcard is Slash && RoomLogic.GetCardSuit(room, _use.Card) == WrappedCard.CardSuit.Heart)
                {
                    List<Player> targets = new List<Player>(_use.To);
                    return new TriggerStruct(Name, player, targets);
                }
            }

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.CardUsedAnnounced || triggerEvent == TriggerEvent.CardResponded)
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            else if (triggerEvent == TriggerEvent.TargetChosen)
                return info;

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player target, ref object data, Player ask_who, TriggerStruct info)
        {
            room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
            room.SendCompulsoryTriggerLog(ask_who, Name, true);
            CardUseStruct use = (CardUseStruct)data;
            LogMessage log = new LogMessage
            {
                Type = "#NoJink",
                From = target.Name
            };
            room.SendLog(log);

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
                        data = use;
                        break;
                    }
                }
            }

            return false;
        }

        public override void GetEffectIndex(Room room, Player player, WrappedCard card, ref int index, ref string skill_name, ref string general_name, ref int skin_id)
        {
            index = -2;
        }
    }

    public class WushenTar : TargetModSkill
    {
        public WushenTar() : base("#wushen-target") { }
        public override bool GetDistanceLimit(Room room, Player from, Player to, WrappedCard card, CardUseReason reason, string pattern)
        {
            if (RoomLogic.PlayerHasSkill(room, from, "wushen") && RoomLogic.GetCardSuit(room, card) == WrappedCard.CardSuit.Heart)
                return true;

            return false;
        }

        public override bool CheckSpecificAssignee(Room room, Player from, Player to, WrappedCard card, string pattern)
        {
            if (RoomLogic.PlayerHasSkill(room, from, "wushen") && card.Name.Contains(Slash.ClassName) && RoomLogic.GetCardSuit(room, card) == WrappedCard.CardSuit.Heart)
                return true;

            return false;
        }

        public override void GetEffectIndex(Room room, Player player, WrappedCard card, ModType type, ref int index, ref string skill_name, ref string general_name, ref int skin_id)
        {
            index = -2;
        }
    }

    public class Wuhun : TriggerSkill
    {
        public Wuhun() : base("wuhun")
        {
            events = new List<TriggerEvent> { TriggerEvent.Death, TriggerEvent.Damaged };
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Wizzard;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.Damaged && base.Triggerable(player, room) && data is DamageStruct damage && damage.From != null && damage.From.Alive)
                return new TriggerStruct(Name, player);
            else if (triggerEvent == TriggerEvent.Death && RoomLogic.PlayerHasSkill(room, player, Name))
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
            if (triggerEvent == TriggerEvent.Damaged && data is DamageStruct damage)
            {
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, damage.From.Name);
                room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                return info;
            }
            else
            {
                int count = 0;
                List<Player> targets = new List<Player>();
                foreach (Player p in room.GetOtherPlayers(player))
                {
                    if (p.GetMark("@night") > count)
                        count = p.GetMark("@night");
                }
                if (count == 0) return new TriggerStruct();

                foreach (Player p in room.GetOtherPlayers(player))
                    if (p.GetMark("@night") == count)
                        targets.Add(p);

                if (targets.Count > 0)
                {
                    Player target = room.AskForPlayerChosen(player, targets, Name, "@wuhun", false, true, info.SkillPosition);
                    player.SetTag(Name, target.Name);
                    room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);
                    return info;
                }
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name);

            if (triggerEvent == TriggerEvent.Damaged && data is DamageStruct damage)
            {
                room.AddPlayerMark(damage.From, "@night", damage.Damage);
            }
            else if (triggerEvent == TriggerEvent.Death)
            {
                Player target = room.FindPlayer(player.GetTag(Name).ToString());
                player.RemoveTag(Name);

                JudgeStruct judge = new JudgeStruct
                {
                    Who = target,
                    Pattern = "Peach#GodSalvation",
                    Good = false,
                    PlayAnimation = true,
                    Reason = Name,
                    Negative = true
                };

                room.Judge(ref judge);

                if (judge.IsEffected() && target.Alive)
                {
                    target.Hp = 0;
                    room.BroadcastProperty(target, "Hp");
                    room.KillPlayer(target, new DamageStruct());
                }
            }

            return false;
        }
    }

    public class Qinyin : TriggerSkill
    {
        public Qinyin() : base("qinyin")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardsMoveOneTime, TriggerEvent.EventPhaseEnd };
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.From != null && move.From.Phase == PlayerPhase.Discard
                && base.Triggerable(move.From, room) && move.From_places.Contains(Place.PlaceHand) && move.Reason.PlayerId == move.From.Name
                && (move.Reason.Reason & MoveReason.S_MASK_BASIC_REASON) == MoveReason.S_REASON_DISCARD)
                foreach (Place place in move.From_places)
                    if (place == Place.PlaceHand)
                        move.From.AddMark(Name);
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseEnd && base.Triggerable(player, room) && player.Phase == PlayerPhase.Discard && player.GetMark(Name) > 1)
            {
                return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player lengtong, TriggerStruct info)
        {
            List<string> choices = new List<string> { "lose" };
            foreach (Player p in room.GetAlivePlayers())
            {
                if (p.IsWounded())
                {
                    choices.Add("recover");
                    break;
                }
            }
            choices.Add("cancel");

            string choice = room.AskForChoice(player, Name, string.Join("+", choices));
            if (choice != "cancel")
            {
                player.SetTag(Name, choice);
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                room.BroadcastSkillInvoke(Name, "male", choice == "recover" ? 1 : 2, gsk.General, gsk.SkinId);
                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player lengtong, TriggerStruct info)
        {
            string choice = player.GetTag(Name).ToString();
            List<Player> targets = room.GetAlivePlayers();
            room.SortByActionOrder(ref targets);
            if (choice == "recover")
            {
                RecoverStruct recover = new RecoverStruct
                {
                    Recover = 1,
                    Who = player
                };
                foreach (Player p in targets)
                    room.Recover(p, recover, true);
            }
            else
            {
                foreach (Player p in targets)
                    room.LoseHp(p);
            }

            return false;
        }
    }

    public class QinyinClear : TriggerSkill
    {
        public QinyinClear() : base("#qinyin-clear")
        {
            events.Add(TriggerEvent.EventPhaseEnd);
        }

        public override int GetPriority()
        {
            return 0;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseEnd && player.Phase == PlayerPhase.Discard && player.GetMark("qinyin") > 0)
                player.SetMark("qinyin", 0);
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class Yeyan : ViewAsSkill
    {
        public Yeyan() : base("yeyan")
        {
            limit_mark = "@fire";
            frequency = Frequency.Limited;
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return player.GetMark(limit_mark) > 0;
        }

        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            if (selected.Count > 3 || room.GetCardPlace(to_select.Id) != Place.PlaceHand || !RoomLogic.CanDiscard(room, player, player, to_select.Id))
                return false;

            if (selected.Count > 0)
            {
                List<WrappedCard.CardSuit> suits = new List<WrappedCard.CardSuit>();
                foreach (WrappedCard card in selected)
                    suits.Add(card.Suit);

                return !suits.Contains(to_select.Suit);
            }

            return true;
        }

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count == 0 || cards.Count == 4)
            {
                WrappedCard yy = new WrappedCard(YeyanCard.ClassName) { Skill = Name };
                yy.AddSubCards(cards);
                return yy;
            }

            return null;
        }
    }

    public class YeyanCard : SkillCard
    {
        public static string ClassName = "YeyanCard";
        public YeyanCard() : base(ClassName)
        {
            votes = true;
            will_throw = true;
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            if (targets.Count >= 3) return false;
            if (card.SubCards.Count == 0)
                return !targets.Contains(to_select);
            else if (card.SubCards.Count == 4 && targets.Count == 2)
                return targets.Contains(to_select) || targets[0] == targets[1];

            return true;
        }

        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            if (targets.Count == 0) return false;
            if (card.SubCards.Count == 0)
                return targets.Count <= 3;
            else if (card.SubCards.Count == 4 && targets.Count > 1 && targets.Count <= 3)
            {
                for (int i = 0; i < targets.Count; i++)
                {
                    Player p1 = targets[i];
                    for (int x = i + 1; x < targets.Count; x++)
                    {
                        Player p2 = targets[x];
                        if (p2 == p1)
                            return true;
                    }
                }
            }

            return false;
        }

        public override void OnUse(Room room, CardUseStruct card_use)
        {
            room.SetPlayerMark(card_use.From, "@fire", 0);
            room.BroadcastSkillInvoke("yeyan", card_use.From, card_use.Card.SkillPosition);
            room.DoSuperLightbox(card_use.From, card_use.Card.SkillPosition, "yeyan");

            List<Player> targets = new List<Player>();
            for (int i = 0; i < card_use.To.Count; i++)
            {
                int count = 1;
                Player p1 = card_use.To[i];
                if (targets.Contains(p1)) continue;
                for (int x = i + 1; x < card_use.To.Count; x++)
                {
                    Player p2 = card_use.To[x];
                    if (p2 == p1)
                    {
                        count++;
                    }
                }

                p1.SetMark("yeyan", count);
                targets.Add(p1);
            }

            card_use.To = targets;
            card_use.Card.Mute = true;
            base.OnUse(room, card_use);
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            if (card_use.Card.SubCards.Count > 0)
                room.LoseHp(card_use.From, 3);

            base.Use(room, card_use);
        }

        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            Player player = effect.From, target = effect.To;
            int count = target.GetMark("yeyan");
            target.SetMark("yeyan", 0);

            if (target.Alive)
                room.Damage(new DamageStruct("yeyan", player, target, count, DamageStruct.DamageNature.Fire));
        }
    }

    public class Qixing : TriggerSkill
    {
        public Qixing() : base("qixing")
        {
            events = new List<TriggerEvent> { TriggerEvent.GameStart, TriggerEvent.EventPhaseEnd };
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.GameStart && base.Triggerable(player, room))
                return new TriggerStruct(Name, player);
            else if (triggerEvent == TriggerEvent.EventPhaseEnd && player.Phase == PlayerPhase.Draw && base.Triggerable(player, room)
                && player.HandcardNum > 0 && player.GetPile(Name).Count > 0)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.GameStart)
            {
                room.NotifySkillInvoked(player, Name);
                return info;
            }
            else if (room.AskForSkillInvoke(player, Name, null, info.SkillPosition))
            {
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            if (triggerEvent == TriggerEvent.GameStart)
            {
                List<int> ids = room.GetNCards(7, true);
                room.AddToPile(player, Name, ids, false, new List<Player> { player });
            }

            List<int> stars = player.GetPile(Name);
            if (stars.Count > 0 && player.HandcardNum > 0)
            {
                List<int> ups = player.GetCards("h");
                AskForMoveCardsStruct move = room.AskForMoveCards(player, ups, stars, true, Name, stars.Count, stars.Count, false, false, new List<int>(), info.SkillPosition);
                if (move.Success)
                {
                    List<int> up = new List<int>(), down = new List<int>();
                    foreach (int id in move.Top)
                        if (room.GetCardPlace(id) != Place.PlaceHand)
                            up.Add(id);

                    foreach (int id in move.Bottom)
                        if (room.GetCardPlace(id) == Place.PlaceHand)
                            down.Add(id);

                    Debug.Assert(up.Count == down.Count);
                    if (up.Count > 0)
                    {
                        List<CardsMoveStruct> moves = new List<CardsMoveStruct>();
                        CardsMoveStruct move1 = new CardsMoveStruct(up, player, Place.PlaceHand, new CardMoveReason(MoveReason.S_REASON_EXCHANGE_FROM_PILE, player.Name, Name, string.Empty));
                        CardsMoveStruct move2 = new CardsMoveStruct(down, player, Place.PlaceSpecial,
                            new CardMoveReason(MoveReason.S_REASON_REMOVE_FROM_GAME, player.Name, Name, string.Empty))
                        {
                            To_pile_name = Name
                        };

                        moves.Add(move1);
                        moves.Add(move2);
                        room.MoveCardsAtomic(moves, false);
                    }
                }
            }

            return false;
        }
    }

    public class QixingClear : DetachEffectSkill
    {
        public QixingClear() : base("qixing", "qixing") { }
    }

    public class Kuangfeng : TriggerSkill
    {
        public Kuangfeng() : base("kuangfeng")
        {
            view_as_skill = new KuangfengVS();
            skill_type = SkillType.Wizzard;
            events = new List<TriggerEvent> { TriggerEvent.TurnStart, TriggerEvent.EventPhaseStart, TriggerEvent.Death };
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            bool remove = false;
            if (triggerEvent == TriggerEvent.TurnStart && room.ContainsTag(Name) && room.GetTag(Name) is Player target && player == target)
            {
                remove = true;
            }
            else if (triggerEvent == TriggerEvent.Death && room.ContainsTag(Name) && room.GetTag(Name) is Player death && death == player)
            {
                remove = true;
            }

            if (remove)
            {
                foreach (Player p in room.GetAlivePlayers())
                    if (p.GetMark("@gale") > 0)
                        room.SetPlayerMark(p, "@gale", 0);
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Finish && base.Triggerable(player, room) && player.GetPile("qixing").Count > 0)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.AskForUseCard(player, "@@kuangfeng", "@kuangfeng", null, -1, HandlingMethod.MethodUse, true, info.SkillPosition);
            return new TriggerStruct();
        }
    }

    public class KuangfengDamage : TriggerSkill
    {
        public KuangfengDamage() : base("#kuangfeng")
        {
            events.Add(TriggerEvent.DamageInflicted);
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is DamageStruct damage && damage.Nature == DamageStruct.DamageNature.Fire
                && player.GetMark("@gale") > 0 && room.ContainsTag("kuangfeng") && room.GetTag("kuangfeng") is Player from && from.Alive)
                return new TriggerStruct(Name, from);

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is DamageStruct damage)
            {
                LogMessage log = new LogMessage
                {
                    Type = "#kuangfeng-add",
                    From = ask_who.Name,
                    Arg = damage.Damage.ToString(),
                    Arg2 = (++damage.Damage).ToString()
                };
                room.SendLog(log);

                data = damage;
            }

            return false;
        }
    }

    public class KuangfengVS : OneCardViewAsSkill
    {
        public KuangfengVS() : base("kuangfeng")
        {
            filter_pattern = ".|.|.|qixing";
            expand_pile = "qixing";
            response_pattern = "@@kuangfeng";
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return false;
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard shun = new WrappedCard(KuangfengCard.ClassName)
            {
                Skill = Name,
                ShowSkill = Name
            };
            shun.AddSubCard(card);
            return shun;
        }
    }

    public class KuangfengCard : SkillCard
    {
        public static string ClassName = "KuangfengCard";
        public KuangfengCard() : base(ClassName)
        {
            will_throw = false;
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From, target = card_use.To[0];
            List<int> ids = new List<int>(card_use.Card.SubCards);
            room.SetTag("kuangfeng", player);

            CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_REMOVE_FROM_PILE, null, "kuangfeng", null);
            room.ThrowCard(ref ids, reason, null);

            room.AddPlayerMark(target, "@gale");
        }
    }

    public class Dawu : TriggerSkill
    {
        public Dawu() : base("dawu")
        {
            view_as_skill = new DawuVS();
            skill_type = SkillType.Wizzard;
            events = new List<TriggerEvent> { TriggerEvent.TurnStart, TriggerEvent.EventPhaseStart, TriggerEvent.Death };
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            bool remove = false;
            if (triggerEvent == TriggerEvent.TurnStart && room.ContainsTag(Name) && room.GetTag(Name) is Player target && player == target)
            {
                remove = true;
            }
            else if (triggerEvent == TriggerEvent.Death && room.ContainsTag(Name) && room.GetTag(Name) is Player death && death == player)
            {
                remove = true;
            }

            if (remove)
            {
                foreach (Player p in room.GetAlivePlayers())
                    if (p.GetMark("@fog") > 0)
                        room.SetPlayerMark(p, "@fog", 0);
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Finish && base.Triggerable(player, room) && player.GetPile("qixing").Count > 0)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.AskForUseCard(player, "@@dawu", "@dawu", null, -1, HandlingMethod.MethodUse, true, info.SkillPosition);
            return new TriggerStruct();
        }
    }

    public class DawuDamage : TriggerSkill
    {
        public DawuDamage() : base("#dawu")
        {
            events.Add(TriggerEvent.DamageDefined);
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is DamageStruct damage && damage.Nature != DamageStruct.DamageNature.Thunder
                && player.GetMark("@fog") > 0 && room.ContainsTag("dawu") && room.GetTag("dawu") is Player from && from.Alive)
                return new TriggerStruct(Name, from);

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            LogMessage log = new LogMessage
            {
                Type = "#damaged-prevent",
                From = player.Name,
                Arg = "dawu"
            };
            room.SendLog(log);

            return true;
        }
    }

    public class DawuVS : ViewAsSkill
    {
        public DawuVS() : base("dawu")
        {
            expand_pile = "qixing";
            response_pattern = "@@dawu";
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return false;
        }

        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return player.GetPile(expand_pile).Contains(to_select.Id);
        }

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count > 0)
            {
                WrappedCard shun = new WrappedCard(DawuCard.ClassName)
                {
                    Skill = Name,
                    ShowSkill = Name
                };
                shun.AddSubCards(cards);
                return shun;
            }

            return null;
        }
    }

    public class DawuCard : SkillCard
    {
        public static string ClassName = "DawuCard";
        public DawuCard() : base(ClassName)
        {
            will_throw = false;
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count < card.SubCards.Count;
        }

        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            return targets.Count == card.SubCards.Count;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From, target = card_use.To[0];
            List<int> ids = new List<int>(card_use.Card.SubCards);
            room.SetTag("dawu", player);

            CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_REMOVE_FROM_PILE, null, "dawu", null);
            room.ThrowCard(ref ids, reason, null);

            base.Use(room, card_use);
        }

        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            room.AddPlayerMark(effect.To, "@fog");
        }
    }

    public class Guixin : MasochismSkill
    {
        public Guixin() : base("guixin")
        {
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
            bool check = false;
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (!p.IsAllNude() && RoomLogic.CanGetCard(room, player, p, "hej"))
                {
                    check = true;
                    break;
                }
            }
            if (check && room.AskForSkillInvoke(player, Name, null, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }

        public override void OnDamaged(Room room, Player target, DamageStruct damage, TriggerStruct info)
        {
            string choice = room.AskForChoice(target, Name, "hand+equip+judge", new List<string> { "@guixin" });
            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetOtherPlayers(target))
            {
                if (!p.IsAllNude() && RoomLogic.CanGetCard(room, target, p, "hej"))
                {
                    room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, target.Name, p.Name);
                    targets.Add(p);
                }
            }

            foreach (Player p in targets)
            {
                if (!p.Alive || p.IsAllNude()) continue;
                List<int> ids = new List<int>();
                switch (choice)
                {
                    case "hand":
                        {
                            if (!p.IsKongcheng() && RoomLogic.CanGetCard(room, target, p, "h"))
                                ids = p.GetCards("h");
                            else if (p.HasEquip() && RoomLogic.CanGetCard(room, target, p, "e"))
                                ids = p.GetCards("e");
                            else if (p.JudgingArea.Count > 0 && RoomLogic.CanGetCard(room, target, p, "j"))
                                ids = p.GetCards("j");
                        }
                        break;
                    case "equip":
                        {
                            if (p.HasEquip() && RoomLogic.CanGetCard(room, target, p, "e"))
                                ids = p.GetCards("e");
                            else if (!p.IsKongcheng() && RoomLogic.CanGetCard(room, target, p, "h"))
                                ids = p.GetCards("h");
                            else if (p.JudgingArea.Count > 0 && RoomLogic.CanGetCard(room, target, p, "j"))
                                ids = p.GetCards("j");
                        }
                        break;
                    case "judge":
                        {
                            if (p.JudgingArea.Count > 0 && RoomLogic.CanGetCard(room, target, p, "j"))
                                ids = p.GetCards("j");
                            else if (!p.IsKongcheng() && RoomLogic.CanGetCard(room, target, p, "h"))
                                ids = p.GetCards("h");
                            else if (p.HasEquip() && RoomLogic.CanGetCard(room, target, p, "e"))
                                ids = p.GetCards("e");
                        }
                        break;
                }

                if (ids.Count > 0)
                {
                    Shuffle.shuffle(ref ids);
                    foreach (int id in ids)
                    {
                        if (RoomLogic.CanGetCard(room, target, p, id))
                        {
                            List<int> cards = new List<int> { id };
                            room.ObtainCard(target, ref cards, new CardMoveReason(MoveReason.S_REASON_EXTRACTION, target.Name, p.Name, Name, string.Empty), false);
                            Thread.Sleep(300);
                            break;
                        }
                    }
                }

                if (!target.Alive) break;
            }

            if (target.Alive) room.TurnOver(target);
        }
    }

    public class Juejing : TriggerSkill
    {
        public Juejing() : base("juejing")
        {
            events = new List<TriggerEvent> { TriggerEvent.Dying, TriggerEvent.QuitDying };
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.Dying && data is DyingStruct dying && dying.Who == player && base.Triggerable(player, room))
                return new TriggerStruct(Name, player);
            else if (triggerEvent == TriggerEvent.QuitDying && base.Triggerable(player, room))
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name);
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            room.DrawCards(player, 1, Name);

            return false;
        }
    }

    public class JuejingMax : MaxCardsSkill
    {
        public JuejingMax() : base("#juejing-max") { }

        public override int GetExtra(Room room, Player target)
        {
            if (RoomLogic.PlayerHasSkill(room, target, "juejing")) return 2;

            return 0;
        }
    }

    public class Longhun : TriggerSkill
    {
        public Longhun() : base("longhun")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardUsedAnnounced, TriggerEvent.CardResponded };
            view_as_skill = new LonghunVS();
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardUsedAnnounced && data is CardUseStruct use && use.Card.Skill == Name && use.Card.SubCards.Count == 2 && player.Alive)
            {
                if (WrappedCard.IsRed(use.Card.Suit))
                {
                    use.ExDamage++;
                    data = use;

                    if (use.Card.Name == FireSlash.ClassName)
                    {
                        LogMessage log = new LogMessage
                        {
                            Type = "#card-damage",
                            From = player.Name,
                            Arg = Name,
                            Arg2 = use.Card.Name
                        };
                        room.SendLog(log);
                    }
                    else
                    {
                        LogMessage log = new LogMessage
                        {
                            Type = "#card-recover",
                            From = player.Name,
                            Arg = Name,
                            Arg2 = use.Card.Name
                        };

                        room.SendLog(log);
                    }
                }
                else if (room.Current.Alive && !room.Current.IsNude() && RoomLogic.CanDiscard(room, player, room.Current, "he"))
                {
                    int id = room.AskForCardChosen(player, room.Current, "he", Name, false, HandlingMethod.MethodDiscard);
                    room.ThrowCard(id, room.Current, player);
                }
            }
            else if (triggerEvent == TriggerEvent.CardResponded && data is CardResponseStruct resp && resp.Card != null && resp.Use
                && resp.Card.Skill == Name && resp.Card.SubCards.Count == 2)
            {
                if (!WrappedCard.IsRed(resp.Card.Suit) && room.Current.Alive && !room.Current.IsNude() && RoomLogic.CanDiscard(room, player, room.Current, "he"))
                {
                    int id = room.AskForCardChosen(player, room.Current, "he", Name, false, HandlingMethod.MethodDiscard);
                    room.ThrowCard(id, room.Current, player);
                }
            }
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class LonghunVS : ViewAsSkill
    {
        public LonghunVS() : base("longhun")
        {
            response_or_use = true;
        }

        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            if (selected.Count > 0 && (selected.Count >= 2 || to_select.Suit != selected[0].Suit)) return false;

            if (room.GetRoomState().GetCurrentCardUseReason() == CardUseReason.CARD_USE_REASON_PLAY)
            {
                if (RoomLogic.IsCardLimited(room, player, to_select, HandlingMethod.MethodUse)) return false;

                if (player.IsWounded() && to_select.Suit == WrappedCard.CardSuit.Heart) return true;
                if (to_select.Suit == WrappedCard.CardSuit.Diamond)
                {
                    WrappedCard slash = new WrappedCard(FireSlash.ClassName, -1, WrappedCard.CardSuit.Diamond, to_select.Number);
                    return Slash.IsAvailable(room, player, slash);
                }
            }
            else if (room.GetRoomState().GetCurrentCardUseReason() == CardUseReason.CARD_USE_REASON_RESPONSE_USE)
            {
                if (RoomLogic.IsCardLimited(room, player, to_select, HandlingMethod.MethodUse)) return false;
                string pattern = room.GetRoomState().GetCurrentCardUsePattern(player);
                WrappedCard slash = new WrappedCard(FireSlash.ClassName);
                WrappedCard peach = new WrappedCard(Peach.ClassName);
                WrappedCard jink = new WrappedCard(Jink.ClassName);
                WrappedCard nulli = new WrappedCard(Nullification.ClassName);
                if (Engine.MatchExpPattern(room, pattern, player, slash) && to_select.Suit == WrappedCard.CardSuit.Diamond) return true;
                if (Engine.MatchExpPattern(room, pattern, player, jink) && to_select.Suit == WrappedCard.CardSuit.Club) return true;
                if (Engine.MatchExpPattern(room, pattern, player, peach) && to_select.Suit == WrappedCard.CardSuit.Heart) return true;
                if (Engine.MatchExpPattern(room, pattern, player, nulli) && to_select.Suit == WrappedCard.CardSuit.Spade) return true;
            }
            else if (room.GetRoomState().GetCurrentCardUseReason() == CardUseReason.CARD_USE_REASON_RESPONSE)
            {
                if (RoomLogic.IsCardLimited(room, player, to_select, HandlingMethod.MethodResponse)) return false;
                string pattern = room.GetRoomState().GetCurrentCardUsePattern(player);
                WrappedCard slash = new WrappedCard(FireSlash.ClassName);
                WrappedCard jink = new WrappedCard(Jink.ClassName);
                if (Engine.MatchExpPattern(room, pattern, player, slash) && to_select.Suit == WrappedCard.CardSuit.Diamond) return true;
                if (Engine.MatchExpPattern(room, pattern, player, jink) && to_select.Suit == WrappedCard.CardSuit.Club) return true;
            }

            return false;
        }

        public override bool IsEnabledAtNullification(Room room, Player player)
        {
            List<int> ids = player.GetCards("he");
            ids.AddRange(player.GetHandPile());
            foreach (int id in ids)
            {
                WrappedCard card = room.GetCard(id);
                if (card.Suit == WrappedCard.CardSuit.Spade)
                    return true;
            }

            return false;
        }

        public override bool IsEnabledAtResponse(Room room, Player player, string pattern)
        {
            WrappedCard slash = new WrappedCard(FireSlash.ClassName);
            WrappedCard peach = new WrappedCard(Peach.ClassName);
            WrappedCard jink = new WrappedCard(Jink.ClassName);
            return Engine.MatchExpPattern(room, pattern, player, slash) || Engine.MatchExpPattern(room, pattern, player, jink) || Engine.MatchExpPattern(room, pattern, player, peach);
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            WrappedCard slash = new WrappedCard(FireSlash.ClassName);
            return player.IsWounded() || Slash.IsAvailable(room, player, slash);
        }

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count == 1 || cards.Count == 2)
            {
                if (cards.Count == 2 && cards[0].Suit != cards[1].Suit) return null;

                switch (cards[0].Suit)
                {
                    case WrappedCard.CardSuit.Heart:
                        {
                            WrappedCard card = new WrappedCard(Peach.ClassName) { Skill = Name };
                            card.AddSubCards(cards);
                            card = RoomLogic.ParseUseCard(room, card);
                            return Peach.Instance.IsAvailable(room, player, card) ? card : null;
                        }
                    case WrappedCard.CardSuit.Spade:
                        {
                            WrappedCard card = new WrappedCard(Nullification.ClassName) { Skill = Name };
                            card.AddSubCards(cards);
                            card = RoomLogic.ParseUseCard(room, card);
                            return card;
                        }
                    case WrappedCard.CardSuit.Club:
                        {
                            WrappedCard card = new WrappedCard(Jink.ClassName) { Skill = Name };
                            card.AddSubCards(cards);
                            card = RoomLogic.ParseUseCard(room, card);
                            return Engine.MatchExpPattern(room, room.GetRoomState().GetCurrentCardUsePattern(player), player, card) ? card : null;
                        }
                    case WrappedCard.CardSuit.Diamond:
                        {
                            WrappedCard card = new WrappedCard(FireSlash.ClassName) { Skill = Name };
                            card.AddSubCards(cards);
                            card = RoomLogic.ParseUseCard(room, card);
                            return room.GetRoomState().GetCurrentCardUseReason() == CardUseReason.CARD_USE_REASON_PLAY
                                || Engine.MatchExpPattern(room, room.GetRoomState().GetCurrentCardUsePattern(player), player, card) ? card : null;
                        }
                }
            }

            return null;
        }
    }

    public class Kuangbao : TriggerSkill
    {
        public Kuangbao() : base("kuangbao")
        {
            frequency = Frequency.Compulsory;
            events = new List<TriggerEvent> { TriggerEvent.Damage, TriggerEvent.Damaged, TriggerEvent.GameStart };
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room))
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name);
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);

            if (triggerEvent == TriggerEvent.GameStart)
                player.AddMark("baonu", 2);
            else if (data is DamageStruct damage)
                player.AddMark("baonu", damage.Damage);

            room.SetPlayerStringMark(player, "baonu", player.GetMark("baonu").ToString());
            return false;
        }
    }

    public class Wumou : TriggerSkill
    {
        public Wumou() : base("wumou")
        {
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Wizzard;
            events.Add(TriggerEvent.CardUsedAnnounced);
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is CardUseStruct use && base.Triggerable(player, room))
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is TrickCard && !(fcard is DelayedTrick))
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name);
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            List<string> choices = new List<string> { "lose" };
            if (player.GetMark("baonu") > 0)
                choices.Add("discard");

            if (room.AskForChoice(player, Name, string.Join("+", choices)) == "lose")
            {
                room.LoseHp(player);
            }
            else
            {
                player.AddMark("baonu", -1);
                if (player.GetMark("baonu") > 0)
                    room.SetPlayerStringMark(player, "baonu", player.GetMark("baonu").ToString());
                else
                    room.RemovePlayerStringMark(player, "baonu");
            }

            return false;
        }
    }

    public class Wuqian : TriggerSkill
    {
        public Wuqian() : base("wuqian")
        {
            events.Add(TriggerEvent.EventPhaseChanging);
            view_as_skill = new WuqianVS();
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (data is PhaseChangeStruct change && player.HasFlag(Name) && change.To == PlayerPhase.NotActive)
            {
                room.HandleAcquireDetachSkills(player, "-wushuang", true);
                Player target = room.FindPlayer(player.GetTag(Name).ToString(), true);
                target.AddMark("Armor_Nullified", -1);
            }
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class WuqianVS : ZeroCardViewAsSkill
    {
        public WuqianVS() : base("wuqian") { }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard(WuqianCard.ClassName) { Skill = Name };
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return player.GetMark("baonu") > 1 && !player.HasUsed(WuqianCard.ClassName);
        }
    }

    public class WuqianCard : SkillCard
    {
        public static string ClassName = "WuqianCard";
        public WuqianCard() : base(ClassName) { }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From, target = card_use.To[0];
            player.AddMark("baonu", -2);

            if (player.GetMark("baonu") > 0)
                room.SetPlayerStringMark(player, "baonu", player.GetMark("baonu").ToString());
            else
                room.RemovePlayerStringMark(player, "baonu");

            player.SetFlags("wuqian");
            player.SetTag("wuqian", target.Name);
            room.HandleAcquireDetachSkills(player, "wushuang", true);

            LogMessage log = new LogMessage
            {
                Type = "#wuqian",
                From = target.Name
            };
            room.SendLog(log);

            target.AddMark("Armor_Nullified");
        }
    }

    public class Shenfen : ZeroCardViewAsSkill
    {
        public Shenfen() : base("shenfen") { skill_type = SkillType.Attack; }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return player.GetMark("baonu") > 5 && !player.HasUsed(ShenfenCard.ClassName);
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard(ShenfenCard.ClassName) { Skill = Name };
        }
    }

    public class ShenfenCard : SkillCard
    {
        public static string ClassName = "ShenfenCard";
        public ShenfenCard() : base(ClassName)
        {
            target_fixed = true;
        }

        public override void OnUse(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            List<Player> targets = room.GetOtherPlayers(player);
            card_use.To = targets;
            base.OnUse(room, card_use);
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            player.AddMark("baonu", -6);
            if (player.GetMark("baonu") > 0)
                room.SetPlayerStringMark(player, "baonu", player.GetMark("baonu").ToString());
            else
                room.RemovePlayerStringMark(player, "baonu");

            foreach (Player p in card_use.To)
                if (p.Alive)
                    room.Damage(new DamageStruct("shenfen", player, p));

            base.Use(room, card_use);

            if (player.Alive)
                room.TurnOver(player);
        }

        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            Player to = effect.To;
            if (to.Alive)
            {
                List<int> ids = new List<int>();
                foreach (int id in to.GetEquips())
                    if (RoomLogic.CanDiscard(room, to, to, id))
                        ids.Add(id);

                if (ids.Count > 0)
                    room.ThrowCard(ref ids, to);

                if (to.Alive && !to.IsKongcheng())
                    room.AskForDiscard(to, "shenfen", 4, 4, false, false, "@shenfen");
            }
        }
    }

    public class Renjie : TriggerSkill
    {
        public Renjie() : base("renjie")
        {
            frequency = Frequency.Compulsory;
            events = new List<TriggerEvent> { TriggerEvent.CardsMoveOneTime, TriggerEvent.Damaged };
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.From != null && move.From.Phase == PlayerPhase.Discard
                && base.Triggerable(move.From, room) && move.From_places.Contains(Place.PlaceHand) && move.Reason.PlayerId == move.From.Name
                && (move.Reason.Reason & MoveReason.S_MASK_BASIC_REASON) == MoveReason.S_REASON_DISCARD)
            {
                foreach (Place place in move.From_places)
                    if (place == Place.PlaceHand)
                        return new TriggerStruct(Name, move.From);
            }
            else if (triggerEvent == TriggerEvent.Damaged && base.Triggerable(player, room))
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(ask_who, Name);
            room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);

            if (triggerEvent == TriggerEvent.Damaged && data is DamageStruct damage)
            {
                room.AddPlayerMark(ask_who, "@patience", damage.Damage);
            }
            else if (data is CardsMoveOneTimeStruct move)
            {
                int count = 0;
                foreach (Place place in move.From_places)
                    if (place == Place.PlaceHand)
                        count++;

                room.AddPlayerMark(ask_who, "@patience", count);
            }
            

            return false;
        }
    }

    public class Lianpo : TriggerSkill
    {
        public Lianpo() : base("lianpo")
        {
            events = new List<TriggerEvent> { TriggerEvent.Death, TriggerEvent.EventPhaseChanging };
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.Death && data is DeathStruct death && death.Damage.From != null && death.Damage.From.Alive)
                death.Damage.From.SetFlags(Name);
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
            {
                Player simayi = RoomLogic.FindPlayerBySkillName(room, Name);
                if (simayi != null && simayi.HasFlag(Name))
                    return new TriggerStruct(Name, simayi);
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

    public class Baiyin : PhaseChangeSkill
    {
        public Baiyin() : base("baiyin")
        {
            frequency = Frequency.Wake;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player.Phase == PlayerPhase.Start && base.Triggerable(player, room) && player.GetMark(Name) == 0 && player.GetMark("@patience") >= 4)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override bool OnPhaseChange(Room room, Player player, TriggerStruct info)
        {
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            room.DoSuperLightbox(player, info.SkillPosition, Name);
            room.SetPlayerMark(player, Name, 1);
            room.SendCompulsoryTriggerLog(player, Name);

            room.LoseMaxHp(player);
            room.HandleAcquireDetachSkills(player, "jilue", true);

            return false;
        }
    }

    public class Jilue : TriggerSkill
    {
        public Jilue() : base("jilue")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardUsed, TriggerEvent.EventPhaseChanging, TriggerEvent.Damaged, TriggerEvent.AskForRetrial };
            skill_type = SkillType.Wizzard;
            view_as_skill = new JilueVS();
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive && player.HasFlag(Name))
            {
                room.HandleAcquireDetachSkills(player, "-wansha", true);
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.CardUsed && base.Triggerable(player, room) && data is CardUseStruct use && use.Card != null && player.GetMark("@patience") > 0
                && (!use.Card.IsVirtualCard() || use.Card.SubCards.Count == 0))
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard.IsNDTrick())
                    return new TriggerStruct(Name, player);
            }
            else if (triggerEvent == TriggerEvent.Damaged && base.Triggerable(player, room) && player.GetMark("@patience") > 0)
                return new TriggerStruct(Name, player);
            else if (triggerEvent == TriggerEvent.AskForRetrial && base.Triggerable(player, room)
                && (!player.IsNude() || player.GetHandPile().Count > 0) && player.GetMark("@patience") > 0)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.CardUsed && room.AskForSkillInvoke(player, Name, "@jilue-jizhi", info.SkillPosition))
            {
                room.AddPlayerMark(player, "@patience", -1);
                room.BroadcastSkillInvoke("jizhi", player, info.SkillPosition);
                return info;
            }
            else if (triggerEvent == TriggerEvent.Damaged)
            {
                Player to = room.AskForPlayerChosen(player, room.GetOtherPlayers(player), Name, "@jilue-fangzhu", true, true, info.SkillPosition);
                if (to != null)
                {
                    room.AddPlayerMark(player, "@patience", -1);

                    room.BroadcastSkillInvoke("fangzhu", player, info.SkillPosition);
                    List<string> target_list = player.ContainsTag("fangzhu_target") ? (List<string>)player.GetTag("fangzhu_target") : new List<string>();
                    target_list.Add(to.Name);
                    player.SetTag("fangzhu_target", target_list);
                    return info;
                }
            }
            else if (triggerEvent == TriggerEvent.AskForRetrial && data is JudgeStruct judge)
            {
                List<string> prompt_list = new List<string> { "@jilue-guicai", judge.Who.Name, string.Empty, judge.Reason };
                string prompt = string.Join(":", prompt_list);

                room.SetTag(Name, data);
                WrappedCard card = room.AskForCard(player, Name, "..", prompt, data, HandlingMethod.MethodResponse, judge.Who, true);
                room.RemoveTag(Name);
                if (card != null)
                {
                    room.AddPlayerMark(player, "@patience", -1);
                    room.BroadcastSkillInvoke("guicai", player, info.SkillPosition);
                    room.Retrial(card, player, ref judge, Name, false, info.SkillPosition);
                    data = judge;
                    return info;
                }
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.CardUsed)
            {
                room.DrawCards(player, 1, Name);
            }
            else if (triggerEvent == TriggerEvent.Damaged)
            {
                List<string> target_list = (List<string>)player.GetTag("fangzhu_target");
                string target_name = target_list[target_list.Count - 1];
                target_list.RemoveAt(target_list.Count - 1);
                player.SetTag("fangzhu_target", target_list);
                Player to = room.FindPlayer(target_name); ;

                if (to != null)
                {
                    if (player.IsWounded())
                        room.DrawCards(to, new DrawCardStruct(player.GetLostHp(), player, Name));
                    room.TurnOver(to);
                }
            }
            else if (data is JudgeStruct judge)
            {
                room.UpdateJudgeResult(ref judge);
                data = judge;
                return false;
            }

            return false;
        }
    }

    public class JilueVS : ViewAsSkill
    {
        public JilueVS() : base("jilue")
        {
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            if (player.GetMark("@patience") == 0) return false;
            if (player.HasUsed(ZhihengJCard.ClassName) && RoomLogic.PlayerHasSkill(room, player, "wansha")) return false;

            return true;
        }
        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            if (player.HasUsed(ZhihengJCard.ClassName)) return false;
            return !RoomLogic.IsJilei(room, player, to_select);
        }
        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count == 0 && !RoomLogic.PlayerHasSkill(room, player, "wansha"))
            {
                return new WrappedCard(JilueCard.ClassName) { Skill = "wansha" };
            }
            else if (cards.Count > 0 && !player.HasUsed(ZhihengJCard.ClassName))
            {
                WrappedCard zhiheng_card = new WrappedCard(JilueCard.ClassName)
                {
                    Skill = "zhiheng_jx",
                };
                zhiheng_card.AddSubCards(cards);
                return zhiheng_card;
            }

            return null;
        }
    }

    public class JilueCard : SkillCard
    {
        public static string ClassName = "JilueCard";
        public JilueCard() : base(ClassName)
        {
            target_fixed = true;
            auto_use = false;
        }

        public override void OnCardAnnounce(Room room, CardUseStruct use, bool ignore_rule)
        {
            if (use.Card.SubCards.Count > 0)
            {
                use.From.AddHistory(ZhihengJCard.ClassName);
                if (use.From.IsLastHandCard(use.Card, true))
                    use.Card.UserString = "1";
            }
            base.OnCardAnnounce(room, use, ignore_rule);
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            room.AddPlayerMark(card_use.From, "@patience", -1);

            if (card_use.Card.SubCards.Count == 0)
            {
                room.HandleAcquireDetachSkills(card_use.From, "wansha", true);
                card_use.From.SetFlags("jilue");
            }
            else if (card_use.From.Alive)
            {
                int count = card_use.Card.SubCards.Count;
                if (card_use.Card.UserString == "1") count++;
                room.DrawCards(card_use.From, count, "zhiheng_jx");
            }
        }
    }

    public class Longnu : TriggerSkill
    {
        public Longnu() : base("longnu")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart, TriggerEvent.EventPhaseChanging, TriggerEvent.EventAcquireSkill, TriggerEvent.EventLoseSkill };
            frequency = Frequency.Compulsory;
            turn = true;
            view_as_skill = new LongnuFilter();
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive && player.HasFlag(Name))
            {
                player.SetFlags("-longnu");
                room.FilterCards(player, player.GetCards("h"), true);
            }
            else if (triggerEvent == TriggerEvent.EventAcquireSkill && data is InfoStruct info && info.Info == Name)
            {
                room.SetTurnSkillState(player, Name, false, info.Head ? "head" : "deputy");
            }
            else if (triggerEvent == TriggerEvent.EventLoseSkill && data is InfoStruct _info && _info.Info == Name)
            {
                room.RemoveTurnSkill(player);
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Play && base.Triggerable(player, room))
            {
                return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
            room.BroadcastSkillInvoke(Name, "male", player.GetMark(Name) == 0 ? 1 : 2, gsk.General, gsk.SkinId);
            if (player.GetMark(Name) == 0)
            {
                room.LoseHp(player);
                if (player.Alive)
                {
                    room.DrawCards(player, 1, Name);
                    player.SetFlags(Name);
                    player.AddMark(Name);
                    room.SetTurnSkillState(player, Name, true, info.SkillPosition);
                }
            }
            else
            {
                room.LoseMaxHp(player);
                if (player.Alive)
                {
                    room.DrawCards(player, 1, Name);
                    player.SetFlags(Name);
                    player.SetMark(Name, 0);
                    room.SetTurnSkillState(player, Name, false, info.SkillPosition);
                }
            }

            room.FilterCards(player, player.GetCards("h"), true);
            return false;
        }
    }

    public class LongnuFilter : FilterSkill
    {
        public LongnuFilter() : base("longnu")
        {
        }

        public override bool ViewFilter(Room room, WrappedCard to_select, Player player)
        {
            int id = to_select.Id;
            if (!player.HasFlag(Name) || player != room.Current || room.GetCardPlace(id) != Place.PlaceHand) return false;
            return (player.GetMark(Name) > 0 && WrappedCard.IsRed(to_select.Suit))
                || (player.GetMark(Name) == 0 && Engine.GetFunctionCard(to_select.Name).TypeID == CardType.TypeTrick);
        }

        public override void ViewAs(Room room, ref RoomCard card, Player player)
        {
            card.ChangeName(player.GetMark(Name) > 0 ? FireSlash.ClassName : ThunderSlash.ClassName);
        }
    }

    public class LongnuTar : TargetModSkill
    {
        public LongnuTar() : base("#longnu-tar", false)
        {
        }

        public override bool GetDistanceLimit(Room room, Player from, Player to, WrappedCard card, CardUseReason reason, string pattern)
        {
            if (from != null && card != null && card.Name == FireSlash.ClassName && from.HasFlag("longnu") && from.GetMark("longnu") > 0)
                return true;

            return false;
        }

        public override int GetResidueNum(Room room, Player from, WrappedCard card)
        {
            if (from != null && card != null && card.Name == ThunderSlash.ClassName && from.HasFlag("longnu") && from.GetMark("longnu") == 0)
                return 999;

            return 0;
        }
    }

    public class Jieying : TriggerSkill
    {
        public Jieying() : base("jieying")
        {
            events = new List<TriggerEvent> { TriggerEvent.GameStart, TriggerEvent.EventPhaseStart, TriggerEvent.ChainStateChanged, TriggerEvent.ChainStateCanceling };
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.GameStart && base.Triggerable(player, room) && !player.Chained)
                return new TriggerStruct(Name, player);
            else if (triggerEvent == TriggerEvent.ChainStateChanged && base.Triggerable(player, room) && !player.Chained
                && data is DamageStruct damage && damage.To == player && damage.Nature != DamageStruct.DamageNature.Normal)
                return new TriggerStruct(Name, player);
            else if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Finish && base.Triggerable(player, room))
            {
                foreach (Player p in room.GetOtherPlayers(player))
                    if (!p.Chained && RoomLogic.CanBeChainedBy(room, p, player))
                        return new TriggerStruct(Name, player);
            }
            else if (triggerEvent == TriggerEvent.ChainStateCanceling && data is bool chain && !chain && base.Triggerable(player, room))
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.GameStart || triggerEvent == TriggerEvent.ChainStateChanged)
            {
                room.SendCompulsoryTriggerLog(player, Name);

                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);

                room.SetPlayerChained(player, true);
            }
            else if (triggerEvent == TriggerEvent.EventPhaseStart)
            {
                List<Player> targets = new List<Player>();
                foreach (Player p in room.GetOtherPlayers(player))
                    if (!p.Chained && RoomLogic.CanBeChainedBy(room, p, player))
                        targets.Add(p);
                Player target = room.AskForPlayerChosen(player, targets, Name, "@jieying", false, true, info.SkillPosition);

                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);

                room.SetPlayerChained(target, true);
            }
            else
            {
                room.SendCompulsoryTriggerLog(player, Name);
                return true;
            }

            return false;
        }
    }

    public class JieyingMax : MaxCardsSkill
    {
        public JieyingMax() : base("#jieying-max") { }

        public override int GetExtra(Room room, Player target)
        {
            Player lb = RoomLogic.FindPlayerBySkillName(room, "jieying");
            if (lb != null && target.Chained) return 2;
            return 0;
        }
    }

    public class Poxi : TriggerSkill
    {
        public Poxi() : base("poxi")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseChanging };
            view_as_skill = new PoxiVS();
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (player.GetMark(Name) > 0 && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
                player.SetMark(Name, 0);
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            return new TriggerStruct();
        }
        public override bool MoveFilter(Room room, int id, List<int> downs)
        {
            if (id != -1)
            {
                Player op = room.Current;
                Player owner = room.GetCardOwner(id);
                if (!RoomLogic.CanDiscard(room, op, owner, id)) return false;
            }

            List<WrappedCard.CardSuit> suits = new List<WrappedCard.CardSuit>();
            foreach (int card_id in downs)
            {
                WrappedCard card = room.GetCard(card_id);
                if (!suits.Contains(card.Suit))
                    suits.Add(card.Suit);
                else
                    return false;
            }
            if (id != -1)
                return !suits.Contains(room.GetCard(id).Suit);

            return true;
        }
    }

    public class PoxiVS : ZeroCardViewAsSkill
    {
        public PoxiVS() : base("poxi")
        {
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(PoxiCard.ClassName);
        }
        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard(PoxiCard.ClassName) { Skill = Name };
        }
    }

    public class PoxiMax : MaxCardsSkill
    {
        public PoxiMax() : base("#poxi") { }

        public override int GetExtra(Room room, Player target)
        {
            return -target.GetMark("poxi");
        }
    }

    public class PoxiCard : SkillCard
    {
        public static string ClassName = "PoxiCard";
        public PoxiCard() : base(ClassName)
        {
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && !to_select.IsKongcheng() && to_select != Self;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From, target = card_use.To[0];
            List<int> ids = player.GetCards("h");
            ids.AddRange(target.GetCards("h"));

            LogMessage log = new LogMessage
            {
                Type = "$ViewAllCards",
                From = player.Name,
                To = new List<string> { player.Name },
                Card_str = string.Join("+", JsonUntity.IntList2StringList(target.HandCards))
            };
            room.SendLog(log, player);

            LogMessage log2 = new LogMessage
            {
                Type = "#KnownBothView",
                From = player.Name,
                To = new List<string> { target.Name },
                Arg = "handcards"
            };
            room.SendLog(log2, new List<Player> { player });

            AskForMoveCardsStruct move = room.AskForMoveCards(player, ids, new List<int>(), false, "poxi", 4, 4, true, false, new List<int>(), card_use.Card.SkillPosition);
            if (move.Success && move.Bottom.Count == 4)
            {
                List<int> self = new List<int>(), other = new List<int>();
                foreach (int id in move.Bottom)
                {
                    if (room.GetCardOwner(id) == player)
                        self.Add(id);
                    else
                        other.Add(id);
                }

                if (self.Count > 0)
                    room.ThrowCard(ref self, player);
                if (other.Count > 0)
                    room.ThrowCard(ref other, target, player);

                if (player.Alive)
                {
                    switch (self.Count)
                    {
                        case 0:
                            room.LoseMaxHp(player);
                            break;
                        case 1:
                            player.AddMark("poxi");
                            player.SetFlags("Global_PlayPhaseTerminated");
                            break;
                        case 3:
                            RecoverStruct recover = new RecoverStruct
                            {
                                Recover = 1,
                                Who = player
                            };
                            room.Recover(player, recover, true);
                            break;
                        case 4:
                            room.DrawCards(player, 4, "poxi");
                            break;
                    }
                }
            }
        }
    }

    public class JieyingGN : TriggerSkill
    {
        public JieyingGN() : base("jieying_gn")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart, TriggerEvent.Death };
            skill_type = SkillType.Attack;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.Death && player.GetSkills(true, false).Contains(Name))
            {
                foreach (Player p in room.Players)
                    if (p.GetMark("@rob") > 0)
                        room.SetPlayerMark(p, "@rob", 0);
            }
            else if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.RoundStart && base.Triggerable(player, room))
            {
                bool mark = false;
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (p.GetMark("@rob") > 0)
                    {
                        mark = true;
                        break;
                    }
                }

                if (!mark) room.AddPlayerMark(player, "@rob");
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && base.Triggerable(player, room) && player.Phase == PlayerPhase.Finish && player.GetMark("@rob") > 0)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player target = room.AskForPlayerChosen(player, room.GetOtherPlayers(player), Name, "@jieying_gn-mark", true, true, info.SkillPosition);
            if (target != null)
            {
                room.SetTag(Name, target);
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart)
            {
                Player target = (Player)room.GetTag(Name);
                room.RemoveTag(Name);
                room.RemovePlayerMark(player, "@rob");
                room.AddPlayerMark(target, "@rob");
            }

            return false;
        }
    }

    public class JieyingGNMax : MaxCardsSkill
    {
        public JieyingGNMax() : base("#jieying_gn-max") { }

        public override int GetExtra(Room room, Player target)
        {
            Player gn = RoomLogic.FindPlayerBySkillName(room, "jieying_gn");
            if (gn != null && target.GetMark("@rob") > 0)
                return 1;

            return 0;
        }
    }

    public class JieyingGNTar : TargetModSkill
    {
        public JieyingGNTar() : base("#jieying_gn-tar", false) { }

        public override int GetResidueNum(Room room, Player from, WrappedCard card)
        {
            Player gn = RoomLogic.FindPlayerBySkillName(room, "jieying_gn");
            if (gn != null && from.GetMark("@rob") > 0)
                return 1;

            return 0;
        }
    }

    public class JieyingDraw : TriggerSkill
    {
        public JieyingDraw() : base("#jieying-draw")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseProceeding, TriggerEvent.EventPhaseStart };
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            Player luji = RoomLogic.FindPlayerBySkillName(room, "jieying_gn");
            if (triggerEvent == TriggerEvent.EventPhaseProceeding && player.GetMark("@rob") > 0 && player.Phase == PlayerPhase.Draw && luji != null)
                return new TriggerStruct(Name, luji);
            else if (triggerEvent == TriggerEvent.EventPhaseStart && player.Alive && player.Phase == PlayerPhase.NotActive
                && player.GetMark("@rob") > 0 && luji != null && player != luji)
                return new TriggerStruct(Name, luji);

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.EventPhaseProceeding && data is int count)
            {
                room.SendCompulsoryTriggerLog(ask_who, Name);
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, ask_who.Name, player.Name);
                count++;
                data = count;
            }
            else if (triggerEvent == TriggerEvent.EventPhaseStart)
            {
                room.SendCompulsoryTriggerLog(ask_who, Name);
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, ask_who.Name, player.Name);
                room.RemovePlayerMark(player, "@rob");

                List<int> ids = new List<int>();
                foreach (int id in player.GetCards("h"))
                    if (RoomLogic.CanGetCard(room, ask_who, player, id))
                        ids.Add(id);

                if (ids.Count > 0)
                {
                    GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, ask_who, "jieying_gn", info.SkillPosition);
                    room.BroadcastSkillInvoke("jieying_gn", "male", 2, gsk.General, gsk.SkinId);
                    room.ObtainCard(ask_who, ref ids, new CardMoveReason(MoveReason.S_REASON_EXTRACTION, ask_who.Name, player.Name, "jieying_gn", string.Empty), false);
                }
            }

            return false;
        }
    }

    public class JieyingDetach : DetachEffectSkill
    {
        public JieyingDetach() : base("jieying_gn", string.Empty) { }
        public override void OnSkillDetached(Room room, Player player, object data)
        {
            foreach (Player p in room.Players)
                if (p.GetMark("@rob") > 0)
                    room.SetPlayerMark(p, "@rob", 0);
        }
    }

    public class Junlue : TriggerSkill
    {
        public Junlue() : base("junlue")
        {
            events = new List<TriggerEvent> { TriggerEvent.Damage, TriggerEvent.Damaged };
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room))
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is DamageStruct damage)
            {
                room.SendCompulsoryTriggerLog(player, Name);
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                room.AddPlayerMark(player, "@junlue", damage.Damage);
            }

            return false;
        }
    }

    public class Cuike : TriggerSkill
    {
        public Cuike() : base("cuike")
        {
            events.Add(TriggerEvent.EventPhaseStart);
            skill_type = SkillType.Attack;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && player.Phase == PlayerPhase.Play)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (player.GetMark("@junlue") == 0 || (player.GetMark("@junlue") & 1) != 1)
            {
                List<Player> targets = new List<Player>();
                foreach (Player p in room.GetAlivePlayers())
                    if ((!p.Chained && RoomLogic.CanBeChainedBy(room, p, player)) || (!p.IsNude() && RoomLogic.CanDiscard(room, player, p, "hej"))) targets.Add(p);
                
                if (targets.Count > 0)
                {
                    player.SetFlags(Name);
                    Player target = room.AskForPlayerChosen(player, targets, Name, "@chuike-chain", true, true, info.SkillPosition);
                    player.SetFlags("-cuike");
                    if (target != null)
                    {
                        room.SetTag(Name, target);
                        room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                        return info;
                    }
                }
            }
            else
            {
                Player target = room.AskForPlayerChosen(player, room.GetAlivePlayers(), Name, "@chuike-damage", true, true, info.SkillPosition);
                if (target != null)
                {
                    room.SetTag(Name, target);
                    room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                    return info;
                }
            }

            if (player.GetMark("@junlue") > 7 && room.AskForSkillInvoke(player, Name, "@cuike-damageall", info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            bool damamge = false;
            if (room.ContainsTag(Name))
            {
                Player target = (Player)room.GetTag(Name);
                room.RemoveTag(Name);
                if (player.GetMark("@junlue") == 0 || (player.GetMark("@junlue") & 1) != 1)
                {
                    if (!target.Chained && RoomLogic.CanBeChainedBy(room, target, player))
                        room.SetPlayerChained(target, true);

                    if (player.Alive && target.Alive && !target.IsAllNude() && RoomLogic.CanDiscard(room, player, target, "hej"))
                    {
                        int id = room.AskForCardChosen(player, target, "hej", Name, false, HandlingMethod.MethodDiscard);
                        room.ThrowCard(id, room.GetCardPlace(id) == Place.PlaceDelayedTrick ? null : target, player != target ? player : null);
                    }
                }
                else
                    room.Damage(new DamageStruct(Name, player, target));
            }
            else
                damamge = true;

            if (player.Alive && (damamge || (player.GetMark("@junlue") > 7 && room.AskForSkillInvoke(player, Name, "@cuike-damageall", info.SkillPosition))))
            {
                room.SetPlayerMark(player, "@junlue", 0);
                foreach (Player p in room.GetOtherPlayers(player))
                    room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, p.Name);

                foreach (Player p in room.GetOtherPlayers(player))
                    if (p.Alive && player.Alive)
                        room.Damage(new DamageStruct(Name, player, p));
            }

            return false;
        }
    }

    public class Zhanhuo : ZeroCardViewAsSkill
    {
        public Zhanhuo() : base("zhanhuo") { limit_mark = "@zhanhuo"; frequency = Frequency.Limited; }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return player.GetMark(limit_mark) > 0 && player.GetMark("@junlue") > 0;
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard(ZhanhuoCard.ClassName) { Mute = true, Skill = Name };
        }
    }

    public class ZhanhuoCard : SkillCard
    {
        public static string ClassName = "ZhanhuoCard";
        public ZhanhuoCard() : base(ClassName) { }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count < Self.GetMark("@junlue") && to_select.Chained;
        }

        public override void OnUse(Room room, CardUseStruct card_use)
        {
            room.SetPlayerMark(card_use.From, "@zhanhuo", 0);
            room.SetPlayerMark(card_use.From, "@junlue", 0);
            room.BroadcastSkillInvoke("zhanhuo", card_use.From, card_use.Card.SkillPosition);
            room.DoSuperLightbox(card_use.From, card_use.Card.SkillPosition, "zhanhuo");
            base.OnUse(room, card_use);
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            foreach (Player p in card_use.To)
            {
                if (p.Alive)
                {
                    List<int> ids = new List<int>();
                    foreach (int id in p.GetEquips())
                        if (RoomLogic.CanDiscard(room, p, p, id)) ids.Add(id);

                    if (ids.Count > 0) room.ThrowCard(ref ids, new CardMoveReason(MoveReason.S_REASON_THROW, p.Name, "zhanhuo", string.Empty), p); 
                }
            }

            if (player.Alive)
            {
                List<Player> targets = new List<Player>();
                foreach (Player p in card_use.To)
                    if (p.Alive) targets.Add(p);

                if (targets.Count > 0)
                {
                    Player target = room.AskForPlayerChosen(player, targets, "zhanhuo", "@zhanhuo-damage", false, true, card_use.Card.SkillPosition);
                    room.Damage(new DamageStruct("zhanhuo", player, target, 1, DamageStruct.DamageNature.Fire));
                }
            }
        }
    }

    public class Shenfu : TriggerSkill
    {
        public Shenfu() : base("shenfu")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart, TriggerEvent.Death };
            skill_type = SkillType.Attack;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.Death && data is DeathStruct death && death.Damage.From != null && !string.IsNullOrEmpty(death.Damage.Reason)
                && death.Damage.Reason == Name)
                death.Damage.From.SetFlags(Name);
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && base.Triggerable(player, room) && player.Phase == PlayerPhase.Finish && !player.IsKongcheng())
            {
                return new TriggerStruct(Name, player);
            }
            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (player.HandcardNum % 2 == 1)
            {
                Player target = room.AskForPlayerChosen(player, room.GetOtherPlayers(player), Name, "@shenfu-damage", true, true, info.SkillPosition);
                if (target != null)
                {
                    DamageStruct damage = new DamageStruct(Name, player, target, 1, DamageStruct.DamageNature.Thunder);
                    room.SetTag(Name, damage);
                    room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                    return info;
                }
            }
            else
            {
                Player target = room.AskForPlayerChosen(player, room.GetAlivePlayers(), Name, "@shenfu-card", true, true, info.SkillPosition);
                if (target != null)
                {
                    room.SetTag(Name, target);
                    room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                    return info;
                }
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.GetTag(Name) is Player target)
            {
                room.RemoveTag(Name);

                while (true)
                {
                    if (target == null)
                    {
                        List<Player> targets = new List<Player>();
                        foreach (Player p in room.GetAlivePlayers())
                        {
                            if (!p.HasFlag(Name)) targets.Add(p);
                        }

                        if (targets.Count > 0)
                        {
                            target = room.AskForPlayerChosen(player, targets, Name, "@shenfu-card", true, true, info.SkillPosition);
                            if (target != null)
                                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                            else
                                break;
                        }
                        else
                            break;
                    }

                    target.SetFlags(Name);
                    List<string> choices = new List<string> { "draw" };
                    if (!target.IsNude() && RoomLogic.CanDiscard(room, player, target, "he"))
                        choices.Add("discard");

                    target.SetFlags("shenfu_target");
                    string choice = room.AskForChoice(player, Name, string.Join("+", choices), new List<string> { "@shenfu-card-to:" + target.Name });
                    target.SetFlags("-shenfu_target");
                    if (choice == "draw")
                    {
                        room.DrawCards(target, new DrawCardStruct(1, player, Name));
                    }
                    else
                    {
                        int card_id;
                        if (player != target)
                            card_id = room.AskForCardChosen(player, target, "he", Name, false, HandlingMethod.MethodDiscard);
                        else
                        {
                            List<int> ids = room.AskForExchange(player, Name, 1, 1, "@discard-self", string.Empty, "..!", info.SkillPosition);
                            card_id = ids[0];
                        }
                        room.ThrowCard(card_id, target, player);
                    }

                    if (!target.Alive || !player.Alive || target.HandcardNum != target.Hp)
                        break;
                    else
                        target = null;
                }
            }
            else if (room.GetTag(Name) is DamageStruct damage && damage.To.Alive)
            {
                room.RemoveTag(Name);
                room.Damage(damage);

                Player victim = damage.To;
                while (!victim.Alive && player.HasFlag(Name))
                {
                    player.SetFlags("-shenfu");

                    victim = room.AskForPlayerChosen(player, room.GetOtherPlayers(player), Name, "@shenfu-damage", true, true, info.SkillPosition);
                    if (victim != null)
                    {
                        DamageStruct _damage = new DamageStruct(Name, player, victim, 1, DamageStruct.DamageNature.Thunder);
                        room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                        room.Damage(_damage);
                    }
                    else
                        break;
                }
            }

            return false;
        }
    }


    public class Qixian : MaxCardsSkill
    {
        public Qixian() : base("qixian")
        {
        }
        public override int GetFixed(Room room, Player target)
        {
            return RoomLogic.PlayerHasShownSkill(room, target, Name) ? 7 : -1;
        }
    }

    public class Chuyuan : TriggerSkill
    {
        public Chuyuan() : base("chuyuan")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventLoseSkill, TriggerEvent.Damaged };
            skill_type = SkillType.Masochism;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventLoseSkill && data is InfoStruct info && info.Info == Name && player.GetPile(Name).Count > 0)
            {
                room.ClearOnePrivatePile(player, Name);
            }
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.Damaged && player.Alive)
            {
                foreach (Player p in RoomLogic.FindPlayersBySkillName(room, Name))
                    if (p.GetPile(Name).Count < p.MaxHp)
                        triggers.Add(new TriggerStruct(Name, p));
            }

            return triggers;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(ask_who, Name, player, info.SkillPosition))
            {
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, ask_who.Name, player.Name);
                room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player target, ref object data, Player ask_who, TriggerStruct info)
        {
            room.DrawCards(target, new DrawCardStruct(1, ask_who, Name));
            if (target.Alive && !target.IsKongcheng() && ask_who.Alive)
            {
                List<int> ids = room.AskForExchange(target, Name, 1, 1, "@chuyuan:" + ask_who.Name, string.Empty, ".", info.SkillPosition);
                room.AddToPile(ask_who, Name, ids, true);
            }

            return false;
        }
    }
    /*
    public class ChuyuanMax : MaxCardsSkill
    {
        public ChuyuanMax() : base("#chuyuan") { }

        public override int GetExtra(Room room, Player target)
        {
            return target.GetPile("chuyuan").Count;
        }
    }
    */
    public class Dengji : PhaseChangeSkill
    {
        public Dengji() : base("dengji")
        {
            frequency = Frequency.Wake;
            skill_type = SkillType.Recover;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player.Phase == PlayerPhase.Start && player.GetMark(Name) == 0 && base.Triggerable(player, room) && player.GetPile("chuyuan").Count > 2)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override bool OnPhaseChange(Room room, Player player, TriggerStruct info)
        {
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            room.DoSuperLightbox(player, info.SkillPosition, Name);
            room.SetPlayerMark(player, Name, 1);
            room.SendCompulsoryTriggerLog(player, Name);

            room.LoseMaxHp(player);
            if (player.Alive && player.GetPile("chuyuan").Count > 0)
            {
                    List<int> ids = player.GetPile("chuyuan");
                    room.ObtainCard(player, ref ids, new CardMoveReason(MoveReason.S_REASON_EXCHANGE_FROM_PILE, player.Name, Name, string.Empty), true);
            }
            if (player.Alive)
                room.HandleAcquireDetachSkills(player, "jianxiong_jx|tianxing", true);

            return false;
        }
    }

    public class Tianxing : PhaseChangeSkill
    {
        public Tianxing() : base("tianxing")
        {
            frequency = Frequency.Wake;
            skill_type = SkillType.Recover;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player.Phase == PlayerPhase.Start && player.GetMark(Name) == 0 && base.Triggerable(player, room) && player.GetPile("chuyuan").Count > 2)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override bool OnPhaseChange(Room room, Player player, TriggerStruct info)
        {
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            room.DoSuperLightbox(player, info.SkillPosition, Name);
            room.SetPlayerMark(player, Name, 1);
            room.SendCompulsoryTriggerLog(player, Name);

            room.LoseMaxHp(player);
            if (player.Alive && player.GetPile("chuyuan").Count > 0)
            {
                List<int> ids = player.GetPile("chuyuan");
                room.ObtainCard(player, ref ids, new CardMoveReason(MoveReason.S_REASON_EXCHANGE_FROM_PILE, player.Name, Name, string.Empty), true);
            }
            if (player.Alive)
            {
                room.HandleAcquireDetachSkills(player, "-chuyuan");
                List<string> choices = new List<string>();
                if (!room.UsedGeneral.Contains("liubei")) choices.Add("rende");
                if (!room.UsedGeneral.Contains("sunquan")) choices.Add("zhiheng_jx");
                if (!room.UsedGeneral.Contains("yuanshao")) choices.Add("luanji_jx");

                if (choices.Count > 0)
                {
                    string choice = room.AskForChoice(player, Name, string.Join("+", choices));
                    switch (choice)
                    {
                        case "rende":
                            room.HandleUsedGeneral("liubei");
                            break;
                        case "zhiheng_jx":
                            room.HandleUsedGeneral("sunquan");
                            break;
                        case "luanji_jx":
                            room.HandleUsedGeneral("yuanshao");
                            break;
                    }

                    room.HandleAcquireDetachSkills(player, choice, true);
                }
            }

            return false;
        }
    }

    public class Duorui : TriggerSkill
    {
        public Duorui() : base("duorui")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseChanging, TriggerEvent.Damage };
            skill_type = SkillType.Attack;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive && player.ContainsTag(Name))
            {
                room.RemovePlayerStringMark(player, Name);
                player.RemoveTag(Name);
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.Damage && base.Triggerable(player, room) && player.Phase == PlayerPhase.Play && data is DamageStruct damage
                && damage.To.Alive && damage.To != player && !damage.To.ContainsTag(Name) && !damage.To.IsDuanchang(true))
            {
                return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is DamageStruct damage && damage.To.Alive && room.AskForSkillInvoke(player, Name, damage.To, info.SkillPosition))
            {
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, damage.To.Name);
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is DamageStruct damage)
            {
                Player target = damage.To;
                List<string> skills = Engine.GetGeneralSkills(target.ActualGeneral1, room.Setting.GameMode, true);
                List<string> choices = new List<string>();
                foreach (string skill in skills)
                {
                    Skill real = Engine.GetSkill(skill);
                    if (real.SkillFrequency != Frequency.Compulsory && !real.Attached_lord_skill)
                        choices.Add(skill);
                }

                if (choices.Count > 0)
                {
                    string choice = room.AskForChoice(player, Name, string.Join("+", choices), new List<string> { "@duorui-skill:" + target.Name }, target);
                    target.SetTag(Name, choice);
                }

               player.SetFlags("Global_PlayPhaseTerminated");
            }

            return false;
        }
    }

    public class DuoruiInvalid : InvalidSkill
    {
        public DuoruiInvalid() : base("#duorui")
        {
        }

        public override bool Invalid(Room room, Player player, string skill)
        {
            Skill s = Engine.GetSkill(skill);
            if (s == null || s.SkillFrequency == Frequency.Compulsory || s.Attached_lord_skill) return false;
            if (player.HasEquip(skill)) return false;

            if (player.ContainsTag("duorui") && player.GetTag("duorui") is string skill_name && skill_name == skill)
                return true;

            return false;
        }
    }

    public class Zhiti : TriggerSkill
    {
        public Zhiti() : base("zhiti")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseProceeding, TriggerEvent.EventPhaseStart };
            skill_type = SkillType.Attack;
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseProceeding && base.Triggerable(player, room) && player.Phase == PlayerPhase.Draw)
            {
                int n = 0;
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (p.IsWounded())
                        n++;
                }
                if (n >= 3)
                    return new TriggerStruct(Name, player);
            }
            else if (triggerEvent == TriggerEvent.EventPhaseStart && base.Triggerable(player, room) && player.Phase == PlayerPhase.Finish)
            {
                int n = 0;
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (p.IsWounded())
                        n++;
                }
                if (n >= 5)
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.EventPhaseProceeding)
            {
                return info;
            }
            else
            {
                List<Player> targets = new List<Player>();
                foreach (Player p in room.GetOtherPlayers(player))
                {
                    for (int i = 0; i < 5; i++)
                    {
                        if (!p.EquipIsBaned(i))
                        {
                            targets.Add(p);
                            break;
                        }
                    }
                }
                if (targets.Count > 0)
                {
                    Player target = room.AskForPlayerChosen(player, targets, Name, "@zhiti-ban", true, true, info.SkillPosition);
                    if (target != null)
                    {
                        room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                        room.SetTag(Name, target);
                        return info;
                    }
                }
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.EventPhaseProceeding && data is int count)
            {
                room.SendCompulsoryTriggerLog(player, Name);
                count++;
                data = count;
            }
            else if (triggerEvent == TriggerEvent.EventPhaseStart && room.GetTag(Name) is Player target)
            {
                room.RemoveTag(Name);
                List<int> indexs = new List<int>();

                for (int i = 0; i < 5; i++)
                {
                    if (!target.EquipIsBaned(i))
                        indexs.Add(i);
                }

                if (indexs.Count > 1)
                    Shuffle.shuffle(ref indexs);

                int n = indexs[0];
                room.AbolisheEquip(target, n, Name);
            }
            return false;
        }
    }

    public class ZhitiMax : MaxCardsSkill
    {
        public ZhitiMax() : base("#zhiti")
        { }

        public override int GetExtra(Room room, Player target)
        {
            int n = 0;
            if (RoomLogic.PlayerHasSkill(room, target, "zhiti"))
            {
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (p.IsWounded())
                    {
                        n = 1;
                        break;
                    }
                }
            }

            if (target.IsWounded())
            {
                foreach (Player p in RoomLogic.FindPlayersBySkillName(room, "zhiti"))
                {
                    if (p != target && RoomLogic.InMyAttackRange(room, p, target))
                        n--;
                }
            }

            return n;
        }
    }
}