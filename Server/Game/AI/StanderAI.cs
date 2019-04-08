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
            events = new List<SkillEvent>
            {
                new JianxiongAI(),
                new GuicaiAI(),
                new FankuiAI(),
                new GanglieAI(),
                new TuxiAI(),
                new LuoyiAI(),
                new TianduAI(),
            };
        }
    }

    public class JianxiongAI : SkillEvent
    {
        public JianxiongAI() : base("jianxiong")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (!ai.WillShowForMasochism()) return false;

            return !ai.NeedKongcheng(player);
        }

        public override ScoreStruct GetDamageScore(TrustedAI ai, DamageStruct damage)
        {
            ScoreStruct score = new ScoreStruct
            {
                Score = 0
            };

            if (damage.To != null && ai.HasSkill(Name, damage.To) && damage.Card != null)
            {
                FunctionCard fcard = Engine.GetFunctionCard(damage.Card.Name);
                if (!(fcard is SkillCard))
                {
                    foreach (int id in damage.Card.SubCards)
                    {
                        double value = Math.Max(ai.GetUseValue(id, damage.To, Player.Place.PlaceHand), ai.GetKeepValue(id, damage.To, Player.Place.PlaceHand));
                        score.Score += value;
                    }

                    if (ai.WillSkipPlayPhase(damage.To))
                        score.Score /= 3;

                    if (damage.Damage >= damage.To.Hp)
                        score.Score /= 2;
                }
            }

            return score;
        }
    }

    public class FankuiAI : SkillEvent
    {
        public FankuiAI() : base("fankui")
        {
            key = new List<string> { "cardChosen" };
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is DamageStruct damage)
            {
                Player to = damage.To;
                List<int> disable = new List<int>();
                foreach (int id in player.GetCards("he"))
                {
                    if (!RoomLogic.CanGetCard(ai.Room, player, to, id))
                        disable.Add(id);
                }

                if (ai.FindCards2Discard(player, damage.From, "he", FunctionCard.HandlingMethod.MethodGet, 1, true, disable).Score > 0)
                    return true;
            }

            return false;
        }

        public override ScoreStruct GetDamageScore(TrustedAI ai, DamageStruct damage)
        {
            ScoreStruct score = new ScoreStruct
            {
                Score = 0
            };
            if (damage.To != null && damage.From != null && !damage.From.IsNude() && damage.Damage <= damage.To.Hp)
            {
                if (ai.IsFriend(damage.From, damage.To) && damage.From.HasEquip())
                {
                    if (ai.HasArmorEffect(damage.From, "SilverLion")) score.Score += 2;
                }
                else if (!ai.IsFriend(damage.From, damage.To))
                {

                }

                if (damage.Damage == damage.To.Hp)
                    score.Score /= 4;
            }
            return score;
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (data is string choice && ai.Self != player)
            {
                string[] choices = choice.Split(':');
                if (choices[1] == Name)
                {
                    Room room = ai.Room;
                    int id = int.Parse(choices[2]);
                    Player target = room.FindPlayer(choices[4]);

                    if (ai.GetPlayerTendency(target) == "unknown" && ai.HasArmorEffect(target, "SilverLion") && id == target.Armor.Key)
                        ai.UpdatePlayerRelation(player, target, true);
                }
            }
        }

        public override List<int> OnCardsChosen(TrustedAI ai, Player from, Player to, string flags, int min, int max, List<int> disable_ids, object data)
        {
            return ai.FindCards2Discard(from, to, "he", FunctionCard.HandlingMethod.MethodGet, 1, true, disable_ids).Ids;
        }
    }

    public class GuicaiAI : SkillEvent
    {
        public GuicaiAI() : base("guicai")
        {
            key = new List<string> { "cardResponded" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string choice && ai.Self != player)
            {
                Room room = ai.Room;
                string[] choices = choice.Split(':');
                if (choices[2] == Name && room.GetTag(Name) is JudgeStruct judge && ai.GetPlayerTendency(judge.Who) == "unknown" && choices[4] != "_nil_")
                {
                    string str = choices[4].Substring(1, choices[4].Length - 2);
                    WrappedCard card = RoomLogic.ParseCard(room, str);
                    WrappedCard.CardSuit suit = RoomLogic.GetCardSuit(room, card);
                    int number = RoomLogic.GetCardNumber(room, card);

                    if (judge.Reason == "beige")
                    {
                        if (!judge.Who.FaceUp && suit == WrappedCard.CardSuit.Spade)
                            ai.UpdatePlayerRelation(player, judge.Who, true);
                    }
                    else if (judge.Reason == "Lightning")
                    {
                        if (judge.Card.Suit == WrappedCard.CardSuit.Spade && judge.Card.Number > 1 && judge.Card.Number <= 9
                            && (suit != WrappedCard.CardSuit.Spade || number > 9 || number == 1))
                            ai.UpdatePlayerRelation(player, judge.Who, true);
                        else if (suit == WrappedCard.CardSuit.Spade && number > 1 && number <= 9)
                            ai.UpdatePlayerRelation(player, judge.Who, false);
                    }
                    else if (judge.Reason == "SupplyShortage")
                    {
                        if (suit != WrappedCard.CardSuit.Club)
                            ai.UpdatePlayerRelation(player, judge.Who, false);
                        else if (suit == WrappedCard.CardSuit.Club)
                            ai.UpdatePlayerRelation(player, judge.Who, true);
                    }
                    else if (judge.Reason == "ganglie")
                    {
                        if (judge.Card.Suit == WrappedCard.CardSuit.Heart && suit != WrappedCard.CardSuit.Heart)
                            ai.UpdatePlayerRelation(player, judge.Who, false);
                        else if (suit == WrappedCard.CardSuit.Heart)
                            ai.UpdatePlayerRelation(player, judge.Who, true);
                    }
                    else if (judge.Reason == "Indulgence")
                    {
                        if (judge.Card.Suit == WrappedCard.CardSuit.Heart && suit != WrappedCard.CardSuit.Heart)
                            ai.UpdatePlayerRelation(player, judge.Who, false);
                        else if (suit == WrappedCard.CardSuit.Heart)
                            ai.UpdatePlayerRelation(player, judge.Who, true);
                    }
                    else if (judge.Reason == "EightDiagram")
                    {
                        if (WrappedCard.IsRed(judge.Card.Suit) && WrappedCard.IsBlack(suit))
                            ai.UpdatePlayerRelation(player, judge.Who, false);
                        else if (WrappedCard.IsRed(suit))
                            ai.UpdatePlayerRelation(player, judge.Who, true);
                    }
                    else if (judge.Reason == "leiji")
                    {
                        if (judge.Card.Suit == WrappedCard.CardSuit.Spade && suit != WrappedCard.CardSuit.Spade)
                            ai.UpdatePlayerRelation(player, judge.Who, true);
                        else if (suit == WrappedCard.CardSuit.Spade)
                            ai.UpdatePlayerRelation(player, judge.Who, false);
                    }
                }
            }
        }
        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, bool isUse, Player.Place place)
        {
            if (ai.HasSkill(Name, player) && place != Player.Place.PlaceEquip && !isUse)
            {
                WrappedCard.CardSuit suit = RoomLogic.GetCardSuit(ai.Room, card);
                if (suit == WrappedCard.CardSuit.Heart)
                    return 1;
                else if (suit == WrappedCard.CardSuit.Club)
                    return 0.5;
                else
                    return 0.2;
            }
            return 0;
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct();
            if (data is JudgeStruct judge)
            {
                int id = ai.GetRetrialCardId(player.GetCards("h"), judge);
                if (id >= 0)
                    use.Card = ai.Room.GetCard(id);
            }

            return use;
        }
    }

    public class GanglieAI : SkillEvent
    {
        public GanglieAI() : base("ganglie")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (!ai.WillShowForMasochism()) return false;

            if (data is DamageStruct damage)
            {
                Player target = damage.From;
                if (target == player || (damage.From == null && !ai.HasSkill("tiandu", player)))
                    return false;
                else if (ai.IsFriend(target))
                {
                    if (target.Hp > 1)
                    {
                        double value = 0;
                        if (ai.HasSkill("jieming", target))
                        {
                            double max = 0;
                            foreach (Player p in ai.GetFriends(player))
                            {
                                int count = p.MaxHp - p.HandcardNum;
                                if (count > max)
                                    max = count;
                            }
                            value += max * 0.6;
                        }

                        if (ai.HasSkill("yiji", target))
                        {
                            value += 1.5;
                        }

                        if (ai.HasSkill("fangzhu", target))
                        {
                            value += 2;
                        }

                        if (ai.HasSkill("fankui", target) && player.HasArmor("SilverLion"))
                        {
                            value += 0.5;
                        }
                        if (ai.HasSkill("wangxi", target))
                        {
                            value += 1.5;
                        }

                        if (value > 3) return true;
                    }

                    return false;
                }
                else if (!ai.IsFriend(target))
                {
                    if (target.Hp > 1)
                    {
                        double value = 0;
                        if (ai.HasSkill("jieming", target))
                        {
                            double max = 0;
                            foreach (Player p in ai.GetFriends(target))
                            {
                                int count = p.MaxHp - p.HandcardNum;
                                if (count > max)
                                    max = count;
                            }
                            value += max * 0.6;
                        }

                        if (ai.HasSkill("yiji", target))
                        {
                            value += 1.5;
                        }

                        if (ai.HasSkill("fangzhu", target))
                        {
                            value += 4;
                        }

                        if (ai.HasSkill("fankui", target) && player.HasWeapon("CrossBow"))
                        {
                            value += 3;
                        }

                        if (ai.HasSkill("ganglie", target))
                        {
                            value += 0.5;
                        }

                        if (value >= 3) return false;
                    }
                }
            }

            return true;
        }

        public override List<int> OnDiscard(TrustedAI ai, Player player, int min, int max, bool option, bool include_equip)
        {
            List<int> result = new List<int>();
            Room room = ai.Room;
            DamageStruct current = (DamageStruct)room.GetTag("CurrentDamageStruct");
            Player xiahou = current.To;
            DamageStruct damage = new DamageStruct
            {
                From = xiahou,
                To = player
            };

            if (ai.DamageEffect(damage, DamageStruct.DamageStep.None) == 0)
                return result;
            else
            {
                bool invoke = false;
                if (ai.HasSkill("jieming", player))
                {
                    int _max = 0;
                    foreach (Player p in ai.GetFriends(player))
                    {
                        int count = p.MaxHp - p.HandcardNum;
                        if (count > _max)
                            _max = count;
                    }
                    if (_max >= 2)
                        invoke = true;
                }

                if (ai.HasSkill("yiji", player))
                    invoke = true;

                if (ai.HasSkill("fangzhu", player))
                    invoke = true;

                if (ai.HasSkill("fankui", player))
                {
                    if (xiahou.HasArmor("SilverLion") && ai.IsFriend(xiahou))
                        invoke = true;
                    else if (xiahou.HasWeapon("CrossBow") && !ai.IsFriend(xiahou))
                        invoke = true;
                }
                if (ai.HasSkill("wangxi", player) && ai.IsFriend(xiahou))
                    invoke = true;

                if (invoke && (player.Hp > 1 || ai.GetCards("Peach", player).Count > 0 || ai.GetCards("Analeptic", player).Count > 0))
                    return result;
            }

            if (player.Hp == 1 && (ai.HasArmorEffect(player, "BreastPlate") || ai.GetCards("Peach", player).Count > 0 && ai.GetCards("Analeptic", player).Count > 0))
                return result;

            if (room.Current == player)
            {
                if (ai.GetCards("Peach", player).Count > 0) return result;
                if (player.Hp - 1 > player.HandcardNum) return result;
            }

            List<int> ids = new List<int>(player.HandCards);
            if (room.Current == player)
                ai.SortByUseValue(ref ids, false);
            else
                ai.SortByKeepValue(ref ids, false);

            result.Add(ids[0]);
            result.Add(ids[1]);

            return result;
        }
    }
    


    public class TuxiAI : SkillEvent
    {
        public TuxiAI() : base("tuxi")
        {
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> target, int min, int max, object data)
        {
            return base.OnPlayerChosen(ai, player, target, min, max, data);
        }
    }

    public class LuoyiAI : SkillEvent
    {
        public LuoyiAI() : base("luoyi")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return base.OnSkillInvoke(ai, player, data);
        }
    }

    public class TianduAI : SkillEvent
    {
        public TianduAI() : base("tiandu")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class YijiAI : SkillEvent
    {
        public YijiAI() : base("yiji")
        {
        }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return base.OnSkillInvoke(ai, player, data);
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            return base.OnResponding(ai, player, pattern, prompt, data);
        }
    }
}
