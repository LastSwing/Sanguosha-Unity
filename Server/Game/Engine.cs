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
using SanguoshaServer.AI;
using static SanguoshaServer.Package.FunctionCard;
using System.Diagnostics;
using SanguoshaServer.Extensions;
using static CommonClass.Game.Player;

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
        private static DataTable general_skin;
        private static DataTable show_avatar;
        private static DataTable show_frame;
        private static DataTable show_bg;
        private static DataTable bot_skills_lines;
        private static DataTable bots;
        private static List<string> bot_names = new List<string>();

        private static DataSet ai_values = new DataSet();
        private static Dictionary<string, List<int>> mode_card_ids = new Dictionary<string, List<int>>();
        private static Dictionary<string, List<int>> package_card_ids = new Dictionary<string, List<int>>();
        private static Dictionary<string, Dictionary<string, General>> generals = new Dictionary<string, Dictionary<string, General>>();
        private static Dictionary<string, List<string>> pack_generals = new Dictionary<string, List<string>>();
        private static Dictionary<string, List<string>> related_skills = new Dictionary<string, List<string>>();
        private static Dictionary<string, List<General>> convert_pairs = new Dictionary<string, List<General>>();
        private static Dictionary<string, SkillEvent> ai_skill_event = new Dictionary<string, SkillEvent>();
        private static Dictionary<string, UseCard> ai_card_event = new Dictionary<string, UseCard>();
        private static List<string> huashen_baned = new List<string>();

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

        private static readonly List<Title> titles = new List<Title>();

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

            //读取AI基本数据
            LoadAIValues();

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
                    if (t != null)
                    {
                        GeneralPackage pack = (GeneralPackage)Activator.CreateInstance(t);
                        foreach (FunctionCard card in pack.SkillCards)
                        {
                            Debug.Assert(!function_cards.ContainsKey(card.Name), string.Format("duplicated skill card {0} in  package {1}", card.Name, pack.Name));
                            function_cards.Add(card.Name, card);
                        }
                        foreach (string key in pack.RelatedSkills.Keys)
                        {
                            Debug.Assert(!related_skills.ContainsKey(key), string.Format("duplicated related skill {0} in  package {1}", key, pack.Name));
                            related_skills.Add(key, pack.RelatedSkills[key]);
                        }
                        AddSkills(pack.Skills);
                        foreach (string key in pack.Patterns.Keys)
                            patterns.Add(key, pack.Patterns[key]);
                    }
                }
                //创建卡牌包中的卡牌、技能卡、技能
                foreach (string package in mode.CardPackage)
                {
                    //反射创建
                    Type t = Type.GetType("SanguoshaServer.Package." + package);
                    if (t != null)
                    {
                        CardPackage pack = (CardPackage)Activator.CreateInstance(t);
                        foreach (FunctionCard card in pack.Cards)
                        {
                            System.Diagnostics.Debug.Assert(!function_cards.ContainsKey(card.Name), string.Format("duplicated card {0} in  package {1}", card.Name, pack.Name));
                            if (!function_cards.ContainsKey(card.Name))
                                function_cards.Add(card.Name, card);
                        }

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

                        foreach (string key in pack.RelatedSkills.Keys)
                            related_skills.Add(key, pack.RelatedSkills[key]);
                    }

                }
                //创建AI
                foreach (string package in mode.GeneralPackage)
                {
                    //反射创建
                    Type t = Type.GetType(string.Format("SanguoshaServer.AI.{0}AI", package));
                    if (t != null)
                    {
                        AIPackage pack = (AIPackage)Activator.CreateInstance(t);
                        foreach (SkillEvent e in pack.Events)
                        {
                            Debug.Assert(!ai_skill_event.ContainsKey(e.Name), string.Format("{0} skill event {1} duplicated", pack.Name, e.Name));
                            ai_skill_event[e.Name] = e;
                        }

                        foreach (UseCard e in pack.UseCards)
                        {
                            Debug.Assert(!ai_card_event.ContainsKey(e.Name), string.Format("{0} use card {1} duplicated", pack.Name, e.Name));
                            ai_card_event[e.Name] = e;
                        }
                    }
                }
                foreach (string package in mode.CardPackage)
                {
                    //反射创建
                    Type t = Type.GetType(string.Format("SanguoshaServer.AI.{0}AI", package));
                    if (t != null)
                    {
                        AIPackage pack = (AIPackage)Activator.CreateInstance(t);
                        foreach (SkillEvent e in pack.Events)
                        {
                            Debug.Assert(!ai_skill_event.ContainsKey(e.Name), string.Format("{0} skill event {1} duplicated", pack.Name, e.Name));
                            ai_skill_event[e.Name] = e;
                        }

                        foreach (UseCard e in pack.UseCards)
                        {
                            Debug.Assert(!ai_card_event.ContainsKey(e.Name), string.Format("{0} use card {1} duplicated", pack.Name, e.Name));
                            ai_card_event[e.Name] = e;
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
        
        private void LoadAIValues()
        {
            string sql = "select * from ai_basic_card_value";
            DataTable dt = DB.GetData(sql, false);
            dt.TableName = "card_values";
            ai_values.Tables.Add(dt);

            sql = "select * from ai_skill_coop_value";
            dt = DB.GetData(sql, false);
            dt.TableName = "skill_coop_value";
            ai_values.Tables.Add(dt);

            sql = "select * from ai_skill_pair_value";
            dt = DB.GetData(sql, false);
            dt.TableName = "skill_pair_value";
            ai_values.Tables.Add(dt);

            sql = "select * from ai_skill_value";
            dt = DB.GetData(sql, false);
            dt.TableName = "skill_value";
            ai_values.Tables.Add(dt);

            sql = "select * from ai_skill_classify";
            dt = DB.GetData(sql, false);
            dt.TableName = "skill_classify";
            ai_values.Tables.Add(dt);

            sql = "select * from ai_general_value";
            dt = DB.GetData(sql, false);
            dt.TableName = "general_value";
            ai_values.Tables.Add(dt);

            sql = "select * from role_tendency";
            dt = DB.GetData(sql, false);
            dt.TableName = "role_tendency";
            ai_values.Tables.Add(dt);
        }

        private void LoatOthers()
        {
            //武将称号
            string sql = "select * from title";
            DataTable dt = DB.GetData(sql, false);
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);

            File.Delete("gamedata/titles.json");
            File.AppendAllText("gamedata/titles.json", JsonUntity.DataSet2Json(ds));

            //皮肤信息
            sql = "select * from general_skin";
            general_skin = DB.GetData(sql, false);
            ds = new DataSet();
            ds.Tables.Add(general_skin);

            //技能台词
            sql = "select * from general_lines";
            DataTable lines = DB.GetData(sql, false);
            lines.TableName = "lines";
            ds.Tables.Add(lines);

            File.Delete("gamedata/skin.json");
            File.AppendAllText("gamedata/skin.json", JsonUntity.DataSet2Json(ds));

            //三国秀
            sql = "select * from show_avatar";
            show_avatar = DB.GetData(sql, false);
            show_avatar.TableName = "avatar";

            sql = "select * from show_frame";
            show_frame = DB.GetData(sql, false);
            show_frame.TableName = "frame";

            sql = "select * from show_bg";
            show_bg = DB.GetData(sql, false);
            show_bg.TableName = "bg";

            ds = new DataSet();
            ds.Tables.Add(show_avatar);
            ds.Tables.Add(show_frame);
            ds.Tables.Add(show_bg);

            File.Delete("gamedata/show.json");
            File.AppendAllText("gamedata/show.json", JsonUntity.DataSet2Json(ds));

            //bot & lines
            sql = "select * from bot_lines";
            bots = DB.GetData(sql, false);
            foreach (DataRow row in bots.Rows)
            {
                bot_names.Add(row["id"].ToString());
            }

            sql = "select * from bot_skill_lines";
            bot_skills_lines = DB.GetData(sql, false);

            //称号、成就收集器
            sql = "select * from title";
            DataTable title_table = DB.GetData(sql, false);

            foreach (DataRow data in title_table.Rows)
            {
                string title_name = data["title_name"].ToString();
                //反射创建
                Type t = Type.GetType("SanguoshaServer.Extensions." + title_name);
                if (t != null)
                {
                    int id = int.Parse(data["title_id"].ToString());
                    object[] pa = { id };
                    Title title = (Title)Activator.CreateInstance(t, pa);
                    titles.Add(title);
                }
            }

            //左慈化身禁表
            sql = "select * from huashen_ban_list";
            DataTable ban_table = DB.GetData(sql, false);
            foreach (DataRow row in ban_table.Rows)
            {
                huashen_baned.Add(row["general"].ToString());
            }
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
                        IsScenario = bool.Parse(r["is_scenario"].ToString()),
                        GeneralPackage = new List<string>(),
                        CardPackage = new List<string>(),
                    };
                    game_modes.Add(mode_name, mode);

                    //反射创建
                    Type t = Type.GetType("SanguoshaServer.Scenario." + mode_name);
                    GameScenario scenario = (GameScenario)Activator.CreateInstance(t);
                    scenarios.Add(mode_name, scenario);
                    AddSkills(scenario.Skills);
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
                bool enable = bool.Parse(r["enable"].ToString());
                if (enable)
                {
                    string mode_name = r["mode"].ToString();
                    string package_name = r["package_name"].ToString();
                    game_modes[mode_name].GeneralPackage.Add(package_name);
                }
            }
            //读取模式对应的卡牌包
            sql = "select * from card_package";
            dt = DB.GetData(sql, false);
            dView = dt.DefaultView;
            dView.Sort = "index asc";
            dt = dView.ToTable();
            foreach (DataRow r in dt.Rows)
            {
                if (bool.Parse(r["enable"].ToString()))
                    game_modes[r["mode"].ToString()].CardPackage.Add(r["package_name"].ToString());
            }

            File.Delete("gamedata/gamemode.json");
            File.AppendAllText("gamedata/gamemode.json", JsonUntity.Dictionary2Json(game_modes));
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
                CardSuit suit = GetSuit(row["suit"].ToString());
                int number = int.Parse(row["number"].ToString());
                bool can_recast = bool.Parse(row["can_recast"].ToString());
                bool transferable = bool.Parse(row["transferable"].ToString());
                //public WrappedCard(string name, int id, CardSuit suit, int number, bool can_recast = false, bool transferable = false)

                wrapped_cards.Add(id, new WrappedCard(name, id, suit, number, can_recast, transferable));
            }
            DataSet card_table = new DataSet();
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
                                                                //按模式给卡牌信息分类
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
            foreach (string mode in game_modes.Keys)
            {
                generals[mode] = new Dictionary<string, General>();
                foreach (string pack in game_modes[mode].GeneralPackage)
                {
                    List<string> general_names = new List<string>();
                    DataRow[] rows = table.Select(string.Format("package = '{0}'", pack));
                    foreach (DataRow row in rows)
                    {
                        string name = row["general_name"].ToString();
                        string kingdom = row["kingdom"].ToString();
                        int double_max_hp = int.Parse(row["HP"].ToString());
                        bool lord = bool.Parse(row["lord"].ToString());
                        bool male = bool.Parse(row["sex"].ToString());
                        bool selectable = bool.Parse(row["selectable"].ToString());
                        bool hidden = bool.Parse(row["hidden"].ToString());
                        int hp_adjust = int.Parse(row["adjust_hp"].ToString());
                        string main = row["main"].ToString();

                        General general = new General(name, kingdom, lord, pack, double_max_hp, male, selectable, hidden);
                        if (hp_adjust > 0)
                            general.Head_max_hp_adjusted_value = -hp_adjust;
                        else
                            general.Deputy_max_hp_adjusted_value = hp_adjust;

                        string companion = row["companion"].ToString();
                        if (!string.IsNullOrEmpty(companion))
                            general.Companions.AddRange(companion.Split(','));

                        generals[mode].Add(name, general);              //按模式分类武将
                        general_names.Add(name);

                        if (!string.IsNullOrEmpty(main))                //添加主武将
                        {
                            if (convert_pairs.ContainsKey(main))
                                convert_pairs[main].Add(general);
                            else
                                convert_pairs[main] = new List<General> { general };
                        }
                    }
                    pack_generals[pack] = general_names;                //按卡牌包给武将分类
                }
            }

            File.Delete("gamedata/generals.json");
            File.AppendAllText("gamedata/generals.json", JsonUntity.Dictionary2Json(pack_generals));
        }

        public static GameScenario GetScenario(string name)
        {
            if (scenarios.Keys.Contains(name))
                return scenarios[name];

            return null;
        }

        #region 机器人相关
        public static DataRow GetRandomBot(List<string> used)
        {
            List<string> names = new List<string>();
            foreach (string name in bot_names)
            {
                if (!used.Contains(name))
                    names.Add(name);
            }
            Shuffle.shuffle(ref names);
            string result = names[0];

            return bots.Select(string.Format("id = '{0}'", result))[0];
        }
        public static List<string> GetBotsNames() => new List<string>(bot_names);

        public static DataRow GetLines(string id)
        {
            return bots.Select(string.Format("id = '{0}'", id))[0];
        }

        public static DataRow[] GetSkillShowingLines()
        {
            return bot_skills_lines.Select("trigger = 'show'");
        }
        public static DataRow[] GetStarPlayLines()
        {
            return bot_skills_lines.Select("trigger = 'play'");
        }
        #endregion

        #region 卡牌相关
        public static List<int> GetGameCards(List<string> packages)
        {
            List<int> ids = new List<int>();
            foreach (string name in package_card_ids.Keys)
                if (packages.Contains(name))
                    ids.AddRange(package_card_ids[name]);

            return ids;
        }
        public static WrappedCard GetRealCard(int id)
        {
            Debug.Assert(id > -1);
            if (wrapped_cards.ContainsKey(id))
            {
                Debug.Assert(wrapped_cards[id] != null);
                return wrapped_cards[id];
            }
            else
                return null;
        }

        public static List<int> GetEngineCards() => new List<int>(wrapped_cards.Keys);

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
                Flags = new List<string>(card.Flags),
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
        public static List<string> GetGenerals(List<string> packages, string mode, bool include_unselectable = true)
        {
            List<string> generals = new List<string>();
            foreach (string key in pack_generals.Keys)
            {
                if (packages.Contains(key))
                {
                    foreach (string name in pack_generals[key])
                    {
                        General general = GetGeneral(name, mode);
                        if (general != null && (include_unselectable || general.Selectable))
                            generals.Add(general.Name);
                    }
                }
            }

            return generals;
        }

        public static List<General> GetConverPairs(string name)
        {
            if (convert_pairs.ContainsKey(name))
                return convert_pairs[name];
            else
                return new List<General>();
        }

        public static string GetMainGeneral(General general)
        {
            foreach (string name in convert_pairs.Keys)
            {
                if (name == general.Name || convert_pairs[name].Contains(general))
                    return name;
            }

            return general.Name;
        }

        public static General GetGeneral(string name, string mode)
        {
            if (generals.ContainsKey(mode) && generals[mode].ContainsKey(name))
                return generals[mode][name];
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
                System.Diagnostics.Debug.Assert(!skills.ContainsKey(skill.Name), string.Format("duplicated skill {0}", skill.Name));
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

        public static List<TriggerSkill> GetPublicTriggerSkills() => public_trigger_skills;
        public static List<Skill> GetEquipTriggerSkills()
        {
            List<Skill> skills = new List<Skill>();
            foreach (string p in pack_equip_triggerskills.Keys)
                    skills.AddRange(pack_equip_triggerskills[p]);

            return skills;
        }
        public static Skill GetMainSkill(string skill_name)
        {
            Skill skill = GetSkill(skill_name);
            if (skill != null && (skill.Visible|| related_skills.Keys.Contains(skill_name))) return skill;
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
                else if (flag == "treasure" && skill.Treasures.Contains(skill_name) && skill.ViewHas(room, player, skill_name))
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

        public static Skill IsProhibited(Room room, Player from, Player to, ProhibitSkill.ProhibitType type)
        {
            foreach (ProhibitSkill skill in prohibit_skills)
            {
                if (skill.IsProhibited(room, from, to, type))
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
            switch (type)
            {
                case TargetModSkill.ModType.Residue:
                    {
                        foreach (TargetModSkill skill in targetmod_skills)
                        {
                            CardPattern p = GetPattern(skill.Pattern);
                            if (p.Match(from, room, card))
                            {
                                int residue = skill.GetResidueNum(room, from, card);
                                if (residue >= 998) return residue;
                                x += residue;
                            }
                        }

                        break;
                    }

                case TargetModSkill.ModType.ExtraMaxTarget:
                    {
                        foreach (TargetModSkill skill in targetmod_skills)
                        {
                            CardPattern p = GetPattern(skill.Pattern);
                            if (p.Match(from, room, card))
                                x += skill.GetExtraTargetNum(room, from, card);
                        }

                        break;
                    }
            }

            return x;
        }
        public static bool CorrectCardTarget(Room room, TargetModSkill.ModType type, Player from, Player to, WrappedCard card)
        {
            switch (type)
            {
                case TargetModSkill.ModType.DistanceLimit:
                    {
                        CardUseStruct.CardUseReason reason = room.GetRoomState().GetCurrentCardUseReason();
                        string pattern = room.GetRoomState().GetCurrentCardUsePattern(from);
                        foreach (TargetModSkill skill in targetmod_skills)
                        {
                            CardPattern p = GetPattern(skill.Pattern);
                            if (p.Match(from, room, card) && skill.GetDistanceLimit(room, from, to, card, reason, pattern)) return true;
                        }

                        break;
                    }

                case TargetModSkill.ModType.SpecificAssignee:
                    {
                        CardUseStruct.CardUseReason reason = room.GetRoomState().GetCurrentCardUseReason();
                        string pattern = room.GetRoomState().GetCurrentCardUsePattern(from);
                        foreach (TargetModSkill skill in targetmod_skills)
                        {
                            CardPattern p = GetPattern(skill.Pattern);
                            if (p.Match(from, room, card) && skill.CheckSpecificAssignee(room, from, to, card, pattern)) return true;
                        }

                        break;
                    }

                case TargetModSkill.ModType.SpecificTarget:
                    {
                        foreach (TargetModSkill skill in targetmod_skills)
                        {
                            CardPattern p = GetPattern(skill.Pattern);
                            if (p.Match(from, room, card) && skill.CheckSpecificTarget(room, from, to, card)) return true;
                        }

                        break;
                    }

                case TargetModSkill.ModType.History:
                    {
                        foreach (TargetModSkill skill in targetmod_skills)
                        {
                            CardPattern p = GetPattern(skill.Pattern);
                            if (p.Match(from, room, card) && skill.IgnoreCount(room, from, card)) return true;
                        }

                        break;
                    }

                case TargetModSkill.ModType.AttackRange:
                    {
                        foreach (TargetModSkill skill in targetmod_skills)
                        {
                            CardPattern p = GetPattern(skill.Pattern);
                            if (p.Match(from, room, card) && skill.InAttackRange(room, from, to, card)) return true;
                        }

                        break;
                    }
            }

            return false;
        }
        public static List<SkillEvent> GetSkillEvents()
        {
            return new List<SkillEvent>(ai_skill_event.Values);
        }

        public static List<TriggerSkill> GetGlobalTriggerSkills() => global_trigger_skills;

        public static List<UseCard> GetCardUsages()
        {
            return new List<UseCard>(ai_card_event.Values);
        }
        public static Dictionary<string, double> GetSkillPairAdjust()
        {
            Dictionary<string, double> result = new Dictionary<string, double>();
            foreach (DataRow row in ai_values.Tables["skill_pair_value"].Rows)
                result.Add(row["skills"].ToString(), double.Parse(row["value"].ToString()));

            return result;
        }

        public static double GetSkillValue(string skill)
        {
            DataRow[] rows = ai_values.Tables["skill_value"].Select(string.Format("skill_name = '{0}'", skill));
            if (rows.Length > 0)
                return double.Parse(rows[0]["value"].ToString());

            return 0;
        }

        public static double GetGeneralValue(string general, string mode)
        {
            DataRow[] rows = ai_values.Tables["general_value"].Select(string.Format("general = '{0}' and mode = '{1}'", general, mode));
            if (rows.Length > 0)
                return double.Parse(rows[0]["value"].ToString());

            return 4;
        }
        public static bool IsAISelectable(string general, string mode)
        {
            DataRow[] rows = ai_values.Tables["general_value"].Select(string.Format("general = '{0}' and mode = '{1}'", general, mode));
            if (rows.Length > 0)
                return bool.Parse(rows[0]["ai_select"].ToString());

            return false;
        }

        static readonly Dictionary<PlayerRole, string> role_map = new Dictionary<PlayerRole, string>
        {
            { PlayerRole.Lord, "lord" },
            { PlayerRole.Loyalist, "loyalist" },
            { PlayerRole.Rebel, "rebel"  },
            { PlayerRole.Renegade, "renegade"  },
        };
        public static int GetRoleTendency(string general, PlayerRole role)
        {
            DataRow[] rows = ai_values.Tables["role_tendency"].Select(string.Format("general = '{0}'", general));
            if (rows.Length > 0)
                return int.Parse(rows[0][role_map[role]].ToString());

            return 0;
        }

        public static SkillEvent GetSkillEvent(string skill)
        {
            if (ai_skill_event.ContainsKey(skill))
                return ai_skill_event[skill];

            return null;
        }

        public static UseCard GetCardUsage(string class_name)
        {
            if (ai_card_event.ContainsKey(class_name))
                return ai_card_event[class_name];

            return null;
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

        public static bool IgnoreHandCard(Room room, Player player, int card_id)
        {
            foreach (MaxCardsSkill skill in maxcards_skills)
                if (skill.Ingnore(room, player, card_id))
                    return true;

            return false;
        }
        #endregion

        public static CardPattern GetPattern(string name)
        {
            lock (patterns)
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
                patterns[name] = expptn;

                return expptn;
            }
        }

        public static bool MatchExpPattern(Room room, string pattern, Player player, WrappedCard card)
        {
            if (card.HasFlag("using")) return false;
            ExpPattern p = (ExpPattern)GetPattern(pattern);
            return p.Match(player, room, card);
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

        public static List<DataRow> GetGeneralSkin(string name, string mode)
        {
            List<DataRow> selected = new List<DataRow>(general_skin.Select(string.Format("general_name = '{0}' and mode = '{1}'", name, mode)));
            List<DataRow> selected2 = new List<DataRow>(general_skin.Select(string.Format("general_name = '{0}' and mode = ''", name)));

            foreach (DataRow row2 in selected2)
            {
                bool app = true;
                foreach (DataRow row1 in selected)
                {
                    if (row2["skin_id"].ToString() == row1["skin_id"].ToString())
                    {
                        app = false;
                        break;
                    }
                }
                if (app) selected.Add(row2); 
            }

            return selected;
        }

        public static bool CheckShwoAvailable(CommonClassLibrary.Profile profile)
        {
             return show_avatar.Select("id = " + profile.Avatar).Length > 0
                && show_frame.Select("id = " + profile.Frame).Length > 0
                && show_bg.Select("id = " + profile.Bg).Length > 0;
        }
        public static List<string> GetHuashenBanList() => huashen_baned;
        public static List<Title> GetTitleCollector() => titles;

        #region AI数据
        public static Dictionary<string, double> GetSkillCoopAdjust(string mode)
        {
            Dictionary<string, double> result = new Dictionary<string, double>();
            foreach (DataRow data in ai_values.Tables["skill_coop_value"].Select(string.Format("mode = '{0}'", mode)))
                result[data["skills"].ToString()] = double.Parse(data["value"].ToString());

            return result;
        }
        public static double GetCardUseValue(string name)
        {
            DataRow[] rows = ai_values.Tables["card_values"].Select(string.Format("card_name = '{0}'", name));
            if (rows.Length > 0) return double.Parse(rows[0]["use_value"].ToString());

            return 0;
        }
        public static double GetCardKeepValue(string name)
        {
            DataRow[] rows = ai_values.Tables["card_values"].Select(string.Format("card_name = '{0}'", name));
            if (rows.Length > 0) return double.Parse(rows[0]["keep_value"].ToString());

            return 0;
        }
        public static double GetCardPriority(string name)
        {
            DataRow[] rows = ai_values.Tables["card_values"].Select(string.Format("card_name = '{0}'", name));
            if (rows.Length > 0) return double.Parse(rows[0]["priority"].ToString());

            return 0;
        }

        public static string GetSkills(string classify)
        {
            DataRow[] rows = ai_values.Tables["skill_classify"].Select(string.Format("type = '{0}'", classify));
            if (rows.Length > 0) return rows[0]["skills"].ToString();

            return string.Empty;
        }
        #endregion

    }
}
