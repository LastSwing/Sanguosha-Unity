using CommonClass.Game;
using SanguoshaServer.Game;
using System.Collections.Generic;
using static CommonClass.Game.Player;

namespace SanguoshaServer.Package
{
    public class ManeuveringCards : CardPackage
    {
        public ManeuveringCards() : base("ManeuveringCards")
        {
            skills = new List<Skill>
            {
                new GudingBladeSkill(),
                new ClassicWoodenOxSkill(),
                new ClassicWoodenOxTriggerSkill(),
            };
            cards = new List<FunctionCard>
            {
                new GudingBlade(),
                new DefensiveHorse("Hualiu"),
                new ClassicWoodenOx(),

                //skillcard
                new ClassicWoodenOxCard(),
            };
        }
    }

    public class GudingBlade : Weapon
    {
        public static string ClassName = "GudingBlade";
        public GudingBlade() : base(ClassName, 2) { }
    }

    public class GudingBladeSkill : WeaponSkill
    {
        public GudingBladeSkill() : base(GudingBlade.ClassName)
        {
            events = new List<TriggerEvent> { TriggerEvent.DamageCaused };
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is DamageStruct damage && damage.To.IsKongcheng()
                && damage.Card != null && damage.Card.Name.Contains(Slash.ClassName) && !damage.Chain && !damage.Transfer && damage.ByUser)
            {
                return new TriggerStruct(Name, player);
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
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
            room.SetEmotion(player, "gudingblade");

            return false;
        }
    }

    public class ClassicWoodenOx : Treasure
    {
        public static string ClassName = "ClassicWoodenOx";
        public ClassicWoodenOx() : base(ClassName) { }
        public override void OnUninstall(Room room, Player player, WrappedCard card)
        {
            player.AddHistory("ClassicWoodenOxCard", 0);
            base.OnUninstall(room, player, card);
        }
    }

    public class ClassicWoodenOxSkill : OneCardViewAsSkill
    {
        public ClassicWoodenOxSkill() : base(ClassicWoodenOx.ClassName)
        {
            filter_pattern = ".|.|.|hand";
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(ClassicWoodenOxCard.ClassName) && player.GetPile("wooden_ox").Count < 5;
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard ox = new WrappedCard(ClassicWoodenOxCard.ClassName);
            ox.AddSubCard(card);
            ox.Skill = Name;
            return ox;
        }
    }

    public class ClassicWoodenOxCard : SkillCard
    {
        public static string ClassName = "ClassicWoodenOxCard";
        public ClassicWoodenOxCard() : base(ClassName)
        {
            target_fixed = true;
            will_throw = false;
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            WrappedCard card = card_use.Card;
            room.AddToPile(card_use.From, "wooden_ox", card.SubCards, false);

            WrappedCard treasure = room.GetCard(card_use.From.Treasure.Key);
            if (treasure != null)
            {
                List<Player> targets = new List<Player>();
                foreach (Player p in room.GetOtherPlayers(card_use.From))
                {
                    if (!p.GetTreasure() && RoomLogic.CanPutEquip(p, treasure))
                        targets.Add(p);
                }
                if (targets.Count == 0)
                    return;
                Player target = room.AskForPlayerChosen(card_use.From, targets, "ClassicWoodenOx", "@wooden_ox-move", true);
                if (target != null)
                {
                    room.MoveCardTo(treasure, card_use.From, target, Place.PlaceEquip,
                        new CardMoveReason(MoveReason.S_REASON_TRANSFER, card_use.From.Name, "ClassicWoodenOx", null));
                }
            }
        }
    }

    public class ClassicWoodenOxTriggerSkill : TreasureSkill
    {
        public ClassicWoodenOxTriggerSkill() : base("ClassicWoodenOx_trigger")
        {
            events.Add(TriggerEvent.CardsMoveOneTime);
            global = true;
            frequency = Frequency.Compulsory;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player target, ref object data, Player ask_who)
        {
            CardsMoveOneTimeStruct move = (CardsMoveOneTimeStruct)data;
            if (move.From == null || !move.From.Alive) return new TriggerStruct();

            Player player = move.From;
            if (player.HasTreasure("ClassicWoodenOx"))
            {
                int count = 0;
                for (int i = 0; i < move.Card_ids.Count; i++)
                    if (move.From_pile_names[i] == "wooden_ox") count++;

                if (count > 0) return new TriggerStruct(Name, player);
            }
            else if (player.GetPile("wooden_ox").Count > 0)
            {
                for (int i = 0; i < move.Card_ids.Count; i++)
                {
                    if (move.From_places[i] != Place.PlaceEquip && move.From_places[i] != Place.PlaceTable) continue;
                    WrappedCard card = Engine.GetRealCard(move.Card_ids[i]);
                    if (card?.Name == "ClassicWoodenOx")
                        return new TriggerStruct(Name, player);
                }
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player target, ref object data, Player ask_who, TriggerStruct info)
        {
            CardsMoveOneTimeStruct move = (CardsMoveOneTimeStruct)data;
            Player player = move.From;
            if (player.HasTreasure("ClassicWoodenOx"))
            {
                int count = 0;
                for (int i = 0; i < move.Card_ids.Count; i++)
                {
                    if (move.From_pile_names[i] == "wooden_ox") count++;
                }
                if (count > 0)
                {
                    LogMessage log = new LogMessage
                    {
                        Type = "#WoodenOx",
                        From = player.Name,
                        Arg = count.ToString(),
                        Arg2 = "wooden_ox"
                    };
                    room.SendLog(log);
                }
            }
            else if (player.GetPile("wooden_ox").Count > 0)
            {
                for (int i = 0; i < move.Card_ids.Count; i++)
                {
                    if (move.From_places[i] != Place.PlaceEquip && move.From_places[i] != Place.PlaceTable) continue;
                    WrappedCard card = Engine.GetRealCard(move.Card_ids[i]);
                    if (card?.Name == "ClassicWoodenOx")
                    {
                        Player to = move.To;
                        if (to != null && to.GetTreasure() && to.Treasure.Value == "ClassicWoodenOx"
                            && move.To_place == Place.PlaceEquip && move.Reason.Reason == MoveReason.S_REASON_TRANSFER)
                        {
                            List<Player> p_list = new List<Player> { to };
                            room.AddToPile(to, "wooden_ox", player.GetPile("wooden_ox"), false, p_list,
                                new CardMoveReason(MoveReason.S_REASON_TRANSFER, player.Name));
                        }
                        else
                        {
                            room.ClearOnePrivatePile(player, "wooden_ox");
                        }
                        return false;
                    }
                }
            }

            return false;
        }
    }
}