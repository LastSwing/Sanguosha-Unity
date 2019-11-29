using CommonClass.Game;
using CommonClassLibrary;
using SanguoshaServer.Game;
using System;
using System.Collections.Generic;
using System.Threading;
using static CommonClass.Game.Player;
using static SanguoshaServer.Package.FunctionCard;

namespace SanguoshaServer.Package
{
    public class Anniversary : GeneralPackage
    {
        public Anniversary() : base("Anniversary")
        {
            skills = new List<Skill>
            {
                new Yanjiao(),
                new YanjiaoMax(),
                new Xingshen(),

                new Guolun(),
                new Songsang(),
                new Zhanji(),
                new Guanwei(),
                new Gongqing(),

                new Jiedao(),
                new JiedaoDis(),
                new Kannan(),
                new KannanDamage(),

                new Qinguo(),
                new QinguoRecover(),
                new Youdi(),
                new Duanfa(),
            };

            skill_cards = new List<FunctionCard>
            {
                new GuolunCard(),
                new KannanCard(),
                new DuanfaCard(),
                new YanjiaoCard(),
            };

            related_skills = new Dictionary<string, List<string>>
            {
                { "jiedao", new List<string>{ "#jiedao" } },
                { "kannan", new List<string>{ "#kannan" } },
                { "yanjiao", new List<string>{ "#yanjiao" } },
                { "qinguo", new List<string>{ "#qinguo" } },
            };
        }
    }

    public class Yanjiao : TriggerSkill
    {
        public Yanjiao() : base("yanjiao")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseChanging };
            view_as_skill = new YanjiaoVS();
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

    public class YanjiaoVS : ZeroCardViewAsSkill
    {
        public YanjiaoVS() : base("yanjiao")
        {
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(YanjiaoCard.ClassName);
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard(YanjiaoCard.ClassName) { Skill = Name };
        }

        public override bool SortFilter(Room room, List<int> to_sorts, List<int> ups, List<int> downs)
        {
            int up = 0, down = 0;
            foreach (int id in ups)
                up += room.GetCard(id).Number;

            foreach (int id in downs)
                down += room.GetCard(id).Number;

            return up == down;
        }
    }

    public class YanjiaoCard : SkillCard
    {
        public static string ClassName = "YanjiaoCard";
        public YanjiaoCard() : base(ClassName)
        {
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select != Self;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From, target = card_use.To[0];
            int count = 4 + player.GetMark("xingshen");
            player.SetMark("xingshen", 0);
            room.RemovePlayerStringMark(player, "xingshen");

            List<int> card_ids = room.GetNCards(count);
            foreach (int id in card_ids)
            {
                room.MoveCardTo(room.GetCard(id), player, Place.PlaceTable, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_TURNOVER, player.Name, "yanjiao", null), false);
                Thread.Sleep(400);
            }
            AskForMoveCardsStruct result = room.AskforSortCards(target, "yanjiao", card_ids, true, card_use.Card.SkillPosition);
            List<CardsMoveStruct> moves = new List<CardsMoveStruct>();
            if (result.Success)
            {
                card_ids.RemoveAll(t => result.Top.Contains(t));
                card_ids.RemoveAll(t => result.Bottom.Contains(t));

                if (result.Bottom.Count > 0 && result.Top.Count > 0)
                {
                    moves.Add(new CardsMoveStruct(result.Bottom, target, Place.PlaceHand, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_GOTBACK, target.Name, "yanjiao", null)));
                    moves.Add(new CardsMoveStruct(result.Top, player, Place.PlaceHand, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_GOTBACK, player.Name, "yanjiao", null)));

                }
            }

            if (card_ids.Count > 0)
            {
                moves.Add(new CardsMoveStruct(card_ids, null, Place.DiscardPile, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_NATURAL_ENTER, null, "yanjiao", null)));
                if (card_ids.Count > 1) player.AddMark("yanjiao");
            }
            room.MoveCards(moves, true);
        }
    }

    public class YanjiaoMax : MaxCardsSkill
    {
        public YanjiaoMax() : base("#yanjiao") { }
        public override int GetExtra(Room room, Player target)
        {
            return -target.GetMark("yanjiao");
        }
    }

    public class Xingshen : MasochismSkill
    {
        public Xingshen() : base("xingshen") { }

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
            int count = 1000, hp = 100;
            int draw = 1;
            int mark = target.GetMark(Name) < 4 ? 1 : 0;
            foreach (Player p in room.GetAlivePlayers())
            {
                if (p.HandcardNum < count) count = p.HandcardNum;
                if (p.Hp < hp) hp = p.Hp;
            }

            if (target.HandcardNum == count) draw = 2;
            room.DrawCards(target, draw, Name);
            if (mark > 0)
            {
                if (target.Hp == hp && target.GetMark(Name) + 2 <= 4) mark = 2;
                target.AddMark(Name, mark);
                room.SetPlayerStringMark(target, Name, target.GetMark(Name).ToString());
            }
        }
    }

    public class Guolun : ZeroCardViewAsSkill
    {
        public Guolun() : base("guolun")
        {
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(GuolunCard.ClassName);
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard(GuolunCard.ClassName) { Skill = Name };
        }
    }

    public class GuolunCard : SkillCard
    {
        public static string ClassName = "GuolunCard";
        public GuolunCard() : base(ClassName)
        {
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && !to_select.IsKongcheng() && to_select != Self;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From, target = card_use.To[0];
            int id = room.AskForCardChosen(player, target, "h", "guolun", false, HandlingMethod.MethodNone);
            room.ShowCard(target, id, "guolun");

            List<int> ids = new List<int>();
            player.SetMark("guolun", id);
            target.SetFlags("guolun");
            if (!player.IsKongcheng())
                ids = room.AskForExchange(player, "guolun", 1, 0, "@guolun:" + target.Name, string.Empty, ".", card_use.Card.SkillPosition);
            player.SetMark("guolun", 0);
            target.SetFlags("-guolun");

            if (ids.Count > 0)
            {
                Player drawer = null;
                if (room.GetCard(ids[0]).Number < room.GetCard(id).Number)
                    drawer = player;
                else if (room.GetCard(ids[0]).Number > room.GetCard(id).Number)
                    drawer = target;

                CardsMoveStruct move1 = new CardsMoveStruct(ids, target, Place.PlaceHand,
                    new CardMoveReason(CardMoveReason.MoveReason.S_REASON_SWAP, player.Name, target.Name, "guolun", string.Empty));
                CardsMoveStruct move2 = new CardsMoveStruct(new List<int> { id }, player, Place.PlaceHand,
                    new CardMoveReason(CardMoveReason.MoveReason.S_REASON_SWAP, target.Name, player.Name, "guolun", string.Empty));
                List<CardsMoveStruct> exchangeMove = new List<CardsMoveStruct> { move1, move2 };
                room.MoveCards(exchangeMove, true);

                if (drawer != null && drawer.Alive)
                    room.DrawCards(drawer, new DrawCardStruct(1, player, "guolun"));
            }
        }
    }
    public class Songsang : TriggerSkill
    {
        public Songsang() : base("songsang")
        {
            events.Add(TriggerEvent.Death);
            frequency = Frequency.Limited;
            skill_type = SkillType.Recover;
            limit_mark = "@sang";
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            List<Player> caopis = RoomLogic.FindPlayersBySkillName(room, Name);
            foreach (Player caopi in caopis)
                if (caopi != player && caopi.GetMark(limit_mark) > 0)
                    triggers.Add(new TriggerStruct(Name, caopi));

            return triggers;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player caopi, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(caopi, Name, data, info.SkillPosition))
            {
                room.SetPlayerMark(caopi, limit_mark, 0);
                room.BroadcastSkillInvoke(Name, caopi, info.SkillPosition);
                room.DoSuperLightbox(caopi, info.SkillPosition, Name);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player caopi, TriggerStruct info)
        {
            if (caopi.IsWounded())
            {
                RecoverStruct recover = new RecoverStruct
                {
                    Who = caopi,
                    Recover = 1
                };
                room.Recover(caopi, recover, true);
            }
            else
            {
                caopi.MaxHp++;
                room.BroadcastProperty(caopi, "MaxHp");

                LogMessage log = new LogMessage
                {
                    Type = "$GainMaxHp",
                    From = caopi.Name,
                    Arg = "1"
                };
                room.SendLog(log);
                room.RoomThread.Trigger(TriggerEvent.MaxHpChanged, room, caopi);
            }
            room.HandleAcquireDetachSkills(caopi, "zhanji", true);

            return false;
        }
    }

    public class Zhanji : TriggerSkill
    {
        public Zhanji() : base("zhanji")
        {
            events.Add(TriggerEvent.CardsMoveOneTime);
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is CardsMoveOneTimeStruct move && move.To != null && base.Triggerable(move.To, room) && move.To.Phase == PlayerPhase.Play
                && move.Reason.Reason == CardMoveReason.MoveReason.S_REASON_DRAW && move.Reason.SkillName != Name)
                return new TriggerStruct(Name, move.To);

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(ask_who, Name);
            room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
            room.DrawCards(ask_who, 1, Name);

            return false;
        }
    }

    public class Guanwei : TriggerSkill
    {
        public Guanwei() : base("guanwei")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardUsed, TriggerEvent.CardResponded, TriggerEvent.EventPhaseEnd, TriggerEvent.EventPhaseChanging };
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardUsed && data is CardUseStruct use && player.Alive && player == room.Current && !player.HasFlag(Name))
            {
                if (Engine.GetFunctionCard(use.Card.Name).TypeID != CardType.TypeSkill)
                {
                    List<WrappedCard.CardSuit> suits = player.ContainsTag(Name) ? (List<WrappedCard.CardSuit>)player.GetTag(Name) : new List<WrappedCard.CardSuit>();
                    if (use.Card.Suit > WrappedCard.CardSuit.Diamond || (suits.Count > 0 && !suits.Contains(use.Card.Suit)))
                    {
                        player.SetFlags(Name);
                    }
                    else
                    {
                        suits.Add(use.Card.Suit);
                        player.SetTag(Name, suits);
                    }
                }
            }
            else if (triggerEvent == TriggerEvent.CardResponded && data is CardResponseStruct resp && resp.Use && player.Alive && player == room.Current && !player.HasFlag(Name))
            {
                List<WrappedCard.CardSuit> suits = player.ContainsTag(Name) ? (List<WrappedCard.CardSuit>)player.GetTag(Name) : new List<WrappedCard.CardSuit>();
                if (resp.Card.Suit > WrappedCard.CardSuit.Diamond || (suits.Count > 0 && !suits.Contains(resp.Card.Suit)))
                {
                    player.SetFlags(Name);
                }
                else
                {
                    suits.Add(resp.Card.Suit);
                    player.SetTag(Name, suits);
                }
            }
            else if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive && player.ContainsTag(Name))
                player.RemoveTag(Name);
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.EventPhaseEnd && player.Phase == PlayerPhase.Play && player.Alive && !player.HasFlag(Name) && player.ContainsTag(Name)
                && player.GetTag(Name) is List<WrappedCard.CardSuit> suits && suits.Count > 1)
            {
                bool invoke = true;
                for (int i = 1; i < suits.Count; i++)
                {
                    if (suits[i] != suits[0])
                    {
                        invoke = false;
                        break;
                    }
                }

                if (invoke)
                {
                    foreach (Player p in RoomLogic.FindPlayersBySkillName(room, Name))
                        triggers.Add(new TriggerStruct(Name, p));
                }
            }

            return triggers;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (player.Alive && ask_who.Alive && !player.HasFlag(Name) && room.AskForDiscard(ask_who, Name, 1, 0, true, true, "@guanwei:" + player.Name, true, info.SkillPosition))
            {
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, ask_who.Name, player.Name);
                room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            player.SetFlags(Name);
            if (player.Alive) room.DrawCards(player, new DrawCardStruct(2, ask_who, Name));
            if (player.Alive) player.AddPhase(PlayerPhase.Play);
            return false;
        }
    }

    public class Gongqing : TriggerSkill
    {
        public Gongqing() : base("gongqing")
        {
            events.Add(TriggerEvent.DamageInflicted);
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Masochism;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is DamageStruct damage && damage.From != null && damage.From.Alive)
            {
                int range = RoomLogic.GetAttackRange(room, damage.From);
                if (range < 3 && damage.Damage > 1 || range > 3)
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            DamageStruct damage = (DamageStruct)data;
            room.NotifySkillInvoked(player, Name);

            int range = RoomLogic.GetAttackRange(room, damage.From);
            if (range < 3 && damage.Damage > 1)
            {
                damage.Damage = 1;
                LogMessage log = new LogMessage
                {
                    Type = "#ReduceDamage",
                    From = player.Name,
                    Arg = Name,
                    Arg2 = (damage.Damage).ToString()
                };
                room.SendLog(log);
            }
            else if (range > 3)
            {
                LogMessage log = new LogMessage
                {
                    Type = "#AddDamaged",
                    From = player.Name,
                    Arg = Name,
                    Arg2 = (++damage.Damage).ToString()
                };
                room.SendLog(log);
            }

            data = damage;

            return false;
        }
    }


    public class Jiedao : TriggerSkill
    {
        public Jiedao() : base("jiedao")
        {
            events = new List<TriggerEvent> { TriggerEvent.DamageCaused, TriggerEvent.EventPhaseChanging };
            skill_type = SkillType.Attack;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.DamageCaused)
                player.AddMark(Name);
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
                foreach (Player p in room.GetAlivePlayers())
                    if (p.GetMark(Name) > 0) p.SetMark(Name, 0);
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.DamageCaused && base.Triggerable(player, room) && player.IsWounded() && player.GetMark(Name) == 1)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is DamageStruct damage && room.AskForSkillInvoke(player, Name, damage.To, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is DamageStruct damage)
            {
                List<string> choices = new List<string>();
                for (int i = 1; i <= player.GetLostHp(); i++)
                    choices.Add(i.ToString());

                string choice = room.AskForChoice(player, Name, string.Join("+", choices), new List<string> { "@jiedao:" + damage.To.Name }, data);
                int count = int.Parse(choice);
                string mark = string.Format("{0}:{1}", Name, choice);
                if (damage.Marks == null)
                    damage.Marks = new List<string> { mark };
                else
                    damage.Marks.Add(mark);

                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, ask_who.Name, damage.To.Name);
                LogMessage log = new LogMessage
                {
                    Type = "#AddDamage",
                    From = player.Name,
                    To = new List<string> { damage.To.Name },
                    Arg = Name,
                    Arg2 = (damage.Damage += count).ToString()
                };
                room.SendLog(log);

                data = damage;
            }
            return false;
        }
    }

    public class JiedaoDis : TriggerSkill
    {
        public JiedaoDis() : base("#jiedao")
        {
            events.Add(TriggerEvent.DamageComplete);
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is DamageStruct damage && player.Alive && !damage.Prevented  && damage.From != null && damage.From.Alive && damage.Marks != null && !damage.From.IsNude())
            {
                foreach (string str in damage.Marks)
                {
                    if (str.StartsWith("jiedao"))
                        return new TriggerStruct(Name, damage.From);
                }
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is DamageStruct damage)
            {
                int count = 0;
                foreach (string str in damage.Marks)
                {
                    if (str.StartsWith("jiedao"))
                    {
                        string[] strs = str.Split(':');
                        count = int.Parse(strs[1]);
                        break;
                    }
                }

                room.AskForDiscard(ask_who, "jiedao", count, count, false, true, "@jiedao-discard:::" + count.ToString(), false, info.SkillPosition);
            }

            return false;
        }
    }

    public class Kannan : TriggerSkill
    {
        public Kannan() : base("kannan")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseChanging };
            view_as_skill = new KannanVS();
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (data is PhaseChangeStruct change && change.From == PlayerPhase.Play && player.HasFlag(Name))
                player.SetFlags("-kannan");
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class KannanVS : ZeroCardViewAsSkill
    {
        public KannanVS() : base("kannan")
        {
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasFlag(Name) && player.UsedTimes(KannanCard.ClassName) < player.Hp && !player.IsKongcheng();
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard(KannanCard.ClassName) { Skill = Name };
        }
    }

    public class KannanCard : SkillCard
    {
        public static string ClassName = "KannanCard";
        public KannanCard() : base(ClassName)
        {
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && Self != to_select && !to_select.HasFlag("kannan") && RoomLogic.CanBePindianBy(room, to_select, Self);
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From, target = card_use.To[0];
            target.SetFlags("kannan");
            PindianStruct pd = room.PindianSelect(player, target, "kannan");
            room.Pindian(ref pd);
            if (pd.From_number > pd.To_numbers[0])
            {
                player.AddMark("kannan");
                room.SetPlayerStringMark(player, "kannan", player.GetMark("kannan").ToString());
                player.SetFlags("kannan");
            }
            else if (pd.To_numbers[0] > pd.From_number)
            {
                target.AddMark("kannan");
                room.SetPlayerStringMark(target, "kannan", target.GetMark("kannan").ToString());
            }
        }
    }

    public class KannanDamage : TriggerSkill
    {
        public KannanDamage() : base("#kannan")
        {
            events.Add(TriggerEvent.CardUsedAnnounced);
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is CardUseStruct use && use.Card.Name.Contains(Slash.ClassName) && player.Alive && player.GetMark("kannan") > 0)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardUseStruct use)
            {
                LogMessage log = new LogMessage
                {
                    Type = "#kannan-add",
                    From = player.Name,
                    Arg = player.GetMark("kannan").ToString(),
                    Arg2 = use.Card.Name
                };
                room.SendLog(log);

                use.ExDamage += player.GetMark("kannan");
                data = use;
                player.SetMark("kannan", 0);
                room.RemovePlayerStringMark(player, "kannan");
            }

            return false;
        }
    }

    public class Qinguo : TriggerSkill
    {
        public Qinguo() : base("qinguo")
        {
            events.Add(TriggerEvent.CardFinished);
            skill_type = SkillType.Attack;
            view_as_skill = new QinguoVS();
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is CardUseStruct use && base.Triggerable(player, room) && room.Current == player && Engine.GetFunctionCard(use.Card.Name) is EquipCard)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.AskForUseCard(player, "@@qinguo", "@qinguo-slash", -1, HandlingMethod.MethodUse, false, info.SkillPosition);
            return new TriggerStruct();
        }

        public override void GetEffectIndex(Room room, Player player, WrappedCard card, ref int index, ref string skill_name, ref string general_name, ref int skin_id)
        {
            index = 1;
        }
    }


    public class QinguoVS : ViewAsSkill
    {
        public QinguoVS() : base("qinguo")
        {
            response_pattern = "@@qinguo";
        }
        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return false;
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return false;
        }

        public override List<WrappedCard> GetGuhuoCards(Room room, List<WrappedCard> cards, Player Self)
        {
            if (cards.Count == 0)
                return new List<WrappedCard> { new WrappedCard(Slash.ClassName) { Skill = Name, ShowSkill = Name } };

            return new List<WrappedCard>();
        }

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count == 1 && cards[0].IsVirtualCard())
                return cards[0];

            return null;
        }
    }

    public class QinguoRecover : TriggerSkill
    {
        public QinguoRecover() : base("#qinguo")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardsMoveOneTime };
            skill_type = SkillType.Recover;
            frequency = Frequency.Compulsory;
        }
        

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move)
            {
                if (move.From != null && base.Triggerable(move.From, room) && move.From_places.Contains(Place.PlaceEquip) && move.From.Hp == move.From.GetEquips().Count
                    && move.Card_ids.Count == move.From_places.Count && move.From.IsWounded())
                {
                    int count = 0;
                    foreach (Place place in move.From_places)
                        if (place == Place.PlaceEquip) count++;
                    if (count > 0) return new TriggerStruct(Name, move.From);

                }
                else if (move.To != null && base.Triggerable(move.To, room) && move.To_place == Place.PlaceEquip && move.To.Hp == move.To.GetEquips().Count
                    && move.Card_ids.Count > 0 && move.To.IsWounded())
                {
                    return new TriggerStruct(Name, move.To);
                }
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (ask_who.Alive && ask_who.IsWounded())
            {
                room.SendCompulsoryTriggerLog(ask_who, Name, true);
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, ask_who, Name, info.SkillPosition);
                room.BroadcastSkillInvoke("qinguo", "male", 2, gsk.General, gsk.SkinId);

                RecoverStruct recover = new RecoverStruct
                {
                    Recover = 1,
                    Who = ask_who
                };
                room.Recover(ask_who, recover, true);
            }

            return false;
        }
    }

    public class Youdi : PhaseChangeSkill
    {
        public Youdi() : base("youdi")
        {
            skill_type = SkillType.Attack;
        }

        public override bool Triggerable(Player target, Room room)
        {
            return base.Triggerable(target, room) && target.Phase == PlayerPhase.Finish && !target.IsKongcheng();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetOtherPlayers(player))
                if (RoomLogic.CanDiscard(room, p, player, "h")) targets.Add(p);

            if (targets.Count > 0)
            {
                Player target = room.AskForPlayerChosen(player, targets, Name, "@youdi", true, true, info.SkillPosition);
                if (target != null)
                {
                    room.SetTag(Name, target);
                    room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                    return info;
                }
            }

            return new TriggerStruct();
        }

        public override bool OnPhaseChange(Room room, Player player, TriggerStruct info)
        {
            Player target = (Player)room.GetTag(Name);
            room.RemoveTag(Name);

            int id = room.AskForCardChosen(target, player, "h", Name, false, HandlingMethod.MethodDiscard);
            List<int> ids = new List<int> { id };
            room.ThrowCard(ref ids, player, target);
            if (ids.Count == 1)
            {
                WrappedCard card = room.GetCard(ids[0]);
                bool red = WrappedCard.IsRed(card.Suit);
                if (player.Alive && !card.Name.Contains(Slash.ClassName) && target.Alive && RoomLogic.CanGetCard(room, player, target, "he"))
                {
                    int get = room.AskForCardChosen(player, target, "he", Name, false, HandlingMethod.MethodGet);
                    List<int> gets = new List<int> { get };
                    room.ObtainCard(player, ref gets, new CardMoveReason(CardMoveReason.MoveReason.S_REASON_EXTRACTION, player.Name, target.Name, string.Empty), false);
                }

                if (red && player.Alive)
                    room.DrawCards(player, 1, Name);
            }

            return false;
        }
    }

    public class DuanfaCard : SkillCard
    {
        public static string ClassName = "DuanfaCard";
        public DuanfaCard() : base(ClassName)
        {
            target_fixed = true;
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            if (card_use.From.Alive)
            {
                int count = card_use.Card.SubCards.Count;
                card_use.From.AddMark("duanfa", count);
                room.DrawCards(card_use.From, count, "duanfa");
            }
        }
    }

    public class Duanfa : TriggerSkill
    {
        public Duanfa() : base("duanfa")
        {
            events.Add(TriggerEvent.EventPhaseChanging);
            view_as_skill = new DuanfaVS();
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (data is PhaseChangeStruct change && change.From == PlayerPhase.Play && player.GetMark(Name) > 0)
                player.SetMark(Name, 0);
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class DuanfaVS : ViewAsSkill
    {
        public DuanfaVS() : base("duanfa")
        {
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.IsNude() && player.GetMark(Name) < player.MaxHp;
        }
        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return !RoomLogic.IsJilei(room, player, to_select) && WrappedCard.IsBlack(to_select.Suit) && selected.Count < player.MaxHp - player.GetMark(Name);
        }
        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count == 0)
                return null;

            WrappedCard zhiheng_card = new WrappedCard(DuanfaCard.ClassName)
            {
                Skill = Name,
                ShowSkill = Name
            };
            zhiheng_card.AddSubCards(cards);
            return zhiheng_card;
        }
    }
}