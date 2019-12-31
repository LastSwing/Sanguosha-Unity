using System.Collections.Generic;
using CommonClass.Game;
using SanguoshaServer.Game;
using SanguoshaServer.Package;

namespace SanguoshaServer.Extensions
{
    //逆转裁判
    public class AceAttorney : Title
    {
        public AceAttorney(int id) : base(id)
        {
            EventList = new List<TriggerEvent> { TriggerEvent.GameFinished };
            MarkId = 1;
        }

        public override void OnEvent(TriggerEvent triggerEvent, Room room, Player player, object data)
        {
            foreach (Player p in room.GetAlivePlayers())
                if (p.ClientId <= 0) return;

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
                        value += 10;        //for test
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
        public MakeAName(int id) : base(id)
        {
            EventList = new List<TriggerEvent> { TriggerEvent.GameFinished };
            MarkId = 2;
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
                        value += 10;        //for test
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

    //我要打十个
    public class KillThemAll : Title
    {
        public KillThemAll(int id) : base(id)
        {
            EventList = new List<TriggerEvent> { TriggerEvent.BeforeGameOverJudge };
        }

        public override void OnEvent(TriggerEvent triggerEvent, Room room, Player player, object data)
        {
            if (data is DeathStruct death && death.Damage.From != null && death.Damage.From.GetMark("multi_kill_count") >= 6)
            {
                int id = death.Damage.From.ClientId;
                if (id > 0 && !ClientDBOperation.CheckTitle(id, TitleId))
                {
                    ClientDBOperation.SetTitle(id, TitleId);
                    Client client = room.Hall.GetClient(id);
                    if (client != null)
                        client.AddProfileTitle(TitleId);
                }
            }
        }
    }

    //螺旋矛盾
    public class Contradictory : Title
    {
        public Contradictory(int id) : base(id)
        {
            EventList = new List<TriggerEvent> { TriggerEvent.FinishJudge };
        }

        public override void OnEvent(TriggerEvent triggerEvent, Room room, Player player, object data)
        {
            if (data is JudgeStruct judge && judge.Reason == EightDiagram.ClassName && judge.IsBad())
            {
                int id = player.ClientId;
                if (id > 0 && !ClientDBOperation.CheckTitle(id, TitleId))
                {
                    player.AddMark("Contradictory");
                    if (player.GetMark("Contradictory") >= 10)
                    {
                        ClientDBOperation.SetTitle(id, TitleId);
                        Client client = room.Hall.GetClient(id);
                        if (client != null)
                            client.AddProfileTitle(TitleId);
                    }
                }
            }
        }
    }

    //无存在感
    public class Nonexistence : Title
    {
        public Nonexistence(int id) : base(id)
        {
            EventList = new List<TriggerEvent> { TriggerEvent.EventPhaseStart, TriggerEvent.Death };
            MarkId = 3;
        }

        public override void OnEvent(TriggerEvent triggerEvent, Room room, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == Player.PlayerPhase.Start && player.GetMark("Nonexistence") == 0)
            {
                player.SetMark("Nonexistence", 1);
            }
            else if (triggerEvent == TriggerEvent.BeforeGameOverJudge && player.GetMark("Nonexistence") == 0)
            {
                int id = player.ClientId;
                if (id > 0 && !ClientDBOperation.CheckTitle(id, TitleId))
                {
                    int value = ClientDBOperation.GetTitleMark(id, MarkId);
                    value++;
                    if (value >= 30)
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

    //逆取顺守
    public class Coup : Title
    {
        public Coup(int id) : base(id)
        {
            EventList = new List<TriggerEvent> { TriggerEvent.Death, TriggerEvent.GameFinished };
        }

        public override void OnEvent(TriggerEvent triggerEvent, Room room, Player player, object data)
        {
            if (room.Setting.GameMode != "Hegemony") return;

            if (triggerEvent == TriggerEvent.Death && data is DeathStruct death && death.Damage.From != null && player.GetRoleEnum() == Player.PlayerRole.Lord)
            {
                if (player.Kingdom == Engine.GetGeneral(death.Damage.From.ActualGeneral1, room.Setting.GameMode).Kingdom)
                    death.Damage.From.SetMark("Coup", 1);
            }
            else if (triggerEvent == TriggerEvent.GameFinished && data is string winners)
            {
                if (winners == ".") return;
                foreach (Player p in room.Players)
                    if (p.ClientId <= 0) return;

                foreach (Player p in room.GetAlivePlayers())
                {
                    int id = p.ClientId;
                    if (id > 0 && p.Name == winners && p.GetMark("Coup") > 0 && !ClientDBOperation.CheckTitle(id, TitleId))
                    {
                        ClientDBOperation.SetTitle(id, TitleId);
                        Client client = room.Hall.GetClient(id);
                        if (client != null)
                            client.AddProfileTitle(TitleId);
                    }
                }
            }
        }
    }

    public class Rebel : Title
    {
        public Rebel(int id) : base(id)
        {
            EventList = new List<TriggerEvent> { TriggerEvent.Death, TriggerEvent.GameFinished };
            MarkId = 4;
        }

        public override void OnEvent(TriggerEvent triggerEvent, Room room, Player player, object data)
        {
            if (room.Setting.GameMode != "Classic" || room.Players.Count < 8) return;

            if (triggerEvent == TriggerEvent.Death && data is DeathStruct death
                && (player.GetRoleEnum() == Player.PlayerRole.Renegade || player.GetRoleEnum() == Player.PlayerRole.Loyalist))
            {
                if (death.Damage.From == null || death.Damage.From.GetRoleEnum() != Player.PlayerRole.Rebel)
                {
                    room.SetTag("Rebel", false);
                }
            }
            else if (triggerEvent == TriggerEvent.GameFinished && data is string winners)
            {
                if (winners == "." || room.ContainsTag("Rebel")) return;
                foreach (Player p in room.Players)
                    if (p.ClientId <= 0) return;

                foreach (Player p in room.GetAlivePlayers())
                    if (p.GetRoleEnum() == Player.PlayerRole.Lord || p.GetRoleEnum() == Player.PlayerRole.Loyalist || p.GetRoleEnum() == Player.PlayerRole.Renegade)
                        return;

                foreach (Player p in room.Players)
                {
                    int id = p.ClientId;
                    if (id > 0 && winners.Contains(p.Name) && !ClientDBOperation.CheckTitle(id, TitleId))
                    {
                        int value = ClientDBOperation.GetTitleMark(id, MarkId);
                        value++;
                        if (value >= 20)
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

    //伍肆叁贰零
    public class OnlyFive : Title
    {
        public OnlyFive(int id) : base(id)
        {
            EventList = new List<TriggerEvent> { TriggerEvent.GameOverJudge };
            MarkId = 5;
        }

        public override void OnEvent(TriggerEvent triggerEvent, Room room, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.GameOverJudge && data is DeathStruct death && player.ClientId > 0)
            {
                if (player.GetRoleEnum() == Player.PlayerRole.Lord && player.General1 == "sunce" && (!string.IsNullOrEmpty(death.Damage.Reason) || death.Damage.Card != null)
                    && death.Damage.Damage > 1 && player.GetMark("hunzi") == 0 && death.Damage.From != player)
                {
                    int id = player.ClientId;
                    if (!ClientDBOperation.CheckTitle(id, TitleId))
                    {
                        int value = ClientDBOperation.GetTitleMark(id, MarkId);
                        value++;
                        if (value >= 20)
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

    //这把稳了
    public class Victory : Title
    {
        public Victory(int id) : base(id)
        {
            EventList = new List<TriggerEvent> { TriggerEvent.CardUsedAnnounced, TriggerEvent.GameFinished };
        }

        public override void OnEvent(TriggerEvent triggerEvent, Room room, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.CardUsedAnnounced && data is CardUseStruct use && use.Card.Name == FumanCard.ClassName && player.ClientId > 0)
            {
                player.AddMark("fuman_used");
            }
            else if (triggerEvent == TriggerEvent.GameFinished && data is string winner)
            {
                if (winner == ".") return;
                List<string> winners = new List<string>(winner.Split('+'));
                foreach (Player p in room.GetAlivePlayers())
                {
                    int id = p.ClientId;
                    if (id > 0 && winners.Contains(p.Name) && p.General1 == "mazhong" && p.GetMark("fuman_used") >= 44 && !ClientDBOperation.CheckTitle(id, TitleId))
                    {
                        ClientDBOperation.SetTitle(id, TitleId);
                        Client client = room.Hall.GetClient(id);
                        if (client != null)
                            client.AddProfileTitle(TitleId);
                    }
                }
            }
        }
    }

    public class LightningMan : Title
    {
        public LightningMan(int id) : base(id)
        {
            EventList = new List<TriggerEvent> { TriggerEvent.Damaged, TriggerEvent.GameFinished };
        }

        public override void OnEvent(TriggerEvent triggerEvent, Room room, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.Damaged && data is DamageStruct damage && player.Alive && player.ClientId > 0
                && ((damage.Card != null && damage.Card.Name == Lightning.ClassName) || (!string.IsNullOrEmpty(damage.Reason) && (damage.Reason == "leiji" || damage.Reason == "leiji_jx"
                || damage.Reason == LightningSummoner.ClassName))) && !damage.Transfer && !damage.Chain)
            {
                player.AddMark("lightning_damaged", damage.Damage);
            }
            else if (triggerEvent == TriggerEvent.GameFinished)
            {
                foreach (Player p in room.GetAlivePlayers())
                {
                    int id = p.ClientId;
                    if (id > 0 && p.GetMark("lightning_damaged") >= 15 && !ClientDBOperation.CheckTitle(id, TitleId))
                    {
                        ClientDBOperation.SetTitle(id, TitleId);
                        Client client = room.Hall.GetClient(id);
                        if (client != null)
                            client.AddProfileTitle(TitleId);
                    }
                }
            }
        }
    }
}
