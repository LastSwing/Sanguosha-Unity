using CommonClass.Game;
using System.Collections.Generic;
using System.Linq;

namespace SanguoshaServer.Game
{
    abstract public class CardPattern
    {
        abstract public bool Match(Player player, Room room, WrappedCard card);
        public virtual bool WillThrow()
        {
            return true;
        }
        public virtual string GetPatternString()
        {
            return string.Empty;
        }
    }

    public class ExpPattern : CardPattern
    {
        public ExpPattern(string exp)
        {
            string pattern = exp;
            if (exp.EndsWith("!"))
                pattern = pattern.Remove(exp.Length - 1);
            this.exp = pattern;
        }

        public override bool Match(Player player, Room room, WrappedCard card)
        {
            foreach (string one_exp in exp.Split('#'))
                if (MatchOne(player, room, card, one_exp))
                    return true;

            return false;
        }
        public override string GetPatternString() 
        {
            return exp;
        }

        public virtual string ReplaceCardType(string card_type)
        {
            WrappedCard card = new WrappedCard(card_type);

            List<string> patterns = new List<string>(), results = new List<string>();
            foreach (string one_exp in exp.Split('#')) {
                if (MatchType(card, one_exp))
                    patterns.Add(one_exp);
            }

            if (patterns.Count == 0) return card_type;

            foreach (string one_exp in patterns) {

                string[] factors = one_exp.Split('|');
                string[] card_types = factors[0].Split(',');

                List<string> or_names = new List<string>();
                foreach (string or_name in card_types) {
                    List<string> names = new List<string>();
                    names.Add(Engine.GetPattern(card_type).GetPatternString());
                    foreach (string _name in or_name.Split('+')) {
                        string name = _name;
                        if (name.StartsWith("^"))
                            name = name.Substring(1);

                        int id;
                        if (int.TryParse(name, out id) || name == "0")
                            names.Add(_name);
                    }
                    or_names.Add(string.Join("+", names));
                }
                factors[0] = string.Join(",", or_names);
                results.Add(string.Join("|", factors));
            }
            return string.Join(("#"), results);
        }

        private string exp;

        #region
        // '|' means 'and', '#' means 'or'.
        // the expression splited by '|' has 3 parts,
        // 1st part means the card name, and ',' means more than one options.
        // 2nd patt means the card suit, and ',' means more than one options.
        // 3rd part means the card number, and ',' means more than one options,
        // the number uses '~' to make a scale for valid expressions
        #endregion

        private bool MatchOne(Player player, Room room, WrappedCard card, string exp)
            {
            string[] factors = exp.Split('|');

            bool checkpoint = false;
            string[] card_types = factors[0].Split(',');
            foreach (string or_name in card_types) {
                checkpoint = false;
                foreach (string _name in or_name.Split('+')) {
                    string name = _name;
                    if (name == ".")
                    {
                        checkpoint = true;
                    }
                    else
                    {
                        bool positive = true;
                        if (name.StartsWith("^"))
                        {
                            positive = false;
                            name = name.Substring(1);
                        }
                       int id;
                        if (Engine.GetFunctionCard(card.Name)?.IsKindOf(name) == true
                            || ("%" + card.Name == name)
                            || (int.TryParse(name, out id) && card.GetEffectiveId() == id))
                            checkpoint = positive;
                        else
                            checkpoint = !positive;
                    }
                    if (!checkpoint) break;
                }
                if (checkpoint) break;
            }
            if (!checkpoint) return false;
            if (factors.Length < 2) return true;

            checkpoint = false;
            string[] card_suits = factors[1].Split(',');
            foreach (string _suit in card_suits) {
                string suit = _suit;
                if (suit == ".")
                {
                    checkpoint = true; break;
                }
                bool positive = true;
                if (suit.StartsWith("^"))
                {
                    positive = false;
                    suit = suit.Substring(1);
                }
                if (WrappedCard.GetSuitString(RoomLogic.GetCardSuit(room, card)) == suit
                    || (WrappedCard.IsBlack(RoomLogic.GetCardSuit(room, card)) && suit == "black")
                    || (WrappedCard.IsRed(RoomLogic.GetCardSuit(room, card)) && suit == "red"))
                    checkpoint = positive;
                else
                    checkpoint = !positive;
                if (checkpoint) break;
            }
            if (!checkpoint) return false;
            if (factors.Length < 3) return true;

            checkpoint = false;
            string[] card_numbers = factors[2].Split(',');
            int cdn = RoomLogic.GetCardNumber(room, card);

            foreach (string number in card_numbers) {
                if (number == ".")
                {
                    checkpoint = true;
                    break;
                }

                int id;
                if (number.Contains('~'))
                {
                    string[] num_params = number.Split('~');
                    int from, to;
                    if (num_params[0].Length == 0)
                        from = 1;
                    else
                        from = int.Parse(num_params[0]);
                    if (num_params.Length == 1 && num_params[1].Length == 0)
                        to = 13;
                    else
                        to = int.Parse(num_params[1]);

                    if (from <= cdn && cdn <= to) checkpoint = true;
                }
                else if (int.TryParse(number,out id) && id == cdn)
                {
                    checkpoint = true;
                }
                else if ((number == "A" && cdn == 1)
                  || (number == "J" && cdn == 11)
                  || (number == "Q" && cdn == 12)
                  || (number == "K" && cdn == 13))
                {
                    checkpoint = true;
                }
                if (checkpoint) break;
            }
            if (!checkpoint) return false;
            if (factors.Length < 4) return true;

            checkpoint = false;
            string place = factors[3];
            if (player == null || place == ".") checkpoint = true;
            if (!checkpoint)
            {
                List<int> ids = card.SubCards;
                if (ids.Count > 0)
                {
                    foreach (int id in ids) {
                        checkpoint = false;
                        WrappedCard sub_card = room.GetCard(id);
                        foreach (string _p in place.Split(',')) {
                            string p = _p;
                            if (p == "equipped" && player.HasEquip(sub_card.Name))
                            {
                                checkpoint = true;
                            }
                            else if (p == "hand" && sub_card.Id >= 0)
                            {
                                foreach (int h_id in player.HandCards) {
                                    if (h_id == id)
                                    {
                                        checkpoint = true;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                if (p.Contains('$'))
                                    p = p.Replace('$', '#');
                                if (p.StartsWith("%"))
                                {
                                    p = p.Substring(1);
                                    foreach (Player pl in room.AlivePlayers) {
                                        if (pl.GetPile(p).Count > 0 && pl.GetPile(p).Contains(id))
                                        {
                                            checkpoint = true;
                                            break;
                                        }
                                    }
                                }
                                else if (player.GetPile(p).Count > 0 && player.GetPile(p).Contains(id))
                                {
                                    checkpoint = true;
                                }
                            }
                            if (checkpoint)
                                break;
                        }
                        if (!checkpoint)
                            break;
                    }
                }
            }
            return checkpoint;
        }

        public bool Match(WrappedCard card)
        {
            foreach (string one_exp in exp.Split('#'))
                if (MatchType(card, one_exp)) return true;

            return false;
        }

        private bool MatchType(WrappedCard card, string exp)
        {
            string[] factors = exp.Split('|');

            bool checkpoint = false;
            string[] card_types = factors[0].Split(',');
            foreach (string or_name in card_types) {
                checkpoint = false;
                foreach (string _name in or_name.Split('+')) {
                    string name = _name;
                    if (name == ".")
                    {
                        checkpoint = true;
                    }
                    else
                    {
                        bool positive = true;
                        if (name.StartsWith("^"))
                        {
                            positive = false;
                            name = name.Substring(1);
                        }
                        int result;
                        if (int.TryParse(name, out result))                                   //if the card match type or name, viewasskill should be actived
                            checkpoint = true;
                        else if (Engine.GetFunctionCard(card.Name)?.IsKindOf(name) == true
                                || ("%" + card.Name == name))
                            checkpoint = positive;
                        else
                            checkpoint = !positive;
                    }
                    if (!checkpoint) break;
                }
                if (checkpoint) break;
            }
            return checkpoint;
        }
    };
}
