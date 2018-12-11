using CommonClass;
using CommonClass.Game;
using SanguoshaServer.Game;
using SanguoshaServer.Scenario;
using System;
using System.Collections.Generic;

namespace SanguoshaServer.AI
{

    public class SmartAI : TrustedAI
    {
        private List<string> kingdoms = new List<string> { "wei", "shu", "wu", "qun" };
        public SmartAI(Room room, Player player) : base(room, player)
        {
        }
        public override void Event(TriggerEvent triggerEvent, Player player, object data)
        {
            if (!self.Alive) return;

            base.Event(triggerEvent, player, data);

            if (player != null && player == self)
            {
                if (triggerEvent == TriggerEvent.GameStart)
                {
                    id_public[self] = self.HasShownOneGeneral() ? (self.Role == "careerist" ? "careerist" : self.Kingdom) : "unknown";
                    if (id_public[self] == "unknown")
                    {
                        foreach (string k in kingdoms)
                        {
                            player_intention_public[self].Add(k, 50);   // default 50
                        }
                    }

                    string role;
                    if (self.HasShownOneGeneral())
                        role = (self.Role == "careerist" ? "careerist" : self.Kingdom);
                    else
                        role = (Hegemony.WillbeRole(room, self) != "careerist" ? self.Kingdom : "careerist");
                    id_tendency[self] = role;
                    foreach (Player p in room.GetOtherPlayers(self))
                    {
                        string kingdom = (p.HasShownOneGeneral() ? (p.Role == "careerist" ? "careerist" : p.Kingdom) : "unknown");
                        id_tendency[p] = kingdom;
                        id_public[p] = kingdom;
                        players_hatred[p] = 0;
                        if (kingdom == "unknown")
                        {
                            foreach (string k in kingdoms)
                            {
                                player_intention[p].Add(k, 50);              // default 50
                                player_intention_public[p].Add(k, 50);
                            }
                        }
                    }
                }
                UpdatePlayers();

            }
            if (triggerEvent == TriggerEvent.EventPhaseStart || triggerEvent == TriggerEvent.RemoveStateChanged
                || triggerEvent == TriggerEvent.BuryVictim || triggerEvent == TriggerEvent.GeneralShown)
                UpdatePlayers();

            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move)
            {
                bool open = false;
                bool pile_open = false;
                Player from = move.From;
                Player to = move.To;

                foreach (Player p in room.AlivePlayers)
                {
                    if (p.HasFlag("Global_GongxinOperator") && (p == self || self.IsSameCamp(p)))
                    {
                        open = true;
                        break;
                    }
                }

                if ((from != null && (from == self || self.IsSameCamp(from))) || (to != null && (to == self || self.IsSameCamp(to)) && move.To_place != Player.Place.PlaceSpecial))
                    open = true;

                if (!open && to != null && !string.IsNullOrEmpty(move.To_pile_name) && !move.To_pile_name.StartsWith("#") && move.To != null)
                {
                    if (move.To.GetPileOpener(move.To_pile_name).Count == room.GetAllPlayers(true).Count)
                        pile_open = true;
                    else
                    {
                        foreach (string name in move.To.GetPileOpener(move.To_pile_name))
                        {
                            Player who = room.FindPlayer(name, true);
                            if (who != null && (who == self || self.IsSameCamp(who)))
                            {
                                open = true;
                                break;
                            }
                        }
                    }
                }

                if (move.To != null && move.To_place == Player.Place.PlaceHand)
                {
                    foreach (int id in move.Card_ids)
                    {
                        int index = move.Card_ids.IndexOf(id);
                        WrappedCard card = room.GetCard(id);
                        if (card.HasFlag("visible") || pile_open
                                || move.From_places[index] == Player.Place.PlaceEquip || move.From_places[index] == Player.Place.PlaceDelayedTrick
                                || move.From_places[index] == Player.Place.DiscardPile || move.From_places[index] == Player.Place.PlaceTable)
                        {
                            public_handcards[move.To].Add(id);
                            private_handcards[move.To].Add(id);
                        }
                        else if (open)
                        {
                            private_handcards[move.To].Add(id);
                        }
                    }
                }

                if (move.To != null && move.To_place == Player.Place.PlaceSpecial && move.To_pile_name == "wooden_ox")
                {
                    foreach (int id in move.Card_ids)
                    {
                        if (open)
                            wooden_cards[move.To].Add(id);
                    }
                }

                if (move.From != null && move.From_places.Contains(Player.Place.PlaceHand))
                {
                    foreach (int id in move.Card_ids)
                    {
                        if (room.GetCard(id).HasFlag("visible") || pile_open || move.To_place == Player.Place.PlaceEquip
                                || move.To_place == Player.Place.PlaceDelayedTrick || move.To_place == Player.Place.DiscardPile
                                || move.To_place == Player.Place.PlaceTable)
                        {
                            public_handcards[move.From].RemoveAll(t => t == id);
                            private_handcards[move.From].RemoveAll(t => t == id);
                        }
                        else
                        {
                            public_handcards[move.From].Clear();
                            if (open)
                                private_handcards[move.From].RemoveAll(t => t == id);
                            else
                                private_handcards[move.From].Clear();
                        }
                    }
                }

                if (move.From != null && move.From_places.Contains(Player.Place.PlaceSpecial) && move.From_pile_names.Contains("wooden_ox"))
                {
                    foreach (int id in move.Card_ids)
                    {
                        int index = move.Card_ids.IndexOf(id);
                        if (open && move.From_pile_names[index] == "wooden_ox" && move.From_places[index] == Player.Place.PlaceSpecial)
                            wooden_cards[move.From].RemoveAll(t => t == id);
                    }
                }

                foreach (int id in move.Card_ids)
                {
                    int index = move.Card_ids.IndexOf(id);
                    WrappedCard card = room.GetCard(id);
                    if (move.From_places[index] == Player.Place.DrawPile)
                    {
                        if (move.To != null && move.To_place == Player.Place.PlaceHand && card.HasFlag("visible2" + self.Name))
                            private_handcards[move.To].Add(id);

                        if (guanxing.Key != null && guanxing.Value.Contains(id))
                        {
                            if (guanxing.Value[0] != id)
                            {
                                List<int> top_cards = new List<int>(guanxing.Value);
                                for (int y = top_cards.IndexOf(id); y < top_cards.Count; y++)
                                    guanxing.Value.RemoveAt(y);
                            }
                            else
                                guanxing.Value.RemoveAll(t => t == id);
                            if (guanxing.Value.Count == 0) guanxing = new KeyValuePair<Player, List<int>>();
                        }
                    }
                }
            }

            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                string class_name = fcard.Name;
                if (fcard is Slash) class_name = "Slash";
                UseCard e = Engine.GetCardUsage(class_name);
                if (e != null)
                    e.OnEvent(this, triggerEvent, player, data);
            }

            if (triggerEvent == TriggerEvent.ChoiceMade)
            {
                string str = data.ToString();
                foreach (SkillEvent e in Engine.GetSkillEvents())
                    foreach (string key in e.Key)
                        if (str.Contains(key))
                            e.OnEvent(this, triggerEvent, player, data);

                foreach (UseCard e in Engine.GetCardUsages())
                    foreach (string key in e.Key)
                        if (str.Contains(key))
                            e.OnEvent(this, triggerEvent, player, data);

                List<string> choices = new List<string>(str.Split(':'));
                if (choices[0] == "viewCards")
                {
                    List<int> ids = new List<int>(player.HandCards);

                    if (choices[choices.Count - 1] == "all")
                    {
                        public_handcards[player] = ids;
                        private_handcards[player] = ids;
                    }
                    else if (choices[choices.Count - 1] == self.Name)
                        private_handcards[player] = ids;
                }
                else if (choices[1] == "showCards")
                {
                    int id = int.Parse(choices[1]);
                    if (choices[choices.Count - 1] == "all")
                    {
                        if (!public_handcards[player].Contains(id)) public_handcards[player].Add(id);
                        if (!private_handcards[player].Contains(id)) private_handcards[player].Add(id);
                    }
                    else if (choices[choices.Count - 1] == self.Name && !private_handcards[player].Contains(id))
                        private_handcards[player].Add(id);
                }
                else if (choices[0] == "cardShow")
                {
                    int id = int.Parse(choices[choices.Count - 1].Substring(1, choices[choices.Count - 1].Length - 1));
                    if (!public_handcards[player].Contains(id)) public_handcards[player].Add(id);
                    if (!private_handcards[player].Contains(id)) private_handcards[player].Add(id);
                }
                else if (choices[0] == "ViewTopCards" || choices[0] == "ViewBottomCards")
                {
                    bool open = choices[choices.Count - 1] == "open";
                    List<int> drawpile = room.DrawPile;
                    List<int> moves = JsonUntity.StringList2IntList(new List<string>(choices[2].Split('+')));
                    if (choices[0] == "ViewTopCards")
                    {
                        guanxing = new KeyValuePair<Player, List<int>>();
                        guanxing_dts.Clear();
                        if (open)
                        {
                            for (int index = 0; index < moves.Count; index++)
                            {
                                int id = drawpile[index];
                                room.SetCardFlag(id, "visible");
                            }
                        }
                        else
                        {
                            foreach (int id in moves)
                                if (player == self || player.IsSameCamp(self))
                                    room.SetCardFlag(id, "visible2" + self.Name);

                            guanxing = new KeyValuePair<Player, List<int>>(player, moves);
                            Player current = room.Current;
                            if (current != null)
                            {
                                if (current.Phase < Player.PlayerPhase.Judge)
                                    foreach (int id in current.JudgingArea)
                                        guanxing_dts.Add(room.GetCard(id));
                                Player next = null;
                                int next_num = 1;
                                while (next == null)
                                {
                                    Player p = room.GetNextAlive(current, next_num);
                                    next_num++;
                                    if (p.FaceUp)
                                        next = p;
                                }
                                if (next != null && (next != current || current.Phase > Player.PlayerPhase.Judge))
                                    foreach (int dt in next.JudgingArea)
                                        guanxing_dts.Add(room.GetCard(dt));
                            }
                        }
                    }
                    else
                    {
                        if (open)
                        {
                            for (int index = drawpile.Count - 1; index >= drawpile.Count - moves.Count; index--)
                            {
                                int id = drawpile[index];
                                room.SetCardFlag(id, "visible");
                            }
                        }
                        else
                        {
                            foreach (int id in moves)
                                room.SetCardFlag(id, "visible2" + choices[1]);
                        }
                    }
                }
            }

            FilterEvent(triggerEvent, player, data);
        }

        private void FilterEvent(TriggerEvent triggerEvent, Player player, object data)
        {
        }

        //评估玩家强度
        public double EvaluatePlayerStrength(Player who)
        {
            double point = 0;
            List<string> skills = GetKnownSkills(who, true, self, true);
            foreach (string skill in skills)
            {
                SkillEvent e = Engine.GetSkillEvent(skill);
                point += Engine.GetSkillValue(skill) + (e != null ? e.GetSkillAdjustValue(this, who) : 0);
            }

            Dictionary<string, double> pairs = Engine.GetSkillPairAdjust();
            List<string> count = new List<string>();
            foreach (string skill1 in skills)
            {
                foreach (string skill2 in skills)
                {
                    if (skill1 == skill2) continue;
                    string pair = skill1 + "+" + skill2;
                    if (pairs.ContainsKey(pair) && !count.Contains(pair))
                    {
                        point += pairs[pair];
                        count.Add(pair);
                    }
                }
            }

            if (!who.General1Showed && !IsKnown(self, who, "h"))
                point += 4;
            if (!string.IsNullOrEmpty(who.General2) && !who.General2Showed && !IsKnown(self, who, "d"))
                point += 4;

            point += (who.MaxHp - 3) * 4;
            //todo: match player equip & handcard
            point += GetDefensePoint(who);

            // emit room->room_message(QString("%1 judge %2:%3,%4 's stength point is %5")
            //     .arg(self->objectName()).arg(who->objectName()).arg(who->getGeneralName()).arg(who->getGeneral2() ? who->getGeneral2Name() : QString()).arg(point));
            return point;
        }
        
        public void CompareByLevel(ref List<Player> players)
        {
            List<Player> alives = room.GetAllPlayers();
            players.Sort((x, y) =>
            {
                if (players_level[x] > players_level[y])
                    return -1;
                else
                    return alives.IndexOf(x) < alives.IndexOf(y) ? -1 : 1;
            });
        }

        public void UpdatePlayers()
        {
            friends.Clear();
            enemies.Clear();
            priority_enemies.Clear();
            friends_noself.Clear();

            CountPlayers();
            UpdateGameProcess();
            UpdatePlayerLevel();

            foreach (Player p in room.AlivePlayers)
            {
                friends[p] = new List<Player>();
                enemies[p] = new List<Player>();
            }
            foreach (Player p in room.AlivePlayers)
            {
                if (players_level[p] < 0)
                    friends[self].Add(p);
                else if (players_level[p] > 0)
                {
                    enemies[self].Add(p);
                    if (players_level[p] > 3)
                        priority_enemies.Add(p);
                }
            }
            /*
            AI *ai = self->getSmartAI();
            auto lambda = [&ai](ServerPlayer *p1, ServerPlayer *p2) {
                ai->getEnemies(p1);
                Room *room = p1->getRoom();
                return room->getAlivePlayers().indexOf(p1) < room->getAlivePlayers().indexOf(p2);
            };

            qSort(priority_enemies.begin(), priority_enemies.end(), lambda);
            */
            CompareByLevel(ref priority_enemies);
            List<Player> players = enemies[self];
            CompareByLevel(ref players);

            friends_noself = friends[self];
            friends[self].RemoveAll(t => t == self);

            string most = process_public.Split('>')[0];
            foreach (Player p1 in room.GetOtherPlayers(self))
            {
                friends[p1].Add(p1);
                string id1 = id_tendency[p1];
                if (id1 != "unknown")
                {
                    foreach (Player p2 in room.GetOtherPlayers(p1))
                    {
                        string id2 = ((IsKnown(self, p2) && IsKnown(p1, p2)) ? id_tendency[p2] : id_public[p2]);
                        if (id1 != "careerist" && id1 == id2)
                            friends[p1].Add(p2);
                        else
                        {
                            if (id2 == "unknown")
                            {
                                if (same_kingdom[p1].Contains(p2))
                                    friends[p1].Add(p2);
                                else
                                {
                                    if (different_kingdom[p1].Contains(p2))
                                    {
                                        if (process_public.Contains(">>>") && !(most == id1 || most == p1.Name)
                                            && !GetPublicPossibleId(p2).Contains(most))
                                            friends[p1].Add(p2);
                                        else
                                            enemies[p1].Add(p2);
                                    }
                                    else if (process_public.Contains(">>") && (most == id1 || most == p1.Name))
                                        enemies[p1].Add(p2);
                                }
                            }
                            else
                            {
                                if (process_public.Contains(">>>"))
                                {
                                    if (most == id_public[p1] || most == p1.Name || most == id_public[p2] || most == p2.Name)
                                        enemies[p1].Add(p2);
                                    else if (id2 != "unknown")
                                        friends[p1].Add(p2);
                                }
                                else
                                    enemies[p1].Add(p2);
                            }
                        }
                    }
                }
                else
                {
                    foreach (Player p2 in room.GetOtherPlayers(p1))
                    {
                        string id2 = ((IsKnown(self, p2) && IsKnown(p1, p2)) ? id_tendency[p2] : id_public[p2]);
                        if (same_kingdom[p1].Contains(p2))
                            friends[p1].Add(p2);
                        else if (most == id2 || most == p2.Name)
                            enemies[p1].Add(p2);
                        else if (process_public.Contains(">>>") && !GetPossibleId(p1).Contains(most)
                                   && (id2 != "unknown" || !GetPublicPossibleId(p2).Contains(most)))
                            friends[p1].Add(p2);
                        else if (different_kingdom[p1].Contains(p2))
                            enemies[p1].Add(p2);
                        else if (id2 != "unknown" && !GetPossibleId(p1).Contains(id2))
                        {
                            enemies[p1].Add(p2);
                        }
                    }
                }
            }
        }
        //判断局势
        private void UpdateGameProcess()
        {
            Dictionary<string, double> kingdom_value = new Dictionary<string, double>(), kingdom_value_public = new Dictionary<string, double>();
            Dictionary<string, List<Player>> kingdoms = new Dictionary<string, List<Player>>(), kingdoms_public = new Dictionary<string, List<Player>>(),
                kingdoms_count = new Dictionary<string, List<Player>>(), anjiang_count = new Dictionary<string, List<Player>>(), anjiang_count_public = new Dictionary<string, List<Player>>();
            List<Player> anjiangs = new List<Player>(), anjiangs_public = new List<Player>();
            Dictionary<string, int> kingdom_pits = new Dictionary<string, int>(), kingdom_pits_public = new Dictionary<string, int>();             // rest pits of each kingdom

            List<Player> players = room.GetAllPlayers(true);
            int all_num = players.Count;
            room.SortByActionOrder(ref players);

            foreach (Player p in players)
            {
                if (p.HasShownOneGeneral() && p.Role != "careerist")
                {
                    if (kingdoms_count.ContainsKey(p.Kingdom))
                        kingdoms_count[p.Kingdom].Add(p);
                    else
                        kingdoms_count[p.Kingdom] = new List<Player> { p };
                }
            }

            foreach (string kingdom in this.kingdoms)
            {
                kingdom_value.Add(kingdom, 0);
                kingdom_value_public.Add(kingdom, 0);
                if (lords.ContainsKey(kingdom) && lords[kingdom] != null)
                {
                    if (lords[kingdom].Alive)
                        kingdom_pits[kingdom] = 100;
                    else
                        kingdom_pits[kingdom] = 0;
                }
                else
                    kingdom_pits[kingdom] = Math.Max(all_num / 2 - (kingdoms_count.ContainsKey(kingdom) ? kingdoms_count[kingdom].Count : 0), 0);

                if (lords_public.ContainsKey(kingdom) && lords_public[kingdom] != null)
                {
                    if (lords_public[kingdom].Alive)
                        kingdom_pits_public[kingdom] = 100;
                    else
                        kingdom_pits_public[kingdom] = 0;
                }
                else
                    kingdom_pits_public[kingdom] = Math.Max(all_num / 2 - (kingdoms_count.ContainsKey(kingdom) ? kingdoms_count[kingdom].Count : 0), 0);
            }

            foreach (Player p in room.AlivePlayers)
            {                        // evaluate all identified players
                double value = EvaluatePlayerStrength(p);
                if (id_tendency[p] == "careerist")
                {
                    kingdom_value[p.Name] = value;                             // careerist counted by his name
                }
                else if (id_tendency[p] != "unknown")
                {
                    if (kingdoms.ContainsKey(id_tendency[p]))
                        kingdoms[id_tendency[p]].Add(p);
                    else
                        kingdoms[id_tendency[p]] = new List<Player> { p };
                    kingdom_value[id_tendency[p]] += value;
                }
                else
                    anjiangs.Add(p);

                if (id_public[p] == "careerist")
                {
                    kingdom_value_public[p.Name] = value;
                }
                else if (id_public[p] != "unknown")
                {
                    if (kingdoms_public.ContainsKey(id_public[p]))
                        kingdoms_public[id_public[p]].Add(p);
                    else
                        kingdoms[id_tendency[p]] = new List<Player> { p };
                    kingdom_value_public[id_public[p]] += value;
                }
                else
                    anjiangs_public.Add(p);
            }

            Dictionary<string, double> coop_skills = Engine.GetSkillCoopAdjust();        // ajust cooperation skills for each kingdom
            foreach (string kingdom in kingdoms.Keys)
            {
                List<string> skills = new List<string>();
                foreach (Player p in kingdoms[kingdom])
                {
                    skills.AddRange(GetKnownSkills(p, true, self, true));
                    if (Engine.GetGeneral(p.ActualGeneral1).IsLord())
                    {
                        if (kingdom != "shu")
                            kingdom_value[kingdom] += kingdoms[kingdom].Count * 1.5;
                        else
                            kingdom_value[kingdom] += kingdoms[kingdom].Count * 0.5;
                    }
                }

                foreach (string skill1 in skills)
                {
                    List<string> count = new List<string>();
                    foreach (string skill2 in skills)
                    {
                        if (skill1 == skill2) continue;
                        string coop = skill1 + "+" + skill2;
                        if (coop_skills.ContainsKey(coop) && !count.Contains(coop))
                        {
                            count.Add(coop);
                            kingdom_value[kingdom] += coop_skills[coop];
                        }
                    }
                }
            }

            foreach (string kingdom in kingdoms_public.Keys)
            {
                List<string> skills = new List<string>();
                foreach (Player p in kingdoms_public[kingdom])
                {
                    foreach (string skill in GetKnownSkills(p, true, self, true))
                    {
                        if (RoomLogic.PlayerHasShownSkill(room, p, skill))
                            skills.Add(skill);
                        if (Engine.GetGeneral(p.ActualGeneral1).IsLord())
                        {
                            if (kingdom != "shu")
                                kingdom_value_public[kingdom] += kingdoms_public[kingdom].Count * 1.5;
                            else
                                kingdom_value_public[kingdom] += kingdoms_public[kingdom].Count * 0.5;
                        }
                    }
                }

                foreach (string skill1 in skills)
                {
                    List<string> count = new List<string>();
                    foreach (string skill2 in skills)
                    {
                        if (skill1 == skill2) continue;
                        string coop = skill1 + "+" + skill2;
                        if (coop_skills.ContainsKey(coop) && !count.Contains(coop))
                        {
                            count.Add(coop);
                            kingdom_value_public[kingdom] += coop_skills[coop];
                        }
                    }
                }
            }

            foreach (Player p in anjiangs)
            {
                List<string> possible = GetPossibleId(p);
                if (possible[0] == "careerist")
                {
                    kingdom_value[p.Name] = EvaluatePlayerStrength(p);
                    continue;
                }
                foreach (string kingdom in possible)
                {
                    if (anjiang_count.ContainsKey(kingdom))
                        anjiang_count[kingdom].Add(p);
                    else
                        anjiang_count[kingdom] = new List<Player> { p };
                }
            }

            foreach (Player p in anjiangs)
            {
                List<string> possible = GetPossibleId(p);
                if (possible[0] == "careerist") continue;
                int point = 0;
                foreach (string kingdom in possible)
                {
                    if (kingdom_pits[kingdom] == 0)
                        possible.RemoveAll(t => t == kingdom);
                    else
                        point += player_intention[p][kingdom];
                }

                foreach (string kingdom in possible)
                {
                    double pit = Math.Min(1, kingdom_pits[kingdom] / anjiang_count[kingdom].Count);
                    double rate = pit * player_intention[p][kingdom] / point;
                    kingdom_value[kingdom] += EvaluatePlayerStrength(p) * rate;
                }
            }

            Dictionary<double, string> kingdoms_sort = new Dictionary<double, string>();
            List<string> high2low = new List<string>(), others = new List<string>();
            CompareByStrength(kingdom_value, ref high2low);
            others = new List<string>(high2low);
            others.RemoveAt(0);

            List<string> message = new List<string>();
            foreach (string kingdom in high2low)
                message.Add(string.Format("{0}->{1}", kingdom, kingdom_value[kingdom]));

            double sum_value1 = 0;
            double sum_value2 = 0;
            for (int i = 1; i < high2low.Count; i++)
                sum_value1 += kingdom_value[high2low[i]];

            if (high2low.Count > 2)
                sum_value2 = kingdom_value[high2low[1]] + kingdom_value[high2low[2]];

            process = process_public = "===";
            if (kingdom_value[high2low[0]] >= sum_value1 && kingdom_value[high2low[0]] > 0)
                process = high2low[0] + ">>>" + string.Join("+", others);
            else if (kingdom_value[high2low[0]] >= sum_value2 && sum_value2 > 0)
                process = high2low[0] + ">>" + string.Join("+", others);
            else if (kingdom_value[high2low[0]] >= kingdom_value[high2low[1]] && kingdom_value[high2low[1]] > 0)
                process = high2low[0] + ">" + string.Join("+", others);

            room.OutPut(self.Name + " : " + process + " as " + string.Join(":", message));

            foreach (Player p in anjiangs_public)
            {
                List<string> possible = GetPublicPossibleId(p);
                if (possible[0] == "careerist")
                {
                    kingdom_value_public[p.Name] = EvaluatePlayerStrength(p);
                    continue;
                }
                foreach (string kingdom in possible)
                {
                    if (anjiang_count_public.ContainsKey(kingdom))
                        anjiang_count_public[kingdom].Add(p);
                    else
                        anjiang_count_public[kingdom] = new List<Player> { p };
                }
            }

            foreach (Player p in anjiangs_public)
            {
                List<string> possible = GetPublicPossibleId(p);
                if (possible[0] == "careerist") continue;
                int point = 0;
                foreach (string kingdom in possible)
                {
                    if (kingdom_pits_public[kingdom] == 0)
                        possible.RemoveAll(t => t == kingdom);
                    else
                        point += player_intention_public[p][kingdom];
                }

                foreach (string kingdom in possible)
                {
                    double pit = Math.Min(1, kingdom_pits_public[kingdom] / anjiang_count_public[kingdom].Count);
                    double rate = pit * player_intention_public[p][kingdom] / point;
                    kingdom_value_public[kingdom] += EvaluatePlayerStrength(p) * rate;
                }
            }

            kingdoms_sort.Clear();
            CompareByStrength(kingdom_value_public, ref high2low);

            message.Clear();
            foreach (string kingdom in high2low)
                message.Add(string.Format("{0}->{1}", kingdom, kingdom_value_public[kingdom]));

            sum_value1 = 0;
            sum_value2 = 0;
            for (int i = 1; i < high2low.Count; i++)
                sum_value1 += kingdom_value_public[high2low[i]];

            if (high2low.Count > 2)
                sum_value2 = kingdom_value_public[high2low[1]] + kingdom_value_public[high2low[2]];

            if (kingdom_value_public[high2low[0]] >= sum_value1 && kingdom_value_public[high2low[0]] > 0)
                process_public = high2low[0] + ">>>";
            else if (kingdom_value_public[high2low[0]] >= sum_value2 && sum_value2 > 0)
                process_public = high2low[0] + ">>";
            else if (kingdom_value_public[high2low[0]] >= kingdom_value_public[high2low[1]] && kingdom_value_public[high2low[1]] > 0)
                process_public = high2low[0] + ">";

            room.OutPut(self.Name + " : " + process_public + " public as " + string.Join(":", message));
        }
        //统计在场国籍数量
        public void CountPlayers()
        {
            Dictionary<string, List<Player>> kingdoms_count = new Dictionary<string, List<Player>>();
            foreach (string kingdom in kingdoms)
                kingdoms_count[kingdom] = new List<Player>();

            List<Player> showns = new List<Player>(), anjiangs = new List<Player>();
            Dictionary<string, int> kingdom_pits = new Dictionary<string, int>(), kingdom_pits_public = new Dictionary<string, int>();

            foreach (Player p in room.GetAllPlayers(true))
            {
                if (p.HasShownOneGeneral())
                {                                        //count shown
                    showns.Add(p);
                }
                else
                    anjiangs.Add(p);

                if (IsKnown(self, p, "h") && Engine.GetGeneral(p.ActualGeneral1).IsLord())
                {    //find lord
                    string kingdom = Engine.GetGeneral(p.ActualGeneral1).Kingdom;
                    lords.Add(kingdom, p);
                    id_tendency[p] = kingdom;
                }

                if (p.General1Showed && Engine.GetGeneral(p.ActualGeneral1).IsLord())
                    lords_public.Add(p.Kingdom, p);
            }

            foreach (Player p in showns)
            {                                 //identify shown players
                if (p.Role == "careerist")
                {
                    if (!lords.ContainsKey(p.Kingdom) || lords[p.Kingdom] == null || !lords[p.Kingdom].Alive)
                        id_tendency[p] = "careerist";
                    else
                        id_tendency[p] = p.Kingdom;
                }
                else
                {
                    id_tendency[p] = p.Kingdom;
                    kingdoms_count[p.Kingdom].Add(p);
                }

                if (p.Role == "careerist")
                {
                    if (!lords_public.ContainsKey(p.Kingdom) || lords_public[p.Kingdom] == null || !lords_public[p.Kingdom].Alive)
                        id_public[p] = "careerist";
                    else
                        id_public[p] = p.Kingdom;
                }
                else
                    id_public[p] = p.Kingdom;
            }

            foreach (string kingdom in kingdoms)
            {
                if (lords.ContainsKey(kingdom) && lords[kingdom] != null)
                {
                    if (lords[kingdom].Alive)
                        kingdom_pits[kingdom] = 100;
                    else
                        kingdom_pits[kingdom] = 0;
                }
                else
                    kingdom_pits[kingdom] = Math.Max(room.GetAllPlayers(true).Count / 2 - (kingdoms_count.ContainsKey(kingdom) ? kingdoms_count[kingdom].Count : 0), 0);

                if (kingdom_pits[kingdom] <= 0)
                {
                    foreach (Player p in room.AlivePlayers)
                        player_intention[p][kingdom] = -100;
                }

                if (lords_public.ContainsKey(kingdom) && lords_public[kingdom] != null)
                {
                    if (lords_public[kingdom].Alive)
                        kingdom_pits_public[kingdom] = 100;
                    else
                        kingdom_pits_public[kingdom] = 0;
                }
                else
                    kingdom_pits_public[kingdom] = Math.Max(room.GetAllPlayers(true).Count / 2 - (kingdoms_count.ContainsKey(kingdom) ? kingdoms_count[kingdom].Count : 0), 0);

                if (kingdom_pits_public[kingdom] <= 0)
                {
                    foreach (Player p in room.AlivePlayers)
                        player_intention_public[p][kingdom] = -100;
                }
            }

            foreach (Player p1 in showns)
            {
                if (id_tendency[p1] == "careerist") continue;
                foreach (Player p2 in same_kingdom[p1])
                    if (id_tendency[p2] == "unknown")
                        id_tendency[p2] = id_tendency[p1];

                foreach (Player p2 in different_kingdom[p1])
                    if (id_tendency[p2] == "unknown")
                        player_intention[p2][p1.Kingdom] = -100;
            }

            foreach (Player p1 in showns)
            {
                if (id_public[p1] == "careerist") continue;
                foreach (Player p2 in same_kingdom[p1])
                    if (id_public[p2] == "unknown")
                        id_public[p2] = id_public[p1];

                foreach (Player p2 in different_kingdom[p1])
                    if (id_public[p2] == "unknown")
                        player_intention_public[p2][p1.Kingdom] = -100;
            }

            foreach (Player p in anjiangs)
            {                                       //identify anjiangs rejudge careerist
                if (id_tendency[p] != "unknown" && id_tendency[p] != "careerist")
                {
                    if ((!lords.ContainsKey(id_tendency[p]) || lords[id_tendency[p]] == null || !lords[id_tendency[p]].Alive) && kingdom_pits[id_tendency[p]] <= 0)
                        id_tendency[p] = "careerist";
                }
                else if (id_tendency[p] == "careerist")
                {
                    if (lords.ContainsKey(p.Kingdom) && lords[p.Kingdom] != null && lords[p.Kingdom].Alive)
                        id_tendency[p] = p.Kingdom;
                }

                if (id_public[p] != "unknown" && id_public[p] != "careerist")
                {
                    if ((!lords_public.ContainsKey(id_public[p]) || lords_public[id_public[p]] == null || !lords_public[id_public[p]].Alive) && kingdom_pits_public[id_public[p]] <= 0)
                        id_public[p] = "careerist";
                }
                else if (id_public[p] == "careerist")
                {
                    if (lords_public.ContainsKey(p.Kingdom) && lords_public[p.Kingdom] != null && lords_public[p.Kingdom].Alive)
                        id_public[p] = p.Kingdom;
                }
            }

            if (!self.HasShownOneGeneral() && id_tendency[self] != "careerist")
            {                // show general when lack of pit
                string kingdom = Engine.GetGeneral(self.ActualGeneral1).Kingdom;
                List<Player> others = new List<Player>();
                bool friends = false;                                                           // find shown friends
                foreach (Player p in room.GetOtherPlayers(self))
                {
                    if (id_tendency[p] == kingdom)
                    {
                        if (!p.HasShownOneGeneral())
                            others.Add(p);
                        else
                            friends = true;
                    }
                }

                if (kingdom_pits[kingdom] > 0 && others.Count + 1 >= kingdom_pits[kingdom])
                {     // if not enough pits
                    if (kingdom_pits[kingdom] == 1)
                    {                                       // if theres only 1 pit, that will be mind
                        foreach (Player p in others)                                   // and others should be careerist
                            id_tendency[p] = "careerist";
                    }

                    if (kingdom_pits[kingdom] > 1 || friends)                               // if will get friends after general show, then do it immediately
                        show_immediately = true;
                }
            }
        }
        //更新敌我识别
        public void UpdatePlayerLevel()
        {
            players_level[self] = -1;
            List<string> strs = new List<string>(process.Split('>'));
            List<string> others = new List<string>(strs[strs.Count - 1].Split('+'));

            foreach (Player p in room.GetOtherPlayers(self))
            {
                players_level[p] = 1;                                                               // level defualt to 1
                if (id_tendency[self] != "careerist" && id_tendency[self] == id_tendency[p])        // same kingdom or tendency should be friends
                    players_level[p] = -1;
                else if (id_tendency[p] == "unknown")
                {                                             // when tendency is unknown
                    List<string> possible = GetPossibleId(p);
                    string big = (possible.Count == 1 ? possible[0] : (player_intention[p][possible[0]] > player_intention[p][possible[1]] ? possible[0] : string.Empty));
                    if (!string.IsNullOrEmpty(big))
                    {                                                           // judge his most probable tendency
                        if (id_tendency[self] != "careerist" && big == id_tendency[p])
                            players_level[p] = 0;
                        else if (process.Split('>')[0] == big)
                        {
                            if (process.Contains(">>>"))
                                players_level[p] = 4;
                            else if (process.Contains(">>"))
                                players_level[p] = 2;
                        }
                    }
                }
                else
                {                                                                            // when id clearly but not same as "me"
                    string id = id_tendency[p];
                    string big = process.Split('>')[0];
                    if (big == id || big == p.Name
                            || big == id_tendency[self] || big == self.Name)
                    {                // if he or me belong to big kingdom
                        if (process.Contains(">>>") || others.Count == 1)                                                // extremely dangerous
                            players_level[p] = 5;
                        else if (process.Contains(">>") && (others[0] == id_tendency[p] || others[0] == p.Name || big == id))  // high dangerous
                            players_level[p] = 3;
                    }
                    else
                    {                                                                        // if not belong to big kingdom
                        if (process.Contains(">>>"))                                                // if the big is extremely dangerous, all smalls will be united
                            players_level[p] = -1;
                        else if (process.Contains(">>"))
                            players_level[p] = 1.5;
                    }
                }
            }
            //todo : ajust hatred
        }
        //更新玩家关系
        public void UpdatePlayerRelation(Player from, Player to, bool friendly)
        {
            if ((from == to) || (from.HasShownOneGeneral() && to.HasShownOneGeneral())) return;

            if (from.HasShownOneGeneral() && from.Role != "careerist")
            {
                if (id_tendency[to] == "unknown")
                {
                    if (friendly)
                        id_tendency[to] = from.Kingdom;
                    else
                        player_intention[to][from.Kingdom] = -100;
                }
                if (id_public[to] == "unknown")
                {
                    if (friendly)
                        id_public[to] = from.Kingdom;
                    else
                        player_intention_public[to][from.Kingdom] = -100;
                }
            }
            else if (to.HasShownOneGeneral() && to.Role != "careerist")
            {
                if (id_tendency[from] == "unknown")
                {
                    if (friendly)
                        id_tendency[from] = to.Kingdom;
                    else
                        player_intention[from][to.Kingdom] = -100;
                }
                if (id_public[from] == "unknown")
                {
                    if (friendly)
                        id_public[from] = to.Kingdom;
                    else
                        player_intention_public[from][to.Kingdom] = -100;
                }
            }
            else
            {
                if (id_tendency[from] != "unknown" && id_tendency[from] != "careerist" && id_tendency[to] == "unknown")
                {
                    if (friendly)
                        id_tendency[to] = id_tendency[from];
                    else
                        player_intention[to][id_tendency[from]] = -100;
                }
                else if (id_tendency[to] != "unknown" && id_tendency[to] != "careerist" && id_tendency[from] == "unknown")
                {
                    if (friendly)
                        id_tendency[from] = id_tendency[to];
                    else
                        player_intention[from][id_tendency[to]] = -100;
                }

                if (id_public[from] != "unknown" && id_public[from] != "careerist" && id_public[to] == "unknown")
                {
                    if (friendly)
                        id_public[to] = id_public[from];
                    else
                        player_intention_public[to][id_public[from]] = -100;
                }
                else if (id_public[to] != "unknown" && id_public[to] != "careerist" && id_public[from] == "unknown")
                {
                    if (friendly)
                        id_public[from] = id_public[to];
                    else
                        player_intention_public[from][id_public[to]] = -100;
                }

                if ((id_tendency[from] == "unknown" && id_tendency[to] == "unknown") || (id_public[from] == "unknown" && id_public[to] == "unknown"))
                {
                    if (friendly)
                    {
                        same_kingdom[from].Add(to);
                        different_kingdom[from].RemoveAll(t => t == to);
                        same_kingdom[to].Add(from);
                        different_kingdom[to].RemoveAll(t => t == from);
                    }
                    else
                    {
                        different_kingdom[from].Add(to);
                        same_kingdom[from].RemoveAll(t => t == to);
                        different_kingdom[to].Add(from);
                        same_kingdom[to].RemoveAll(t => t == from);
                    }
                }
            }

            UpdatePlayers();
        }
        //更新玩家身份的倾向
        public void UpdatePlayerIntention(Player player, string kingdom, int intention)
        {
            if (intention >= 100)
                id_tendency[player] = kingdom;
            else
                player_intention[player][kingdom] += intention;

            UpdatePlayers();
        }
        //更新仇恨值
        public void UpdatePlayerHatred(Player to, double hatred)
        {
            players_hatred[to] += hatred;
        }
    }
}
