using CommonClass.Game;
using CommonClassLibrary;
using SanguoshaServer.Game;
using System.Collections.Generic;
using System.Threading;

namespace SanguoshaServer.Package
{
    public class ClassicStanderCards : CardPackage
    {
        public ClassicStanderCards() : base("ClassicStanderCards")
        {
            skills = new List<Skill>
            {
                new ClassicBladeSkill(),
                new BladeTag(),
                new ClassicHalberdSkill(),
                new SaberSkill(),  //七宝刀
            };
            cards = new List<FunctionCard> {
                new ClassicBlade(),
                new ClassicHalberd(),
                new Saber(),            //七宝刀
                new HiddenDagger(),     //笑里藏刀
                new HoneyTrap(),        //美人计
            };

            related_skills = new Dictionary<string, List<string>>
            {
                { ClassicBlade.ClassName, new List<string> { "#blade-target-mod" } },
            };
        }
    }

    public class ClassicBlade : Weapon
    {
        public static string ClassName = "ClassicBlade";
        public ClassicBlade() : base(ClassName, 3) { }
    }

    public class ClassicBladeSkill : WeaponSkill
    {
        public ClassicBladeSkill() : base(ClassicBlade.ClassName)
        {
            events = new List<TriggerEvent> { TriggerEvent.SlashMissed };
            frequency = Frequency.Compulsory;
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            SlashEffectStruct effect = (SlashEffectStruct)data;
            if (!effect.To.Alive || effect.To.GetMark("Equips_of_Others_nullified_to_You") > 0)
                return false;

            player.SetFlags("slashTargetFix");
            player.SetFlags("slashTargetFixToOne");
            room.SetCardFlag(player.Weapon.Key, "using");
            effect.To.SetFlags("SlashAssignee");

            WrappedCard slash = room.AskForUseCard(player, "Slash:ClassicBlade", "@blade:" + effect.To.Name, null, -1, FunctionCard.HandlingMethod.MethodUse, false);
            if (slash == null)
            {
                player.SetFlags("-slashTargetFix");
                player.SetFlags("-slashTargetFixToOne");
                effect.To.SetFlags("-SlashAssignee");
                if (player.HasWeapon(Name))
                    room.SetCardFlag(player.Weapon.Key, "-using");
            }

            return false;
        }
    }

    public class BladeTag : TargetModSkill
    {
        public BladeTag() : base("#blade-target-mod", false) {}
        public override bool GetDistanceLimit(Room room, Player from, Player to, WrappedCard card, CardUseStruct.CardUseReason reason, string pattern)
        {
            if (from.HasWeapon("ClassicBlade") && reason == CardUseStruct.CardUseReason.CARD_USE_REASON_RESPONSE_USE
                && (room.GetRoomState().GetCurrentResponseSkill() == "ClassicBlade" || pattern == "Slash:ClassicBlade"))
                return true;

            return false;
        }
    }

    public class ClassicHalberd : Weapon
    {
        public static string ClassName = "ClassicHalberd";
        public ClassicHalberd() : base(ClassName, 4) { }
    }

    public class ClassicHalberdSkill : TargetModSkill
    {
        public ClassicHalberdSkill() : base(ClassicHalberd.ClassName) { }

        public override int GetExtraTargetNum(Room room, Player from, WrappedCard card)
        {
            if (from.HasWeapon(Name) && card.SubCards.Count > 0 && from.IsLastHandCard(card, true) && !card.SubCards.Contains(from.Weapon.Key))
                return 2;

            return 0;
        }
    }

    public class Saber : Weapon
    {
        public static string ClassName = "Saber";
        public Saber() : base(ClassName, 2) { }
    }
    public class SaberSkill : WeaponSkill
    {
        public SaberSkill() : base(Saber.ClassName)
        {
            events = new List<TriggerEvent> { TriggerEvent.TargetChosen, TriggerEvent.DamageCaused };
            frequency = Frequency.Compulsory;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.TargetChosen && data is CardUseStruct use && base.Triggerable(player, room) && use.Card.Name.Contains(Slash.ClassName) && use.To.Count > 0)
                return new TriggerStruct(Name, player, use.To);
            else if (triggerEvent == TriggerEvent.DamageCaused && data is DamageStruct damage && base.Triggerable(player, room) && !damage.To.IsWounded()
                && damage.To.Alive && damage.Card != null && damage.Card.Name.Contains(Slash.ClassName) && !damage.Chain && !damage.Transfer)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SetEmotion(ask_who, "saber");
            Thread.Sleep(400);
            return info;
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player target, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.TargetChosen && data is CardUseStruct use)
            {
                target.AddQinggangTag(RoomLogic.CardToString(room, use.Card));
            }
            else if (triggerEvent == TriggerEvent.DamageCaused && data is DamageStruct damage)
            {
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, ask_who.Name, damage.To.Name);
                LogMessage log = new LogMessage
                {
                    Type = "#AddDamage",
                    From = ask_who.Name,
                    To = new List<string> { damage.To.Name },
                    Arg = Name,
                    Arg2 = (++damage.Damage).ToString()
                };

                room.SendLog(log);
                data = damage;
            }

            return false;
        }
    }

    public class HiddenDagger : TrickCard
    {
        public static string ClassName = "HiddenDagger";
        public override string GetSubtype() => "single_target_trick";
        public HiddenDagger() : base(ClassName)
        {
        }

        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            Player player = effect.From, target = effect.To;
            int count = target.GetLostHp();
            if (count > 0) room.DrawCards(target, new DrawCardStruct(count, player, Name));
            if (player.Alive && target.Alive)
                room.Damage(new DamageStruct(effect.Card, player, target));
        }
    }

    public class HoneyTrap : TrickCard
    {
        public static string ClassName = "HoneyTrap";
        public override string GetSubtype() => "single_target_trick";
        public HoneyTrap() : base(ClassName)
        {
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return base.TargetFilter(room, targets, to_select, Self, card) && to_select.IsMale() && !to_select.IsKongcheng();
        }

        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            Player player = effect.From, target = effect.To;
            List<Player> players = new List<Player>();
            foreach (Player p in room.GetAlivePlayers())
                if (p.IsFemale()) players.Add(p);

            if (players.Count > 0)
            {
                room.SortByActionOrder(ref players);
                foreach (Player p in players)
                {
                    if (target.Alive && !target.IsKongcheng() && RoomLogic.CanGetCard(room, p, target, "h"))
                    {
                        int id = room.AskForCardChosen(p, target, "h", Name, false, HandlingMethod.MethodGet);
                        List<int> ids = new List<int> { id };
                        room.ObtainCard(p, ref ids, new CardMoveReason(MoveReason.S_REASON_EXTRACTION, p.Name, target.Name, Name, string.Empty), false);

                        if (p != player && p.Alive && player.Alive)
                        {
                            ids = p.GetCards("h");
                            if (ids.Count > 1)
                                ids = room.AskForExchange(p, Name, 1, 1, "@HoneyTrap:" + player.Name, string.Empty, ".", string.Empty);

                            if (ids.Count > 0)
                                room.ObtainCard(player, ref ids, new CardMoveReason(MoveReason.S_REASON_GIVE, p.Name, player.Name, Name, string.Empty), false);
                        }
                    }
                }
            }

            if (player.Alive && target.Alive && player.HandcardNum != target.HandcardNum)
            {
                if (player.HandcardNum < target.HandcardNum)
                    room.Damage(new DamageStruct(effect.Card, player, target));
                else
                    room.Damage(new DamageStruct(effect.Card, target, player));
            }
        }
    }
}