using System;
using System.Collections.Generic;
using CommonClass.Game;
using SanguoshaServer.Game;

namespace SanguoshaServer.AI
{
    public class StanderQunAI : AIPackage
    {
        public StanderQunAI() : base("Stander-qun")
        {
            events = new List<SkillEvent>
            {
                new WeimuAI(),
                new SuishiAI(),
            };

            use_cards = new List<UseCard>
            {
            };
        }
    }

    public class WeimuAI : SkillEvent
    {
        public WeimuAI() : base("weimu")
        { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class SuishiAI : SkillEvent
    {
        public SuishiAI() : base("suishi")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            TriggerEvent e = (TriggerEvent)(int)data;
            if (e == TriggerEvent.Dying)
                return true;

            return false;
        }
    }

    public class SijianAI : SkillEvent
    {
        public SijianAI() : base("sijian")
        {
        }
        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> target, int min, int max)
        {
            Room room = ai.Room;
            List<ScoreStruct> scores = new List<ScoreStruct>();
            foreach (Player p in room.GetOtherPlayers(player))
            {
                ScoreStruct score = ai.FindCards2Discard(player, p, "he", FunctionCard.HandlingMethod.MethodDiscard);
                scores.Add(score);
            }
            if (scores.Count > 0)
            {
                scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                if (scores[0].Score > 0)
                {
                    return scores[0].Players;
                }
            }

            return null;
        }
    }
}