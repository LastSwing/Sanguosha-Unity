using CommonClass;
using CommonClass.Game;
using CommonClassLibrary;
using SanguoshaServer.Game;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using static CommonClass.Game.CardUseStruct;
using static CommonClass.Game.Player;

namespace SanguoshaServer.Package
{
    public class ClassicYing : GeneralPackage
    {
        public ClassicYing() : base("ClassicYing")
        {
            skills = new List<Skill>
            {
                new Jianxiang(),
                new Shenshi(),
                new ShenshiDraw(),
                new Zhengu(),
                new ZhenguEnd(),
                new Zhengrong(),
                new Hongju(),
                new Qingce(),

                new Chenglue(),
                new ChenglueTar(),
                new ShicaiJX(),
                //new ShicaiJXRecord(),
                new Cunmu(),
                new Tushe(),
                new Limu(),
                new LimuTar(),
                new Mingren(),
                new MingrenClear(),
                new Zhenliang(),
                new Xiongluan(),
                new XiongluanTar(),
                new CongjianJX(),

                new Zuilun(),
                new Fuyin(),
                new Juzhan(),
                new JuzhanPro(),
                new Feijun(),
                new Binglue(),
                new Wanglie(),
                new WanglieResp(),
                new WanglieTar(),

                new Kongsheng(),
                new KongshengClear(),
                new Liangyin(),
                new Qianjie(),
                new Jueyan(),
                new JueyanTar(),
                new JueyanMax(),
                new Poshi(),
                new Huairou(),
                new Kuizhu(),
                new Chezheng(),
                new ChezhengProhibit(),
                new Lijun(),
                new Huaiju(),
                new HuaijuDetach(),
                new Yili(),
                new Zhenglun(),
            };

            skill_cards = new List<FunctionCard>
            {
                new ChenglueCard(),
                new LimuCard(),
                new JueyanCard(),
                new HuairouCard(),
                new KuizhuCard(),
                new ShenshiCard(),
                new ZhenliangCard(),
                new FeijunCard(),
                new QingceCard(),
                new XiongluanCard(),
                new CongjianCard(),
            };

            related_skills = new Dictionary<string, List<string>>
            {
                { "chenglue", new List<string>{ "#chenglue-tar" } },
                { "limu", new List<string>{ "#limu-tar" } },
                { "jueyan", new List<string>{ "#jueyan-target", "#jueyan-max" } },
                { "kongsheng", new List<string>{ "#kongsheng-clear" } },
                { "chezheng", new List<string>{ "#chezheng-pro" } },
                { "huaiju", new List<string>{ "#huaiju-clear" } },
                { "juzhan", new List<string>{ "#juzhan-prohibit" } },
                { "shenshi", new List<string>{ "#shenshi" } },
                { "mingren", new List<string>{ "#mingren-clear" } },
                { "wanglie", new List<string>{ "#wanglie", "#wanglie-tar" } },
                { "zhengu", new List<string>{ "#zhengu" } },
                { "xiongluan", new List<string>{ "#xiongluan" } },
                //{ "shicai_jx", new List<string>{ "#shicai_jx" } },
            };
        }
    }

    public class Jianxiang : TriggerSkill
    {
        public Jianxiang() : base("jianxiang")
        {
            events.Add(TriggerEvent.TargetConfirmed);
            skill_type = SkillType.Replenish;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is CardUseStruct use && use.From != player && base.Triggerable(player, room))
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard.TypeID != FunctionCard.CardType.TypeSkill)
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            int count = 1000;
            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetAlivePlayers())
                if (p.HandcardNum < count)
                    count = p.HandcardNum;

            foreach (Player p in room.GetAlivePlayers())
                if (p.HandcardNum == count) targets.Add(p);

            Player target = room.AskForPlayerChosen(player, targets, Name, "@jianxiang", true, true, info.SkillPosition);
            if (target != null)
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                room.SetTag(Name, target);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player target = (Player)room.GetTag(Name);
            room.RemoveTag(Name);
            room.DrawCards(target, new DrawCardStruct(1, player, Name));

            return false;
        }
    }
    public class Shenshi : TriggerSkill
    {
        public Shenshi() : base("shenshi")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseChanging, TriggerEvent.EventAcquireSkill, TriggerEvent.EventLoseSkill, TriggerEvent.Damaged, TriggerEvent.Death };
            view_as_skill = new ShenshiVS();
            turn = true;

            priority = new Dictionary<TriggerEvent, double>
            {
                { TriggerEvent.EventPhaseChanging, 2 },
                { TriggerEvent.EventAcquireSkill,3 },
                { TriggerEvent.EventLoseSkill,3 },
                { TriggerEvent.Damaged, 3 },
                { TriggerEvent.Death, 3 },
                //{ TriggerEvent.CardsMoveOneTime,3 },
            };
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventAcquireSkill && data is InfoStruct info && info.Info == Name)
            {
                room.SetTurnSkillState(player, Name, false, info.Head ? "head" : "deputy");
            }
            else if (triggerEvent == TriggerEvent.EventLoseSkill && data is InfoStruct _info && _info.Info == Name)
            {
                room.RemoveTurnSkill(player);
            }
            //else if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.From != null
            //    && move.From.ContainsTag(Name) && move.From.GetTag(Name) is Dictionary<int, string> names)
            //{
            //    foreach (int id in move.Card_ids)
            //        if (names.ContainsKey(id))
            //            names.Remove(id);

            //    move.From.SetTag(Name, names);
            //}
            else if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
                foreach (Player p in room.GetAlivePlayers())
                    if (p.ContainsTag(Name))
                        p.RemoveTag(Name);
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.Death && data is DeathStruct death && death.Damage.From != null && death.Damage.Reason == Name && base.Triggerable(death.Damage.From, room)
                && !death.Damage.Chain && !death.Damage.Transfer)
            {
                return new TriggerStruct(Name, death.Damage.From);
            }
            else if (triggerEvent == TriggerEvent.Damaged && base.Triggerable(player, room) && player.GetMark(Name) == 1 && data is DamageStruct damage && damage.From != null
                && damage.From.Alive && damage.From != player && !damage.From.IsKongcheng())
            {
                return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.Death)
            {
                List<Player> targets = new List<Player>();
                foreach (Player p in room.GetAlivePlayers())
                    if (p.HandcardNum < 4) targets.Add(p);

                if (targets.Count > 0)
                {
                    Player target = room.AskForPlayerChosen(ask_who, targets, Name, "@shenshi", true, true, info.SkillPosition);
                    if (target != null)
                    {
                        room.SetTag(Name, target);
                        //room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                        return info;
                    }
                }
            }
            else if (triggerEvent == TriggerEvent.Damaged && data is DamageStruct damage && damage.From.Alive)
            {
                if (room.AskForSkillInvoke(player, Name, damage.From, info.SkillPosition))
                {
                    room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, ask_who.Name, damage.From.Name);
                    GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, ask_who, Name, info.SkillPosition);
                    room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);
                    return info;
                }
            }


            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.Death)
            {
                Player target = (Player)room.GetTag(Name);
                room.RemoveTag(Name);

                room.DrawCards(target, new DrawCardStruct(4 - target.HandcardNum, ask_who, Name));
            }
            else if (data is DamageStruct damage)
            {
                if (!damage.From.IsKongcheng()) room.ShowAllCards(damage.From, player, Name);
                if (!player.IsNude())
                {
                    damage.From.SetFlags(Name);
                    List<int> ids = room.AskForExchange(ask_who, Name, 1, 1, "@shenshi-give:" + damage.From.Name, string.Empty, "..", info.SkillPosition);
                    damage.From.SetFlags("-shenshi");
                    if (ids.Count > 0)
                    {
                        Dictionary<int, string> names = damage.From.ContainsTag(Name) ? (Dictionary<int, string>)damage.From.GetTag(Name) : new Dictionary<int, string>();
                        names[ids[0]] = ask_who.Name;
                        damage.From.SetTag(Name, names);
                        room.ObtainCard(damage.From, ref ids, new CardMoveReason(MoveReason.S_REASON_GIVE, damage.From.Name, player.Name, Name, string.Empty), false);
                    }
                }

                player.SetMark(Name, 0);
                if (player.Alive)
                    room.SetTurnSkillState(player, Name, false, info.SkillPosition);
            }

            return false;
        }
    }

    public class ShenshiDraw : TriggerSkill
    {
        public ShenshiDraw() : base("#shenshi")
        {
            events.Add(TriggerEvent.EventPhaseChanging);
            frequency = Frequency.Compulsory;
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
            {
                List<Player> targets = new List<Player>();
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (p.ContainsTag("shenshi") && p.GetTag("shenshi") is Dictionary<int, string> names)
                    {
                        foreach (int id in names.Keys)
                        {
                            if (room.GetCardOwner(id) == p && (room.GetCardPlace(id) == Place.PlaceHand || room.GetCardPlace(id) == Place.PlaceEquip))
                            {
                                Player target = room.FindPlayer(names[id]);
                                if (target != null && !targets.Contains(target))
                                {
                                    targets.Add(target);
                                    if (base.Triggerable(target, room) && target.HandcardNum < 4) triggers.Add(new TriggerStruct(Name, target));
                                }
                            }
                        }
                    }
                }
            }

            return triggers;
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.NotifySkillInvoked(ask_who, "shenshi");
            room.DrawCards(ask_who, 4 - ask_who.HandcardNum, "shenshi");

            return false;
        }
    }

    public class ShenshiVS : OneCardViewAsSkill
    {
        public ShenshiVS() : base("shenshi")
        {
            filter_pattern = "..";
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(ShenshiCard.ClassName) && !player.IsNude() && player.GetMark(Name) == 0;
        }

        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard ss = new WrappedCard(ShenshiCard.ClassName) { Skill = Name, Mute = true };
            ss.AddSubCard(card);
            return ss;
        }
    }

    public class ShenshiCard : SkillCard
    {
        public static string ClassName = "ShenshiCard";
        public ShenshiCard() : base(ClassName) { will_throw = false; }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            int count = 0;
            foreach (Player p in room.GetAlivePlayers())
                if (p.HandcardNum > count) count = p.HandcardNum;
            return targets.Count == 0 && to_select != Self && to_select.HandcardNum == count;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From, target = card_use.To[0];
            player.SetMark("shenshi", 1);

            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, "shenshi", card_use.Card.SkillPosition);
            room.BroadcastSkillInvoke("shenshi", "male", 1, gsk.General, gsk.SkinId);

            List<int> ids = new List<int>(card_use.Card.SubCards);
            room.ObtainCard(target, ref ids, new CardMoveReason(MoveReason.S_REASON_GIVE, player.Name, target.Name, "shenshi", string.Empty), false);
            room.Damage(new DamageStruct("shenshi", player, target));

            if (player.Alive)
                room.SetTurnSkillState(player, "shenshi", true, card_use.Card.SkillPosition);
        }
    }

    public class Zhengu : TriggerSkill
    {
        public Zhengu() : base("zhengu")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart, TriggerEvent.EventPhaseChanging };
            priority = new Dictionary<TriggerEvent, double>
            {
                { TriggerEvent.EventPhaseStart, 3 },
                { TriggerEvent.EventPhaseChanging, 2 },
            };
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive && player.ContainsTag(Name))
            {
                player.RemoveTag(Name);
                room.RemovePlayerStringMark(player, Name);
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Finish && base.Triggerable(player, room))
            {
                return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetOtherPlayers(player))
                if (!p.ContainsTag(Name)) targets.Add(p);

            if (targets.Count > 0)
            {
                Player target = room.AskForPlayerChosen(player, targets, Name, "@zhengu", true, true, info.SkillPosition);
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
            Player target = (Player)room.GetTag(Name);
            room.RemoveTag(Name);
            target.SetTag(Name, player.Name);
            target.SetFlags(Name);
            room.SetPlayerStringMark(target, Name, string.Empty);

            return false;
        }
    }

    public class ZhenguEnd : TriggerSkill
    {
        public ZhenguEnd() : base("#zhengu")
        {
            events.Add(TriggerEvent.EventPhaseChanging);
            frequency = Frequency.Compulsory;
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
            {
                if (base.Triggerable(player, room))
                {
                    foreach (Player p in room.GetOtherPlayers(player))
                    {
                        if (p.HasFlag("zhengu"))
                        {
                            triggers.Add(new TriggerStruct(Name, player));
                            break;
                        }
                    }
                }

                if (player.Alive && player.ContainsTag("zhengu") && player.GetTag("zhengu") is string haozhao)
                {
                    Player owner = room.FindPlayer(haozhao);
                    if (base.Triggerable(owner, room) && owner.HandcardNum != player.HandcardNum)
                        triggers.Add(new TriggerStruct(Name, owner));
                }
            }

            return triggers;
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player target = player;
            if (target == ask_who)
            {
                foreach (Player p in room.GetOtherPlayers(ask_who))
                {
                    if (p.HasFlag("zhengu"))
                    {
                        target = p;
                        break;
                    }
                }
            }

            room.NotifySkillInvoked(ask_who, "zhengu");
            room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, ask_who.Name, target.Name);

            int target_num = ask_who.HandcardNum;
            if (target.HandcardNum > target_num)
                room.AskForDiscard(target, "zhengu", target.HandcardNum - target_num, target.HandcardNum - target_num, false, false,
                    string.Format("@zhengu-discard:{0}::{1}", ask_who.Name, target_num), false);
            else if (target.HandcardNum < target_num && target.HandcardNum < 5)
            {
                int count = Math.Min(5, target_num);
                room.DrawCards(target, new DrawCardStruct(count - target.HandcardNum, ask_who, "zhengu"));
            }

            return false;
        }
    }
    /*
    public class Zhengrong : TriggerSkill
    {
        public Zhengrong() : base("zhengrong")
        {
            events.Add(TriggerEvent.Damage);
            skill_type = SkillType.Attack;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is DamageStruct damage && base.Triggerable(player, room) && player != damage.To && damage.To.HandcardNum > player.HandcardNum)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is DamageStruct damage && room.AskForSkillInvoke(player, Name, damage.To, info.SkillPosition))
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
                int id = room.AskForCardChosen(player, damage.To, "he", Name);
                room.AddToPile(player, Name, id);
            }

            return false;
        }
    }
    */
    public class Zhengrong : TriggerSkill
    {
        public Zhengrong() : base("zhengrong")
        {
            events.Add(TriggerEvent.TargetChosen);
            skill_type = SkillType.Attack;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is CardUseStruct use && base.Triggerable(player, room) && use.To.Count > 0
                && (use.Card.Name.Contains(Slash.ClassName) || use.Card.Name == ArcheryAttack.ClassName || use.Card.Name == Duel.ClassName
                || use.Card.Name == FireAttack.ClassName || use.Card.Name == SavageAssault.ClassName))
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use)
            {
                List<Player> targets = new List<Player>();
                foreach (Player p in use.To)
                    if (p != player && !p.IsNude() && p.HandcardNum >= player.HandcardNum)
                        targets.Add(p);

                if (targets.Count > 0)
                {
                    Player target = room.AskForPlayerChosen(player, targets, Name, "@zhengrong", true, true, info.SkillPosition);
                    if (target != null)
                    {
                        room.SetTag(Name, target);
                        room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                        return info;
                    }
                }
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player target = (Player)room.GetTag(Name);
            room.RemoveTag(Name);
            int id = room.AskForCardChosen(player, target, "he", Name);
            room.AddToPile(player, Name, id);

            return false;
        }
    }
    public class Hongju : PhaseChangeSkill
    {
        public Hongju() : base("hongju")
        {
            frequency = Frequency.Wake;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player.Phase == PlayerPhase.Start && base.Triggerable(player, room) && player.GetMark(Name) == 0 && player.GetPile("zhengrong").Count >= 3)
                //&& room.AliveCount() < room.Players.Count)
            {
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override bool OnPhaseChange(Room room, Player player, TriggerStruct info)
        {
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            room.DoSuperLightbox(player, info.SkillPosition, Name);
            room.SetPlayerMark(player, Name, 1);
            room.SendCompulsoryTriggerLog(player, Name);

            List<int> ups = player.GetCards("h"), downs = player.GetPile("zhengrong");
            AskForMoveCardsStruct move = room.AskForMoveCards(player, ups, downs, true, Name, downs.Count, downs.Count, true, false, new List<int>(), info.SkillPosition);
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
                        To_pile_name = "zhengrong"
                    };

                    moves.Add(move1);
                    moves.Add(move2);
                    room.MoveCardsAtomic(moves, false);
                }
            }

            if (player.Alive)
            {
                room.LoseMaxHp(player);

                if (player.Alive)
                    room.HandleAcquireDetachSkills(player, "qingce", true);
            }


            return false;
        }
    }

    public class Qingce : OneCardViewAsSkill
    {
        public Qingce() : base("qingce")
        {
            filter_pattern = ".|.|.|zhengrong";
            expand_pile = "zhengrong";
            skill_type = SkillType.Attack;
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return player.GetPile("zhengrong").Count > 0;
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard shun = new WrappedCard(QingceCard.ClassName)
            {
                Skill = Name,
                ShowSkill = Name
            };
            shun.AddSubCard(card);
            return shun;
        }
    }

    public class QingceCard : SkillCard
    {
        public static string ClassName = "QingceCard";
        public QingceCard() : base(ClassName)
        {
            will_throw = false;
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && RoomLogic.CanDiscard(room, Self, to_select, "ej");
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From, target = card_use.To[0];
            List<int> ids = new List<int>(card_use.Card.SubCards);
            room.ObtainCard(player, ref ids, new CardMoveReason(MoveReason.S_REASON_REMOVE_FROM_PILE, player.Name, "qingce", string.Empty));
            room.AskForDiscard(player, "qingce", 1, 1, false, true, "@qingce-discard", false, card_use.Card.SkillPosition);

            room.SetEmotion(player, "dismantlement");
            if (!RoomLogic.CanDiscard(room, player, target, "ej"))
                return;

            int card_id = room.AskForCardChosen(player, target, "ej", "qingce", false, HandlingMethod.MethodDiscard);
            room.ThrowCard(card_id, room.GetCardPlace(card_id) == Place.PlaceDelayedTrick ? null : target, player != target ? player : null);
        }
    }

    public class Chenglue : TriggerSkill
    {
        public Chenglue() : base("chenglue")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseChanging, TriggerEvent.EventAcquireSkill, TriggerEvent.EventLoseSkill };
            view_as_skill = new ChenglueVS();
            turn = true;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive && player.ContainsTag(Name))
            {
                player.RemoveTag(Name);
                room.RemovePlayerStringMark(player, Name);
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

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class ChenglueVS : ZeroCardViewAsSkill
    {
        public ChenglueVS() : base("chenglue")
        {
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(ChenglueCard.ClassName);
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            WrappedCard cl = new WrappedCard(ChenglueCard.ClassName)
            {
                Skill = Name
            };
            return cl;
        }
    }

    public class ChenglueTar : TargetModSkill
    {
        public ChenglueTar() : base("#chenglue-tar", false)
        {
            pattern = "BasicCard,TrickCard";
        }

        public override bool GetDistanceLimit(Room room, Player from, Player to, WrappedCard card, CardUseReason reason, string pattern)
        {
            if (from != null && from.ContainsTag("chenglue") && from.GetTag("chenglue") is List<WrappedCard.CardSuit> suits && suits.Contains(RoomLogic.GetCardSuit(room, card)))
            {
                return true;
            }

            return false;
        }

        public override int GetResidueNum(Room room, Player from, WrappedCard card)
        {
            if (from != null && from.ContainsTag("chenglue") && from.GetTag("chenglue") is List<WrappedCard.CardSuit> suits && suits.Contains(RoomLogic.GetCardSuit(room, card)))
            {
                return 999;
            }

            return 0;
        }
    }

    public class ChenglueCard : SkillCard
    {
        public static string ClassName = "ChenglueCard";
        public ChenglueCard() : base(ClassName)
        {
            will_throw = true;
            target_fixed = true;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            int count = player.GetMark("chenglue") == 0 ? 1 : 2;
            player.SetMark("chenglue", count == 1 ? 1 : 0);

            room.DrawCards(player, count, "chenglue");
            if (player.Alive)
            {
                List<int> ids = new List<int>();
                foreach (int id in player.GetCards("h"))
                    if (RoomLogic.CanDiscard(room, player, player, id))
                        ids.Add(id);

                if (ids.Count > (count == 1 ? 2 : 1))
                    ids = room.AskForExchange(player, "chenglue", count == 1 ? 2 : 1, count == 1 ? 2 : 1,
                        string.Format("@chenglue-discard:::{0}", count == 1 ? 2 : 1), string.Empty, ".!", card_use.Card.SkillPosition);

                room.ThrowCard(ref ids, player);

                string mark = player.StringMarks.ContainsKey("chenglue") ? player.StringMarks["chenglue"] : string.Empty;
                List<WrappedCard.CardSuit> discards = player.ContainsTag("chenglue") ? (List<WrappedCard.CardSuit>)player.GetTag("chenglue") : new List<WrappedCard.CardSuit>();
                foreach (int id in ids)
                {
                    WrappedCard.CardSuit suit = room.GetCard(id).Suit;
                    string suit_string = WrappedCard.GetSuitIcon(suit);
                    if (!mark.Contains(suit_string)) mark += suit_string;
                    if (!discards.Contains(suit)) discards.Add(suit);
                }

                if (player.Alive && !string.IsNullOrEmpty(mark))
                {
                    player.SetTag("chenglue", discards);
                    room.SetPlayerStringMark(player, "chenglue", mark);
                }
            }

            if (player.Alive)
                room.SetTurnSkillState(player, "chenglue", count == 1, card_use.Card.SkillPosition);
        }
    }

    public class ShicaiJX : TriggerSkill
    {
        public ShicaiJX() : base("shicai_jx")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseChanging, TriggerEvent.CardFinished, TriggerEvent.TargetConfirmed };
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
            {
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (p.ContainsTag(Name))
                        p.RemoveTag(Name);
                }
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.CardFinished && data is CardUseStruct use && base.Triggerable(player, room))
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                List<int> ids = room.GetSubCards(use.Card);
                if (!(fcard is SkillCard) && ids.SequenceEqual(use.Card.SubCards) && ids.Count > 0)
                {
                    bool dis = true;
                    foreach (int id in use.Card.SubCards)
                    {
                        if (room.GetCardPlace(id) != Place.DiscardPile)
                        {
                            dis = false;
                            break;
                        }
                    }

                    FunctionCard.CardType type = fcard.TypeID;
                    List<FunctionCard.CardType> types = player.ContainsTag(Name) ? (List<FunctionCard.CardType>)player.GetTag(Name) : new List<FunctionCard.CardType>();
                    if (dis && !types.Contains(type))
                        return new TriggerStruct(Name, player);
                }
            }
            else if (triggerEvent == TriggerEvent.TargetConfirmed && data is CardUseStruct _use && base.Triggerable(player, room))
            {
                FunctionCard fcard = Engine.GetFunctionCard(_use.Card.Name);
                List<FunctionCard.CardType> types = player.ContainsTag(Name) ? (List<FunctionCard.CardType>)player.GetTag(Name) : new List<FunctionCard.CardType>();
                List<int> ids = room.GetSubCards(_use.Card);
                if (fcard is EquipCard && !types.Contains(FunctionCard.CardType.TypeEquip) && ids.Count > 0 && ids.SequenceEqual(_use.Card.SubCards)
                    && room.GetCardPlace(_use.Card.GetEffectiveId()) == Place.PlaceTable)
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                FunctionCard.CardType type = fcard.TypeID;
                List<FunctionCard.CardType> types = player.ContainsTag(Name) ? (List<FunctionCard.CardType>)player.GetTag(Name) : new List<FunctionCard.CardType>();
                if (!types.Contains(type))
                {
                    types.Add(type);
                    player.SetTag(Name, types);
                }

                List<int> dis = room.GetSubCards(use.Card);
                if (dis.Count > 0 && room.AskForSkillInvoke(player, Name, data, info.SkillPosition))
                {
                    if (dis.Count > 1)
                    {
                        AskForMoveCardsStruct result = room.AskForMoveCards(player, dis, new List<int>(), false, Name, dis.Count, 0, false, true, new List<int>(), info.SkillPosition);
                        if (result.Success && result.Top.Count == dis.Count)
                            dis = result.Top;
                    }

                    room.RemoveSubCards(use.Card);
                    room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                    CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_PUT, player.Name, Name, string.Empty);
                    CardsMoveStruct move = new CardsMoveStruct(dis, null, Place.DrawPile, reason)
                    {
                        To_pile_name = string.Empty,
                        From = null
                    };

                    List<CardsMoveStruct> moves = new List<CardsMoveStruct> { move };
                    room.MoveCardsAtomic(moves, true);

                    return info;
                }
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.DrawCards(player, 1, Name);
            return false;
        }
    }

    public class ShicaiJXRecord : TriggerSkill
    {
        public ShicaiJXRecord() : base("#shicai_jx")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardFinished, TriggerEvent.TargetConfirmed };
        }

        public override int GetPriority() => 2;

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardFinished && data is CardUseStruct use && base.Triggerable(player, room))
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                FunctionCard.CardType type = fcard.TypeID;
                List<FunctionCard.CardType> types = player.ContainsTag("shicai_jx") ? (List<FunctionCard.CardType>)player.GetTag("shicai_jx") : new List<FunctionCard.CardType>();
                if (!types.Contains(type))
                {
                    types.Add(type);
                    player.SetTag("shicai_jx", types);
                }
            }
            else if (triggerEvent == TriggerEvent.TargetConfirmed && data is CardUseStruct _use && base.Triggerable(player, room))
            {
                FunctionCard fcard = Engine.GetFunctionCard(_use.Card.Name);
                List<FunctionCard.CardType> types = player.ContainsTag("shicai_jx") ? (List<FunctionCard.CardType>)player.GetTag("shicai_jx") : new List<FunctionCard.CardType>();
                List<int> ids = room.GetSubCards(_use.Card);
                if (fcard is EquipCard && !types.Contains(FunctionCard.CardType.TypeEquip))
                {
                    types.Add(FunctionCard.CardType.TypeEquip);
                    player.SetTag("shicai_jx", types);
                }
            }
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class Cunmu : TriggerSkill
    {
        public Cunmu() : base("cunmu")
        {
            events.Add(TriggerEvent.CardDrawing);
            skill_type = SkillType.Replenish;
            frequency = Frequency.Compulsory;
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name);
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            if (data is List<int> ids)
            {
                int count = ids.Count;
                ids.Clear();
                for (int i = 0; i < count; i++)
                    ids.Add(room.DrawPile[room.DrawPile.Count - 1 - i]);

                data = ids;
            }

            return false;
        }
    }

    public class Tushe : TriggerSkill
    {
        public Tushe() : base("tushe")
        {
            events = new List<TriggerEvent> { TriggerEvent.TargetChosen };
            skill_type = SkillType.Replenish;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is CardUseStruct use && use.Card != null)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (!(fcard is EquipCard) && !(fcard is SkillCard))
                {
                    bool check = true;
                    foreach (int id in player.GetCards("h"))
                    {
                        WrappedCard card = room.GetCard(id);
                        if (Engine.GetFunctionCard(card.Name) is BasicCard)
                        {
                            check = false;
                            break;
                        }
                    }
                    if (check)
                        return new TriggerStruct(Name, player);
                }
            }
            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            bool check = true;
            foreach (int id in player.GetCards("h"))
            {
                WrappedCard card = room.GetCard(id);
                if (Engine.GetFunctionCard(card.Name) is BasicCard)
                {
                    check = false;
                    break;
                }
            }
            if (check && room.AskForSkillInvoke(player, Name, data, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use)
                room.DrawCards(player, use.To.Count, Name);

            return false;
        }
    }

    public class Limu : OneCardViewAsSkill
    {
        public Limu() : base("limu")
        {
            filter_pattern = ".|diamond";
            response_or_use = true;
            skill_type = SkillType.Alter;
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            WrappedCard card = new WrappedCard(Indulgence.ClassName);
            return !RoomLogic.PlayerContainsTrick(room, player, Indulgence.ClassName) && RoomLogic.IsProhibited(room, player, player, card) == null && player.JudgingAreaAvailable;
        }

        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard indulgence = new WrappedCard(LimuCard.ClassName);
            indulgence.AddSubCard(card);
            return indulgence;
        }

        public override void GetEffectIndex(Room room, Player player, WrappedCard card, ref int index, ref string skill_name, ref string general_name, ref int skin_id)
        {
            index = 1;
        }
    }

    public class LimuCard : SkillCard
    {
        public static string ClassName = "LimuCard";
        public LimuCard() : base(ClassName)
        {
            will_throw = false;
            target_fixed = true;
        }

        public override void OnUse(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            WrappedCard indulgence = new WrappedCard(Indulgence.ClassName)
            {
                Skill = "limu",
                ShowSkill = "limu"
            };
            indulgence.AddSubCard(card_use.Card.GetEffectiveId());
            indulgence = RoomLogic.ParseUseCard(room, indulgence);
            room.UseCard(new CardUseStruct(indulgence, player, player));

            if (player.IsWounded())
            {
                RecoverStruct recover = new RecoverStruct
                {
                    Who = player,
                    Recover = 1
                };
                room.Recover(player, recover, true);
            }
        }
    }

    public class LimuTar : TargetModSkill
    {
        public LimuTar() : base("#limu-tar")
        {
            pattern = "BasicCard,TrickCard";
        }

        public override bool GetDistanceLimit(Room room, Player from, Player to, WrappedCard card, CardUseReason reason, string pattern)
        {
            if (from != null && to != null && RoomLogic.PlayerHasShownSkill(room, from, "limu") && from != to
                && RoomLogic.InMyAttackRange(room, from, to, card) && from.JudgingArea.Count > 0)
                return true;

            return false;
        }

        public override bool CheckSpecificAssignee(Room room, Player from, Player to, WrappedCard card, string pattern)
        {
            if (from != null && to != null && RoomLogic.PlayerHasShownSkill(room, from, "limu") && RoomLogic.InMyAttackRange(room, from, to, card) && from != to
                && from.JudgingArea.Count > 0)
                return true;

            return false;
        }

        public override void GetEffectIndex(Room room, Player player, WrappedCard card, ModType type, ref int index, ref string skill_name, ref string general_name, ref int skin_id)
        {
            index = -2;
        }
    }

    public class Mingren : TriggerSkill
    {
        public Mingren() : base("mingren")
        {
            events = new List<TriggerEvent> { TriggerEvent.GameStart, TriggerEvent.EventPhaseEnd };
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.GameStart && base.Triggerable(player, room))
                return new TriggerStruct(Name, player);
            else if (triggerEvent == TriggerEvent.EventPhaseEnd && player.Phase == PlayerPhase.Finish && base.Triggerable(player, room)
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
                room.DrawCards(player, 1, Name);
                List<int> ids = room.AskForExchange(player, Name, 1, 1, "@mingren-pile", string.Empty, ".", info.SkillPosition);
                room.AddToPile(player, Name, ids);
            }
            else
            {
                List<int> stars = player.GetPile(Name);
                if (stars.Count > 0 && player.HandcardNum > 0)
                {
                    List<int> ups = player.GetCards("h");
                    AskForMoveCardsStruct move = room.AskForMoveCards(player, ups, stars, true, Name, 1, 1, false, false, new List<int>(), info.SkillPosition);
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
            }

            return false;
        }
    }

    public class MingrenClear : DetachEffectSkill
    {
        public MingrenClear() : base("mingren", "mingren") { }
    }

    public class Zhenliang : TriggerSkill
    {
        public Zhenliang() : base("zhenliang")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardsMoveOneTime, TriggerEvent.EventAcquireSkill, TriggerEvent.EventLoseSkill };
            view_as_skill = new ZhenliangVS();
            turn = true;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventAcquireSkill && data is InfoStruct info && info.Info == Name)
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
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.To_place == Place.DiscardPile
                && ((move.Reason.Reason & MoveReason.S_MASK_BASIC_REASON) == MoveReason.S_REASON_USE
                || move.Reason.Reason == MoveReason.S_REASON_RESPONSE))
            {
                Player from = room.FindPlayer(move.Reason.PlayerId);
                if (base.Triggerable(from, room) && from.Phase == PlayerPhase.NotActive && from.GetPile("mingren").Count > 0 && from.GetMark(Name) > 0)
                {
                    WrappedCard mr = room.GetCard(from.GetPile("mingren")[0]);
                    WrappedCard use = move.Reason.Card;
                    if (Engine.GetFunctionCard(mr.Name).TypeID == Engine.GetFunctionCard(use.Name).TypeID)
                        return new TriggerStruct(Name, from);
                }
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player target = room.AskForPlayerChosen(ask_who, room.GetAlivePlayers(), Name, "@zhenliang", true, true, info.SkillPosition);
            if (target != null)
            {
                room.SetTag(Name, target);
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player target = (Player)room.GetTag(Name);
            room.RemoveTag(Name);

            ask_who.SetMark(Name, 0);
            room.SetTurnSkillState(ask_who, Name, false, info.SkillPosition);

            room.DrawCards(target, new DrawCardStruct(1, ask_who, Name));

            return false;
        }
    }

    public class ZhenliangVS : ViewAsSkill
    {
        public ZhenliangVS() : base("zhenliang")
        {
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return player.GetPile("mingren").Count > 0 && !player.HasUsed(ZhenliangCard.ClassName) && player.GetMark(Name) == 0 && !player.IsNude();
        }

        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            List<int> pile = player.GetPile("mingren");
            bool black = WrappedCard.IsBlack(room.GetCard(pile[0]).Suit);

            return !RoomLogic.IsCardLimited(room, player, to_select, FunctionCard.HandlingMethod.MethodDiscard) && WrappedCard.IsBlack(to_select.Suit) == black;
        }

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count >= 1)
            {
                WrappedCard zl = new WrappedCard(ZhenliangCard.ClassName) { Skill = Name };
                zl.AddSubCards(cards);
                return zl;
            }

            return null;
        }
    }

    public class ZhenliangCard : SkillCard
    {
        public static string ClassName = "ZhenliangCard";
        public ZhenliangCard() : base(ClassName) { }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            if (targets.Count > 0 || to_select == Self || !RoomLogic.InMyAttackRange(room, Self, to_select, card))
                return false;

            int count = Math.Max(1,  Math.Abs(Self.Hp - to_select.Hp));
            return card.SubCards.Count == count;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From, target = card_use.To[0];
            player.SetMark("zhenliang", 1);
            room.SetTurnSkillState(player, "zhenliang", true, card_use.Card.SkillPosition);

            room.Damage(new DamageStruct("zhenliang", player, target));
        }
    }

    public class Xiongluan : TriggerSkill
    {
        public Xiongluan() : base("xiongluan")
        {
            events.Add(TriggerEvent.EventPhaseChanging);
            limit_mark = "@luan";
            frequency = Frequency.Limited;
            skill_type = SkillType.Alter;
            view_as_skill = new XiongluanVS();
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
            {
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (p.HasFlag(Name))
                    {
                        RoomLogic.RemovePlayerCardLimitation(p, Name);
                        room.RemovePlayerStringMark(p, Name);
                    }
                }
            }
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class XiongluanVS : ZeroCardViewAsSkill
    {
        public XiongluanVS() : base("xiongluan")
        {
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return player.GetMark("@luan") > 0;
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard(XiongluanCard.ClassName);
        }
    }

    public class XiongluanCard : SkillCard
    {
        public static string ClassName = "XiongluanCard";
        public XiongluanCard() : base(ClassName)
        {
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select != Self;
        }

        public override void OnUse(Room room, CardUseStruct card_use)
        {
            room.SetPlayerMark(card_use.From, "@luan", 0);
            room.BroadcastSkillInvoke("xiongluan", card_use.From, card_use.Card.SkillPosition);
            room.DoSuperLightbox(card_use.From, card_use.Card.SkillPosition, "xiongluan");

            base.OnUse(room, card_use);
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From, target = card_use.To[0];
            for (int i = 0; i < 5; i++)
                room.AbolisheEquip(player, i, "xiongluan");

            room.AbolishJudgingArea(player, "xiongluan");

            target.SetFlags("xiongluan");

            string pattern = ".|.|.|hand$0";
            room.SetPlayerStringMark(target, "xiongluan", string.Empty);
            RoomLogic.SetPlayerCardLimitation(target, "xiongluan", "use,response", pattern, false);
        }
    }

    public class XiongluanTar : TargetModSkill
    {
        public XiongluanTar() : base("#xiongluan", false) { pattern = "."; }

        public override bool GetDistanceLimit(Room room, Player from, Player to, WrappedCard card, CardUseReason reason, string pattern)
        {
            return from != null && to != null && from == room.Current && to.HasFlag("xiongluan");
        }

        public override bool CheckSpecificAssignee(Room room, Player from, Player to, WrappedCard card, string pattern)
        {
            return from != null && to != null && from == room.Current && to.HasFlag("xiongluan");
        }
    }

    public class CongjianJX : TriggerSkill
    {
        public CongjianJX() : base("congjian_jx")
        {
            events = new List<TriggerEvent> { TriggerEvent.TargetConfirming };
            skill_type = SkillType.Replenish;
            view_as_skill = new CongjianVS();
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is CardUseStruct use && use.To.Count > 1 && base.Triggerable(player, room))
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard.TypeID == FunctionCard.CardType.TypeTrick)
                    foreach (Player p in use.To)
                        if (p != player) return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use)
            {
                room.SetTag(Name, data);
                room.AskForUseCard(player, "@@congjian_jx", "@congjian_jx:::" + use.Card.Name, null, -1, FunctionCard.HandlingMethod.MethodUse, true, info.SkillPosition);
                room.RemoveTag(Name);
            }

            return new TriggerStruct();
        }
    }

    public class CongjianVS : OneCardViewAsSkill
    {
        public CongjianVS() : base("congjian_jx")
        {
            response_pattern = "@@congjian_jx";
            filter_pattern = "..";
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return false;
        }

        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard cj = new WrappedCard(CongjianCard.ClassName) { Skill = Name };
            cj.AddSubCard(card);
            return cj;
        }
    }

    public class CongjianCard : SkillCard
    {
        public static string ClassName = "CongjianCard";
        public CongjianCard() : base(ClassName)
        {
            will_throw = false;
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            if (room.GetTag("congjian_jx") is CardUseStruct use)
                return targets.Count == 0 && Self != to_select && use.To.Contains(to_select);

            return false;
        }

        public override void OnUse(Room room, CardUseStruct card_use)
        {
            room.RemoveTag("congjian_jx");
            base.OnUse(room, card_use);
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From, target = card_use.To[0];
            bool equip = false;
            FunctionCard fcard = Engine.GetFunctionCard(room.GetCard(card_use.Card.GetEffectiveId()).Name);
            if (fcard.TypeID == CardType.TypeEquip)
                equip = true;

            List<int> ids = new List<int>(card_use.Card.SubCards);
            room.ObtainCard(target, ref ids, new CardMoveReason(MoveReason.S_REASON_GIVE, player.Name, target.Name, "congjian_jx", string.Empty), false);

            if (player.Alive)
                room.DrawCards(player, equip ? 2 : 1, "congjian_jx");
        }
    }

    public class Zuilun : TriggerSkill
    {
        public Zuilun() : base("zuilun")
        {
            events = new List<TriggerEvent> { TriggerEvent.DamageComplete, TriggerEvent.CardsMoveOneTime, TriggerEvent.EventPhaseStart };
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.DamageComplete && data is DamageStruct damage && damage.From != null && room.Current == damage.From && damage.From.Alive)
                damage.From.SetFlags("zuilun_damage");
            else if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.From != null && move.From == room.Current && move.From.Alive
                && (move.Reason.Reason & MoveReason.S_MASK_BASIC_REASON) == MoveReason.S_REASON_DISCARD)
                move.From.SetFlags("zuilun_discard");
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && base.Triggerable(player, room) && player.Phase == PlayerPhase.Finish)
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

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player zhuge, ref object data, Player ask_who, TriggerStruct info)
        {
            List<int> ids = room.GetNCards(3, true);
            int count = 0;
            if (zhuge.HasFlag("zuilun_damage")) count++;
            if (!zhuge.HasFlag("zuilun_discard")) count++;
            int hand = 1000;
            foreach (Player p in room.GetAlivePlayers())
                if (p.HandcardNum < hand) hand = p.HandcardNum;
            if (zhuge.HandcardNum == hand) count++;

            if (count == 3)
            {
                room.ObtainCard(zhuge, ref ids, new CardMoveReason(MoveReason.S_REASON_GOTCARD, zhuge.Name, Name, string.Empty), false);
            }
            else
            {
                LogMessage log = new LogMessage
                {
                    Type = "$ViewDrawPile",
                    From = zhuge.Name,
                    Card_str = string.Join("+", JsonUntity.IntList2StringList(ids))
                };
                room.SendLog(log, zhuge);
                log.Type = "$ViewDrawPile2";
                log.Arg = ids.Count.ToString();
                log.Card_str = null;
                room.SendLog(log, new List<Player> { zhuge });

                AskForMoveCardsStruct result = room.AskForMoveCards(zhuge, ids, new List<int>(), true, Name, count, count, false, true, new List<int>(), info.SkillPosition);
                List<int> top_cards = new List<int>(), bottom_cards = new List<int>();
                if (result.Success && result.Bottom.Count == count)
                {
                    top_cards = result.Top;
                    bottom_cards = result.Bottom;
                }
                else
                {
                    for (int i = 0; i < ids.Count; i++)
                    {
                        if (i < count)
                            bottom_cards.Add(ids[i]);
                        else
                            top_cards.Add(ids[i]);
                    }
                }

                if (bottom_cards.Count > 0)
                    room.ObtainCard(zhuge, ref bottom_cards, new CardMoveReason(MoveReason.S_REASON_GOTCARD, zhuge.Name, Name, string.Empty), false);

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

                room.ReturnToDrawPile(top_cards, false, zhuge);

                if (count == 0)
                {
                    Player target = room.AskForPlayerChosen(zhuge, room.GetOtherPlayers(zhuge), Name, "@zuilun", false, true, info.SkillPosition);
                    room.LoseHp(zhuge);
                    if (target.Alive)
                        room.LoseHp(target);
                }
            }

            return false;
        }
    }

    public class Fuyin : TriggerSkill
    {
        public Fuyin() : base("fuyin")
        {
            events = new List<TriggerEvent> { TriggerEvent.TargetConfirmed, TriggerEvent.EventPhaseChanging };
            skill_type = SkillType.Defense;
            frequency = Frequency.Compulsory;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
            {
                foreach (Player p in room.GetAlivePlayers())
                    if (p.GetMark(Name) > 0) p.SetMark(Name, 0);
            }
            else if (triggerEvent == TriggerEvent.TargetConfirmed && data is CardUseStruct use)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is Slash || fcard is Duel)
                    player.AddMark(Name);
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is CardUseStruct use && use.From != null && use.From.Alive
                && use.From != player && player.GetMark(Name) == 1 && player.HandcardNum < use.From.HandcardNum)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is Slash || fcard is Duel)
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (player.Alive && data is CardUseStruct use)
            {
                room.SendCompulsoryTriggerLog(player, Name);
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                int index = 0;
                for (int i = 0; i < use.EffectCount.Count; i++)
                {
                    CardBasicEffect effect = use.EffectCount[i];
                    if (effect.To == player)
                    {
                        index++;
                        if (index == info.Times)
                        {
                            effect.Nullified = true;
                            use.EffectCount[i] = effect;
                            data = use;
                            break;
                        }
                    }
                }
            }

            return false;
        }
    }

    public class Juzhan : TriggerSkill
    {
        public Juzhan() : base("juzhan")
        {
            events = new List<TriggerEvent> { TriggerEvent.TargetConfirmed, TriggerEvent.TargetChosen, TriggerEvent.EventAcquireSkill, TriggerEvent.EventLoseSkill };
            turn = true;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {

            if (triggerEvent == TriggerEvent.EventAcquireSkill && data is InfoStruct info && info.Info == Name)
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
            if (triggerEvent == TriggerEvent.TargetConfirmed && data is CardUseStruct use && use.From.Alive
                && use.Card.Name.Contains(Slash.ClassName) && base.Triggerable(player, room) && player.GetMark(Name) == 0)
            {
                return new TriggerStruct(Name, player);
            }
            else if (triggerEvent == TriggerEvent.TargetChosen && data is CardUseStruct _use && _use.Card.Name.Contains(Slash.ClassName)
                && base.Triggerable(player, room) && player.GetMark(Name) > 0)
            {
                List<Player> targets = new List<Player>();
                foreach (Player p in _use.To)
                {
                    if (!p.IsNude() && RoomLogic.CanGetCard(room, player, p, "he"))
                        targets.Add(p);
                }

                if (targets.Count > 0)
                    return new TriggerStruct(Name, player, targets);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SetTag(Name, data);
            int mark = 1;
            bool invoke = false;
            if (triggerEvent == TriggerEvent.TargetConfirmed && data is CardUseStruct use && use.From.Alive)
            {
                invoke = room.AskForSkillInvoke(player, Name, use.From, info.SkillPosition);
            }
            else if (triggerEvent == TriggerEvent.TargetChosen && ask_who.Alive && player.Alive && !player.IsNude()
                && RoomLogic.CanGetCard(room, ask_who, player, "he") && ask_who.GetMark(Name) > 0)
            {
                invoke = room.AskForSkillInvoke(ask_who, Name, player, info.SkillPosition);
                mark = 2;
            }
            room.RemoveTag(Name);

            if (invoke)
            {
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, ask_who, Name, info.SkillPosition);
                room.BroadcastSkillInvoke(Name, "male", mark, gsk.General, gsk.SkinId);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.TargetConfirmed && data is CardUseStruct use)
            {
                room.DrawCards(player, 1, Name);
                if (use.From.Alive)
                    room.DrawCards(use.From, 1, Name);

                if (player.Alive)
                {
                    ask_who.SetMark(Name, 1);
                    room.SetTurnSkillState(player, Name, true, info.SkillPosition);
                }

                if (player.Alive && use.From.Alive)
                {
                    use.From.SetFlags("juzhan_from");
                    player.SetFlags("juzhan_to");
                }
            }
            else
            {
                int id = room.AskForCardChosen(ask_who, player, "he", Name, false, FunctionCard.HandlingMethod.MethodGet);
                List<int> ids = new List<int> { id };
                room.ObtainCard(ask_who, ref ids, new CardMoveReason(MoveReason.S_REASON_EXTRACTION, ask_who.Name, player.Name, Name, string.Empty), false);

                if (ask_who.Alive)
                {
                    room.SetTurnSkillState(ask_who, Name, false, info.SkillPosition);
                    ask_who.SetMark(Name, 0);
                }

                if (player.Alive && ask_who.Alive)
                {
                    ask_who.SetFlags("juzhan_from");
                    player.SetFlags("juzhan_to");
                }
            }

            return false;
        }
    }

    public class JuzhanPro : ProhibitSkill
    {
        public JuzhanPro() : base("#juzhan-prohibit") { }

        public override bool IsProhibited(Room room, Player from, Player to, WrappedCard card, List<Player> others = null)
        {
            if (from != null && to != null && card != null && Engine.GetFunctionCard(card.Name).TypeID != FunctionCard.CardType.TypeSkill && from.HasFlag("juzhan_from")
                && to.HasFlag("juzhan_to"))
                return true;

            return false;
        }
    }

    public class Feijun : OneCardViewAsSkill
    {
        public Feijun() : base("feijun")
        {
            filter_pattern = "..!";
        }

        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard fj = new WrappedCard(FeijunCard.ClassName) { Skill = Name };
            fj.AddSubCard(card);
            return fj;
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.IsNude() && !player.HasUsed(FeijunCard.ClassName);
        }
    }

    public class FeijunCard : SkillCard
    {
        public static string ClassName = "FeijunCard";
        public FeijunCard() : base(ClassName)
        {
            target_fixed = true;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;

            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (p.HandcardNum > player.HandcardNum || p.GetEquips().Count > player.GetEquips().Count)
                    targets.Add(p);
            }

            if (targets.Count > 0)
            {
                Player target = room.AskForPlayerChosen(player, targets, "feijun", "@feijun", false, true, card_use.Card.SkillPosition);
                List<string> choices = new List<string>();
                if (target.HandcardNum > player.HandcardNum) choices.Add("handcard");
                if (target.GetEquips().Count > player.GetEquips().Count) choices.Add("equip");

                string mark = string.Format("feijun_{0}", player.Name);
                if (target.GetMark(mark) == 0 && RoomLogic.PlayerHasSkill(room, player, "binglue"))
                {
                    room.NotifySkillInvoked(player, "binglue");
                    room.BroadcastSkillInvoke("binglue", player);
                    room.DrawCards(player, 2, "binglue");
                }
                target.AddMark(mark);

                string choice = room.AskForChoice(player, "feijun", string.Join("+", choices), new List<string> { "@to-player:" + target.Name }, target);
                if (choice == "handcard")
                {
                    List<int> ids = room.AskForExchange(target, "feijun", 1, 1, "@feijun-give:" + player.Name, string.Empty, "..", string.Empty);
                    room.ObtainCard(player, ref ids, new CardMoveReason(MoveReason.S_REASON_GIVE, target.Name, player.Name, "feijun", string.Empty), false);
                }
                else
                {
                    List<int> ids = room.AskForExchange(target, "feijun", 1, 1, "@feijun-disacard:" + player.Name, string.Empty, ".|.|.|equipped!", string.Empty);
                    room.ThrowCard(ids[0], target);
                }
            }
        }
    }

    public class Binglue : Skill
    {
        public Binglue() : base("binglue") { frequency = Frequency.Compulsory; }
    }

    public class Wanglie : TriggerSkill
    {
        public Wanglie() : base("wanglie")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardUsedAnnounced, TriggerEvent.CardUsed, TriggerEvent.EventPhaseChanging };
            skill_type = SkillType.Attack;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardUsedAnnounced && data is CardUseStruct use && player.Phase == PlayerPhase.Play && !player.HasFlag(Name))
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard.TypeID != FunctionCard.CardType.TypeSkill)
                    player.SetFlags(Name);
            }
            else if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.From == PlayerPhase.Play && player.HasFlag(Name))
                player.SetFlags("-wanglie");
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.CardUsed && data is CardUseStruct use && player.Phase == PlayerPhase.Play && base.Triggerable(player, room))
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard.TypeID != FunctionCard.CardType.TypeSkill && !(fcard is DelayedTrick) && use.Card.Name != Analeptic.ClassName && use.Card.Name != Peach.ClassName)
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
            if (data is CardUseStruct use)
            {
                use.Card.SetFlags(string.Format("{0}_{1}", Name, player.Name));
                RoomLogic.SetPlayerCardLimitation(player, Name, "use", ".$1");
            }

            return false;
        }
    }

    public class WanglieResp : TriggerSkill
    {
        public WanglieResp() : base("#wanglie")
        {
            frequency = Frequency.Compulsory;
            events = new List<TriggerEvent> { TriggerEvent.TargetChosen, TriggerEvent.TrickCardCanceling };
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.TargetChosen && data is CardUseStruct use && player.Alive && player.Phase == PlayerPhase.Play
                && use.Card.HasFlag(string.Format("wanglie_{0}", player.Name)))
            {
                if (use.Card.Name.Contains(Slash.ClassName) || use.Card.Name == Duel.ClassName || use.Card.Name == Collateral.ClassName
                    || use.Card.Name == ArcheryAttack.ClassName || use.Card.Name == SavageAssault.ClassName)
                {
                    return new TriggerStruct(Name, player, use.To);
                }
            }
            else if (triggerEvent == TriggerEvent.TrickCardCanceling && data is CardEffectStruct effect && effect.From != null && effect.From.Alive
                && effect.From.Phase == PlayerPhase.Play && effect.Card.HasFlag(string.Format("wanglie_{0}", effect.From.Name)))
            {
                return new TriggerStruct(Name, effect.From);
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.TargetChosen && data is CardUseStruct chose_use)
            {
                int index = 0;
                for (int i = 0; i < chose_use.EffectCount.Count; i++)
                {
                    CardBasicEffect effect = chose_use.EffectCount[i];
                    if (effect.To == player)
                    {
                        index++;
                        if (index == info.Times)
                        {
                            effect.Effect2 = 0;
                            data = chose_use;
                            break;
                        }
                    }
                }
            }
            else if (triggerEvent == TriggerEvent.TrickCardCanceling)
                return true;

            return false;
        }
    }

    public class WanglieTar : TargetModSkill
    {
        public WanglieTar() : base("#wanglie-tar") { pattern = "."; }

        public override bool GetDistanceLimit(Room room, Player from, Player to, WrappedCard card, CardUseReason reason, string pattern)
        {
            if (from != null && to != null && from.Phase == PlayerPhase.Play && RoomLogic.PlayerHasSkill(room, from, "wanglie") && !from.HasFlag("wanglie"))
                return true;

            return false;
        }

        public override void GetEffectIndex(Room room, Player player, WrappedCard card, ModType type, ref int index, ref string skill_name, ref string general_name, ref int skin_id)
        {
            index = -2;
        }
    }

    public class Liangyin : TriggerSkill
    {
        public Liangyin() : base("liangyin")
        {
            events.Add(TriggerEvent.CardsMoveOneTime);
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (data is CardsMoveOneTimeStruct move)
            {
                if ((move.To_place == Place.PlaceSpecial && !move.From_places.Contains(Place.PlaceSpecial) && !move.From_places.Contains(Place.PlaceUnknown))
                    || (move.To != null && move.To_place == Place.PlaceHand && move.From_places.Contains(Place.PlaceSpecial)))
                {
                    List<Player> zhoufeis = RoomLogic.FindPlayersBySkillName(room, Name);
                    foreach (Player p in zhoufeis)
                        triggers.Add(new TriggerStruct(Name, p));
                }
            }

            return triggers;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardsMoveOneTimeStruct move)
            {
                List<Player> targets = new List<Player>();
                string prompt = "@liangyin-discard";

                if (move.To_place == Place.PlaceSpecial)
                {
                    prompt = "@liangyin-draw";
                    foreach (Player p in room.GetOtherPlayers(ask_who))
                    {
                        if (p.HandcardNum > ask_who.HandcardNum)
                            targets.Add(p);
                    }
                }
                else
                {
                    foreach (Player p in room.GetOtherPlayers(ask_who))
                    {
                        if (p.HandcardNum < ask_who.HandcardNum && RoomLogic.CanDiscard(room, ask_who, p, "he"))
                            targets.Add(p);
                    }
                }

                if (targets.Count > 0)
                {
                    Player target = room.AskForPlayerChosen(ask_who, targets, Name, prompt, true, true, info.SkillPosition);
                    if (target != null)
                    {
                        ask_who.SetTag(Name, target.Name);
                        room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                        return info;
                    }
                }
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player target = room.FindPlayer(ask_who.GetTag(Name).ToString());
            ask_who.RemoveTag(Name);
            if (data is CardsMoveOneTimeStruct move)
            {
                if (move.To_place == Place.PlaceSpecial)
                {
                    room.DrawCards(target, new DrawCardStruct(1, ask_who, Name));
                }
                else
                {
                    room.AskForDiscard(target, Name, 1, 1, false, true, "@liangyin-discard-self:" + ask_who.Name);
                }
            }

            return false;
        }
    }

    public class Kongsheng : PhaseChangeSkill
    {
        public Kongsheng() : base("kongsheng")
        {
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && ((player.Phase == PlayerPhase.Start && !player.IsNude()) || (player.Phase == PlayerPhase.Finish && player.GetPile(Name).Count > 0)))
            {
                return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            bool invoke = false;
            if (player.Phase == PlayerPhase.Start && !player.IsNude())
            {
                List<int> ids = room.AskForExchange(player, Name, player.GetCardCount(true), 0, "@kongsheng", string.Empty, "..", info.SkillPosition);
                if (ids.Count > 0)
                {
                    invoke = true;
                    player.SetTag(Name, ids);
                }
            }
            else if (player.Phase == PlayerPhase.Finish)
                invoke = true;

            if (invoke)
            {
                room.NotifySkillInvoked(player, Name);
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool OnPhaseChange(Room room, Player player, TriggerStruct info)
        {
            if (player.Phase == PlayerPhase.Start)
            {
                List<int> ids = (List<int>)player.GetTag(Name);
                player.RemoveTag(Name);
                room.AddToPile(player, Name, ids);
            }
            else
            {
                List<int> ids = new List<int>();
                foreach (int id in player.GetPile(Name))
                {
                    WrappedCard card = room.GetCard(id);
                    FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                    if (fcard is EquipCard && fcard.IsAvailable(room, player, card))
                        ids.Add(id);
                }

                while (ids.Count > 0)
                {
                    if (ids.Count == 1)
                    {
                        room.UseCard(new CardUseStruct(room.GetCard(ids[0]), player, new List<Player>()));
                        ids.Clear();
                    }
                    else
                    {
                        List<int> used = room.AskForExchange(player, Name, 1, 1, "@kongsheng-use", Name, string.Format("{0}|.|.|{1}", string.Join("#", ids), Name), info.SkillPosition);
                        ids.RemoveAll(t => used.Contains(t));
                        room.UseCard(new CardUseStruct(room.GetCard(used[0]), player, new List<Player>()));
                    }
                }

                ids = player.GetPile(Name);
                room.ObtainCard(player, ref ids, new CardMoveReason(MoveReason.S_REASON_EXCHANGE_FROM_PILE, player.Name, Name, string.Empty));
            }

            return false;
        }
    }

    public class KongshengClear : DetachEffectSkill
    {
        public KongshengClear() : base("kongsheng", "kongsheng") { }
    }

    public class Qianjie : ProhibitSkill
    {
        public Qianjie() : base("qianjie") { }
        public override bool IsProhibited(Room room, Player from, Player to, WrappedCard card, List<Player> others = null)
        {
            if (to != null && RoomLogic.PlayerHasSkill(room, to, Name))
            {
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                if (fcard is DelayedTrick)
                    return true;
            }

            return false;
        }
        public override bool IsProhibited(Room room, Player from, Player to, ProhibitType type)
        {
            if (RoomLogic.PlayerHasSkill(room, to, Name))
                return true;

            return false;
        }

        public override void GetEffectIndex(Room room, Player player, WrappedCard card, ref int index, ref string skill_name, ref string general_name, ref int skin_id)
        {
            index = -1;
        }
    }
    
    public class Jueyan : TriggerSkill
    {
        public Jueyan() : base("jueyan")
        {
            view_as_skill = new JueyanVS();
            events.Add(TriggerEvent.EventPhaseChanging);
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive && player.HasFlag("jueyan_skill"))
                room.HandleAcquireDetachSkills(player, "-jizhi_jx", true);
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class JueyanVS : ZeroCardViewAsSkill
    {
        public JueyanVS() : base("jueyan") { }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            if (!player.HasUsed(JueyanCard.ClassName))
            {
                for (int i = 0; i < 5; i++)
                    if (!player.EquipIsBaned(i))
                        return true;
            }

            return false;
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard(JueyanCard.ClassName) { Skill = Name };
        }
    }

    public class JueyanCard : SkillCard
    {
        public static string ClassName = "JueyanCard";
        public JueyanCard() : base(ClassName)
        {
            target_fixed = true;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            List<string> choices = new List<string>();
            for (int i = 0; i < 5; i++)
            {
                if (!player.EquipIsBaned(i))
                {
                    switch (i)
                    {
                        case 0:
                            choices.Add("Weapon");
                            break;
                        case 1:
                            choices.Add("Armor");
                            break;
                        case 2:
                            choices.Add("Horse");
                            break;
                        case 3:
                            if (!choices.Contains("Horse"))
                                choices.Add("Horse");
                            break;
                        case 4:
                            choices.Add("Treasure");
                            break;
                    }
                }
            }

            string choice = room.AskForChoice(player, "jueyan", string.Join("+", choices));
            switch (choice)
            {
                case "Weapon":
                    room.AbolisheEquip(player, 0, "jueyan");
                    player.SetFlags("jueyan_slash");
                    player.SetMark("jueyan", 3);
                    break;
                case "Armor":
                    room.AbolisheEquip(player, 1, "jueyan");
                    player.SetFlags("jueyan_max");
                    player.SetMark("jueyan", 3);
                    room.DrawCards(player, 3, "jueyan");
                    break;
                case "Horse":
                    room.AbolisheEquip(player, 2, "jueyan");
                    room.AbolisheEquip(player, 3, "jueyan");
                    player.SetFlags("jueyan_distance");
                    break;
                case "Treasure":
                    room.AbolisheEquip(player, 4, "jueyan");
                    room.HandleAcquireDetachSkills(player, "jizhi_jx", true);
                    player.SetFlags("jueyan_skill");
                    break;
            }
        }
    }

    public class JueyanTar : TargetModSkill
    {
        public JueyanTar() : base("#jueyan-target", false)
        {
            pattern = "^SkillCard";
        }

        public override int GetResidueNum(Room room, Player from, WrappedCard card)
        {
            if (card.Name.Contains(Slash.ClassName) && from.HasFlag("jueyan_slash"))
                return from.GetMark("jueyan");

            return 0;
        }

        public override bool GetDistanceLimit(Room room, Player from, Player to, WrappedCard card, CardUseReason reason, string pattern)
        {
            return from.HasFlag("jueyan_distance");
        }
    }

    public class JueyanMax : MaxCardsSkill
    {
        public JueyanMax() : base("#jueyan-max") { }

        public override int GetExtra(Room room, Player target)
        {
            return target.HasFlag("jueyan_max") ? target.GetMark("jueyan") : 0;
        }
    }

    public class Poshi : PhaseChangeSkill
    {
        public Poshi() : base("poshi")
        {
            frequency = Frequency.Wake;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player.Phase == PlayerPhase.Start && base.Triggerable(player, room) && player.GetMark(Name) == 0)
            {
                bool check = true;
                for (int i = 0; i < 5; i++)
                {
                    if (!player.EquipIsBaned(i))
                    {
                        check = false;
                        break;
                    }
                }

                if (player.Hp == 1 || check)
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override bool OnPhaseChange(Room room, Player player, TriggerStruct info)
        {
            room.SetPlayerMark(player, Name, 1);
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            room.DoSuperLightbox(player, info.SkillPosition, Name);

            room.LoseMaxHp(player);
            if (player.Alive)
            {
                if (player.HandcardNum < player.MaxHp)
                    room.DrawCards(player, player.MaxHp - player.HandcardNum, Name);

                room.HandleAcquireDetachSkills(player, "-jueyan", false);
                room.HandleAcquireDetachSkills(player, "huairou", true);
            }

            return false;
        }
    }

    public class HuairouCard : SkillCard
    {
        public static string ClassName = "HuairouCard";
        public HuairouCard() : base(ClassName)
        {
            will_throw = false;
            handling_method = HandlingMethod.MethodRecast;
            target_fixed = true;
        }

        public override void OnUse(Room room, CardUseStruct card_use)
        {
            DoRecast(room, card_use);
        }
    }

    public class Huairou : OneCardViewAsSkill
    {
        public Huairou() : base("huairou")
        {
        }

        public override bool ViewFilter(Room room, WrappedCard to_select, Player player)
        {
            FunctionCard fcard = Engine.GetFunctionCard(to_select.Name);
            return fcard is EquipCard && !RoomLogic.IsCardLimited(room, player, to_select, FunctionCard.HandlingMethod.MethodRecast, true);
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.IsNude();
        }

        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard yy = new WrappedCard(HuairouCard.ClassName)
            {
                Skill = Name
            };
            yy.AddSubCard(card);
            return yy;
        }
    }

    public class Kuizhu : TriggerSkill
    {
        public Kuizhu() : base("kuizhu")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseEnd, TriggerEvent.CardsMoveOneTime, TriggerEvent.EventPhaseChanging };
            view_as_skill = new KuizhuVS();
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && base.Triggerable(move.From, room)
                && move.From.Phase == PlayerPhase.Discard && move.From == room.Current && move.Reason.PlayerId == move.From.Name
                && (move.From_places.Contains(Place.PlaceEquip) || move.From_places.Contains(Place.PlaceHand))
                && (move.Reason.Reason & MoveReason.S_MASK_BASIC_REASON) == MoveReason.S_REASON_DISCARD)
            {
                move.From.AddMark(Name, move.Card_ids.Count);
                room.SetPlayerStringMark(move.From, Name, move.From.GetMark(Name).ToString());
            }
            else if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive && player.GetMark(Name) > 0)
            {
                player.SetMark(Name, 0);
                room.RemovePlayerStringMark(player, Name);
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseEnd && player.Phase == PlayerPhase.Discard && base.Triggerable(player, room) && player.GetMark(Name) > 0)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player erzhang, TriggerStruct info)
        {
            room.AskForUseCard(player, "@@kuizhu", string.Format("@kuizhu:::{0}", player.GetMark(Name)), null, -1, FunctionCard.HandlingMethod.MethodUse, true, info.SkillPosition);
            return new TriggerStruct();
        }
    }

    public class KuizhuVS : ZeroCardViewAsSkill
    {
        public KuizhuVS() : base("kuizhu") { response_pattern = "@@kuizhu"; }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return false;
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard(KuizhuCard.ClassName) { Skill = Name };
        }
    }

    public class KuizhuCard : SkillCard
    {
        public static string ClassName = "KuizhuCard";
        public KuizhuCard() : base(ClassName)
        {
        }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            int count = Self.GetMark("kuizhu");
            int hp = 0;
            foreach (Player p in targets)
                hp += p.Hp;
            return targets.Count < count || hp + to_select.Hp <= count;
        }

        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            if (targets.Count == 0) return false;
            int count = Self.GetMark("kuizhu");
            int hp = 0;
            foreach (Player p in targets)
                hp += p.Hp;
            return targets.Count <= count || count == hp;
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            int count = player.GetMark("kuizhu");
            int hp = 0;
            foreach (Player p in card_use.To)
                hp += p.Hp;

            bool draw = card_use.To.Count <= count;
            if (draw && hp == count)
                draw = room.AskForChoice(player, "kuizhu", "draw+damage", null, card_use.To) == "draw";

            foreach (Player p in card_use.To)
            {
                if (p.Alive)
                {
                    if (draw)
                        room.DrawCards(p, new DrawCardStruct(1, player, "kuizhu"));
                    else
                        room.Damage(new DamageStruct("kuizhu", player, p));
                }
            }

            if (!draw && player.Alive && card_use.To.Count > 1)
                room.Damage(new DamageStruct("kuizhu", null, player));
        }
    }

    public class Chezheng : TriggerSkill
    {
        public Chezheng() : base("chezheng")
        {
            skill_type = SkillType.Attack;
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseEnd, TriggerEvent.CardUsedAnnounced, TriggerEvent.EventPhaseChanging, TriggerEvent.CardResponded };
            frequency = Frequency.Compulsory;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardUsedAnnounced && player.Phase == PlayerPhase.Play && data is CardUseStruct use)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard.TypeID != FunctionCard.CardType.TypeSkill)
                {
                    player.AddMark(Name);
                }
            }
            else if (triggerEvent == TriggerEvent.CardResponded && data is CardResponseStruct resp && resp.Use && player.Phase == PlayerPhase.Play)
            {
                player.AddMark(Name);
            }
            else if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive && player.GetMark(Name) > 0)
                player.SetMark(Name, 0);
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseEnd && player.Phase == PlayerPhase.Play && base.Triggerable(player, room))
            {
                int count = 0;
                foreach (Player p in room.GetOtherPlayers(player))
                    if (!RoomLogic.InMyAttackRange(room, p, player))
                        count++;

                if (player.GetMark(Name) < count)
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetOtherPlayers(player))
                if (!RoomLogic.InMyAttackRange(room, p, player) && RoomLogic.CanDiscard(room, player, p, "he"))
                    targets.Add(p);
            if (targets.Count > 0)
            {
                Player target = room.AskForPlayerChosen(player, targets, Name, "@chezheng", false, true, info.SkillPosition);

                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);

                int id = room.AskForCardChosen(player, target, "he", Name, false, FunctionCard.HandlingMethod.MethodDiscard);
                room.ThrowCard(id, target, player);
            }

            return false;
        }
    }

    public class ChezhengProhibit : ProhibitSkill
    {
        public ChezhengProhibit() : base("#chezheng-pro") { }

        public override bool IsProhibited(Room room, Player from, Player to, WrappedCard card, List<Player> others = null)
        {
            if (from != null && to != null && card != null && RoomLogic.PlayerHasSkill(room, from, "chezheng") && from.Phase == PlayerPhase.Play && !RoomLogic.InMyAttackRange(room, to, from))
            {
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                if (fcard.TypeID != FunctionCard.CardType.TypeSkill) return true;
            }

            return false;
        }

        public override void GetEffectIndex(Room room, Player player, WrappedCard card, ref int index, ref string skill_name, ref string general_name, ref int skin_id)
        {
            Player lord = RoomLogic.FindPlayerBySkillName(room, "chezheng");
            if (lord != null)
            {
                room.SendCompulsoryTriggerLog(lord, "chezheng");
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, lord, Name);
                room.BroadcastSkillInvoke("chezheng", "male", 2, gsk.General, gsk.SkinId);
            }
        }
    }

    public class Lijun : TriggerSkill
    {
        public Lijun() : base("lijun")
        {
            events.Add(TriggerEvent.CardFinished);
            lord_skill = true;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is CardUseStruct use && use.Card.Name.Contains(Slash.ClassName) && player != null && player.GetRoleEnum() != PlayerRole.Lord
                && player.Alive && player.Kingdom == "wu" && player.Phase == PlayerPhase.Play && use.Card.SubCards.Count > 0)
            {
                Player lord = RoomLogic.FindPlayerBySkillName(room, Name);
                if (lord != null && lord.GetRoleEnum() == PlayerRole.Lord && !lord.HasFlag(Name))
                {
                    List<int> ids = new List<int>(use.Card.SubCards);
                    if (room.GetSubCards(use.Card).SequenceEqual(ids))
                    {
                        bool check = true;
                        foreach (int id in use.Card.SubCards)
                        {
                            if (room.GetCardPlace(id) != Place.DiscardPile)
                            {
                                check = false;
                                break;
                            }
                        }
                        if (check) return new TriggerStruct(Name, player);
                    }
                }
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player lord = RoomLogic.FindPlayerBySkillName(room, Name);
            if (room.AskForSkillInvoke(player, Name, lord))
            {
                room.NotifySkillInvoked(lord, Name);
                room.BroadcastSkillInvoke(Name, lord);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use)
            {
                List<int> ids = new List<int>();
                foreach (int id in use.Card.SubCards)
                    if (room.GetCardPlace(id) == Place.DiscardPile)
                        ids.Add(id);

                if (ids.Count > 0)
                {
                    Player lord = RoomLogic.FindPlayerBySkillName(room, Name);

                    ResultStruct result = player.Result;
                    result.Assist = +ids.Count;
                    player.Result = result;

                    lord.SetFlags(Name);
                    room.ObtainCard(lord, ref ids, new CardMoveReason(MoveReason.S_REASON_RECYCLE, player.Name, lord.Name, Name, string.Empty));

                    if (lord.Alive && player.Alive && room.AskForSkillInvoke(lord, Name, "@lijun:" + player.Name))
                        room.DrawCards(player, new DrawCardStruct(1, lord, Name));
                }
            }

            return false;
        }
    }

    public class Huaiju : TriggerSkill
    {
        public Huaiju() : base("huaiju")
        {
            events = new List<TriggerEvent> { TriggerEvent.GameStart, TriggerEvent.DamageDefined, TriggerEvent.EventPhaseProceeding, TriggerEvent.Death };
            frequency = Frequency.Compulsory;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.Death && player.GetSkills(true, false).Contains(Name))
                foreach (Player p in room.Players)
                    if (p.GetMark("@tangerine") > 0)
                        room.SetPlayerMark(p, "@tangerine", 0);
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            Player luji = RoomLogic.FindPlayerBySkillName(room, Name);
            switch (triggerEvent)
            {
                case TriggerEvent.EventPhaseProceeding when player.GetMark("@tangerine") > 0 && player.Phase == PlayerPhase.Draw && luji != null:
                    return new TriggerStruct(Name, luji);
                case TriggerEvent.DamageDefined when player.GetMark("@tangerine") > 0 && luji != null:
                    return new TriggerStruct(Name, luji);
                case TriggerEvent.GameStart when base.Triggerable(player, room):
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, ask_who, Name, info.SkillPosition);
            switch (triggerEvent)
            {
                case TriggerEvent.EventPhaseProceeding when data is int count:
                    {
                        if (ask_who != player)
                        {
                            ResultStruct result = ask_who.Result;
                            result.Assist++;
                            ask_who.Result = result;
                        }

                        room.SendCompulsoryTriggerLog(ask_who, Name);
                        room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                        room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, ask_who.Name, player.Name);
                        count++;
                        data = count;
                        break;
                    }

                case TriggerEvent.DamageDefined:
                    {
                        room.SendCompulsoryTriggerLog(ask_who, Name);
                        room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                        room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, ask_who.Name, player.Name);
                        room.RemovePlayerMark(player, "@tangerine");

                        if (ask_who != player)
                        {
                            ResultStruct result = ask_who.Result;
                            result.Assist++;
                            ask_who.Result = result;
                        }

                        LogMessage log = new LogMessage
                        {
                            Type = "#damaged-prevent",
                            From = player.Name,
                            Arg = Name
                        };
                        room.SendLog(log);

                        return true;
                    }

                case TriggerEvent.GameStart:
                    room.SendCompulsoryTriggerLog(player, Name);
                    room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);
                    room.AddPlayerMark(player, "@tangerine", 3);
                    break;
            }

            return false;
        }
    }

    public class HuaijuDetach : DetachEffectSkill
    {
        public HuaijuDetach() : base("huaiju", string.Empty) { }
        public override void OnSkillDetached(Room room, Player player, object data)
        {
            foreach (Player p in room.Players)
                if (p.GetMark("@tangerine") > 0)
                    room.SetPlayerMark(p, "@tangerine", 0);
        }
    }

    public class Yili : PhaseChangeSkill
    {
        public Yili() : base("yili")
        {
        }

        public override bool Triggerable(Player target, Room room)
        {
            return target.Phase == PlayerPhase.Play && base.Triggerable(target, room) && (target.Hp > 0 || target.GetMark("@tangerine") > 0);
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player target = room.AskForPlayerChosen(player, room.GetOtherPlayers(player), Name, "@yili", true, true, info.SkillPosition);
            if (target != null)
            {
                room.SetTag(Name, target);
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool OnPhaseChange(Room room, Player player, TriggerStruct info)
        {
            Player target = (Player)room.GetTag(Name);
            room.RemoveTag(Name);

            List<string> choice = new List<string>();
            if (player.Hp > 0) choice.Add("losehp");
            if (player.GetMark("@tangerine") > 0) choice.Add("remove");

            if (room.AskForChoice(player, Name, string.Join("+", choice)) == "losehp")
                room.LoseHp(player);
            else
                room.AddPlayerMark(player, "@tangerine", -1);

            if (player.Alive && target.Alive)
                room.AddPlayerMark(target, "@tangerine", 1);

            return false;
        }
    }

    public class Zhenglun : TriggerSkill
    {
        public Zhenglun() : base("zhenglun")
        {
            events.Add(TriggerEvent.EventPhaseStart);// << EventPhaseProceeding;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player lidian, ref object data, Player ask_who)
        {
            return (base.Triggerable(lidian, room) && lidian.Phase == PlayerPhase.Draw && lidian.GetMark("@tangerine") == 0) ? new TriggerStruct(Name, lidian) : new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player lidian, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(lidian, Name, null, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, lidian, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.AddPlayerMark(player, "@tangerine", 1);
            return true;
        }
    }
}
