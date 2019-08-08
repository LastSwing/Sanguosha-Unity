using System.Collections.Generic;
using CommonClass.Game;
using SanguoshaServer.Game;

namespace SanguoshaServer.Extensions
{
    public class TitlesContainer
    {
        public List<Title> Titles => titles;
        readonly List<Title> titles = new List<Title>();

        public TitlesContainer()
        {
            titles = new List<Title>
            {
                new AceAttorney(),
                new MakeAName(),
            };
        }
    }

    //逆转裁判
    public class AceAttorney : Title
    {
        public AceAttorney() : base(11, 1)
        {
            EventList = new List<TriggerEvent> { TriggerEvent.GameFinished };
        }

        public override void OnEvent(TriggerEvent triggerEvent, Room room, Player player, object data)
        {
            if (data is string winners)
            {
                if (winners == ".") return;
                foreach (Player p in room.GetAlivePlayers())
                {
                    int id = p.ClientId;
                    if (id > 0 && p.Name == winners && p.Hp == 1 && !ClientDBOperation.CheckTitle(id, TitleId))
                    {
                        int value = ClientDBOperation.GetTitleMark(id, MarkId);
                        //value++;
                        value += 30;        //for test
                        if (value >= 50)
                        {
                            ClientDBOperation.SetTitle(id, TitleId);
                            Client client = room.Hall.GetClient(id);
                            if (client != null)
                                client.AddProfileTitle(TitleId);
                        }
                        else
                            ClientDBOperation.SetTitleMark(id, MarkId, value);
                    }
                }
            }
        }
    }

    //扬名立万
    public class MakeAName : Title
    {
        public MakeAName() : base(12, 2)
        {
            EventList = new List<TriggerEvent> { TriggerEvent.GameFinished };
        }

        public override void OnEvent(TriggerEvent triggerEvent, Room room, Player player, object data)
        {
            if (data is string winners)
            {
                if (winners == ".") return;
                foreach (Player p in room.Players)
                {
                    int id = p.ClientId;
                    if (id > 0 && winners.Contains(p.Name) && !ClientDBOperation.CheckTitle(id, TitleId))
                    {
                        int value = ClientDBOperation.GetTitleMark(id, MarkId);
                        //value++;
                        value += 50;        //for test
                        if (value >= 100)
                        {
                            ClientDBOperation.SetTitle(id, TitleId);
                            Client client = room.Hall.GetClient(id);
                            if (client != null)
                                client.AddProfileTitle(TitleId);
                        }
                        else
                            ClientDBOperation.SetTitleMark(id, MarkId, value);
                    }
                }
            }
        }
    }
}
