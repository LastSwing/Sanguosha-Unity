using CommonClassLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using static CommonClass.Game.Player;

namespace CommonClass.Game
{
    public struct DamageStruct
    {
        public enum DamageNature
        {
            Normal, // normal slash, duel and most damage caused by skill
            Fire,  // fire slash, fire attack and few damage skill (Yeyan, etc)
            Thunder // lightning, thunder slash, and few damage skill (Leiji, etc)
        };

        public enum DamageStep
        {
            None,
            Caused,
            Done,
        }

        public DamageStruct(Player from) {
            From = from;
            To = null;
            Card = null;
            Damage = 1;
            this.Nature = DamageNature.Normal;
            Chain = false; ;
            Transfer = false;
            ByUser = true;
            reason = string.Empty;
            TransferReason = string.Empty;
            Prevented = false;
            Steped = DamageStep.None;
            Drank = false;
            Marks = new List<string>();
            ChainStarter = false;
        }

        public DamageStruct(WrappedCard card, Player from, Player to, int damage = 1, DamageNature nature = DamageNature.Normal) {
            From = from;
            To = to;
            Card = card;
            Damage = damage;
            this.Nature = nature;
            Chain = false; ;
            Transfer = false;
            ByUser = true;
            reason = string.Empty;
            TransferReason = string.Empty;
            Prevented = false;
            Steped = DamageStep.None;
            Drank = false;
            Marks = new List<string>();
            ChainStarter = false;
        }

        public DamageStruct(string reason, Player from, Player to, int damage = 1, DamageNature nature = DamageNature.Normal)
        {
            From = from;
            To = to;
            Card = null;
            Damage = damage;
            this.Nature = nature;
            Chain = false; ;
            Transfer = false;
            ByUser = true;
            this.reason = reason;
            TransferReason = string.Empty;
            Prevented = false;
            Steped = DamageStep.None;
            Drank = false;
            Marks = new List<string>();
            ChainStarter = false;
        }

        public Player From { get; set; }
        public Player To { get; set; }
        public WrappedCard Card { get; set; }
        public int Damage { get; set; }
        public DamageNature Nature { get; set; }
        public bool Chain { get; set; }
        public bool Transfer { get; set; }
        public bool ByUser { get; set; }
        public string Reason
        {
            get
            {
                if (!string.IsNullOrEmpty(reason))
                    return reason;
                else if (Card != null)
                    return Card.Name;
                return string.Empty; ;
            }

            set { reason = value; }
        }
        private string reason;
        public string TransferReason { get; set; }
        public bool Prevented { get; set; }
        public DamageStep Steped { get; set; }
        public bool Drank { set; get; }
        public List<string> Marks { set; get; }
        public bool ChainStarter { set; get; }
    }

    public struct RoomSetting
    {
        public int RespondTime;
        public int NullificationRespondTime;
        public bool ForbiddenChat;
        public bool EnableLord;
        public bool EnableFirstReward;
        public List<string> ForbiddenGenerals;
        public List<string> Packages;
        public List<string> SkillModify;
    }

    public struct CardUseStruct
    {
        public enum CardUseReason
        {
            CARD_USE_REASON_UNKNOWN = 0x00,
            CARD_USE_REASON_PLAY = 0x01,
            CARD_USE_REASON_RESPONSE = 0x02,
            CARD_USE_REASON_RESPONSE_USE = 0x12
        }
        public CardUseReason Reason { get; set; }
        public WrappedCard Card { set; get; }
        public Player From { get; set; }
        public List<Player> To { get; set; }
        public bool IsOwnerUse { set; get; }
        public bool AddHistory { set; get; }
        public bool IsHandcard { set; get; }
        public string Pattern { set; get; }
        public bool IsDummy { set; get; }
        public int Drank { set; get; }
        public int ExDamage { set; get; }
        public List<CardBasicEffect> EffectCount { set; get; }
        public UseRespond RespondData { set; get; }

        public CardUseStruct(WrappedCard card)
        {
            Reason = CardUseReason.CARD_USE_REASON_UNKNOWN;
            Card = card;
            From = null;
            To = new List<Player>();
            IsOwnerUse = true;
            IsHandcard = false;
            AddHistory = true;
            Pattern = string.Empty;
            IsDummy = false;
            Drank = 0;
            EffectCount = null;
            ExDamage = 0;
            RespondData = null;
        }
        public CardUseStruct(WrappedCard card, Player from, List<Player> to, bool isOwnerUse = true)
        {
            Reason = CardUseReason.CARD_USE_REASON_UNKNOWN;
            Card = card;
            From = from;
            To = to;
            IsOwnerUse = isOwnerUse;
            IsHandcard = false;
            AddHistory = true;
            Pattern = string.Empty;
            IsDummy = false;
            Drank = 0;
            EffectCount = null;
            ExDamage = 0;
            RespondData = null;
        }
        public CardUseStruct(WrappedCard card, Player from, Player target, bool isOwnerUse = true)
        {
            Reason = CardUseReason.CARD_USE_REASON_UNKNOWN;
            Card = card;
            From = from;
            To = new List<Player> { target };
            IsOwnerUse = isOwnerUse;
            IsHandcard = false;
            AddHistory = true;
            Pattern = string.Empty;
            IsDummy = false;
            Drank = 0;
            EffectCount = null;
            ExDamage = 0;
            RespondData = null;
        }
    }
    public class UseRespond
    {
        public UseRespond(Player from, List<Player> targets, WrappedCard card)
        {
            From = from;
            Targets = targets;
            Card = card;
        }
        public Player From { set; get; }
        public List<Player> Targets { set; get; }
        public WrappedCard Card { set; get; }
    }
    public class CardBasicEffect
    {
        public CardBasicEffect(Player to, int count, int count2, int count3)
        {
            To = to;
            Effect1 = count;
            Effect2 = count2;
            Effect3 = count3;
            Nullified = false;
        }
        public Player To { set; get; }
        public int Effect1 { set; get; }
        public int Effect2 { set; get; }
        public int Effect3 { set; get; }
        public bool Nullified { set; get; }
        public bool Triggered { set; get; }
    }

    public struct CardEffectStruct
    {
        public WrappedCard Card { set; get; }
        public Player From { set; get; }
        public Player To { set; get; }

        public bool Multiple { set; get; } // helper to judge whether the card has multiple targets
                                           // does not make sense if the card inherits SkillCard
        public int Drank { set; get; }
        public int ExDamage { set; get; }
        public CardBasicEffect BasicEffect { set; get; }
        public List<Player> StackPlayers { set; get; }
    }

    public struct SlashEffectStruct
    {
        public int Jink_num { set; get; }
        public WrappedCard Slash { set; get; }
        public WrappedCard Jink { set; get; }
        public Player From { set; get; }
        public Player To { set; get; }
        public int Drank { set; get; }
        public int ExDamage { set; get; }
        public DamageStruct.DamageNature Nature { set; get; }
        public bool Nullified { set; get; }
    };

    public struct CardAskStruct
    {
        public object Data { set; get; }
        public string Pattern { set; get; }
        public string Prompt { set; get; }
        public string Reason { set; get; }
    }

    public struct LogMessage
    {
        public LogMessage(string type)
        {
            Type = type;
            From = null;
            To = null;
            Card_str = string.Empty;
            Arg = string.Empty;
            Arg2 = string.Empty;
        }

        public void SetFrom(Player player){
            From = player.Name;
        }
        public void SetTos(List<Player> to_players)
        {
            To = new List<string>();
            foreach (Player p in to_players)
                To.Add(p.Name);
        }
        public void AddTo(Player player)
        {
            if (!To.Contains(player.Name))
                To.Add(player.Name);
        }
        //QString toString() const;
        //QVariant toVariant(Room* room) const;

        public string Type { set; get; }
        public string From { set; get; }
        public List<String> To { set; get; }
        public string Card_str { set; get; }
        public string Arg { set; get; }
        public string Arg2 { set; get; }
    };

    public struct GeneralSkin
    {
        public string General { set; get; }
        public int SkinId { set; get; }
    }

    public enum MoveReason
    {
        S_REASON_UNKNOWN = 0x00,
        S_REASON_USE = 0x01,
        S_REASON_RESPONSE = 0x02,
        S_REASON_DISCARD = 0x03,
        S_REASON_RECAST = 0x04,          // ironchain etc.
        S_REASON_PINDIAN = 0x05,
        S_REASON_DRAW = 0x06,
        S_REASON_GOTCARD = 0x07,
        S_REASON_SHOW = 0x08,
        S_REASON_TRANSFER = 0x09,
        S_REASON_PUT = 0x0A,
        S_REASON_ANNOUNCE = 0x0B,       //virtual skill card use
        S_REASON_ABOLISH = 0x0C,        //abolish an equip
                                        //subcategory of use
        S_REASON_LETUSE = 0x11,           // use a card when self is not current

        //subcategory of response
        S_REASON_RETRIAL = 0x12,

        //subcategory of discard
        S_REASON_RULEDISCARD = 0x13,       //  discard at one's Player::Discard for gamerule
        S_REASON_THROW = 0x23,             /*  gamerule(dying or punish)
                                                            as the cost of some skills   */
        S_REASON_DISMANTLE = 0x33,         //  one throw card of another

        //subcategory of gotcard
        S_REASON_GIVE = 0x17,             // from one hand to another hand
        S_REASON_EXTRACTION = 0x27,        // from another's place to one's hand
        S_REASON_GOTBACK = 0x37,          // from placetable to hand
        S_REASON_RECYCLE = 0x47,          // from discardpile to hand
        S_REASON_ROB = 0x57,               // got a definite card from other's hand
        S_REASON_PREVIEWGIVE = 0x67,       // give cards after previewing, i.e. Yiji & Miji

        //subcategory of show
        S_REASON_TURNOVER = 0x18,          // show n cards from drawpile
        S_REASON_JUDGE = 0x28,           // show a card from drawpile for judge
        S_REASON_PREVIEW = 0x38,          // Not done yet, plan for view some cards for self only(guanxing yiji miji)
        S_REASON_DEMONSTRATE = 0x48,       // show a card which copy one to move to table
        S_REASON_DELAYTRICK_EFFECT = 0x58,    //delay trick move to table and start judge

        //subcategory of transfer
        S_REASON_SWAP = 0x19,              // exchange card for two players
        S_REASON_OVERRIDE = 0x29,          // exchange cards from cards in game
        S_REASON_EXCHANGE_FROM_PILE = 0x39,// exchange cards from cards moved out of game (for qixing only)

        //subcategory of put
        S_REASON_NATURAL_ENTER = 0x1A,     //  a card with no-owner move into discardpile
                                           //  e.g. delayed trick enters discardpile
        S_REASON_REMOVE_FROM_PILE = 0x2A,  //  cards moved out of game go back into discardpile
        S_REASON_JUDGEDONE = 0x3A,         //  judge card move into discardpile
        S_REASON_CHANGE_EQUIP = 0x4A,     //  replace existed equip
        S_REASON_REMOVE_FROM_GAME = 0x5A, //  remove cards out of game, such as like add to player's plie

        S_MASK_BASIC_REASON = 0x0F,
    }
    public class CardMoveReason
    {
        public MoveReason Reason { set; get; } = MoveReason.S_REASON_UNKNOWN;
        public string PlayerId { set; get; } = null;        // the cause (not the source) of the movement, such as "lusu" when "dimeng", or "zhanghe" when "qiaobian"
        public string TargetId { set; get; } = null;        // To keep this structure lightweight, currently this is only used for UI purpose.
                                                            // It will be set to empty if multiple targets are involved. NEVER use it for trigger condition
                                                            // judgement!!! It will not accurately reflect the real reason.
        public string SkillName { set; get; } = null;       // skill that triggers movement of the cards, such as "longdang", "dimeng"
        public string EventName { set; get; } = null;       // additional arg such as "lebusishu" on top of "S_REASON_JUDGE"
        //public string CardString { set; get; } = null;      // if the card moved by using/responding, then it has this
        public WrappedCard Card { set; get; } = null;      // if the card moved by using/responding, then it has this
        public GeneralSkin General { set; get; } = new GeneralSkin();    //卡牌信息不该由客户端计算

        public CardMoveReason()
        {
            Reason = MoveReason.S_REASON_UNKNOWN;
        }
        public CardMoveReason(MoveReason moveReason, string playerId)
        {
            Reason = moveReason;
            PlayerId = playerId;
        }
        public CardMoveReason(MoveReason moveReason, string playerId, string skillName, string eventName)
        {
            Reason = moveReason;
            PlayerId = playerId;
            SkillName = skillName;
            EventName = eventName;
        }

        public CardMoveReason(MoveReason moveReason, string playerId, string targetId, string skillName, string eventName)
        {
            Reason = moveReason;
            PlayerId = playerId;
            TargetId = targetId;
            SkillName = skillName;
            EventName = eventName;
        }

        //public bool tryParse(const QVariant &);
        //QVariant toVariant() const;

        public override bool Equals(object obj)
        {
            if (!(obj is CardMoveReason other)) return false;
            return Reason == other.Reason
                && PlayerId == other.PlayerId && TargetId == other.TargetId
                && SkillName == other.SkillName
                && EventName == other.EventName;
        }
        public override int GetHashCode()
        {
            return Reason.GetHashCode() * (!string.IsNullOrEmpty(PlayerId) ? PlayerId.GetHashCode() : 10) * (!string.IsNullOrEmpty(TargetId) ? TargetId.GetHashCode() : 11)
                * (!string.IsNullOrEmpty(SkillName) ? SkillName.GetHashCode() : 12) * (!string.IsNullOrEmpty(EventName) ? EventName.GetHashCode() : 13);
        }
    };

    public struct CardMoveReasonStruct
    {
        public MoveReason Reason { set; get; }
        public string PlayerId { set; get; }        // the cause (not the source) of the movement, such as "lusu" when "dimeng", or "zhanghe" when "qiaobian"
        public string TargetId { set; get; }        // To keep this structure lightweight, currently this is only used for UI purpose.
                                                            // It will be set to empty if multiple targets are involved. NEVER use it for trigger condition
                                                            // judgement!!! It will not accurately reflect the real reason.
        public string SkillName { set; get; }       // skill that triggers movement of the cards, such as "longdang", "dimeng"
        public string EventName { set; get; }       // additional arg such as "lebusishu" on top of "S_REASON_JUDGE"
        public string CardString { set; get; }      // if the card moved by using/responding, then it has this
        public GeneralSkin General { set; get; }    //卡牌信息不该由客户端计算
    };

    public struct ClientCardsMoveStruct
    {
        public ClientCardsMoveStruct(int id, Player to, Place to_place, CardMoveReasonStruct reason)
        {
            Card_ids = new List<int> { id };
            From_place = Place.PlaceUnknown;
            To_place = to_place;
            From = string.Empty;
            To = to != null ? to.Name : string.Empty;
            Reason = reason;
            Is_last_handcard = false;
            Open = false;
            From_pile_name = null;
            To_pile_name = null;
            Origin_from_place = Place.PlaceUnknown;
            Origin_to_place = Place.PlaceUnknown;
            Origin_from = null;
            Origin_to = null;
            Origin_from_pile_name = null;
            Origin_to_pile_name = null;
        }
        public List<int> Card_ids { set; get; }
        public Place From_place { set; get; }
        public Place To_place { set; get; }
        public string From_pile_name { set; get; }
        public string To_pile_name { set; get; }
        public string From { set; get; }
        public string To { set; get; }
        public CardMoveReasonStruct Reason { set; get; }
        public bool Open { set; get; } // helper to prevent sending card_id to unrelevant clients
        public bool Is_last_handcard { set; get; }
        public Place Origin_from_place { set; get; }
        public Place Origin_to_place { set; get; }
        public string Origin_from { set; get; }
        public string Origin_to { set; get; }
        public string Origin_from_pile_name { set; get; }
        public string Origin_to_pile_name { set; get; } //for case of the movement transitted
    };

    public struct CardsMoveOneTimeStruct
    {
        public CardsMoveOneTimeStruct(Player from)
        {
            Card_ids = new List<int>();
            From_places = new List<Place>();
            To_place = Place.PlaceUnknown; ;
            Reason = new CardMoveReason(MoveReason.S_REASON_UNKNOWN, string.Empty);
            From = from;
            To = null;
            From_pile_names = new List<string>();
            To_pile_name = string.Empty;
            Origin_from_places = new List<Place>();
            Origin_to_place = Place.PlaceUnknown;
            Origin_from = null;
            Origin_to = null;
            Origin_from_pile_names = new List<string>();
            Origin_to_pile_name = string.Empty;
            Open = new List<bool>();
            Is_last_handcard = false;
        }
        public List<int> Card_ids { set; get; }
        public List<Player.Place> From_places { set; get; }
        public Player.Place To_place { set; get; }
        public CardMoveReason Reason { set; get; }
        public Player From { set; get; }
        public Player To { set; get; }
        public List<string> From_pile_names { set; get; }
        public string To_pile_name { set; get; }
        public List<Player.Place> Origin_from_places { set; get; }
        public Player.Place Origin_to_place { set; get; }
        public Player Origin_from { set; get; }
        public Player Origin_to { set; get; }
        public List<string> Origin_from_pile_names { set; get; }
        public String Origin_to_pile_name { set; get; } //for case of the movement transitted
        public List<bool> Open { set; get; } // helper to prevent sending card_id to unrelevant clients
        public bool Is_last_handcard { set; get; }
    };

    public struct CardsMoveStruct
    {
        public CardsMoveStruct(List<int> ids, Player from, Player to, Place from_place, Place to_place, CardMoveReason reason)
        {
            Card_ids = ids;
            From_place = from_place;
            To_place = to_place;
            From = from != null ? from.Name: string.Empty;
            To = to != null ? to.Name : string.Empty;
            Reason = reason;
            Is_last_handcard = false;

            Open = false;

            From_pile_name = null;
            To_pile_name = null;
            Origin_from_place = Place.PlaceUnknown;
            Origin_to_place = Place.PlaceUnknown;
            Origin_from = null;
            Origin_to = null;
            Origin_from_pile_name = null;
            Origin_to_pile_name = null;
    }

        public CardsMoveStruct(List<int> ids, Player to, Place to_place, CardMoveReason reason)
        {
            Card_ids = ids;
            From_place = Place.PlaceUnknown;
            To_place = to_place;
            From = string.Empty;
            To = to != null ? to.Name : string.Empty;
            Reason = reason;
            Is_last_handcard = false;

            Open = false;

            From_pile_name = null;
            To_pile_name = null;
            Origin_from_place = Place.PlaceUnknown;
            Origin_to_place = Place.PlaceUnknown;
            Origin_from = null;
            Origin_to = null;
            Origin_from_pile_name = null;
            Origin_to_pile_name = null;
        }

        public CardsMoveStruct(int id, Player from, Player to, Place from_place, Place to_place, CardMoveReason reason)
        {
            Card_ids = new List<int> { id };
            From_place = from_place;
            To_place = to_place;
            From = from != null ? from.Name : string.Empty;
            To = to != null ? to.Name : string.Empty;
            Reason = reason;
            Is_last_handcard = false;

            Open = false;

            From_pile_name = null;
            To_pile_name = null;
            Origin_from_place = Place.PlaceUnknown;
            Origin_to_place = Place.PlaceUnknown;
            Origin_from = null;
            Origin_to = null;
            Origin_from_pile_name = null;
            Origin_to_pile_name = null;
        }

        public CardsMoveStruct(int id, Player to, Place to_place, CardMoveReason reason)
        {
            Card_ids = new List<int> { id };
            From_place = Place.PlaceUnknown;
            To_place = to_place;
            From = string.Empty;
            To = to != null ? to.Name : string.Empty;
            Reason = reason;
            Is_last_handcard = false;
            Open = false;
            From_pile_name = null;
            To_pile_name = null;
            Origin_from_place = Place.PlaceUnknown;
            Origin_to_place = Place.PlaceUnknown;
            Origin_from = null;
            Origin_to = null;
            Origin_from_pile_name = null;
            Origin_to_pile_name = null;
        }

        public void Copy(CardsMoveStruct other)
        {
            Card_ids = other.Card_ids;
            From = other.From;
            From_place = other.Origin_from_place;
            From_pile_name = other.From_pile_name;
            To = other.To;
            To_place = other.To_place;
            To_pile_name = other.To_pile_name;
            Reason = other.Reason;
            Open = other.Open;
            Is_last_handcard = other.Is_last_handcard;
            Origin_from = other.Origin_from;
            Origin_from_place = other.Origin_from_place;
            Origin_to_place = other.Origin_to_place;
            Origin_to = other.Origin_to;
            Origin_from_pile_name = other.Origin_from_pile_name;
            Origin_to_pile_name = other.Origin_to_pile_name;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is CardsMoveStruct other)) return false;
            return From == other.From && From_place == other.From_place && From_pile_name == other.From_pile_name;
        }
        public override int GetHashCode()
        {
            return (!string.IsNullOrEmpty(From) ? From.GetHashCode() : 10) * From_place.GetHashCode() * (!string.IsNullOrEmpty(From_pile_name) ? From_pile_name.GetHashCode() : 11);
        }

        //public static bool operator <(CardsMoveStruct origin, CardsMoveStruct other)
        //{
        //    return origin.From < other.From || origin.From_place < other.From_place
        //        || origin.From_pile_name < other.From_pile_name || origin.From_player_name < other.From_player_name;
        //}

        //public static bool operator >(CardsMoveStruct origin, CardsMoveStruct other)
        //{
        //    return From < other.From || From_place < other.From_place
        //        || From_pile_name < other.From_pile_name || From_player_name < other.From_player_name;
        //}

        public List<int> Card_ids { set; get; }
        public Place From_place { set; get; }
        public Place To_place { set; get; }
        public string From_pile_name { set; get; }
        public string To_pile_name { set; get; }
        public string From { set; get; }
        public string To { set; get; }
        public CardMoveReason Reason { set; get; }
        public bool Open { set; get; } // helper to prevent sending card_id to unrelevant clients
        public bool Is_last_handcard { set; get; }
        public Place Origin_from_place { set; get; }
        public Place Origin_to_place { set; get; }
        public string Origin_from { set; get; }
        public string Origin_to { set; get; }
        public string Origin_from_pile_name { set; get; }
        public string Origin_to_pile_name { set; get; } //for case of the movement transitted

        //bool tryParse(const QVariant &arg);
        //QVariant toVariant() const;
    };

    public struct DyingStruct
    {
        //DyingStruct();

        public Player Who { set; get; } // who is ask for help
        public DamageStruct Damage { set; get; } // if it is NULL that means the dying is caused by losing hp
    };

    public struct DeathStruct
    {
        //public DeathStruct();

        public Player Who { set; get; }              // who is dead
        public DamageStruct Damage { set; get; }    // if it is NULL that means the dying is caused by losing hp
    };

    public struct RecoverStruct
    {
        //public RecoverStruct();

        public int Recover { set; get; }
        public Player Who { set; get; }
        public WrappedCard Card { set; get; }
    };

    public struct DrawCardStruct
    {
        public DrawCardStruct(int count, Player who,  string reason)
        {
            Draw = count;
            Who = who;
            Reason = reason;
        }
        public int Draw { set; get; }
        public Player Who { set; get; }
        public string Reason { set; get; }
    }

    public struct PindianStruct
    {
        public PindianStruct(string reason)
        {
            Reason = reason;
            Success = false;
            From = null;
            Tos = new List<Player>();
            From_card = null;
            To_cards = new List<WrappedCard>();
            From_number = 0;
            To_numbers = new List<int>();
            Index = 0;
        }
        public bool Success { set; get; }
        public Player From { set; get; }
        public List<Player> Tos { set; get; }
        public WrappedCard From_card { set; get; }
        public List<WrappedCard> To_cards { set; get; }
        public int From_number { set; get; }
        public List<int> To_numbers { set; get; }
        public string Reason { set; get; }
        public int Index { set; get; }
    };

    public struct JudgeStruct
    {
        //public JudgeStruct();
        public bool IsGood()
        {
            bool effected = IsEffected();
            return Negative != effected;
        }
        public bool IsBad() => !IsGood();
        public bool IsEffected()
        {
            return _m_result == TrialResult.TRIAL_RESULT_GOOD;
        }
        public void UpdateResult(bool effected)
        {
            if (effected)
                _m_result = TrialResult.TRIAL_RESULT_GOOD;
            else
                _m_result = TrialResult.TRIAL_RESULT_BAD;
        }
        public bool Good { set; get; }
        public Player Who { set; get; }
        public WrappedCard Card { set; get; }
        public string Pattern { set; get; }
        public string Reason { set; get; }
        public bool Time_consuming { set; get; }
        public bool Negative { set; get; }
        public bool PlayAnimation { set; get; }
        public int JudgeNumber { set; get; }
        public WrappedCard.CardSuit JudgeSuit { set; get; }

        private enum TrialResult
        {
            TRIAL_RESULT_UNKNOWN,
            TRIAL_RESULT_GOOD,
            TRIAL_RESULT_BAD
        }
        private TrialResult _m_result;
    };

    public struct PhaseChangeStruct
{
        public PlayerPhase From { set; get; }
        public PlayerPhase To { set; get; }
    };

    public struct PhaseStruct
    {
        public PlayerPhase Phase { set; get; }
        public bool Skipped { set; get; }
        public bool Finished { set; get; }
    };

    public struct CardResponseStruct
    {
        public CardResponseStruct(Player from, WrappedCard card)
        {
            From = from;
            Card = card;
            Who = null;
            Use = false;
            Retrial = false;
            Handcard = true;
            Reason = string.Empty;
            Data = null;
        }

        public CardResponseStruct(Player from, WrappedCard card, Player who)
        {
            From = from;
            Card = card;
            Who = who;
            Use = false;
            Retrial = false;
            Handcard = true;
            Reason = string.Empty;
            Data = null;
        }

        public CardResponseStruct(Player from, WrappedCard card, bool isUse)
        {
            From = from;
            Card = card;
            Who = null;
            Use = isUse;
            Retrial = false;
            Handcard = true;
            Reason = string.Empty;
            Data = null;
        }

        public CardResponseStruct(Player from, WrappedCard card, Player who, bool isUse)
        {
            From = from;
            Card = card;
            Who = who;
            Use = isUse;
            Retrial = false;
            Handcard = true;
            Reason = string.Empty;
            Data = null;
        }

        public Player From { set; get; }
        public WrappedCard Card { set; get; }
        public Player Who { set; get; }
        public bool Use { set; get; }
        public bool Handcard { set; get; }
        public string Reason { set; get; }
        public bool Retrial { set; get; }
        public object Data { set; get; }
    };

    public struct PromoteStruct
    {
        //public PromoteStruct();
        //public PromoteStruct(const QString &name, const QString &skill_name, const QString &pattern, CardUseStruct::CardUseReason reason, const QString &skill_position);
        public Player Player { set; get; }
        public string SkillName { set; get; }
        public string Pattern { set; get; }
        public CardUseStruct.CardUseReason Reason { set; get; }
        public string SkillPosition { set; get; }
    };

    public struct InfoStruct
    {
        //public InfoStruct();
        //public InfoStruct(const QString &info, bool head);
        public string Info { set; get; }
        public bool Head { set; get; }
    };

    public struct ScoreStruct
    {
        //public ScoreStruct();

        public string Info { set; get; }
        public List<int> Ids { set; get; }
        public List<Player> Players { set; get; }
        public DamageStruct Damage { set; get; }
        public WrappedCard Card { set; get; }
        public double Score { set; get; }
        public int Draws { set; get; }
        public bool DoDamage { set; get; }
        public double Rate { set; get; }
    };

    public struct GameSetting
    {
        public string Name { set; get; }
        public string PassWord { set; get; }
        public string GameMode { set; get; }
        public int PlayerNum { set; get; }
        public int ControlTime { set; get; }
        public int NullTime { set; get; }
        public bool LordConvert { set; get; }
        public int GeneralCount { set; get; }
        public bool SpeakForbidden { set; get; }
        public bool LuckCard { set; get; }
        public List<string> GeneralPackage { set; get; }
        public List<string> CardPackage { set; get; }

        public float GetCommandTimeout(CommonClassLibrary.CommandType command, ProcessInstanceType instance)
        {
            float timeOut = 0;

            if (command == CommonClassLibrary.CommandType.S_COMMAND_NULLIFICATION)
            {
                timeOut = NullTime * 1000;
            }
            else
            {
                if (command == CommonClassLibrary.CommandType.S_COMMAND_CHOOSE_GENERAL)
                    timeOut = ControlTime * 1500;
                else if (command == CommonClassLibrary.CommandType.S_COMMAND_SKILL_MOVECARDS
                    || command == CommonClassLibrary.CommandType.S_COMMAND_ARRANGE_GENERAL)
                    timeOut = ControlTime * 2000;
                else
                    timeOut = ControlTime * 1000;
            }

            if (timeOut > 0 && instance == ProcessInstanceType.S_SERVER_INSTANCE)
                timeOut += 1500;

            return timeOut;
        }
    }

    public struct GameMode
    {
        public string Name { set; get; }
        public List<int> PlayerNum { set; get; }
        public bool IsScenario { set; get; }
        public List<string> GeneralPackage { set; get; }
        public List<string> CardPackage { set; get; }
    }
    public struct TriggerStruct
    {
        public TriggerStruct(string skill_name)
        {
            SkillName = skill_name;
            Invoker = null;
            Targets = new List<string>();
            SkillOwner = null;
            SkillPosition = null;
            Times = 1;
            ResultTarget = null;
        }
        public TriggerStruct(string skill_name, Player invoker)
        {
            SkillName = skill_name;
            Invoker = invoker.Name;
            Targets = new List<string>();
            SkillOwner = null;
            SkillPosition = null;
            Times = 1;
            ResultTarget = null;
        }
        public TriggerStruct(string skill_name, Player invoker, Player skill_owner)
        {
            SkillName = skill_name;
            Invoker = invoker.Name;
            Targets = new List<string>();
            SkillOwner = skill_owner.Name;
            SkillPosition = null;
            Times = 1;
            ResultTarget = null;
        }
        public TriggerStruct(string skill_name, Player invoker, List<Player> targets)
        {
            SkillName = skill_name;
            Invoker = invoker.Name;
            Targets = new List<string>();
            foreach (Player player in targets)
                Targets.Add(player.Name);
            SkillOwner = null;
            SkillPosition = null;
            Times = 1;
            ResultTarget = null;
        }
        public override bool Equals(object obj)
        {
            if (obj is TriggerStruct other)
            {
                return SkillName == other.SkillName && Invoker == other.Invoker && SkillOwner == other.SkillOwner;
            }

            return false;
        }
        public override int GetHashCode()
        {
            return (!string.IsNullOrEmpty(SkillName) ? SkillName.GetHashCode() : 10) * (!string.IsNullOrEmpty(Invoker) ? Invoker.GetHashCode() : 11)
                * (!string.IsNullOrEmpty(SkillOwner) ? SkillOwner.GetHashCode() : 12);
        }

        public string SkillName { set; get; }
        public string Invoker { set; get; }
        public List<string> Targets { set; get; }
        public string SkillOwner { set; get; }
        public string SkillPosition { set; get; }
        public int Times { set; get; }
        public string ResultTarget { set; get; }
    };
    public struct Operate
    {
        public bool Request { set; get; }
        public string Operator { set; get; }
        public string Prompt { set; get; }
        public int NoticeIndex { set; get; }
        public bool SkillInvoke { set; get; }
        public List<string> HighLightSkills { set; get; }
        public string SkillPosition { set; get; }
        public string SkillOwner { set; get; }
        public List<string> SelectedTargets { set; get; }
        public List<string> AvailableTargets { set; get; }
        public bool OKEnable { set; get; }
        public bool CancelEnable { set; get; }
        public Dictionary<string, List<string>> AvailableEquip { set; get; }
        public Dictionary<string, List<string>> AvailableHead { set; get; }
        public Dictionary<string, List<string>> AvailableDeputy { set; get; }
        public Dictionary<string, List<string>> AvailableCards { set; get; }
        public Dictionary<string, List<string>> SelectedCards { set; get; }
        public Dictionary<string, List<string>> PrependPile { set; get; }
        public Dictionary<string, List<string>> AppendPile { set; get; }
        public List<string> GuhuoCards { set; get; }
        public string Guhuo { set; get; }
        public int GuhuoType { set; get; }
        public List<string> ExInfo { set; get; }
    }

    public struct AskForMoveCardsStruct
    {
        public List<int> Moves { set; get; }
        public List<int> Top { set; get; }
        public List<int> Bottom { set; get; }
        public bool Success { set; get; }
    }

    public struct RoomInfoStruct
    {
        public int Id { set; get; }
        public string Name { get; set; }
        public bool PassWord { set; get; }
        public string Mode { set; get; }
        public bool Started { set; get; }
        public int CurrentPlayers { set; get; }
        public int MaxPlayres { set; get; }
    }

    public struct PindianInfo
    {
        public enum PindianType
        {
            Pindian,
            Show,
        }

        public Player From { set; get; }
        public string Reason { set; get; }
        public Dictionary<Player, WrappedCard> Cards { set; get; }
    }
}
