using CommonClass.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static CommonClass.Game.Player;
using static CommonClass.Game.WrappedCard;
using static SanguoshaServer.Game.EquipCard;
using static SanguoshaServer.Game.FunctionCard;

namespace SanguoshaServer.Game
{
    //This is Instead old ServerPlayer
    public class RoomLogic
    {
        public static bool PlayerContainsTrick(Room room, Player player, string name)
        {
            foreach (int id in player.JudgingArea)
                if (room.GetCard(id).Name == name)
                    return true;

            return false;
        }
        public static Skill IsProhibited(Room room, Player from, Player to, WrappedCard card, List<Player> others = null)
        {
            return Engine.IsProhibited(room, from, to, card, others);
        }

        public static bool CanSlash(Room room, Player slasher, Player other, WrappedCard slash, int rangefix = 0, List<Player> others = null)
        {
            if (other == slasher || !other.Alive)
                return false;

            WrappedCard newslash = new WrappedCard("Slash");

            if (RoomLogic.IsProhibited(room, slasher, other, slash ?? newslash, others) != null) return false;

            if (DistanceTo(room, slasher, other, slash) == -1) return false;

            if (slash != null && !slash.DistanceLimited
                    || Engine.CorrectCardTarget(room, TargetModSkill.ModType.DistanceLimit, slasher, other, slash ?? newslash)) return true;

            return InMyAttackRange(room, slasher, other, slash, rangefix);
        }

        public static bool CanSlash(Room room, Player slasher, Player other, int rangefix = 0, List<Player> others = null)
        {
            return CanSlash(room, slasher, other, null, rangefix, others);
        }

        public static bool CanSlashWithoutCrossBow(Room room, Player player, WrappedCard slash)
        {
            WrappedCard newslash = new WrappedCard("Slash");
            int slash_count = player.GetSlashCount();
            int valid_slash_count = 1;
            valid_slash_count += Engine.CorrectCardTarget(room, TargetModSkill.ModType.Residue, player, slash);

            return slash_count < valid_slash_count;

        }

        public static int GetAttackRange(Room room, Player player, bool include_weapon = true, int range_fix = 0)
        {
            if (player.HasFlag("InfinityAttackRange") || player.GetMark("InfinityAttackRange") > 0)
                return 1000;

            include_weapon = include_weapon && player.Weapon.Key != -1;

            int fixeddis = Engine.CorrectAttackRange(room, player, include_weapon, true);
            if (fixeddis > 0)
                return fixeddis;

            int original_range = 1, weapon_range = 0;

            if (include_weapon)
            {
                Weapon card = (Weapon)Engine.GetFunctionCard(room.GetCard(player.Weapon.Key).Name);
                weapon_range = card.Range - range_fix;
                if (weapon_range <= 0) include_weapon = false;
            }

            int real_range = Math.Max(original_range, weapon_range) + Engine.CorrectAttackRange(room, player, include_weapon, false);

            if (real_range < 0)
                real_range = 0;

            return real_range;
        }
        public static bool InMyAttackRange(Room room, Player from, Player other, WrappedCard card = null, int range_fix = 0)
        {
            if (DistanceTo(room, from, other) == -1)
                return false;
            List<string> in_attack_range_players = from.ContainsTag("in_my_attack_range") ? (List<string>)from.GetTag("in_my_attack_range") : new List<string>();
            if (in_attack_range_players.Contains(other.Name)) // for DIY Skills
                return true;

            bool inclue_weapon = from.Weapon.Key != -1;
            if (card != null && inclue_weapon)
            {
                List<int> ids = card.SubCards;
                if (ids.Contains(from.Weapon.Key))
                    inclue_weapon = false;
            }

            return DistanceTo(room, from, other, card) <= GetAttackRange(room, from, inclue_weapon, range_fix);
        }
        public static int OriginalRightDistance(Room room, Player from, Player other)
        {
            int right = 0;
            Player next_p = from;
            while (next_p != other)
            {
                next_p = room.GetNextAlive(next_p);
                right++;
            }
            return right;
        }

        public static int DistanceTo(Room room, Player from, Player other, WrappedCard card = null, bool ignore_remove = false)
        {
            if (from == other || !from.Alive || !other.Alive) return 0;

            if (!ignore_remove && (from.Removed || other.Removed)) return -1;

            int distance_fixed = Engine.GetFixedDistance(room, from, other);
            if (distance_fixed > 0) return distance_fixed;

            int right = OriginalRightDistance(room, from, other);
            int left = room.AliveCount(false) - right;
            int distance = Math.Min(left, right);

            distance += Engine.CorrectDistance(room, from, other, card);

            // keep the distance >=1
            if (distance < 1)
                distance = 1;

            return distance;
        }

        public static string CardToString(Room room, WrappedCard card)
        {
            bool modified = false;
            if (!IsVirtualCard(room, card))
            {
                WrappedCard origin = Engine.GetRealCard(card.Id);
                if (origin.Equals(card))
                    return card.Id.ToString();
                else
                    modified = true;
            }

            StringBuilder str = new StringBuilder(string.Format("{0}:{1}[{2}:{3}]={4}&{5}:{6}${7}%{8};{9}", card.Name, card.Skill,
                GetSuitString(GetCardSuit(room, card)), GetNumberString(GetCardNumber(room, card)), card.SubcardString(),
                card.ShowSkill, card.ExtraTarget.ToString(), card.DistanceLimited.ToString(), card.SkillPosition, card.UserString));

            if (card.Mute) str.Append("*");
            if (modified)
                str.Insert(0, string.Format("{0}?", card.Id));

            return str.ToString();
        }

        public static bool IsVirtualCard(Room room, WrappedCard card)
        {
            if (card.Id < 0)
                return true;
            else
            {
                WrappedCard ori_card = room.GetCard(card.Id);
                return ori_card != card;
            }
        }

        public static WrappedCard ParseUseCard(Room room, WrappedCard card)
        {
            if (!IsVirtualCard(room, card))
            {
                return card;
            }
            else
            {
                if (!string.IsNullOrEmpty(card.UserString))
                {
                    WrappedCard user_card = ParseCard(room, card.UserString);
                    if (user_card != null)
                        return user_card;
                }
                WrappedCard new_card = new WrappedCard(card.Name, card.Id, GetCardSuit(room, card), GetCardNumber(room, card), card.CanRecast, card.Transferable)
                {
                    ExtraTarget = card.ExtraTarget,
                    DistanceLimited = card.DistanceLimited,
                    Mute = card.Mute,
                    Skill = card.Skill,
                    ShowSkill = card.ShowSkill,
                    SkillPosition = card.SkillPosition,
                    UserString = card.UserString
                };
                new_card.AddSubCards(card.SubCards);

                return new_card;
            }
        }

        public static WrappedCard ParseCard(Room room, string card_str)
        {
            if (card_str.Contains('?'))
                return room.GetCard(int.Parse(card_str.Split('?')[0]));
            else
            {
                int id = -1;
                if (int.TryParse(card_str, out id) && id > -1)
                    return room.GetCard(id);
            }

            bool isMute = false;
            if (card_str.EndsWith("*"))
            {
                isMute = true;
                card_str.Remove(card_str.Length - 1);
            }
            Match match = Regex.Match(card_str, @"(\w+):(\w*)\[(\w+):(\S\d?)\]=([^:]+)&(\w*)\:(\w+)\$(\w+)%(\w+)?\;(\w+)?");
            if (match.Length > 0)
            {
                string card_name = match.Groups[1].ToString();
                string m_skillName = match.Groups[2].ToString();
                string suit_string = match.Groups[3].ToString();
                string number_string = match.Groups[4].ToString();
                string subcard_str = match.Groups[5].ToString();
                string show_skill = match.Groups[6].ToString();
                bool extra = bool.Parse(match.Groups[7].ToString());
                bool distance = bool.Parse(match.Groups[8].ToString());
                string skill_position = match.Groups[9].ToString();
                string user_string = match.Groups[10].ToString();

                WrappedCard new_card = new WrappedCard(card_name, -1, GetSuit(suit_string), GetNumber(number_string))
                {
                    Skill = m_skillName,
                    ShowSkill = show_skill,
                    SkillPosition = skill_position,
                    Mute = isMute,
                    ExtraTarget = extra,
                    DistanceLimited = distance,
                    UserString = user_string
                };
                if (subcard_str != ".")
                {
                    foreach (string id in subcard_str.Split('+'))
                    {
                        new_card.AddSubCard(int.Parse(id));
                    }
                }

                return new_card;
            }

            return null;
        }

        public static int GetCardNumber(Room room, WrappedCard card)
        {
            if (Engine.GetFunctionCard(card.Name)?.IsKindOf("SkillCard") == true)
                return 0;
            else
            {
                return card.SubCards.Count == 1 ? room.GetCard(card.SubCards[0]).Number : 0;
            }
        }

        public static CardSuit GetCardSuit(Room room, WrappedCard card)
        {
            if (IsVirtualCard(room, card))
            {
                if (Engine.GetFunctionCard(card.Name) is SkillCard)
                    return CardSuit.NoSuit;
                else
                {
                    if (card.SubCards.Count == 0)
                        return CardSuit.NoSuit;
                    else if (card.SubCards.Count == 1)
                        return room.GetCard(card.SubCards[0]).Suit;
                    else
                    {
                        CardColor color = CardColor.Colorless;
                        foreach (int id in card.SubCards)
                        {
                            CardColor color2 = GetColor(room.GetCard(id).Suit);
                            if (color == CardColor.Colorless)
                                color = color2;
                            else if (color != color2)
                                return CardSuit.NoSuit;
                        }
                        return (color == CardColor.Red) ? CardSuit.NoSuitRed : CardSuit.NoSuitBlack;
                    }
                }
            }
            else
                return card.Suit;
        }

        public static bool InSiegeRelation(Room room, Player other, Player skill_owner, Player victim)
        {
            if (IsFriendWith(room, other, victim) || !IsFriendWith(room, other, skill_owner) || !victim.HasShownOneGeneral()) return false;
            if (other == skill_owner)
                return (room.GetNextAlive(other) == victim && IsFriendWith(room, room.GetNextAlive(other, 2), other))
                || (room.GetLastAlive(other) == victim && IsFriendWith(room, room.GetLastAlive(other, 2), other));
            else
                return (room.GetNextAlive(other) == victim && room.GetNextAlive(other, 2) == skill_owner)
                || (room.GetLastAlive(other) == victim && room.GetLastAlive(other, 2) == skill_owner);
        }
        public static List<Player> GetFormation(Room room, Player player)
        {
            List<Player> teammates = new List<Player> { player };
            int n = room.AliveCount(false);
            int num = n;
            for (int i = 1; i < n; ++i)
            {
                Player target = room.GetNextAlive(player, i);
                if (IsFriendWith(room, player, target))
                    teammates.Add(target);
                else
                {
                    num = i;
                    break;
                }
            }

            n -= num;
            for (int i = 1; i < n; ++i)
            {
                Player target = room.GetLastAlive(player, i);
                if (IsFriendWith(room, player, target))
                    teammates.Add(target);
                else break;
            }

            return teammates;
        }
        public static bool InFormationRalation(Room room, Player player, Player teammate)
        {
            List<Player> teammates = GetFormation(room, player);
            return teammates.Count > 1 && teammates.Contains(teammate);
        }

        public static bool IsFriendWith(Room room, Player player, Player other)
        {
            return room.Scenario.IsFriendWith(room, player, other);
        }

        public static bool WillBeFriendWith(Room room, Player player, Player other)
        {
            return room.Scenario.WillBeFriendWith(room, player, other);
        }

        public static bool PlayerHasSkill(Room room, Player player, string skill_name, bool include_lose = false)
        {
            TriggerSkill trigger = Engine.GetTriggerSkill(skill_name);
            if (trigger != null && trigger.Global) return true;

            Skill skill = Engine.GetSkill(skill_name);
            if (skill == null)
                return false;

            if (!skill.Visible)
            {
                Skill main_skill = Engine.GetMainSkill(skill_name);
                if (main_skill != null && main_skill != skill)
                    return PlayerHasSkill(room, player, main_skill.Name);
            }


            if ((player.HeadSkills.ContainsKey(skill_name) && (player.General1Showed || (player.HeadSkills[skill_name] && player.CanShowGeneral("h"))))
                    || (player.DeputySkills.ContainsKey(skill_name) && (player.General2Showed || (player.DeputySkills[skill_name] && player.CanShowGeneral("d"))))
                    || player.HeadAcquiredSkills.Contains(skill_name) || player.DeputyAcquiredSkills.Contains(skill_name))
                return Engine.Invalid(room, player, skill_name) == null;

            if (Engine.ViewHas(room, player, skill_name, "skill").Count > 0) return true;

            if (include_lose && player.OwnSkill(skill_name)) return true;

            return false;
        }
        public static bool PlayerHasShownSkill(Room room, Player player, string skill)
        {
            return PlayerHasShownSkill(room, player, Engine.GetSkill(skill));
        }
        public static bool PlayerHasShownSkill(Room room, Player player, Skill skill)
        {
            if (skill == null) return false;

            if (player.HeadAcquiredSkills.Contains(skill.Name) || player.DeputyAcquiredSkills.Contains(skill.Name))
                return Engine.Invalid(room, player, skill.Name) == null;

            if (skill is TriggerSkill tr_skill)
            {
                if (tr_skill != null && tr_skill.Global)
                    return true;
            }

            if (!skill.Visible)
            {
                Skill main_skill = Engine.GetMainSkill(skill.Name);
                if (main_skill != null && skill != main_skill)
                    return PlayerHasShownSkill(room, player, main_skill);
                else
                    return false;
            }

            if (player.General1Showed && player.HeadSkills.ContainsKey(skill.Name))
                return Engine.Invalid(room, player, skill.Name) == null;
            else if (player.General2Showed && player.DeputySkills.ContainsKey(skill.Name))
                return Engine.Invalid(room, player, skill.Name) == null;

            List<ViewHasSkill> vhskills = Engine.ViewHas(room, player, skill.Name, "skill");
            if (vhskills.Count > 0)
            {
                foreach (ViewHasSkill vhskill in vhskills)
                {
                    if (vhskill.Global)                                                                //isGlobal do not need to show general
                        return true;
                }
                foreach (ViewHasSkill vhskill in vhskills)
                {
                    Skill main = Engine.GetMainSkill(vhskill.Name);
                    if (player.HeadAcquiredSkills.Contains(main.Name) || player.DeputyAcquiredSkills.Contains(main.Name))
                        return true;
                }
                foreach (ViewHasSkill vhskill in vhskills)
                {
                    Skill main = Engine.GetMainSkill(vhskill.Name);
                    if (player.OwnSkill(main.Name))
                    {
                        if (player.General1Showed && player.HeadSkills.ContainsKey(main.Name))
                            return true;
                        else if (player.General2Showed && player.DeputySkills.ContainsKey(main.Name))
                            return true;
                    }
                }
                return player.HasShownOneGeneral();                                                    //if player doesnt own it, then must showOneGeneral
            }
            return false;
        }
        public static bool InPlayerHeadSkills(Player player, string skill_name)
        {
            string main_skill = Engine.GetMainSkill(skill_name).Name;
            if (main_skill != skill_name)
                return InPlayerHeadSkills(player, main_skill);
            return player.HeadSkills.ContainsKey(skill_name) || player.HeadAcquiredSkills.Contains(skill_name);
        }
        public static bool InPlayerDeputykills(Player player, string skill_name)
        {
            string main_skill = Engine.GetMainSkill(skill_name).Name;
            if (main_skill != skill_name)
                return InPlayerDeputykills(player, main_skill);
            return player.DeputySkills.ContainsKey(skill_name) || player.DeputyAcquiredSkills.Contains(skill_name);
        }

        public static bool IsJilei(Room room, Player player, WrappedCard card) => IsCardLimited(room, player, card, HandlingMethod.MethodDiscard);
        public static bool IsLocked(Room room, Player player, WrappedCard card) => IsCardLimited(room, player, card, HandlingMethod.MethodUse);

        public static void SetPlayerCardLimitation(Player player, string limit_list, string pattern, bool single_turn = false)
        {
            List<string> limit_type = new List<string>(limit_list.Split(','));
            string _pattern = pattern;
            if (!pattern.EndsWith("$1") && !pattern.EndsWith("$0"))
            {
                string symb = single_turn ? "$1" : "$0";
                _pattern = _pattern + symb;
            }
            Dictionary<int, List<string>> limitation = new Dictionary<int, List<string>>(player.Limitation);
            foreach (string limit in limit_type)
            {
                HandlingMethod method = Engine.GetCardHandlingMethod(limit);
                if (limitation.ContainsKey((int)method))
                    limitation[(int)method].Add(_pattern);
                else
                    limitation[(int)method] = new List<string> { _pattern };
            }
            player.Limitation = limitation;
        }

        public static void RemovePlayerCardLimitation(Player player, string limit_list, string pattern)
        {
            List<string> limit_type = new List<string>(limit_list.Split(','));
            string _pattern = pattern;
            if (!_pattern.EndsWith("$1") && !_pattern.EndsWith("$0"))
                _pattern = _pattern + "$0";

            Dictionary<int, List<string>> limitation = new Dictionary<int, List<string>>(player.Limitation);
            foreach (string limit in limit_type)
            {
                HandlingMethod method = Engine.GetCardHandlingMethod(limit);
                if (limitation.ContainsKey((int)method))
                    limitation[(int)method].Remove(_pattern);
            }
            player.Limitation = limitation;
        }

        public static void ClearPlayerCardLimitation(Player player, bool single_turn = false)
        {
            List<HandlingMethod> limit_type = new List<HandlingMethod>
            {
                HandlingMethod .MethodUse, HandlingMethod.MethodResponse, HandlingMethod.MethodDiscard,
                HandlingMethod.MethodRecast, HandlingMethod.MethodPindian
            };

            Dictionary<int, List<string>> limitation = new Dictionary<int, List<string>>(player.Limitation);
            foreach (HandlingMethod method in limit_type)
            {
                if (!limitation.ContainsKey((int)method)) continue;
                List<string> limit_patterns = new List<string>(limitation[(int)method]);
                foreach (string pattern in limit_patterns)
                {
                    if (!single_turn || pattern.EndsWith("$1"))
                        limitation[(int)method].Remove(pattern);
                }
            }
            player.Limitation = limitation;
        }

        public static bool IsCardLimited(Room room, Player player, WrappedCard card, HandlingMethod method, bool isHandcard = false)
        {
            if (method == HandlingMethod.MethodNone)
                return false;

            FunctionCard fcard = Engine.GetFunctionCard(card.Name);
            Dictionary<int, List<string>> limitation = new Dictionary<int, List<string>>(player.Limitation);
            if (fcard != null && fcard.TypeID == CardType.TypeSkill && method == fcard.Method)
            {
                foreach (int card_id in card.SubCards)
                {
                    if (!limitation.ContainsKey((int)method)) continue;
                    WrappedCard c = room.GetCard(card_id);
                    foreach (string pattern in limitation[(int)method])
                    {
                        string _pattern = pattern.Split('$')[0];
                        if (isHandcard)
                            _pattern = _pattern.Replace("hand", ".");
                        ExpPattern p = new ExpPattern(_pattern);
                        if (p.Match(player, room, c)) return true;
                    }
                }
            }
            else
            {
                if (player.Removed && (method == HandlingMethod.MethodResponse || method == HandlingMethod.MethodUse))
                    return true;

                if (limitation.ContainsKey((int)method))
                {
                    foreach (string pattern in limitation[(int)method])
                    {
                        string _pattern = pattern.Split('$')[0];
                        if (isHandcard)
                            _pattern = _pattern.Replace("hand", ".");
                        if (Engine.MatchExpPattern(room, _pattern, player, card))
                            return true;
                    }
                }
            }

            return false;
        }

        public static GeneralSkin GetGeneralSkin(Room room, Player player, string skill_name, string position = null)
        {
            GeneralSkin result = new GeneralSkin();
            Skill skill = Engine.GetSkill(skill_name);
            if (skill != null)
            {
                if (!string.IsNullOrEmpty(position))
                {
                    result.General = position == "head" ? player.ActualGeneral1 : player.ActualGeneral2;
                    result.SkinId = position == "head" ? player.HeadSkinId : player.DeputySkinId;
                }
                else if (skill.Attached_lord_skill && GetLord(room, Engine.GetGeneral(player.ActualGeneral1).Kingdom) != null)
                {
                    Player lord = GetLord(room, Engine.GetGeneral(player.ActualGeneral1).Kingdom);
                    result.General = lord.ActualGeneral1;
                    result.SkinId = lord.HeadSkinId;
                }
                else
                {
                    if (InPlayerHeadSkills(player, skill_name))
                    {
                        result.SkinId = player.HeadSkinId;
                        if (PlayerHasShownSkill(room, player, skill_name))
                            result.General = player.General1;
                        else
                            result.General = player.ActualGeneral1;
                    }
                    else if (InPlayerDeputykills(player, skill_name))
                    {
                        result.SkinId = player.DeputySkinId;
                        if (PlayerHasShownSkill(room, player, skill_name))
                            result.General = player.General2;
                        else
                            result.General = player.ActualGeneral2;
                    }
                    else
                    {
                        result.General = player.General1Showed ? player.General1 : player.General2Showed ? player.General2 : "anjian";
                        result.SkinId = player.General1Showed ? player.HeadSkinId : player.General2Showed ? player.DeputySkinId : 0;
                    }
                }
            }
            return result;
        }
        public static Player GetLord(Room room, string kingdom, bool include_dead = false)
        {
            List<Player> players = include_dead ? room.Players : room.AlivePlayers;
            foreach (Player p in players)
            {
                General g = Engine.GetGeneral(p.ActualGeneral1);
                if (g.IsLord() && g.Kingdom == kingdom)
                    return p;
            }

            return null;
        }
        public static int GetMaxCards(Room room, Player player)
        {
            int origin = Engine.CorrectMaxCards(room, player, true);

            if (origin == -1)
                origin = Math.Max(player.Hp, 0);

            origin += Engine.CorrectMaxCards(room, player, false);

            return Math.Max(origin, 0);
        }

        public static List<Skill> GetHeadActivedSkills(Room room, Player player, bool include_acquired = true, bool shown = false, bool ignore_preshow = true)
        {
            List<Skill> skillList = new List<Skill>();
            List<string> skills = include_acquired ? new List<string>(player.HeadAcquiredSkills) : new List<string>();
            foreach (string skill_name in player.HeadSkills.Keys)
            {
                if (player.General1Showed || ((ignore_preshow || player.HeadSkills[skill_name]) && !shown && player.CanShowGeneral("h")))
                    skills.Add(skill_name);
            }
            foreach (string skill_name in skills)
            {
                if (Engine.Invalid(room, player, skill_name) != null) continue;

                Skill skill = Engine.GetSkill(skill_name);
                if (skill != null)
                {
                    if (!PlayerHasEquipSkill(player, skill.Name))
                    {
                        skillList.Add(skill);
                    }
                    if (skill.Visible)
                    {
                        List<Skill> related_skill = Engine.GetRelatedSkills(skill.Name);
                        foreach (Skill s in related_skill)
                            if (!skillList.Contains(s) && !s.Visible)
                                skillList.Add(s);
                    }
                }
            }
            List<Skill> skillList_copy = new List<Skill>(skillList);
            foreach (Skill s in skillList_copy)
            {
                if (s is ViewHasSkill skill)
                {
                    foreach (string name in skill.Skills)
                    {
                        Skill mskill = Engine.GetSkill(name);
                        if (mskill == null) continue;
                        if (!skillList.Contains(mskill) && skill.ViewHas(room, player, name))
                        {
                            skillList.Add(mskill);
                            if (mskill.Visible)
                            {
                                List<Skill> related_skill = Engine.GetRelatedSkills(mskill.Name);
                                foreach (Skill rskill in related_skill)
                                    if (!skillList.Contains(rskill) && !s.Visible)
                                        skillList.Add(rskill);
                            }
                        }
                    }
                }
            }

            return skillList;
        }

        public static List<Skill> GetDeputyActivedSkills(Room room, Player player, bool include_acquired = true, bool shown = false, bool ignore_preshow = true)
        {
            List<Skill> skillList = new List<Skill>();
            List<string> skills = include_acquired ? new List<string>(player.DeputyAcquiredSkills) : new List<string>();
            foreach (string skill_name in player.DeputySkills.Keys)
            {
                if (player.General2Showed || ((ignore_preshow || player.DeputySkills[skill_name]) && !shown && player.CanShowGeneral("d")))
                    skills.Add(skill_name);
            }
            foreach (string skill_name in skills)
            {
                if (Engine.Invalid(room, player, skill_name) != null) continue;
                
                Skill skill = Engine.GetSkill(skill_name);
                if (skill != null)
                {
                    if (!PlayerHasEquipSkill(player, skill.Name))
                    {
                        skillList.Add(skill);
                    }
                    if (skill.Visible)
                    {
                        List<Skill> related_skill = Engine.GetRelatedSkills(skill.Name);
                        foreach (Skill s in related_skill)
                            if (!skillList.Contains(s) && !s.Visible)
                                skillList.Add(s);
                    }
                }
            }

            List<Skill> skillList_copy = new List<Skill>(skillList);
            foreach (Skill s in skillList_copy)
            {
                if (s is ViewHasSkill skill)
                {
                    foreach (string name in skill.Skills)
                    {
                        Skill mskill = Engine.GetSkill(name);
                        if (mskill == null) continue;
                        if (!skillList.Contains(mskill) && skill.ViewHas(room, player, name))
                        {
                            skillList.Add(mskill);
                            if (mskill.Visible)
                            {
                                List<Skill> related_skill = Engine.GetRelatedSkills(mskill.Name);
                                foreach (Skill rskill in related_skill)
                                    if (!skillList.Contains(rskill) && !s.Visible)
                                        skillList.Add(rskill);
                            }
                        }
                    }
                }
            }

            return skillList;
        }

        public static bool PlayerHasEquipSkill(Player player, string name)
        {
            if (player.Weapon.Key != -1)
            {
                Skill skill = Engine.GetSkill(player.Weapon.Value);
                if (skill != null && skill.Name == name)
                    return true;
            }
            if (player.Armor.Key != -1)
            {
                Skill skill = Engine.GetSkill(player.Armor.Value);
                if (skill != null && skill.Name == name)
                    return true;
            }
            if (player.Treasure.Key != -1)
            {
                Skill skill = Engine.GetSkill(player.Treasure.Value);
                if (skill != null && skill.Name == name)
                    return true;
            }

            return false;
        }

        public static void RemovePlayerCard(Room room, Player player, WrappedCard card, Place place)
        {
            switch (place)
            {
                case Place.PlaceHand:
                    {
                        player.HandCards.Remove(card.Id);
                        break;
                    }
                case Place.PlaceEquip:
                    {
                        EquipCard equip = null;
                        if (Engine.GetFunctionCard(card.Name) is EquipCard)
                            equip = (EquipCard)Engine.GetFunctionCard(card.Name);
                        else
                            equip = (EquipCard)Engine.GetFunctionCard(Engine.GetRealCard(card.Id).Name);
                        equip.OnUninstall(room, player, card);
                        player.RemoveEquip((int)equip.EquipLocation());
                        bool show_log = true;
                        foreach (string flag in player.Flags)
                            if (flag.EndsWith("_InTempMoving"))
                            {
                                show_log = false;
                                break;
                            }
                        if (show_log)
                        {
                            LogMessage log = new LogMessage("$Uninstall")
                            {
                                Card_str = card.GetEffectiveId().ToString(),
                                From = player.Name
                            };
                            room.SendLog(log);
                        }
                        break;
                    }
                case Place.PlaceDelayedTrick:
                    {
                        player.RemoveDelayedTrick(card);
                        break;
                    }
                case Place.PlaceSpecial:
                    {
                        int card_id = card.GetEffectiveId();
                        string pile_name = player.GetPileName(card_id);

                        //@todo: sanity check required
                        if (!string.IsNullOrEmpty(pile_name))
                            player.Piles[pile_name].Remove(card_id);

                        break;
                    }
                default:
                    break;
            }
        }

        public static void AddPlayerCard(Room room, Player player, WrappedCard card, Place place)
        {
            switch (place)
            {
                case Place.PlaceHand:
                    {
                        player.HandCards.Add(card.Id);
                        break;
                    }
                case Place.PlaceEquip:
                    {
                        WrappedCard wrapped = room.GetCard(card.Id);
                        EquipCard equip = (EquipCard)Engine.GetFunctionCard(card.Name);
                        player.SetEquip(wrapped, (int)equip.EquipLocation());
                        equip.OnInstall(room, player, wrapped);
                        break;
                    }
                case Place.PlaceDelayedTrick:
                    {
                        player.AddDelayedTrick(card);
                        break;
                    }
                default:
                    break;
            }
        }

        public static List<WrappedCard> GetPlayerCards(Room room, Player player, string flag)
        {
            List<WrappedCard> cards = new List<WrappedCard>();
            if (flag.Contains("h"))
                cards.AddRange(GetPlayerHandcards(room, player));
            if (flag.Contains("e"))
                cards.AddRange(GetPlayerEquips(room, player));
            if (flag.Contains("j"))
                cards.AddRange(GetPlayerJudgingArea(room, player));

            return cards;
        }

        public static List<WrappedCard> GetPlayerEquips(Room room, Player player)
        {
            List<WrappedCard> cards = new List<WrappedCard>();
            foreach (int id in player.GetEquips())
                cards.Add(room.GetCard(id));
            return cards;
        }

        public static List<WrappedCard> GetPlayerHandcards(Room room, Player player)
        {
            List<WrappedCard> cards = new List<WrappedCard>();
            foreach (int id in player.HandCards)
                cards.Add(room.GetCard(id));
            return cards;
        }

        public static List<WrappedCard> GetPlayerJudgingArea(Room room, Player player)
        {
            List<WrappedCard> cards = new List<WrappedCard>();
            foreach (int id in player.JudgingArea)
                cards.Add(room.GetCard(id));
            return cards;
        }

        public static List<Player> ParsePlayers(Room room, List<string> names)
        {
            List<Player> players = new List<Player>();
            foreach (string name in names)
            {
                Player player = room.FindPlayer(name, true);
                if (player != null)
                    players.Add(player);
            }
            return players;
        }

        public static bool HasNullification(Room room, Player player)
        {
            foreach (WrappedCard card in GetPlayerHandcards(room, player))
            {
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                if (fcard is Nullification)
                    return true;
            }
            foreach (string pile in player.GetHandPileList(false))
            {
                foreach (int id in player.GetPile(pile))
                {
                    FunctionCard fcard = Engine.GetFunctionCard(room.GetCard(id).Name);
                    if (fcard is Nullification)
                        return true;
                }
            }
            foreach (string skill_name in room.Skills)
            {
                ViewAsSkill vsskill = Engine.GetViewAsSkill(skill_name);
                if (vsskill != null && vsskill.IsAvailable(room, player, CardUseStruct.CardUseReason.CARD_USE_REASON_RESPONSE_USE, "Nullification")) return true;
            }

            return false;
        }

        public static bool CanGetCard(Room room, Player from, Player to, string flags)
        {
            int count = 0;
            if (flags.Contains("h") && from != to && Engine.IsCardFixed(room, from, to, "h", HandlingMethod.MethodGet) == null)
                count += to.HandcardNum;

            if (flags.Contains("j") && Engine.IsCardFixed(room, from, to, "j", HandlingMethod.MethodGet) == null)
                count += to.JudgingArea.Count;

            if (flags.Contains("e") && Engine.IsCardFixed(room, from, to, "e", HandlingMethod.MethodGet) == null)
            {
                foreach (int id in to.GetEquips())
                    if (CanGetCard(room, from, to, id))
                        count++;
            }

            return count > 0;
        }

        private static readonly Dictionary<Location, string> equip_map = new Dictionary<Location, string> {
            { Location.WeaponLocation, "w" },
            { Location.ArmorLocation, "a" },
            { Location.OffensiveHorseLocation, "o"},
            {Location.DefensiveHorseLocation,"d" },
            {Location.TreasureLocation, "t" }
        };
        public static bool CanGetCard(Room room, Player from, Player to, int card_id)
        {
            if (from == to && from.HandCards.Contains(card_id))
                return false;

            foreach (int id in to.JudgingArea)
                if (id == card_id && Engine.IsCardFixed(room, from, to, "j", HandlingMethod.MethodGet) != null)
                    return false;

            foreach (int id in to.GetEquips())
            {
                if (id == card_id)
                {
                    FunctionCard fcard = Engine.GetFunctionCard(room.GetCard(id).Name);
                    EquipCard equip = (EquipCard)fcard;
                    if (Engine.IsCardFixed(room, from, to, equip_map[equip.EquipLocation()], HandlingMethod.MethodGet) != null)
                        return false;
                }
            }

            return true;
        }

        public static bool CanDiscard(Room room, Player from, Player to, string flags)
        {
            int count = 0;
            if (flags.Contains("h") && Engine.IsCardFixed(room, from, to, "h", HandlingMethod.MethodDiscard) == null)
                count += to.HandcardNum;

            if (flags.Contains("j") && Engine.IsCardFixed(room, from, to, "j", HandlingMethod.MethodDiscard) == null)
                count += to.JudgingArea.Count;

            if (flags.Contains("e") && Engine.IsCardFixed(room, from, to, "e", HandlingMethod.MethodDiscard) == null)
            {
                foreach (int id in to.GetEquips())
                    if (CanDiscard(room, from, to, id))
                        count++;
            }

            return count > 0;
        }

        public static bool CanDiscard(Room room, Player from, Player to, int card_id)
        {
            if (from == to && IsJilei(room, from, room.GetCard(card_id)))
                return false;

            foreach (int id in to.JudgingArea)
                if (id == card_id && Engine.IsCardFixed(room, from, to, "j", HandlingMethod.MethodDiscard) != null)
                    return false;

            foreach (int id in to.GetEquips())
            {
                if (id == card_id)
                {
                    FunctionCard fcard = Engine.GetFunctionCard(room.GetCard(id).Name);
                    EquipCard equip = (EquipCard)fcard;
                    if (Engine.IsCardFixed(room, from, to, equip_map[equip.EquipLocation()], HandlingMethod.MethodDiscard) != null)
                        return false;
                }
            }

            return true;
        }
        public static List<string> GetBigKingdoms(Room room)
        {
            Dictionary<string, int> kingdom_map = new Dictionary<string, int>
            {
                { "wei", 0 },
                { "shu", 0 },
                { "wu", 0 },
                { "qun", 0 }
            };
            List<Player> players = room.AlivePlayers;
            foreach (Player p in players)
            {
                if (!p.HasShownOneGeneral())
                    continue;
                if (p.Role == "careerist")
                {
                    kingdom_map["careerist"] = 1;
                    continue;
                }
                ++kingdom_map[p.Kingdom];
            }

            List<string> big_kingdoms = new List<string>();
            foreach (string key in kingdom_map.Keys)
            {
                if (kingdom_map[key] == 0)
                    continue;
                if (big_kingdoms.Count == 0)
                {
                    if (kingdom_map[key] > 1)
                        big_kingdoms.Add(key);
                    continue;
                }
                if (kingdom_map[key] == kingdom_map[big_kingdoms[0]])
                {
                    big_kingdoms.Add(key);
                }
                else if (kingdom_map[key] > kingdom_map[big_kingdoms[0]])
                {
                    big_kingdoms.Clear();
                    big_kingdoms.Add(key);
                }
            }
            Player jade_seal_owner = null;
            foreach (Player p in players)
            {
                if (p.HasTreasure("JadeSeal") && p.HasShownOneGeneral())
                {
                    jade_seal_owner = p;
                    break;
                }
            }
            if (jade_seal_owner != null)
            {
                if (jade_seal_owner.Role == "careerist")
                {
                    big_kingdoms.Clear();
                    big_kingdoms.Add(jade_seal_owner.Name); // record player's objectName who has JadeSeal.
                }
                else
                { // has shown one general but isn't careerist
                    string kingdom = jade_seal_owner.Kingdom;
                    big_kingdoms.Clear();
                    big_kingdoms.Add(kingdom);
                }
            }
            return big_kingdoms;
        }
        public static bool CanBeChainedBy(Room room, Player target, Player _source)
        {
            Player source = _source??target;
            if (target.Chained)
            {
                return true;
            }
            else
            {
                if (RoomLogic.HasArmorEffect(room, target, "IronArmor"))
                {
                    List<string> big_kingdoms = GetBigKingdoms(room);
                    if (big_kingdoms.Count > 0)
                    {
                        string kingdom = (target.HasShownOneGeneral() ? (target.Role == "careerist" ? target.Name : target.Kingdom) : string.Empty);
                        if (!big_kingdoms.Contains(kingdom))
                            return false;
                    }
                }
                return true;
            }
        }
        public static bool HasShownArmorEffect(Room room, Player player)
        {
            if ((player.ContainsTag("Qinggang") && ((List<string>)player.GetTag("Qinggang")).Count > 0) || player.GetMark("Armor_Nullified") > 0
                || player.GetMark("Equips_Nullified_to_Yourself") > 0)
                return false;

            if (player.GetArmor()) return true;
            List <ViewHasSkill> vhskills = Engine.ViewHasArmorEffect(room, player);
            if (vhskills.Count > 0)
            {
                foreach (ViewHasSkill skill in vhskills) {
                    if (PlayerHasShownSkill(room, player, skill)) return true;
                }
            }

            return false;
        }

        public static bool HasArmorEffect(Room room, Player player, string armor_name, bool include_viewhas = true)
        {
            if ((player.ContainsTag("Qinggang") && ((List<string>)player.GetTag("Qinggang")).Count > 0) || player.GetMark("Armor_Nullified") > 0
                || player.GetMark("Equips_Nullified_to_Yourself") > 0)
                return false;

            if (include_viewhas && Engine.ViewHas(room, player, armor_name, "armor").Count > 0)
                return true;

            if (!player.GetArmor()) return false;
            if (player.Armor.Value == armor_name) return true;
            return false;
        }

        public static int GetPlayerNumWithSameKingdom(Room room, Player player, string to_calculate = null)
        {
            if (string.IsNullOrEmpty(to_calculate))
            {
                if (!player.HasShownOneGeneral())
                    return 0;
                else if (player.Role == "careerist")
                    return 1;
                else
                    to_calculate = player.Kingdom;
            }

            List <Player> players = room.AlivePlayers;
            int num = 0;

            if (to_calculate == "careerist")
            {
                foreach (Player p in players)
                    if (p.Role == "careerist")
                    return 1;
            }
            else
            {
                foreach (Player p in players) {
                    if (!p.HasShownOneGeneral() || p.Role == "careerist" || p.Kingdom != to_calculate)
                        continue;

                    num++;
                }
            }

            return num;
        }

        public static List<Player> FindPlayersBySkillName(Room room, string skill_name)
        {
            List<Player> list = new List<Player>();
            foreach (Player player in room.GetAllPlayers())
            {
                if (RoomLogic.PlayerHasSkill(room, player, skill_name))
                    list.Add(player);
            }
            return list;
        }

        public static Player FindPlayerBySkillName(Room room, string skill_name)
        {
            foreach (Player player in room.GetAllPlayers())
            {
                if (PlayerHasSkill(room, player, skill_name))
                    return player;
            }
            return null;
        }
    }
}
