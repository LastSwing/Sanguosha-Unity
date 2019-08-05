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
                new LuanjiJX(),
                new Xueyi(),
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
                new Jianying(),
                new JianyingRecord(),
                new Shibei(),
                new Bifa(),
                new Songci(),
                new Jigong(),
                new JigongMax(),
                new Shifei(),
                new QiceJX(),
                new Shefu(),
                new ShefuClear(),
                new Benyu(),
                new DuanliangJX(),
                new DuanliangJXTargetMod(),
                new Jiezhi(),
                new JushouJX(),
                new Jiewei(),
                new Yuanhu(),
                new Mashu("guanyu_sp"),
                new Nuzhan(),
                new NuzhanTM(),
                new Danji(),
                new Shenduan(),
                new ShenduanClear(),
                new Yonglue(),
                new Zhenjun(),
                new JiemingJX(),
            };
            skill_cards = new List<FunctionCard>
            {
                new ShicaiCard(),
                new ZhezhuCard(),
                new ShushouCard(),
                new YuanlueCard(),
                new MoushiCard(),
                new BifaCard(),
                new SongciCard(),
                new ShifeiCard(),
                new ShefuCard(),
                new BenyuCard(),
                new ShenduanCard(),
            };
            related_skills = new Dictionary<string, List<string>>
            {
                { "moushi", new List<string> { "#moushi-draw" } },
                { "jianying", new List<string> { "#jianying-record" } },
                { "shefu", new List<string>{ "#shefu-clear" } },
                { "duanliang_jx", new List<string>{ "#jxduanliang-target" } },
                { "nuzhan", new List<string>{ "#nuzhan" } },
                { "shenduan", new List<string>{ "#shenduan-clear" } },
            };
        }
    }

    //袁绍
    public class LuanjiJX : ViewAsSkill
    {
        public LuanjiJX() : base("luanji_jx")
        {
            response_or_use = true;
            skill_type = SkillType.Alter;
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return true;
        }

        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            if (selected.Count > 1 || room.GetCardPlace(to_select.Id) == Player.Place.PlaceEquip) return false;
            if (selected.Count == 1)
                return selected[0].Suit == to_select.Suit;

            return true;
        }

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count < 2) return null;
            WrappedCard aa = new WrappedCard(ArcheryAttack.ClassName) { Skill = Name };
            aa.AddSubCards(cards);
            return aa;
        }
    }

    public class Xueyi : MaxCardsSkill
    {
        public Xueyi() : base("xueyi")
        {
            lord_skill = true;
        }

        public override int GetExtra(Room room, Player target)
        {
            int count = 0;
            if (RoomLogic.PlayerHasShownSkill(room, target, Name))
            {
                foreach (Player p in room.GetOtherPlayers(target))
                    if (p.Kingdom == "qun")
                        count += 2;
            }

            return count;
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
            handling_method = HandlingMethod.MethodDiscard;
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
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == Player.PlayerPhase.Play && base.Triggerable(player, room))
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
            else if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.From == Player.PlayerPhase.Play)
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
                    room.ObtainCard(lord, ids, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_GIVE, card_use.From.Name, lord.Name, "zhezhu", string.Empty), false);
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
                        RoomLogic.RemovePlayerCardLimitation(p, "use,response", "..$1");
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
                    RoomLogic.SetPlayerCardLimitation(p, "use,response", "..", true);
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
            return to_select.HasFlag(Name);
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
            handling_method = HandlingMethod.MethodNone;
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select.HasFlag("shushou") && to_select != Self;
        }

        public override void OnUse(Room room, CardUseStruct card_use)
        {
            room.SetTag("shushou_target", card_use.To[0]);
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
            foreach (int id in cards)
                room.SetCardFlag(id, Name);

            List<CardsMoveStruct> moves = new List<CardsMoveStruct>();
            while (cards.Count > 0 && friends.Count > 0)
            {
                WrappedCard card = room.AskForUseCard(player, "@@shushou", "@shushou", -1, FunctionCard.HandlingMethod.MethodNone, true, info.SkillPosition);
                if (card != null)
                {
                    Player target = (Player)room.GetTag("shushou_target");
                    room.RemoveTag("shushou_target");
                    target.SetFlags("-" + Name);
                    friends.Remove(target);
                    targets.Add(target);
                    CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_GIVE, player.Name, target.Name, Name, string.Empty);
                    CardsMoveStruct move = new CardsMoveStruct(new List<int>(card.SubCards), target, Player.Place.PlaceHand, reason);
                    moves.Add(move);
                    foreach (int id in card.SubCards)
                    {
                        room.SetCardFlag(id, "-shushou");
                        cards.Remove(id);
                    }
                    /*
                    JsonArray val;
                    QString footname = "@attribute:" + target->objectName();
                    val << footname;
                    val << JsonUtils::toJsonArray(card->getSubcards());
                    room->doNotify(player->getClient(), QSanProtocol::S_COMMAND_UPDATE_CARD_FOOTNAME, val);
                    */
                }
                else
                    break;
            }

            foreach (Player p in room.Players)
                p.SetFlags("-shushou");

            foreach (int id in player.GetCards("he"))
                room.SetCardFlag(id, "-shushou");

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
            handling_method = HandlingMethod.MethodNone;
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 1 && to_select != Self;
        }

        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            return card.SubCards.Count == 1 && targets.Count == 1;
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            Player target = card_use.To[0];
            CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_GIVE, card_use.From.Name, target.Name, "yuanlue", string.Empty);
            room.ObtainCard(target, new List<int>(card_use.Card.SubCards), reason, true);

            int card_id = card_use.Card.SubCards[0];
            if (room.GetCardOwner(card_id) == target && room.GetCardPlace(card_id) == Player.Place.PlaceHand)
            {
                WrappedCard card = room.GetCard(card_id);
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                if (fcard.IsAvailable(room, target, card))
                {
                    WrappedCard use = room.AskForUseCard(target, card_id.ToString(), "@yuanlue:" + card_use.From.Name);

                    if (card != null && card_use.From.Alive)
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
                    if (!p.IsKongcheng())
                    {
                        can_invoke = true;
                        break;
                    }
                }

                return can_invoke ? new TriggerStruct(Name, player) : new TriggerStruct();
            }
            else if (triggerEvent == TriggerEvent.Pindian && data is PindianStruct pd && pd.Reason == Name && pd.From == player && pd.To.Alive)
            {
                int card_id = pd.From_card.Id;
                if (room.GetCardPlace(card_id) == Player.Place.PlaceTable)
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
                    if (!p.IsKongcheng())
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
                room.SetTag(Name, pd.To);
                bool invoke = room.AskForSkillInvoke(player, Name, "fenglue-give", info.SkillPosition);
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
                Player target = pd.To;
                bool success = room.Pindian(pd);
                if (success)
                {
                    List<int> ids = new List<int>();
                    if (!target.IsKongcheng())
                        ids.AddRange(room.AskForExchange(target, Name, 1, 1, "@fenglue-hand:" + player.Name, string.Empty, ".", string.Empty));

                    if (target.HasEquip())
                        ids.AddRange(room.AskForExchange(target, Name, 1, 1, "@fenglue-equip:" + player.Name, string.Empty, ".|.|.|equipped", string.Empty));

                    if (target.JudgingArea.Count > 0)
                        ids.Add(room.AskForCardChosen(target, target, "j", Name));

                    room.ObtainCard(player, ids, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_GIVE, target.Name, player.Name, Name, string.Empty), false);
                }
                else if (!player.IsNude())
                {
                    List<int> ids = room.AskForExchange(player, Name, 1, 1, "@fenglue-fail:" + target.Name, string.Empty, "..", info.SkillPosition);
                    room.ObtainCard(target, ids, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_GIVE, player.Name, target.Name, Name, string.Empty), false);
                }
            }
            else if (data is PindianStruct pindian)
            {
                int card_id = pindian.From_card.Id;
                if (room.GetCardPlace(card_id) == Place.PlaceTable && pindian.To.Alive)
                    room.ObtainCard(pindian.To, card_id);
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
            handling_method = HandlingMethod.MethodNone;
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
            CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_GIVE, player.Name, target.Name, "moushi", string.Empty);
            room.ObtainCard(target, new List<int>(card_use.Card.SubCards), reason, false);

            room.SetPlayerStringMark(target, "moushi", string.Empty);
            room.SetPlayerMark(target, "moushi", 1);
            string key = string.Format("moushi_{0}", target.Name);
            List<Player> _targets = room.ContainsTag(key) ? (List<Player>)room.GetTag(key) : new List<Player>();
            _targets.Add(player);
            room.SetTag(key, _targets);
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
            else if (triggerEvent == TriggerEvent.Damage && player.Phase == Player.PlayerPhase.Play && data is DamageStruct damage)
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
                        new CardMoveReason(CardMoveReason.MoveReason.S_REASON_CHANGE_EQUIP, player.Name));
                    exchangeMove.Add(move1);
                    room.MoveCardsAtomic(exchangeMove, true);
                }
                CardsMoveStruct move2 = new CardsMoveStruct(new List<int> { catapult }, room.GetCardOwner(catapult), player, room.GetCardPlace(catapult),
                                      Player.Place.PlaceEquip, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_PUT, player.Name, Name, string.Empty));
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
                    if (room.GetCardPlace(equipped_id) == Player.Place.PlaceTable)
                    {
                        CardsMoveStruct move3 = new CardsMoveStruct(new List<int> { equipped_id }, null, Player.Place.DiscardPile,
                           new CardMoveReason(CardMoveReason.MoveReason.S_REASON_CHANGE_EQUIP, player.Name));
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
            if (base.Triggerable(player, room) && triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == Player.PlayerPhase.Play)
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
                    room.ObtainCard(player, ids, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_GIVE, target.Name, player.Name, Name, string.Empty), false);
                    if (player.ContainsTag(Name) && player.GetTag(Name) is string card_name && !string.IsNullOrEmpty(card_name))
                    {
                        WrappedCard card = new WrappedCard(card_name);
                        FunctionCard fcard = Engine.GetFunctionCard(card_name);
                        if (fcard != null && fcard.IsAvailable(room, player, card))
                        {
                            room.AskForUseCard(player, "@@choulue", "@choulue-use:::" + card_name, -1, FunctionCard.HandlingMethod.MethodUse, false, info.SkillPosition);
                        }
                    }
                }
            }

            return false;
        }
    }

    //陈琳
    public class BifaCard : SkillCard
    {
        public static string ClassName = "BifaCard";
        public BifaCard() : base(ClassName)
        {
            will_throw = false;
            handling_method = HandlingMethod.MethodNone;
        }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select.GetPile("bifa").Count == 0 && to_select != Self;
        }
        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            return targets.Count == 1 && card.SubCards.Count == 1;
        }
        public override void OnUse(Room room, CardUseStruct card_use)
        {
            Player target = card_use.To[0];
            card_use.From.SetTag("bifa_target", target.Name);
        }
    }
    public class BifaViewAsSkill : OneCardViewAsSkill
    {
        public BifaViewAsSkill() : base("bifa")
        {
            filter_pattern = ".";
            response_pattern = "@@bifa";
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard bf = new WrappedCard(BifaCard.ClassName) { Skill = Name };
            bf.AddSubCard(card);
            return card;
        }
    }

    public class Bifa : TriggerSkill
    {
        public Bifa() : base("bifa")
        {
            events.Add(TriggerEvent.EventPhaseStart);
            view_as_skill = new BifaViewAsSkill();
            skill_type = SkillType.Attack;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (player.Phase == Player.PlayerPhase.RoundStart && player.GetPile("bifa").Count > 0)
            {
                int card_id = player.GetPile("bifa")[0];
                Player chenlin = room.FindPlayer(player.GetTag(string.Format("BifaSource{0}", card_id)).ToString());
                player.RemoveTag(string.Format("BifaSource{0}", card_id));
                List<int> ids = new List<int>
                {
                    card_id
                };

                LogMessage log = new LogMessage
                {
                    Type = "$BifaView",
                    From = player.Name,
                    Card_str = card_id.ToString(),
                    Arg = Name
                };
                room.SendLog(log, player);

                room.FillAG(Name, ids, player);
                FunctionCard cd = Engine.GetFunctionCard(room.GetCard(card_id).Name);
                string pattern = string.Empty;
                if (cd is BasicCard)
                    pattern = "BasicCard";
                else if (cd is TrickCard)
                    pattern = "TrickCard";
                else if (cd is EquipCard)
                    pattern = "EquipCard";
                pattern += "|.|.|hand";
                List<int> to_give = new List<int>();
                if (!player.IsKongcheng() && chenlin != null && chenlin.Alive)
                {
                    to_give = room.AskForExchange(player, Name, 1, 0, "@bifa-give:" + chenlin, string.Empty, pattern, string.Empty);
                    if (to_give.Count == 1)
                    {
                        CardMoveReason reasonG = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_GIVE, player.Name, chenlin.Name, Name, string.Empty);
                        room.ObtainCard(chenlin, to_give, reasonG, false);
                        CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_EXCHANGE_FROM_PILE, player.Name, Name, string.Empty);
                        room.ObtainCard(player, new List<int> { card_id }, reason, false);
                    }
                }

                if (to_give.Count == 0)
                {
                    CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_REMOVE_FROM_PILE, string.Empty, Name, string.Empty);
                    List<int> dis = player.GetPile("bifa");
                    room.ThrowCard(ref dis, reason, null);
                    room.LoseHp(player);
                }
                room.ClearAG(player);
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && player.Phase == Player.PlayerPhase.Finish && !player.IsKongcheng())
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            WrappedCard card = room.AskForUseCard(player, "@@bifa", "@bifa-remove", -1, FunctionCard.HandlingMethod.MethodNone, true, info.SkillPosition);
            if (card != null)
            {
                player.SetMark(Name, card.GetEffectiveId());
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player target = room.FindPlayer(player.GetTag("bifa_target").ToString());
            player.RemoveTag("bifa_target");
            if (target != null && target.Alive)
            {
                int id = player.GetMark(Name);
                target.SetTag(string.Format("BifaSource{0}", id), player.Name);
                room.AddToPile(target, Name, id, false);
            }

            return false;
        }
    }

    public class SongciCard : SkillCard
    {
        public static string ClassName = "SongciCard";
        public SongciCard() : base(ClassName)
        {
        }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select.GetMark(string.Format("songci_{0}", Self.Name)) == 0 && to_select.HandcardNum != to_select.Hp;
        }
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            int handcard_num = effect.To.HandcardNum;
            int hp = effect.To.Hp;
            room.SetPlayerMark(effect.To, "@songci", 1);
            effect.To.SetMark(string.Format("songci_{0}", effect.From.Name), 1);
            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, effect.From, "songci", effect.Card.SkillPosition);
            if (handcard_num > hp)
            {
                room.BroadcastSkillInvoke("songci", "male", 2, gsk.General, gsk.SkinId);
                room.AskForDiscard(effect.To, "songci", 2, 2, false, true);
            }
            else if (handcard_num < hp)
            {
                room.BroadcastSkillInvoke("songci", "male", 1, gsk.General, gsk.SkinId);
                room.DrawCards(effect.To, new DrawCardStruct(2, effect.From, "songci"));
            }
        }
    }

    public class Songci : ZeroCardViewAsSkill
    {
        public Songci() : base("songci")
        {
            skill_type = SkillType.Wizzard;
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            foreach (Player sib in room.GetAlivePlayers())
                if (sib.GetMark("songci_" + player.Name) == 0 && sib.HandcardNum != sib.Hp)
                    return true;
            return false;
        }
        public override WrappedCard ViewAs(Room room, Player player)
        {
            WrappedCard c = new WrappedCard(SongciCard.ClassName) { Skill = Name, Mute = true };
            return c;
        }
    }

    public class Jianying : TriggerSkill
    {
        public Jianying() : base("jianying")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardUsed, TriggerEvent.CardResponded };
            skill_type = SkillType.Replenish;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && player.Phase == Player.PlayerPhase.Play)
            {
                WrappedCard card = null;
                if (triggerEvent == TriggerEvent.CardUsed && data is CardUseStruct use)
                    card = use.Card;
                else if (triggerEvent == TriggerEvent.CardResponded && data is CardResponseStruct resp && resp.Use)
                    card = resp.Card;

                if (card != null)
                {
                    FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                    if (!(fcard is SkillCard) && !(fcard is ImperialOrder))
                    {
                        int suit = player.GetMark("JianyingSuit"), number = player.GetMark("JianyingNumber");
                        if (player.Alive && ((suit > 0 && (int)card.Suit + 1 == suit) || (number > 0 && card.Number == number)))
                            return new TriggerStruct(Name, player);
                    }
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
            if (player.Alive)
                room.DrawCards(player, 1, Name);

            return false;
        }
    }

    public class JianyingRecord : TriggerSkill
    {
        public JianyingRecord() : base("#jianying-record")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardUsed, TriggerEvent.CardResponded, TriggerEvent.EventPhaseChanging };
        }
        public override int GetPriority() => 2;

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (!base.Triggerable(player, room)) return;

            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.From == Player.PlayerPhase.Play)
            {
                player.SetMark("JianyingSuit", 0);
                player.SetMark("JianyingNumber", 0);
                room.RemovePlayerStringMark(player, "jianying_number");
                room.RemovePlayerStringMark(player, "jianying_suit");
            }
            else if ((triggerEvent == TriggerEvent.CardUsed || triggerEvent == TriggerEvent.CardResponded) && player.Phase == Player.PlayerPhase.Play)
            {
                WrappedCard card = null;
                if (triggerEvent == TriggerEvent.CardUsed && data is CardUseStruct use)
                    card = use.Card;
                else if (triggerEvent == TriggerEvent.CardResponded && data is CardResponseStruct resp && resp.Use)
                    card = resp.Card;

                if (card != null)
                {
                    FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                    if (!(fcard is SkillCard) && !(fcard is ImperialOrder))
                    {
                        player.SetMark("JianyingSuit", (int)card.Suit > 3 ? 0 : (int)card.Suit + 1);
                        player.SetMark("JianyingNumber", card.Number);
                        room.SetPlayerStringMark(player, "jianying_suit", WrappedCard.GetSuitString(card.Suit));
                        room.SetPlayerStringMark(player, "jianying_number", card.Number.ToString());
                    }
                }
            }
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class Shibei : TriggerSkill
    {
        public Shibei() : base("shibei")
        {
            events = new List<TriggerEvent> { TriggerEvent.Damaged, TriggerEvent.PreDamageDone, TriggerEvent.EventPhaseChanging };
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Wizzard;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == Player.PlayerPhase.NotActive)
            {
                foreach (Player p in room.GetAlivePlayers())
                    p.SetMark(Name, 0);
            }
            else if (triggerEvent == TriggerEvent.PreDamageDone && base.Triggerable(player, room))
            {
                Player current = room.Current;
                if (current == null || !current.Alive || current.Phase == PlayerPhase.NotActive)
                    return;

                player.AddMark(Name);
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.Damaged && base.Triggerable(player, room))
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (RoomLogic.PlayerHasShownSkill(room, player, Name) || room.AskForSkillInvoke(player, Name, data, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name, true);
            if (player.GetMark(Name) > 0)
            {
                if (player.GetMark(Name) == 1)
                {
                    RecoverStruct recover = new RecoverStruct
                    {
                        Who = player,
                        Recover = 1
                    };
                    room.Recover(player, recover, true);
                }
                else
                    room.LoseHp(player);
            }

            return false;
        }
    }
    //郭图逄纪
    public class Jigong : TriggerSkill
    {
        public Jigong() : base("jigong")
        {
            events = new List<TriggerEvent> { TriggerEvent.DamageDone, TriggerEvent.EventPhaseStart, TriggerEvent.EventPhaseChanging };
            skill_type = SkillType.Replenish;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.DamageDone && data is DamageStruct damage && damage.From != null && damage.From.HasFlag(Name) && damage.From.Phase == Player.PlayerPhase.Play)
            {
                damage.From.AddMark("damage_point_play_phase", damage.Damage);
                room.SetPlayerStringMark(damage.From, Name, damage.From.GetMark("damage_point_play_phase").ToString());
            }
            else if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && player.GetMark("damage_point_play_phase") > 0
                && change.To == Player.PlayerPhase.NotActive)
            {
                player.SetMark("damage_point_play_phase", 0);
                room.RemovePlayerStringMark(player, Name);
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
            if (room.AskForSkillInvoke(player, Name, data, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SetPlayerStringMark(player, Name, "0");
            player.SetFlags(Name);
            room.DrawCards(player, 2, Name);

            return false;
        }
    }

    public class JigongMax : MaxCardsSkill
    {
        public JigongMax() : base("#jigong")
        {
        }
        public override int GetFixed(Room room, Player target)
        {
            if (target.HasFlag("jigong"))
                return target.GetMark("damage_point_play_phase");

            return -1;
        }
    }

    public class ShifeiCard : SkillCard
    {
        public static string ClassName = "ShifeiCard";
        public ShifeiCard() : base(ClassName)
        {
            target_fixed = true;
        }
        public override WrappedCard ValidateInResponse(Room room, Player player, WrappedCard card)
        {
            string response_id = room.GetRoomState().GetCurrentResponseID().ToString();
            player.SetFlags("shifei_" + response_id);

            room.NotifySkillInvoked(player, "shifei");

            Player current = room.Current;
            if (current == null || !current.Alive || current.Phase == Player.PlayerPhase.NotActive)
                return null;

            room.DrawCards(current, new DrawCardStruct(1, player, "shifei"));
            List<Player> mosts = new List<Player>();
            int most = -1;
            foreach (Player p in room.GetAlivePlayers())
            {
                int h = p.HandcardNum;
                if (h > most)
                {
                    mosts.Clear();
                    most = h;
                    mosts.Add(p);
                }
                else if (most == h)
                    mosts.Add(p);
            }

            if (most < 0 || mosts.Contains(current))
                return null;

            List<Player> mosts_copy = new List<Player>(mosts);
            foreach (Player p in mosts_copy)
                if (!RoomLogic.CanDiscard(room, player, p, "he"))
                    mosts.Remove(p);

            if (mosts.Count == 0)
                return null;

            Player vic = room.AskForPlayerChosen(player, mosts, "shifei", "@shifei-dis", false, false, card.SkillPosition);
            // it is impossible that vic == NULL
            if (vic == player)
                room.AskForDiscard(player, "shifei", 1, 1, false, true, "@shifei-disself", false, card.SkillPosition);
            else
            {
                int id = room.AskForCardChosen(player, vic, "he", "shifei", false, HandlingMethod.MethodDiscard);
                room.ThrowCard(id, vic, player);
            }

            room.BroadcastSkillInvoke("shifei", player, card.SkillPosition);
            WrappedCard jink = new WrappedCard(Jink.ClassName) { Skill = "_shifei" };
            return jink;
        }
    }


    public class Shifei : ZeroCardViewAsSkill
    {
        public Shifei() : base("shifei")
        {
            skill_type = SkillType.Wizzard;
        }
        public override bool IsEnabledAtPlay(Room room, Player player) => false;
        public override bool IsEnabledAtResponse(Room room, Player player, string pattern)
        {
            string response_id = room.GetRoomState().GetCurrentResponseID().ToString();
            Player current = room.Current;
            if (pattern == "Jink" && !player.HasFlag("shifei_" + response_id) && current != null)
            {
                WrappedCard jink = new WrappedCard(Jink.ClassName);
                if (Jink.Instance.IsAvailable(room, player, jink) && current.Alive && current.Phase != PlayerPhase.NotActive)
                    return true;
            }

            return false;
        }
        public override WrappedCard ViewAs(Room room, Player player)
        {
            WrappedCard c = new WrappedCard(ShifeiCard.ClassName) { Skill = Name };
            return c;
        }
    }

    //于禁
    public class Zhenjun : TriggerSkill
    {
        public Zhenjun() : base("zhenjun")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart };
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && base.Triggerable(player, room) && player.Phase == Player.PlayerPhase.Start)
            {
                foreach (Player p in room.GetAlivePlayers())
                    if (p.HandcardNum > p.Hp && RoomLogic.CanDiscard(room, player, p, "he"))
                        return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetAlivePlayers())
                if (p.HandcardNum > p.Hp && RoomLogic.CanDiscard(room, player, p, "he"))
                    targets.Add(p);

            if (targets.Count > 0)
            {
                Player to = room.AskForPlayerChosen(player, targets, Name, "@zhenjun-invoke", true, true, info.SkillPosition);
                if (to != null)
                {
                    player.SetTag("zhenjun_target", to.Name);
                    room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                    return info;
                }
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player to = room.FindPlayer(player.GetTag("zhenjun_target").ToString());
            player.RemoveTag("zhenjun_target");
            if (to != null)
            {
                int count = to.HandcardNum - to.Hp;
                List<int> ids = new List<int>();
                if (to == player)
                {
                    ids = room.AskForExchange(player, Name, count, count, "@zhenjun", string.Empty, "..!", info.SkillPosition);
                }
                else
                {
                    List<string> handle = new List<string>();
                    for (int i = 0; i < count; i++)
                        handle.Add("he^false^discard");

                    ids = room.AskForCardsChosen(player, to, handle, Name);
                }
                CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_DISMANTLE, player.Name, to.Name, Name, string.Empty)
                {
                    General = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition)
                };
                room.ThrowCard(ref ids, reason, to, to == player ? null : player);

                if (ids.Count > 0 && player.Alive)
                {
                    int discard_count = 0;
                    foreach (int id in ids)
                    {
                        WrappedCard card = room.GetCard(id);
                        if (!(Engine.GetFunctionCard(card.Name) is EquipCard))
                            discard_count++;
                    }

                    List<string> choices = new List<string>(), prompts = new List<string> { "@zhenjun-choose:" };
                    if (to.Alive)
                    {
                        choices.Add("draw");
                        prompts.Add(string.Format("@zhenjun-draw:{0}::{1}", to.Name, ids.Count));
                    }

                    bool can_discard = false;
                    if (discard_count > 0)
                    {
                        int discard_self = 0;
                        foreach (int id in player.GetCards("he"))
                            if (!RoomLogic.IsJilei(room, player, room.GetCard(id))) discard_self++;

                        if (discard_self >= discard_count) can_discard = true;
                    }

                    if (discard_count > 0)
                    {
                        if (can_discard)
                        {
                            choices.Add("discard");
                            prompts.Add(string.Format("@zhenjun-discard:::{0}", discard_count));
                        }
                    }
                    else
                        choices.Add("cancel");

                    string result = room.AskForChoice(player, Name, string.Join("+", choices), prompts);
                    if (result == "draw" && to.Alive)
                        room.DrawCards(to, new DrawCardStruct(ids.Count, player, Name));
                    else if (result == "discard" && player.Alive)
                        room.AskForDiscard(player, Name, discard_count, discard_count, false, true, string.Format("@zhenjun-discard:::{0}", discard_count), false, info.SkillPosition);
                }
            }

            return false;
        }
    }

    //荀攸
    public class QiceJX : ViewAsSkill
    {
        public QiceJX() : base("qice_jx")
        {
            skill_type = SkillType.Alter;
        }
        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player) => false;
        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count == 1 && cards[0].Id < 0)
            {
                WrappedCard card = new WrappedCard(cards[0].Name) { Skill = Name };
                card.AddSubCards(player.HandCards);
                return card;
            }

            return null;
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            if (!player.IsKongcheng() && !player.HasUsed("ViewAsSkill_qice_jxCard"))
            {
                foreach (int id in player.HandCards)
                {
                    WrappedCard card = room.GetCard(id);
                    if (RoomLogic.IsCardLimited(room, player, card, FunctionCard.HandlingMethod.MethodUse))
                        return false;
                }

                return true;
            }
            return false;
        }
        public override List<WrappedCard> GetGuhuoCards(Room room, List<WrappedCard> cards, Player player)
        {
            return GetGuhuoCards(room, player);
        }

        public static List<WrappedCard> GetGuhuoCards(Room room, Player player)
        {
            List<string> names = GetGuhuoCards(room, "t");
            List<WrappedCard> all_cards = new List<WrappedCard>();
            foreach (string name in names)
            {
                WrappedCard card = new WrappedCard(name);
                card.AddSubCards(player.HandCards);
                card = RoomLogic.ParseUseCard(room, card);
                all_cards.Add(card);
            }
            return all_cards;
        }
    }

    public class BenyuCard : SkillCard
    {
        public static string ClassName = "BenyuCard";
        public BenyuCard() : base(ClassName)
        {
            will_throw = true;
            target_fixed = true;
        }

        public override void OnUse(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            List<int> ids = new List<int>(card_use.Card.SubCards);
            CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_DISCARD, player.Name, "benyu", string.Empty)
            {
                General = RoomLogic.GetGeneralSkin(room, player, "benyu", card_use.Card.SkillPosition)
            };
            room.ThrowCard(ref ids, reason, player, null, "benyu");
        }
    }

    public class BenyuVS : ViewAsSkill
    {
        public BenyuVS() : base("benyu")
        {
            response_pattern = "@@benyu";
        }
        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return !player.HasEquip(to_select.Name) && RoomLogic.CanDiscard(room, player, player, to_select.Id);
        }
        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            DamageStruct damage = (DamageStruct)room.GetTag(Name);
            int count = damage.From.HandcardNum;
            if (cards.Count > count)
            {
                WrappedCard dummy = new WrappedCard(BenyuCard.ClassName) { Skill = Name };
                dummy.AddSubCards(cards);
                return dummy;
            }

            return null;
        }
    }


    public class Benyu : MasochismSkill
    {
        public Benyu() : base("benyu")
        {
            view_as_skill = new BenyuVS();
            skill_type = SkillType.Defense;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is DamageStruct damage && damage.From != null && damage.From.Alive)
            {
                if (damage.From.HandcardNum > player.HandcardNum && player.HandcardNum < 5)
                    return new TriggerStruct(Name, player);
                else if (player.HandcardNum > damage.From.HandcardNum)
                {
                    int count = 0;
                    foreach (int id in player.HandCards)
                        if (RoomLogic.CanDiscard(room, player, player, id)) count++;
                    if (count > damage.From.HandcardNum)
                        return new TriggerStruct(Name, player);
                }
            }

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is DamageStruct damage)
            {
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                bool invoke = false;
                if (damage.From.HandcardNum > player.HandcardNum && player.HandcardNum < 5)
                {
                    if (room.AskForSkillInvoke(player, Name, "@benyu-draw", info.SkillPosition))
                    {
                        room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                        player.SetTag("benyu_type", true);
                        invoke = true;
                    }
                }
                else if (player.HandcardNum > damage.From.HandcardNum)
                {
                    int count = 0;
                    foreach (int id in player.HandCards)
                        if (RoomLogic.CanDiscard(room, player, player, id)) count++;
                    if (count > damage.From.HandcardNum)
                    {
                        room.SetTag(Name, data);
                        WrappedCard card = room.AskForUseCard(player, "@@benyu", string.Format("@benyu::{0}:{1}", damage.From.Name, damage.From.HandcardNum + 1),
                            -1, FunctionCard.HandlingMethod.MethodUse, true, info.SkillPosition);
                        room.RemoveTag(Name);

                        if (card != null)
                        {
                            room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);
                            player.SetTag("benyu_type", false);
                            invoke = true;
                        }
                    }
                }
                if (invoke) return info;
            }

            return new TriggerStruct();
        }
        public override void OnDamaged(Room room, Player target, DamageStruct damage, TriggerStruct info)
        {
            bool type = (bool)target.GetTag("benyu_type");
            target.RemoveTag("benyu_type");
            if (type)
            {
                int count = Math.Min(5 - target.HandcardNum, damage.From.HandcardNum - target.HandcardNum);
                if (count > 0)
                    room.DrawCards(target, count, Name);
            }
            else if (damage.From.Alive)
                room.Damage(new DamageStruct(Name, target, damage.From));
        }
    }

    //徐晃
    public class DuanliangJX : OneCardViewAsSkill
    {
        public DuanliangJX() : base("duanliang_jx")
        {
            filter_pattern = "BasicCard,EquipCard|black";
            response_or_use = true;
            skill_type = SkillType.Alter;
        }
        public override bool IsEnabledAtPlay(Room room, Player player) => true;
        public override bool IsEnabledAtResponse(Room room, Player player, string pattern)
        {
            if (room.GetRoomState().GetCurrentCardUseReason() == CardUseStruct.CardUseReason.CARD_USE_REASON_RESPONSE_USE)
            {
                WrappedCard card = new WrappedCard(SupplyShortage.ClassName);
                if (Engine.MatchExpPattern(room, pattern, player, card)) return true;
            }
            return false;
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard shortage = new WrappedCard(SupplyShortage.ClassName)
            {
                Skill = Name,
                ShowSkill = Name
            };
            shortage.AddSubCard(card);
            shortage = RoomLogic.ParseUseCard(room, shortage);

            return shortage;
        }
    }

    public class DuanliangJXTargetMod : TargetModSkill
    {
        public DuanliangJXTargetMod() : base("#jxduanliang-target")
        {
            pattern = "SupplyShortage";
        }
        public override bool GetDistanceLimit(Room room, Player from, Player to, WrappedCard card)
        {
            if (!Engine.MatchExpPattern(room, pattern, from, card) || to == null)
                return false;

            if (RoomLogic.PlayerHasSkill(room, from, "duanliang_jx") && to.HandcardNum >= from.HandcardNum)
                return true;
            else
                return false;
        }
    }

    public class Jiezhi : TriggerSkill
    {
        public Jiezhi() : base("jiezhi")
        {
            events.Add(TriggerEvent.EventPhaseSkipping);
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Replenish;
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (data is PhaseChangeStruct change && change.To == Player.PlayerPhase.Draw && player.Alive)
            {
                List<Player> xuhuangs = RoomLogic.FindPlayersBySkillName(room, Name);
                foreach (Player p in xuhuangs)
                    if (p != player)
                        triggers.Add(new TriggerStruct(Name, p));
            }

            return triggers;
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(ask_who, Name, true);
            room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
            room.DrawCards(ask_who, 1, Name);
            return false;
        }
    }

    //曹仁
    public class JieweiVS : OneCardViewAsSkill
    {
        public JieweiVS() : base("jiewei")
        {
            filter_pattern = ".|.|.|equipped";
            response_pattern = "Nullification";
            skill_type = SkillType.Alter;
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard ncard = new WrappedCard(Nullification.ClassName) { Skill = Name };
            ncard.AddSubCard(card);
            ncard = RoomLogic.ParseUseCard(room, ncard);
            return ncard;
        }
        public override bool IsEnabledAtNullification(Room room, Player player)
        {
            return player.HasEquip();
        }
    }

    public class JushouJX : PhaseChangeSkill
    {
        public JushouJX() : base("jushou_jx")
        {
            frequency = Frequency.Frequent;
            skill_type = SkillType.Defense;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            return base.Triggerable(player, room) && player.Phase == PlayerPhase.Finish ?
                new TriggerStruct(Name, player) : new TriggerStruct(); ;
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
        public override bool OnPhaseChange(Room room, Player caoren, TriggerStruct info)
        {
            room.TurnOver(caoren);
            room.DrawCards(caoren, 4, Name);

            List<int> ids = room.AskForExchange(caoren, Name, 1, 1, "@jxjushou", null, ".!", info.SkillPosition);
            if (ids.Count == 1)
            {
                WrappedCard card = room.GetCard(ids[0]);
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                if (fcard is EquipCard && fcard.IsAvailable(room, caoren, card))
                    room.UseCard(new CardUseStruct(card, caoren, new List<Player>(), true));
                else
                {
                    GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, caoren, info.SkillPosition);
                    CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_DISCARD, caoren.Name, null, Name, null)
                    {
                        General = gsk
                    };
                    room.ThrowCard(ref ids, reason, caoren, caoren);
                }
            }

            return false;
        }
    }
    public class Jiewei : TriggerSkill
    {
        public Jiewei() : base("jiewei")
        {
            events.Add(TriggerEvent.TurnedOver);
            view_as_skill = new JieweiVS();
            skill_type = SkillType.Wizzard;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            return (base.Triggerable(player, room) && player.FaceUp && player.Alive) ? new TriggerStruct(Name, player) : new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForDiscard(player, Name, 1, 1, true, true, "@jiewei", true, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetAlivePlayers())
            {
                if (p.GetCards("ej").Count > 0)
                    targets.Add(p);
            }
            if (targets.Count > 0)
            {
                Player target1 = room.AskForPlayerChosen(player, targets, Name, "@jiewei1", true, false, info.SkillPosition);
                if (target1 != null)
                {
                    int card_id = room.AskForCardChosen(player, target1, "ej", Name);
                    WrappedCard card = room.GetCard(card_id);
                    Player.Place place = room.GetCardPlace(card_id);

                    FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                    int equip_index = -1;
                    if (place == Player.Place.PlaceEquip)
                    {
                        EquipCard equip = (EquipCard)fcard;
                        equip_index = (int)equip.EquipLocation();
                    }

                    List<Player> tos = new List<Player>();
                    foreach (Player p in room.GetAlivePlayers())
                    {
                        if (equip_index != -1)
                        {
                            if (p.GetEquip(equip_index) < 0)
                                tos.Add(p);
                        }
                        else
                        {
                            if (RoomLogic.IsProhibited(room, player, p, card) == null && !RoomLogic.PlayerContainsTrick(room, p, card.Name))
                                tos.Add(p);
                        }
                    }

                    room.SetTag("MouduanTarget", target1);
                    string position = info.SkillPosition;
                    Player to = room.AskForPlayerChosen(player, tos, Name, "@jiewei-to:::" + card.Name, false, false, position);
                    if (to != null)
                    {
                        room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, target1.Name, to.Name);
                        CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_TRANSFER, player.Name, Name, null);
                        room.MoveCardTo(card, target1, to, place, reason);

                        if (place == Player.Place.PlaceDelayedTrick)
                        {
                            CardUseStruct use = new CardUseStruct(card, null, to);
                            object _data = use;
                            room.RoomThread.Trigger(TriggerEvent.TargetConfirming, room, to, ref _data);
                            CardUseStruct new_use = (CardUseStruct)_data;
                            if (new_use.To.Count == 0)
                                fcard.OnNullified(room, to, card);

                            foreach (Player p in room.GetAllPlayers())
                                room.RoomThread.Trigger(TriggerEvent.TargetConfirmed, room, p, ref _data);
                        }
                    }
                    room.RemoveTag("MouduanTarget");
                }
            }
            return false;
        }
    }

    public class YuanhuViewAsSkill : OneCardViewAsSkill
    {
        public YuanhuViewAsSkill() : base("yuanhu")
        {
            filter_pattern = "EquipCard";
            response_pattern = "@@yuanhu";
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard first = new WrappedCard(HuyuanCard.ClassName) { Skill = Name, Mute = true };
            first.AddSubCard(card);
            return first;
        }
    }

    public class Yuanhu : PhaseChangeSkill
    {
        public Yuanhu() : base("yuanhu")
        {
            view_as_skill = new YuanhuViewAsSkill();
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            return base.Triggerable(player, room) && player.Phase == PlayerPhase.Finish && !player.IsNude()
                ? new TriggerStruct(Name, player)
                : new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            player.RemoveTag("huyuan_equip");
            player.RemoveTag("huyuan_target");
            WrappedCard card = room.AskForUseCard(player, "@@yuanhu", "@huyuan-equip", -1, FunctionCard.HandlingMethod.MethodNone, true, info.SkillPosition);
            if (card != null && player.ContainsTag("huyuan_target"))
            {
                player.SetMark(Name, card.GetEffectiveId());
                return info;
            }

            return new TriggerStruct();
        }
        public override bool OnPhaseChange(Room room, Player caohong, TriggerStruct info)
        {
            Player target = room.FindPlayer((string)caohong.GetTag("huyuan_target"));
            FunctionCard fcard = Engine.GetFunctionCard(room.GetCard(caohong.GetMark(Name)).Name);

            if (fcard != null)
            {
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, caohong, Name, info.SkillPosition);
                if (fcard is Weapon)
                {
                    room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                    List<Player> targets = new List<Player>();
                    foreach (Player p in room.GetAlivePlayers())
                    {
                        if (RoomLogic.DistanceTo(room, target, p) == 1 && RoomLogic.CanDiscard(room, caohong, p, "he"))
                            targets.Add(p);
                    }
                    if (targets.Count > 0)
                    {
                        Player to_dismantle = room.AskForPlayerChosen(caohong, targets, Name, "@huyuan-discard:" + target.Name, true, false, info.SkillPosition);
                        if (to_dismantle != null)
                        {
                            int card_id = room.AskForCardChosen(caohong, to_dismantle, "he", Name, false, FunctionCard.HandlingMethod.MethodDiscard);
                            room.ThrowCard(card_id, to_dismantle, caohong);
                        }
                    }
                }
                else if (fcard is Armor && target != null && target.Alive)
                {
                    room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);
                    room.DrawCards(target, new DrawCardStruct(1, caohong, Name));
                }
                else if (fcard is Horse && target != null && target.Alive && target.IsWounded())
                {
                    room.BroadcastSkillInvoke(Name, "male", 3, gsk.General, gsk.SkinId);
                    RecoverStruct recover = new RecoverStruct
                    {
                        Recover = 1,
                        Who = caohong
                    };
                    room.Recover(target, recover, true);
                }
            }

            return false;
        }
    }

    //sp关羽
    public class Danji : PhaseChangeSkill
    {
        public Danji() : base("danji")
        {
            frequency = Frequency.Wake;
            skill_type = SkillType.Attack;
        }
        public override bool Triggerable(Player player, Room room)
        {
            foreach (Player p in room.GetAlivePlayers())
                if (p.GetRoleEnum() == PlayerRole.Lord && p.Name.Contains("liubei")) return false;

            return base.Triggerable(player, room) && player.Phase == PlayerPhase.Start
                    && player.HandcardNum > player.Hp && player.GetMark(Name) == 0;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SetPlayerMark(player, Name, 1);
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            room.DoSuperLightbox(player, info.SkillPosition, Name);
            return info;
        }
        public override bool OnPhaseChange(Room room, Player player, TriggerStruct info)
        {
            room.LoseMaxHp(player);
            List<string> skills = new List<string>();
            if (info.SkillPosition == "head")
            {
                skills.Add("mashu_guanyu_sp");
                skills.Add("nuzhan");
            }
            else
            {
                skills.Add("mashu_guanyu_sp!");
                skills.Add("nuzhan!");
            }
            room.HandleAcquireDetachSkills(player, skills);

            return false;
        }
    }

    public class Nuzhan : TriggerSkill
    {
        public Nuzhan() : base("nuzhan")
        {
            events.Add(TriggerEvent.ConfirmDamage);
            frequency = Frequency.Compulsory;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is DamageStruct damage && damage.Card != null && !damage.Chain && !damage.Transfer && damage.ByUser)
            {
                FunctionCard fcard = Engine.GetFunctionCard(damage.Card.Name);
                if (fcard is Slash && RoomLogic.IsVirtualCard(room, damage.Card) && damage.Card.SubCards.Count == 1)
                {
                    if (Engine.GetFunctionCard(room.GetCard(damage.Card.GetEffectiveId()).Name) is EquipCard)
                        return new TriggerStruct(Name, player);
                }
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
            room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);
            DamageStruct damage = (DamageStruct)data;
            LogMessage log = new LogMessage
            {
                Type = "#NuzhanBuff",
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

    public class NuzhanTM : TargetModSkill
    {
        public NuzhanTM() : base("#nuzhan")
        {
        }
        public override bool IgnoreCount(Room room, Player from, WrappedCard card)
        {
            if (RoomLogic.PlayerHasSkill(room, from, "nuzhan") && Engine.MatchExpPattern(room, pattern, from, card)
                    && RoomLogic.IsVirtualCard(room, card) && card.SubCards.Count == 1)
            {
                if (Engine.GetFunctionCard(room.GetCard(card.GetEffectiveId()).Name) is TrickCard)
                    return true;
            }

            return false;
        }
        public override void GetEffectIndex(Room room, Player player, WrappedCard card, ModType type, ref int index, ref string skill_name, ref string general_name, ref int skin_id)
        {
            if (type == ModType.History)
                index = 1;
            else
                index = -1;
        }
    }
    //韩屎
    public class ShenduanCard : SkillCard
    {
        public static string ClassName = "ShenduanCard";
        public ShenduanCard() : base(ClassName)
        {
            handling_method = HandlingMethod.MethodUse;
            will_throw = false;
        }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            WrappedCard ss = new WrappedCard(SupplyShortage.ClassName)
            {
                DistanceLimited = false,
                Skill = "_shenduan"
            };
            ss.AddSubCard(card);
            ss = RoomLogic.ParseUseCard(room, ss);

            return SupplyShortage.Instance.TargetFilter(room, targets, to_select, Self, ss);
        }
        public override WrappedCard Validate(Room room, CardUseStruct use)
        {
            Player player = use.From;
            room.NotifySkillInvoked(player, "shenduan");
            room.BroadcastSkillInvoke("shenduan", player, use.Card.SkillPosition);

            WrappedCard ss = new WrappedCard(SupplyShortage.ClassName)
            {
                DistanceLimited = false,
                Skill = "_shenduan"
            };
            ss.AddSubCard(use.Card);
            ss = RoomLogic.ParseUseCard(room, ss);

            return ss;
        }
    }

    public class ShenduanVS : OneCardViewAsSkill
    {
        public ShenduanVS() : base("shenduan")
        {
            expand_pile = "#shenduan";
            response_pattern = "@@shenduan";
        }
        public override bool ViewFilter(Room room, WrappedCard to_select, Player player)
        {
            return player.GetPile("#shenduan").Contains(to_select.Id);
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard ss = new WrappedCard(ShenduanCard.ClassName) { Skill = Name };
            ss.AddSubCard(card);
            return ss;
        }
    }

    public class Shenduan : TriggerSkill
    {
        public Shenduan() : base("shenduan")
        {
            events.Add(TriggerEvent.CardsMoveOneTime);
            view_as_skill = new ShenduanVS();
            skill_type = SkillType.Attack;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (data is CardsMoveOneTimeStruct move && move.From != null && base.Triggerable(move.From, room)
               && (move.Reason.Reason & CardMoveReason.MoveReason.S_MASK_BASIC_REASON) == CardMoveReason.MoveReason.S_REASON_DISCARD && move.To_place == Player.Place.PlaceTable)
            {
                for (int i = 0; i < move.Card_ids.Count; i++)
                {
                    int card_id = move.Card_ids[i];
                    if (room.GetCardPlace(card_id) == Player.Place.PlaceTable && (move.From_places[i] == Player.Place.PlaceHand || move.From_places[i] == Player.Place.PlaceEquip))
                        room.SetCardFlag(card_id, Name);
                }
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is CardsMoveOneTimeStruct move && move.From != null && base.Triggerable(move.From, room)
                   && (move.Reason.Reason & CardMoveReason.MoveReason.S_MASK_BASIC_REASON) == CardMoveReason.MoveReason.S_REASON_DISCARD && move.To_place == Player.Place.DiscardPile)
            {
                for (int i = 0; i < move.Card_ids.Count; i++)
                {
                    int card_id = move.Card_ids[i];
                    WrappedCard card = room.GetCard(card_id);
                    if (card.HasFlag(Name) && WrappedCard.IsBlack(card.Suit) && Engine.GetFunctionCard(card.Name) is BasicCard)
                    {
                        return new TriggerStruct(Name, move.From);
                    }
                }
            }

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            CardsMoveOneTimeStruct move = (CardsMoveOneTimeStruct)data;
            List<int> ids = new List<int>(), _ids;

            foreach (int id in move.Card_ids)
            {
                int index = move.Card_ids.IndexOf(id);
                WrappedCard card = room.GetCard(id);
                if (card.HasFlag(Name) && WrappedCard.IsBlack(card.Suit) && Engine.GetFunctionCard(card.Name) is BasicCard && room.GetCardPlace(id) == Player.Place.DiscardPile)
                    ids.Add(id);
            }

            while (ids.Count > 0)
            {
                ask_who.Piles["#shenduan"] = ids;
                WrappedCard card = room.AskForUseCard(ask_who, "@@shenduan", "@shenduan-use", -1, FunctionCard.HandlingMethod.MethodUse, false, info.SkillPosition);
                ask_who.Piles["#shenduan"] = new List<int>();

                if (card != null)
                    ids.RemoveAll(t => card.SubCards.Contains(t));
                else
                    break;

                _ids = new List<int>(ids);
                foreach (int id in _ids)
                    if (room.GetCardPlace(id) != Place.DiscardPile)
                        ids.Remove(id);
            }

            return new TriggerStruct();
        }
    }

    public class ShenduanClear : TriggerSkill
    {
        public ShenduanClear() : base("#shenduan-clear")
        {
            events.Add(TriggerEvent.CardsMoveOneTime);
        }
        public override int GetPriority() => -1;
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            CardsMoveOneTimeStruct move = (CardsMoveOneTimeStruct)data;
            if (move.From != null)
            {
                for (int i = 0; i < move.Card_ids.Count; i++)
                {
                    WrappedCard card = room.GetCard(move.Card_ids[i]);
                    if (move.From_places[i] == Player.Place.PlaceTable && card.HasFlag("shenduan"))
                        card.SetFlags("-shenduan");
                }
            }
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class Yonglue : TriggerSkill
    {
        public Yonglue() : base("yonglue")
        {
            events = new List<TriggerEvent> { TriggerEvent.PreDamageDone, TriggerEvent.EventPhaseStart };
            skill_type = SkillType.Attack;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.PreDamageDone && data is DamageStruct damage && damage.Card != null && damage.Card.GetSkillName() == Name)
            {
                player.SetFlags(Name);
            }
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Alive && player.Phase == PlayerPhase.Judge && player.JudgingArea.Count > 0)
            {
                List<Player> hss = RoomLogic.FindPlayersBySkillName(room, Name);
                foreach (Player hs in hss)
                {
                    if (hs != player && RoomLogic.InMyAttackRange(room, hs, player))
                        triggers.Add(new TriggerStruct(Name, hs));
                }
            }

            return triggers;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (player.Alive && ask_who.Alive && player.JudgingArea.Count > 0 && RoomLogic.InMyAttackRange(room, ask_who, player)
                && room.AskForSkillInvoke(ask_who, Name, player, info.SkillPosition))
            {
                int id = room.AskForCardChosen(ask_who, player, "j", Name, false, FunctionCard.HandlingMethod.MethodDiscard);
                CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_DISMANTLE, ask_who.Name, player.Name, Name, string.Empty)
                {
                    General = RoomLogic.GetGeneralSkin(room, ask_who, Name, info.SkillPosition)
                };
                List<int> ids = new List<int> { id };
                room.ThrowCard(ref ids, reason, player, ask_who);
                room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            WrappedCard slash = new WrappedCard(Slash.ClassName) { Skill = "_yonglue", SkillPosition = info.SkillPosition };
            slash.UserString = ask_who.Name;

            if (Engine.IsProhibited(room, ask_who, player, slash) == null)
                room.UseCard(new CardUseStruct(slash, ask_who, player));

            if (!ask_who.HasFlag(Name))
                room.DrawCards(ask_who, 1, Name);

            ask_who.SetFlags("-yonglue");

            return false;
        }
    }

    public class JiemingJX : MasochismSkill
    {
        public JiemingJX() : base("jieming_jx")
        {
            skill_type = SkillType.Masochism;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player != null && player.Alive && RoomLogic.PlayerHasSkill(room, player, Name) && data is DamageStruct damage)
            {
                TriggerStruct trigger = new TriggerStruct(Name, player);
                trigger.Times = damage.Damage;
                return trigger;
            }

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (!player.Alive)
                return new TriggerStruct();

            Player target = room.AskForPlayerChosen(player, room.GetAlivePlayers(), Name, "jieming-invoke", true, true, info.SkillPosition);
            if (target != null)
            {
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                room.BroadcastSkillInvoke(Name, "male", (target == player ? 2 : 1), gsk.General, gsk.SkinId);

                List<string> target_list = player.ContainsTag("jieming_target") ? (List<string>)player.GetTag("jieming_target") : new List<string>();
                target_list.Add(target.Name);
                player.SetTag("jieming_target", target_list);

                return info;
            }
            return new TriggerStruct();
        }
        public override void OnDamaged(Room room, Player player, DamageStruct damage, TriggerStruct info)
        {
            List<string> target_list = (List<string>)player.GetTag("jieming_target");
            string target_name = target_list[target_list.Count - 1];
            target_list.RemoveAt(target_list.Count - 1);
            player.SetTag("jieming_target", target_list);

            Player to = room.FindPlayer(target_name);

            if (to != null)
            {
                int upper = Math.Min(5, to.MaxHp);
                int x = upper - to.HandcardNum;
                if (x > 0)
                    room.DrawCards(to, new DrawCardStruct(x, player, Name));
            }
        }
    }
}
