using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonClass.Game;
using SanguoshaServer.Game;

namespace SanguoshaServer.AI
{
    public class StanderAI : AIPackage
    {
        public StanderAI() : base("Stander")
        {
            use_cards = new List<UseCard>
            {
                new SlashAI(),
            };
        }
    }

    public class SlashAI : UseCard
    {
        public SlashAI() : base("Slash")
        {
            key = new List<string> { "slash-jink", "@multi-jink" };
        }

        public override void DamageEffect(TrustedAI ai, ref DamageStruct damage)
        {
            if (damage.From != null && damage.From.Alive)
                damage.Damage += damage.From.GetMark("drank");
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            Room room = ai.Room;
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {

                foreach (Player p in use.To)
                    if (ai.GetPossibleId(player).Count == 1 || ai.GetPossibleId(p).Count == 1 || ai.IsKnown(player, p))
                        ai.UpdatePlayerRelation(player, p, false);          //若杀的使用者和目标中的一方身份已判明，则更新双方关系为敌对
            }

            if (triggerEvent == TriggerEvent.ChoiceMade)
            {
                string str = (string)data;
                if (str.StartsWith("cardResponded"))
                {
                    List<string> strs = new List<string>(str.Split(':'));
                    if (strs[strs.Count - 1] == "_nil_")
                    {
                        string prompt = strs[2];
                        if (!prompt.StartsWith("@multi-jink") || strs[strs.Count - 2] == "1")
                        {
                            List<CardUseStruct> uses = (List<CardUseStruct>)room.GetTag("card_proceeing");
                            use = uses[uses.Count - 1];
                            if (use.Card != null && use.Card.Name.Contains("Slash"))
                            {
                                DamageStruct damage = new DamageStruct(use.Card, use.From, player);
                                if (use.Card.Name == "FireSlash")
                                    damage.Nature = DamageStruct.DamageNature.Fire;
                                else if (use.Card.Name == "ThunderSlash")
                                    damage.Nature = DamageStruct.DamageNature.Thunder;

                                ScoreStruct score = ai.GetDamageScore(damage);
                                bool lack = true;
                                if (ai.IsFriend(use.From, player) && score.Score > 0)
                                    lack = false;
                                else if (ai.IsEnemy(player) && score.Score < 0)
                                    lack = false;

                                if (lack) ai.SetCardLack(player, "Jink");
                            }
                        }
                    }
                }
            }
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data, FunctionCard.HandlingMethod method)
        {
            if (prompt.StartsWith("slash-jink") || prompt.StartsWith("@multi-jink"))
            {
                SlashEffectStruct effect = (SlashEffectStruct)data;
                DamageStruct damage = new DamageStruct(effect.Slash, effect.From, effect.To);
                if (effect.Slash.Name == "FireSlash")
                    damage.Nature = DamageStruct.DamageNature.Fire;
                else if (effect.Slash.Name == "ThunderSlash")
                    damage.Nature = DamageStruct.DamageNature.Thunder;
                ScoreStruct score = ai.GetDamageScore(damage);
                if (score.Damage.Damage > 0 && player.Chained && score.Damage.Nature != DamageStruct.DamageNature.Normal)
                {
                    if (ai.IsGoodSpreadStarter(damage))
                        score.Score += 6;
                    else if (ai.IsGoodSpreadStarter(damage, false))
                        score.Score -= 8;
                }

                CardUseStruct use = new CardUseStruct
                {
                    From = player
                };
                List <WrappedCard> cards = ai.GetCards("Jink", player);
                if (score.Score == 0 || cards.Count == 0) return use;

                int rest = 1;
                if (prompt.StartsWith("@multi-jink"))
                {
                    List<string> strs = new List<string>(prompt.Split(':'));
                    rest = int.Parse(strs[strs.Count - 1]);
                }
                WrappedCard result = null;
                int available = 0;
                foreach (WrappedCard jink in cards) {
                    if (!RoomLogic.IsCardLimited(ai.Room, player, jink, FunctionCard.HandlingMethod.MethodUse))
                    {
                        available++;
                        result = jink;
                    }
                }
                if (result == null) result = cards[0];

                int rate = 1;
                if (ai.WillSkipPlayPhase(player) || !ai.WillShowForAttack() || ai.GetOverflow(player) > 0) rate = 2;
                double value = -ai.GetKeepValue(result, player);
                if (value < 0) value /= rate;
                if (ai.HasSkill("leiji", player)) value += 2;
                if (result.Skill == "longdan")
                {
                    if (ai.HasSkill("chongzhen", player) && !ai.IsFriend(player, effect.From) && !effect.From.IsKongcheng())
                        value += 2;

                    foreach (Player p in ai.Room.GetOtherPlayers(player))
                    {
                        if (p.IsWounded() && ai.IsFriend(p, player))
                            value += 3;
                    }
                }

                if (damage.Damage > 0 && score.Score < -2 && ai.HasSkill("tianxiang", player))
                {
                    SkillEvent tianxiang = Engine.GetSkillEvent("tianxiang");
                    if (tianxiang != null)
                    {
                        CardUseStruct tianxiang_use = tianxiang.OnResponding(ai, player, "@@tianxiang", string.Empty, damage, FunctionCard.HandlingMethod.MethodUse);
                        if (tianxiang_use.Card != null && tianxiang_use.To != null && tianxiang_use.To.Count > 0)
                            score.Score = -2;
                    }
                }

                if (rest > 1)
                {
                    double need = rest;
                    if (ai.HasArmorEffect(player, "EightDiagram"))
                        need = (rest - 1) * 0.6 + 1;

                    if (available >= need)
                        value -= 4 * (int)(need - 1);
                    else
                        value += score.Score * Math.Min(1, need - 1);
                }

                if (value >= score.Score) use.Card = result;

                return use;
            }

            return new CardUseStruct();
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            use = ai.FindSlashandTarget(player);
        }
    }
}
