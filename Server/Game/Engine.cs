using CommonClass;
using CommonClass.Game;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using static CommonClass.Game.WrappedCard;
using SanguoshaServer.Scenario;
using SanguoshaServer.Package;
using static SanguoshaServer.Game.FunctionCard;

namespace SanguoshaServer.Game
{
    public class Engine
    {
        private static Dictionary<string, Skill> skills = new Dictionary<string, Skill>();
        private static Dictionary<string, List<TriggerSkill>> pack_equip_triggerskills = new Dictionary<string, List<TriggerSkill>>();
        private static Dictionary<string, FunctionCard> function_cards = new Dictionary<string, FunctionCard>();
        private static Dictionary<string, GameMode> game_modes = new Dictionary<string, GameMode>();
        private static Dictionary<int, WrappedCard> wrapped_cards = new Dictionary<int, WrappedCard>();
        private static Dictionary<string, GameScenario> scenarios = new Dictionary<string, GameScenario>();
        private static DataTable general_skills;
        
        private static DataSet card_table = new DataSet();
        private static Dictionary<string, List<int>> mode_card_ids = new Dictionary<string, List<int>>();
        private static Dictionary<string, List<int>> package_card_ids = new Dictionary<string, List<int>>();

        private static Dictionary<string, General> generals = new Dictionary<string, General>();
        private static Dictionary<string, List<string>> pack_generals = new Dictionary<string, List<string>>();

        private static Dictionary<string, List<string>> related_skills = new Dictionary<string, List<string>>();
        private static Dictionary<string, List<string>> convert_pairs = new Dictionary<string, List<string>>();

        private static List<ProhibitSkill> prohibit_skills = new List<ProhibitSkill>();
        private static List<FixCardSkill> fixcard_skills = new List<FixCardSkill>();
        private static List<ViewHasSkill> viewhas_skills = new List<ViewHasSkill>();
        private static List<DistanceSkill> distance_skills = new List<DistanceSkill>();
        private static List<MaxCardsSkill> maxcards_skills= new List<MaxCardsSkill>();
        private static List<TargetModSkill> targetmod_skills = new List<TargetModSkill>();
        private static List<AttackRangeSkill> attackrange_skills = new List<AttackRangeSkill>();
        private static List<InvalidSkill> invalid_skills = new List<InvalidSkill>();
        private static List<TriggerSkill> global_trigger_skills = new List<TriggerSkill>();
        private static readonly List<TriggerSkill> public_trigger_skills = new List<TriggerSkill>();

        private static Dictionary<string, CardPattern> patterns = new Dictionary<string, CardPattern>();
        private static List<ExpPattern> enginePatterns = new List<ExpPattern>();

        public Engine()
        {
            //初始化游戏数据
            InitGameData();
        }

        private void InitGameData()
        {
            //读取翻译
            LoadTranslations();
            //读取武将转换列表

            //读取游戏模式
            CreateGameMode();

            //读取武将
            CreateGenerals();

            //读取卡牌
            CreateCardTable();

            //读取技能
            LoadScenario();

            LoatOthers();
        }

        private void LoadScenario()
        {
            foreach (GameMode mode in game_modes.Values)
            {
                //创建武将包中的技能、技能卡
                foreach (string package in mode.GeneralPackage)
                {
                    //反射创建
                    Type t = Type.GetType("SanguoshaServer.Package." + package);
                    GeneralPackage pack = (GeneralPackage)Activator.CreateInstance(t);
                    foreach (FunctionCard card  in pack.SkillCards)
                        function_cards.Add(card.Name, card);
                    foreach (string key in pack.RelatedSkills.Keys)
                        related_skills.Add(key, pack.RelatedSkills[key]);
                    foreach (string key in pack.ConvertPairs.Keys)
                    {
                        if (convert_pairs.ContainsKey(key))
                            convert_pairs[key].AddRange(pack.ConvertPairs[key]);
                        else
                            convert_pairs.Add(key, pack.ConvertPairs[key]);
                    }
                    AddSkills(pack.Skills);
                    foreach (string key in pack.Patterns.Keys)
                        patterns.Add(key, pack.Patterns[key]);
                }
                //创建卡牌包中的卡牌、技能卡、技能
                foreach (string package in mode.CardPackage)
                {
                    //反射创建
                    Type t = Type.GetType("SanguoshaServer.Package." + package);
                    CardPackage pack = (CardPackage)Activator.CreateInstance(t);
                    foreach (FunctionCard card in pack.Cards)
                        if (!function_cards.ContainsKey(card.Name))
                            function_cards.Add(card.Name, card);

                    AddSkills(pack.Skills);
                    foreach (Skill skill in pack.Skills)
                    {
                        if (skill is TriggerSkill trigger)
                        {
                            if (pack_equip_triggerskills.ContainsKey(package))
                                pack_equip_triggerskills[package].Add(trigger);
                            else
                                pack_equip_triggerskills[package] = new List<TriggerSkill> { trigger };
                        }
                    }
                }
            }

            //生成装备的位置文件
            Dictionary<string, int> equips = new Dictionary<string, int>();
            foreach (FunctionCard card in function_cards.Values)
                if (card is EquipCard equip)
                    equips[equip.Name] = (int)equip.EquipLocation();
            File.Delete("gamedata/equips.json");
            File.AppendAllText("gamedata/equips.json", JsonUntity.Dictionary2Json(equips));

            //武将关联技能表
            string sql = "select * from general_skill";
            general_skills = DB.GetData(sql, false);
            general_skills.TableName = "general";

            DataTable skills_info = DB.GetData("select * from skills", false);
            skills_info.TableName = "skill";
            DataSet skills_ds = new DataSet();
            skills_ds.Tables.Add(general_skills);
            skills_ds.Tables.Add(skills_info);
            File.Delete("gamedata/skills.json");
            File.AppendAllText("gamedata/skills.json", JsonUntity.DataSet2Json(skills_ds));
        }

        //生成客户端的数据文件
        private void LoatOthers()
        {
            string sql = "select * from title";
            DataTable dt = DB.GetData(sql, false);
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);

            File.Delete("gamedata/titles.json");
            File.AppendAllText("gamedata/titles.json", JsonUntity.DataSet2Json(ds));
        }

        private void LoadTranslations()
        {
            string sql = "select [key], translation from translation";
            DataTable dt = DB.GetData(sql, false);
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);

            File.Delete("gamedata/translations.json");
            File.AppendAllText("gamedata/translations.json", JsonUntity.DataSet2Json(ds));
        }

        private void CreateGameMode()
        {            
            //游戏包
            string sql = "select * from game_mode";
            DataTable dt = DB.GetData(sql, false);
            DataView dView = dt.DefaultView;
            dView.Sort = "index asc";               //按index升序
            dt = dView.ToTable();

            foreach (DataRow r in dt.Rows)
            {
                string mode_name = r["mode_name"].ToString();
                if (!game_modes.ContainsKey(mode_name))
                {
                    GameMode mode = new GameMode
                    {
                        Name = mode_name,
                        PlayerNum = new List<int> { int.Parse(r["players_count"].ToString()) },
                        IsScenario = Boolean.Parse(r["is_scenario"].ToString()),
                        GeneralPackage = new List<string>(),
                        CardPackage = new List<string>(),
                    };
                    game_modes.Add(mode_name, mode);

                    //反射创建
                    Type t = Type.GetType("SanguoshaServer.Scenario." + mode_name);
                    GameScenario scenario = (GameScenario)Activator.CreateInstance(t);
                    scenarios.Add(mode_name, scenario);
                }
                else
                {
                    game_modes[mode_name].PlayerNum.Add(int.Parse(r["players_count"].ToString()));
                }
            }

            //读取模式对应的武将包
            sql = "select * from general_package";
            dt = DB.GetData(sql, false);
            dView = dt.DefaultView;
            dView.Sort = "index asc";
            dt = dView.ToTable();
            foreach (DataRow r in dt.Rows)
            {
                game_modes[r["mode"].ToString()].GeneralPackage.Add(r["package_name"].ToString());
            }
            //读取模式对应的卡牌包
            sql = "select * from card_package";
            dt = DB.GetData(sql, false);
            dView = dt.DefaultView;
            dView.Sort = "index asc";
            dt = dView.ToTable();
            foreach (DataRow r in dt.Rows)
            {
                game_modes[r["mode"].ToString()].CardPackage.Add(r["package_name"].ToString());
            }

            File.Delete("gamedata/gamemode.json");
            File.AppendAllText("gamedata/gamemode.json", JsonUntity.Dictionary2Json<string, GameMode>(game_modes));
        }
        private void CreateCardTable()
        {
            //将服务器中卡牌信息写入table
            string sql = "select * from cards";
            DataTable table = DB.GetData(sql, false);
            DataView dView = table.DefaultView;
            dView.Sort = "id asc";               //按index升序
            table = dView.ToTable();

            //生成原始卡牌模板
            foreach (DataRow row in table.Rows)
            {
                string name = row["name"].ToString();
                int id = int.Parse(row["id"].ToString());
                CardSuit suit = WrappedCard.GetSuit(row["suit"].ToString());
                int number = int.Parse(row["number"].ToString());
                bool can_recast = Boolean.Parse(row["can_recast"].ToString());
                bool transferable = Boolean.Parse(row["transferable"].ToString());
                //public WrappedCard(string name, int id, CardSuit suit, int number, bool can_recast = false, bool transferable = false)

                wrapped_cards.Add(id, new WrappedCard(name, id, suit, number, can_recast, transferable));
            }

            foreach (string name in game_modes.Keys)
            {
                DataTable new_table = table.Clone();
                new_table.TableName = name;
                DataRow[] rows = table.Select("mode = '" + name + "'");

                List<int> ids = new List<int>();

                foreach (DataRow row in rows)
                {
                    new_table.ImportRow(row);                   // 将DataRow添加到DataTable中
                    ids.Add(int.Parse(row["id"].ToString()));
                }
                mode_card_ids.Add(name, ids);                        //按模式给卡牌id分类
                //card_table.Add(name, new_table);                //按模式给卡牌信息分类
                card_table.Tables.Add(new_table);
            }
            foreach (GameMode mode in game_modes.Values)
            {
                foreach (string package in mode.CardPackage)
                {
                    DataRow[] rows = table.Select(string.Format("mode='{0}' and package='{1}'", mode.Name, package));
                    List<int> ids = new List<int>();
                    foreach (DataRow row in rows)
                    {
                        ids.Add(int.Parse(row["id"].ToString()));
                    }
                    package_card_ids.Add(package, ids);                        //按卡牌包给卡牌id分类
                }
            }

            //读取funciont cards;
            sql = "select * from function_card";
            DataTable function_table = DB.GetData(sql, false);
            DataView function_View = function_table.DefaultView;
            function_View.Sort = "index asc";               //按index升序
            function_table = function_View.ToTable();
            function_table.TableName = "FunctionCard";
            //foreach (DataRow row in function_table.Rows)
            //{
            //    string class_name = row["name"].ToString();
            //    //反射创建
            //    Type t = Type.GetType("SanguoshaServer.Game." + class_name);
            //    if (t != null)
            //    {
            //        FunctionCard card = (FunctionCard)Activator.CreateInstance(t);
            //        function_cards[class_name] = card;
            //    }
            //}

            card_table.Tables.Add(function_table);

            File.Delete("gamedata/cards.json");
            File.AppendAllText("gamedata/cards.json", JsonUntity.DataSet2Json(card_table));
        }
        private void CreateGenerals()
        {
            string sql = "select * from generals";
            DataTable table = DB.GetData(sql, false);
            table.TableName = "general_info";
            DataSet general_data = new DataSet();
            general_data.Tables.Add(table);
            File.Delete("gamedata/generalinfo.json");
            File.AppendAllText("gamedata/generalinfo.json", JsonUntity.DataSet2Json(general_data));

            //生成武将模板
            foreach (DataRow row in table.Rows)
            {
                string name = row["general_name"].ToString();
                string kingdom = row["kingdom"].ToString();
                int double_max_hp = int.Parse(row["HP"].ToString());
                bool hegemony_lord = Boolean.Parse(row["hegemony_lord"].ToString());
                bool classic_lord = Boolean.Parse(row["classic_lord"].ToString());
                bool male = Boolean.Parse(row["sex"].ToString());
                bool hidden = Boolean.Parse(row["hidden"].ToString());
                //string name, string kingdom, bool classic_lord = false, bool hegemony_lord = false, int double_max_hp = 4, bool male = true, bool hidden = false

                generals.Add(name, new General(name, kingdom, classic_lord, hegemony_lord, double_max_hp, male, hidden));
                string companion = row["companion"].ToString();
                if (!string.IsNullOrEmpty(companion))
                    generals[name].Companions.AddRange(companion.Split(','));
            }

            //国战模式
            foreach (string pack in game_modes["Hegemony"].GeneralPackage)
            {
                DataRow[] rows = table.Select("hegemony_package = '" + pack + "'");
                List<string> general_names = new List<string>();
                foreach (DataRow row in rows)
                {
                    general_names.Add(row["general_name"].ToString());
                }
                //File.AppendAllText("gamedata/result.json", string.Format("{0}:{1}", pack, general_names.Count));
                pack_generals[pack] = general_names;                //按卡牌包给武将分类
            }

            //身份模式
            if (game_modes.ContainsKey("Classic"))
            {
                foreach (string pack in game_modes["Classic"].GeneralPackage)
                {
                    DataRow[] rows = table.Select("classic_package = '" + pack + "'");
                    List<string> general_names = new List<string>();
                    foreach (DataRow row in rows)
                    {
                        general_names.Add(row["general_name"].ToString());
                    }
                    pack_generals[pack] = general_names;                //按卡牌包给武将分类
                }
            }

            //其他模式
            foreach (string pack in game_modes.Keys)
            {
                if (pack == "Hegemony" || pack == "Classic") continue;
                DataRow[] rows = table.Select("scenario = '" + pack + "'");
                List<string> general_names = new List<string>();
                foreach (DataRow row in rows)
                {
                    general_names.Add(row["general_name"].ToString());
                }
                pack_generals[pack] = general_names;                //按卡牌包给武将分类
            }

            File.Delete("gamedata/generals.json");
            File.AppendAllText("gamedata/generals.json", JsonUntity.Dictionary2Json<string, List<string>>(pack_generals));
        }

        public static GameScenario GetScenario(string name)
        {
            if (scenarios.Keys.Contains(name))
                return scenarios[name];

            return null;
        }

        #region 卡牌相关
        public static List<int> GetGameCards(List<string> packages)
        {
            List<int> ids = new List<int>();
            foreach (string name in package_card_ids.Keys)
            {
                if (packages.Contains(name))
                {
                    ids.AddRange(package_card_ids[name]);
                }
            }

            return ids;
        }
        public static WrappedCard GetRealCard(int id)
        {
            if (wrapped_cards.ContainsKey(id))
                return wrapped_cards[id];
            else
                return null;
        }
        
        public static FunctionCard GetFunctionCard(string card_name)
        {
            if (function_cards.ContainsKey(card_name))
                return function_cards[card_name];
            else
                return null;
        }
        public static WrappedCard CloneCard(WrappedCard card)
        {
            WrappedCard new_card = new WrappedCard(card.Name, card.Id, card.Suit, card.Number, card.CanRecast, card.Transferable)
            {
                CanRecast = card.CanRecast,
                Skill = card.Skill,
                ShowSkill = card.ShowSkill,
                UserString = card.UserString,
                Flags = card.Flags,
                Mute = card.Mute,
            };
            new_card.ExtraTarget = card.ExtraTarget;
            new_card.DistanceLimited = card.DistanceLimited;
            new_card.ClearSubCards();
            new_card.AddSubCards(card.SubCards);

            return new_card;
        }
        #endregion

        #region 武将相关
        public static List<string> GetGenerals(List<string> packages)
        {
            List<string> generals = new List<string>();
            foreach (string key in pack_generals.Keys)
            {
                if (packages.Contains(key))
                {
                    foreach (string name in pack_generals[key])
                    {
                        General general = GetGeneral(name);
                        if (general != null && !general.Hidden)
                            generals.Add(general.Name);
                    }
                }
            }

            return generals;
        }

        public static List<string> GetConverPairs(string name)
        {
            if (convert_pairs.ContainsKey(name))
                return convert_pairs[name];
            else
                return new List<string>();
        }

        public static string GetMainGeneral(string general_name)
        {
            foreach (string name in convert_pairs.Keys)
            {
                if (name == general_name || convert_pairs[name].Contains(general_name))
                    return name;
            }

            return general_name;
        }

        public static General GetGeneral(string name)
        {
            if (generals.ContainsKey(name))
                return generals[name];
            else
                return null;
        }
        public static List<string> GetGeneralSkills(string name, string mode, bool head)
        {
            DataRow[] rows = general_skills.Select(string.Format("general = '{0}' and mode = '{1}'", name, mode));
            if (rows.Length > 0)
            {
                if (mode == "Hegemony")
                {
                    List<string> result = new List<string>();
                    foreach (string skill_name in rows[0]["skill_names"].ToString().Split(','))
                    {
                        Skill skill = GetSkill(skill_name);
                        if (skill != null && (string.IsNullOrEmpty(skill.Relate_to_place) || (head && skill.Relate_to_place == "head") || (!head && skill.Relate_to_place == "deputy")))
                        {
                            result.Add(skill_name);
                        }
                    }
                    return result;
                }
                else
                    return new List<string>(rows[0]["skill_names"].ToString().Split(','));
            }

            return new List<string>();
        }

        public static List<string> GetGeneralSkills(string name, string mode)
        {
            DataRow[] rows = general_skills.Select(string.Format("general = '{0}' and mode = '{1}'", name, mode));
            if (rows.Length > 0)
            {
                return new List<string>(rows[0]["skill_names"].ToString().Split(','));
            }

            return new List<string>();
        }

        public static List<string> GetGeneralRelatedSkills(string name, string mode)
        {
            DataRow[] rows = general_skills.Select(string.Format("general = '{0}' and mode = '{1}'", name, mode));
            if (rows.Length > 0)
            {
                string str = rows[0]["related_skills"].ToString();
                if (!string.IsNullOrEmpty(str))
                    return new List<string>(str.Split(','));
            }

            return new List<string>();
        }
        #endregion

        #region 技能相关
        public static void AddSkills(List<Skill> all_skills)
        {
            foreach (Skill skill in all_skills)
            {
                if (skill == null || skills.ContainsKey(skill.Name))
                {
                    continue;
                }

                skills.Add(skill.Name, skill);

                if (skill is ProhibitSkill pskill)
                    prohibit_skills.Add(pskill);
                else if (skill is FixCardSkill fskill)
                    fixcard_skills.Add(fskill);
                else if (skill is ViewHasSkill vhskill)
                    viewhas_skills.Add(vhskill);
                else if (skill is DistanceSkill dskill)
                    distance_skills.Add(dskill);
                else if (skill is MaxCardsSkill mskill)
                    maxcards_skills.Add(mskill);
                else if (skill is TargetModSkill tmskill)
                    targetmod_skills.Add(tmskill);
                else if (skill is AttackRangeSkill askill)
                    attackrange_skills.Add(askill);
                else if (skill is InvalidSkill iskill)
                    invalid_skills.Add(iskill);
                else if (skill is TriggerSkill tskill)
                {
                    if (tskill != null && tskill.Global)
                        global_trigger_skills.Add(tskill);
                }
            }
        }
        public static Skill GetSkill(string skill_name)
        {
            if (!string.IsNullOrEmpty(skill_name) && skills.ContainsKey(skill_name))
                return skills[skill_name];
            else
                return null;
        }
        public static List<string> GetSkillNames() => new List<string>(skills.Keys);
        public static TriggerSkill GetTriggerSkill(string skill_name)
        {
            Skill skill = GetSkill(skill_name);
            if (skill != null && skill is TriggerSkill ts)
                return ts;

            return null;
        }

        public static ViewAsSkill GetViewAsSkill(string skill_name)
        {
            Skill skill = GetSkill(skill_name);
            if (skill == null) return null;

            if (skill is ViewAsSkill)
                return (ViewAsSkill)skill;
            else if (skill is TriggerSkill trigger_skill)
            {
                return trigger_skill.ViewAsSkill;
            }
            else if (skill is DistanceSkill distance_skill)
            {
                return distance_skill.ViewAsSkill;
            }
            else
                return null;
        }
        public static List<TriggerSkill> GetGlobalTriggerSkills() => global_trigger_skills;
        public static List<TriggerSkill> GetPublicTriggerSkills() => public_trigger_skills;
        public static List<Skill> GetEquipTriggerSkills(List<string> packs)
        {
            List<Skill> skills = new List<Skill>();
            foreach (string p in pack_equip_triggerskills.Keys)
                if (packs.Contains(p))
                    skills.AddRange(pack_equip_triggerskills[p]);

            return skills;
        }
        public static Skill GetMainSkill(string skill_name)
        {
            Skill skill = GetSkill(skill_name);
            if (skill != null && (skill.Visible || related_skills.Keys.Contains(skill_name))) return skill;
            foreach (string key in related_skills.Keys) {
                foreach (string name in related_skills[key])
                    if (name == skill_name) return GetSkill(key);
            }

            return skill;
        }
        public static List<ViewHasSkill> ViewHas(Room room, Player player, string skill_name, string flag)
        {
            List <ViewHasSkill> skills = new List<ViewHasSkill>();
            foreach (ViewHasSkill skill in viewhas_skills)
            {
                if (flag == "armor" && skill.Armors.Contains(skill_name) && skill.ViewHas(room, player, skill_name))
                    skills.Add(skill);
                else if (flag == "skill" && skill.Skills.Contains(skill_name) && skill.ViewHas(room, player, skill_name))
                    skills.Add(skill);
            }

            return skills;
        }
        public static List<Skill> GetRelatedSkills(string name)
        {
            List<Skill> skills = new List<Skill>();
            if (related_skills.ContainsKey(name))
            {
                foreach (string skill_name in related_skills[name])
                {
                    skills.Add(GetSkill(skill_name));
                }
            }

            return skills;
        }
        public static Skill IsProhibited(Room room, Player from, Player to, WrappedCard card, List<Player> others = null)
        {
            foreach (ProhibitSkill skill in prohibit_skills)
            {
                if (skill.IsProhibited(room, from, to, card, others))
                    return skill;
            }

            return null;
        }
        public static Skill Invalid(Room room, Player player, string skill)
        {
            foreach (InvalidSkill sk in invalid_skills)
                if (sk.Invalid(room, player, skill))
                    return sk;

            return null;
        }
        public static int CorrectAttackRange(Room room, Player target, bool include_weapon = true, bool disctance_fixed = false)
        {
            int extra = 0;

            foreach (AttackRangeSkill skill in attackrange_skills)
            {
                if (disctance_fixed)
                {
                    int f = skill.GetFixed(room, target, include_weapon);
                    if (f > extra)
                        extra = f;
                }
                else
                {
                    extra += skill.GetExtra(room, target, include_weapon);
                }
            }

            return extra;
        }
        public static int CorrectCardTarget(Room room, TargetModSkill.ModType type, Player from, WrappedCard card)
        {
            int x = 0;

            if (type == TargetModSkill.ModType.Residue)
            {
                foreach (TargetModSkill skill in targetmod_skills)
                {
                    ExpPattern p = new ExpPattern(skill.Pattern);
                    if (p.Match(from, room, card))
                    {
                        int residue = skill.GetResidueNum(room, from, card);
                        if (residue >= 998) return residue;
                        x += residue;
                    }
                }
            }
            else if (type == TargetModSkill.ModType.ExtraMaxTarget)
            {
                foreach (TargetModSkill skill in targetmod_skills)
                {
                    ExpPattern p = new ExpPattern(skill.Pattern);
                    if (p.Match(from, room, card))
                        x += skill.GetExtraTargetNum(room, from, card);
                }
            }

            return x;
        }
        public static bool CorrectCardTarget(Room room, TargetModSkill.ModType type, Player from, Player to, WrappedCard card)
        {
            if (type == TargetModSkill.ModType.DistanceLimit)
            {
                foreach (TargetModSkill skill in targetmod_skills)
                {
                    ExpPattern p = new ExpPattern(skill.Pattern);
                    if (p.Match(from, room, card) && skill.GetDistanceLimit(room, from, to, card)) return true;
                }
            }
            else if (type == TargetModSkill.ModType.SpecificAssignee)
            {
                foreach (TargetModSkill skill in targetmod_skills)
                {
                    ExpPattern p = new ExpPattern(skill.Pattern);
                    if (p.Match(from, room, card) && skill.CheckSpecificAssignee(room, from, to, card)) return true;
                }
            }
            else if (type == TargetModSkill.ModType.History)
            {
                foreach (TargetModSkill skill in targetmod_skills)
                {
                    ExpPattern p = new ExpPattern(skill.Pattern);
                    if (p.Match(from, room, card) && skill.IgnoreCount(room, from, card)) return true;
                }
            }

            return false;
        }
        public static int CorrectDistance(Room room, Player from, Player to, WrappedCard card = null)
        {
            int correct = 0;

            foreach (DistanceSkill skill in distance_skills)
                correct += skill.GetCorrect(room, from, to, card);

            return correct;
        }
        public static int GetFixedDistance(Room room, Player from, Player to)
        {
            foreach (DistanceSkill skill in distance_skills)
            {
                int distance_fixed = skill.GetFixed(room, from, to);
                if (distance_fixed > 0) return distance_fixed;
            }

            return 0;
        }
        public static FixCardSkill IsCardFixed(Room room, Player from, Player to, string flag, HandlingMethod method)
        {
            foreach (FixCardSkill skill in fixcard_skills)
            {
                if (skill.IsCardFixed(room, from, to, flag, method))
                    return skill;
            }

            return null;
        }
        public static List<ViewHasSkill> ViewHasArmorEffect(Room room, Player player)
        {

            List<ViewHasSkill> skills = new List<ViewHasSkill>();
            foreach (ViewHasSkill skill in viewhas_skills)
            {
                foreach (string armor in skill.Armors)
                {
                    if (skill.ViewHas(room, player, armor))
                        skills.Add(skill);
                }
            }

            return skills;
        }
        public static int CorrectMaxCards(Room room, Player target, bool fix)
        {
            int extra = (fix ? -1 : 0);

            foreach (MaxCardsSkill skill in maxcards_skills)
            {
                if (fix)
                {
                    int f = skill.GetFixed(room, target);
                    if (f > extra)
                        extra = f;
                }
                else
                {
                    extra += skill.GetExtra(room, target);
                }
            }

            return extra;
        }
        #endregion

        public static CardPattern GetPattern(string name)
        {
            if (name == null)
                name = string.Empty;

            if (patterns.ContainsKey(name))
            {
                CardPattern ptn = patterns[name];
                if (ptn != null) return ptn;
            }

            ExpPattern expptn = new ExpPattern(name);
            enginePatterns.Add(expptn);
            patterns.Add(name, expptn);

            return expptn;
        }

        public static bool MatchExpPattern(Room room, string pattern, Player player, WrappedCard card)
        {
            if (card.HasFlag("using")) return false;
            ExpPattern p = (ExpPattern)GetPattern(pattern);
            return p.Match(player, room, card);
        }

        public static bool MatchExpPatternType(string pattern, WrappedCard card)
        {
            ExpPattern p = (ExpPattern)GetPattern(pattern);
            return p.Match(card);
        }
        public static HandlingMethod GetCardHandlingMethod(string method_name)
        {
            if (method_name == "use")
                return HandlingMethod.MethodUse;
            else if (method_name == "response")
                return HandlingMethod.MethodResponse;
            else if (method_name == "discard")
                return HandlingMethod.MethodDiscard;
            else if (method_name == "recast")
                return HandlingMethod.MethodRecast;
            else if (method_name == "pindian")
                return HandlingMethod.MethodPindian;
            else if (method_name == "get")
                return HandlingMethod.MethodGet;
            else if (method_name == "none")
                return HandlingMethod.MethodNone;
            else
            {
                return HandlingMethod.MethodNone;
            }
        }


        public static GameMode GetMode(string name)
        {
            if (game_modes.ContainsKey(name))
                return game_modes[name];
            else
                return new GameMode();
        }

        public static string GetMappedRole(String kingdom)
        {
            switch (kingdom)
            {
                case "wei":
                    return "lord"; ;
                case "shu":
                    return "loyalist";
                case "wu":
                    return "rebel";
                case "qun":
                    return "renegade";
                default:
                    return "careerist";
            }
        }
    }
}
