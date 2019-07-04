using System;
using System.Collections.Generic;
using System.Data;
using CommonClass;
using CommonClass.Game;
using CommonClassLibrary;
using SanguoshaServer.Game;
using SanguoshaServer.Package;
using static CommonClass.Game.Player;
using CommandType = CommonClassLibrary.CommandType;

namespace SanguoshaServer.Scenario
{
    public class Hegemony : GameScenario
    {
        public Hegemony()
        {
            mode_name = "Hegemony";
            rule = new HegemonyRule();
        }
        public override void Assign(Room room)
        {
            AssignGeneralsForPlayers(room, out Dictionary <Player, List<string> > options);

            List<Client> receivers = new List<Client>();
            foreach (Player player in options.Keys) {
                List<string> args = new List<string>
                {
                    player.Name,
                    string.Empty,
                    JsonUntity.Object2Json(options[player]),
                    false.ToString(),
                    true.ToString(),
                    false.ToString()
                };
                Client client = room.GetClient(player);
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

            foreach (Player player in options.Keys) {
                if (!string.IsNullOrEmpty(player.General1)) continue;
                bool success = true;
                List<string> reply = room.GetClient(player).ClientReply;
                if (!room.GetClient(player).IsClientResponseReady || reply == null || reply.Count == 0 || string.IsNullOrEmpty(reply[0]))
                    success = false;
                else
                {
                    string generalName = reply[0];
                    string[] generals = generalName.Split('+');
                    if (generals.Length != 2 || (!options[player].Contains(Engine.GetMainGeneral(generals[0])) && room.GetClient(player).UserRight < 3)
                        || (!options[player].Contains(Engine.GetMainGeneral(generals[1])) && room.GetClient(player).UserRight < 3)
                        || !SetPlayerGeneral(room, player, generals[0], true)
                        || !SetPlayerGeneral(room, player, generals[1], false))
                    {
                        success = false;
                    }
                }
                if (!success)
                {
                    List<string> default_generals = GeneralSelctor.GeInstance().SelectGenerals(room, options[player]);
                    SetPlayerGeneral(room, player, default_generals[0], true);
                    SetPlayerGeneral(room, player, default_generals[1], false);
                }
            }

            foreach (Player player in players) {
                List<string> names = new List<string>(); 
                if (!string.IsNullOrEmpty(player.General1))
                {
                    string name = player.General1;
                    player.Kingdom = Engine.GetGeneral(player.General1).Kingdom;
                    string role = Engine.GetMappedRole(player.Kingdom);
                    if (string.IsNullOrEmpty(role))
                        role = Engine.GetGeneral(player.General1).Kingdom;
                    names.Add(name);
                    player.ActualGeneral1 = name;
                    player.Role = role;
                    player.General1 = "anjiang";
                    foreach (Client p in room.Clients)
                    {
                        if (p != room.GetClient(player))
                            room.NotifyProperty(p, player, "Kingdom", "god");
                    }
                    room.BroadcastProperty(player, "General1");
                    room.NotifyProperty(room.GetClient(player), player, "ActualGeneral1");
                    room.NotifyProperty(room.GetClient(player), player, "Kingdom");
                }
                if (!string.IsNullOrEmpty(player.General2))
                {
                    string name = player.General2;
                    names.Add(name);
                    player.ActualGeneral2 = name;
                    player.General2 = "anjiang";
                    room.BroadcastProperty(player, "General2");
                    room.NotifyProperty(room.GetClient(player), player, "ActualGeneral2");
                }
                room.SetTag(player.Name, names);
                
                room.HandleUsedGeneral(names[0]);
                room.HandleUsedGeneral(names[1]);
                // setup AI
                //AI* ai = cloneAI(player);
                //ais << ai;
                //player->setAI(ai);
            }

            //君主转换
            if (room.Setting.LordConvert)
                room.AskForLordConvert();

            foreach (Player player in players)
            {
                General general1 = Engine.GetGeneral(player.ActualGeneral1);
                General general2 = Engine.GetGeneral(player.ActualGeneral2);

                if (general1.CompanionWith(player.ActualGeneral2))
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
            General general = Engine.GetGeneral(generalName);
            if (general == null)
            {
                return false;
            }
            //自由选将判断
            //else if (!Config.FreeChoose && !player->getSelected().contains(Sanguosha->getMainGenerals(generalName)))
            //    return false;

            if (isFirst)
            {
                player.General1 = general.Name;
            }
            else if (general.Kingdom != Engine.GetGeneral(player.General1).Kingdom)
            {
                return false;
            }
            else
            {
                player.General2 = general.Name;
            }
            return true;
        }

        private void AssignGeneralsForPlayers(Room room, out Dictionary<Player, List<string>> options)
        {
            options = new Dictionary<Player, List<string>>();

            int max_choice = room.Setting.GeneralCount;
            List<string> generals = room.Generals;
            if (generals.Count < max_choice * room.Players.Count)
                max_choice = generals.Count / room.Players.Count;

            foreach (Player player in room.Players)
            {
                List<string> choices = new List<string>();
                for (int i = 0; i < max_choice; i++)
                {
                    Shuffle.shuffle<string>(ref generals);
                    choices.Add(generals[0]);
                    generals.RemoveAt(0);
                }
                options.Add(player, choices);
            }
        }

        public override List<string> GetWinners(Room room)
        {
            throw new NotImplementedException();
        }

        public override void PrepareForStart(Room room, ref List<Player> room_players, ref List<int> game_cards, ref List<int> m_drawPile)
        {
            if (room.Setting.LordConvert)
            {
                foreach (int id in Engine.GetGameCards(room.Setting.CardPackage))
                    if (Engine.GetRealCard(id).Name != "DoubleSword"
                        && Engine.GetRealCard(id).Name != "JingFan"
                        && Engine.GetRealCard(id).Name != "SixSwords"
                        && Engine.GetRealCard(id).Name != "Zhuahuangfeidian")
                        game_cards.Add(id);
            }
            else
                game_cards = Engine.GetGameCards(room.Setting.CardPackage);
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

                player.ClientId = client.UserID;
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

            if (player.Role == "careerist" || other.Role == "careerist")
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
                if (WillbeRole(room, player, show_skill) == "careerist") return false;
                string kingdom = Engine.GetGeneral(player.ActualGeneral1).Kingdom;
                if (Engine.GetGeneral(player.ActualGeneral1).IsLord(true) && kingdom == other.Kingdom) return true;
                if (other.Role == "careerist") return false;
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
            if (Engine.GetGeneral(judger.ActualGeneral1).IsLord(true))
                return judger;

            foreach (Player p in room.Players)
            {
                if (p == judger || (!include_death && !p.Alive)) continue;
                if (p.General1Showed && p.Kingdom == Engine.GetGeneral(judger.ActualGeneral1).Kingdom && Engine.GetGeneral(p.General1).IsLord(true))
                    return p;
            }

            return null;
        }
    }

    public class GeneralSelctor
    {
        private static GeneralSelctor selctor;

        public static GeneralSelctor GeInstance()
        {
            if (selctor == null)
                selctor = new GeneralSelctor();

            return selctor;
        }
        
        private DataTable pair_value;
        public GeneralSelctor()
        {
            string sql = "select * from ai_pair_value";
            pair_value = DB.GetData(sql, false);
        }

        public List<string> SelectGenerals(Room room, List<string> candidates, bool assign_kingdom = false)
        {
            List<string> generals = candidates;
                string kingdom = null;
            if (assign_kingdom) {
                foreach (string name in candidates) {
                    if (string.IsNullOrEmpty(kingdom))
                        kingdom = Engine.GetGeneral(name).Kingdom;
                    else if (kingdom != Engine.GetGeneral(name).Kingdom) {
                        kingdom = string.Empty;
                        break;
                    }
                }
            }

            foreach (string name in candidates) {
                List<string> subs = Engine.GetConverPairs(name);
                if (subs.Count != 0) {
                    generals.Remove(name);
                    subs.Add(name);
                    Shuffle.shuffle<string>(ref subs);
                    foreach (string general in subs) {
                        if (string.IsNullOrEmpty(kingdom) || Engine.GetGeneral(general).Kingdom == kingdom) {
                            generals.Add(general);
                            break;
                        }
                    }
                }
            }
            Dictionary<string, double> points = CalculatePairValues(room, generals);

            double max_score = 0;
            string best_pair = null;

            foreach (string key in points.Keys)
            {
                if (points[key] > max_score)
                {
                    max_score = points[key];
                    best_pair = key;
                }
            }

            return new List<string>(best_pair.Split('+'));
        }

        private Dictionary<string, double> CalculatePairValues(Room room, List<string> _candidates)
        {
            Dictionary<string, double> points = new Dictionary<string, double>();
            foreach (string first in _candidates) {
                Dictionary<string, double> result = CalculateDeputyValue(room, first, _candidates);
                foreach (string key in result.Keys)
                    points[key] = result[key];
            }

            return points;
        }

        private Dictionary<string, double> points = new Dictionary<string, double>();
        private Dictionary<string, double> CalculateDeputyValue(Room room, string first, List<string> _candidates)
        {
            List<string> candidates = _candidates;
            Dictionary<string, double> points = new Dictionary<string, double>();
            foreach (string second in candidates) {
                if (this.points.ContainsKey(string.Format("{0}+{1}", first, second)))
                {
                    points[string.Format("{0}+{1}", first, second)] = this.points[string.Format("{0}+{1}", first, second)];
                    continue;
                }

                if (first == second || Engine.GetGeneral(first).Kingdom != Engine.GetGeneral(second).Kingdom) continue;
                DataRow[] rows1 = pair_value.Select(string.Format("general1 = '{0}' and general2 = '{1}'", first, second));
                if (rows1.Length > 0)
                {
                    this.points[string.Format("{0}+{1}", first, second)] = int.Parse(rows1[0]["value1"].ToString());
                    points[string.Format("{0}+{1}", first, second)] = int.Parse(rows1[0]["value1"].ToString());
                }
                else {
                    General general1 = Engine.GetGeneral(first);
                    General general2 = Engine.GetGeneral(second);
                    double general2_value = Engine.GetGeneralValue(second, "Hegemony");
                    double v = Engine.GetGeneralValue(first, "Hegemony") + general2_value;

                    int max_hp = general1.GetMaxHpHead() + general2.GetMaxHpDeputy();
                    if (max_hp % 2 > 0) v -= 0.5;
                    if (max_hp >= 8) v += 1;

                    if (room.Setting.LordConvert) {
                        string lord = "lord_" + first;
                        General lord_general = Engine.GetGeneral(lord);
                        if (lord_general != null)
                            v += 5;
                    }

                    if (general1.CompanionWith(second)) v += 3;

                    if (general1.IsFemale()) {
                        if ("wu" == general1.Kingdom && !general1.HasSkill("jieying", "Hegemony", true))
                            v -= 1;
                        else if (general1.Kingdom != "qun")
                            v += 0.5;
                    } else if ("qun" == general1.Kingdom)
                        v += 0.5;

                    if (general1.HasSkill("baoling", "Hegemony", true) && general2_value > 6) v -= 5;

                    if (max_hp < 8) {
                        List<string> need_high_max_hp_skills = new List<string> { "zhiheng", "zaiqi", "yinghun", "kurou" };
                        foreach (string skill in need_high_max_hp_skills) {
                            if (Engine.GetGeneralSkills(first, "Hegemony", true).Contains(skill) || Engine.GetGeneralSkills(second, "Hegemony", false).Contains(skill))
                                v -= 5;
                        }
                    }
                    this.points.Add(string.Format("{0}+{1}", first, second), v);
                    points.Add(string.Format("{0}+{1}", first, second), v);
                }
            }
            return points;
        }
    }


    public class GameRule_AskForGeneralShowHead : TriggerSkill
    {
        public GameRule_AskForGeneralShowHead() : base("GameRule_AskForGeneralShowHead")
        {
            events.Add(TriggerEvent.EventPhaseStart);
            global = true;
        }

        public override TriggerStruct Cost(TriggerEvent trigger_event, Room room, Player player, ref object data, Player target, TriggerStruct trigger_struct)
        {
            room.ShowGeneral(player, true, true);
            return new TriggerStruct();
        }

        public override bool Triggerable(Player player, Room room)
        {
            return player.Phase == PlayerPhase.Start && !player.General1Showed && player.DisableShowList(true).Count == 0;
        }
    }

    public class GameRule_AskForGeneralShowDeputy : TriggerSkill
    {
        public GameRule_AskForGeneralShowDeputy() : base("GameRule_AskForGeneralShowDeputy")
        {
            events.Add(TriggerEvent.EventPhaseStart);
            global = true;
        }

        public override TriggerStruct Cost(TriggerEvent trigger_event, Room room, Player player, ref object data, Player target, TriggerStruct trigger_struct)
        {
            room.ShowGeneral(player, false, true);
            return new TriggerStruct();
        }

        public override bool Triggerable(Player player, Room room)
        {
            return player.Phase == PlayerPhase.Start && !string.IsNullOrEmpty(player.General2)
                && !player.General2Showed && player.DisableShowList(false).Count == 0;
        }
    }

    public class GameRule_AskForArraySummon : TriggerSkill
    {
        public GameRule_AskForArraySummon() : base("GameRule_AskForArraySummon")
        {
            events.Add(TriggerEvent.EventPhaseStart);
            global = true;
        }

        public override TriggerStruct Cost(TriggerEvent trigger_event, Room room, Player player, ref object data, Player target, TriggerStruct trigger_struct)
        {
            foreach (string skill in player.GetSkills(true, false))
            {
                Skill real_skill = Engine.GetSkill(skill);
                if (real_skill != null && real_skill is BattleArraySkill baskill && room.AskForSkillInvoke(player, Name))
                {
                    room.ShowGeneral(player, RoomLogic.InPlayerHeadSkills(player, skill));
                    baskill.SummonFriends(room, player);
                    break;
                }
            }
            return new TriggerStruct();
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> result = new List<TriggerStruct>();
            if (player.Phase == PlayerPhase.Start && room.AliveCount() >= 4)
            {
                foreach (string skill in player.GetHeadSkillList(true, false))
                {
                    Skill real_skill = Engine.GetSkill(skill);
                    if (real_skill != null && real_skill is BattleArraySkill baskill)
                    {
                        if (baskill.ViewAsSkill.IsEnabledAtPlay(room, player))
                        {
                            TriggerStruct trigger = new TriggerStruct(Name, player)
                            {
                                SkillPosition = "head"
                            };
                            result.Add(trigger);
                        }
                    }
                }
                foreach (string skill in player.GetDeputySkillList(true, false))
                {
                    Skill real_skill = Engine.GetSkill(skill);
                    if (real_skill != null && real_skill is BattleArraySkill baskill)
                    {
                        if (baskill.ViewAsSkill.IsEnabledAtPlay(room, player))
                        {
                            TriggerStruct trigger = new TriggerStruct(Name, player)
                            {
                                SkillPosition = "deputy"
                            };
                            result.Add(trigger);
                        }
                    }
                }
            }

            return result;
        }
    }


    public class HegemonyRule : GameRule
    {
        public HegemonyRule() : base("hegemony_rule")
        {
        }

        public override string GetWinner(Room room)
        {
            List<string> winners = new List<string>();
                List<Player> players = room.GetAlivePlayers();
                Player win_player = players[0];
                if (players.Count == 1)
                {
                    if (!win_player.General1Showed)
                        room.ShowGeneral(win_player, true, false, false);
                    if (!win_player.General2Showed)
                        room.ShowGeneral(win_player, false, false, false);
                    foreach (Player p in room.Players) {
                        if (RoomLogic.IsFriendWith(room, win_player, p))
                            winners.Add(p.Name);
                    }
                }
                else
                {
                    bool has_diff_kingdoms = false;
                    string left_kingdom = null;
                    foreach (Player p in players) {
                        left_kingdom = Engine.GetGeneral(p.ActualGeneral1).Kingdom;
                        foreach (Player p2 in players)
                    {
                            if (p == p2) continue;
                            if (p.HasShownOneGeneral() && p2.HasShownOneGeneral() && !RoomLogic.IsFriendWith(room, p, p2))
                            {
                                has_diff_kingdoms = true;
                                break;// if both shown but not friend, hehe.
                            }
                            if ((p.HasShownOneGeneral() && !p2.HasShownOneGeneral() && !RoomLogic.WillBeFriendWith(room, p2, p))
                                || (!p.HasShownOneGeneral() && p2.HasShownOneGeneral() && !RoomLogic.WillBeFriendWith(room, p, p2)))
                            {
                                has_diff_kingdoms = true;
                                break;// if either shown but not friend, hehe.
                            }
                            if (!p.HasShownOneGeneral() && !p2.HasShownOneGeneral())
                            {
                                if (Engine.GetGeneral(p.ActualGeneral1).Kingdom != Engine.GetGeneral(p2.ActualGeneral1).Kingdom)
                                {
                                    has_diff_kingdoms = true;
                                    break;  // if neither shown and not friend, hehe.
                                }
                            }
                        }
                        if (has_diff_kingdoms)
                            break;
                    }

                    bool all_live_shown = true;
                    if (!has_diff_kingdoms)
                    {                      //check all shown before judging careerist, cos same skills could change kindom such as dragonphoenix
                        foreach (Player p in players) {
                            if (!p.HasShownOneGeneral())
                                all_live_shown = false;
                        }
                    }

                    if (!has_diff_kingdoms && !all_live_shown)
                    { // judge careerist
                        List<string> lords = new List<string>();
                        int all = 0;
                        foreach (Player p in room.Players) {
                            if (p.HasShownOneGeneral())
                            {
                                if (p.Kingdom == left_kingdom && p.Role != "careerist")
                                    all++;
                            }
                            else if (Engine.GetGeneral(p.ActualGeneral1).Kingdom == left_kingdom)
                                all++;
                        }

                        foreach (Player p in room.Players) {
                            if (Engine.GetGeneral(p.ActualGeneral1).IsLord(true))
                            {
                                if (p.Alive && p.General1Showed)                //the lord has shown
                                    lords.Add(Engine.GetGeneral(p.ActualGeneral1).Kingdom);
                                else if (!p.Alive && p.Kingdom == left_kingdom)   //the lord is dead, all careerist
                                    return null;
                                else if (p.Alive && !p.HasShownOneGeneral() && all > room.Players.Count / 2) //the lord not yet shown
                                    return null;
                            }
                        }
                        if (lords.Count == 0 && all > room.Players.Count / 2)       //careerist exists
                            has_diff_kingdoms = true;
                    }

                    if (has_diff_kingdoms) return null;    //if has enemy, hehe

                    // if run here, all are friend.
                    foreach (Player p in players) {
                        if (!p.General1Showed)
                            room.ShowGeneral(p, true, false, false); // dont trigger event
                        if (!p.General2Showed)
                            room.ShowGeneral(p, false, false, false);
                    }

                    foreach (Player p in room.Players) {
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
                if (killer.Role == "careerist")
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

        protected override void AddRuleSkill()
        {
            List<Skill> list = new List<Skill> { new GameRule_AskForGeneralShowHead(), new GameRule_AskForGeneralShowDeputy(), new GameRule_AskForArraySummon() };
            List<Skill> list_copy = new List<Skill>();
            foreach (Skill s in list)
            {
                if (Engine.GetSkill(s.Name) == null)
                {
                    list_copy.Add(s);
                }
            }
            Engine.AddSkills(list_copy);
        }
    }
}
