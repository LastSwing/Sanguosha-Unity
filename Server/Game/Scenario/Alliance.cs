using CommonClass;
using CommonClass.Game;
using CommonClassLibrary;
using SanguoshaServer.AI;
using SanguoshaServer.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using static SanguoshaServer.Game.Skill;

namespace SanguoshaServer.Scenario
{
    public class Alliance : GameScenario
    {
        public Alliance()
        {
            mode_name = "Alliance";
            rule = new AllianceRule();
            skills = new List<Skill> { new HuaxiongHp() };
        }

        public override void Assign(Room room)
        {
            System.Threading.Thread.Sleep(1000);

            List<Player> players = new List<Player>();
            if (room.Players.Count == 8)
            {
                List<string> soldiers = new List<string> { "longxiangjun", "hubenjun", "baoluejun", "fengyaojun", "feixiongjun", "tanlangjun" };
                Shuffle.shuffle(ref soldiers);

                room.Players[1].General1 = room.Players[1].ActualGeneral1 = soldiers[0];
                room.Players[2].General1 = room.Players[2].ActualGeneral1 = "huaxiong_al";
                room.Players[3].General1 = room.Players[3].ActualGeneral1 = soldiers[1];
                room.Players[5].General1 = room.Players[5].ActualGeneral1 = soldiers[2];
                room.Players[6].General1 = room.Players[6].ActualGeneral1 = "sunjian_jx";
                room.Players[7].General1 = room.Players[7].ActualGeneral1 = soldiers[3];

                players.Add(room.Players[0]);
                players.Add(room.Players[4]);
                foreach (Player lord in room.Players)
                {
                    if (lord.Camp == Game3v3Camp.S_CAMP_WARM || lord.General1 == "sunjian_jx")
                    {
                        General lord_gen = Engine.GetGeneral(lord.General1, room.Setting.GameMode);
                        lord.PlayerGender = lord_gen.GeneralGender;
                        lord.Kingdom = General.GetKingdom(lord_gen.Kingdom[0]);
                        lord.General1Showed = true;
                        room.BroadcastProperty(lord, "General1");
                        room.BroadcastProperty(lord, "PlayerGender");
                        room.BroadcastProperty(lord, "Kingdom");
                        room.BroadcastProperty(lord, "General1Showed");
                        foreach (string skill in Engine.GetGeneralSkills(lord.General1, Name, true))
                        {
                            room.AddPlayerSkill(lord, skill);
                            Skill s = Engine.GetSkill(skill);
                            if (s != null && s.SkillFrequency == Frequency.Limited && !string.IsNullOrEmpty(s.LimitMark))
                                room.SetPlayerMark(lord, s.LimitMark, 1);
                        }

                        room.SendPlayerSkillsToOthers(lord, true);

                        //技能预亮
                        lord.SetSkillsPreshowed("hd");
                        room.NotifyPlayerPreshow(lord);
                        room.HandleUsedGeneral(lord.General1);
                    }
                }
            }
            else
            {
                //随机关卡
                Random rand = new Random();
                int stage = rand.Next(0, 6);
                //AI将领
                switch (stage)
                {
                    case 0:
                        room.Players[3].General1 = room.Players[3].ActualGeneral1 = "zhangji_al";
                        room.Players[2].General1 = room.Players[2].ActualGeneral1 = room.Players[4].General1 = room.Players[4].ActualGeneral1 = "longxiangjun";
                        break;
                    case 1:
                        room.Players[3].General1 = room.Players[3].ActualGeneral1 = "fanchou_al";
                        room.Players[2].General1 = room.Players[2].ActualGeneral1 = room.Players[4].General1 = room.Players[4].ActualGeneral1 = "hubenjun";
                        break;
                    case 2:
                        room.Players[3].General1 = room.Players[3].ActualGeneral1 = "dongxie_al";
                        room.Players[2].General1 = room.Players[2].ActualGeneral1 = room.Players[4].General1 = room.Players[4].ActualGeneral1 = "fengyaojun";
                        break;
                    case 3:
                        room.Players[3].General1 = room.Players[3].ActualGeneral1 = "dongyue_al";
                        room.Players[2].General1 = room.Players[2].ActualGeneral1 = room.Players[4].General1 = room.Players[4].ActualGeneral1 = "baoluejun";
                        break;
                    case 4:
                        room.Players[2].General1 = room.Players[2].ActualGeneral1 = "tanlangjun";
                        room.Players[3].General1 = room.Players[3].ActualGeneral1 = "lijue_al";
                        room.Players[4].General1 = room.Players[4].ActualGeneral1 = "feixiongjun";
                        break;
                    case 5:
                        room.Players[2].General1 = room.Players[2].ActualGeneral1 = "tanlangjun";
                        room.Players[3].General1 = room.Players[3].ActualGeneral1 = "guosi_al";
                        room.Players[4].General1 = room.Players[4].ActualGeneral1 = "feixiongjun";
                        break;
                }

                players.Add(room.Players[0]);
                players.Add(room.Players[1]);
                foreach (Player lord in room.Players)
                {
                    if (lord.Camp == Game3v3Camp.S_CAMP_WARM)
                    {
                        General lord_gen = Engine.GetGeneral(lord.General1, room.Setting.GameMode);
                        lord.PlayerGender = lord_gen.GeneralGender;
                        lord.Kingdom = General.GetKingdom(lord_gen.Kingdom[0]);
                        lord.General1Showed = true;
                        room.BroadcastProperty(lord, "General1");
                        room.BroadcastProperty(lord, "PlayerGender");
                        room.BroadcastProperty(lord, "Kingdom");
                        room.BroadcastProperty(lord, "General1Showed");
                        foreach (string skill in Engine.GetGeneralSkills(lord.General1, Name, true))
                        {
                            room.AddPlayerSkill(lord, skill);
                            Skill s = Engine.GetSkill(skill);
                            if (s != null && s.SkillFrequency == Frequency.Limited && !string.IsNullOrEmpty(s.LimitMark))
                                room.SetPlayerMark(lord, s.LimitMark, 1);
                        }

                        room.SendPlayerSkillsToOthers(lord, true);

                        //技能预亮
                        lord.SetSkillsPreshowed("hd");
                        room.NotifyPlayerPreshow(lord);
                        room.HandleUsedGeneral(lord.General1);
                    }
                }
            }
            System.Threading.Thread.Sleep(700);

            //为其余玩家分配武将
            AssignGeneralsForPlayers(room, out Dictionary<Player, List<string>> options);
            //选将      
            List<Interactivity> receivers = new List<Interactivity>();
            Dictionary<string, List<string>> choose = new Dictionary<string, List<string>>();
            foreach (Player player in options.Keys)
                choose[player.Name] = options[player];

            foreach (Player player in options.Keys)
            {
                player.SetTag("generals", JsonUntity.Object2Json(options[player]));
                List<string> args = new List<string>
                {
                    player.Name,
                    JsonUntity.Object2Json(choose),
                };
                Interactivity client = room.GetInteractivity(player);
                if (client != null)
                {
                    client.CommandArgs = args;
                    receivers.Add(client);
                }
                players.Add(player);
            }

            Countdown countdown = new Countdown
            {
                Max = room.Setting.GetCommandTimeout(CommandType.S_COMMAND_GENERAL_PICK, ProcessInstanceType.S_CLIENT_INSTANCE),
                Type = Countdown.CountdownType.S_COUNTDOWN_USE_SPECIFIED
            };
            room.NotifyMoveFocus(players, countdown);
            room.DoBroadcastRequest(receivers, CommandType.S_COMMAND_GENERAL_PICK);
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

                    if (!success)
                    {
                        if (client == null)
                            continue;
                        else
                            generalName = options[player][0];
                    }
                    else if (!string.IsNullOrEmpty(generalName) && !options[player].Contains(generalName) && room.GetClient(player).UserRight < 3)
                        generalName = options[player][0];

                    player.General1 = generalName;
                    player.ActualGeneral1 = generalName;
                    player.Kingdom = General.GetKingdom(Engine.GetGeneral(generalName, room.Setting.GameMode).Kingdom[0]);
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

            foreach (Player player in options.Keys)
            {
                if (string.IsNullOrEmpty(player.General1))
                {
                    Player human = null;
                    foreach (Player p2 in options.Keys)
                    {
                        if (p2 != player)
                        {
                            human = p2;
                            break;
                        }
                    }
                    string generalName = room.AskForGeneral(human, options[player], null, true, Name, null, true);

                    player.General1 = generalName;
                    player.ActualGeneral1 = generalName;
                    player.Kingdom = General.GetKingdom(Engine.GetGeneral(generalName, room.Setting.GameMode).Kingdom[0]);
                    player.General1Showed = true;

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
            }
        }

        private void AssignGeneralsForPlayers(Room room, out Dictionary<Player, List<string>> options)
        {
            options = new Dictionary<Player, List<string>>();
            List<string> generals = new List<string>(room.Generals), reserved = new List<string>();

            for (int i = 0; i < room.Clients.Count; i++)
            {
                Client client = room.Clients[i];
                if (client.UserId < 0) continue;
                List<string> reserved_generals = new List<string>(client.GeneralReserved);
                if (reserved_generals == null || reserved_generals.Count == 0) continue;
                reserved.AddRange(reserved_generals);
            }

            List<string> duplicated = reserved.GroupBy(x => x).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
            for (int i = 0; i < room.Clients.Count; i++)
            {
                Client client = room.Clients[i];
                if (client.UserId < 0) continue;
                if (client.GeneralReserved == null || client.GeneralReserved.Count == 0) continue;
                client.GeneralReserved.RemoveAll(t => duplicated.Contains(t));
                generals.RemoveAll(t => client.GeneralReserved.Contains(t));
            }


            foreach (Player player in room.Players)
            {
                if (player.Camp == Game3v3Camp.S_CAMP_COOL && player.General1 != "sunjian_jx")
                {
                    List<string> choices = new List<string>();
                    Client client = room.GetClient(player);
                    if (client != null && client.UserId >= 0 && client.GeneralReserved != null)
                    {
                        choices.AddRange(client.GeneralReserved);
                        client.GeneralReserved = null;
                    }

                    int count = 6 - choices.Count;
                    for (int i = 0; i < count; i++)
                    {
                        Shuffle.shuffle(ref generals);
                        choices.Add(generals[0]);
                        generals.RemoveAt(0);
                    }
                    options.Add(player, choices);
                }
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

        public override TrustedAI GetAI(Room room, Player player)
        {
            return new AllianceAI(room, player);
        }
        public override bool IsFriendWith(Room room, Player player, Player other)
        {
            return player.Camp == other.Camp;
        }
        public override bool WillBeFriendWith(Room room, Player player, Player other, string skill_name = null)
        {
            return IsFriendWith(room, player, other);
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

            General general = Engine.GetGeneral(generalName, Name);
            if (!success || general == null || general.Hidden || !room.Setting.GeneralPackage.Contains(general.Package))
            {
                generalName = options[0];
            }

            player.General1 = generalName;
            player.ActualGeneral1 = generalName;
            player.Kingdom = General.GetKingdom(Engine.GetGeneral(generalName, room.Setting.GameMode).Kingdom[0]);
            player.General1Showed = true;
            room.NotifyProperty(room.GetClient(player), player, "General1");
            room.NotifyProperty(room.GetClient(player), player, "Kingdom");

            foreach (Player p in room.Players)
            {
                if (p != player && p.Camp == player.Camp)
                {
                    room.NotifyProperty(room.GetClient(p), player, "General1");
                    room.NotifyProperty(room.GetClient(p), player, "Kingdom");
                }
            }
        }

        public override void PrepareForPlayers(Room room)
        {
            //玩家添加技能
            //调整血量
            foreach (Player player in room.Players)
            {
                string general1_name = player.ActualGeneral1;
                if (player.Camp == Game3v3Camp.S_CAMP_COOL && player.General1 != "sunjian_jx")
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
                int max_hp = g.DoubleMaxHp;
                if (room.Players.Count == 8 && player.Camp == Game3v3Camp.S_CAMP_WARM && player.GetRoleEnum() != Player.PlayerRole.Lord)
                    max_hp--;
                player.MaxHp = max_hp;
                player.Hp = Math.Max(1, player.MaxHp + g.Head_max_hp_adjusted_value);

                room.BroadcastProperty(player, "MaxHp");
                room.BroadcastProperty(player, "Hp");
            }
        }

        public override void PrepareForStart(Room room, ref List<Player> room_players, ref List<int> game_cards, ref List<int> m_drawPile)
        {
            //生成卡牌
            game_cards = Engine.GetGameCards(room.Setting.CardPackage);
            m_drawPile = game_cards;
            Shuffle.shuffle(ref m_drawPile);

            //随机关卡
            Random rand = new Random();
            int stage = rand.Next(0, 7);

            if (stage != 6)
            {
                //首先添加3位敌方AI，不需要用到client
                int computer_index = 0;
                for (int i = 2; i < 5; i++)
                {
                    Player computer = new Player { Name = string.Format("SGS{0}", i + 1), Seat = i + 1 };
                    computer.Status = "bot";
                    computer.ClientId = 0;
                    computer.Camp = Game3v3Camp.S_CAMP_WARM;
                    computer.SceenName = string.Format("computer{0}", computer_index);

                    if (i == 3)
                        computer.Role = "lord";
                    else
                        computer.Role = "loyalist";

                    computer_index++;
                    room_players.Add(computer);
                }

                List<Client> clients = new List<Client>(room.Clients);
                Shuffle.shuffle(ref clients);
                for (int i = 0; i < 2; i++)
                {
                    Player player = room_players[i];
                    player.Camp = Game3v3Camp.S_CAMP_COOL;
                    Client client = clients[i];
                    if (client.UserId > 0) client.Status = Client.GameStatus.online;
                    player.SceenName = client.Profile.NickName;
                    player.Status = client.Status.ToString();
                    player.ClientId = client.UserId;
                    player.Role = "loyalist";
                }
            }
            else //华雄关
            {
                //首先添加6位AI，不需要用到client
                int computer_index = 0;
                for (int i = 2; i < 8; i++)
                    room_players.Add(new Player { Name = string.Format("SGS{0}", i + 1), Seat = i + 1 });

                List<Client> clients = new List<Client>(room.Clients);
                Shuffle.shuffle(ref clients);

                //玩家1
                room_players[0].Camp = Game3v3Camp.S_CAMP_COOL;
                if (clients[0].UserId > 0) clients[0].Status = Client.GameStatus.online;
                room_players[0].SceenName = clients[0].Profile.NickName;
                room_players[0].Status = clients[0].Status.ToString();
                room_players[0].ClientId = clients[0].UserId;
                room_players[0].Role = "loyalist";

                //电脑1       小兵1
                room_players[1].Camp = Game3v3Camp.S_CAMP_WARM;
                room_players[1].SceenName = string.Format("computer{0}", computer_index);
                room_players[1].Status = "bot";
                room_players[1].ClientId = 0;
                room_players[1].Role = "loyalist";
                computer_index++;

                //电脑2       华雄
                room_players[2].Camp = Game3v3Camp.S_CAMP_WARM;
                room_players[2].SceenName = string.Format("computer{0}", computer_index);
                room_players[2].Status = "bot";
                room_players[2].ClientId = 0;
                room_players[2].Role = "lord";
                computer_index++;

                //电脑3       小兵2
                room_players[3].Camp = Game3v3Camp.S_CAMP_WARM;
                room_players[3].SceenName = string.Format("computer{0}", computer_index);
                room_players[3].Status = "bot";
                room_players[3].ClientId = 0;
                room_players[3].Role = "loyalist";
                computer_index++;

                //玩家2
                room_players[4].Camp = Game3v3Camp.S_CAMP_COOL;
                if (clients[1].UserId > 0) clients[1].Status = Client.GameStatus.online;
                room_players[4].SceenName = clients[1].Profile.NickName;
                room_players[4].Status = clients[1].Status.ToString();
                room_players[4].ClientId = clients[1].UserId;
                room_players[4].Role = "loyalist";

                //电脑4       小兵3
                room_players[5].Camp = Game3v3Camp.S_CAMP_WARM;
                room_players[5].SceenName = string.Format("computer{0}", computer_index);
                room_players[5].Status = "bot";
                room_players[5].ClientId = 0;
                room_players[5].Role = "loyalist";
                computer_index++;

                //电脑5       孙坚
                room_players[6].Camp = Game3v3Camp.S_CAMP_COOL;
                room_players[6].SceenName = string.Format("computer{0}", computer_index);
                room_players[6].Status = "bot";
                room_players[6].ClientId = 0;
                room_players[6].Role = "loyalist";
                computer_index++;

                //电脑4       小兵4
                room_players[7].Camp = Game3v3Camp.S_CAMP_WARM;
                room_players[7].SceenName = string.Format("computer{0}", computer_index);
                room_players[7].Status = "bot";
                room_players[7].ClientId = 0;
                room_players[7].Role = "loyalist";
            }

            //通知身份
            for (int i = 0; i < room_players.Count; i++)
            {
                Player player = room_players[i];
                if (i == room_players.Count - 1)
                    player.Next = room_players[0].Name;
                else
                    player.Next = room_players[i + 1].Name;
                room.BroadcastProperty(player, "Camp");
                room.BroadcastProperty(player, "Role");
            }
        }


        public override string GetPreWinner(Room room, Client client)
        {
            Player surrender = room.GetPlayers(client.UserId)[0];
            List<string> winners = new List<string>();
            Game3v3Camp dead_camp = surrender.Camp;
            foreach (Player p in room.Players)
            {
                if (p.Camp != dead_camp)
                    winners.Add(p.Name);
            }

            return string.Join("+", winners);
        }

        public override List<Interactivity> CheckSurrendAvailable(Room room)
        {
            List<Interactivity> clients = new List<Interactivity>(), alives = new List<Interactivity>();
            foreach (Player p in room.Players)
            {
                if (p.Camp == Game3v3Camp.S_CAMP_COOL && p.ClientId > 0)
                {
                    Interactivity inter = room.GetInteractivity(p.ClientId);
                    clients.Add(inter);
                    if (p.Alive)
                        alives.Add(inter);
                }
            }

            return alives.Count <= 1 ? alives : new List<Interactivity>();
        }
    }

    public class AllianceRule : GameRule
    {
        public AllianceRule() : base("alliance_rule")
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
            foreach (Player p in room.Players)
            {
                int count = 4;
                if (p.Camp == Game3v3Camp.S_CAMP_COOL) count = 5;
                room.DrawCards(p, count, "gamerule");
            }

            room.AskForLuckCard(1);

            //游戏开始动画
            room.DoBroadcastNotify(CommandType.S_COMMAND_GAME_START, new List<string>());
            System.Threading.Thread.Sleep(2000);
        }

        //胜利条件
        public override string GetWinner(Room room)
        {
            if (!string.IsNullOrEmpty(room.PreWinner)) return room.PreWinner;

            List<string> winners = new List<string>();
            bool alli_win = false;
            bool enemy_win = true;
            foreach (Player p in room.Players)
            {
                if (p.GetRoleEnum() == Player.PlayerRole.Lord && !p.Alive && p.Camp == Game3v3Camp.S_CAMP_WARM)
                {
                    alli_win = true;
                    break;
                }

                if (p.Alive && p.Camp == Game3v3Camp.S_CAMP_COOL)
                    enemy_win = false;
            }

            if (alli_win)
            {
                foreach (Player p in room.Players)
                    if (p.Camp == Game3v3Camp.S_CAMP_COOL)
                        winners.Add(p.Name);
            }
            else if (enemy_win)
            {
                foreach (Player p in room.Players)
                    if (p.Camp == Game3v3Camp.S_CAMP_WARM)
                        winners.Add(p.Name);
            }

            return string.Join("+", winners);
        }

        public override void CheckBigKingdoms(Room room)
        {
        }
        protected override void OnBuryVictim(Room room, Player player, ref object data)
        {
            room.BuryPlayer(player);
            if (player.Camp == Game3v3Camp.S_CAMP_COOL && data is DeathStruct death && (death.Damage.From == null || death.Damage.From.Camp != player.Camp))
            {
                foreach (Player p in room.Players)
                {
                    if (p != player && p.Alive && p.Camp == player.Camp)
                    {
                        room.DrawCards(p, 3, "gamerule");
                        break;
                    }
                }
            }
        }
        protected override void OnDeath(Room room, Player player, ref object data)
        {
        }
    }

    public class HuaxiongHp : TriggerSkill
    {
        public HuaxiongHp() : base("huaxiong_hp_lose")
        {
            frequency = Frequency.Compulsory;
            events.Add(TriggerEvent.EventPhaseStart);
            skill_type = SkillType.Wizzard;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player.Phase == Player.PlayerPhase.RoundStart && player.General1 == "huaxiong_al")
                return new TriggerStruct(Name, player);
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<Player> players = room.GetAlivePlayers();
            room.SortByActionOrder(ref players);
            foreach (Player p in players)
                if (p.Camp == Game3v3Camp.S_CAMP_WARM && p.Alive && p.Hp > 1)
                    room.LoseHp(p);

            return false;
        }
    }
}
