using CommonClass;
using CommonClass.Game;
using CommonClassLibrary;
using SanguoshaServer.AI;
using SanguoshaServer.Package;
using SanguoshaServer.Scenario;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using static CommonClass.Game.Player;
using static SanguoshaServer.Game.Skill;
using static SanguoshaServer.Package.FunctionCard;
using CommandType = CommonClassLibrary.CommandType;

namespace SanguoshaServer.Game
{
    public delegate void BroadcastRoomDelegate(Room room);
    public delegate void RemoveRoomDelegate(Room room, Client host, List<Client> clients);
    public class Room : IDisposable
    {
        public Client Host { get; private set; }
        public Player Current { get; private set; }

        public List<Player> GetAlivePlayers() => new List<Player>(_alivePlayers);

        public List<Client> Clients { get; private set; } = new List<Client>();
        public List<Player> Players => new List<Player>(m_players);
        public RoomThread RoomThread { get; private set; }
        public int RoomId { get; }
        public bool GameStarted { get; private set; }
        public List<int> RoomCards => new List<int>(m_cards.Keys);
        public List<FunctionCard> AvailableFunctionCards { private set; get; } = new List<FunctionCard>();
        public GameSetting Setting { set; get; }
        public GameScenario Scenario { get; private set; }
        public List<string> Generals { get; private set; } = new List<string>();
        public List<string> Skills { get; private set; } = new List<string>();
        public List<string> UsedGeneral { get; private set; } = new List<string>();
        public List<int> DiscardPile => m_discardPile;
        public int Round { get; private set; }
        public bool Finished { get; private set; }
        public bool BloodBattle { set; get; }
        public List<int> DrawPile => m_drawPile;
        public List<TrustedAI> AIs => new List<TrustedAI>(player_ai.Values);
        public int NullTimes { get; private set; }
        public bool HegNull { get; set; }
        public bool HegNullEffected { get; private set; }
        public bool RoomTerminated { get; private set; } = false;
        public int SwapTime { get; private set; } = 0;
        public bool SkipGameRule { set; get; } = false;
        public string PreWinner { private set; get; }
        public GameHall Hall { get; private set; }
        public ConcurrentDictionary<Interactivity, bool> Surrender = new ConcurrentDictionary<Interactivity, bool>();

        private Thread thread;
        private Dictionary<Player, Client> player_client = new Dictionary<Player, Client>();
        private Dictionary<Player, TrustedAI> player_ai = new Dictionary<Player, TrustedAI>();
        private RoomState _m_roomState;

        private Dictionary<int, RoomCard> m_cards = new Dictionary<int, RoomCard>();
        private Dictionary<int, Player> owner_map = new Dictionary<int, Player>();
        private readonly Dictionary<int, Place> place_map = new Dictionary<int, Place>();
        private Dictionary<string, object> tag = new Dictionary<string, object>();

        private int _m_lastMovementId;
        private List<Player> m_players = new List<Player>();
        private Dictionary<int, Interactivity> m_interactivities = new Dictionary<int, Interactivity>();
        private readonly List<Interactivity> m_watcher = new List<Interactivity>();
        private List<int> pile1 = new List<int>(), table_cards = new List<int>(), m_drawPile = new List<int>(), m_discardPile = new List<int>();
        private Queue<DamageStruct> m_damageStack = new Queue<DamageStruct>();
        private bool create_new;
        private Dictionary<WrappedCard, List<int>> card_subcards = new Dictionary<WrappedCard, List<int>>();
        private Dictionary<WrappedCard, List<Player>> heg_nullification_targets = new Dictionary<WrappedCard, List<Player>>();
        private List<CardUseStruct> m_use_list = new List<CardUseStruct>();
        private List<JudgeStruct> m_judge_list = new List<JudgeStruct>();
        private List<Player> m_dyings = new List<Player>();
        private Queue<Player> m_extra_turn = new Queue<Player>();
        private Dictionary<int, int> cards_replace = new Dictionary<int, int>();
        private readonly Dictionary<int, bool> intel_select = new Dictionary<int, bool>();

        private System.Timers.Timer timer = new System.Timers.Timer();
        //helper variables for race request function
        private bool _m_raceStarted;

        private Player _m_raceWinner;
        //private Client _m_raceClientWinner;
        private Player _m_AIraceWinner;
        public Room(GameHall hall, int room_id, Client host, GameSetting setting)
        {
            this.Hall = hall;
            this.Host = host;
            GameStarted = false;
            _m_lastMovementId = 0;
            this.RoomId = room_id;
            Setting = setting;
            Scenario = Engine.GetScenario(setting.GameMode);
            _m_roomState = new RoomState();
            timer.Elapsed += Timer1_Elapsed;

            if (Scenario != null)
            {
                OnHostInter();
                InitCallbacks();
                IdleBeforeStart();
            }
            else
                StopGame();
        }
        public Room(GameHall hall, int room_id, GameSetting setting)
        {
            Hall = hall;
            GameStarted = false;
            _m_lastMovementId = 0;
            this.RoomId = room_id;
            Setting = setting;
            Scenario = Engine.GetScenario(setting.GameMode);
            _m_roomState = new RoomState();
            timer.Elapsed += Timer1_Elapsed;

            if (Scenario != null)
            {
                seat2clients = new Dictionary<int, Client>();
                for (int i = 0; i < setting.PlayerNum; i++)
                {
                    int bot_id = Hall.GetBotId();
                    Profile profile = Bot.GetBot(this);
                    profile.UId = bot_id;
                    Client bot = new Client(Hall, profile)
                    {
                        GameRoom = RoomId,
                        Status = Client.GameStatus.bot,
                    };
                    Hall.AddBot(bot);
                    Clients.Add(bot);

                    seat2clients[i] = bot;
                    if (i == 0)
                        Host = bot;
                }

                Hall.StartGame(this);
            }
        }

        public List<Player> GetPlayers(int client_id)
        {
            List<Player>  players = new List<Player>();
            foreach (Player p in m_players)
                if (p.ClientId == client_id)
                    players.Add(p);

            return players;
        }

        public void SetCurrent(Player regular_next) => Current = regular_next;
        public bool IsFull()
        {
            return Clients.Count >= Setting.PlayerNum;
        }
        public List<int> DrawCards(Player player, int n, string reason)
        {
            DrawCardStruct _reason = new DrawCardStruct(n, player, reason);
            return DrawCards(player, _reason);
        }
        public List<int> DrawCards(Player player, DrawCardStruct reason)
        {
            List<Player> players = new List<Player> { player };
            List<DrawCardStruct> reasons = new List<DrawCardStruct> { reason };
            return DrawCards(players, reasons);
        }

        public List<int> DrawCards(List<Player> players, List<DrawCardStruct> reasons)
        {
            List<CardsMoveStruct> moves = new List<CardsMoveStruct>();
            for (int i = 0; i < players.Count; i++)
            {
                Player player = players[i];
                DrawCardStruct reason = reasons[i];
                if (!player.Alive && reason.Reason != "reform") continue;
                int n = reason.Draw;
                if (n <= 0) continue;

                if (reason.Who != null && player != reason.Who && (RoomLogic.WillBeFriendWith(this, player, reason.Who)
                    || RoomLogic.WillBeFriendWith(this, reason.Who, player, reason.Reason)))
                {
                    ResultStruct result = reason.Who.Result;
                    result.Assist += n;
                    reason.Who.Result = result;
                }
                
                List<int> card_ids = GetNCards(n, false);
                object data = card_ids;
                RoomThread.Trigger(TriggerEvent.CardDrawing, this, player, ref data);
                card_ids = (List<int>)data;
                RemoveFromDrawPile(card_ids);

                CardsMoveStruct move = new CardsMoveStruct(card_ids, player, Place.PlaceHand,
                    new CardMoveReason(MoveReason.S_REASON_DRAW, reason.Who != null ? reason.Who.Name : string.Empty, player.Name, reason.Reason, string.Empty));
                moves.Add(move);
            }
            return MoveCardsAtomic(moves, false);
        }
        public List<int> GetNCards(int n, bool update_pile_number = true)
        {
            List<int> card_ids = new List<int>();
            int m = m_drawPile.Count;
            while (n > m)
            {
                for (int i = 0; i < m; i++)
                {
                    card_ids.Add(m_drawPile[0]);
                    m_drawPile.RemoveAt(0);
                    //room_thread.Trigger(TriggerEvent.FetchDrawPileCard, this, null);
                }
                SwapPile();
                ReturnToDrawPile(card_ids, false);
                m = m_drawPile.Count;
            }

            card_ids.Clear();
            for (int i = 0; i < n; i++)
                card_ids.Add(m_drawPile[i]);

            if (update_pile_number)
                RemoveFromDrawPile(card_ids);

            return card_ids;
        }
        public void ReturnToDrawPile(List<int> cards, bool isBottom, Player move_from = null, bool open = false)
        {
            if (cards.Count == 0) return;
            if (isBottom)
            {
                foreach (int id in cards)
                {
                    SetCardMapping(id, null, Place.DrawPile);
                    m_drawPile.Remove(id);
                    m_drawPile.Add(id);
                }
            }
            else
            {
                for (int i = cards.Count - 1; i >= 0; i--)
                {
                    int id = cards[i];

                    SetCardMapping(id, null, Place.DrawPile);
                    m_drawPile.Remove(id);
                    m_drawPile.Insert(0, id);
                }
            }
            object data = m_drawPile.Count;
            RoomThread.Trigger(TriggerEvent.DrawPileChanged, this, null, ref data);
            List<string> arg = new List<string>
            {
                m_drawPile.Count.ToString()
            };
            DoBroadcastNotify(CommandType.S_COMMAND_UPDATE_PILE, arg);

            if (move_from != null)
            {
                string top = isBottom ? "ViewBottomCards:" : "ViewTopCards:";
                object decisionData = string.Format("{0}:{1}:{2}:{3}", isBottom ? "ViewBottomCards" : "ViewTopCards", move_from.Name, string.Join("+", JsonUntity.IntList2StringList(cards)),
                    open ? "open" : "private");
                RoomThread.Trigger(TriggerEvent.ChoiceMade, this, move_from, ref decisionData);
            }
        }
        public void RemoveFromDrawPile(int id)
        {
            RemoveFromDrawPile(new List<int> { id });
        }
        public void RemoveFromDrawPile(List<int> ids)
        {
            bool update = false;
            foreach (int id in ids)
            {
                if (m_drawPile.Contains(id))
                {
                    if (m_drawPile[0] != id)
                    {
                        int index = m_drawPile.IndexOf(id);
                        LogMessage log = new LogMessage("#RemoveFromDrawPile")
                        {
                            Arg = (index + 1).ToString(),
                        };
                        SendLog(log);
                    }
                    m_drawPile.Remove(id);
                    //room_thread.Trigger(TriggerEvent.FetchDrawPileCard, this, null);
                    update = true;
                }
            }

            if (update)
            {
                object data = m_drawPile.Count;
                List<string> arg = new List<string>
                {
                    m_drawPile.Count.ToString()
                };
                DoBroadcastNotify(CommandType.S_COMMAND_UPDATE_PILE, arg);
                RoomThread.Trigger(TriggerEvent.DrawPileChanged, this, null, ref data);
            }

            if (m_drawPile.Count == 0)
                SwapPile();
        }
        public void SwapPile()
        {
            if (m_discardPile.Count == 0)
            {
                // the standoff
                GameOver(".");
            }

            SwapTime++;
            int limit = 5;
            if (limit > 0 && SwapTime == limit) GameOver(".");
            DoBroadcastNotify(CommandType.S_COMMAND_RESET_PILE, new List<string> { SwapTime.ToString() });

            Shuffle.shuffle(ref m_discardPile);
            foreach (int card_id in m_discardPile)
            {
                ClearCardFlag(card_id);
                SetCardMapping(card_id, null, Place.DrawPile);
            }
            m_drawPile.AddRange(m_discardPile);
            m_discardPile.Clear();

            DoBroadcastNotify(CommandType.S_COMMAND_UPDATE_PILE, new List<string> { m_drawPile.Count.ToString() });

            object data = m_drawPile.Count;
            RoomThread.Trigger(TriggerEvent.SwapPile, this, null, ref data);
            RoomThread.Trigger(TriggerEvent.DrawPileChanged, this, null, ref data);
        }
        public void GameOver(string winner)
        {
            PreWinner = string.Empty;

            List<string> arg = new List<string> { winner };
            foreach (Player player in m_players)
            {
                Player copy = new Player();
                copy.CopyAll(player);
                arg.Add(JsonUntity.Object2Json(copy));
                if (player.HandcardNum > 0)
                {
                    List<int> cards= player.GetCards("h");
                    player.SetTag("last_handcards", cards);
                }
            }

            Finished = true;

            if (!ContainsTag("NextGameMode") || string.IsNullOrEmpty((string)GetTag("NextGameMode")))
            {
                string name = (string)GetTag("NextGameMode");
                RemoveTag("NextGameMode");
            }

            if (Scenario.Name == "Classic")
            {
                foreach (Player p in Players)
                    BroadcastProperty(p, "Role");
            }

            DoBroadcastNotify(CommandType.S_COMMAND_GAME_OVER, arg);

            object data = winner;
            RoomThread.Trigger(TriggerEvent.GameFinished, this, null, ref data);

            create_new = true;
            //记录游戏结果
            bool bot = false;
            foreach (Player p in m_players)
            {
                if (p.ClientId < 0)
                {
                    bot = true;
                    break;
                }
            }

            //当不存在AI时才记录胜负记录
            try
            {
                if (!bot)
                {
                    List<int> clients = new List<int>();
                    foreach (Player p in m_players)
                    {
                        int id = p.ClientId;
                        if (!clients.Contains(id))
                        {
                            clients.Add(id);
                            int win = 0;
                            if (winner != ".")
                            {
                                if (winner.Contains(p.Name))
                                    win = 1;
                                else
                                    win = -1;
                            }

                            ClientDBOperation.UpdateProfileGamePlay(Hall.GetClient(id), id, win, p.Status == "escape");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug(string.Format("error on updating game play {0} {1}", e.Message, e.TargetSite));
                LogHelper.WriteLog(null, e);
            }


            StopGame();
        }

        public void NotifyUsingVirtualCard(string card_str, ClientCardsMoveStruct cards_moves)
        {
            List<string> arg = new List<string>
            {
                JsonUntity.Object2Json(cards_moves),
                card_str
            };
            DoBroadcastNotify(CommandType.S_COMMAND_USE_VIRTUAL_CARD, arg);
        }

        public RoomCard GetCard(int id)
        {
            if (m_cards.ContainsKey(id))
            {
                System.Diagnostics.Debug.Assert(m_cards[id] != null, id.ToString());
                return m_cards[id];
            }

            return null;
        }
        public void Recover(Player player, RecoverStruct recover_struct, bool set_emotion = false)
        {
            if (player.GetLostHp() == 0 || !player.Alive)
                return;

            object data = recover_struct;
            if (RoomThread.Trigger(TriggerEvent.PreHpRecover, this, player, ref data))
                return;

            recover_struct = (RecoverStruct)data;
            int recover_num = recover_struct.Recover;

            int new_hp = Math.Min(player.Hp + recover_num, player.MaxHp);
            int num = new_hp - player.Hp;

            object _lose = num;
            if (!RoomThread.Trigger(TriggerEvent.HpChanging, this, player, ref _lose))
            {
                player.Hp = new_hp;
                BroadcastProperty(player, "Hp");

                List<string> arg = new List<string> { player.Name, recover_num.ToString(), "0" };
                DoBroadcastNotify(CommandType.S_COMMAND_CHANGE_HP, arg);
                if (set_emotion) SetEmotion(player, "recover");
                
                RoomThread.Trigger(TriggerEvent.HpChanged, this, player, ref _lose);

                ResultStruct new_result = player.Result;
                new_result.Recover += num;
                player.Result = new_result;

                Player from = recover_struct.Who ?? player;
                new_result = from.Result;
                new_result.Heal += num;
                from.Result = new_result;

                RoomThread.Trigger(TriggerEvent.HpRecover, this, player, ref data);
            }
        }

        public void CancelTarget(ref CardUseStruct use, Player player)
        {
            if (player == null)
                return;
            
            LogMessage log = new LogMessage();
            if (use.From != null)
            {
                log.Type = "$CancelTarget";
                log.From = use.From.Name;
            }
            else
            {
                log.Type = "$CancelTargetNoUser";
            }
            log.To = new List<string> { player.Name };
            log.Arg = use.Card.Name;
            SendLog(log);

            SetEmotion(player, "cancel");
            Thread.Sleep(400);

            int index = use.To.IndexOf(player);
            use.To.RemoveAt(index);
            if (use.EffectCount != null)
                use.EffectCount.RemoveAt(index);
            if (use.Card != null)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is Slash)
                    SlashSettlementFinished(player, use.Card);
            }
        }
        public void SlashSettlementFinished(Player player, WrappedCard slash)
        {
            RemoveQinggangTag(player, slash);

            List<string> blade_use = player.ContainsTag("blade_use") ? (List<string>)player.GetTag("blade_use") : new List<string>();
            string str = RoomLogic.CardToString(this, slash);
            if (blade_use.Contains(str))
            {
                blade_use.Remove(str);
                player.SetTag("blade_use", blade_use);

                if (blade_use.Count == 0)
                    RemovePlayerDisableShow(player, Blade.ClassName);
            }
        }

        public bool CardEffect(WrappedCard card, Player from, Player to, bool multiple = false)
        {
            CardEffectStruct effect = new CardEffectStruct
            {
                Card = card,
                From = from,
                To = to,
                Multiple = multiple,
                BasicEffect = Engine.GetFunctionCard(card.Name).FillCardBasicEffct(this, to)
            };

            return CardEffect(effect);
        }

        public void AttachSkillToPlayer(Player player, string skill_name)
        {
            player.AcquireSkill(skill_name);
            //DoBroadcastNotify(CommandType.S_COMMAND_ATTACH_SKILL, new List<string> { player.Name, skill_name });
            DoNotify(GetClient(player), CommandType.S_COMMAND_ATTACH_SKILL, new List<string> { player.Name, skill_name });
        }

        public void DetachSkillFromPlayer(Player player, string skill_name, bool is_equip = false, bool acquire_only = false, bool head = true)
        {
            Skill skill = Engine.GetSkill(skill_name);
            if (skill == null || !skill.Visible) return;
            if (head && !player.GetHeadSkillList(true, is_equip).Contains(skill_name)) return;
            if (!head && !player.GetDeputySkillList(true, is_equip).Contains(skill_name)) return;

            if (acquire_only && player.GetAcquiredSkills(head ? "head" : "deputy").Contains(skill_name))
            {
                player.DetachSkill(skill_name, head);
                List<string> args = new List<string>
                {
                    GameEventType.S_GAME_EVENT_DETACH_SKILL.ToString(),
                    player.Name,
                    skill_name,
                    head.ToString()
                };
                DoBroadcastNotify(CommandType.S_COMMAND_LOG_EVENT, args);
            }
            else if (!acquire_only)
            {
                player.LoseSkill(skill_name, head);
                List<string> args = new List<string> { GameEventType.S_GAME_EVENT_LOSE_SKILL.ToString(), player.Name, skill_name, head.ToString() };
                DoBroadcastNotify(CommandType.S_COMMAND_LOG_EVENT, args);
            }
            else
            {
                return;
            }

            if (!is_equip)
            {
                LogMessage log = new LogMessage("#LoseSkill")
                {
                    From = player.Name,
                    Arg = skill_name
                };
                SendLog(log);

                object data = new InfoStruct() { Info = skill_name, Head = head };
                RoomThread.Trigger(TriggerEvent.EventLoseSkill, this, player, ref data);
            }
        }

        //skill.CanPreShow() || head ? player.General1Showed : player.General2Showed ,

        public void HandleAcquireDetachSkills(Player player, List<string> skill_names, bool acquire_only = false)
        {
            if (skill_names == null || skill_names.Count == 0) return;
            List<bool> isLost = new List<bool>(); ;
            List<bool> isHead = new List<bool>();
            List<string> triggerList = new List<string>();
            foreach (string _skill_name in skill_names)
            {
                if (_skill_name.StartsWith("-"))
                {
                    string actual_skill = _skill_name.Substring(1);
                    bool head = true;
                    if (actual_skill.EndsWith("!"))
                    {
                        actual_skill = actual_skill.Remove(actual_skill.Length - 1);
                        head = false;
                    }
                    if (head && !player.GetAcquiredSkills("head").Contains(actual_skill) && !player.GetHeadSkillList().Contains(actual_skill)) continue;
                    if (!head && !player.GetAcquiredSkills("deputy").Contains(actual_skill) && !player.GetDeputySkillList().Contains(actual_skill)) continue;
                    if (player.GetAcquiredSkills(head ? "head" : "deputy").Contains(actual_skill))
                        player.DetachSkill(actual_skill, head);
                    else if (!acquire_only)
                        player.LoseSkill(actual_skill, head);
                    else
                    {
                        continue;
                    }

                    Skill skill = Engine.GetSkill(actual_skill);
                    if (skill != null && skill.Visible)
                    {
                        List<string> args = new List<string>
                        {
                            GameEventType.S_GAME_EVENT_DETACH_SKILL.ToString(),
                            player.Name,
                            actual_skill,
                            head.ToString()
                        };
                        if (!acquire_only) args[0] = GameEventType.S_GAME_EVENT_LOSE_SKILL.ToString();

                        DoBroadcastNotify(CommandType.S_COMMAND_LOG_EVENT, args);

                        LogMessage log = new LogMessage("#LoseSkill")
                        {
                            From = player.Name,
                            Arg = actual_skill
                        };
                        SendLog(log);

                        triggerList.Add(actual_skill);
                        isLost.Add(true);
                        isHead.Add(head);
                    }
                }
                else
                {
                    bool head = true;
                    string skill_name = _skill_name;
                    if (skill_name.EndsWith("!"))
                    {
                        skill_name = skill_name.Remove(skill_name.Length - 1);
                        head = false;
                    }
                    Skill skill = Engine.GetSkill(skill_name);
                    if (skill == null) continue;
                    if (player.GetAcquiredSkills().Contains(skill_name)) continue;
                    player.AcquireSkill(skill_name, head);

                    if (skill.SkillFrequency == Skill.Frequency.Limited && !string.IsNullOrEmpty(skill.LimitMark))
                        SetPlayerMark(player, skill.LimitMark, 1);

                    if (skill.Visible)
                    {
                        List<string> arg = new List<string>
                        {
                            GameEventType.S_GAME_EVENT_ACQUIRE_SKILL.ToString(),
                            player.Name,
                            skill_name,
                            head.ToString()
                        };
                        DoBroadcastNotify(CommandType.S_COMMAND_LOG_EVENT, arg);

                        LogMessage log = new LogMessage("#AcquireSkill")
                        {
                            From = player.Name,
                            Arg = skill_name
                        };
                        SendLog(log);

                        triggerList.Add(skill_name);
                        isLost.Add(false);
                        isHead.Add(head);
                    }
                }
            }
            if (triggerList.Count > 0)
            {
                for (int i = 0; i < triggerList.Count; i++)
                {
                    object data = new InfoStruct() { Info = triggerList[i], Head = isHead[i] };
                    RoomThread.Trigger(isLost[i] ? TriggerEvent.EventLoseSkill : TriggerEvent.EventAcquireSkill, this, player, ref data);
                }
            }
        }

        public RoomState GetRoomState()
        {
            return _m_roomState;
        }

        public void SetPlayerFlag(Player player, string flag)
        {
            if (flag.StartsWith("-"))
            {
                string set_flag = flag.Substring(1);
                if (!player.HasFlag(set_flag)) return;
            }
            player.SetFlags(flag);
            DoBroadcastNotify(CommandType.S_COMMAND_SET_FLAG, new List<string> { player.Name, flag });
        }

        public void SetPlayerMark(Player player, string mark, int value)
        {
            player.SetMark(mark, value);

            List<string> arg = new List<string>
            {
                player.Name,
                mark,
                value.ToString()
            };
            DoBroadcastNotify(CommandType.S_COMMAND_SET_MARK, arg);
        }

        public void AddPlayerMark(Player player, string mark, int add_num = 1)
        {
            int value = player.GetMark(mark);
            value += add_num;
            SetPlayerMark(player, mark, value);
        }

        public void RemovePlayerMark(Player player, string mark, int remove_num = 1)
        {
            int value = player.GetMark(mark);
            if (value == 0) return;
            value -= remove_num;
            value = Math.Max(0, value);
            SetPlayerMark(player, mark, value);
        }

        public void SetPlayerStringMark(Player player, string mark, string value, Client except = null)
        {
            player.SetStringMark(mark, value);
            List<Client> clients = new List<Client>(Clients);
            foreach (Client client in clients)
                if (client != except)
                    DoNotify(client, CommandType.S_COMMAND_SET_STRINGMARK, new List<string> { player.Name, mark, value });
        }

        public void RemovePlayerStringMark(Player player, string mark, Client except = null)
        {
            player.RemoveStringMark(mark);
            List<Client> clients = new List<Client>(Clients);
            foreach (Client client in clients)
                if (client != except)
                    DoNotify(client, CommandType.S_COMMAND_SET_STRINGMARK, new List<string> { player.Name, mark });
        }

        public void SetPlayerDisableShow(Player player, string flags, string reason)
        {
            player.SetDisableShow(flags, reason);

            List<string> arg = new List<string>
            {
                player.Name,
                true.ToString(),
                flags.ToString(),
                reason
            };
            DoBroadcastNotify(CommandType.S_COMMAND_DISABLE_SHOW, arg);
        }

        public void RemovePlayerDisableShow(Player player, string reason)
        {
            player.RemoveDisableShow(reason);

            List<string> arg = new List<string>
            {
                player.Name,
                false.ToString(),
                string.Empty,
                reason
            };
            DoBroadcastNotify(CommandType.S_COMMAND_DISABLE_SHOW, arg);
        }

        public List<int> AddToPile(Player player, string pile_name, WrappedCard card, bool open = true, List<Player> open_players = null)
        {
            List<int> card_ids = new List<int>(card.SubCards);
            return AddToPile(player, pile_name, card_ids, open, open_players);
        }

        public List<int> AddToPile(Player player, string pile_name, int card_id, bool open = true, List<Player> open_players = null)
        {
            return AddToPile(player, pile_name, new List<int> { card_id }, open, open_players);
        }

        public List<int> AddToPile(Player player, string pile_name, List<int> card_ids, bool open = true, List<Player> open_players = null)
        {
            return AddToPile(player, pile_name, card_ids, open, open_players, new CardMoveReason(MoveReason.S_REASON_REMOVE_FROM_GAME, player.Name));
        }

        public List<int> AddToPile(Player player, string pile_name, List<int> card_ids, bool open, List<Player> open_players, CardMoveReason reason)
        {
            open_players = open_players ?? new List<Player>();

            if (!open)
            {
                if (open_players.Count == 0)
                {
                    foreach (int id in card_ids)
                    {
                        Player owner = GetCardOwner(id);
                        if (owner != null && !open_players.Contains(owner))
                            open_players.Add(owner);
                    }
                }
            }
            else
            {
                open_players = new List<Player>(m_players);
            }
            foreach (Player p in open_players)
                player.SetPileOpen(pile_name, p.Name);
            //player.PileChange(pile_name, card_ids);

            CardsMoveStruct move = new CardsMoveStruct(card_ids, player, Place.PlaceSpecial, reason)
            {
                To_pile_name = pile_name
            };
            return MoveCardsAtomic(move, open);
        }

        public void SetCardFlag(WrappedCard card, string flag)
        {
            if (string.IsNullOrEmpty(flag)) return;

            card.SetFlags(flag);
            if (!card.IsVirtualCard()) SetCardFlag(card.Id, flag);
        }

        public void SetCardFlag(int card_id, string flag)
        {
            if (string.IsNullOrEmpty(flag)) return;

            GetCard(card_id).SetFlags(flag);
        }

        public void ClearCardFlag(WrappedCard card)
        {
            card.ClearFlags();

            if (!card.IsVirtualCard())
                ClearCardFlag(card.Id);
        }

        public void ClearCardFlag(int card_id)
        {
            GetCard(card_id).ClearFlags();
        }

        public void HandleAcquireDetachSkills(Player player, string skill_names, bool acquire_only = false)
        {
            HandleAcquireDetachSkills(player, new List<string>(skill_names.Split('|')), acquire_only);
        }
        public bool CardEffect(CardEffectStruct effect)
        {
            object data = effect;
            bool cancel = false;
            if (effect.To.Alive || Engine.GetFunctionCard(effect.Card.Name) is Slash)
            { // Be care!!!
              // No skills should be triggered here!
                RoomThread.Trigger(TriggerEvent.CardEffect, this, effect.To, ref data);
                // Make sure that effectiveness of Slash isn't judged here!
                if (!RoomThread.Trigger(TriggerEvent.CardEffected, this, effect.To, ref data))
                {
                    cancel = true;
                }
                else
                {
                    if (!effect.To.HasFlag("Global_NonSkillNullify"))
                    {    //setEmotion(effect.to, "skill_nullify");
                    }
                    else
                        effect.To.SetFlags("-Global_NonSkillNullify");
                }
            }

            return cancel;
        }


        public void NotifySkillInvoked(Player player, string skill_name)
        {
            if (!RoomLogic.PlayerHasSkill(this, player, skill_name)) return;
            Skill skill = Engine.GetMainSkill(skill_name);
            if (skill != null)
            {
                if (RoomLogic.PlayerHasSkill(this, player, skill_name))
                {
                    ResultStruct result = player.Result;
                    result.SkillInvoke++;
                    player.Result = result;
                }

                List<string> args = new List<string>
                {
                    GameEventType.S_GAME_EVENT_SKILL_INVOKED.ToString(),
                    player.Name,
                    skill_name,
                    ((int)skill.Skill_type).ToString()
                };
                DoBroadcastNotify(CommandType.S_COMMAND_LOG_EVENT, args);
            }
        }

        public void BroadcastUpdateCard(List<Player> players, int cardId, WrappedCard wrapped)
        {
        }

        public void SetCardMapping(int card_id, Player owner, Place place)
        {
            if (place == Place.PlaceHand)
                System.Diagnostics.Debug.Assert(!owner.GetEquips().Contains(card_id));

            owner_map[card_id] = owner;
            place_map[card_id] = (place == Place.DrawPileBottom ? Place.DrawPile : place);
        }

        public Player GetCardOwner(int card_id)
        {
            if (owner_map.ContainsKey(card_id))
                return owner_map[card_id];
            else
                return null;
        }

        public Place GetCardPlace(int card_id)
        {
            if (card_id < 0) return Place.PlaceUnknown;
            return place_map[card_id];
        }

        public List<int> GetCardIdsOnTable(WrappedCard virtual_card)
        {
            if (virtual_card == null)
                return new List<int>();

            return GetCardIdsOnTable(virtual_card.SubCards);
        }

        public List<int> GetCardIdsOnTable(List<int> card_ids)
        {
            List<int> r = new List<int>();
            foreach (int id in card_ids)
            {
                if (GetCardPlace(id) == Place.PlaceTable)
                    r.Add(id);
            }
            return r;
        }
        private void _fillMoveInfo(ref CardsMoveStruct moves, int card_index)
        {
            int card_id = moves.Card_ids[card_index];
            if (string.IsNullOrEmpty(moves.From) && GetCardOwner(card_id) != null)
                moves.From = GetCardOwner(card_id).Name;
            moves.From_place = GetCardPlace(card_id);
            if (!string.IsNullOrEmpty(moves.From))
            { // Hand/Equip/Judge
                //if (moves.From_place == Place.PlaceSpecial || moves.From_place == Place.PlaceTable)
                if (moves.From_place == Place.PlaceSpecial)
                    moves.From_pile_name = FindPlayer(moves.From, true).GetPileName(card_id);
            }
            /*
            if (!string.IsNullOrEmpty(moves.To))
            {
                //if (moves.To_place == Place.PlaceSpecial || moves.To_place == Place.PlaceTable)
                if (moves.To_place == Place.PlaceSpecial)
                    moves.To_pile_name = FindPlayer(moves.To, true).GetPileName(card_id);
            }
            */
        }

        public List<int> MoveCardTo(WrappedCard card, Player dstPlayer, Place dstPlace, bool forceMoveVisible = false)
        {
            CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_UNKNOWN, null);
            return MoveCardTo(card, dstPlayer, dstPlace, reason, forceMoveVisible);
        }
        public List<int> MoveCardTo(WrappedCard card, Player dstPlayer, Place dstPlace, CardMoveReason reason, bool forceMoveVisible = false)
        {
            return MoveCardTo(card, null, dstPlayer, dstPlace, string.Empty, reason, forceMoveVisible);
        }
        public List<int> MoveCardTo(WrappedCard card, Player srcPlayer, Player dstPlayer, Place dstPlace, CardMoveReason reason, bool forceMoveVisible = false)
        {
            return MoveCardTo(card, srcPlayer, dstPlayer, dstPlace, string.Empty, reason, forceMoveVisible);
        }
        public List<int> MoveCardTo(WrappedCard card, Player srcPlayer, Player dstPlayer, Place dstPlace, string pileName, CardMoveReason reason, bool forceMoveVisible = false)
        {
            if (card.SubCards.Count == 0) return new List<int>();
            reason.Card = card;
            CardsMoveStruct move = new CardsMoveStruct(new List<int>(card.SubCards), dstPlayer, dstPlace, reason)
            {
                To_pile_name = pileName,
                From = srcPlayer?.Name
            };

            //if (!string.IsNullOrEmpty(pileName))
            //    dstPlayer.PileChange(pileName, move.Card_ids);

            List<CardsMoveStruct> moves = new List<CardsMoveStruct> { move };
            return MoveCardsAtomic(moves, forceMoveVisible);
        }

        public void MoveCards(List<CardsMoveStruct> cards_moves, bool forceMoveVisible, bool enforceOrigin = true)
        {
            List<CardsMoveStruct> all_sub_moves = _breakDownCardMoves(cards_moves);
            _MoveCards(all_sub_moves, forceMoveVisible, enforceOrigin);
        }

        private void _MoveCards(List<CardsMoveStruct> cards_moves, bool forceMoveVisible, bool enforceOrigin)
        {
            // First, process remove card

            List<CardsMoveOneTimeStruct> moveOneTimes = _mergeMoves(cards_moves);
            int index = 0;
            foreach (CardsMoveOneTimeStruct _moveOneTime in new List<CardsMoveOneTimeStruct>(moveOneTimes))
            {
                if (_moveOneTime.Card_ids.Count == 0)
                {
                    index++;
                    continue;
                }
                Player origin_to = _moveOneTime.To;
                Place origin_place = _moveOneTime.To_place;
                Player origin_from = _moveOneTime.From;
                List<Place> origin_from_places = _moveOneTime.From_places;
                CardsMoveOneTimeStruct moveOneTime = _moveOneTime;
                moveOneTime.Origin_to_place = origin_place;
                moveOneTime.Origin_to = origin_to;
                moveOneTime.To = null;
                moveOneTime.To_place = Place.PlaceTable;
                object data = moveOneTime;
                RoomThread.Trigger(TriggerEvent.BeforeCardsMove, this, null, ref data);
                moveOneTime = (CardsMoveOneTimeStruct)data;
                moveOneTime.Origin_from_places = origin_from_places;
                moveOneTime.Origin_from = origin_from;
                moveOneTime.To = origin_to;
                moveOneTime.To_place = origin_place;
                moveOneTimes[index] = moveOneTime;
                index++;
            }

            cards_moves = _separateMoves(moveOneTimes);

            NotifyMoveCards(true, cards_moves, forceMoveVisible);
            List<CardsMoveStruct> origin = new List<CardsMoveStruct>(cards_moves);

            List<Place> final_places = new List<Place>();
            List<string> move_tos = new List<string>();
            for (int i = 0; i < cards_moves.Count; i++)
            {
                CardsMoveStruct cards_move = cards_moves[i];
                final_places.Add(cards_move.To_place);
                move_tos.Add(cards_move.To);
                cards_move.To_place = Place.PlaceTable;
                cards_move.To = null;
                cards_moves[i] = cards_move;
            }

            for (int i = 0; i < cards_moves.Count; i++)
            {
                CardsMoveStruct cards_move = cards_moves[i];
                for (int j = 0; j < cards_move.Card_ids.Count; j++)
                {
                    int card_id = cards_move.Card_ids[j];
                    Player from = FindPlayer(cards_move.From, true);
                    if (cards_move.From_place == Place.PlaceHand || cards_move.From_place == Place.PlaceEquip)
                        System.Diagnostics.Debug.Assert(from != null, card_id.ToString());
                    if (from != null) // Hand/Equip/Judge
                    {
                        RoomLogic.RemovePlayerCard(this, from, card_id, cards_move.From_place);
                        System.Diagnostics.Debug.Assert(!from.GetCards("he").Contains(card_id));
                    }
                    

                    switch (cards_move.From_place)
                    {
                        case Place.DiscardPile:
                            m_discardPile.Remove(card_id);
                            break;
                        case Place.DrawPile:
                            RemoveFromDrawPile(card_id);
                            break;
                        case Place.DrawPileBottom:
                            RemoveFromDrawPile(card_id);
                            break;
                        case Place.PlaceSpecial:
                            table_cards.Remove(card_id);
                            break;
                        default:
                            break;
                    }

                    SetCardMapping(card_id, null, Place.PlaceTable);
                }
            }

            foreach (CardsMoveStruct move in cards_moves)
                UpdateCardsOnLose(move);

            //trigger event
            moveOneTimes = _mergeMoves(cards_moves);
            foreach (CardsMoveOneTimeStruct moveOneTime in moveOneTimes)
            {
                if (moveOneTime.Card_ids.Count == 0) continue;
                object data = moveOneTime;
                RoomThread.Trigger(TriggerEvent.CardsMoveOneTime, this, null, ref data);
            }

            for (int i = 0; i < cards_moves.Count; i++)
            {
                CardsMoveStruct cards_move = cards_moves[i];
                cards_move.To = move_tos[i];
                cards_move.To_place = final_places[i];
                cards_moves[i] = cards_move;
            }

            if (enforceOrigin)
            {
                for (int i = 0; i < cards_moves.Count; i++)
                {
                    CardsMoveStruct cards_move = cards_moves[i];
                    Player to = FindPlayer(cards_move.To);
                    if (to == null)
                    {
                        cards_move.To = null;
                        cards_move.To_place = Place.DiscardPile;
                    }
                    cards_moves[i] = cards_move;
                }
            }

            for (int i = 0; i < cards_moves.Count; i++)
            {
                CardsMoveStruct cards_move = cards_moves[i];
                cards_move.From_place = Place.PlaceTable;
                cards_moves[i] = cards_move;
            }

            moveOneTimes = _mergeMoves(cards_moves);
            index = 0;
            foreach (CardsMoveOneTimeStruct moveOneTime in new List<CardsMoveOneTimeStruct>(moveOneTimes))
            {
                if (moveOneTime.Card_ids.Count == 0)
                {
                    index++;
                    continue;
                }
                object data = moveOneTime;
                RoomThread.Trigger(TriggerEvent.BeforeCardsMove, this, null, ref data);
                CardsMoveOneTimeStruct moveOneTime2 = (CardsMoveOneTimeStruct)data;
                moveOneTimes[index] = moveOneTime2;
                index++;
            }
            cards_moves = _separateMoves(moveOneTimes);

            // Now, process add cards
            for (int i = 0; i < cards_moves.Count; i++)
            {
                CardsMoveStruct cards_move = cards_moves[i];
                for (int j = 0; j < cards_move.Card_ids.Count; j++)
                    SetCardMapping(cards_move.Card_ids[j], FindPlayer(cards_move.To, true), cards_move.To_place);
            }
            foreach (CardsMoveStruct move in cards_moves)
                UpdateCardsOnGet(move);

            List<CardsMoveStruct> origin_x = new List<CardsMoveStruct>();
            foreach (CardsMoveStruct m in origin)
            {
                CardsMoveStruct m_x = m;
                m_x.Card_ids = new List<int>();
                m_x.To = null;
                m_x.To_place = Place.DiscardPile;
                foreach (int id in m.Card_ids)
                {
                    bool sure = false;
                    foreach (CardsMoveStruct cards_move in cards_moves)
                    {
                        if (cards_move.Card_ids.Contains(id))
                        {
                            m_x.Card_ids.Add(id);
                            if (!sure)
                            {
                                m_x.To = cards_move.To;
                                m_x.To_place = cards_move.To_place;
                                sure = true;
                            }
                        }
                    }
                }
                origin_x.Add(m_x);
            }
            NotifyMoveCards(false, origin_x, forceMoveVisible);

            for (int i = 0; i < cards_moves.Count; i++)
            {
                CardsMoveStruct cards_move = cards_moves[i];
                for (int j = 0; j < cards_move.Card_ids.Count; j++)
                {
                    int card_id = cards_move.Card_ids[j];
                    WrappedCard card = GetCard(card_id);
                    if (forceMoveVisible && cards_move.To_place == Place.PlaceHand)
                        card.SetFlags("visible");
                    else
                        card.SetFlags("-visible");

                    Player to = null;
                    if (!string.IsNullOrEmpty(cards_move.To)) // Hand/Equip/Judge
                        to = FindPlayer(cards_move.To, true);

                    if (to != null) // Hand/Equip/Judge
                    {
                        RoomLogic.AddPlayerCard(this, to, card_id, cards_move.To_place);
                        if (cards_move.To_place == Place.PlaceHand || cards_move.To_place == Place.PlaceEquip)
                            System.Diagnostics.Debug.Assert(to.GetCards("he").Contains(card_id) && GetCardOwner(card_id) == to && GetCardPlace(card_id) == cards_move.To_place);
                        else if (cards_move.To_place == Place.PlaceDelayedTrick)
                            System.Diagnostics.Debug.Assert(to.JudgingArea.Contains(card_id) && GetCardOwner(card_id) == to && GetCardPlace(card_id) == cards_move.To_place);
                    }
                    if (cards_move.To_place == Place.PlaceSpecial)
                        to.PileChange(cards_move.To_pile_name, new List<int> { card_id });

                    Player from = null;
                    if (!string.IsNullOrEmpty(cards_move.From)) from = FindPlayer(cards_move.From);
                    bool open = forceMoveVisible || cards_move.From_place == Place.PlaceEquip;

                    switch (cards_move.To_place)
                    {
                        case Place.DiscardPile:
                            m_discardPile.Insert(0, card_id);
                            break;
                        case Place.DrawPile:
                            ReturnToDrawPile(new List<int> { card_id }, false, from, open);
                            //m_drawPile->prepend(card_id);
                            break;
                        case Place.DrawPileBottom:
                            ReturnToDrawPile(new List<int> { card_id }, true, from, open);
                            //m_drawPile->append(card_id);
                            break;
                        case Place.PlaceSpecial:
                            table_cards.Add(card_id);
                            break;
                        default:
                            break;
                    }
                }
            }

            //trigger event
            moveOneTimes = _mergeMoves(cards_moves);
            foreach (CardsMoveOneTimeStruct moveOneTime in moveOneTimes)
            {
                if (moveOneTime.Card_ids.Count == 0) continue;
                object data = moveOneTime;
                RoomThread.Trigger(TriggerEvent.CardsMoveOneTime, this, null, ref data);
            }
        }


        public List<int> MoveCardsAtomic(CardsMoveStruct cards_move, bool forceMoveVisible)
        {
            List<CardsMoveStruct> cards_moves = new List<CardsMoveStruct> { cards_move };
            return MoveCardsAtomic(cards_moves, forceMoveVisible);
        }

        public List<int> MoveCardsAtomic(List<CardsMoveStruct> _cards_moves, bool forceMoveVisible)
        {
            List<CardsMoveStruct> cards_moves = _breakDownCardMoves(_cards_moves);

            List<CardsMoveOneTimeStruct> moveOneTimes = _mergeMoves(cards_moves);
            for (int i = 0; i < moveOneTimes.Count; i++)
            {
                if (moveOneTimes[i].Card_ids.Count == 0)
                    continue;

                object data = moveOneTimes[i];
                RoomThread.Trigger(TriggerEvent.BeforeCardsMove, this, null, ref data);
                CardsMoveOneTimeStruct moveOneTime2 = (CardsMoveOneTimeStruct)data;
                moveOneTimes[i] = moveOneTime2;
            }
            cards_moves = _separateMoves(moveOneTimes);

            // First, process remove card
            for (int i = 0; i < cards_moves.Count; i++)
            {
                CardsMoveStruct cards_move = cards_moves[i];
                for (int j = 0; j < cards_move.Card_ids.Count; j++)
                {
                    int card_id = cards_move.Card_ids[j];
                    Player from = string.IsNullOrEmpty(cards_move.From) ? null : FindPlayer(cards_move.From, true);

                    if (cards_move.From_place == Place.PlaceHand || cards_move.From_place == Place.PlaceEquip)
                        System.Diagnostics.Debug.Assert(from != null, card_id.ToString());

                    if (cards_move.From_place == Place.PlaceSpecial)
                        System.Diagnostics.Debug.Assert(from != null, cards_move.From_pile_name);
                    if (from != null) // Hand/Equip/Judge
                    {
                        RoomLogic.RemovePlayerCard(this, from, card_id, cards_move.From_place);
                        System.Diagnostics.Debug.Assert(!from.GetCards("he").Contains(card_id));
                    }

                    switch (cards_move.From_place)
                    {
                        case Place.DiscardPile:
                            m_discardPile.Remove(card_id);
                            break;
                        case Place.DrawPile:
                            RemoveFromDrawPile(card_id);
                            break;
                        case Place.DrawPileBottom:
                            RemoveFromDrawPile(card_id);
                            break;
                        case Place.PlaceSpecial:
                            table_cards.Remove(card_id);
                            break;
                        default:
                            break;
                    }
                }
            }
            NotifyMoveCards(true, cards_moves, forceMoveVisible);

            foreach (CardsMoveStruct move in cards_moves)
                UpdateCardsOnLose(move);

            for (int i = 0; i < cards_moves.Count; i++)
            {
                CardsMoveStruct cards_move = cards_moves[i];
                for (int j = 0; j < cards_move.Card_ids.Count; j++)
                    SetCardMapping(cards_move.Card_ids[j], FindPlayer(cards_move.To, true), cards_move.To_place);
            }
            foreach (CardsMoveStruct move in cards_moves)
                UpdateCardsOnGet(move);
            NotifyMoveCards(false, cards_moves, forceMoveVisible);

            // Now, process add cards
            for (int i = 0; i < cards_moves.Count; i++)
            {
                CardsMoveStruct cards_move = cards_moves[i];
                for (int j = 0; j < cards_move.Card_ids.Count; j++)
                {
                    int card_id = cards_move.Card_ids[j];
                    WrappedCard card = GetCard(card_id);
                    if (forceMoveVisible && cards_move.To_place == Place.PlaceHand)
                        card.SetFlags("visible");
                    else
                        card.SetFlags("-visible");

                    Player to = null;
                    if (!string.IsNullOrEmpty(cards_move.To))
                        to = FindPlayer(cards_move.To, true);
                    if (to != null) // Hand/Equip/Judge
                    {
                        RoomLogic.AddPlayerCard(this, to, card_id, cards_move.To_place);
                        if (cards_move.To_place == Place.PlaceHand || cards_move.To_place == Place.PlaceEquip)
                            System.Diagnostics.Debug.Assert(to.GetCards("he").Contains(card_id) && GetCardOwner(card_id) == to && GetCardPlace(card_id) == cards_move.To_place);
                        else if (cards_move.To_place == Place.PlaceDelayedTrick)
                            System.Diagnostics.Debug.Assert(to.JudgingArea.Contains(card_id) && GetCardOwner(card_id) == to && GetCardPlace(card_id) == cards_move.To_place);
                    }
                    if (cards_move.To_place == Place.PlaceSpecial)
                        to.PileChange(cards_move.To_pile_name, new List<int> { card_id });

                    Player from = null;
                    if (cards_move.From != null) from = FindPlayer(cards_move.From);
                    bool open = forceMoveVisible || cards_move.From_place == Place.PlaceEquip;

                    switch (cards_move.To_place)
                    {
                        case Place.DiscardPile:
                            m_discardPile.Insert(0, card_id);
                            break;
                        case Place.DrawPile:
                            ReturnToDrawPile(new List<int> { card_id }, false, from, open);
                            break;
                        case Place.DrawPileBottom:
                            ReturnToDrawPile(new List<int>() { card_id }, true, from, open);
                            break;
                        case Place.PlaceSpecial:
                            table_cards.Add(card_id);
                            break;
                        default:
                            break;
                    }
                }
            }

            //trigger event
            moveOneTimes = _mergeMoves(cards_moves);
            List<int> result = new List<int>();
            //foreach (Player player in GetAllPlayers())
            //{
            //    result.Clear();
            //    foreach (CardsMoveOneTimeStruct moveOneTime in moveOneTimes)
            //    {
            //        if (moveOneTime.Card_ids.Count == 0) continue;
            //        object data = moveOneTime;
            //        room_thread.Trigger(TriggerEvent.CardsMoveOneTime, this, player, ref data);
            //        CardsMoveOneTimeStruct moveOneTime_result = (CardsMoveOneTimeStruct)data;
            //        result.AddRange(moveOneTime_result.Card_ids);
            //    }
            //}

            foreach (CardsMoveOneTimeStruct moveOneTime in moveOneTimes)
            {
                if (moveOneTime.Card_ids.Count == 0) continue;
                object data = moveOneTime;
                RoomThread.Trigger(TriggerEvent.CardsMoveOneTime, this, null, ref data);
                CardsMoveOneTimeStruct moveOneTime_result = (CardsMoveOneTimeStruct)data;
                result.AddRange(moveOneTime_result.Card_ids);
            }

            return result;
        }

        private void UpdateCardsOnLose(CardsMoveStruct move)
        {
            for (int i = 0; i < move.Card_ids.Count; i++)
            {
                RoomCard card = GetCard(move.Card_ids[i]);
                if ((card.Modified || card.GetUsedCard().Modified) && move.To_place == Place.DiscardPile)
                    ResetCard(move.Card_ids[i]);
            }
        }

        private void ResetCard(int id)
        {
            WrappedCard real = Engine.GetRealCard(id);
            WrappedCard clone = Engine.CloneCard(real);
            m_cards[id].TakeOver(clone);
            m_cards[id].Modified = false;
        }

        public void UpdateCardsOnGet(CardsMoveStruct move)
        {
            if (move.Card_ids.Count == 0) return;
            Player player = FindPlayer(move.From, true);
            if (player != null && move.To_place == Place.PlaceDelayedTrick)
            {
                for (int i = 0; i < move.Card_ids.Count; i++)
                {
                    RoomCard card = GetCard(move.Card_ids[i]);
                    WrappedCard engine_card = Engine.GetRealCard(move.Card_ids[i]);
                    if (card.Suit != engine_card.Suit || card.Number != engine_card.Number)
                    {
                        WrappedCard trick = card.GetUsedCard();
                        trick.SetSuit(engine_card.Suit);
                        trick.SetNumber(engine_card.Number);
                        card.TakeOver(trick);
                    }
                }
                return;
            }

            player = FindPlayer(move.To, true);
            if (player != null && (move.To_place == Place.PlaceHand
                || move.To_place == Place.PlaceEquip
                || move.To_place == Place.PlaceJudge
                || move.To_place == Place.PlaceSpecial))
            {
                FilterCards(player, move.Card_ids, true);
            }
        }

        public void TransformDeputyGeneral(Player player, int count = 3)
        {
            if (!RoomLogic.CanTransform(player) || count <= 0) return;

            ShowGeneral(player, false);

            List<string> names = new List<string> { player.ActualGeneral1, player.ActualGeneral2 };
            List<string> available = new List<string>();
            foreach (string name in Generals)
                if (!name.StartsWith("lord_") && !UsedGeneral.Contains(name) && Engine.GetGeneral(name, Setting.GameMode).Kingdom == player.Kingdom)
                    available.Add(name);
            if (available.Count == 0) return;

            RemoveGeneral(player, false);
            Thread.Sleep(1000);

            Shuffle.shuffle(ref available);
            List<string> generals = new List<string>();
            for (int i = 0; i < Math.Min(count, available.Count); i++)
                generals.Add(available[i]);

            string general_name = AskForGeneral(player, generals, null, true, "transform", null, true, true);
            HandleUsedGeneral(general_name);
            
            foreach (string skill_name in Engine.GetGeneralSkills(general_name, "Hegemony", false))
            {
                Skill skill = Engine.GetSkill(skill_name);
                if (!Skills.Contains(skill_name))
                {
                    Skills.Add(skill_name);
                    if (skill is TriggerSkill tskill)
                        RoomThread.AddTriggerSkill(tskill);
                }

                foreach (Skill _skill in Engine.GetRelatedSkills(skill_name))
                {
                    if (!Skills.Contains(_skill.Name))
                    {
                        Skills.Add(_skill.Name);
                        if (_skill is TriggerSkill tskill)
                            RoomThread.AddTriggerSkill(tskill);
                    }
                }

                AddPlayerSkill(player, skill_name, false);
            }

            foreach (string skill in Engine.GetGeneralRelatedSkills(general_name, "Hegemony"))
            {
                if (!Skills.Contains(skill))
                {
                    Skills.Add(skill);
                    Skill main = Engine.GetSkill(skill);
                    if (main is TriggerSkill tskill)
                        RoomThread.AddTriggerSkill(tskill);
                }

                foreach (Skill _skill in Engine.GetRelatedSkills(skill))
                {
                    if (!Skills.Contains(_skill.Name))
                    {
                        Skills.Add(_skill.Name);
                        if (_skill is TriggerSkill tskill)
                            RoomThread.AddTriggerSkill(tskill);
                    }
                }
            }

            player.ActualGeneral2 = general_name;
            NotifyProperty(GetClient(player), player, "ActualGeneral2");

            names[1] = general_name;
            player.General2Showed = false;
            BroadcastProperty(player, "General2Showed");

            ChangePlayerGeneral2(player, "anjiang");

            //if (player.IsAutoPreshow())
            //    player.SetSkillsPreshowed("d");
            NotifyPlayerPreshow(player, "d");

            foreach (string skill_name in Engine.GetGeneralSkills(general_name, "Hegemony", false))
            {
                Skill skill = Engine.GetSkill(skill_name);
                if (skill.SkillFrequency == Frequency.Limited && !string.IsNullOrEmpty(skill.LimitMark))
                {
                    player.SetMark(skill.LimitMark, 1);
                    List<string> arg2 = new List<string> { player.Name, skill.LimitMark, "1" };
                    DoNotify(GetClient(player), CommandType.S_COMMAND_SET_MARK, arg2);
                }
            }

            ShowGeneral(player, false);
        }

        public bool NotifyMoveCards(bool isLostPhase, List<CardsMoveStruct> cards_moves, bool forceVisible, List<Player> players = null)
        {
            if (players == null) players = Players;
            List<Client> receivers = new List<Client>();
            foreach (Player player in players)
            {
                if (GetClient(player) != null && !receivers.Contains(GetClient(player)))
                    receivers.Add(GetClient(player));
            }

            // Notify clients
            int moveId;
            if (isLostPhase)
                moveId = _m_lastMovementId++;
            else
                moveId = --_m_lastMovementId;
            //Q_ASSERT(_m_lastMovementId >= 0);
            foreach (Client player in receivers)
            {
                if (player.Status == Client.GameStatus.offline) continue;
                List<string> arg = new List<string>
                {
                    moveId.ToString()
                };
                int move_num = cards_moves.Count;
                for (int i = 0; i < move_num; i++)
                {
                    CardsMoveStruct cards_move = cards_moves[i];
                    bool relevant = false;
                    Player from = (cards_move.From != null ? FindPlayer(cards_move.From, true) : null);
                    Player to = (cards_move.To != null ? FindPlayer(cards_move.To, true) : null);
                    Player p = GetPlayers(player.UserID)[0];

                    if (cards_move.Reason.Reason == MoveReason.S_REASON_PREVIEWGIVE)         //郭嘉遗迹、伊籍机捷
                    {
                        Player p2 = FindPlayer(cards_move.Reason.PlayerId, true);
                        if (p2 != null && (p2 == p || p.IsSameCamp(p2)))
                            relevant = true;
                    }

                    if (!relevant && (from != null && (from == p || p.IsSameCamp(from))) || (to != null && (to == p || p.IsSameCamp(to)) && cards_move.To_place != Place.PlaceSpecial))
                        relevant = true;

                    if (!relevant && to != null && !string.IsNullOrEmpty(cards_move.To_pile_name) && to != null)
                    {
                        foreach (string name in to.GetPileOpener(cards_move.To_pile_name))
                        {
                            Player who = FindPlayer(name, true);
                            if (who != null && (who == p || p.IsSameCamp(who)))
                            {
                                relevant = true;
                                break;
                            }
                        }
                    }

                    cards_move.Open = forceVisible || relevant
                        // forceVisible will override cards to be visible
                        || cards_move.To_place == Place.PlaceEquip
                        || cards_move.From_place == Place.PlaceEquip
                        || cards_move.To_place == Place.PlaceDelayedTrick
                        || cards_move.From_place == Place.PlaceDelayedTrick
                        // only cards moved to hand/special can be invisible
                        || cards_move.From_place == Place.DiscardPile
                        || cards_move.To_place == Place.DiscardPile
                        // any card from/to discard pile should be visible
                        || cards_move.From_place == Place.PlaceTable
                        || (cards_move.To_place == Place.PlaceTable
                            && (cards_move.Reason.SkillName != "guhuo" || (cards_move.From_place != Place.PlaceHand && cards_move.From_place != Place.PlaceSpecial)));
                    // any card from/to place table should be visible
                    // the player put someone's cards to the drawpile

                    ClientCardsMoveStruct notify = new ClientCardsMoveStruct()
                    {
                        Card_ids = new List<int>(),
                        From_place = cards_move.From_place,
                        //To_place = cards_move.To_place == Place.PlaceUnknown ? Place.DrawPile : cards_move.To_place,
                        To_place = cards_move.To_place,
                        From = cards_move.From,
                        To = cards_move.To,
                        Reason = new CardMoveReasonStruct
                        {
                            EventName = cards_move.Reason.EventName,
                            TargetId = cards_move.Reason.TargetId,
                            PlayerId = cards_move.Reason.PlayerId,
                            Reason = cards_move.Reason.Reason,
                            General = cards_move.Reason.General,
                            SkillName = cards_move.Reason.SkillName,
                            CardString = cards_move.Reason.Card != null ? RoomLogic.CardToString(this, cards_move.Reason.Card) : string.Empty
                        },
                        Open = cards_move.Open,
                        From_pile_name = cards_move.From_pile_name,
                        To_pile_name = cards_move.To_pile_name
                    };
                    if (cards_move.Open)
                    {
                        notify.Card_ids = cards_move.Card_ids;
                    }
                    else
                    {
                        int count = cards_move.Card_ids.Count;
                        for (int index = 0; index < count; index++)
                            notify.Card_ids.Add(-1);
                    }
                    arg.Add(JsonUntity.Object2Json(notify));
                }
                DoNotify(player, isLostPhase ? CommandType.S_COMMAND_LOSE_CARD : CommandType.S_COMMAND_GET_CARD, arg);
            }
            Thread.Sleep(20);
            return true;
        }

        public Player FindPlayer(string general_name, bool include_dead = false)
        {
            if (string.IsNullOrEmpty(general_name)) return null;
            List<Player> list = include_dead ? new List<Player>(m_players) : GetAlivePlayers();

            if (general_name.Contains("+"))
            {
                string[] names = general_name.Split('+');
                return list.Find(t => names.Contains(t.General1) || names.Contains(t.Name));
            }
            return list.Find(t => t.General1 == general_name || t.Name == general_name);
        }

        public void SetPromotSkill(Player player, string skill_name, string head, string pattern, CardUseStruct.CardUseReason reason)
        {
            Interactivity interactivity = GetInteractivity(player);
            if (interactivity != null)
                interactivity.SetPromoteSkill(player, skill_name, head, pattern, reason);
        }

        private List<CardsMoveStruct> _separateMoves(List<CardsMoveOneTimeStruct> moveOneTimes)
        {
            List<_MoveSeparateClassifier> classifiers = new List<_MoveSeparateClassifier>();
            List<List<int>> ids = new List<List<int>>();
            foreach (CardsMoveOneTimeStruct moveOneTime in moveOneTimes)
            {
                for (int i = 0; i < moveOneTime.Card_ids.Count; i++)
                {
                    _MoveSeparateClassifier classifier = new _MoveSeparateClassifier(moveOneTime, i);
                    if (classifiers.Contains(classifier))
                    {
                        int pos = classifiers.IndexOf(classifier);
                        ids[pos].Add(moveOneTime.Card_ids[i]);
                    }
                    else
                    {
                        classifiers.Add(classifier);
                        List<int> new_ids = new List<int> { moveOneTime.Card_ids[i] };
                        ids.Add(new_ids);
                    }
                }
            }

            List<CardsMoveStruct> card_moves = new List<CardsMoveStruct>();
            int index = 0;
            Dictionary<Player, List<int>> from_handcards = new Dictionary<Player, List<int>>();
            foreach (_MoveSeparateClassifier cls in classifiers)
            {
                Player from = cls.From;
                if (from != null && !from_handcards.ContainsKey(from))
                    from_handcards[from] = from.GetCards("h");

                CardsMoveStruct card_move = new CardsMoveStruct
                {
                    From = cls.From?.Name,
                    To = cls.To?.Name,
                    From_place = cls.From_place,
                    To_place = cls.To_place,
                    From_pile_name = cls.From_pile_name,
                    To_pile_name = cls.To_pile_name,
                    Open = cls.Open,
                    Card_ids = ids[index],
                    Reason = cls.Reason,
                    Origin_from = cls.From?.Name,
                    Origin_to = cls.To?.Name,
                    Origin_from_place = cls.From_place,
                    Origin_to_place = cls.To_place,
                    Origin_from_pile_name = cls.From_pile_name,
                    Origin_to_pile_name = cls.To_pile_name,
                };

                if (from != null && from_handcards.ContainsKey(from))
                {
                    List<int> move_ids = from_handcards[from];
                    if (move_ids.Count > 0)
                    {
                        foreach (int id in card_move.Card_ids)
                            move_ids.Remove(id);
                        card_move.Is_last_handcard = move_ids.Count == 0;
                    }
                }

                card_moves.Add(card_move);
                index++;
            }
            if (card_moves.Count > 1)
                card_moves.Sort((x, y) => CompareByActionOrder(x, y));
            return card_moves;
        }

        public int CompareByActionOrder(CardsMoveStruct move1, CardsMoveStruct move2)
        {
            Player a = FindPlayer(move1.From, true);
            if (a == null) a = FindPlayer(move1.To, true);
            Player b = FindPlayer(move2.From, true);
            if (b == null) b = FindPlayer(move2.To, true);

            if (a == null || b == null)
                return a != null ? -1 : 1;

            return GetFront(a, b) == a ? -1 : 1;
        }


        private List<CardsMoveStruct> _breakDownCardMoves(List<CardsMoveStruct> cards_moves)
        {
            List<CardsMoveStruct> all_sub_moves = new List<CardsMoveStruct>();
            for (int i = 0; i < cards_moves.Count; i++)
            {
                CardsMoveStruct move = cards_moves[i];
                if (move.Card_ids.Count == 0) continue;

                Dictionary<_MoveSourceClassifier, List<int>> moveMap = new Dictionary<_MoveSourceClassifier, List<int>>();
                // reassemble move sources

                for (int j = 0; j < move.Card_ids.Count; j++)
                {
                    _fillMoveInfo(ref move, j);
                    _MoveSourceClassifier classifier = new _MoveSourceClassifier
                    {
                        From_player_name = move.From,
                        From_place = move.From_place,
                        From_pile_name = move.From_pile_name
                    };
                    if (moveMap.ContainsKey(classifier))
                        moveMap[classifier].Add(move.Card_ids[j]);
                    else
                        moveMap[classifier] = new List<int> { move.Card_ids[j] };
                }


                foreach (_MoveSourceClassifier cls in moveMap.Keys)
                {
                    CardsMoveStruct sub_move = new CardsMoveStruct();
                    sub_move.Copy(move);
                    cls.CopyTo(ref sub_move);
                    if ((sub_move.From == sub_move.To && sub_move.From_place == sub_move.To_place)
                        || sub_move.Card_ids.Count == 0)
                        continue;
                    sub_move.Card_ids = new List<int>(moveMap[cls]);
                    all_sub_moves.Add(sub_move);
                }
            }
            return all_sub_moves;
        }

        private List<CardsMoveOneTimeStruct> _mergeMoves(List<CardsMoveStruct> cards_moves)
        {
            Dictionary<_MoveMergeClassifier, List<CardsMoveStruct>> moveMap = new Dictionary<_MoveMergeClassifier, List<CardsMoveStruct>>();

            foreach (CardsMoveStruct cards_move in cards_moves)
            {
                _MoveMergeClassifier classifier = new _MoveMergeClassifier()
                {
                    From = FindPlayer(cards_move.From, true),
                    To = FindPlayer(cards_move.To, true),
                    To_place = cards_move.To_place,
                    To_pile_name = cards_move.To_pile_name,

                    Origin_from = FindPlayer(cards_move.Origin_from, true),
                    Origin_to = FindPlayer(cards_move.Origin_to, true),
                    Origin_to_place = cards_move.Origin_to_place,
                    Origin_to_pile_name = cards_move.Origin_to_pile_name,
                    //(cards_move)
                };
                if (moveMap.ContainsKey(classifier))
                    moveMap[classifier].Add(cards_move);
                else
                    moveMap[classifier] = new List<CardsMoveStruct> { cards_move };
            }

            List<CardsMoveOneTimeStruct> result = new List<CardsMoveOneTimeStruct>();
            foreach (_MoveMergeClassifier cls in moveMap.Keys)
            {
                CardsMoveOneTimeStruct moveOneTime = new CardsMoveOneTimeStruct(cls.From)
                {
                    Reason = moveMap[cls][0].Reason,
                    To = cls.To,
                    To_place = cls.To_place,
                    To_pile_name = cls.To_pile_name,
                    Is_last_handcard = false,
                    Origin_from = cls.Origin_from,
                    Origin_to = cls.Origin_to,
                    Origin_to_place = cls.Origin_to_place,
                    Origin_to_pile_name = cls.Origin_to_pile_name
                };
                foreach (CardsMoveStruct move in moveMap[cls])
                {
                    moveOneTime.Card_ids.AddRange(move.Card_ids);
                    for (int i = 0; i < move.Card_ids.Count; i++)
                    {
                        moveOneTime.From_places.Add(move.From_place);
                        moveOneTime.Origin_from_places.Add(move.From_place);
                        moveOneTime.From_pile_names.Add(move.From_pile_name);
                        moveOneTime.Origin_from_pile_names.Add(move.From_pile_name);
                        moveOneTime.Open.Add(move.Open);
                    }
                    if (move.Is_last_handcard)
                        moveOneTime.Is_last_handcard = true;
                }
                result.Add(moveOneTime);
            }

            if (result.Count > 1)
                result.Sort((x, y) => CompareByActionOrder_OneTime(x, y));

            return result;
        }

        private int CompareByActionOrder_OneTime(CardsMoveOneTimeStruct move1, CardsMoveOneTimeStruct move2)
        {
            Player a = move1.From;
            if (a == null) a = move1.To;
            Player b = move2.From;
            if (b == null) b = move2.To;

            if (a == null || b == null)
            {
                if (a != null)
                    return -1;
                else
                    return 1;
            }

            return GetFront(a, b) == a ? -1 : 1;
        }

        public struct _MoveSourceClassifier
        {
            //    public _MoveSourceClassifier(CardsMoveStruct move)
            //    {
            //        m_from = move.from; m_from_place = move.from_place;
            //        m_from_pile_name = move.from_pile_name; m_from_player_name = move.from_player_name;
            //    }
            public void CopyTo(ref CardsMoveStruct move)
            {
                move.From = From_player_name;
                move.From_place = From_place;
                move.From_pile_name = From_pile_name;
            }
            public override bool Equals(object obj)
            {
                return obj is _MoveSourceClassifier other
                    && From_place == other.From_place && From_pile_name == other.From_pile_name && From_player_name == other.From_player_name;
            }

            public static bool operator ==(_MoveSourceClassifier other, object another) => other.Equals(another);
            public static bool operator !=(_MoveSourceClassifier other, object another) => !other.Equals(another);

            public override int GetHashCode()
            {
                return From_place.GetHashCode() * (!string.IsNullOrEmpty(From_pile_name) ? From_pile_name.GetHashCode() : 12)
                    * (!string.IsNullOrEmpty(From_player_name) ? From_player_name.GetHashCode() : 13);
            }

            //public static bool operator <(_MoveSourceClassifier ori, _MoveSourceClassifier other)
            //{
            //    return m_from<other.m_from || m_from_place<other.m_from_place
            //        || m_from_pile_name<other.m_from_pile_name || m_from_player_name<other.m_from_player_name;
            //}
            public Place From_place { set; get; }
            public string From_pile_name { set; get; }
            public string From_player_name { set; get; }
        };

        public struct _MoveMergeClassifier
        {
            public override bool Equals(object obj)
            {
                return obj is _MoveMergeClassifier other && From == other.From && To == other.To
                    && To_place == other.To_place && To_pile_name == other.To_pile_name;
            }
            public override int GetHashCode()
            {
                return (From != null ? From.GetHashCode() : 10) * (To != null ? To.GetHashCode() : 11) * To_place.GetHashCode()
                    * (!string.IsNullOrEmpty(To_pile_name) ? To_pile_name.GetHashCode() : 13);
            }
            public static bool operator ==(_MoveMergeClassifier other, object another) => other.Equals(another);
            public static bool operator !=(_MoveMergeClassifier other, object another) => !other.Equals(another);

            public Player From { set; get; }
            public Player To { set; get; }
            public Place To_place { set; get; }
            public string To_pile_name { set; get; }
            public Player Origin_from { set; get; }
            public Player Origin_to { set; get; }
            public Place Origin_to_place { set; get; }
            public string Origin_to_pile_name { set; get; }
        };

        public struct _MoveSeparateClassifier
        {
            public _MoveSeparateClassifier(CardsMoveOneTimeStruct moveOneTime, int i)
            {
                From = moveOneTime.From;
                To = moveOneTime.To;
                From_place = moveOneTime.From_places[i];
                To_place = moveOneTime.To_place;
                From_pile_name = moveOneTime.From_pile_names[i];
                To_pile_name = moveOneTime.To_pile_name;
                Open = moveOneTime.Open[i];
                Reason = moveOneTime.Reason;
                Is_last_handcard = moveOneTime.Is_last_handcard;
            }

            public override bool Equals(object obj)
            {
                return obj is _MoveSeparateClassifier other && From == other.From && To == other.To
                    && From_place == other.From_place && To_place == other.To_place
                    && From_pile_name == other.From_pile_name && To_pile_name == other.To_pile_name
                    && Open == other.Open && Reason.Equals(other.Reason) && Is_last_handcard == other.Is_last_handcard;
            }

            public static bool operator ==(_MoveSeparateClassifier other, object another) => other.Equals(another);
            public static bool operator !=(_MoveSeparateClassifier other, object another) => !other.Equals(another);
            public override int GetHashCode()
            {
                return (From != null ? From.GetHashCode() : 10) * (To != null ? To.GetHashCode() : 11) * From_place.GetHashCode() * To_place.GetHashCode()
                    * (!string.IsNullOrEmpty(From_pile_name) ? From_pile_name.GetHashCode() : 13) * (!string.IsNullOrEmpty(To_pile_name) ? To_pile_name.GetHashCode() : 14)
                    * Open.GetHashCode() * Reason.GetHashCode() * Is_last_handcard.GetHashCode();
            }
            public Player From { set; get; }
            public Player To { set; get; }
            public Place From_place { set; get; }
            public Place To_place { set; get; }
            public string From_pile_name { set; get; }
            public string To_pile_name { set; get; }
            public bool Open { set; get; }
            public CardMoveReason Reason { set; get; }
            public bool Is_last_handcard { set; get; }
        };
        public void SortByActionOrder(ref List<Player> players)
        {
            if (players.Count > 1)
                players.Sort((x, y) => CompareByActionOrder(x, y));
        }
        public void SortByActionOrder(ref CardUseStruct use)
        {
            if (use.To.Count <= 1) return;
            
            List<Player> news = new List<Player>(use.To);
            List<CardBasicEffect> counts = new List<CardBasicEffect>();
            SortByActionOrder(ref news);
            use.To = news;

            if (use.EffectCount != null)
            {
                foreach (Player p in news)
                {
                    int count = use.EffectCount.Count;
                    for (int i = 0; i < count; i++)
                    {
                        if (use.EffectCount[i].To == p)
                        {
                            counts.Add(use.EffectCount[i]);
                            use.EffectCount.RemoveAt(i);
                            break;
                        }
                    }
                }

                use.EffectCount = counts;
            }
        }
        public int CompareByActionOrder(Player a, Player b)
        {
            return GetFront(a, b) == a ? -1 : 1;
        }

        #region 客户端进入请求
        public bool OnClientRequestInter(Client client, int id, string pwd)
        {
            try
            {
                if (id != RoomId || Clients.Count == 0 || banned_clients.Contains(client.UserID))
                    return false;

                bool reconnect = false;

                if (!GameStarted && (IsFull() || (!string.IsNullOrEmpty(Setting.PassWord) && pwd != Setting.PassWord)))
                {
                    return false;
                }
                else if (GameStarted)
                {
                    //检查room中玩家是否符合重连条件
                    foreach (Player p in Players)
                    {
                        if (p.ClientId == client.UserID)
                        {
                            reconnect = true;
                            break;
                        }
                    }

                    if (!reconnect)
                        return false;
                }

                lock (this)
                {
                    client.Disconnected += OnClientDisconnected;
                    client.LeaveRoom += OnClientLeave;
                    client.GetReady += OnClientReady;
                    //client.GameControl += ProcessClientPacket;

                    Clients.Add(client);
                    client.GameRoom = RoomId;
                    client.RoleReserved = string.Empty;
                    client.GeneralReserved = new List<string>();

                    SendRoomSetting2Client(client);

                    //为改玩家安排座次
                    foreach (int index in seat2clients.Keys)
                    {
                        if (seat2clients[index] == null)
                        {
                            seat2clients[index] = client;
                            break;
                        }
                    }
                    client.Status = Client.GameStatus.normal;
                    if (GameStarted)
                        client.Status = Client.GameStatus.online;

                    //通知room中的其他玩家
                    RoomMessage.NotifyPlayerJoinorLeave(this, client, true);
                    UpdateClientsInfo();

                    //测试
                    //int id = Thread.CurrentThread.ManagedThreadId;
                    //OutPut(string.Format("current thread {0}", id));

                    //room_thread = new RoomThread(this);
                    //thread = new Thread(room_thread.Start);
                    //thread.Start();

                    //reconnect
                    if (reconnect)
                    {
                        Marshal(client);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Debug(string.Format("{0} {1} {2}", e.Message, e.TargetSite, e.Source));
                Debug(string.Format("error at client {0} {1} into room", client.UserName, client.Profile.NickName));
                LogHelper.WriteLog(null, e);
            }

            return false;
        }

        private Dictionary<int, Client> seat2clients;
        public void OnHostInter()
        {
            lock (this)
            {
                Host.Disconnected += OnClientDisconnected;
                Host.LeaveRoom += OnClientLeave;
                //Host.GameControl += ProcessClientPacket;

                Clients.Add(Host);

                seat2clients = new Dictionary<int, Client>();
                for (int i = 0; i < Setting.PlayerNum; i++)
                    seat2clients.Add(i, null);
                foreach (int index in seat2clients.Keys)
                {
                    if (seat2clients[index] == null)
                    {
                        seat2clients[index] = Host;
                        break;
                    }
                }

                Host.GameRoom = RoomId;
                Host.Status = Client.GameStatus.ready;
                Host.RoleReserved = string.Empty;
                Host.GeneralReserved = new List<string>();

                SendRoomSetting2Client(Host);

                //通知客户端当前玩家(client)信息
                Host.Status = Client.GameStatus.normal;


                //通知room中的其他玩家
                UpdateClientsInfo();

                //测试
                //int id = Thread.CurrentThread.ManagedThreadId;
                //OutPut(string.Format("current thread {0}", id));

                //room_thread = new RoomThread(this);
                //thread = new Thread(room_thread.Start);
                //thread.Start();
            }
        }

        private int GetClientIndex(Client client)
        {
            foreach (int index in seat2clients.Keys)
            {
                if (seat2clients[index] == client)
                    return index;
            }

            return -1;
        }

        private void UpdateClientsInfo()
        {
            List<string> info = new List<string> { Host.UserID.ToString() };
            List<Client> clients = new List<Client>(Clients);
            foreach (Client client in clients)
            {
                //OutPut(string.Format("{0} 是 第{1}位", client.UserID, GetClientIndex(client)));
                info.Add(GetClientIndex(client).ToString());
                info.Add(JsonUntity.Object2Json(client.Profile));
                info.Add(((int)client.Status).ToString());
            }

            MyData data = new MyData
            {
                Description = PacketDescription.Room2Cient,
                Protocol = Protocol.UpdateRoom,
                Body = info,
            };

            foreach (Client client in clients)
            {
                client.SendSwitchReply(data);
            }

            //通知hall更新信息
            Hall.BroadCastRoom(this);

            //OutPut(string.Format("添加玩家的线程为{0} room id 为{1}", Thread.CurrentThread.ManagedThreadId, room_id));
        }

        //通知客户端转跳场景
        private void SendRoomSetting2Client(Client client)
        {
            GameSetting setting = Setting;
            setting.PassWord = "*******";
            MyData data = new MyData()
            {
                Description = PacketDescription.Room2Cient,
                Protocol = Protocol.JoinRoom,
                Body = new List<string> { Host.UserID.ToString(), JsonUntity.Object2Json(setting), GameStarted.ToString() }
            };
            client.SendSwitchReply(data);
        }
        #endregion

        #region 客户端断线处理
        private void OnClientDisconnected(object sender, EventArgs e)
        {
            lock (this)
            {
                Client client = (Client)sender;
                client.Status = Client.GameStatus.offline;

                bool human_stay = false;
                List<Client> clients = new List<Client>(Clients);
                foreach (Client other in clients)
                {
                    if (other != client && other.Status != Client.GameStatus.bot && other.Status != Client.GameStatus.offline)
                    {
                        human_stay = true;
                        break;
                    }
                }

                if (!GameStarted)
                {
                    client.GameRoom = 0;
                    ClientDBOperation.UpdateGameRoom(client.UserName, 0);

                    if (!human_stay)
                        StopGame();
                    else
                    {
                        RomoveClientOnGamePlay(client);

                        //通知room中的其他玩家
                        RoomMessage.NotifyPlayerDisconnected(this, client);
                        UpdateClientsInfo();
                    }
                }
                else
                {
                    if (!human_stay)
                    {
                        //游戏结束结算战绩
                        StopGame();
                        return;
                    }

                    foreach (Player p in GetPlayers(client.UserID))
                    {
                        p.Status = "offline";
                        BroadcastProperty(p, "Status");
                    }
                    RomoveClientOnGamePlay(client);

                    UpdateClientsInfo();
                }
            }
        }
        #endregion

        #region 客户端离开房间
        private List<int> banned_clients = new List<int>();
        private void OnClientLeave(object sender, ClientEventArgs args)
        {
            if (sender is Client client)
            {
                lock (this)
                {
                    bool human_stay = false;
                    List<Client> clients = new List<Client>(Clients);
                    foreach (Client other in clients)
                    {
                        if (other != client && other.Status != Client.GameStatus.bot && other.Status != Client.GameStatus.offline)
                        {
                            human_stay = true;
                            break;
                        }
                    }

                    if (!GameStarted)
                    {
                        if (!human_stay)
                        {
                            StopGame();
                            return;
                        }
                        RomoveClientOnGamePlay(client);

                        //通知room中的其他玩家
                        RoomMessage.NotifyPlayerJoinorLeave(this, client, false);
                        UpdateClientsInfo();
                    }
                    else
                    {
                        if (!human_stay)
                        {
                            //游戏结束结算战绩
                            StopGame();
                            return;
                        }

                        //游戏继续
                        //判断是否因角色死亡而离开
                        bool check = true;
                        foreach (Player p in GetPlayers(client.UserID))
                        {
                            if (p.Alive)
                            {
                                check = false;
                                break;
                            }
                        }

                        foreach (Player p in GetPlayers(client.UserID))
                        {
                            p.Status = check ? "leave" : "escape";
                            BroadcastProperty(p, "Status");
                        }

                        RomoveClientOnGamePlay(client);

                        UpdateClientsInfo();
                    }

                    if (args.Bool)
                    {
                        if (client.Status == Client.GameStatus.bot)
                        {
                            Hall.RemoveBot(client);
                            client = null;
                        }
                        else
                        {
                            //通知客户端
                            MyData data = new MyData
                            {
                                Description = PacketDescription.Room2Cient,
                                Protocol = Protocol.LeaveRoom,
                            };
                            client.SendSwitchReply(data);

                            //一分钟内禁止进入
                            banned_clients.Add(client.UserID);
                            WaitForClear(client.UserID);
                        }
                    }
                }
            }
        }

        private void RomoveClientOnGamePlay(Client client)
        {
            Interactivity interactivity = GetInteractivity(client.UserID);
            if (interactivity != null && interactivity.IsWaitingReply)
            {
                MyData data = new MyData
                {
                    Body = new List<string> { interactivity.ExpectedReplyCommand.ToString() }
                };
                ProcessClientReply(interactivity, data);
            }

            foreach (int index in seat2clients.Keys)
            {
                if (seat2clients[index] == client)
                {
                    seat2clients[index] = null;
                    break;
                }
            }

            foreach (Player p in GetPlayers(client.UserID))
                player_client.Remove(p);
            Clients.Remove(client);
            client.Disconnected -= OnClientDisconnected;
            client.LeaveRoom -= OnClientLeave;
            client.GetReady -= OnClientReady;
            //client.GameControl -= ProcessClientPacket;
            List<Client> clients = new List<Client>(Clients);
            if (client == Host)
            {
                foreach (Client other in clients)
                {
                    if (other.Status != Client.GameStatus.bot && other.Status != Client.GameStatus.offline)
                    {
                        Host = other;
                        break;
                    }
                }
            }
        }

        private void StopGame()
        {
            timer.Elapsed -= Timer1_Elapsed;
            RoomTerminated = true;

            //Debug("stop game2 " + Thread.CurrentThread.ManagedThreadId.ToString());
            List<Client> clients = new List<Client>(Clients);
            foreach (Client client in clients)
            {
                if (client.GameRoom == RoomId)
                    client.GameRoom = -1;
                client.Disconnected -= OnClientDisconnected;
                client.LeaveRoom -= OnClientLeave;
                client.GetReady -= OnClientReady;
                //client.GameControl -= ProcessClientPacket;
            }
            /*
            for (int i = 0; i < m_players.Count; i++)
            {
                player_ai[m_players[i]] = null;
                m_players[i] = null;
            }
            player_ai.Clear();
            m_players.Clear();
            */

            //Debug("delegate at " + Thread.CurrentThread.ManagedThreadId.ToString());
            if (create_new && Host.UserID < 0)
                Hall.RemoveRoom(this);
            else
                Hall.RemoveRoom(this, create_new ? Host : null, Clients);

            //RoomThread = null;
            if (thread != null)
            {
                thread.Abort();
                thread = null;
            }
        }

        private async void WaitForClear(int id)
        {
            await Task.Delay(60000);
            banned_clients.Remove(id);
        }

        //机器人在等待时的对话
        private async void IdleBeforeStart()
        {
            while (!RoomTerminated && !GameStarted)
            {
                Random rd = new Random();
                int time = 30000 + rd.Next(0, 20000);
                await Task.Delay(time);

                if (!GameStarted)
                {
                    List<Client> ais = new List<Client>();
                    List<Client> clients = new List<Client>(Clients);
                    foreach (Client client in clients)
                    {
                        if (client.UserID < 0)
                            ais.Add(client);
                    }

                    if (ais.Count > 0)
                    {
                        Shuffle.shuffle(ref ais);
                        Bot.IdleTalk(this, ais[0]);
                    }
                }
            }
        }

        #endregion

        #region 客户端准备游戏的状态
        private void OnClientReady(object sender, ClientEventArgs ready)
        {
            if (sender is Client client)
            {
                if (GameStarted || Host == client) return;
                lock (this)
                {
                    client.Status = ready.Bool ? Client.GameStatus.ready : Client.GameStatus.normal;
                    UpdateClientsInfo();
                }
            }
        }
        #endregion

        #region 修改房间设置
        public void ChangeSetting(GameSetting gameSetting)
        {
            lock (this)
            {
                gameSetting.Name = Setting.Name;
                gameSetting.PassWord = Setting.PassWord;
                gameSetting.GameMode = Setting.GameMode;
                gameSetting.PlayerNum = Setting.PlayerNum;
                Setting = gameSetting;

                SendRoomSetting2Client();
            }
        }

        private void SendRoomSetting2Client()
        {
            GameSetting setting = Setting;
            setting.PassWord = "*******";
            MyData data = new MyData()
            {
                Description = PacketDescription.Room2Cient,
                Protocol = Protocol.ConfigChange,
                Body = new List<string> { JsonUntity.Object2Json(setting) }
            };
            List<Client> clients = new List<Client>(Clients);
            foreach (Client client in clients)
                client.SendSwitchReply(data);
        }
        #endregion

        #region 通过游戏操作指令执行的客户端交互
        public void ProcessClientPacket(Client client, MyData data)
        {
            try
            {
                Interactivity interactivity = GetInteractivity(client.UserID);
                CommandType command = (CommandType)Enum.Parse(typeof(CommandType), data.Body[0]);
                List<string> arg = new List<string>(data.Body);
                arg.RemoveAt(0);

                switch (data.Protocol)
                {
                    case Protocol.GameReply when interactivity != null:
                        ProcessClientReply(interactivity, data);
                        break;
                    case Protocol.GameRequest when interactivity != null:
                        {
                            if (!interactivity.ControlGame(data))
                            {
                                if (interactions.ContainsKey(command))
                                    interactions[command](client, arg);
                                else
                                    Debug(string.Format("interactions not contains key {0}", data.Body[0]));
                            }
                            break;
                        }
                    case Protocol.GameNotification:
                        callbacks[command](client, arg);
                        break;
                }
            }
            catch (Exception e)
            {
                Debug(string.Format("error at room ProcessClientPacket {0} {1} {2}", e.Message, e.TargetSite, e.Source));
                LogHelper.WriteLog(null, e);
            }
        }

        public void ProcessClientReply(Interactivity player, MyData data)
        {
            try
            {
                lock (this)
                {
                    player.mutex.WaitOne();
                    bool success = false;
                    CommandType reply_command = (CommandType)Enum.Parse(typeof(CommandType), data.Body[0]);
                    CommandType player_command = player.ExpectedReplyCommand;
                    if (!player.IsWaitingReply || player.IsClientResponseReady)
                        Debug(string.Format("Server is not waiting for reply from {0}", GetClient(player.ClientId).UserName));
                    else if (reply_command != player_command
                             && (!m_requestResponsePair.ContainsKey(player_command) || m_requestResponsePair[player_command] != reply_command))
                        Debug(string.Format("Reply command should be {0} instead of {1}", player_command, reply_command));
                    //else if (packet.localSerial != player->m_expectedReplySerial)
                    //            emit room_message(tr("Reply serial should be %1 instead of %2")
                    //    .arg(player->m_expectedReplySerial).arg(packet.localSerial));
                    else
                        success = true;

                    if (success)
                    {
                        data.Body.RemoveAt(0);
                        player.ClientReply = data.Body;

                        if (!replies.ContainsKey(player_command))
                        {
                            player.IsClientResponseReady = true;
                            if (m_broadcardrequest)
                                m_broadcardrequest_result--;
                            if (!m_broadcardrequest || m_broadcardrequest_result <= 0)
                            {
                                timer.Enabled = false;
                                if (_waitHandle.SafeWaitHandle != null && !_waitHandle.SafeWaitHandle.IsClosed)
                                    _waitHandle.Set();
                            }
                            else if (reply_command == CommandType.S_COMMAND_CHOOSE_GENERAL)
                                OnChooseGeneralReply(player);
                        }
                    }

                    player.mutex.ReleaseMutex();
                    if (success && replies.ContainsKey(player_command))
                        replies[player_command](player);
                }
            }
            catch (Exception e)
            {
                Debug(string.Format("error at room ProcessClientReply {0} {1} {2}", e.Message, e.TargetSite, e.Source));
                LogHelper.WriteLog(null, e);
            }
        }

        private void OnChooseGeneralReply(Interactivity client)
        {
            Scenario.OnChooseGeneralReply(this, client);
        }

        private void OnRaceReply(Interactivity client)
        {
            client.mutex.WaitOne();
            // do not give this client any more change
            client.ExpectedReplyCommand = CommandType.S_COMMAND_UNKNOWN;
            client.IsWaitingReply = false;
            client.IsClientResponseReady = true;

            if (_m_raceStarted && _m_raceWinner == null)
            {
                bool check = false;
                if (client.ClientReply != null && client.ClientReply.Count > 1)
                {
                    Player player = FindPlayer(client.ClientReply[0]);
                    WrappedCard card = RoomLogic.ParseCard(this, client.ClientReply[1]);
                    if (player != null && player.ClientId == client.ClientId && card != null)
                    {
                        check = true;
                        _m_raceWinner = player;
                    }
                }

                m_broadcardrequest_result--;
                if (check || m_broadcardrequest_result <= 0)
                {
                    timer.Enabled = false;
                    if (_waitHandle.SafeWaitHandle != null && !_waitHandle.SafeWaitHandle.IsClosed)
                        _waitHandle.Set();
                }
            }
            client.mutex.ReleaseMutex();
        }

        private void OnLuckCardReply(Interactivity player)
        {
            player.mutex.WaitOne();
            bool check = true;
            if (m_broadcardrequest && player_luckcard.ContainsKey(player) && player_luckcard[player] > 0)

            {
                List<string> clientReply = player.ClientReply;
                if (clientReply == null || clientReply.Count == 0 || !bool.TryParse(clientReply[0], out bool succesful) || !succesful)
                {
                    check = false;
                }
                else
                {
                    player_luckcard[player] = player_luckcard[player] - 1;
                    LogMessage log = new LogMessage
                    {
                        Type = "#UseLuckCard"
                    };
                    foreach (Player p in GetPlayers(player.ClientId))
                    {
                        log.From = p.Name;
                        SendLog(log);
                    }

                    List<int> draw_list = new List<int>();
                    foreach (Player p in GetPlayers(player.ClientId))
                    {
                        draw_list.Add(p.HandcardNum);

                        CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_PUT, p.Name, "luck_card", string.Empty);
                        List<CardsMoveStruct> moves = new List<CardsMoveStruct>();
                        CardsMoveStruct move = new CardsMoveStruct(p.GetCards("h"), null, Place.DrawPile, reason)
                        {
                            From = p.Name,
                            From_place = Place.PlaceHand
                        };
                        moves.Add(move);
                        moves = _breakDownCardMoves(moves);

                        List<Player> tmp_list = new List<Player> { p };

                        NotifyMoveCards(true, moves, false, tmp_list);


                        for (int j = 0; j < move.Card_ids.Count; j++)
                        {
                            int card_id = move.Card_ids[j];
                            RoomLogic.RemovePlayerCard(this, p, card_id, Place.PlaceHand);
                        }

                        UpdateCardsOnLose(move);
                        for (int j = 0; j < move.Card_ids.Count; j++)
                            SetCardMapping(move.Card_ids[j], null, Place.DrawPile);
                        UpdateCardsOnGet(move);

                        NotifyMoveCards(false, moves, false, tmp_list);
                        for (int j = 0; j < move.Card_ids.Count; j++)
                        {
                            int card_id = move.Card_ids[j];
                            m_drawPile.Insert(0, card_id);
                        }
                    }
                    Shuffle.shuffle<int>(ref m_drawPile);
                    int index = -1;
                    foreach (Player p in GetPlayers(player.ClientId))
                    {
                        index++;
                        List<CardsMoveStruct> moves = new List<CardsMoveStruct>();
                        CardsMoveStruct move = new CardsMoveStruct(GetNCards(draw_list[index], false), p, Place.PlaceHand, new CardMoveReason())
                        {
                            From = null,
                            From_place = Place.DrawPile,
                        };
                        moves.Add(move);
                        moves = _breakDownCardMoves(moves);

                        NotifyMoveCards(true, moves, false);
                        for (int j = 0; j < move.Card_ids.Count; j++)
                        {
                            int card_id = move.Card_ids[j];
                            m_drawPile.Remove(card_id);
                        }

                        UpdateCardsOnLose(move);
                        for (int j = 0; j < move.Card_ids.Count; j++)
                            SetCardMapping(move.Card_ids[j], p, Place.PlaceHand);
                        UpdateCardsOnGet(move);

                        NotifyMoveCards(false, moves, false);
                        for (int j = 0; j < move.Card_ids.Count; j++)
                        {
                            int card_id = move.Card_ids[j];
                            RoomLogic.AddPlayerCard(this, p, card_id, Place.PlaceHand);
                        }
                    }
                    DoBroadcastNotify(CommandType.S_COMMAND_UPDATE_PILE, new List<string> { m_drawPile.Count.ToString() });
                }
            }
            else
                check = false;

            if (!check || player_luckcard[player] == 0)
            {
                DoNotify(GetClient(player.ClientId), CommandType.S_COMMAND_UNKNOWN, new List<string> { true.ToString() });
                player.ExpectedReplyCommand = CommandType.S_COMMAND_UNKNOWN;
                player.IsWaitingReply = false;
                player.IsClientResponseReady = true;
                m_broadcardrequest_result--;
                player.mutex.ReleaseMutex();
            }
            else
            {
                player.mutex.ReleaseMutex();
                DoRequest(player, player.ExpectedReplyCommand, player.CommandArgs, 0, false);
            }
            if (!m_broadcardrequest || m_broadcardrequest_result <= 0)
            {
                timer.Enabled = false;
                if (_waitHandle.SafeWaitHandle != null && !_waitHandle.SafeWaitHandle.IsClosed)
                    _waitHandle.Set();
            }
        }

        public void Provide(WrappedCard card)
        {
            provided = card;
            has_provided = true;
        }

        private readonly Dictionary<CommandType, Action<Client, List<string>>> interactions = new Dictionary<CommandType, Action<Client, List<string>>>();
        private readonly Dictionary<CommandType, Action<Client, List<string>>> callbacks = new Dictionary<CommandType, Action<Client, List<string>>>();
        private Dictionary<CommandType, Action<Interactivity>> replies = new Dictionary<CommandType, Action<Interactivity>>();
        private Dictionary<CommandType, CommandType> m_requestResponsePair = new Dictionary<CommandType, CommandType>();
        private void InitCallbacks()
        {
            // init request response pair
            m_requestResponsePair.Add(CommandType.S_COMMAND_PLAY_CARD, CommandType.S_COMMAND_RESPONSE_CARD);
            m_requestResponsePair.Add(CommandType.S_COMMAND_CHOOSE_DIRECTION, CommandType.S_COMMAND_MULTIPLE_CHOICE);
            m_requestResponsePair.Add(CommandType.S_COMMAND_LUCK_CARD, CommandType.S_COMMAND_INVOKE_SKILL);
            m_requestResponsePair.Add(CommandType.S_COMMAND_VIEW_GENERALS, CommandType.S_COMMAND_SKILL_GONGXIN);

            // client request handlers
            interactions[CommandType.S_COMMAND_SURRENDER] = new Action<Client, List<string>>(ProcessRequestSurrender);
            interactions[CommandType.S_COMMAND_CHEAT] = new Action<Client, List<string>>(ProcessRequestCheat);
            interactions[CommandType.S_COMMAND_PRESHOW] = new Action<Client, List<string>>(ProcessRequestPreshow);
            interactions[CommandType.S_COMMAND_CHANGE_SKIN] = new Action<Client, List<string>>(ChangeSkinCommand);
            interactions[CommandType.S_COMMAND_SHOWDISTANCE] = new Action<Client, List<string>>(ShowDistance);
            interactions[CommandType.S_COMMAND_TRUST] = new Action<Client, List<string>>(TrustCommand);

            // Client notifications
            callbacks[CommandType.S_COMMAND_GAME_START] = new Action<Client, List<string>>(GameStart);
            callbacks[CommandType.S_COMMAND_ADD_ROBOT] = new Action<Client, List<string>>(AddRobotCommand);
            callbacks[CommandType.S_COMMAND_FILL_ROBOTS] = new Action<Client, List<string>>(FillRobotsCommand);
            callbacks[CommandType.S_COMMAND_INTELSELECT] = new Action<Client, List<string>>(UpdateIntelSelect);

            // handle reply
            replies[CommandType.S_COMMAND_NULLIFICATION] = new Action<Interactivity>(OnRaceReply);
            replies[CommandType.S_COMMAND_LUCK_CARD] = new Action<Interactivity>(OnLuckCardReply);

            // Cheat commands
            //cheatCommands[".BroadcastRoles"] = &Room::broadcastRoles;
            //cheatCommands[".ShowHandCards"] = &Room::showHandCards;
            //cheatCommands[".ShowPrivatePile"] = &Room::showPrivatePile;
            //cheatCommands[".SetAIDelay"] = &Room::setAIDelay;
            //cheatCommands[".SetGameMode"] = &Room::setGameMode;
            //cheatCommands[".Pause"] = &Room::pause;
            //cheatCommands[".Resume"] = &Room::resume;
        }

        private void GameStart(Client client, List<string> data)
        {
            if (GameStarted || thread != null) return;
            bool check = true;
            if (Scenario.IsFull(this) && Host == client)
            {
                List<Client> clients = new List<Client>(Clients);
                foreach (Client c in clients)
                {
                    if (c != Host && c.Status != Client.GameStatus.bot && c.Status != Client.GameStatus.ready)
                    {
                        check = false;
                        break;
                    }
                }
            }

            if (!check) return;

            Hall.StartGame(this);
        }

        public void Run()
        {
            if (thread != null) return;

            GameStarted = true;
            Hall.BroadCastRoom(this);
            PrepareForStart();
            PreparePlayers();

            //加载本局游戏技能
            foreach (Player p in Players)
            {
                foreach (string skill in Engine.GetGeneralSkills(p.ActualGeneral1, Setting.GameMode))
                {
                    if (!Skills.Contains(skill))
                        Skills.Add(skill);

                    foreach (Skill _skill in Engine.GetRelatedSkills(skill))
                        if (!Skills.Contains(_skill.Name))
                            Skills.Add(_skill.Name);
                }

                foreach (string skill in Engine.GetGeneralRelatedSkills(p.ActualGeneral1, Setting.GameMode))
                {
                    if (!Skills.Contains(skill))
                        Skills.Add(skill);

                    foreach (Skill _skill in Engine.GetRelatedSkills(skill))
                        if (!Skills.Contains(_skill.Name))
                            Skills.Add(_skill.Name);
                }

                if (!string.IsNullOrEmpty(p.ActualGeneral2))
                {
                    foreach (string skill in Engine.GetGeneralSkills(p.ActualGeneral2, Setting.GameMode))
                    {
                        if (!Skills.Contains(skill))
                            Skills.Add(skill);

                        foreach (Skill _skill in Engine.GetRelatedSkills(skill))
                            if (!Skills.Contains(_skill.Name))
                                Skills.Add(_skill.Name);
                    }

                    foreach (string skill in Engine.GetGeneralRelatedSkills(p.ActualGeneral2, Setting.GameMode))
                    {
                        if (!Skills.Contains(skill))
                            Skills.Add(skill);

                        foreach (Skill _skill in Engine.GetRelatedSkills(skill))
                            if (!Skills.Contains(_skill.Name))
                                Skills.Add(_skill.Name);
                    }
                }
            }

            Current = m_players[0];

            RoomThread = Scenario.GetThread(this);
            thread = new Thread(RoomThread.Start);
            thread.Start();
        }

        private void PrepareForStart()
        {
            //添加玩家
            for (int i = 0; i < Clients.Count; i++)
            {
                m_players.Add(new Player { Name = string.Format("SGS{0}", i + 1), Seat = i + 1 });
                if (Clients[i].UserID > 0)
                {
                    intel_select.TryGetValue(Clients[i].UserID, out bool intel);
                    m_interactivities[Clients[i].UserID] = new Interactivity(this, Clients[i].UserID, intel);
                }
            }

            //加载武将
            Generals = Engine.GetGenerals(Setting.GeneralPackage, Setting.GameMode, false);
            //游戏卡牌、玩家身份、座次由剧本函数生成
            Scenario.PrepareForStart(this, ref m_players, ref pile1, ref m_drawPile);

            //更新游戏卡牌信息，通知客户端
            PrepareCards();
            foreach (int id in pile1)
            {
                FunctionCard fcard = Engine.GetFunctionCard(GetCard(id).Name);
                if (fcard != null && !AvailableFunctionCards.Contains(fcard))
                    AvailableFunctionCards.Add(fcard);
            }

            //将玩家座次信息通知客户端
            List<string> players = new List<string>();
            foreach (Player player in m_players)
            {
                Client client = Hall.GetClient(player.ClientId);
                if (client != null && client.Status == Client.GameStatus.online)
                {
                    player.Status = "online";
                    TrustedAI ai = null;
                    if (client.UserRight >= 3)
                        ai = Scenario.GetAI(this, player);
                    else
                        ai = new TrustedAI(this, player);
                    player_ai.Add(player, ai);
                }
                else if (client == null || client.Status == Client.GameStatus.bot)
                {
                    player.Status = "bot";
                    player_ai.Add(player, Scenario.GetAI(this, player));
                }

                players.Add(JsonUntity.Object2Json(player));
                player_client.Add(player, client);
            }

            DoBroadcastNotify(CommandType.S_COMMAND_ARRANGE_SEATS, players);

            //玩家选将由剧本函数完成，相关客户端通知也由此函数完成
            _alivePlayers = new List<Player>(m_players);
            Scenario.Assign(this);
            Scenario.SeatReAdjust(this, ref m_players);
        }

        //准备游戏卡牌
        private void PrepareCards()
        {
            //生成此room的专属卡牌
            foreach (int id in pile1)
            {
                WrappedCard real = Engine.GetRealCard(id);
                WrappedCard clone = Engine.CloneCard(real);
                m_cards.Add(id, new RoomCard(clone));
            }

            DoBroadcastNotify(CommandType.S_COMMAND_INIT_CARDS, new List<string> { JsonUntity.Object2Json(pile1) });

            foreach (int card_id in m_drawPile)
                SetCardMapping(card_id, null, Place.DrawPile);

            DoBroadcastNotify(CommandType.S_COMMAND_UPDATE_PILE, new List<string> { m_drawPile.Count.ToString() });
        }

        public void ReplaceRoomCard(int old_id, int new_id)
        {
            if (pile1.Contains(old_id) && !pile1.Contains(new_id) && old_id != new_id && (GetCardPlace(old_id) == Place.DrawPile || GetCardPlace(old_id) == Place.DiscardPile))
            {
                WrappedCard real = Engine.GetRealCard(new_id);
                if (real != null)
                {
                    RoomCard saber = new RoomCard(real);
                    RoomCard old = GetCard(old_id);
                    saber.Flags = old.Flags;
                    int index = m_drawPile.IndexOf(old_id);
                    m_drawPile.Remove(old_id);
                    m_drawPile.Insert(index, new_id);
                    cards_replace[old_id] = new_id;
                    SetCardMapping(new_id, null, GetCardPlace(old_id));
                    SetCardMapping(old_id, null, Place.PlaceUnknown);
                    m_cards.Add(new_id, saber);
                }
            }
        }

        public void AddNewCard(int id)
        {
            if (!pile1.Contains(id))
            {
                WrappedCard real = Engine.GetRealCard(id);
                if (real != null)
                {
                    RoomCard saber = new RoomCard(real);
                    SetCardMapping(id, null, Place.PlaceUnknown);
                    m_cards.Add(id, saber);
                    AddToPile(Players[0], "#virtual_cards", id, false);
                }
            }
        }

        public void PreparePlayers()
        {
            Scenario.PrepareForPlayers(this);
        }

        public void AddPlayerSkill(Player player, string skill_name, bool head_skill = true)
        {
            Skill skill = Engine.GetSkill(skill_name);
            if (skill != null)
            {
                bool preshow = (skill is TriggerSkill tskill) ? !tskill.CanPreShow() : true;
                player.AddSkill(skill_name, head_skill, preshow);
                List<string> args = new List<string> { GameEventType.S_GAME_EVENT_ADD_SKILL.ToString(), player.Name, skill_name, head_skill.ToString() };
                DoNotify(GetClient(player), CommandType.S_COMMAND_LOG_EVENT, args);
            }
        }

        public void HandleUsedGeneral(string general)
        {
            bool remove = general.StartsWith("-") ? true : false;
            string general_name = general;
            if (remove) general_name = general_name.Substring(1);
            string main = Engine.GetMainGeneral(Engine.GetGeneral(general_name, Setting.GameMode));
            if (remove)
                UsedGeneral.Remove(main);
            else if (!UsedGeneral.Contains(main))
                UsedGeneral.Add(main);
            foreach (General sub in Engine.GetConverPairs(main))
            {
                if (!Setting.GeneralPackage.Contains(sub.Package)) continue;
                if (remove)
                    UsedGeneral.Remove(sub.Name);
                else if (!UsedGeneral.Contains(sub.Name))
                    UsedGeneral.Add(sub.Name);
            }
        }

        private void ChangeSkinCommand(Client client, List<string> args)
        {
            if (args.Count != 3) return;
            Player who = FindPlayer(args[0], true);
            if (who == null)
            {
                Debug(string.Format("get no player {0}", args[0]));
                return;
            }
                
            int skin_id = int.Parse(args[1]);
            bool is_head = bool.Parse(args[2]);
            if (who.ClientId != client.UserID) return;

            string name = is_head ? who.ActualGeneral1 : who.ActualGeneral2;
            List<DataRow> datas = Engine.GetGeneralSkin(name, Setting.GameMode);
            bool check = false;
            foreach (DataRow data in datas)
            {
                if (data["skin_id"].ToString() == args[1])
                {
                    check = true;
                    break;
                }
            }
            if (!check) return;

            string propertyName;
            if (is_head)
            {
                who.HeadSkinId = skin_id;
                propertyName = "HeadSkinId";
            }
            else
            {
                who.DeputySkinId = skin_id;
                propertyName = "DeputySkinId";
            }

            bool show = is_head ? who.General1Showed : who.General2Showed;
            List<Client> clients = new List<Client>(Clients);
            foreach (Client target in clients)
            {
                if (client != target && !show) continue;
                NotifyProperty(target, who, propertyName);
            }
        }
        private void ShowDistance(Client client, List<string> args)
        {
            if (Finished || args.Count != 1) return;
            Player who = FindPlayer(args[0], true);
            if (who == null || who.ClientId != client.UserID)
            {
                Debug(string.Format("show distance get no player {0}", args[0]));
                return;
            }

            Dictionary<string, int> distance = new Dictionary<string, int>();
            foreach (Player p in GetOtherPlayers(who))
                distance.Add(p.Name, RoomLogic.DistanceTo(this, who, p));
            
            List<string> arg = new List<string> { GameEventType.S_GAME_EVENT_SHOWDISTANCE.ToString(), JsonUntity.Dictionary2Json(distance) };
            DoNotify(client, CommandType.S_COMMAND_LOG_EVENT, arg);
        }
        private void TrustCommand(Client client, List<string> arg)
        {
            lock (this)
            {
                if (client.Status == Client.GameStatus.online)
                {
                    bool trust = false;

                    foreach (Player p in GetPlayers(client.UserID))
                    {
                        if (!trust && p.Status == "online")
                            trust = true;
                        p.Status = trust ? "trust" : "online";
                        BroadcastProperty(p, "Status");
                    }
                    DoNotify(client, CommandType.S_COMMAND_TRUST, new List<string>());
                    Interactivity interactivity = GetInteractivity(client.UserID);
                    if (trust && interactivity.IsWaitingReply)
                    {
                        MyData data = new MyData
                        {
                            Body = new List<string> { interactivity.ExpectedReplyCommand.ToString() }
                        };
                        ProcessClientReply(interactivity, data);
                    }
                }
            }
        }

        private void UpdateIntelSelect(Client client, List<string> arg2)
        {
            if (client != null)
            {
                intel_select[client.UserID] = bool.Parse(arg2[0]);
                Interactivity interactivity = GetInteractivity(client.UserID);
                if (interactivity != null)
                    interactivity.IntelSelect = intel_select[client.UserID];
            }
        }

        private void FillRobotsCommand(Client arg1, List<string> arg2)
        {
            throw new NotImplementedException();
        }

        private void AddRobotCommand(Client client, List<string> data)
        {
            int index = int.Parse(data[0]);
            lock (this)
            {
                if (client == Host && !IsFull() && !GameStarted && seat2clients[index] == null)
                {
                    int bot_id = Hall.GetBotId();
                    /*
                    Profile profile = new Profile
                    {
                        NickName = string.Format("女装ZY {0}号", Math.Abs(bot_id)),
                        Avatar = 100156,
                        Frame = 200156,
                        Bg = 300156,
                        UId = bot_id,
                        Title = 0,
                    };
                    */
                    Profile profile = Bot.GetBot(this);
                    profile.UId = bot_id;
                    Client bot = new Client(Hall, profile)
                    {
                        GameRoom = RoomId,
                        Status = Client.GameStatus.bot,
                    };
                    Hall.AddBot(bot);
                    Clients.Add(bot);
                    bot.LeaveRoom += OnClientLeave;

                    seat2clients[index] = bot;
                    RoomMessage.NotifyPlayerJoinorLeave(this, bot, true);
                    UpdateClientsInfo();

                    Bot.SayHellow(this, bot);
                }
            }
        }

        public void Speak(Client client, string message)
        {
            if (client == null) return;
            if (client.UserRight < 3 && GameStarted && Setting.SpeakForbidden) return;
            MyData data = new MyData
            {
                Description = PacketDescription.Room2Cient,
                Protocol = Protocol.Message2Room,
                Body = new List<string> { client.UserID.ToString(), message }
            };
            List<Client> clients = new List<Client>(Clients);
            foreach (Client dest in clients)
                if (dest.GameRoom == RoomId)
                    dest.SendMessage(data);
        }

        public void Emotion(Client client, string group, string message)
        {
            MyData data = new MyData
            {
                Description = PacketDescription.Room2Cient,
                Protocol = Protocol.Message2Room,
                Body = new List<string> { client.UserID.ToString(), group, message }
            };
            List<Client> clients = new List<Client>(Clients);
            foreach (Client dest in clients)
                if (dest.GameRoom == RoomId)
                    dest.SendMessage(data);
        }

        private void ProcessRequestPreshow(Client client, List<string> args)
        {
            if (args.Count != 3) return;

            Player player = FindPlayer(args[0]);
            string skill_name = args[1];
            TriggerSkill skill = Engine.GetTriggerSkill(skill_name);
            Interactivity interactivity = GetInteractivity(player);
            if (player == null || interactivity == null || !player.OwnSkill(skill_name) || skill == null || !skill.CanPreShow()) return;
            interactivity.mutex.WaitOne();
            bool head = bool.Parse(args[2]);
            bool isPreshowed = player.HasPreshowedSkill(skill_name, head);
            player.SetSkillPreshowed(skill_name, !isPreshowed, head);
            NotifyPlayerPreshow(player, head ? "h" : "d");
            interactivity.mutex.ReleaseMutex();
        }

        private void ProcessRequestCheat(Client client, List<string> arg)
        {
            if (client.UserRight < 3) return;

            Interactivity interactivity = GetInteractivity(client.UserID);
            interactivity.CheatArgs = arg;
            if (interactivity.IsWaitingReply
                && interactivity.ExpectedReplyCommand == CommandType.S_COMMAND_PLAY_CARD
                && _m_roomState.GetCurrentCardUseReason() == CardUseStruct.CardUseReason.CARD_USE_REASON_PLAY)
            {
                timer.Enabled = false;
                if (_waitHandle.SafeWaitHandle != null && !_waitHandle.SafeWaitHandle.IsClosed)
                    _waitHandle.Set();
            }
        }

        private bool MakeCheat(Player player)
        {
            Interactivity interactivity = GetInteractivity(player);
            if (interactivity.CheatArgs == null || interactivity.CheatArgs.Count == 0) return false;

            List<string> arg = new List<string>(interactivity.CheatArgs);
            interactivity.CheatArgs = null;
            CheatCode code = (CheatCode)Enum.Parse(typeof(CheatCode), arg[0]);
            //if (code == CheatCode.S_CHEAT_KILL_PLAYER)
            //{
            //    JsonArray arg1 = arg[1].value<JsonArray>();
            //    if (!JsonUtils::isStringArray(arg1, 0, 1)) return false;
            //    makeKilling(arg1[0].toString(), arg1[1].toString());

            //}
            //else if (code == S_CHEAT_MAKE_DAMAGE)
            //{
            //    JsonArray arg1 = arg[1].value<JsonArray>();
            //    if (arg1.size() != 4 || !JsonUtils::isStringArray(arg1, 0, 1)
            //        || !JsonUtils::isNumber(arg1[2]) || !JsonUtils::isNumber(arg1[3]))
            //        return false;
            //    makeDamage(arg1[0].toString(), arg1[1].toString(),
            //        (QSanProtocol::CheatCategory)arg1[2].toInt(), arg1[3].toInt());

            //}
            //else if (code == S_CHEAT_REVIVE_PLAYER)
            //{
            //    if (!JsonUtils::isString(arg[1])) return false;
            //    makeReviving(arg[1].toString());

            //}
            //else if (code == S_CHEAT_RUN_SCRIPT)
            //{
            //    if (!JsonUtils::isString(arg[1])) return false;

            //    QByteArray data = QByteArray::fromBase64(arg[1].toString().toLatin1());
            //    data = qUncompress(data);

            //    if (data == "aidebug")
            //    {
            //        player->getSmartAI()->outputAIdebug();
            //        return true;
            //    }

            //    doScript(data);

            //}
            if (code == CheatCode.S_CHEAT_GET_ONE_CARD)
            {
                if (int.TryParse(arg[1], out int card_id))
                {
                    if (cards_replace.ContainsKey(card_id))
                        card_id = cards_replace[card_id];

                    if (m_cards.ContainsKey(card_id))
                    {
                        LogMessage log = new LogMessage
                        {
                            Type = "$CheatCard",
                            From = player.Name,
                            Card_str = arg[1]
                        };
                        SendLog(log);
                        ObtainCard(player, card_id);
                        return true;
                    }
                }
            }

            return false;
        }

        private void ProcessRequestSurrender(Client client, List<string> obj)
        {
            lock (Surrender)
            {
                Interactivity inter = GetInteractivity(client.UserID);
                bool success = Surrender.TryGetValue(inter, out bool available);
                if (client.UserID > 0 && string.IsNullOrEmpty(PreWinner) && available)
                {
                    List<Interactivity> clients = new List<Interactivity>(Surrender.Keys);
                    Surrender.Clear();
                    foreach (Interactivity _inter in clients)
                    {
                        Client to_notify = GetClient(_inter.ClientId);
                        DoNotify(to_notify, CommandType.S_COMMAND_ENABLE_SURRENDER, new List<string> { false.ToString() });
                    }

                    PreWinner = Scenario.GetPreWinner(this, client);

                    foreach (Interactivity interactivity in m_interactivities.Values)
                    {
                        if (interactivity.IsWaitingReply)
                        {
                            MyData data = new MyData
                            {
                                Body = new List<string> { interactivity.ExpectedReplyCommand.ToString() }
                            };

                            ProcessClientReply(interactivity, data);
                        }
                    }
                }
            }
        }

        #endregion

        #region 客户端重连游戏
        private void Marshal(Client client)
        {
            DoNotify(client, CommandType.S_COMMAND_INIT_CARDS, new List<string> { JsonUntity.Object2Json(pile1) });
            DoNotify(client, CommandType.S_COMMAND_UPDATE_PILE, new List<string> { m_drawPile.Count.ToString() });

            //玩家信息
            List<string> players = new List<string>();
            foreach (Player player in m_players)
            {
                if (player.ClientId == client.UserID)
                    player_client[player] = client;

                Player _player = Scenario.Marshal(this, client, player);
                players.Add(JsonUntity.Object2Json(_player));
            }

            //通知客户端
            DoNotify(client, CommandType.S_COMMAND_ARRANGE_SEATS, players);
            //五谷丰登
            if (!string.IsNullOrEmpty(table_ag.Key) && table_ag.Value.Count > 0)
                FillAG(table_ag.Key, table_ag.Value, GetPlayers(client.UserID)[0]);
            //鏖战通知
            if (BloodBattle)
                DoNotify(client, CommandType.S_COMMAND_GAMEMODE_BLOODBATTLE, new List<string>());

            //装备、手牌
            List<Player> _players = new List<Player> { GetPlayers(client.UserID)[0] };
            foreach (Player player in m_players)
            {
                if (player.IsAllNude()) continue;

                List<CardsMoveStruct> moves = new List<CardsMoveStruct>();

                if (!player.IsKongcheng())
                {
                    CardsMoveStruct move = new CardsMoveStruct
                    {
                        Card_ids= player.GetCards("h"),
                        From_place = Place.DrawPile,
                        To = player.Name,
                        To_place = Place.PlaceHand,
                        Reason = new CardMoveReason(MoveReason.S_REASON_DRAW, player.Name)
                    };
                    moves.Add(move);
                }

                if (player.HasEquip())
                {
                    CardsMoveStruct move = new CardsMoveStruct
                    {
                        Card_ids = player.GetEquips(),
                        From_place = Place.DrawPile,
                        To = player.Name,
                        To_place = Place.PlaceEquip,
                        Reason = new CardMoveReason(MoveReason.S_REASON_USE, player.Name)
                    };
                    moves.Add(move);
                }

                if (player.JudgingArea.Count > 0)
                {
                    foreach (int id in player.JudgingArea)
                    {
                        CardsMoveStruct move = new CardsMoveStruct
                        {
                            Card_ids = new List<int> { id },
                            From_place = Place.DrawPile,
                            To = player.Name,
                            To_place = Place.PlaceDelayedTrick,
                            Reason = new CardMoveReason(MoveReason.S_REASON_USE, player.Name)
                        };
                        move.Reason.Card = GetCard(id);

                        moves.Add(move);
                    }
                }

                NotifyMoveCards(true, moves, false, _players);
                NotifyMoveCards(false, moves, false, _players);
            }

            foreach (Player p in GetPlayers(client.UserID))
            {
                p.Status = "online";
                BroadcastProperty(p, "Status");
            }

            Interactivity inter = GetInteractivity(client.UserID);
            if (inter != null && Surrender.TryGetValue(inter, out bool enable) && enable)
            {
                DoNotify(client, CommandType.S_COMMAND_ENABLE_SURRENDER, new List<string> { true.ToString() });
            }
        }
        #endregion

        public void Debug(string msg)
        {
            Hall.Debug(msg);
        }

        public void SetTag(string key, object value)
        {
            tag[key] = value;
            //if (scenario) scenario->onTagSet(this, key);
        }

        public bool ContainsTag(string key) => tag.ContainsKey(key);

        public object GetTag(string key)
        {
            if (tag.ContainsKey(key))
                return tag[key];
            else
                return null;
        }

        public void RemoveTag(string key)
        {
            if (tag.ContainsKey(key))
                tag.Remove(key);
        }

        public Client GetClient(Player player)
        {
            if (player_client.ContainsKey(player))
                return player_client[player];

            return null;
        }

        public Client GetClient(int client_id)
        {
            List<Client> clients = new List<Client>(Clients);
            foreach (Client client in clients)
                if (client.UserID == client_id)
                    return client;

            return null;
        }

        public Interactivity GetInteractivity(Player player)
        {
            return GetInteractivity(player.ClientId);
        }
        public Interactivity GetInteractivity(int client_id)
        {
            m_interactivities.TryGetValue(client_id, out Interactivity interactivity);
            return interactivity;
        }

        public TrustedAI GetAI(Player player, bool ai = false)
        {
            Client client = Hall.GetClient(player.ClientId);
            if (ai || client == null ||  client.Status == Client.GameStatus.bot || client.Status == Client.GameStatus.offline || player.Status == "trust" || player.Status == "escape")
                return player_ai[player];

            return null;
        }


        #region 将游戏信息通知客户端
        public bool DoNotify(Client client, CommandType command, List<string> message_body)
        {
            if (client == null || client.GameRoom != RoomId) return false;
            List<string> _message = new List<string>(message_body);
            _message.Insert(0, command.ToString());
            client.SendRoomNotify(_message);
            return true;
        }

        public bool DoBroadcastNotify(CommandType command, List<string> message_body, Client except = null)
        {
            List<Client> clients = new List<Client>(Clients);
            foreach (Client player in clients)
                if (except == null || player != except)
                    DoNotify(player, command, message_body);

            return true;
        }

        public bool DoBroadcastNotify(List<Client> clients, CommandType command, List<string> message_body)
        {
            foreach (Client player in clients)
                DoNotify(player, command, message_body);
            return true;
        }

        public bool DoBroadcastNotify(CommandType command, List<string> message_body)
        {
            return DoBroadcastNotify(new List<Client>(Clients), command, message_body);
        }

        public bool NotifyProperty(Client clientToNotify, Player propertyOwner, string propertyName, string value = null)
        {
            if (propertyOwner == null || clientToNotify == null) return false;
            if (string.IsNullOrEmpty(value)) value = propertyOwner.GetType().GetProperty(propertyName).GetValue(propertyOwner).ToString();

            List<string> arg = new List<string>
            {
                propertyOwner.Name,
                propertyName,
                value
            };

            return DoNotify(clientToNotify, CommandType.S_COMMAND_SET_PROPERTY, arg);
        }

        public bool BroadcastProperty(Player player, string propertyName, string value = null)
        {
            if (player == null) return false;
            if (string.IsNullOrEmpty(value)) value = player.GetType().GetProperty(propertyName).GetValue(player).ToString();

            if (string.Equals(propertyName, "Role"))
                player.RoleShown = true;

            List<string> arg = new List<string>
            {
                player.Name,
                propertyName,
                value
            };
            return DoBroadcastNotify(CommandType.S_COMMAND_SET_PROPERTY, arg);
        }
        #endregion

        #region 通知客户端显示倒计时条
        public bool NotifyMoveFocus(Player focus)
        {
            List<Player> players = new List<Player> { focus };
            Countdown countdown = new Countdown
            {
                Type = Countdown.CountdownType.S_COUNTDOWN_NO_LIMIT
            };
            return NotifyMoveFocus(players, countdown, focus);
        }

        bool NotifyMoveFocus(Player focus, CommandType command)
        {
            List<Player> players = new List<Player> { focus };
            Countdown countdown = new Countdown
            {
                Max = Setting.GetCommandTimeout(command, ProcessInstanceType.S_CLIENT_INSTANCE),
                Command = command
            };
            if (countdown.Max == Setting.GetCommandTimeout(CommandType.S_COMMAND_UNKNOWN, ProcessInstanceType.S_CLIENT_INSTANCE))
            {
                countdown.Type = Countdown.CountdownType.S_COUNTDOWN_USE_DEFAULT;
            }
            else
            {
                countdown.Type = Countdown.CountdownType.S_COUNTDOWN_USE_SPECIFIED;
            }

            return NotifyMoveFocus(players, countdown, focus);
        }
        public bool NotifyMoveFocus(List<Player> focuses, Countdown countdown, Player except = null)
        {
            List<string> arg = new List<string>();
            //============================================
            //for protecting anjiang
            //============================================
            bool verify = false;
            foreach (Player focus in focuses)
            {
                if (focus.HasFlag("Global_askForSkillCost"))
                {
                    verify = true;
                    break;
                }
            }

            List<string> players = new List<string>();
            if (!verify)
            {
                for (int i = 0; i < focuses.Count; i++)
                {
                    players.Add(focuses[i].Name);
                }
            }
            else
            {
                foreach (Player p in GetAlivePlayers())
                    players.Add(p.Name);
            }
            arg.Add(JsonUntity.Object2Json(players));
            arg.Add(JsonUntity.Object2Json(countdown));


            return DoBroadcastNotify(CommandType.S_COMMAND_MOVE_FOCUS, arg, except != null ? GetClient(except) : null);
        }

        public void FocusAll(int time)
        {
            //notify focus
            Countdown countdown = new Countdown
            {
                Max = time,
                Type = Countdown.CountdownType.S_COUNTDOWN_USE_ALL
            };

            List<string> names = new List<string>();
            foreach (Player p in GetAlivePlayers())
                names.Add(p.Name);
            List<string> arg = new List<string> { JsonUntity.Object2Json(names), JsonUntity.Object2Json(countdown) };
            DoBroadcastNotify(CommandType.S_COMMAND_MOVE_FOCUS, arg, null);
            Thread.Sleep(time + 200);
            DoBroadcastNotify(CommandType.S_COMMAND_UNKNOWN, new List<string> { false.ToString() });
        }

        #endregion

        #region 向客户端发送请求
        //全局请求（选将）
        public bool DoBroadcastRequest(List<Interactivity> receivers, CommandType command)
        {
            float timeOut = Setting.GetCommandTimeout(command, ProcessInstanceType.S_SERVER_INSTANCE);
            return DoBroadcastRequest(receivers, command, timeOut);
        }
        private bool m_broadcardrequest = false;
        private int m_broadcardrequest_result;
        public bool DoBroadcastRequest(List<Interactivity> players, CommandType command, float timeOut, bool wait = false)
        {
            List<Interactivity> receivers = new List<Interactivity>();
            foreach (Interactivity player in players)
            {
                if (GetPlayers(player.ClientId)[0].Status == "online")
                    receivers.Add(player);
            }
            if (receivers.Count == 0 && !(timeOut > 0 && wait)) return true;
            m_broadcardrequest_result = receivers.Count;
            m_broadcardrequest = true;
            foreach (Interactivity player in receivers)
                DoRequest(player, command, player.CommandArgs, timeOut, false);

            Stopwatch sw = new Stopwatch();
            sw.Start();
            float remainTime = timeOut;
            if (timeOut != 0 || Setting.ControlTime != 0)
            {
                StartWaitingReply(timeOut);
            }
            if (_waitHandle.SafeWaitHandle != null && !_waitHandle.SafeWaitHandle.IsClosed) _waitHandle.WaitOne();
            m_broadcardrequest = false;
            sw.Stop();
            remainTime -= sw.ElapsedMilliseconds;

            if (wait && remainTime > 0)
                Thread.Sleep((int)remainTime);

            return true;
        }

        public Player DoBroadcastRaceRequest(List<Player> players, CommandType command, float timeOut)
        {
            _m_raceStarted = true;
            _m_raceWinner = null;

            List<Interactivity> receivers = new List<Interactivity>();
            foreach (Player player in players)
            {
                Interactivity client = GetInteractivity(player);
                if (GetAI(player) == null && !receivers.Contains(client))
                    receivers.Add(client);
            }

            m_broadcardrequest_result = receivers.Count;
            foreach (Interactivity player in receivers)
                DoRequest(player, command, player.CommandArgs, timeOut, false);

            /*
                doEventLoop(timeOut);

                _m_semRoomMutex.tryAcquire();

                ServerPlayer *winner = _m_raceWinner;
                _m_raceWinner = NULL;
                if (winner == NULL && _m_AIraceWinner != NULL) winner = _m_AIraceWinner;

                foreach (ServerClient *player, receivers) {
                    player->acquireLock(ServerClient::SEMA_MUTEX);
                    player->m_expectedReplyCommand = S_COMMAND_UNKNOWN;
                    player->m_isWaitingReply = false;
                    player->m_expectedReplySerial = -1;
                    player->releaseLock(ServerClient::SEMA_MUTEX);
                }
                _m_raceStarted = false;
                _m_semRoomMutex.release();
            */

            float time = timeOut;
            if (_m_AIraceWinner != null)
            {
                time = 0;
                if (Setting.NullTime == 0)
                {
                    time = 5000 - 800;
                }
                else
                {
                    time = Setting.GetCommandTimeout(CommandType.S_COMMAND_NULLIFICATION, ProcessInstanceType.S_CLIENT_INSTANCE);
                    if (time > 1800) time -= 1800;
                }
                Random rd = new Random();
                time = 800 + rd.Next(0, (int)time);
            }

            return GetRaceResult(receivers, command, time);
        }

        public Player GetRaceResult(List<Interactivity> players, CommandType command, float timeOut)
        {

            if (timeOut != 0 || Setting.NullTime != 0)
            {
                StartWaitingReply(timeOut);
            }
            if (_waitHandle.SafeWaitHandle != null && !_waitHandle.SafeWaitHandle.IsClosed)
                _waitHandle.WaitOne();
            _m_raceStarted = false;

            lock (this)
            {
                foreach (Interactivity player in players)
                {
                    player.mutex.WaitOne();
                    player.ExpectedReplyCommand = CommandType.S_COMMAND_UNKNOWN;
                    player.IsWaitingReply = false;
                    //player->m_expectedReplySerial = -1;
                    player.mutex.ReleaseMutex();
                }
            }

            if (_m_raceWinner != null)
            {
                return _m_raceWinner;
            }
            else
            {
                return _m_AIraceWinner;
            }
        }

        public bool DoRequest(Player player, CommandType command, List<string> arg, bool wait = true)
        {
            if (command == CommandType.S_COMMAND_PLAY_CARD && player.HasFlag("kuangcai"))                       //狂才耦合
            {
                float timeOut = 6000 - player.GetMark("kuangcai") * 1000;
                return DoRequest(GetInteractivity(player), command, arg, timeOut, wait);
            }
            else if (command  == CommandType.S_COMMAND_RESPONSE_CARD && player.HasFlag("kuangcai"))
            {
                float timeOut = 6000;
                return DoRequest(GetInteractivity(player), command, arg, timeOut, wait);
            }
            else
            {
                float timeOut = Setting.GetCommandTimeout(command, ProcessInstanceType.S_SERVER_INSTANCE);
                return DoRequest(GetInteractivity(player), command, arg, timeOut, wait);
            }
        }

        public bool DoRequest(Player player, CommandType command, List<string> arg, float timeOut, bool wait)
        {
            return DoRequest(GetInteractivity(player), command, arg, timeOut, wait);
        }
        public bool DoRequest(Interactivity interactivity, CommandType command, List<string> arg, float timeOut, bool wait)
        {
            if (interactivity == null) return false;
            interactivity.mutex.WaitOne();
            interactivity.IsClientResponseReady = false;
            interactivity.ClientReply = null;
            interactivity.IsWaitingReply = true;
            interactivity.ExpectedReplyCommand = command;

            Client client = GetClient(interactivity.ClientId);
            if (client != null)
                client.SendRoomRequest(command, arg);
            
            interactivity.mutex.ReleaseMutex();
            if (wait) return GetResult(interactivity, timeOut);
            else return true;
        }

        bool GetResult(Interactivity player, float timeOut)
        {
            bool validResult = false;
            player.mutex.WaitOne();

            List<Player> players = GetPlayers(player.ClientId);
            if (players[0].Status == "online")
            {
                player.mutex.ReleaseMutex();

                if (timeOut != 0 || Setting.ControlTime != 0)
                {
                    StartWaitingReply(timeOut);
                }
                if (_waitHandle.SafeWaitHandle != null && !_waitHandle.SafeWaitHandle.IsClosed)
                    _waitHandle.WaitOne();

                // Note that we rely on processResponse to filter out all unrelevant packet.
                // By the time the lock is released, m_clientResponse must be the right message
                // assuming the client side is not tampered.

                // Also note that lock can be released when a player switch to trust or offline status.
                // It is ensured by trustCommand and reportDisconnection that the player reports these status
                // is the player waiting the lock. In these cases, the serial number and command type doesn't matter.

                player.mutex.WaitOne();
                validResult = player.IsClientResponseReady;
            }
            player.ExpectedReplyCommand = CommandType.S_COMMAND_UNKNOWN;
            player.IsWaitingReply = false;
            //player->m_expectedReplySerial = -1;
            player.mutex.ReleaseMutex();
            return validResult;
        }

        private EventWaitHandle _waitHandle = new AutoResetEvent(false);
        public void StartWaitingReply(float time)
        {
            if (timer != null)
            {
                timer.Enabled = true;
                timer.Interval = time;//执行间隔时间,单位为毫秒
                timer.AutoReset = false;    //循环执行
                timer.Start();
            }
        }

        private void Timer1_Elapsed(object sender, ElapsedEventArgs e)
        {
            System.Timers.Timer timer = (System.Timers.Timer)sender;
            timer.Enabled = false;
            if (_waitHandle.SafeWaitHandle != null && !_waitHandle.SafeWaitHandle.IsClosed)
                _waitHandle.Set();
        }
        #endregion

        //通知客户端更新技能preshow
        public void NotifyPlayerPreshow(Player player, string flag = "hd")
        {
            if (flag.Contains("h"))
            {
                List<string> args = new List<string>
                {
                    GameEventType.S_GAME_EVENT_UPDATE_PRESHOW.ToString(),
                    player.Name,
                    true.ToString(),
                    JsonUntity.Dictionary2Json<string, bool>(player.HeadSkills)
                };

                DoNotify(GetClient(player), CommandType.S_COMMAND_LOG_EVENT, args);
            }

            if (flag.Contains("d"))
            {
                List<string> args = new List<string>
                {
                    GameEventType.S_GAME_EVENT_UPDATE_PRESHOW.ToString(),
                    player.Name,
                    false.ToString(),
                    JsonUntity.Dictionary2Json<string, bool>(player.DeputySkills)
                };

                DoNotify(GetClient(player), CommandType.S_COMMAND_LOG_EVENT, args);
            }
        }
        public void UpdateSkill(Player player)
        {
            DoNotify(GetClient(player), CommandType.S_COMMAND_LOG_EVENT, new List<string> { GameEventType.S_GAME_EVENT_UPDATE_SKILL.ToString() });
        }

        public void ChangePlayerGeneral(Player player, string new_general)
        {
            player.General1 = new_general;
            List<Client> players = new List<Client>(Clients);
            if (new_general == "anjiang") players.Remove(GetClient(player));

            foreach (Client p in players)
                NotifyProperty(p, player, "General1");

            if (new_general != "anjiang")
                player.PlayerGender = Engine.GetGeneral(new_general, Setting.GameMode).GeneralGender;
            else
            {
                if (player.General2Showed)
                    player.PlayerGender = Engine.GetGeneral(player.General2, Setting.GameMode).GeneralGender;
                else
                    player.PlayerGender = Gender.Sexless;
            }
            BroadcastProperty(player, "PlayerGender");

            FilterCards(player, player.GetCards("he"), true);
        }

        public void ChangePlayerGeneral2(Player player, string new_general)
        {
            player.General2 = new_general;
            List<Client> players = new List<Client>(Clients);
            if (new_general == "anjiang") players.Remove(GetClient(player));

            foreach (Client p in players)
                NotifyProperty(p, player, "General2");


            if (!player.General1Showed)
            {
                if (new_general != "anjiang")
                {
                    player.PlayerGender = Engine.GetGeneral(new_general, Setting.GameMode).GeneralGender;
                }
                else
                {
                    player.PlayerGender = Gender.Sexless;
                }
                BroadcastProperty(player, "PlayerGender");
            }

            FilterCards(player, player.GetCards("he"), true);
        }

        public void ShowGeneral(Player player, bool head_general = true, bool trigger_event = true, bool sendLog = true, bool ignore_rule = true)
        {
            //room->tryPause();
            Thread.Sleep(300);

            if (head_general)
            {
                string general_name = player.ActualGeneral1;
                if (string.IsNullOrEmpty(general_name)) return;
                if (!ignore_rule && !player.CanShowGeneral("h")) return;
                if (player.General1 != "anjiang") return;

                player.SetSkillsPreshowed("h");
                NotifyPlayerPreshow(player, "h");
                player.General1Showed = true;
                BroadcastProperty(player, "General1Showed");

                List<string> arg = new List<string>
                {
                    GameEventType.S_GAME_EVENT_CHANGE_HERO.ToString(),
                    player.Name,
                    general_name.ToString(),
                    false.ToString(),
                    false.ToString()
                };
                DoBroadcastNotify(CommandType.S_COMMAND_LOG_EVENT, arg);
                ChangePlayerGeneral(player, general_name);


                if (!player.DuanChang.Contains("head"))
                {
                    SendPlayerSkillsToOthers(player, true);
                    foreach (string skill_name in player.GetHeadSkillList())
                    {
                        Skill skill = Engine.GetSkill(skill_name);
                        if (skill != null && skill.SkillFrequency == Frequency.Limited
                            && !string.IsNullOrEmpty(skill.LimitMark) && (!skill.LordSkill || Engine.GetGeneral(player.General1, Setting.GameMode).IsLord()))
                        {
                            List<string> args = new List<string>
                            {
                                player.Name,
                                skill.LimitMark,
                                player.GetMark(skill.LimitMark).ToString()
                            };
                            DoBroadcastNotify(CommandType.S_COMMAND_SET_MARK, args);
                        }
                    }
                }
                List<Client> clients = new List<Client>(Clients);
                if (player.HeadSkinId > 0)
                {
                    foreach (Client p in clients)
                        if (p != GetClient(player))
                            NotifyProperty(p, player, "HeadSkinId");
                }

                if (!player.General2Showed)
                {
                    string kingdom = player.Kingdom;
                    BroadcastProperty(player, "Kingdom");
                    player.Role = Hegemony.WillbeRole(this, player);
                    BroadcastProperty(player, "Role");
                }

                if (Engine.GetGeneral(player.General1, Setting.GameMode).IsLord())
                {
                    string kingdom = player.Kingdom;
                    foreach (Player p in m_players)
                    {
                        if (p.Kingdom == kingdom && p.GetRoleEnum() == Player.PlayerRole.Careerist)
                        {
                            p.Role = Engine.GetMappedRole(kingdom);
                            BroadcastProperty(p, "Role");
                            BroadcastProperty(p, "Kingdom");
                        }
                    }
                }
            }
            else
            {
                string general_name = player.ActualGeneral2;
                if (string.IsNullOrEmpty(general_name)) return;

                if (!ignore_rule && !player.CanShowGeneral("d")) return;
                if (player.General2 != "anjiang") return;
                player.SetSkillsPreshowed("d");
                NotifyPlayerPreshow(player, "d");
                player.General2Showed = true;
                BroadcastProperty(player, "General2Showed");

                List<string> arg = new List<string>
                {
                    GameEventType.S_GAME_EVENT_CHANGE_HERO.ToString(),
                    player.Name,
                    general_name.ToString(),
                    true.ToString(),
                    false.ToString()
                };
                DoBroadcastNotify(CommandType.S_COMMAND_LOG_EVENT, arg);
                ChangePlayerGeneral2(player, general_name);

                if (!player.DuanChang.Contains("deputy"))
                {
                    SendPlayerSkillsToOthers(player, false);
                    foreach (string skill_name in player.GetDeputySkillList())
                    {
                        Skill skill = Engine.GetSkill(skill_name);
                        if (skill != null && skill.SkillFrequency == Skill.Frequency.Limited
                            && !string.IsNullOrEmpty(skill.LimitMark) && (!skill.LordSkill || Engine.GetGeneral(player.General1, Setting.GameMode).IsLord()))
                        {
                            List<string> args = new List<string>
                            {
                                player.Name,
                                skill.LimitMark,
                                player.GetMark(skill.LimitMark).ToString()
                            };
                            DoBroadcastNotify(CommandType.S_COMMAND_SET_MARK, args);
                        }
                    }
                }
                List<Client> clients = new List<Client>(Clients);
                if (player.DeputySkinId > 0)
                {
                    foreach (Client p in clients)
                        if (p != GetClient(player))
                            NotifyProperty(p, player, "DeputySkinId");
                }

                if (!player.General1Showed)
                {
                    string kingdom = player.Kingdom;
                    BroadcastProperty(player, "Kingdom");
                    player.Role = Hegemony.WillbeRole(this, player);
                    BroadcastProperty(player, "Role");
                }
            }

            if (sendLog)
            {
                LogMessage log = new LogMessage("#BasaraReveal")
                {
                    From = player.Name,
                    Arg = player.General1,
                    Arg2 = player.General2
                };
                SendLog(log);
            }

            if (trigger_event)
            {
                object _head = head_general;
                RoomThread.Trigger(TriggerEvent.GeneralShown, this, player, ref _head);
            }

            FilterCards(player, player.GetCards("he"), true);
        }

        public void SendLog(LogMessage log, List<Player> except = null)
        {
            if (string.IsNullOrEmpty(log.Type))
                return;

            List<Client> except_client = new List<Client>();
            except = except ?? new List<Player>();
            foreach (Player p in except)
            {
                Client client = GetClient(p);
                if (client != null)
                    except_client.Add(client);
            }

            //QVariant arg = log.toVariant(this);
            List<string> message = new List<string> { JsonUntity.Object2Json(log) };
            List<Client> clients = new List<Client>(Clients);
            foreach (Client p in clients)
                if (!except_client.Contains(p))
                    DoNotify(p, CommandType.S_COMMAND_LOG_SKILL, message);
        }

        public void SendLog(LogMessage log, Player player)
        {
            if (string.IsNullOrEmpty(log.Type))
                return;

            DoNotify(GetClient(player), CommandType.S_COMMAND_LOG_SKILL, new List<string> { JsonUntity.Object2Json(log) });
        }

        public void SendCompulsoryTriggerLog(Player player, string skill_name, bool notify_skill = true)
        {
            if (!player.ContainsTag("JustShownSkill") || (string)player.GetTag("JustShownSkill") != skill_name)
            {
                LogMessage log = new LogMessage
                {
                    Type = "#TriggerSkill",
                    Arg = skill_name,
                    From = player.Name
                };
                SendLog(log);
            }
            if (notify_skill)
                NotifySkillInvoked(player, skill_name);
        }

        public void SendPlayerSkillsToOthers(Player player, bool head_skill = true)
        {
            List<string> skills = head_skill ? player.GetHeadSkillList() : player.GetDeputySkillList();
            List<Client> clients = new List<Client>(Clients);
            foreach (string skill in skills)
            {
                List<string> args = new List<string> { GameEventType.S_GAME_EVENT_ADD_SKILL.ToString(), player.Name, skill, head_skill.ToString() };
                foreach (Client p in clients)
                    if (p != GetClient(player))
                        DoNotify(p, CommandType.S_COMMAND_LOG_EVENT, args);

            }
        }

        public bool ShowSkill(Player player, string skill_name, string skill_position)
        {
            if (string.IsNullOrEmpty(skill_name)) return false;
            bool result = false;
            int type = 0;
            Skill skill = Engine.GetSkill(skill_name);
            if (skill == null)
            {
                return false;
            }

            List<Skill> heads = RoomLogic.GetHeadActivedSkills(this, player, false);
            List<Skill> deputys = RoomLogic.GetDeputyActivedSkills(this, player, false);

            if (heads.Contains(skill) || deputys.Contains(skill))
            {
                bool head = heads.Contains(skill);
                if (!string.IsNullOrEmpty(skill_position))
                    head = skill_position == "head" ? true : false;
                if (head && !player.General1Showed)
                {
                    ShowGeneral(player, true);
                    result = true;
                }
                if (!head && !player.General2Showed)
                {
                    ShowGeneral(player, false);
                    result = true;
                }
            }
            else if (!RoomLogic.PlayerHasShownSkill(this, player, skill))
            {
                List<ViewHasSkill> vhskills = Engine.ViewHas(this, player, skill.Name, "skill");
                foreach (ViewHasSkill vhskill in vhskills)
                {
                    if (!vhskill.Global)
                    {
                        type = 1;
                        break;
                    }
                }
            }

            //当某玩家首次亮出技能时bot聊天
            if (result)
            {
                Bot.OnSkillShow(this, player, skill_name);
            }

            if (type == 1)
            {
                List<TriggerStruct> q = new List<TriggerStruct>();
                if (player.CanShowGeneral("h"))
                    q.Add(new TriggerStruct("GameRule_AskForGeneralShowHead", player));
                if (player.CanShowGeneral("d"))
                    q.Add(new TriggerStruct("GameRule_AskForGeneralShowDeputy", player));

                TriggerStruct name = AskForSkillTrigger(player, "GameRule:ShowGeneral", q, false, null, false);
                ShowGeneral(player, name.SkillName == "GameRule_AskForGeneralShowHead" ? true : false, true, true, false);
                result = true;
            }
            return result;
        }

        public Player GetLastAlive(Player player, int n = 1, bool ignoreRemoved = true)
        {
            return GetNextAlive(player, AliveCount(!ignoreRemoved) - n, ignoreRemoved);
        }
        public int AliveCount(bool includeRemoved = true)
        {
            int n = GetAlivePlayers().Count;
            if (!includeRemoved)
            {
                foreach (Player p in GetAlivePlayers())
                {
                    if (p.Removed)
                        n--;
                }
            }

            return n;
        }

        public Player GetNextAlive(Player from, int n = 1, bool ignoreRemoved = true)
        {
            Player next = from;
            for (int i = 0; i < n; i++)
            {
                if (next == null)
                    return from;

                do next = GetNext(next, ignoreRemoved);
                while (next != null && !next.Alive && next != from);
            }

            return next;
        }

        public Player GetNext(Player from, bool ignoreRemoved = true)
        {
            Player next_p = FindPlayer(from.Next, true);
            if (ignoreRemoved && next_p.Removed)
                return GetNext(next_p, ignoreRemoved);

            return next_p;
        }

        public void DoSuperLightbox(Player player, string head, string skillName)
        {
            GeneralSkin gsk = RoomLogic.GetGeneralSkin(this, player, skillName, head);
            DoAnimate(AnimateType.S_ANIMATE_LIGHTBOX, JsonUntity.Object2Json(gsk), skillName);
            Thread.Sleep(1500);
        }
        public void DoAnimate(AnimateType type, string arg1 = null, string arg2 = null, List<Player> players = null)
        {
            List<Client> receivers = new List<Client>();
            if (players == null || players.Count == 0)
                receivers = Clients;
            else
                foreach (Player p in players)
                    if (GetClient(p) != null && !receivers.Contains(GetClient(p)))
                        receivers.Add(GetClient(p));

            List<string> arg = new List<string>
            {
                type.ToString(),
                arg1,
                arg2
            };
            DoBroadcastNotify(receivers, CommandType.S_COMMAND_ANIMATE, arg);
        }

        public void DoBattleArrayAnimate(Player player, Player target = null)
        {
            if (AliveCount() < 4) return;
            if (target == null)
            {
                List<string> names = new List<string>();
                List<Player> players = RoomLogic.GetFormation(this, player);
                SortByActionOrder(ref players);
                foreach (Player p in players)
                    names.Add(p.Name);
                if (names.Count > 1)
                    DoAnimate(AnimateType.S_ANIMATE_BATTLEARRAY, JsonUntity.Object2Json(names));
            }
            else
            {
                foreach (Player p in GetOtherPlayers(player))
                    if (RoomLogic.InSiegeRelation(this, p, player, target))
                        DoAnimate(AnimateType.S_ANIMATE_BATTLEARRAY, JsonUntity.Object2Json(new List<string> { player.Name, p.Name }));
            }
        }

        public List<Player> GetOtherPlayers(Player except, bool include_dead = false)
        {
            List<Player> other_players = GetAllPlayers(include_dead);
            if (except != null && (except.Alive || include_dead))
                other_players.Remove(except);

            return other_players;
        }
        public Player GetFront(Player a, Player b)
        {
            List<Player> players = GetAllPlayers();
            int index_a = players.IndexOf(a), index_b = players.IndexOf(b);
            if (index_a < index_b)
                return a;
            else
                return b;
        }
        public List<Player> GetAllPlayers(bool include_dead = false)
        {
            if (Current == null)
                return include_dead ? new List<Player>(m_players) : GetAlivePlayers();

            List<Player> count_players = new List<Player>(m_players);
            Player starter = Current;
            int index = count_players.IndexOf(starter);
            if (index == -1)
                return count_players;

            List<Player> all_players = new List<Player>();
            for (int i = index; i < count_players.Count; i++)
            {
                if (include_dead || count_players[i].Alive)
                    all_players.Add(count_players[i]);
            }

            for (int i = 0; i < index; i++)
            {
                if (include_dead || count_players[i].Alive)
                    all_players.Add(count_players[i]);
            }

            if (Current.Phase == PlayerPhase.NotActive && all_players.Contains(Current))
            {
                all_players.Remove(Current);
                all_players.Add(Current);
            }

            return all_players;
        }


        public void BroadcastSkillInvoke(Player player, WrappedCard card)
        {
            if (card.Mute) return;

            string skill_name = card.Skill;
            Skill skill = Engine.GetSkill(skill_name);
            FunctionCard f_card = Engine.GetFunctionCard(card.Name);
            if (skill == null)
            {
                if (string.IsNullOrEmpty(f_card.GetCommonEffectName()))
                    BroadcastSkillInvoke(card.Name, player.IsMale() ? "male" : "female", -1);
                else
                    BroadcastSkillInvoke(f_card.GetCommonEffectName(), "common", -1);
                return;
            }
            else
            {
                int index = -1;
                string _skill_name = skill_name;
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(this, player, card.Skill, card.SkillPosition);
                string general = gsk.General;
                int skin_id = gsk.SkinId;
                skill.GetEffectIndex(this, player, card, ref index, ref _skill_name, ref general, ref skin_id);
                if (index == 0) return;

                if (index == -2)
                {
                    if (string.IsNullOrEmpty(f_card.GetCommonEffectName()))
                        BroadcastSkillInvoke(card.Name, player.IsMale() ? "male" : "female", -1);
                    else
                        BroadcastSkillInvoke(f_card.GetCommonEffectName(), "common", -1);
                }
                else
                {
                    BroadcastSkillInvoke(skill_name, "male", index, general, skin_id);
                }
            }
        }
        public void BroadcastSkillInvoke(string skill_name, Player who, string position = null)
        {
            if (who != null)
            {
                GeneralSkin general = RoomLogic.GetGeneralSkin(this, who, skill_name, position);
                BroadcastSkillInvoke(skill_name, "male", -1, general.General, general.SkinId);
            }
            else
                BroadcastSkillInvoke(skill_name, "male", -1);
        }
        public void BroadcastSkillInvoke(string skill_name, string category, int type, string general = null, int skin_id = 0)
        {
            List<string> args = new List<string>
            {
                GameEventType.S_GAME_EVENT_PLAY_EFFECT.ToString(),
                skill_name,
                category,
                type.ToString(),
                general,
                skin_id.ToString()
            };

            DoBroadcastNotify(CommandType.S_COMMAND_LOG_EVENT, args);
        }

        public void SummonFriends(Player player, BattleArraySkill.ArrayType type)
        {
            if (GetAlivePlayers().Count < 4) return;
            LogMessage log = new LogMessage
            {
                Type = "#InvokeSkill",
                From = player.Name,
                Arg = "GameRule_AskForArraySummon"
            };
            SendLog(log);

            LogMessage log2 = new LogMessage
            {
                Type = "#SummonType",
                Arg = type == BattleArraySkill.ArrayType.Siege ? "summon_type_siege" : "summon_type_formation"
            };
            SendLog(log2);
            switch (type)
            {
                case BattleArraySkill.ArrayType.Siege:
                    {
                        if (RoomLogic.IsFriendWith(this, player, GetNextAlive(player)) && RoomLogic.IsFriendWith(this, player, GetLastAlive(player))) return;
                        bool failed = true;
                        if (!RoomLogic.IsFriendWith(this, player, GetNextAlive(player)) && GetNextAlive(player).HasShownOneGeneral())
                        {
                            Player target = GetNextAlive(player, 2);
                            if (!target.HasShownOneGeneral())
                            {
                                string prompt = RoomLogic.WillBeFriendWith(this, target, player) ? "SiegeSummon" : "SiegeSummon!";
                                bool success = AskForSkillInvoke(target, prompt);
                                log = new LogMessage
                                {
                                    Type = "#SummonResult",
                                    From = target.Name,
                                    Arg = success ? "summon_success" : "summon_failed"
                                };
                                SendLog(log);
                                if (success)
                                {
                                    AskForGeneralShow(target, "GameRule_AskForArraySummon");
                                    DoAnimate(AnimateType.S_ANIMATE_BATTLEARRAY, player.Name, string.Format("{0}+{1}", player.Name, target.Name));       //player success animation
                                    failed = false;
                                }
                            }
                        }
                        if (!RoomLogic.IsFriendWith(this, player, GetLastAlive(player)) && GetLastAlive(player).HasShownOneGeneral())
                        {
                            Player target = GetLastAlive(player, 2);
                            if (!target.HasShownOneGeneral())
                            {
                                string prompt = RoomLogic.WillBeFriendWith(this, target, player) ? "SiegeSummon" : "SiegeSummon!";
                                bool success = AskForSkillInvoke(target, prompt);
                                log = new LogMessage
                                {
                                    Type = "#SummonResult",
                                    From = target.Name,
                                    Arg = success ? "summon_success" : "summon_failed"
                                };
                                SendLog(log);
                                if (success)
                                {
                                    AskForGeneralShow(target, "GameRule_AskForArraySummon");
                                    DoAnimate(AnimateType.S_ANIMATE_BATTLEARRAY, player.Name, string.Format("{0}+{1}", player.Name, target.Name));       //player success animation
                                    failed = false;
                                }
                            }
                        }
                        if (failed)
                            player.SetFlags("Global_SummonFailed");
                        break;
                    }
                case BattleArraySkill.ArrayType.Formation:
                    {
                        int n = AliveCount(false);
                        int asked = n;
                        bool failed = true;
                        for (int i = 1; i < n; ++i)
                        {
                            Player target = GetNextAlive(player, i);
                            if (RoomLogic.IsFriendWith(this, player, target))
                                continue;
                            else if (!target.HasShownOneGeneral())
                            {
                                string prompt = RoomLogic.WillBeFriendWith(this, target, player) ? "FormationSummon" : "FormationSummon!";
                                bool success = AskForSkillInvoke(target, prompt);
                                log = new LogMessage
                                {
                                    Type = "#SummonResult",
                                    From = target.Name,
                                    Arg = success ? "summon_success" : "summon_failed"
                                };
                                SendLog(log);

                                if (success)
                                {
                                    AskForGeneralShow(target, "GameRule_AskForArraySummon");
                                    DoBattleArrayAnimate(target);       //player success animation
                                    failed = false;
                                }
                                else
                                {
                                    asked = i;
                                    break;
                                }
                            }
                            else
                            {
                                asked = i;
                                break;
                            }
                        }

                        n -= asked;
                        for (int i = 1; i < n; ++i)
                        {
                            Player target = GetLastAlive(player, i);
                            if (RoomLogic.IsFriendWith(this, player, target))
                                continue;
                            else
                            {
                                if (!target.HasShownOneGeneral())
                                {
                                    string prompt = RoomLogic.WillBeFriendWith(this, target, player) ? "FormationSummon" : "FormationSummon!";
                                    bool success = AskForSkillInvoke(target, prompt);
                                    log = new LogMessage
                                    {
                                        Type = "#SummonResult",
                                        From = target.Name,
                                        Arg = success ? "summon_success" : "summon_failed"
                                    };
                                    SendLog(log);

                                    if (success)
                                    {
                                        AskForGeneralShow(target, "GameRule_AskForArraySummon");
                                        DoBattleArrayAnimate(target);       //player success animation
                                        failed = false;
                                    }
                                }
                                break;
                            }
                        }
                        if (failed)
                            player.SetFlags("Global_SummonFailed");
                        break;
                    }
            }
        }

        public void Judge(ref JudgeStruct judge_star)
        {
            object data = judge_star;

            SetTag("judge", (ContainsTag("judge") ? (int)GetTag("judge") : 0) + 1);
            m_judge_list.Add(judge_star);
            SetTag("judge_draw", (ContainsTag("judge_draw") ? (int)GetTag("judge_draw") : 0) + 1);

            RoomThread.Trigger(TriggerEvent.StartJudge, this, judge_star.Who, ref data);

            SetTag("judge_draw", (int)GetTag("judge_draw") - 1);

            List<Player> players = GetAllPlayers();
            foreach (Player player in players)
            {
                if (RoomThread.Trigger(TriggerEvent.AskForRetrial, this, player, ref data))
                    break;
            }
            RoomThread.Trigger(TriggerEvent.FinishRetrial, this, judge_star.Who, ref data);
            RoomThread.Trigger(TriggerEvent.JudgeResult, this, judge_star.Who, ref data);

            JudgeStruct judge = (JudgeStruct)data;
            judge.JudgeNumber = judge.Card.Number;
            judge.JudgeSuit = judge.Card.Suit;
            data = judge;

            SetTag("judge", (int)GetTag("judge") - 1);
            m_judge_list.RemoveAt(m_judge_list.Count - 1);

            RoomThread.Trigger(TriggerEvent.FinishJudge, this, judge_star.Who, ref data);
            judge_star = (JudgeStruct)data;

            Thread.Sleep(300);
        }

        public bool AskForGeneralShow(Player player, string reason, bool one = true, bool refusable = false)
        {
            if (player.HasShownAllGenerals())
                return false;

            List<string> choices = new List<string>();

            if (!player.General1Showed && player.DisableShowList(true).Count == 0)
                choices.Add("show_head_general");
            if (!player.General2Showed && player.DisableShowList(false).Count == 0)
                choices.Add("show_deputy_general");
            if (choices.Count == 0)
                return false;
            if (!one && choices.Count == 2)
                choices.Add("show_both_generals");
            if (refusable)
                choices.Add("cancel");

            string choice = AskForChoice(player, reason, string.Join("+", choices));

            if (choice == "show_head_general" || choice == "show_both_generals")
                ShowGeneral(player);
            if (choice == "show_deputy_general" || choice == "show_both_generals")
                ShowGeneral(player, false);

            return choice.StartsWith("s");
        }

        public bool AskForSkillInvoke(Player player, string skill_name, object data = null, string position = null)
        {
            NotifyMoveFocus(player, CommandType.S_COMMAND_INVOKE_SKILL);

            bool invoked = false;
            TrustedAI ai = GetAI(player);
            if (ai != null)
            {
                invoked = ai.AskForSkillInvoke(skill_name, data);
                if (skill_name.EndsWith("!"))
                    invoked = false;
                Skill skill = Engine.GetSkill(skill_name);
                if (invoked && !(skill != null && skill.SkillFrequency != Frequency.NotFrequent))
                    Thread.Sleep(400);
            }
            else
            {
                List<string> skillCommand = new List<string> { player.Name };
                if (data is string)
                {
                    skillCommand.Add(skill_name);
                    skillCommand.Add((string)data);
                }
                else
                {
                    string data_str = null;
                    if (data is Player && (Player)data != null)
                    {
                        data_str = "playerdata:" + ((Player)data).Name;
                    }
                    skillCommand.Add(skill_name);
                    skillCommand.Add(data_str);
                }
                if (player != null && !string.IsNullOrEmpty(position))
                    skillCommand.Add(position);

                if (!DoRequest(player, CommandType.S_COMMAND_INVOKE_SKILL, skillCommand, true) || skill_name.EndsWith("!"))
                    invoked = false;
                else
                {
                    Interactivity client = GetInteractivity(player);
                    List<string> clientReply = client?.ClientReply;
                    if (clientReply != null && clientReply.Count > 0)
                    {
                    invoked = bool.TryParse(clientReply[0], out bool success);
                    if (!success)
                        invoked = false;
                    }
                }
            }
            DoBroadcastNotify(CommandType.S_COMMAND_UNKNOWN, new List<string> { false.ToString() });

            if (invoked)
            {
                Skill skill = Engine.GetSkill(skill_name);
                if (skill != null && skill is TriggerSkill)
                {
                    TriggerSkill tr_skill = (TriggerSkill)skill;
                    if (tr_skill != null && !tr_skill.Global)
                    {
                        List<string> msg = new List<string> { skill_name, player.Name };
                        if (RoomLogic.PlayerHasSkill(this, player, skill_name) || RoomLogic.PlayerHasEquipSkill(player, skill_name))
                            DoBroadcastNotify(CommandType.S_COMMAND_INVOKE_SKILL, msg);
                        NotifySkillInvoked(player, skill_name);
                    }
                }
            }

            object decisionData = "skillInvoke:" + skill_name + ":" + (invoked ? "yes" : "no");
            RoomThread.Trigger(TriggerEvent.ChoiceMade, this, player, ref decisionData);
            return invoked;
        }

        public string AskForChoice(Player player, string skill_name, string choices, List<string> descriptions = null, object data = null)
        {
            List<string> validChoices = new List<string>();
            foreach (string choice in choices.Split('|'))
                validChoices.AddRange(choice.Split('+'));
            string skillname = skill_name;

            string answer = string.Empty;
            if (validChoices.Count == 1)
            {
                answer = validChoices[0];
            }
            else
            {
                NotifyMoveFocus(player, CommandType.S_COMMAND_MULTIPLE_CHOICE);

                TrustedAI ai = GetAI(player);
                if (ai != null)
                {
                    answer = ai.AskForChoice(skillname, choices, data);
                    Thread.Sleep(400);
                }
                else
                {
                    bool success = DoRequest(player, CommandType.S_COMMAND_MULTIPLE_CHOICE, new List<string> { player.Name, skill_name, choices, JsonUntity.Object2Json(descriptions) }, true);
                    Interactivity client = GetInteractivity(player);
                    List<string> clientReply = client?.ClientReply;
                    if (client == null || !success || clientReply == null || clientReply.Count == 0)
                    {
                        answer = ".";
                    }
                    else
                        answer = clientReply[0];
                }
                DoBroadcastNotify(CommandType.S_COMMAND_UNKNOWN, new List<string> { false.ToString() });

            }
            bool check = false;
            foreach (string c in validChoices)
            {
                if (c == answer)
                {
                    check = true;
                    break;
                }
            }
            if (!check)
            {
                if (validChoices.Contains("cancel"))
                    answer = "cancel";
                else
                    answer = validChoices[0];
            }

            if (RoomThread != null)
            {
                object decisionData = "skillChoice:" + skillname + ":" + answer;
                RoomThread.Trigger(TriggerEvent.ChoiceMade, this, player, ref decisionData);
            }

            return answer;
        }
        public void AskForLordConvert()
        {
            List<Player> lords = new List<Player>();
            foreach (Player p in m_players)
            {
                if (!string.IsNullOrEmpty(p.ActualGeneral1))
                {
                    string lord = "lord_" + p.ActualGeneral1;
                    bool check = true;
                    foreach (Player p2 in GetOtherPlayers(p))
                    {                                 //no duplicate lord
                        if (p != p2 && lord == "lord_" + p2.ActualGeneral1)
                        {
                            check = false;
                            break;
                        }
                    }
                    General lord_general = Engine.GetGeneral(lord, Setting.GameMode);
                    if (check && lord_general != null)
                        lords.Add(p);
                }
            }

            if (lords.Count > 0)
            {
                List<Interactivity> receivers = new List<Interactivity>();
                foreach (Player player in lords)
                {
                    List<string> args = new List<string> { player.Name, "userdefine:changetolord", null };
                    TrustedAI ai = GetAI(player);
                    Interactivity client = GetInteractivity(player);
                    if (!receivers.Contains(client) && ai == null)
                    {
                        client.CommandArgs = args;
                        receivers.Add(client);
                    }
                    else if (ai != null)
                    {
                        if (ai.AskForSkillInvoke("userdefine:changetolord", null))
                        {
                            player.ActualGeneral1 = "lord_" + player.ActualGeneral1;
                            NotifyProperty(GetClient(player), player, "ActualGeneral1");
                        }
                    }
                }

                bool solo = true;
                int human = 0;
                List<Client> clients = new List<Client>(Clients);
                foreach (Client client in clients)
                {
                    if (GetPlayers(client.UserID)[0].Status != "bot")
                        human++;
                }
                if (human > 1)
                {
                    solo = false;
                    Countdown countdown = new Countdown
                    {
                        Max = 6000,
                        Type = Countdown.CountdownType.S_COUNTDOWN_USE_ALL
                    };

                    List<string> players = new List<string>();
                    foreach (Player p in m_players)
                        players.Add(p.Name);

                    List<string> arg = new List<string> { JsonUntity.Object2Json(players), JsonUntity.Object2Json(countdown) };
                    DoBroadcastNotify(CommandType.S_COMMAND_MOVE_FOCUS, arg);
                }

                if (solo)
                    DoBroadcastRequest(receivers, CommandType.S_COMMAND_INVOKE_SKILL);
                else
                    DoBroadcastRequest(receivers, CommandType.S_COMMAND_INVOKE_SKILL, 5000, true);

                DoBroadcastNotify(CommandType.S_COMMAND_UNKNOWN, new List<string> { false.ToString() });

                foreach (Player player in lords)
                {
                    Interactivity client = GetInteractivity(player);
                    if (client != null && client.IsClientResponseReady && receivers.Contains(client))
                    {
                        List<string> invoke = client.ClientReply;
                        if (invoke != null && invoke.Count > 0 && bool.TryParse(invoke[0], out bool success) && success)
                        {
                            player.HeadSkinId = 0;
                            NotifyProperty(GetClient(player), player, "HeadSkinId");
                            player.ActualGeneral1 = "lord_" + player.ActualGeneral1;
                            NotifyProperty(GetClient(player), player, "ActualGeneral1");
                        }
                    }
                }
            }
        }

        public void KingdomSummons(string kingdom)
        {
            List<Player> no_shown = new List<Player>();
            foreach (Player p in _alivePlayers)
                if (!p.HasShownOneGeneral())
                    no_shown.Add(p);

            if (no_shown.Count > 0)
            {
                Dictionary<Player, string> answers = new Dictionary<Player, string>();
                Dictionary<Player, List<TriggerStruct>> choices = new Dictionary<Player, List<TriggerStruct>>();
                List<Interactivity> receivers = new List<Interactivity>();
                foreach (Player player in no_shown)
                {
                    List<TriggerStruct> skills = new List<TriggerStruct>();
                    if (Hegemony.WillbeRole(this, player) != "careerist" && player.Kingdom == kingdom)
                    {
                        if (player.CanShowGeneral("h"))
                            skills.Add(new TriggerStruct("GameRule_AskForGeneralShowHead"));

                        int i = 1;
                        foreach (Player p in Players)
                        {
                            if (p == player) continue;
                            if (p.HasShownOneGeneral() && p.GetRoleEnum() != PlayerRole.Careerist && p.Kingdom == kingdom)
                                ++i;
                        }

                        if (i <= (Players.Count + 1) / 2 && player.CanShowGeneral("d"))
                            skills.Add(new TriggerStruct("GameRule_AskForGeneralShowDeputy"));
                    }

                    choices[player] = skills;

                    TrustedAI ai = GetAI(player);
                    if (ai != null)
                    {
                        if (skills.Count > 0)
                        {
                            List<string> c = new List<string> { "cancel" };
                            foreach (TriggerStruct trigger in skills)
                                c.Add(trigger.SkillName);

                            answers[player] = ai.AskForChoice("summon", string.Join("+", c), null);
                        }
                        else
                            answers[player] = "cancel";
                    }
                    else
                    {
                        List<string> args = new List<string> {
                            player.Name,
                            "summon",
                            "true",
                            JsonUntity.Object2Json(skills)
                        };

                        Interactivity client = GetInteractivity(player);
                        if (client != null && !receivers.Contains(client))
                        {
                            client.CommandArgs = args;
                            receivers.Add(client);
                        }
                    }
                }

                Countdown countdown = new Countdown
                {
                    Max = Setting.GetCommandTimeout(CommandType.S_COMMAND_TRIGGER_ORDER, ProcessInstanceType.S_CLIENT_INSTANCE),
                    Type = Countdown.CountdownType.S_COUNTDOWN_USE_SPECIFIED
                };
                NotifyMoveFocus(no_shown, countdown);

                DoBroadcastRequest(receivers, CommandType.S_COMMAND_TRIGGER_ORDER);

                DoBroadcastNotify(CommandType.S_COMMAND_UNKNOWN, new List<string> { false.ToString() });

                SortByActionOrder(ref no_shown);
                foreach (Player player in no_shown)
                {
                    string answer = "cancel";
                    Interactivity client = GetInteractivity(player);
                    if (receivers.Contains(client))
                    {
                        bool success = true;
                        List<string> reply = GetInteractivity(player).ClientReply;
                        if (!GetInteractivity(player).IsClientResponseReady || reply == null || reply.Count == 0 || string.IsNullOrEmpty(reply[0]))
                            success = false;

                        if (success && reply[0] != "cancel")
                        {
                            try
                            {
                                TriggerStruct trigger = JsonUntity.Json2Object<TriggerStruct>(reply[0]);
                                if (!choices.ContainsKey(player) || !choices[player].Contains(trigger))
                                    success = false;
                                else
                                    answer = trigger.SkillName;
                            }
                            catch(Exception e)
                            {
                                LogHelper.WriteLog(null, e);
                                Debug("Parse Kingdom Summon Reply error");
                                success = false;
                            }
                        }
                        answers[player] = answer;
                    }
                }

                foreach (Player player in no_shown)
                {
                    string answer = answers[player];
                    if (answer != "cancel" && Hegemony.WillbeRole(this, player) != "careerist" && choices[player].Count > 0)
                    {
                        if (answer.Contains("Head"))
                        {
                            string general = player.ActualGeneral1;
                            List<string> skills = Engine.GetGeneralSkills(general, Setting.GameMode, true), _skills = new List<string>(skills);
                            foreach (string skill in _skills)
                            {
                                if (skill.Contains("masu") || skill.Contains("qicai"))
                                    skills.Remove(skill);
                            }
                            if (skills.Count > 0)
                            {
                                Shuffle.shuffle(ref skills);
                                BroadcastSkillInvoke(skills[0], "male", -1, general, player.HeadSkinId);
                            }

                            ShowGeneral(player, true);
                        }
                        else
                        {
                            int i = 1;
                            foreach (Player p in Players)
                            {
                                if (p == player) continue;
                                if (p.HasShownOneGeneral() && p.GetRoleEnum() != PlayerRole.Careerist && p.Kingdom == kingdom)
                                    ++i;
                            }

                            if (i <= (Players.Count + 1) / 2)
                            {
                                string general = player.ActualGeneral2;
                                List<string> skills = Engine.GetGeneralSkills(general, Setting.GameMode, false), _skills = new List<string>(skills);
                                foreach (string skill in _skills)
                                {
                                    if (skill.Contains("masu") || skill.Contains("qicai"))
                                        skills.Remove(skill);
                                }
                                if (skills.Count > 0)
                                {
                                    Shuffle.shuffle(ref skills);
                                    BroadcastSkillInvoke(skills[0], "male", -1, general, player.DeputySkinId);
                                }

                                ShowGeneral(player, false);
                            }
                        }
                    }
                }
            }
        }

        private Dictionary<Interactivity, int> player_luckcard = new Dictionary<Interactivity, int>();
        public void AskForLuckCard(int count)
        {
            if (count <= 0) return;
            player_luckcard.Clear();
            List<Interactivity> players = new List<Interactivity>();
            List<Client> clients = new List<Client>(Clients);
            foreach (Client player in clients)
            {
                Interactivity interactivity = GetInteractivity(player.UserID);
                if (interactivity != null && GetAI(GetPlayers(player.UserID)[0]) == null)
                {
                    interactivity.CommandArgs = new List<string>();
                    players.Add(interactivity);
                    player_luckcard[interactivity] = count;
                }
            }

            if (players.Count == 0)
                return;

            Countdown countdown = new Countdown
            {
                Max = Setting.GetCommandTimeout(CommandType.S_COMMAND_LUCK_CARD, ProcessInstanceType.S_CLIENT_INSTANCE),
                Type = Countdown.CountdownType.S_COUNTDOWN_USE_SPECIFIED
            };
            NotifyMoveFocus(m_players, countdown);
            DoBroadcastRequest(players, CommandType.S_COMMAND_LUCK_CARD);
            DoBroadcastNotify(CommandType.S_COMMAND_UNKNOWN, new List<string> { false.ToString() });
        }

        public void FilterCards(Player player, List<int> ids, bool refilter)
        {
            if (refilter)
            {
                for (int i = 0; i < ids.Count; i++)
                {
                    int cardId = ids[i];
                    RoomCard card = GetCard(cardId);
                    if (card.Modified || card.GetUsedCard().Modified)
                    {
                        ResetCard(cardId);
                    }
                }
            }

            List<bool> cardChanged = new List<bool>();
            for (int i = 0; i < ids.Count; i++)
                cardChanged.Add(false);

            List<string> skills = player.GetSkills(false, false);
            List<Skill> _skills = new List<Skill>();
            List<FilterSkill> filterSkills = new List<FilterSkill>();
            foreach (string skill_name in skills)
            {
                Skill s = Engine.GetSkill(skill_name);
                if (s != null && s is ViewHasSkill skill)
                {
                    foreach (string name1 in skill.Skills)
                    {
                        Skill mskill = Engine.GetSkill(name1);
                        if (mskill == null) continue;
                        if ((mskill is FilterSkill) || (mskill is TriggerSkill) && !_skills.Contains(mskill)
                                && skill.ViewHas(this, player, name1))
                            _skills.Add(mskill);
                    }
                }
                else if (s != null)
                    _skills.Add(s);
            }

            foreach (Skill skill in _skills)
            {
                if (RoomLogic.PlayerHasShownSkill(this, player, skill))
                {
                    if (skill is FilterSkill filter)
                    {
                        filterSkills.Add(filter);
                    }
                    else if (skill is TriggerSkill trigger)
                    {
                        ViewAsSkill vsskill = trigger.ViewAsSkill;
                        if (vsskill != null && vsskill is FilterSkill _filter)
                            filterSkills.Add(_filter);
                    }
                }
            }
            if (filterSkills.Count == 0) return;

            for (int i = 0; i < ids.Count; i++)
            {
                RoomCard card = GetCard(ids[i]);
                foreach (FilterSkill skill in filterSkills)
                {
                    if (skill.ViewFilter(this, card, player))
                    {
                        skill.ViewAs(this, ref card, player);
                        card.Modified = true;
                        cardChanged[i] = true;
                        break;
                    }
                }
            }

            for (int i = 0; i < ids.Count; i++)
            {
                int cardId = ids[i];
                Place place = GetCardPlace(cardId);
                if (!cardChanged[i]) continue;
                if (place == Place.PlaceJudge)
                {
                    LogMessage log = new LogMessage
                    {
                        Type = "#FilterJudge",
                        Arg = GetCard(cardId).Skill,
                        From = player.Name
                    };

                    SendLog(log);
                    BroadcastSkillInvoke(GetCard(cardId).Skill, player);
                }
            }
        }

        public void EnterDying(Player player, DamageStruct reason)
        {
            player.SetFlags("Global_Dying");
            m_dyings.Add(player);

            List<string> arg = new List<string> { GameEventType.S_GAME_EVENT_PLAYER_DYING.ToString(), player.Name };
            DoBroadcastNotify(CommandType.S_COMMAND_LOG_EVENT, arg);

            DyingStruct dying = new DyingStruct
            {
                Who = player,
                Damage = reason
            };
            object dying_data = dying;
            RoomThread.Trigger(TriggerEvent.Dying, this, player, ref dying_data);

            if (player.Alive)
            {
                if (player.Hp > 0)
                {
                    player.SetFlags("-Global_Dying");
                }
                else
                {
                    LogMessage log = new LogMessage
                    {
                        Type = "#AskForPeaches",
                        From = player.Name,
                        To = new List<string>(),
                        Arg = (1 - player.Hp).ToString()
                    };
                    log.SetTos(GetAlivePlayers());
                    SendLog(log);

                    foreach (Player saver in GetAllPlayers())
                    {
                        if (player.Hp > 0 || !player.Alive)
                            break;

                        RoomThread.Trigger(TriggerEvent.AskForPeaches, this, saver, ref dying_data);
                    }
                    RoomThread.Trigger(TriggerEvent.AskForPeachesDone, this, player, ref dying_data);

                    player.SetFlags("-Global_Dying");
                }
            }
            m_dyings.Remove(player);

            if (player.Alive)
            {
                arg = new List<string> { GameEventType.S_GAME_EVENT_PLAYER_QUITDYING.ToString(), player.Name };
                DoBroadcastNotify(CommandType.S_COMMAND_LOG_EVENT, arg);
            }
            RoomThread.Trigger(TriggerEvent.QuitDying, this, player, ref dying_data);
        }

        public void SetPlayerChained(Player player, bool chained, bool trigger = true)
        {
            if (player.Chained == chained) return;

            object data = chained;
            if (!RoomThread.Trigger(TriggerEvent.ChainStateCanceling, this, player, ref data))
            {
                player.Chained = chained;
                BroadcastProperty(player, "Chained");
                if (trigger && player.Alive)
                {
                    DamageStruct damage = new DamageStruct();
                    object _data = damage;
                    RoomThread.Trigger(TriggerEvent.ChainStateChanged, this, player, ref _data);
                }
            }
        }

        public void NotifyChainedAnimation(Player player)
        {
            List<string> arg = new List<string> { GameEventType.S_GAME_EVENT_CHAIN_ANIMATION.ToString() };
            DoNotify(GetClient(player), CommandType.S_COMMAND_LOG_EVENT, arg);
        }

        public void DoChainedAnimation(Player player = null, DamageStruct.DamageNature nature = DamageStruct.DamageNature.Normal)
        {
            if (player.Chained)
            {
                List<string> arg = new List<string> { GameEventType.S_GAME_EVENT_CHAIN_ANIMATION.ToString(), player.Name, ((int)nature).ToString() };
                DoBroadcastNotify(CommandType.S_COMMAND_LOG_EVENT, arg);
            }
        }
        public void ChainedRemoveOnDamageDone(Player player, DamageStruct damage)
        {
            if (!player.Chained) return;
            player.Chained = false;
            List<string> arg = new List<string> { player.Name };
            DoBroadcastNotify(CommandType.S_COMMAND_CHAIN_REMOVE, arg);                                 //由伤害造成的横置状态解除不会触发ChainStateCanceling时机
                                                                                                        //也不能取消
            if (player.Alive)
            {
                object data = damage;
                RoomThread.Trigger(TriggerEvent.ChainStateChanged, this, player, ref data);
            }
        }

        public void SetEmotion(Player target, string emotion)
        {
            List<string> arg = new List<string> { target.Name, string.IsNullOrEmpty(emotion) ? "." : emotion };
            DoBroadcastNotify(CommandType.S_COMMAND_SET_EMOTION, arg);
        }
        public void UpdateStateItem()
        {
            if (Setting.GameMode == "Classic")
            {
                List<Player> players = new List<Player>(m_players);
                players.Sort((x, y) => CompareByRole(x, y));
                List<string> roles = new List<string>();
                foreach (Player p in players)
                {
                    string c = "ZCFN"[(int)p.GetRoleEnum()].ToString();
                    if (!p.Alive)
                        c = c.ToLower();

                    roles.Add(c);
                }
                DoBroadcastNotify(CommandType.S_COMMAND_UPDATE_STATE_ITEM, roles);
            }
        }
        public static int CompareByRole(Player player1, Player player2)
        {
            int role1 = (int)player1.GetRoleEnum();
            int role2 = (int)player2.GetRoleEnum();

            if (role1 != role2)
                return role1 < role2 ? -1 : 1;
            else
                return player1.Alive ? -1 : 1;
        }
        public void KillPlayer(Player victim, DamageStruct reason)
        {
            Player killer = reason.From;

            victim.Alive = false;
            SetPlayerChained(victim, false, false);
            victim.Removed = false;
            BroadcastProperty(victim, "Removed");

            int index = GetAlivePlayers().IndexOf(victim);
            for (int i = index + 1; i < GetAlivePlayers().Count; i++)
            {
                Player p = GetAlivePlayers()[i];
                p.Seat = p.Seat - 1;
            }
            _alivePlayers.Remove(victim);

            if (reason.From != null)
            {
                ResultStruct new_result = reason.From.Result;
                new_result.Kill += 1;
                reason.From.Result = new_result;
            }

            DeathStruct death = new DeathStruct
            {
                Who = victim,
                Damage = reason
            };
            object data = death;
            RoomThread.Trigger(TriggerEvent.BeforeGameOverJudge, this, victim, ref data);

            UpdateStateItem();

            LogMessage log = new LogMessage
            {
                Type = killer != null ? (killer == victim ? "#Suicide" : "#Murder") : "#Contingency",
                To = new List<string> { victim.Name },
                Arg = victim.Kingdom,
                From = killer?.Name
            };
            SendLog(log);

            BroadcastProperty(victim, "Alive");
            BroadcastProperty(victim, "Role");

            List<string> array = new List<string> { victim.Name };
            if (killer != null) array.Add(killer.Name);
            DoBroadcastNotify(CommandType.S_COMMAND_KILL_PLAYER, array);

            Thread.Sleep(2500);

            RoomThread.Trigger(TriggerEvent.GameOverJudge, this, victim, ref data);
            RoomThread.Trigger(TriggerEvent.Death, this, victim, ref data);

            victim.DetachAllSkills();
            RoomThread.Trigger(TriggerEvent.BuryVictim, this, victim, ref data);

            /*
            if (!victim.Alive)
            {
                bool expose_roles = true;
                foreach (Player player in GetAlivePlayers())
                {
                    if (GetClient(player).Status != Client.GameStatus.offline)
                    {
                        expose_roles = false;
                        break;
                    }
                }

                if (expose_roles)
                {
                    foreach (Player player in GetAlivePlayers())
                    {
                        string role = player.Kingdom;
                        if (role == "god")
                        {
                            role = Engine.GetGeneral(player.ActualGeneral1).Kingdom;
                            role = Engine.GetMappedRole(role);
                            BroadcastProperty(player, "Role", role);
                        }
                    }
                }
            }
            */
            CheckSurrend();
        }

        public void CheckSurrend()
        {
            List<Interactivity> availables = Scenario.CheckSurrendAvailable(this);
            Surrender.Clear();
            foreach (Interactivity inter in availables)
            {
                Surrender.TryAdd(inter, true);
                Client client = GetClient(inter.ClientId);
                DoNotify(client, CommandType.S_COMMAND_ENABLE_SURRENDER, new List<string> { true.ToString() });
            }
        }

        public void SendDamageLog(DamageStruct data)
        {
            LogMessage log = new LogMessage();

            if (data.From != null)
            {
                log.Type = "#Damage";
                log.From = data.From.Name;
            }
            else
            {
                log.Type = "#DamageNoSource";
            }

            log.To = new List<string> { data.To.Name };
            log.Arg = data.Damage.ToString();

            switch (data.Nature)
            {
                case DamageStruct.DamageNature.Normal: log.Arg2 = "normal_nature"; break;
                case DamageStruct.DamageNature.Fire: log.Arg2 = "fire_nature"; break;
                case DamageStruct.DamageNature.Thunder: log.Arg2 = "thunder_nature"; break;
            }

            SendLog(log);
        }
        public void LoseHp(Player victim, int lose = 1)
        {
            if (lose <= 0)
                return;

            if (!victim.Alive)
                return;
            object data = lose;
            if (RoomThread.Trigger(TriggerEvent.PreHpLost, this, victim, ref data))
                return;

            lose = (int)data;

            if (lose <= 0)
                return;

            LogMessage log = new LogMessage
            {
                Type = "#LoseHp",
                From = victim.Name,
                Arg = lose.ToString()
            };
            SendLog(log);

            object _lose = -lose;
            if (!RoomThread.Trigger(TriggerEvent.HpChanging, this, victim, ref _lose))
            {
                victim.Hp -= lose;
                BroadcastProperty(victim, "Hp");

                List<string> arg = new List<string> { victim.Name, (-lose).ToString(), "-1" };
                DoBroadcastNotify(CommandType.S_COMMAND_CHANGE_HP, arg);

                LogMessage log2 = new LogMessage
                {
                    Type = "#GetHp",
                    From = victim.Name,
                    Arg = victim.Hp.ToString(),
                    Arg2 = victim.MaxHp.ToString()
                };
                SendLog(log2);

                RoomThread.Trigger(TriggerEvent.HpChanged, this, victim, ref _lose);
                RoomThread.Trigger(TriggerEvent.PostHpReduced, this, victim, ref data);
                RoomThread.Trigger(TriggerEvent.HpLost, this, victim, ref data);
            }
        }

        public void LoseMaxHp(Player victim, int lose = 1)
        {
            if (lose <= 0)
                return;

            int hp_1 = victim.Hp;
            victim.MaxHp = Math.Max(victim.MaxHp - lose, 0);
            int hp_2 = victim.Hp;

            BroadcastProperty(victim, "MaxHp");
            BroadcastProperty(victim, "Hp");

            LogMessage log = new LogMessage
            {
                Type = "#LoseMaxHp",
                From = victim.Name,
                Arg = lose.ToString()
            };
            SendLog(log);

            //JsonArray arg;
            //arg << victim->Name;
            //arg << -lose;
            //doBroadcastNotify(S_COMMAND_CHANGE_MAXHP, arg);

            LogMessage log2 = new LogMessage
            {
                Type = "#GetHp",
                From = victim.Name,
                Arg = victim.Hp.ToString(),
                Arg2 = victim.MaxHp.ToString()
            };
            SendLog(log2);

            if (victim.MaxHp == 0)
                KillPlayer(victim, new DamageStruct());
            else
            {
                RoomThread.Trigger(TriggerEvent.MaxHpChanged, this, victim);
                if (hp_1 > hp_2)
                {
                    object data = hp_1 - hp_2;
                    object _data = hp_2 - hp_1;
                    RoomThread.Trigger(TriggerEvent.HpChanged, this, victim, ref _data);
                    RoomThread.Trigger(TriggerEvent.PostHpReduced, this, victim, ref data);
                }
            }
        }

        public bool ApplyDamage(Player victim, DamageStruct damage)
        {
            List<string> arg = new List<string> { victim.Name, (-damage.Damage).ToString(), ((int)damage.Nature).ToString() };
            DoBroadcastNotify(CommandType.S_COMMAND_CHANGE_HP, arg);

            if (damage.Nature != DamageStruct.DamageNature.Normal && victim.Chained)
                ChainedRemoveOnDamageDone(victim, damage);

            object data = -damage.Damage;
            bool result = RoomThread.Trigger(TriggerEvent.HpChanging, this, victim, ref data);
            if (!result)
            {
                int new_hp = victim.Hp - damage.Damage;
                victim.Hp = new_hp;
                BroadcastProperty(victim, "Hp");

                LogMessage log = new LogMessage
                {
                    Type = "#GetHp",
                    From = victim.Name,
                    Arg = victim.Hp.ToString(),
                    Arg2 = victim.MaxHp.ToString()
                };
                SendLog(log);

                ResultStruct new_result = damage.To.Result;
                new_result.Damaged += damage.Damage;
                damage.To.Result = new_result;
                if (damage.From != null && !RoomLogic.WillBeFriendWith(this, damage.From, damage.To))
                {
                    new_result = damage.From.Result;
                    new_result.Damage += damage.Damage;
                    damage.From.Result = new_result;
                }

                RoomThread.Trigger(TriggerEvent.HpChanged, this, victim, ref data);
            }


            return result;
        }

        private void RemoveQinggangTag(DamageStruct damage_data)
        {
            if (damage_data.Card != null && Engine.GetFunctionCard(damage_data.Card.Name) is Slash)
                RemoveQinggangTag(damage_data.To, damage_data.Card);
        }
        public void RemoveQinggangTag(Player player, WrappedCard card)
        {
            if (!player.ContainsTag("Qinggang")) return;

            List<string> qinggang = (List<string>)player.GetTag("Qinggang");
            qinggang.Remove(RoomLogic.CardToString(this, card));
            if (qinggang.Count == 0)
                player.RemoveTag("Qinggang");
            else
                player.SetTag("Qinggang", qinggang);
        }
        public void Damage(DamageStruct data)
        {
            DamageStruct damage_data = data;
            if (damage_data.To == null || !damage_data.To.Alive)
                return;

            object qdata = damage_data;

            if (!damage_data.Chain && !damage_data.Transfer)
            {
                RoomThread.Trigger(TriggerEvent.ConfirmDamage, this, damage_data.From, ref qdata);
                damage_data = (DamageStruct)qdata;
            }

            // Predamage
            if (RoomThread.Trigger(TriggerEvent.Predamage, this, damage_data.From, ref qdata))
            {
                RemoveQinggangTag(damage_data);
                return;
            }

            bool enter_stack = false;
            do
            {
                if (RoomThread.Trigger(TriggerEvent.DamageForseen, this, damage_data.To, ref qdata))
                {
                    RemoveQinggangTag(damage_data);
                    break;
                }
                if (damage_data.Chain && damage_data.Nature != DamageStruct.DamageNature.Normal)
                {
                    DoChainedAnimation(damage_data.To, damage_data.Nature);
                    Thread.Sleep(600);
                }
                if (damage_data.From != null)
                {
                    if (RoomThread.Trigger(TriggerEvent.DamageCaused, this, damage_data.From, ref qdata))
                    {
                        RemoveQinggangTag(damage_data);
                        break;
                    }
                }

                damage_data = (DamageStruct)qdata;
                string str = damage_data.To.Name + "_TransferDamage";
                RemoveTag(str);
                if (RoomThread.Trigger(TriggerEvent.DamageInflicted, this, damage_data.To, ref qdata) || RoomThread.Trigger(TriggerEvent.DamageDefined, this, damage_data.To, ref qdata))
                {
                    RemoveQinggangTag(damage_data);
                    // Make sure that the trigger in which 'TransferDamage' tag is set returns TRUE
                    if (ContainsTag(str) && GetTag(str) is DamageStruct transfer_damage_data && transfer_damage_data.To != null)
                        Damage(transfer_damage_data);

                    break;
                }

                enter_stack = true;
                m_damageStack.Enqueue(damage_data);
                SetTag("CurrentDamageStruct", qdata);

                RoomThread.Trigger(TriggerEvent.PreDamageDone, this, damage_data.To, ref qdata);

                RemoveQinggangTag(damage_data);
                RoomThread.Trigger(TriggerEvent.DamageDone, this, damage_data.To, ref qdata);

                if (damage_data.From != null && !damage_data.From.HasFlag("Global_DFDebut"))
                    RoomThread.Trigger(TriggerEvent.Damage, this, damage_data.From, ref qdata);

                if (!damage_data.To.HasFlag("Global_DFDebut"))
                    RoomThread.Trigger(TriggerEvent.Damaged, this, damage_data.To, ref qdata);
            } while (false);


            if (!enter_stack)
            {
                damage_data = (DamageStruct)qdata;
                damage_data.Prevented = true;
                qdata = damage_data;
                SkipGameRule = true;
            }
            damage_data = (DamageStruct)qdata;
            RoomThread.Trigger(TriggerEvent.DamageComplete, this, damage_data.To, ref qdata);

            if (enter_stack)
            {
                m_damageStack.Dequeue();
                if (m_damageStack.Count == 0)
                    RemoveTag("CurrentDamageStruct");
                else
                    SetTag("CurrentDamageStruct", m_damageStack.Peek());
            }
        }

        public bool IsJinkEffected(Player user, CardResponseStruct response)
        {
            if (response.Card == null || user == null) return false;

            object jink_data = response;
            return !RoomThread.Trigger(TriggerEvent.JinkEffect, this, user, ref jink_data);
        }

        public void SlashEffect(SlashEffectStruct effect)
        {
            object data = effect;
            if (RoomThread.Trigger(TriggerEvent.SlashEffected, this, effect.To, ref data))
            {
                if (!effect.To.HasFlag("Global_NonSkillNullify"))
                { //setEmotion(effect.to, "skill_nullify");
                }
                else
                    effect.To.SetFlags("-Global_NonSkillNullify");
                if (effect.Slash != null)
                    RemoveQinggangTag(effect.To, effect.Slash);
            }
        }
        public void SlashResult(SlashEffectStruct effect, WrappedCard jink = null)
        {
            SlashEffectStruct result_effect = effect;
            result_effect.Jink = jink;
            object data = result_effect;

            if (jink == null)
            {
                if (effect.To.Alive)
                    RoomThread.Trigger(TriggerEvent.SlashHit, this, effect.From, ref data);
            }
            else
            {
                if (effect.To.Alive)
                {
                    SetEmotion(effect.To, "jink");
                }
                if (effect.Slash != null)
                    RemoveQinggangTag(effect.To, effect.Slash);
                RoomThread.Trigger(TriggerEvent.SlashMissed, this, effect.From, ref data);
            }
        }

        public WrappedCard AskForCard(Player player, string reason, string pattern, string prompt, object data, string skill_name)
        {
            return AskForCard(player, reason, pattern, prompt, data, HandlingMethod.MethodDiscard, null, false, skill_name, false);
        }

        public WrappedCard AskForCard(Player player, string reason, string pattern, string prompt,
            object data = null, HandlingMethod method = HandlingMethod.MethodDiscard, Player to = null, bool isRetrial = false,
            string _skill_name = null, bool isProvision = false)
        {
            return AskForCard(player, reason, pattern, prompt, data, method, _skill_name, to, isRetrial, isProvision).Card;
        }

        private WrappedCard provided = null;
        private bool has_provided;
        private List<Player> _alivePlayers = new List<Player>();

        public CardResponseStruct AskForCard(Player player, string reason, string _pattern, string prompt, object data, HandlingMethod method,
            string _skill_name, Player to, bool isRetrial, bool isProvision)
        {
            CardResponseStruct resp = new CardResponseStruct() { From = player };
            string pattern = _pattern.Split(':')[0];
            if (player == null || !player.Alive) return resp;

            WrappedCard card = null;
            CardUseStruct.CardUseReason use_reason = CardUseStruct.CardUseReason.CARD_USE_REASON_RESPONSE;
            if (method == HandlingMethod.MethodResponse || pattern.StartsWith("@@"))
                use_reason = CardUseStruct.CardUseReason.CARD_USE_REASON_RESPONSE;
            else if (method == HandlingMethod.MethodUse)
                use_reason = CardUseStruct.CardUseReason.CARD_USE_REASON_RESPONSE_USE;
            _m_roomState.SetCurrentCardUseReason(use_reason);
            _m_roomState.SetCurrentCardUsePattern(pattern);
            _m_roomState.SetCurrentCardResponsePrompt(prompt);

            CardAskStruct asked = new CardAskStruct
            {
                Pattern = pattern,
                Reason = reason,
                Prompt = prompt,
                Data = data
            };
            object asked_data = asked;
            if ((method == HandlingMethod.MethodUse || method == HandlingMethod.MethodResponse) && !isRetrial)
                RoomThread.Trigger(TriggerEvent.CardAsked, this, player, ref asked_data);

            _m_roomState.SetGlobalResponseID();
            int response_id = _m_roomState.GlobalResponseID;

            string tag = string.Format("current_{0}", pattern);
            if (data != null) SetTag(tag, data);
            while (card == null)
            {
                if (has_provided)
                {
                    if (provided != null && player.Alive && !RoomLogic.IsCardLimited(this, player, provided, method))
                        card = provided;
                    provided = null;
                    has_provided = false;
                }
                
                if (player.Alive && card == null)
                {
                    NotifyMoveFocus(player, CommandType.S_COMMAND_RESPONSE_CARD);
                    Thread.Sleep(300);

                    _m_roomState.SetCurrentResponseID(response_id);
                    _m_roomState.SetCurrentCardUseReason(use_reason);
                    _m_roomState.SetCurrentCardUsePattern(pattern);
                    _m_roomState.SetCurrentCardResponsePrompt(prompt);
                    if (!_pattern.StartsWith("@") && _pattern.Contains(":"))
                    {
                        string[] strs = _pattern.Split(':');
                        _m_roomState.SetCurrentResponseSkill(strs[strs.Length - 1]);
                    }
                    else
                        _m_roomState.SetCurrentResponseSkill(null);

                    TrustedAI ai = GetAI(player);
                    if (ai != null)
                    {
                        card = ai.AskForCard(reason, pattern, prompt, data);

                        if (card != null)
                        {
                            card = RoomLogic.ParseUseCard(this, card);
                            if (card.Name == DummyCard.ClassName && card.SubCards.Count == 1)
                                card = GetCard(card.GetEffectiveId());
                        }
                        if (card != null && RoomLogic.IsCardLimited(this, player, card, method)) card = null;
                        if (card != null) Thread.Sleep(400);
                    }
                    else
                    {
                        Interactivity client = GetInteractivity(player);
                        bool success = client != null && client.PlayCardRequst(this, player, CommandType.S_COMMAND_RESPONSE_CARD, prompt, method);
                        List<string> clientReply = client?.ClientReply;
                        if (client != null && success && clientReply != null && clientReply.Count != 0)
                        {
                            card = RoomLogic.ParseCard(this, clientReply[1]);
                        }
                    }

                    _m_roomState.SetCurrentResponseSkill(null);
                    DoBroadcastNotify(CommandType.S_COMMAND_UNKNOWN, new List<string> { false.ToString() });
                }
                player.SetFlags("-TargetFixed");

                if (card == null)
                {
                    object decisionData = string.Format("cardResponded%{0}%{1}%{2}%_nil_", reason, pattern, prompt);
                    RoomThread.Trigger(TriggerEvent.ChoiceMade, this, player, ref decisionData);
                    return resp;
                }

                card = Engine.GetFunctionCard(card.Name).ValidateInResponse(this, player, card);
            }
            RemoveTag(tag);

            bool isHandcard = true;
            if (card != null)
            {
                card = card.GetUsedCard();
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                List<int> ids = new List<int>(card.SubCards);
                if (ids.Count > 0)
                {
                    foreach (int id in ids)
                    {
                        if (GetCardOwner(id) != player || GetCardPlace(id) != Place.PlaceHand)
                        {
                            isHandcard = false;
                            break;
                        }
                    }
                }
                else
                {
                    isHandcard = false;
                }

                object decisionData = string.Format("cardResponded%{0}%{1}%{2}%_{3}_", reason, pattern, prompt, RoomLogic.CardToString(this, card));
                RoomThread.Trigger(TriggerEvent.ChoiceMade, this, player, ref decisionData);

                List<int> move_result = new List<int>();
                bool move = false;
                if (method == HandlingMethod.MethodUse)
                {
                    List<int> card_ids = new List<int>(card.SubCards);
                    CardMoveReason move_reason = new CardMoveReason(MoveReason.S_REASON_LETUSE, player.Name, null, card.Skill, null);
                    if (card_ids.Count > 0)
                    {
                        RecordSubCards(card);
                        move_reason.Card = card;
                        move_reason.General = RoomLogic.GetGeneralSkin(this, player, card.Skill, card.SkillPosition);
                        List<CardsMoveStruct> moves = new List<CardsMoveStruct>();
                        foreach (int id in card_ids)
                        {
                            CardsMoveStruct this_move = new CardsMoveStruct(id, null, Place.PlaceTable, move_reason);
                            moves.Add(this_move);
                        }
                        move_result = MoveCardsAtomic(moves, true);
                        move = true;
                    }
                    else if (fcard.TypeID != CardType.TypeSkill)
                    {
                        CardMoveReasonStruct virtual_move = new CardMoveReasonStruct
                        {
                            TargetId = move_reason.TargetId,
                            PlayerId = move_reason.PlayerId,
                            SkillName = move_reason.SkillName,
                            CardString = RoomLogic.CardToString(this, card),
                            EventName = move_reason.EventName,
                            Reason = move_reason.Reason,
                            General = move_reason.General
                        };

                        ClientCardsMoveStruct this_move = new ClientCardsMoveStruct(-1, player, Place.PlaceTable, virtual_move);
                        NotifyUsingVirtualCard(RoomLogic.CardToString(this, card), this_move);
                    }
                }
                else if (method == HandlingMethod.MethodDiscard)
                {
                    CardMoveReason move_reason = new CardMoveReason(MoveReason.S_REASON_THROW, player.Name, reason, string.Empty);
                    move_result = MoveCardTo(card, player, null, Place.PlaceTable, move_reason, true);
                    move = true;

                    LogMessage log = new LogMessage
                    {
                        Type = string.IsNullOrEmpty(_skill_name) ? "$DiscardCard" : "$DiscardCardWithSkill",
                        From = player.Name,
                        Card_str = string.Join("+", JsonUntity.IntList2StringList(move_result))
                    };
                    if (!string.IsNullOrEmpty(_skill_name))
                        log.Arg = _skill_name;
                    SendLog(log);
                    if (!string.IsNullOrEmpty(_skill_name))
                        NotifySkillInvoked(player, _skill_name);

                    List<int> table_cardids = GetCardIdsOnTable(move_result);
                    if (table_cardids.Count > 0)
                    {
                        CardsMoveStruct this_move = new CardsMoveStruct(table_cardids, player, null, Place.PlaceTable, Place.DiscardPile, move_reason);
                        MoveCardsAtomic(new List<CardsMoveStruct> { this_move }, true);
                    }
                }
                else if (method != HandlingMethod.MethodNone && !isRetrial)
                {
                    List<int> card_ids = new List<int>(card.SubCards);
                    CardMoveReason move_reason = new CardMoveReason(MoveReason.S_REASON_RESPONSE, player.Name);
                    if (card_ids.Count > 0)
                    {
                        RecordSubCards(card);
                        move_reason.SkillName = card.Skill;
                        move_reason.Card = card;
                        move_reason.General = RoomLogic.GetGeneralSkin(this, player, card.Skill, card.SkillPosition);
                        List<CardsMoveStruct> moves = new List<CardsMoveStruct>();
                        foreach (int id in card_ids)
                        {
                            CardsMoveStruct this_move = new CardsMoveStruct(id, null, Place.PlaceTable, move_reason);
                            moves.Add(this_move);
                        }
                        move_result = MoveCardsAtomic(moves, true);
                        move = true;
                    }
                    else if (fcard.TypeID != CardType.TypeSkill)
                    {
                        CardMoveReasonStruct virtual_move = new CardMoveReasonStruct
                        {
                            PlayerId = move_reason.PlayerId,
                            TargetId = move_reason.TargetId,
                            SkillName = move_reason.SkillName,
                            EventName = move_reason.EventName,
                            General = move_reason.General,
                        };
                        ClientCardsMoveStruct this_move = new ClientCardsMoveStruct(-1, player, Place.PlaceTable, virtual_move)
                        {
                            Open = true
                        };
                        NotifyUsingVirtualCard(RoomLogic.CardToString(this, card), this_move);
                    }
                }
                if (!move) move_result = ids;

                if ((method == HandlingMethod.MethodUse || method == HandlingMethod.MethodResponse) && !isRetrial)
                {
                    LogMessage log = new LogMessage
                    {
                        Card_str = RoomLogic.CardToString(this, card),
                        From = player.Name,
                        Type = string.Format("#{0}", card.Name)
                    };
                    if (method == HandlingMethod.MethodResponse)
                        log.Type += "_Resp";
                    SendLog(log);

                    BroadcastSkillInvoke(player, card);
                    ShowSkill(player, card.ShowSkill, card.SkillPosition);
                    if (!string.IsNullOrEmpty(card.Skill) && card.Skill == card.GetSkillName() && RoomLogic.PlayerHasSkill(this, player, card.Skill))
                    {
                        NotifySkillInvoked(player, card.Skill);
                    }
                    resp = new CardResponseStruct(player, card, to, method == HandlingMethod.MethodUse)
                    {
                        Handcard = isHandcard,
                        Reason = reason,
                        Data = data
                    };
                    object resp_data = resp;
                    RoomThread.Trigger(TriggerEvent.CardResponded, this, player, ref resp_data);
                    if (method == HandlingMethod.MethodUse)
                    {
                        List<int> table_cardids = GetCardIdsOnTable(GetSubCards(card));
                        if (table_cardids.Count > 0)
                        {
                            //DummyCard dummy(table_cardids);
                            CardMoveReason move_reason = new CardMoveReason(MoveReason.S_REASON_LETUSE, player.Name, null, card.Skill, null)
                            {
                                Card = card,
                                General = RoomLogic.GetGeneralSkin(this, player, card.Skill, card.SkillPosition)
                            };
                            CardsMoveStruct this_move = new CardsMoveStruct(table_cardids, player, Place.DiscardPile, move_reason);
                            MoveCardsAtomic(new List<CardsMoveStruct> { this_move }, true);
                        }
                        CardUseStruct card_use = new CardUseStruct
                        {
                            Card = card,
                            From = player,
                            To = new List<Player>(),
                            Reason = CardUseStruct.CardUseReason.CARD_USE_REASON_RESPONSE,
                        };
                        if (to != null) card_use.To.Add(to);
                        if (data is CardEffectStruct effect)
                        {
                            UseRespond usedata = new UseRespond(effect.From, null, effect.Card);
                            card_use.RespondData = usedata;
                        }
                        else if (data is SlashEffectStruct slashEffect)
                        {
                            UseRespond usedata = new UseRespond(slashEffect.From, null, slashEffect.Slash);
                            card_use.RespondData = usedata;
                        }

                        object data2 = card_use;
                        RoomThread.Trigger(TriggerEvent.CardFinished, this, player, ref data2);
                    }
                    else if (!isProvision)
                    {
                        List<int> table_cardids = GetCardIdsOnTable(GetSubCards(card));
                        if (table_cardids.Count > 0)
                        {
                            CardMoveReason move_reason = new CardMoveReason(MoveReason.S_REASON_RESPONSE, player.Name)
                            {
                                SkillName = card.Skill,
                                Card = card,
                                General = RoomLogic.GetGeneralSkin(this, player, card.Skill, card.SkillPosition)
                            };
                            CardsMoveStruct this_move = new CardsMoveStruct(table_cardids, player, Place.DiscardPile, move_reason);
                            MoveCardsAtomic(new List<CardsMoveStruct> { this_move }, true);
                        }
                        RemoveSubCards(card);
                    }
                }
                resp.Card = card;
            }

            return resp;
        }

        public void SendJudgeResult(JudgeStruct judge)
        {
            List<string> arg = new List<string> { GameEventType.S_GAME_EVENT_JUDGE_RESULT.ToString(), judge.Card.Id.ToString(), judge.IsEffected().ToString() };
            DoBroadcastNotify(CommandType.S_COMMAND_LOG_EVENT, arg);
        }

        public TriggerStruct AskForSkillTrigger(Player player, string reason, List<TriggerStruct> skills, bool optional = true, object data = null, bool ignore_preshow = false)
        {
            //tryPause();
            Thread.Sleep(400);

            TriggerStruct answer = new TriggerStruct();
            List<TriggerStruct> all_skills = new List<TriggerStruct>(), all_skills_copy = new List<TriggerStruct>();
            List<string> all_skills_q = new List<string>();

            foreach (TriggerStruct skill in skills)
            {                                  //append target
                if (string.IsNullOrEmpty(skill.SkillName) || string.IsNullOrEmpty(skill.Invoker)) continue;

                bool can_skip = true;
                Player invoker = FindPlayer(skill.Invoker, true);
                Player skill_owner = FindPlayer(skill.SkillOwner, true);
                if (skill_owner == null) skill_owner = invoker;
                TriggerSkill trskill = Engine.GetTriggerSkill(skill.SkillName);
                if (trskill != null && (RoomLogic.PlayerHasShownSkill(this, skill_owner, trskill) || trskill.Global)
                        && (trskill.SkillFrequency == Frequency.Compulsory || trskill.SkillFrequency == Frequency.Wake))
                {
                    optional = false;
                    can_skip = false;
                }
                if (skill.Targets.Count == 0)
                {
                    TriggerStruct skill_copy = skill;
                    if (!string.IsNullOrEmpty(skill.SkillOwner)) skill_copy.ResultTarget = skill.SkillOwner;
                    all_skills.Add(skill_copy);
                }
                else
                {
                    if (!can_skip)
                    {
                        TriggerStruct skill_copy = skill;
                        skill_copy.ResultTarget = skill.Targets[0];
                        all_skills.Add(skill_copy);
                    }
                    else
                    {
                        foreach (string name in skill.Targets)
                        {
                            TriggerStruct skill_copy = skill;
                            skill_copy.ResultTarget = name;
                            all_skills.Add(skill_copy);
                        }
                    }
                }
            }

            foreach (TriggerStruct skill in all_skills)
                all_skills_q.Add(JsonUntity.Object2Json(skill));
            Dictionary<string, int> duplicated = new Dictionary<string, int>();
            foreach (string skill_q in all_skills_q)
            {                                      //count duplicated
                if (!duplicated.ContainsKey(skill_q))
                    duplicated[skill_q] = 1;
                else
                    duplicated[skill_q]++;
            }

            foreach (string key in duplicated.Keys)
            {
                TriggerStruct skill = JsonUntity.Json2Object<TriggerStruct>(key);
                skill.Times = duplicated[key];
                all_skills_copy.Add(skill);
            }

            all_skills.Clear();
            foreach (TriggerStruct skill in all_skills_copy)
            {                         //judge head or deputy
                if (!string.IsNullOrEmpty(skill.SkillPosition) || Engine.GetSkill(skill.SkillName) == null)
                    all_skills.Add(skill);
                else
                {
                    Player invoker = FindPlayer(skill.Invoker, true);
                    Player skill_owner = FindPlayer(skill.SkillOwner, true);
                    if (skill_owner == null) skill_owner = invoker;
                    Skill real_skill = Engine.GetSkill(skill.SkillName);
                    if (skill_owner != invoker)
                    {
                        if (RoomLogic.PlayerHasShownSkill(this, skill_owner, real_skill))
                        {
                            if (RoomLogic.GetHeadActivedSkills(this, skill_owner, true, true).Contains(real_skill))
                            {
                                TriggerStruct skill_copy = skill;
                                skill_copy.SkillPosition = "head";
                                all_skills.Add(skill_copy);
                            }
                            if (RoomLogic.GetDeputyActivedSkills(this, skill_owner, true, true).Contains(real_skill))
                            {
                                TriggerStruct skill_copy = skill;
                                skill_copy.SkillPosition = "deputy";
                                all_skills.Add(skill_copy);
                            }
                        }
                    }
                    else
                    {
                        if (RoomLogic.GetHeadActivedSkills(this, skill_owner, true, false, ignore_preshow).Contains(real_skill)
                            || RoomLogic.GetDeputyActivedSkills(this, skill_owner, true, false, ignore_preshow).Contains(real_skill))
                        {
                            if (RoomLogic.GetHeadActivedSkills(this, skill_owner, true, false, ignore_preshow).Contains(real_skill))
                            {
                                TriggerStruct skill_copy = skill;
                                skill_copy.SkillPosition = "head";
                                all_skills.Add(skill_copy);
                            }
                            if (RoomLogic.GetDeputyActivedSkills(this, skill_owner, true, false, ignore_preshow).Contains(real_skill))
                            {
                                TriggerStruct skill_copy = skill;
                                skill_copy.SkillPosition = "deputy";
                                all_skills.Add(skill_copy);
                            }
                        }
                        else
                            all_skills.Add(skill);
                    }
                }
            }

            all_skills_copy = new List<TriggerStruct>(all_skills);                                           //re match duplicated
            all_skills.Clear();
            foreach (TriggerStruct skill1 in all_skills_copy)
            {
                if (skill1.Targets.Count == 0)
                {
                    all_skills.Add(skill1);
                    continue;
                }

                bool same = false;
                foreach (TriggerStruct skill2 in all_skills)
                {
                    if (skill1.Equals(skill2) && skill1.Targets.Count > 0 && skill2.Targets.Count > 0 &&
                            skill1.ResultTarget == skill2.ResultTarget && skill1.SkillPosition == skill2.SkillPosition)
                    {
                        same = true;
                        break;
                    }
                }
                if (!same)
                {
                    int times = 0;
                    List<string> targets = new List<string>();
                    foreach (TriggerStruct skill3 in all_skills_copy)
                    {
                        if (skill1.Equals(skill3) && skill1.Targets.Count > 0 && skill3.Targets.Count > 0 &&
                            skill1.ResultTarget == skill3.ResultTarget && skill1.SkillPosition == skill3.SkillPosition)
                        {
                            times += skill3.Times;
                            if (skill3.Targets.Count > targets.Count) targets = skill3.Targets;
                        }
                    }
                    TriggerStruct new_skill = skill1;
                    new_skill.Targets = targets;
                    new_skill.Times = times;
                    all_skills.Add(new_skill);
                }
            }

            if (all_skills.Count == 1 && reason != "GameRule:TurnStart")
            {                                        //do not use this function when only 1 choice except turnstart
                answer = all_skills[0];
            }
            else
            {
                NotifyMoveFocus(player, CommandType.S_COMMAND_TRIGGER_ORDER);

                TrustedAI ai = GetAI(player);
                if (ai != null)
                {
                    //Temporary method to keep compatible with existing AI system
                    List<string> alls = new List<string>();
                    foreach (TriggerStruct skill in skills)
                        alls.Add(skill.SkillName);

                    if (optional)
                        alls.Add("cancel");

                    string reply = ai.AskForChoice(reason, string.Join("+", alls), data);
                    if (reply != "cancel")
                    {
                        foreach (TriggerStruct skill in all_skills)
                        {
                            if (skill.SkillName == reply)
                            {
                                answer = skill;
                                break;
                            }
                        }
                    }
                    Thread.Sleep(400);
                }
                else
                {
                    List<string> args = new List<string> { player.Name, reason, optional.ToString(), JsonUntity.Object2Json(all_skills) };
                    bool success = DoRequest(player, CommandType.S_COMMAND_TRIGGER_ORDER, args, true);
                    Interactivity client = GetInteractivity(player);
                    List<string> clientReply = client?.ClientReply;
                    if (clientReply == null || clientReply.Count == 0) success = false;
                    if (success && clientReply[0] != "cancel")
                    {
                        try
                        {
                            answer = JsonUntity.Json2Object<TriggerStruct>(clientReply[0]);
                            if (!all_skills.Contains(answer))
                            {
                                success = false;
                                answer = new TriggerStruct();
                            }
                        }
                        catch (Exception e)
                        {
                            LogHelper.WriteLog(null, e);
                            Debug("Parse AskForSkillTrigger Reply error");
                            success = false;
                        }
                    }
                    if (!success && !optional)
                        answer = all_skills[1];
                }
                DoBroadcastNotify(CommandType.S_COMMAND_UNKNOWN, new List<string> { false.ToString() });
            }
            answer.Times = 1;

            return answer;
        }

        public void TurnOver(Player player)
        {
            player.FaceUp = !player.FaceUp;
            BroadcastProperty(player, "FaceUp");

            if (Setting.GameMode == "Hegemony")
            {
                LogMessage log = new LogMessage
                {
                    Type = "#TurnOver",
                    From = player.Name,
                    Arg = player.FaceUp ? "face_up" : "face_down"
                };
                SendLog(log);
            }
            else
            {
                LogMessage log = new LogMessage
                {
                    Type = "#TurnOver1",
                    From = player.Name,
                    Arg = player.FaceUp ? "face_up1" : "face_down1"
                };
                SendLog(log);
            }

            RoomThread.Trigger(TriggerEvent.TurnedOver, this, player);
        }

        public void EndPlay(Player player)
        {
            PlayerPhase current_phase = player.Phase;
            player.Phase = PlayerPhase.PhaseNone;

            PhaseChangeStruct phase_change = new PhaseChangeStruct
            {
                From = current_phase,
                To =  PlayerPhase.NotActive
            };
            object data = phase_change;

            RoomThread.Trigger(TriggerEvent.EventPhaseChanging, this, player, ref data);

            player.Phase =  PlayerPhase.NotActive;
            BroadcastProperty(player, "Phase");

            RoomThread.Trigger(TriggerEvent.EventPhaseStart, this, player);
        }

        public void Play(Player player, List<PlayerPhase> set_phases = null)
        {
            if (set_phases != null && set_phases.Count > 0)
            {
                if (!set_phases.Contains(PlayerPhase.NotActive))
                    set_phases.Add(PlayerPhase.NotActive);
            }
            else
                set_phases = new List<PlayerPhase>
                {
                    PlayerPhase.RoundStart , PlayerPhase.Start , PlayerPhase.Judge , PlayerPhase.Draw , PlayerPhase.Play,
                    PlayerPhase.Discard ,PlayerPhase.Finish , PlayerPhase.NotActive
                };

            player.Phases = set_phases;
            player.PhasesState.Clear();
            for (int i = 0; i < set_phases.Count; i++)
            {
                PhaseStruct _phase = new PhaseStruct
                {
                    Phase = set_phases[i]
                };
                player.PhasesState.Add(_phase);
            }

            //每执行完一个回合就从头开始检查，因为游戏过程中可能会插入一个新的额外阶段
            do
            {
                if (!player.Alive)                                              //玩家死亡则直接结束
                {
                    EndPlay(player);
                    break;
                }

                bool proceed = false;
                List<PlayerPhase> phases = new List<PlayerPhase>(player.Phases);
                List<PhaseStruct> phases_state = new List<PhaseStruct>(player.PhasesState);
                for (int i = 0; i < phases.Count; i++)
                {
                    if (phases_state[i].Finished)
                    {
                        continue;                       //若该阶段已经执行过则跳过
                    }

                    proceed = true;
                    player.PhasesIndex = i;
                    PhaseChangeStruct phase_change = new PhaseChangeStruct
                    {
                        From = player.Phase,
                        To = phases[i]
                    };

                    player.Phase = PlayerPhase.PhaseNone;
                    object data = phase_change;
                    bool phase_proceed = true;
                    if (phase_change.To == PlayerPhase.NotActive)
                    {
                        RoomThread.Trigger(TriggerEvent.EventPhaseChanging, this, player, ref data);                                                            //回合结束无论如何都不能跳过
                    }
                    else
                    {
                        bool skip = phases_state[i].Skipped || RoomThread.Trigger(TriggerEvent.EventPhaseChanging, this, player, ref data);               //不能改变跳至的阶段
                        if (!skip)
                        {
                            foreach (PhaseStruct phase in player.PhasesState)
                            {
                                if (phase.Phase == phases[i] && !phase.Finished && phase.Skipped)
                                {
                                    skip = true;
                                    break;
                                }
                            }
                        }
                        if (skip && !RoomThread.Trigger(TriggerEvent.EventPhaseSkipping, this, player, ref data))                                         //应该用插入一个新阶段
                            phase_proceed = false;
                    }

                    if (phase_proceed)
                    {
                        for (int index = 0; index < player.PhasesState.Count; index++)                      //某些技能存在“不能跳过XX阶段”，因此要修正记录
                        {
                            if (player.PhasesState[index].Phase == phases[i] && !player.PhasesState[index].Finished)
                            {
                                player.PhasesState[index] = new PhaseStruct() { Phase = phases[i], Skipped = false };
                                break;
                            }
                        }

                        PhaseStruct phase = phases_state[i];
                        phase.Phase = phases[i] = phase_change.To;
                        player.Phase = phases[i];
                        BroadcastProperty(player, "Phase");
                        Thread.Sleep(200);

                        if (!string.IsNullOrEmpty(PreWinner)) break;

                        if (!RoomThread.Trigger(TriggerEvent.EventPhaseStart, this, player))
                        {
                            if (!string.IsNullOrEmpty(PreWinner)) break;
                            
                            if (player.Phase != PlayerPhase.NotActive)                                                                          //回合结束没有执行和结束时机
                            {
                                if (player.Phase == PlayerPhase.Draw)
                                {
                                    object draw = 2;
                                    RoomThread.Trigger(TriggerEvent.EventPhaseProceeding, this, player, ref draw);
                                }
                                else
                                    RoomThread.Trigger(TriggerEvent.EventPhaseProceeding, this, player);
                            }
                            else
                            {
                                proceed = false;
                                break;
                            }
                        }

                        if (!string.IsNullOrEmpty(PreWinner)) break;

                        RoomThread.Trigger(TriggerEvent.EventPhaseEnd, this, player);
                    }
                    else
                    {
                        for (int index = 0; index < player.PhasesState.Count; index++)                              //记录为改阶段跳过
                        {
                            if (player.PhasesState[index].Phase == phases[i] && !player.PhasesState[index].Finished)
                            {
                                player.PhasesState[index] = new PhaseStruct() { Phase = phases[i], Skipped = true };
                                break;
                            }
                        }
                    }

                    for (int index = 0; index < player.PhasesState.Count; index++)                                                                  //运行到这一段表示该阶段已经执行完毕
                    {
                        if (player.PhasesState[index].Phase == phase_change.To && !player.PhasesState[index].Finished)
                        {
                            player.PhasesState[index] = new PhaseStruct() { Phase = player.PhasesState[index].Phase, Skipped = player.PhasesState[index].Skipped, Finished = true };
                            break;
                        }
                    }

                    break;
                }
                if (!proceed) break;                            //当没有任何一个阶段被执行，或是进入了回合结束，则结束循环检查
            }
            while (string.IsNullOrEmpty(PreWinner));
        }

        private readonly List<string> phase_strings = new List<string> {"round_start", "start" , "judge" , "draw"
                ,"play" , "discard" , "finish" , "not_active" };
        public void SkipPhase(Player player, PlayerPhase phase, bool sendLog = true)
        {
            for (int i = player.PhasesIndex; i < player.PhasesState.Count; i++)
            {
                if (player.PhasesState[i].Phase == phase)
                {
                    if (player.PhasesState[i].Skipped) return;
                    player.PhasesState[i] = new PhaseStruct { Phase = phase, Skipped = true };
                    break;
                }
            }

            int index = (int)phase;

            if (sendLog)
            {
                LogMessage log = new LogMessage
                {
                    Type = "$SkipPhase",
                    From = player.Name,
                    Arg = phase_strings[index]
                };
                SendLog(log);
            }
        }

        public bool AskForDiscard(Player player, string reason, int discard_num, int min_num, bool optional = false,
            bool include_equip = false, string prompt = null, bool notify_skill = false, string position = null)
        {
            string flag = "h";
            if (include_equip) flag = "he";
            List<int> ids = player.GetCards(flag);

            return AskForDiscard(player, ids, reason, discard_num, min_num, optional, prompt, notify_skill, position);
        }

        public bool AskForDiscard(Player player, List<int> ids, string reason, int discard_num, int min_num, bool optional = false,
                         string prompt = null, bool notify_skill = false, string position = null)
        {
            ids = ids ?? player.GetCards("h");
            if (!player.Alive) return false;

            //tryPause();
            Thread.Sleep(400);
            NotifyMoveFocus(player, CommandType.S_COMMAND_DISCARD_CARD);

            List<int> dummy = new List<int>();
            List<int> jilei_list = new List<int>();
            foreach (int id in ids)
            {
                WrappedCard card = GetCard(id);
                if (!RoomLogic.IsJilei(this, player, card))
                    dummy.Add(card.Id);
                else
                    jilei_list.Add(card.Id);
            }

            bool success = true;
            if (!optional)
            {
                int card_num = dummy.Count;
                if (card_num <= min_num)
                {
                    DoBroadcastNotify(CommandType.S_COMMAND_UNKNOWN, new List<string> { false.ToString() });
                    if (card_num > 0)
                    {
                        CardMoveReason movereason = new CardMoveReason
                        {
                            PlayerId = player.Name,
                            SkillName = reason
                        };
                        if (reason == "gamerule")
                        {
                            movereason.Reason = MoveReason.S_REASON_RULEDISCARD;
                            movereason.SkillName = string.Empty;
                        }
                        else
                            movereason.Reason = MoveReason.S_REASON_THROW;

                        if (notify_skill)
                        {
                            NotifySkillInvoked(player, reason);
                            movereason.General = RoomLogic.GetGeneralSkin(this, player, reason, position);
                        }
                        ThrowCard(ref dummy, movereason, player);

                        object choice = string.Format("{0}:{1}:{2}", "cardDiscard", reason, string.Join("+", JsonUntity.IntList2StringList(dummy)));
                        RoomThread.Trigger(TriggerEvent.ChoiceMade, this, player, ref choice);
                    }

                    if (card_num < min_num && jilei_list.Count > 0)
                    {
                        List<string> gongxinArgs = new List<string> { player.Name, JsonUntity.Object2Json(jilei_list), reason };

                        //foreach (int cardId in jilei_list) {
                        //    //WrappedCard *card = Sanguosha->getWrappedCard(cardId);
                        //    WrappedCard* card = qobject_cast<WrappedCard*>(getCard(cardId));
                        //    if (card->isModified())
                        //        broadcastUpdateCard(getOtherPlayers(player), cardId, card);
                        //    else
                        //        broadcastResetCard(getOtherPlayers(player), cardId);
                        //}

                        LogMessage log = new LogMessage
                        {
                            Type = "$JileiShowAllCards",
                            From = player.Name
                        };

                        foreach (int card_id in jilei_list)
                            GetCard(card_id).SetFlags("visible");
                        log.Card_str = string.Join("+", JsonUntity.IntList2StringList(jilei_list));
                        SendLog(log);

                        DoBroadcastNotify(CommandType.S_COMMAND_SHOW_CARD, gongxinArgs);
                        FocusAll(3000);
                        return false;
                    }
                    return true;
                }
            }

            TrustedAI ai = GetAI(player);
            List<int> to_discard = new List<int>();
            if (ai != null)
            {
                to_discard = ai.AskForDiscard(new List<int>(dummy), reason, discard_num, min_num, optional);
                if (optional && to_discard.Count > 0)
                    Thread.Sleep(400);
            }
            else
            {
                Interactivity client = GetInteractivity(player);
                success = client != null && client.DiscardRequest(this, player, ids, prompt, reason, position, discard_num, min_num, optional);
                if (client != null)
                {
                    List<string> clientReply = client.ClientReply;
                    string str = success && clientReply != null && clientReply.Count > 0 ? clientReply[0] : null;
                    if (!string.IsNullOrEmpty(str))
                    {
                        to_discard = JsonUntity.Json2List<int>(str);
                    }
                }

            }
            DoBroadcastNotify(CommandType.S_COMMAND_UNKNOWN, new List<string> { false.ToString() });

            if (!success || to_discard.Count > discard_num || to_discard.Count < min_num)
            {
                if (optional)
                {
                    DoBroadcastNotify(CommandType.S_COMMAND_UNKNOWN, new List<string> { false.ToString() });
                    return false;
                }
                // time is up, and the server choose the cards to discard
                to_discard = ForceToDiscard(player, ids, discard_num, true, to_discard);
            }

            if (to_discard.Count == 0) return false;

            if (reason == "gamerule")
            {
                CardMoveReason move_reason = new CardMoveReason(MoveReason.S_REASON_RULEDISCARD, player.Name, reason, string.Empty);
                ThrowCard(ref to_discard, move_reason, player);
            }
            else
            {
                CardMoveReason move_reason = new CardMoveReason(MoveReason.S_REASON_THROW, player.Name, reason, string.Empty);
                if (notify_skill)
                {
                    NotifySkillInvoked(player, reason);
                    move_reason.General = RoomLogic.GetGeneralSkin(this, player, reason, position);
                }
                ThrowCard(ref to_discard, move_reason, player, null, notify_skill ? reason : null);
            }
            object data = string.Format("{0}:{1}:{2}", "cardDiscard", reason, string.Join("+", JsonUntity.IntList2StringList(to_discard)));
            RoomThread.Trigger(TriggerEvent.ChoiceMade, this, player, ref data);

            return true;
        }

        public List<int> ForceToDiscard(Player player, List<int> ids, int discard_num, bool is_discard = true, List<int> reserved_discard = null)
        {
            List<int> to_discard = reserved_discard != null ? new List<int>(reserved_discard) : new List<int>(), all_cards = new List<int>(ids);
            if (to_discard.Count > discard_num)
            {
                Shuffle.shuffle(ref to_discard);
                for (int i = to_discard.Count; i > discard_num; i--)
                    to_discard.RemoveAt(i - 1);
            }
            else
            {
                Shuffle.shuffle(ref all_cards);
                for (int i = 0; i < all_cards.Count; i++)
                {
                    if (!to_discard.Contains(all_cards[i]) && (!is_discard || !RoomLogic.IsJilei(this, player, GetCard(all_cards[i]))))
                        to_discard.Add(all_cards[i]);
                    if (to_discard.Count >= discard_num)
                        break;
                }
            }

            return to_discard;
        }

        public List<int> ForceToExchange(Player player, int discard_num, string pattern, string expand_pile)
        {
            List<int> to_discard = new List<int>();
            List<WrappedCard> all_cards = new List<WrappedCard>();
            string _pattern = pattern;
            bool is_discard = false;
            if (pattern.EndsWith("!"))
            {
                is_discard = true;
                _pattern = _pattern.Remove(_pattern.Length - 1);
            }

            foreach (WrappedCard c in RoomLogic.GetPlayerCards(this, player, "he"))
            {
                if (Engine.MatchExpPattern(this, _pattern, player, c))
                    all_cards.Add(c);
            }
            expand_pile = expand_pile ?? string.Empty;
            foreach (string pile in expand_pile.Split(','))
            {
                foreach (int id in player.GetPile(pile))
                {
                    WrappedCard c = GetCard(id);
                    if (string.IsNullOrEmpty(pattern) || Engine.MatchExpPattern(this, _pattern, player, c))
                        all_cards.Add(c);
                }
            }
            Shuffle.shuffle(ref all_cards);

            for (int i = 0; i < all_cards.Count; i++)
            {
                if (!is_discard || !RoomLogic.IsJilei(this, player, all_cards[i]))
                    to_discard.Add(all_cards[i].Id);
                if (to_discard.Count == discard_num)
                    break;
            }

            return to_discard;
        }

        public void ThrowCard(int card_id, Player who, Player thrower = null, string skill_name = null)
        {
            List<int> to_discard = new List<int> { card_id };
            ThrowCard(ref to_discard, who, thrower, skill_name);
        }

        public void ThrowCard(ref List<int> to_discard, Player who, Player thrower = null, string skill_name = null)
        {
            CardMoveReason reason = new CardMoveReason();
            if (thrower == null)
            {
                reason.Reason = MoveReason.S_REASON_THROW;
                reason.PlayerId = who?.Name;
            }
            else
            {
                reason.Reason = MoveReason.S_REASON_DISMANTLE;
                reason.TargetId = who?.Name;
                reason.PlayerId = thrower.Name;
            }
            reason.SkillName = skill_name;
            ThrowCard(ref to_discard, reason, who, thrower, skill_name);
        }

        public void ThrowCard(ref List<int> to_discard, CardMoveReason reason, Player who, Player thrower = null, string skill_name = null)
        {
            if (to_discard.Count == 0) return;
            List<int> result = new List<int>();

            LogMessage log = new LogMessage();
            if (who != null)
            {
                if (thrower == null)
                {
                    if (string.IsNullOrEmpty(skill_name))
                        log.Type = "$DiscardCard";
                    else
                    {
                        log.Type = "$DiscardCardWithSkill";
                        log.Arg = skill_name;
                    }
                    log.From = who.Name;
                }
                else
                {
                    log.Type = "$DiscardCardByOther";
                    log.From = thrower.Name;
                    log.To = new List<string> { who.Name };
                }
            }
            else
            {
                log.Type = "$EnterDiscardPile";
            }

            if (who != null)
            { // player's card cannot enter discard_pile directly
                CardsMoveStruct move = new CardsMoveStruct(to_discard, who, null, Place.PlaceUnknown, Place.PlaceTable, reason);
                result = MoveCardsAtomic(move, true);
                log.Card_str = string.Join("+", JsonUntity.IntList2StringList(result));
                SendLog(log);

                List<int> new_list = GetCardIdsOnTable(result);
                if (new_list.Count > 0)
                {
                    CardsMoveStruct move2 = new CardsMoveStruct(new_list, who, null, Place.PlaceTable, Place.DiscardPile, reason);
                    MoveCardsAtomic(move2, true);
                }
            }
            else if ((reason.Reason & MoveReason.S_MASK_BASIC_REASON) == MoveReason.S_REASON_DISCARD)
            {
                // discard must through place_table
                CardsMoveStruct move = new CardsMoveStruct(to_discard, null, Place.PlaceTable, reason);
                result = MoveCardsAtomic(move, true);
                log.Card_str = string.Join("+", JsonUntity.IntList2StringList(result));
                SendLog(log);

                List<int> new_list = GetCardIdsOnTable(result);
                if (new_list.Count > 0)
                {
                    CardsMoveStruct move2 = new CardsMoveStruct(new_list, null, Place.DiscardPile, reason);
                    MoveCardsAtomic(move2, true);
                }
            }
            else
            { // other conditions
                CardsMoveStruct move = new CardsMoveStruct(to_discard, null, Place.DiscardPile, reason);
                result = MoveCardsAtomic(move, true);
                log.Card_str = string.Join("+", JsonUntity.IntList2StringList(result));
                SendLog(log);
            }
            to_discard = new List<int>(result);
        }

        public void Activate(Player player, out CardUseStruct card_use, bool add_index = true)
        {
            //tryPause();
            Thread.Sleep(300);

            card_use = new CardUseStruct(null, player, new List<Player>(), true);
            if (player.HasFlag("Global_PlayPhaseTerminated"))
            {
                player.SetFlags("-Global_PlayPhaseTerminated");
                return;
            }

            if (player.Phase != PlayerPhase.Play)
                return;

            if (player.HasFlag("kuangcai"))
            {
                List<Player> players = new List<Player> { player };
                Countdown countdown = new Countdown
                {
                    Max = 5000 - player.GetMark("kuangcai") * 1000,
                    Command = CommandType.S_COMMAND_PLAY_CARD,
                    Type = Countdown.CountdownType.S_COUNTDOWN_USE_SPECIFIED
                };

                NotifyMoveFocus(players, countdown, player);
            }
            else
                NotifyMoveFocus(player, CommandType.S_COMMAND_PLAY_CARD);

            _m_roomState.SetCurrentCardUsePattern("..");
            _m_roomState.SetCurrentCardUseReason(CardUseStruct.CardUseReason.CARD_USE_REASON_PLAY);
            if (add_index) _m_roomState.SetGlobalActivateID();

            TrustedAI ai = GetAI(player);
            if (ai != null)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                float remainTime = 600;
                ai.Activate(ref card_use);
                sw.Stop();
                remainTime -= sw.ElapsedMilliseconds;

                if (remainTime > 0)
                    Thread.Sleep((int)remainTime);
            }
            else
            {
                bool success = GetInteractivity(player).PlayCardRequst(this, player, CommandType.S_COMMAND_PLAY_CARD);
                List<string> clientReply = GetInteractivity(player).ClientReply;

                //if (m_surrenderRequestReceived)
                //{
                //    makeSurrender(player->getClient());
                //    if (!game_finished)
                //    {
                //        return activate(player, card_use);
                //    }
                //}
                //else
                //{
                Client client = GetClient(player);
                if (client == null) return;
                if (MakeCheat(player) && client != null && client.UserRight >= 3)
                {
                    if (player.Alive && string.IsNullOrEmpty(PreWinner))
                    {
                        DoBroadcastNotify(CommandType.S_COMMAND_UNKNOWN, new List<string> { false.ToString() });
                        Activate(player, out card_use, false);
                        return;
                    }
                }
                //}

                if (!success || clientReply == null || clientReply.Count == 0)
                {
                    DoBroadcastNotify(CommandType.S_COMMAND_UNKNOWN, new List<string> { false.ToString() });
                    return;
                }
                if (clientReply.Count > 1)
                {
                    card_use.Card = RoomLogic.ParseCard(this, clientReply[1]);
                }

                if (clientReply.Count > 2)
                    card_use.To = RoomLogic.ParsePlayers(this, JsonUntity.Json2List<string>(clientReply[2]));
                if (card_use.Card == null)
                {
                    card_use = new CardUseStruct();
                    DoBroadcastNotify(CommandType.S_COMMAND_UNKNOWN, new List<string> { false.ToString() });
                    Debug(string.Format("Card cannot be parsed:\n {0}", clientReply[1]));
                    return;
                }
                card_use.From = player;
            }
            DoBroadcastNotify(CommandType.S_COMMAND_UNKNOWN, new List<string> { false.ToString() });
            card_use.Reason = CardUseStruct.CardUseReason.CARD_USE_REASON_PLAY;

            if (card_use.Card != null)
            {
                List<string> targets = new List<string>();
                foreach (Player p in card_use.To)
                {
                    System.Diagnostics.Debug.Assert(p != null, card_use.Card.Name);
                    targets.Add(p.Name);
                }
                object data = string.Format("Activate:{0}->{1}", RoomLogic.CardToString(this, card_use.Card), string.Join("+", targets));
                RoomThread.Trigger(TriggerEvent.ChoiceMade, this, player, ref data);
            }
        }

        public bool UseCard(CardUseStruct card_use, bool add_history = true, bool ignore_rule = false)
        {
            card_use.AddHistory = false;
            card_use.IsHandcard = false;
            WrappedCard card = RoomLogic.ParseUseCard(this, card_use.Card);
            card_use.Card = card;
            if (card_use.To == null) card_use.To = new List<Player>();
            FunctionCard fcard = Engine.GetFunctionCard(card.Name);
            if (card == null || (RoomLogic.IsCardLimited(this, card_use.From, card, fcard.Method) && !fcard.CanRecast(this, card_use.From, card)))
                return false;

            List<int> ids = new List<int>(card.SubCards);
            if (ids.Count > 0)
            {
                bool check = true;
                foreach (int id in ids)
                {
                    if (GetCardOwner(id) != card_use.From || GetCardPlace(id) != Place.PlaceHand)
                    {
                        check = false;
                        break;
                    }
                }

                if (check)
                    card_use.IsHandcard = true;
            }

            string key = card.Name;

            card = fcard.Validate(this, card_use);
            if (card == null)
                return false;

            card_use.Card = card;
            fcard = Engine.GetFunctionCard(card.Name);
            if (card_use.From.Phase == PlayerPhase.Play && add_history
                    && (!(fcard is Slash) || _m_roomState.GetCurrentCardUseReason() == CardUseStruct.CardUseReason.CARD_USE_REASON_PLAY
                    || card_use.Reason == CardUseStruct.CardUseReason.CARD_USE_REASON_PLAY || card_use.Pattern == "@@rende")
                    && !Engine.CorrectCardTarget(this, TargetModSkill.ModType.History, card_use.From, null, card))
            {
                card_use.AddHistory = true;
                card_use.From.AddHistory(key);
                if (card.Name != key && (card.Name.Contains(Slash.ClassName) || card.Name == Analeptic.ClassName))
                    card_use.From.AddHistory(card.Name);

                if (card.IsVirtualCard() && fcard.TypeID != CardType.TypeSkill && !string.IsNullOrEmpty(card.Skill))
                    card_use.From.AddHistory(string.Format("ViewAsSkill_{0}Card", card.Skill));
            }

            fcard.OnCardAnnounce(this, card_use, ignore_rule);
            return true;
        }
        public CardUseStruct AskForSinglePeach(Player player, Player dying, DyingStruct dying_struct)
        {
            //tryPause();
            Thread.Sleep(300);
            NotifyMoveFocus(player, CommandType.S_COMMAND_ASK_PEACH);
            _m_roomState.SetCurrentCardUseReason(CardUseStruct.CardUseReason.CARD_USE_REASON_RESPONSE_USE);
            _m_roomState.SetCurrentCardUsePattern(Peach.ClassName);

            _m_roomState.SetCurrentAskforPeachPlayer(dying);
            _m_roomState.SetCurrentCardResponsePrompt("askForSinglePeach");

            WrappedCard card = null;
            Player repliedPlayer = null;
            CardUseStruct use = new CardUseStruct
            {
                To = new List<Player> { dying },
                Reason = CardUseStruct.CardUseReason.CARD_USE_REASON_RESPONSE_USE
            };

            TrustedAI ai = GetAI(player);
            if (ai != null)
            {
                card = ai.AskForSinglePeach(dying, dying_struct);
                repliedPlayer = player;
                if (card != null) card = RoomLogic.ParseUseCard(this, card);
            }
            else
            {
                Interactivity client = GetInteractivity(player);
                int peaches = 1 - dying.Hp;
                
                bool success = client != null && client.PeachRequest(this, dying, peaches);
                if (client != null)
                {
                    List<string> clientReply = client.ClientReply;
                    if (!success || clientReply == null || clientReply.Count == 0)
                    {
                        DoBroadcastNotify(CommandType.S_COMMAND_UNKNOWN, new List<string> { false.ToString() });
                        _m_roomState.SetCurrentCardResponsePrompt(null);
                        _m_roomState.SetCurrentAskforPeachPlayer(null);
                        return use;
                    }

                    card = RoomLogic.ParseCard(this, clientReply[1]);
                    repliedPlayer = FindPlayer(clientReply[0]);
                }
            }
            DoBroadcastNotify(CommandType.S_COMMAND_UNKNOWN, new List<string> { false.ToString() });
            _m_roomState.SetCurrentCardResponsePrompt(null);
            _m_roomState.SetCurrentAskforPeachPlayer(null);

            if (card != null && RoomLogic.IsCardLimited(this, repliedPlayer, card, HandlingMethod.MethodUse)) card = null;
            if (card != null)
            {
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                card = fcard.ValidateInResponse(this, repliedPlayer, card);
            }
            else
                return use;

            if (card != null)
            {
                object decisionData = string.Format("peach:{0}:{1}:{2}", dying.Name, 1 - dying.Hp, RoomLogic.CardToString(this, card));
                RoomThread.Trigger(TriggerEvent.ChoiceMade, this, repliedPlayer, ref decisionData);
                use.From = repliedPlayer;
                use.Card = card;
            }
            else
                use = AskForSinglePeach(player, dying, dying_struct);

            return use;
        }

        public void BuryPlayer(Player player)
        {
            player.Flags.Clear();
            player.ClearHistory();
            player.ClearHistory(Analeptic.ClassName);
            ThrowAllCards(player);
            player.ArmorNullifiedList.Clear();
            ThrowAllMarks(player);
            ClearPrivatePiles(player);
            RoomLogic.ClearPlayerCardLimitation(player, false);
        }
        public void ThrowAllHandCards(Player player)
        {
            int card_length = player.HandcardNum;
            AskForDiscard(player, null, null, card_length, card_length);
        }
        public void ThrowAllEquips(Player player)
        {
            List<int> equips = player.GetEquips();
            if (equips.Count == 0) return;

            foreach (int id in new List<int>(equips))
                if (RoomLogic.IsJilei(this, player, GetCard(id)))
                    equips.Remove(id);

            if (equips.Count > 0)
                ThrowCard(ref equips, player);
        }
        public void ThrowAllHandCardsAndEquips(Player player)
        {
            int card_length = player.GetCardCount(true);
            AskForDiscard(player, "gamerule", card_length, card_length, false, true);
        }
        public void ThrowAllCards(Player player)
        {
            List<int> ids= player.GetCards("h");
            ids.AddRange(player.GetEquips());
            if (ids.Count > 0)
                ThrowCard(ref ids, player);

            CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_THROW, player.Name);
            ids = new List<int>(player.JudgingArea);
            ThrowCard(ref ids, reason, null);
        }

        public void ThrowAllMarks(Player player, bool visible_only = true)
        {
            // throw all marks
            RemovePlayerStringMark(player, ".");
            SetPlayerMark(player, "drank", 0);
            foreach (string mark_name in new List<string>(player.Marks.Keys))
            {
                if (!mark_name.StartsWith("@"))
                    continue;

                int n = player.GetMark(mark_name);
                if (n != 0)
                    SetPlayerMark(player, mark_name, 0);
            }

            if (!visible_only)
                player.Marks.Clear();
        }

        public void ClearOnePrivatePile(Player player, string pile_name)
        {

            if (!player.Piles.ContainsKey(pile_name))
                return;

            List<int> ids = new List<int>();
            foreach (int id in player.Piles[pile_name])
            {
                if (GetCardOwner(id) == player && GetCardPlace(id) == Place.PlaceSpecial)
                    ids.Add(id);
            }

            if (ids.Count > 0)
            {
                CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_REMOVE_FROM_PILE, player.Name);
                ThrowCard(ref ids, reason, null);
            }

            player.Piles.Remove(pile_name);
        }

        public void ClearPrivatePiles(Player player)
        {
            foreach (string pile_name in new List<string>(player.Piles.Keys))
                ClearOnePrivatePile(player, pile_name);

            player.Piles.Clear();
        }

        private KeyValuePair<string, List<int>> table_ag = new KeyValuePair<string, List<int>>();
        public void FillAG(string reason, List<int> card_ids, Player who = null, List<int> disabled_ids = null, List<Player> watchers = null, string prompt = null)
        {
            if (who == null) table_ag = new KeyValuePair<string, List<int>>(reason, card_ids);
            prompt = prompt ?? string.Empty;
            List<string> arg = new List<string> { reason, JsonUntity.Object2Json(card_ids), JsonUntity.Object2Json(disabled_ids), prompt };
            if (who != null)
            {
                if (watchers != null && watchers.Count > 0)
                {
                    List<Client> receivers = new List<Client>
                    {
                        GetClient(who)
                    };
                    foreach (Player player in watchers)
                        if (!receivers.Contains(GetClient(player)))
                            receivers.Add(GetClient(player));
                    foreach (Client player in receivers)
                        DoNotify(player, CommandType.S_COMMAND_FILL_AMAZING_GRACE, arg);
                }
                else
                    DoNotify(GetClient(who), CommandType.S_COMMAND_FILL_AMAZING_GRACE, arg);

            }
            else
                DoBroadcastNotify(CommandType.S_COMMAND_FILL_AMAZING_GRACE, arg);
        }

        public void TakeAG(Player player, int card_id, bool move_cards = true)
        {
            List<string> arg = new List<string> { player?.Name, card_id.ToString(), move_cards.ToString() };

            if (player != null)
            {
                CardsMoveOneTimeStruct moveOneTime = new CardsMoveOneTimeStruct();
                if (move_cards)
                {
                    CardsMoveOneTimeStruct move = new CardsMoveOneTimeStruct
                    {
                        From = null,
                        From_places = new List<Place> { Place.DrawPile },
                        To = player,
                        To_place = Place.PlaceHand,
                        Reason = new CardMoveReason(MoveReason.S_REASON_RECYCLE, player.Name),
                        Card_ids = new List<int> { card_id }
                    };
                    object data = move;
                    RoomThread.Trigger(TriggerEvent.BeforeCardsMove, this, player, ref data);
                    move = (CardsMoveOneTimeStruct)data;

                    if (move.Card_ids.Count > 0)
                    {
                        System.Diagnostics.Debug.Assert(!player.HandCards.Contains(card_id));
                        RoomLogic.AddPlayerCard(this, player, card_id, Place.PlaceHand);
                        SetCardMapping(card_id, player, Place.PlaceHand);
                        GetCard(card_id).SetFlags("visible");
                        FilterCards(player, move.Card_ids, false);
                        moveOneTime = move;
                    }
                    else
                    {
                        arg[2] = false.ToString();
                    }
                }
                DoBroadcastNotify(CommandType.S_COMMAND_TAKE_AMAZING_GRACE, arg);
                Thread.Sleep(300);
                if (move_cards && moveOneTime.Card_ids?.Count > 0)
                {
                    object data = moveOneTime;
                    RoomThread.Trigger(TriggerEvent.CardsMoveOneTime, this, player, ref data);
                }
            }
            else
            {
                DoBroadcastNotify(CommandType.S_COMMAND_TAKE_AMAZING_GRACE, arg);
                if (!move_cards) return;
                LogMessage log = new LogMessage
                {
                    Type = "$EnterDiscardPile",
                    Card_str = card_id.ToString()
                };
                SendLog(log);

                m_discardPile.Insert(0, card_id);
                SetCardMapping(card_id, null, Place.DiscardPile);
            }
        }

        public void ClearAG(Player player = null)
        {
            table_ag = new KeyValuePair<string, List<int>>(); ;
            if (player != null)
                DoNotify(GetClient(player), CommandType.S_COMMAND_CLEAR_AMAZING_GRACE, new List<string>());
            else
                DoBroadcastNotify(CommandType.S_COMMAND_CLEAR_AMAZING_GRACE, new List<string>());
        }

        public int AskForAG(Player player, List<int> card_ids, bool refusable, string reason)
        {
            //tryPause();
            NotifyMoveFocus(player, CommandType.S_COMMAND_AMAZING_GRACE);
            DoBroadcastNotify(CommandType.S_COMMAND_TAKE_AMAZING_GRACE, new List<string> { player.Name }, GetClient(player));
            Thread.Sleep(300);

            int card_id = -1;
            if (card_ids.Count == 1 && !refusable)
                card_id = card_ids[0];
            else
            {
                TrustedAI ai = GetAI(player);
                if (ai != null)
                {
                    Thread.Sleep(400);
                    card_id = ai.AskForAG(new List<int>(card_ids), refusable, reason);
                }
                else
                {
                    List<string> req = new List<string> { player.Name, refusable.ToString() };
                    bool success = DoRequest(player, CommandType.S_COMMAND_AMAZING_GRACE, req, true);
                    Interactivity client = GetInteractivity(player);
                    List<string> clientReply = client?.ClientReply;

                    if (client != null && success && clientReply != null && clientReply.Count > 0)
                        int.TryParse(clientReply[0], out card_id);
                }

                if (!card_ids.Contains(card_id))
                    card_id = refusable ? -1 : card_ids[0];
            }
            DoBroadcastNotify(CommandType.S_COMMAND_UNKNOWN, new List<string> { false.ToString() });

            object decisionData = string.Format("AGChosen:{0}:{1}", reason, card_id);
            RoomThread.Trigger(TriggerEvent.ChoiceMade, this, player, ref decisionData);

            return card_id;
        }

        public void ObtainCard(Player target, WrappedCard card, bool unhide = true)
        {
            if (card == null) return;
            CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_GOTBACK, target.Name);
            ObtainCard(target, card, reason, unhide);
        }

        public void ObtainCard(Player target, int card_id, bool unhide = true)
        {
            ObtainCard(target, GetCard(card_id), unhide);
        }

        public void ObtainCard(Player target, WrappedCard card, CardMoveReason reason, bool unhide = true)
        {
            if (card == null) return;

            List<int> card_ids = new List<int>(card.SubCards);
            ObtainCard(target, ref card_ids, reason, unhide);
        }

        public void ObtainCard(Player target, ref List<int> card_ids, CardMoveReason reason, bool unhide = true)
        {
            if (card_ids.Count == 0) return;

            CardsMoveStruct move = new CardsMoveStruct(card_ids, target, Place.PlaceHand, reason);
            card_ids = MoveCardsAtomic(new List<CardsMoveStruct> { move }, unhide);
        }

        public WrappedCard AskForUseCard(Player player, string _pattern, string prompt, UseRespond data, int notice_index = -1,
            HandlingMethod method = HandlingMethod.MethodUse, bool addHistory = true, string position = null)
        {

            //tryPause();
            Thread.Sleep(300);
            NotifyMoveFocus(player, CommandType.S_COMMAND_RESPONSE_CARD);

            const string rx_pattern = @"([_A-Za-z]+!?):(\w+)";
            Match result = Regex.Match(_pattern, rx_pattern);

            string pattern = result.Success && result.Length > 0 ? result.Groups[1].ToString() : _pattern;
            _m_roomState.SetCurrentCardUsePattern(pattern);
            _m_roomState.SetCurrentCardUseReason(CardUseStruct.CardUseReason.CARD_USE_REASON_RESPONSE_USE);
            _m_roomState.SetCurrentCardResponsePrompt(prompt);
            if (result.Success && result.Length > 0)
                _m_roomState.SetCurrentResponseSkill(result.Groups[2].ToString());
            else
                _m_roomState.SetCurrentResponseSkill(null);

            _m_roomState.SetGlobalResponseID();

            int response_id = _m_roomState.GlobalResponseID;
            int real_id = response_id;
            WrappedCard card = null;

            while (true)
            {
                _m_roomState.SetCurrentResponseID(response_id);
                CardUseStruct card_use = new CardUseStruct
                {
                    IsOwnerUse = true,
                    AddHistory = true,
                    RespondData = data
                };

                bool isCardUsed = false;
                TrustedAI ai = GetAI(player);
                if (ai != null)
                {
                    CardUseStruct answer = ai.AskForUseCard(pattern, prompt, method);
                    if (answer.Card != null)
                    {
                        answer.Card = RoomLogic.ParseUseCard(this, answer.Card);
                        isCardUsed = true;
                        card_use = answer;
                        Thread.Sleep(400);
                    }
                }
                else
                {
                    Interactivity client = GetInteractivity(player);
                    bool success = client != null && client.PlayCardRequst(this, player, CommandType.S_COMMAND_RESPONSE_CARD, prompt, method, notice_index, position);
                    if (success)
                    {
                        List<string> clientReply = client?.ClientReply;
                        card_use.Card = clientReply != null && clientReply.Count > 1 ? RoomLogic.ParseCard(this, clientReply[1]) : null;
                        isCardUsed = card_use.Card != null;
                        if (clientReply.Count > 2)
                            card_use.To = RoomLogic.ParsePlayers(this, JsonUntity.Json2List<string>(clientReply[2]));
                        if (isCardUsed)
                            card_use.From = player;
                    }
                }
                _m_roomState.SetCurrentResponseSkill(null);
                DoBroadcastNotify(CommandType.S_COMMAND_UNKNOWN, new List<string> { false.ToString() });
                card_use.Reason = CardUseStruct.CardUseReason.CARD_USE_REASON_RESPONSE_USE;
                card_use.Pattern = _pattern;
                if (isCardUsed)
                {
                    List<string> targets = new List<string>();
                    if (card_use.To != null)
                        foreach (Player p in card_use.To)
                            targets.Add(p.Name);

                    object decisionData = string.Format("cardUsed:{0}:{1}:{2}:{3}", pattern, prompt, RoomLogic.CardToString(this, card_use.Card), string.Join("+", targets));
                    RoomThread.Trigger(TriggerEvent.ChoiceMade, this, player, ref decisionData);

                    _m_roomState.SetCurrentResponseID(real_id);
                    if (UseCard(card_use, addHistory))
                    {
                        card = card_use.Card;
                        real_id = _m_roomState.GlobalResponseID;
                        break;
                    }
                    real_id = _m_roomState.GlobalResponseID;
                }
                else
                {
                    break;
                }
            }
            _m_roomState.SetCurrentResponseID(real_id);

            if (card == null)
            {
                object decisionData = string.Format("cardUsed:{0}:{1}:nil", pattern, prompt);
                RoomThread.Trigger(TriggerEvent.ChoiceMade, this, player, ref decisionData);
            }

            return card;
        }

        public WrappedCard AskForUseSlashTo(Player slasher, List<Player> victims, string prompt, UseRespond data, bool addHistory = false)
        {
            // The realization of this function in the Slash::onUse and Slash::targetFilter.
            slasher.SetFlags("slashTargetFix");
            if (victims.Count == 1)
                slasher.SetFlags("slashTargetFixToOne");
            foreach (Player victim in victims)
                victim.SetFlags("SlashAssignee");

            WrappedCard slash = AskForUseCard(slasher, Slash.ClassName, prompt, data, -1, HandlingMethod.MethodUse, addHistory);
            if (slash == null)
            {
                slasher.SetFlags("-slashTargetFix");
                slasher.SetFlags("-slashTargetFixToOne");
                foreach (Player victim in victims)
                    victim.SetFlags("-SlashAssignee");
            }

            return slash;
        }

        public WrappedCard AskForUseSlashTo(Player slasher, Player victim, string prompt, UseRespond data, bool addHistory = false)
        {
            List<Player> victims = new List<Player> { victim };
            return AskForUseSlashTo(slasher, victims, prompt, data, addHistory);
        }
        public bool IsCanceled(CardEffectStruct effect)
        {
            FunctionCard fcard = Engine.GetFunctionCard(effect.Card.Name);
            if (!fcard.IsCancelable(this, effect))
                return false;

            if (heg_nullification_targets.ContainsKey(effect.Card) && heg_nullification_targets[effect.Card].Contains(effect.To))
            {
                LogMessage log = new LogMessage
                {
                    Type = "$HegNullificationEffect",
                    From = effect.From.Name,
                    To = new List<string> { effect.To.Name },
                    Card_str = RoomLogic.CardToString(this, effect.Card)
                };
                SendLog(log);
                return true;
            }
            
            NullTimes = 0;
            HegNull = false;
            HegNullEffected = false;
            bool result = AskForNullification(effect, true);
            if (HegNullEffected && fcard.IsNDTrick())
            {
                List<Player> targets = new List<Player>();
                foreach (Player p in m_players)
                {
                    if (p.Alive && RoomLogic.IsFriendWith(this, p, effect.To))
                        targets.Add(p);
                }
                heg_nullification_targets[effect.Card] = targets;
            }
            
            return result;
        }

        public void RemoveHegNullification(WrappedCard card)
        {
            heg_nullification_targets.Remove(card);
        }

        public bool AskForNullification(CardEffectStruct effect, bool positive)
        {
            return _AskForNullification(effect.Card, effect.From, effect.To, positive, effect);
        }
        public bool _AskForNullification(WrappedCard trick, Player from, Player to, bool positive, CardEffectStruct helper)
        {
            //tryPause();
            Thread.Sleep(300);

            _m_roomState.SetCurrentCardUseReason(CardUseStruct.CardUseReason.CARD_USE_REASON_RESPONSE_USE);
            _m_roomState.SetCurrentCardUsePattern(Nullification.ClassName);
            string trick_name = trick.Name;
            List<Player> validHumanPlayers = new List<Player>();
            List<Player> validAiPlayers = new List<Player>();

            CardEffectStruct trickEffect = new CardEffectStruct
            {
                Card = trick,
                From = from,
                To = to
            };
            object data = trickEffect;
            foreach (Player player in GetAlivePlayers())
            {
                if (RoomLogic.HasNullification(this, player))
                {
                    if (!RoomThread.Trigger(TriggerEvent.TrickCardCanceling, this, player, ref data))
                    {
                        if (GetAI(player) == null)
                        {
                            validHumanPlayers.Add(player);
                        }
                        else
                            validAiPlayers.Add(player);
                    }
                }
            }
            Dictionary<Player, WrappedCard> ai_cards = new Dictionary<Player, WrappedCard>();
            foreach (Player player in validAiPlayers)
            {
                TrustedAI ai = GetAI(player);
                CardEffectStruct real = new CardEffectStruct
                {
                    Card = trick,
                    From = from,
                    To = to
                };
                WrappedCard nulli = ai.AskForNullification(helper, positive, real);
                if (nulli != null)
                    ai_cards[player] = nulli;
            }
            _m_AIraceWinner = null;
            List<Player> ais = new List<Player>(ai_cards.Keys);
            if (ais.Count > 0)
            {
                Shuffle.shuffle<Player>(ref ais);
                _m_AIraceWinner = ais[0];
            }

            Player repliedPlayer = null;
            float timeOut = Setting.GetCommandTimeout(CommandType.S_COMMAND_NULLIFICATION, ProcessInstanceType.S_SERVER_INSTANCE);
            Countdown countdown = new Countdown
            {
                Max = timeOut,
                Type = Countdown.CountdownType.S_COUNTDOWN_USE_SPECIFIED
            };
            NotifyMoveFocus(GetAlivePlayers(), countdown);

            if (validHumanPlayers.Count > 0)
            {
                List<Interactivity> receivers = new List<Interactivity>();
                foreach (Player p in validHumanPlayers)
                {
                    Interactivity client = GetInteractivity(p);
                    if (!receivers.Contains(client))
                        receivers.Add(client);
                }
                if (_m_AIraceWinner != null)
                    validHumanPlayers.Add(_m_AIraceWinner);

                foreach (Interactivity p in receivers)
                {
                    p.NullificationRequest(this, trick_name, from, to);
                    DoNotify(GetClient(p.ClientId), CommandType.S_COMMAND_NULLIFICATION_ASKED, new List<string> { trick.Name });
                }

                repliedPlayer = DoBroadcastRaceRequest(validHumanPlayers, CommandType.S_COMMAND_NULLIFICATION, timeOut);
            }

            if (validHumanPlayers.Count == 0 && _m_AIraceWinner != null)
            {
                repliedPlayer = _m_AIraceWinner;
                Thread.Sleep(500);
            }

            WrappedCard card = null;
            if (repliedPlayer != null)
            {
                if (repliedPlayer == _m_AIraceWinner)
                {
                    foreach (Player p in ais)
                    {
                        if (p == repliedPlayer)
                        {
                            card = ai_cards[p];
                            break;
                        }
                    }
                }
                else
                {
                    Interactivity client = GetInteractivity(repliedPlayer);
                    List<string> clientReply = client?.ClientReply;
                    if (clientReply != null && clientReply.Count > 1)
                    {
                        //OutPut(string.Format("{0},{1}", clientReply[0], clientReply[1]));
                        card = RoomLogic.ParseCard(this, clientReply[1]);
                        repliedPlayer = FindPlayer(clientReply[0], true);
                    }
                }
            }

            DoBroadcastNotify(CommandType.S_COMMAND_UNKNOWN, new List<string> { false.ToString() });

            if (card == null || repliedPlayer == null) return false;
            FunctionCard fcard = Engine.GetFunctionCard(card.Name);
            card = fcard != null ? fcard.ValidateInResponse(this, repliedPlayer, card) : card;

            if (card == null)
                return _AskForNullification(trick, from, to, positive, helper);
            if (to != null)
                DoAnimate(AnimateType.S_ANIMATE_NULLIFICATION, repliedPlayer.Name, to.Name);

            CardUseStruct use = new CardUseStruct(card, repliedPlayer, new List<Player>())
            {
                Reason = CardUseStruct.CardUseReason.CARD_USE_REASON_RESPONSE_USE,
                RespondData = new UseRespond(from, helper.StackPlayers??new List<Player> { to }, trick),
                IsHandcard = true
            };
            if (to != null && positive && !trick.Name.Contains(Nullification.ClassName))
                use.To.Add(to);
            else if (!positive || trick.Name.Contains(Nullification.ClassName))
                use.To.Add(from);
            
            List<int> subcards = new List<int>(card.SubCards);
            if (subcards.Count > 0)
            {
                foreach (int id in subcards)
                {
                    if (GetCardOwner(id) != repliedPlayer || GetCardPlace(id) != Place.PlaceHand)
                    {
                        use.IsHandcard = false;
                        break;
                    }
                }
            }
            else
                use.IsHandcard = false;

            if (use.From != helper.To)          //为别人打无懈辅助+1
            {
                ResultStruct assist = repliedPlayer.Result;
                assist.Assist++;
                repliedPlayer.Result = assist;
            }

            UseCard(use);

            object decisionData = string.Format("Nullification:{0}:{1}:{2}:{3}", trick.Name, helper.From != null ? helper.From.Name : string.Empty, to.Name, positive ? "true" : "false");
            RoomThread.Trigger(TriggerEvent.ChoiceMade, this, repliedPlayer, ref decisionData);

            object use_data = use;
            if (RoomThread.Trigger(TriggerEvent.NullificationEffect, this, repliedPlayer, ref use_data))
                return _AskForNullification(trick, from, to, positive, helper);

            bool isHegNullification = NullTimes == 0 && HegNull;
            NullTimes++;

            bool result = true;
            CardEffectStruct effect = new CardEffectStruct
            {
                Card = card,
                To = repliedPlayer
            };
            if (fcard.IsCancelable(this, effect))
                result = !_AskForNullification(card, repliedPlayer, to, !positive, helper);

            if (isHegNullification && result)
                HegNullEffected = true;

            return result;
        }
        public int AskForCardChosen(Player player, Player who, string flags, string reason, bool handcard_visible = false,
            HandlingMethod method = HandlingMethod.MethodNone, List<int> disabled_ids = null)
        {
            //tryPause();
            NotifyMoveFocus(player, CommandType.S_COMMAND_CHOOSE_CARD);
            Thread.Sleep(300);

            List<int> disabled_ids_copy = disabled_ids != null ? new List<int>(disabled_ids) : new List<int>();
            string flags_copy = flags;
            List<int> card_ids = who.GetCards(flags);
            if (method == HandlingMethod.MethodDiscard)
            {
                foreach (int id in card_ids)
                    if (!RoomLogic.CanDiscard(this, player, who, id))
                        disabled_ids_copy.Add(id);
                if (flags_copy.Contains("h") && !RoomLogic.CanDiscard(this, player, who, "h"))
                    flags_copy = flags_copy.Replace("h", string.Empty);
                if (flags_copy.Contains("e") && !RoomLogic.CanDiscard(this, player, who, "e"))
                    flags_copy = flags_copy.Replace("e", string.Empty);
                if (flags_copy.Contains("j") && !RoomLogic.CanDiscard(this, player, who, "j"))
                    flags_copy = flags_copy.Replace("j", string.Empty);
            }
            if (method == HandlingMethod.MethodGet)
            {
                foreach (int id in card_ids)
                    if (!RoomLogic.CanGetCard(this, player, who, id))
                        disabled_ids_copy.Add(id);
                if (flags_copy.Contains("h") && !RoomLogic.CanGetCard(this, player, who, "h"))
                    flags_copy = flags_copy.Replace("h", string.Empty);
                if (flags_copy.Contains("e") && !RoomLogic.CanGetCard(this, player, who, "e"))
                    flags_copy = flags_copy.Replace("e", string.Empty);
                if (flags_copy.Contains("j") && !RoomLogic.CanGetCard(this, player, who, "j"))
                    flags_copy = flags_copy.Replace("j", string.Empty);
            }

            List<int> available = who.GetCards(flags_copy);
            foreach (int id in disabled_ids_copy)
            {
                if (available.Contains(id))
                    available.Remove(id);
            }
            if (available.Count == 0) return -1;
            if (handcard_visible && !who.IsKongcheng())
            {
                List<int> handcards = who.GetCards("h");
                List<string> arg = new List<string> { who.Name, JsonUntity.Object2Json(handcards) };
                DoNotify(GetClient(player), CommandType.S_COMMAND_SET_KNOWN_CARDS, arg);
            }
            int card_id = -1;
            if (who != player && !handcard_visible && GetAI(player) != null
                && (flags_copy == "h" || (flags_copy == "he" && !who.HasEquip()) || (flags_copy == "hej" && !who.HasEquip() && who.JudgingArea.Count == 0)))
            {
                List<int> handcards = who.GetCards("h");
                foreach (int id in disabled_ids_copy)
                    handcards.Remove(id);

                Shuffle.shuffle(ref handcards);
                card_id = handcards[0];
            }
            else
            {
                TrustedAI ai = GetAI(player);
                if (ai != null)
                {
                    Thread.Sleep(500);
                    card_id = ai.AskForCardChosen(who, flags_copy, reason.Split('%')[0], method, disabled_ids_copy);
                    if (card_id == -1)
                    {
                        List<int> cards = who.GetCards(flags_copy);
                        foreach (int id in who.GetCards(flags_copy))
                            if (disabled_ids_copy.Contains(id))
                                cards.Remove(id);

                        Shuffle.shuffle(ref cards);
                        card_id = cards[0];
                    }
                }
                else
                {
                    List<int> handcards;
                    if (who.HasFlag("continuous_card_chosen"))
                    {
                        handcards = (List<int>)GetTag("askforCardsChosen");
                    }
                    else
                    {
                        handcards = who.GetCards("h");
                        Shuffle.shuffle(ref handcards);
                    }

                    List<string> disable_list = new List<string>();
                    foreach (int id in disabled_ids_copy)
                    {
                        if (GetCardPlace(id) != Place.PlaceHand || handcard_visible)
                            disable_list.Add(id.ToString());
                        else
                            disable_list.Add("*" + handcards.IndexOf(id).ToString());
                    }
                    List<string> arg = new List<string> { player.Name, who.Name, flags_copy, reason, handcard_visible.ToString(), JsonUntity.Object2Json(disable_list) };
                    bool success = DoRequest(player, CommandType.S_COMMAND_CHOOSE_CARD, arg, true);

                    //@todo: check if the card returned is valid
                    Interactivity client = GetInteractivity(player);
                    List<string> clientReply = client?.ClientReply;
                    if (!success || clientReply == null || clientReply.Count != 2)
                    {
                        // randomly choose a card
                        List<int> cards = who.GetCards(flags_copy);
                        foreach (int id in who.GetCards(flags_copy))
                            if (disabled_ids_copy.Contains(id))
                                cards.Remove(id);

                        Shuffle.shuffle<int>(ref cards);
                        card_id = cards[0];
                    }
                    else
                    {
                        card_id = int.Parse(clientReply[0]);
                        int index = int.Parse(clientReply[1]);

                        if (card_id == -1)
                            card_id = handcards[index];
                    }
                }
            }
            DoBroadcastNotify(CommandType.S_COMMAND_UNKNOWN, new List<string> { false.ToString() });

            object decisionData = string.Format("cardChosen:{0}:{1}:{2}:{3}", reason, card_id, player.Name, who.Name);
            if (!who.HasFlag("continuous_card_chosen"))
                RoomThread.Trigger(TriggerEvent.ChoiceMade, this, player, ref decisionData);
            return card_id;
        }

        public List<int> AskForCardsChosen(Player chooser, Player choosee, List<string> handle_list, string reason)
        {
            List<int> result = new List<int>();
            choosee.SetFlags("continuous_card_chosen");

            List<int> handcard_ids = choosee.GetCards("h");
            Shuffle.shuffle<int>(ref handcard_ids);
            SetTag("askforCardsChosen", handcard_ids);

            if (chooser != null && chooser.Alive && choosee != null && choosee.Alive && !choosee.IsAllNude())
            {
                foreach (string src in handle_list)
                {
                    if (string.IsNullOrEmpty(src)) continue;
                    List<string> handle = new List<string>(src.Split('^'));
                    if (string.IsNullOrEmpty(handle[0])) continue;
                    if (choosee.GetCards(handle[0]).Count == 0) continue;
                    if (handle.Count == 1) handle.Add("false");
                    if (handle.Count == 2) handle.Add("none");
                    List<int> ids = new List<int>(result);
                    if (handle.Count > 3)
                    {
                        foreach (string id_str in handle[3].Split('+'))
                            ids.Append(int.Parse(id_str));
                    }
                    int id = AskForCardChosen(chooser, choosee, handle[0], reason, handle[1] == "true",
                        Engine.GetCardHandlingMethod(handle[2]), ids);
                    if (id != -1)
                        result.Add(id);
                }
            }
            choosee.SetFlags("-continuous_card_chosen");
            RemoveTag("askforCardsChosen");

            object decisionData = string.Format("cardChosen:{0}:{1}:{2}:{3}", reason, string.Join("+", JsonUntity.IntList2StringList(result)),
                chooser.Name, choosee.Name);
            RoomThread.Trigger(TriggerEvent.ChoiceMade, this, chooser, ref decisionData);

            return result;
        }

        public List<WrappedCard> AskForCardsChosen(Player chooser, Player choosee, string handle_string, string reason)
        {
            List<int> value = AskForCardsChosen(chooser, choosee, new List<string>(handle_string.Split('|')), reason);
            List<WrappedCard> result = new List<WrappedCard>();
            foreach (int id in value)
                result.Add(GetCard(id));
            return result;
        }
        public int AskForCardShow(Player player, Player requestor, string reason, object data = null)
        {
            //tryPause();
            Thread.Sleep(300);
            NotifyMoveFocus(player, CommandType.S_COMMAND_SHOW_CARD);
            int card_id = -1;
            if (player.HandcardNum == 1)
                card_id = player.GetCards("h")[0];
            _m_roomState.SetCurrentCardUsePattern(".");
            _m_roomState.SetCurrentCardUseReason(CardUseStruct.CardUseReason.CARD_USE_REASON_UNKNOWN);

            if (card_id == -1)
            {
                TrustedAI ai = GetAI(player);
                if (ai != null)
                {
                    card_id = ai.AskForCardShow(requestor, reason, data).Id;
                }
                else
                {
                    Interactivity client = GetInteractivity(player);
                    bool success = client != null && client.ShowCardRequest(this, player, requestor);
                    List<string> clientReply = client?.ClientReply;
                    if (!success || clientReply == null || clientReply.Count == 0 || !int.TryParse(clientReply[1], out card_id))
                    {
                        card_id = -1;
                    }
                }
            }
            DoBroadcastNotify(CommandType.S_COMMAND_UNKNOWN, new List<string> { false.ToString() });

            if (card_id == -1)
                card_id = GetRandomHandCard(player);

            object decisionData = string.Format("cardShow:{0}:_{1}_", reason, card_id);
            RoomThread.Trigger(TriggerEvent.ChoiceMade, this, player, ref decisionData);
            return card_id;
        }

        public int GetRandomHandCard(Player player)
        {
            List<int> ids= player.GetCards("h");
            Shuffle.shuffle<int>(ref ids);
            return ids[0];
        }

        public void ShowCards(Player player, List<int> card_ids, string reason, Player only_viewer = null)
        {

            List<string> show_arg = new List<string> { player.Name, JsonUntity.Object2Json(card_ids), reason };
            
            if (only_viewer != null)
            {
                List<Client> players = new List<Client> { GetClient(only_viewer) };
                if (!players.Contains(GetClient(player)))
                    players.Add(GetClient(player));

                DoBroadcastNotify(players, CommandType.S_COMMAND_SHOW_CARD, show_arg);
            }
            else
            {
                foreach (int card_id in card_ids)
                    if (card_id >= 0)
                        GetCard(card_id).SetFlags("visible");

                DoBroadcastNotify(CommandType.S_COMMAND_SHOW_CARD, show_arg);
            }

            object decisionData = string.Format("showCards:{0}:{1}", string.Join("+", JsonUntity.IntList2StringList(card_ids)), only_viewer != null ? only_viewer.Name : "all");
            RoomThread.Trigger(TriggerEvent.ChoiceMade, this, player, ref decisionData);
        }

        public void ShowCard(Player player, int card_id, string reason, Player only_viewer = null)
        {
            if (GetCardOwner(card_id) != player) return;

            //tryPause();
            Thread.Sleep(300);
            List<int> ids = new List<int> { card_id };
            ShowCards(player, ids, reason, only_viewer);
        }

        public void ShowAllCards(Player player, Player to = null, string reason = null, string position = null)
        {
            if (player.IsKongcheng())
                return;
            //tryPause();
            Thread.Sleep(300);

            bool isUnicast = (to != null);
            if (isUnicast)
            {
                NotifyMoveFocus(to, CommandType.S_COMMAND_SKILL_GONGXIN);

                LogMessage log = new LogMessage
                {
                    Type = "$ViewAllCards",
                    From = to.Name,
                    To = new List<string> { player.Name },
                    Card_str = string.Join("+", JsonUntity.IntList2StringList(player.HandCards))
                };
                SendLog(log, to);

                LogMessage log2 = new LogMessage
                {
                    Type = "#KnownBothView",
                    From = to.Name,
                    To = new List<string> { player.Name },
                    Arg = "handcards"
                };
                SendLog(log2, new List<Player> { to });

                List<string> gongxinArgs = new List<string> { to.Name, player.Name, true.ToString(), JsonUntity.Object2Json(player.HandCards), string.Empty, reason, string.Empty, position };
                if (to.Status == "trust")
                {
                    DoNotify(GetClient(to), CommandType.S_COMMAND_SHOW_CARD, new List<string> { player.Name, JsonUntity.Object2Json(player.HandCards), reason });
                    Thread.Sleep(3000);
                }
                else
                    DoRequest(to, CommandType.S_COMMAND_SKILL_GONGXIN, gongxinArgs, true);

                DoBroadcastNotify(CommandType.S_COMMAND_UNKNOWN, new List<string> { false.ToString() });
            }
            else
            {
                List<string> gongxinArgs = new List<string> { player.Name, JsonUntity.Object2Json(player.HandCards), reason };
                LogMessage log = new LogMessage
                {
                    Type = "$ShowAllCards",
                    From = player.Name,
                    Card_str = string.Join("+", JsonUntity.IntList2StringList(player.HandCards))
                };
                foreach (int card_id in player.GetCards("h"))
                    GetCard(card_id).SetFlags("visible");

                SendLog(log);

                DoBroadcastNotify(CommandType.S_COMMAND_SHOW_CARD, gongxinArgs);
                FocusAll(3000);
            }

            object decisionData = string.Format("viewCards:{0}:all", to != null ? to.Name : "all");
            RoomThread.Trigger(TriggerEvent.ChoiceMade, this, player, ref decisionData);
        }

        public void ViewCards(Player player, List<int> cards, string reason = null, string prompt = null)
        {
            if (cards.Count == 0) return;
            //tryPause();
            Thread.Sleep(300);

            NotifyMoveFocus(player, CommandType.S_COMMAND_SKILL_GONGXIN);

            List<string> gongxinArgs = new List<string> { player.Name, string.Empty, true.ToString(), JsonUntity.Object2Json(cards), string.Empty, reason, prompt, string.Empty };
            if (player.Status == "trust")
            {
                DoNotify(GetClient(player), CommandType.S_COMMAND_SHOW_CARD, new List<string> { string.Empty, JsonUntity.Object2Json(cards), reason });
                Thread.Sleep(3000);
            }
            else
                DoRequest(player, CommandType.S_COMMAND_SKILL_GONGXIN, gongxinArgs, true);

            DoBroadcastNotify(CommandType.S_COMMAND_UNKNOWN, new List<string> { false.ToString() });
        }

        public void ViewGenerals(Player player, List<string> names, string reason = null, string position = null)
        {
            NotifyMoveFocus(player, CommandType.S_COMMAND_VIEW_GENERALS);
            List<string> gongxinArgs = new List<string> { reason, JsonUntity.Object2Json(names), reason, position };
            if (player.Status == "trust")
            {
                gongxinArgs.Add("false");
                DoNotify(GetClient(player), CommandType.S_COMMAND_VIEW_GENERALS, gongxinArgs);
                Thread.Sleep(3000);
            }
            else
            {
                gongxinArgs.Add("true");
                DoRequest(player, CommandType.S_COMMAND_VIEW_GENERALS, gongxinArgs, true);
            }

            DoBroadcastNotify(CommandType.S_COMMAND_UNKNOWN, new List<string> { false.ToString() });
        }

        public List<Player> AskForExtraTargets(Player player, List<Player> selected_targets,
            WrappedCard card, string skillName, string prompt, bool notify_skill, string position = null)
        {
            //tryPause();
            Thread.Sleep(300);
            NotifyMoveFocus(player, CommandType.S_COMMAND_CHOOSE_EXTRA_TARGET);

            TrustedAI ai = GetAI(player);
            List<Player> result = new List<Player>();
            if (ai != null)
            {
                result = ai.AskForPlayersChosen(GetAlivePlayers(), skillName, 1, 0);
                if (result.Count > 0)
                    Thread.Sleep(400);
            }
            else
            {
                Interactivity client = GetInteractivity(player);
                bool success = client != null && client.ExtraRequest(this, player, selected_targets, card, prompt, skillName, position);
                List<string> clientReply = client?.ClientReply;
                if (success && clientReply != null && clientReply.Count > 0)
                    result = RoomLogic.ParsePlayers(this, JsonUntity.Json2List<string>(clientReply[0]));
            }
            DoBroadcastNotify(CommandType.S_COMMAND_UNKNOWN, new List<string> { false.ToString() });

            if (result.Count > 0 && notify_skill)
            {
                foreach (Player p in result)
                    DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, p.Name);

                string main = Engine.GetMainSkill(skillName).Name;
                NotifySkillInvoked(player, main);
                LogMessage log = new LogMessage
                {
                    Type = "$extra_target",
                    From = player.Name,
                    To = new List<string>(),
                    Card_str = RoomLogic.CardToString(this, card),
                    Arg = main
                };
                foreach (Player p in result)
                    log.To.Add(p.Name);
                SendLog(log);

                ShowSkill(player, main, position);
                TargetModSkill skill = (TargetModSkill)Engine.GetSkill(skillName);
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(this, player, skillName, position);
                string _skill_name = main;
                string general = gsk.General;
                int skin_id = gsk.SkinId;
                int index = -1;
                skill.GetEffectIndex(this, player, card, TargetModSkill.ModType.ExtraTarget, ref index, ref _skill_name, ref general, ref skin_id);
                if (index != -2)
                    BroadcastSkillInvoke(_skill_name, "male", index, general, skin_id);
            }
            return result;
        }

        public bool AskForExCollateral(Player player, ref CardUseStruct use)
        {
            SetTag("Current_Collateral", use);
            WrappedCard card = AskForUseCard(player, "@@CollateralEx", "@Collateral", null);
            bool invoke = false;
            if (card != null && GetTag("CollateralEx") is Player target)
            {
                DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, target.Name);
                DoAnimate(AnimateType.S_ANIMATE_INDICATE, target.Name, (string)target.GetTag("collateralVictim"));
                use.To.Add(target);
                SortByActionOrder(ref use);
                RemoveTag("CollateralEx");
                invoke = true;
            }
            RemoveTag("Current_Collateral");
            return invoke;
        }

        public Player AskForPlayerChosen(Player player, List<Player> _targets, string skillName,
            string prompt = null, bool optional = false, bool notify_skill = false, string position = null)
        {
            List<Player> targets = new List<Player>(_targets);
            Player choice = null;
            if (targets.Count == 0)
            {
                return null;
            }
            else if (targets.Count == 1 && !optional)
            {
                choice = targets[0];
            }

            //tryPause();
            Thread.Sleep(300);
            NotifyMoveFocus(player, CommandType.S_COMMAND_CHOOSE_PLAYER);
            if (choice == null)
            {
                TrustedAI ai = GetAI(player);
                if (ai != null)
                {
                    List<Player> result = ai.AskForPlayersChosen(targets, skillName, 1, optional ? 0 : 1);
                    if (result.Count > 0)
                        choice = result[0];
                    if (choice != null && notify_skill)
                        Thread.Sleep(400);
                }
                else
                {
                    Interactivity client = GetInteractivity(player);
                    bool success = client != null && client.ChooseRequest(this, player, targets, prompt, skillName, position, 1, optional ? 0 : 1);
                    List<string> clientReply = client?.ClientReply;
                    if (success && clientReply != null && clientReply.Count > 0)
                    {
                        List<Player> result = RoomLogic.ParsePlayers(this, JsonUntity.Json2List<string>(clientReply[0]));
                        if (result.Count == 1)
                            choice = result[0];
                    }
                }
                DoBroadcastNotify(CommandType.S_COMMAND_UNKNOWN, new List<string> { false.ToString() });
                if (choice != null && !targets.Contains(choice))
                    choice = null;
                if (choice == null && !optional)
                {
                    Shuffle.shuffle(ref targets);
                    choice = targets[0];
                }
            }

            if (choice != null)
            {
                if (notify_skill)
                {
                    NotifySkillInvoked(player, skillName);
                    DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, choice.Name);
                    LogMessage log = new LogMessage
                    {
                        Type = "#ChoosePlayerWithSkill",
                        From = player.Name,
                        To = new List<string> { choice.Name },
                        Arg = skillName
                    };
                    SendLog(log);
                }
                object data = string.Format("{0}:{1}:{2}", "playerChosen", skillName, choice.Name);
                RoomThread.Trigger(TriggerEvent.ChoiceMade, this, player, ref data);
            }
            return choice;
        }

        public List<Player> AskForPlayersChosen(Player player, List<Player> targets,
                            string skillName, int min_num = 0, int max_num = 2, string prompt = null, bool notify_skill = false, string position = null)
        {
            if (targets.Count <= min_num)
            {
                List<string> names = new List<string>();
                foreach (Player p in targets)
                    names.Add(p.Name);
                object data = string.Format("{0}:{1}:{2}", "playerChosen", skillName, string.Join("+", names));
                RoomThread.Trigger(TriggerEvent.ChoiceMade, this, player, ref data);
                return targets;
            }

            //tryPause();
            Thread.Sleep(300);
            min_num = Math.Min(min_num, targets.Count);
            max_num = Math.Min(max_num, targets.Count);
            NotifyMoveFocus(player, CommandType.S_COMMAND_CHOOSE_PLAYER);
            TrustedAI ai = GetAI(player);
            List<Player> result = new List<Player>();
            if (ai != null)
            {
                result = ai.AskForPlayersChosen(targets, skillName, max_num, min_num);
                if (result.Count > 0 && notify_skill)
                    Thread.Sleep(400);
            }
            else
            {
                Interactivity client = GetInteractivity(player);
                bool success = client != null && client.ChooseRequest(this, player, targets, prompt, skillName, position, max_num, min_num);
                List<string> clientReply = client?.ClientReply;
                if (success && clientReply != null && clientReply.Count > 0)
                {
                    result = RoomLogic.ParsePlayers(this, JsonUntity.Json2List<string>(clientReply[0]));
                }
            }
            DoBroadcastNotify(CommandType.S_COMMAND_UNKNOWN, new List<string> { false.ToString() });
            if (result.Count < min_num)
            {
                List<Player> copy = new List<Player>(targets);
                foreach (Player p in result)
                    copy.Remove(p);
                while (result.Count < min_num)
                {
                    Shuffle.shuffle(ref copy);
                    result.Add(copy[0]);
                    copy.RemoveAt(0);
                }
            }
            if (result.Count > 0)
            {
                if (notify_skill)
                {
                    NotifySkillInvoked(player, skillName); object decisionData = string.Format("skillInvoke:{0}:yes", skillName);
                    RoomThread.Trigger(TriggerEvent.ChoiceMade, this, player, ref decisionData);

                    LogMessage log = new LogMessage
                    {
                        Type = "#ChoosePlayerWithSkill",
                        From = player.Name,
                        To = new List<string>(),
                        Arg = skillName
                    };
                    foreach (Player choice in result)
                    {
                        DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, choice.Name);
                        log.To.Add(choice.Name);
                    }
                    SendLog(log);
                }
                List<string> names = new List<string>();
                foreach (Player p in result)
                    names.Add(p.Name);
                object data = string.Format("{0}:{1}:{2}", "playerChosen", skillName, string.Join("+", names));
                RoomThread.Trigger(TriggerEvent.ChoiceMade, this, player, ref data);
            }
            return result;
        }
        public Player GetExtraTurn()
        {
            if (m_extra_turn.Count > 0) return m_extra_turn.Dequeue();

            return null;
        }
        public void GainAnExtraTurn(Player player) => m_extra_turn.Enqueue(player);

        public string AskForGeneral(Player player, List<string> generals, string default_choice = null, bool single_result = true,
                            string skill_name = null, object data = null, bool can_convert = false, bool assign_kingdom = false)
        {
            Thread.Sleep(300);
            NotifyMoveFocus(player, CommandType.S_COMMAND_CHOOSE_GENERAL);

            if (generals.Count == 1)
                return generals[0];

            if (!single_result && generals.Count == 2)
                return string.Join("+", generals);


            if (string.IsNullOrEmpty(default_choice))
            {
                Shuffle.shuffle(ref generals);
                default_choice = generals[0];

                if (!single_result)
                {
                    List<string> heros = generals;
                    bool good = false;
                    foreach (string name1 in heros)
                    {
                        foreach (string name2 in heros)
                        {
                            if (name1 != name2 && Engine.GetGeneral(name1, Setting.GameMode).Kingdom == Engine.GetGeneral(name2, Setting.GameMode).Kingdom)
                            {
                                default_choice = name1 + "+" + name2;
                                good = true;
                                break;
                            }
                        }
                        if (good) break;
                    }
                }
            }

            TrustedAI ai = GetAI(player);
            if (ai != null && !string.IsNullOrEmpty(skill_name))
            {
                List<string> general = new List<string>(ai.AskForChoice(skill_name.Split(':')[0], string.Join("+", generals), data).Split('+'));
                Thread.Sleep(400);
                bool check = true;
                if (!single_result && general.Count != 2) check = false;
                if (single_result && general.Count != 1) check = false;
                foreach (string name in general)
                {
                    if (!generals.Contains(name))
                    {
                        check = false;
                        break;
                    }
                }
                if (check) default_choice = string.Join("+", general);
            }
            else
            {
                List<string> options = new List<string> {
                    player.Name,
                    skill_name,
                    JsonUntity.Object2Json(generals),
                    single_result.ToString(),
                    can_convert.ToString(),
                    assign_kingdom.ToString()
                };
                bool success = DoRequest(player, CommandType.S_COMMAND_CHOOSE_GENERAL, options, true);

                Interactivity client = GetInteractivity(player);
                List<string> clientResponse = client?.ClientReply;
                if (clientResponse != null && clientResponse.Count > 0)
                {
                    List<string> answer = new List<string>(clientResponse[0].Split('+'));
                    bool valid = true;
                    bool cheat = false;
                    foreach (string name in answer)
                    {
                        if (skill_name == "gamerule" && !generals.Contains(name))
                        {
                            if (GetClient(player).UserRight < 3)
                            {
                                valid = false;
                                break;
                            }
                            else
                            {
                                cheat = true;
                            }
                        }
                        else if (skill_name != "gamerule" && !generals.Contains(name))
                        {
                            valid = false;
                            break;
                        }
                    }

                    if (cheat)
                    {
                        LogMessage log = new LogMessage();
                        log.Type = "#cheat_pick";
                        log.From = player.Name;
                        SendLog(log);
                    }

                    if (success && valid)
                        default_choice = clientResponse[0];
                }
            }
            DoBroadcastNotify(CommandType.S_COMMAND_UNKNOWN, new List<string> { false.ToString() });

            return default_choice;
        }

        public string AskforGeneral(Player player, List<string> _generals, string reason, bool option, string prompt, object data, string position = null)
        {
            List<string> generals = new List<string>(_generals);
            string choice = string.Empty;

            Thread.Sleep(300);
            NotifyMoveFocus(player, CommandType.S_COMMAND_SELECT_GENERAL);

            if (generals.Count == 1 && !option)
                return generals[0];

            TrustedAI ai = GetAI(player);
            if (ai != null)
            {
                generals.Add("cancel");
                string general = ai.AskForChoice(reason, string.Join("+", generals), data);
                Thread.Sleep(400);
                if (generals.Contains(general) && general != "cancel")
                    choice = general;
            }
            else
            {
                List<string> options = new List<string>
                {
                    player.Name,
                    reason,
                    JsonUntity.Object2Json(generals),
                    option.ToString(),
                    prompt,
                    position
                };
                bool success = DoRequest(player, CommandType.S_COMMAND_SELECT_GENERAL, options, true);

                Interactivity client = GetInteractivity(player);
                List<string> clientResponse = client?.ClientReply;
                if (clientResponse != null && clientResponse.Count > 0)
                {
                    string answer = clientResponse[0];
                    if (generals.Contains(answer)) choice = answer;
                }
            }
            DoBroadcastNotify(CommandType.S_COMMAND_UNKNOWN, new List<string> { false.ToString() });

            if (!option && string.IsNullOrEmpty(choice))
                choice = generals[0];

            return choice;
        }

        /*
        public void DoDragonPhoenix(Player player, string general1_name, string general2_name,
            bool full_state = true, string kingdom = null, bool sendLog = true, string show_flags = null, bool resetHp = false)
        {
            List<string> names = new List<string> { player.ActualGeneral1, player.ActualGeneral2 };
            List<string> names_orig = new List<string>(names);
            names_orig.RemoveAll(n => n == "sujiang");
            names_orig.RemoveAll(n => n == "sujiangf");
            if (player.Alive)
                return;
            ThrowAllHandCardsAndEquips(player);
            if (!string.IsNullOrEmpty(player.General1))
                RemoveGeneral(player, true);
            if (!string.IsNullOrEmpty(player.General2))
                RemoveGeneral(player, false);
            ThrowAllMarks(player);          // necessary.

            object void_data = null;
            List<TriggerSkill> game_start = new List<TriggerSkill>();

            List<string> duanchang = new List<string>(player.DuanChang.Split(','));
            int max_hp = 0;
            if (!string.IsNullOrEmpty(general1_name))
            {
                HandleUsedGeneral(general1_name);
                if (duanchang.Contains("head"))
                    duanchang.Remove("head");

                List<string> arg = new List<string> { GameEventType.S_GAME_EVENT_CHANGE_HERO.ToString(), player.Name, general1_name, false.ToString(), false.ToString() };
                DoBroadcastNotify(CommandType.S_COMMAND_LOG_EVENT, arg);

                foreach (string skill_name in Engine.GetGeneralSkills(general1_name, Setting.GameMode, true))
                {
                    Skill skill = Engine.GetSkill(skill_name);
                    if (skill is TriggerSkill tr)
                    {
                        if (tr != null)
                        {
                            if (tr.TriggerEvents.Contains(TriggerEvent.GameStart) && tr.Triggerable(TriggerEvent.GameStart, this, player, ref void_data).Count > 0)
                                game_start.Add(tr);
                        }
                    }
                    AddPlayerSkill(player, skill_name, true);
                }

                ChangePlayerGeneral(player, "anjiang");
                player.ActualGeneral1 = general1_name;
                player.Kingdom = Engine.GetGeneral(general1_name, Setting.GameMode).Kingdom;
                NotifyProperty(GetClient(player), player, "ActualGeneral1");
                NotifyProperty(GetClient(player), player, "General1");

                max_hp += Engine.GetGeneral(general1_name, Setting.GameMode).GetMaxHpHead();
                names[0] = general1_name;
                player.General1Showed = false;
                BroadcastProperty(player, "General1Showed");
            }
            if (!string.IsNullOrEmpty(general2_name))
            {
                HandleUsedGeneral(general2_name);
                if (duanchang.Contains("deputy"))
                    duanchang.Remove("deputy");

                List<string> arg = new List<string> { GameEventType.S_GAME_EVENT_CHANGE_HERO.ToString(), player.Name, general2_name, true.ToString(), false.ToString() };
                DoBroadcastNotify(CommandType.S_COMMAND_LOG_EVENT, arg);

                foreach (string skill_name in Engine.GetGeneralSkills(general2_name, Setting.GameMode, false))
                {
                    Skill skill = Engine.GetSkill(skill_name);
                    if (skill is TriggerSkill tr)
                    {
                        if (tr != null)
                        {
                            if (tr.TriggerEvents.Contains(TriggerEvent.GameStart) && tr.Triggerable(TriggerEvent.GameStart, this, player, ref void_data).Count > 0)
                                game_start.Add(tr);
                        }
                    }
                    AddPlayerSkill(player, skill_name, false);
                }

                ChangePlayerGeneral2(player, "anjiang");
                player.ActualGeneral2 = general2_name;
                if (string.IsNullOrEmpty(general1_name))
                    player.Kingdom = Engine.GetGeneral(general2_name, Setting.GameMode).Kingdom;
                NotifyProperty(GetClient(player), player, "ActualGeneral2");
                NotifyProperty(GetClient(player), player, "General2");

                max_hp += Engine.GetGeneral(general2_name, Setting.GameMode).GetMaxHpDeputy();
                names[1] = general2_name;
                player.General2Showed = false;
                BroadcastProperty(player, "General2Showed");
            }

            player.DuanChang = string.Join(",", duanchang);
            BroadcastProperty(player, "DuanChang");

            RevivePlayer(player);

            player.Hp = 1;
            if (resetHp)
            {
                if (string.IsNullOrEmpty(general1_name) || string.IsNullOrEmpty(general2_name))
                    max_hp *= 2;
                player.SetMark("HalfMaxHpLeft", max_hp % 2);
                player.MaxHp = max_hp / 2;
                BroadcastProperty(player, "MaxHp");
                player.Hp = player.MaxHp;
            }
            BroadcastProperty(player, "Hp");
            player.SetFlags("Global_DFDebut");

            if (string.IsNullOrEmpty(show_flags))
                NotifyProperty(GetClient(player), player, "Kingdom");
            else
                BroadcastProperty(player, "Kingdom");
            string role = Engine.GetMappedRole(string.IsNullOrEmpty(kingdom) ? Engine.GetGeneral(general1_name, Setting.GameMode).Kingdom : kingdom);
            player.Role = role;
            if (string.IsNullOrEmpty(show_flags))
                NotifyProperty(GetClient(player), player, "Role");
            else
                BroadcastProperty(player, "Role");

            foreach (string skill_name in player.GetSkills())
            {
                Skill skill = Engine.GetSkill(skill_name);
                if (skill.SkillFrequency == Frequency.Limited && !string.IsNullOrEmpty(skill.LimitMark))
                {
                    player.SetMark(skill.LimitMark, 1);
                    List<string> arg = new List<string> { player.Name, skill.LimitMark, "1" };
                    DoNotify(GetClient(player), CommandType.S_COMMAND_SET_MARK, arg);
                }
            }

            foreach (TriggerSkill skill in game_start)
            {
                if (skill.Cost(TriggerEvent.GameStart, this, player, ref void_data, player, new TriggerStruct(skill.Name, player)).SkillName == skill.Name)
                    skill.Effect(TriggerEvent.GameStart, this, player, ref void_data, player, new TriggerStruct());
            }

            if (full_state)
            {
                SetPlayerChained(player, false, false);
                player.FaceUp = true;
                BroadcastProperty(player, "FaceUp");
                if (Engine.GetGeneral(general1_name, Setting.GameMode).CompanionWith(general2_name))
                    SetPlayerMark(player, "CompanionEffect", 1);
            }

            if (sendLog)
            {
                LogMessage l = new LogMessage
                {
                    Type = "#doDragonPhoenix",
                    From = player.Name
                };
                SendLog(l);
            }

            //ResetAI(player);
            player.SetSkillsPreshowed();

            if (show_flags.Contains("h"))
                ShowGeneral(player, true, false, false);
            if (show_flags.Contains("d"))
                ShowGeneral(player, false, false, false);
        }
        */
        public void RevivePlayer(Player player)
        {
            player.Alive = true;
            ThrowAllMarks(player, false);
            BroadcastProperty(player, "Alive");
            //setEmotion(player, "revive");

            _alivePlayers.Clear();
            foreach (Player p in m_players)
            {
                if (p.Alive)
                    _alivePlayers.Add(p);
            }

            DoBroadcastNotify(CommandType.S_COMMAND_REVIVE_PLAYER, new List<string> { player.Name });
            UpdateStateItem();

            List<string> new_big = RoomLogic.GetBigKingdoms(this);
            DoBroadcastNotify(CommandType.S_COMMAND_LOG_EVENT, new List<string> { GameEventType.S_GAME_EVENT_BIG_KINGDOM.ToString(), JsonUntity.Object2Json(new_big) });
        }

        public void RemoveGeneral(Player player, bool head_general = true)
        {
            string general_name, from_general;

            //room->tryPause();
            Thread.Sleep(300);
            //SetEmotion(player, "remove");

            object _head = head_general;
            if (head_general)
            {
                if (!player.General1Showed)
                    ShowGeneral(player, true, false, false);   //zoushi?

                from_general = player.ActualGeneral1;
                if (from_general.Contains("sujiang")) return;
                RoomThread.Trigger(TriggerEvent.GeneralStartRemove, this, player, ref _head);

                Gender gender = Engine.GetGeneral(from_general, Setting.GameMode).GeneralGender;
                general_name = gender == Gender.Male ? "sujiang" : "sujiangf";
                player.ActualGeneral1 = player.General1 = general_name;
                player.HeadSkinId = 0;
                DoAnimate(AnimateType.S_ANIMATE_REMOVE, player.Name, true.ToString());

                DisconnectSkillsFromOthers(player, true, false);

                foreach (string skill_name in player.GetHeadSkillList())
                {
                    Skill skill = Engine.GetSkill(skill_name);
                    if (skill != null)
                        DetachSkillFromPlayer(player, skill.Name, false, false, true);
                }
            }
            else
            {
                if (!player.General2Showed)
                    ShowGeneral(player, false, false, false); //zoushi?

                from_general = player.ActualGeneral2;
                if (from_general.Contains("sujiang")) return;

                RoomThread.Trigger(TriggerEvent.GeneralStartRemove, this, player, ref _head);
                Gender gender = Engine.GetGeneral(from_general, Setting.GameMode).GeneralGender;
                general_name = gender == Gender.Male ? "sujiang" : "sujiangf";
                player.ActualGeneral2 = player.General2 = general_name;
                player.DeputySkinId = 0;
                DoAnimate(AnimateType.S_ANIMATE_REMOVE, player.Name, false.ToString());

                DisconnectSkillsFromOthers(player, false, false);

                foreach (string skill_name in player.GetDeputySkillList())
                {
                    Skill skill = Engine.GetSkill(skill_name);
                    if (skill != null)
                        DetachSkillFromPlayer(player, skill.Name, false, false, false);
                }
            }

            LogMessage log = new LogMessage
            {
                Type = "#BasaraRemove",
                From = player.Name,
                Arg = head_general ? "head_general" : "deputy_general",
                Arg2 = from_general
            };
            SendLog(log);

            HandleUsedGeneral("-" + from_general);
            object _from = new InfoStruct { Info = from_general, Head = head_general };
            RoomThread.Trigger(TriggerEvent.GeneralRemoved, this, player, ref _from);

            FilterCards(player, player.GetCards("he"), true);
        }

        public void DisconnectSkillsFromOthers(Player player, bool head_skill = true, bool trigger = true)
        {
            List<string> skills = head_skill ? new List<string>(player.HeadSkills.Keys) : new List<string>(player.DeputySkills.Keys);
            List<Client> clients = new List<Client>(Clients);
            foreach (string skill in skills)
            {
                object _skill = new InfoStruct { Info = skill, Head = head_skill };
                if (trigger)
                    RoomThread.Trigger(TriggerEvent.EventLoseSkill, this, player, ref _skill);
                List<string> args = new List<string> { GameEventType.S_GAME_EVENT_DETACH_SKILL.ToString(), player.Name, skill, head_skill.ToString() };
                foreach (Client p in clients)
                    if (p != GetClient(player))
                        DoNotify(p, CommandType.S_COMMAND_LOG_EVENT, args);
            }
        }

        public void UpdateJudgeResult(ref JudgeStruct judge)
        {
            if (string.IsNullOrEmpty(judge.Pattern))
                judge.UpdateResult(true);
            else
                judge.UpdateResult(judge.Good == Engine.GetPattern(judge.Pattern).Match(judge.Who, this, judge.Card));
        }

        public void Retrial(WrappedCard card, Player player, ref JudgeStruct judge, string skill_name, bool exchange = false, string position = null)
        {
            if (card == null) return;
            bool triggerResponded = GetCardOwner(card.GetEffectiveId()) == player;
            bool isHandcard = (triggerResponded && GetCardPlace(card.GetEffectiveId()) == Place.PlaceHand);

            WrappedCard oldJudge = judge.Card;
            judge.Card = GetCard(card.GetEffectiveId());

            CardsMoveStruct move1 = new CardsMoveStruct(new List<int>(), judge.Who, Place.PlaceJudge,
                new CardMoveReason(MoveReason.S_REASON_RETRIAL, player.Name, judge.Who.Name, skill_name, null));
            move1.Reason.General = RoomLogic.GetGeneralSkin(this, player, skill_name, position);

            move1.Card_ids.Add(card.GetEffectiveId());
            MoveReason reasonType;
            if (exchange)
                reasonType = MoveReason.S_REASON_OVERRIDE;
            else
                reasonType = MoveReason.S_REASON_JUDGEDONE;

            CardMoveReason reason = new CardMoveReason(reasonType, player.Name, exchange ? skill_name : null, null);
            CardsMoveStruct move2 = new CardsMoveStruct(new List<int>(), judge.Who, exchange ? player : null, Place.PlaceUnknown, exchange ? Place.PlaceHand : Place.DiscardPile, reason);

            move2.Card_ids.Add(oldJudge.GetEffectiveId());

            NotifySkillInvoked(player, skill_name);
            LogMessage log = new LogMessage
            {
                Type = "$ChangedJudge",
                Arg = skill_name,
                From = player.Name,
                To = new List<string> { judge.Who.Name },
                Card_str = card.GetEffectiveId().ToString()
            };
            SendLog(log);

            List<CardsMoveStruct> moves = new List<CardsMoveStruct> { move1, move2 };
            MoveCardsAtomic(moves, true);
            //judge->updateResult();

            if (triggerResponded)
            {
                CardResponseStruct resp = new CardResponseStruct(player, card, judge.Who)
                {
                    Handcard = isHandcard,
                    Retrial = true,
                    Reason = skill_name,
                    Data = judge
                };
                object data = resp;
                RoomThread.Trigger(TriggerEvent.CardResponded, this, player, ref data);
            }
        }

        public bool AskForYiji(Player guojia, List<int> cards, string skill_name = null,  bool is_preview = false, bool visible = false, bool optional = true, int max_num = -1,
            List<Player> players = null, CardMoveReason reason = null, string prompt = null, string expand_pile = null, bool notify_skill = true, string position = null)
        {
            if (max_num == -1)
                max_num = cards.Count;
            players = players ?? new List<Player>();
            if (players.Count == 0)
                players = GetOtherPlayers(guojia);
            if (cards.Count == 0 || max_num == 0)
                return false;
            if (reason == null) reason = new CardMoveReason();
            if (reason.Reason == MoveReason.S_REASON_UNKNOWN)
            {
                reason.PlayerId = guojia.Name;
                reason.SkillName = skill_name;
                // when we use ? : here, compiling error occurs under debug mode...
                if (is_preview)
                    reason.Reason = MoveReason.S_REASON_PREVIEWGIVE;
                else
                    reason.Reason = MoveReason.S_REASON_GIVE;
            }
            Thread.Sleep(300);

            Player target = null;
            List<int> ids = new List<int>();
            TrustedAI ai = GetAI(guojia);
            do
            {
                NotifyMoveFocus(guojia, CommandType.S_COMMAND_SKILL_YIJI);
                if (ai != null)
                {
                    int card_id = -1;
                    Player who = ai.AskForYiji(cards, skill_name, ref card_id);
                    if (who == null)
                        break;
                    else
                    {
                        target = who;
                        ids.Add(card_id);
                    }
                }
                else
                {
                    Interactivity client = GetInteractivity(guojia);
                    bool success = client != null && client.YijiRequest(this, guojia, cards, skill_name, players, prompt, max_num, optional, expand_pile, position);
                    //Validate client response
                    List<string> clientReply = client?.ClientReply;
                    if (!success || clientReply == null || clientReply.Count != 2)
                        break;

                    ids = JsonUntity.Json2List<int>(clientReply[0]);

                    foreach (int id in ids)
                    {
                        if (!cards.Contains(id))
                            break;
                    }

                    Player who = FindPlayer(clientReply[1]);
                    if (who == null)
                        break;
                    else
                        target = who;
                }
                DoBroadcastNotify(CommandType.S_COMMAND_UNKNOWN, new List<string> { false.ToString() });
            } while (false);

            if (target == null)
            {
                if (optional)
                    return false;
                else
                {
                    ids.Clear();
                    Shuffle.shuffle<int>(ref cards);
                    Shuffle.shuffle<Player>(ref players);
                    ids.Add(cards[0]);
                    target = players[0];
                }
            }

            //DummyCard dummy_card;
            List<int> to_move = new List<int>();
            foreach (int card_id in ids)
            {
                cards.Remove(card_id);
                //dummy_card.addSubcard(card_id);
                to_move.Add(card_id);
            }

            object decisionData = string.Format("Yiji:{0}:{1}:{2}:{3}", skill_name, guojia.Name, target.Name, string.Join("+", JsonUntity.IntList2StringList(ids)));
            RoomThread.Trigger(TriggerEvent.ChoiceMade, this, guojia, ref decisionData);

            if (notify_skill)
            {
                LogMessage log = new LogMessage
                {
                    Type = "#InvokeSkill",
                    From = guojia.Name,
                    Arg = skill_name
                };
                SendLog(log);

                Skill skill = Engine.GetSkill(skill_name);
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(this, guojia, position);
                string _skill_name = skill_name;
                string general = gsk.General;
                int index = -1, skin_id = gsk.SkinId;
                if (guojia != target) DoAnimate(AnimateType.S_ANIMATE_INDICATE, guojia.Name, target.Name);
                if (skill != null)
                {
                    skill.GetEffectIndex(this, target, null, ref index, ref _skill_name, ref general, ref skin_id);
                    if (index != -2)
                        BroadcastSkillInvoke(_skill_name, "male", index, general, skin_id);
                }
                NotifySkillInvoked(guojia, _skill_name);
            }

            if (guojia != target)          //为别人送牌加辅助
            {
                ResultStruct assist = guojia.Result;
                assist.Assist += to_move.Count;
                guojia.Result = assist;
            }
            
            reason.TargetId = target.Name;
            MoveCardsAtomic(new CardsMoveStruct(to_move, target, Place.PlaceHand, reason), visible);
            return true;
        }

        public List<int> NotifyChooseCards(Player player, List<int> cards, string reason,
                                   int max_num, int min_num, string prompt, string pattern, string position)
        {
            player.PileChange("#" + reason, cards);
            List<int> result = AskForExchange(player, reason, max_num, min_num, prompt, "#" + reason, pattern, position);
            player.PileChange("#" + reason, cards, false);

            return result;
        }

        public List<int> AskForExchange(Player player, string reason, int exchange_num, int min_num,
                                string prompt, string _expand_pile, string pattern, string position)
        {
            if (!player.Alive)
                return new List<int>();

            Thread.Sleep(300);

            NotifyMoveFocus(player, CommandType.S_COMMAND_EXCHANGE_CARD);

            string new_pattern = pattern;
            string expand_pile = _expand_pile;
            if (!string.IsNullOrEmpty(expand_pile) && expand_pile.Contains("#"))
                expand_pile = expand_pile.Replace("#", "$");
            if (string.IsNullOrEmpty(pattern))
                new_pattern = ".|.|.|" + (string.IsNullOrEmpty(_expand_pile) ? "." : expand_pile);

            TrustedAI ai = GetAI(player);
            List<int> to_exchange = new List<int>();
            if (ai != null)
            {
                player.SetFlags("Global_AIDiscardExchanging");
                to_exchange = ai.AskForExchange(reason, new_pattern, exchange_num, min_num, _expand_pile);
                player.SetFlags("-Global_AIDiscardExchanging");
                if (min_num == 0 && to_exchange.Count > 0)
                    Thread.Sleep(500);
            }
            else
            {
                Interactivity client = GetInteractivity(player);
                bool success = client != null && client.ExchangeRequest(this, player, prompt, reason, position, exchange_num, min_num, new_pattern, _expand_pile);
                //@todo: also check if the player does have that card!!!
                if (success)
                {
                    List<string> clientReply = client?.ClientReply;
                    if (clientReply != null && clientReply.Count > 0)
                    {
                        to_exchange = JsonUntity.Json2List<int>(clientReply[0]);
                    }
                }
            }
            DoBroadcastNotify(CommandType.S_COMMAND_UNKNOWN, new List<string> { false.ToString() });

            if (to_exchange.Count < min_num || to_exchange.Count > exchange_num)
            {
                if (min_num == 0)
                    to_exchange.Clear();
                else
                    to_exchange = ForceToExchange(player, min_num, pattern, _expand_pile);
            }

            object data = string.Format("{0}:{1}:{2}", "cardExchange", reason, string.Join("+", JsonUntity.IntList2StringList(to_exchange)));
            RoomThread.Trigger(TriggerEvent.ChoiceMade, this, player, ref data);

            return to_exchange;
        }

        public PindianStruct PindianSelect(Player from, Player target, string reason, WrappedCard card1 = null, PindianInfo.PindianType type = PindianInfo.PindianType.Pindian)
        {
            if (from == target) return new PindianStruct();
            PindianStruct pd = PindianSelect(from, new List<Player> { target }, reason, card1, type);
            return pd;
        }

        public PindianStruct PindianSelect(Player from, List<Player> targets, string reason, WrappedCard card1 = null, PindianInfo.PindianType type = PindianInfo.PindianType.Pindian)
        {
            if (targets.Contains(from)) return new PindianStruct();
            LogMessage log = new LogMessage
            {
                Type = type == PindianInfo.PindianType.Pindian ? "#Pindian" : "#Show",
                From = from.Name,
                To = new List<string>()
            };
            foreach (Player p in targets)
                log.To.Add(p.Name);
            SendLog(log);

            Thread.Sleep(300);

            List<WrappedCard> cards = AskForPindianRace(from, targets, reason, card1, type);
            card1 = cards[0];
            foreach (WrappedCard card in cards)
                if (card == null) return new PindianStruct();

            cards.Remove(card1);

            PindianStruct pindian = new PindianStruct
            {
                From = from,
                Tos = targets,
                From_card = card1,
                To_cards = cards,
                From_number = card1.Number,
                To_numbers = new List<int>(),
                Reason = reason
            };

            if (type == PindianInfo.PindianType.Pindian)
            {
                List<CardsMoveStruct> pd_move = new List<CardsMoveStruct>();
                CardsMoveStruct move1 = new CardsMoveStruct
                {
                    Card_ids = new List<int> { pindian.From_card.Id },
                    From = pindian.From.Name,
                    To = null,
                    To_place = Place.PlaceTable
                };
                CardMoveReason reason1 = new CardMoveReason(MoveReason.S_REASON_PINDIAN, pindian.From.Name, null, pindian.Reason, null);
                move1.Reason = reason1;
                pd_move.Add(move1);

                for (int i = 0; i < targets.Count; i++)
                {
                    CardsMoveStruct move2 = new CardsMoveStruct
                    {
                        Card_ids = new List<int> { cards[i].Id },
                        From = targets[i].Name,
                        To = null,
                        To_place = Place.PlaceTable
                    };
                    CardMoveReason reason2 = new CardMoveReason(MoveReason.S_REASON_PINDIAN, targets[i].Name);
                    move2.Reason = reason2;
                    pd_move.Add(move2);
                }


                LogMessage log2 = new LogMessage
                {
                    Type = "$PindianResult",
                    From = pindian.From.Name,
                    Card_str = pindian.From_card.Id.ToString()
                };
                SendLog(log2);

                for (int i = 0; i < targets.Count; i++)
                {
                    log2.Type = "$PindianResult";
                    log2.From = pindian.Tos[i].Name;
                    log2.Card_str = pindian.To_cards[i].Id.ToString();
                    SendLog(log2);
                }

                MoveCardsAtomic(pd_move, true);
            }

            return pindian;
        }

        public void Pindian(ref PindianStruct pd, int index = 0, PindianInfo.PindianType type = PindianInfo.PindianType.Pindian)
        {
            if (index == 0)
            {
                int old_number = pd.From_number = pd.From_card.Number;
                List<int> old_to = new List<int>();
                for (int i = 0; i < pd.Tos.Count; i++)
                {
                    pd.To_numbers.Add(pd.To_cards[i].Number);
                    old_to.Add(pd.To_numbers[i]);
                }

                for (int i = 0; i < pd.Tos.Count; i++)
                {
                    Player target = pd.Tos[i];
                    List<string> arg = new List<string> { GameEventType.S_GAME_EVENT_REVEAL_PINDIAN.ToString(), target.Name, pd.From_card.Id.ToString(), pd.To_cards[i].Id.ToString() };
                    DoBroadcastNotify(CommandType.S_COMMAND_LOG_EVENT, arg);
                    Thread.Sleep(400);
                }

                if (type == PindianInfo.PindianType.Pindian)
                {
                    object data = pd;
                    RoomThread.Trigger(TriggerEvent.PindianVerifying, this, pd.From, ref data);
                    pd = (PindianStruct)data;

                    Thread.Sleep(2000);

                    if (old_number != pd.From_number)
                    {
                        List<string> arg = new List<string> { GameEventType.S_GAME_EVENT_ALTER_PINDIAN.ToString(), pd.From.Name, pd.From_number.ToString(), pd.From_card.Id.ToString() };
                        DoBroadcastNotify(CommandType.S_COMMAND_LOG_EVENT, arg);
                        Thread.Sleep(500);
                    }

                    for (int i = 0; i < pd.Tos.Count; i++)
                    {
                        Player target = pd.Tos[i];
                        if (old_to[i] != pd.To_numbers[i])
                        {
                            List<string> arg = new List<string> { GameEventType.S_GAME_EVENT_ALTER_PINDIAN.ToString(), target.Name, pd.To_numbers[i].ToString(), pd.To_cards[i].Id.ToString() };
                            DoBroadcastNotify(CommandType.S_COMMAND_LOG_EVENT, arg);
                            Thread.Sleep(500);
                        }
                    }
                }
            }
            pd.Index = index;
            pd.Success = pd.From_number > pd.To_numbers[index];
            Player to = pd.Tos[index];

            int pindian_type = type == PindianInfo.PindianType.Show ? 0 : pd.Success ? 1 : pd.From_number == pd.To_numbers[index] ? 2 : 3;
            List<string> _arg = new List<string> { GuanxingStep.S_GUANXING_FINISH.ToString(), pindian_type.ToString(), (index + 1).ToString() };
            DoBroadcastNotify(CommandType.S_COMMAND_PINDIAN, _arg);

            Thread.Sleep(2000);

            if (type == PindianInfo.PindianType.Pindian)
            {
                object decisionData = string.Format("pindian:{0}:{1}:{2}:{3}:{4}", pd.Reason, pd.From.Name, pd.From_card.Id, to.Name,
                pd.To_cards[index].Id);
                RoomThread.Trigger(TriggerEvent.ChoiceMade, this, pd.From, ref decisionData);

                LogMessage log = new LogMessage
                {
                    Type = pd.Success ? "#PindianSuccess" : "#PindianFailure",
                    From = pd.From.Name,
                    To = new List<string> { to.Name }
                };
                SendLog(log);

                object _data = pd;
                RoomThread.Trigger(TriggerEvent.Pindian, this, pd.From, ref _data);

                List<CardsMoveStruct> pd_move = new List<CardsMoveStruct>();
                if (GetCardPlace(pd.From_card.Id) == Place.PlaceTable && index == pd.Tos.Count - 1)
                {
                    CardsMoveStruct move1 = new CardsMoveStruct
                    {
                        Card_ids = new List<int> { pd.From_card.Id },
                        From = pd.From.Name,
                        To = null,
                        To_place = Place.DiscardPile,
                        Reason = new CardMoveReason(MoveReason.S_REASON_PINDIAN, pd.From.Name, pd.Reason, null)
                    };
                    pd_move.Add(move1);
                }

                if (GetCardPlace(pd.To_cards[index].GetEffectiveId()) == Place.PlaceTable)
                {
                    CardsMoveStruct move2 = new CardsMoveStruct
                    {
                        Card_ids = new List<int> { pd.To_cards[index].Id },
                        From = to.Name,
                        To = null,
                        To_place = Place.DiscardPile,
                        Reason = new CardMoveReason(MoveReason.S_REASON_PINDIAN, to.Name)
                    };

                    pd_move.Add(move2);
                }

                if (pd_move.Count > 0)
                    MoveCardsAtomic(pd_move, true);
            }
            else
            {
                object decisionData = string.Format("showCards:{0}:{1}", pd.From_card.Id, "all");
                RoomThread.Trigger(TriggerEvent.ChoiceMade, this, pd.From, ref decisionData);

                decisionData = string.Format("showCards:{0}:{1}", pd.To_cards[index].Id, "all");
                RoomThread.Trigger(TriggerEvent.ChoiceMade, this, pd.Tos[index], ref decisionData);
            }
        }

        private List<WrappedCard> AskForPindianRace(Player from, List<Player> to, string reason, WrappedCard card, PindianInfo.PindianType type)
        {
            List<WrappedCard> result = new List<WrappedCard>();
            List<int> to_cards = new List<int>();
            for (int index = 0; index < to.Count; index++)
                to_cards.Add(-1);
            if (!from.Alive || from.IsKongcheng())
                return result;
            List<string> names = new List<string> { from.Name };
            foreach (Player p in to)
            {
                names.Add(p.Name);
                if (!p.Alive || p.IsKongcheng())
                    return result;
            }

            PindianInfo info = new PindianInfo()
            {
                From = from,
                Reason = reason,
                Cards = new Dictionary<Player, WrappedCard>()
            };
            info.Cards[from] = card;
            foreach (Player p in to)
                info.Cards[p] = null;

            if (type == PindianInfo.PindianType.Pindian)
            {
                object data = info;
                RoomThread.Trigger(TriggerEvent.PindianCard, this, null, ref data);

                info = (PindianInfo)data;
                card = info.Cards[from];

                for (int index = 0; index < to.Count; index++)
                    if (info.Cards[to[index]] != null)
                        to_cards[index] = info.Cards[to[index]].GetEffectiveId();
            }

            Thread.Sleep(500);

            Countdown countdown = new Countdown
            {
                Max = Setting.GetCommandTimeout(CommandType.S_COMMAND_PINDIAN, ProcessInstanceType.S_CLIENT_INSTANCE),
                Type = Countdown.CountdownType.S_COUNTDOWN_USE_SPECIFIED
            };

            WrappedCard from_card = card;
            List<Player> players = new List<Player>();
            _m_roomState.SetCurrentCardUsePattern(".");
            _m_roomState.SetCurrentCardUseReason(CardUseStruct.CardUseReason.CARD_USE_REASON_UNKNOWN);

            TrustedAI ai;
            if (from_card == null)
            {
                ai = GetAI(from);
                if (from.HandcardNum == 1)
                {
                    from_card = GetCard(from.HandCards[0]);
                }
                else if (ai != null)
                {
                    from_card = ai.AskForPindian(from, to, reason);
                }
                else
                    players.Add(from);
            }

            bool check = true;
            for (int index = 0; index < to.Count; index++)
            {
                if (to_cards[index] >= 0) continue; 
                if (to[index].HandcardNum == 1)
                    to_cards[index] = to[index].HandCards[0];
                else
                {
                    ai = GetAI(to[index]);
                    if (ai != null)
                    {
                        to_cards[index] = ai.AskForPindian(from, to, reason).Id;
                    }
                    else
                        players.Add(to[index]);
                }
                if (to_cards[index] == -1)
                    check = false;
            }

            List<string> stepArgs = new List<string> { GuanxingStep.S_GUANXING_START.ToString(), reason, JsonUntity.Object2Json(names) };
            DoBroadcastNotify(CommandType.S_COMMAND_PINDIAN, stepArgs);

            Thread.Sleep(500);

            if (from_card != null)
            {
                DoBroadcastNotify(CommandType.S_COMMAND_PINDIAN, new List<string> { GuanxingStep.S_GUANXING_MOVE.ToString(), from.Name, "-1" }, GetClient(from));
                DoNotify(GetClient(from), CommandType.S_COMMAND_PINDIAN, new List<string> { GuanxingStep.S_GUANXING_MOVE.ToString(), from.Name, from_card.Id.ToString() });
            }

            for (int index = 0; index < to_cards.Count; index++)
            {
                int id = to_cards[index];
                if (id == -1) continue;
                Player who = to[index];
                DoBroadcastNotify(CommandType.S_COMMAND_PINDIAN, new List<string> { GuanxingStep.S_GUANXING_MOVE.ToString(), who.Name, "-1" }, GetClient(who));
                DoNotify(GetClient(who), CommandType.S_COMMAND_PINDIAN, new List<string> { GuanxingStep.S_GUANXING_MOVE.ToString(), who.Name, id.ToString() });
            }

            if (from_card != null && check)
            {
                Thread.Sleep(500);
                result.Add(from_card);
                foreach (int id in to_cards)
                    result.Add(GetCard(id));

                return result;
            }

            int i = 0;
            while (players.Count > 0)
            {
                List<Interactivity> responsers = new List<Interactivity>();
                List<Player> response_players = new List<Player>();
                foreach (Player p in players)
                {
                    if (!responsers.Contains(GetInteractivity(p)))
                    {
                        GetInteractivity(p).PindianRequest(this, p, from, type);
                        responsers.Add(GetInteractivity(p));
                        response_players.Add(p);
                    }
                }

                NotifyMoveFocus(response_players, countdown);
                DoBroadcastRequest(responsers, CommandType.S_COMMAND_PINDIAN);
                DoBroadcastNotify(CommandType.S_COMMAND_UNKNOWN, new List<string> { false.ToString() });

                foreach (Player player in response_players)
                {
                    WrappedCard c = null;
                    Interactivity client = GetInteractivity(player);
                    List<string> clientReply = client?.ClientReply;
                    if (client == null || !client.IsClientResponseReady || clientReply == null || clientReply.Count == 0)
                    {
                        int card_id = GetRandomHandCard(player);
                        c = GetCard(card_id);

                        DoBroadcastNotify(CommandType.S_COMMAND_PINDIAN, new List<string> { GuanxingStep.S_GUANXING_MOVE.ToString(), player.Name, "-1" }, GetClient(player));
                        DoNotify(GetClient(player), CommandType.S_COMMAND_PINDIAN, new List<string> { GuanxingStep.S_GUANXING_MOVE.ToString(), player.Name, card_id.ToString() });
                    }
                    else
                    {
                        c = GetCard(RoomLogic.ParseCard(this, clientReply[1]).GetEffectiveId());
                    }

                    if (player == from)
                        from_card = c;
                    else
                    {
                        for (int index = 0; index < to.Count; index++)
                        {
                            if (to[index] == player)
                            {
                                to_cards[index] = c.GetEffectiveId();
                                break;
                            }
                        }
                    }
                    players.Remove(player);
                }

                i++;
            }

            result.Add(from_card);
            foreach (int id in to_cards)
                result.Add(GetCard(id));

            return result;
        }

        public void AbolisheEquip(Player player, int index, string skill)
        {
            if (index >= 5 || player.EquipIsBaned(index)) return;

            int id = player.GetEquip(index);
            if (id >= 0)
            {
                CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_ABOLISH, player.Name, skill, string.Empty);
                MoveCardTo(GetCard(id), null, Place.PlaceTable, reason, true);

                List<int> table_cardids = GetCardIdsOnTable(GetCard(id));
                if (table_cardids.Count > 0)
                {
                    CardsMoveStruct move = new CardsMoveStruct(table_cardids, player, null, Place.PlaceTable, Place.DiscardPile, reason);
                    MoveCardsAtomic(new List<CardsMoveStruct>() { move }, true);
                }
            }
            player.BanEquip(index);
            List<string> args = new List<string>
                {
                    GameEventType.S_GAME_EVENT_EQUIP_ABOLISH.ToString(),
                    player.Name,
                    index.ToString(),
                    "true"
                };
            DoBroadcastNotify(CommandType.S_COMMAND_LOG_EVENT, args);

            LogMessage log = new LogMessage("#abolish")
            {
                From = player.Name,
            };
            switch (index)
            {
                case 0:
                    log.Arg = "Weapon";
                    break;
                case 1:
                    log.Arg = "Armor";
                    break;
                case 2:
                    log.Arg = "DefensiveHorse";
                    break;
                case 3:
                    log.Arg = "OffensiveHorse";
                    break;
                case 4:
                    log.Arg = "Treasure";
                    break;
            }

            SendLog(log);
        }

        public void AbolishJudgingArea(Player player, string skill)
        {
            if (!player.JudgingAreaAvailable) return;
            player.JudgingAreaAvailable = false;
            List<int> ids = new List<int>(player.JudgingArea);
            player.JudgingArea.Clear();
            if (ids.Count >= 0)
            {
                CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_ABOLISH, player.Name, skill, string.Empty);

                CardsMoveStruct move = new CardsMoveStruct(ids, player, null, Place.PlaceTable, Place.DiscardPile, reason);
                MoveCardsAtomic(new List<CardsMoveStruct>() { move }, true);
            }
            

            LogMessage log = new LogMessage("#abolish")
            {
                From = player.Name,
                Arg = "JudgingArea"
            };

            SendLog(log);
        }

        public void AskForGuanxing(Player zhuge, List<int> cards, string skill_name, bool both_side, string position)
        {
            AskForMoveCardsStruct result = AskForMoveCards(zhuge, cards, new List<int>(), true, skill_name, 0, both_side ? cards.Count : 0, false, true, new List<int>(), position);
            List<int> top_cards = result.Top, bottom_cards = result.Bottom;

            LogMessage log = new LogMessage
            {
                Type = "#GuanxingResult",
                From = zhuge.Name,
                Arg = top_cards.Count.ToString(),
                Arg2 = bottom_cards.Count.ToString()
            };
            SendLog(log);

            if (top_cards.Count > 0)
            {
                LogMessage log1 = new LogMessage
                {
                    Type = "$GuanxingTop",
                    From = zhuge.Name,
                    Card_str = string.Join("+", JsonUntity.IntList2StringList(top_cards))
                };
                SendLog(log1, zhuge);
            }
            if (bottom_cards.Count > 0)
            {
                LogMessage log1 = new LogMessage
                {
                    Type = "$GuanxingBottom",
                    From = zhuge.Name,
                    Card_str = string.Join("+", JsonUntity.IntList2StringList(bottom_cards))
                };
                SendLog(log, zhuge);
            }

            ReturnToDrawPile(top_cards, false, zhuge);
            ReturnToDrawPile(bottom_cards, true, zhuge);
        }

        public AskForMoveCardsStruct AskForMoveCards(Player zhuge, List<int> upcards, List<int> downcards, bool visible, string skillName,
            int min_num, int max_num, bool can_refuse, bool write_step, List<int> notify_visible_list, string position)
        {
            List<int> top_cards = new List<int>(), bottom_cards = new List<int>(), to_move = new List<int>(upcards);
            notify_visible_list = notify_visible_list ?? new List<int>();
            to_move.AddRange(downcards);
            bool success = true;

            Thread.Sleep(300);

            NotifyMoveFocus(zhuge, CommandType.S_COMMAND_SKILL_MOVECARDS);
            List<string> stepArgs = new List<string>
            {
                GuanxingStep.S_GUANXING_START.ToString(), zhuge.Name, skillName, JsonUntity.Object2Json(upcards), JsonUntity.Object2Json(downcards),
                MoveCardType.S_TYPE_MOVE.ToString(), min_num.ToString(), max_num.ToString(), position
            };

            if (visible)
            {
                List<int> notify_up = new List<int>(), notify_down = new List<int>();
                foreach (int id in upcards)
                {
                    if (notify_visible_list.Contains(id))
                        notify_up.Add(id);
                    else
                        notify_up.Add(-1);
                }

                foreach (int id in downcards)
                {
                    if (notify_visible_list.Contains(id))
                        notify_down.Add(id);
                    else
                        notify_down.Add(-1);
                }
                stepArgs[3] = JsonUntity.Object2Json(notify_up);
                stepArgs[4] = JsonUntity.Object2Json(notify_down);
                DoBroadcastNotify(CommandType.S_COMMAND_MIRROR_MOVECARDS_STEP, stepArgs, GetClient(zhuge));
            }
            TrustedAI ai = GetAI(zhuge);
            bool isTrustAI = zhuge.Status == "trust";
            if (ai != null)
            {
                if (isTrustAI)
                {
                    stepArgs[3] = JsonUntity.Object2Json(upcards);
                    stepArgs[4] = JsonUntity.Object2Json(downcards);
                    DoNotify(GetClient(zhuge), CommandType.S_COMMAND_MIRROR_MOVECARDS_STEP, stepArgs);
                }

                AskForMoveCardsStruct map = ai.AskForMoveCards(upcards, downcards, skillName, min_num, max_num);
                top_cards = map.Top;
                bottom_cards = map.Bottom;
                List<int> reserved = new List<int>(top_cards);
                reserved.AddRange(bottom_cards);
                bool length_equal = reserved.Count == to_move.Count;
                bool result_equal = to_move.Except(reserved).Count() == 0;
                if (length_equal && result_equal && bottom_cards.Count >= min_num && (bottom_cards.Count <= max_num || max_num == 0))
                    success = true;

                if (visible || isTrustAI) Thread.Sleep(1000);

                if (success)
                {
                    bool match = upcards.Count == top_cards.Count && downcards.Count == bottom_cards.Count;
                    if (match)
                    {
                        for (int i = 0; i < top_cards.Count; ++i)
                        {
                            if (top_cards[i] != upcards[i])
                            {
                                match = false;
                                break;
                            }
                        }
                    }
                    if (match)
                    {
                        for (int i = 0; i < bottom_cards.Count; ++i)
                        {
                            if (bottom_cards[i] != downcards[i])
                            {
                                match = false;
                                break;
                            }
                        }
                    }
                    if (!match && (visible || isTrustAI))
                    {
                        List<int> ups = new List<int>(upcards);
                        List<int> downs = new List<int>(downcards);
                        int upcount = Math.Max(upcards.Count, downcards.Count);

                        int fromPos;
                        int toPos;
                        for (int i = 0; i < top_cards.Count; ++i)
                        {
                            fromPos = 0;
                            if (top_cards[i] != ups[i])
                            {
                                int target_id = top_cards[i];
                                toPos = i + 1;
                                foreach (int id in ups)
                                {
                                    if (id == target_id)
                                    {
                                        fromPos = ups.IndexOf(id) + 1;
                                        break;
                                    }
                                }
                                if (fromPos != 0)
                                {
                                    ups.Remove(target_id);
                                }
                                else
                                {
                                    foreach (int id in downs)
                                    {
                                        if (id == target_id)
                                        {
                                            fromPos = -downs.IndexOf(id) - 1;
                                            break;
                                        }
                                    }
                                    downs.Remove(top_cards[i]);
                                }
                                List<int> move = new List<int>(ups), empty = new List<int>();
                                for (int c = i; c < move.Count; ++c)
                                {
                                    ups.Remove(move[c]);
                                    empty.Add(move[c]);
                                }
                                ups.Add(top_cards[i]);
                                ups.AddRange(empty);
                                if (ups.Count > upcount)
                                {
                                    int adjust_id = ups[ups.Count - 1];
                                    ups.Remove(adjust_id);
                                    downs.Add(adjust_id);
                                }
                                List<string> movearg_base = new List<string> { GuanxingStep.S_GUANXING_MOVE.ToString(), "-1", fromPos.ToString(), toPos.ToString() };
                                if (visible)
                                    DoBroadcastNotify(CommandType.S_COMMAND_MIRROR_MOVECARDS_STEP, movearg_base);
                                else
                                    DoNotify(GetClient(zhuge), CommandType.S_COMMAND_MIRROR_MOVECARDS_STEP, movearg_base);
                                Thread.Sleep(500);
                            }
                        }

                        if (ups.Count > top_cards.Count)
                        {
                            int newcount = ups.Count - top_cards.Count;
                            for (int i = 1; i <= newcount; ++i)
                            {
                                fromPos = ups.Count;
                                int adjust_id = ups[ups.Count - 1];
                                ups.Remove(adjust_id);
                                toPos = -downs.Count - 1;
                                downs.Add(adjust_id);

                                List<string> movearg_base = new List<string> { GuanxingStep.S_GUANXING_MOVE.ToString(), "-1", fromPos.ToString(), toPos.ToString() };
                                if (visible)
                                    DoBroadcastNotify(CommandType.S_COMMAND_MIRROR_MOVECARDS_STEP, movearg_base);
                                else
                                    DoNotify(GetClient(zhuge), CommandType.S_COMMAND_MIRROR_MOVECARDS_STEP, movearg_base);
                                Thread.Sleep(500);
                            }
                        }

                        for (int i = 0; i < bottom_cards.Count - 1; ++i)
                        {
                            fromPos = 0;
                            if (bottom_cards[i] != downs[i])
                            {
                                toPos = -i - 1;
                                foreach (int id in downs)
                                {
                                    if (id == bottom_cards[i])
                                    {
                                        fromPos = -downs.IndexOf(id) - 1;
                                        break;
                                    }
                                }
                                downs.Remove(bottom_cards[i]);

                                List<int> move = new List<int>(downs), empty = new List<int>();
                                for (int c = i; c < move.Count; ++c)
                                {
                                    downs.Remove(move[c]);
                                    empty.Add(move[c]);
                                }
                                downs.Add(bottom_cards[i]);
                                downs.AddRange(empty);

                                List<string> movearg_base = new List<string> { GuanxingStep.S_GUANXING_MOVE.ToString(), "-1", fromPos.ToString(), toPos.ToString() };
                                if (visible)
                                    DoBroadcastNotify(CommandType.S_COMMAND_MIRROR_MOVECARDS_STEP, movearg_base);
                                else
                                    DoNotify(GetClient(zhuge), CommandType.S_COMMAND_MIRROR_MOVECARDS_STEP, movearg_base);
                                Thread.Sleep(500);
                            }
                        }
                    }
                    Thread.Sleep(1000);
                }
                else
                    System.Diagnostics.Debug.Assert(success);
            }
            else
            {
                Interactivity client = GetInteractivity(zhuge);
                if (client != null)
                    client.GuanxingRequest(this, zhuge, skillName, upcards, downcards, min_num, max_num, can_refuse, write_step, visible, position);
                AskForMoveCardsStruct guanxin = new AskForMoveCardsStruct
                {
                    Top = new List<int>(),
                    Bottom = new List<int>()
                };

                if (client != null && client.ClientReply != null && client.ClientReply.Count > 0)
                    guanxin = JsonUntity.Json2Object<AskForMoveCardsStruct>(client.ClientReply[0]);
                success = guanxin.Success;
                if (success)
                {
                    top_cards = guanxin.Top;
                    bottom_cards = guanxin.Bottom;

                    List<int> reserved = new List<int>(top_cards);
                    reserved.AddRange(bottom_cards);
                    bool length_equal = reserved.Count == to_move.Count;
                    bool result_equal = to_move.Except(reserved).Count() == 0;

                    if (!length_equal || !result_equal)
                    {
                        top_cards.Clear();
                        bottom_cards.Clear();
                        success = false;
                    }
                }

                if (!success && !can_refuse)
                {
                    while (bottom_cards.Count < min_num)
                    {
                        bottom_cards.Add(to_move[0]);
                        to_move.RemoveAt(0);

                    }
                    top_cards = to_move;
                }
            }
            
            DoBroadcastNotify(CommandType.S_COMMAND_MIRROR_MOVECARDS_STEP, new List<string> { GuanxingStep.S_GUANXING_FINISH.ToString() }, isTrustAI ? null : GetClient(zhuge));
            DoBroadcastNotify(CommandType.S_COMMAND_UNKNOWN, new List<string> { false.ToString() });

            object decisionData = string.Format("{0}chose:{1}:{2}:{3}", skillName, zhuge.Name,
                string.Join("+", JsonUntity.IntList2StringList(top_cards)), string.Join("+", JsonUntity.IntList2StringList(bottom_cards)));
            RoomThread.Trigger(TriggerEvent.ChoiceMade, this, zhuge, ref decisionData);
            AskForMoveCardsStruct returns = new AskForMoveCardsStruct
            {
                Top = top_cards,
                Bottom = bottom_cards,
                Success = success
            };
            return returns;
        }

        public AskForMoveCardsStruct AskforSortCards(Player player, string skillName, List<int> cards, bool visible, string position)
        {
            List<int> top_cards = new List<int>(), bottom_cards = new List<int>(), to_move = new List<int>(cards);
            bool success = true;

            Thread.Sleep(300);

            NotifyMoveFocus(player, CommandType.S_COMMAND_SKILL_MOVECARDS);
            List<string> stepArgs = new List<string>
            {
                GuanxingStep.S_GUANXING_START.ToString(), player.Name, skillName, JsonUntity.Object2Json(cards), string.Empty, MoveCardType.S_TYPE_SORT.ToString(), position
            };

            if (visible)
                DoBroadcastNotify(CommandType.S_COMMAND_MIRROR_MOVECARDS_STEP, stepArgs, GetClient(player));

            TrustedAI ai = GetAI(player);
            bool isTrustAI = player.Status == "trust";
            if (ai != null)
            {
                if (isTrustAI)
                    DoNotify(GetClient(player), CommandType.S_COMMAND_MIRROR_MOVECARDS_STEP, stepArgs);

                Thread.Sleep(1500);
                AskForMoveCardsStruct map = ai.AskForMoveCards(cards, new List<int>(), skillName, cards.Count, cards.Count);
                top_cards = map.Top;
                bottom_cards = map.Bottom;

                if (top_cards.Count > 0 && bottom_cards.Count > 0)
                {
                    bool check = true;
                    foreach (int id in top_cards)
                    {
                        if (bottom_cards.Contains(id) || !to_move.Contains(id))
                        {
                            check = false;
                            break;
                        }
                    }
                    if (check)
                    {
                        foreach (int id in bottom_cards)
                        {
                            if (!to_move.Contains(id))
                            {
                                check = false;
                                break;
                            }
                        }
                    }

                    success = check;
                }

                if (isTrustAI) Thread.Sleep(700);

                if (success)
                {
                    if (isTrustAI || visible)
                    {
                        foreach (int id in top_cards)
                        {
                            List<string> movearg_base = new List<string> { GuanxingStep.S_GUANXING_MOVE.ToString(), to_move.IndexOf(id).ToString(), "0", "1" };
                            to_move.Remove(id);
                            if (visible)
                                DoBroadcastNotify(CommandType.S_COMMAND_MIRROR_MOVECARDS_STEP, movearg_base);
                            else
                                DoNotify(GetClient(player), CommandType.S_COMMAND_MIRROR_MOVECARDS_STEP, movearg_base);
                            Thread.Sleep(500);
                        }

                        foreach (int id in bottom_cards)
                        {
                            List<string> movearg_base = new List<string> { GuanxingStep.S_GUANXING_MOVE.ToString(), to_move.IndexOf(id).ToString(), "0", "2" };
                            to_move.Remove(id);
                            if (visible)
                                DoBroadcastNotify(CommandType.S_COMMAND_MIRROR_MOVECARDS_STEP, movearg_base);
                            else
                                DoNotify(GetClient(player), CommandType.S_COMMAND_MIRROR_MOVECARDS_STEP, movearg_base);
                            Thread.Sleep(500);
                        }

                        Thread.Sleep(1500);
                    }
                }
                else
                {
                    top_cards.Clear();
                    bottom_cards.Clear();
                }
                if (isTrustAI)
                    DoNotify(GetClient(player), CommandType.S_COMMAND_MIRROR_MOVECARDS_STEP, new List<string> { GuanxingStep.S_GUANXING_FINISH.ToString() });
            }
            else
            {
                Interactivity client = GetInteractivity(player);
                if (client != null) client.SortCardRequest(this, player, skillName, cards, visible, position);
                AskForMoveCardsStruct guanxin = new AskForMoveCardsStruct
                {
                    Top = new List<int>(),
                    Bottom = new List<int>()
                };

                if (client != null && client.ClientReply != null && client.ClientReply.Count > 0)
                    guanxin = JsonUntity.Json2Object<AskForMoveCardsStruct>(client.ClientReply[0]);
                success = guanxin.Success;
                if (success)
                {
                    top_cards = guanxin.Top;
                    bottom_cards = guanxin.Bottom;

                    if (top_cards.Count > 0 && bottom_cards.Count > 0)
                    {
                        bool check = true;
                        foreach (int id in top_cards)
                        {
                            if (bottom_cards.Contains(id) || !to_move.Contains(id))
                            {
                                check = false;
                                break;
                            }
                        }
                        if (check)
                        {
                            foreach (int id in bottom_cards)
                            {
                                if (!to_move.Contains(id))
                                {
                                    check = false;
                                    break;
                                }
                            }
                        }

                        if (!check)
                        {
                            top_cards.Clear();
                            bottom_cards.Clear();
                        }
                    }
                }
            }
            if (visible)
                DoBroadcastNotify(CommandType.S_COMMAND_MIRROR_MOVECARDS_STEP, new List<string> { GuanxingStep.S_GUANXING_FINISH.ToString()}, GetClient(player));
            DoBroadcastNotify(CommandType.S_COMMAND_UNKNOWN, new List<string> { false.ToString() });

            object decisionData = string.Format("{0}chose:{1}:{2}:{3}", skillName, player.Name,
                string.Join("+", JsonUntity.IntList2StringList(top_cards)), string.Join("+", JsonUntity.IntList2StringList(bottom_cards)));
            RoomThread.Trigger(TriggerEvent.ChoiceMade, this, player, ref decisionData);
            AskForMoveCardsStruct returns = new AskForMoveCardsStruct
            {
                Top = top_cards,
                Bottom = bottom_cards,
                Success = success
            };
            return returns;
        }

        public void HideGeneral(Player player, bool head_general)
        {
            if (head_general)
            {
                if (player.General1 == "anjiang") return;

                player.SetSkillsPreshowed("h", false);
                // dirty hack for temporary convenience.
                player.General1Showed = false;
                BroadcastProperty(player, "General1Showed");
                player.SetSkillsPreshowed("h", true);
                NotifyPlayerPreshow(player, "h");

                List<string> arg = new List<string> { GameEventType.S_GAME_EVENT_CHANGE_HERO.ToString(), player.Name, "anjiang", false.ToString(), false.ToString() };
                DoBroadcastNotify(CommandType.S_COMMAND_LOG_EVENT, arg);
                ChangePlayerGeneral(player, "anjiang");

                DisconnectSkillsFromOthers(player);

                if (!string.IsNullOrEmpty(player.General2) && !player.General2Showed)
                {
                    player.Kingdom = "god";
                    player.Role = Engine.GetMappedRole("god");
                    BroadcastProperty(player, "Kingdom");
                    BroadcastProperty(player, "Role");
                }
            }
            else
            {
                if (player.General2 == "anjiang") return;

                player.SetSkillsPreshowed("d", false);
                // dirty hack for temporary convenience.
                player.General2Showed = false;
                BroadcastProperty(player, "General2Showed");
                player.SetSkillsPreshowed("d", true);
                NotifyPlayerPreshow(player, "d");

                List<string> arg = new List<string> { GameEventType.S_GAME_EVENT_CHANGE_HERO.ToString(), player.Name, "anjiang", true.ToString(), false.ToString() };
                DoBroadcastNotify(CommandType.S_COMMAND_LOG_EVENT, arg);
                ChangePlayerGeneral2(player, "anjiang");

                DisconnectSkillsFromOthers(player);

                if (!player.General1Showed)
                {
                    player.Kingdom = "god";
                    player.Role = Engine.GetMappedRole("god");
                    BroadcastProperty(player, "Kingdom");
                    BroadcastProperty(player, "Role");
                }
            }

            LogMessage log = new LogMessage
            {
                Type = "#BasaraConceal",
                From = player.Name,
                Arg = player.General1,
                Arg2 = player.General2
            };
            SendLog(log);
            Thread.Sleep(300);

            object _head = head_general;
            RoomThread.Trigger(TriggerEvent.GeneralHidden, this, player, ref _head);

            FilterCards(player, player.GetCards("he"), true);
        }

        public void AcquireSkill(Player player, Skill skill, bool open = true, bool head = true)
        {
            string skill_name = skill.Name;
            if (player.GetAcquiredSkills().Contains(skill_name))
                return;
            player.AcquireSkill(skill_name, head);

            if (skill.SkillFrequency == Frequency.Limited && !string.IsNullOrEmpty(skill.LimitMark))
                SetPlayerMark(player, skill.LimitMark, 1);

            if (skill.Visible)
            {
                if (open)
                {
                    List<string> args = new List<string> { GameEventType.S_GAME_EVENT_ACQUIRE_SKILL.ToString(), player.Name, skill_name, head.ToString() };
                    DoBroadcastNotify(CommandType.S_COMMAND_LOG_EVENT, args);
                }
                InfoStruct info = new InfoStruct
                {
                    Info = skill_name,
                    Head = head
                };
                object data = info;
                RoomThread.Trigger(TriggerEvent.EventAcquireSkill, this, player, ref data);
            }
        }

        public void AcquireSkill(Player player, string skill_name, bool open = true, bool head = true)
        {
            Skill skill = Engine.GetSkill(skill_name);
            if (skill != null) AcquireSkill(player, skill, open, head);
        }

        public void SetTurnSkillState(Player player, string skill_name, bool state, string position)
        {
            player.TurnSkillState[skill_name] = state;
            List<string> arg = new List<string>
            {
                GameEventType.S_GAME_EVENT_SKILL_TURN.ToString(),
                player.Name,
                skill_name,
                state.ToString(),
                position
            };
            DoBroadcastNotify(CommandType.S_COMMAND_LOG_EVENT, arg);
        }

        public void RemoveTurnSkill(Player player)
        {
            player.TurnSkillState.Clear();
            List<string> arg = new List<string>
            {
                GameEventType.S_GAME_EVENT_SKILL_TURN.ToString(),
                player.Name,
            };
            DoBroadcastNotify(CommandType.S_COMMAND_LOG_EVENT, arg);
        }

        public int DoGongxin(Player shenlvmeng, Player target, List<int> cards, List<int> enabled_ids, string skill_name, string prompt, string position)
        {
            Thread.Sleep(300);

            LogMessage log = new LogMessage
            {
                Type = "$ViewAllCards",
                From = shenlvmeng.Name,
                To = new List<string> { target.Name },
                Card_str = string.Join("+", JsonUntity.IntList2StringList(cards))
            };
            SendLog(log, shenlvmeng);

            object decisionData = string.Format("viewCards:{0}:{1}", shenlvmeng.Name, string.Join("+", JsonUntity.IntList2StringList(cards)));
            RoomThread.Trigger(TriggerEvent.ChoiceMade, this, target, ref decisionData);

            NotifyMoveFocus(shenlvmeng, CommandType.S_COMMAND_SKILL_GONGXIN);

            int card_id = -1;
            TrustedAI ai = GetAI(shenlvmeng);
            if (ai != null)
            {
                bool isTrustAI = shenlvmeng.Status == "trust";
                if (isTrustAI)
                {
                    List<string> gongxinArgs = new List<string> { target.Name, JsonUntity.Object2Json(cards), skill_name };
                    DoNotify(GetClient(shenlvmeng), CommandType.S_COMMAND_SHOW_CARD, gongxinArgs);
                }

                if (enabled_ids.Count > 0)
                    card_id = ai.AskForAG(new List<int>(enabled_ids), true, skill_name);
            }
            else
            {
                List<string> gongxinArgs = new List<string> {
                    shenlvmeng.Name,
                    target.Name,
                    false.ToString(),
                    JsonUntity.Object2Json(cards),
                    JsonUntity.Object2Json(enabled_ids),
                    skill_name,
                    prompt,
                    position
                };
                bool success = DoRequest(shenlvmeng, CommandType.S_COMMAND_SKILL_GONGXIN, gongxinArgs, true);
                List<string> clientReply = GetInteractivity(shenlvmeng).ClientReply;
                if (!success || clientReply == null || clientReply.Count == 0 || !cards.Contains(int.Parse(clientReply[0])))
                {
                }
                else
                    card_id = int.Parse(clientReply[0]);
            }
            DoBroadcastNotify(CommandType.S_COMMAND_UNKNOWN, new List<string> { false.ToString() });

            decisionData = string.Format("gongxin:{0}:{1}:{2}:{3}", skill_name, shenlvmeng.Name, target.Name, card_id);
            RoomThread.Trigger(TriggerEvent.ChoiceMade, this, target, ref decisionData);

            return card_id;
        }

        public void BeforeRoundStart(bool new_round)
        {
            if (new_round)
            {
                Round++;
                DoBroadcastNotify(CommandType.S_COMMAND_UPDATE_ROUND, new List<string> { Round.ToString() });
            }

            //进入鏖战模式判断
            if (AliveCount() <= Players.Count / 2 && Setting.GameMode == "Hegemony" && !BloodBattle)
            {
                bool check = true;
                foreach (Player p in GetAlivePlayers())
                {
                    if (RoomLogic.GetPlayerNumWithSameKingdom(this, p) > 1)
                    {
                        check = false;
                        break;
                    }
                }
                if (check)
                {
                    BloodBattle = true;
                    DoBroadcastNotify(CommandType.S_COMMAND_GAMEMODE_BLOODBATTLE, new List<string>());
                    Thread.Sleep(2500 + AliveCount() * 500);
                }
            }

            if (new_round)
            {
                object data = Round;
                RoomThread.Trigger(TriggerEvent.RoundStart, this, null, ref data);
            }
        }

        public void RecordSubCards(WrappedCard card)
        {
            card_subcards[card] = new List<int>(card.SubCards);
        }

        public List<int> GetSubCards(WrappedCard card)
        {
            if (card_subcards.ContainsKey(card))
                return new List<int>(card_subcards[card]);

            return new List<int>();
        }

        public void RemoveSubCards(WrappedCard card)
        {
            card_subcards.Remove(card);
        }

        public void AddUseList(CardUseStruct use)
        {
            m_use_list.Add(use);
        }

        public void RemoveUseOnFinish()
        {
            m_use_list.RemoveAt(m_use_list.Count - 1);
        }

        public List<CardUseStruct> GetUseList() => new List<CardUseStruct>(m_use_list);
        public List<JudgeStruct> GetJudgeList() => new List<JudgeStruct>(m_judge_list);
        public void UpdateSubCards(WrappedCard card, List<int> ids)
        {
            card_subcards[card] = new List<int>(ids);
        }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                    _waitHandle.Close();
                    timer.Close();
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~Room() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}



