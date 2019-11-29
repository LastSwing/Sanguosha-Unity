using CommonClass.Game;
using System.Collections.Generic;
using System.Diagnostics;

namespace SanguoshaServer.Game
{
    public class RoomCard : WrappedCard
    {
        private WrappedCard m_card = null;
        public RoomCard(WrappedCard card)
        {
            Id = card.Id;
            SubCards = new List<int>(card.SubCards);
            Name = card.Name;
            Number = card.Number;
            Suit = card.Suit;
            CanRecast = card.CanRecast;
            Transferable = card.Transferable;
            Skill = card.Skill;
            ShowSkill = card.ShowSkill;
            UserString = card.UserString;
            Flags = new List<string>(card.Flags);
            DistanceLimited = card.DistanceLimited;
            ExtraTarget = card.ExtraTarget;
            Cancelable = card.Cancelable;
            Modified = false;

            m_card = card;
        }
        

        //当卡牌的功能改变时，如红颜或当作延时锦囊使用
        public void TakeOver(WrappedCard card)
        {
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
            Flags = new List<string>(card.Flags);
            DistanceLimited = card.DistanceLimited;
            ExtraTarget = card.ExtraTarget;
            Cancelable = card.Cancelable;

            m_card = card;
        }

        public override void ChangeName(string card_name)
        {
            m_card.ChangeName(card_name);
            Name = card_name;
            Modified = true;
        }

        public override WrappedCard GetRealCard()
        {
            return m_card;
        }

        public override void SetFlags(string flag)
        {
            lock (Flags)
            {
                m_card.SetFlags(flag);
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
        public override void ClearFlags()
        {
            lock (Flags)
            {
                Flags.Clear();
                m_card.ClearFlags();
            }
        }
    }
}
