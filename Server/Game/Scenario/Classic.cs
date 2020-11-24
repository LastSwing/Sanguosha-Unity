using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
    public class Classic : GameScenario
    {
        public Classic()
        {
            mode_name = "Classic";
            rule = new ClassicRule();
        }

        public override bool IsFull(Room room)
        {
            return room.Clients.Count >= 4;
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
            List<Player> all = new List<Player>(room.Players);
            all.Remove(lord);

            //点选内奸的玩家
            List<Player> to_choose = new List<Player>();
            foreach (Player p in room.Players)
                if (p.GetRoleEnum() != PlayerRole.Lord && room.GetClient(p).RoleReserved == "renegade")
                    to_choose.Add(p);

            if (to_choose.Count > 0)
            {
                List<string> renegades = roles.FindAll(t => t == "renegade");
                Shuffle.shuffle(ref to_choose);
                for (int i = 0; i < Math.Min(to_choose.Count, renegades.Count); i++)
                {
                    to_choose[i].Role = "renegade";
                    roles.Remove("renegade");
                    all.Remove(to_choose[i]);
                }
            }

            //点选忠臣的玩家
            to_choose.Clear();
            foreach (Player p in room.Players)
                if (p.GetRoleEnum() != PlayerRole.Lord && room.GetClient(p).RoleReserved == "loyalist")
                    to_choose.Add(p);

            if (to_choose.Count > 0)
            {
                List<string> loyalists = roles.FindAll(t => t == "loyalist");
                Shuffle.shuffle(ref to_choose);
                for (int i = 0; i < Math.Min(loyalists.Count, to_choose.Count); i++)
                {
                    to_choose[i].Role = "loyalist";
                    roles.Remove("loyalist");
                    all.Remove(to_choose[i]);
                }
            }
            //点选反贼的玩家
            to_choose.Clear();
            foreach (Player p in room.Players)
                if (p.GetRoleEnum() != PlayerRole.Lord && room.GetClient(p).RoleReserved == "rebel")
                    to_choose.Add(p);

            if (to_choose.Count > 0)
            {
                List<string> rebels = roles.FindAll(t => t == "rebel");
                Shuffle.shuffle(ref to_choose);
                for (int i = 0; i < Math.Min(rebels.Count, to_choose.Count); i++)
                {
                    to_choose[i].Role = "rebel";
                    roles.Remove("rebel");
                    all.Remove(to_choose[i]);
                }
            }

            //为剩余玩家随机身份
            if (roles.Count > 0)
            {
                Shuffle.shuffle(ref roles);
                for (int i = 0; i < all.Count; i++)
                    all[i].Role = roles[i];
            }

            //通知各玩家身份
            foreach (Player p in room.Players)
                room.NotifyProperty(room.GetClient(p), p, "Role");

            room.UpdateStateItem();

            Thread.Sleep(2500);

            AssignGeneralsForPlayers(room, out Dictionary<Player, List<string>> options);
            //主公选将
            string lord_general = room.AskForGeneral(lord, new List<string>(options[lord]), string.Empty, true, "gamerule", null, true);

            LogMessage log = new LogMessage
            {
                Type = "#lord_selected",
                From = lord.Name,
                Arg = lord_general
            };
            room.SendLog(log);

            //通知
            if (reserved.TryGetValue(lord, out List<string> lord_reserved) && lord_reserved.Contains(lord_general))
            {
                LogMessage reserved_log = new LogMessage();
                reserved_log.Type = "#reserved_pick";
                reserved_log.From = lord.Name;
                room.SendLog(reserved_log);
            }

            lord.General1 = lord_general;
            lord.ActualGeneral1 = lord_general;
            General lord_gen = Engine.GetGeneral(lord_general, room.Setting.GameMode);
            lord.PlayerGender = lord_gen.GeneralGender;
            lord.Kingdom = lord_gen.Kingdom;
            lord.General1Showed = true;
            room.BroadcastProperty(lord, "General1");
            room.BroadcastProperty(lord, "PlayerGender");
            room.NotifyProperty(room.GetClient(lord), lord, "ActualGeneral1");
            room.BroadcastProperty(lord, "Kingdom");
            room.BroadcastProperty(lord, "General1Showed");
            room.HandleUsedGeneral(lord.General1);

            foreach (string skill in Engine.GetGeneralSkills(lord_general, Name, true))
            {
                room.AddPlayerSkill(lord, skill);
                Skill s = Engine.GetSkill(skill);
                if (s != null && s.SkillFrequency == Frequency.Limited  && !string.IsNullOrEmpty(s.LimitMark))
                    room.SetPlayerMark(lord, s.LimitMark, 1);
            }
            room.SendPlayerSkillsToOthers(lord, true);

            //技能预亮
            lord.SetSkillsPreshowed("hd");
            room.NotifyPlayerPreshow(lord);
            //主公神将选国籍
            string choice = "wei+qun+shu+wu";
            List<string> prompts = new List<string> { "@choose-kingdom" };
            if (lord.Kingdom == "god")
            {
                lord.Kingdom = room.AskForChoice(lord, "Kingdom", choice, prompts);
                room.BroadcastProperty(lord, "Kingdom");
            }

            Thread.Sleep(1000);

            //其他玩家选将
            List<Interactivity> receivers = new List<Interactivity>();
            List<Player> players = new List<Player>();
            foreach (Player player in options.Keys)
            {
                if (player.GetRoleEnum() == PlayerRole.Lord) continue;
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
                if (player == lord) continue;
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

                //通知
                if (reserved.TryGetValue(player, out List<string> p_reserved) && p_reserved.Contains(player.General1))
                {
                    LogMessage reserved_log = new LogMessage();
                    reserved_log.Type = "#reserved_pick";
                    reserved_log.From = player.Name;
                    room.SendLog(reserved_log);
                }

                if (!options[player].Contains(player.General1))
                {
                    LogMessage cheat_log = new LogMessage();
                    cheat_log.Type = "#cheat_pick";
                    cheat_log.From = player.Name;
                    room.SendLog(cheat_log);
                }
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
                else
                {
                    TrustedAI ai = room.GetAI(player);
                    if (ai != null && ai is StupidAI)
                    {
                        foreach (Player p in room.GetAllPlayers())
                        {
                            if (p.GetRoleEnum() == PlayerRole.Lord)
                            {
                                answer = p.Kingdom;
                                break;
                            }
                        }
                    }
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

            General general = Engine.GetGeneral(generalName, "Classic");
            if (!success || general == null || general.Hidden || !room.Setting.GeneralPackage.Contains(general.Package)
                 || (!options.Contains(generalName) && room.GetClient(client.ClientId).UserRight < 3))
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
            List<string> generals = new List<string>(room.Generals), reserved = new List<string>(), lord_generals = new List<string>();
            if (generals.Count - 1 < max_choice * room.Players.Count)
                max_choice = (generals.Count - 1) / room.Players.Count;

            for (int i = 0; i < room.Clients.Count; i++)
            {
                Client client = room.Clients[i];
                if (client.UserID < 0) continue;
                List<string> reserved_generals = new List<string>(client.GeneralReserved);
                if (reserved_generals == null || reserved_generals.Count == 0) continue;
                reserved.AddRange(reserved_generals);
            }

            List<string> duplicated = reserved.GroupBy(x => x).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
            for (int i = 0; i < room.Clients.Count; i++)
            {
                Client client = room.Clients[i];
                if (client.UserID < 0) continue;
                if (client.GeneralReserved == null || client.GeneralReserved.Count == 0) continue;
                client.GeneralReserved.RemoveAll(t => duplicated.Contains(t));
            }

            //优先给主公找出3张主公武将
            Player lord = null;
            Client lord_client = null;
            foreach (Player player in room.Players)
            {
                if (player.GetRoleEnum() == PlayerRole.Lord)
                {
                    lord = player;
                    break;
                }
            }

            foreach (Client client in room.Clients)
            {
                if (client.UserID == lord.ClientId)
                {
                    lord_client = client;
                    if (client.UserID >= 0 && client.GeneralReserved != null && client.GeneralReserved.Count > 0 && client.GeneralReserved.Count <= 2)
                    {
                        options[lord] = new List<string>();
                        foreach (string general in client.GeneralReserved)
                        {
                            if (generals.Contains(general))
                            {
                                options[lord].Add(general);
                                generals.Remove(general);
                            }
                        }
                        this.reserved[lord] = new List<string>(options[lord]);
                    }

                    client.GeneralReserved = null;
                    break;
                }
            }

            int lord_adjust = options.ContainsKey(lord) ? options[lord].Count : 0;
            //应该为主公配的主公武将数
            int lest = max_choice + 1 - lord_adjust;
            int lord_count = Math.Min(3, lest);

            List<string> lord_choices = new List<string>();
            Shuffle.shuffle(ref generals);
            foreach (string general_name in generals)
            {
                if (lord.ClientId < 0 && !Engine.IsAISelectable(general_name, room.Setting.GameMode)) continue;
                if (Engine.GetGeneral(general_name, room.Setting.GameMode).IsLord())
                    lord_choices.Add(general_name);

                if (lord_choices.Count >= lord_count) break;
            }

            if (lord_choices.Count < lord_count)
            {
                foreach (Client client in room.Clients)
                {
                    if (client.UserID == lord.ClientId || client.UserID < 0 || client.GeneralReserved == null && client.GeneralReserved.Count == 0) continue;
                    List<string> reserver_p = new List<string>(client.GeneralReserved);
                    foreach (string general in reserver_p)
                    {
                        if (lord.ClientId < 0 && !Engine.IsAISelectable(general, room.Setting.GameMode)) continue;
                        if (generals.Contains(general) && Engine.GetGeneral(general, room.Setting.GameMode).IsLord())
                        {
                            client.GeneralReserved.Remove(general);
                            lord_choices.Add(general);
                        }

                        if (lord_choices.Count >= lord_count) break;
                    }

                    if (lord_choices.Count >= lord_count) break;
                }
            }
            generals.RemoveAll(t => lord_choices.Contains(t));

            foreach (Client client in room.Clients)
            {
                if (client == lord_client) continue;
                if (client.UserID >= 0 && client.GeneralReserved != null && client.GeneralReserved.Count > 0 && client.GeneralReserved.Count <= 2)
                {
                    foreach (Player p in room.Players)
                    {
                        if (p.ClientId == client.UserID)
                        {
                            options[p] = new List<string>();
                            foreach (string general in client.GeneralReserved)
                            {
                                if (generals.Contains(general))
                                {
                                    options[p].Add(general);
                                    generals.Remove(general);
                                }
                            }
                            this.reserved[p] = new List<string>(options[p]);

                            break;
                        }
                    }
                }

                client.GeneralReserved = null;
            }
            
            for (int i = lord_choices.Count; i < max_choice + 1 - lord_adjust; i++)
            {
                Shuffle.shuffle(ref generals);
                string choice = string.Empty;
                foreach (string general_name in generals)
                {
                    if (lord.ClientId < 0 && !Engine.IsAISelectable(general_name, room.Setting.GameMode)) continue;
                    choice = general_name;
                    break;
                }
                lord_choices.Add(choice);
                generals.Remove(choice);
            }
            if (options.ContainsKey(lord))
                options[lord].AddRange(lord_choices);
            else
                options.Add(lord, lord_choices);

            foreach (Player player in room.Players)
            {
                if (player.GetRoleEnum() == PlayerRole.Lord) continue;
                List<string> choices = new List<string>();

                int adjust = options.ContainsKey(player) ? options[player].Count : 0;
                for (int i = adjust; i < max_choice; i++)
                {
                    Shuffle.shuffle(ref generals);
                    choices.Add(generals[0]);
                    generals.RemoveAt(0);
                }
                if (options.ContainsKey(player))
                    options[player].AddRange(choices);
                else
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
            //首先从点选主公的玩家中选出1号位
            List<Client> lords = new List<Client>();
            foreach (Client client in room.Clients)
            {
                if (client.RoleReserved == "lord")
                    lords.Add(client);
            }

            //没人点选则首先从未指定身份的玩家中选出
            if (lords.Count == 0)
            {
                foreach (Client client in room.Clients)
                    if (string.IsNullOrEmpty(client.RoleReserved))
                        lords.Add(client);
            }
            //若所有人都有点选非主公身份，则需要随机牺牲一人
            if (lords.Count == 0) lords = new List<Client>(room.Clients);
            Shuffle.shuffle(ref lords);
            Client lord = lords[0];

            //为所有玩家分配座次
            List<Client> clients = new List<Client>(room.Clients);
            clients.Remove(lord);
            Shuffle.shuffle(ref clients);
            clients.Insert(0, lord);

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
            switch (player.GetRoleEnum())
            {
                case PlayerRole.Lord:
                    return other.GetRoleEnum() == PlayerRole.Loyalist;
                case PlayerRole.Loyalist:
                    return other.GetRoleEnum() == PlayerRole.Lord || other.GetRoleEnum() == PlayerRole.Loyalist;
                case PlayerRole.Rebel:
                    return other.GetRoleEnum() == PlayerRole.Rebel;
            }

            return false;
        }

        public override void PrepareForPlayers(Room room)
        {
            //非主公玩家添加技能，血量
            foreach (Player player in room.Players)
            {
                string general1_name = player.ActualGeneral1;
                if (player.GetRoleEnum() != PlayerRole.Lord)
                {
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
                    room.SendPlayerSkillsToOthers(player, true);

                    //技能预亮
                    player.SetSkillsPreshowed("hd");
                    room.NotifyPlayerPreshow(player);
                }

                General g = Engine.GetGeneral(general1_name, room.Setting.GameMode);
                int max_hp = g.DoubleMaxHp
                    + (room.Players.Count > 4 && player.GetRoleEnum() == PlayerRole.Lord ? 1 : 0);
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
                    if (p == surrender) continue;
                    if (p.GetRoleEnum() == PlayerRole.Rebel || p.GetRoleEnum() == PlayerRole.Renegade)
                    {
                        check = false;
                        break;
                    }
                }
                if (check)
                {
                    foreach (Player p in room.Players)
                        if (p.GetRoleEnum() == PlayerRole.Lord || p.GetRoleEnum() == PlayerRole.Loyalist)
                            winners.Add(p.Name);
                }
            }

            return string.Join("+", winners);
        }

        public override List<Interactivity> CheckSurrendAvailable(Room room)
        {
            List<Interactivity> clients = new List<Interactivity>();
            int loyalist = 0;
            List<Player> rebel = new List<Player>(), rena = new List<Player>();
            Player lord = null;
            foreach (Player p in room.GetAlivePlayers())
            {
                switch (p.GetRoleEnum())
                {
                    case PlayerRole.Lord:
                        lord = p;
                        break;
                    case PlayerRole.Loyalist:
                        loyalist++;
                        break;
                    case PlayerRole.Rebel:
                        rebel.Add(p);
                        break;
                    case PlayerRole.Renegade:
                        rena.Add(p);
                        break;
                }
            }

            if (loyalist == 0 && (rebel.Count == 0 || rena.Count == 0) && lord.ClientId > 0)
            {
                clients.Add(room.GetInteractivity(lord.ClientId));
            }
            if (rebel.Count == 1 && rena.Count == 0 && rebel[0].ClientId > 0)
            {
                clients.Add(room.GetInteractivity(rebel[0].ClientId));
            }
            if (rebel.Count == 0 && rena.Count == 1 && rena[0].ClientId > 0)
            {
                clients.Add(room.GetInteractivity(rena[0].ClientId));
            }

            return clients;
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

            if (room.Setting.LuckCard)
                room.AskForLuckCard(3);

            //游戏开始动画
            room.DoBroadcastNotify(CommandType.S_COMMAND_GAME_START, new List<string>());
            Thread.Sleep(2000);
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
                    if (p.GetRoleEnum() == PlayerRole.Rebel || p.GetRoleEnum() == PlayerRole.Renegade)
                    {
                        check = false;
                        break;
                    }
                }
                if (check)
                {
                    foreach (Player p in room.Players)
                        if (p.GetRoleEnum() == PlayerRole.Lord || p.GetRoleEnum() == PlayerRole.Loyalist)
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

            if (room.ContainsTag("SkipNormalDeathProcess") && (bool)room.GetTag("SkipNormalDeathProcess"))
                return;

            Player killer = death.Damage.From;
            if (killer != null)
            {
                killer.SetMark("multi_kill_count", killer.GetMark("multi_kill_count") + 1);
                int kill_count = killer.GetMark("multi_kill_count");
                if (kill_count > 1 && kill_count < 8)
                    room.SetEmotion(killer, string.Format("kill{0}", kill_count));
                else if (kill_count > 7)
                    room.SetEmotion(killer, "zylove");
                RewardAndPunish(room, killer, player);
            }

            if (room.AliveCount() == 2)
            {
                bool check = false;
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (p.GetRoleEnum() == PlayerRole.Renegade)
                    {
                        check = true;
                        break;
                    }
                }
                if (check)
                {
                    room.DoBroadcastNotify(CommandType.S_COMMAND_GAMEMODE_BLOODBATTLE, new List<string>());
                    Thread.Sleep(2500 + room.AliveCount() * 500);
                }
            }
        }
        protected override void OnDeath(Room room, Player player, ref object data)
        {
        }

        protected override void RewardAndPunish(Room room, Player killer, Player victim)
        {
            if (!killer.Alive) return;

            if (victim.GetRoleEnum() == PlayerRole.Rebel)
                room.DrawCards(killer, 3, "gamerule");
            if (victim.GetRoleEnum() == PlayerRole.Loyalist && killer.GetRoleEnum() == PlayerRole.Lord)
                room.ThrowAllHandCardsAndEquips(killer);
        }
    }

    public class GeneralSelector
    {
        public static string GetGeneral(Room room, List<string> choices, PlayerRole role, Player self)
        {
            Debug.Assert(choices.Count > 0);

            if (choices.Count == 1) return choices[0];
            Player lord = null;
            foreach (Player p in room.Players)
            {
                if (p.GetRoleEnum() == PlayerRole.Lord)
                {
                    lord = p;
                    break;
                }
            }

            Dictionary<string, double> points = new Dictionary<string, double>();
            foreach (string general in choices)
            {
                if (!Engine.IsAISelectable(general, "Classic"))
                    points.Add(general, 0);
                else
                {
                    double basic = Engine.GetGeneralValue(general, "Classic");
                    basic += Engine.GetRoleTendency(general, role) / 10;
                    basic += AdjustRolePoints(room, lord, general, role, self);
                    points.Add(general, basic);
                }
            }

            choices.Sort((x, y) => { return points[x] > points[y] ? -1 : 1; });
            if (role == PlayerRole.Lord)
            {
                int index = 0;
                Random rd = new Random();
                index = rd.Next(0, Math.Min(3, choices.Count));
                return choices[index];
            }

            return choices[0];
        }

        public static double AdjustRolePoints(Room room, Player lord, string general_name, PlayerRole role, Player self)
        {
            double value = 0;
            General general = Engine.GetGeneral(general_name, "Classic");
            List<string> skills = new List<string>(TrustedAI.MasochismSkill.Split('|'));
            bool blood = false;
            foreach (string skill in skills)
            {
                if (RoomLogic.PlayerHasSkill(room, lord, skill))
                {
                    blood = true;
                    break;
                }
            }

            if (role == PlayerRole.Loyalist)
            {
                General lord_general = Engine.GetGeneral(lord.General1, "Classic");
                if (lord_general.IsLord() && lord_general.Kingdom == general.Kingdom)                           //正规主公，忠臣应尽量选国籍相同的武将
                {
                    value += 1;
                    if (lord.General1 == "yuanshu" || lord.General1 == "sunliang") value += 0.5;                //袁术孙亮主公还有额外0.5加成
                }
                if (lord.General1 == "xizhicai" && (general_name == "jvshou" || general_name == "caorui"))      //主公戏字才，忠臣巨兽曹叡佳
                    value += 4;
                if (lord.IsFemale() && general_name == "xiahoushi") value -= 3;                                 //主公女性，夏侯氏不宜作忠臣
                if (lord.General1 == "yuanshao" && general_name == "caopi") value += 2;                         //主公袁绍，曹丕忠臣佳
                if ((lord.General1 == "maliang" && general_name == "xiahoudun_jx") || (general_name == "maliang" && lord.General1 == "xiahoudun_jx"))   //马良和夏侯敦配合佳
                    value += 3;
                if ((lord.General1 == "maliang" && general_name == "xiahoushi") || (general_name == "maliang" && lord.General1 == "xiahoushi"))         //马良和夏侯氏配合佳
                    value += 1;
                if (lord.General1 == "maliang" && (general_name == "lusu" || general_name == "liubei" || general_name == "caorui" || general_name == "zhangchangpu"))    //专克马良
                    value -= 1;
                if (general_name == "maliang" && (lord.General1 == "liubei" || lord.General1 == "lusu" || lord.General1 == "caorui")) value -= 2;
                if (blood && general_name == "zhangchunhua") value -= 3;                                                                                //春哥克卖血
                if (lord.General1 == "zhangchunhua")
                {
                    List<string> _skills = Engine.GetGeneralSkills(general_name, room.Setting.GameMode);
                    foreach (string skill in _skills)
                        if (TrustedAI.MasochismGood.Contains(skill)) value -= 1.5;
                }

                if (lord.General1 == "simahui" && general_name == "guanyinping") value += 3;                                                    //关妹配合司马徽
                if ((lord.General1 == "liubei" || lord.General1 == "caorui" || lord.General1 == "liuxie") && general_name == "haozhao")
                    value += 4;                                                                                                                 //郝昭配合给牌
                if ((general_name == "liubei" || general_name == "caorui" || general_name == "liuxie") && lord.General1 == "haozhao")
                    value += 4;
                if (general_name == "xujing" && (lord.General1 == "lingtong" || lord.General1 == "sunshangxiang" || lord.General1 == "maliang" || lord.General1 == "pangtong_sp"))
                    value += 4;                                                                                                                 //许靖做配合
            }
            else if (role == PlayerRole.Rebel)
            {
                General lord_general = Engine.GetGeneral(lord.General1, "Classic");
                if ((general_name == "diaochan" || general_name == "diaochan_sp") && lord.IsFemale())                                              //主公女性，貂蝉不宜做反贼
                    value -= 2;
                if (general_name == "machao" && room.Players.Count > 5 && (self.Seat - lord.Seat > 0 && self.Seat - lord.Seat <= 2 || self.Seat >= room.Players.Count - 1))
                {
                    if (blood) value += 1;
                }
                if (blood && general_name == "zhangchunhua") value += 2.5;                                                                        //春哥克卖血
                if (general_name == "quyi" && (self.Seat - lord.Seat > 0 && self.Seat - lord.Seat <= 1 || self.Seat == room.Players.Count))     //近位麹义反贼有利
                    value += 1.5;

                if (lord.General1 == "xizhicai" && (general_name == "jvshou" || general_name == "caorui"))     //主公戏字才，反贼巨兽曹叡极不利
                    value -= 4;

                if (lord.IsFemale() && general_name == "xushi")                                                 //主公女性，徐氏不利
                    value -= 0.7;

                if (lord.General1 == "wutugu" && (general_name == "guanyinping"
                    || general_name == "wolong" || general_name == "zhouyu_god" || general_name == "liubei_god"))   //关银屏卧龙神周瑜神刘备对兀突骨有利
                    value += 2;

                if (lord.General1 == "maliang" && (general_name == "lusu" || general_name == "wangji" || lord.General1 == "diaochan_sp"))         //专克马良主
                    value += 2;

                if (lord.General1 == "zhangchunhua")                                                            //春哥克卖血
                {
                    List<string> _skills = Engine.GetGeneralSkills(general_name, room.Setting.GameMode);
                    foreach (string skill in _skills)
                        if (skills.Contains(skill)) value -= 1.5;
                }
                if (lord.General1 == "caocao_god" && general_name == "haozhao") value += 3;                     //郝昭克神曹操 
                if (general_name == "lusu" || general_name == "liubei" || general_name == "panjun" || general_name == "zhangchangpu")
                    value -= 0.5 * (8 - room.Players.Count);                                                    //以8人局为标准，每少1人强度减少0.5

                if (lord.General1 == "simahui" && general_name == "guanyinping") value -= 4;
            }
            else if (role == PlayerRole.Renegade)
            {
                General lord_general = Engine.GetGeneral(lord.General1, "Classic");
                if (lord.General1 == "xizhicai" && (general_name == "jvshou" || general_name == "caorui"))      //主公戏字才，内奸巨兽曹叡极不利
                    value -= 4;

                if (lord.IsFemale() && general_name == "xushi")                                                 //主公女性，徐氏不利
                    value -= 0.7;

                if (lord.General1 == "zhangchunhua")                                                            //春哥克卖血
                {
                    List<string> _skills = Engine.GetGeneralSkills(general_name, room.Setting.GameMode);
                    foreach (string skill in _skills)
                        if (skills.Contains(skill)) value -= 1.5;
                }
                if (lord.General1 == "simahui" && general_name == "guanyinping") value -= 2;
            }

            if (role != PlayerRole.Lord && (lord.General1 == "jvshou" || general_name == "caorui") && general_name == "xizhicai")
                value += 2;
            if (role != PlayerRole.Lord && lord.General1 == "guanyinping" && general_name == "simahui") value += 3;

            if (general_name == "caocao_god" || general_name == "tadun") value -= 0.5 * (8 - room.Players.Count);   //以8人局为标准，每少1人强度减少0.5
            if (general_name == "liuyu") value += 0.2 * room.Players.Count - 4;                                 //游戏人数用4人起，每多1人强度+0.2
            if (general_name == "shixie" || general_name == "liubiao")
                value -= 0.5 * (8 - room.Players.Count);                                                        //以8人局为标准，每少1人强度减少0.5

            return value;
        }
    }
}
