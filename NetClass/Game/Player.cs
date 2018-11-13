using CommonClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommonClass.Game
{
    public class Player
    {
        public enum PlayerPhase
        {
            RoundStart, Start, Judge, Draw, Play, Discard, Finish, NotActive, PhaseNone
        };
        public enum Place
        {
            PlaceHand, PlaceEquip, PlaceDelayedTrick, PlaceJudge,
            PlaceSpecial, DiscardPile, DrawPile, PlaceTable, PlaceUnknown,
            PlaceWuGu, DrawPileBottom
        };
        public enum PlayerRole
        {
            Lord, Loyalist, Rebel, Renegade
        };

        public enum Gender
        {
            Sexless, Male, Female, Neuter
        };
        public string Name { set; get; }
        public int ClientId { set; get; }
        public string SceenName { set; get; }
        public Dictionary<string, int> Marks { set; get; } = new Dictionary<string, int>();
        public Dictionary<string, List<int>> Piles { set; get; } = new Dictionary<string, List<int>>();
        public Dictionary<string, List<string>> PileOpen { set; get; } = new Dictionary<string, List<string>>();
        public List<string> HeadAcquiredSkills { set; get; } = new List<string>();
        public List<string> DeputyAcquiredSkills { set; get; } = new List<string>();
        public Dictionary<string, bool> HeadSkills { set; get; } = new Dictionary<string, bool>();
        public Dictionary<string, bool> DeputySkills { set; get; } = new Dictionary<string, bool>();
        public List<string> Flags { set; get; } = new List<string>();
        public Dictionary<string, int> History { set; get; } = new Dictionary<string, int>();
        public string General1 { set; get; }
        public string General2 { set; get; }
        public int HeadSkinId { set; get; } = 0;
        public int DeputySkinId { set; get; } = 0;
        public bool Dual { set; get; } = true;
        public Gender PlayerGender { set; get; }
        public int Hp { set; get; }
        public int MaxHp {
            get => _maxHp;
            set {
                _maxHp = value;
                if (_maxHp < Hp)
                    Hp = _maxHp;
            }
        }
        public string Kingdom { set; get; }
        public string Role { set; get; }
        public bool RoleShown { set; get; } = false;
        public string Status { set; get; }
        public int Seat { set; get; }
        public bool Alive { set; get; } = true;
        public Game3v3Camp Camp { set; get; } = Game3v3Camp.S_CAMP_NONE;
        public bool General1Showed { set; get; } = false;
        public bool General2Showed { set; get; } = false;
        public PlayerPhase Phase { set; get; } = PlayerPhase.NotActive;
        public KeyValuePair<int, string> Weapon { set; get; } = new KeyValuePair<int, string>(-1, null);
        public KeyValuePair<int, string> Armor { set; get; } = new KeyValuePair<int, string>(-1, null);
        public KeyValuePair<int, string> DefensiveHorse { set; get; } = new KeyValuePair<int, string>(-1, null);
        public KeyValuePair<int, string> OffensiveHorse { set; get; } = new KeyValuePair<int, string>(-1, null);
        public KeyValuePair<int, string> Treasure { set; get; } = new KeyValuePair<int, string>(-1, null);
        public bool FaceUp { set; get; } = true;
        public bool Chained { set; get; } = false;
        public bool Removed { set; get; } = false;
        public List<int> JudgingArea { set; get; } = new List<int>();
        public List<string> DisableShow { set; get; } = new List<string>();
        public bool ScenarioRoleShown { set; get; } = false;
        public int HandcardNum { get { return HandCards.Count(); } }
        public List<int> HandCards { set; get; } = new List<int>();
        public List<int> KnownCards { set; get; } = new List<int>();
        public List<int> VisibleCards { set; get; } = new List<int>();
        public string DuanChang { set; get; } = string.Empty;
        public string ActualGeneral1 { get; set; }
        public string ActualGeneral2 { get; set; }
        public string Next { set; get; }
        public List<PlayerPhase> Phases { set; get; } = new List<PlayerPhase>();
        public int PhasesIndex { set; get; }
        public List<PhaseStruct> PhasesState { set; get; } = new List<PhaseStruct>();
        public Dictionary<int, List<string>> Limitation { get; set; } = new Dictionary<int, List<string>>();

        private int _maxHp;
        private Dictionary<string, object> tag = new Dictionary<string, object>();

        public object GetTag(string key)
        {
            if (tag.ContainsKey(key))
                return tag[key];

            return null;
        }

        //绝对不能给Player类设置class类的tag
        //否则json打包player信息给客户端时会出错
        public void SetTag(string key, object data)
        {
            tag[key] = data;
        }

        public void RemoveTag(string key)
        {
            if (tag.ContainsKey(key))
                tag.Remove(key);
        }

        public bool ContainsTag(string key) => tag.ContainsKey(key);

        public int GetLostHp()
        {
            return MaxHp - Math.Max(Hp, 0);
        }

        public bool IsWounded()
        {
            if (Hp < 0)
                return true;
            else
                return Hp < MaxHp;
        }

        public bool HasShownOneGeneral()
        {
            return General1Showed || General2Showed;
        }

        public bool IsMale()
        {
            return PlayerGender == Gender.Male;
        }

        public bool IsFemale()
        {
            return PlayerGender == Gender.Female;
        }

        public void SetDisableShow(string flags, string reason)
        {
            if (Flags.Contains("h"))
            {
                if (DisableShowList(true).Contains(reason))
                    return;
            }
            if (flags.Contains('d'))
            {
                if (DisableShowList(false).Contains(reason))
                    return;
            }

            string dis_str = flags + ',' + reason;
            DisableShow.Add(dis_str);
            //通知UI更新
        }
        public void RemoveDisableShow(string reason)
        {
            List<string> remove_list = new List<string>();
            foreach (string dis_str in DisableShow)
            {
                string dis_reason = dis_str.Split(',')[1];
                if (dis_reason == reason)
                    remove_list.Add(dis_str);
            }

            if (remove_list.Count() == 0) return;

            foreach (string to_remove in remove_list)
                DisableShow.Remove(to_remove);


            //通知UI更新
        }

        public List<string> DisableShowList(bool head)
        {
            string head_flag = "h";
            if (!head)
                head_flag = "d";

            List<string> r = new List<string>();
            foreach (string dis_str in DisableShow)
            {
                string[] dis_list = dis_str.Split(',');
                if (dis_list[0].Contains(head_flag))
                    r.Add(dis_list[0]);
            }

            return r;
        }

        public bool CanShowGeneral(string flags)
        {
            bool head = true, deputy = true;
            foreach (string dis_str in DisableShow)
            {
                string[] dis_list = dis_str.Split(',');
                if (dis_list[0].Contains("h")) head = false;
                if (dis_list[0].Contains("d")) deputy = false;
            }
            if (string.IsNullOrEmpty(flags)) return head || deputy || HasShownOneGeneral();
            if (flags == "h") return head || General1Showed;
            if (flags == "d") return deputy || General2Showed;
            if (flags == "hd") return (deputy || General2Showed) && (head || General1Showed);
            return false;
        }

        public bool HasShownAllGenerals()
        {
            return General1Showed && (string.IsNullOrEmpty(General2) || General2Showed);
        }

        public void SetFlags(string flag)
        {
            if (flag == ".")
            {
                Flags.Clear();
                return;
            }

            if (flag.StartsWith("-"))
            {
                string copy = flag.Substring(1);
                Flags.Remove(copy);
            }
            else
            {
                Flags.Add(flag);
            }
        }

        public bool GetWeapon() => Weapon.Key > -1;
        public bool GetArmor() => Armor.Key > -1;
        public bool GetDefensiveHorse() => DefensiveHorse.Key > -1;
        public bool GetOffensiveHorse() => OffensiveHorse.Key > -1;
        public bool GetTreasure() => Treasure.Key > -1;

        public List<int> GetCards(string flags)
        {
            List <int> cards = new List<int>();
            if (flags.Contains("h"))
                cards.AddRange(new List<int>(HandCards));
            if (flags.Contains("e"))
                    cards.AddRange(GetEquips());
            if (flags.Contains("j"))
                cards.AddRange(JudgingArea);

            return cards;
        }

        public bool HasFlag(string flag)
        {
            return Flags.Contains(flag);
        }

        public bool IsKongcheng()
        {
            return HandcardNum == 0;
        }

        public bool IsNude()
        {
            return IsKongcheng() && !HasEquip();
        }

        public bool HasEquip()
        {
            return Weapon.Key != -1 || Armor.Key != -1 || DefensiveHorse.Key != -1 || OffensiveHorse.Key != -1 || Treasure.Key != -1;
        }
        public bool HasEquip(int location)
        {
            switch (location)
            {
                case 0:
                    return GetWeapon();
                case 1:
                    return GetArmor();
                case 2:
                    return GetDefensiveHorse();
                case 3:
                    return GetOffensiveHorse();
                case 4:
                    return GetTreasure();
                default:
                    return false;
            }
        }

        public bool HasEquip(string name)
        {
            return (!string.IsNullOrEmpty(Weapon.Value) && Weapon.Value == name)
                || (!string.IsNullOrEmpty(Armor.Value) && Armor.Value == name)
                || (!string.IsNullOrEmpty(DefensiveHorse.Value) && DefensiveHorse.Value == name)
                || (!string.IsNullOrEmpty(OffensiveHorse.Value) && OffensiveHorse.Value == name)
                || (!string.IsNullOrEmpty(Treasure.Value) && Treasure.Value == name);
        }

        public List<int> GetEquips()
        {
            List<int> equips = new List<int>();
            if (Weapon.Key != -1)
                equips.Add(Weapon.Key);
            if (Armor.Key != -1)
                equips.Add(Armor.Key);
            if (DefensiveHorse.Key != -1)
                equips.Add(DefensiveHorse.Key);
            if (OffensiveHorse.Key != -1)
                equips.Add(OffensiveHorse.Key);
            if (Treasure.Key != -1)
                equips.Add(Treasure.Key);

            return equips;
        }

        public int GetEquip(int index)
        {
            switch (index) {
                case 0: return Weapon.Key;
                case 1: return Armor.Key;
                case 2: return DefensiveHorse.Key;
                case 3: return OffensiveHorse.Key;
                case 4: return Treasure.Key;
                default:
                    return -1;
            }
        }

        public bool HasWeapon(string weapon_name)
        {
            if (Weapon.Key == -1 || GetMark("Equips_Nullified_to_Yourself") > 0) return false;
            if (Weapon.Value == weapon_name) return true;

            return false;
        }
        public bool HasArmor(string name)
        {
            if (Armor.Key == -1 || GetMark("Equips_Nullified_to_Yourself") > 0) return false;
            if (Armor.Value == name) return true;

            return false;
        }

        public bool HasTreasure(string treasure_name)
        {
            if (Treasure.Key == -1 || GetMark("Equips_Nullified_to_Yourself") > 0) return false;
            if (Treasure.Value == treasure_name) return true;

            return false;
        }

        public bool IsDuanchang(bool head)
        {
            if (head && DuanChang.Split(',').Contains("head"))
                return true;
            else if (!head && DuanChang.Split(',').Contains("deputy"))
                return true;
            else
                return false;
        }

        public void AddMark(string mark, int add_num = 1)
        {
            if (Marks.ContainsKey(mark))
            {
                int value = Marks[mark];
                value += add_num;
                SetMark(mark, value);
            }
            else
                Marks.Add(mark, add_num);
        }

        public void RemoveMark(string mark, int remove_num = 1)
        {
            int value = Marks.ContainsKey(mark) ? Marks[mark] : 0;
            value -= remove_num;
            value = Math.Max(0, value);
            SetMark(mark, value);
        }

        public void SetMark(string mark, int value)
        {
            if (Marks.ContainsKey(mark))
            {
                if (Marks[mark] != value)
                    Marks[mark] = value;
            }
            else
                Marks.Add(mark, value);
        }

        public int GetMark(string mark)
        {
            if (Marks.ContainsKey(mark))
                return Marks[mark];
            return 0;
        }

        public int GetCardCount(bool include_equip)
        {
            int count = HandcardNum;
            if (include_equip)
            {
                if (Weapon.Key != -1) count++;
                if (Armor.Key != -1) count++;
                if (DefensiveHorse.Key != -1) count++;
                if (OffensiveHorse.Key != -1) count++;
                if (Treasure.Key != -1) count++;
            }
            return count;
        }

        public List<int> GetPile(string pile_name)
        {
            if (Piles.ContainsKey(pile_name))
                return new List<int>(Piles[pile_name]);

            return new List<int>();
        }

        public List<string> GetPileNames()
        {
            List<string> names = new List<string>();
            foreach (string pile_name in Piles.Keys)
                names.Add(pile_name);

            return names;
        }

        public string GetPileName(int card_id)
        {
            foreach (string pile_name in Piles.Keys)
            {
                List<int> pile = Piles[pile_name];
                if (pile.Contains(card_id))
                    return pile_name;
            }

            return string.Empty; ;
        }

        public List<int> GetHandPile(bool include_wooden_ox = true)
        {
            List<int> result = new List<int>();
            foreach (string pile in GetHandPileList(false))
            {
                if (pile == "wooden_ox" && !include_wooden_ox) continue;
                foreach (int id in GetPile(pile))
                    result.Add(id);
            }

            return result;
        }

        public List<string> GetHandPileList(bool view_as_skill = true)
        {
            List<string> handlist = new List<string>();
            if (view_as_skill)
                handlist.Add("hand");
            foreach (string pile in GetPileNames())
            {
                if (pile.StartsWith("&") || pile == "wooden_ox" || pile.StartsWith("^"))
                    handlist.Add(pile);
            }

            return handlist;
        }

        public bool PileOpenList(string pile_name, string player)
        {
            return PileOpen.ContainsKey(pile_name) && PileOpen[pile_name].Contains(player);
        }

        public void SetPileOpen(string pile_name, string player)
        {
            if (PileOpen.ContainsKey(pile_name) && PileOpen[pile_name].Contains(player)) return;
            if (PileOpen.ContainsKey(pile_name))
                PileOpen[pile_name].Add(player);
            else
                PileOpen[pile_name] = new List<string> { player };
        }

        public void AddHistory(string name, int times = 1)
        {
            if (name == ".")
                ClearHistory();
            else if (times == 0)
                ClearHistory(name);
            else if (History.ContainsKey(name))
                History[name] += times;
            else if (times >= 0)
            {
                History[name] = times;
            }
        }

        public int GetSlashCount()
        {
            return (History.ContainsKey("Slash") ? History["Slash"] : 0)
                + (History.ContainsKey("ThunderSlash") ? History["ThunderSlash"] : 0)
                + (History.ContainsKey("FireSlash") ? History["FireSlash"] : 0);
        }

        public void ClearHistory(string name = null)
        {
            if (string.IsNullOrEmpty(name))
            {                                   //analeptic must be deleted manually
                Dictionary<string, int> history_copy = History;
                foreach (string key in new List<string>(history_copy.Keys))
                    if (!key.Contains("Analeptic"))
                        History[key] = 0;
            }
            else
            {
                History[name] = 0;
                Dictionary<string, int> history_copy = History;
                if (name == "Analeptic")
                    foreach (string key in new List<string>(history_copy.Keys))
                        if (key.Contains("Analeptic"))
                            History[key] = 0;
            }
        }

        public bool HasUsed(string card_class)
        {
            if (History.ContainsKey(card_class))
                return History[card_class] > 0;

            return false;
        }

        public int UsedTimes(string card_class)
        {
            if (History.ContainsKey(card_class))
                return History[card_class];

            return 0;
        }

        public bool HasPreshowedSkill(string name, bool head)
        {
            if (head && HeadSkills.ContainsKey(name))
                return HeadSkills[name];
            else if (!head && DeputySkills.ContainsKey(name))
                return DeputySkills[name];

            return false;
        }

        public void SetSkillPreshowed(string skill, bool preshowed = true, bool head = true)
        {
            if (head && HeadSkills.ContainsKey(skill))
                HeadSkills[skill] = preshowed;
            else if (!head && DeputySkills.ContainsKey(skill))
                DeputySkills[skill] = preshowed;
        }

        public void AcquireSkill(string skill_name, bool head = true)
        {
            if (head)
                HeadAcquiredSkills.Add(skill_name);
            else
                DeputyAcquiredSkills.Add(skill_name);
        }

        public void DetachSkill(string skill_name, bool head = true)
        {
            if (head)
                HeadAcquiredSkills.Remove(skill_name);
            else
                DeputyAcquiredSkills.Remove(skill_name);
        }

        public void DetachAllSkills()
        {
            HeadAcquiredSkills.Clear();
            DeputyAcquiredSkills.Clear();
        }

        public void SetSkillsPreshowed(string flags = "hd", bool preshowed = true)
        {
            if (flags.Contains("h"))
            {
                foreach (string skill in new List<string>(HeadSkills.Keys))
                {
                    HeadSkills[skill] = preshowed;
                }
            }
            if (flags.Contains("d"))
            {
                foreach (string skill in new List<string>(DeputySkills.Keys))
                {
                    DeputySkills[skill] = preshowed;
                }
            }
        }

        public bool OwnSkill(string skill_name)
        {
            return HeadSkills.ContainsKey(skill_name) || DeputySkills.ContainsKey(skill_name);
        }

        public List<string> GetHeadSkillList(bool include_acquired = false, bool include_equip = false)
        {
            List<string> skills = new List<string>();
            skills.AddRange(HeadSkills.Keys);
            if (include_acquired)
            {
                foreach (string name in HeadAcquiredSkills)
                {
                    if (include_equip || !HasEquip(name))
                        skills.Add(name);
                }
            }

            return skills;
        }

        public List<string> GetDeputySkillList(bool include_acquired = false, bool include_equip = false)
        {
            List<string> skills = new List<string>();
            skills.AddRange(DeputySkills.Keys);
            if (include_acquired)
            {
                foreach (string name in DeputyAcquiredSkills)
                {
                    if (include_equip || !HasEquip(name))
                        skills.Add(name);
                }
            }

            return skills;
        }

        public List<string> GetSkills(bool include_acquired = true, bool inclue_equip = true)
        {
            List<string> skills = GetHeadSkillList(include_acquired, inclue_equip);
            skills.AddRange(GetDeputySkillList(include_acquired, inclue_equip));
            return skills;
        }

        public List<string> GetAcquiredSkills()                     //this function & below get visible skills only. by weirdouncle
        {
            return HeadAcquiredSkills.Concat(DeputyAcquiredSkills).ToList<string>();
        }

        public List<string> GetAcquiredSkills(String flags)
        {
            if (flags == "all")
                return GetAcquiredSkills();
            if (flags == "head")
                return HeadAcquiredSkills;
            if (flags == "deputy")
                return DeputyAcquiredSkills;
            return null;
        }

        public bool HasShownSkill(string skill_name, bool head)
        {
            return head ? (HeadAcquiredSkills.Contains(skill_name) || (HeadSkills.ContainsKey(skill_name) && General1Showed))
                : (DeputyAcquiredSkills.Contains(skill_name) || (DeputySkills.ContainsKey(skill_name) && General2Showed));
        }
        public void AddSkill(string skill_name, bool head_skill, bool preshow = false)
        {
            if (head_skill)
                HeadSkills[skill_name] = preshow;
            else
                DeputySkills[skill_name] = preshow;
        }

        public void LoseSkill(string skill_name, bool head = true)
        {
            if (head)
                HeadSkills.Remove(skill_name);
            else
                DeputySkills.Remove(skill_name);
        }

        public void RemoveEquip(int location)
        {
            switch (location)
            {
                case 0:
                    Weapon = new KeyValuePair<int, string>(-1, null);
                    break;
                case 1:
                    Armor = new KeyValuePair<int, string>(-1, null);
                    break;
                case 2:
                    DefensiveHorse = new KeyValuePair<int, string>(-1, null);
                    break;
                case 3:
                    OffensiveHorse = new KeyValuePair<int, string>(-1, null);
                    break;
                case 4:
                    Treasure = new KeyValuePair<int, string>(-1, null);
                    break;
            }
        }

        public void RemoveDelayedTrick(WrappedCard card)
        {
            int index = JudgingArea.IndexOf(card.Id);
            if (index >= 0)
                JudgingArea.RemoveAt(index);
        }

        public bool IsSameCamp(Player other) => Camp != Game3v3Camp.S_CAMP_NONE && Camp == other.Camp;

        public List<string> GetPileOpener(string pile_name) => PileOpen.ContainsKey(pile_name) ? PileOpen[pile_name] : new List<string>();

        public void SetEquip(WrappedCard equip, int location)
        {
            switch (location)
            {
                case 0:
                    Weapon = new KeyValuePair<int, string>(equip.Id, equip.Name);
                    break;
                case 1:
                    Armor = new KeyValuePair<int, string>(equip.Id, equip.Name);
                    break;
                case 2:
                    DefensiveHorse = new KeyValuePair<int, string>(equip.Id, equip.Name);
                    break;
                case 3:
                    OffensiveHorse = new KeyValuePair<int, string>(equip.Id, equip.Name);
                    break;
                case 4:
                    Treasure = new KeyValuePair<int, string>(equip.Id, equip.Name);
                    break;
            }
        }

        public void AddDelayedTrick(WrappedCard card)
        {
            JudgingArea.Add(card.GetEffectiveId());
        }

        public void PileChange(string pile_name, List<int> card_ids, bool in_pile = true)
        {
            if (in_pile)
            {
                if (Piles.ContainsKey(pile_name))
                    Piles[pile_name].AddRange(card_ids);
                else
                    Piles[pile_name] = new List<int>(card_ids);
            }
            else
            {
                if (Piles.ContainsKey(pile_name))
                    foreach (int id in card_ids)
                        Piles[pile_name].Remove(id);
            }
        }
        public bool IsSkipped(PlayerPhase phase)
        {
            for (int i = PhasesIndex; i < PhasesState.Count; i++)
            {
                if (PhasesState[i].Phase == phase)
                    return PhasesState[i].Finished;
            }
            return false;
        }

        public void InsertPhase(PlayerPhase phase)
        {
            PhaseStruct _phase = new PhaseStruct
            {
                Phase = phase,
                Finished = false
            };
            Phases.Insert(PhasesIndex, phase);
            PhasesState.Insert(PhasesIndex, _phase);
        }

        public void AddCard(WrappedCard card, Place place, int location = 0)
        {
            switch (place)
            {
                case Place.PlaceHand:
                    HandCards.Add(-1);
                    break;
                case Place.PlaceEquip:
                    SetEquip(card, location);
                    break;
                case Place.PlaceDelayedTrick:
                    AddDelayedTrick(card);
                    break;
                default:
                    break;
            }
        }

        public void RemoveCard(WrappedCard card, Place place, int location = 0)
        {
            switch (place)
            {
                case Place.PlaceHand:
                    HandCards.Remove(-1);
                    break;
                case Place.PlaceEquip:
                    RemoveEquip(location);
                    break;
                case Place.PlaceDelayedTrick:
                    RemoveDelayedTrick(card);
                    break;
                default:
                    break;
            }
        }

        public bool IsAllNude() => IsNude() && JudgingArea.Count == 0;

        public bool IsLastHandCard(WrappedCard card, bool contain = false)
        {
            if (!contain)
            {
                foreach (int card_id in card.SubCards) {
                    if (!HandCards.Contains(card_id))
                        return false;
                }
                return HandCards.Count == card.SubCards.Count;
            }
            else
            {
                foreach (int card_id in  HandCards) {
                    if (!card.SubCards.Contains(card_id))
                        return false;
                }
                return true;
            }
        }

        public void AddQinggangTag(string card_string)
        {
            List<string> qinggang = tag.ContainsKey("Qinggang") ? (List<string>)tag["Qinggang"] : new List<string>();
            if (!qinggang.Contains(card_string))
                qinggang.Add(card_string);
            tag["Qinggang"] = qinggang;
        }

        static Dictionary<string, PlayerRole> role_map = new Dictionary<string, PlayerRole> {
            { "lord", PlayerRole.Lord },
            { "loyalist", PlayerRole.Loyalist },
            { "rebel", PlayerRole.Rebel },
            { "renegade", PlayerRole.Renegade },
        };
        public PlayerRole GetRoleEnum()
        {
            return role_map[Role];
        }
    }
}
