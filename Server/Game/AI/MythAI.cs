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
            };

            use_cards = new List<UseCard>
            {
                new PoxiCardAI(),
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
}