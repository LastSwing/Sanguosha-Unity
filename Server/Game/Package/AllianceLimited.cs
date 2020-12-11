using CommonClass.Game;
using CommonClassLibrary;
using SanguoshaServer.Game;
using System.Collections.Generic;
using static CommonClass.Game.CardUseStruct;
using static CommonClass.Game.Player;
using static SanguoshaServer.Package.FunctionCard;

namespace SanguoshaServer.Package
{
    public class AllianceGeneral : GeneralPackage
    {
        public AllianceGeneral() : base("AllianceGeneral")
        {
            skills = new List<Skill> {
                new Jielue(),
                new Mojun(),
                new Longying(),
                new TunjunSP(),
                new Jiaoxia(),
                new FengyingFYJ(),
                new Fangong(),
                new FangongTag(),
                new Huying(),
                new Kuangxi(),
                new Baoying(),
                new Yangwu(),
                new Jingji(),
                new Ruiji(),
                new Yanglie(),
                new Moqu(),
                new YaowuSP(),
                new PoluSJSP(),
            };
            skill_cards = new List<FunctionCard>
            {
                new KuangxiCard(),
            };
            related_skills = new Dictionary<string, List<string>>
            {
                { "fangong", new List<string> { "#fangong" } },
            };
        }
    }

    //张济
    public class Jielue : TriggerSkill
    {
        public Jielue() : base("jielue")
        {
            frequency = Frequency.Compulsory;
            events.Add(TriggerEvent.Damage);
            skill_type = SkillType.Attack;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is DamageStruct damage && damage.To.Alive && damage.To != player && !damage.To.IsAllNude()
                && RoomLogic.CanGetCard(room, player, damage.To, "hej"))
                return new TriggerStruct(Name, player);
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is DamageStruct damage)
            {
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, damage.To.Name);
                room.SendCompulsoryTriggerLog(player, name, true);
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);

                Player target = damage.To;
                List<string> handle = new List<string>();
                if (!target.IsKongcheng() && RoomLogic.CanGetCard(room, player, target, "h"))
                    handle.Add("h^false^get");
                if (target.HasEquip() && RoomLogic.CanGetCard(room, player, target, "e"))
                    handle.Add("e^false^get");
                if (target.JudgingArea.Count > 0 && RoomLogic.CanGetCard(room, player, target, "j"))
                    handle.Add("j^false^get");

                List<int> ids = room.AskForCardsChosen(player, damage.To, handle, Name);
                if (ids.Count > 0)
                    room.ObtainCard(player, ref ids, new CardMoveReason(MoveReason.S_REASON_EXTRACTION, player.Name, damage.To.Name, Name, string.Empty), false);

                if (player.Alive)
                    room.LoseHp(player);
            }

            return false;
        }
    }

    public class Mojun : TriggerSkill
    {
        public Mojun() : base("mojun")
        {
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Alter;
            events.Add(TriggerEvent.Damage);
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (player.Alive && data is DamageStruct damage && damage.Card != null && damage.Card.Name.Contains(Slash.ClassName)
                && !damage.Transfer && !damage.Chain)
            {
                List<Player> lords = RoomLogic.FindPlayersBySkillName(room, Name);
                foreach (Player p in lords)
                {
                    if (p.Camp == player.Camp)
                        triggers.Add(new TriggerStruct(name, p));
                }
            }
            return triggers;
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
                room.SendCompulsoryTriggerLog(ask_who, name, true);
                room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);

            JudgeStruct judge = new JudgeStruct
            {
                Pattern = ".|black",
                Good = true,
                Reason = Name,
                PlayAnimation = true,
                Who = player,
            };
            room.Judge(ref judge);

            if (judge.IsEffected())
            {
                List<Player> players = new List<Player>();
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (p.Camp == ask_who.Camp)
                    {
                        room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, p.Name);
                        players.Add(p);
                    }
                }

                room.SortByActionOrder(ref players);
                foreach (Player p in players)
                    room.DrawCards(p, new DrawCardStruct(1, ask_who, Name));
            }

            return false;
        }
    }

    //龙骧军
    public class Longying : TriggerSkill
    {
        public Longying() : base("longying")
        {
            events.Add(TriggerEvent.EventPhaseStart);
            skill_type = SkillType.Recover;
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && player.Phase == PlayerPhase.Play)
            {
                Player lord = null;
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (p.GetRoleEnum() == PlayerRole.Lord && p.IsWounded())
                    {
                        lord = p;
                        break;
                    }
                }

                if (lord != null) return new TriggerStruct(Name, player);
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player lord = null;
            foreach (Player p in room.GetAlivePlayers())
            {
                if (p.GetRoleEnum() == PlayerRole.Lord )
                {
                    lord = p;
                    break;
                }
            }

            room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, lord.Name);
            room.SendCompulsoryTriggerLog(player, Name, true);
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);

            room.LoseHp(player);
            if (lord.Alive)
            {
                RecoverStruct recover = new RecoverStruct
                {
                    Who = player,
                    Recover = 1
                };
                room.Recover(lord, recover, true);
                if (lord.Alive)
                    room.DrawCards(lord, new DrawCardStruct(1, player, Name));
            }

            return false;
        }
    }

    //牛辅&董翓
    public class TunjunSP : TriggerSkill
    {
        public TunjunSP() : base("tunjun_sp")
        {
            frequency = Frequency.Compulsory;
            events.Add(TriggerEvent.RoundStart);
            skill_type = SkillType.Replenish;
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            List<Player> nd = RoomLogic.FindPlayersBySkillName(room, Name);
            foreach (Player p in nd)
            {
                if (p.MaxHp > 1)
                    triggers.Add(new TriggerStruct(Name, p));
            }

            return triggers;
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(ask_who, Name, true);
            room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);

            room.LoseMaxHp(ask_who);
            if (ask_who.Alive)
                room.DrawCards(ask_who, ask_who.MaxHp, Name);

            return false;
        }
    }

    public class Jiaoxia : MaxCardsSkill
    {
        public Jiaoxia() : base("jiaoxia")
        {
        }
        public override bool Ingnore(Room room, Player player, int card_id)
        {
            Player lord = RoomLogic.FindPlayerBySkillName(room, Name);
            if (lord != null && player.Camp == lord.Camp && WrappedCard.IsBlack(room.GetCard(card_id).Suit))
                return true;
            return false;
        }
    }

    //凤瑶军
    public class FengyingFYJ : ProhibitSkill
    {
        public FengyingFYJ() : base("fengying_fyj")
        {}

        public override bool IsProhibited(Room room, Player from, Player to, WrappedCard card, List<Player> others = null)
        {
            Player lord = RoomLogic.FindPlayerBySkillName(room, Name);
            FunctionCard fcard = Engine.GetFunctionCard(card.Name);
            if (!(fcard is SkillCard) && lord != null && to != null && from != null && from.Camp != to.Camp && lord.Camp == to.Camp)
            {
                foreach (Player p in room.GetOtherPlayers(to))
                {
                    if (p.Hp <= to.Hp && to.Camp == p.Camp)
                        return false;
                }
                return true;
            }
            return false;
        }
    }

    public class Fangong : TriggerSkill
    {
        public Fangong() : base("fangong")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardFinished, TriggerEvent.CardUsedAnnounced };
            skill_type = SkillType.Attack;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardUsedAnnounced && data is CardUseStruct use && use.Pattern == "Slash:fangong")
            {
                room.ShowSkill(player, Name, string.Empty);
                room.NotifySkillInvoked(player, Name);
                room.BroadcastSkillInvoke(Name, player);
            }
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.CardFinished && player != null && player.Alive && data is CardUseStruct use && use.Card != null && use.Card.Name != Jink.ClassName)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (!(fcard is SkillCard))
                {
                    foreach (Player p in use.To)
                        if (player.Camp != p.Camp && base.Triggerable(p, room))
                            triggers.Add(new TriggerStruct(Name, p));
                }
            }

            return triggers;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (player.Alive && ask_who.Alive)
            {
                ask_who.SetFlags("slashTargetFix");
                player.SetFlags("SlashAssignee");

                WrappedCard used = room.AskForUseCard(ask_who, "Slash:fangong", "@fangong-slash:" + player.Name, null, -1, HandlingMethod.MethodUse, false);
                if (used == null)
                {
                    ask_who.SetFlags("-slashTargetFix");
                    player.SetFlags("-SlashAssignee");
                }
            }

            return new TriggerStruct();
        }
    }

    public class FangongTag : TargetModSkill
    {
        public FangongTag() : base("#fangong", false) { }
        public override bool GetDistanceLimit(Room room, Player from, Player to, WrappedCard card, CardUseReason reason, string pattern)
        {
            if (reason == CardUseReason.CARD_USE_REASON_RESPONSE_USE && to.HasFlag("SlashAssignee")
                && (room.GetRoomState().GetCurrentResponseSkill() == "fangong" || pattern == "Slash:fangong"))
                return true;

            return false;
        }
    }

    public class Huying : TriggerSkill
    {
        public Huying() : base("huying")
        {
            events.Add(TriggerEvent.EventPhaseStart);
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && player.Phase == PlayerPhase.Play)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player lord = null;
            foreach (Player p in room.GetAlivePlayers())
            {
                if (p.GetRoleEnum() == PlayerRole.Lord)
                {
                    lord = p;
                    break;
                }
            }
            List<int> give = room.AskForExchange(player, Name, 1, 0, "@huying:" + lord.Name, string.Empty, Slash.ClassName, info.SkillPosition);

            room.SendCompulsoryTriggerLog(player, Name, true);
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, lord.Name);

            if (give.Count > 0)
            {
                room.ObtainCard(lord, ref give, new CardMoveReason(MoveReason.S_REASON_GIVE, player.Name, lord.Name, Name, string.Empty), true);
            }
            else
            {
                room.LoseHp(player);
                if (lord.Alive)
                {
                    foreach (int id in room.DrawPile)
                    {
                        WrappedCard card = room.GetCard(id);
                        if (card.Name.Contains(Slash.ClassName))
                        {
                            CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_GOTBACK, lord.Name, Name, string.Empty);
                            List<int> ids = new List<int> { id };
                            room.ObtainCard(lord, ref ids, reason, true);
                            break;
                        }
                    }
                }
            }

            return false;
        }
    }

    public class Kuangxi : TriggerSkill
    {
        public Kuangxi() : base("kuangxi")
        {
            events.Add(TriggerEvent.QuitDying);
            skill_type = SkillType.Attack;
            view_as_skill = new KuangxiVS();
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (data is DyingStruct dying && dying.Damage.From != null && !string.IsNullOrEmpty(dying.Damage.Reason) && dying.Damage.Reason == Name && dying.Damage.From.Alive)
            {
                LogMessage log = new LogMessage
                {
                    Type = "#kuangxi-lose",
                    From = dying.Damage.From.Name,
                    Arg = Name
                };

                dying.Damage.From.SetFlags(Name);
                room.SendLog(log);
            }
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class KuangxiVS : ZeroCardViewAsSkill
    {
        public KuangxiVS() : base("kuangxi")
        {
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return player.Hp > 0 && !player.HasFlag(Name);
        }
        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard(KuangxiCard.ClassName) { Skill = Name };
        }
    }

    public class KuangxiCard : SkillCard
    {
        public static string ClassName = "KuangxiCard";
        public KuangxiCard() : base(ClassName)
        {
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select != Self;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From, target = card_use.To[0];
            room.LoseHp(player);
            if (target.Alive)
                room.Damage(new DamageStruct("kuangxi", player, target));
        }
    }

    public class Baoying : TriggerSkill
    {
        public Baoying() : base("baoying")
        {
            events.Add(TriggerEvent.Dying);
            skill_type = SkillType.Recover;
            frequency = Frequency.Limited;
            limit_mark = "@bao";
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (player.Alive && player.HasFlag("Global_Dying") && player.Hp < 1)
            {
                foreach (Player p in RoomLogic.FindPlayersBySkillName(room, Name))
                    if (p.Camp == player.Camp && p.GetMark(limit_mark) > 0) triggers.Add(new TriggerStruct(Name, p));
            }
            return triggers;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(ask_who, Name, player, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                //room.DoSuperLightbox(ask_who, info.SkillPosition, Name);
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, ask_who.Name, player.Name);
                room.SetPlayerMark(ask_who, limit_mark, 0);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            int count = 1 - player.Hp;
            if (count > 0)
            {
                RecoverStruct recover = new RecoverStruct
                {
                    Who = ask_who,
                    Recover = count
                };
                room.Recover(player, recover, true);
            }

            return false;
        }
    }

    public class Yangwu : TriggerSkill
    {
        public Yangwu() : base("yangwu")
        {
            events.Add(TriggerEvent.EventPhaseStart);
            skill_type = SkillType.Attack;
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && player.Phase == PlayerPhase.Start)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name, true);
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            List<Player> targets = room.GetOtherPlayers(player);
            room.SortByActionOrder(ref targets);
            foreach (Player p in targets)
            {
                if (p.Alive && player.Alive)
                {
                    room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, p.Name);
                    room.Damage(new DamageStruct(Name, player, p));
                }
            }

            if (player.Alive)
                room.LoseHp(player);

            return false;
        }
    }

    public class Jingji : DistanceSkill
    {
        public Jingji() : base("jingji")
        {
        }

        public override int GetCorrect(Room room, Player from, Player to, WrappedCard card = null)
        {
            int distance = 0;
            List<Player> players = RoomLogic.FindPlayersBySkillName(room, Name);
            foreach (Player p in players)
            {
                if (from.Camp == p.Camp)
                    distance -= 1;
            }

            return distance;
        }
    }
    public class Ruiji : TriggerSkill
    {
        public Ruiji() : base("ruiji")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseProceeding };
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Replenish;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            Player luji = RoomLogic.FindPlayerBySkillName(room, Name);
            if (player.Phase == PlayerPhase.Draw && luji != null && luji.Camp == player.Camp)
                return new TriggerStruct(Name, luji);

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (ask_who != player)
            {
                ResultStruct result = ask_who.Result;
                result.Assist++;
                ask_who.Result = result;
            }

            room.SendCompulsoryTriggerLog(ask_who, Name);
            room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
            room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, ask_who.Name, player.Name);
            int count = (int)data;
            count++;
            data = count;
            return false;
        }
    }

    public class Yanglie : TriggerSkill
    {
        public Yanglie() : base("yanglie")
        {
            events.Add(TriggerEvent.EventPhaseStart);
            skill_type = SkillType.Wizzard;
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && player.Phase == PlayerPhase.Start)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name, true);
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            List<Player> targets = room.GetOtherPlayers(player);
            room.SortByActionOrder(ref targets);
            foreach (Player p in targets)
            {
                if (p.Alive && player.Alive && !p.IsAllNude() && RoomLogic.CanGetCard(room, player, p, "hej"))
                {
                    room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, p.Name);
                    int id = room.AskForCardChosen(player, p, "hej", Name, false, HandlingMethod.MethodGet);
                    List<int> ids = new List<int> { id };
                    room.ObtainCard(player, ref ids, new CardMoveReason(MoveReason.S_REASON_EXTRACTION, player.Name, p.Name, Name, string.Empty), false);
                }
            }

            if (player.Alive)
                room.LoseHp(player);

            return false;
        }
    }

    public class Moqu : TriggerSkill
    {
        public Moqu() : base("moqu")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart, TriggerEvent.Damaged };
            frequency = Frequency.Compulsory;
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Finish)
            {
                List<Player> hx = RoomLogic.FindPlayersBySkillName(room, Name);
                foreach (Player p in hx)
                    if (p.HandcardNum < p.Hp)
                        triggers.Add(new TriggerStruct(Name, p));
            }
            else if (triggerEvent == TriggerEvent.Damaged && player.Alive)
            {
                List<Player> hx = RoomLogic.FindPlayersBySkillName(room, Name);
                foreach (Player p in hx)
                    if (p != player && p.Camp == player.Camp && !player.IsNude() && RoomLogic.CanDiscard(room, p, p, "he"))
                        triggers.Add(new TriggerStruct(Name, p));
            }

            return triggers;
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart)
            {
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                room.SendCompulsoryTriggerLog(ask_who, Name, true);
                room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                room.DrawCards(ask_who, 2, Name);
            }
            else
            {
                room.AskForDiscard(ask_who, Name, 1, 1, false, true, "@moqu", false, info.SkillPosition);
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);
                room.SendCompulsoryTriggerLog(ask_who, Name, true);
            }
            return false;
        }
    }

    public class YaowuSP : TriggerSkill
    {
        public YaowuSP() : base("yaowu_sp")
        {
            events.Add(TriggerEvent.Damage);
            frequency = Frequency.Compulsory;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player != null && player.Alive && data is DamageStruct damage && damage.Card != null && base.Triggerable(damage.To, room)
                && damage.Card.Name.Contains(Slash.ClassName) && WrappedCard.IsRed(damage.Card.Suit))
                return new TriggerStruct(Name, damage.To);

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(ask_who, Name);
            room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
            List<string> choices = new List<string> { "draw" };
            if (player.IsWounded())
                choices.Add("recover");

            string choice = room.AskForChoice(player, Name, string.Join("+", choices));
            if (choice == "draw")
            {
                room.DrawCards(player, 1, Name);
            }
            else
            {
                RecoverStruct recover = new RecoverStruct
                {
                    Who = player,
                    Recover = 1
                };
                room.Recover(player, recover, true);
            }

            return false;
        }

    }
    public class PoluSJSP : TriggerSkill
    {
        public PoluSJSP() : base("polu_sj_sp")
        {
            events.Add(TriggerEvent.Death);
            skill_type = SkillType.Replenish;
            frequency = Frequency.Compulsory;
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (data is DeathStruct death)
            {
                if (base.Triggerable(player, room))
                    triggers.Add(new TriggerStruct(Name, player));
                else if (death.Damage.From != null && death.Damage.From.Camp != player.Camp)
                {
                    foreach (Player p in RoomLogic.FindPlayersBySkillName(room, Name))
                        if (p.Camp == death.Damage.From.Camp)
                            triggers.Add(new TriggerStruct(Name, p));
                }
            }

            return triggers;
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(ask_who, Name, true);
            room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
            int count = ask_who.GetMark(Name);
            count++;
            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetAlivePlayers())
                if (p.Camp == ask_who.Camp)
                    targets.Add(p);

            room.SortByActionOrder(ref targets);
            foreach (Player p in targets)
            {
                if (p.Alive)
                {
                    room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, ask_who.Name, p.Name);
                    room.DrawCards(p, new DrawCardStruct(count, ask_who, Name));
                }
            }

            if (ask_who.Alive)
            {
                ask_who.SetMark(Name, count);
                room.SetPlayerStringMark(ask_who, Name, count.ToString());
            }

            return false;
        }
    }
}
