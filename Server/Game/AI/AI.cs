using CommonClass;
using CommonClass.Game;
using SanguoshaServer.Game;
using SanguoshaServer.Package;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static CommonClass.Game.Player;
using static SanguoshaServer.Package.FunctionCard;

namespace SanguoshaServer.AI
{
    public class TrustedAI
    {
        public Dictionary<Player, List<string>> PlayerKnown { set; get; } = new Dictionary<Player, List<string>>();
        public Room Room => room;
        public List<Player> FriendNoSelf {
            get {
                List<Player> frie = GetFriends(self);
                frie.Remove(self);
                return frie;
            }
        }
        public Player Self => self;
        public Dictionary<string, string> Choice { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, Player> Target { get; set; } = new Dictionary<string, Player>();
        public Dictionary<string, double> Number { get; set; } = new Dictionary<string, double>();
        public Dictionary<string, int> CardCounts { get; set; } = new Dictionary<string, int>();

        public Dictionary<Player, double> PlayersLevel { get; set; } = new Dictionary<Player, double>();
        public string Process { get; protected set; } = string.Empty;
        public string ProcessPublic { get; protected set; } = string.Empty;

        protected Player self;
        protected Room room;
        protected Dictionary<string, SkillEvent> skill_events = new Dictionary<string, SkillEvent>();
        protected Dictionary<string, UseCard> card_events = new Dictionary<string, UseCard>();

        protected bool show_immediately;
        protected bool skill_invoke_postpond;
        protected List<CardUseStruct> to_use;

        protected Dictionary<Player, List<Player>> friends = new Dictionary<Player, List<Player>>();
        protected Dictionary<Player, List<Player>> enemies = new Dictionary<Player, List<Player>>();

        protected List<Player> priority_enemies = new List<Player>();

        protected Dictionary<Player, string> id_tendency = new Dictionary<Player, string>();
        protected Dictionary<Player, string> id_public = new Dictionary<Player, string>();

        protected Dictionary<string, Player> lords = new Dictionary<string, Player>();
        protected Dictionary<string, Player> lords_public = new Dictionary<string, Player>();

        protected Dictionary<Player, Dictionary<string, int>> player_intention = new Dictionary<Player, Dictionary<string, int>>();
        protected Dictionary<Player, Dictionary<string, int>> player_intention_public = new Dictionary<Player, Dictionary<string, int>>();
        protected Dictionary<Player, List<Player>> same_kingdom = new Dictionary<Player, List<Player>>();
        protected Dictionary<Player, List<Player>> different_kingdom = new Dictionary<Player, List<Player>>();
        protected Dictionary<Player, double> players_hatred = new Dictionary<Player, double>();

        protected int turn_count;

        protected Dictionary<Player, List<int>> public_handcards = new Dictionary<Player, List<int>>();
        protected Dictionary<Player, List<int>> private_handcards = new Dictionary<Player, List<int>>();
        protected Dictionary<Player, List<int>> wooden_cards = new Dictionary<Player, List<int>>();

        protected KeyValuePair<Player, List<int>> guanxing = new KeyValuePair<Player, List<int>>();
        protected Dictionary<Player, List<int>> pre_discard = new Dictionary<Player, List<int>>();
        protected List<Player> pre_ignore_armor = new List<Player>();
        protected Dictionary<Player, string> pre_disable = new Dictionary<Player, string>();
        protected Dictionary<Player, string> pre_noncom_invalid = new Dictionary<Player, string>();
        protected Dictionary<Player, List<string>> card_lack = new Dictionary<Player, List<string>>();
        protected WrappedCard pre_drink;

        public static readonly string MasochismSkill = Engine.GetSkills("masochism_skill");
        public static readonly string LoseEquipSkill = Engine.GetSkills("lose_equip_skill");
        public static readonly string MasochismGood = Engine.GetSkills("masochism_good");
        public static readonly string NeedKongchengSkill = Engine.GetSkills("need_kongcheng");
        public static readonly string DefenseSkill = Engine.GetSkills("defense_skill");
        public static readonly string UsefullSkill = Engine.GetSkills("usefull_skill");
        public static readonly string DrawcardSkill = Engine.GetSkills("drawcard_skill");
        public static readonly string AttackSkill = Engine.GetSkills("attack_skill");
        public static readonly string WizardHarmSkill = Engine.GetSkills("wizard_harm_skill");
        public static readonly string PrioritySkill = Engine.GetSkills("priority_skill");
        public static readonly string SaveSkill = Engine.GetSkills("save_skill");
        public static readonly string ExclusiveSkill = Engine.GetSkills("exclusive_skill");
        public static readonly string ActiveCardneedSkill = Engine.GetSkills("Active_cardneed_skill");
        public static readonly string CardneedSkill = Engine.GetSkills("cardneed_skill");
        public static readonly string NotActiveCardneedSkill = Engine.GetSkills("notActive_cardneed_skill");
        public static readonly string DrawpeachAkill = Engine.GetSkills("drawpeach_skill");
        public static readonly string RecoverSkill = Engine.GetSkills("recover_skill");
        public static readonly string UseLionSkill = Engine.GetSkills("use_lion_skill");
        public static readonly string NeedEquipSkill = Engine.GetSkills("need_equip_skill");
        public static readonly string JudgeReason = Engine.GetSkills("judge_reason");
        public static readonly string WizzardSkill = Engine.GetSkills("wizzard_skill");

        //others
        CardUseStruct ai_AOE_data;

        public List<Player> GetPrioEnemies() => new List<Player>(priority_enemies);

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
                PlayersLevel[p] = 1;
                players_hatred[p] = 0;
                public_handcards[p] = new List<int>();
                private_handcards[p] = new List<int>();
                wooden_cards[p] = new List<int>();
                pre_discard[p] = new List<int>();
                pre_disable[p] = string.Empty;
                pre_noncom_invalid[p] = string.Empty;
                card_lack[p] = new List<string>();
            }
        }

        public bool IsFriend(Player other, Player another = null)
        {
            if (another == null)
                return (other == self) || GetFriends(self).Contains(other);

            return other == another || (friends.ContainsKey(other) && friends[other].Contains(another));
        }

        public bool IsEnemy(Player other, Player another = null)
        {
            if (another == null)
                return GetEnemies(self).Contains(other);

            return enemies.ContainsKey(other) && enemies[other].Contains(another);
        }

        public bool HasSkill(string skill_name, Player who = null, bool ignore_invalid = false)
        {
            Player player = who ?? self;
            foreach (string skills in skill_name.Split('|'))
            {
                bool check_point = true;
                foreach (string skill in skills.Split('+'))
                {
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

        public double GetGeneralStength(Player who, bool head, bool include_compulsory = true)
        {
            List<Skill> skills = new List<Skill>();
            if (head && !pre_disable[who].Contains("h"))
                skills = RoomLogic.GetHeadActivedSkills(room, who, true, !(IsKnown(self, who, "h") && IsKnown(self, who, "h")));
            else if (!head && !string.IsNullOrEmpty(who.General2) && !pre_disable[who].Contains("d"))
                skills.AddRange(RoomLogic.GetDeputyActivedSkills(room, who, true, !(IsKnown(self, who, "d") && IsKnown(self, who, "d"))));

            List<string> skill_names = new List<string>();
            foreach (Skill skill in skills)
            {
                if ((skill.SkillFrequency == Skill.Frequency.Limited && who.GetMark(skill.LimitMark) == 0)
                        || (skill.SkillFrequency == Skill.Frequency.Wake && who.GetMark(skill.Name) > 0)
                        || (!include_compulsory && skill.SkillFrequency == Skill.Frequency.Compulsory)) continue;
                if (Engine.Invalid(room, who, skill.Name) != null) continue;
                if (!RoomLogic.PlayerHasSkill(room, who, skill.Name) && self == who) continue;
                if (skill.Visible)
                    skill_names.Add(skill.Name);
            }

            double point = 0;
            foreach (string skill in skill_names)
            {
                SkillEvent e = Engine.GetSkillEvent(skill);
                point += Engine.GetSkillValue(skill) + (e != null ? e.GetSkillAdjustValue(this, who) : 0);
            }

            return point;
        }

        public virtual void UpdatePlayerRelation(Player player, Player p, bool friendly)
        {
        }
        public virtual void UpdatePlayerIntention(Player player, string kingdom, int intention)
        {
        }
        //更新仇恨值
        public virtual void UpdatePlayerHatred(Player to, double hatred)
        {
            players_hatred[to] += hatred;
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
            List<Skill> skills = new List<Skill>();
            if (!pre_disable[to].Contains("h"))
            {
                foreach (Skill skill in RoomLogic.GetHeadActivedSkills(room, to, true, !(IsKnown(self, to, "h") && IsKnown(from, to, "h"))))
                    if (to.HeadAcquiredSkills.Contains(skill.Name) || !pre_noncom_invalid[to].Contains("h") || skill.SkillFrequency != Skill.Frequency.Compulsory)
                        skills.Add(skill);
            }
            if (!string.IsNullOrEmpty(to.General2) && !pre_disable[to].Contains("d"))
                foreach (Skill skill in RoomLogic.GetDeputyActivedSkills(room, to, true, !(IsKnown(self, to, "d") && IsKnown(from, to, "d"))))
                    if (to.DeputyAcquiredSkills.Contains(skill.Name) || !pre_noncom_invalid[to].Contains("d") || skill.SkillFrequency != Skill.Frequency.Compulsory)
                        skills.Add(skill);

            List<string> skill_names = new List<string>();
            foreach (Skill skill in skills)
            {
                if ((skill.SkillFrequency == Skill.Frequency.Limited && to.GetMark(skill.LimitMark) == 0)
                        || (skill.SkillFrequency == Skill.Frequency.Wake && to.GetMark(skill.Name) > 0)) continue;
                if (!ignore_invalid && Engine.Invalid(room, to, skill.Name) != null) continue;
                if (!RoomLogic.PlayerHasSkill(room, to, skill.Name) && self == to && from == self) continue;
                if (skill.Visible|| !visible)
                    skill_names.Add(skill.Name);
            }

            if (to.GetMark("Equips_Nullified_to_Yourself") == 0)
            {
                foreach (int id in to.GetEquips())
                {
                    Skill skill = Engine.GetSkill(room.GetCard(id).Name);
                    if (skill != null)
                        skill_names.Add(skill.Name);
                }
            }

            return skill_names;
        }

        public int GetEnemisBySeat(Player player)
        {
            if (room.Current == player || room.Current == null || !player.Alive)
                return 0;

            int result = 0;
            Player current = room.Current;
            while (current != player)
            {
                if (IsEnemy(current, player))
                    result++;

                current = room.GetNextAlive(current, 1, false);
            }

            return result;
        }

        public Player FindPlayerBySkill(string skill_name)
        {
            foreach (Player p in room.GetAlivePlayers())
                if (HasSkill(skill_name, p)) return p;

            return null;
        }
        public List<Player> GetEnemies(Player player)
        {
            if (!enemies.ContainsKey(player)) return new List<Player>();
            return new List<Player>(enemies[player]);
        }

        public List<Player> GetFriends(Player player)
        {
            if (!friends.ContainsKey(player)) return new List<Player> { player }; 

            List<Player> f = new List<Player>(friends[player]);
            if (!f.Contains(player))
                f.Add(player);
            return f;
        }
        public virtual List<string> GetPossibleId(Player who)
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

        public virtual string GetPlayerTendency(Player player)
        {
            return id_tendency[player];
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
            foreach (string kingdom in player_intention_public[who].Keys)
            {
                int intention = player_intention_public[who][kingdom];
                if (intention > 0)
                    sort.Add(kingdom, intention);
            }

            List<string> result = new List<string>();
            CompareByStrength(sort, ref result);

            if (result.Count == 0) result.Add("careerist");
            return result;
        }

        public void ClearKnownCards(Player who)
        {
            private_handcards[who].Clear();
        }

        public void SetPrivateKnownCards(Player who, int id)
        {
            if (!private_handcards[who].Contains(id))
                private_handcards[who].Add(id);
        }
        public void SetPublicKnownCards(Player who, int id)
        {
            SetPrivateKnownCards(who, id);

            if (!public_handcards[who].Contains(id))
                public_handcards[who].Add(id);
        }

        public List<int> GetKnownCards(Player who)
        {
            List<int> ids = new List<int>();
            if (who == self)
            {
                ids= self.GetCards("h");
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
                ids= self.GetCards("h");
                ids.AddRange(who.GetHandPile());
            }
            else
            {
                foreach (int id in private_handcards[who])
                    if (!ids.Contains(id))
                        ids.Add(id);

                ids.AddRange(who.GetHandPile(false));
                ids.AddRange(wooden_cards[who]);
            }

            return ids;
        }

        public List<WrappedCard> GetCards(string card_name, Player player, bool no_duplicated = false)
        {
            List<WrappedCard> result = new List<WrappedCard>();
            List<int> ids = new List<int>();
            if (self == player)
            {
                foreach (int id in self.GetCards("h"))
                {
                    if (pre_drink != null && pre_drink.SubCards.Contains(id)) continue;
                    ids.Add(id);
                }
            }
            else
                ids = new List<int>(private_handcards[player]);

            foreach (int id in player.GetHandPile(false))
            {
                if (player == self && pre_drink != null && pre_drink.SubCards.Contains(id)) continue;
                ids.Add(id);
            }
            if (player == self)
            {
                foreach (int id in player.GetPile("wooden_ox"))
                {
                    if (player == self && pre_drink != null && pre_drink.SubCards.Contains(id)) continue;
                    ids.Add(id);
                }
            }
            else
            {
                ids.AddRange(wooden_cards[player]);
            }

            foreach (int id in player.GetEquips())
            {
                if (player == self && pre_drink != null && pre_drink.SubCards.Contains(id)) continue;
                ids.Add(id);
            }

            foreach (int id in ids)
            {
                List<WrappedCard> cards = GetViewAsCards(player, id);
                foreach (WrappedCard card in cards)
                {
                    if (card.Name.Contains(card_name))
                    {
                        result.Add(card);
                        if (no_duplicated) break;
                    }
                }
            }

            foreach (string skill in GetKnownSkills(player))
            {
                SkillEvent e = Engine.GetSkillEvent(skill);
                if (e != null)
                {
                    result.AddRange(e.GetViewAsCards(this, card_name, player));
                }
            }

            if (self == player)
                SortByUseValue(ref result);
            return result;
        }

        public List<WrappedCard> GetViewAsCards(Player player, int id, Place place = Place.PlaceUnknown)
        {
            List<WrappedCard> result = new List<WrappedCard>();
            WrappedCard card = room.GetCard(id);
            WrappedCard engine_card = Engine.CloneCard(Engine.GetRealCard(id));
            if (room.GetCardPlace(id) == Place.PlaceDelayedTrick && place == Place.PlaceHand && card.Name != engine_card.Name)
                card = engine_card;

            if (card.Name == Peach.ClassName && room.BloodBattle && room.Setting.GameMode == "Hegemony")
            {
                WrappedCard slash = new WrappedCard(Slash.ClassName);
                slash.AddSubCard(id);
                slash = RoomLogic.ParseUseCard(room, slash);
                result.Add(slash);

                WrappedCard jink = new WrappedCard(Jink.ClassName);
                jink.AddSubCard(id);
                jink = RoomLogic.ParseUseCard(room, jink);
                result.Add(jink);
            }
            else
            {
                result.Add(card);
            }

            if (place == Place.PlaceUnknown && room.GetCardOwner(id) == player)
                place = room.GetCardPlace(id);

            foreach (string skill in GetKnownSkills(player))
            {
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

        public int GetKnownCardsNums(string pattern, string flags, Player player, Player from = null, bool contains_handpile = true)
        {
            from = from ?? self;
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

                    if (contains_handpile)
                    {
                        foreach (int id in GetKnownHandPileCards(player))
                            if (IsCard(id, pattern, player, self))
                                hands.Add(id);
                    }
                }
                else
                {
                    foreach (int id in public_handcards[player])
                        if (IsCard(id, pattern, player, from))
                            hands.Add(id);
                }
                hand = hands.Count;
            }

            foreach (string skill in GetKnownSkills(player))
            {
                SkillEvent e = Engine.GetSkillEvent(skill);
                if (e != null)
                {
                    hand += e.GetViewAsCards(this, pattern, player).Count;
                }
            }

            return hand + eq;
        }

        public bool IsCard(int id, string pattern, Player player, Player from = null)
        {
            from = from ?? self;
            List<WrappedCard> result = new List<WrappedCard>();
            WrappedCard card = room.GetCard(id);
            if (card.Name == Peach.ClassName && room.BloodBattle)
            {
                WrappedCard slash = new WrappedCard(Slash.ClassName);
                slash.AddSubCard(card);
                slash = RoomLogic.ParseUseCard(room, slash);
                result.Add(slash);

                WrappedCard jink = new WrappedCard(Jink.ClassName);
                jink.AddSubCard(card);
                jink = RoomLogic.ParseUseCard(room, jink);
                result.Add(jink);
            }
            else
                result.Add(card);

            foreach (string skill in GetKnownSkills(player, true, from))
            {
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

        public virtual bool IsGeneralExpose()
        {
            return false;
        }

        public bool WillShowForAttack()
        {
            if (IsGeneralExpose() || show_immediately || skill_invoke_postpond) return true;

            bool reward = true;
            bool kill = false;
            foreach (Player p in room.GetOtherPlayers(self))
            {
                if (HasSkill("xiongyi", p) && p.GetMark("@arise") > 0 && RoomLogic.WillBeFriendWith(room, self, p))
                    return true;

                    if (p.HasShownOneGeneral())
                    reward = false;
                if (IsWeak(p) && id_tendency[p] != "unknown" && id_tendency[p] != id_tendency[self])
                    kill = true;
            }
            //首亮奖励
            if (reward && room.Setting.GameMode == "Hegemony" || IsWeak()) return true;
            if (kill && room.Current == self) return true;

            return IsSituationClear();
        }

        public bool WillShowForDefence()
        {
            if (IsGeneralExpose() || show_immediately || skill_invoke_postpond) return true;

            bool reward = true;
            foreach (Player p in room.GetOtherPlayers(self))
            {
                if (HasSkill("xiongyi", p) && p.GetMark("@arise") > 0 && RoomLogic.WillBeFriendWith(room, self, p))
                    return true;

                if (p.HasShownOneGeneral())
                    reward = false;
            }

            if (reward && room.Setting.GameMode == "Hegemony" || IsWeak()) return true;

            return IsSituationClear();
        }

        public virtual bool IsSituationClear()
        {
            return true;
        }


        public bool WillShowForMasochism()
        {
            if (IsGeneralExpose() || show_immediately || skill_invoke_postpond) return true;

            bool reward = true;
            foreach (Player p in room.GetAlivePlayers())
                if (p.HasShownOneGeneral())
                    reward = false;

            if (reward && room.Setting.GameMode == "Hegemony" || IsWeak()) return true;
            DamageStruct damage = (DamageStruct)room.GetTag("CurrentDamageStruct");

            if (damage.From != null && damage.From.Alive && IsFriend(damage.From)) return true;

            if (room.GetAlivePlayers().Count < 3) return true;

            if (self.GetLostHp() == 0 && GetCards(Peach.ClassName, self).Count > 0) return false;

            return true;
        }

        public bool NeedShowImmediately()
        {
            if (this is SmartAI smart)
            {
                smart.CountPlayers();
                return show_immediately;
            }

            return false;
        }


        public bool CanResist(Player player, int damage)
        {
            foreach (SkillEvent e in skill_events.Values)
                if (e.CanResist(this, damage)) return true;

            if (HasArmorEffect(player, BreastPlate.ClassName) && damage >= player.Hp) return true;

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
            return (RoomLogic.PlayerHasShownSkill(room, player, "kongcheng|kongcheng_jx") && player.IsKongcheng());
        }

        public double GetLostEquipEffect(Player player, List<int> except = null)
        {
            List<int> ids = player.GetEquips();

            List<double> values = new List<double>();
            foreach (int id in ids)
            {
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
                return armor == EightDiagram.ClassName && HasSkill("bazhen", player);

            if (player == self)
                return RoomLogic.HasArmorEffect(room, player, armor);
            else
            {
                if (RoomLogic.HasArmorEffect(room, player, armor, false)) return true;
                if (RoomLogic.HasArmorEffect(room, player, armor))
                {
                    List<ViewHasSkill> skills = Engine.ViewHas(room, player, armor, "armor");
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
                    return RoomLogic.CanGetCard(room, from, to, id);
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
        public ScoreStruct FindCards2Discard(Player from, Player player, string Reason, string flags,
                                  HandlingMethod method, int times = 1, bool onebyone = false, List<int> disable = null)
        {
            if (string.IsNullOrEmpty(flags))
                Debug.Assert(!string.IsNullOrEmpty(flags));

            List<int> result = new List<int>();
            disable = disable ?? new List<int>();
            ScoreStruct score = new ScoreStruct
            {
                Players = new List<Player> { player }
            };
            double expect = 0, expect_self = 0;
            if (!CanOperate(from, player, flags, method))
                return score;

            if (this is StupidAI && !IsFriend(from, player) && !IsEnemy(from, player) && method != HandlingMethod.MethodGet)
                return score;

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
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (HasSkill("guicai|guidao", p) && IsFriend(p, from)) wizzard_friend = true;
                    if (HasSkill("guicai|guidao", p) && IsEnemy(p, from)) wizzard_enemy = true;
                }

                foreach (int id in player.JudgingArea)
                {
                    WrappedCard card = room.GetCard(id);
                    if (card.Name == Indulgence.ClassName) indu = id;
                    else if (card.Name == SupplyShortage.ClassName) supply = id;
                    else if (card.Name == Lightning.ClassName) light = id;
                }

                if (light >= 0 && !wizzard_friend)
                {
                    foreach (Player p in room.GetAlivePlayers())
                    {
                        DamageStruct damage = new DamageStruct(room.GetCard(light), null, p, 3, DamageStruct.DamageNature.Thunder);
                        int point = DamageEffect(damage, DamageStruct.DamageStep.Done);
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
                        expect += 11;
                        if (IsFriend(player))
                            expect_self += 11;

                        if (IsFriend(from) && method == HandlingMethod.MethodGet)
                        {
                            double use_value = GetUseValue(light, from);
                            if (use_value > 0)
                            {
                                expect += use_value;
                                expect_self += use_value;
                            }
                        }
                    }
                    if (times > result.Count && indu >= 0 && !(HasSkill("guanxing|yizhi|guanxing_jx", player)
                            && room.GetAlivePlayers().Count > 3) && !HasSkill("guanxing+yizhi", player))
                    {
                        result.Add(indu);
                        expect += 10;
                        if (IsFriend(player))
                            expect_self += 10;
                        else if (IsEnemy(player))
                            expect_self -= 10;

                        if (IsFriend(from) && method == HandlingMethod.MethodGet)
                        {
                            double use_value = GetUseValue(indu, from);
                            if (use_value > 0)
                            {
                                expect += use_value;
                                expect_self += use_value;
                            }
                        }
                    }
                    if (times > result.Count && supply >= 0 && !(HasSkill("guanxing|yizhi|guanxing_jx", player)
                            && room.GetAlivePlayers().Count > 3) && !HasSkill("guanxing+yizhi", player))
                    {
                        result.Add(supply);
                        expect += 8;
                        if (IsFriend(player))
                            expect_self += 8;
                        else if (IsEnemy(player))
                            expect_self -= 8;

                        if (IsFriend(from) && method == HandlingMethod.MethodGet)
                        {
                            double use_value = GetUseValue(supply, from);
                            if (use_value > 0)
                            {
                                expect += use_value;
                                expect_self += use_value;
                            }
                        }
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
                        result.AddRange(player.GetCards("h"));
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
                        if (IsFriend(from) && method == HandlingMethod.MethodGet && (GetEnemies(self).Count > 0 || IsSituationClear()))
                        {
                            if (FindSameEquipCards(room.GetCard(id), false, false).Count == 0)
                            {
                                double use_value = GetUseValue(id, from);
                                if (use_value > 0)
                                    value -= use_value / 5;             //拿友方的价值大减
                            }
                        }

                        if (value < 0)
                        {
                            values.Add(value);
                            maps.Add(id, value);
                        }
                    }
                    ids = new List<int>(maps.Keys);
                    ids.Sort((x, y) => { return maps[x] < maps[y] ? -1 : 1; });
                    values.Sort((x, y) => { return x < y ? -1 : 1; });

                    double adjust = 0;                                               //if not move only once,
                    if (!onebyone)
                    {                                                //must ajust for xiaoji/xuanlue  **for now
                        if (HasSkill("xiaoji|xuanfeng", player)) adjust += 6;
                        if (HasSkill("xuanlue", player)) adjust += 3;
                    }
                    for (int i = 0; i < Math.Min(values.Count, times - result.Count); i++)
                    {
                        int id = ids[i];
                        double value = maps[id] + (i == 0 ? 0 : adjust);
                        if (!result.Contains(id))
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

                Dictionary<int, double> maps = new Dictionary<int, double>();
                List<int> equip_ids = new List<int>();
                if (flags.Contains("e") && CanOperate(from, player, "e", method) && times > result.Count)
                {
                    foreach (int id in player.GetEquips())
                        if (!disable.Contains(id) && CanOperate(from, player, id.ToString(), method))
                            equip_ids.Add(id);

                    foreach (int id in equip_ids)
                    {
                        double value = GetKeepValue(id, player);     // todo:
                        if (value > 0)
                        {
                            //在局势不明朗时对拆装备的打分进行修正,让AI不会积极的行动
                            if (!IsSituationClear())
                            {
                                WrappedCard card = room.GetCard(id);
                                if (method != HandlingMethod.MethodGet || GetSameEquip(card, from) != null)
                                {
                                    FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                                    if (fcard is Horse || fcard is SpecialEquip)      //马匹的分值为一半 
                                        value /= 2;
                                    if (fcard is Armor)                               //护甲为2/3
                                        value = value * 2 / 3;
                                }
                            }
                            if (IsFriend(from) && method == HandlingMethod.MethodGet)
                            {
                                double use_value = GetUseValue(id, from);
                                value += use_value;
                            }
                            if (this is StupidAI && from == self && !IsEnemy(from, player) && method == HandlingMethod.MethodGet && value > 0 && from.GetRoleEnum() == PlayerRole.Renegade)
                            {
                                if (GetPlayerTendency(player) == "rebel" && Process.Contains(">"))
                                    value /= 5;
                                else if ((GetPlayerTendency(player) == "loyalist" || player.GetRoleEnum() == PlayerRole.Lord) && Process.Contains("<"))
                                    value /= 5;
                            }

                            maps.Add(id, value);
                        }
                    }
                }

                equip_ids = new List<int>(maps.Keys);
                equip_ids.Sort((x, y) => { return maps[x] > maps[y] ? -1 : 1; });

                double hand_value = 0;
                double singel_value = 0;
                if (flags.Contains("h") && CanOperate(from, player, "h", method) && times > result.Count)
                {
                    foreach (int id in player.GetCards("h"))
                    {
                        if (disable.Contains(id)) continue;
                        if (GetKnownCards(player).Contains(id))
                            hand_value += GetKeepValue(id, player);
                        else
                            hand_value += (player.HandcardNum - hand_ajust > 8 ? 2 : (player.HandcardNum - hand_ajust > 5 ? 3 : 4));
                    }
                    singel_value = hand_value / (player.HandcardNum - hand_ajust);

                    //room.OutPut("hand value " + hand_value.ToString() + " single value " + singel_value.ToString());

                    if (singel_value == hand_value)
                    {
                        if (NeedKongcheng(player))
                            hand_value = -100;
                    }

                    //在局势不明朗时对拆手牌的打分进行修正,让AI不会积极的行动，手牌分数仅为1/4
                    if (room.Setting.GameMode == "Hegemony" && !IsSituationClear())
                        singel_value /= 4;

                    if (IsFriend(from) && singel_value > 0 && method == HandlingMethod.MethodGet)
                        singel_value *= 1.3;

                    if (this is StupidAI && from == self && !IsEnemy(from, player))
                    {
                        if (from.GetRoleEnum() == PlayerRole.Renegade)
                        {
                            if (GetPlayerTendency(player) == "rebel" && Process.Contains(">"))
                                singel_value /= 5;
                            else if ((GetPlayerTendency(player) == "loyalist" || player.GetRoleEnum() == PlayerRole.Lord) && Process.Contains("<"))
                                singel_value /= 5;
                        }
                        else if (!IsSituationClear())
                            singel_value /= 3;
                    }
                }

                double adjust = 0;                                               //if not move only once,
                if (!onebyone)
                {                                                //must ajust for xiaoji/xuanlue  **for now
                    if (HasSkill("xiaoji|xuanfeng", player)) adjust += 6;
                    if (HasSkill("xuanlue", player)) adjust += 3;
                }
                if ((HasSkill("lianying", player) && (Reason == Dismantlement.ClassName || Reason == Snatch.ClassName || times - result.Count >= player.HandcardNum))
                    || (HasSkill("shoucheng", player) && player.HandcardNum == 1))
                {
                    adjust -= singel_value * 3 / 4;
                }

                for (int i = 0; i < times - result.Count; i++)
                {
                    int hand = 0;
                    int eq = 0;
                    double value = (equip_ids.Count > 0) ?
                        value = maps[equip_ids[0]] : 0;

                    //room.OutPut("equip value " + value.ToString());

                    if (hand_value > 0 && (equip_ids.Count == 0 || singel_value > value) && singel_value > 0)
                    {
                        bool append = false;
                        while (!append)
                        {
                            int id = room.GetRandomHandCard(player);
                            if (!result.Contains(id) && !disable.Contains(id))
                            {
                                result.Add(id);
                                append = true;
                            }
                        }
                        expect += singel_value;
                        if (IsFriend(player))
                            expect_self -= singel_value;
                        else if (IsEnemy(player))
                            expect_self += singel_value;
                        else
                            expect_self += singel_value / 3;

                        hand++;
                        hand_value = (hand == player.HandcardNum ? 0 : hand_value - singel_value);
                        if (singel_value == hand_value)
                        {
                            if (NeedKongcheng(player))
                                hand_value = -100;
                        }
                    }
                    else if (value > singel_value && value > 0 && (eq == 0 || value + adjust > 0))
                    {
                        int id = equip_ids[0];
                        equip_ids.Remove(id);
                        result.Add(id);
                        expect -= (value + (eq == 0 ? 0 : adjust));
                        if (IsFriend(player))
                            expect_self -= (value + (eq == 0 ? 0 : adjust));
                        else if (IsEnemy(player))
                            expect_self += (value + (eq == 0 ? 0 : adjust));
                        else
                            expect_self += (value + (eq == 0 ? 0 : adjust / 3));
                        eq++;
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

            score.Ids = result;
            if (priority_enemies.Contains(player))
                expect_self *= 1.2;

            score.Score = expect_self;
            return score;
        }

        public bool WillSkipPlayPhase(Player player, Player exception_helper = null)
        {
            if (player.Phase == PlayerPhase.Play) return false;
            if (player.IsSkipped(PlayerPhase.Play) || (room.Current != player && !player.FaceUp)) return true;
            if (player.HasFlag("willSkipPlayPhase")) return true;

            if (RoomLogic.PlayerContainsTrick(room, player, Indulgence.ClassName))
            {
                if (HasSkill("shensu", player, true)) return false;
                if (HasSkill("qiaobian", player, true) && !player.IsKongcheng()) return false;
                List<Player> friends = GetFriends(player);
                foreach (Player f in friends)
                {
                    if (exception_helper != null && f == exception_helper) continue;
                    if (GetCards(Nullification.ClassName, f).Count > 0) return false;
                    if (room.GetAlivePlayers().IndexOf(f) < room.GetAlivePlayers().IndexOf(player)
                            && (GetCards(Dismantlement.ClassName, f).Count > 0 && RoomLogic.CanDiscard(room, f, player, "j"))
                            || (GetCards(Snatch.ClassName, f).Count > 0 && RoomLogic.CanGetCard(room, f, player, "j")))
                        return false;
                }

                Player retrialer = GetWizzardRaceWinner(Indulgence.ClassName, player, player);
                if (retrialer != null && IsFriend(retrialer, player)) return false;
                if (HasSkill("guanxing+yizhi", player, true) || (HasSkill("guanxing|yizhi", player, true) && room.AliveCount() >= 4)
                        && (retrialer == null || !IsEnemy(retrialer, player))) return false;

                WrappedCard indu = null;
                foreach (int id in player.JudgingArea)
                {
                    WrappedCard card = room.GetCard(id);
                    if (card.Name == Indulgence.ClassName)
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
                    Player last = room.GetLastAlive(player, i, false);
                    if (last.FaceUp)
                        help = last;
                    i++;
                }
                if (help != null && help != room.Current && help != player && IsFriend(help, player) && HasSkill("guanxing|yizhi", help, true))
                {
                    int count = Math.Min(5, room.AliveCount());
                    if (HasSkill("guanxing+yizhi", help, true)) count = 5;
                    if (count - help.JudgingArea.Count >= 4 && !RoomLogic.PlayerContainsTrick(room, help, Lightning.ClassName) && (retrialer == null || !IsEnemy(retrialer, player)))
                        return false;
                }

                return true;
            }

            return false;
        }

        public bool WillSkipDrawPhase(Player player, Player exception_helper = null)
        {
            if (player.IsSkipped(PlayerPhase.Draw) || (room.Current != player && !player.FaceUp)) return true;
            if (player.HasFlag("willSkipDrawPhase")) return true;

            if (RoomLogic.PlayerContainsTrick(room, player, SupplyShortage.ClassName))
            {
                List<Player> friends = GetFriends(player);
                foreach (Player f in friends)
                {
                    if (exception_helper != null && f == exception_helper) continue;
                    if (GetCards(Nullification.ClassName, f).Count > 0) return false;
                    if (room.GetAlivePlayers().IndexOf(f) < room.GetAlivePlayers().IndexOf(player)
                            && (GetCards(Dismantlement.ClassName, f).Count > 0 && RoomLogic.CanDiscard(room, f, player, "j"))
                            || (GetCards(Snatch.ClassName, f).Count > 0 && RoomLogic.CanGetCard(room, f, player, "j")))
                        return false;
                }

                Player retrialer = GetWizzardRaceWinner(SupplyShortage.ClassName, player, player);
                if (retrialer != null && IsFriend(retrialer, player)) return false;
                if (HasSkill("guanxing+yizhi", player, true) || (HasSkill("guanxing|yizhi", player, true) && room.AliveCount() >= 4)
                        && (retrialer == null || !IsEnemy(retrialer, player))) return false;

                WrappedCard sppply = null;
                foreach (int id in player.JudgingArea)
                {
                    WrappedCard card = room.GetCard(id);
                    if (card.Name == SupplyShortage.ClassName)
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
                    Player last = room.GetLastAlive(player, i, false);
                    if (last.FaceUp)
                        help = last;
                    i++;
                }
                if (help != null && help != player && IsFriend(help, player) && HasSkill("guanxing|yizhi", help, true))
                {
                    int count = Math.Min(5, room.AliveCount());
                    if (HasSkill("guanxing+yizhi", help, true)) count = 5;
                    if (count - help.JudgingArea.Count >= 4 && !RoomLogic.PlayerContainsTrick(room, help, Lightning.ClassName) && (retrialer == null || !IsEnemy(retrialer, player)))
                        return false;
                }

                return true;
            }
            return false;
        }

        public void SetGuanxingResult(Player who, List<int> ups)
        {
            guanxing = new KeyValuePair<Player, List<int>>(who, ups);
        }

        public bool IsGuanxingEffected(Player player, bool expect_effect, WrappedCard card)
        {
            if (player != room.Current)
            {
                Player next = null;
                int i = 1;
                while (next == null)
                {
                    Player p = room.GetNextAlive(room.Current, i, false);
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
            if (player.Phase == PlayerPhase.Judge && room.ContainsTag("judge_draw") && (int)room.GetTag("judge_draw") > 0)
            {
                judging++;
                List<JudgeStruct> judges = room.GetJudgeList();
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
                WrappedCard.CardSuit suit = HasSkill("hongyan|hongyan_jx", player) && j_card.Suit == WrappedCard.CardSuit.Spade ? WrappedCard.CardSuit.Heart : j_card.Suit;
                if (expect_effect)
                    return (reason == Indulgence.ClassName && suit != WrappedCard.CardSuit.Heart) || (reason == SupplyShortage.ClassName && suit != WrappedCard.CardSuit.Club)
                                       || (reason == Lightning.ClassName && suit == WrappedCard.CardSuit.Spade && j_card.Number <= 9 && j_card.Number >= 2);
                else
                    return (reason == Indulgence.ClassName && suit == WrappedCard.CardSuit.Heart) || (reason == SupplyShortage.ClassName && suit == WrappedCard.CardSuit.Club)
                                       || (reason == Lightning.ClassName && (suit != WrappedCard.CardSuit.Spade || j_card.Number > 9 || j_card.Number < 2));
            }
            else if (guanxing.Key != null && guanxing.Value.Count >= index + 1 - player.GetPile("incantation").Count)
                return (IsFriend(guanxing.Key, player) && !expect_effect) || (IsEnemy(guanxing.Key, player) && expect_effect);

            return false;
        }

        public bool NeedKongcheng(Player player)
        {
            if (HasSkill("kongcheng|kongcheng_jx", player) && player.HandcardNum <= 1)
            {
                List<Player> enemies = new List<Player>();
                Player next = room.GetNextAlive(room.Current, 1 , false);
                while (next != player)
                {
                    if (IsEnemy(next, player) && !WillSkipPlayPhase(next) && RoomLogic.CanSlash(room, next, player))
                        enemies.Add(next);
                    next = room.GetNextAlive(next, 1, false);
                }
                if (enemies.Count > 0 && player.IsKongcheng()) return true;
                if (enemies.Count > player.Hp + 1)
                {
                    bool garbage = true;
                    foreach (int id in GetKnownCards(player))
                        if (IsCard(id, Jink.ClassName, player, self) || IsCard(id, Peach.ClassName, player, self) || IsCard(id, Analeptic.ClassName, player, self))
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
                        next = room.GetNextAlive(next, 1, false);
                    }
                    if (player.Hp > enemies.Count + 2) return true;
                }
            }
            return false;
        }

        public bool NeedToLoseHp(DamageStruct damage, bool passive, bool recover, DamageStruct.DamageStep step = DamageStruct.DamageStep.Caused)
        {
            ScoreStruct score = GetDamageScore(damage, step);
            if (score.Score < -6 || score.Damage.Damage != 1) return false;

            int n = damage.To.Hp;
            if (recover)
                return n == damage.To.MaxHp;

            if (!passive)
            {
                if (!damage.To.IsWounded() && HasSkill("rende", damage.To) && !WillSkipPlayPhase(damage.To) && GetFriends(damage.To).Count > 1
                        && ((room.Current == damage.To && damage.To.Phase <= PlayerPhase.Play
                        && damage.To.GetMark("rende") < 2 && damage.To.HandcardNum >= 2 - damage.To.GetMark("rende"))
                            || (damage.To.Phase == PlayerPhase.NotActive && MaySave(damage.To))))
                    return true;
                else if (!damage.To.IsWounded() && HasSkill("jieyin", damage.To) && !WillSkipPlayPhase(damage.To) && ((room.Current == damage.To
                        && damage.To.Phase <= PlayerPhase.Play && !damage.To.HasUsed("JieyinCard") && damage.To.HandcardNum >= 2)
                            || (damage.To.Phase == PlayerPhase.NotActive && MaySave(damage.To))))
                {
                    foreach (Player p in GetFriends(damage.To))
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
                foreach (Player p in GetFriends(xiangxiang))
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
        public int DamageEffect(DamageStruct damage, DamageStruct.DamageStep step)
        {
            if (damage.Steped >= step) return damage.Damage;

            Player to = damage.To;
            Player from = damage.From;

            if (step <= DamageStruct.DamageStep.Caused && damage.Card != null && from != null && HasSkill("wuyan", from))
            {
                FunctionCard fcard = Engine.GetFunctionCard(damage.Card.Name);
                if (fcard is TrickCard) return 0;
            }

            if (from != null && from.Alive && damage.To.HasFlag(string.Format("yinju_{0}", from.Name)))
                return 0;

            foreach (SkillEvent e in skill_events.Values)
                e.DamageEffect(this, ref damage, step);
            if (damage.Damage < 0) return 0;

            if (damage.Card != null)
            {
                if (damage.Card.Name.Contains(Slash.ClassName) && step <= DamageStruct.DamageStep.Caused)
                {
                    if (damage.From != null && damage.From.Alive)
                    {
                        if (pre_drink != null)
                            damage.Damage++;
                        damage.Damage += damage.From.GetMark("drank");
                    }
                }

                UseCard e = Engine.GetCardUsage(damage.Card.Name);
                if (e != null) e.DamageEffect(this, ref damage, step);

                if (from != null && damage.Card != null && damage.Card.Name.Contains(Slash.ClassName) && from.HasWeapon(GudingBlade.ClassName) && to.IsKongcheng())
                    damage.Damage++;
            }

            if (damage.Steped < DamageStruct.DamageStep.Done && step <= DamageStruct.DamageStep.Done)
            {
                if (damage.Card != null && damage.Card.HasFlag("chijie")) return 0;
                if (damage.Card != null && damage.Card.Name.Contains(Slash.ClassName) && HasSkill("renshi", to)) return 0;
                if (damage.Card != null && HasSkill("wuyan", to))
                {
                    FunctionCard fcard = Engine.GetFunctionCard(damage.Card.Name);
                    if (fcard is TrickCard) return 0;
                }
                if (from != null && from != to && HasSkill("daigong", to) && !to.HasFlag("daigong") && !to.IsKongcheng() && IsFriend(from, to)) return 0;
                if (damage.Nature == DamageStruct.DamageNature.Fire && damage.To.GetMark("@gale") > 0) damage.Damage++;
                if (damage.Nature != DamageStruct.DamageNature.Thunder && damage.To.GetMark("@fog") > 0) return 0;
                if (damage.To.GetMark("@tangerine") > 0) return 0;
                if (damage.Nature == DamageStruct.DamageNature.Fire && HasSkill("shixin", damage.To)) return 0;
                if (HasSkill("yinshi", damage.To) && damage.To.GetMark("@dragon") == 0 && damage.To.GetMark("@phenix") == 0
                    && !damage.To.EquipIsBaned(1) && !damage.To.GetArmor() && (damage.Card != null && Engine.GetFunctionCard(damage.Card.Name).TypeID == CardType.TypeTrick
                    || damage.Nature != DamageStruct.DamageNature.Normal))
                    return 0;
                if (HasSkill("andong", to) && from != null && from != to && from != to)
                {
                    int count = from.HandcardNum;
                    if (damage.Card != null)
                    {
                        foreach (int id in damage.Card.SubCards)
                            if (room.GetCardOwner(id) == from && room.GetCardPlace(id) == Place.PlaceHand)
                                count--;
                    }
                    if (count <= 0) return 0;

                    if (IsFriend(from, to) && (!to.HasFlag("andong") || damage.Chain))
                        return 0;
                }

                if (damage.From != null && damage.From.Alive && HasSkill("gongqing", damage.To) && damage.Damage > 1)
                {
                    bool weapon = true;
                    if (damage.Card != null && damage.Card.SubCards.Contains(damage.From.Weapon.Key))
                        weapon = false;
                    int range = RoomLogic.GetAttackRange(room, damage.From, weapon);
                    if (range < 3) damage.Damage = 1;
                }

                if (from != null && HasSkill("mingshi", to) && !from.HasShownAllGenerals())
                {
                    if (!damage.From.HasShownOneGeneral())
                        damage.Damage--;
                    else
                    {
                        bool shown = false;
                        if (!string.IsNullOrEmpty(damage.Reason) && RoomLogic.PlayerHasSkill(room, damage.From, damage.Reason)
                            && !RoomLogic.PlayerHasShownSkill(room, damage.From, damage.Reason))
                            shown = true;
                        if (damage.Card != null && !string.IsNullOrEmpty(damage.Card.Skill) && RoomLogic.PlayerHasSkill(room, damage.From, damage.Card.Skill)
                            && !RoomLogic.PlayerHasShownSkill(room, damage.From, damage.Card.Skill))
                            shown = true;

                        if (!shown)
                            damage.Damage--;
                    }
                }
                if (from != null && from.Alive && from != to && HasSkill("yuanyu", to) && room.GetNextAlive(from) != to && room.GetNextAlive(to) != from)
                    damage.Damage--;

                if (damage.Damage <= 0) return 0;

                bool armor_ignore = false;
                if (from != null && damage.Card != null && damage.Card.Name.Contains(Slash.ClassName))
                {
                    if (from.HasWeapon(QinggangSword.ClassName) || from.HasWeapon(Saber.ClassName))
                        armor_ignore = true;
                    else if (HasSkill("paoxiao|paoxiao_fz", from))
                    {
                        Player lord = FindPlayerBySkill("shouyue");
                        if (lord != null && RoomLogic.PlayerHasShownSkill(room, lord, "shouyue") && RoomLogic.WillBeFriendWith(room, from, lord))
                            armor_ignore = true;
                    }
                    else if (HasSkill("anjian", from) && !RoomLogic.InMyAttackRange(room, to, from))
                        armor_ignore = true;
                }

                if (!armor_ignore)
                {
                    if (damage.Damage > 0 && damage.Nature == DamageStruct.DamageNature.Fire && HasArmorEffect(to, Vine.ClassName)) damage.Damage += 1;
                    if (damage.Damage > 1 && HasArmorEffect(to, SilverLion.ClassName)) damage.Damage = 1;
                    if (damage.Nature != DamageStruct.DamageNature.Normal && HasArmorEffect(to, PeaceSpell.ClassName)) damage.Damage = 0;
                    if (damage.Damage >= to.Hp && HasArmorEffect(to, BreastPlate.ClassName)) damage.Damage = 0;
                }
            }

            return damage.Damage;
        }
        public bool MaySave(Player player)
        {
            if (player == null || !player.Alive) return false;
            Player next = room.GetNextAlive(room.Current, 1, false);
            List<Player> enemies = new List<Player>();
            while (next != player)
            {
                if (IsEnemy(next, player) && !WillSkipPlayPhase(next))
                    enemies.Add(next);
                next = room.GetNextAlive(next, 1, false);
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
            else if (fcard is SpecialEquip)
            {
                if (player.GetDefensiveHorse())
                    return room.GetCard(player.DefensiveHorse.Key);

                return room.GetCard(player.OffensiveHorse.Key);
            }

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
                if (player.GetDefensiveHorse() || player.GetSpecialEquip())
                    defense += 0.5;
            }
            else
            {
                if (player.GetArmor())
                    defense += 1;
                if (player.GetDefensiveHorse() || player.GetSpecialEquip())
                    defense += 0.8;
            }

            defense += player.HandcardNum * 0.35;
            defense += player.GetPile("wooden_ox").Count * 0.25;
            if (!for_attack)
            {
                if (player.HasTreasure(LuminouSpearl.ClassName))
                    defense += 1;
                if (RoomLogic.HasTreasureEffect(room, player, JadeSeal.ClassName))
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
            List<WrappedCard> cards = GetViewAsCards(player, id);
            double value = 0;
            foreach (WrappedCard card in cards)
            {
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
                    basic += card_event.CardValue(this, player, true, card, room.GetCardPlace(id));

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
                if (room.GetCardOwner(id) == player)
                {
                    WrappedCard equip = room.GetCard(id);
                    FunctionCard _fcard = Engine.GetFunctionCard(equip.Name);
                    if (_fcard is EquipCard)
                    {
                        if (room.GetCardPlace(id) == Place.PlaceEquip)
                            basic -= GetKeepValue(id, player);
                        else if (GetSameEquip(equip, player) == null)
                        {
                            basic -= GetUseValue(equip, player);
                        }
                    }
                    else if (room.GetCardPlace(id) == Place.PlaceHand || place == Place.PlaceHand)
                    {
                        handcard_ajust += GetKeepValue(id, player, Place.PlaceHand);
                        handcard_count++;
                    }
                }
            }

            int over_flow = GetOverflow(player);
            if (over_flow < handcard_count && handcard_ajust != 0)
                basic -= (handcard_count - GetOverflow(player)) / handcard_count * handcard_ajust;

            foreach (SkillEvent e in skill_events.Values)
                basic += e.CardValue(this, player, card, true, place);

            SkillEvent ev = Engine.GetSkillEvent(card.Skill);
            if (ev != null)
                basic += ev.UseValueAdjust(this, player, new List<Player>(), card);

            return basic;
        }

        public double GetKeepValue(int id, Player player, Place place = Place.PlaceUnknown, bool ignore_lose_equip = false)
        {
            if (place == Place.PlaceUnknown && room.GetCardOwner(id) == player)
                place = room.GetCardPlace(id);

            List<WrappedCard> cards = GetViewAsCards(player, id, place);
            List<double> sum = new List<double>();
            foreach (WrappedCard card in cards)
            {
                double card_v = GetKeepValue(card, player, place, ignore_lose_equip);
                sum.Add(card_v);
            }
            sum.Sort((x, y) => { return x < y ? -1 : 1; });
            double value = sum[sum.Count - 1];
            sum.RemoveAt(sum.Count - 1);
            for (int i = sum.Count - 1; i >= 0; i--)
                value += sum[i] / (sum.Count - i + 2);

            return value;
        }

        public double GetKeepValue(WrappedCard card, Player player, Place place = Place.PlaceUnknown, bool ignore_lose_equip = false, bool find_same = true)
        {
            if (card.IsVirtualCard() && card.SubCards.Count != 1)
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
                if (e != null)
                    basic += e.CardValue(this, player, false, card, place);
            }
            else if (find_same && fcard is EquipCard)
            {
                WrappedCard same = GetSameEquip(card, player);
                if (same != null)
                    basic -= GetKeepValue(same, player, Place.PlaceUnknown, false, false);
            }
            else if (place == Place.PlaceHand)
            {
                if (card.Name == Peach.ClassName && HasSkill(MasochismSkill, player) && player.IsWounded())
                {
                    basic += 1;
                    if (player.Hp <= 1) basic += 2;
                }
                if (card.Name == Analeptic.ClassName && HasSkill(MasochismSkill, player) && player.Hp <= 1)
                    basic += 2;
            }

            foreach (string skill in GetKnownSkills(player))
            {
                if (ignore_lose_equip && LoseEquipSkill.Contains(skill)) continue;
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
            if (class_name.Contains(Slash.ClassName)) class_name = Slash.ClassName;
            if (class_name.Contains(Slash.ClassName) || class_name == Jink.ClassName || class_name == Peach.ClassName || class_name == Analeptic.ClassName)
            {
                int n = 0;
                foreach (string name in class_names)
                    if (name == class_name)
                        n++;
                if (n > 0 && basic > 0)
                    dec = basic - basic / (2 * n);
            }

            return dec;
        }

        public List<double> SortByUseValue(ref List<WrappedCard> cards, bool descending = true)
        {
            Dictionary<WrappedCard, double> card_values = new Dictionary<WrappedCard, double>();
            List<WrappedCard> results = new List<WrappedCard>();

            foreach (WrappedCard card in cards)
            {
                card_values[card] = GetUseValue(card, self);
                //room.OutPut(card.Name + " use value is " + card_values[card].ToString());
            }

            cards.Sort((x, y) => { return descending ? card_values[x] > card_values[y] ? -1 : 1 : card_values[x] < card_values[y] ? -1 : 1; });

            List<double> points = new List<double>();
            foreach (WrappedCard card in cards)
                points.Add(card_values[card]);

            return points;
        }

        public List<double> SortByUseValue(ref List<int> ids, bool descending = true)
        {
            Dictionary<int, double> card_values = new Dictionary<int, double>();
            foreach (int id in ids)
            {
                card_values[id] = GetUseValue(id, self);
                //room.OutPut(id.ToString() + " use value is " + card_values[id].ToString());
            }

            ids.Sort((x, y) => { return descending ? card_values[x] > card_values[y] ? -1 : 1 : card_values[x] < card_values[y] ? -1 : 1; });

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
                Debug.Assert(cards.Count != 0, room.GetCard(id).Name);
                int2cards.Add(id, cards);
                foreach (WrappedCard card in cards)
                    card_values[card] = GetKeepValue(card, self, place);
            }

            //对类似小鸡这类玩装备的技能重新评分，只计算一次价值最低的装备
            if (HasSkill(LoseEquipSkill))
            {
                List<WrappedCard> re_sort = new List<WrappedCard>(card_values.Keys);
                re_sort.Sort((x, y) => { return card_values[x] < card_values[y] ? -1 : 1; });
                int id = -1;
                foreach (WrappedCard card in re_sort)
                {

                    Place place = room.GetCardPlace(card.GetEffectiveId());
                    if (place == Place.PlaceEquip)
                    {
                        if (id == -1)
                            id = card.GetEffectiveId();
                        else if (id >= 0 && card.GetEffectiveId() != id)
                            card_values[card] = GetKeepValue(card, self, place, true);
                    }
                }
            }

            while (results.Count < ids.Count)
            {
                WrappedCard best = null;
                double best_value = -10000000;
                foreach (WrappedCard card in card_values.Keys)
                {
                    double card_value = card_values[card];
                    card_value -= CaculateSameClass(card.Name, class_names);
                    if (card_value >= best_value)
                    {
                        best = card;
                        best_value = card_value;
                    }
                }

                if (best.Name.Contains(Slash.ClassName))
                    class_names.Add(Slash.ClassName);
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

        /*
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
        */
        public void SortByDefense(ref List<Player> players, bool descending = true)
        {
            Dictionary<Player, double> values = new Dictionary<Player, double>();
            foreach (Player p in players)
                values[p] = GetDefensePoint(p, true);

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
        public void SortByHandcards(ref List<Player> players, bool descending = true)
        {
            players.Sort((x, y) =>
            {
                if (x.HandcardNum < y.HandcardNum)
                    return descending ? -1 : 1;
                else
                    return descending ? 1 : -1;
            });
        }

        public void SortByHp(ref List<Player> players, bool descending = true)
        {
            players.Sort((x, y) =>
            {
                if (x.Hp < y.Hp)
                    return descending ? -1 : 1;
                else
                    return descending ? 1 : -1;
            });
        }

        public bool HasCrossbowEffect(Player player)
        {
            WrappedCard slash = new WrappedCard(Slash.ClassName);
            if (HasSkill("kuangcai", player) && player.GetMark("kuangcai") < 3) return true;

            if (Engine.CorrectCardTarget(room, TargetModSkill.ModType.Residue, player, slash) > 5)
                return true;

            if (player.HasWeapon(CrossBow.ClassName)) return true;
            List<int> ids = GetKnownCards(player);
            foreach (int id in ids)
                if (room.GetCard(id).Name == CrossBow.ClassName) return true;

            return false;
        }
        public int PlayerGetRound(Player player, Player source = null)
        {
            if (source == null) source = room.Current;
            if (player == source || !player.Alive) return 0;
            int players_num = room.AliveCount();

            int round = 0;
            Player next = room.GetNextAlive(source, 1, false);
            while (next != player)
            {
                round++;
                next = room.GetNextAlive(next, 1, false);
            }

            return round % players_num;
        }
        public bool ToTurnOver(Player player, int n = 1, string reason = null)
        {
            if (!player.FaceUp) return false;
            if (!string.IsNullOrEmpty(reason) && reason == "fangzhu" && player.Hp == 1 && ai_AOE_data.Card != null)
                if (ai_AOE_data.To.Contains(player) && IsCardEffect(ai_AOE_data.Card, player, ai_AOE_data.From)
                    && PlayerGetRound(player) > PlayerGetRound(self) && player.IsKongcheng())
                    return false;

            if (n > 1)
                if ((player.Phase < PlayerPhase.Discard && (HasSkill(ActiveCardneedSkill, player) || HasCrossbowEffect(player)))
                    || (player.Phase == PlayerPhase.NotActive && HasSkill(NotActiveCardneedSkill, player)))
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
            if (RoomLogic.HasTreasureEffect(room, player, JadeSeal.ClassName) && player.HasShownOneGeneral()) count++;
            foreach (string skill in skills)
            {
                if (skill == "luoyi") count--;
                if (skill.Contains("yingzi")) count++;
            }

            return count;
        }

        public bool DontRespondPeachInJudge(JudgeStruct judge)
        {
            int peach_num = GetCards(Peach.ClassName, self).Count;
            if (peach_num == 0) return false;

            if (WillSkipPlayPhase(self) && peach_num > GetOverflow(self)) return false;
            if (judge.Reason == Lightning.ClassName && IsFriend(judge.Who)) return false;

            if (peach_num > self.GetLostHp())
            {
                bool weak = false;
                foreach (Player p in FriendNoSelf)
                    if (IsWeak(p)) weak = true;
                if (!weak) return false;
            }

            if (judge.Reason == EightDiagram.ClassName && IsFriend(judge.Who))
            {
                bool effect = true;
                List<CardUseStruct> list = room.GetUseList();
                CardUseStruct use = list[list.Count - 1];
                if (use.To.Contains(judge.Who))
                {
                    DamageStruct damage = new DamageStruct(use.Card, use.From, judge.Who);
                    if (use.Card != null && use.Card.Name == FireSlash.ClassName)
                        damage.Nature = DamageStruct.DamageNature.Fire;
                    else if (use.Card != null && use.Card.Name == ThunderSlash.ClassName)
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
            if (reason == Lightning.ClassName)
            {
                DamageStruct damage = new DamageStruct
                {
                    Damage = 3,
                    Nature = DamageStruct.DamageNature.Thunder,
                    To = who
                };
                if (HasSkill("hongyan|hongyan_jx", who)) return false;
                if (DamageEffect(damage, DamageStruct.DamageStep.Done) == 0)
                    return false;
                else
                {
                    if (judge.IsGood() && (IsEnemy(who) && GetDamageScore(damage).Score < 5 || (!IsFriend(who) && ChainDamage(damage) > 0))) return true;
                    if (judge.IsBad() && IsFriend(who) && (GetDamageScore(damage).Score < 5 || ChainDamage(damage) < 0)) return true;
                }
            }
            else if (reason == Indulgence.ClassName)
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
            else if (reason == SupplyShortage.ClassName)
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
            else if (reason == "beige" || reason == "beige_jx")
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
            bool good = judge.IsGood();

            if ((IsFriend(who) && good) || (IsEnemy(who) && !good)) return -1;

            foreach (int id in card_ids)
            {
                WrappedCard card_x = Engine.CloneCard(Engine.GetRealCard(id));
                if (card_x.HasFlag("using") || RoomLogic.IsCardLimited(room, self, room.GetCard(id), HandlingMethod.MethodResponse)) continue;
                bool is_peach = IsFriend(who) && HasSkill("tiandu", who) || IsCard(id, Peach.ClassName, who, self);
                if (HasSkill("hongyan|hongyan_jx", who) && card_x.Suit == WrappedCard.CardSuit.Spade)
                {
                    card_x.SetSuit(WrappedCard.CardSuit.Heart);
                }

                if (string.IsNullOrEmpty(judge.Pattern))
                {
                    if (reason == "beige" || reason == "beige_jx")
                    {
                        if (!is_peach)
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
                    }
                    else if (reason == "ganglie_jx")
                    {
                        if (!is_peach)
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
                    else if (reason == "qiangwu" && IsFriend(who) && judge.Card.Number >= 10 && who.HandcardNum > 4)
                    {
                        if (!is_peach && card_x.Number < 5)
                            can_use.Add(id);
                    }
                    else
                        break;
                }
                else if (reason == "lueming")
                {
                    Player target = null;
                    foreach (Player p in room.GetOtherPlayers(who))
                    {
                        if (p.HasFlag("lueming"))
                        {
                            target = p;
                            break;
                        }
                    }

                    if (target != null && target.Alive && who.Alive)
                    {
                        DamageStruct damgage = new DamageStruct("lueming", who, target, 2);
                        ScoreStruct score = GetDamageScore(damgage);
                        if (!judge.Good && score.Score > 7 && Engine.GetPattern(judge.Pattern).Match(judge.Who, room, card_x))
                            can_use.Add(id);
                        else if (judge.Good && score.Score < -6 && !Engine.GetPattern(judge.Pattern).Match(judge.Who, room, card_x))
                            can_use.Add(id);
                    }
                }
                else if (reason == "zhuilie")
                {
                    Player target = room.FindPlayer(who.GetTag("zhuilie").ToString());
                    if (target != null && target.Hp > 1 && (!target.HasArmor(SilverLion.ClassName) || who.HasWeapon(QinggangSword.ClassName)))
                    {
                        FunctionCard fcard = Engine.GetFunctionCard(room.GetCard(id).Name);
                        if ((judge.Good && IsFriend(target) && (fcard is Weapon || fcard is Horse))
                            || (!judge.Good && IsEnemy(target) && !(fcard is Weapon || fcard is Horse)))
                        {
                            List<CardUseStruct> uses = room.GetUseList();
                            CardUseStruct use = uses[uses.Count - 1];
                            if (use.Card.Name.Contains(Slash.ClassName) && use.From == who)
                            {
                                DamageStruct damage = new DamageStruct(use.Card, use.From, target, 1 + use.Drank + use.ExDamage);
                                if (use.Card.Name == FireSlash.ClassName)
                                    damage.Nature = DamageStruct.DamageNature.Fire;
                                else if (use.Card.Name == ThunderSlash.ClassName)
                                    damage.Nature = DamageStruct.DamageNature.Thunder;
                                ScoreStruct score1 = GetDamageScore(damage);
                                damage.Damage += target.Hp - 1;
                                ScoreStruct score2 = GetDamageScore(damage);

                                if ((IsFriend(target) && score2.Score < score1.Score) || (IsFriend(who) && score2.Score > score1.Score))
                                    can_use.Add(id);
                            }
                        }
                    }
                }
                else if (IsFriend(who) && judge.Negative != (Engine.GetPattern(judge.Pattern).Match(judge.Who, room, card_x) == judge.Good)
                           && !(self_card && DontRespondPeachInJudge(judge) && is_peach))
                    can_use.Add(id);
                else if (IsEnemy(who) && judge.Negative == (Engine.GetPattern(judge.Pattern).Match(judge.Who, room, card_x) == judge.Good)
                        && !(self_card && DontRespondPeachInJudge(judge) && is_peach))
                    can_use.Add(id);
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

        public virtual void FindSlashandTarget(ref CardUseStruct use, Player player, List<WrappedCard> slashes = null)
        {
            List<ScoreStruct> values = CaculateSlashIncome(player, slashes);
            double best = 0;
            if (values.Count > 0)
                best = values[0].Score;

            //check will use analeptic
            if (!use.IsDummy && player.Phase == PlayerPhase.Play && pre_drink == null && IsSituationClear()
                && room.GetRoomState().GetCurrentCardUseReason() == CardUseStruct.CardUseReason.CARD_USE_REASON_PLAY)
            {
                List<WrappedCard> analeptics = GetCards(Analeptic.ClassName, player);
                bool will_use = false;
                bool drink_hand = false;
                if (analeptics.Count > 0)
                {
                    foreach (int id in analeptics[0].SubCards)
                    {
                        if (room.GetCardPlace(id) == Place.PlaceHand && room.GetCardOwner(id) == player)
                        {
                            drink_hand = true;
                            break;
                        }
                    }
                }
                if (analeptics.Count > 1 || enemies[player].Count == 1 || (GetOverflow(player) > 0 && drink_hand)) will_use = true;
                //room.OutPut(string.Format("{0} 检查酒的数量为 {1} 是否需要使用 {2}", player.SceenName, analeptics.Count, will_use.ToString()));

                FunctionCard fcard = Analeptic.Instance;
                foreach (WrappedCard analeptic in analeptics)
                {
                    if (fcard.IsAvailable(room, player, analeptic) && (will_use
                            || (analeptic.IsVirtualCard() && analeptic.SubCards.Count == 0 && GetUseValue(analeptic, player) > 0)))
                    {
                        pre_drink = analeptic;
                        double drank_value = 0;
                        List<ScoreStruct> drank_values = CaculateSlashIncome(player, slashes);
                        pre_drink = null;
                        if (drank_values.Count > 0)
                            drank_value = drank_values[0].Score;

                        foreach (string skill in GetKnownSkills(player))
                        {
                            SkillEvent skill_e = Engine.GetSkillEvent(skill);
                            if (skill_e != null)
                                drank_value += skill_e.TargetValueAdjust(this, analeptic, player, new List<Player> { player }, player);
                        }

                        //room.OutPut(string.Format("{0} 喝酒后的分数由 {1} 提高至 {2}", player.SceenName, best, drank_value));

                        if (drank_value - best >= 6 || (GetOverflow(player) > 0 && drink_hand && drank_value - best > 2))
                        {
                            use = new CardUseStruct(analeptic, player, new List<Player>());
                            return;
                        }
                    }
                }
            }
            
            if (values.Count > 0 && values[0].Score > 0)
            {
                if (!player.HasShownOneGeneral())
                {
                    if ((values[0].Card != null && !string.IsNullOrEmpty(values[0].Card.ShowSkill) && HasSkill(values[0].Card.ShowSkill, player))
                        || (values[0].Card != null && player.HasUsed(Slash.ClassName) && !player.HasWeapon(CrossBow.ClassName)))
                        if (!WillShowForAttack())
                            return;
                }

                bool will_slash = false;
                if (values[0].Score >= 10)
                {
                    will_slash = true;
                }
                else if (enemies[player].Count == 1 && GetOverflow(player) > 0)
                {
                    foreach (int id in values[0].Card.SubCards)
                    {
                        if (room.GetCardPlace(id) == Place.PlaceHand)
                        {
                            will_slash = true;
                            break;
                        }
                    }
                }

                if (!will_slash && IsSituationClear())
                {
                    foreach (Player p in values[0].Players)
                    {
                        if (GetPrioEnemies().Contains(p))
                        {
                            will_slash = true;
                            break;
                        }
                    }
                }

                if (!will_slash && values[0].Score > 0 && (values[0].Card.SubCards.Count == 0 || player.Phase == PlayerPhase.NotActive))
                    will_slash = true;

                //todo: adjust ai personality
                if (!will_slash && GetOverflow(player) > 0)
                {
                    foreach (int id in values[0].Card.SubCards)
                    {
                        if (room.GetCardPlace(id) == Place.PlaceHand)
                        {
                            will_slash = true;
                            break;
                        }
                    }
                }

                if (will_slash)
                {
                    if (player.ContainsTag("xianzhen") && player.GetTag("xianzhen") is List<string> names)
                    {
                        WrappedCard slash = null;
                        foreach (ScoreStruct score in values)
                        {
                            if (names.Contains(score.Players[0].Name) && score.Score > 0)
                            {
                                slash = score.Card;
                                break;
                            }
                        }

                        if (slash != null)
                        {
                            foreach (ScoreStruct score in values)
                            {
                                if (score.Score > 0 && !names.Contains(score.Players[0].Name) && score.Players.Count == 1 && slash == score.Card)
                                {
                                    use.Card = score.Card;
                                    use.To = score.Players;
                                    return;
                                }
                            }
                        }
                    }

                    if (HasSkill("duanbing") && WillShowForAttack() && values[0].Players.Count == 1 && RoomLogic.DistanceTo(room, player, values[0].Players[0], values[0].Card) == 1
                        && values.Count > 1)
                    {
                        for (int i = 1; i < values.Count; i++)
                        {
                            if (values[i].Score > 0 && values[i].Card == values[0].Card && !values[i].Players.Contains(values[0].Players[0]))
                            {
                                use.From = player;
                                use.To = values[i].Players;
                                use.Card = values[0].Card;
                                return;
                            }
                        }
                    }
                    else
                    {
                        use.From = player;
                        use.To = values[0].Players;
                        use.Card = values[0].Card;
                    }
                }
            }
        }
        public void ClearPreInfo()
        {
            foreach (Player p in room.GetAlivePlayers())
            {
                pre_discard[p].Clear();
                pre_disable[p] = string.Empty;
                pre_noncom_invalid[p] = string.Empty;
            }

            pre_ignore_armor.Clear();
        }

        public virtual List<ScoreStruct> CaculateSlashIncome(Player player, List<WrappedCard> slashes = null, List<Player> targets = null, bool play = true)
        {
            List<ScoreStruct> values = new List<ScoreStruct>();
            if (slashes == null || slashes.Count == 0)
                slashes = GetCards(Slash.ClassName, player);

            List<WrappedCard> cards = new List<WrappedCard>();
            FunctionCard fcard = Slash.Instance;
            foreach (WrappedCard slash in slashes)
            {
                if (!slash.Name.Contains(Slash.ClassName) || (play && !fcard.IsAvailable(room, player, slash))) continue;
                cards.Add(slash);
                if (player.HasWeapon(Fan.ClassName) && slash.Name == Slash.ClassName && !slash.SubCards.Contains(player.Weapon.Key))
                {
                    WrappedCard fire_slash = new WrappedCard(FireSlash.ClassName)
                    {
                        DistanceLimited = slash.DistanceLimited
                    };
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
                //use value的比重应减小
                double value = (GetUseValue(slash, self) - Engine.GetCardUseValue(slash.Name)) / 2;

                int extra = Engine.CorrectCardTarget(room, TargetModSkill.ModType.ExtraMaxTarget, player, slash);
                Dictionary<Player, ScoreStruct> available_targets = new Dictionary<Player, ScoreStruct>();
                foreach (Player target in targets)
                {
                    ClearPreInfo();
                    if (!Slash.Instance.TargetFilter(room, new List<Player>(), target, player, slash)) continue;
                    available_targets.Add(target, SlashIsEffective(slash, target));
                }

                int target_count = Math.Min(available_targets.Keys.Count, extra + 1);
                List<List<Player>> uses = new List<List<Player>>();
                for (int i = 1; i <= target_count; i++)
                {
                    List<List<Player>> players = GetCombinationList(new List<Player>(available_targets.Keys), i);
                    uses.AddRange(players);
                }

                foreach (List<Player> use in uses)
                {
                    List<Player> _targets = use;
                    room.SortByActionOrder(ref _targets);

                    bool chain = false;
                    double use_value = 0;
                    foreach (Player target in _targets)
                    {
                        ScoreStruct score = available_targets[target];
                        if (score.Score == 0) continue;
                        use_value += score.Score;

                        foreach (string skill in GetKnownSkills(target))
                        {
                            SkillEvent ev = Engine.GetSkillEvent(skill);
                            if (ev != null)
                                use_value += ev.TargetValueAdjust(this, slash, player, _targets, target);
                        }

                        if (IsFriend(target) && target.Chained && chain && score.Damage.Damage > 0)
                            use_value -= 10;

                        if (score.DoDamage && !chain)
                        {
                            if (target.Chained && !IsSpreadStarter(score.Damage) && score.Damage.Nature != DamageStruct.DamageNature.Normal)
                                value -= 0.5;
                            else
                            {
                                double chain_value = ChainDamage(score.Damage);
                                if (IsSpreadStarter(score.Damage))
                                {
                                    if (chain_value < 0)
                                    {
                                        use_value += chain_value;
                                        chain = true;
                                    }
                                    else if (score.Info != "miss" && score.Rate > 0.5)
                                    {
                                        use_value += chain_value * score.Rate;
                                        chain = true;
                                    }
                                }
                            }
                        }
                    }
                    if (use_value >= 0)
                    {
                        use_value += value;
                        if (!string.IsNullOrEmpty(slash.Skill))
                        {
                            SkillEvent e = Engine.GetSkillEvent(slash.Skill);
                            if (e != null)
                                use_value += e.UseValueAdjust(this, player, _targets, slash);
                        }
                    }

                    if (_targets.Count == 1 && !IsFriend(_targets[0]) && use_value >= 4)
                    {            //yuji?
                        bool yuji = false;
                        foreach (Player p in room.GetAlivePlayers())
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
                                    if (!_targets.Contains(p) && fcard.ExtratargetFilter(room, targets_const, p, player, slash) && RoomLogic.DistanceTo(room, player, p, slash) == 1)
                                        duanbings.Add(p);

                                foreach (Player p in duanbings)
                                    if (SlashIsEffective(slash, p).Score >= 0)
                                        extra_target = true;
                            }
                            if (!extra_target && player.HasFlag("TianyiSuccess"))
                            {
                                List<Player> tianyis = new List<Player>();
                                foreach (Player p in room.GetOtherPlayers(player))
                                    if (!_targets.Contains(p) && fcard.ExtratargetFilter(room, targets_const, p, player, slash))
                                        tianyis.Add(p);

                                foreach (Player p in tianyis)
                                    if (SlashIsEffective(slash, p).Score >= 0)
                                        extra_target = true;
                            }
                            if (!extra_target && player.HasWeapon(Halberd.ClassName) && !slash.SubCards.Contains(player.Weapon.Key))
                            {
                                List<Player> halberds = new List<Player>();
                                foreach (Player p in room.GetOtherPlayers(player))
                                    if (!_targets.Contains(p) && fcard.ExtratargetFilter(room, targets_const, p, player, slash) && !RoomLogic.IsFriendWith(room, p, _targets[0]))
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
                        Players = _targets,
                        Score = use_value
                    };
                    

                    values.Add(result_score);
                }
            }

            CompareByScore(ref values);
            return values;
        }
        /// <summary>
        /// 获得从n个不同元素中任意选取m个元素的组合的所有组合形式的列表
        /// </summary>
        /// <param name="elements">供组合选择的元素</param>
        /// <param name="m">组合中选取的元素个数</param>
        /// <returns>返回一个包含列表的列表，包含的每一个列表就是每一种组合可能</returns>
        public static List<List<T>> GetCombinationList<T>(List<T> elements, int m)
        {
            List<List<T>> result = new List<List<T>>();//存放返回的列表
            List<List<T>> temp = null; //临时存放从下一级递归调用中返回的结果
            List<T> oneList = null; //存放每次选取的第一个元素构成的列表，当只需选取一个元素时，用来存放剩下的元素分别取其中一个构成的列表；
            T oneElment; //每次选取的元素
            List<T> source = new List<T>(elements); //将传递进来的元素列表拷贝出来进行处理，防止后续步骤修改原始列表，造成递归返回后原始列表被修改；
            int n = 0; //待处理的元素个数

            if (elements != null)
            {
                n = elements.Count;
            }
            if (n == m && m != 1)//n=m时只需将剩下的元素作为一个列表全部输出
            {
                result.Add(source);
                return result;
            }
            if (m == 1)  //只选取一个时，将列表中的元素依次列出
            {
                foreach (T el in source)
                {
                    oneList = new List<T>
                    {
                        el
                    };
                    result.Add(oneList);
                    oneList = null;
                }
                return result;
            }

            for (int i = 0; i <= n - m; i++)
            {
                oneElment = source[0];
                source.RemoveAt(0);
                temp = GetCombinationList(source, m - 1);
                for (int j = 0; j < temp.Count; j++)
                {
                    oneList = new List<T>
                    {
                        oneElment
                    };
                    oneList.AddRange(temp[j]);
                    result.Add(oneList);
                    oneList = null;
                }
            }


            return result;
        }

        public bool SlashProhibit(WrappedCard card, Player enemy, Player from)
        {
            if (enemy.Removed) return true;
            card = card ?? new WrappedCard(Slash.ClassName);
            from = from ?? self;

            foreach (string skill in GetKnownSkills(enemy))
            {           // todo
                SkillEvent e = Engine.GetSkillEvent(skill);
                if (e != null && e.IsProhibit(this, from, enemy, card)) return true;
            }

            return false;
        }
        //判断杀是否可以生效
        public ScoreStruct SlashIsEffective(WrappedCard card, Player to)
        {
            Player from = self;
            //青龙刀预封锁
            if (from.HasWeapon(Blade.ClassName))
            {
                if (!to.General1Showed)
                    pre_disable[to] = pre_disable[to] + "h";
                if (!string.IsNullOrEmpty(to.General2) && !to.General2Showed)
                    pre_disable[to] = pre_disable[to] + "d";
            }

            //判断铁骑需要预封锁的武将
            if (HasSkill("tieqi|tieqi_fz", from) && WillShowForAttack() && to.HasShownOneGeneral())
            {
                double g1 = to.General1Showed ? GetGeneralStength(to, true, false) : 0;
                double g2 = to.General2Showed ? GetGeneralStength(to, false, false) : 0;
                if (to.General1Showed && !pre_noncom_invalid[to].Contains("h") && (g1 > g2 || !to.General2Showed))
                    pre_noncom_invalid[to] = pre_noncom_invalid[to] + "h";
                else if (to.General2Showed && !pre_noncom_invalid[to].Contains("d") && (g2 > g1 || !to.General1Showed))
                    pre_noncom_invalid[to] = pre_noncom_invalid[to] + "d";
            }
            if (HasSkill("tieqi_jx", from) && !pre_noncom_invalid[to].Contains("h"))
                pre_noncom_invalid[to] = pre_noncom_invalid[to] + "h";

            if (IsCancelTarget(card, to, from)) return new ScoreStruct();
            ScoreStruct result_score = new ScoreStruct();
            if (from.HasWeapon(PosionedDagger.ClassName) && WrappedCard.IsBlack(card.Suit))
            {
                if (HasSkill("zhaxiang", to) && to.Hp > 1)
                {
                    if (IsFriend(from, to))
                    {
                        result_score.Score += IsFriend(to) ? 4 : -4;
                    }
                }
                else if (!IsFriend(from, to))
                {
                    double posion = 3;
                    if (to.Hp == 1) posion += 4;
                    if (IsFriend(to)) result_score.Score += posion;
                    if (IsEnemy(to)) result_score.Score -= posion;
                }
            }

            if (from != null && HasSkill("fuyin", to) && to.GetMark("fuyin") == 0)
            {
                int hand = from.HandcardNum;
                foreach (int id in card.SubCards)
                    if (room.GetCardPlace(id) == Place.PlaceHand && room.GetCardOwner(id) == from)
                        hand--;
                if (pre_drink != null)
                {
                    foreach (int id in pre_drink.SubCards)
                        if (room.GetCardPlace(id) == Place.PlaceHand && room.GetCardOwner(id) == from)
                            hand--;
                }

                if (hand > to.HandcardNum)
                {
                    if (IsFriend(from, to))
                        result_score.Score = -1;
                    else
                        result_score.Score = 1;

                    return result_score;
                }
            }

            double result = 0;
            if (from.HasWeapon(QinggangSword.ClassName) || from.HasWeapon(Saber.ClassName))
                pre_ignore_armor.Add(to);
            if (from.HasWeapon(DragonPhoenix.ClassName) && !IsFriend(from, to) && to.GetArmor() && to.GetCards("he").Count == 1)
                pre_discard[to].Add(to.Armor.Key);

            if (from.HasWeapon(DoubleSword.ClassName) && (from.IsMale() && to.IsFemale() || (from.IsFemale() && to.IsMale())))
                result += 3;

            if (from.HasWeapon(DragonPhoenix.ClassName) && !to.IsNude())
            {
                if (!IsFriend(from, to) && GetLostEquipEffect(to) == 0)
                    result += 3;
                else if (IsFriend(from, to))
                    result -= GetLostEquipEffect(to);
            }

            if (!IsCardEffect(card, to, from))
            {                            // card will be nullified
                if (IsEnemy(from, to) && from.HasWeapon(LightningSummoner.ClassName))           //天雷刃保证有0.5的最低分
                {
                    if (IsFriend(to)) result -= 0.5;
                    if (IsEnemy(to)) result += 0.5;
                }
                result_score.Score = result;
                return result_score;
            }

            //evaluate hit rate
            double jink = 0;
            double force_hit = 0;

            if (from.ContainsTag("wenji") && from.GetTag("wenji") is List<string> names && names.Contains(Slash.ClassName))
                force_hit = 1;
            if (HasSkill("fuqi", from) && RoomLogic.DistanceTo(room, from, to) == 1)
                force_hit = 1;
            if (HasSkill("wanglie", from) && from.Phase == PlayerPhase.Play && !IsFriend(from, to)) force_hit = 1;
            if (HasSkill("wushen", from) && RoomLogic.GetCardSuit(room, card) == WrappedCard.CardSuit.Heart) force_hit = 1;

            bool no_red = to.GetMark("@qianxi_red") > 0;
            bool no_black = to.GetMark("@qianxi_black") > 0;
            if (force_hit < 1 && !IsFriend(to, from))
            {
                int jink_need = HasSkill("wushuang", from) ? 2 : 1;
                if (HasSkill("tieqi|tieqi_fz|tieq_jx", from)) force_hit = 7 / 10;
                if (HasSkill("jianchu", from) && to.HasEquip()) force_hit = 1;
                if (HasSkill("liegong_fz|liegong", from) && from.Phase == PlayerPhase.Play)
                {
                    bool weapon = from.GetWeapon() && !card.SubCards.Contains(from.Weapon.Key);
                    if (to.HandcardNum >= from.Hp || to.HandcardNum <= RoomLogic.GetAttackRange(room, from, weapon))
                        force_hit = 1;
                }
                if (HasSkill("liegong_jx", from))
                {
                    int count = from.HandcardNum;
                    foreach (int id in card.SubCards)
                        if (from.GetCards("h").Contains(id)) count--;

                    if (count >= to.HandcardNum) force_hit = 1;
                }
                if (force_hit < 1 && HasArmorEffect(to, EightDiagram.ClassName))
                {
                    if (HasSkill("tiandu+qingguo", to) && !no_black)
                        jink = jink_need;
                    else
                    {
                        Player winner = GetWizzardRaceWinner(EightDiagram.ClassName, to);
                        if (winner == null || !IsFriend(winner))
                            jink = 4 / 10;
                        if (winner != null && IsFriend(winner, to))
                            jink = jink_need;
                    }
                    if (HasSkill("tiandu", to))
                        result -= 3 * jink_need;
                }

                foreach (WrappedCard jink_card in GetCards(Jink.ClassName, to))
                    if (!RoomLogic.IsCardLimited(room, to, jink_card, HandlingMethod.MethodUse)) jink++;

                if (!IsLackCard(to, Jink.ClassName))
                {
                    int rate = 5;
                    if (HasSkill("longdan|longdan_fz|qingguo|longdan_jx", to)) rate = 2;
                    if (GetKnownCards(to).Count != to.HandcardNum)
                    {
                        rate = 6;
                        if (HasSkill("longdan|longdan_fz|longdan_jx", to))
                        {
                            rate -= 3;
                            if (no_black)
                                rate += 2;
                        }
                        if (HasSkill("qingguo", to) && !no_black)
                            rate -= 2;

                        int count = to.HandcardNum - GetKnownCards(to).Count;
                        if (RoomLogic.IsHandCardLimited(room, to, HandlingMethod.MethodUse)) count = 0;

                        count += to.GetHandPile(true).Count - GetKnownHandPileCards(to).Count;
                        if (no_red)
                        {
                            if (rate == 6)
                                count = 0;
                            else
                                rate += 5;
                        }
                        jink += ((double)count / rate);
                    }
                }

                if (force_hit < 1 && jink > 0.3 && NotSlashJiaozhu(self, to, card))
                {
                    result_score.Score = -20;
                    result_score.Info = "no";
                    return result_score;
                }
                //躲闪率
                double dodge = (jink - jink_need >= 0) ? 1 : (jink - jink_need <= -1) ? 0 : (1 + jink - jink_need);
                if (force_hit < 1 && from.HasWeapon(Axe.ClassName) && !card.SubCards.Contains(from.Weapon.Key))
                {
                    List<int> ids = new List<int>(card.SubCards);
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

                //room.OutPut(string.Format("{0}: {1} dodge rate {2}", from.SceenName, to.SceenName, dodge));

                jink = dodge;
            }
            else if (force_hit < 1 && JiaozhuneedSlash(from, to, card))
            {
                int jink_need = RoomLogic.PlayerHasShownSkill(room, from, "wushuang") ? 2 : 1;
                if (HasArmorEffect(to, EightDiagram.ClassName))
                {
                    if (HasSkill("tiandu+qingguo", to))
                        jink = jink_need;
                    else
                    {
                        Player winner = GetWizzardRaceWinner(EightDiagram.ClassName, to);
                        if (winner == null || !IsEnemy(winner))
                            jink = 4 / 10;
                        if (winner != null && IsFriend(winner, to))
                            jink = jink_need;
                    }
                    if (HasSkill("tiandu", to))
                        result += 3 * (jink_need > 1 ? 1.5 : 1);
                }

                List<WrappedCard> jinks = GetCards(Jink.ClassName, to);
                foreach (WrappedCard jink_card in jinks)
                    if (!RoomLogic.IsCardLimited(room, to, jink_card, HandlingMethod.MethodUse)) jink = jink + 1;
                if (!IsLackCard(to, Jink.ClassName))
                {
                    int rate = 5;
                    if (HasSkill("longdan|qingguo|longdan_fz|longdan_jx", to)) rate = 2;
                    jink += (to.GetHandPile(true).Count - GetKnownHandPileCards(to).Count) / rate;
                    if (GetKnownCards(to).Count != to.HandcardNum)
                    {
                        rate = 6;
                        if (HasSkill("longdan|longdan_fz|longdan_jx", to))
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
            int damage_count = 1;
            if (!IsFriend(to) && HasSkill("liegong_jx", from) && from.Hp <= to.Hp) damage_count++;
            DamageStruct damage = new DamageStruct(card, from, to, damage_count)
            {
                Nature = card.Name == Slash.ClassName ? DamageStruct.DamageNature.Normal
                        : card.Name == FireSlash.ClassName ? DamageStruct.DamageNature.Fire : DamageStruct.DamageNature.Thunder
            };                 //evaluate damage

            ScoreStruct score = GetDamageScore(damage);
            result_score.Damage = score.Damage;
            if (IsFriend(to, from))
            {
                result_score.Score = result + score.Score;
                result_score.Rate = 1;
                
                return result_score;
            }
            else
            {
                //命中率
                double rate = force_hit + (1 - force_hit) * (1 - jink);
                result_score.Score = result + score.Score * rate;
                result_score.Rate = rate;
                
                return result_score;
            }
        }

        public virtual ScoreStruct SlashIsEffective(WrappedCard card, Player from, Player to)
        {
            //青龙刀预封锁
            if (from.HasWeapon(Blade.ClassName))
            {
                if (!to.General1Showed)
                    pre_disable[to] = pre_disable[to] + "h";
                if (!string.IsNullOrEmpty(to.General2) && !to.General2Showed)
                    pre_disable[to] = pre_disable[to] + "d";
            }

            //判断铁骑需要预封锁的武将
            if (HasSkill("tieqi|tieqi_fz", from) && WillShowForAttack() && to.HasShownOneGeneral())
            {
                double g1 = to.General1Showed ? GetGeneralStength(to, true, false) : 0;
                double g2 = to.General2Showed ? GetGeneralStength(to, false, false) : 0;
                if (to.General1Showed && !pre_noncom_invalid[to].Contains("h") && (g1 > g2 || !to.General2Showed))
                    pre_noncom_invalid[to] = pre_noncom_invalid[to] + "h";
                else if (to.General2Showed && !pre_noncom_invalid[to].Contains("d") && (g2 > g1 || !to.General1Showed))
                    pre_noncom_invalid[to] = pre_noncom_invalid[to] + "d";
            }
            if (HasSkill("tieqi_jx", from) && !pre_noncom_invalid[to].Contains("h"))
                pre_noncom_invalid[to] = pre_noncom_invalid[to] + "h";

            if (IsCancelTarget(card, to, from)) return new ScoreStruct();

            ScoreStruct result_score = new ScoreStruct();
            if (from.HasWeapon(PosionedDagger.ClassName) && WrappedCard.IsBlack(card.Suit))
            {
                if (HasSkill("zhaxiang", to) && to.Hp > 1)
                {
                    if (IsFriend(from, to))
                    {
                        result_score.Score += IsFriend(to) ? 4 : -4;
                    }
                }
                else if(!IsFriend(from, to))
                {
                    double posion = 3;
                    if (to.Hp == 1) posion += 4;
                    if (IsFriend(to)) result_score.Score += posion;
                    if (IsEnemy(to)) result_score.Score -= posion;
                }
            }

            if (from != null && HasSkill("fuyin", to) && to.GetMark("fuyin") == 0)
            {
                int hand = from.HandcardNum;
                foreach (int id in card.SubCards)
                    if (room.GetCardPlace(id) == Place.PlaceHand && room.GetCardOwner(id) == from)
                        hand--;

                if (hand > to.HandcardNum)
                {
                    if (IsFriend(to))
                        result_score.Score = -1;
                    else if (IsEnemy(to))
                        result_score.Score = 1;

                    return result_score;
                }
            }

            double result = 0;
            if (from.HasWeapon(QinggangSword.ClassName) || from.HasWeapon(Saber.ClassName))
                pre_ignore_armor.Add(to);
            if (from.HasWeapon(DragonPhoenix.ClassName) && !IsFriend(from, to) && to.GetArmor() && to.GetCards("he").Count == 1)
                pre_discard[to].Add(to.Armor.Key);

            if (from.HasWeapon(DoubleSword.ClassName) && (from.IsMale() && to.IsFemale() || (from.IsFemale() && to.IsMale())))
                result += 3;

            if (from.HasWeapon(DragonPhoenix.ClassName) && !to.IsNude())
            {
                if (!IsFriend(from, to) && GetLostEquipEffect(to) == 0)
                    result += 3;
                else if (IsFriend(from, to))
                    result -= GetLostEquipEffect(to);
            }

            if (!IsCardEffect(card, to, from))
            {                            // card will be nullified
                if (IsEnemy(from, to) && from.HasWeapon(LightningSummoner.ClassName))           //天雷刃保证有0.5的最低分
                {
                    if (IsFriend(to)) result -= 0.5;
                    if (IsEnemy(to)) result += 0.5;
                }
                result_score.Score = result;
                return result_score;
            }

            //evaluate hit rate
            double jink = 0;
            double force_hit = 0;

            if (from.ContainsTag("wenji") && from.GetTag("wenji") is List<string> names && names.Contains(Slash.ClassName))
                force_hit = 1;
            if (HasSkill("fuqi", from) && RoomLogic.DistanceTo(room, from, to) == 1)
                force_hit = 1;

            bool no_red = to.GetMark("@qianxi_red") > 0;
            bool no_black = to.GetMark("@qianxi_black") > 0;
            if (force_hit < 1 && !IsFriend(to, from))
            {
                int jink_need = HasSkill("wushuang", from) ? 2 : 1;
                if (HasSkill("tieqi|tieqi_fz|tieqi_jx", from)) force_hit = 7 / 10;
                if (HasSkill("jianchu", from) && to.HasEquip()) force_hit = 1;
                if (HasSkill("liegong_fz|liegong", from) && from.Phase == PlayerPhase.Play)
                {
                    bool weapon = from.GetWeapon() && !card.SubCards.Contains(from.Weapon.Key);
                    if (to.HandcardNum >= from.Hp || to.HandcardNum <= RoomLogic.GetAttackRange(room, from, weapon))
                        force_hit = 1;
                }

                if (HasSkill("liegong_jx", from) && from.HandcardNum >= to.HandcardNum)
                    force_hit = 1;

                if (force_hit < 1 && HasArmorEffect(to, EightDiagram.ClassName))
                {
                    if (HasSkill("tiandu+qingguo", to) && !no_black)
                        jink = jink_need;
                    else
                    {
                        Player winner = GetWizzardRaceWinner(EightDiagram.ClassName, to);
                        if (winner == null || !IsFriend(winner))
                            jink = 4 / 10;
                        if (winner != null && IsFriend(winner, to))
                            jink = jink_need;
                    }
                    if (HasSkill("tiandu", to))
                        result -= 3 * jink_need;
                }

                foreach (WrappedCard jink_card in GetCards(Jink.ClassName, to))
                    if (!RoomLogic.IsCardLimited(room, to, jink_card, HandlingMethod.MethodUse)) jink++;

                if (!IsLackCard(to, Jink.ClassName))
                {
                    int rate = 5;
                    if (HasSkill("longdan|qingguo|longdan_fz|longdan_jx", to)) rate = 2;
                    if (GetKnownCards(to).Count != to.HandcardNum)
                    {
                        rate = 6;
                        if (HasSkill("longdan|longdan_fz|longdan_jx", to))
                        {
                            rate -= 3;
                            if (no_black)
                                rate += 2;
                        }
                        if (HasSkill("qingguo", to) && !no_black)
                            rate -= 2;

                        int count = to.HandcardNum - GetKnownCards(to).Count;
                        if (RoomLogic.IsHandCardLimited(room, to, HandlingMethod.MethodUse)) count = 0;

                        count += to.GetHandPile(true).Count - GetKnownHandPileCards(to).Count;
                        if (no_red)
                        {
                            if (rate == 6)
                                count = 0;
                            else
                                rate += 5;
                        }
                        jink += ((double)count / rate);
                    }
                }

                if (force_hit < 1 && jink > 0.3 && NotSlashJiaozhu(self, to, card))
                {
                    result_score.Score = -20;
                    result_score.Info = "no";
                    return result_score;
                }
                //躲闪率
                double dodge = (jink - jink_need >= 0) ? 1 : (jink - jink_need <= -1) ? 0 : (1 + jink - jink_need);
                if (force_hit < 1 && from.HasWeapon(Axe.ClassName) && !card.SubCards.Contains(from.Weapon.Key))
                {
                    List<int> ids = new List<int>(card.SubCards);
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
            else if (force_hit < 1 && JiaozhuneedSlash(from, to, card))
            {
                int jink_need = RoomLogic.PlayerHasShownSkill(room, from, "wushuang") ? 2 : 1;
                if (HasArmorEffect(to, EightDiagram.ClassName))
                {
                    if (HasSkill("tiandu+qingguo", to))
                        jink = jink_need;
                    else
                    {
                        Player winner = GetWizzardRaceWinner(EightDiagram.ClassName, to);
                        if (winner == null || !IsEnemy(winner))
                            jink = 4 / 10;
                        if (winner != null && IsFriend(winner, to))
                            jink = jink_need;
                    }
                    if (HasSkill("tiandu", to))
                        result += 3 * (jink_need > 1 ? 1.5 : 1);
                }

                List<WrappedCard> jinks = GetCards(Jink.ClassName, to);
                foreach (WrappedCard jink_card in jinks)
                    if (!RoomLogic.IsCardLimited(room, to, jink_card, HandlingMethod.MethodUse)) jink = jink + 1;
                if (!IsLackCard(to, Jink.ClassName))
                {
                    int rate = 5;
                    if (HasSkill("longdan|qingguo|longdan_fz|longdan_jx", to)) rate = 2;
                    jink += (to.GetHandPile(true).Count - GetKnownHandPileCards(to).Count) / rate;
                    if (GetKnownCards(to).Count != to.HandcardNum)
                    {
                        rate = 6;
                        if (HasSkill("longdan|longdan_fz|longdan_jx", to))
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
            int damage_count = 1;
            if (!IsFriend(from, to) && HasSkill("liegong_jx", from) && from.Hp <= to.Hp) damage_count++;
            DamageStruct damage = new DamageStruct(card, from, to, damage_count)
            {
                Nature = card.Name == Slash.ClassName ? DamageStruct.DamageNature.Normal
                        : card.Name == FireSlash.ClassName ? DamageStruct.DamageNature.Fire : DamageStruct.DamageNature.Thunder
            };                 //evaluate damage

            ScoreStruct score = GetDamageScore(damage);
            result_score.Damage = score.Damage;
            if (IsFriend(to, from))
            {
                result_score.Score = result + score.Score;
                result_score.Rate = 1;

                //room.OutPut(string.Format("{0}:friend {1} result {2}",from.SceenName, to.SceenName, result_score.Score));

                return result_score;
            }
            else
            {
                //命中率
                double rate = force_hit + (1 - force_hit) * (1 - jink);
                result_score.Score = result + score.Score * rate;
                result_score.Rate = rate;

                //room.OutPut(string.Format("{0}:enemy {1}, hit rate{3} result {2}", from.SceenName, to.SceenName, result_score.Score, rate));

                return result_score;
            }
        }

        public bool IsCancelTarget(WrappedCard card, Player to, Player from)
        {
            foreach (SkillEvent e in skill_events.Values)
                if (e.IsCancelTarget(this, card, from, to)) return true;

            if (HasArmorEffect(to, IronArmor.ClassName) && (card.Name == FireSlash.ClassName || card.Name == FireAttack.ClassName || card.Name == BurningCamps.ClassName))
                return true;

            return false;
        }

        public bool IsCardEffect(WrappedCard card, Player to, Player from)
        {
            foreach (SkillEvent e in skill_events.Values)
                if (!e.IsCardEffect(this, card, from, to)) return false;

            UseCard ev = Engine.GetCardUsage(card.Name);
            if (ev != null && !ev.IsCardEffect(this, card, from, to)) return false;

            bool armor_ignore = false;
            if (from != null && to.ArmorIsNullifiedBy(from)) armor_ignore = true;

            if (!armor_ignore && from != null && card.Name.Contains(Slash.ClassName))
            {
                if (from.HasWeapon(QinggangSword.ClassName) || from.HasWeapon(Saber.ClassName))
                    armor_ignore = true;
                else if (HasSkill("paoxiao|paoxiao_fz", from))
                {
                    Player lord = FindPlayerBySkill("shouyue");
                    if (lord != null && RoomLogic.PlayerHasShownSkill(room, lord, "shouyue") && RoomLogic.WillBeFriendWith(room, from, lord))
                        armor_ignore = true;
                }
                else if (HasSkill("anjian", from) && !RoomLogic.InMyAttackRange(room, to, from))
                    armor_ignore = true;
            }

            if (!armor_ignore && from != null && HasSkill("benxi", from))
            {
                if (from.HasFlag("benxi_choose"))
                {
                    string card_str = RoomLogic.CardToString(room, card);
                    string str = string.Format("{0}_{1}", "benxi", card_str);
                    if (from.ContainsTag(str) && from.GetTag(str) is List<int> cards && cards.Contains(1))
                        armor_ignore = true;
                }
                else
                {
                    if (!IsFriend(from, to))
                    {
                        bool check = true;
                        foreach (Player p in room.GetOtherPlayers(from))
                        {
                            int distance = RoomLogic.DistanceTo(room, from, p, card, true);
                            if (distance != 1)
                            {
                                check = false;
                                break;
                            }
                        }

                        if (check)
                            armor_ignore = true;
                    }
                }
            }

            if (!armor_ignore && card.Name.Contains(Slash.ClassName) && !IsFriend(from, to) && HasSkill("moukui|pojun|jianchu", from) && to.GetArmor())
            {
                if (HasSkill("pojun", from) || RoomLogic.CanDiscard(room, from, to, to.Armor.Key))
                {
                    List<CardUseStruct> use_list = room.GetUseList();
                    if (self.HasFlag("target_confirming") || use_list.Count == 0 || use_list[use_list.Count - 1].Card != card || use_list[use_list.Count - 1].From != from)
                        armor_ignore = true;
                }
            }

            if (!armor_ignore && card.Name.Contains(Slash.ClassName) && !IsFriend(from, to) && HasSkill("juzhan", from)
                && from.GetMark("juzhan") > 0 && to.GetArmor() && RoomLogic.CanGetCard(room, from, to, to.Armor.Key))
            {
                List<CardUseStruct> use_list = room.GetUseList();
                if (self.HasFlag("target_confirming") || use_list.Count == 0 || use_list[use_list.Count - 1].Card != card || use_list[use_list.Count - 1].From != from)
                    armor_ignore = true;
            }

            if (!armor_ignore && card.Name.Contains(Slash.ClassName) && HasArmorEffect(to, RenwangShield.ClassName) && WrappedCard.IsBlack(card.Suit))
            {
                return false;
            }
            if (!armor_ignore && HasArmorEffect(to, Vine.ClassName) && (card.Name == SavageAssault.ClassName || card.Name == ArcheryAttack.ClassName || card.Name == Slash.ClassName))
            {
                return false;
            }

            return true;
        }

        public virtual ScoreStruct GetDamageScore(DamageStruct _damage, DamageStruct.DamageStep step = DamageStruct.DamageStep.Caused)
        {
            DamageStruct damage = new DamageStruct(_damage.Card, _damage.From, _damage.To, _damage.Damage, _damage.Nature)
            {
                Reason = _damage.Reason,
                Steped = _damage.Steped,
                Transfer = _damage.Transfer,
                TransferReason = _damage.TransferReason,
                Chain = _damage.Chain
            };

            damage.Damage = DamageEffect(damage, step);
            Player from = damage.From;
            Player to = damage.To;
            if (damage.Steped < DamageStruct.DamageStep.Caused)
                damage.Steped = DamageStruct.DamageStep.Caused;

            ScoreStruct result_score = new ScoreStruct
            {
                Damage = damage,
                DoDamage = true
            };

            if (damage.Damage > 0 && NotHurtXiaoqiao(damage))
            {
                result_score.Score = -20;
                return result_score;
            }
            damage.Damage = DamageEffect(damage, DamageStruct.DamageStep.Done);
            damage.Steped = DamageStruct.DamageStep.Done;
            result_score.Damage = damage;

            List<ScoreStruct> scores = new List<ScoreStruct>();
            bool deadly = false;
            if (damage.Damage > 0 && !to.Removed)
            {
                double value = 0;
                value = Math.Min(damage.Damage, to.Hp) * 3.5;
                if (room.BloodBattle && room.Setting.GameMode == "Hegemony") value *= 1.35;                    //鏖战状态下应给予伤害加分
                if (IsWeak(to))
                {
                    value += 4;
                    if (damage.Damage > to.Hp)
                        if (!CanSave(to, damage.Damage - to.Hp + 1))
                        {
                            int over_damage = damage.Damage - to.Hp;
                            for (int i = 1; i <= over_damage; i++)
                            {
                                double x = HasSkill("buqu", to) ? 1 / Math.Pow(i, 2) : (double)8 / Math.Pow(i, 2);
                                value += x;
                            }
                        }
                        else
                            deadly = true;
                }

                if (!to.Removed && CanResist(to, damage.Damage)) result_score.Score = 3;

                bool from_shown = false;
                if (damage.From != null)
                {
                    from_shown = damage.From.HasShownOneGeneral();
                    if (!from_shown)
                    {
                        if ((!string.IsNullOrEmpty(damage.Reason) && HasSkill(damage.Reason, damage.From)) ||
                            (damage.Card != null && !string.IsNullOrEmpty(damage.Card.ShowSkill) && HasSkill(damage.Card.ShowSkill, damage.From))
                            || (damage.Card != null && damage.Card.Name.Contains(Slash.ClassName) && damage.From.HasUsed(Slash.ClassName) && !damage.From.HasWeapon(CrossBow.ClassName)))
                            from_shown = true;
                    }
                }

                if (IsFriend(to))
                {
                    value = -value;
                    if (deadly && damage.From != null && damage.From.Alive)
                    {
                        if (RoomLogic.IsFriendWith(room, damage.From, damage.To) && !damage.From.IsNude())
                            value -= 3;
                        else if (from_shown && !RoomLogic.WillBeFriendWith(room, damage.From, damage.To))
                            value -= 2;
                    }
                }
                else
                {
                    if (deadly && damage.From != null && damage.From.Alive)
                    {
                        if (RoomLogic.IsFriendWith(room, damage.From, damage.To) && !damage.From.IsNude())
                            value += 2.5;
                        else if (from_shown && IsFriend(damage.From) && !RoomLogic.WillBeFriendWith(room, damage.From, damage.To))
                            value += 4;
                    }
                }

                if (deadly)
                {
                    Player caopi = FindPlayerBySkill("xingshang");
                    if (caopi != null && caopi != to)
                        value += IsFriend(caopi) ? 0.5 * to.GetCardCount(true) : -0.5 * to.GetCardCount(true);
                }

                foreach (SkillEvent e in skill_events.Values)
                {
                    if (e.Name != damage.Reason)
                        value += e.GetDamageScore(this, damage).Score;
                }
                if (priority_enemies.Contains(to) && value > 0)
                    value *= 1.5;
                else if (!IsSituationClear() && value > 0)
                    value /= 2;

                result_score.Score = value;
            }
            scores.Add(result_score);

            if (from != null && HasSkill("zhiman|zhiman_jx", from) && from != to && RoomLogic.GetPlayerCards(room, to, "ej").Count > 0)
            {
                ScoreStruct score = FindCards2Discard(from, to, string.Empty, "ej", HandlingMethod.MethodGet);
                scores.Add(score);
            }
            if (damage.Card != null && from != null && !damage.Transfer)
            {
                if ((damage.Card.Name.Contains(Slash.ClassName) || damage.Card.Name == Duel.ClassName) && HasSkill("chuanxin", from) && !RoomLogic.IsFriendWith(room, from, to) && to.HasShownOneGeneral()
                        && !string.IsNullOrEmpty(to.General2) && !to.General2.Contains("sujiang") && !to.IsDuanchang(false))
                {
                    double value = ((to.HasEquip() && !IsWeak(to)) ? (to.GetEquips().Count * 3 + 4) : 7);
                    ScoreStruct score = new ScoreStruct
                    {
                        Score = value
                    };
                    scores.Add(score);
                }
                if (damage.Card.Name.Contains(Slash.ClassName) && from.HasWeapon(IceSword.ClassName) && !to.IsNude())
                {
                    ScoreStruct score = FindCards2Discard(from, to, string.Empty, "he", HandlingMethod.MethodDiscard, 2, true);
                    scores.Add(score);
                }
            }

            CompareByScore(ref scores);
            return scores[0];
        }

        public virtual bool IsSpreadStarter(DamageStruct damage, bool for_self = true)
        {
            if (damage.To == null || !damage.To.Alive || !damage.To.Chained || damage.Chain || damage.Damage <= 0 || damage.Nature == DamageStruct.DamageNature.Normal
                || HasSkill("gangzhi|lixun", damage.To) || (damage.From != null && HasSkill("jueqing|gangzhi_classic", damage.From)))
                return false;
            if (damage.Card != null && HasSkill("renshi", damage.To) && damage.Card.Name.Contains(Slash.ClassName)) return false;

            List<Player> players = GetSpreadTargets(damage);
            return players.Count > 0;
        }

        public virtual double ChainDamage(DamageStruct damage)
        {
            if (damage.To == null || !damage.To.Alive || !damage.To.Chained || damage.Chain || damage.Damage <= 0 || damage.Nature == DamageStruct.DamageNature.Normal
                || HasSkill("gangzhi", damage.To) || (damage.From != null && HasSkill("jueqing|gangzhi_classic", damage.From)))
                return 0;

            List<Player> players = GetSpreadTargets(damage);
            if (players.Count == 0) return 0;
            double good = 0;
            foreach (Player p in players)
            {
                DamageStruct spread = new DamageStruct(damage.Card, damage.From, damage.To, damage.Damage, damage.Nature);
                damage.Reason = damage.Reason;
                damage.Steped = damage.Steped;
                spread.Chain = true;
                spread.To = p;
                int cause = DamageEffect(spread, DamageStruct.DamageStep.Caused);
                bool caculat_break = false;
                if (cause > 0)
                {
                    if (IsFriend(p))
                    {
                        if (spread.From != null && HasSkill("zhiman|zhiman_jx", spread.From) && spread.From != spread.To && IsFriend(spread.From, spread.To))
                        {
                            if (spread.To.JudgingArea.Count > 0 || GetLostEquipEffect(spread.To) < 0)
                            {
                                caculat_break = true;
                                good += 4;
                            }
                        }
                    }
                    else if (IsEnemy(p))
                    {
                        if (spread.From != null && HasSkill("zhiman|zhiman_jx", spread.From) && spread.From != spread.To && IsFriend(spread.From, spread.To))
                        {
                            if (spread.To.JudgingArea.Count > 0 || GetLostEquipEffect(spread.To) < 0)
                            {
                                good -= 4;
                                caculat_break = true;
                            }
                        }
                    }
                }

                if (caculat_break) continue;

                int done = DamageEffect(spread, DamageStruct.DamageStep.Done);
                if (done > 0)
                {
                    ScoreStruct score = GetDamageScore(spread);
                    good += score.Score;
                }
            }

            return good;
        }

        public List<Player> GetSpreadTargets(DamageStruct damage)
        {
            List<Player> players = new List<Player>();
            if (damage.To == null || !damage.To.Alive || !damage.To.Chained || damage.Chain || damage.Damage <= 0 || damage.Nature == DamageStruct.DamageNature.Normal)
                return players;

            foreach (Player p in room.GetOtherPlayers(damage.To))
                if (p.Chained)
                    players.Add(p);

            return players;
        }
        /*
        public bool CanDodamage(DamageStruct damage, double adjust_vlaue)
        {
            damage.Damage = DamageEffect(damage);
            if (damage.Damage == 0) return false;

            double adjust = 0;
            if (IsGoodSpreadStarter(damage))
                adjust = 6;
            else if (IsGoodSpreadStarter(damage, false))
                adjust = -6;
            if (IsFriend(damage.To) && adjust + adjust_vlaue + GetDamageScore(damage).Score < 4) return false;
            if (IsEnemy(damage.To) && adjust + adjust_vlaue + GetDamageScore(damage).Score < -4) return false;

            return true;
        }
        */
        public bool CanRetrial(Player retrialer, string judge_reasion, Player judge_who)
        {
            foreach (string skill in GetKnownSkills(retrialer))
            {
                SkillEvent e = Engine.GetSkillEvent(skill);
                if (e != null && e.CanRetrial(this, judge_reasion, retrialer, judge_who))
                    return true;
            }

            return false;
        }

        public bool RetrialCardMatch(Player retrialer, Player judge_who, string reason, int id)
        {
            WrappedCard card = room.GetCard(id);
            if (card.HasFlag("using")) return false;
            WrappedCard.CardSuit suit = (RoomLogic.PlayerHasShownSkill(room, judge_who, "hongyan|hongyan_jx") && card.Suit == WrappedCard.CardSuit.Spade ? WrappedCard.CardSuit.Heart : card.Suit);
            if (IsFriend(retrialer, judge_who))
            {
                if ((reason == Indulgence.ClassName && suit == WrappedCard.CardSuit.Heart) || (reason == SupplyShortage.ClassName && suit == WrappedCard.CardSuit.Club)
                        || (reason == Lightning.ClassName && (suit != WrappedCard.CardSuit.Spade || card.Number > 9 || card.Number < 2)))
                    return true;
            }
            else if (IsEnemy(retrialer, judge_who))
            {
                if ((reason == Indulgence.ClassName && suit != WrappedCard.CardSuit.Heart) || (reason == SupplyShortage.ClassName && suit != WrappedCard.CardSuit.Club)
                        || (reason == Lightning.ClassName && suit == WrappedCard.CardSuit.Spade && card.Number <= 9 && card.Number >= 2))
                    return true;
            }

            foreach (SkillEvent e in skill_events.Values)
                if (e.RetrialCardMatch(this, retrialer, judge_who, reason, id))
                    return true;

            return false;
        }

        public Player GetWizzardRaceWinner(string reason, Player judge_who, Player starter = null)
        {
            Player winner = null;
            Player zhangqiying = FindPlayerBySkill("zhenyi");
            if (zhangqiying != null && CanRetrial(zhangqiying, reason, judge_who))
                return zhangqiying;

            List<Player> players = new List<Player>();
            if (starter != null)
            {
                players.Add(starter);
                for (int i = 1; i < room.AliveCount(); i++)
                    players.Add(room.GetNextAlive(starter));
            }
            else
                players = room.GetAlivePlayers();

            foreach (Player p in players)
            {
                if (CanRetrial(p, reason, judge_who))
                    winner = p;
            }

            return winner;
        }

        public KeyValuePair<Player, int> GetCardNeedPlayer(List<int> ids = null, List<Player> players = null, Place dest_place = Place.PlaceHand, string reason = null)
        {
            ids = ids??GetKnownCards(self);
            if (ids.Count == 0) return new KeyValuePair<Player, int>();

            List<Player> weaks = new List<Player>(), acts = new List<Player>();
            Player target = null;
            players = players ?? FriendNoSelf;

            bool chain = false;     //如果是遗计需要判断对方是否处于铁索状态，免得白给
            bool chained = false;
            DamageStruct chain_damage = new DamageStruct();
            if (!string.IsNullOrEmpty(reason) && (reason == "yiji" || reason == "yiji_jx") && room.GetTag("CurrentDamageStruct") is DamageStruct damage && Self.Chained
                && damage.Nature != DamageStruct.DamageNature.Normal)
            {
                chain = true;
                chained = damage.Chain;
                chain_damage = damage;
            }

            List<Player> all = room.GetAlivePlayers();
            room.SortByActionOrder(ref all);

            List<Player> skip = new List<Player>();
            if (chain)
            {
                foreach (Player p in players)
                {
                    if (p.Chained && (!chained || all.IndexOf(Self) < all.IndexOf(p)))
                    {
                        DamageStruct _damage = chain_damage;
                        _damage.To = p;
                        _damage.Steped = DamageStruct.DamageStep.Caused;
                        _damage.Chain = true;
                        _damage.Damage = DamageEffect(_damage, DamageStruct.DamageStep.Caused);
                        if (_damage.Damage >= p.Hp)
                        {
                            if (HasSkill("tianxiang|tianxiang_jx", p))
                            {
                                foreach (int id in ids)
                                {
                                    WrappedCard card = room.GetCard(id);
                                    if (card.Suit == WrappedCard.CardSuit.Heart || (card.Suit == WrappedCard.CardSuit.Spade
                                        && RoomLogic.PlayerHasShownSkill(room, p, "hongyan|hongyan_jx")))
                                        return new KeyValuePair<Player, int>(p, id);
                                }
                            }

                            if (_damage.Damage == p.Hp)
                            {
                                foreach (int id in ids)
                                {
                                    if (IsCard(id, Analeptic.ClassName, p))
                                        return new KeyValuePair<Player, int>(p, id);
                                }

                                if (room.Current.Alive && RoomLogic.PlayerHasShownSkill(room, room.Current, "wansha"))
                                {
                                    foreach (int id in ids)
                                    {
                                        if (IsCard(id, Peach.ClassName, p))
                                            return new KeyValuePair<Player, int>(p, id);
                                    }
                                }

                                skip.Add(p);
                            }
                        }
                    }
                }
            }

            //优先判断当前或下一轮行动的玩家
            Player current = null;
            foreach (Player p in all)
            {
                if (dest_place == Place.PlaceHand && HasSkill("zishu", p) && p.Phase == PlayerPhase.NotActive || skip.Contains(p)) continue; 
                if (players.Contains(p) && !WillSkipPlayPhase(p))
                {
                    if (HasSkill("xiantu", p) && p.ContainsTag("xiantu"))
                    {
                        foreach (int id in ids)
                        {
                            if (IsCard(id, Analeptic.ClassName, p) || IsCard(id, Peach.ClassName, p))
                                return new KeyValuePair<Player, int>(p, id);
                        }

                        if (p.Hp == 1)
                            continue;
                    }

                    current = p;
                    break;
                }
                else
                {
                    if (!IsFriend(p, Self) || WillSkipPlayPhase(p))
                        continue;

                    break;
                }
            }

            double point = 0;
            int result = -1;
            if (current != null && dest_place == Place.PlaceHand)
            {
                if (current.Phase == PlayerPhase.Play)
                {
                    foreach (int id in ids)
                    {
                        if (IsCard(id, ExNihilo.ClassName, current) || IsCard(id, BefriendAttacking.ClassName, current))
                        {
                            result = id;
                            break;
                        }
                    }

                    if (result == -1)
                    {
                        foreach (int id in ids)
                        {
                            if (IsCard(id, Dismantlement.ClassName, current))
                            {
                                result = id;
                                break;
                            }
                        }
                    }

                    if (result == -1)
                    {
                        foreach (int id in ids)
                        {
                            WrappedCard snatch = room.GetCard(id);
                            if (snatch.Name == Snatch.ClassName)
                            {
                                foreach (Player p in room.GetOtherPlayers(current))
                                {
                                    if ((RoomLogic.DistanceTo(room, current, p) == 1 || HasSkill("qicai|qicai_jx", current)) && RoomLogic.CanGetCard(room, current, p, "he")
                                        && (IsEnemy(p) || p.JudgingArea.Count > 0) && RoomLogic.IsProhibited(room, current, p, snatch) == null && IsCardEffect(snatch, p, current)
                                        && !IsCancelTarget(snatch, p, current))
                                    {
                                        result = id;
                                        break;
                                    }
                                }
                                if (result != -1) break;
                            }
                        }
                    }

                    if (result == -1)
                    {
                        foreach (int id in ids)
                        {
                            if (IsCard(id, IronChain.ClassName, current))
                            {
                                result = id;
                                break;
                            }
                        }
                    }
                }

                if (result == -1)
                {
                    foreach (int id in ids)
                    {
                        double value = GetUseValue(id, current, dest_place);
                        if (HasSkill(ActiveCardneedSkill, current))
                            value += 1;
                        if (value >= 6 && value > point)
                        {
                            point = value;
                            result = id;
                        }
                    }
                }
                if (result > -1)
                {
                    Debug.Assert(ids.Contains(result));
                    return new KeyValuePair<Player, int>(current, result);
                }
            }

            foreach (Player p in players)
            {
                if (dest_place == Place.PlaceHand && HasSkill("zishu", p) && p.Phase == PlayerPhase.NotActive || skip.Contains(p)) continue;
                if (IsWeak(p) && !MaySave(p))
                    weaks.Add(p);
            }

            if (weaks.Count > 0)
            {
                SortByDefense(ref weaks, false);
                target = weaks[0];
            }

            point = 0;
            result = -1;
            if (target != null)
            {
                bool judge_skip = false;
                if (HasSkill("xiantu", target) && target.ContainsTag("xiantu"))
                {
                    foreach (int id in ids)
                    {
                        if (IsCard(id, Analeptic.ClassName, target) || IsCard(id, Peach.ClassName, target))
                            return new KeyValuePair<Player, int>(target, id);
                    }

                    if (target.Hp == 1)
                    {
                        judge_skip = true;
                        skip.Add(target);
                    }
                }

                if (!judge_skip)
                {
                    foreach (int id in ids)
                    {
                        double value = GetKeepValue(id, target, dest_place);
                        if (HasSkill(CardneedSkill, target))
                            value += 1;
                        if (value >= 6 && value > point)
                        {
                            point = value;
                            result = id;
                        }
                    }
                    if (result >= 0)
                    {
                        Debug.Assert(ids.Contains(result));
                        return new KeyValuePair<Player, int>(target, result);
                    }
                }
            }

            foreach (Player p in players)
            {
                if (dest_place == Place.PlaceHand && HasSkill("zishu", p) && p.Phase == PlayerPhase.NotActive || skip.Contains(p)) continue;
                if (p.Phase == PlayerPhase.Play || !WillSkipPlayPhase(p))
                    acts.Add(p);
            }
            room.SortByActionOrder(ref acts);

            target = null;
            foreach (Player p in acts)
            {
                foreach (int id in ids)
                {
                    double value = GetUseValue(id, p, dest_place);
                    if (HasSkill(CardneedSkill, p))
                        value += 1;
                    if (value >= 6 && value > point)
                    {
                        point = value;
                        result = id;
                        target = p;
                    }
                }
            }
            if (target != null)
            {
                Debug.Assert(ids.Contains(result));
                return new KeyValuePair<Player, int>(target, result);
            }

            foreach (Player p in acts)
            {
                if (p.Phase == PlayerPhase.NotActive || dest_place != Place.PlaceHand || GetOverflow(p) <= 0)
                {
                    target = p;
                    break;
                }
            }
            if (target != null)
            {
                foreach (int id in ids)
                {
                    double value = GetUseValue(id, target, dest_place);
                    if (HasSkill(CardneedSkill, target))
                        value += 1;
                    if (value >= 6 && value > point)
                    {
                        point = value;
                        result = id;
                    }
                }
                if (result >= 0)
                {
                    Debug.Assert(ids.Contains(result));
                    return new KeyValuePair<Player, int>(target, result);
                }
            }

            return new KeyValuePair<Player, int>();
        }

        public bool NotHurtXiaoqiao(DamageStruct damage)
        {
            Player from = damage.From;
            Player to = damage.To;

            if (from == null || HasSkill("jueqing|gangzhi_classic", from)) return false;

            if (!HasSkill("tianxiang|tianxiang_jx", to) || damage.Damage <= 0 || IsFriend(to)
                || (GetKnownCards(to).Count == to.HandcardNum && GetKnownCardsNums(".|heart", "h", to, self) == 0)
                || IsLackCard(to, "heart"))
                return false;

            if (damage.Steped == DamageStruct.DamageStep.Done && HasArmorEffect(to, Vine.ClassName) && damage.Damage > 1 && damage.Nature == DamageStruct.DamageNature.Fire) damage.Damage -= 1;

            if (from != null && from.Alive && !IsFriend(from, to))
            {
                if ((HasSkill("zhiman|zhiman_jx", from) && from != to)
                        || (!damage.Transfer && damage.Card != null && (damage.Card.Name.Contains(Slash.ClassName) || damage.Card.Name == Duel.ClassName) && HasSkill("chuanxin", from))
                        || (!damage.Transfer && damage.Card != null && damage.Card.Name.Contains(Slash.ClassName) && !to.IsNude() && from.HasWeapon(IceSword.ClassName)))
                    return false;
            }

            damage.Transfer = true;
            foreach (Player p in room.GetOtherPlayers(to))
            {
                DamageStruct _damage = damage;
                _damage.To = p;
                int hurt = DamageEffect(_damage, DamageStruct.DamageStep.Done);
                _damage.Damage = hurt;
                if (IsFriend(p) && p.Hp <= hurt && !HasBuquEffect(p) && !HasNiepanEffect(p) && !CanResist(p, hurt))
                    return true;
                if (IsFriend(p) && WillSkipPlayPhase(p) && hurt > 0 && !CanResist(p, hurt)) return true;
                if (IsFriend(to, p) && IsEnemy(p)
                        && ((p.Hp - hurt > 0 || HasBuquEffect(p) || CanResist(p, hurt)) && (p.GetLostHp() + hurt > hurt * 2
                        || GetDamageScore(_damage).Score > 5)))
                    return true;
            }

            return false;
        }

        public bool NotSlashJiaozhu(Player from, Player jiaozhu, WrappedCard card)
        {
            if (IsFriend(jiaozhu) || !HasSkill("leiji|leiji_jx", jiaozhu)) return false;
            if (HasSkill("fuqi", from) && RoomLogic.DistanceTo(room, from, jiaozhu) == 1) return false;

            if (from.ContainsTag("wenji") && card != null && from.GetTag("wenji") is List<string> names
                && (names.Contains(card.Name) || (card.Name.Contains(Slash.ClassName) && names.Contains(Slash.ClassName))))
                return false;

            DamageStruct damage = new DamageStruct
            {
                From = jiaozhu,
                Damage = 2,
                Nature = DamageStruct.DamageNature.Thunder
            };
            foreach (Player p in room.GetAlivePlayers())
            {
                DamageStruct _damage = damage;
                _damage.To = p;
                _damage.Damage = DamageEffect(_damage, DamageStruct.DamageStep.Done);
                if (_damage.Damage > 0 && (!CanResist(p, _damage.Damage) || IsFriend(p, jiaozhu)) && (IsFriend(p) || ChainDamage(_damage) < 0))
                {
                    Player wizzard = GetWizzardRaceWinner("leiji", p);
                    if (wizzard == null || !IsFriend(wizzard) && !IsFriend(jiaozhu, p) && !IsFriend(wizzard, p))
                        return true;
                }
            }

            return false;
        }

        public bool JiaozhuneedSlash(Player from, Player jiaozhu, WrappedCard card)
        {
            if (!IsFriend(jiaozhu) || !HasSkill("leiji|leiji_jx", jiaozhu)) return false;

            if (HasSkill("fuqi", from) && RoomLogic.DistanceTo(room, from, jiaozhu) == 1) return false;

            if (from.ContainsTag("wenji") && card != null && from.GetTag("wenji") is List<string> names
                && (names.Contains(card.Name) || (card.Name.Contains(Slash.ClassName) && names.Contains(Slash.ClassName))))
                return false;

            DamageStruct damage = new DamageStruct
            {
                From = jiaozhu,
                Damage = 2,
                Nature = DamageStruct.DamageNature.Thunder
            };
            foreach (Player p in room.GetAlivePlayers())
            {
                DamageStruct _damage = damage;
                _damage.To = p;
                _damage.Damage = DamageEffect(_damage, DamageStruct.DamageStep.Done);
                if (_damage.Damage > 1 && !CanResist(p, _damage.Damage) && IsEnemy(p) && ChainDamage(_damage) >= 0)
                {
                    Player wizzard = GetWizzardRaceWinner("leiji", p);
                    if (wizzard != null && IsFriend(wizzard))
                        return true;
                }
            }

            return false;
        }

        public bool GetAoeValue(WrappedCard card)
        {
            Player attacker = self;
            double good = 0, bad = 0;
            Player current = room.Current;
            int target_num = 0;
            bool wansha = RoomLogic.PlayerHasShownSkill(room, current, "wansha");
            int peach_num = GetKnownCardsNums(Peach.ClassName, "he", self);
            int null_num = GetKnownCardsNums(Nullification.ClassName, "he", self);
            int enemies = 0;
            
            Player dongyun = FindPlayerBySkill("sheyan");
            if (dongyun != null && (dongyun == self || RoomLogic.IsProhibited(room, self, dongyun, card) != null)) dongyun = null;
            double best_good = 0, best_bad = 0;

            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetOtherPlayers(self))
            {
                if (RoomLogic.IsProhibited(room, self, p, card) == null && !IsCancelTarget(card, p, self) && IsCardEffect(card, p, self))
                    targets.Add(p);
                if (RoomLogic.IsProhibited(room, self, p, card) == null && !IsCancelTarget(card, p, self))
                    target_num++;
            }

            foreach (Player p in room.GetOtherPlayers(self))
            {
                if (!IsFriend(p) && GetPlayerTendency(p) != self.Kingdom)
                    enemies++;

                bool fuqi = false;
                if (HasSkill("fuqi") && RoomLogic.DistanceTo(room, self, p) == 1)
                    fuqi = true;
                if (self.ContainsTag("wenji") && self.GetTag("wenji") is List<string> names && names.Contains(card.Name))
                    fuqi = true;

                if (IsFriend(p))
                {
                    int p_count = 0;
                    if (!wansha)
                    {
                        foreach (WrappedCard c in GetCards(Peach.ClassName, p, true))
                            if (!RoomLogic.IsCardLimited(room, p, c, HandlingMethod.MethodUse))
                                p_count++;
                    }

                    if (!fuqi)
                        foreach (WrappedCard c in GetCards(Nullification.ClassName, p, true))
                            if (!RoomLogic.IsCardLimited(room, p, c, HandlingMethod.MethodUse))
                                null_num++;
                }
                else if (!fuqi)
                    null_num -= GetKnownCardsNums(Nullification.ClassName, "he", p, self);
            }
            foreach (int id in card.SubCards)
            {
                if (IsCard(id, Peach.ClassName, self))
                    peach_num--;
                if (IsCard(id, Nullification.ClassName, self))
                    null_num--;
            }
            string pattern = Jink.ClassName;
            if (card.Name == SavageAssault.ClassName)
            {
                pattern = Slash.ClassName;
                Player menghuo = FindPlayerBySkill("huoshou");
                if (menghuo != null && RoomLogic.PlayerHasShownSkill(room, menghuo, "huoshou"))
                    attacker = menghuo;
            }

            foreach (Player p in targets)
            {
                DamageStruct damage = new DamageStruct(card, attacker, p);
                ScoreStruct score = GetDamageScore(damage);

                bool fuqi = false;
                if (HasSkill("fuqi") && RoomLogic.DistanceTo(room, self, p) == 1)
                    fuqi = true;

                foreach (string skill in GetKnownSkills(p))
                {
                    SkillEvent e = Engine.GetSkillEvent(skill);
                    if (e != null)
                        score.Score += e.TargetValueAdjust(this, card, self, targets, p);
                }

                if (!fuqi && pattern == Jink.ClassName)
                {
                    if (NotSlashJiaozhu(self, p, card))
                    {
                        bad += 20;
                        if (best_bad < 20) best_bad = 20;
                        continue;
                    }
                    if (JiaozhuneedSlash(self, p, card))
                    {
                        good += 20;
                        if (best_good < 20) best_good = 20;
                        continue;
                    }
                }

                double value = score.Score;
                if (IsFriend(p) && score.Score > 0)
                {
                    good += value;
                    if (best_good < value) best_good = value;
                }
                else if (!IsFriend(p) && score.Score < 0)
                {
                    bad -= value;
                    if (best_bad < -value) best_bad = -value;
                }
                else
                {
                    if (fuqi)
                    {
                        if (IsFriend(p))
                        {
                            bad += value;
                            if (best_bad < -value) best_bad = -value;
                        }
                        else
                        {
                            good += value;
                            if (best_good < value) best_good = value;
                        }
                    }
                    else
                    {
                        //room.OutPut("对" + p.SceenName + "计算命中率");
                        //计算命中率
                        double basic_rate = 0;
                        bool no_red = p.GetMark("@qianxi_red") > 0;
                        bool no_black = p.GetMark("@qianxi_black") > 0;
                        double hit_rate = 0;
                        double rate = pattern == Slash.ClassName ? 4 : 6;
                        if (pattern == Jink.ClassName && HasArmorEffect(p, EightDiagram.ClassName))
                        {
                            if (HasSkill("tiandu+qingguo", p) && !no_black)
                                basic_rate = 1;
                            else
                                basic_rate = 0.35;
                            if (HasSkill("tiandu", p))
                            {
                                if (IsFriend(p))
                                    good += 2;
                                else
                                    bad += 2;
                            }
                        }
                        if (HasSkill("longdan|longdan_fz|longdan_jx", p))
                            rate -= 2;
                        if (no_black && pattern == Slash.ClassName)
                            rate += 2.5;
                        if (no_red && pattern == Jink.ClassName)
                            rate = 0;

                        if (basic_rate < 1)
                        {
                            int count = 0;
                            foreach (WrappedCard c in GetCards(pattern, p, true))
                                if (!RoomLogic.IsCardLimited(room, p, c, HandlingMethod.MethodResponse))
                                    count++;

                            bool guess = true;
                            if (count > 0)
                            {
                                hit_rate = 0;
                                guess = false;
                            }
                            else
                            {
                                count = p.HandcardNum + p.GetHandPile().Count - GetKnownHandPileCards(p).Count;
                                if (RoomLogic.IsHandCardLimited(room, p, HandlingMethod.MethodResponse)) count -= p.HandcardNum;

                                hit_rate = Math.Max(0, 1 - count / rate);
                            }

                            if (guess && (p.GetRoleEnum() == PlayerRole.Lord || IsFriend(p)) && value < -10)                     //如果风险很大，就不能赌
                            {
                                bad -= value;
                                if (best_bad < -value) best_bad = -value;
                            }
                            else
                            {
                                double _good = 0, _bad = 0;
                                if (IsFriend(p))
                                {
                                    _bad -= score.Score * Math.Max(1 - hit_rate - basic_rate, 0);
                                    _bad -= (1 - basic_rate) * (1 - hit_rate) * 2;
                                    if (IsWeak(p)) _bad += 8;
                                }
                                else
                                {
                                    _good += score.Score * Math.Max(1 - hit_rate - basic_rate, 0);
                                    _good += (1 - basic_rate) * (1 - hit_rate) * 2;
                                }

                                if (_bad > 10 && null_num > 0)
                                {
                                    null_num--;
                                    _bad = 4;
                                }
                                if (_bad > 10 && peach_num > 0)
                                {
                                    peach_num--;
                                    _bad = 4;
                                }
                                if (_good > 10 && null_num < 0)
                                {
                                    null_num++;
                                    _good = 4;
                                }
                                bad += _bad;
                                good += _good;
                                if (best_good < _good) best_good = _good;
                                if (best_bad < _bad) best_bad = _bad;
                            }
                        }

                        if (card.Skill == "luanji" && IsFriend(p))
                            bad -= Math.Min(hit_rate + basic_rate, 1) * 2;
                    }
                }
            }
            double adjust = 0;
            if (HasSkill("tushe"))
            {
                bool basic = false;
                foreach (int id in self.GetCards("h"))
                {
                    WrappedCard wrapped = room.GetCard(id);
                    if (Engine.GetFunctionCard(wrapped.Name).TypeID == CardType.TypeBasic)
                    {
                        basic = true;
                        break;
                    }
                }
                if (!basic) adjust = 1.5 * target_num;
                good += adjust;
            }

            if (dongyun != null)
            {
                if (IsFriend(dongyun))
                    bad -= best_bad;
                else if (IsEnemy(dongyun))
                    good -= best_good;
            }
            if (self.HasFlag("qiaoshui") && self.GetMark("qiaoshui") == 0)
                bad -= best_bad;

            return good > bad;
        }
        public bool CanSave(Player dying, int peaches)
        {
            if (peaches <= 0) return true;
            if (dying.Removed) return true;                     //new lure tiger

            int count = 0;
            List<WrappedCard> analeptics = GetCards(Analeptic.ClassName, dying);
            foreach (WrappedCard card in analeptics)
                if (!RoomLogic.IsCardLimited(room, dying, card, HandlingMethod.MethodUse) && RoomLogic.IsProhibited(room, dying, dying, card) == null)
                    count++;

            WrappedCard ana = new WrappedCard(Analeptic.ClassName);
            foreach (Player p in GetFriends(dying))
            {
                List<WrappedCard> peach = GetCards(Peach.ClassName, p);
                foreach (WrappedCard card in peach)
                    if (!RoomLogic.IsCardLimited(room, p, card, HandlingMethod.MethodUse) && RoomLogic.IsProhibited(room, p, dying, card) == null)
                        count++;
                if (HasSkill("chunlao", p) && !RoomLogic.IsCardLimited(room, dying, ana, HandlingMethod.MethodUse) && RoomLogic.IsProhibited(room, dying, dying, ana) == null)
                    count += p.GetPile("chun").Count;
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
        public void ClearCardLack(Player who)
        {
            card_lack[who].Clear();
        }

        public void ClearCardLack(Player who, int id)
        {
            List<WrappedCard> cards = GetViewAsCards(who, id);
            foreach (WrappedCard card in cards)
            {
                string card_name = card.Name.Contains(Slash.ClassName) ? Slash.ClassName : card.Name;
                SetCardLack(who, "-" + card_name);
            }

            SetCardLack(who, "-" + room.GetCard(id).Suit.ToString());
        }

        public bool IsLackCard(Player who, string flag)
        {
            if (flag == Slash.ClassName && HasSkill("longdan|longdan_jx", who) && card_lack[who].Contains(Jink.ClassName))
                return true;

            if (flag == Jink.ClassName && HasSkill("longdan|longdan_jx", who) && card_lack[who].Contains(Slash.ClassName))
                return true;

            return card_lack[who].Contains(flag);
        }
        private readonly List<string> kingdoms = new List<string> { "wei", "shu", "wu", "qun" };
        public virtual void Event(TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.GameStart && player == self)
            {
                foreach (string skill in room.Skills)
                {
                    SkillEvent e = Engine.GetSkillEvent(skill);
                    if (e != null)
                        skill_events[skill] = e;
                }

                foreach (FunctionCard card in room.AvailableFunctionCards)
                {
                    SkillEvent e = Engine.GetSkillEvent(card.Name);
                    if (e != null)
                        skill_events[card.Name] = e;
                }

                foreach (int id in room.RoomCards)
                {
                    WrappedCard card = Engine.GetRealCard(id);
                    if (!CardCounts.ContainsKey(card.Name))
                        CardCounts[card.Name] = 1;
                    else
                        CardCounts[card.Name]++;
                }
            }
            else if (triggerEvent == TriggerEvent.EventAcquireSkill && data is InfoStruct info && !skill_events.ContainsKey(info.Info))
            {
                SkillEvent e = Engine.GetSkillEvent(info.Info);
                if (e != null) skill_events[info.Info] = e;
            }
        }

        public void FilterSkillCard(ref List<WrappedCard> cards)
        {
            foreach (string skill in room.Skills)
            {
                if (skill.StartsWith("#")) continue;
                if (RoomLogic.PlayerHasSkill(room, self, skill) || (skill == "shuangxiong" && (self.HasFlag("shuangxiong_head") || self.HasFlag("shuangxiong_deputy")))
                    || (skill == "shuangxiong_jx" && (self.HasFlag("shuangxiong_jx_head") || self.HasFlag("shuangxiong_jx_deputy"))))
                {
                    SkillEvent e = Engine.GetSkillEvent(skill);
                    if (e != null)
                    {
                        List<WrappedCard> skill_cards = e.GetTurnUse(this, self);
                        if (skill_cards != null)
                            cards.AddRange(skill_cards);
                    }
                }
            }
            //equip
            if (self.GetMark("Equips_Nullified_to_Yourself") == 0)
            {
                foreach (int id in self.GetEquips())
                {
                    SkillEvent e = Engine.GetSkillEvent(room.GetCard(id).Name);
                    if (e != null)
                    {
                        List<WrappedCard> skill_cards = e.GetTurnUse(this, self);
                        if (skill_cards != null)
                            cards.AddRange(skill_cards);
                    }
                }
            }
            //hegemony addition marks
            if (self.GetMark("@companion") > 0)
            {
                SkillEvent e = Engine.GetSkillEvent("companion");
                if (e != null)
                {
                    List<WrappedCard> skill_cards = e.GetTurnUse(this, self);
                    if (skill_cards != null)
                        cards.AddRange(skill_cards);
                }
            }
            if (self.GetMark("@megatama") > 0)
            {
                SkillEvent e = Engine.GetSkillEvent("megatama");
                if (e != null)
                {
                    List<WrappedCard> skill_cards = e.GetTurnUse(this, self);
                    if (skill_cards != null)
                        cards.AddRange(skill_cards);
                }
            }
            if (self.GetMark("@pioneer") > 0)
            {
                SkillEvent e = Engine.GetSkillEvent("pioneer");
                if (e != null)
                {
                    List<WrappedCard> skill_cards = e.GetTurnUse(this, self);
                    if (skill_cards != null)
                        cards.AddRange(skill_cards);
                }
            }

            SkillEvent transfer = Engine.GetSkillEvent("transfer");
            {
                if (transfer != null)
                {
                    List<WrappedCard> skill_cards = transfer.GetTurnUse(this, self);
                    if (skill_cards != null)
                        cards.AddRange(skill_cards);
                }
            }
        }

        protected bool weapon_use;
        protected int predicted_range;
        protected int slash_avail;

        public virtual List<CardUseStruct> GetTurnUse()
        {
            List<WrappedCard> cards = new List<WrappedCard>();
            List<CardUseStruct> result = new List<CardUseStruct>();
            foreach (WrappedCard card in RoomLogic.GetPlayerHandcards(room, self))
            {
                WrappedCard _card = card;
                if (card.Name == Peach.ClassName && room.BloodBattle && room.Setting.GameMode == "Hegemony")
                {
                    WrappedCard vs_slash = new WrappedCard(Slash.ClassName);
                    vs_slash.AddSubCard(card);
                    _card = RoomLogic.ParseUseCard(room, vs_slash);
                }
                FunctionCard fcard = Engine.GetFunctionCard(_card.Name);
                if (fcard.IsAvailable(room, self, _card))
                    cards.Add(_card);
            }
            foreach (int id in self.GetHandPile())
            {
                WrappedCard card = room.GetCard(id);
                if (card.Name == Peach.ClassName && room.BloodBattle && room.Setting.GameMode == "Hegemony")
                {
                    WrappedCard vs_slash = new WrappedCard(Slash.ClassName);
                    vs_slash.AddSubCard(card);
                    card = RoomLogic.ParseUseCard(room, vs_slash);
                }
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                if (fcard.IsAvailable(room, self, card))
                    cards.Add(card);
            }

            WrappedCard slash = new WrappedCard(Slash.ClassName);
            int slashAvail = 1 + Engine.CorrectCardTarget(room, TargetModSkill.ModType.Residue, self, slash);
            predicted_range = RoomLogic.GetAttackRange(room, self);
            FilterSkillCard(ref cards);

            if (self.HasWeapon(CrossBow.ClassName))
                slashAvail = 100;
            slash_avail = slashAvail;

            bool slash_check = false;
            foreach (WrappedCard card in cards)
            {
                UseCard e = Engine.GetCardUsage(card.Name);
                if (e != null)
                {
                    if (e.Name.Contains(Slash.ClassName) && slash_check)
                        continue;

                    CardUseStruct use = new CardUseStruct
                    {
                        From = self,
                        To = new List<Player>(),
                        IsDummy = true,
                    };
                    e.Use(this, self, ref use, card);
                    if (use.Card != null)
                    {
                        FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                        if (fcard is Slash)
                        {
                            slash_check = true;
                            result.Add(use);
                            if (use.Card.HasFlag("AIGlobal_KillOff"))
                                break;
                        }
                        else
                        {
                            if (self.HasFlag("InfinityAttackRange") || self.GetMark("InfinityAttackRange") > 0)
                                predicted_range = 10000;
                            else if (fcard is Weapon weapon)
                            {
                                predicted_range = weapon.Range;
                                weapon_use = true;
                            }
                            else
                                predicted_range = 1;

                            if (fcard is CrossBow)
                            {
                                slashAvail = 100;
                                slash_avail = slashAvail;
                            }
                            result.Add(use);
                        }

                        if (GetDynamicUsePriority(use) >= 9)
                            break;
                    }
                }
            }

            return result;
        }
        public virtual double GetUsePriority(CardUseStruct use)
        {
            WrappedCard card = use.Card;
            FunctionCard fcard = Engine.GetFunctionCard(card.Name);
            double v = Engine.GetCardPriority(card.Name);

            UseCard e = Engine.GetCardUsage(card.Name);
            if (e != null)
                v += e.UsePriorityAdjust(this, self, use.To, card);

            if (fcard is SkillCard)
                return v;

            if (self.GetHandPile().Contains(card.GetEffectiveId()))
                v += 0.1;

            foreach (string skill in GetKnownSkills(self))
            {
                SkillEvent s = Engine.GetSkillEvent(skill);
                if (s != null)
                    v += s.UsePriorityAdjust(this, self, use);
            }

            return v;
        }

        public double GetDynamicUsePriority(CardUseStruct use)
        {
            WrappedCard card = use.Card;
            if (card == null) return 0;
            if (card.HasFlag("AIGlobal_KillOff")) return 15;
            FunctionCard fcard = Engine.GetFunctionCard(card.Name);

            if (fcard is AmazingGrace)
            {
                Player zhugeliang = FindPlayerBySkill("kongcheng|kongcheng_jx");
                if (zhugeliang != null && IsEnemy(zhugeliang) && zhugeliang.IsKongcheng())
                    return Math.Max(Engine.GetCardPriority(Slash.ClassName), Engine.GetCardPriority(Duel.ClassName)) + 0.1f;
            }
            else if (fcard is Peach && RoomLogic.PlayerHasSkill(room, self, "kuanggu")) return 1.01;
            else if (fcard is Duel)
            {
                if (HasCrossbowEffect(self) || RoomLogic.CanSlashWithoutCrossBow(room, self)
                        || Engine.CorrectCardTarget(room, TargetModSkill.ModType.Residue, self, new WrappedCard(Slash.ClassName)) > 0
                        || self.HasUsed("FenxunCard"))
                    return Engine.GetCardPriority(Slash.ClassName) - 0.1f;
            }
            else if (fcard is AwaitExhausted && HasSkill("guose|duanliang"))
                return 0;

            double value = GetUsePriority(use);
            return value;
        }

        public virtual bool HasLoseHandcardEffective(Player player = null)
        {
            player = player ?? self;
            if (HasSkill("lianying", player)) return true;
            if (player.Phase == PlayerPhase.NotActive)
            {
                foreach (Player p in GetFriends(player))
                {
                    if (HasSkill("shoucheng", p) && RoomLogic.IsFriendWith(room, p, player))
                        return true;
                }
            }

            return false;
        }
        public List<Player> Exclude(List<Player> targets, WrappedCard card, Player from = null)
        {
            from = from ?? self;
            List<Player> result = new List<Player>();
            FunctionCard fcard = Engine.GetFunctionCard(card.Name);
            if (fcard.IsAvailable(room, from, card) && !fcard.TargetFixed(card))
            {
                foreach (Player p in targets)
                    if (fcard.TargetFilter(room, new List<Player>(), p, from, card) && !IsCancelTarget(card, p, from) && IsCardEffect(card, p, from))
                        result.Add(p);
            }

            return result;
        }

        public List<Player> GetChainedFriends(Player player = null)
        {
            player = player ?? self;
            List<Player> chainedFriends = new List<Player>();
            foreach (Player p in GetFriends(player))
                if (p.Chained)
                    chainedFriends.Add(p);

            return chainedFriends;
        }

        public WrappedCard GetMaxCard(Player player = null, List<int> cards = null, List<string> exceptions = null)
        {
            player = player ?? Self;
            if (player.IsKongcheng()) return null;

            cards = cards ?? new List<int>(GetKnownCards(player));
            WrappedCard max_card = null;
            int max_point = 0;

            if (self == player && exceptions != null && exceptions.Count > 0)
            {
                List<int> ex = new List<int>();
                foreach (string card_name in exceptions)
                {
                    foreach (int id in cards)
                    {
                        if (IsCard(id, card_name, player))
                            ex.Add(id);
                    }
                    cards.RemoveAll(t => ex.Contains(t));
                }
            }

            List<int> result = new List<int>();
            foreach (int id in cards)
            {
                WrappedCard card = room.GetCard(id);
                if (card.Number > max_point)
                    max_point = card.Number;
            }

            foreach (int id in cards)
            {
                WrappedCard card = room.GetCard(id);
                if (max_point == card.Number)
                    result.Add(id);
            }

            if (result.Count > 0)
            {
                if (room.Current == player)
                    SortByUseValue(ref result, false);
                else
                    SortByKeepValue(ref result, false);

                max_card = room.GetCard(result[0]);
            }
            
            return max_card;
        }

        public WrappedCard GetMinCard(Player player = null, List<int> cards = null, List<string> exceptions = null)
        {
            player = player ?? Self;
            if (player.IsKongcheng()) return null;

            cards = cards ?? new List<int>(GetKnownCards(player));
            WrappedCard min_card = null;
            int min_point = 14;

            if (self == player && exceptions != null && exceptions.Count > 0)
            {
                List<int> ex = new List<int>();
                foreach (string card_name in exceptions)
                {
                    foreach (int id in cards)
                    {
                        if (IsCard(id, card_name, player))
                            ex.Add(id);
                    }
                    cards.RemoveAll(t => ex.Contains(t));
                }
            }

            foreach (int id in cards)
            {
                WrappedCard card = room.GetCard(id);
                if (card.Number < min_point)
                {
                    min_point = card.Number;
                    min_card = card;
                }
            }

            return min_card;
        }

        private void GuanxingDebug(List<int> in_ups, List<int> in_downs, List<int> ups, List<int> downs)
        {
            foreach (int id in in_ups)
            {
                WrappedCard card = room.GetCard(id);
                room.Debug(string.Format("观星牌上{0}是 {1},点数{2},花色{3}", in_ups.IndexOf(id), card.Name, card.Number, WrappedCard.GetSuitString(card.Suit)));
            }
            foreach (int id in in_downs)
            {
                WrappedCard card = room.GetCard(id);
                room.Debug(string.Format("观星牌下{0}是 {1},点数{2},花色{3}", in_downs.IndexOf(id), card.Name, card.Number, WrappedCard.GetSuitString(card.Suit)));
            }
            foreach (int id in ups)
            {
                WrappedCard card = room.GetCard(id);
                room.Debug(string.Format("结果上{0}是 {1},点数{2},花色{3}", ups.IndexOf(id), card.Name, card.Number, WrappedCard.GetSuitString(card.Suit)));
            }
            foreach (int id in downs)
            {
                WrappedCard card = room.GetCard(id);
                room.Debug(string.Format("结果下{0}是 {1},点数{2},花色{3}", downs.IndexOf(id), card.Name, card.Number, WrappedCard.GetSuitString(card.Suit)));
            }
        }

        public AskForMoveCardsStruct Guanxing(List<int> up_input)
        {
            AskForMoveCardsStruct move = new AskForMoveCardsStruct
            {
                Success = true
            };
            List<int> up = new List<int>(), bottom = new List<int>(), cards = new List<int>(up_input);
            WrappedCard lightning = null;
            List<WrappedCard> judged_list = new List<WrappedCard>();
            bool willSkipDrawPhase = false;
            int nulli_skip = 0;

            for (int i = 0; i < cards.Count; i++)
            {
                WrappedCard card = room.GetCard(cards[i]);
            }

            for (int i = Self.JudgingArea.Count - 1; i >= 0; i--)
            {
                WrappedCard card = room.GetCard(Self.JudgingArea[i]);
                judged_list.Add(card);
                if (card.Name == Lightning.ClassName)
                    lightning = card;
            }

            bool lightning_effect = false;
            if (lightning != null)
            {
                DamageStruct damage = new DamageStruct(lightning, null, self, 3, DamageStruct.DamageNature.Thunder);
                if (GetDamageScore(damage).Score < 0)
                    lightning_effect = true;
            }

            if (judged_list.Count > 0 && Self.GetPile("incantation").Count > 0) //被张宝咒缚
                judged_list.RemoveAt(0);

            List<List<int>> ids_for_judge = new List<List<int>>();
            List<int> first = new List<int>(), second = new List<int>(), last = new List<int>();
            if (judged_list.Count > 0)
            {
                for (int i = 0; i < Math.Min(cards.Count, judged_list.Count); i++)
                {
                    foreach (int id in cards)
                    {
                        WrappedCard card = room.GetCard(id);
                        int number = card.Number;
                        WrappedCard.CardSuit suit = card.Suit;
                        if (suit == WrappedCard.CardSuit.Spade && HasSkill("hongyan|hongyan_jx"))
                            suit = WrappedCard.CardSuit.Heart;

                        bool add = false;
                        if (judged_list[i].Name == Lightning.ClassName)
                        {
                            if (number == 1 || number > 9 || suit != WrappedCard.CardSuit.Spade)
                                add = true;
                        }
                        else if (judged_list[i].Name == SupplyShortage.ClassName && suit == WrappedCard.CardSuit.Club)
                            add = true;
                        else if (judged_list[i].Name == Indulgence.ClassName && suit == WrappedCard.CardSuit.Heart)
                            add = true;

                        if (add)
                        {
                            switch (i)
                            {
                                case 0:
                                    first.Add(id);
                                    break;
                                case 1:
                                    second.Add(id);
                                    break;
                                case 2:
                                    last.Add(id);
                                    break;
                            }
                        }
                    }
                }


                int nuli = GetKnownCardsNums(Nullification.ClassName, "he", self);
                List<int> _first = first.Count > 0 ? new List<int>(first) : new List<int>(cards);
                if (first.Count == 0)
                {
                    bool use_nulli = false;
                    if (judged_list[0].Name == Lightning.ClassName)
                    {
                        if (lightning_effect)
                        {
                            move.Top = new List<int>();
                            move.Bottom = new List<int>(cards);
                            return move;                //闪电没有安全牌直接全部放底
                        }
                    }
                    else if (nuli > 0)
                    {
                        if (judged_list.Count > 1 && judged_list[1].Name == Indulgence.ClassName && second.Count == 0)
                        {
                            nuli--;             //为下一张乐保留无懈可击
                        }
                        if (judged_list.Count > 2 && judged_list[2].Name == Indulgence.ClassName && last.Count == 0)
                        {
                            nuli--;             //为最后一张乐保留无懈可击
                        }
                        if (nuli > 0)
                            use_nulli = true;
                    }
                    else if (judged_list[0].Name == Indulgence.ClassName)
                    {
                        move.Top = new List<int>();
                        move.Bottom = new List<int>(cards);
                        return move;                     //中乐又没有无懈可击直接全部放底
                    }

                    if (use_nulli)
                    {
                        nulli_skip++;
                        _first.Clear();                     //使用无懈可击回避第一个
                    }
                    else if (judged_list.Count > 1 && second.Count > 0)     //硬吃第一个的前提是后面有安全牌
                    {
                        if (judged_list[0].Name == SupplyShortage.ClassName)
                            willSkipDrawPhase = true;           //为了后面能过判，硬吃第一个兵
                    }
                    else
                    {
                        move.Top = new List<int>();
                        move.Bottom = new List<int>(cards);
                        return move;                     //判定超过1张而且即没有无邪也没安全牌就直接全部放底
                    } 
                }

                foreach (int id in _first)
                    ids_for_judge.Add(new List<int> { id });

                nuli = GetKnownCardsNums(Nullification.ClassName, "he", self);
                if (judged_list.Count > 1 && second.Count == 0)
                {
                    bool use_nulli = false;
                    if (judged_list[1].Name == Lightning.ClassName)
                    {
                        if (lightning_effect)
                        {
                            cards.RemoveAll(t => ids_for_judge[0].Contains(t));
                            move.Top = ids_for_judge[0];
                            move.Bottom = new List<int>(cards);
                            return move;                     //如果是闪电，没有安全牌就放底
                        }
                        else
                            second = new List<int>(cards);

                    }
                    else if (nuli > 0)
                    {
                        if (judged_list[1].Name == Indulgence.ClassName && first.Count == 0)
                        {
                            nuli--;             //为第一张乐保留无懈可击
                        }
                        if (judged_list.Count > 2 && judged_list[2].Name == Indulgence.ClassName && last.Count == 0)
                        {
                            nuli--;             //为最后一张乐保留无懈可击
                        }
                        if (nuli > 0)
                            use_nulli = true;
                    }
                    else if (judged_list[1].Name == Indulgence.ClassName)
                    {
                        cards.RemoveAll(t => ids_for_judge[0].Contains(t));
                        move.Top = ids_for_judge[0];
                        move.Bottom = new List<int>(cards);
                        return move;          //中乐又没有无懈可击直接全部放底
                    }

                    if (use_nulli)
                    {
                        nulli_skip++;                   //使用无懈可击回避第二个
                    }
                    else if (judged_list.Count > 2 && judged_list[2].Name == Indulgence.ClassName && last.Count > 0)     //硬吃第二个的前提是后面的乐有安全牌
                    {
                        second = new List<int>(cards);
                        willSkipDrawPhase = true;           //为了后面能过判，硬吃第二个兵
                    }
                    else
                    {
                        cards.RemoveAll(t => ids_for_judge[0].Contains(t));
                        move.Top = ids_for_judge[0];
                        move.Bottom = new List<int>(cards);
                        return move;                    //判定超过1张而且即没有无邪也没安全牌就直接全部放底
                    }
                }

                if (judged_list.Count > 1)
                {
                    List<List<int>> judge2 = new List<List<int>>(ids_for_judge);
                    ids_for_judge.Clear();
                    foreach (List<int> ids in judge2)
                    {
                        List<int> _second = new List<int>(second);
                        _second.RemoveAll(t => ids.Contains(t));
                        foreach (int id in _second)
                        {
                            List<int> pair = new List<int>(ids)
                            {
                                id
                            };
                            ids_for_judge.Add(pair);
                        }
                    }

                    if (judged_list.Count > 2)
                    {
                        if (last.Count == 0)
                        {
                            nuli = GetKnownCardsNums(Nullification.ClassName, "he", self);
                            if (judged_list[1].Name == Lightning.ClassName)
                            {
                                if (lightning_effect)
                                {
                                    cards.RemoveAll(t => ids_for_judge[0].Contains(t));
                                    move.Top = ids_for_judge[0];
                                    move.Bottom = new List<int>(cards);
                                    return move;        //如果是闪电，没有安全牌就放底
                                }
                                else
                                    last = new List<int>(cards);
                            }
                            else if (nuli > 0 && judged_list[2].Name == SupplyShortage.ClassName)
                            {
                                if (judged_list[0].Name == Indulgence.ClassName && first.Count == 0)
                                {
                                    nuli--;             //为第一张乐保留无懈可击
                                }
                                if (judged_list[1].Name == Indulgence.ClassName && second.Count == 0)
                                {
                                    nuli--;             //为第二张保留无懈可击
                                }
                            }

                            if (nuli <= 0)
                            {
                                cards.RemoveAll(t => ids_for_judge[0].Contains(t));
                                move.Top = ids_for_judge[0];
                                move.Bottom = new List<int>(cards);
                                return move;            //没有安全牌也没有无懈可击了
                            }
                            else
                                nulli_skip++;
                        }

                        List<List<int>> judge3 = new List<List<int>>(ids_for_judge);
                        foreach (List<int> ids in judge3)
                        {
                            List<int> _last = new List<int>(last);
                            _last.RemoveAll(t => ids.Contains(t));
                            foreach (int id in _last)
                            {
                                List<int> pair = new List<int>(ids)
                                {
                                    id
                                };
                                ids_for_judge.Add(pair);
                            }
                        }
                    }
                }
            }

            //把判定需要用的牌移除
            List<int> judge_remove = new List<int>();
            if (ids_for_judge.Count > 0)
            {
                double value = 1000;
                foreach (List<int> ids in ids_for_judge)
                {
                    double _value = 0;
                    foreach (int id in ids)
                        _value += GetUseValue(id, self);

                    if (_value < value)
                    {
                        value = _value;
                        judge_remove = ids;
                    }
                }

                if (judge_remove.Count > 0)
                    cards.RemoveAll(t => judge_remove.Contains(t));
            }

            //牌断摸牌数
            int draws = 0;
            if (!willSkipDrawPhase)
                draws = ImitateResult_DrawNCards(self);
            if (self.HasTreasure(JadeSeal.ClassName) && HasSkill("jizhi"))
            {
                foreach (Player p in Room.GetOtherPlayers(self))
                {
                    if (!p.HasShownOneGeneral() || p.HandcardNum > 0)
                    {
                        draws++;
                        break;
                    }
                }
            }
            int ex = GetKnownCardsNums(ExNihilo.ClassName, "he", self);
            foreach (int id in cards)
            {
                if (IsCard(id, ExNihilo.ClassName, self))
                    ex++;
            }
            draws += (2 * ex);

            int aw = GetKnownCardsNums(AwaitExhausted.ClassName, "he", self);
            int same = RoomLogic.GetPlayerNumWithSameKingdom(room, self);
            draws += (same * aw * 2);

            if (HasSkill("jizhi"))
            {
                draws += ex;
                draws += aw;
            }

            //给牌评分并按高->低排序
            List<int> best = new List<int>();
            List<int> _cards = new List<int>(cards);
            for (int i = 1; i < Math.Min(cards.Count, draws); i++)
            {
                double draw_value = -10000000000;
                int best_card = -1;
                foreach (int id in _cards)
                {
                    double _value = GetUseValue(id, self, Place.PlaceHand);
                    if (_value > draw_value)
                    {
                        draw_value = _value;
                        best_card = id;
                    }
                }

                Debug.Assert(best_card > -1);
                best.Add(best_card);
                _cards.Remove(best_card);
            }

            bool option = false;
            //判断下家的延时锦囊
            List<int> judge_for_next = new List<int>();
            if (cards.Count > best.Count)
            {
                Player next = room.GetNextAlive(self, 1, false);
                int count = 1;
                while (!next.FaceUp && count < room.AliveCount())
                {
                    count++;
                    next = room.GetNextAlive(next, 1, false);
                }

                List<int> lest = new List<int>(cards);
                lest.RemoveAll(t => best.Contains(t));

                bool skip = false;
                if (HasSkill("luoshen|luoshen_jx", next))
                {
                    skip = true;
                    if (next.GetPile("incantation").Count == 0)
                    {
                        if (IsFriend(next))
                        {
                            foreach (int id in lest)
                                if (WrappedCard.IsBlack(room.GetCard(id).Suit))
                                    judge_for_next.Add(id);
                        }
                        else
                        {
                            foreach (int id in lest)
                            {
                                if (WrappedCard.IsRed(room.GetCard(id).Suit))
                                {
                                    judge_for_next.Add(id);
                                    break;
                                }
                            }
                        }
                    }
                }

                if (!skip)
                {
                    List<WrappedCard> next_judge = new List<WrappedCard>();
                    for (int i = next.JudgingArea.Count - 1; i >= 0; i--)
                    {
                        WrappedCard card = room.GetCard(next.JudgingArea[i]);
                        next_judge.Add(card);
                    }
                    if (lightning != null && next == room.GetNextAlive(self, 1, false))
                        next_judge.Insert(0, lightning);

                    if (next_judge.Count > 0 && next.GetPile("incantation").Count > 0) //被张宝咒缚
                        next_judge.RemoveAt(0);

                    //清除判不到的延时锦囊
                    int judge_count = next_judge.Count;
                    for (int i = judge_count; i > lest.Count; i--)
                    {
                        next_judge.RemoveAt(i - 1);
                    }

                    if (next_judge.Count > 0)
                    {
                        if (IsFriend(next))
                        {
                            List<int> for_light = null, for_indu = null, for_supply = null;
                            foreach (WrappedCard judge in next_judge)
                            {
                                if (judge.Name == Lightning.ClassName)
                                {
                                    DamageStruct damage = new DamageStruct(judge, null, next, 3, DamageStruct.DamageNature.Thunder);
                                    if (GetDamageScore(damage).Score < 0)
                                    {
                                        for_light = new List<int>();
                                    }
                                }
                                else if (judge.Name == Indulgence.ClassName)
                                    for_indu = new List<int>();
                                else
                                    for_supply = new List<int>();
                            }

                            foreach (int id in lest)
                            {
                                WrappedCard card = room.GetCard(id);
                                int number = card.Number;
                                WrappedCard.CardSuit suit = card.Suit;
                                if (suit == WrappedCard.CardSuit.Spade && HasSkill("hongyan|hongyan_jx", next))
                                    suit = WrappedCard.CardSuit.Heart;

                                if (for_light != null)
                                {
                                    if (number == 1 || number > 9 || suit != WrappedCard.CardSuit.Spade)
                                        for_light.Add(id);
                                }
                                else if (for_supply != null && suit == WrappedCard.CardSuit.Club)
                                    for_supply.Add(id);
                                else if (for_indu != null && suit == WrappedCard.CardSuit.Heart)
                                    for_indu.Add(id);
                            }

                            foreach (WrappedCard judge in next_judge)
                            {
                                if (judge.Name == Lightning.ClassName)
                                {
                                    if (for_light == null || for_light.Count == 0)
                                        break;
                                    else
                                    {
                                        int id = -1;
                                        foreach (int i in for_light)
                                        {
                                            if (judge_for_next.Contains(i))
                                                continue;

                                            if ((for_indu == null || !for_indu.Contains(i)) && (for_supply == null || !for_supply.Contains(i)))
                                            {
                                                id = i;
                                                break;
                                            }
                                            else if (for_indu != null && for_indu.Contains(i) && for_indu.Count > 1)
                                            {
                                                id = i;
                                                break;
                                            }
                                            else if (for_supply != null && for_supply.Contains(i) && for_supply.Count > 1)
                                            {
                                                id = i;
                                                break;
                                            }
                                        }
                                        if (id >= 0)
                                            judge_for_next.Add(id);
                                    }
                                }
                                else if (judge.Name == Indulgence.ClassName)
                                {
                                    if (for_indu.Count == 0)
                                        break;
                                    else
                                    {
                                        judge_for_next.Add(for_indu[0]);
                                    }
                                }
                                else
                                {
                                    if (for_supply.Count == 0)
                                        break;
                                    else
                                    {
                                        judge_for_next.Add(for_supply[0]);
                                    }
                                }
                            }

                            if (next_judge[0].Name == Lightning.ClassName && judge_for_next.Count == 1)
                                option = true;
                        }
                        else if (IsEnemy(next))
                        {
                            //仅为敌人判断闪电
                            List<int> for_light = null;
                            foreach (WrappedCard judge in next_judge)
                            {
                                if (judge.Name == Lightning.ClassName)
                                {
                                    DamageStruct damage = new DamageStruct(judge, null, next, 3, DamageStruct.DamageNature.Thunder);
                                    if (GetDamageScore(damage).Score > 4)
                                    {
                                        for_light = new List<int>();
                                        foreach (int id in lest)
                                        {
                                            WrappedCard card = room.GetCard(id);
                                            int number = card.Number;
                                            WrappedCard.CardSuit suit = card.Suit;
                                            if (suit == WrappedCard.CardSuit.Spade && HasSkill("hongyan|hongyan_jx", next))
                                                suit = WrappedCard.CardSuit.Heart;

                                            if (number > 1 && number <= 9 && suit == WrappedCard.CardSuit.Spade)
                                                for_light.Add(id);
                                        }
                                    }
                                }
                            }

                            if (for_light != null && for_light.Count > 0)
                            {
                                for (int i = 0; i < next_judge.Count; i++)
                                {
                                    WrappedCard judge = next_judge[i];
                                    if (judge.Name == Lightning.ClassName)
                                    {
                                        judge_for_next.Add(for_light[0]);
                                        break;
                                    }
                                    else
                                    {
                                        if (next_judge[i + 1].Name == Lightning.ClassName && for_light.Count > 1)
                                        {
                                            judge_for_next.Add(for_light[0]);
                                            for_light.RemoveAt(0);
                                        }
                                        else
                                        {
                                            foreach (int id in lest)
                                            {
                                                if (!for_light.Contains(id))
                                                {
                                                    judge_for_next.Add(id);
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            up.AddRange(judge_remove);
            if (judge_for_next.Count > 0 && !option)
            {
                up.AddRange(best);
                up.AddRange(judge_for_next);
                cards.RemoveAll(t => up.Contains(t));
                move.Top = up;
                move.Bottom = new List<int>(cards);
                return move;
            }

            foreach (int id in best)
            {
                double _value = GetUseValue(id, self, Place.PlaceHand);
                WrappedCard card = room.GetCard(id);
                //room.OutPut(string.Format("观星判断{0}的价值是{1}", card.Name, _value));
                if (_value > 4)
                {
                    up.Add(id);
                }
                else
                    break;
            }
            cards.RemoveAll(t => up.Contains(t));
            move.Top = up;
            move.Bottom = new List<int>(cards);
            return move;
        }

        public AskForMoveCardsStruct GuanxingForNext(List<int> cards)
        {
            List<int> judge_for_next = new List<int>();
            Player next = room.GetNextAlive(self, 1, false);
            int count = 1;
            while (!next.FaceUp && count < room.AliveCount())
            {
                count++;
                next = room.GetNextAlive(next, 1, false);
            }

            List<int> lest = new List<int>(cards);

            bool skip = false;
            if (HasSkill("luoshen|luoshen_jx", next))
            {
                skip = true;
                if (next.GetPile("incantation").Count == 0)
                {
                    if (IsFriend(next))
                    {
                        foreach (int id in lest)
                            if (WrappedCard.IsBlack(room.GetCard(id).Suit))
                                judge_for_next.Add(id);
                    }
                    else
                    {
                        foreach (int id in lest)
                        {
                            if (WrappedCard.IsRed(room.GetCard(id).Suit))
                            {
                                judge_for_next.Add(id);
                                break;
                            }
                        }
                    }
                }
            }
            
            if (!skip)
            {
                List<WrappedCard> next_judge = new List<WrappedCard>();
                for (int i = next.JudgingArea.Count - 1; i >= 0; i--)
                {
                    WrappedCard card = room.GetCard(next.JudgingArea[i]);
                    next_judge.Add(card);
                }

                if (next_judge.Count > 0 && next.GetPile("incantation").Count > 0) //被张宝咒缚
                    next_judge.RemoveAt(0);

                //清除判不到的延时锦囊
                int judge_count = next_judge.Count;
                for (int i = judge_count; i > lest.Count; i--)
                {
                    next_judge.RemoveAt(i - 1);
                }

                if (next_judge.Count > 0)
                {
                    if (IsFriend(next))
                    {
                        List<int> for_light = null, for_indu = null, for_supply = null;
                        foreach (WrappedCard judge in next_judge)
                        {
                            if (judge.Name == Lightning.ClassName)
                            {
                                DamageStruct damage = new DamageStruct(judge, null, next, 3, DamageStruct.DamageNature.Thunder);
                                if (GetDamageScore(damage).Score < 0)
                                {
                                    for_light = new List<int>();
                                }
                            }
                            else if (judge.Name == Indulgence.ClassName)
                                for_indu = new List<int>();
                            else
                                for_supply = new List<int>();
                        }

                        foreach (int id in lest)
                        {
                            WrappedCard card = room.GetCard(id);
                            int number = card.Number;
                            WrappedCard.CardSuit suit = card.Suit;
                            if (suit == WrappedCard.CardSuit.Spade && HasSkill("hongyan|hongyan_jx", next))
                                suit = WrappedCard.CardSuit.Heart;

                            if (for_light != null)
                            {
                                if (number == 1 || number > 9 || suit != WrappedCard.CardSuit.Spade)
                                    for_light.Add(id);
                            }
                            else if (for_supply != null && suit == WrappedCard.CardSuit.Club)
                                for_supply.Add(id);
                            else if (for_indu != null && suit == WrappedCard.CardSuit.Heart)
                                for_indu.Add(id);
                        }

                        foreach (WrappedCard judge in next_judge)
                        {
                            if (judge.Name == Lightning.ClassName)
                            {
                                if (for_light == null || for_light.Count == 0)
                                {
                                    skip = true;
                                    break;
                                }
                                else
                                {
                                    int id = -1;
                                    foreach (int i in for_light)
                                    {
                                        if (judge_for_next.Contains(i))
                                            continue;

                                        if ((for_indu == null || !for_indu.Contains(i)) && (for_supply == null || !for_supply.Contains(i)))
                                        {
                                            id = i;
                                            break;
                                        }
                                        else if (for_indu != null && for_indu.Contains(i) && for_indu.Count > 1)
                                        {
                                            id = i;
                                            break;
                                        }
                                        else if (for_supply != null && for_supply.Contains(i) && for_supply.Count > 1)
                                        {
                                            id = i;
                                            break;
                                        }
                                    }
                                    if (id >= 0)
                                        judge_for_next.Add(id);
                                }
                            }
                            else if (judge.Name == Indulgence.ClassName)
                            {
                                if (for_indu.Count == 0)
                                {
                                    skip = true;
                                    break;
                                }
                                else
                                {
                                    judge_for_next.Add(for_indu[0]);
                                }
                            }
                            else
                            {
                                if (for_supply.Count == 0)
                                {
                                    skip = true;
                                    break;
                                }
                                else
                                {
                                    judge_for_next.Add(for_supply[0]);
                                }
                            }
                        }
                    }
                    else if (IsEnemy(next))
                    {
                        //仅为敌人判断闪电
                        List<int> for_light = null;
                        foreach (WrappedCard judge in next_judge)
                        {
                            if (judge.Name == Lightning.ClassName)
                            {
                                DamageStruct damage = new DamageStruct(judge, null, next, 3, DamageStruct.DamageNature.Thunder);
                                if (GetDamageScore(damage).Score > 4)
                                {
                                    for_light = new List<int>();
                                    foreach (int id in lest)
                                    {
                                        WrappedCard card = room.GetCard(id);
                                        int number = card.Number;
                                        WrappedCard.CardSuit suit = card.Suit;
                                        if (suit == WrappedCard.CardSuit.Spade && HasSkill("hongyan|hongyan_jx", next))
                                            suit = WrappedCard.CardSuit.Heart;

                                        if (number > 1 && number <= 9 && suit == WrappedCard.CardSuit.Spade)
                                            for_light.Add(id);
                                    }
                                }
                            }
                        }

                        if (for_light != null && for_light.Count > 0)
                        {
                            for (int i = 0; i < next_judge.Count; i++)
                            {
                                WrappedCard judge = next_judge[i];
                                if (judge.Name == Lightning.ClassName)
                                {
                                    judge_for_next.Add(for_light[0]);
                                    break;
                                }
                                else
                                {
                                    if (next_judge[i + 1].Name == Lightning.ClassName && for_light.Count > 1)
                                    {
                                        judge_for_next.Add(for_light[0]);
                                        for_light.RemoveAt(0);
                                    }
                                    else
                                    {
                                        foreach (int id in lest)
                                        {
                                            if (!for_light.Contains(id))
                                            {
                                                judge_for_next.Add(id);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            AskForMoveCardsStruct result = new AskForMoveCardsStruct
            {
                Success = true,
                Top = new List<int>(),
                Bottom = new List<int>()
            };
            if (judge_for_next.Count > 0)
            {
                result.Top = judge_for_next;
                lest.RemoveAll(t => judge_for_next.Contains(t));

                if (skip)
                {
                    result.Bottom = lest;
                }
                else if (cards.Count > judge_for_next.Count)
                {
                    if (IsFriend(next))
                    {
                        foreach (int id in lest)
                            if (GetUseValue(id, next, Place.PlaceHand) >= 6)
                                result.Top.Add(id);

                        lest.RemoveAll(t => result.Top.Contains(t));
                        result.Bottom = lest;
                    }
                    else
                    {
                        foreach (int id in lest)
                            if (GetUseValue(id, next, Place.PlaceHand) < 3)
                                result.Top.Add(id);

                        lest.RemoveAll(t => result.Top.Contains(t));
                        result.Bottom = lest;
                    }
                }
            }
            else
            {
                if (skip)
                {
                    result.Top = new List<int>();
                    result.Bottom = new List<int>(cards);
                }
                else
                {
                    result.Top = new List<int>();
                    if (IsFriend(next))
                    {
                        foreach (int id in cards)
                            if (GetUseValue(id, next, Place.PlaceHand) >= 6)
                                result.Top.Add(id);

                        lest.RemoveAll(t => result.Top.Contains(t));
                        result.Bottom = lest;
                    }
                    else
                    {
                        foreach (int id in cards)
                            if (GetUseValue(id, next, Place.PlaceHand) < 3)
                                result.Top.Add(id);

                        lest.RemoveAll(t => result.Top.Contains(t));
                        result.Bottom = lest;
                    }
                }
            }

            Debug.Assert(result.Top.Count + result.Bottom.Count == cards.Count);

            return result;
        }

        public virtual bool CardAskNullifilter(DamageStruct damage)
        {
            damage.Damage = DamageEffect(damage, DamageStruct.DamageStep.Done);
            damage.Steped = DamageStruct.DamageStep.Done;
            ScoreStruct score = GetDamageScore(damage);
            if (score.Score > 0) return true;

            if (damage.Damage > 0 && score.Score < -2 && HasSkill("tianxiang", damage.To))
            {
                SkillEvent tianxiang = Engine.GetSkillEvent("tianxiang");
                if (tianxiang != null)
                {
                    CardUseStruct tianxiang_use = tianxiang.OnResponding(this, damage.To, "@@tianxiang", string.Empty, damage);
                    if (tianxiang_use.Card != null && tianxiang_use.To != null && tianxiang_use.To.Count > 0)
                        return true;
                }
            }
            else if (damage.Damage == 0)
                return true;

            return false;
        }

        public virtual double GetEquipPriorityAdjust(WrappedCard card)
        {
            double value = 0;
            FunctionCard fcard = Engine.GetFunctionCard(card.Name);
            if (fcard is EquipCard)
            {
                if (card.Name == SilverLion.ClassName && self.IsWounded())
                    value += 2;

                if (fcard is Treasure && ((self.HasTreasure(WoodenOx.ClassName) && !self.HasUsed(WoodenOxCard.ClassName) && FriendNoSelf.Count > 0)
                    || (self.HasTreasure(LuminouSpearl.ClassName)) && !self.HasUsed("ZhihengCard")))
                {
                    value -= 9;
                }
                else if (HasSkill(LoseEquipSkill))
                {
                    value += 9;
                    if (card.Name == DragonCarriage.ClassName)
                        value -= 2;
                }
                else if (GetSameEquip(card, self) != null)
                {
                    if (((HasSkill("zhiheng") && WillShowForAttack()) || self.HasTreasure(LuminouSpearl.ClassName)) && !self.HasUsed("ZhihengCard"))
                        value -= 15;
                    else if (!(fcard is Weapon))
                    {
                        value -= 3;
                    }
                    else if (fcard is Weapon && !self.HasUsed(Slash.ClassName))
                    {
                        value -= 1;
                    }
                }

                foreach (Player p in room.GetAlivePlayers())
                    if (HasSkill("diaodu", p) && RoomLogic.WillBeFriendWith(room, p, self, "diaodu"))
                        value += 1;
            }

            return value + (Number.ContainsKey(card.Name) ? Number[card.Name] : 0);
        }

        public virtual void UseEquipCard(ref CardUseStruct use, WrappedCard card)
        {
            //第一步，判断这张卡牌是否是当前最值得使用的（可被技能转化为其他卡牌）
            int id = card.Id;
            List<WrappedCard> cards = GetViewAsCards(self, id);
            Dictionary<WrappedCard, double> points = new Dictionary<WrappedCard, double>();
            foreach (WrappedCard c in cards)
            {
                points[c] = GetUseValue(c, self);
            }
            cards.Sort((x, y) => { return points[x] > points[y] ? -1 : 1; });
            foreach (WrappedCard c in cards)
            {
                if (c.Name == card.Name)
                    break;

                FunctionCard fcard = Engine.GetFunctionCard(c.Name);
                if (fcard.IsAvailable(room, self, c))
                {
                    CardUseStruct _use = new CardUseStruct(c, self, new List<Player>())
                    {
                        IsDummy = true
                    };
                    UseCard use_event = Engine.GetCardUsage(c.Name);
                    use_event.Use(this, self, ref _use, c);
                    if (_use.Card != null)
                        return;
                }
            }

            if (points[card] > 0)
            {
                //第二步，判断要替换的装备是否还具有使用价值（可被技能转化为其他卡牌）
                WrappedCard same = GetSameEquip(card, self);
                if (same != null)
                {
                    //木马要先用
                    if (same.Name == WoodenOx.ClassName && !self.HasUsed(WoodenOxCard.ClassName))
                    {
                        foreach (Player p in FriendNoSelf)
                            if (!p.GetTreasure() && RoomLogic.CanPutEquip(p, same))
                                return;
                    }
                    //有夜明珠要先用掉制衡
                    if (same.Name == LuminouSpearl.ClassName && !self.HasUsed("ZhihengCard"))
                        return;

                    List<WrappedCard> _cards = GetViewAsCards(self, same.Id);
                    Dictionary<WrappedCard, double> _points = new Dictionary<WrappedCard, double>();
                    foreach (WrappedCard c in cards)
                    {
                        _points[c] = GetUseValue(c, self);
                    }
                    cards.Sort((x, y) => { return _points[x] > _points[y] ? -1 : 1; });
                    foreach (WrappedCard c in _cards)
                    {
                        if (c.Name == same.Name)
                            break;

                        FunctionCard _fcard = Engine.GetFunctionCard(c.Name);
                        if (_fcard.IsAvailable(room, self, c))
                        {
                            CardUseStruct _use = new CardUseStruct(c, self, new List<Player>())
                            {
                                IsDummy = true
                            };
                            UseCard use_event = Engine.GetCardUsage(c.Name);
                            use_event.Use(this, self, ref _use, c);
                            if (_use.Card != null)
                                return;
                        }
                    }
                    //如果需替换的是连弩则先用光杀
                    if (same.Name == CrossBow.ClassName)
                    {
                        List<WrappedCard> slashes = GetCards(Slash.ClassName, self);
                        if (slashes.Count > 1 || !RoomLogic.CanSlashWithoutCrossBow(room, self))
                        {
                            UseCard slash_event = Engine.GetCardUsage(Slash.ClassName);
                            CardUseStruct _use = new CardUseStruct(null, self, new List<Player>())
                            {
                                IsDummy = true
                            };
                            slash_event.Use(this, self, ref _use, null);
                            if (use.Card != null)
                                return;
                        }
                    }
                }
            }

            //第三步，获取改同位置的装配牌
            List<WrappedCard> sames = FindSameEquipCards(card);
            sames.Add(card);

            //如果白银狮子价值最高，则先用（通常是角色已受伤)
            List<double> values = SortByUseValue(ref sames);
            if (sames[0].Name == SilverLion.ClassName)
            {
                if (HasSkill("bazhen"))
                {
                    if (self.IsWounded() && (!self.GetArmor() || self.HasArmor(EightDiagram.ClassName))
                        && sames.Count == 1 && GetKnownCardsNums(AwaitExhausted.ClassName, "he", self) == 0)
                        return;
                    if (!self.IsWounded() && WillShowForDefence())
                        return;
                }

                use.Card = sames[0];
                Number[use.Card.Name] = 9;
                return;
            }
            //判断是否需要使用重复装备，如有则从价值最低的装备开始使用
            //1、小鸡等失去装备时有技能发动的
            //2、需要空城时装备卡手的
            //3、手牌溢出不留给二张的
            //4、有吕范能摸牌的
            //5、吕岱勤国
            //情况2和3时只从手牌中判断
            bool use_same = false;
            bool guzheng = false;
            Player erzhang = FindPlayerBySkill("guzheng");
            if (erzhang != null && erzhang != self && IsFriend(erzhang) || id_tendency[self] == "wu")
                guzheng = true;
            
            if (HasSkill("lirang") && FriendNoSelf.Count > 0)
                guzheng = true;
            if (HasSkill("weidi_jx"))
            {
                foreach (Player p in FriendNoSelf)
                {
                    if (p.Kingdom == "qun")
                    {
                        guzheng = true;
                        break;
                    }
                }
            }

            bool lv_fan = false;
            foreach (Player p in room.GetAlivePlayers())
            {
                if (HasSkill("diaodu", p) && RoomLogic.WillBeFriendWith(room, p, self, "diaodu"))
                {
                    lv_fan = true;
                    break;
                }
            }

            if (sames.Count > 1 && (HasSkill("zhijian_jx") || NeedKongcheng(self) || (!guzheng && GetOverflow(self) > 1) || lv_fan || HasSkill(LoseEquipSkill)))
                use_same = true;
            if (!use_same && HasSkill("qinguo"))
            {
                WrappedCard slash = new WrappedCard(Slash.ClassName);
                List<ScoreStruct> scores = CaculateSlashIncome(self, new List<WrappedCard> { slash }, null, false);
                if (scores.Count > 0 && scores[0].Score > 0) use_same = true;
            }

            if (!use_same)
            {
                WrappedCard same = GetSameEquip(card, self);
                double value = values[0];
                if (same != null)
                {
                    bool keep = false;
                    if (value < 1)      //如果替换的装备增加的价值不大于1，则保留
                        keep = true;
                    else if (GetOverflow(self) <= 0 && room.GetCardPlace(sames[0].GetEffectiveId()) == Place.PlaceHand)     //手牌不溢出，不是防具也保留
                    {
                        FunctionCard equip = Engine.GetFunctionCard(sames[0].Name);
                        if (!(equip is Armor))
                            keep = true;
                    }
                    if (keep)
                        return;
                }

                if (Engine.GetFunctionCard(sames[0].Name) is Armor && HasSkill("bazhen") && !WillShowForDefence())
                    value += 2;

                if (value > 0)
                {
                    use.Card = sames[0];
                    Number[use.Card.Name] = 0;
                }
            }
            else
            {
                SortByUseValue(ref sames, false);
                use.Card = sames[0];
                Number[use.Card.Name] = 0;
                if (GetSameEquip(use.Card, self) != null && !lv_fan && !HasSkill(LoseEquipSkill))
                    Number[use.Card.Name] = -5;
            }
        }

        public List<WrappedCard> FindSameEquipCards(WrappedCard card, bool hand_only = false, bool sort = true)
        {
            List<WrappedCard> result = new List<WrappedCard>();
            FunctionCard fcard = Engine.GetFunctionCard(card.Name);
            if (fcard is EquipCard equip)
            {
                List<int> ids = self.GetCards("h");
                if (!hand_only)
                    ids.AddRange(self.GetHandPile());

                List<int> equips = new List<int>();
                foreach (int id in ids)
                {
                    if (id == card.Id) continue;
                    WrappedCard c = room.GetCard(id);
                    FunctionCard _fcard = Engine.GetFunctionCard(c.Name);
                    if (_fcard is EquipCard _equip)
                    {
                        if (_equip.EquipLocation() == equip.EquipLocation()
                            || (equip.EquipLocation() == EquipCard.Location.SpecialLocation &&
                            (_equip.EquipLocation() == EquipCard.Location.DefensiveHorseLocation || _equip.EquipLocation() == EquipCard.Location.OffensiveHorseLocation)))
                            equips.Add(id);
                    }
                }

                if (sort)
                {
                    foreach (int id in equips)
                    {
                        WrappedCard _card = room.GetCard(id);
                        List<WrappedCard> cards = GetViewAsCards(self, id);
                        Dictionary<WrappedCard, double> points = new Dictionary<WrappedCard, double>();
                        foreach (WrappedCard c in cards)
                        {
                            points[c] = GetUseValue(c, self);
                        }
                        cards.Sort((x, y) => { return points[x] > points[y] ? -1 : 1; });
                        foreach (WrappedCard c in cards)
                        {
                            if (c.Name == _card.Name)
                            {
                                result.Add(c);
                                break;
                            }

                            FunctionCard _fcard = Engine.GetFunctionCard(c.Name);
                            if (_fcard.IsAvailable(room, self, c))
                            {
                                CardUseStruct _use = new CardUseStruct(c, self, new List<Player>())
                                {
                                    IsDummy = true
                                };
                                UseCard use_event = Engine.GetCardUsage(c.Name);
                                use_event.Use(this, self, ref _use, c);
                                if (_use.Card != null)
                                    break;
                            }
                        }
                    }
                }

                foreach (int id in equips)
                    result.Add(room.GetCard(id));
            }

            return result;
        }

        public double AjustWeaponRangeValue(Player player, WrappedCard card)
        {
            FunctionCard fcard = Engine.GetFunctionCard(card.Name);
            double value = 0;
            if (fcard is Weapon weapon)
            {
                int range = weapon.Range;
                foreach (Player p in GetEnemies(player))
                {
                    if (range < RoomLogic.DistanceTo(room, player, p, null, true))
                    {
                        if (IsFriend(player) || IsFriend(p))
                        {
                            value += 0.3;
                            if (GetPublicPossibleId(p).Count == 1)
                                value += 0.2;
                        }
                    }
                }
            }


            return value;
        }

        public double AjudstDHorseValue(Player player, WrappedCard card, Place place)
        {
            double value = 0;
            if (place != Place.PlaceEquip && (player.GetDefensiveHorse() || player.GetSpecialEquip())) return value;
            if (place != Place.PlaceEquip)
            {
                foreach (Player p in GetEnemies(player))
                {
                    if (RoomLogic.InMyAttackRange(room, p, player) && !RoomLogic.InMyAttackRange(room, p, player, null, 1))
                    {
                        if (IsFriend(player) || IsFriend(p))
                            value += 0.65;
                    }

                    if (RoomLogic.DistanceTo(room, p, player, null, true) == 1)
                        if (IsFriend(player) || IsFriend(p))
                            value += 1;
                }
            }
            else
            {
                foreach (Player p in GetEnemies(player))
                {
                    if (!RoomLogic.InMyAttackRange(room, p, player) && RoomLogic.InMyAttackRange(room, p, player, null, -1))
                    {
                        if (IsFriend(player) || IsFriend(p))
                            value += 0.65;
                    }

                    if (RoomLogic.DistanceTo(room, p, player, null, true) == 2)
                        if (IsFriend(player) || IsFriend(p))
                            value += 1;
                }
            }

            return value;
        }

        public double AjustOHorseValue(Player player, WrappedCard card, Place place)
        {
            double value = 0;
            if (place != Place.PlaceEquip && (player.GetOffensiveHorse() || player.GetSpecialEquip())) return value;
            if (place != Place.PlaceEquip)
            {
                foreach (Player p in GetEnemies(player))
                {
                    if (!RoomLogic.InMyAttackRange(room, player, p) && RoomLogic.InMyAttackRange(room, player, p, null, -1))
                    {
                        if (IsFriend(player) || IsFriend(p))
                            value += 0.5;
                    }

                    if (RoomLogic.DistanceTo(room, p, player, null, true) == 1 && RoomLogic.DistanceTo(room, p, player, card, true) > 1)
                        if (IsFriend(player) || IsFriend(p))
                            value += 0.4;
                }
            }

            if (player == self && FindSameEquipCards(card, false, false).Count == 0 && HasCrossbowEffect(player))
            {
                foreach (Player p in GetEnemies(player))
                    if (RoomLogic.DistanceTo(room, player, p, null, true) == 2)
                        value += 0.1;
            }

            return value;
        }

        public bool IsPriorFriendOfSlash(Player friend, WrappedCard card, Player source)
        {
            source = source ?? Self;
            if (HasSkill("zhiman|zhiman_jx", source) && FindCards2Discard(source, friend, string.Empty, "ej", HandlingMethod.MethodGet).Score >= 0)
                return true;

            if (card.Name.Contains(Slash.ClassName) && card.Name != Slash.ClassName && friend.Chained)
            {
                DamageStruct damage = new DamageStruct(card, source, friend);
                if (card.Name.Contains("Fire"))
                    damage.Nature = DamageStruct.DamageNature.Fire;
                else if (card.Name.Contains("Thunder"))
                    damage.Nature = DamageStruct.DamageNature.Thunder;

                damage.Damage = DamageEffect(damage, DamageStruct.DamageStep.None);

                if (ChainDamage(damage) >= 0)
                    return true;
            }

            return false;
        }

        public void SetPreDrink(WrappedCard card)
        {
            if (card != null)
                pre_drink = card;
        }

        public void RemovePreDrink()
        {
            pre_drink = null;
        }

        public Player AddExtraSlashTarget(List<Player> targets, CardUseStruct use, List<Player> selected = null)
        {
            selected = selected ?? new List<Player>();
            WrappedCard card = use.Card;
            List<Player> current_targets = new List<Player>(use.To);
            current_targets.AddRange(selected);
            List<Player> result = new List<Player>();

            Dictionary<Player, ScoreStruct> available_targets = new Dictionary<Player, ScoreStruct>();
            foreach (Player target in targets)
            {
                if (!RoomLogic.CanSlash(room, self, target, card, 0, current_targets) || SlashProhibit(card, target, self)) continue;
                available_targets.Add(target, SlashIsEffective(card, target));
            }

            bool chain = false;
            foreach (Player p in current_targets)
            {
                if (p.Chained)
                {
                    chain = true;
                    break;
                }
            }
            List<ScoreStruct> scores = new List<ScoreStruct>();
            foreach (Player target in available_targets.Keys)
            {
                double use_value = 0;

                ScoreStruct score = available_targets[target];
                if (score.Score == 0) continue;
                use_value += score.Score;
                foreach (string skill in GetKnownSkills(target))
                {
                    SkillEvent ev = Engine.GetSkillEvent(skill);
                    if (ev != null)
                        use_value += ev.TargetValueAdjust(this, card, self, current_targets, target);
                }

                if (IsFriend(target) && target.Chained && chain && score.Damage.Damage > 0)
                        use_value -= 10;

                if (score.DoDamage)
                {
                    double chain_value = ChainDamage(score.Damage);
                    if (IsSpreadStarter(score.Damage) && score.Info != "miss" && score.Rate > 0.5 && !chain)
                    {
                        chain = true;
                        use_value += chain_value;
                    }
                }

                ScoreStruct result_score = new ScoreStruct
                {
                    Card = card,
                    Players = new List<Player> { target },
                    Score = use_value
                };

                scores.Add(result_score);
            }

            CompareByScore(ref scores);
            if (scores.Count > 0 && scores[0].Score > 0)
            {
                return scores[0].Players[0];
            }

            return null;
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
            Shuffle.shuffle(ref choices);
            return choices[0];
        }
        public virtual List<int> AskForDiscard(List<int> ids, string reason, int discard_num, int min_num, bool optional)
        {
            List<int> to_discard = new List<int>();
            if (optional)
                return to_discard;
            else
                return room.ForceToDiscard(self, ids, discard_num, self.HasFlag("Global_AIDiscardExchanging"));
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
        public virtual WrappedCard AskForNullification(CardEffectStruct effect, bool positive, CardEffectStruct real) => null;
        public virtual int AskForCardChosen(Player who, string flags, string reason, HandlingMethod method, List<int> disabled_ids) => -1;
        // virtual List<int> AskForCardsChosen(List<Player> targets, string flags, string reason, int min, int max, List<int> disabled_ids) => new List<int>();
        public virtual WrappedCard AskForCard(string reason, string pattern, string prompt, object data)
        {
            if (!pattern.Contains(Slash.ClassName) && !pattern.Contains(Jink.ClassName) && !pattern.Contains(Peach.ClassName) && !pattern.Contains(Analeptic.ClassName))
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

            if (result != null && result.Name == Peach.ClassName && room.BloodBattle && room.Setting.GameMode == "Hegemony")
            {
                WrappedCard slash = new WrappedCard(Slash.ClassName);
                slash.AddSubCard(result);
                slash = RoomLogic.ParseUseCard(room, slash);
                if (Engine.MatchExpPattern(room, pattern, self, slash))
                    return slash;
                else
                {
                    WrappedCard jink = new WrappedCard(Jink.ClassName);
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
            if (reason == AmazingGrace.ClassName)
            {
                double best = -1000;
                int result = -1;
                foreach (int id in card_ids)
                {
                    double value = GetUseValue(id, self, Player.Place.PlaceHand);
                    if (value > best)
                    {
                        best = value;
                        result = id;
                    }
                }
                if (best > 0 && result != -1)
                    return result;
            }

            SkillEvent e = Engine.GetSkillEvent(reason);
            if (e != null)
                return e.OnPickAG(this, self, card_ids, refusable);

            if (refusable)
                return -1;

            Shuffle.shuffle(ref card_ids);
            return card_ids[0];
        }
        public virtual WrappedCard AskForCardShow(Player requestor, string reason, object data) => room.GetCard(room.GetRandomHandCard(self));
        public virtual WrappedCard AskForPindian(Player requestor, List<Player> to, string reason)
        {
            SkillEvent e = Engine.GetSkillEvent(reason);
            if (e != null)
            {
                WrappedCard card = e.OnPindian(this, requestor, to);
                if (card != null)
                    return card;
            }

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
        public virtual WrappedCard AskForSinglePeach(Player dying, DyingStruct dying_struct)
        {
            if (self == dying)
            {
                List<WrappedCard> cards = RoomLogic.GetPlayerHandcards(room, self);
                List<int> piles = self.GetHandPile();
                foreach (int id in piles)
                    cards.Add(room.GetCard(id));

                FunctionCard f_ana = Analeptic.Instance;
                FunctionCard f_peach = Peach.Instance;
                foreach (WrappedCard card in cards)
                {
                    if (card.Name == Analeptic.ClassName && f_ana.IsAvailable(room, self, card) && Engine.IsProhibited(room, self, dying, card) == null)
                        return card;
                }

                foreach (WrappedCard card in cards)
                {
                    if (card.Name == Peach.ClassName && !room.BloodBattle && f_peach.IsAvailable(room, self, card) && Engine.IsProhibited(room, self, dying, card) == null)
                        return card;
                }
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
                return room.ForceToExchange(self, min_num, pattern, expand_pile);
        }
    }
}
