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
    public class AnniversaryAI : AIPackage
    {
        public AnniversaryAI() : base("Anniversary")
        {
            events = new List<SkillEvent>
            {
                new GuolunAI(),
                new SongSangAI(),
            };

            use_cards = new List<UseCard>
            {
                new GuolunCardAI(),
            };
        }
    }

    public class GuolunAI : SkillEvent
    {
        public GuolunAI() : base("guolun") { key = new List<string> { "cardExchange" }; }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str && ai.Self != player)
            {
                string[] strs = str.Split(':');
                if (strs[1] == Name)
                {
                    Player target = null;
                    Room room = ai.Room;
                    foreach (Player p in room.GetOtherPlayers(player))
                    {
                        if (p.HasFlag(Name))
                        {
                            target = p;
                            break;
                        }
                    }
                    int id = player.GetMark(Name);
                    if (int.TryParse(strs[2], out int give) && ai.GetPlayerTendency(target) != "unknown" && room.GetCard(id).Number < room.GetCard(give).Number)
                        ai.UpdatePlayerRelation(player, target, true);
                }
            }
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.IsKongcheng() && !player.HasUsed(GuolunCard.ClassName))
            {
                return new List<WrappedCard> { new WrappedCard(GuolunCard.ClassName) { Skill = Name } };
            }

            return new List<WrappedCard>();
        }
        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            int id = player.GetMark(Name);
            List<int> ids = player.GetCards("h");
            ai.SortByUseValue(ref ids, false);
            Player target = null;
            Room room = ai.Room;
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (p.HasFlag(Name))
                {
                    target = p;
                    break;
                }
            }

            if (ai.IsFriend(target))
            {
                KeyValuePair<Player, int> pair = ai.GetCardNeedPlayer(ids, new List<Player> { target });
                if (pair.Key == target && (room.GetCard(pair.Value).Number != room.GetCard(id).Number || ai.HasSkill("zhanji")))
                {
                    return new List<int> { pair.Value };
                }

                double value = ai.GetKeepValue(id, target, Place.PlaceHand);
                foreach (int card in ids)
                {
                    double keep = ai.GetKeepValue(card, target, Place.PlaceHand);
                    if (room.GetCard(card).Number != room.GetCard(id).Number && keep >= value)
                        return new List<int> { card };
                }
                if (ai.HasSkill("zhanji"))
                {
                    foreach (int card in ids)
                    {
                        if (room.GetCard(card).Number != room.GetCard(id).Number)
                            return new List<int> { card };
                    }

                    return new List<int> { ids[0] };
                }
            }
            else
            {
                double value = ai.GetUseValue(id, player, Place.PlaceHand);
                foreach (int card in ids)
                {
                    double keep = ai.GetKeepValue(card, target, Place.PlaceHand);
                    if (room.GetCard(card).Number < room.GetCard(id).Number && keep < value)
                        return new List<int> { card };
                }

                if (ai.HasSkill("zhanji"))
                {
                    foreach (int card in ids)
                    {
                        double keep = ai.GetKeepValue(card, target, Place.PlaceHand);
                        if (room.GetCard(card).Number == room.GetCard(id).Number && keep < value)
                            return new List<int> { card };
                    }
                }
            }

            return new List<int>();
        }
    }

    public class GuolunCardAI : UseCard
    {
        public GuolunCardAI() : base(GuolunCard.ClassName) { }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.Number[Name] = 2.9;
            List<int> ids = player.GetCards("h");
            if (ids.Count > 0)
            {
                Room room = ai.Room;
                ai.SortByUseValue(ref ids);
                int give = -1;
                foreach (int id in ids)
                {
                    if (ai.GetUseValue(id, player) < 2 && room.GetCard(id).Number < 5)
                    {
                        give = id;
                        break;
                    }
                }

                if (give >= 0)
                {
                    List<Player> enemies = ai.GetEnemies(player);
                    ai.SortByDefense(ref enemies, false);
                    foreach (Player p in enemies)
                    {
                        if (!p.IsKongcheng())
                        {
                            ai.Number[Name] = 9;
                            use.Card = card;
                            use.To = new List<Player> { p };
                            return;
                        }
                    }
                }

                List<Player> friends = ai.FriendNoSelf;
                ai.SortByDefense(ref friends);
                foreach (Player p in friends)
                {
                    if (!p.IsKongcheng())
                    {
                        use.Card = card;
                        use.To = new List<Player> { p };
                        return;
                    }
                }
            }
            foreach (Player p in ai.GetEnemies(player))
            {
                if (!p.IsKongcheng() && ai.GetKnownCards(p).Count < p.HandcardNum)
                {
                    use.Card = card;
                    use.To = new List<Player> { p };
                    return;
                }
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return ai.Number[Name];
        }
    }

    public class SongSangAI : SkillEvent
    {
        public SongSangAI() : base("songsang") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }
}