using CommonClass.Game;
using SanguoshaServer.Game;
using SanguoshaServer.Package;
using System.Collections.Generic;

namespace SanguoshaServer.AI
{
    public class LordCardsAI : AIPackage
    {
        public LordCardsAI() : base("LordCards")
        {
            use_cards = new List<UseCard>
            {
                new DragonPhoenixAI(),
                new PeaceSpellAI(),
                new LuminouSpearlAI(),
                new DragonCarriageAI(),
            };

            events = new List<SkillEvent> { new LuminouSpearlSkillAI() };
        }
    }

    public class DragonPhoenixAI : UseCard
    {
        public DragonPhoenixAI() : base(DragonPhoenix.ClassName)
        { }

        public override double CardValue(TrustedAI ai, Player player, bool use, WrappedCard card, Player.Place place)
        {
            Room room = ai.Room;
            double value = 0;
            if (!use && ai.HasSkill("zhangwu", player))
                value -= 2;
            else
            {
                Player target = null;
                foreach (Player p in room.GetOtherPlayers(player))
                {
                    if (ai.HasSkill("zhangwu", p))
                    {
                        target = p;
                        break;
                    }
                }

                if (target != null)
                {
                    if (ai.IsFriend(target, player))
                    {
                        if (use && room.GetCardOwner(card.GetEffectiveId()) == player && !player.GetWeapon())
                            value += 2;
                        else
                            value -= 7;
                    }
                    else
                    {
                        if (!ai.HasSkill(TrustedAI.LoseEquipSkill, player) || !use)
                            value -= 6;
                    }
                }
            }

            return value;
        }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is Player p)
            {
                if (!player.HasFlag(Name))
                {
                    if (ai.IsFriend(p, player))
                    {
                        foreach (int id in p.GetEquips())
                            if (ai.GetKeepValue(id, p, Player.Place.PlaceEquip) < 0)
                                return true;

                        return false;
                    }
                    else
                    {
                        foreach (int id in p.GetEquips())
                            if (ai.GetKeepValue(id, p, Player.Place.PlaceEquip) < 0)
                                return false;
                    }
                }
                else
                    return !ai.IsFriend(p, player);
            }

            return true;
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            Player target = null;
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (ai.HasSkill("zhangwu", p))
                {
                    target = p;
                    break;
                }
            }

            if (target != null && !ai.IsFriend(target, player) && !ai.HasSkill(TrustedAI.LoseEquipSkill, player))
                return;

            ai.UseEquipCard(ref use, card);
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return ai.GetEquipPriorityAdjust(card);
        }
    }

    public class PeaceSpellAI : UseCard
    {
        public PeaceSpellAI() : base(PeaceSpell.ClassName)
        { }
        public override double CardValue(TrustedAI ai, Player player, bool use, WrappedCard card, Player.Place place)
        {
            double value = 0;
            Room room = ai.Room;
            if (ai.HasSkill("wendao", player))
            {
                if (use)
                    value += 5;
                else
                {
                    if (!player.HasUsed(WendaoCard.ClassName) || room.Current != player)
                        value -= 10;
                }
            }
            else
            {
                Player target = null;
                foreach (Player p in room.GetOtherPlayers(player))
                {
                    if (ai.HasSkill("wendao", p))
                    {
                        target = p;
                        break;
                    }
                }

                if (target != null)
                {
                    if (!ai.HasSkill("flamemap", player) || !use)
                        value -= 7;
                }
                else
                {
                    foreach (Player p in room.GetOtherPlayers(player))
                        if (RoomLogic.IsFriendWith(room, player, p))
                            value += 0.5;
                }

                if (!use && player.Hp == 1 && player.HandcardNum <= 1 && place == Player.Place.PlaceEquip)
                    value -= 3;
            }
            if (player.Chained) value += 1;

            return value;
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            Player target = null;
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (ai.HasSkill("wendao", p))
                {
                    target = p;
                    break;
                }
            }

            bool will_use = true;
            if (target != null && !ai.IsFriend(target, player))
            {
                will_use = false;
                if (ai.GetOverflow(player) > 0 && room.GetCardPlace(card.GetEffectiveId()) == Player.Place.PlaceHand && room.GetCardOwner(card.GetEffectiveId()) == player)
                    will_use = true;
            }

            if (will_use)
                ai.UseEquipCard(ref use, card);
        }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return ai.GetEquipPriorityAdjust(card);
        }
    }

    public class LuminouSpearlAI : UseCard
    {
        public LuminouSpearlAI() : base(LuminouSpearl.ClassName)
        { }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            Player target = null;
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (ai.HasSkill("jubao", p))
                {
                    target = p;
                    break;
                }
            }

            if (target != null && !ai.IsFriend(target, player))
                return;

            ai.UseEquipCard(ref use, card);
        }

        public override double CardValue(TrustedAI ai, Player player, bool use, WrappedCard card, Player.Place place)
        {
            double value = 0;
            Room room = ai.Room;
            Player target = null;
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (ai.HasSkill("jubao", p))
                {
                    target = p;
                    break;
                }
            }

            bool self_use = true;
            if (target != null)
            {
                if (ai.IsFriend(target, player))
                {
                    value += 5;
                }
                else
                {
                    self_use = false;
                    value -= 10;
                }
            }
            
            if (self_use && ai.HasSkill(TrustedAI.LoseEquipSkill, player))
                value += 6;

            return value;
        }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return ai.GetEquipPriorityAdjust(card);
        }
    }
    public class LuminouSpearlSkillAI : SkillEvent
    {
        public LuminouSpearlSkillAI() : base(LuminouSpearl.ClassName)
        {}

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!ai.HasSkill("zhiheng") && !player.HasUsed("ZhihengCard") && player.GetCards("he").Count > 1)
                return new List<WrappedCard> { new WrappedCard("ZhihengCard") { Skill = Name, Mute = true } };

            return null;
        }
    }

    public class DragonCarriageAI : UseCard
    {
        public DragonCarriageAI() : base(DragonCarriage.ClassName)
        {}

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            if (player.GetOffensiveHorse() && player.GetDefensiveHorse() && !ai.HasSkill(TrustedAI.LoseEquipSkill, player))
                return;

            ai.UseEquipCard(ref use, card);
        }

        public override double CardValue(TrustedAI ai, Player player, bool use, WrappedCard card, Player.Place place)
        {
            return ai.HasSkill(TrustedAI.LoseEquipSkill, player) ? -4 : 0;
        }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return ai.GetEquipPriorityAdjust(card);
        }
    }
}