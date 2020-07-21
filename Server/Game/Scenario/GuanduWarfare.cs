using System;
using System.Collections.Generic;
using CommonClass;
using CommonClass.Game;
using CommonClassLibrary;
using SanguoshaServer.AI;
using SanguoshaServer.Game;
using static SanguoshaServer.Game.Skill;

namespace SanguoshaServer.Scenario
{
    public class GuanduWarfare : GameScenario
    {
        public GuanduWarfare()
        {
            mode_name = "GuanduWarfare";
            rule = new GuanduWarfareRule();
        }
        public override void Assign(Room room)
        {
            System.Threading.Thread.Sleep(1000);
            //先确定两边主公
            foreach (Player lord in room.Players)
            {
                if (lord.GetRoleEnum() == Player.PlayerRole.Lord)
                {
                    if (lord.Camp == Game3v3Camp.S_CAMP_COOL)
                    {
                        lord.General1 = lord.ActualGeneral1 = "caocao_jx";
                    }
                    else
                    {
                        lord.General1 = lord.ActualGeneral1 = "yuanshao";
                    }

                    General lord_gen = Engine.GetGeneral(lord.General1, room.Setting.GameMode);
                    lord.PlayerGender = lord_gen.GeneralGender;
                    lord.Kingdom = lord_gen.Kingdom;
                    lord.General1Showed = true;
                    room.BroadcastProperty(lord, "General1");
                    room.BroadcastProperty(lord, "PlayerGender");
                    room.NotifyProperty(room.GetClient(lord), lord, "ActualGeneral1");
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
                }
            }

            System.Threading.Thread.Sleep(1000);

            //为其余玩家分配武将
            List<string> generals = room.Generals, warms = new List<string>(), cools = new List<string>();
            generals.Remove("caocao_jx"); generals.Remove("yuanshao");
            foreach (string general in generals)
            {
                General gen = Engine.GetGeneral(general, room.Setting.GameMode);
                if (gen.Kingdom == "wei")
                    cools.Add(general);
                else
                    warms.Add(general);
            }
            Shuffle.shuffle(ref warms);
            Shuffle.shuffle(ref cools);

            Dictionary<Player, List<string>> options = new Dictionary<Player, List<string>>();
            foreach (Player player in room.Players)
            {
                if (player.GetRoleEnum() == Player.PlayerRole.Lord) continue;
                List<string> choices = new List<string>();

                if (player.Camp == Game3v3Camp.S_CAMP_COOL)
                {
                    choices.Add(cools[0]);
                    cools.RemoveAt(0);
                    choices.Add(cools[0]);
                    cools.RemoveAt(0);
                }
                else
                {
                    choices.Add(warms[0]);
                    warms.RemoveAt(0);
                    choices.Add(warms[0]);
                    warms.RemoveAt(0);
                }

                options.Add(player, choices);
            }

            //玩家选将
            List<Interactivity> receivers = new List<Interactivity>();
            List<Player> players = new List<Player>();
            foreach (Player player in options.Keys)
            {
                if (player.GetRoleEnum() == Player.PlayerRole.Lord) continue;
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

            //按武将强度排序
            List<string> prefer_cools = new List<string>{ "xunyou", "xunyu", "chengyu", "guojia", "liuye", "caoren", "guanyu_sp", "xuhuang_jx", "zhangliao_jx",
                "hanhaoshihuan", "yujin", "caohong", "manchong", "litong", "zangba" };
            List<string> prefer_warms = new List<string>{ "chunyuqiong", "xunchen", "shenpei", "liubei_gd", "chenlin_gd", "jvshou",  "xuyou", "zhanghe_gd", "gaolan",
                "guotupangji", "tianfeng", "yanliangwenchou", "eryuan", "xinpi_gd", "erlv"  };

            //给AI和超时的玩家自动选择武将
            foreach (Player player in options.Keys)
            {
                player.RemoveTag("generals");
                if (string.IsNullOrEmpty(player.General1))
                {
                    string generalName = string.Empty;
                    List<string> reply = room.GetInteractivity(player)?.ClientReply;
                    bool success = true;
                    if (reply == null || reply.Count == 0 || string.IsNullOrEmpty(reply[0]))
                        success = false;
                    else
                        generalName = reply[0];

                    if (!success || (!options[player].Contains(generalName) && room.GetClient(player).UserRight < 3)
                        || (player.Camp == Game3v3Camp.S_CAMP_COOL && Engine.GetGeneral(generalName, room.Setting.GameMode).Kingdom != "wei")
                        || (player.Camp == Game3v3Camp.S_CAMP_WARM && Engine.GetGeneral(generalName, room.Setting.GameMode).Kingdom != "qun"))
                    {
                        if (player.Status == "bot")
                        {
                            List<string> prefers;
                            if (player.Camp == Game3v3Camp.S_CAMP_COOL)
                                prefers = prefer_cools;
                            else
                                prefers = prefer_warms;

                            options[player].Sort((x, y) => { return prefers.IndexOf(x) < prefers.IndexOf(y) ? -1 : 1; });
                        }
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
        }

        public override TrustedAI GetAI(Room room, Player player)
        {
            return new GuanduAI(room, player);
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
            //该模式下玩家完成选将会立即先所有人公布
            Player player = room.GetPlayers(client.ClientId)[0];
            List<string> options = JsonUntity.Json2List<string>((string)player.GetTag("generals"));
            List<string> reply = client.ClientReply;
            bool success = true;
            string generalName = string.Empty;
            if (reply == null || reply.Count == 0 || string.IsNullOrEmpty(reply[0]))
                success = false;
            else
                generalName = reply[0];

            if (!success || (!options.Contains(generalName) && room.GetClient(player).UserRight < 3)
                || (player.Camp == Game3v3Camp.S_CAMP_COOL && Engine.GetGeneral(generalName, room.Setting.GameMode).Kingdom != "wei")
                || (player.Camp == Game3v3Camp.S_CAMP_WARM && Engine.GetGeneral(generalName, room.Setting.GameMode).Kingdom != "qun"))
            {
                generalName = options[0];
            }

            player.General1 = generalName;
            player.ActualGeneral1 = generalName;
            player.Kingdom = Engine.GetGeneral(generalName, room.Setting.GameMode).Kingdom;
            player.General1Showed = true;
            room.BroadcastProperty(player, "General1");
            room.BroadcastProperty(player, "Kingdom");
        }

        public override void PrepareForPlayers(Room room)
        {
            //非主公玩家添加技能
            //调整血量
            foreach (Player player in room.Players)
            {
                string general1_name = player.ActualGeneral1;
                if (player.GetRoleEnum() != Player.PlayerRole.Lord)
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

                int max_hp = Engine.GetGeneral(general1_name, room.Setting.GameMode).DoubleMaxHp + (player.GetRoleEnum() == Player.PlayerRole.Lord ? 1 : 0);
                player.MaxHp = max_hp;
                player.Hp = player.MaxHp;

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

            //首先添加4位AI，不需要用到client
            for (int i = 4; i < 8; i++)
                room_players.Add(new Player { Name = string.Format("SGS{0}", i + 1), Seat = i + 1 });

            //根据随机座次
            List<List<Game3v3Camp>> camps = new List<List<Game3v3Camp>>
            {
                new List<Game3v3Camp>
                {
                    Game3v3Camp.S_CAMP_COOL, Game3v3Camp.S_CAMP_WARM, Game3v3Camp.S_CAMP_WARM, Game3v3Camp.S_CAMP_COOL,
                    Game3v3Camp.S_CAMP_COOL, Game3v3Camp.S_CAMP_WARM, Game3v3Camp.S_CAMP_WARM, Game3v3Camp.S_CAMP_COOL
                },
                new List<Game3v3Camp>
                {
                    Game3v3Camp.S_CAMP_COOL, Game3v3Camp.S_CAMP_WARM, Game3v3Camp.S_CAMP_COOL, Game3v3Camp.S_CAMP_COOL,
                    Game3v3Camp.S_CAMP_WARM, Game3v3Camp.S_CAMP_WARM, Game3v3Camp.S_CAMP_COOL, Game3v3Camp.S_CAMP_WARM
                },
                new List<Game3v3Camp>
                {
                    Game3v3Camp.S_CAMP_COOL, Game3v3Camp.S_CAMP_WARM, Game3v3Camp.S_CAMP_COOL, Game3v3Camp.S_CAMP_WARM,
                    Game3v3Camp.S_CAMP_COOL, Game3v3Camp.S_CAMP_WARM, Game3v3Camp.S_CAMP_COOL, Game3v3Camp.S_CAMP_WARM
                },
                new List<Game3v3Camp>
                {
                    Game3v3Camp.S_CAMP_COOL, Game3v3Camp.S_CAMP_WARM, Game3v3Camp.S_CAMP_WARM, Game3v3Camp.S_CAMP_COOL,
                    Game3v3Camp.S_CAMP_WARM, Game3v3Camp.S_CAMP_COOL, Game3v3Camp.S_CAMP_WARM, Game3v3Camp.S_CAMP_COOL
                },
            };

            Random rd = new Random();
            int index = rd.Next(0, 4);
            List<Game3v3Camp> games = camps[index], choose = new List<Game3v3Camp> { Game3v3Camp.S_CAMP_COOL, Game3v3Camp.S_CAMP_WARM };
            Game3v3Camp player_camp = choose[rd.Next(2)];
            List<Client> clients = new List<Client>(room.Clients);
            Shuffle.shuffle(ref clients);
            int player_index = 0;
            int computer_index = 1;
            bool warm_lord = false;
            for (int i = 0; i < room_players.Count; i++)
            {
                Player player = room_players[i];
                player.Camp = games[i];
                if (player.Camp == player_camp)
                {
                    Client client = clients[player_index];
                    player_index++;
                    if (client.UserID > 0)
                        client.Status = Client.GameStatus.online;
                    player.SceenName = client.Profile.NickName;
                    player.Status = client.Status.ToString();
                    player.ClientId = client.UserID;
                }
                else
                {
                    player.SceenName = string.Format("computer{0}", computer_index);
                    computer_index++;
                    player.Status = "bot";
                    player.ClientId = 0;
                }

                if (i == 0)
                {
                    player.Role = "lord";
                    player.Next = room_players[i + 1].Name;
                }
                else if (i == 7)
                    player.Next = room_players[0].Name;
                else
                    player.Next = room_players[i + 1].Name;

                if (player.Camp == Game3v3Camp.S_CAMP_WARM && !warm_lord)
                {
                    player.Role = "lord";
                    warm_lord = true;
                }

                if (player.GetRoleEnum() != Player.PlayerRole.Lord)
                    player.Role = "loyalist";

                room.BroadcastProperty(player, "Camp");
                room.BroadcastProperty(player, "Role");
            }
        }


        public override string GetPreWinner(Room room, Client client)
        {
            Player surrender = room.GetPlayers(client.UserID)[0];
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
            List<Interactivity> clients = new List<Interactivity>();
            int cool = 0, warm = 0;
            Interactivity cool_lord = null;
            Interactivity warm_lord = null;
            foreach (Player p in room.GetAlivePlayers())
            {
                if (p.Camp == Game3v3Camp.S_CAMP_COOL)
                {
                    if (p.GetRoleEnum() == Player.PlayerRole.Lord && p.ClientId > 0)
                        cool_lord = room.GetInteractivity(p.ClientId);
                    cool++;
                }
                else
                {
                    if (p.GetRoleEnum() == Player.PlayerRole.Lord && p.ClientId > 0)
                        warm_lord = room.GetInteractivity(p.ClientId);
                    warm++;
                }
            }

            if (warm == 1 && warm_lord != null)
                clients.Add(warm_lord);
            if (cool == 1 && cool_lord != null)
                clients.Add(cool_lord);

            return clients;
        }
    }

    public class GuanduWarfareRule : GameRule
    {
        public GuanduWarfareRule() : base("guandu_rule")
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
                room.DrawCards(p, 4, "gamerule");

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
            List<Player> players = room.GetAlivePlayers();
            Game3v3Camp dead_camp = Game3v3Camp.S_CAMP_NONE;
            foreach (Player p in room.Players)
            {
                if (p.GetRoleEnum() == Player.PlayerRole.Lord && !p.Alive)
                {
                    dead_camp = p.Camp;
                    break;
                }
            }

            if (dead_camp == Game3v3Camp.S_CAMP_WARM)
            {
                foreach (Player p in room.Players)
                    if (p.Camp == Game3v3Camp.S_CAMP_COOL)
                        winners.Add(p.Name);
            }
            else if (dead_camp == Game3v3Camp.S_CAMP_COOL)
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
            DeathStruct death = (DeathStruct)data;
            Player killer = death.Damage.From;

            //杀死敌对阵营获得其手牌
            if (killer != null && killer.Alive && killer.Camp != player.Camp && !player.IsKongcheng())
            {
                List<int> ids = player.GetCards("h");
                room.ObtainCard(killer, ref ids, new CardMoveReason(MoveReason.S_REASON_EXTRACTION, killer.Name));
            }

            room.BuryPlayer(player);

            if (room.ContainsTag("SkipNormalDeathProcess") && (bool)room.GetTag("SkipNormalDeathProcess"))
                return;

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
        }
        protected override void OnDeath(Room room, Player player, ref object data)
        {
        }

        protected override void RewardAndPunish(Room room, Player killer, Player victim)
        {
            //杀死同阵营惩罚
            if (killer.Alive && victim.Camp == killer.Camp)
                room.ThrowAllHandCardsAndEquips(killer);
        }
    }
}
