using CommonClass.Game;
using System.Collections.Generic;

namespace SanguoshaServer.Game
{
    public class RoomCard : WrappedCard
    {
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
        }

        public override WrappedCard GetUsedCard() => Engine.CloneCard(this);
    }
}
