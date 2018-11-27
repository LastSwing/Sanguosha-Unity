using CommonClass;
using CommonClass.Game;
using SanguoshaServer.Game;
using System.Collections.Generic;
using static SanguoshaServer.Game.FunctionCard;

namespace SanguoshaServer.AI
{
    public class TrustedAI
    {
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
        protected Dictionary<Player, List<string>> player_known = new Dictionary<Player, List<string>>();
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
                friends[p] = new List<Player>();
                enemies[p] = new List<Player>();
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

            if (requestor != self && IsFriend(requestor))
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
            if (IsFriend(dying))
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

    public class SmartAI : TrustedAI
    {
        public SmartAI(Room room, Player player) : base(room, player)
        {
        }
    }
}
