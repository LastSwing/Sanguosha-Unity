using System;
using System.Collections.Generic;
using CommonClass;
using CommonClass.Game;
using SanguoshaServer.Game;
using SanguoshaServer.Package;

namespace SanguoshaServer.AI
{
    public class MythAI : AIPackage
    {
        public MythAI() : base("Myth")
        {
            events = new List<SkillEvent>
            {
                new GuixinAI(),
                new PoxiAI(),
                new WumouAI(),
                new CuikeAI(),
            };

            use_cards = new List<UseCard>
            {
                new PoxiCardAI(),
                new ZhanhuoCardAI(),
            };
        }
    }


    public class GuixinAI : SkillEvent
    {
        public GuixinAI() : base("guixin")
        {
        }

        public override ScoreStruct GetDamageScore(TrustedAI ai, DamageStruct damage)
        {
            ScoreStruct score = new ScoreStruct
            {
                Score = 0
            };
            if (ai.HasSkill(Name, damage.To) && damage.Damage <= damage.To.Hp)
            {
                double value = 0;
                if (!damage.To.FaceUp && damage.Damage == 1) value += 4;
                Room room = ai.Room;
                foreach (Player p in room.GetOtherPlayers(damage.To))
                {
                    if (RoomLogic.CanGetCard(room, damage.To, p, "hej"))
                    {
                        value += 0.3;
                        if (ai.IsEnemy(damage.To, p))
                        {
                            if (!p.IsNude()) value += 0.5;
                            else
                                value -= 1;
                        }
                    }
                }
                if (damage.Damage > 1) value *= (0.6 * (damage.Damage - 1)) + 1;
                if (value > 0 && damage.Damage >= damage.To.Hp)
                    value /= 2;

                if (!ai.IsFriend(damage.To))
                    value = -value;

                score.Score = value;
            }

            return score;
        }
    }

    public class PoxiAI : SkillEvent
    {
        public PoxiAI() : base("poxi")
        {
            key = new List<string> { "poxichose" };
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string choice && player != ai.Self)
            {
                string[] choices = choice.Split(':');
                List<string> strs = new List<string>(choices[3].Split('+'));
                List<int> ids = JsonUntity.StringList2IntList(strs);

                foreach (int id in ids)
                {
                    Player own = ai.Room.GetCardOwner(id);
                    if (own != player)
                    {
                        ai.UpdatePlayerRelation(player, own, false);
                        break;
                    }
                }
            }
        }
    }

    public class PoxiCardAI : UseCard
    {
        public PoxiCardAI() : base(PoxiCard.ClassName) { }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {
                Player target = use.To[0];
                List<int> ids = target.GetCards("h");
                foreach (int id in ids)
                    ai.SetPrivateKnownCards(target, id);
            }
        }
    }

    public class WumouAI : SkillEvent
    {
        public WumouAI() : base("wumou") { }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, bool isUse, Player.Place place)
        {
            if (ai.HasSkill(Name, player) && card != null && Engine.GetFunctionCard(card.Name) is TrickCard)
                return -3;

            return 0;
        }
    }

    public class CuikeAI : SkillEvent
    {
        public CuikeAI() : base("cuike")
        {
            key = new List<string> { "playerChosen:cuike", "cardChosen:cuike" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str)
            {
                Room room = ai.Room;
                string[] strs = str.Split(':');
                if (str.StartsWith("playerChosen:cuike") && strs[1] == Name)
                {
                    Player target = room.FindPlayer(strs[2]);
                    if (ai.GetPlayerTendency(target) != "unknown" && ai is StupidAI _ai)
                    {
                         if (!player.HasFlag(Name) && !_ai.NeedDamage(new DamageStruct(Name, player, target)))
                            ai.UpdatePlayerRelation(player, target, false);
                         else if (player.HasFlag(Name) && player.IsAllNude())
                            ai.UpdatePlayerRelation(player, target, false);
                    }
                }
                else if (str.StartsWith("cardChosen:cuike"))
                {

                    int id = int.Parse(strs[2]);
                    Player target = room.FindPlayer(strs[3]);
                    if (ai.GetPlayerTendency(target) != "unknown")
                    {
                        if (room.GetCardPlace(id) == Player.Place.PlaceEquip)
                            ai.UpdatePlayerRelation(player, target, ai.GetKeepValue(id, target) > 0);
                        else if (room.GetCardPlace(id) == Player.Place.PlaceDelayedTrick)
                            ai.UpdatePlayerRelation(player, target, true);
                        else
                            ai.UpdatePlayerRelation(player, target, false);
                    }
                }
            }
        }
    }

    public class ZhanhuoCardAI : UseCard
    {
        public ZhanhuoCardAI() : base(ZhanhuoCard.ClassName) { }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {
                foreach (Player p in use.To)
                {
                    if (ai.GetPlayerTendency(p) != "unknown")
                        ai.UpdatePlayerRelation(player, p, false);
                }
            }
        }
    }
}