using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using CommonClass;
using CommonClass.Game;
using CommonClassLibrary;
using SanguoshaServer.AI;
using SanguoshaServer.Game;
using SanguoshaServer.Package;
using static CommonClass.Game.Player;
using CommandType = CommonClassLibrary.CommandType;

namespace SanguoshaServer.Scenario
{
    public class Classic : GameScenario
    {
        public Classic()
        {
            mode_name = "Classic";
            rule = new ClassicRule();
        }
        public override TrustedAI GetAI(Room room, Player player)
        {
            return new StupidAI(room, player);
        }
        public override void Assign(Room room)
        {
            //确定身份
            Player lord = room.Players[0];
            lord.Role = "lord";
            room.BroadcastProperty(lord, "Role");

            List<string> roles = new List<string> { "loyalist", "rebel", "renegade" };
            if (room.Players.Count == 5)
                roles.Add("rebel");
            else if (room.Players.Count == 6)
            {
                roles.Add("rebel");
                roles.Add("rebel");
            }
            else if (room.Players.Count == 7)
            {
                roles.Add("loyalist");
                roles.Add("rebel");
                roles.Add("rebel");
            }
            else if (room.Players.Count == 8)
            {
                roles.Add("loyalist");
                roles.Add("rebel");
                roles.Add("rebel");
                roles.Add("rebel");
            }

            //通知各玩家身份
            Shuffle.shuffle(ref roles);
            for (int i = 1; i < room.Players.Count; i++)
            {
                room.Players[i].Role = roles[i - 1];
                room.NotifyProperty(room.GetClient(room.Players[i]), room.Players[i], "Role");
            }
            room.UpdateStateItem();

            Thread.Sleep(2500);

            AssignGeneralsForPlayers(room, out Dictionary<Player, List<string>> options);
            //主公选将
            string lord_general = room.AskForGeneral(lord, new List<string>(options[lord]), string.Empty, true, string.Empty, null, true);
            lord.General1 = lord_general;
            lord.ActualGeneral1 = lord_general;
            lord.Kingdom = Engine.GetGeneral(lord_general, room.Setting.GameMode).Kingdom;
            lord.General1Showed = true;
            room.BroadcastProperty(lord, "General1");
            room.NotifyProperty(room.GetClient(lord), lord, "ActualGeneral1");
            room.BroadcastProperty(lord, "Kingdom");
            room.BroadcastProperty(lord, "General1Showed");

            foreach (string skill in Engine.GetGeneralSkills(lord_general, Name, true))
                room.AddPlayerSkill(lord, skill);

            //技能预亮
            lord.SetSkillsPreshowed("hd");
            room.NotifyPlayerPreshow(lord);
            lord.PlayerGender = Engine.GetGeneral(lord.General1, room.Setting.GameMode).GeneralGender;
            //主公神将选国籍
            string choice = "wei+qun+shu+wu";
            List<string> prompts = new List<string> { "@choose-kingdom" };
            if (lord.Kingdom == "god")
            {
                lord.Kingdom = room.AskForChoice(lord, "Kingdom", choice, prompts);
                room.BroadcastProperty(lord, "Kingdom");
            }

            //其他玩家选将
            List<Client> receivers = new List<Client>();
            List<Player> players = new List<Player>();
            foreach (Player player in options.Keys)
            {
                if (player.Role == "lord") continue;
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
                Client client = room.GetClient(player);
                if (client != null && !receivers.Contains(client))
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
                    List<string> reply = room.GetClient(player).ClientReply;
                    bool success = true;
                    if (reply == null || reply.Count == 0 || string.IsNullOrEmpty(reply[0]))
                        success = false;
                    else
                        generalName = reply[0];

                    if (!success || (!options[player].Contains(Engine.GetMainGeneral(generalName)) && room.GetClient(player).UserRight < 3)
                        || (!options[player].Contains(Engine.GetMainGeneral(generalName)) && room.GetClient(player).UserRight < 3))
                        generalName = options[player][0];
                    
                    player.General1 = generalName;
                    player.ActualGeneral1 = generalName;
                    player.Kingdom = Engine.GetGeneral(generalName, room.Setting.GameMode).Kingdom;
                    player.General1Showed = true;
                }

                room.BroadcastProperty(player, "General1");
                room.NotifyProperty(room.GetClient(player), player, "ActualGeneral1");
                room.BroadcastProperty(player, "Kingdom");
                room.BroadcastProperty(player, "General1Showed");

                player.SetSkillsPreshowed("hd");
                room.NotifyPlayerPreshow(player);
                player.PlayerGender = Engine.GetGeneral(player.General1, room.Setting.GameMode).GeneralGender;
                List<string> names = new List<string> { player.General1 };
                room.SetTag(player.Name, names);
                room.HandleUsedGeneral(player.General1);
            }

            //非主公神将选国籍
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
                Client client = room.GetClient(player);
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
                List<string> clientReply = room.GetClient(player).ClientReply;
                if (clientReply != null && clientReply.Count > 0)
                    answer = clientReply[0];
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
        public override void OnChooseGeneralReply(Room room, Client client)
        {
            Player player = room.GetPlayers(client)[0];
            List<string> options = JsonUntity.Json2List<string>((string)player.GetTag("generals"));
            List<string> reply = client.ClientReply;
            bool success = true;
            string generalName = string.Empty;
            if (reply == null || reply.Count == 0 || string.IsNullOrEmpty(reply[0]))
                success = false;
            else
                generalName = reply[0];

            if (!success || (!options.Contains(Engine.GetMainGeneral(generalName)) && room.GetClient(player).UserRight < 3)
                || (!options.Contains(Engine.GetMainGeneral(generalName)) && room.GetClient(player).UserRight < 3))
            {
                generalName = options[0];
            }

            player.General1 = generalName;
            player.ActualGeneral1 = generalName;
            player.Kingdom = Engine.GetGeneral(generalName, room.Setting.GameMode).Kingdom;
            player.General1Showed = true;
            room.NotifyProperty(room.GetClient(player), player, "General1");
            room.NotifyProperty(room.GetClient(player), player, "Kingdom");
        }

        private void AssignGeneralsForPlayers(Room room, out Dictionary<Player, List<string>> options)
        {
            options = new Dictionary<Player, List<string>>();

            int max_choice = room.Setting.GeneralCount;
            List<string> generals = room.Generals;
            if (generals.Count < max_choice * room.Players.Count)
                max_choice = (generals.Count - 1) / room.Players.Count;

            foreach (Player player in room.Players)
            {
                if (player.Role == "lord")
                {
                    List<string> choices = new List<string>();
                    Shuffle.shuffle(ref generals);
                    foreach (string general_name in generals)
                    {
                        if (Engine.GetGeneral(general_name, room.Setting.GameMode).IsLord())
                            choices.Add(general_name);

                        if (choices.Count >= 3) break;
                    }
                    generals.RemoveAll(t => choices.Contains(t));

                    for (int i = 3; i < max_choice + 1; i++)
                    {
                        Shuffle.shuffle(ref generals);
                        choices.Add(generals[0]);
                        generals.RemoveAt(0);
                    }
                    options.Add(player, choices);
                    break;
                }
            }

            foreach (Player player in room.Players)
            {
                if (player.Role == "lord") continue;
                List<string> choices = new List<string>();
                for (int i = 0; i < max_choice; i++)
                {
                    Shuffle.shuffle(ref generals);
                    choices.Add(generals[0]);
                    generals.RemoveAt(0);
                }
                options.Add(player, choices);
            }
        }

        public override void PrepareForStart(Room room, ref List<Player> room_players, ref List<int> game_cards, ref List<int> m_drawPile)
        {
            game_cards = Engine.GetGameCards(room.Setting.CardPackage);
            m_drawPile = game_cards;

            Shuffle.shuffle(ref m_drawPile);
            AdjustSeats(room, ref room_players);
        }
        private void AdjustSeats(Room room, ref List<Player> room_players)
        {
            List<Client> clients = room.Clients;
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
            return false;
        }

        public override bool WillBeFriendWith(Room room, Player player, Player other, string show_skill = null)
        {
            return false;
        }

        public override void PrepareForPlayers(Room room)
        {
            //非主公玩家添加技能，血量
            foreach (Player player in room.Players)
            {
                string general1_name = player.ActualGeneral1;
                if (player.Role != "lord")
                {
                    foreach (string skill in Engine.GetGeneralSkills(general1_name, Name, true))
                        if (!skill.StartsWith("$"))
                            room.AddPlayerSkill(player, skill);

                    //技能预亮
                    player.SetSkillsPreshowed("hd");
                    room.NotifyPlayerPreshow(player);
                }

                int max_hp = Engine.GetGeneral(general1_name, room.Setting.GameMode).DoubleMaxHp + (player.Role == "lord" ? 1 : 0);
                player.MaxHp = max_hp;
                player.Hp = player.MaxHp;

                room.BroadcastProperty(player, "MaxHp");
                room.BroadcastProperty(player, "Hp");
            }
        }
    }
 

    public class ClassicRule : GameRule
    {
        public ClassicRule() : base("classic_rule")
        {
        }
        protected override void OnGameStart(Room room, ref object data)
        {

            foreach (Player player in room.Players)
            {
                foreach (string skill_name in player.GetSkills())
                {
                    Skill skill = Engine.GetSkill(skill_name);
                    if (skill.SkillFrequency == Frequency.Limited && !string.IsNullOrEmpty(skill.LimitMark))
                        room.SetPlayerMark(player, skill.LimitMark, 1);
                }
            }
            room.SetTag("FirstRound", true);
            foreach (Player p in room.Players)
                room.DrawCards(p, 4, "gamerule");

            if (room.Setting.LuckCard)
                room.AskForLuckCard(3);

            //游戏开始动画
            room.DoBroadcastNotify(CommandType.S_COMMAND_GAME_START, new List<string>());
            Thread.Sleep(2000);
        }

        public override string GetWinner(Room room)
        {
            List<string> winners = new List<string>();
            List<Player> players = room.GetAlivePlayers();
            bool lord_dead = false;
            foreach (Player p in room.Players)
            {
                if (p.Role == "lord" && !p.Alive)
                {
                    lord_dead = true;
                    break;
                }
            }
            if (lord_dead)
            {
                if (players.Count == 1 && players[0].Role == "renegade")
                    winners.Add(players[0].Name);
                else
                {
                    foreach (Player p in room.Players)
                        if (p.Role == "rebel")
                            winners.Add(p.Name);
                }
            }
            else
            {
                bool check = true;
                foreach (Player p in players)
                {
                    if (p.Role == "rebel" || p.Role == "renegade")
                    {
                        check = false;
                        break;
                    }
                }
                if (check)
                {
                    foreach (Player p in room.Players)
                        if (p.Role == "lord" || p.Role == "loyalist")
                            winners.Add(p.Name);
                }
            }

            return string.Join("+", winners);
        }

        protected override void AddRuleSkill()
        {
        }

        public override void CheckBigKingdoms(Room room)
        {
        }

        protected override void OnDeath(Room room, Player player, ref object data)
        {
        }

        protected override void RewardAndPunish(Room room, Player killer, Player victim)
        {
            if (!killer.Alive) return;

            if (victim.Role == "rebel")
                room.DrawCards(killer, 3, "gamerule");
            if (victim.Role == "loyalist" && killer.Role == "lord")
                room.ThrowAllHandCardsAndEquips(killer);
        }
    }
}
