using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
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
    public class HegemonyChaos : GameScenario
    {
        public HegemonyChaos()
        {
            mode_name = "HegemonyChaos";
            rule = new HegemonyChaosRule();
        }
        public override TrustedAI GetAI(Room room, Player player)
        {
            return new TrustedAI(room, player);
        }

        public override List<Skill> GetGameRuleSkills()
        {
            return new List<Skill> { Engine.GetSkill("GameRule_AskForGeneralShowHead"),
                Engine.GetSkill("GameRule_AskForGeneralShowDeputy"),
                Engine.GetSkill("GameRule_AskForArraySummon") };
        }

        public override void Assign(Room room)
        {
            AssignGeneralsForPlayers(room, out Dictionary<Player, List<string>> options);

            List<Interactivity> receivers = new List<Interactivity>();
            foreach (Player player in options.Keys)
            {
                player.SetTag("generals", JsonUntity.Object2Json(options[player]));
                List<string> args = new List<string>
                {
                    player.Name,                                    //操作的玩家名
                    string.Empty,                                   //原因
                    JsonUntity.Object2Json(options[player]),        //备选武将
                    false.ToString(),                               //是否单将
                    true.ToString(),                                //是否可以武将转换SP
                    false.ToString()                                //双将时是否匹配相同国籍
                };
                Interactivity client = room.GetInteractivity(player);
                if (client != null && !receivers.Contains(client))
                {
                    client.CommandArgs = args;
                    receivers.Add(client);
                }
            }

            List<Player> players = room.Players;
            Countdown countdown = new Countdown
            {
                Max = room.Setting.GetCommandTimeout(CommandType.S_COMMAND_CHOOSE_GENERAL, ProcessInstanceType.S_CLIENT_INSTANCE),
                Type = Countdown.CountdownType.S_COUNTDOWN_USE_SPECIFIED
            };
            room.NotifyMoveFocus(players, countdown);
            room.DoBroadcastRequest(receivers, CommandType.S_COMMAND_CHOOSE_GENERAL);
            room.DoBroadcastNotify(CommandType.S_COMMAND_UNKNOWN, new List<string> { false.ToString() });

            foreach (Player player in options.Keys)
            {
                player.RemoveTag("generals");
                if (!string.IsNullOrEmpty(player.General1)) continue;
                bool success = true;
                Interactivity client = room.GetInteractivity(player);
                List<string> reply = client?.ClientReply;
                if (client == null || !client.IsClientResponseReady || reply == null || reply.Count == 0 || string.IsNullOrEmpty(reply[0]))
                    success = false;
                else
                {
                    string generalName = reply[0];
                    string[] generals = generalName.Split('+');
                    if (generals.Length != 2 || (!options[player].Contains(generals[0]) && room.GetClient(player).UserRight < 3)
                        || (!options[player].Contains(generals[1]) && room.GetClient(player).UserRight < 3)
                        || !SetPlayerGeneral(room, player, generals[0], true)
                        || !SetPlayerGeneral(room, player, generals[1], false))
                    {
                        success = false;
                    }
                }
                if (!success)
                {
                    SetPlayerGeneral(room, player, options[player][0], true);
                    SetPlayerGeneral(room, player, options[player][1], false);
                }
            }

            foreach (Player player in players)
            {
                List<string> names = new List<string>();
                if (!string.IsNullOrEmpty(player.General1))
                {
                    string name = player.General1;
                    player.Kingdom = "god";
                    string role = Engine.GetMappedRole(player.Kingdom);
                    if (string.IsNullOrEmpty(role))
                        role = General.GetKingdom(Engine.GetGeneral(player.General1, room.Setting.GameMode).Kingdom[0]);
                    names.Add(name);
                    player.Role = role;
                    player.General1 = "anjiang";
                    room.BroadcastProperty(player, "General1");
                    room.NotifyProperty(room.GetClient(player), player, "ActualGeneral1");

                    foreach (Client p in room.Clients)
                    {
                        if (p != room.GetClient(player))
                            room.NotifyProperty(p, player, "Kingdom", "god");
                    }
                }

                if (!string.IsNullOrEmpty(player.General2))
                {
                    string name = player.General2;
                    names.Add(name);
                    player.General2 = "anjiang";
                    room.BroadcastProperty(player, "General2");
                    room.NotifyProperty(room.GetClient(player), player, "ActualGeneral2");
                }
                room.SetTag(player.Name, names);

                room.HandleUsedGeneral(names[0]);
                room.HandleUsedGeneral(names[1]);

                if (reserved.TryGetValue(player, out List<string> p_reserved) && (p_reserved.Contains(names[0]) || p_reserved.Contains(names[1])))
                {
                    LogMessage reserved_log = new LogMessage();
                    reserved_log.Type = "#reserved_pick";
                    reserved_log.From = player.Name;
                    room.SendLog(reserved_log);
                }

                if (!options[player].Contains(names[0]) || !options[player].Contains(names[1]))
                {
                    LogMessage log = new LogMessage();
                    log.Type = "#cheat_pick";
                    log.From = player.Name;
                    room.SendLog(log);
                }
            }

            //选择国籍
            receivers.Clear();
            players.Clear();
            List<string> prompts = new List<string> { "@choose-kingdom" };
            Dictionary<Player, List<string>> kingdoms = new Dictionary<Player, List<string>>();
            foreach (Player player in room.Players)
            {
                List<string> choices = new List<string> { "careerist" };
                General g1 = Engine.GetGeneral(player.ActualGeneral1, Name);
                General g2 = Engine.GetGeneral(player.ActualGeneral2, Name);
                foreach (General.KingdomENUM kingdom in g1.Kingdom)
                    if (!choices.Contains(General.GetKingdom(kingdom)))
                        choices.Add(General.GetKingdom(kingdom));
                foreach (General.KingdomENUM kingdom in g2.Kingdom)
                    if (!choices.Contains(General.GetKingdom(kingdom)))
                        choices.Add(General.GetKingdom(kingdom));
                kingdoms.Add(player, choices);
                List<string> args = new List<string>
                {
                    player.Name,
                    "Kingdom",
                    string.Join("+", choices),
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

                List<string> choices = kingdoms[player];
                if (string.IsNullOrEmpty(answer) || !choices.Contains(answer))
                {
                    Shuffle.shuffle(ref choices);
                    answer = choices[0];
                }

                if (answer != "careerist")
                {
                    player.Kingdom = answer;
                    room.NotifyProperty(room.GetClient(player), player, "Kingdom");
                }
                else
                {
                    player.Role = answer;
                    room.NotifyProperty(room.GetClient(player), player, "Role");
                }
            }

            //君主转换
            if (room.Setting.LordConvert)
                room.AskForLordConvert();

            foreach (Player player in players)
            {
                General general1 = Engine.GetGeneral(player.ActualGeneral1, room.Setting.GameMode);
                General general2 = Engine.GetGeneral(player.ActualGeneral2, room.Setting.GameMode);

                if (general1.CompanionWith(player.ActualGeneral2, Name))
                    player.AddMark("CompanionEffect");

                int max_hp = general1.GetMaxHpHead() + general2.GetMaxHpDeputy();
                player.SetMark("HalfMaxHpLeft", max_hp % 2);

                player.MaxHp = max_hp / 2;
                player.Hp = player.MaxHp;

                room.BroadcastProperty(player, "MaxHp");
                room.BroadcastProperty(player, "Hp");
            }
        }


        private bool SetPlayerGeneral(Room room, Player player, string generalName, bool isFirst)
        {
            General general = Engine.GetGeneral(generalName, room.Setting.GameMode);
            if (general == null)
            {
                return false;
            }
            //自由选将判断
            //else if (!Config.FreeChoose && !player->getSelected().contains(Sanguosha->getMainGenerals(generalName)))
            //    return false;

            if (isFirst)
                player.ActualGeneral1 = player.General1 = general.Name;
            else
                player.ActualGeneral2 = player.General2 = general.Name;

            return true;
        }

        private void AssignGeneralsForPlayers(Room room, out Dictionary<Player, List<string>> options)
        {
            options = new Dictionary<Player, List<string>>();

            int max_choice = room.Setting.GeneralCount;
            List<string> generals = new List<string>(room.Generals);
            if (generals.Count < max_choice * room.Players.Count)
                max_choice = generals.Count / room.Players.Count;

            for (int i = 0; i < room.Clients.Count; i++)
            {
                Client client = room.Clients[i];
                if (client.UserId < 0) continue;
                List<string> reserved_generals = client.GeneralReserved;
                if (reserved_generals == null || reserved_generals.Count == 0) continue;

                foreach (string general in reserved_generals)
                {
                    for (int y = i + 1; y < room.Clients.Count; y++)
                    {
                        Client client2 = room.Clients[y];
                        if (client == client2 || client2.UserId < 0 || client2.GeneralReserved == null || client2.GeneralReserved.Count == 0) continue;
                        if (client2.GeneralReserved.Contains(general))
                        {
                            client.GeneralReserved.RemoveAll(t => t == general);
                            client2.GeneralReserved.RemoveAll(t => t == general);
                        }
                    }
                }
            }
            foreach (Client client in room.Clients)
            {
                if (client.GeneralReserved != null && client.GeneralReserved.Count > 0 && client.GeneralReserved.Count <= 2)
                {
                    foreach (Player p in room.Players)
                    {
                        if (p.ClientId == client.UserId)
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
                            reserved[p] = new List<string>(options[p]);

                            break;
                        }
                    }
                }

                client.GeneralReserved = null;
            }

            foreach (Player player in room.Players)
            {
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
        }

        public override void PrepareForStart(Room room, ref List<Player> room_players, ref List<int> game_cards, ref List<int> m_drawPile)
        {
            if (room.Setting.LordConvert)
            {
                foreach (int id in Engine.GetGameCards(room.Setting.CardPackage))
                    if (Engine.GetRealCard(id).Name != DoubleSword.ClassName
                        && Engine.GetRealCard(id).Name != "Jingfan"
                        && Engine.GetRealCard(id).Name != SixSwords.ClassName
                        && Engine.GetRealCard(id).Name != "Zhuahuangfeidian")
                        game_cards.Add(id);
            }
            else
            {
                foreach (int id in Engine.GetGameCards(room.Setting.CardPackage))
                    if (Engine.GetRealCard(id).Name != DragonPhoenix.ClassName
                        && Engine.GetRealCard(id).Name != PeaceSpell.ClassName
                        && Engine.GetRealCard(id).Name != LuminouSpearl.ClassName
                        && Engine.GetRealCard(id).Name != DragonCarriage.ClassName)
                        game_cards.Add(id);
            }
            m_drawPile = game_cards;

            Shuffle.shuffle<int>(ref m_drawPile);
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

                player.ClientId = client.UserId;
            }
        }


        public override bool IsFriendWith(Room room, Player player, Player other)
        {
            if (player == null || other == null)
                return false;
            if (other == player)
                return true;
            if (!player.HasShownOneGeneral() || !other.HasShownOneGeneral())
                return false;

            if (player.GetRoleEnum() == PlayerRole.Careerist || other.GetRoleEnum() == PlayerRole.Careerist)
                return false;

            return player.Kingdom == other.Kingdom;
        }

        public override bool WillBeFriendWith(Room room, Player player, Player other, string show_skill = null)
        {
            if (player == null || other == null)
                return false;
            if (other == player)
                return true;
            if (IsFriendWith(room, player, other))
                return true;
            if (!other.HasShownOneGeneral())
                return false;
            if (!player.HasShownOneGeneral())
            {
                if (player.GetRoleEnum() == PlayerRole.Careerist || WillbeRole(room, player, show_skill) == "careerist") return false;
                string kingdom = player.Kingdom;
                if (Engine.GetGeneral(player.ActualGeneral1, room.Setting.GameMode).IsLord() && kingdom == other.Kingdom) return true;
                if (other.GetRoleEnum() == PlayerRole.Careerist) return false;
                if (kingdom == other.Kingdom) return true;
            }

            return false;
        }

        public static string WillbeRole(Room room, Player player, string show_skill = null)
        {
            string kingdom = player.Kingdom;
            string role = Engine.GetMappedRole(kingdom);

            Player lord = GetLord(room, player, true);
            if (lord != null && (lord.General1Showed || (string.IsNullOrEmpty(show_skill) && player == lord)
                || (!string.IsNullOrEmpty(show_skill) && player.GetHeadSkillList().Contains(show_skill))))
            {
                if (lord.Alive)
                {
                    return role;
                }
                else
                    return "careerist";
            }

            int i = 1;
            foreach (Player p in room.Players)
            {
                if (p == player) continue;
                if (p.HasShownOneGeneral() && p.Role != "careerist" && p.Kingdom == kingdom)
                    ++i;
            }

            if (i > (room.Players.Count + 1) / 2)
                role = "careerist";
            return role;
        }

        public static Player GetLord(Room room, Player judger, bool include_death = false)
        {
            if (Engine.GetGeneral(judger.ActualGeneral1, room.Setting.GameMode).IsLord())
                return judger;

            foreach (Player p in room.Players)
            {
                if (p == judger || (!include_death && !p.Alive)) continue;
                if (p.General1Showed && p.Kingdom == General.GetKingdom(Engine.GetGeneral(judger.ActualGeneral1, room.Setting.GameMode).Kingdom[0])
                    && Engine.GetGeneral(p.General1, room.Setting.GameMode).IsLord())
                    return p;
            }

            return null;
        }

        public override void PrepareForPlayers(Room room)
        {
            foreach (Player player in room.Players)
            {
                string general1_name = player.ActualGeneral1;
                if (!player.DuanChang.Contains("head"))
                {
                    foreach (string skill in Engine.GetGeneralSkills(general1_name, Name, true))
                        room.AddPlayerSkill(player, skill);
                }
                string general2_name = player.ActualGeneral2;
                if (!string.IsNullOrEmpty(general2_name) && general2_name != general1_name && !player.DuanChang.Contains("deputy"))
                {
                    foreach (string skill in Engine.GetGeneralSkills(general2_name, Name, false))
                        room.AddPlayerSkill(player, skill, false);
                }

                //技能预亮
                //if (player->isAutoPreshow())
                //    player->setSkillsPreshowed("hd");
                room.NotifyPlayerPreshow(player);
                Client client = room.GetClient(player);
                if (client == null || client.Status == Client.GameStatus.bot)
                {
                    player.SetSkillsPreshowed("hd");
                }

                if (!player.HasShownOneGeneral())
                    player.PlayerGender = Gender.Sexless;
                else
                    player.PlayerGender = player.General1Showed ? Engine.GetGeneral(player.General1, room.Setting.GameMode).GeneralGender
                        : Engine.GetGeneral(player.General2, room.Setting.GameMode).GeneralGender;
            }
        }

        public override void OnChooseGeneralReply(Room room, Interactivity client)
        {
            Player player = room.GetPlayers(client.ClientId)[0];
            List<string> options = JsonUntity.Json2List<string>((string)player.GetTag("generals"));
            List<string> reply = client.ClientReply;
            bool success = true;
            string generalName = string.Empty;
            if (!client.IsClientResponseReady || reply == null || reply.Count == 0 || string.IsNullOrEmpty(reply[0]))
                success = false;
            else
            {
                generalName = reply[0];
                string[] generals = generalName.Split('+');
                if (generals.Length != 2)
                    success = false;
                else
                {
                    General general1 = Engine.GetGeneral(generals[0], Name);
                    General general2 = Engine.GetGeneral(generals[1], Name);
                    if (general1 == null || general1.Hidden || general2 == null || general2.Hidden
                        || !room.Setting.GeneralPackage.Contains(general1.Package)
                        || !room.Setting.GeneralPackage.Contains(general2.Package)
                        || (!options.Contains(generals[0]) && room.GetClient(player).UserRight < 3)
                        || (!options.Contains(generals[1]) && room.GetClient(player).UserRight < 3)
                        || !SetPlayerGeneral(room, player, generals[0], true)
                        || !SetPlayerGeneral(room, player, generals[1], false))
                    {
                        success = false;
                    }
                }
            }

            if (!success)
            {
                List<string> default_generals = GeneralSelctor.GeInstance().SelectGenerals(room, options);
                SetPlayerGeneral(room, player, default_generals[0], true);
                SetPlayerGeneral(room, player, default_generals[1], false);
            }

            if (!string.IsNullOrEmpty(player.General1))
            {
                string name = player.General1;
                string kingdom = General.GetKingdom(Engine.GetGeneral(name, room.Setting.GameMode).Kingdom[0]);
                room.NotifyProperty(room.GetClient(player), player, "ActualGeneral1", name);
                room.NotifyProperty(room.GetClient(player), player, "Kingdom", kingdom);
            }
            if (!string.IsNullOrEmpty(player.General2))
                room.NotifyProperty(room.GetClient(player), player, "ActualGeneral2", player.General2);
        }

        public override string GetPreWinner(Room room, Client client)
        {
            Player surrender = room.GetPlayers(client.UserId)[0];
            List<string> winners = new List<string>();
            string winner_kingdom = string.Empty;
            List<Player> players = room.GetAlivePlayers();
            foreach (Player p in players)
            {
                if (p == surrender) continue;
                if (p.GetRoleEnum() == PlayerRole.Careerist)
                    return p.Name;
                else
                {
                    winner_kingdom = p.Kingdom;
                    break;
                }
            }

            foreach (Player p in room.Players)
            {
                if (p.Kingdom == winner_kingdom)
                    winners.Add(p.Name);
            }

            return string.Join("+", winners);
        }

        public override List<Interactivity> CheckSurrendAvailable(Room room)
        {
            List<Interactivity> clients = new List<Interactivity>();
            bool check = true;
            Dictionary<string, int> kingdoms = new Dictionary<string, int>();
            foreach (Player p in room.GetAlivePlayers())
            {
                if (!p.HasShownOneGeneral())
                {
                    check = false;
                    break;
                }
                if (p.GetRoleEnum() == PlayerRole.Careerist)
                    kingdoms.Add(p.Name, 1);
                else if (!kingdoms.ContainsKey(p.Kingdom))
                    kingdoms.Add(p.Kingdom, 1);
                else
                    kingdoms[p.Kingdom]++;
            }

            if (check && kingdoms.Keys.Count == 2)
            {
                List<string> lest_kingdom = new List<string>(kingdoms.Keys);
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (lest_kingdom.Contains(p.Name) && p.ClientId > 0)
                    {
                        clients.Add(room.GetInteractivity(p.ClientId));
                    }
                    else if (kingdoms.ContainsKey(p.Kingdom) && kingdoms[p.Kingdom] == 1 && p.ClientId > 0)
                    {
                        clients.Add(room.GetInteractivity(p.ClientId));
                    }
                }
            }

            return clients;
        }
    }

    public class HegemonyChaosRule : GameRule
    {
        public HegemonyChaosRule() : base("hegemony_chaos_rule")
        {
        }

        public override string GetWinner(Room room)
        {
            if (!string.IsNullOrEmpty(room.PreWinner)) return room.PreWinner;

            List<string> winners = new List<string>();
            List<Player> players = room.GetAlivePlayers();
            Player win_player = players[0];
            if (players.Count == 1)
            {
                if (!win_player.General1Showed)
                    room.ShowGeneral(win_player, true, false, false);
                if (!win_player.General2Showed)
                    room.ShowGeneral(win_player, false, false, false);
                foreach (Player p in room.Players)
                {
                    if (RoomLogic.IsFriendWith(room, win_player, p))
                        winners.Add(p.Name);
                }
            }
            else
            {
                string left_kingdom = null;
                foreach (Player p in players)
                {
                    left_kingdom = p.Kingdom;
                    foreach (Player p2 in players)
                    {
                        if (p == p2) continue;
                        if (p.Role == "careerist" || p2.Kingdom != p.Kingdom)
                            return null;
                    }
                }

                bool all_live_shown = true;
                //check all shown before judging careerist, cos same skills could change kindom such as dragonphoenix
                foreach (Player p in players)
                {
                    if (!p.HasShownOneGeneral())
                        all_live_shown = false;
                }

                if (!all_live_shown)
                {
                    // judge careerist
                    List<string> lords = new List<string>();
                    int all = players.Count;

                    foreach (Player p in room.Players)
                    {
                        if (Engine.GetGeneral(p.ActualGeneral1, room.Setting.GameMode).IsLord())
                        {
                            if (p.Alive && p.General1Showed)                //the lord has shown
                                lords.Add(General.GetKingdom(Engine.GetGeneral(p.ActualGeneral1, room.Setting.GameMode).Kingdom[0]));
                            else if (!p.Alive && p.Kingdom == left_kingdom)   //the lord is dead, all careerist
                                return null;
                            else if (p.Alive && !p.HasShownOneGeneral() && all > room.Players.Count / 2) //the lord not yet shown
                                return null;
                        }
                    }
                    if (lords.Count == 0 && all > room.Players.Count / 2)       //careerist exists
                        return null;
                }

                // if run here, all are friend.
                foreach (Player p in players)
                {
                    if (!p.General1Showed)
                        room.ShowGeneral(p, true, false, false); // dont trigger event
                    if (!p.General2Showed)
                        room.ShowGeneral(p, false, false, false);
                }

                foreach (Player p in room.Players)
                {
                    if (RoomLogic.IsFriendWith(room, win_player, p))
                        winners.Add(p.Name);
                }
            }

            return string.Join("+", winners);
        }

        protected override void RewardAndPunish(Room room, Player killer, Player victim)
        {
            if (!killer.Alive || !killer.HasShownOneGeneral())
                return;

            if (!RoomLogic.IsFriendWith(room, killer, victim))
            {
                if (killer.GetRoleEnum() == PlayerRole.Careerist)
                    room.DrawCards(killer, 3, "gamerule");
                else
                {
                    int n = 1;
                    foreach (Player p in room.GetOtherPlayers(victim))
                    {
                        if (RoomLogic.IsFriendWith(room, victim, p))
                            ++n;
                    }
                    room.DrawCards(killer, n, "gamerule");
                }
            }
            else
                room.ThrowAllHandCardsAndEquips(killer);
        }

        protected override void OnBuryVictim(Room room, Player player, ref object data)
        {
            DeathStruct death = (DeathStruct)data;
            room.BuryPlayer(player);

            if (room.ContainsTag("SkipNormalDeathProcess") && (bool)room.GetTag("SkipNormalDeathProcess"))
                return;

            Player killer = death.Damage.From ?? null;
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

            if (Engine.GetGeneral(player.General1, room.Setting.GameMode).IsLord() && player == death.Who)
            {
                foreach (Player p in room.GetOtherPlayers(player, true))
                {
                    if (p.Kingdom == player.Kingdom)
                    {
                        p.Role = "careerist";
                        if (p.HasShownOneGeneral())
                        {
                            room.BroadcastProperty(p, "Role");
                        }
                        else
                        {
                            room.NotifyProperty(room.GetClient(p), p, "Role");
                        }
                    }
                }
                CheckBigKingdoms(room);
            }
        }
    }
}
