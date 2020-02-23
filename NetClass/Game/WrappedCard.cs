using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace CommonClass.Game
{
    #region
    //包装卡，仅仅将卡牌信息封装，不具备任何功能
    //实际花色和点数以及功能实现依赖于运行的Room和Name对应的FunctionCard
    //Room在卡牌实际执行时会将其重新封装成正确的属性
    #endregion
    public class WrappedCard
    {
        public enum CardSuit
        {
            Spade, Club, Heart, Diamond, NoSuitBlack, NoSuitRed, NoSuit, SuitToBeDecided = -1
        };
        public enum CardColor
        {
            Red, Black, Colorless
        };

        public string Name { get; protected set; }
        public int Number { get; protected set; } = 0;
        public CardSuit Suit { get; protected set; } = CardSuit.NoSuit;
        public bool ExtraTarget
        {
            get => _extraTarget;
            set
            {
                _extraTarget = value;
                if (Id > -1 && !_extraTarget)
                    Modified = true;
            }
        }
        public bool DistanceLimited
        {
            get => _distanceLimited;
            set
            {
                _distanceLimited = value;
                if (Id > -1 && !_distanceLimited)
                    Modified = true;
            }
        }
        public int Id { set; get; }
        public List<int> SubCards { get; protected set; } = new List<int>();
        public string UserString { get; set; } = string.Empty;
        public string SkillPosition { get; set; } = string.Empty;
        public bool CanRecast { get; set; } = false;
        public bool Transferable { get; set; } = false;
        public bool Mute { get; set; } = false;
        public string Skill { get; set; } = string.Empty;
        public string ShowSkill { get; set; } = string.Empty;
        //public ConcurrentBag<string> Flags { get; set; } = new ConcurrentBag<string>();
        public List<string> Flags { get; set; } = new List<string>();
        public bool Modified { get; set; } = false;
        public bool Cancelable { get; set; } = true;

        private bool _extraTarget = true;
        private bool _distanceLimited = true;

        public string GetSkillName() => !string.IsNullOrEmpty(Skill) && Skill.StartsWith("_") ? Skill.Substring(1) : Skill;

        public void AddSubCard(WrappedCard card)
        {
            foreach (int id in card.SubCards)
                if (!SubCards.Contains(id))
                    SubCards.Add(id);
        }
        public void AddSubCard(int id)
        {
            if (id >= 0 && !SubCards.Contains(id))
                SubCards.Add(id);
        }

        public void AddSubCards(List<WrappedCard> cards)
        {
            foreach (WrappedCard card in cards)
                AddSubCard(card);
        }
        public void AddSubCards(List<int> ids)
        {
            foreach (int id in ids)
                AddSubCard(id);
        }

        public void ClearSubCards()
        {
            SubCards.Clear();
        }

        //just use for Json, do not use for create
        public WrappedCard()
        {
        }
        public WrappedCard(string name)
        {
            Name = name;
            Id = -1;
        }
        public WrappedCard(string name, int id, CardSuit suit, int number, bool can_recast = false, bool transferable = false)
        {
            Name = name;
            Id = id;
            if (id > -1)
                SubCards = new List<int> { id };
            Suit = suit;
            Number = number;
            CanRecast = can_recast;
            Transferable = transferable;
        }

        public override bool Equals(object obj)
        {
            if (obj is WrappedCard other)
            {
                return Name == other.Name && Id == other.Id && Suit == other.Suit && Number == other.Number && Skill == other.Skill
                    && ShowSkill == other.ShowSkill && UserString == other.UserString && DistanceLimited == other.DistanceLimited && ExtraTarget == other.ExtraTarget
                    && CanRecast == other.CanRecast && Transferable == other.Transferable && SubCards.SequenceEqual(other.SubCards) && Cancelable == other.Cancelable;
            }

            return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public virtual WrappedCard GetUsedCard()
        {
            return this;
        }
        public virtual void ChangeName(string card_name)
        {
            Name = card_name;
            Modified = true;
        }
        /*
        //当卡牌的功能改变时，如红颜或当作延时锦囊使用
        public void TakeOver(WrappedCard card)
        {
            if (card == null || card.SubCards.Count != 1)
            {
                System.Diagnostics.Debug.Assert(card.SubCards.Count == 1, card.Id.ToString());
            }
            SubCards = new List<int>(card.SubCards);
            Modified = true;
            Name = card.Name;
            Number = card.Number;
            Suit = card.Suit;
            CanRecast = card.CanRecast;
            Transferable = card.Transferable;
            Skill = card.Skill;
            ShowSkill = card.ShowSkill;
            UserString = card.UserString;
            Flags = card.Flags;
            DistanceLimited = card.DistanceLimited;
            ExtraTarget = card.ExtraTarget;
            Cancelable = card.Cancelable;
        }
        */

        public void SetFlags(string flag)
        {
            lock (Flags)
            {
                string symbol_c = "-";
                if (string.IsNullOrEmpty(flag))
                    return;
                else if (flag == ".")
                    Flags.Clear();
                else if (flag.StartsWith(symbol_c))
                {
                    string copy = flag.Substring(1);
                    Flags.Remove(copy);
                }
                else if (!Flags.Contains(flag))
                    Flags.Add(flag);
            }
        }
        public void ClearFlags()
        {
            lock (Flags)
            {
                Flags.Clear();
            }
        }
        public bool HasFlag(string flag)
        {
            return Flags.Contains(flag);
        }

        public int GetEffectiveId()
        {
            if (SubCards.Count > 0)
                return SubCards[0];
            else
                return Id;
        }

        public static CardColor GetColor(CardSuit suit)
        {
            switch (suit)
            {
                case CardSuit.Spade:
                case CardSuit.Club:
                case CardSuit.NoSuitBlack:
                    return CardColor.Black;
                case CardSuit.Heart:
                case CardSuit.Diamond:
                case CardSuit.NoSuitRed:
                    return CardColor.Red;
                default:
                    return CardColor.Colorless;
            }
        }

        public string SubcardString()
        {
            if (SubCards.Count == 0)
                return ".";

            List<string> str = new List<string>();
            foreach (int subcard in SubCards)
                str.Add(subcard.ToString());

            return string.Join("+", str);
        }

        public static string GetSuitString(CardSuit suit)
        {
            switch (suit)
            {
                case CardSuit.Spade: return "spade";
                case CardSuit.Heart: return "heart";
                case CardSuit.Club: return "club";
                case CardSuit.Diamond: return "diamond";
                case CardSuit.NoSuitBlack: return "no_suit_black";
                case CardSuit.NoSuitRed: return "no_suit_red";
                default: return "no_suit";
            }
        }

        public static string GetSuitIcon(CardSuit suit)
        {
            switch (suit)
            {
                case CardSuit.Spade: return "♠";
                case CardSuit.Heart: return "♥";
                case CardSuit.Club: return "♣";
                case CardSuit.Diamond: return "♦";
                default: return string.Empty;
            }
        }

        public static string GetSuitChar(CardSuit suit)
        {
            switch (suit)
            {
                case CardSuit.Spade: return "spade_char";
                case CardSuit.Heart: return "heart_char";
                case CardSuit.Club: return "club_char";
                case CardSuit.Diamond: return "diamond_char";
                case CardSuit.NoSuitBlack: return "no_suit_black";
                case CardSuit.NoSuitRed: return "no_suit_red";
                default: return "no_suit";
            }
        }

        public static string GetNumberString(int number)
        {
            if (number == 10)
                return "10";
            else
            {
                string number_string = "-A23456789-JQK";
                return number_string.Substring(number, 1);
            }
        }

        public static bool IsRed(CardSuit suit)
        {
            return GetColor(suit) == CardColor.Red;
        }

        public static bool IsBlack(CardSuit suit)
        {
            return GetColor(suit) == CardColor.Black;
        }

        public static CardSuit GetSuit(string suit)
        {
            switch (suit)
            {
                case "spade":
                    return CardSuit.Spade;
                case "club":
                    return CardSuit.Club;
                case "heart":
                    return CardSuit.Heart;
                case "diamond":
                    return CardSuit.Diamond;
                case "no_suit_red":
                    return CardSuit.NoSuitRed;
                case "no_suit_black":
                    return CardSuit.NoSuitBlack;
                default:
                    return CardSuit.NoSuit;
            }
        }

        public static int GetNumber(string number_string)
        {
            int number = 0;
            if (number_string == "A")
                number = 1;
            else if (number_string == "J")
                number = 11;
            else if (number_string == "Q")
                number = 12;
            else if (number_string == "K")
                number = 13;
            else
            {
                if (int.TryParse(number_string, out int result))
                    return result;
                else
                    return 0;
            }

            return number;
        }

        public void SetSuit(CardSuit suit)
        {
            Suit = suit;
        }

        public void SetNumber(int number)
        {
            Number = number;
        }

        public bool IsVirtualCard()
        {
            return Id < 0;
        }
    }
}
