using System;
using System.Collections.Generic;
using CommonClass;
using CommonClass.Game;
using SanguoshaServer.Game;
using SanguoshaServer.Package;

namespace SanguoshaServer.AI
{
    public class HegemonySpecialAI : AIPackage
    {
        public HegemonySpecialAI() : base("HegemonySpecial")
        {
            events = new List<SkillEvent>
            {
                new TunchuAI(),
                new ShuliangAI(),
                new DujinAI(),
                new GuishuAI(),
                new YuanyuAI(),
                new WeichengAI(),
                new DaoshuAI(),
            };
            use_cards = new List<UseCard>
            {
                new GuishuCardAI(),
                new DaoshuCardAI(),
            };
        }
    }

    public class TunchuAI : SkillEvent
    {
        public TunchuAI() : base("tunchu") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            Room room = ai.Room;
            if (ai.WillSkipPlayPhase(player))
            {
                List<int> ids = new List<int>(), cards = player.GetCards("h");
                ai.SortByKeepValue(ref cards, false);

                while (cards.Count >= player.Hp)
                {
                    ids.Add(cards[0]);
                    cards.RemoveAt(0);
                }

                if (ids.Count == 0 && cards.Count > 0)
                    return new List<int> { cards[0] };
                else
                    return ids;
            }
            if (ai is SmartAI)
            {
                if (ai.GetOverflow(player) > 0 && !ai.IsSituationClear())
                {
                    List<int> ids = new List<int>();
                    foreach (int id in player.GetCards("h"))
                        if (room.GetCard(id).Name.Contains("Slash"))
                            ids.Add(id);

                    return ids;
                }
            }

            return new List<int>();
        }
    }

    public class ShuliangAI : SkillEvent
    {
        public ShuliangAI() : base("shuliang")
        {
            key = new List<string> { "cardExchange:shuliang" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str)
            {
                Room room = ai.Room;
                string[] strs = str.Split(':');
                if (strs[1] == Name)
                {
                    Player target = room.Current;
                    if (ai is SmartAI && player != ai.Self)
                    {
                        if (ai.GetPlayerTendency(target) == "unknown")
                            ai.UpdatePlayerRelation(player, target, true);
                    }
                    else if (ai is StupidAI _ai)
                    {
                        if (_ai.GetPlayerTendency(target) != "unknown")
                            _ai.UpdatePlayerRelation(player, target, true);
                    }
                }
            }
        }
        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            Room room = ai.Room;
            Player target = room.Current;
            if (ai.IsFriend(target))
                return new List<int> { player.GetPile("commissariat")[0] };

            return new List<int>();
        }
    }

    public class DujinAI : SkillEvent
    {
        public DujinAI() : base("dujin") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return ai.WillShowForAttack();
        }
        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, bool isUse, Player.Place place)
        {
            if (!isUse && place == Player.Place.PlaceEquip)
            {
                return 4;
            }

            if (isUse && card.IsVirtualCard())
            {
                foreach (int id in card.SubCards)
                {
                    if (ai.Room.GetCardPlace(id) == Player.Place.PlaceEquip)
                        return -3;
                }
            }

            return 0;
        }
    }

    public class GuishuAI : SkillEvent
    {
        public GuishuAI() : base("guishu") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            Room room = ai.Room;
            List<int> ids = player.GetCards("h");
            ids.AddRange(player.GetHandPile());
            List<int> spades = new List<int>();
            foreach (int id in ids)
                if (room.GetCard(id).Suit == WrappedCard.CardSuit.Spade)
                    spades.Add(id);

            List<WrappedCard> result = new List<WrappedCard>();
            if (spades.Count > 0)
            {
                ai.SortByUseValue(ref spades, false);
                WrappedCard card = new WrappedCard(GuishuCard.ClassName)
                {
                    Skill = Name,
                    ShowSkill = Name
                };
                card.AddSubCard(spades[0]);

                if (player.GetMark(Name) == 0 || player.GetMark(Name) == 1)
                {
                    card.UserString = BefriendAttacking.ClassName;
                    WrappedCard ba = new WrappedCard(BefriendAttacking.ClassName)
                    {
                        Skill = Name,
                        ShowSkill = Name,
                    };
                    ba.AddSubCard(card);
                    ba = RoomLogic.ParseUseCard(room, ba);
                    ba.UserString = RoomLogic.CardToString(room, card);
                    result.Add(ba);
                }
                else if (spades.Count > 1)
                {
                    card.UserString = KnownBoth.ClassName;
                    result.Add(card);
                }
            }
            return result;
        }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, bool isUse, Player.Place place)
        {
            if (ai.HasSkill(Name, player) && card.Id >= 0 && place == Player.Place.PlaceHand && !isUse && card.Suit == WrappedCard.CardSuit.Spade)
                return 1;

            return 0;
        }
    }

    public class GuishuCardAI : UseCard
    {
        public GuishuCardAI() : base(GuishuCard.ClassName) { }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            if (player.GetMark("guishu") == 2)
            {
                WrappedCard kb = new WrappedCard(KnownBoth.ClassName)
                {
                    Skill = Name,
                    ShowSkill = Name,
                };
                kb.AddSubCard(card);
                kb = RoomLogic.ParseUseCard(ai.Room, kb);

                UseCard e = Engine.GetCardUsage(KnownBoth.ClassName);
                if (e != null)
                {
                    CardUseStruct dummy = new CardUseStruct(null, player, new List<Player>())
                    {
                        IsDummy = true
                    };
                    e.Use(ai, player, ref dummy, kb);
                    if (dummy.Card == kb && dummy.To.Count > 0)
                    {
                        use.Card = card;
                        use.To = dummy.To;
                        return;
                    }
                }

                Room room = ai.Room;
                List<Player> targets = ai.Exclude(room.GetOtherPlayers(player), kb);
                if (targets.Count > 0)
                {
                    use.Card = card;
                    use.To.Add(targets[0]);
                }
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 9;
        }
    }

    public class YuanyuAI : SkillEvent
    {
        public YuanyuAI() : base("yuanyu") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class WeichengAI : SkillEvent
    {
        public WeichengAI() : base("weicheng") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class DaoshuAI : SkillEvent
    {
        public DaoshuAI() : base("daoshu")
        {
        }
        private readonly List<string> suits = new List<string> { "spade", "heart", "club", "diamond" };
        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (ai.WillShowForAttack() && !player.HasFlag(Name))
            {
                WrappedCard card = new WrappedCard(DaoshuCard.ClassName)
                {
                    Skill = Name,
                    ShowSkill = Name
                };

                return new List<WrappedCard> { card };
            }

            return new List<WrappedCard>();
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            Player target = null;
            Room room = ai.Room;
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (p.HasFlag("daoshu_target"))
                {
                    target = p;
                    break;
                }
            }

            List<int> ids = ai.GetKnownCards(target);
            List<string> suits = new List<string>(this.suits);
            if (ids.Count > 0)
            {
                Dictionary<string, int> pairs = new Dictionary<string, int>();
                foreach (string suit in suits)
                    pairs[suit] = 0;
                foreach (int id in ids)
                {
                    string suit = WrappedCard.GetSuitString(Engine.GetRealCard(id).Suit);
                    pairs[suit]++;
                }

                suits.Sort((x, y) => { return pairs[x] > pairs[y] ? -1: 1; });
                return suits[0];
            }
            else
            {
                Shuffle.shuffle(ref suits);
                return suits[0];
            }
        }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            List<int> ids = new List<int>();
            Room room = ai.Room;
            foreach (int id in player.GetCards("h"))
            {
                if (Engine.MatchExpPattern(room, pattern, player, room.GetCard(id)))
                    ids.Add(id);
            }
            ai.SortByUseValue(ref ids, false);
            return new List<int> { ids[0] };
        }
    }

    public class DaoshuCardAI : UseCard
    {
        public DaoshuCardAI() : base(DaoshuCard.ClassName)
        {
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            Room room = ai.Room;
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {
                if (ai is SmartAI && ai.Self != player)
                {
                    if (!player.HasShownOneGeneral())
                    {
                        string role = (Scenario.Hegemony.WillbeRole(room, player) != "careerist" ? player.Kingdom : "careerist");
                        ai.UpdatePlayerIntention(player, role, 100);
                    }
                    foreach (Player p in use.To)
                        ai.UpdatePlayerRelation(player, p, false);
                }
                else if (ai is StupidAI)
                {
                    foreach (Player p in use.To)
                        if (ai.GetPlayerTendency(p) != "unknown")
                            ai.UpdatePlayerRelation(player, p, false);
                }
            }
        }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 5;
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            List<Player> targets = ai.GetEnemies(player);
            ai.SortByDefense(ref targets, false);
            foreach (Player p in targets)
            {
                if (!p.IsKongcheng())
                {
                    use.Card = card;
                    use.To.Add(p);
                    return;
                }
            }
        }
    }
}