using System;
using System.Collections.Generic;
using System.Diagnostics;
using CommonClass.Game;
using SanguoshaServer.Game;
using SanguoshaServer.Package;
using static CommonClass.Game.Player;
using static SanguoshaServer.Package.FunctionCard;

namespace SanguoshaServer.AI
{
    public class ClassicYingAI : AIPackage
    {
        public ClassicYingAI() : base("ClassicYing")
        {
            events = new List<SkillEvent>
            {
                new ChenglueAI(),
                new ShicaiJXAI(),
                new TusheAI(),
                new LimuAI(),

                new ZuilunAI(),

                new LiangyinAI(),
                new KongshengAI(),
                new JueyanAI(),
                new HuairouAI(),


            };

            use_cards = new List<UseCard>
            {
                 new ChenglueCardAI(),
                 new LimuCardAI(),
                 new JueyanCardAI(),
                 new HuairouCardAI(),
            };
        }
    }

    public class ChenglueAI : SkillEvent
    {
        public ChenglueAI() : base("chenglue") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.HasUsed(ChenglueCard.ClassName))
                return new List<WrappedCard> { new WrappedCard(ChenglueCard.ClassName) { Skill = Name } };

            return new List<WrappedCard>();
        }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            return ai.AskForDiscard(player.GetCards("h"), Name, max, min, false);
        }
    }

    public class ChenglueCardAI : UseCard
    {
        public ChenglueCardAI() : base(ChenglueCard.ClassName) { }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            use.Card = card;
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 6;
        }
    }

    public class ShicaiJXAI : SkillEvent
    {
        public ShicaiJXAI() : base("shicai_jx") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class TusheAI : SkillEvent
    {
        public TusheAI() : base("tushe")
        { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class LimuAI : SkillEvent
    {
        public LimuAI() : base("limu") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            Room room = ai.Room;
            if (!RoomLogic.PlayerContainsTrick(room, player, Indulgence.ClassName)
                && ((player.HasWeapon(Spear.ClassName) && !RoomLogic.PlayerContainsTrick(room, player, Lightning.ClassName))
                || player.Hp == 1 || (ai.GetOverflow(player) > 0 && player.IsWounded())))
            {
                List<int> ids = new List<int>();
                foreach (int id in player.GetCards("he"))
                {
                    if (room.GetCard(id).Name == Lightning.ClassName) return new List<WrappedCard>();
                    if (room.GetCard(id).Suit == WrappedCard.CardSuit.Diamond)
                        ids.Add(id);
                }
                foreach (int id in player.GetHandPile())
                {
                    if (room.GetCard(id).Name == Lightning.ClassName) return new List<WrappedCard>();
                    if (room.GetCard(id).Suit == WrappedCard.CardSuit.Diamond)
                        ids.Add(id);
                }

                if (ids.Count > 0)
                {
                    List<double> values = ai.SortByKeepValue(ref ids, false);
                    if (values[0] < 0)
                    {
                        WrappedCard lm = new WrappedCard(LimuCard.ClassName);
                        lm.AddSubCard(ids[0]);
                        lm.Skill = Name;
                        return new List<WrappedCard> { lm };
                    }

                    ai.SortByUseValue(ref ids, false);
                    WrappedCard _lm = new WrappedCard(LimuCard.ClassName);
                    _lm.AddSubCard(ids[0]);
                    _lm.Skill = Name;
                    return new List<WrappedCard> { _lm };
                }
            }

            return new List<WrappedCard>();
        }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, bool isUse, Place place)
        {
            if (card.Name == Spear.ClassName)
                return 20;

            return 0;
        }
    }

    public class LimuCardAI : UseCard
    {
        public LimuCardAI() : base(LimuCard.ClassName) { }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            use.Card = card;
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 6;
        }
    }

    public class ZuilunAI : SkillEvent
    {
        public ZuilunAI() : base("zuilun")
        {
            key = new List<string> { "playerChosen" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (data is string choice)
            {
                string[] choices = choice.Split(':');
                if (choices[1] == Name)
                {
                    Room room = ai.Room;
                    Player target = room.FindPlayer(choices[2], true);

                    if (ai.GetPlayerTendency(target) != "unknown" && (!ai.HasSkill("zhaxiang", target) || target.Hp <= 1))
                        ai.UpdatePlayerRelation(player, target, false);
                }
            }
        }
        public override bool OnSkillInvoke(TrustedAI ai, Player zhuge, object data)
        {
            Room room = ai.Room;
            int count = 0;
            if (zhuge.HasFlag("zuilun_damage")) count++;
            if (!zhuge.HasFlag("zuilun_discard")) count++;
            int hand = 1000;
            foreach (Player p in room.GetAlivePlayers())
                if (p.HandcardNum < hand) hand = p.HandcardNum;
            if (zhuge.HandcardNum == hand) count++;

            if (count > 0) return true;
            if (zhuge.Hp > 2)
            {
                foreach (Player p in room.GetOtherPlayers(zhuge))
                {
                    if (ai.IsFriend(p) && ai.HasSkill("zhaxiang", p) && p.Hp > 1) return true;
                    if (ai.IsEnemy(p) && p.Hp == 1 && !ai.HasSkill("buqu|buqu_jx", p)) return true;
                }
            }

            return false;
        }

        public override AskForMoveCardsStruct OnMoveCards(TrustedAI ai, Player player, List<int> ups, List<int> downs, int min, int max)
        {
            AskForMoveCardsStruct move = ai.GuanxingForNext(new List<int>(ups));
            if (move.Bottom.Count == min)
            {
                return move;
            }
            else if (move.Top.Count > 0)
            {
                if (move.Bottom.Count < min)
                {
                    while (move.Bottom.Count < min)
                    {
                        int id = move.Top[move.Top.Count - 1];
                        move.Top.Remove(id);
                        move.Bottom.Add(id);
                    }
                }
                else
                {
                    List<int> down = move.Bottom;
                    ai.SortByKeepValue(ref down, false);
                    while (move.Bottom.Count > min)
                    {
                        int id = down[0];
                        down.Remove(id);
                        move.Bottom.Remove(id);
                        move.Top.Add(id);
                    }
                }
            }
            else
            {
                move.Top.Clear();
                move.Bottom.Clear();
                ai.SortByKeepValue(ref ups);
                for (int i = 0; i < ups.Count; i++)
                {
                    if (i < min)
                        move.Bottom.Add(ups[i]);
                    else
                        move.Top.Add(ups[i]);
                }
            }

            Debug.Assert(move.Bottom.Count == min);
            return move;
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            List<Player> enemies = ai.GetEnemies(player);
            if (enemies.Count > 0)
            {
                ai.SortByDefense(ref enemies);
                foreach (Player p in enemies)
                    if (p.Hp == 1) return new List<Player> { p };

                foreach (Player p in enemies)
                    if (!ai.HasSkill("zhaxiang", p)) return new List<Player> { p };
            }

            return new List<Player> { targets[0] };
        }
    }


    public class LiangyinAI : SkillEvent
    {
        public LiangyinAI() : base("liangyin")
        {
            key = new List<string> { "playerChosen", "cardChosen" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (ai.Self == player) return;
            Room room = ai.Room;
            if (data is string choice && ai.Self != player)
            {
                string[] choices = choice.Split(':');
                if (choice.StartsWith("playerChosen") && choices[1] == Name)
                {
                    Player target = room.FindPlayer(choices[2]);

                    if (ai.GetPlayerTendency(target) != "unknown" && target.HandcardNum < player.HandcardNum)
                        ai.UpdatePlayerRelation(player, target, true);
                }
                else if (choice.StartsWith("cardChosen") && choices[1] == Name)
                {
                    int card_id = int.Parse(choices[2]);
                    Player target = room.FindPlayer(choices[4]);

                    if (room.GetCardPlace(card_id) == Place.PlaceHand && target.HandcardNum > 1)
                    {
                        ai.UpdatePlayerRelation(player, target, ai.HasSkill("kongcheng|kongcheng_jx") && target.HandcardNum == 1 ? true : false);
                    }
                    else if (room.GetCardPlace(card_id) == Place.PlaceEquip)
                    {
                        ai.UpdatePlayerRelation(player, target, ai.GetKeepValue(card_id, target, Place.PlaceEquip) > 0 ? false : true);
                    }
                }
            }
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            bool draw = targets[0].HandcardNum < player.HandcardNum;

            if (draw)
            {
                ai.SortByDefense(ref targets, false);
                foreach (Player p in targets)
                {
                    if (ai.IsFriend(p))
                        return new List<Player> { p };
                }
            }
            else
            {
                List<ScoreStruct> scores = new List<ScoreStruct>();
                foreach (Player p in targets)
                {
                    ScoreStruct score = ai.FindCards2Discard(player, p, Name, "hej", FunctionCard.HandlingMethod.MethodDiscard);
                    score.Players = new List<Player> { p };
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
            }

            return new List<Player>();
        }
    }

    public class KongshengAI : SkillEvent
    {
        public KongshengAI(): base("kongsheng"){}
        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            List<int> result = new List<int>();
            Room room = ai.Room;
            if (pattern.Contains(Name))
            {
                List<int> ids = new List<int>();
                foreach (int id in player.GetPile(Name))
                {
                    WrappedCard card = room.GetCard(id);
                    FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                    if (fcard is EquipCard && fcard.IsAvailable(room, player, card))
                        ids.Add(id);
                }

                if (player.IsWounded())
                    foreach (int id in ids)
                        if (room.GetCard(id).Name == SilverLion.ClassName)
                            return new List<int> { id };

                ai.SortByUseValue(ref ids, false);
                return new List<int> { ids[0] };
            }
            else
            {
                List<int> ids = player.GetCards("he");
                if (ids.Count > 0)
                {
                    List<double> values = ai.SortByKeepValue(ref ids, false);
                    if (values[0] < 0)
                        result.Add(ids[0]);


                    if (ai.WillSkipPlayPhase(player))
                    {
                        foreach (int id in player.GetCards("h"))
                            if (!result.Contains(id) && room.GetCard(id).Name != Nullification.ClassName)
                                result.Add(id);

                        Player zhongyao = RoomLogic.FindPlayerBySkillName(room, "zuoding");
                        if (zhongyao != null && ai.IsFriend(zhongyao))
                        {
                            foreach (int id in player.GetCards("e"))
                                if (!result.Contains(id) && room.GetCard(id).Suit == WrappedCard.CardSuit.Spade)
                                    result.Add(id);
                        }
                    }
                    else
                    {
                        foreach (int id in player.GetCards("h"))
                        {
                            if (!result.Contains(id) && room.GetCard(id).Name == Jink.ClassName)
                                result.Add(id);
                            if (!result.Contains(id) && !player.IsWounded() && room.GetCard(id).Name == Peach.ClassName)
                                result.Add(id);
                        }

                        Player zhongyao = RoomLogic.FindPlayerBySkillName(room, "zuoding");
                        if (zhongyao != null && ai.IsFriend(zhongyao) && player.GetDefensiveHorse() && room.GetCard(player.DefensiveHorse.Key).Suit == WrappedCard.CardSuit.Spade
                            && !result.Contains(player.DefensiveHorse.Key))
                        {
                            result.Add(player.DefensiveHorse.Key);
                        }
                    }
                }
            }

            return result;
        }
    }

    public class JueyanAI : SkillEvent
    {
        public JueyanAI() : base("jueyan") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.HasUsed(JueyanCard.ClassName))
            {
                for (int i = 0; i < 5; i++)
                    if (!player.EquipIsBaned(i))
                        return new List<WrappedCard> { new WrappedCard(JueyanCard.ClassName) { Skill = Name } };
            }

            return new List<WrappedCard>();
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            return ai.Choice[JueyanCard.ClassName];
        }
    }

    public class JueyanCardAI : UseCard
    {
        public JueyanCardAI() : base(JueyanCard.ClassName) { }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.Number[Name] = 12;
            Room room = ai.Room;
            if (!player.EquipIsBaned(1) && ai.GetOverflow(player) == 0)
            {
                if (player.IsWounded() && ai.GetKnownCardsNums(SilverLion.ClassName, "he", player) > 0 && !player.HasArmor(SilverLion.ClassName))
                    return;

                use.Card = card;
                ai.Choice[Name] = "Armor";
                return;
            }

            if (!player.EquipIsBaned(0) && ai.GetKnownCardsNums(Slash.ClassName, "h", player) >= 2)
            {
                List<WrappedCard> cards = ai.GetCards(Slash.ClassName, player);
                int count = 0;
                foreach (Player p in room.GetOtherPlayers(player))
                {
                    if (RoomLogic.DistanceTo(room, player, p) == 1 && ai.IsEnemy(p))
                    {
                        foreach (WrappedCard slash in cards)
                        {
                            if (!ai.IsCancelTarget(slash, p, player) && ai.IsCardEffect(slash, p, player) && RoomLogic.IsProhibited(room, player, p, slash) == null)
                            {
                                count++;
                            }
                        }
                    }
                }
                if (count >= 2)
                {
                    use.Card = card;
                    ai.Choice[Name] = "Weapon";
                    return;
                }
            }

            if (!player.EquipIsBaned(4) && ai.GetKnownCardsNums("Trick", "h", player) >= 2 && !player.HasTreasure(ClassicWoodenOx.ClassName))
            {
                use.Card = card;
                ai.Choice[Name] = "Treasure";
                return;
            }

            if (!player.EquipIsBaned(2) || !player.EquipIsBaned(3))
            {
                if (player.GetMark("poshi") == 0 && player.EquipIsBaned(0) && player.EquipIsBaned(1) && player.EquipIsBaned(4))
                {
                    use.Card = card;
                    ai.Choice[Name] = "Horse";
                    return;
                }
                if (ai.GetKnownCardsNums("Snatch", "h", player) + ai.GetKnownCardsNums("Slash", "h", player) + ai.GetKnownCardsNums(SupplyShortage.ClassName, "h", player) > 2)
                {
                    use.Card = card;
                    ai.Choice[Name] = "Horse";
                    return;
                }
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return ai.Number[Name];
        }
    }

    public class HuairouAI : SkillEvent
    {
        public HuairouAI() : base("huairou") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            Room room = ai.Room; 
            foreach (int id in player.GetCards("h"))
            {
                WrappedCard card = room.GetCard(id);
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                if (fcard is EquipCard equip && player.EquipIsBaned((int)equip.EquipLocation()))
                {
                    WrappedCard hr = new WrappedCard(HuairouCard.ClassName) { Skill = Name };
                    hr.AddSubCard(id);
                    return new List<WrappedCard> { hr };
                }
            }

            foreach (int id in player.GetCards("e"))
            {
                if (ai.GetKeepValue(id, player) < 0)
                {
                    WrappedCard hr = new WrappedCard(HuairouCard.ClassName) { Skill = Name };
                    hr.AddSubCard(id);
                    return new List<WrappedCard> { hr };
                }
            }

            foreach (int id in player.GetCards("h"))
            {
                if (ai.GetUseValue(id, player) < 1)
                {
                    WrappedCard hr = new WrappedCard(HuairouCard.ClassName) { Skill = Name };
                    hr.AddSubCard(id);
                    return new List<WrappedCard> { hr };
                }
            }

            return new List<WrappedCard>();
        }
    }

    public class HuairouCardAI : UseCard
    {
        public HuairouCardAI() : base(HuairouCard.ClassName) { }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            use.Card = card;
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 10;
        }
    }
}