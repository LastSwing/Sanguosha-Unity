﻿using CommonClass;
using CommonClass.Game;
using SanguoshaServer.Game;
using System;
using System.Collections.Generic;
using static CommonClass.Game.Player;
using static SanguoshaServer.Game.FunctionCard;

namespace SanguoshaServer.AI
{
    public class TrustedAI
    {
        public Dictionary<Player, List<string>> PlayerKnown { set; get; } = new Dictionary<Player, List<string>>();

        protected Player self;
        protected Room room;
        protected string process;
        protected bool show_immediately;
        protected bool skill_invoke_postpond;

        protected Dictionary<Player, List<Player>> friends = new Dictionary<Player, List<Player>>();
        protected Dictionary<Player, List<Player>> enemies = new Dictionary<Player, List<Player>>();
        protected List<Player> friends_noself = new List<Player>();
        protected List<Player> priority_enemies = new List<Player>();

        protected Dictionary<Player, string> id_tendency = new Dictionary<Player, string>();
        protected Dictionary<Player, string> id_public = new Dictionary<Player, string>();

        protected Dictionary<string, Player> lords = new Dictionary<string, Player>();
        protected Dictionary<string, Player> lords_public = new Dictionary<string, Player>();
        protected Dictionary<Player, Dictionary<string, int>> player_intention = new Dictionary<Player, Dictionary<string, int>>();
        protected Dictionary<Player, Dictionary<string, int>> player_intention_public = new Dictionary<Player, Dictionary<string, int>>();
        protected Dictionary<Player, List<Player>> same_kingdom = new Dictionary<Player, List<Player>>();
        protected Dictionary<Player, List<Player>> different_kingdom = new Dictionary<Player, List<Player>>();
        protected Dictionary<Player, double> players_level = new Dictionary<Player, double>();
        protected Dictionary<Player, double> players_hatred = new Dictionary<Player, double>();

        protected int turn_count;
        protected string process_public;

        protected Dictionary<Player, List<int>> public_handcards = new Dictionary<Player, List<int>>();
        protected Dictionary<Player, List<int>> private_handcards = new Dictionary<Player, List<int>>();
        protected Dictionary<Player, List<int>> wooden_cards = new Dictionary<Player, List<int>>();

        protected KeyValuePair<Player, List<int>> guanxing = new KeyValuePair<Player, List<int>>();
        protected List<WrappedCard> guanxing_dts = new List<WrappedCard>();
        protected Dictionary<Player, List<int>> pre_discard = new Dictionary<Player, List<int>>();
        protected List<Player> pre_ignore_armor = new List<Player>();
        protected Dictionary<Player, string> pre_disable = new Dictionary<Player, string>();
        protected Dictionary<Player, List<string>> card_lack = new Dictionary<Player, List<string>>();
        protected WrappedCard pre_drink;

        //others
        CardUseStruct ai_AOE_data;

        public TrustedAI(Room room, Player player)
        {
            this.room = room;
            self = player;

            foreach (Player p in room.Players)
            {
                PlayerKnown[p] = new List<string>();
                friends[p] = new List<Player>();
                enemies[p] = new List<Player>();
                id_tendency[p] = "unknown";
                id_public[p] = "unknown";
                player_intention[p] = new Dictionary<string, int>();
                player_intention_public[p] = new Dictionary<string, int>();
                same_kingdom[p] = new List<Player>();
                different_kingdom[p] = new List<Player>();
                players_level[p] = 1;
                players_hatred[p] = 0;
                public_handcards[p] = new List<int>();
                private_handcards[p] = new List<int>();
                wooden_cards[p] = new List<int>();
                pre_discard[p] = new List<int>();
                pre_disable[p] = string.Empty;
                card_lack[p] = new List<string>();
            }
        }

        public bool IsFriend(Player other, Player another = null)
        {
            if (another == null)
                return friends[self].Contains(other);

            return friends[other].Contains(another);
        }

        public bool IsEnemy(Player other, Player another = null)
        {
            if (another == null)
                return enemies[self].Contains(other);

            return enemies[other].Contains(another);
        }

        public bool HasSkill(string skill_name, Player who = null, bool ignore_invalid = false)
        {
            Player player = who ?? self;
            foreach (string skills in skill_name.Split('|')) {
                bool check_point = true;
                foreach (string skill in skills.Split('+')) {
                    bool check = GetKnownSkills(player, true, self, ignore_invalid).Contains(skill);
                    if (!check)
                    {
                        check_point = false;
                        break;
                    }
                }
                if (check_point) return true;
            }
            return false;
        }
        public void SetKnown(Player to, string flags)
        {
            if (flags.Contains("h"))
                PlayerKnown[to].Add("head");

            if (flags.Contains("d"))
                PlayerKnown[to].Add("deputy");
        }

        public bool IsKnown(Player from, Player to, string flags = null)
        {
            if (from == to) return true;

            if (string.IsNullOrEmpty(flags)) return to.HasShownOneGeneral() || room.GetAI(from, true).PlayerKnown[to].Contains("head")
                    || room.GetAI(from, true).PlayerKnown[to].Contains("deputy");

            if (flags.Contains("h") && !to.General1Showed && !room.GetAI(from, true).PlayerKnown[to].Contains("head"))
                return false;

            if (flags.Contains("d") && !string.IsNullOrEmpty(to.General2) && (!to.General2Showed) && !room.GetAI(from, true).PlayerKnown[to].Contains("deputy"))
                return false;

            return true;
        }
        public List<string> GetKnownSkills(Player to, bool visible = true, Player from = null, bool ignore_invalid = false)
        {
            from = from ?? self;
            List <Skill> skills = new List<Skill>();
            if (!pre_disable[to].Contains("h"))
                skills = RoomLogic.GetHeadActivedSkills(room, to, true, !(IsKnown(self, to, "h") && IsKnown(from, to, "h")));
            if (!string.IsNullOrEmpty(to.General2) && !pre_disable[to].Contains("d"))
                skills.AddRange(RoomLogic.GetDeputyActivedSkills(room, to, true, !(IsKnown(self, to, "d") && IsKnown(from, to, "d"))));

            List<string> skill_names = new List<string>();
            foreach (Skill skill in skills) {
                if ((skill.SkillFrequency == Skill.Frequency.Limited && to.GetMark(skill.LimitMark) == 0)
                        || (skill.SkillFrequency == Skill.Frequency.Wake && to.GetMark(skill.Name) > 0)) continue;
                if (!ignore_invalid && Engine.Invalid(room, to, skill.Name) != null) continue;
                if (!RoomLogic.PlayerHasSkill(room, to, skill.Name) && self == to && from == self) continue;
                if (skill.Visible || !visible)
                    skill_names.Add(skill.Name);
            }

            return skill_names;
        }
        public Player FindPlayerBySkill(string skill_name)
        {
            foreach (Player p in room.AlivePlayers)
                if (HasSkill(skill_name, p)) return p;

            return null;
        }
        public List<Player> GetEnemies(Player player)
        {
            List<Player> players = room.GetOtherPlayers(self);
            List<Player> enemies = new List<Player>();
            foreach (Player p in players)
                if (IsEnemy(p, player)) enemies.Add(p);

            return enemies;
        }

        public List<Player> GetFriends(Player player)
        {
            List<Player> players = room.GetOtherPlayers(self);
            List<Player> friends = new List<Player>();
            foreach (Player p in players)
                if (IsFriend(p, player)) friends.Add(p);

            return friends;
        }
        public List<string> GetPossibleId(Player who)
        {
            if (id_tendency[who] != "unknown") return new List<string> { id_tendency[who] };

            Dictionary<string, double> sort = new Dictionary<string, double>();
            foreach (string kingdom in player_intention[who].Keys)
            {
                int intention = player_intention[who][kingdom];
                if (intention > 0)
                    sort.Add(kingdom, intention);
            }

            List<string> result = new List<string>();
            CompareByStrength(sort, ref result);

            if (result.Count == 0) result.Add("careerist");
            return result;
        }

        public void CompareByStrength(Dictionary<string, double> container, ref List<string> result)
        {
            result.Clear();
            foreach (string name in container.Keys)
                result.Add(name);

            result.Sort((x, y) => { return container[x] > container[y] ? -1 : 1; });
        }

        public void CompareByScore(ref List<ScoreStruct> list) => list.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });

        public List<int> CompareByValue(Dictionary<int, double> container, bool descending)
        {
            List<int> result = new List<int>(container.Keys);

            if (descending)
                result.Sort((x, y) => { return container[x] < container[y] ? -1 : 1; });
            else
                result.Sort((x, y) => { return container[x] > container[y] ? -1 : 1; });
            return result;
        }

        public List<string> GetPublicPossibleId(Player who)
        {
            if (id_public[who] != "unknown") return new List<string> { id_public[who] };

            Dictionary<string, double> sort = new Dictionary<string, double>();
            foreach (string kingdom in player_intention_public[who].Keys) {
                int intention = player_intention_public[who][kingdom];
                if (intention > 0)
                    sort.Add(kingdom, intention);
            }

            List<string> result = new List<string>();
            CompareByStrength(sort, ref result);

            if (result.Count == 0) result.Add("careerist");
            return result;
        }

        public List<int> GetKnownCards(Player who)
        {
            List<int> ids = new List<int>();
            if (who == self)
            {
                ids = new List<int>(self.HandCards);
            }
            else
            {
                foreach (int id in private_handcards[who])
            if (!ids.Contains(id))
                    ids.Add(id);
            }

            return ids;
        }

        public List<int> GetKnownHandPileCards(Player who)
        {
            List<int> ids = new List<int>();
            if (who == self)
            {
                ids = new List<int>(self.HandCards);
                ids.AddRange(who.GetHandPile());
            }
            else
            {
                foreach (int id in private_handcards[who])
            if (!ids.Contains(id))
                    ids .Add(id);

                ids.AddRange(who.GetHandPile(false));
                ids.AddRange(wooden_cards[who]);
            }

            return ids;
        }

        public List<WrappedCard> GetCards(string card_name, Player player, bool no_duplicated = false)
        {
            List <WrappedCard> result = new List<WrappedCard>();
            List<int> ids = new List<int>();
            if (self == player)
            {
                foreach (int id in self.HandCards) {
                    if (pre_drink != null && pre_drink.SubCards.Contains(id)) continue;
                    ids.Add(id);
                }
            }
            else
                ids = private_handcards[player];

            foreach (int id in player.GetHandPile(false)) {
                if (player == self && pre_drink != null && pre_drink.SubCards.Contains(id)) continue;
                ids.Add(id);
            }
            if (player == self)
            {
                foreach (int id in player.GetPile("wooden_ox")) {
                    if (player == self && pre_drink != null && pre_drink.SubCards.Contains(id)) continue;
                    ids.Add(id);
                }
            }
            else
            {
                    ids.AddRange(wooden_cards[player]);
            }

            foreach (int id  in player.GetEquips()) {
                if (player == self && pre_drink != null && pre_drink.SubCards.Contains(id)) continue;
                ids.Add(id);
            }

            foreach (int id in ids) {
                List <WrappedCard> cards = GetViewAsCards(player, id);
                foreach (WrappedCard card in cards) {
                    if (card.Name.Contains(card_name))
                    {
                        result.Add(card);
                        if (no_duplicated) break;
                    }
                }
            }

            if (self == player)
                SortByUseValue(ref result);
            return result;
        }

        List<WrappedCard> GetViewAsCards(Player player, int id, Place place = Place.PlaceUnknown)
        {
            List <WrappedCard> result = new List<WrappedCard>();
            WrappedCard card = room.GetCard(id);
            if (card.Name == "Peach" && room.BloodBattle)
            {
                WrappedCard slash = new WrappedCard("Slash");
                slash.AddSubCard(card);
                slash = RoomLogic.ParseUseCard(room, slash);
                result.Add(slash);

                WrappedCard jink = new WrappedCard("Jink");
                jink.AddSubCard(card);
                jink = RoomLogic.ParseUseCard(room, jink);
                result.Add(jink);
            }
            else
                result.Add(card);

            if (place ==  Place.PlaceUnknown && room.GetCardOwner(id) == player)
                place = room.GetCardPlace(id);

            foreach (string skill in GetKnownSkills(player)) {
                SkillEvent e = Engine.GetSkillEvent(skill);
                if (e != null)
                {
                    WrappedCard v_card = e.ViewAs(this, player, id, room.Current == player, place);
                    if (v_card != null)
                        result.Add(v_card);
                }
            }

            return result;
        }

        public int GetKnownCardsNums(string pattern, string flags, Player player, Player from)
        {
            int hand = 0;
            int eq = 0;
            if (flags.Contains("e"))
            {
                List<int> eqs = new List<int>();
                foreach (int id in player.GetEquips())
                    if (!pre_discard[player].Contains(id) && IsCard(id, pattern, player, self))
                        eqs.Add(id);

                eq = eqs.Count;
            }


            if (flags.Contains("h"))
            {
                List<int> hands = new List<int>();
                if (from == self)
                {
                    foreach (int id in GetKnownCards(player))
                        if (IsCard(id, pattern, player, self))
                            hands.Add(id);

                }
                else
                {
                    foreach (int id in public_handcards[player])
                        if (IsCard(id, pattern, player, from))
                            hands.Add(id);
                }
                hand = hands.Count;
            }
            return hand + eq;
        }

        public bool IsCard(int id, string pattern, Player player, Player from)
        {
            List <WrappedCard> result = new List<WrappedCard>();
            WrappedCard card = room.GetCard(id);
            if (card.Name == "Peach" && room.BloodBattle)
            {
                WrappedCard slash = new WrappedCard("Slash");
                slash.AddSubCard(card);
                slash = RoomLogic.ParseUseCard(room, slash);
                result.Add(slash);

                WrappedCard jink = new WrappedCard("Jink");
                jink.AddSubCard(card);
                jink = RoomLogic.ParseUseCard(room, jink);
                result.Add(jink);
            }
            else
                result.Add(card);

            foreach (string skill in GetKnownSkills(player, true, from)) {
                SkillEvent e = Engine.GetSkillEvent(skill);
                if (e != null)
                {
                    WrappedCard v_card = e.ViewAs(this, player, id, room.Current == player, room.GetCardPlace(id));
                    if (v_card != null)
                        result.Add(v_card);
                }
            }

            foreach (WrappedCard c in result)
                if (Engine.MatchExpPattern(room, pattern, player, c))
                    return true;

            return false;
        }

        public bool IsWeak(Player player = null)
        {
            if (player == null) player = self;
            if (HasBuquEffect(player)) return false;
            if (HasNiepanEffect(player)) return false;
            //if (hasSkill("kongcheng", player) && player->isKongcheng() && player->getHp() >= 2) return false;
            if ((player.Hp <= 2 && player.HandcardNum <= 2) || player.Hp <= 1) return true;
            return false;
        }

        public bool IsRoleExpose()
        {
            if (self.HasShownOneGeneral() || id_public[self] != "unknown" || GetPublicPossibleId(self).Count == 1
                    || player_intention_public[self][GetPublicPossibleId(self)[0]] > player_intention_public[self][GetPublicPossibleId(self)[1]] + 50)
                return true;
            return false;
        }

        public bool WillShowForAttack()
        {
            if (IsRoleExpose() || show_immediately || skill_invoke_postpond) return true;

            bool reward = true;
            bool kill = true;
            foreach (Player p in room.AlivePlayers) {
                if (p.HasShownOneGeneral())
                    reward = false;
                if (IsWeak(p) && id_tendency[p] != "unknown" && id_tendency[p] != id_tendency[self])
                    kill = true;
            }
            //首亮奖励
            if (reward && room.Setting.GameMode == "Hegemony" || IsWeak()) return true;
            if (kill && room.Current == self) return true;
            if (room.AlivePlayers.Count < 3) return true;

            return false;
        }

        public bool WillShowForDefence()
        {
            if (IsRoleExpose() || show_immediately || skill_invoke_postpond) return true;

            bool reward = true;
            foreach (Player p in room.AlivePlayers) {
                if (p.HasShownOneGeneral())
                    reward = false;
            }

            if (reward && room.Setting.GameMode == "Hegemony" || IsWeak()) return true;
            if (room.AlivePlayers.Count < 3) return true;

            return false;
        }


        public bool WillShowForMasochism()
        {
            if (IsRoleExpose() || show_immediately || skill_invoke_postpond) return true;

            bool reward = true;
            foreach (Player p in room.AlivePlayers)
        if (p.HasShownOneGeneral())
                reward = false;

            if (reward && room.Setting.GameMode == "Hegemony" || IsWeak()) return true;
            DamageStruct damage = (DamageStruct)room.GetTag("CurrentDamageStruct");

            if (damage.From != null && damage.From.Alive && IsFriend(damage.From)) return true;

            if (room.AlivePlayers.Count < 3) return true;

            if (self.GetLostHp() == 0 && GetCards("Peach", self).Count > 0) return false;

            return true;
        }

        public bool CanResist(Player player, int damage)
        {
            foreach (SkillEvent e in Engine.GetSkillEvents())
                if (e.CanResist(this, damage)) return true;
            if (HasArmorEffect(player, "BreastPlate") && damage >= player.Hp) return true;

            return false;
        }

        public bool HasBuquEffect(Player player)
        {
            return HasSkill("buqu", player) && player.GetPile("buqu").Count <= 4;
        }
        public bool HasNiepanEffect(Player player)
        {
            return (HasSkill("niepan", player) && player.GetMark("@nirvana") > 0)
                    || (HasSkill("jizhao", player) && player.GetMark("@jizhao") > 0);
        }
        public bool HasKongchengEffect(Player player)
        {
            return (RoomLogic.PlayerHasShownSkill(room, player, "kongcheng") && player.IsKongcheng());
        }

        public double GetLostEquipEffect(Player player, List<int> except = null)
        {
            List<int> ids = new List<int>(player.GetEquips());

            List<double> values = new List<double>();
            foreach (int id in ids) {
                if (except != null && except.Contains(id) || pre_discard[player].Contains(id)) continue;
                double value = GetKeepValue(id, player);
                if (value < 0) values.Add(value);
            }

            values.Sort((x, y) => { return x < y ? -1 : 1; });
            if (values.Count == 0) return 0;
            return values[0];
        }

        public bool HasArmorEffect(Player player, string armor)
        {
            if (pre_ignore_armor.Contains(self))
                return false;

            if (player.GetArmor() && pre_discard[self].Contains(player.Armor.Key))
                return armor == "EightDiagram" && HasSkill("bazhen", player);

            if (player == self)
                return RoomLogic.HasArmorEffect(room, player, armor);
            else
            {
                if (RoomLogic.HasArmorEffect(room, player, armor, false)) return true;
                if (RoomLogic.HasArmorEffect(room, player, armor))
                {
                    List <ViewHasSkill> skills = Engine.ViewHas(room, player, armor, "armor");
                    foreach (ViewHasSkill skill in skills)
                        if (HasSkill(skill.Name, player)) return true;
                }
            }

            return false;
        }

        public bool CanOperate(Player from, Player to, string flags, HandlingMethod method)
        {
            if (int.TryParse(flags, out int id))
            {
                if (method == HandlingMethod.MethodDiscard)
                    return RoomLogic.CanDiscard(room, from, to, id);
                if (method == HandlingMethod.MethodGet)
                    return RoomLogic.CanDiscard(room, from, to, id);
            }
            else
            {
                if (method == HandlingMethod.MethodDiscard)
                    return RoomLogic.CanDiscard(room, from, to, flags);
                if (method == HandlingMethod.MethodGet)
                    return RoomLogic.CanGetCard(room, from, to, flags);
            }

            return true;
        }
        public ScoreStruct FindCards2Discard(Player from, Player player, string flags,
                                  HandlingMethod method, int times = 1, bool onebyone = false, List<int> disable = null)
        {
            List<int> result = new List<int>();
            disable = disable ?? new List<int>();
            ScoreStruct score = new ScoreStruct
            {
                Players = new List<Player> { player }
            };
            double expect = 0, expect_self = 0;
            if (!CanOperate(from, player, flags, method)) return score;

            bool wizzard_friend = false;
            bool wizzard_enemy = false;
            int indu = -1;
            int supply = -1;
            int light = -1;
            int light_effect = 0;
            int hand_ajust = 0;

            foreach (int id in disable)
            {
                if (room.GetCardPlace(id) == Place.PlaceHand && room.GetCardOwner(id) == player)
                    hand_ajust++;
            }

            if (flags.Contains("j") && CanOperate(from, player, "j", method))
            {
                foreach (Player p in room.AlivePlayers)
                {
                    if (HasSkill("guicai|guidao", p) && IsFriend(p, from)) wizzard_friend = true;
                    if (HasSkill("guicai|guidao", p) && IsEnemy(p, from)) wizzard_enemy = true;
                }

                foreach (int id in player.JudgingArea)
                {
                    WrappedCard card = room.GetCard(id);
                    if (card.Name == "Indulgence") indu = id;
                    else if (card.Name == "SupplyShortage") supply = id;
                    else if (card.Name == "Lightning") light = id;
                }

                if (light >= 0 && !wizzard_friend)
                {
                    foreach (Player p in room.AlivePlayers)
                    {
                        DamageStruct damage = new DamageStruct(room.GetCard(light), null, p, 3, DamageStruct.DamageNature.Thunder);
                        int point = DamageEffect(damage);
                        if (IsFriend(from, p) && IsCardEffect(room.GetCard(light), p, null) && (point > 0 && IsWeak(p) || point > 1)
                                && !(HasSkill("qiaobian", p) || (HasSkill("guanxing|yizhi", p) && !wizzard_enemy)))
                            light_effect++;
                    }
                }
            }

            if (IsFriend(from, player))
            {
                if (player.FaceUp)
                {
                    if (light >= 0 && light_effect > 0 && wizzard_enemy)
                    {
                        result.Add(light);
                        expect += 8;
                        if (IsFriend(player))
                            expect_self += 8;
                    }
                    if (times > result.Count && indu >= 0 && !(HasSkill("guanxing|yizhi", player)
                            && room.AlivePlayers.Count > 3) && !HasSkill("guanxing+yizhi", player))
                    {
                        result.Add(indu);
                        expect += 6;
                        if (IsFriend(player))
                            expect_self += 6;
                        else if (IsEnemy(player))
                            expect_self -= 6;
                    }
                    if (times > result.Count && supply >= 0 && !(HasSkill("guanxing|yizhi", player)
                            && room.AlivePlayers.Count > 3) && !HasSkill("guanxing+yizhi", player))
                    {
                        result.Add(supply);
                        expect += 5;
                        if (IsFriend(player))
                            expect_self += 5;
                        else if (IsEnemy(player))
                            expect_self -= 5;
                    }
                    if (times > result.Count && light >= 0 && !result.Contains(light) && light_effect > GetEnemies(from).Count)
                    {
                        result.Add(light);
                        expect += 5;
                        if (IsFriend(player))
                            expect_self += 5;
                    }
                }

                if (NeedKongcheng(player) && flags.Contains("h") && CanOperate(from, player, "h", method))
                {
                    if (player.HandcardNum - hand_ajust == times - result.Count)
                    {

                        result.AddRange(player.HandCards);
                        expect += 5;
                        if (IsFriend(player))
                            expect_self += 5;
                    }
                }

                if (flags.Contains("e") && CanOperate(from, player, "e", method) && times > result.Count)
                {
                    List<int> ids = new List<int>();
                    foreach (int id in player.GetEquips())
                        if (!disable.Contains(id) && CanOperate(from, player, id.ToString(), method))
                            ids.Add(id);

                    List<double> values = new List<double>();
                    Dictionary<int, double> maps = new Dictionary<int, double>();
                    foreach (int id in ids)
                    {
                        double value = GetKeepValue(id, player);     // todo:
                        if (value < 0)
                        {
                            values.Add(value);
                            maps.Add(id, value);
                        }
                    }
                    values.Sort((x, y) => { return x < y ? -1 : 1; });

                    double adjust = 0;                                               //if not move only once,
                    if (!onebyone)
                    {                                                //must ajust for xiaoji/xuanlue  **for now
                        if (HasSkill("xiaoji", player)) adjust += 6;
                        if (HasSkill("xuanlue", player)) adjust += 3;
                    }
                    for (int i = 0; i < Math.Min(values.Count, times - result.Count); i++)
                    {
                        foreach (int id in maps.Keys)
                        {
                            double value = values[i] + (i == 0 ? 0 : adjust);
                            if (!result.Contains(id) && maps[id] == values[i] && value < 0)
                            {
                                result.Add(id);
                                expect -= value;
                                if (IsFriend(player))
                                    expect_self -= value;
                                else if (IsEnemy(player))
                                    expect_self += value;
                            }
                        }
                    }
                }

                if (flags.Contains("j") && CanOperate(from, player, "j", method))
                {
                    if (times > result.Count && !result.Contains(light) && light >= 0 && light_effect > 0 && wizzard_enemy)
                    {
                        result.Add(light);
                        expect += 5;
                        if (IsFriend(player))
                            expect_self += 5;
                    }
                    if (times > result.Count && !result.Contains(indu) && indu >= 0
                            && !(HasSkill("guanxing|yizhi", player) && room.AliveCount() > 3) && !HasSkill("guanxing+yizhi", player))
                    {
                        result.Add(indu);
                        expect += 4;
                        if (IsFriend(player))
                            expect_self += 4;
                        else if (IsEnemy(player))
                            expect_self -= 4;
                    }
                    if (times > result.Count && !result.Contains(supply) && supply >= 0
                            && !(HasSkill("guanxing|yizhi", player) && room.AliveCount() > 3) && !HasSkill("guanxing+yizhi", player))
                    {
                        result.Add(supply);
                        expect += 3;
                        if (IsFriend(player))
                            expect_self += 3;
                        else if (IsEnemy(player))
                            expect_self -= 3;
                    }
                    if (times > result.Count && light >= 0 && !result.Contains(light) && light_effect > enemies[self].Count)
                    {
                        result.Add(light);
                        expect += 3;
                        if (IsFriend(player))
                            expect_self += 3;
                    }
                }

                if (HasSkill("tuntian", player))
                {
                    int count = 0;
                    foreach (int id in result)
                        if (room.GetCardPlace(id) != Place.PlaceDelayedTrick)
                            count++;

                    if (onebyone)
                    {
                        expect += count * 1.5;
                        if (IsFriend(player))
                            expect_self += count * 1.5;
                        else if (IsEnemy(player))
                            expect_self -= count * 1.5;
                    }
                    else if (count > 0)
                    {
                        expect += 1.5;
                        if (IsFriend(player))
                            expect_self += 1.5;
                        else if (IsEnemy(player))
                            expect_self -= 1.5;
                    }
                }
            }
            else
            {
                if (player.FaceUp && light >= 0 && light_effect > 0 && wizzard_enemy)
                {
                    result.Add(light);
                    expect += 7;
                    if (IsFriend(from))
                        expect_self += 7;
                }

                List<double> values = new List<double>();
                Dictionary<int, double> maps = new Dictionary<int, double>();
                if (flags.Contains("e") && CanOperate(from, player, "e", method) && times > result.Count)
                {
                    List<int> ids = new List<int>();
                    foreach (int id in player.GetEquips())
                        if (!disable.Contains(id) && CanOperate(self, player, id.ToString(), method))
                            ids.Add(id);

                    foreach (int id in ids)
                    {
                        double value = 0 - GetKeepValue(id, player);     // todo:
                        if (value < 0)
                        {
                            values.Add(value);
                            maps.Add(id, value);
                        }
                    }
                }

                double hand_value = 0;
                double singel_value = 0;
                if (flags.Contains("h") && CanOperate(self, player, "h", method) && times > result.Count)
                {
                    foreach (int id in player.HandCards)
                    {
                        if (disable.Contains(id)) continue;
                        if (GetKnownCards(player).Contains(id))
                            hand_value -= GetKeepValue(id, player);
                        else
                            hand_value -= (player.HandcardNum - hand_ajust > 8 ? 2 : (player.HandcardNum - hand_ajust > 5 ? 3 : 4));
                    }
                    singel_value = hand_value / (player.HandcardNum - hand_ajust);
                    if (singel_value == hand_value)
                    {
                        if (NeedKongcheng(player))
                            hand_value = 100;
                        //else if (lastcardeffect)
                        //    singel_value = singel_value + 3;                  todo: *shoucheng
                    }
                }

                double adjust = 0;                                               //if not move only once,
                if (!onebyone)
                {                                                //must ajust for xiaoji/xuanlue  **for now
                    if (HasSkill("xiaoji", player)) adjust += 6;
                    if (HasSkill("xuanlue", player)) adjust += 3;
                }

                values.Sort((x, y) => { return x < y ? -1 : 1; });

                for (int i = 0; i < times - result.Count; i++)
                {
                    int hand = 0;
                    int eq = 0;
                    if (hand_value < 0 && (values.Count == 0 || singel_value < values[0]) && singel_value < 0)
                    {
                        bool append = false;
                        while (!append)
                        {
                            int id = room.GetRandomHandCard(player);
                            if (!result.Contains(id))
                            {
                                result.Add(id);
                                append = true;
                            }
                        }
                        expect -= singel_value;
                        if (IsFriend(player))
                            expect_self += singel_value;
                        else if (IsEnemy(player))
                            expect_self -= singel_value;
                        hand++;
                        hand_value = (hand == player.HandcardNum ? 0 : hand_value - singel_value);
                        if (singel_value == hand_value)
                        {
                            if (NeedKongcheng(player))
                                hand_value = 100;
                            //else if (lastcardeffect)
                            //    singel_value = singel_value + 3;                  todo: *shoucheng
                        }
                    }
                    else if (values.Count > 0 && values[0] < singel_value && values[0] < 0
                             && (eq == 0 || values[0] + adjust < 0))
                    {
                        double value = values[0];
                        values.RemoveAt(0);
                        foreach (int id in maps.Keys)
                            if (!result.Contains(id) && maps[id] == value)
                                result.Add(id);
                        expect -= (value + (eq == 0 ? 0 : adjust));
                        if (IsFriend(player))
                            expect_self += (value + (eq == 0 ? 0 : adjust));
                        else if (IsEnemy(player))
                            expect_self -= (value + (eq == 0 ? 0 : adjust));
                    }
                    else
                        break;
                }

                if (HasSkill("tuntian", player))
                {
                    int count = 0;
                    foreach (int id in result)
                        if (room.GetCardPlace(id) != Place.PlaceDelayedTrick)
                            count++;

                    if (onebyone)
                    {
                        expect -= count * 1.5;
                        if (IsFriend(player))
                            expect_self += count * 1.5;
                        else if (IsEnemy(player))
                            expect_self -= count * 1.5;
                    }
                    else if (count > 0)
                    {
                        expect -= 1.5;
                        if (IsFriend(player))
                            expect_self += 1.5;
                        else if (IsEnemy(player))
                            expect_self -= 1.5;
                    }
                }
            }

            score.Ids = result;
            if (priority_enemies.Contains(player))
                expect_self = expect_self * 1.5;

            if (method == HandlingMethod.MethodGet)
            {
                double get_value = 0;
                foreach (int id in result)
                    get_value += GetUseValue(id, from, Place.PlaceHand);

                if (IsFriend(from))
                    expect_self += get_value;
                else if (IsEnemy(from))
                    expect_self -= get_value;
            }

            score.Score = expect_self;
            return score;
        }

        public bool WillSkipPlayPhase(Player player)
        {
            if (player.Phase == PlayerPhase.Play) return false;
            if (player.IsSkipped(PlayerPhase.Play) || (room.Current != player && !player.FaceUp)) return true;
            if (player.HasFlag("willSkipPlayPhase")) return true;

            if (RoomLogic.PlayerContainsTrick(room, player, "Indulgence"))
            {
                if (HasSkill("shensu", player, true)) return false;
                if (HasSkill("qiaobian", player, true) && !player.IsKongcheng()) return false;
                List<Player> friends = GetFriends(player);
                foreach (Player f in friends)
                {
                    if (GetCards("Nullification", f).Count > 0) return false;
                    if (room.AlivePlayers.IndexOf(f) < room.AlivePlayers.IndexOf(player)
                            && (GetCards("Dismantlement", f).Count > 0 && RoomLogic.CanDiscard(room, f, player, "j"))
                            || (GetCards("Snatch", f).Count > 0 && RoomLogic.CanGetCard(room, f, player, "j")))
                        return false;
                }

                Player retrialer = GetWizzardRaceWinner("Indulgence", player, player);
                if (retrialer != null && IsFriend(retrialer, player)) return false;
                if (HasSkill("guanxing+yizhi", player, true) || (HasSkill("guanxing|yizhi", player, true) && room.AliveCount() >= 4)
                        && (retrialer == null || !IsEnemy(retrialer, player))) return false;

                WrappedCard indu = null;
                foreach (int id in player.JudgingArea)
                {
                    WrappedCard card = room.GetCard(id);
                    if (card.Name == "Indulgence")
                    {
                        indu = card;
                        break;
                    }
                }
                if (IsGuanxingEffected(player, false, indu) && (retrialer == null || !IsEnemy(retrialer, player))) return false;

                Player help = null;
                int i = 1;
                while (help == null)
                {
                    Player last = room.GetLastAlive(player, i);
                    if (last.FaceUp)
                        help = last;
                    i++;
                }
                if (help != null && help != room.Current && help != player && IsFriend(help, player) && HasSkill("guanxing|yizhi", help, true))
                {
                    int count = Math.Min(5, room.AliveCount());
                    if (HasSkill("guanxing+yizhi", help, true)) count = 5;
                    if (count - help.JudgingArea.Count >= 4 && !RoomLogic.PlayerContainsTrick(room, help, "Lightning") && (retrialer == null || !IsEnemy(retrialer, player)))
                        return false;
                }

                return true;
            }

            return false;
        }

        public bool WillSkipDrawPhase(Player player)
        {
            if (player.IsSkipped(PlayerPhase.Draw) || (room.Current != player && !player.FaceUp)) return true;
            if (player.HasFlag("willSkipDrawPhase")) return true;

            if (RoomLogic.PlayerContainsTrick(room, player, "SupplyShortage"))
            {
                List<Player> friends = GetFriends(player);
                foreach (Player f in friends)
                {
                    if (GetCards("Nullification", f).Count > 0) return false;
                    if (room.AlivePlayers.IndexOf(f) < room.AlivePlayers.IndexOf(player)
                            && (GetCards("Dismantlement", f).Count > 0 && RoomLogic.CanDiscard(room, f, player, "j"))
                            || (GetCards("Snatch", f).Count > 0 && RoomLogic.CanGetCard(room, f, player, "j")))
                        return false;
                }

                Player retrialer = GetWizzardRaceWinner("SupplyShortage", player, player);
                if (retrialer != null && IsFriend(retrialer, player)) return false;
                if (HasSkill("guanxing+yizhi", player, true) || (HasSkill("guanxing|yizhi", player, true) && room.AliveCount() >= 4)
                        && (retrialer == null || !IsEnemy(retrialer, player))) return false;

                WrappedCard sppply = null;
                foreach (int id in player.JudgingArea)
                {
                    WrappedCard card = room.GetCard(id);
                    if (card.Name == "SupplyShortage")
                    {
                        sppply = card;
                        break;
                    }
                }
                if (IsGuanxingEffected(player, false, sppply) && (retrialer == null || !IsEnemy(retrialer, player))) return false;

                Player help = null;
                int i = 1;
                while (help == null)
                {
                    Player last = room.GetLastAlive(player, i);
                    if (last.FaceUp)
                        help = last;
                    i++;
                }
                if (help != null && help != player && IsFriend(help, player) && HasSkill("guanxing|yizhi", help, true))
                {
                    int count = Math.Min(5, room.AliveCount());
                    if (HasSkill("guanxing+yizhi", help, true)) count = 5;
                    if (count - help.JudgingArea.Count >= 4 && !RoomLogic.PlayerContainsTrick(room, help, "Lightning") && (retrialer == null || !IsEnemy(retrialer, player)))
                        return false;
                }

                return true;
            }
            return false;
        }

        public bool IsGuanxingEffected(Player player, bool expect_effect, WrappedCard card)
        {
            if (player != room.Current)
            {
                Player next = null;
                int i = 1;
                while (next == null)
                {
                    Player p = room.GetNextAlive(room.Current, i);
                    i++;
                    if (p.FaceUp)
                        next = p;
                }
                if (next != player) return false;
            }
            else if (player.Phase > PlayerPhase.Judge)
                return false;

            int index = -1;
            int judging = 0;
            if (player.Phase == PlayerPhase.Judge && (int)room.GetTag("judge_draw") > 0)
            {
                judging++;
                List<JudgeStruct> judges = (List<JudgeStruct>)room.GetTag("current_judge");
                JudgeStruct judge = judges[judges.Count - 1];
                if (judge.Reason == card.Name)
                    index = 0;
            }
            List<WrappedCard> dts = RoomLogic.GetPlayerJudgingArea(room, player);
            if (index == -1 && dts.Contains(card))
                index = dts.IndexOf(card) + judging;

            List<int> judge_cards = player.GetPile("incantation");
            List<int> drawpile = room.DrawPile;
            if (index == -1 || drawpile.Count < index + 1 - judge_cards.Count) return false;

            int y = 0;
            for (int i = player.GetPile("incantation").Count; i <= index; i++)
            {
                int id = drawpile[y];
                y++;
                if (room.GetCard(id).HasFlag("visible") || room.GetCard(id).HasFlag("visible2" + self.Name))
                    judge_cards.Add(id);
                else
                    judge_cards.Add(-1);
            }

            int judge_card = judge_cards[judge_cards.Count - 1];
            string reason = card.Name;
            if (judge_card != -1)
            {
                WrappedCard j_card = room.GetCard(judge_card);
                WrappedCard.CardSuit suit = HasSkill("hongyan", player) && j_card.Suit == WrappedCard.CardSuit.Spade ? WrappedCard.CardSuit.Heart : j_card.Suit;
                if (expect_effect)
                    return (reason == "Indulence" && suit != WrappedCard.CardSuit.Heart) || (reason == "SupplyShortage" && suit != WrappedCard.CardSuit.Club)
                                       || (reason == "Lightning" && suit == WrappedCard.CardSuit.Spade && j_card.Number <= 9 && j_card.Number >= 2);
                else
                    return (reason == "Indulence" && suit == WrappedCard.CardSuit.Heart) || (reason == "SupplyShortage" && suit == WrappedCard.CardSuit.Club)
                                       || (reason == "Lightning" && (suit != WrappedCard.CardSuit.Spade || j_card.Number > 9 || j_card.Number < 2));
            }
            else if (guanxing.Key != null && guanxing.Value.Count >= index + 1 - player.GetPile("incantation").Count && guanxing_dts.Contains(card))
                return (IsFriend(guanxing.Key, player) && !expect_effect) || (IsEnemy(guanxing.Key, player) && expect_effect);

            return false;
        }

        public bool NeedKongcheng(Player player)
        {
            if (HasSkill("kongcheng", player) && player.HandcardNum <= 1)
            {
                List<Player> enemies = new List<Player>();
                Player next = room.GetNextAlive(room.Current);
                while (next != player)
                {
                    if (IsEnemy(next, player) && !WillSkipPlayPhase(next) && RoomLogic.CanSlash(room, next, player))
                        enemies.Add(next);
                    next = room.GetNextAlive(next);
                }
                if (enemies.Count > 0 && player.IsKongcheng()) return true;
                if (enemies.Count > player.Hp + 1)
                {
                    bool garbage = true;
                    foreach (int id in GetKnownCards(player))
                        if (IsCard(id, "Jink", player, self) || IsCard(id, "Peach", player, self) || IsCard(id, "Analeptic", player, self))
                            garbage = false;
                    if (garbage) return true;
                }
            }

            if (HasSkill("hengzheng", player, true) && player.Hp != 1 && !WillSkipDrawPhase(player))
            {
                int friends = 0;
                int others = 0;
                Player erzhang = null;
                List<Player> enemies = new List<Player>();
                foreach (Player p in room.GetOtherPlayers(player))
                {
                    if (IsFriend(player, p))
                    {
                        if (p.JudgingArea.Count > 0 || GetLostEquipEffect(p) < 0)
                            friends += 1;
                    }
                    else
                    {
                        if (!p.IsNude())
                            others = others + 1;
                    }
                    if (IsEnemy(p, player) && HasSkill("guzheng", p)) erzhang = p;
                }
                if (others - friends > 2 + player.HandcardNum && (!WillSkipPlayPhase(player) || others + friends <= 2 || erzhang == null))
                {
                    Player next = room.GetNextAlive(room.Current);
                    while (next != player)
                    {
                        if (IsEnemy(next, player) && !WillSkipPlayPhase(next) && RoomLogic.CanSlash(room, next, player))
                            enemies.Add(next);
                        next = room.GetNextAlive(next);
                    }
                    if (player.Hp > enemies.Count + 2) return true;
                }
            }
            return false;
        }

        public bool NeedToLoseHp(DamageStruct damage, bool passive, bool recover)
        {
            ScoreStruct score = GetDamageScore(damage);
            if (score.Score < -6 || score.Damage.Damage != 1) return false;

            int n = damage.To.Hp;
            if (recover)
                return n == damage.To.MaxHp;

            if (!passive)
            {
                if (!damage.To.IsWounded() && HasSkill("rende", damage.To) && !WillSkipPlayPhase(damage.To) && friends[damage.To].Count > 1
                        && ((room.Current == damage.To && damage.To.Phase <= PlayerPhase.Play
                        && damage.To.GetMark("rende") < 3 && damage.To.HandcardNum >= 2 - damage.To.GetMark("rende"))
                            || (damage.To.Phase == PlayerPhase.NotActive && MaySave(damage.To))))
                    return true;
                else if (!damage.To.IsWounded() && HasSkill("jieyin", damage.To) && !WillSkipPlayPhase(damage.To) && ((room.Current == damage.To
                        && damage.To.Phase <= PlayerPhase.Play && !damage.To.HasUsed("JieyinCard") && damage.To.HandcardNum >= 2)
                            || (damage.To.Phase == PlayerPhase.NotActive && MaySave(damage.To))))
                {
                    foreach (Player p in friends[damage.To])
                        if (p != damage.To && p.Alive && p.IsMale() && p.IsWounded())
                            return true;
                }
                else if (HasSkill("hengzheng", damage.To) && SimpleGuixinInvoke(damage.To) && n == 2)
                    return true;
                else if (HasSkill("hunshang", damage.To) && n == 2 && MaySave(damage.To))
                    return true;
                else if (HasSkill("zaiqi", damage.To) && MaySave(damage.To) && !WillSkipDrawPhase(damage.To) && n >= 2 && damage.To.GetLostHp() >= 2)
                    return true;
            }
            if (HasSkill("jliegong", damage.To)) return n - score.Damage.Damage == 3;

            Player xiangxiang = RoomLogic.FindPlayerBySkillName(room, "jieyin");
            if (xiangxiang != null && xiangxiang.IsWounded() && IsFriend(xiangxiang, damage.To) && !damage.To.IsWounded() && damage.To.IsMale() && xiangxiang != damage.To
                    && !WillSkipPlayPhase(xiangxiang) && ((room.Current == xiangxiang && xiangxiang.Phase <= PlayerPhase.Play
                    && !xiangxiang.HasUsed("JieyinCard") && xiangxiang.HandcardNum >= 2) || xiangxiang.Phase == PlayerPhase.NotActive))
            {
                bool wounded = false;
                foreach (Player p in friends[xiangxiang])
                    if (p != xiangxiang && p.Alive && p.IsMale() && p.IsWounded())
                        wounded = true;
                if (wounded) return true;
            }

            return false;
        }

        public bool SimpleGuixinInvoke(Player player)
        {
            if (!WillSkipDrawPhase(player))
            {
                int friends = 0;
                int others = 0;
                Player erzhang = null;
                foreach (Player p in room.GetOtherPlayers(player))
                {
                    if (IsFriend(player, p))
                    {
                        if (p.JudgingArea.Count > 0 || GetLostEquipEffect(p) < 0)
                            friends += 1;
                    }
                    else
                    {
                        if (!p.IsNude())
                            others = others + 1;
                    }
                    if (IsEnemy(p, player) && HasSkill("guzheng", p)) erzhang = p;
                }
                if (others - friends > 2 + player.HandcardNum && (!WillSkipPlayPhase(player) || others + friends <= 2 || erzhang == null))
                    return MaySave(player);
            }

            return false;
        }
        public int DamageEffect(DamageStruct damage)
        {
            Player to = damage.To;
            Player from = damage.From;
            foreach (SkillEvent e in Engine.GetSkillEvents())
                if (e != null) e.DamageEffect(this, ref damage);

            if (damage.Card != null)
            {
                UseCard e = Engine.GetCardUsage(damage.Card.Name);
                if (e != null) e.DamageEffect(this, ref damage);
            }

            if (from != null && HasSkill("mingshi", to) && !from.HasShownAllGenerals())
            {
                string skill_name;
                if (damage.Card != null)
                    skill_name = damage.Card.GetSkillName();
                else
                    skill_name = damage.Reason;
                if (!from.HasShownOneGeneral() || !RoomLogic.PlayerHasSkill(room, from, skill_name) || RoomLogic.PlayerHasShownSkill(room, from, skill_name))
                    damage.Damage = damage.Damage - 1;
            }

            if (damage.Damage > 0 && damage.Nature == DamageStruct.DamageNature.Fire && HasArmorEffect(to, "Vine")) damage.Damage += 1;
            if (damage.Damage > 1 && HasArmorEffect(to, "SilverLion")) damage.Damage = 1;
            if (damage.Nature != DamageStruct.DamageNature.Normal && HasArmorEffect(to, "PeaceSpell")) damage.Damage = 0;
            if (damage.Damage >= to.Hp && HasArmorEffect(to, "BreastPlate")) damage.Damage = 0;

            return damage.Damage;
        }
        public bool MaySave(Player player)
        {
            Player next = room.GetNextAlive(room.Current);
            List<Player> enemies = new List<Player>();
            while (next != player)
            {
                if (IsEnemy(next, player) && !WillSkipPlayPhase(next) && RoomLogic.CanSlash(room, next, player))
                    enemies.Add(next);
                next = room.GetNextAlive(next);
            }
            if (enemies.Count == 0) return true;

            return false;
        }

        public int GetOverflow(Player player)
        {
            int MaxCards = RoomLogic.GetMaxCards(room, player);
            if (HasSkill("qiaobian", player) && !player.HasFlag("AI_ConsideringQiaobianSkipDiscard"))
            {
                MaxCards = Math.Max(player.HandcardNum - 1, MaxCards);
                player.SetFlags("-AI_ConsideringQiaobianSkipDiscard");
            }

            return player.HandcardNum - MaxCards;
        }

        public WrappedCard GetSameEquip(WrappedCard card, Player player)
        {
            if (card == null) return null;
            FunctionCard fcard = Engine.GetFunctionCard(card.Name);
            if (fcard is Weapon)
                return room.GetCard(player.Weapon.Key);
            else if (fcard is Armor)
                return room.GetCard(player.Armor.Key);
            else if (fcard is DefensiveHorse)
                return room.GetCard(player.DefensiveHorse.Key);
            else if (fcard is OffensiveHorse)
                return room.GetCard(player.OffensiveHorse.Key);
            else if (fcard is Treasure)
                return room.GetCard(player.Treasure.Key);

            return null;
        }
        public virtual double GetDefensePoint(Player player, bool for_attack = false)
        {
            double defense = 0;
            if (!for_attack)
            {
                defense += player.GetEquips().Count * 0.4;
                if (player.GetArmor())
                    defense += 0.5;
                if (player.GetDefensiveHorse())
                    defense += 0.5;
            }
            else
            {
                if (player.GetArmor())
                    defense += 1;
                if (player.GetDefensiveHorse())
                    defense += 0.8;
            }

            defense += player.HandcardNum * 0.35;
            defense += player.GetPile("wooden_ox").Count * 0.25;
            if (!for_attack)
            {
                if (player.GetTreasure() && player.Treasure.Value == "LuminouSpearl")
                    defense += 1;
                if (player.GetTreasure() && player.Treasure.Value == "JadeSeal")
                    defense += 1;
            }

            double hp = player.Hp * 2;

            return defense * 0.8 + hp * 0.2;
        }
        public List<Player> GetValuableEnemies()
        {
            List<Player> result = new List<Player>(priority_enemies), other_enemies = new List<Player>();
            result.Sort((x, y) => { return GetDefensePoint(x) < GetDefensePoint(y) ? -1 : 1; });

            foreach (Player p in enemies[self])
                if (!result.Contains(p))
                    other_enemies.Add(p);

            other_enemies.Sort((x, y) => { return GetDefensePoint(x) < GetDefensePoint(y) ? -1 : 1; });

            foreach (Player p in other_enemies)
                result.Add(p);

            return result;
        }

        public double GetUseValue(int id, Player player, Place place = Place.PlaceUnknown)
        {
            if (place == Place.PlaceUnknown) place = room.GetCardPlace(id);
            List <WrappedCard> cards = GetViewAsCards(player, id);
            double value = 0;
            foreach (WrappedCard card in cards) {
                double card_value = GetUseValue(card, player, place);
                if (card_value > value)
                    value = card_value;
            }

            return value;
        }

        public double GetUseValue(WrappedCard card, Player player, Place place = Place.PlaceUnknown)
        {
            double basic = Engine.GetCardUseValue(card.Name);
            FunctionCard fcard = Engine.GetFunctionCard(card.Name);
            List<int> ids = new List<int>();
            if (fcard is SkillCard)
                return basic;
            else if (fcard is EquipCard)
            {
                int id = card.Id;
                UseCard card_event = Engine.GetCardUsage(card.Name);
                if (card_event != null)
                    basic += card_event.CardValue(this, player, card, Place.PlaceEquip);

                if (room.GetCardOwner(id) == player && room.GetCardPlace(id) == Place.PlaceEquip)
                {
                    return basic;
                }
                else
                {
                    WrappedCard same = GetSameEquip(card, player);
                    if (same != null)
                        basic -= GetKeepValue(same.Id, player);
                }
            }
            else
                ids = new List<int>(card.SubCards);

            double handcard_ajust = 0;
            int handcard_count = 0;
            foreach (int id in ids)
            {
                WrappedCard equip = room.GetCard(id);
                UseCard e = Engine.GetCardUsage(equip.Name);
                if (e != null && room.GetCardOwner(id) == player && room.GetCardPlace(id) == Place.PlaceEquip)
                    basic -= GetKeepValue(id, player);
                else if (room.GetCardOwner(id) == player && room.GetCardPlace(id) == Place.PlaceHand || place == Place.PlaceHand)
                {
                    handcard_ajust += GetKeepValue(id, player, Place.PlaceHand);
                    handcard_count++;
                }
            }

            if (GetOverflow(player) < handcard_count)
                basic -= (handcard_count - GetOverflow(player)) / handcard_count * handcard_ajust;

            foreach (SkillEvent e in Engine.GetSkillEvents())
                basic += e.CardValue(this, player, card, true, place);

            return basic;
        }

        public double GetKeepValue(int id, Player player, Place place = Place.PlaceUnknown)
        {
            if (place == Place.PlaceUnknown && room.GetCardOwner(id) == player)
                place = room.GetCardPlace(id);

            List <WrappedCard> cards = GetViewAsCards(player, id, place);
            List<double> sum = new List<double>();
            foreach (WrappedCard card in cards) {
                double card_v = GetKeepValue(card, player, place);
                sum.Add(card_v);
            }
            sum.Sort((x, y) => { return x < y ? -1 : 1; });
            double value = sum[sum.Count - 1];
            sum.RemoveAt(sum.Count - 1);
            for (int i = sum.Count - 1; i >= 0; i--)
                value += sum[i] / (sum.Count - i + 2);

            return value;
        }

        public double GetKeepValue(WrappedCard card, Player player, Place place = Place.PlaceUnknown)
        {
            if (RoomLogic.IsVirtualCard(room, card) && card.SubCards.Count != 1)
            {
                string skill_name = card.GetSkillName();
                if (!string.IsNullOrEmpty(skill_name))
                {
                    SkillEvent e = Engine.GetSkillEvent(skill_name);
                    if (e != null) return e.CardValue(this, player, card, false, Place.PlaceUnknown);
                }

                return 0;
            }

            int id = card.GetEffectiveId();
            if (place == Place.PlaceUnknown && room.GetCardOwner(id) == player)
                place = room.GetCardPlace(id);

            double basic = Engine.GetCardKeepValue(card.Name);
            FunctionCard fcard = Engine.GetFunctionCard(card.Name);
            if (place == Place.PlaceEquip)
            {
                WrappedCard equip = room.GetCard(id);
                UseCard e = Engine.GetCardUsage(equip.Name);
                if (e!= null)
                    basic += e.CardValue(this, player, card, place);
            }
            else if (fcard is EquipCard)
            {
                WrappedCard same = GetSameEquip(card, player);
                if (same != null)
                    basic -= GetKeepValue(same, player);
            }

            foreach (string skill in GetKnownSkills(player)) {
                SkillEvent e = Engine.GetSkillEvent(skill);
                if (e != null)
                    basic += e.CardValue(this, player, card, false, place);
            }

            return basic;
        }

        public double CaculateSameClass(string class_name, List<string> class_names)
        {
            double dec = 0;
            double basic = Engine.GetCardKeepValue(class_name);
            if (class_name.Contains("Slash")) class_name = "Slash";
            if (class_name.Contains("Slash") || class_name == "Jink" || class_name == "Peach" || class_name == "Analeptic")
            {
                int n = 0;
                foreach (string name in class_names)
                    if (name == class_name)
                        n++;
                if (n > 0)
                    dec = basic - basic / 2 / n;
            }

            return dec;
        }

        public List<double> SortByUseValue(ref List<WrappedCard> cards, bool descending = true)
        {
            Dictionary<WrappedCard, double> card_values = new Dictionary<WrappedCard, double>();
            List <WrappedCard> results = new List<WrappedCard>();

            foreach (WrappedCard card in cards)
                card_values[card] = GetUseValue(card, self);

            while (results.Count < cards.Count)
            {
                WrappedCard best = null;
                double best_value = 0;
                foreach (WrappedCard card in card_values.Keys) {
                    double card_value = card_values[card];
                    if (card_value > best_value)
                    {
                        best = card;
                        best_value = card_value;
                    }
                }

                if (descending)
                    results.Add(best);
                else
                    results.Insert(0, best);

                card_values.Remove(best);
            }

            cards = results;
            List<double> points = new List<double>();
            foreach (WrappedCard card in cards)
                points.Add(card_values[card]);

            return points;
        }

        public List<double> SortByUseValue(ref List<int> ids, bool descending = true)
        {
            Dictionary<int, double> card_values = new Dictionary<int, double>();
            List<int> results = new List<int>();

            foreach (int id in ids)
                card_values[id] = GetUseValue(id, self);


            while (results.Count < ids.Count)
            {
                int best = -1;
                double best_value = 0;
                foreach (int id in card_values.Keys) {
                    double card_value = card_values[id];
                    if (card_value > best_value)
                    {
                        best = id;
                        best_value = card_value;
                    }
                }

                if (descending)
                    results.Add(best);
                else
                    results.Insert(0, best);

                card_values.Remove(best);
            }

            ids = results;
            List<double> points = new List<double>();
            foreach (int id in ids)
                points.Add(card_values[id]);

            return points;
        }

        public List<double> SortByKeepValue(ref List<int> ids, bool descending = true)
        {
            Dictionary<WrappedCard, double> card_values = new Dictionary<WrappedCard, double>();
            Dictionary<int, double> card_values_ajust = new Dictionary<int, double>();
            Dictionary<int, List<WrappedCard>> int2cards = new Dictionary<int, List<WrappedCard>>();
            List<int> results = new List<int>();
            List<string> class_names = new List<string>();

            foreach (int id in ids)
            {
                Place place = room.GetCardPlace(id);
                List<WrappedCard> cards = GetViewAsCards(self, id, place);
                int2cards[id] = cards;
                foreach (WrappedCard card in cards)
                    card_values[card] = GetKeepValue(card, self, place);
            }

            while (results.Count < ids.Count)
            {
                WrappedCard best = null;
                double best_value = 0;
                foreach (WrappedCard card in card_values.Keys)
                {
                    double card_value = card_values[card];
                    card_value -= CaculateSameClass(card.Name, class_names);
                    if (card_value > best_value)
                    {
                        best = card;
                        best_value = card_value;
                    }
                }

                if (best.Name.Contains("Slash"))
                    class_names.Add("Slash");
                else
                    class_names.Add(best.Name);

                int id = best.GetEffectiveId();
                card_values_ajust[id] = best_value;

                if (descending)
                    results.Add(id);
                else
                    results.Insert(0, id);

                foreach (WrappedCard card in int2cards[id])
                    card_values.Remove(card);
            }

            ids = results;

            List<double> points = new List<double>();
            foreach (int id in ids)
                points.Add(card_values_ajust[id]);

            return points;
        }

        public void SortByKeepValue(ref List<WrappedCard> cards, bool descending = true)
        {
            Dictionary<WrappedCard, double> values = new Dictionary<WrappedCard, double>();
            foreach (WrappedCard card in cards)
                values.Add(card, GetKeepValue(card, self));

            cards.Sort((x, y) =>
            {
                return values[x] < values[y] ? descending ? 1 : -1 : descending ? -1 : 1;
            });
        }

        public void SortByDefense(ref List<Player> players, bool descending = true)
        {
            Dictionary<Player, double> values = new Dictionary<Player, double>();
            foreach (Player p in players)
                values[p]= GetDefensePoint(p, true);

            players.Sort((x, y) =>
            {
                if (values[x] != values[y])
                    return (values[x] < values[y]) ? descending ? 1 : -1 : descending ? -1 : 1;
                else if (x.Hp != y.Hp)
                    return (x.Hp < y.Hp) ? descending ? 1 : -1 : descending ? -1 : 1;
                else
                    return x.HandcardNum < y.HandcardNum ? descending ? 1 : -1 : descending ? -1 : 1;
            });
        }

        public bool HasCrossbowEffect(Player player)
        {
            WrappedCard slash = new WrappedCard("Slash");
            if (Engine.CorrectCardTarget(room, TargetModSkill.ModType.Residue, player, slash) > 5 || GetCards("CrossBow", player).Count > 0)
                return true;

            return false;
        }
        public int PlayerGetRound(Player player, Player source = null)
        {
            if (source == null) source = room.Current;
            if (player == source) return 0;
            int players_num = room.AliveCount();
            int round = (player.Seat - source.Seat) % players_num;
            return round;
        }
        public bool ToTurnOver(Player player, int n = 1, string reason = null)
        {
            if (!player.FaceUp) return false;
            if (!string.IsNullOrEmpty(reason) && reason == "fangzhu" && player.Hp == 1 && ai_AOE_data.Card != null)
                if (ai_AOE_data.To.Contains(player) && IsCardEffect(ai_AOE_data.Card, player, ai_AOE_data.From)
                    && PlayerGetRound(player) > PlayerGetRound(self) && player.IsKongcheng())
                    return false;

            if (n > 1)
                if ((player.Phase < PlayerPhase.Discard && (HasSkill(Engine.GetSkills("Active_cardneed"), player) || HasCrossbowEffect(player)))
                    || (player.Phase == PlayerPhase.NotActive && HasSkill(Engine.GetSkills("notActive_cardneed"), player)))
                    return false;

            if (HasSkill("jushou", player) && player.Phase <= PlayerPhase.Finish) return false;
            return true;
        }

        public int ImitateResult_DrawNCards(Player player, List<string> skills = null)
        {
            if (player.IsSkipped(PlayerPhase.Draw)) return 0;
            if (skills == null || skills.Count == 0)
                skills = GetKnownSkills(player);

            int count = 2;
            if (player.HasTreasure("JadeSeal") && player.HasShownOneGeneral()) count++;
            foreach (string skill in skills)
            {
                if (skill == "luoyi") count--;
                if (skill.Contains("yingzi")) count++;
            }

            return count;
        }

        public bool DontRespondPeachInJudge(JudgeStruct judge)
        {
            int peach_num = GetCards("Peach", self).Count;
            if (peach_num == 0) return false;

            if (WillSkipPlayPhase(self) && peach_num > GetOverflow(self)) return false;
            if (judge.Reason == "Lightning" && IsFriend(judge.Who)) return false;

            if (peach_num > self.GetLostHp())
            {
                bool weak = false;
                foreach (Player p in friends_noself)
                    if (IsWeak(p)) weak = true;
                if (!weak) return false;
            }

            if (judge.Reason == "EightDiagram" && IsFriend(judge.Who))
            {
                bool effect = true;
                List<CardUseStruct> list = (List<CardUseStruct>)room.GetTag("card_proceeing");
                CardUseStruct use = list[list.Count - 1];
                if (use.To.Contains(judge.Who))
                {
                    DamageStruct damage = new DamageStruct(use.Card, use.From, judge.Who);
                    if (use.Card != null && use.Card.Name == "FireSlash")
                        damage.Nature = DamageStruct.DamageNature.Fire;
                    else if (use.Card != null && use.Card.Name == "ThunderSlash")
                        damage.Nature = DamageStruct.DamageNature.Thunder;

                    ScoreStruct score = GetDamageScore(damage);
                    if (score.Score > -2)
                        effect = true;
                }
                if (effect && IsWeak(judge.Who)) return false;
            }

            return true;
        }
        public bool NeedRetrial(JudgeStruct judge)
        {
            string reason = judge.Reason;
            Player who = judge.Who;
            if (reason == "Lightning")
            {
                DamageStruct damage = new DamageStruct
                {
                    Damage = 3,
                    Nature = DamageStruct.DamageNature.Thunder,
                    To = who
                };
                if (HasSkill("hongyan", who)) return false;
                damage.Damage = DamageEffect(damage);
                if (damage.Damage == 0)
                    return false;
                else
                {
                    if (judge.IsGood() && (IsEnemy(who) && GetDamageScore(damage).Score < 5 || (!IsFriend(who) && IsGoodSreadStarter(damage)))) return true;
                    if (judge.IsBad() && IsFriend(who) && (GetDamageScore(damage).Score < 5 || IsGoodSreadStarter(damage, false))) return true;
                }
            }
            else if (reason == "Indulgence")
            {
                if (who.IsSkipped(PlayerPhase.Draw) && who.IsKongcheng())
                {
                    if (HasSkill("kurou", who) && who.Hp >= 3 && !who.IsNude())
                    {
                        if (IsFriend(who))
                            return !judge.IsGood();
                        else
                            return judge.IsGood();
                    }
                }
                if (IsFriend(who))
                {
                    int drawcardnum = ImitateResult_DrawNCards(who);
                    if (who.Hp - who.HandcardNum >= drawcardnum && GetOverflow(self) < 0) return false;
                    return !judge.IsGood();
                }
                else if (IsEnemy(who) || who.HandcardNum > 5)
                    return judge.IsGood();
            }
            else if (reason == "SupplyShortage")
            {
                if (IsFriend(who))
                {
                    if (HasSkill("guidao|tiandu", who)) return false;
                    return !judge.IsGood();
                }
                else
                    return judge.IsGood();
            }
            else if (reason == "luoshen")
            {
                if (IsFriend(who))
                {
                    if (who.HandcardNum > 10) return false;
                    if (HasCrossbowEffect(who))
                        return !judge.IsGood();
                    if (GetOverflow(who) > 1 && self.HandcardNum < 3 && room.GetNextAlive(who) != self) return false;
                    return !judge.IsGood();
                }
                else
                    return judge.IsGood();

            }
            else if (reason == "tuntian")
            {
                if (IsFriend(who))
                {
                    if (self == who)
                        return !IsWeak(self);
                    else
                        return !judge.IsGood() && !HasSkill("tiandu", who);
                }
                else
                    return judge.IsGood();
            }
            else if (reason == "beige")
                return true;

            if (IsFriend(who))
                return !judge.IsGood();
            else if (IsEnemy(who))
                return judge.IsGood();
            else
                return false;
        }

        public int GetRetrialCardId(List<int> card_ids, JudgeStruct judge, bool self_card = true)
        {
            List<int> can_use = new List<int>(), other_suit = new List<int>();
            bool hasSpade = false;
            string reason = judge.Reason;
            Player who = judge.Who;

            foreach (int id in card_ids) {
                WrappedCard card_x = Engine.GetRealCard(id);
                if (RoomLogic.IsCardLimited(room, self, room.GetCard(id), HandlingMethod.MethodResponse)) continue;
                bool is_peach = IsFriend(who) && HasSkill("tiandu", who) || IsCard(id, "Peach", who, self);
                if (HasSkill("hongyan", who) && card_x.Suit == WrappedCard.CardSuit.Spade)
                {
                    card_x = Engine.CloneCard(card_x);
                    card_x.SetSuit(WrappedCard.CardSuit.Heart);
                }

                if (reason == "beige" && !is_peach)
                {
                    DamageStruct damage = (DamageStruct)room.GetTag("CurrentDamageStruct");
                    if (damage.From != null)
                    {
                        if (IsFriend(damage.From))
                        {
                            if (!ToTurnOver(damage.From) && judge.Card.Suit != WrappedCard.CardSuit.Spade && card_x.Suit == WrappedCard.CardSuit.Spade)
                            {
                                can_use.Add(id);
                                hasSpade = true;
                            }
                            else if ((!self_card || GetOverflow(self) > 0) && judge.Card.Suit != card_x.Suit)
                            {
                                bool retr = true;
                                if ((judge.Card.Suit == WrappedCard.CardSuit.Heart && who.IsWounded() && IsFriend(who))
                                    || (judge.Card.Suit == WrappedCard.CardSuit.Club && GetLostEquipEffect(damage.From) < 0))
                                    retr = false;
                                if (retr && ((IsFriend(who) && card_x.Suit == WrappedCard.CardSuit.Heart && who.IsWounded())
                                        || (card_x.Suit == WrappedCard.CardSuit.Club && (GetLostEquipEffect(damage.From) < 0 || damage.From.IsNude())))
                                        || (judge.Card.Suit == WrappedCard.CardSuit.Spade && ToTurnOver(damage.From)))
                                    other_suit.Add(id);
                            }
                        }
                        else
                        {
                            if (!ToTurnOver(damage.From) && card_x.Suit != WrappedCard.CardSuit.Spade && judge.Card.Suit == WrappedCard.CardSuit.Spade)
                                can_use.Add(id);
                        }
                    }
                }
                else if (IsFriend(who) && judge.Good == new ExpPattern(judge.Pattern).Match(judge.Who, room, card_x) && reason != "jganglie"
                           && !(self_card && DontRespondPeachInJudge(judge) && is_peach))
                    can_use.Add(id);
                else if (IsEnemy(who) && judge.Good != new ExpPattern(judge.Pattern).Match(judge.Who, room, card_x) && reason != "jganglie"
                        && !(self_card && DontRespondPeachInJudge(judge) && is_peach))
                    can_use.Add(id);
                if (reason == "jganglie" && !is_peach)
                {
                    DamageStruct damage = (DamageStruct)room.GetTag("CurrentDamageStruct");
                    if (damage.From != null && (!self_card || GetOverflow(self) > 0) && !DontRespondPeachInJudge(judge))
                    {
                        if (WrappedCard.IsRed(judge.Card.Suit) && IsFriend(damage.From))
                            if ((damage.From.Hp == 1 || damage.From.IsNude()) && WrappedCard.IsBlack(card_x.Suit))
                                can_use.Add(id);

                        if (WrappedCard.IsBlack(judge.Card.Suit) && IsEnemy(damage.From))
                            if ((damage.From.Hp == 1 || damage.From.IsNude()) && WrappedCard.IsRed(card_x.Suit))
                                can_use.Add(id);
                    }
                }
            }
            if (!hasSpade && other_suit.Count > 0)
                can_use.AddRange(other_suit);

            if (can_use.Count > 0)
            {
                SortByKeepValue(ref can_use, false);
                return can_use[0];
            }
            else
                return -1;
        }

        public CardUseStruct FindSlashandTarget(Player player)
        {
            List<ScoreStruct> values = CaculateSlashIncome(player);
            double best = 0;
            if (values.Count > 0)
                best = values[values.Count - 1].Score;

            if (HasSkill("keji", player) && !player.HasFlag("KejiSlashInPlayPhase") && GetOverflow(player) > 2)
            {          //keji
                bool multi_slash = false;
                if (GetKnownCardsNums("Slash", "h", player, self) >= GetOverflow(player))
                {
                    WrappedCard slash = new WrappedCard("Slash");
                    if (Engine.CorrectCardTarget(room, TargetModSkill.ModType.Residue, player, slash) >= GetOverflow(player))
                        multi_slash = true;
                    if (!multi_slash && GetCards("CrossBow", player).Count > 0)
                    {
                        foreach (Player p in room.GetOtherPlayers(player)) {
                            if (RoomLogic.DistanceTo(room, player, p) == 1 && !IsFriend(p) && !HasSkill("fankui", p))
                            {
                                multi_slash = true;
                                break;
                            }
                        }
                    }

                    if (!multi_slash && !(process.Contains(">>") && best > 6))
                        return new CardUseStruct();
                }
            }

            CardUseStruct use = new CardUseStruct();
            //check will use analeptic
            if (player.Phase == PlayerPhase.Play && pre_drink == null && room.GetRoomState().GetCurrentCardUseReason() == CardUseStruct.CardUseReason.CARD_USE_REASON_PLAY)
            {
                List <WrappedCard> analeptics = GetCards("Analeptic", player);
                bool will_use = false;
                if (process.Contains(">>") || (analeptics.Count > 1) || (enemies[player].Count == 1 && GetOverflow(player) > 0)) will_use = true;
                FunctionCard fcard = Engine.GetFunctionCard("Analeptic");
                foreach (WrappedCard analeptic in analeptics) {
                    if (fcard.IsAvailable(room, player, analeptic) && (will_use
                            || (RoomLogic.IsVirtualCard(room, analeptic) && analeptic.SubCards.Count == 0 && GetUseValue(analeptic, player) > 0)))
                    {
                        pre_drink = analeptic;
                        double drank_value = 0;
                        List<ScoreStruct> drank_values = CaculateSlashIncome(player);
                        pre_drink = null;
                        if (drank_values.Count > 0)
                            drank_value = drank_values[drank_values.Count - 1].Score;
                        if (drank_value - best >= 6)
                        {
                            use = new CardUseStruct(analeptic, player, new List<Player>());
                            return use;
                        }
                    }
                }
            }

            if (values.Count > 0 && (process.Contains(">>") || values[values.Count - 1].Score >= 6) || (enemies[player].Count == 1 && GetOverflow(player) > 0))
            {
                use.From = player;
                use.To = values[values.Count - 1].Players;
                use.Card = values[values.Count - 1].Card;
            }
            return use;
        }
        private void ClearPreInfo()
        {
            foreach (Player p in room.AlivePlayers)
            {
                pre_discard[p].Clear();
                pre_disable[p] = string.Empty;
            }

            pre_ignore_armor.Clear();
        }

        public List<ScoreStruct> CaculateSlashIncome(Player player, List<WrappedCard> slashes = null, List<Player> targets = null)
        {
            List<ScoreStruct> values = new List<ScoreStruct>();
            if (slashes == null || slashes.Count == 0)
                slashes = GetCards("Slash", player);

            List<WrappedCard> cards = new List<WrappedCard>();
            FunctionCard fcard = Engine.GetFunctionCard("Slash");
            foreach (WrappedCard slash in slashes) {
                if (!slash.Name.Contains("Slash") || !fcard.IsAvailable(room, player, slash)) continue;
                cards.Add(slash);
                if (player.HasWeapon("Fan") && slash.Name == "slash" && !slash.SubCards.Contains(player.Weapon.Key))
                {
                    WrappedCard fire_slash = new WrappedCard("FireSlash");
                    fire_slash.AddSubCard(slash);
                    fire_slash = RoomLogic.ParseUseCard(room, fire_slash);
                    fire_slash.UserString = RoomLogic.CardToString(room, slash);
                    cards.Add(fire_slash);
                }
            }

            if (targets == null || targets.Count == 0)
                targets = room.GetOtherPlayers(player);

            foreach (WrappedCard slash in cards)
            {
                if (!fcard.IsAvailable(room, player, slash)) continue;
                double value = GetUseValue(slash, self);

                int extra = Engine.CorrectCardTarget(room, TargetModSkill.ModType.ExtraMaxTarget, player, slash);
                Dictionary<Player, ScoreStruct> available_targets = new Dictionary<Player, ScoreStruct>();
                foreach (Player target in targets)
                {
                    ClearPreInfo();
                    if (!RoomLogic.CanSlash(room, player, target) || SlashProhibit(slash, target, player)) continue;
                    available_targets.Add(target, SlashIsEffective(slash, target));
                }

                //auto lambda = [&available_targets](ServerPlayer *p1, ServerPlayer *p2) {
                //    return available_targets[p1] > available_targets(p2);
                //};
                //qSort(target.begin(), target.end(), lambda);

                extra = Math.Min(available_targets.Keys.Count, extra + 1);
                int z = 1;
                for (int m = available_targets.Keys.Count; m > 0; m--)
                    z *= m;

                List<string> uses = new List<string>();
                List<Player> _targets = new List<Player>();
                for (int i = 1; i <= (extra + 1); i++)
                {
                    int max = z;
                    for (int x = (available_targets.Keys.Count - i); x > 1; x--)
                        max = max / x;
                    for (int n = i; n > 1; n--)
                        max = max / n;

                    while (true)
                    {
                        foreach (Player p in available_targets.Keys)
                        {
                            bool check = true;
                            _targets.Add(p);
                            int count = 0;
                            foreach (string list in uses)
                            {
                                if (list.Split('+').Length == i)
                                {
                                    count++;
                                    if (_targets.Count == i)
                                    {
                                        bool same = true;
                                        foreach (Player _p in _targets)
                                        {
                                            if (!list.Contains(_p.Name))
                                                same = false;
                                        }
                                        check = !same;
                                    }
                                }
                            }
                            if (check)
                            {
                                List<string> names = new List<string>();
                                foreach (Player t in targets)
                                    names.Add(t.Name);
                                uses.Add(string.Join("+", names));
                                targets.Clear();
                                break;
                            }
                            else if (_targets.Count == i)
                                _targets.RemoveAt(_targets.Count - 1);

                            if (count >= max) break;
                        }
                    }
                }

                foreach (string name_string in uses)
                {
                    _targets.Clear();
                    foreach (string name in name_string.Split('+'))
                        _targets.Add(room.FindPlayer(name));

                    room.SortByActionOrder(ref _targets);

                    bool chain = false;
                    double use_value = 0;
                    foreach (Player target in _targets) {
                        ScoreStruct score = available_targets[target];
                        if (score.Score == 0) continue;
                        use_value += score.Score;
                        if (IsFriend(target) && target.Chained && chain && score.Damage.Damage > 0)
                            use_value -= 10;

                        if (IsGoodSreadStarter(score.Damage) && score.Info != "miss" && score.Rate > 0.5 && !chain)
                        {
                            chain = true;
                            use_value += 6;
                        }
                        else if (!IsGoodSreadStarter(score.Damage) && score.Damage.Damage > 0 && !chain
                                 && target.Chained && score.Info != "miss" && score.Damage.Nature != DamageStruct.DamageNature.Normal)
                        {
                            chain = true;
                            use_value -= 6;
                        }
                    }
                    if (use_value != 0) use_value += value;

                    if (targets.Count == 1 && !IsFriend(targets[0]) && use_value >= 4)
                    {            //yuji?
                        bool yuji = false;
                        foreach (Player p in room.AlivePlayers)
                    if (HasSkill("qianhuan", p) && RoomLogic.IsFriendWith(room, p, _targets[0]) && p.GetPile("sorcery").Count > 0)
                            yuji = true;

                        List<Player> targets_const = new List<Player>
                        {
                            _targets[0]
                        };
                        if (yuji)
                        {
                            bool extra_target = false;
                            if (HasSkill("duanbing", player))
                            {
                                List<Player> duanbings = new List<Player>();
                                foreach (Player p in room.GetOtherPlayers(player))
                            if (!targets.Contains(p) && fcard.ExtratargetFilter(room, targets_const, p, player, slash) && RoomLogic.DistanceTo(room, player, p, slash) == 1)
                                    duanbings.Add(p);

                                foreach (Player p in duanbings)
                            if (SlashIsEffective(slash, p).Score >= 0)
                                        extra_target = true;
                            }
                            if (!extra_target && player.HasFlag("TianyiSuccess"))
                            {
                                List<Player> tianyis = new List<Player>();
                                foreach (Player p in room.GetOtherPlayers(player))
                            if (!targets.Contains(p) && fcard.ExtratargetFilter(room, targets_const, p, player, slash))
                                    tianyis.Add(p);

                                foreach (Player p in tianyis)
                            if (SlashIsEffective(slash, p).Score >= 0)
                                        extra_target = true;
                            }
                            if (!extra_target && player.HasWeapon("Halberd") && !slash.SubCards.Contains(player.Weapon.Key))
                            {
                                List<Player> halberds = new List<Player>();
                                foreach (Player p in room.GetOtherPlayers(player))
                            if (!targets.Contains(p) && fcard.ExtratargetFilter(room, targets_const, p, player, slash) && !RoomLogic.IsFriendWith(room, p, targets[0]))
                                    halberds.Add(p);

                                foreach (Player p in halberds)
                            if (SlashIsEffective(slash, p).Score >= 0)
                                        extra_target = true;
                            }

                            if (!extra_target) use_value = 3;
                        }
                    }

                    ScoreStruct result_score = new ScoreStruct
                    {
                        Card = slash,
                        Players = targets,
                        Score = use_value
                    };
                    values.Add(result_score);
                }
            }

            CompareByScore(ref values);
            return values;
        }

        public bool SlashProhibit(WrappedCard card, Player enemy, Player from)
        {
            if (enemy.Removed) return true;
            card = card ?? new WrappedCard("Slash");
            from = from ?? self;

            foreach (string skill in GetKnownSkills(enemy)) {           // todo
                SkillEvent e = Engine.GetSkillEvent(skill);
                if (e!=null && e.IsProhibit(this, from, enemy, card)) return true;
            }

            return false;
        }

        public ScoreStruct SlashIsEffective(WrappedCard card, Player to)
        {
            Player from = self;

            if (from.HasWeapon("Blade"))
            {
                if (!to.General1Showed)
                    pre_disable[to] = pre_disable[to] + "h";
                if (!string.IsNullOrEmpty(to.General2) && !to.General2Showed)
                    pre_disable[to] = pre_disable[to] + "d";
            }

            if (IsCancelTarget(card, to, from)) return new ScoreStruct();

            ScoreStruct result_score = new ScoreStruct();
            double result = 0;
            if (from.HasWeapon("QinggangSword"))
                pre_ignore_armor.Add(to);
            if (from.HasWeapon("DragonPhoenix") && !IsFriend(from, to) && to.GetArmor() && to.GetCards("he").Count == 1)
                pre_discard[to].Add(to.Armor.Key);

            if (from.HasWeapon("DoubleSword") && (from.IsMale() && to.IsFemale() || (from.IsFemale() && to.IsMale())))
                result += 3;

            if (from.HasWeapon("DragonPhoenix") && !to.IsNude())
            {
                if (!IsFriend(from, to) && GetLostEquipEffect(to) == 0)
                    result += 3;
                else if (IsFriend(from, to))
                    result -= GetLostEquipEffect(to);
            }

            if (!IsCardEffect(card, to, from))
            {                            // card will be nullified
                result_score.Score = result;
                return result_score;
            }

            //evaluate hit rate
            double jink = 0;
            double force_hit = 0;
            bool no_red = to.GetMark("@qianxi_red") > 0;
            bool no_black = to.GetMark("@qianxi_black") > 0;
            if (!IsFriend(to, from))
            {
                int jink_need = HasSkill("wushuang", from) ? 2 : 1;
                if (HasSkill("tieqi", from)) force_hit = 6 / 10;
                if (HasSkill("jianchu", from) && to.HasEquip()) force_hit = 1;
                if (HasSkill("liegong", from) && from.Phase == PlayerPhase.Play)
                {
                    bool weapon = from.GetWeapon() && !card.SubCards.Contains(from.Weapon.Key);
                    if (to.HandcardNum >= from.Hp || to.HandcardNum <= RoomLogic.GetAttackRange(room, from, weapon))
                        force_hit = 1;
                }
                if (force_hit < 1 && HasArmorEffect(to, "EightDiagram"))
                {
                    if (HasSkill("tiandu+qingguo", to) && !no_black)
                        jink = jink_need;
                    else
                    {
                        Player winner = GetWizzardRaceWinner("EightDiagram", to);
                        if (winner == null || !IsFriend(winner))
                            jink = 4 / 10;
                        if (winner != null && IsFriend(winner, to))
                            jink = jink_need;
                    }
                    if (HasSkill("tiandu", to))
                        result -= 3 * jink_need;
                }

                foreach (WrappedCard jink_card in GetCards("Jink", to))
                    if (!RoomLogic.IsCardLimited(room, to, jink_card, HandlingMethod.MethodUse)) jink = jink + 1;
                if (!IsLackCard(to, "jink"))
                {
                    int rate = 5;
                    if (HasSkill("longdan|qingguo", to)) rate = 2;
                    jink += (to.GetHandPile(true).Count - GetKnownHandPileCards(to).Count) / rate;
                    if (GetKnownCards(to).Count != to.HandcardNum)
                    {
                        rate = 6;
                        if (HasSkill("longdan", to))
                        {
                            rate -= 3;
                            if (no_black)
                                rate += 2;
                        }
                        if (HasSkill("qingguo", to) && !no_black)
                            rate -= 2;
                        int count = to.HandcardNum - GetKnownCards(to).Count;
                        if (no_red)
                        {
                            if (rate == 6)
                                count = 0;
                            else
                                rate += 5;
                        }
                        jink += count / rate;
                    }
                }

                if (force_hit < 1 && jink > 0.3 && NotSlashJiaozhu(to))
                {
                    result_score.Score = -20;
                    result_score.Info = "no";
                    return result_score;
                }

                double dodge = (jink - jink_need + 1 >= 1) ? 1 : (jink - jink_need + 1);
                if (force_hit < 1 && from.HasWeapon("Axe") && !card.SubCards.Contains(from.Weapon.Key))
                {
                    List<int> ids = card.SubCards;
                    if (pre_drink != null)
                        ids.AddRange(pre_drink.SubCards);

                    int count = 0;
                    foreach (int id in ids)
                        if (room.GetCardOwner(id) == from)
                            count++;
                    if (RoomLogic.GetPlayerCards(room, from, "he").Count - count > 2)
                    {
                        double rate = (1 - force_hit) * dodge;
                        force_hit = 1;
                        result -= 3 * rate;
                    }
                }

                if (force_hit == 0 && dodge >= 1)
                {
                    if (HasSkill("mengjin", from) && !to.IsNude())
                        result += 3;
                    result_score.Score = result + jink_need * 3;
                    result_score.Info = "miss";
                    return result_score;
                }

                jink = dodge;
            }
            else if (JiaozhuneedSlash(to))
            {
                int jink_need = RoomLogic.PlayerHasShownSkill(room, from, "wushuang") ? 2 : 1;
                if (HasArmorEffect(to, "EightDiagram"))
                {
                    if (HasSkill("tiandu+qingguo", to))
                        jink = jink_need;
                    else
                    {
                        Player winner = GetWizzardRaceWinner("EightDiagram", to);
                        if (winner == null || !IsEnemy(winner))
                            jink = 4 / 10;
                        if (winner != null && IsFriend(winner, to))
                            jink = jink_need;
                    }
                    if (HasSkill("tiandu", to))
                        result += 3 * (jink_need > 1 ? 1.5 : 1);
                }

                List<WrappedCard> jinks = GetCards("Jink", to);
                foreach (WrappedCard jink_card in jinks)
                    if (!RoomLogic.IsCardLimited(room, to, jink_card, HandlingMethod.MethodUse)) jink = jink + 1;
                if (!IsLackCard(to, "jink"))
                {
                    int rate = 5;
                    if (HasSkill("longdan|qingguo", to)) rate = 2;
                    jink += (to.GetHandPile(true).Count - GetKnownHandPileCards(to).Count) / rate;
                    if (GetKnownCards(to).Count != to.HandcardNum)
                    {
                        rate = 6;
                        if (HasSkill("longdan", to))
                        {
                            rate -= 3;
                            if (no_black)
                                rate += 2;
                        }
                        if (HasSkill("qingguo", to) && !no_black)
                            rate -= 2;
                        int count = to.HandcardNum - GetKnownCards(to).Count;
                        if (no_red)
                        {
                            if (rate == 6)
                                count = 0;
                            else
                                rate += 5;
                        }
                        jink += count / rate;
                    }
                }

                if (jink - jink_need > 0)
                {
                    result_score.Info = "miss";
                    result_score.Score = 10 * jink_need + result;
                    return result_score;
                }
            }

            DamageStruct damage = new DamageStruct(card, from, to)
            {
                Nature = card.Name == "Slash" ? DamageStruct.DamageNature.Normal
                        : card.Name == "FireSlash" ? DamageStruct.DamageNature.Fire : DamageStruct.DamageNature.Thunder
            };                 //evaluate damage

            ScoreStruct score = GetDamageScore(damage);
            result_score.Damage = score.Damage;
            if (IsFriend(to, from))
            {
                result_score.Score = result + score.Score;
                return result_score;
            }
            else
            {
                double rate = force_hit + (1 - force_hit) * jink;
                result_score.Score = result + score.Score * rate;
                return result_score;
            }
        }

        public bool IsCancelTarget(WrappedCard card, Player to, Player from)
        {
            foreach (SkillEvent e in Engine.GetSkillEvents())
                if (e != null && e.IsCancelTarget(this, card, from, to)) return true;

            if (HasArmorEffect(to, "IronArmor") && (card.Name == "FireSlash" || card.Name == "FireAttack" || card.Name == "BurningCamps"))
                return true;

            return false;
        }

        public bool IsCardEffect(WrappedCard card, Player to, Player from)
        {
            foreach (SkillEvent e in Engine.GetSkillEvents())
                if (e != null && !e.IsCardEffect(this, card, from, to)) return false;

            UseCard ev = Engine.GetCardUsage(card.Name);
            if (ev != null && !ev.IsCardEffect(this, card, from, to)) return false;

            if (card.Name.Contains("Slash") && HasArmorEffect(to, "RenwangShield") && WrappedCard.IsBlack(card.Suit))
                return false;
            if (HasArmorEffect(to, "Vine") && (card.Name == "SavageAssault" || card.Name == "ArcheryAttack"
                                               || card.Name == "Slash"))
                return false;

            return true;
        }

        public ScoreStruct GetDamageScore(DamageStruct damage)
        {
            damage.Damage = DamageEffect(damage);
            Player from = damage.From;
            Player to = damage.To;

            ScoreStruct result_score = new ScoreStruct
            {
                Damage = damage
            };
            List<ScoreStruct> scores = new List<ScoreStruct>();

            if (damage.Damage > 0)
            {
                double value = 0;
                if (NotHurtXiaoqiao(damage))
                {
                    result_score.Score = -20;
                    return result_score;
                }
                else
                {
                    value = damage.Damage * 4;
                    if (IsWeak(to))
                    {
                        value += 4;
                        if (damage.Damage >= to.Hp && !CanSave(to, damage.Damage - to.Hp + 1))
                            value += 10;
                    }

                    if (CanResist(to, damage.Damage)) result_score.Score = 4;
                    if (IsFriend(to)) value = -value;

                    foreach (SkillEvent e in Engine.GetSkillEvents())
                if (e.Name != damage.Reason)
                    value += e.GetDamageScore(this, damage).Score;

            if (priority_enemies.Contains(to)) value = value* 1.5;
            if (from != null && HasSkill("kuanggu", from) && (to == from && from.Hp > damage.Damage || RoomLogic.DistanceTo(room, from, to) == 1))
            {
                double coeff = 1;
                if (priority_enemies.Contains(from)) coeff = 1.5;
                if (IsFriend(from))
                    value += damage.Damage* 4;
                else if (IsEnemy(from))
                    value -= damage.Damage* 4 * coeff;
                    }
                    result_score.Score = value;
                }
            }
            scores.Add(result_score);

            if (from != null && HasSkill("zhiman", from) && RoomLogic.GetPlayerCards(room, to, "ej").Count > 0)
            {
                ScoreStruct score = FindCards2Discard(from, to, "ej", HandlingMethod.MethodGet);
                scores.Add(score);
            }
            if (damage.Card != null && from != null && !damage.Transfer)
            {
                if ((damage.Card.Name.Contains("Slash") || damage.Card.Name == "Duel") && HasSkill("chuanxin", from) && !RoomLogic.IsFriendWith(room, from, to) && to.HasShownOneGeneral()
                        && !string.IsNullOrEmpty(to.General2) && !to.General2.Contains("sujiang") && !to.IsDuanchang(false))
                {
                    double value = ((to.HasEquip() && !IsWeak(to)) ? (to.GetEquips().Count * 3 + 4) : 7);
                    ScoreStruct score = new ScoreStruct
                    {
                        Score = value
                    };
                    scores.Add(score);
                }
                if (damage.Card.Name.Contains("Slash") && from.HasWeapon("IceSword") && !to.IsNude())
                {
                    ScoreStruct score = FindCards2Discard(from, to, "he", HandlingMethod.MethodDiscard, 2, true);
                    scores.Add(score);
                }
            }

            CompareByScore(ref scores);
            return scores[scores.Count - 1];
        }

        public bool IsGoodSreadStarter(DamageStruct damage, bool for_self = true)
        {
            if (damage.To == null || !damage.To.Alive || !damage.To.Chained || damage.Chain || damage.Damage <= 0 || damage.Nature == DamageStruct.DamageNature.Normal)
                return false;

            List<Player> players = GetSreadTargets(damage);
            double good = 0;
            double bad = 0;
            foreach (Player p in players)
            {
                DamageStruct spread = damage;
                spread.Chain = true;
                spread.To = p;
                spread.Damage = DamageEffect(spread);
                if (spread.Damage > 0)
                {
                    if (IsFriend(p))
                    {
                        if (spread.From != null && HasSkill("zhiman", spread.From) && IsFriend(spread.From, spread.To))
                        {
                            if (spread.To.JudgingArea.Count > 0 || GetLostEquipEffect(spread.To) < 0)
                                good += 4;
                        }
                        else if (CanResist(p, spread.Damage))
                        {
                            bad += 4;
                        }
                        else
                        {
                            bad += spread.Damage * 4;
                            ScoreStruct score = GetDamageScore(spread);
                            if (score.Score > 0)
                                good += score.Score;
                            if (IsWeak(spread.To))
                            {
                                bad += 6;
                                if (spread.To.Removed && spread.Damage >= spread.To.Hp)
                                    bad += 10;
                                if (RoomLogic.IsFriendWith(room, self, spread.To))
                                    bad += 100;
                            }
                        }
                    }
                    else if (IsEnemy(p))
                    {
                        if (spread.From != null && HasSkill("zhiman", spread.From) && IsFriend(spread.From, spread.To))
                        {
                            if (spread.To.JudgingArea.Count > 0 || GetLostEquipEffect(spread.To) < 0)
                                bad += 4;
                        }
                        else if (CanResist(p, spread.Damage))
                        {
                            good += 4;
                        }
                        else
                        {
                            double value = spread.Damage * 4;
                            ScoreStruct score = GetDamageScore(spread);
                            if (score.Score > 0)
                                bad += value;
                            if (IsWeak(spread.To))
                            {
                                value += 4;
                                if (spread.To.Removed && spread.Damage >= spread.To.Hp)
                                    value += 8;
                            }
                            if (priority_enemies.Contains(damage.To))
                                value = value * 1.5;
                            good += value;
                        }
                    }
                }
                else if (spread.From != null && HasSkill("zhiman", spread.From) && IsFriend(spread.From, spread.To)
                         && (spread.To.JudgingArea.Count > 0 || GetLostEquipEffect(spread.To) < 0))
                {
                    if (IsFriend(p))
                        good += 4;
                    else
                        bad += 4;
                }
            }
            if (for_self)
                return good > bad;
            else
                return bad > good;
        }

        public List<Player> GetSreadTargets(DamageStruct damage)
        {
            List<Player> players = new List<Player>();
            if (damage.To == null || !damage.To.Alive || !damage.To.Chained || damage.Chain || damage.Damage <= 0 || damage.Nature == DamageStruct.DamageNature.Normal)
                return players;

            foreach (Player p in room.GetOtherPlayers(damage.To))
                if (p.Chained)
                    players.Add(p);

            return players;
        }

        public bool CanDodamage(DamageStruct damage, double adjust_vlaue)
        {
            damage.Damage = DamageEffect(damage);
            if (damage.Damage == 0) return false;

            double adjust = 0;
            if (IsGoodSreadStarter(damage))
                adjust = 6;
            else if (IsGoodSreadStarter(damage, false))
                adjust = -6;
            if (IsFriend(damage.To) && adjust + adjust_vlaue + GetDamageScore(damage).Score < 4) return false;
            if (IsEnemy(damage.To) && adjust + adjust_vlaue + GetDamageScore(damage).Score < -4) return false;

            return true;
        }

        public bool CanRetrial(Player retrialer, string judge_reasion, Player judge_who)
        {
            foreach (string skill in GetKnownSkills(retrialer)) {
                SkillEvent e = Engine.GetSkillEvent(skill);
                if (e!=null && e.CanRetrial(this, judge_reasion, retrialer, judge_who))
                    return true;
            }

            return false;
        }

        public bool RetrialCardMatch(Player retrialer, Player judge_who, string reason, int id)
        {
            WrappedCard card = room.GetCard(id);
            WrappedCard.CardSuit suit = (RoomLogic.PlayerHasShownSkill(room, judge_who, "hongyan") && card.Suit == WrappedCard.CardSuit.Spade ? WrappedCard.CardSuit.Heart : card.Suit);
            if (IsFriend(retrialer, judge_who))
            {
                if ((reason == "Indulence" && suit == WrappedCard.CardSuit.Heart) || (reason == "SupplyShortage" && suit == WrappedCard.CardSuit.Club)
                        || (reason == "Lightning" && (suit != WrappedCard.CardSuit.Spade || card.Number > 9 || card.Number < 2)))
                    return true;
            }
            else if (IsEnemy(retrialer, judge_who))
            {
                if ((reason == "Indulence" && suit != WrappedCard.CardSuit.Heart) || (reason == "SupplyShortage" && suit != WrappedCard.CardSuit.Club)
                        || (reason == "Lightning" && suit == WrappedCard.CardSuit.Spade && card.Number <= 9 && card.Number >= 2))
                    return true;
            }

            foreach (SkillEvent e in Engine.GetSkillEvents())
                if (e.RetrialCardMatch(this, retrialer, judge_who, reason, id))
                return true;

            return false;
        }

        public Player GetWizzardRaceWinner(string reason, Player judge_who, Player starter = null)
        {
            Player winner = null;
            List<Player> players = new List<Player>();
            if (starter != null)
            {
                players.Add(starter);
                for (int i = 1; i < room.AliveCount(); i++)
                    players.Add(room.GetNextAlive(starter));
            }
            else
                players = room.AlivePlayers;

            foreach (Player p in players) {
                if (CanRetrial(p, reason, judge_who))
                    winner = p;
            }

            return winner;
        }

        public KeyValuePair<Player, int> GetCardNeedPlayer(List<int> ids = null, List<Player> players = null, Place dest_place = Place.PlaceHand)
        {
            if (ids == null || ids.Count == 0)
                ids = GetKnownCards(self);

            List<Player> weaks = new List<Player>(), acts = new List<Player>();
            Player target = new Player();
            players = players ?? room.GetOtherPlayers(self);
            foreach (Player p in players)
                if (IsWeak(p) && !MaySave(p))
                    weaks.Add(p);

            SortByDefense(ref weaks, false);
            target = weaks[0];

            double point = 0;
            int result = -1;
            if (target != null)
            {
                foreach (int id in ids) {
                    double value = GetKeepValue(id, target, dest_place);
                    if (value >= 6 && value > point)
                    {
                        point = value;
                        result = id;
                    }
                }
                if (result >= 0) return new KeyValuePair<Player, int>(target, result);
            }

            foreach (Player p in players)
                if (p.Phase == PlayerPhase.Play || !WillSkipPlayPhase(p))
                    acts.Add(p);
            room.SortByActionOrder(ref acts);

            target = null;
            foreach (Player p in acts) {
                foreach (int id in ids) {
                    double value = GetUseValue(id, p, dest_place);
                    if (value >= 6 && value > point)
                    {
                        point = value;
                        result = id;
                        target = p;
                    }
                }
            }
            if (target != null) return new KeyValuePair<Player, int>(target, result);

            foreach (Player p in acts) {
                if (p.Phase == PlayerPhase.NotActive || dest_place != Place.PlaceHand || GetOverflow(p) <= 0)
                {
                    target = p;
                    break;
                }
            }
            if (target != null)
            {
                foreach (int id in ids) {
                    double value = GetUseValue(id, target, dest_place);
                    if (value >= 6 && value > point)
                    {
                        point = value;
                        result = id;
                    }
                }
                if (result >= 0) return new KeyValuePair<Player, int>(target, result);
            }

            return new KeyValuePair<Player, int>();
        }

        public bool NotHurtXiaoqiao(DamageStruct damage)
        {
            Player from = damage.From;
            Player to = damage.To;

            if (!HasSkill("tianxiang", to) || damage.Damage <= 0 || IsFriend(to)
                || (GetKnownCards(to).Count == to.HandcardNum && GetKnownCardsNums(".|heart", "h", to, self) == 0)
                || IsLackCard(to, "heart"))
                return false;

            if (HasArmorEffect(to, "Vine") && damage.Damage > 1 && damage.Nature == DamageStruct.DamageNature.Fire) damage.Damage -= 1;

            if (from != null && from.Alive && !IsFriend(from, to))
            {
                if (HasSkill("zhiman", from)
                        || (!damage.Transfer && damage.Card !=null && (damage.Card.Name.Contains("Slash") || damage.Card.Name == "Duel") && HasSkill("chuanxin", from))
                        || (!damage.Transfer && damage.Card != null && damage.Card.Name.Contains("Slash") && !to.IsNude() && from.HasWeapon("IceSword")))
                    return false;
            }

            damage.Transfer = true;
            foreach (Player p in room.GetOtherPlayers(to)) {
                int hurt = DamageEffect(damage);
                if (IsFriend(p) && p.Hp <= hurt && !HasBuquEffect(p) && !HasNiepanEffect(p) && !CanResist(p, hurt))
                    return true;
                if (IsFriend(p) && WillSkipPlayPhase(p) && hurt > 0 && !CanResist(p, hurt) && !HasSkill("jijiu", p)) return true;
                if (IsFriend(to, p) && IsEnemy(p)
                        && ((p.Hp - hurt > 0 || HasBuquEffect(p) || CanResist(p, hurt)) && (p.GetLostHp() + hurt > hurt * 2
                        || GetDamageScore(damage).Score > 5)))
                    return true;
            }

            return false;
        }

        public bool NotSlashJiaozhu(Player jiaozhu)
        {
            if (IsFriend(jiaozhu) || !HasSkill("leiji", jiaozhu)) return false;

            DamageStruct damage = new DamageStruct
            {
                From = jiaozhu,
                Damage = 2,
                Nature = DamageStruct.DamageNature.Thunder
            };
            foreach (Player p in room.AlivePlayers) {
                DamageStruct _damage = damage;
                _damage.To = p;
                _damage.Damage = DamageEffect(_damage);
                if (_damage.Damage > 0 && (!CanResist(p, _damage.Damage) || IsFriend(p, jiaozhu)) && (IsFriend(p) || IsGoodSreadStarter(_damage, false)))
                {
                    Player wizzard = GetWizzardRaceWinner("leiji", p);
                    if (wizzard == null || !IsFriend(wizzard) && !IsFriend(jiaozhu, p) && !IsFriend(wizzard, p))
                        return true;
                }
            }

            return false;
        }

        public bool JiaozhuneedSlash(Player jiaozhu)
        {
            if (!IsFriend(jiaozhu) || !HasSkill("leiji", jiaozhu)) return false;

            DamageStruct damage = new DamageStruct
            {
                From = jiaozhu,
                Damage = 2,
                Nature = DamageStruct.DamageNature.Thunder
            };
            foreach (Player p in room.AlivePlayers)
            {
                DamageStruct _damage = damage;
                _damage.To = p;
                _damage.Damage = DamageEffect(_damage);
                if (_damage.Damage > 1 && !CanResist(p, _damage.Damage) && IsEnemy(p) && !IsGoodSreadStarter(_damage, false))
                {
                    Player wizzard = GetWizzardRaceWinner("leiji", p);
                    if (wizzard != null && IsFriend(wizzard))
                        return true;
                }
            }

            return false;
        }

        public bool CanSave(Player dying, int peaches)
        {
            if (peaches <= 0) return true;

            int count = 0;
            List<WrappedCard> analeptics = GetCards("Analeptic", dying);
            foreach (WrappedCard card in analeptics)
                if (!RoomLogic.IsCardLimited(room, dying, card, HandlingMethod.MethodUse) && RoomLogic.IsProhibited(room, dying, dying, card) == null)
                    count++;

            foreach (Player p in friends[dying])
            {
                List<WrappedCard> peach = GetCards("Peach", dying);
                foreach (WrappedCard card in peach)
                    if (!RoomLogic.IsCardLimited(room, p, card, HandlingMethod.MethodUse) && RoomLogic.IsProhibited(room, p, dying, card) == null)
                        count++;
            }
            return count >= peaches;
        }
        public void SetCardLack(Player who, string flag)
        {
            if (flag.StartsWith("-"))
            {
                string _flag = flag.Substring(1);
                card_lack[who].RemoveAll(t => t == _flag);
            }
            else
                card_lack[who].Add(flag);
        }
        public bool IsLackCard(Player who, string flag)
        {
            return card_lack[who].Contains(flag);
        }
        private List<string> kingdoms = new List<string> {"wei", "shu", "wu", "qun" };
        public virtual void Event(TriggerEvent triggerEvent, Player player, object data)
        {
        }
        public virtual void Activate(ref CardUseStruct card_use)
        {
        }

        public virtual WrappedCard.CardSuit AskForSuit(string str) => WrappedCard.CardSuit.Diamond;

        public virtual string AskForKingdom()
        {
            List<string> kingdom_list = new List<string> { "shu", "wu", "qun", "wei" };
            Shuffle.shuffle<string>(ref kingdom_list);
            return kingdom_list[0];
        }
        public virtual bool AskForSkillInvoke(string skill_name, object data) => false;
        public virtual string AskForChoice(string skill_name, string choice, object data)
        {
            List<string> choices = new List<string>(choice.Split('+'));
            if (choices.Contains("cancel")) return "cancel";
            Shuffle.shuffle<string>(ref choices);
            return choices[0];
        }
        public virtual List<int> AskForDiscard(string reason, int discard_num, int min_num, bool optional, bool include_equip)
        {
            List<int> to_discard = new List<int>();
            if (optional)
                return to_discard;
            else
                return room.ForceToDiscard(self, discard_num, include_equip, self.HasFlag("Global_AIDiscardExchanging"));
        }
        public virtual AskForMoveCardsStruct AskForMoveCards(List<int> upcards, List<int> downcards, string reason, int min_num, int max_num)
        {
            AskForMoveCardsStruct result = new AskForMoveCardsStruct
            {
                Bottom = new List<int>(),
                Top = new List<int>(),
                Success = false
            };
            return result;
        }
        public virtual WrappedCard AskForNullification(WrappedCard trick, Player from, Player to, bool positive) => null;
        public virtual int AskForCardChosen(Player who, string flags, string reason, HandlingMethod method, List<int> disabled_ids) => -1;
        public virtual List<int> AskForCardsChosen(List<Player> targets, string flags, string reason, int min, int max, List<int> disabled_ids) => new List<int>();
        public virtual WrappedCard AskForCard(string pattern, string prompt, object data)
        {
            if (!pattern.Contains("Slash") && !pattern.Contains("Jink") && !pattern.Contains("Peach") && !pattern.Contains("Analeptic"))
                return null;

            List<WrappedCard> cards = RoomLogic.GetPlayerHandcards(room, self);
            List<int> piles = self.GetHandPile();
            foreach (int id in piles)
                cards.Add(room.GetCard(id));

            WrappedCard result = null;
            foreach (WrappedCard card in cards)
            {
                if (Engine.MatchExpPattern(room, pattern, self, card))
                {
                    result = card;
                    break;
                }
            }

            if (result != null && result.Name == "Peach" && room.BloodBattle)
            {
                WrappedCard slash = new WrappedCard("Slash");
                slash.AddSubCard(result);
                slash = RoomLogic.ParseUseCard(room, slash);
                if (Engine.GetFunctionCard("Slash").IsAvailable(room, self, slash))
                    return slash;
                else
                {
                    WrappedCard jink = new WrappedCard("Jink");
                    jink.AddSubCard(result);
                    jink = RoomLogic.ParseUseCard(room, jink);
                    return jink;
                }
            }

            return result;
        }
        public virtual CardUseStruct AskForUseCard(string pattern, string prompt, HandlingMethod method)
        {
            return new CardUseStruct(null, self, new List<Player>(), false);
        }
        public virtual int AskForAG(List<int> card_ids, bool refusable, string reason)
        {
            if (refusable)
                return -1;

            Shuffle.shuffle<int>(ref card_ids);
            return card_ids[0];
        }
        public virtual WrappedCard AskForCardShow(Player requestor, string reason, object data) => room.GetCard(room.GetRandomHandCard(self));
        public virtual WrappedCard AskForPindian(Player requestor, string reason)
        {
            List<WrappedCard> cards = RoomLogic.GetPlayerHandcards(room, self);
            cards.Sort((x, y) => x.Number > y.Number ? -1 : 1);

            if (requestor != self && RoomLogic.IsFriendWith(room, self, requestor))
                return cards[0];
            else
                return cards[cards.Count - 1];
        }
        public virtual List<Player> AskForPlayersChosen(List<Player> targets, string reason, int max_num, int min_num)
        {
            List<Player> result = new List<Player>();
            Shuffle.shuffle(ref targets);
            for (int i = 0; i < min_num; i++)
                result.Add(targets[i]);

            return result;
        }
        public virtual WrappedCard AskForSinglePeach(Player dying)
        {
            if (RoomLogic.IsFriendWith(room, self, dying))
            {
                List<WrappedCard> cards = RoomLogic.GetPlayerHandcards(room, self);
                List<int> piles = self.GetHandPile();
                foreach (int id in piles)
                    cards.Add(room.GetCard(id));

                return AskForCard(room.GetRoomState().GetCurrentCardUsePattern(self), null, null);
            }

            return null;
        }
        public virtual Player AskForYiji(List<int> cards, string reason, ref int card_id) => null;

        public virtual bool UseCard(WrappedCard card)
        {
            FunctionCard fcard = Engine.GetFunctionCard(card.Name);
            if (fcard is EquipCard equip)
            {
                switch (equip.EquipLocation())
                {
                    case EquipCard.Location.WeaponLocation:
                        {
                            if (!self.GetWeapon()) return true;

                            Weapon new_weapon = (Weapon)equip;
                            Weapon ole_weapon = (Weapon)Engine.GetFunctionCard(self.Weapon.Value);
                            return new_weapon.Range > ole_weapon.Range;
                        }
                    case EquipCard.Location.ArmorLocation: return !self.GetArmor();
                    case EquipCard.Location.OffensiveHorseLocation: return !self.GetOffensiveHorse();
                    case EquipCard.Location.DefensiveHorseLocation: return !self.GetDefensiveHorse();
                    case EquipCard.Location.TreasureLocation: return !self.GetTreasure();
                    default:
                        return true;
                }
            }
            return false;
        }

        public virtual List<int> AskForExchange(string reason, string pattern, int max_num, int min_num, string expand_pile)
        {
            List<int> to_discard = new List<int>();
            if (min_num == 0)
                return to_discard;
            else
                return  room.ForceToDiscard(self, min_num, pattern, expand_pile, false);
        }
    }
}
