using System;
using System.Collections.Generic;
using CommonClass.Game;
using SanguoshaServer.Game;
using SanguoshaServer.Package;
using static CommonClass.Game.Player;
using static SanguoshaServer.Package.FunctionCard;

namespace SanguoshaServer.AI
{
    public class FamousGeneralAI : AIPackage
    {
        public FamousGeneralAI() : base("FamousGeneral")
        {
            events = new List<SkillEvent>
            {
                new JiushiAI(),
                new LuoyingAI(),
                new HuituoAI(),
                new MingjianAI(),
                new XingshuaiAI(),
                new ZhenlieAI(),
                new MijiAI(),
                new ShangshiAI(),
                new QuanjiAI(),
                new PaiyiAI(),
                new HuomoAI(),
                new ZuodingAI(),

                new ZhichiAI(),
                new MingceAI(),
                new QietingAI(),
                new XianzhouAI(),
                new ZishouAI(),

                new EnJXAI(),
                new YuanJXAI(),
                new XuanhuoJXAI(),
                new ZhimanJXAI(),
                new FumianAI(),
                new DaiyanAI(),
                new QiaoshiAI(),
                new YanyuAI(),
                new XiantuAI(),
                new QiangzhiAI(),

                new AnxuAI(),
                new ZhuiyiAI(),
                new BingyiAI(),
                new ShenxingAI(),
                new BuyiJXAI(),
                new AnguoAI(),
            };

            use_cards = new List<UseCard>
            {
                new MingjianCardAI(),
                new MingceCardAI(),
                new AnxuCardAI(),
                new XianzhouCardAI(),
                new ShenxingCardAI(),
                new YanyuCardAI(),
                new PaiyiCardAI(),
                new AnguoCardAI(),
            };
        }
    }

    public class JiushiAI : SkillEvent
    {
        public JiushiAI() : base("jiushi") { }

        public override List<WrappedCard> GetViewAsCards(TrustedAI ai, string pattern, Player player)
        {
            List<WrappedCard> result = new List<WrappedCard>();
            if (player.FaceUp)
            {
                WrappedCard ana = new WrappedCard(Analeptic.ClassName) { Skill = Name };
                WrappedCard jiushi = new WrappedCard(JiushiCard.ClassName) { Skill = Name };
                ana.UserString = RoomLogic.CardToString(ai.Room, jiushi);
                result.Add(ana);
            }

            return result;
        }

        public override double UseValueAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return -7;
        }
    }

    public class LuoyingAI : SkillEvent
    {
        public LuoyingAI() : base("luoying") { }

        public override AskForMoveCardsStruct OnMoveCards(TrustedAI ai, Player player, List<int> ups, List<int> downs, int min, int max)
        {
            AskForMoveCardsStruct move = new AskForMoveCardsStruct
            {
                Success = true,
                Top = ups,
                Bottom = downs
            };
            return move;
        }
    }

    public class HuituoAI : SkillEvent
    {
        public HuituoAI() : base("huituo") { key = new List<string> { "playerChosen" }; }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str && player != ai.Self)
            {
                List<string> strs = new List<string>(str.Split(':'));
                if (strs[1] == Name)
                {
                    Room room = ai.Room;
                    Player target = room.FindPlayer(strs[2]);
                    if (ai.GetPlayerTendency(target) != "unknown") ai.UpdatePlayerRelation(player, target, true);
                }
            }
        }
        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            List<Player> friends = ai.GetFriends(player);
            ai.SortByDefense(ref friends, false);
            return new List<Player> { friends[0] };
        }

        public override ScoreStruct GetDamageScore(TrustedAI ai, DamageStruct damage)
        {
            ScoreStruct score = new ScoreStruct
            {
                Score = ai.HasSkill(Name, damage.To) ? 1.5 : 0
            };
            return score;
        }
    }

    public class MingjianAI : SkillEvent
    {
        public MingjianAI() : base("mingjian") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.HasUsed(MingjianCard.ClassName) && !player.IsKongcheng())
                return new List<WrappedCard> { new WrappedCard(MingjianCard.ClassName) { Skill = Name } };
            return new List<WrappedCard>();
        }
    }

    public class MingjianCardAI : UseCard
    {
        public MingjianCardAI() : base(MingjianCard.ClassName) { }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use && player != ai.Self)
            {
                Player target = use.To[0];
                if (ai.GetPlayerTendency(target) != "unknown") ai.UpdatePlayerRelation(player, target, true);
            }
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            List<Player> friends = ai.FriendNoSelf;
            Room room = ai.Room;
            room.SortByActionOrder(ref friends);
            foreach (Player p in friends)
            {
                if (!ai.WillSkipPlayPhase(p))
                {
                    card.AddSubCards(player.GetCards("h"));
                    use.Card = card;
                    use.To.Add(p);
                    return;
                }
            }
        }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 0;
        }
    }

    public class XingshuaiAI : SkillEvent
    {
        public XingshuaiAI() : base("xingshuai") { }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is string str && str.StartsWith("@xingshuai-src"))
            {
                Room room = ai.Room;
                string[] strs = str.Split(':');
                Player target = room.FindPlayer(strs[1]);
                if (target.GetRoleEnum() == Player.PlayerRole.Lord)
                {
                    if (player.GetRoleEnum() == Player.PlayerRole.Loyalist) return true;
                    if (player.GetRoleEnum() == Player.PlayerRole.Renegade && room.GetAlivePlayers().Count > 2
                        && ai.GetKnownCardsNums(Analeptic.ClassName, "he", target, player) + ai.GetKnownCardsNums(Peach.ClassName, "he", target, player) == 0
                        && player.Hp > 1) return true;
                }
            }
            else
                return true;

            return false;
        }
    }

    public class ZhenlieAI : SkillEvent
    {
        public ZhenlieAI() : base("zhenlie") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            Room room = ai.Room;
            if (room.GetTag(Name) is CardUseStruct use)
            {
                if (!ai.IsCardEffect(use.Card, player, use.From)) return false;

                if (use.Card.Name.Contains(Slash.ClassName) || use.Card.Name == Duel.ClassName || use.Card.Name == ArcheryAttack.ClassName || use.Card.Name == SavageAssault.ClassName)
                {
                    bool avoid = false;
                    if (use.Card.Name.Contains(Slash.ClassName))
                    {
                        int index = use.To.IndexOf(player);
                        if ((use.EffectCount == null || use.EffectCount.Count <= index || use.EffectCount[index].Count == 1) && ai.GetKnownCardsNums(Jink.ClassName, "he", player) > 0)
                            avoid = true;
                    }
                    else if (use.Card.Name == ArcheryAttack.ClassName && ai.GetKnownCardsNums(Jink.ClassName, "he", player) > 0)
                        avoid = true;
                    else if (use.Card.Name == SavageAssault.ClassName && ai.GetKnownCardsNums(Slash.ClassName, "he", player) > 0)
                        avoid = true;
                    else if (use.Card.Name == Duel.ClassName && ai.GetKnownCardsNums(Slash.ClassName, "he", player) > 2 && !ai.HasSkill("wushuang", use.From))
                        avoid = true;

                    if (!avoid)
                    {
                        DamageStruct damage = new DamageStruct(use.Card, use.From, player, 1 + use.Drank);
                        if (use.Card.Name == FireSlash.ClassName)
                            damage.Nature = DamageStruct.DamageNature.Fire;
                        else if (damage.Card.Name == ThunderSlash.ClassName)
                            damage.Nature = DamageStruct.DamageNature.Thunder;

                        ScoreStruct score = ai.GetDamageScore(damage);
                        if (score.Score < -4 && !ai.IsFriend(use.From))
                            return true;
                        else if (ai.IsFriend(use.From) && score.Score < -20 && player.Hp == 1 && use.From.GetRoleEnum() == Player.PlayerRole.Lord)
                            return true;
                    }
                }
                else if (ai.IsFriend(use.From))
                {
                    return false;
                }
                else if (use.Card.Name == Snatch.ClassName && player.Hp > 1)
                    return true;
            }

            return false;
        }
    }

    public class MijiAI : SkillEvent
    {
        public MijiAI() : base("miji") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class ShangshiAI : SkillEvent
    {
        public ShangshiAI() : base("shangshi") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class QuanjiAI : SkillEvent
    {
        public QuanjiAI() : base("quanji") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            List<int> ids = player.GetCards("h");
            ai.SortByKeepValue(ref ids, false);
            return new List<int> { ids[0] };
        }
    }

    public class PaiyiAI : SkillEvent
    {
        public PaiyiAI() : base("paiyi")
        { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            List<WrappedCard> result = new List<WrappedCard>();
            if (player.GetPile("quanji").Count > 0 && !player.HasUsed(PaiyiCard.ClassName))
            {
                WrappedCard card = new WrappedCard(PaiyiCard.ClassName) { Skill = Name };
                card.AddSubCard(player.GetPile("quanji")[0]);
                result.Add(card);
            }

            return result;
        }
    }

    public class PaiyiCardAI : UseCard
    {
        public PaiyiCardAI() : base(PaiyiCard.ClassName) { }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use && player != ai.Self)
            {
                Player target = use.To[0];
                if (target.HandcardNum + 2 <= player.HandcardNum && player != target && ai.GetPlayerTendency(target) != "unknown")
                    ai.UpdatePlayerRelation(player, target, true);
            }
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            use.Card = card;
            use.To.Add(player);
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 6;
        }
    }

    public class ZuodingAI : SkillEvent
    {
        public ZuodingAI() : base("zuoding") { key = new List<string> { "playerChosen" }; }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str && player != ai.Self)
            {
                string[] strs = str.Split(':');
                if (strs[1] == Name)
                {
                    Room room = ai.Room;
                    Player target = room.FindPlayer(strs[2]);
                    if (ai.GetPlayerTendency(target) != "unknown")
                        ai.UpdatePlayerRelation(player, target, true);
                }
            }
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            ai.SortByDefense(ref targets, false);
            foreach (Player p in targets)
                if (ai.IsFriend(p))
                    return new List<Player> { p };

            return new List<Player>();
        }

        public override double TargetValueAdjust(TrustedAI ai, WrappedCard card, Player from, List<Player> targets, Player to)
        {
            Room room = ai.Room;
            Player zhongyao = RoomLogic.FindPlayerBySkillName(room, Name);
            if (!(Engine.GetFunctionCard(card.Name) is DelayedTrick) && card.Suit == WrappedCard.CardSuit.Spade
                && !room.ContainsTag(Name) && zhongyao != null && ai.IsFriend(zhongyao, to))
            {
                return ai.IsFriend(to) ? 1.5 : -1;
            }

            return 0;
        }
    }

    public class HuomoAI : SkillEvent
    {
        public HuomoAI() : base("huomo") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            List<int> ids = new List<int>();
            Room room = ai.Room;
            foreach (int id in player.GetCards("he"))
            {
                WrappedCard card = room.GetCard(id);
                if (!(Engine.GetFunctionCard(card.Name) is BasicCard) && WrappedCard.IsBlack(card.Suit))
                    ids.Add(id);
            }
            List<WrappedCard> result = new List<WrappedCard>();
            if (ids.Count > 0)
            {
                int sub = -1;
                List<double> values = ai.SortByKeepValue(ref ids, false);
                if (values[0] < 0) sub = ids[0];
                else
                {
                    ai.SortByUseValue(ref ids, false);
                    sub = ids[0];
                }

                WrappedCard huomo = new WrappedCard(HuomoCard.ClassName) { Skill = Name };
                huomo.AddSubCard(sub);

                if (!player.HasFlag("huomo_Slash"))
                {
                    WrappedCard slash = new WrappedCard(Slash.ClassName) { Skill = Name };
                    slash.AddSubCard(sub);
                    huomo.UserString = Slash.ClassName;
                    slash.UserString = RoomLogic.CardToString(room, huomo);

                    result.Add(slash);
                }

                if (!player.HasFlag("huomo_Peach"))
                {
                    WrappedCard slash = new WrappedCard(Peach.ClassName) { Skill = Name };
                    slash.AddSubCard(sub);
                    huomo.UserString = Peach.ClassName;
                    slash.UserString = RoomLogic.CardToString(room, huomo);

                    result.Add(slash);
                }
            }

            return result;
        }

        public override List<WrappedCard> GetViewAsCards(TrustedAI ai, string pattern, Player player)
        {
            List<int> ids = new List<int>();
            Room room = ai.Room;
            List<WrappedCard> result = new List<WrappedCard>();
            if (room.GetRoomState().GetCurrentCardUseReason() == CardUseStruct.CardUseReason.CARD_USE_REASON_RESPONSE_USE
                    || room.GetRoomState().GetCurrentCardUseReason() == CardUseStruct.CardUseReason.CARD_USE_REASON_PLAY)
            {
                foreach (int id in player.GetCards("he"))
                {
                    WrappedCard card = room.GetCard(id);
                    if (!(Engine.GetFunctionCard(card.Name) is BasicCard) && WrappedCard.IsBlack(card.Suit))
                        ids.Add(id);
                }
                if (ids.Count > 0)
                {
                    if (pattern == Slash.ClassName && !player.HasFlag("huomo_Slash"))
                    {
                        int sub = -1;
                        List<double> values = ai.SortByKeepValue(ref ids, false);
                        if (values[0] < 0)
                            sub = ids[0];
                        else
                        {
                            ai.SortByUseValue(ref ids, false);
                            sub = ids[0];
                        }

                        WrappedCard huomo = new WrappedCard(HuomoCard.ClassName) { Skill = Name };
                        huomo.AddSubCard(sub);

                        WrappedCard slash = new WrappedCard(Slash.ClassName) { Skill = Name };
                        slash.AddSubCard(sub);
                        huomo.UserString = Slash.ClassName;
                        slash.UserString = RoomLogic.CardToString(room, huomo);
                        result.Add(slash);

                        WrappedCard fslash = new WrappedCard(FireSlash.ClassName) { Skill = Name };
                        fslash.AddSubCard(sub);
                        huomo.UserString = FireSlash.ClassName;
                        fslash.UserString = RoomLogic.CardToString(room, huomo);
                        result.Add(fslash);

                        WrappedCard tslash = new WrappedCard(ThunderSlash.ClassName) { Skill = Name };
                        tslash.AddSubCard(sub);
                        huomo.UserString = ThunderSlash.ClassName;
                        tslash.UserString = RoomLogic.CardToString(room, huomo);
                        result.Add(tslash);
                    }
                    else if (pattern == Analeptic.ClassName && !player.HasFlag("huomo_Analeptic"))
                    {
                        int sub = -1;
                        List<double> values = ai.SortByKeepValue(ref ids, false);
                        if (values[0] < 0)
                            sub = ids[0];
                        else
                        {
                            ai.SortByUseValue(ref ids, false);
                            sub = ids[0];
                        }

                        WrappedCard huomo = new WrappedCard(HuomoCard.ClassName) { Skill = Name };
                        huomo.AddSubCard(sub);

                        WrappedCard slash = new WrappedCard(Analeptic.ClassName) { Skill = Name };
                        slash.AddSubCard(sub);
                        huomo.UserString = Analeptic.ClassName;
                        slash.UserString = RoomLogic.CardToString(room, huomo);

                        result.Add(slash);
                    }
                    else if (pattern == Peach.ClassName && !player.HasFlag("huomo_Peach"))
                    {
                        int sub = -1;
                        List<double> values = ai.SortByKeepValue(ref ids, false);
                        if (values[0] < 0)
                            sub = ids[0];
                        else
                        {
                            ai.SortByUseValue(ref ids, false);
                            sub = ids[0];
                        }

                        WrappedCard huomo = new WrappedCard(HuomoCard.ClassName) { Skill = Name };
                        huomo.AddSubCard(sub);

                        WrappedCard slash = new WrappedCard(Peach.ClassName) { Skill = Name };
                        slash.AddSubCard(sub);
                        huomo.UserString = Peach.ClassName;
                        slash.UserString = RoomLogic.CardToString(room, huomo);

                        result.Add(slash);
                    }
                    else if (pattern == Jink.ClassName && !player.HasFlag("huomo_Jink"))
                    {
                        int sub = -1;
                        List<double> values = ai.SortByKeepValue(ref ids, false);
                        if (values[0] < 0)
                            sub = ids[0];
                        else
                        {
                            ai.SortByUseValue(ref ids, false);
                            sub = ids[0];
                        }

                        WrappedCard huomo = new WrappedCard(HuomoCard.ClassName) { Skill = Name };
                        huomo.AddSubCard(sub);

                        WrappedCard slash = new WrappedCard(Jink.ClassName) { Skill = Name };
                        slash.AddSubCard(sub);
                        huomo.UserString = Jink.ClassName;
                        slash.UserString = RoomLogic.CardToString(room, huomo);

                        result.Add(slash);
                    }
                }
            }

            return result;
        }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, bool isUse, Place place)
        {
            if (ai.HasSkill(Name, player) && !(Engine.GetFunctionCard(ai.Room.GetCard(card.GetEffectiveId()).Name) is BasicCard)
                && WrappedCard.IsBlack(ai.Room.GetCard(card.GetEffectiveId()).Suit))
            {
                return 2;
            }

            return 0;
        }

        public override double UseValueAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return -2;
        }
    }


    public class ZhichiAI : SkillEvent
    {
        public ZhichiAI() : base("zhichi")
        {
        }

        public override bool IsCardEffect(TrustedAI ai, WrappedCard card, Player from, Player to)
        {
            if (to != null && to.GetMark("@late") > 0)
            {
                Room room = ai.Room;
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                if (fcard is Slash || (fcard is TrickCard && !(fcard is DelayedTrick)))
                    return false;
            }

            return true;
        }
    }

    public class MingceAI : SkillEvent
    {
        public MingceAI() : base("mingce")
        {
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.HasUsed(MingceCard.ClassName))
                return new List<WrappedCard> { new WrappedCard(MingceCard.ClassName) { Skill = Name } };

            return new List<WrappedCard>();
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            Room room = ai.Room;
            Player target = null;
            foreach (Player p in room.GetAlivePlayers())
            {
                if (p.HasFlag("MingceTarget"))
                {
                    target = p;
                    break;
                }
            }
            WrappedCard slash = new WrappedCard(Slash.ClassName);
            List<ScoreStruct> scores = ai.CaculateSlashIncome(player, new List<WrappedCard> { slash }, new List<Player> { target }, false);
            if (scores.Count > 0 && scores[0].Score > 1.5)
                return "use";

            return "draw";
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            if (ai.Target[Name] != null && targets.Contains(ai.Target[Name]))
                return new List<Player> { ai.Target[Name] };

            return new List<Player>();
        }
    }

    public class MingceCardAI : UseCard
    {
        public MingceCardAI() : base(MingceCard.ClassName)
        {
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.Target["mingce"] = null;
            ai.Number[Name] = 5;
            List<Player> enemies = ai.GetEnemies(player), friends = ai.FriendNoSelf;
            ai.SortByDefense(ref enemies, false);
            ai.SortByDefense(ref friends, false);
            Room room = ai.Room;

            List<int> ids = new List<int>();
            foreach (int id in player.GetCards("he"))
            {
                WrappedCard wrapped = room.GetCard(id);
                FunctionCard fcard = Engine.GetFunctionCard(wrapped.Name);
                if (fcard is Slash || fcard is EquipCard)
                    ids.Add(id);
            }
            if (ids.Count > 0 && friends.Count > 0)
            {
                int sub = -1;
                List<double> values = ai.SortByKeepValue(ref ids, false);
                if (values[0] < 0)
                    sub = ids[0];

                WrappedCard slash = new WrappedCard(Slash.ClassName);
                List<ScoreStruct> scores = new List<ScoreStruct>();
                foreach (Player p in friends)
                {
                    foreach (Player enemy in enemies)
                    {
                        if (RoomLogic.InMyAttackRange(room, p, enemy, slash) && RoomLogic.IsProhibited(room, p, enemy, slash) == null)
                        {
                            ScoreStruct score = new ScoreStruct
                            {
                                Players = new List<Player> { p },
                                Card = slash,
                            };

                            DamageStruct damage = new DamageStruct(slash, p, enemy);
                            if (ai.HasArmorEffect(enemy, Vine.ClassName) && slash.Name == Slash.ClassName && p.HasWeapon(Fan.ClassName))
                            {
                                WrappedCard fan = new WrappedCard(FireSlash.ClassName);
                                fan.AddSubCard(slash);
                                fan = RoomLogic.ParseUseCard(room, fan);
                                damage.Card = fan;
                            }

                            if (damage.Card.Name == FireSlash.ClassName)
                                damage.Nature = DamageStruct.DamageNature.Fire;
                            else if (damage.Card.Name == ThunderSlash.ClassName)
                                damage.Nature = DamageStruct.DamageNature.Thunder;

                            ScoreStruct damage_score = ai.GetDamageScore(damage);
                            if (damage_score.Score > 0)
                            {
                                ScoreStruct effect = ai.SlashIsEffective(damage.Card, p, enemy);
                                if (effect.Score > 0)
                                {
                                    score.Score = effect.Score;
                                    if (effect.Rate > 0)
                                    {
                                        score.Score += Math.Min(1, effect.Rate) * damage_score.Score;
                                        score.Damage = damage;
                                        score.DoDamage = damage_score.DoDamage;
                                        scores.Add(score);
                                    }
                                }
                            }
                        }
                    }
                }

                if (scores.Count > 0)
                {
                    scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                    if (scores[0].Score > 2)
                    {
                        card.AddSubCard(sub);
                        use.Card = card;
                        use.To = scores[0].Players;
                        ai.Target["mingce"] = scores[0].Damage.To;
                        if (sub > -1)
                        {
                            use.Card.AddSubCard(sub);
                            return;
                        }

                    }
                }

                List<Player> targets = use.To.Count > 0 ? new List<Player>(use.To) : ai.FriendNoSelf;
                KeyValuePair<Player, int> pair = ai.GetCardNeedPlayer(ids, targets);
                if (pair.Key != null && pair.Value > -1)
                {
                    card.AddSubCard(pair.Value);
                    use.Card = card;
                    if (use.To.Count == 0)
                        use.To.Add(pair.Key);
                    return;
                }

                if (ai.GetOverflow(player) > 0)
                {
                    ai.Number[Name] = 0.5;
                    ai.SortByUseValue(ref ids, false);
                    card.AddSubCard(ids[0]);
                    use.Card = card;
                    use.To.Add(friends[0]);
                }
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return ai.Number[Name];
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use && player != ai.Self)
            {
                Player target = use.To[0];
                if (ai.GetPlayerTendency(target) != "unknown") ai.UpdatePlayerRelation(player, target, true);
            }
        }
    }

    public class QietingAI : SkillEvent
    {
        public QietingAI() : base("qieting")
        {
            key = new List<string> { "cardChosen" };
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str && player != ai.Self)
            {
                Room room = ai.Room;
                List<string> strs = new List<string>(str.Split(':'));
                if (strs[1] == Name)
                {
                    int id = int.Parse(strs[2]);
                    Player target = room.FindPlayer(strs[3]);
                    if (ai.GetPlayerTendency(target) != "unknown")
                        ai.UpdatePlayerRelation(player, target, ai.GetKeepValue(id, target) > 0);
                }
            }
        }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            Room room = ai.Room;
            Player target = room.Current;
            if (ai.IsFriend(target))
            {
                for (int i = 0; i < 6; i++)
                {
                    if (target.GetEquip(i) >= 0 && ai.GetKeepValue(target.GetEquip(i), target, Player.Place.PlaceEquip) < 0
                        && player.GetEquip(i) < 0 && RoomLogic.CanPutEquip(player, room.GetCard(target.GetEquip(i))))
                        return "getequipc";
                }

                return "draw";
            }
            else if (ai.IsEnemy(target))
            {
                for (int i = 0; i < 6; i++)
                {
                    if (target.GetEquip(i) >= 0 && ai.GetKeepValue(target.GetEquip(i), target, Player.Place.PlaceEquip) > 0
                        && player.GetEquip(i) < 0 && RoomLogic.CanPutEquip(player, room.GetCard(target.GetEquip(i))))
                        return "getequipc";
                }

                return "draw";
            }

            return "getequipc";
        }

        public override List<int> OnCardsChosen(TrustedAI ai, Player from, Player to, string flags, int min, int max, List<int> disable_ids)
        {
            Room room = ai.Room;
            if (ai.IsFriend(to))
            {
                for (int i = 0; i < 6; i++)
                {
                    if (to.GetEquip(i) >= 0 && ai.GetKeepValue(to.GetEquip(i), to, Player.Place.PlaceEquip) < 0
                        && from.GetEquip(i) < 0 && RoomLogic.CanPutEquip(from, room.GetCard(to.GetEquip(i))))
                        return new List<int> { to.GetEquip(i) };
                }
            }
            else if (ai.IsEnemy(to))
            {
                if (to.GetTreasure() && !from.GetTreasure() && RoomLogic.CanPutEquip(from, room.GetCard(to.Treasure.Key)))
                    return new List<int> { to.Treasure.Key };
                if (to.GetArmor() && !from.GetArmor() && RoomLogic.CanPutEquip(from, room.GetCard(to.Armor.Key)))
                    return new List<int> { to.Armor.Key };
                if (to.GetDefensiveHorse() && !from.GetDefensiveHorse() && RoomLogic.CanPutEquip(from, room.GetCard(to.DefensiveHorse.Key)))
                    return new List<int> { to.DefensiveHorse.Key };
                if (to.GetWeapon() && !from.GetWeapon() && RoomLogic.CanPutEquip(from, room.GetCard(to.Weapon.Key)))
                    return new List<int> { to.Weapon.Key };
                if (to.GetOffensiveHorse() && !from.GetOffensiveHorse() && RoomLogic.CanPutEquip(from, room.GetCard(to.OffensiveHorse.Key)))
                    return new List<int> { to.OffensiveHorse.Key };
            }

            return base.OnCardsChosen(ai, from, to, flags, min, max, disable_ids);
        }
    }

    public class XianzhouAI : SkillEvent
    {
        public XianzhouAI() : base("xianzhou")
        {
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (player.GetEquips().Count > 0 && player.GetMark("@handover") > 0)
            {
                Room room = ai.Room;
                foreach (int id in player.GetCards("h"))
                {
                    WrappedCard card = room.GetCard(id);
                    FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                    if (fcard is EquipCard && ai.GetSameEquip(card, player) == null && RoomLogic.CanPutEquip(player, card))
                        return new List<WrappedCard>();
                }

                return new List<WrappedCard> { new WrappedCard(XianzhouCard.ClassName) { Skill = Name, Mute = true } };
            }

            return new List<WrappedCard>();
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            Room room = ai.Room;
            Player caifuren = null;
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (p.HasFlag("xianzhou_source"))
                {
                    caifuren = p;
                    break;
                }
            }
            int count = player.GetMark("xianzhou");

            double max = 0;
            Dictionary<Player, double> _points = new Dictionary<Player, double>();
            foreach (Player _p in room.GetOtherPlayers(player))
            {
                if (!RoomLogic.InMyAttackRange(room, player, _p)) continue;
                DamageStruct damage = new DamageStruct("xianzhou", player, _p);
                double value = ai.GetDamageScore(damage).Score;
                if (value > 0)
                    _points[_p] = value;
            }

            List<Player> targets = new List<Player>(_points.Keys);
            if (targets.Count > 0)
            {
                targets.Sort((x, y) => { return _points[x] > _points[y] ? -1 : 1; });

                for (int i = 0; i < Math.Min(targets.Count, count); i++)
                    max += _points[targets[i]];
            }

            double recover = 0;
            if (caifuren.Alive && ai.IsFriend(caifuren))
                recover = 3 * Math.Min(caifuren.GetLostHp(), count);

            if (max > recover) return "damage";

            return "recover";
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> choose, int min, int max)
        {
            List<Player> result = new List<Player>();
            Room room = ai.Room;
            Dictionary<Player, double> _points = new Dictionary<Player, double>();
            foreach (Player _p in choose)
            {
                if (!RoomLogic.InMyAttackRange(room, player, _p)) continue;
                DamageStruct damage = new DamageStruct("xianzhou", player, _p);
                double value = ai.GetDamageScore(damage).Score;
                if (value > 0)
                    _points[_p] = value;
            }

            List<Player> targets = new List<Player>(_points.Keys);
            if (targets.Count > 0)
            {
                targets.Sort((x, y) => { return _points[x] > _points[y] ? -1 : 1; });

                for (int i = 0; i < Math.Min(targets.Count, max); i++)
                    result.Add(targets[i]);
            }

            return result;
        }
    }

    public class ZishouAI : SkillEvent
    {
        public ZishouAI() : base("zishou") { }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is int n && player.HandcardNum + n + Zishou.GetAliveKingdoms(ai.Room) <= RoomLogic.GetMaxCards(ai.Room, player) + 1)
                return true;

            return false;
        }
    }

    public class XianzhouCardAI : UseCard
    {
        public XianzhouCardAI() : base(XianzhouCard.ClassName)
        {
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            bool recover = false;
            if (card.SubCards.Count > 1 && player.GetLostHp() > 1) recover = true;
            Room room = ai.Room;
            List<Player> friends = ai.FriendNoSelf;
            ai.SortByDefense(ref friends, false);
            if (recover && friends.Count > 0)
            {
                use.Card = card;
                use.To.Add(friends[0]);
                return;
            }

            Dictionary<Player, double> points = new Dictionary<Player, double>();
            foreach (Player p in friends)
            {
                Dictionary<Player, double> _points = new Dictionary<Player, double>();
                foreach (Player _p in room.GetOtherPlayers(p))
                {
                    if (!RoomLogic.InMyAttackRange(room, p, _p)) continue;
                    DamageStruct damage = new DamageStruct("xianzhou", p, _p);
                    double value = ai.GetDamageScore(damage).Score;
                    if (value > 0)
                        _points[_p] = value;
                }

                List<Player> targets = new List<Player>(_points.Keys);
                if (targets.Count > 0)
                {
                    targets.Sort((x, y) => { return _points[x] > _points[y] ? -1 : 1; });
                    double max = 0;
                    for (int i = 0; i < Math.Min(targets.Count, card.SubCards.Count); i++)
                        max += _points[targets[i]];

                    points[p] = max;
                }
            }

            List<Player> _targets = new List<Player>(points.Keys);
            if (_targets.Count > 0)
            {
                _targets.Sort((x, y) => { return points[x] > points[y] ? -1 : 1; });
                if (points[_targets[0]] > 10)
                {
                    use.Card = card;
                    use.To.Add(_targets[0]);
                }
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 4;
        }
    }

    public class YuanJXAI : SkillEvent
    {
        public YuanJXAI() : base("enyuan_jx_yuan")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return data is Player target && !ai.IsFriend(target);
        }
    }
    public class EnJXAI : SkillEvent
    {
        public EnJXAI() : base("enyuan_jx_en")
        {
            key = new List<string> { "skillInvoke" };
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str && player != ai.Self)
            {
                List<string> strs = new List<string>(str.Split(':'));
                if (strs[1] == Name && strs[2] == "yes")
                {
                    Player target = null;
                    foreach (Player p in ai.Room.GetAlivePlayers())
                    {
                        if (p.HasFlag("en_target"))
                        {
                            target = p;
                            break;
                        }
                    }
                    if (ai.GetPlayerTendency(target) != "unknown") ai.UpdatePlayerRelation(player, target, true);
                }
            }
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return data is Player target && ai.IsFriend(target);
        }
    }

    public class XuanhuoJXAI : SkillEvent
    {
        public XuanhuoJXAI() : base("xuanhuo_jx")
        {
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            Player target = null;
            Room room = ai.Room;
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (p.HasFlag("xuanhuo_source"))
                {
                    target = p;
                    break;
                }
            }

            if (target == null)
            {
                List<Player> friends = ai.FriendNoSelf;
                if (friends.Count > 0)
                {
                    ai.SortByDefense(ref friends, false);
                    foreach (Player p in friends)
                    {
                        if (ai.HasSkill("tuntian", p))
                            return new List<Player> { p };
                    }

                    foreach (Player p in friends)
                    {
                        foreach (int id in p.GetEquips())
                        {
                            if (ai.GetKeepValue(id, p, Player.Place.PlaceEquip) < 0)
                                return new List<Player> { p };
                        }
                    }

                    return new List<Player> { friends[0] };
                }
            }
            else
            {
                return new List<Player> { targets[0] };
            }

            return new List<Player>();
        }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct
            {
                From = player
            };
            string[] strs = prompt.Split(':');
            Room room = ai.Room;
            Player source = room.FindPlayer(strs[1], true);
            Player victim = room.FindPlayer(strs[2], true);
            if (!ai.IsFriend(source))
            {
                List<ScoreStruct> scores = ai.CaculateSlashIncome(player, null, new List<Player> { victim }, false);
                if (scores.Count > 0 && scores[0].Score >= 0 && scores[0].Card != null)
                {
                    use.Card = scores[0].Card;
                    use.To = scores[0].Players;
                }
            }

            return use;
        }
    }

    public class ZhimanJXAI : SkillEvent
    {
        public ZhimanJXAI() : base("zhiman_jx")
        {
            key = new List<string> { "skillInvoke", "cardChosen" };
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str && player != ai.Self && ai is StupidAI _ai)
            {
                string[] strs = str.Split(':');
                Room room = ai.Room;
                if (strs[0] == "skillInvoke" && strs[1] == Name && room.GetTag("zhiman_data") is DamageStruct damage)
                {
                    Player target = damage.To;
                    if (ai.GetPlayerTendency(target) != "unknown")
                    {
                        bool good = target.JudgingArea.Count > 0;
                        if (!good)
                        {
                            foreach (int id in target.GetEquips())
                            {
                                if (ai.GetKeepValue(id, target, Place.PlaceEquip) < 0)
                                {
                                    good = true;
                                    break;
                                }
                            }
                        }
                        if (good && strs[2] == "yes")
                            return;
                        else if (_ai.NeedDamage(damage) && !good)
                        {
                            ai.UpdatePlayerRelation(player, target, strs[2] == "no");
                        }
                        else if (player.GetCards("ej").Count == 0 && strs[2] == "yes")
                            ai.UpdatePlayerRelation(player, target, true);
                    }
                }
                else if (strs[0] == "cardChosen" && strs[1] == Name)
                {
                    int card_id = int.Parse(strs[2]);
                    Player target = room.FindPlayer(strs[4]);

                    if (room.GetCardPlace(card_id) == Place.PlaceJudge)
                    {
                        if (room.GetCard(card_id).Name != Lightning.ClassName)
                            ai.UpdatePlayerRelation(player, target, true);
                        else
                        {
                            Player winner = ai.GetWizzardRaceWinner(room.GetCard(card_id).Name, target, target);
                            if (winner != null && ai.IsFriend(winner, target))
                                ai.UpdatePlayerRelation(player, target, false);
                            else
                                ai.UpdatePlayerRelation(player, target, true);
                        }
                    }
                    else if (room.GetCardPlace(card_id) == Place.PlaceEquip && ai.GetKeepValue(card_id, target, Place.PlaceEquip) > 0)
                    {
                        ai.UpdatePlayerRelation(player, target, true);
                    }
                }
            }
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            Room room = ai.Room;
            if ((ai.WillShowForAttack() || ai.WillShowForDefence())
                && room.ContainsTag("zhiman_data") && room.GetTag("zhiman_data") is DamageStruct damage)
            {
                ScoreStruct get = ai.FindCards2Discard(player, damage.To, string.Empty, "ej", HandlingMethod.MethodGet);
                ScoreStruct score = ai.GetDamageScore(damage);

                //if (get.Score < score.Score)
                //{
                //    Debug.Assert(ai.IsEnemy(damage.To));
                //}

                return get.Score >= score.Score;
            }

            return false;
        }
    }

    public class AnxuAI : SkillEvent
    {
        public AnxuAI() : base("anxu")
        {
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.HasUsed(AnxuCard.ClassName))
                return new List<WrappedCard> { new WrappedCard(AnxuCard.ClassName) { Skill = Name } };

            return new List<WrappedCard>();
        }
    }

    public class FumianAI : SkillEvent
    {
        public FumianAI() : base("fumian") { }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            if (ai.WillSkipDrawPhase(player) || (player.GetMark(Name) > 0 && player.GetMark(Name) < 3)) return "target";
            if (ai.WillSkipPlayPhase(player)) return "draw";
            return "draw";
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            Room room = ai.Room;
            List<Player> result = new List<Player>();
            if (room.GetTag(Name) is CardUseStruct use)
            {
                if (use.Card.Name == Slash.ClassName)
                {
                    List<ScoreStruct> scores = ai.CaculateSlashIncome(player, new List<WrappedCard> { use.Card }, targets, false);
                    if (scores.Count > 0)
                    {
                        foreach (ScoreStruct score in scores)
                        {
                            if (score.Score > 0)
                            {
                                bool contain = false;
                                foreach (Player p in score.Players)
                                {
                                    if (result.Contains(p))
                                    {
                                        contain = true;
                                        break;
                                    }
                                }
                                if (contain) continue;
                                int count = result.Count;
                                for (int i = 0; i < Math.Min(score.Players.Count, max - count); i++)
                                    result.Add(score.Players[i]);

                                if (result.Count >= max) break;
                            }
                        }
                    }
                }
                else if (use.Card.Name == Peach.ClassName || use.Card.Name == ExNihilo.ClassName)
                {
                    ai.SortByDefense(ref targets, false);
                    foreach (Player p in targets)
                        if (ai.IsFriend(p) && result.Count < max)
                            result.Add(p);
                }
                else if (use.Card.Name == Dismantlement.ClassName)
                {
                    List<Player> players = new List<Player>();
                    foreach (Player p in targets)
                        if (!ai.IsFriend(p)) players.Add(p);

                    foreach (Player p in players)
                        if (result.Count < max && ai.FindCards2Discard(player, p, Dismantlement.ClassName, "hej", HandlingMethod.MethodDiscard).Score > 0)
                            result.Add(p);
                }
                else if (use.Card.Name == Snatch.ClassName)
                {
                    List<Player> players = new List<Player>();
                    foreach (Player p in targets)
                        if (!ai.IsFriend(p)) players.Add(p);

                    foreach (Player p in players)
                        if (result.Count < max && ai.FindCards2Discard(player, p, Snatch.ClassName, "hej", HandlingMethod.MethodGet).Score > 0)
                            result.Add(p);
                }
            }

            return result;
        }
    }

    public class DaiyanAI : SkillEvent
    {
        public DaiyanAI() : base("daiyan") { }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            List<Player> friends = ai.FriendNoSelf;
            if (friends.Count > 0)
            {
                ai.SortByDefense(ref friends, false);
                foreach (Player p in friends)
                    if (!player.ContainsTag(Name) || (string)player.GetTag(Name) != p.Name)
                        return new List<Player> { p };
            }
            else if (ai.IsSituationClear())
            {
                List<Player> enemis = ai.GetEnemies(player);
                ai.SortByDefense(ref enemis, false);
                Player target = null;
                if (player.ContainsTag(Name))
                {
                    target = ai.Room.FindPlayer((string)player.GetTag(Name), true);
                    if (!target.Alive)
                        target = null;
                }
                if (target == null)
                {
                    if (enemis.Count > 0)
                        return new List<Player> { enemis[0] };
                }
                else if (targets.Contains(target))
                    return new List<Player> { target };
            }

            return new List<Player>();
        }
    }

    public class QiaoshiAI : SkillEvent
    {
        public QiaoshiAI() : base("qiaoshi")
        {
            key = new List<string> { "skillInvoke" };
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str && player != ai.Self)
            {
                Room room = ai.Room;
                List<string> strs = new List<string>(str.Split(':'));
                if (strs[1] == Name && strs[2] == "yes")
                {
                    Player target = room.Current;
                    if (ai.GetPlayerTendency(target) != "unknown") ai.UpdatePlayerRelation(player, target, true);
                }
                else if (strs[1] == Name && strs[2] == "no" && ai is StupidAI _ai && ai.GetPlayerTendency(room.Current) != "unknown")
                {
                    Player to = room.Current;
                    if (_ai.GetPlayerTendency(to) == "loyalist" || to.GetRoleEnum() == PlayerRole.Lord)
                        _ai.UpdatePlayerIntention(player, "rebel", 60);
                    else
                        _ai.UpdatePlayerIntention(player, "loyalist", 60);
                }
            }
        }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return ai.IsFriend(ai.Room.Current);
        }
    }

    public class YanyuAI : SkillEvent
    {
        public YanyuAI() : base("yanyu") { key = new List<string> { "playerChosen" }; }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str && player != ai.Self)
            {
                string[] strs = str.Split(':');
                if (strs[1] == Name)
                {
                    Room room = ai.Room;
                    Player target = room.FindPlayer(strs[2]);
                    if (ai.GetPlayerTendency(target) != "unknown")
                            ai.UpdatePlayerRelation(player, target, true);
                }
            }
        }
        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            Room room = ai.Room;
            List<int> ids = new List<int>();
            foreach (int id in player.GetCards("h"))
                if (room.GetCard(id).Name.Contains(Slash.ClassName))
                    ids.Add(id);


            if (ids.Count > 0)
            {
                bool male = false;
                foreach (Player p in ai.GetFriends(player))
                {
                    if (p.IsMale())
                    {
                        male = true;
                        break;
                    }
                }
                ai.SortByUseValue(ref ids, false);
                if (male)
                {
                    ai.Number[Name] = 5;
                    if (player.GetMark(Name) < 2 || ai.GetOverflow(player) > 0)
                    {
                        if (player.GetMark(Name) >= 2)
                            ai.Number[Name] = 0.1;

                        WrappedCard card = new WrappedCard(YanyuCard.ClassName) { Skill = Name };
                        card.AddSubCard(ids[0]);
                        return new List<WrappedCard> { card };
                    }
                }
                else if (ai.GetOverflow(player) > 0 || ids.Count > 0)
                {
                    ai.Number[Name] = 0.1;
                    WrappedCard card = new WrappedCard(YanyuCard.ClassName) { Skill = Name };
                    card.AddSubCard(ids[0]);
                    return new List<WrappedCard> { card };
                }
            }

            return new List<WrappedCard>();
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            ai.Room.SortByActionOrder(ref targets);
            foreach (Player p in targets)
                if (ai.IsFriend(p) && !ai.WillSkipPlayPhase(p))
                    return new List<Player> { p };

            foreach (Player p in targets)
                if (ai.IsFriend(p))
                    return new List<Player> { p };

            return new List<Player>();
        }
    }

    public class YanyuCardAI : UseCard
    {
        public YanyuCardAI() : base(YanyuCard.ClassName) { }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            use.Card = card;
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return ai.Number["yanyu"];
        }
    }

    public class XiantuAI : SkillEvent
    {
        public XiantuAI() : base("xiantu")
        {
            key = new List<string> { "skillInvoke" };
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str && player != ai.Self)
            {
                Room room = ai.Room;
                List<string> strs = new List<string>(str.Split(':'));
                if (strs[1] == Name && strs[2] == "yes")
                {
                    Player target = room.Current;
                    if (ai.GetPlayerTendency(target) != "unknown") ai.UpdatePlayerRelation(player, target, true);
                }
                else if (strs[1] == Name && strs[2] == "no" && ai is StupidAI _ai && room.Current.GetRoleEnum() == PlayerRole.Lord)
                {
                    _ai.UpdatePlayerIntention(player, "rebel", 50);
                }
            }
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is Player target)
                return ai.IsFriend(target);

            return false;
        }

        public override List<int> OnExchange(TrustedAI ai, Player player, string pattern, int min, int max, string pile)
        {
            List<int> result = new List<int>();
            List<int> ids = player.GetCards("he");
            List<double> values = ai.SortByKeepValue(ref ids, false);
            for (int i = 0; i < 2; i++)
                if (values[i] < 0)
                    result.Add(ids[i]);

            if (result.Count < 2)
            {
                ai.SortByUseValue(ref ids);
                foreach (int id in ids)
                {
                    if (!result.Contains(id))
                        result.Add(id);

                    if (result.Count >= 2)
                        break;
                }
            }

            return result;
        }
    }

    public class QiangzhiAI : SkillEvent
    {
        public QiangzhiAI() : base("qiangzhi") { }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            List<Player> enemies = ai.GetEnemies(player);
            if (enemies.Count > 0)
            {
                ai.SortByDefense(ref enemies, false);
                foreach (Player p in enemies)
                    if (!p.IsKongcheng()) return new List<Player> { p };
            }

            return new List<Player> { targets[0] };
        }
    }

    public class AnxuCardAI : UseCard
    {
        public AnxuCardAI() : base(AnxuCard.ClassName)
        {
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && player != ai.Self && data is CardUseStruct use)
            {
                Player target = use.To[0];
                if (use.To[0].HandcardNum > use.To[1].HandcardNum)
                    target = use.To[1];

                if (ai.GetPlayerTendency(target) != "unknown")
                    ai.UpdatePlayerRelation(player, target, true);
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 9;
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            List<Player> friends = ai.GetFriends(player);
            List<Player> enemies = ai.GetEnemies(player);
            ai.SortByDefense(ref friends, false);
            ai.SortByDefense(ref enemies, false);

            foreach (Player p in friends)
            {
                if (player == p) continue;
                foreach (Player e in enemies)
                {
                    if (e.HandcardNum > p.HandcardNum)
                    {
                        use.To = new List<Player> { p, e };
                        use.Card = card;
                        return;
                    }
                }
            }

            foreach (Player p in friends)
            {
                if (player == p) continue;
                foreach (Player e in room.GetOtherPlayers(p))
                {
                    if (player == e || ai.IsFriend(e)) continue;
                    if (e.HandcardNum > p.HandcardNum)
                    {
                        use.To = new List<Player> { p, e };
                        use.Card = card;
                        return;
                    }
                }
            }
        }
    }

    public class ZhuiyiAI : SkillEvent
    {
        public ZhuiyiAI() : base("zhuiyi") { }
        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            List<Player> friends = ai.FriendNoSelf;
            if (friends.Count > 0)
            {
                ai.SortByDefense(ref friends, false);
                foreach (Player p in friends)
                    if (targets.Contains(p)) return new List<Player> { p };
            }

            return new List<Player>();
        }
    }

    public class ShenxingAI : SkillEvent
    {
        public ShenxingAI() : base("shenxing")
        {
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            List<WrappedCard> result = new List<WrappedCard>();
            List<int> ids = player.GetCards("he");
            if (ids.Count >= 2)
            {
                Room room = ai.Room;
                List<int> red = new List<int>(), black = new List<int>(), adjust = new List<int>(), subs = new List<int>();
                foreach (int id in player.GetCards("h"))
                {
                    if (WrappedCard.IsBlack(room.GetCard(id).Suit))
                        black.Add(id);
                    else
                        red.Add(id);
                }

                if (black.Count > 0 && red.Count > 0)
                {
                    if (black.Count > red.Count)
                        adjust = red;
                    else
                        adjust = black;
                }

                List<double> values = ai.SortByKeepValue(ref ids, false);
                if (values[0] < 0)
                {
                    subs.Add(ids[0]);
                    foreach (int id in ids)
                    {
                        if (subs.Contains(id) || (adjust.Count > 0 && !adjust.Contains(id))) continue;
                        subs.Add(id);
                        if (subs.Count >= 2)
                        {
                            WrappedCard card = new WrappedCard(ShenxingCard.ClassName) { Skill = Name };
                            card.AddSubCards(subs);
                            result.Add(card);
                            return result;
                        }
                    }
                }

                if (adjust.Count > 0 || ai.GetOverflow(player) > 0)
                {
                    ai.SortByKeepValue(ref adjust, false);
                    foreach (int id in adjust)
                    {
                        subs.Add(id);
                        if (subs.Count >= 2)
                        {
                            WrappedCard card = new WrappedCard(ShenxingCard.ClassName) { Skill = Name };
                            card.AddSubCards(subs);
                            result.Add(card);
                            return result;
                        }
                    }

                    foreach (int id in ids)
                    {
                        if (subs.Contains(id)) continue;
                        subs.Add(id);
                        if (subs.Count >= 2)
                        {
                            WrappedCard card = new WrappedCard(ShenxingCard.ClassName) { Skill = Name };
                            card.AddSubCards(subs);
                            result.Add(card);
                            return result;
                        }
                    }
                }
            }

            return result;
        }
    }

    public class ShenxingCardAI : UseCard
    {
        public ShenxingCardAI() : base(ShenxingCard.ClassName)
        { }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            use.Card = card;
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 0.5;
        }
    }

    public class BingyiAI : SkillEvent
    {
        public BingyiAI() : base("bingyi") { key = new List<string> { "playerChosen" }; }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str && player != ai.Self)
            {
                string[] strs = str.Split(':');
                if (strs[1] == Name)
                {
                    Room room = ai.Room;
                    List<Player> targets = new List<Player>();
                    foreach (string name in strs[2].Split('+'))
                        targets.Add(room.FindPlayer(name));

                    foreach (Player p in targets)
                        if (p != player && ai.GetPlayerTendency(p) != "unknown")
                            ai.UpdatePlayerRelation(player, p, true);
                }
            }
        }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            List<int> red = new List<int>(), black = new List<int>();
            foreach (int id in player.GetCards("h"))
            {
                if (WrappedCard.IsBlack(ai.Room.GetCard(id).Suit))
                    black.Add(id);
                else
                    red.Add(id);
            }

            return black.Count == 0 || red.Count == 0;
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            List<Player> friends = ai.GetFriends(player), result = new List<Player>();
            ai.SortByDefense(ref friends, false);

            for (int i = 0; i < Math.Min(friends.Count, max); i++)
                result.Add(friends[i]);

            return result;
        }
    }

    public class BuyiJXAI : SkillEvent
    {
        public BuyiJXAI() : base("buyi_jx")
        {
            key = new List<string> { "skillInvoke" };
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str && player != ai.Self)
            {
                List<string> strs = new List<string>(str.Split(':'));
                if (strs[1] == Name && strs[2] == "yes")
                {
                    Player target = null;
                    foreach (Player p in ai.Room.GetAlivePlayers())
                    {
                        if (p.HasFlag("Global_Dying"))
                        {
                            target = p;
                            break;
                        }
                    }
                    if (ai.GetPlayerTendency(target) != "unknown") ai.UpdatePlayerRelation(player, target, true);
                }
            }
        }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is Player target)
            {
                if (ai.IsFriend(target)) return true;

                if (player.GetRoleEnum() == Player.PlayerRole.Renegade && ai.Room.GetAlivePlayers().Count > 2 && target.GetRoleEnum() == Player.PlayerRole.Lord)
                    return true;
            }

            return false;
        }
    }

    public  class AnguoAI : SkillEvent
    {
        public AnguoAI() : base("anguo") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (player.HasUsed(AnguoCard.ClassName))
                return new List<WrappedCard> { new WrappedCard(AnguoCard.ClassName) { Skill = Name } };

            return new List<WrappedCard>();
        }
    }

    public class AnguoCardAI : UseCard
    {
        public AnguoCardAI() : base(AnguoCard.ClassName) { }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use && player != ai.Self)
            {
                Room room = ai.Room;
                int count = 100, hp = 100, eq = 100;
                foreach (Player p in room.GetAllPlayers())
                {
                    if (p.HandcardNum < count)
                        count = p.HandcardNum;
                    if (p.Hp < hp)
                        hp = p.Hp;
                    if (p.GetEquips().Count < eq)
                        eq = p.GetEquips().Count;
                }

                Player target = use.To[0];
                if (ai.GetPlayerTendency(target) != "unknown" && (target.Hp == hp || (target.HandcardNum == count && target.GetEquips().Count == eq)))
                {
                    ai.UpdatePlayerRelation(player, target, true);
                }
            }
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            int count = 100, hp = 100, eq = 100;
            foreach (Player p in room.GetAllPlayers())
            {
                if (p.HandcardNum < count)
                    count = p.HandcardNum;
                if (p.Hp < hp)
                    hp = p.Hp;
                if (p.GetEquips().Count < eq)
                    eq = p.GetEquips().Count;
            }

            List<Player> friends = ai.FriendNoSelf;
            ai.SortByDefense(ref friends, false);
            foreach (Player p in friends)
            {
                int match = 0;
                if (p.HandcardNum == count) match++;
                if (p.Hp == hp) match++;
                if (p.GetEquips().Count == eq) match++;

                if (match == 3)
                {
                    use.Card = card;
                    use.To.Add(p);
                    return;
                }
            }

            foreach (Player p in friends)
            {
                int match = 0;
                if (p.HandcardNum == count) match++;
                if (p.Hp == hp) match++;
                if (p.GetEquips().Count == eq) match++;

                if (match == 2)
                {
                    use.Card = card;
                    use.To.Add(p);
                    return;
                }
            }

            foreach (Player p in friends)
            {
                if (p.Hp == hp)
                {
                    use.Card = card;
                    use.To.Add(p);
                    return;
                }
            }

            foreach (Player p in friends)
            {
                int match = 0;
                if (p.HandcardNum == count) match++;
                if (p.Hp == hp) match++;
                if (p.GetEquips().Count == eq) match++;

                if (match > 0)
                {
                    use.Card = card;
                    use.To.Add(p);
                    return;
                }
            }

            int self = 0;
            if (player.Hp == hp) self++;
            if (player.HandcardNum == count) self++;
            if (player.GetEquips().Count == eq) self++;
            if (self > 0)
            {
                foreach (Player p in room.GetAlivePlayers())
                {
                    int match = 0;
                    if (p.HandcardNum == count) match++;
                    if (p.Hp == hp) match++;
                    if (p.GetEquips().Count == eq) match++;

                    if (match == 0)
                    {
                        use.Card = card;
                        use.To.Add(p);
                        return;
                    }
                }

                foreach (Player p in room.GetAlivePlayers())
                {
                    int match = 0;
                    if (p.HandcardNum == count) match++;
                    if (p.Hp == hp) match++;
                    if (p.GetEquips().Count == eq) match++;

                    if (p.HandcardNum == count && match == 1 && (player.Hp == hp || player.GetEquips().Count == eq))
                    {
                        use.Card = card;
                        use.To.Add(p);
                        return;
                    }
                }
            }
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 7;
        }
    }
}