using System;
using System.Collections.Generic;
using System.Threading;
using CommonClass;
using CommonClass.Game;
using CommonClassLibrary;
using SanguoshaServer.AI;
using SanguoshaServer.Game;
using static CommonClass.Game.Player;
using static SanguoshaServer.Game.Skill;
using CommandType = CommonClassLibrary.CommandType;

namespace SanguoshaServer.Scenario
{
    public class Doudizhu : GameScenario
    {
        public Doudizhu()
        {
            mode_name = "Doudizhu";
            rule = new DoudizhuRule();
        }

        public override bool IsFull(Room room)
        {
            return room.Clients.Count >= 3;
        }
        public override TrustedAI GetAI(Room room, Player player)
        {
            return new TrustedAI(room, player);
        }
        public override void Assign(Room room)
        {
            Thread.Sleep(1000);

            //分配武将
            AssignGeneralsForPlayers(room, out Dictionary<Player, List<string>> options);
            //武将预览
            foreach (Player p in room.Players)
            {
                List<string> gongxinArgs = new List<string> { string.Empty, JsonUntity.Object2Json(options[p]), string.Empty, string.Empty, "false" };
                room.DoNotify(room.GetClient(p), CommandType.S_COMMAND_VIEW_GENERALS, gongxinArgs);
            }

            List<Interactivity> receivers = new List<Interactivity>();
            List<Player> lords = new List<Player>();
            foreach (Client client in room.Clients)
            {
                List<string> args = new List<string> { string.Empty, "userdefine:getlandlord", null };
                Interactivity inter = room.GetInteractivity(client.UserID);
                if (inter != null)
                {
                    inter.CommandArgs = args;
                    receivers.Add(inter);
                }
            }
            room.DoBroadcastRequest(receivers, CommandType.S_COMMAND_INVOKE_SKILL);
            room.DoBroadcastNotify(CommandType.S_COMMAND_UNKNOWN, new List<string> { false.ToString() });

            foreach (Player player in room.Players)
            {
                Interactivity client = room.GetInteractivity(player);
                if (client != null && client.IsClientResponseReady && receivers.Contains(client))
                {
                    List<string> invoke = client.ClientReply;
                    if (invoke != null && invoke.Count > 0 && bool.TryParse(invoke[0], out bool success) && success)
                    {
                        lords.Add(player);
                    }
                }
            }

            bool get = true;
            if (lords.Count == 0)
            {
                get = false;
                lords = new List<Player>(room.Players);
            }
            Player lord = null;
            Shuffle.shuffle(ref lords);
            lord = lords[0];

            if (get)
            {
                LogMessage log = new LogMessage
                {
                    Type = "#get_landlord",
                    From = lord.Name
                };
                room.SendLog(log);
            }
            else
            {
                LogMessage log = new LogMessage
                {
                    Type = "#distribute_landlord",
                    From = lord.Name
                };
                room.SendLog(log);
            }

            foreach (Player p in room.Players)
            {
                if (p == lord)
                {
                    p.Camp = Game3v3Camp.S_CAMP_COOL;
                    p.Role = "lord";
                    room.BroadcastProperty(p, "Camp");
                }
                else
                {
                    p.Role = "rebel";
                }
                room.BroadcastProperty(p, "Role");
            }

            //地主增加2框
            List<string> generals = (List<string>)room.GetTag(Name);
            for (int i = 0; i < 2; i++)
            {
                Shuffle.shuffle(ref generals);
                options[lord].Add(generals[0]);
                generals.RemoveAt(0);
            }

            //选将      
            receivers.Clear();
            List<Player> players = new List<Player>();
            foreach (Player player in options.Keys)
            {
                player.SetTag("generals", JsonUntity.Object2Json(options[player]));
                List<string> args = new List<string>
                {
                    player.Name,
                    string.Empty,
                    JsonUntity.Object2Json(options[player]),
                    true.ToString(),
                    true.ToString(),
                    false.ToString()
                };
                Interactivity client = room.GetInteractivity(player);
                if (client != null )
                {
                    client.CommandArgs = args;
                    receivers.Add(client);
                }
                players.Add(player);
            }

            Countdown countdown = new Countdown
            {
                Max = room.Setting.GetCommandTimeout(CommandType.S_COMMAND_CHOOSE_GENERAL, ProcessInstanceType.S_CLIENT_INSTANCE),
                Type = Countdown.CountdownType.S_COUNTDOWN_USE_SPECIFIED
            };
            room.NotifyMoveFocus(players, countdown);
            room.DoBroadcastRequest(receivers, CommandType.S_COMMAND_CHOOSE_GENERAL);
            room.DoBroadcastNotify(CommandType.S_COMMAND_UNKNOWN, new List<string> { false.ToString() });

            //给AI和超时的玩家自动选择武将
            foreach (Player player in options.Keys)
            {
                player.RemoveTag("generals");
                if (string.IsNullOrEmpty(player.General1))
                {
                    string generalName = string.Empty;
                    Interactivity client = room.GetInteractivity(player);
                    List<string> reply = client?.ClientReply;
                    bool success = true;
                    if (reply == null || reply.Count == 0 || string.IsNullOrEmpty(reply[0]))
                        success = false;
                    else
                        generalName = reply[0];

                    if (!success || (!options[player].Contains(generalName) && room.GetClient(player).UserRight < 3))
                    {
                        TrustedAI ai = room.GetAI(player);
                        if (ai != null && ai is StupidAI)
                        {
                            generalName = GeneralSelector.GetGeneral(room, options[player], player.GetRoleEnum(), player);
                        }
                        else
                            generalName = options[player][0];
                    }
                    
                    player.General1 = generalName;
                    player.ActualGeneral1 = generalName;
                    player.Kingdom = Engine.GetGeneral(generalName, room.Setting.GameMode).Kingdom;
                    player.General1Showed = true;
                }

                room.BroadcastProperty(player, "General1");
                room.NotifyProperty(room.GetClient(player), player, "ActualGeneral1");
                room.BroadcastProperty(player, "Kingdom");
                room.BroadcastProperty(player, "General1Showed");
                player.PlayerGender = Engine.GetGeneral(player.General1, room.Setting.GameMode).GeneralGender;
                room.BroadcastProperty(player, "PlayerGender");

                player.SetSkillsPreshowed("hd");
                room.NotifyPlayerPreshow(player);
                List<string> names = new List<string> { player.General1 };
                room.SetTag(player.Name, names);
                room.HandleUsedGeneral(player.General1);
            }

            //神将选国籍
            string choice = "wei+qun+shu+wu";
            List<string> prompts = new List<string> { "@choose-kingdom" };

            receivers.Clear();
            players.Clear();
            foreach (Player player in room.Players)
            {
                if (player.Kingdom != "god") continue;
                List<string> args = new List<string>
                {
                    player.Name,
                    "Kingdom",
                    choice,
                    JsonUntity.Object2Json(prompts)
                };
                Interactivity client = room.GetInteractivity(player);
                if (client != null && !receivers.Contains(client))
                {
                    client.CommandArgs = args;
                    receivers.Add(client);
                }
                players.Add(player);
            }
            
            countdown = new Countdown
            {
                Max = room.Setting.GetCommandTimeout(CommandType.S_COMMAND_MULTIPLE_CHOICE, ProcessInstanceType.S_CLIENT_INSTANCE),
                Type = Countdown.CountdownType.S_COUNTDOWN_USE_SPECIFIED
            };
            room.NotifyMoveFocus(players, countdown);
            room.DoBroadcastRequest(receivers, CommandType.S_COMMAND_MULTIPLE_CHOICE);
            room.DoBroadcastNotify(CommandType.S_COMMAND_UNKNOWN, new List<string> { false.ToString() });

            foreach (Player player in players)
            {
                string answer = string.Empty;
                Interactivity interactivity = room.GetInteractivity(player);
                if (interactivity != null)
                {
                    List<string> clientReply = interactivity.ClientReply;
                    if (clientReply != null && clientReply.Count > 0)
                        answer = clientReply[0];
                }

                List<string> choices = new List<string>(choice.Split('+'));
                if (string.IsNullOrEmpty(answer) || !choices.Contains(answer))
                {
                    Shuffle.shuffle(ref choices);
                    answer = choices[0];
                }
                player.Kingdom = answer;
                room.BroadcastProperty(player, "Kingdom");
            }
        }

        public override void OnChooseGeneralReply(Room room, Interactivity client)
        {
            Player player = room.GetPlayers(client.ClientId)[0];
            List<string> options = JsonUntity.Json2List<string>((string)player.GetTag("generals"));
            List<string> reply = client.ClientReply;
            bool success = true;
            string generalName = string.Empty;
            if (reply == null || reply.Count == 0 || string.IsNullOrEmpty(reply[0]))
                success = false;
            else
                generalName = reply[0];

            General general = Engine.GetGeneral(generalName, "Doudizhu");
            if (!success || general == null || general.Hidden || !room.Setting.GeneralPackage.Contains(general.Package))
            {
                generalName = options[0];
            }

            player.General1 = generalName;
            player.ActualGeneral1 = generalName;
            player.Kingdom = Engine.GetGeneral(generalName, room.Setting.GameMode).Kingdom;
            player.General1Showed = true;
            room.NotifyProperty(room.GetClient(player), player, "General1");
            room.NotifyProperty(room.GetClient(player), player, "Kingdom");

            if (player.GetRoleEnum() == PlayerRole.Rebel)
            {
                foreach (Player p in room.Players)
                {
                    if (p != player && p.GetRoleEnum() == player.GetRoleEnum())
                    {
                        room.NotifyProperty(room.GetClient(p), player, "General1");
                        room.NotifyProperty(room.GetClient(p), player, "Kingdom");
                    }
                }
            }
        }

        private void AssignGeneralsForPlayers(Room room, out Dictionary<Player, List<string>> options)
        {
            options = new Dictionary<Player, List<string>>();
            int max_choice = 5;
            Player lord = null;
            Client lord_client = null;
            foreach (Player player in room.Players)
            {
                if (player.GetRoleEnum() == PlayerRole.Lord)
                {
                    lord = player;
                    lord_client = room.GetClient(player);
                    break;
                }
            }
            List<string> generals = new List<string>(room.Generals);

            foreach (Player player in room.Players)
            {
                int max = max_choice;
                List<string> choices = new List<string>();
                
                for (int i = 0; i < max; i++)
                {
                    Shuffle.shuffle(ref generals);
                    choices.Add(generals[0]);
                    generals.RemoveAt(0);
                }
                options.Add(player, choices);
            }

            foreach (Player player in options.Keys)
            {
                List<string> all = new List<string>(options[player]);
                foreach (string general in all)
                {
                    foreach (General g in Engine.GetConverPairs(general))
                        if (room.Setting.GeneralPackage.Contains(g.Package))
                            options[player].Add(g.Name);
                }
            }
            room.SetTag(Name, generals);
        }

        public override void PrepareForStart(Room room, ref List<Player> room_players, ref List<int> game_cards, ref List<int> m_drawPile)
        {
            game_cards = Engine.GetGameCards(room.Setting.CardPackage);
            m_drawPile = game_cards;
            room.Skills.Add("landlord");
            room.Skills.Add("landlord-tar");

            Shuffle.shuffle(ref m_drawPile);
            AdjustSeats(room, ref room_players);
        }
        private void AdjustSeats(Room room, ref List<Player> room_players)
        {
            //为所有玩家分配座次
            List<Client> clients = new List<Client>(room.Clients);
            Shuffle.shuffle(ref clients);

            for (int i = 0; i < clients.Count; i++)
            {
                Client client = clients[i];
                if (client.Status != Client.GameStatus.bot)
                    client.Status = Client.GameStatus.online;
                Player player = room_players[i];
                player.SceenName = client.Profile.NickName;
                player.Status = client.Status.ToString();
                if (i == clients.Count - 1)
                    player.Next = room_players[0].Name;
                else
                    player.Next = room_players[i + 1].Name;

                player.ClientId = client.UserID;
            }
        }


        public override bool IsFriendWith(Room room, Player player, Player other)
        {
            return player.GetRoleEnum() == other.GetRoleEnum();
        }

        public override bool WillBeFriendWith(Room room, Player player, Player other, string show_skill = null)
        {
            return player.GetRoleEnum() == other.GetRoleEnum();
        }

        public override void PrepareForPlayers(Room room)
        {
            //非主公玩家添加技能，血量
            foreach (Player player in room.Players)
            {
                string general1_name = player.ActualGeneral1;
                foreach (string skill in Engine.GetGeneralSkills(general1_name, Name, true))
                {
                    Skill s = Engine.GetSkill(skill);
                    if (s != null && !s.LordSkill)
                    {
                        room.AddPlayerSkill(player, skill);
                        if (s.SkillFrequency == Frequency.Limited && !string.IsNullOrEmpty(s.LimitMark))
                            room.SetPlayerMark(player, s.LimitMark, 1);
                    }
                }
                if (player.GetRoleEnum() == PlayerRole.Lord) room.AddPlayerSkill(player, "landlord");

                room.SendPlayerSkillsToOthers(player, true);

                //技能预亮
                player.SetSkillsPreshowed("hd");
                room.NotifyPlayerPreshow(player);

                General g = Engine.GetGeneral(general1_name, room.Setting.GameMode);
                int max_hp = g.DoubleMaxHp + (player.GetRoleEnum() == PlayerRole.Lord ? 1 : 0);
                player.MaxHp = max_hp;
                player.Hp = Math.Max(1, player.MaxHp + g.Head_max_hp_adjusted_value);

                room.BroadcastProperty(player, "MaxHp");
                room.BroadcastProperty(player, "Hp");
            }
        }

        public override string GetPreWinner(Room room, Client client)
        {
            Player surrender = room.GetPlayers(client.UserID)[0];
            List<string> winners = new List<string>();
            List<Player> players = room.GetAlivePlayers();
            bool lord_dead = false;
            foreach (Player p in room.Players)
            {
                if (p.GetRoleEnum() == PlayerRole.Lord && (!p.Alive || surrender == p))
                {
                    players.Remove(p);
                    lord_dead = true;
                    break;
                }
            }
            if (lord_dead)
            {
                foreach (Player p in room.Players)
                    if (p.GetRoleEnum() == PlayerRole.Rebel)
                        winners.Add(p.Name);
            }
            else
            {
                bool check = true;
                foreach (Player p in players)
                {
                    if (p == surrender) continue;
                    if (p.GetRoleEnum() == PlayerRole.Rebel)
                    {
                        check = false;
                        break;
                    }
                }
                if (check)
                {
                    foreach (Player p in room.Players)
                        if (p.GetRoleEnum() == PlayerRole.Lord)
                            winners.Add(p.Name);
                }
            }

            return string.Join("+", winners);
        }

        public override List<Interactivity> CheckSurrendAvailable(Room room)
        {
            List<Interactivity> clients = new List<Interactivity>();
            List<Player> rebel = new List<Player>();
            Player lord = null;
            foreach (Player p in room.GetAlivePlayers())
            {
                if (p.GetRoleEnum() == PlayerRole.Rebel)
                    rebel.Add(p);
                else
                    lord = p;
            }
            if (lord != null) clients.Add(room.GetInteractivity(lord.ClientId));
            if (rebel.Count == 1 && rebel[0].ClientId > 0)
            {
                clients.Add(room.GetInteractivity(rebel[0].ClientId));
            }

            return clients;
        }

        public override void SeatReAdjust(Room room, ref List<Player> players)
        {
            Player lord = null;
            foreach (Player p in players)
            {
                if (p.GetRoleEnum() == PlayerRole.Lord)
                {
                    lord = p;
                    break;
                }
            }
            players.Remove(lord);
            players.Sort((x, y) => { return x.Name == lord.Next ? -1 : 1; });
            players.Insert(0, lord);
        }
    }

    public class DoudizhuRule : GameRule
    {
        public DoudizhuRule() : base("doudizhu_rule")
        {
        }
        protected override void OnGameStart(Room room, ref object data)
        {
            foreach (Player player in room.Players)
            {
                List<string> skills = new List<string>(player.HeadSkills.Keys);
                foreach (string skill_name in skills)
                {
                    Skill skill = Engine.GetSkill(skill_name);
                    if (skill.SkillFrequency == Frequency.Limited && !string.IsNullOrEmpty(skill.LimitMark))
                        room.SetPlayerMark(player, skill.LimitMark, 1);
                    else if (skill.Turn)
                        room.SetTurnSkillState(player, skill_name, false, "head");
                }

                List<string> d_skills = new List<string>(player.DeputySkills.Keys);
                foreach (string skill_name in d_skills)
                {
                    Skill skill = Engine.GetSkill(skill_name);
                    if (skill.SkillFrequency == Frequency.Limited && !string.IsNullOrEmpty(skill.LimitMark))
                        room.SetPlayerMark(player, skill.LimitMark, 1);
                    else if (skill.Turn)
                        room.SetTurnSkillState(player, skill_name, false, "deputy");
                }
            }
            foreach (Player p in room.Players)
                room.DrawCards(p, 4, "gamerule");

            room.AskForLuckCard(3);

            //游戏开始动画
            room.DoBroadcastNotify(CommandType.S_COMMAND_GAME_START, new List<string>());
            Thread.Sleep(2000);

            SurrenderAfterStart(room);
        }

        private async void SurrenderAfterStart(Room room)
        {
            int time = 300000;
            await System.Threading.Tasks.Task.Delay(time);

            if (!room.Finished && room.Surrender.Count == 0)
                room.CheckSurrend();
        }

        public override string GetWinner(Room room)
        {
            if (!string.IsNullOrEmpty(room.PreWinner)) return room.PreWinner;

            List<string> winners = new List<string>();
            List<Player> players = room.GetAlivePlayers();
            bool lord_dead = false;
            foreach (Player p in room.Players)
            {
                if (p.GetRoleEnum() == PlayerRole.Lord && !p.Alive)
                {
                    lord_dead = true;
                    break;
                }
            }
            if (lord_dead)
            {
                if (players.Count == 1 && players[0].GetRoleEnum() == PlayerRole.Renegade)
                    winners.Add(players[0].Name);
                else
                {
                    foreach (Player p in room.Players)
                        if (p.GetRoleEnum() == PlayerRole.Rebel)
                            winners.Add(p.Name);
                }
            }
            else
            {
                bool check = true;
                foreach (Player p in players)
                {
                    if (p.GetRoleEnum() == PlayerRole.Rebel)
                    {
                        check = false;
                        break;
                    }
                }
                if (check)
                {
                    foreach (Player p in room.Players)
                        if (p.GetRoleEnum() == PlayerRole.Lord)
                            winners.Add(p.Name);
                }
            }

            return string.Join("+", winners);
        }
        
        public override void CheckBigKingdoms(Room room)
        {
        }
        protected override void OnBuryVictim(Room room, Player player, ref object data)
        {
            DeathStruct death = (DeathStruct)data;
            room.BuryPlayer(player);
            if (player.GetRoleEnum() == PlayerRole.Rebel)
            {
                foreach (Player p in room.Players)
                {
                    if (p != player && p.Alive && p.GetRoleEnum() == PlayerRole.Rebel)
                    {
                        room.DrawCards(p, 2, "gamerule");
                        break;
                    }
                }
            }
        }
        protected override void OnDeath(Room room, Player player, ref object data)
        {
        }
    }

}

namespace SanguoshaServer.Package
{
    public class DoudizhuGeneral : GeneralPackage
    {
        public DoudizhuGeneral() : base("DoudizhuGeneral")
        {
            skills = new List<Skill>
            {
                new LandLord(),
                new LandLordTar(),
            };
        }
    }
    public class LandLord : TriggerSkill
    {
        public LandLord() : base("landlord")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart };
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (room.Setting.GameMode == "Doudizhu" && player.Alive && player.GetRoleEnum() == PlayerRole.Lord
                && (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Start && player.JudgingArea.Count > 0 && player.GetCardCount(true) > 1
                || player.Phase == PlayerPhase.RoundStart))
                    return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (player.Phase == PlayerPhase.RoundStart)
            {
                room.SendCompulsoryTriggerLog(player, Name, true);
                room.DrawCards(player, 1, Name);
            }
            else
            {
                if (room.AskForDiscard(player, Name, 2, 2, true, true, "@landlord", true))
                {
                    int id = -1;
                    if (player.JudgingArea.Count == 1)
                        id = player.JudgingArea[0];
                    else
                        id = room.AskForCardChosen(player, player, "j", Name, false, FunctionCard.HandlingMethod.MethodDiscard);

                    room.ThrowCard(id, player);
                }
            }
            return false;
        }
    }

    public class LandLordTar : TargetModSkill
    {
        public LandLordTar() : base("landlord-tar", false)
        {
        }
        public override int GetResidueNum(Room room, Player from, WrappedCard card)
        {
            if (room.Setting.GameMode == "Doudizhu" && from.GetRoleEnum() == PlayerRole.Lord)
                return 1;
            else
                return 0;
        }
    }
}