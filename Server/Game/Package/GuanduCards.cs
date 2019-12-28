using CommonClass.Game;
using CommonClassLibrary;
using SanguoshaServer.Game;
using System.Collections.Generic;
using System.Threading;

namespace SanguoshaServer.Package
{
    public class GuanduCards : CardPackage
    {
        public GuanduCards() : base("GuanduCards")
        {
            skills = new List<Skill>
            {
                new CatapultSkill(),
            };
            cards = new List<FunctionCard>
            {
                new Catapult(),
                new HoardUp(),
                new Reinforcement(),
                new GDFighttogether(),
            };
        }
    }

    //霹雳车
    public class Catapult : Weapon
    {
        public static string ClassName = "Catapult";
        public Catapult() : base(ClassName, 9) { }
    }

    public class CatapultSkill : WeaponSkill
    {
        public CatapultSkill() : base(Catapult.ClassName)
        {
            events = new List<TriggerEvent> { TriggerEvent.Damage, TriggerEvent.BeforeCardsMove };
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.BeforeCardsMove && data is CardsMoveOneTimeStruct move)
            {
                if (move.From != null && move.From_places.Contains(Player.Place.PlaceEquip) && move.To_pile_name != "#virtual_cards")
                {
                    int catapult = -1, card_index = -1;
                    foreach (int id in move.Card_ids)
                    {
                        int index = move.Card_ids.IndexOf(id);
                        if (move.From_places[index] == Player.Place.PlaceEquip && room.GetCard(id).Name == Name)
                        {
                            catapult = id;
                            card_index = index;
                            break;
                        }
                    }

                    if (catapult > -1)
                    {
                        move.From_places.RemoveAt(card_index);
                        move.Card_ids.Remove(catapult);
                        data = move;

                        Player holder = room.Players[0];
                        room.SetEmotion(move.From, "catapult_broken");
                        room.AddToPile(holder, "#virtual_cards", catapult, false);
                    }
                }
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.Damage && base.Triggerable(player, room) && data is DamageStruct damage)
            {
                if (damage.To != player && damage.To.Alive && (damage.To.GetArmor() || damage.To.GetDefensiveHorse()))
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is DamageStruct damage && (damage.To.GetArmor() || damage.To.GetDefensiveHorse())
                && room.AskForSkillInvoke(player, Name, damage.To))
            {
                room.SetEmotion(player, "catapult_invoke");
                Thread.Sleep(100);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is DamageStruct damage && damage.To.Alive)
            {
                room.SetEmotion(damage.To, "catapult_hit");
                Thread.Sleep(200);

                List<int> ids = new List<int>();
                if (damage.To.GetArmor()) ids.Add(damage.To.Armor.Key);
                if (damage.To.GetDefensiveHorse()) ids.Add(damage.To.DefensiveHorse.Key);

                room.ThrowCard(ref ids, damage.To, player, Name);
            }

            return false;
        }
    }

    //屯粮
    public class HoardUp : TrickCard
    {
        public static string ClassName = "HoardUp";
        public static FunctionCard Instance = null;
        public HoardUp() : base(ClassName)
        {
            Instance = this;
        }

        public override string GetSubtype()
        {
            return "hoard_up";
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            int total_num = 3 + Engine.CorrectCardTarget(room, TargetModSkill.ModType.ExtraMaxTarget, Self, card);
            if (targets.Count >= total_num)
                return false;

            return !RoomLogic.IsCardLimited(room, Self, card, HandlingMethod.MethodUse) && Engine.IsProhibited(room, Self, to_select, card, targets) == null;
        }

        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            room.DrawCards(effect.To, new DrawCardStruct(1, effect.From, Name));
        }
    }
    //援军
    public class Reinforcement : TrickCard
    {
        public static string ClassName = "Reinforcement";
        public static FunctionCard Instance = null;
        public Reinforcement() : base(ClassName) { Instance = this; }
        public override string GetSubtype()
        {
            return "reinforcement";
        }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            int total_num = 2 + Engine.CorrectCardTarget(room, TargetModSkill.ModType.ExtraMaxTarget, Self, card);
            if (targets.Count >= total_num)
                return false;

            return !RoomLogic.IsCardLimited(room, Self, card, HandlingMethod.MethodUse)
                && Engine.IsProhibited(room, Self, to_select, card, targets) == null && to_select != Self && to_select.IsWounded();
        }

        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            if (effect.To.IsWounded())
            {
                RecoverStruct recover = new RecoverStruct
                {
                    Recover = 1,
                    Who = effect.From,
                    Card = effect.Card
                };
                room.Recover(effect.To, recover, true);
            }
        }
    }
    //戮力同心·官渡
    public class GDFighttogether : TrickCard
    {
        public static string ClassName = "GDFighttogether";
        public static FunctionCard Instance = null;
        public GDFighttogether() : base(ClassName) { Instance = this; }

        public override string GetSubtype()
        {
            return "gd_fight_together";
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            if (targets.Count > 0 || RoomLogic.IsCardLimited(room, Self, card, HandlingMethod.MethodUse))
                return false;

            Game3v3Camp camp = to_select.Camp;
            foreach (Player p in room.GetAlivePlayers())
            {
                if (Engine.IsProhibited(room, Self, p, card, targets) != null) continue;
                if (Self.Camp == camp && p.Camp == camp && p.Chained)
                    return true;
                else if (Self.Camp != camp && p.Camp == camp && !p.Chained)
                    return true;
            }

            return false;
        }

        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            if (targets.Count == 0)
                return CanRecast(room, Self, card);

            return true;
        }

        public override void OnUse(Room room, CardUseStruct card_use)
        {
            if (card_use.To.Count == 0)
            {
                DoRecast(room, card_use);
            }
            else
            {
                Player player = card_use.From;
                CardUseStruct use = new CardUseStruct(card_use.Card, player, new List<Player>());
                List<Player> targets = new List<Player>();
                Game3v3Camp camp = card_use.To[0].Camp;
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (player.Camp == camp && p.Camp == camp && p.Chained)
                        targets.Add(p);
                    else if (player.Camp != camp && p.Camp == camp && !p.Chained)
                        targets.Add(p);
                }

                foreach (Player p in targets)
                {
                    Skill skill = Engine.IsProhibited(room, player, p, card_use.Card);
                    if (skill != null)
                    {
                        skill = Engine.GetMainSkill(skill.Name);
                        LogMessage log = new LogMessage
                        {
                            Type = "#SkillAvoid",
                            From = p.Name,
                            Arg = skill.Name,
                            Arg2 = Name
                        };
                        room.SendLog(log);
                        if (RoomLogic.PlayerHasShownSkill(room, p, skill))
                        {
                            room.NotifySkillInvoked(p, skill.Name);
                            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, p, skill.Name);
                            string genral = gsk.General;
                            int skin_id = gsk.SkinId;
                            string skill_name = skill.Name;
                            int audio = -1;
                            skill.GetEffectIndex(room, p, card_use.Card, ref audio, ref skill_name, ref genral, ref skin_id);
                            if (audio >= -1)
                                room.BroadcastSkillInvoke(skill_name, "male", audio, genral, skin_id);
                        }
                    }
                    else
                        use.To.Add(p);
                }

                room.SortByActionOrder(ref use);
                base.OnUse(room, use);
            }
        }

        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            Player player = effect.From;
            if (player.Camp == effect.To.Camp && effect.To.Chained)
                room.DrawCards(effect.To, new DrawCardStruct(1, player, Name));
            else if (player.Camp != effect.To.Camp && !effect.To.Chained && RoomLogic.CanBeChainedBy(room, effect.To, player))
                room.SetPlayerChained(effect.To, true);
        }
    }
}
