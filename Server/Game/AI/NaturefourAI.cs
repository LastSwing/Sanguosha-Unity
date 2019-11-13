using System;
using System.Collections.Generic;
using CommonClass.Game;
using SanguoshaServer.Game;
using SanguoshaServer.Package;

namespace SanguoshaServer.AI
{
    public class NaturefourAI : AIPackage
    {
        public NaturefourAI() : base("Naturefour")
        {
            events = new List<SkillEvent>
            {
                new LeijiJXAI(),
                new ShuangxiongJXAI(),
                new GuhuoAI(),
                new JiuchiAI(),
                new BaonueAI(),

                new LiegongJXAI(),
                new KuangguJXAI(),
                new QimouAI(),
                new ZaiqiJXAI(),
                new LierenJXAI(),

                new FenjiJXAI(),
                new ZhibaAI(),
                new TianxiangJXAI(),
                new PoluSJAI(),

                new QiangxiJXAI(),

                new GuixinAI(),
            };

            use_cards = new List<UseCard>
            {
                new ZhibaCardAI(),
                new QimouCardAI(),
                new QiangxiJXCardAI(),
            };
        }
    }
    public class LeijiJXAI : SkillEvent
    {
        public LeijiJXAI() : base("leiji_jx")
        {
            key = new List<string> { "playerChosen" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (ai.Self == player) return;
            if (data is string choice && ai.Self != player)
            {
                string[] choices = choice.Split(':');
                if (choices[1] == Name)
                {
                    Room room = ai.Room;
                    Player target = room.FindPlayer(choices[2]);
                    if (ai.GetPlayerTendency(target) != "unknown")
                        ai.UpdatePlayerRelation(player, target, false);
                }
            }
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> target, int min, int max)
        {
            if (!ai.WillShowForAttack() || !ai.WillShowForDefence()) return new List<Player>();

            Room room = ai.Room;
            List<Player> enemies = ai.GetEnemies(player);
            ai.SortByDefense(ref enemies, false);
            List<ScoreStruct> scores = new List<ScoreStruct>();
            foreach (Player p in enemies)
            {
                if (ai.HasSkill("hongyan", p)) continue;
                DamageStruct damage = new DamageStruct(Name, player, p, 2, DamageStruct.DamageNature.Thunder);
                ScoreStruct score = ai.GetDamageScore(damage);
                if (score.Score > 0 && score.DoDamage)
                    score.Score += ai.ChainDamage(damage);
                scores.Add(score);
            }

            if (scores.Count > 0)
            {
                ai.CompareByScore(ref scores);
                if (scores[0].Score > 0)
                    return new List<Player> { scores[0].Damage.To };
            }

            return new List<Player>();
        }
    }

    public class LiegongJXAI : SkillEvent
    {
        public LiegongJXAI() : base("liegong_jx")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is Player target && ai.IsEnemy(target))
                return true;

            return false;
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            if (choice.Contains("nojink"))
                return "nojink";
            else if (choice.Contains("damage"))
                return "damage";

            return base.OnChoice(ai, player, choice, data);
        }
    }
    public class KuangguJXAI : SkillEvent
    {
        public KuangguJXAI() : base("kuanggu_jx")
        {}
        
        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            if (ai.HasCrossbowEffect(player))
            {
                foreach (Player p in ai.GetEnemies(player))
                    if (RoomLogic.InMyAttackRange(ai.Room, player, p))
                        return "draw";
            }

            return "recover";
        }

        public override ScoreStruct GetDamageScore(TrustedAI ai, DamageStruct damage)
        {
            Player from = damage.From;
            Player to = damage.To;
            Room room = ai.Room;
            ScoreStruct score = new ScoreStruct();
            if (from != null && ai.HasSkill(Name, from) && (to == from && from.Hp > damage.Damage || RoomLogic.DistanceTo(room, from, to) == 1))
            {
                double value = 0;
                if (to == from)
                    value = damage.Damage * 4;
                else
                {
                    int heal = Math.Min(from.GetLostHp(), damage.Damage);
                    value = heal * 4 + (damage.Damage - heal) * 1.5;
                }
                if (ai.IsFriend(from))
                    score.Score = value;
                else if (ai.IsEnemy(from))
                    score.Score = -value;
            }

            return score;
        }
    }
    public class QimouAI : SkillEvent
    {
        public QimouAI() : base("qimou") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (player.GetMark("@mou") > 0 && player.Hp >= 2)
            {
                Room room = ai.Room;
                if (player.IsWounded() && ai.GetKnownCardsNums(Peach.ClassName, "he", player) > 0) return new List<WrappedCard>();
                double value = 0.6 * room.GetOtherPlayers(player).Count
                    * (ai.GetKnownCardsNums(SavageAssault.ClassName, "he", player) + ai.GetKnownCardsNums(ArcheryAttack.ClassName, "he", player));
                if (ai.GetEnemies(player).Count > 1 || ai.IsSituationClear())
                    value += ai.GetKnownCardsNums(Slash.ClassName, "he", player);
                if (value > 3.5)
                    return new List<WrappedCard> { new WrappedCard(QimouCard.ClassName) { Skill = Name, Mute = true } };
            }

            return new List<WrappedCard>();
        }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, bool isUse, Player.Place place)
        {
            if (player.GetMark("@mou") > 0 && (card.Name.Contains(Slash.ClassName) || card.Name == SavageAssault.ClassName || card.Name == ArcheryAttack.ClassName))
                return 1.5;
            if (player.GetMark("@mou") > 0 && card.Name == Peach.ClassName && player.IsWounded())
                return 2;

            return 0;
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            return (player.Hp - 1).ToString();
        }
    }

    public class QimouCardAI : UseCard
    {
        public QimouCardAI() : base(QimouCard.ClassName) { }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            use.Card = card;
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 5;
        }
    }

    public class ZaiqiJXAI : SkillEvent
    {
        public ZaiqiJXAI() : base("zaiqi_jx")
        {
            key = new List<string> { "playerChosen" };
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (data is string choice && ai.Self != player)
            {
                string[] choices = choice.Split(':');
                if (choices[1] == Name)
                {
                    Room room = ai.Room;
                    List<Player> targets = new List<Player>();
                    foreach (string general in choices[2].Split('+'))
                        targets.Add(room.FindPlayer(general, true));

                    foreach (Player p in targets)
                    {
                        if (p != player && ai.GetPlayerTendency(p) != "unknown")
                            ai.UpdatePlayerRelation(player, p, true);
                    }
                }
            }
        }
        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            List<Player> result = new List<Player> { player };
            ai.SortByDefense(ref targets, false);
            if (result.Count < max)
            {
                foreach (Player p in targets)
                {
                    if (ai.IsFriend(p) && !result.Contains(p))
                        result.Add(p);

                    if (result.Count >= max) break;
                }
            }

            return result;
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            if (data is Player target && ai.IsFriend(target) && ai.IsWeak(target))
                return "heal";

            return "draw";
        }
    }

    public class LierenJXAI : SkillEvent
    {
        public LierenJXAI() : base("lieren_jx")
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
                    Room room = ai.Room;
                    Player target = null;
                    foreach (Player p in room.GetAlivePlayers())
                    {
                        if (p.HasFlag("lieren_target"))
                        {
                            target = p;
                            break;
                        }
                    }

                    if (ai.GetPlayerTendency(target) != "unknown")
                        ai.UpdatePlayerRelation(player, target, false);
                }
            }
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is Player target && ai.IsEnemy(target))
            {
                bool can_pindian = false;
                if (target.HandcardNum != 1 || !ai.NeedKongcheng(target))
                {
                    if (target.HandcardNum == 1)
                    {
                        foreach (int id in target.GetEquips())
                        {
                            if (RoomLogic.CanGetCard(ai.Room, player, target, id) && ai.GetKeepValue(id, target, Player.Place.PlaceEquip) > 0)
                            {
                                can_pindian = true;
                                break;
                            }
                        }
                    }
                }

                Room room = ai.Room;
                if (can_pindian)
                {
                    List<int> ids = new List<int>();
                    if (ai.Room.Current == player)
                    {
                        foreach (int id in player.GetCards("h"))
                        {
                            if (ai.GetUseValue(id, player, Player.Place.PlaceHand) < 6 || room.GetCard(id).Number > 11)
                                ids.Add(id);
                        }
                    }
                    else
                    {
                        foreach (int id in player.GetCards("h"))
                        {
                            if (ai.GetKeepValue(id, player, Player.Place.PlaceHand) < 5 || room.GetCard(id).Number > 11)
                                ids.Add(id);
                        }
                    }

                    if (ids.Count > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override double TargetValueAdjust(TrustedAI ai, WrappedCard card, Player from, List<Player> targets, Player to)
        {
            if (card.Name.Contains(Slash.ClassName) && from != null && to != null && !ai.IsFriend(from, to) && ai.HasSkill("lieren_jx", from) && !from.IsKongcheng()
                && RoomLogic.CanBePindianBy(ai.Room, to, from))
                return 1.5;

            return 0;
        }

        public override WrappedCard OnPindian(TrustedAI ai, Player requestor, List<Player> player)
        {
            if (ai.Self == requestor)
            {
                List<int> ids = new List<int>();
                if (ai.Room.Current == requestor)
                {
                    foreach (int id in requestor.GetCards("h"))
                    {
                        if (ai.GetUseValue(id, requestor, Player.Place.PlaceHand) < 6)
                            ids.Add(id);
                    }
                }
                else
                {
                    foreach (int id in requestor.GetCards("h"))
                    {
                        if (ai.GetKeepValue(id, requestor, Player.Place.PlaceHand) < 5)
                            ids.Add(id);
                    }
                }

                if (ids.Count > 0)
                {
                    return ai.GetMaxCard(requestor, ids);
                }
                else
                {
                    if (player[0].GetCards("he").Count > 1)
                        return ai.GetMaxCard(requestor);
                    else
                    {
                        ids = requestor.GetCards("h");
                        if (ai.Room.Current == requestor)
                        {
                            ai.SortByUseValue(ref ids, false);
                        }
                        else
                        {
                            ai.SortByKeepValue(ref ids, false);
                        }

                        return ai.Room.GetCard(ids[0]);
                    }
                }
            }
            else
            {
                return ai.GetMaxCard(ai.Self);
            }
        }
    }


    public class FenjiJXAI : SkillEvent
    {
        public FenjiJXAI() : base("fenji_jx")
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
                    Room room = ai.Room;
                    Player target = null;
                    foreach (Player p in room.GetAlivePlayers())
                    {
                        if (p.HasFlag("fenji_target"))
                        {
                            target = p;
                            break;
                        }
                    }

                    if (ai.GetPlayerTendency(target) != "unknown")
                        ai.UpdatePlayerRelation(player, target, true);
                }
            }
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is Player target && ai.IsFriend(target) && (player.Hp > 1 || player.GetPile("buqu_jx").Count <= 4))
                return true;

            return false;
        }
    }

    public class ZhibaAI : SkillEvent
    {
        public ZhibaAI() : base("zhibavs")
        {
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            return base.GetTurnUse(ai, player);
        }
    }

    public class ZhibaCardAI : UseCard
    {
        public ZhibaCardAI() : base(ZhibaCard.ClassName)
        {
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            base.Use(ai, player, ref use, card);
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return base.UsePriorityAdjust(ai, player, targets, card);
        }
    }

    public class TianxiangJXAI : SkillEvent
    {
        public TianxiangJXAI() : base("tianxiang_jx")
        { }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct(null, player, new List<Player>());
            Room room = ai.Room;
            if (room.GetTag("TianxiangDamage") is DamageStruct damage)
            {
                if (ai.GetDamageScore(damage, DamageStruct.DamageStep.Done).Score < 0 && !ai.NeedToLoseHp(damage, false, false, DamageStruct.DamageStep.Done))
                {
                    List<int> ids = new List<int>();
                    foreach (int id in player.GetCards("h"))
                        if (RoomLogic.CanDiscard(room, player, player, id) && room.GetCard(id).Suit == WrappedCard.CardSuit.Heart)
                            ids.Add(id);

                    ai.SortByKeepValue(ref ids, false);
                    if (ids.Count > 0)
                    {
                        WrappedCard tianxiangCard = new WrappedCard("TianxiangCard")
                        {
                            Skill = Name
                        };
                        tianxiangCard.AddSubCard(ids[0]);

                        foreach (Player p in ai.FriendNoSelf)
                        {
                            if (ai.HasSkill("buqu_jx", p) && p.GetPile("buqu_jx").Count < 3 && p.Hp <= 2)
                            {
                                use.Card = tianxiangCard;
                                use.To.Add(p);
                                return use;
                            }

                            if (ai.HasSkill("hunzi", p) && p.GetMark("hunzi") == 0 && p.Hp == 2)
                            {
                                use.Card = tianxiangCard;
                                use.To.Add(p);
                                return use;
                            }

                            if (ai.HasSkill("zhaxiang", p) && !ai.WillSkipPlayPhase(p) && p.Hp > 1)
                            {
                                use.Card = tianxiangCard;
                                use.To.Add(p);
                                return use;
                            }
                        }

                        List<Player> enemies = ai.GetPrioEnemies();
                        ai.SortByDefense(ref enemies, false);
                        foreach (Player p in enemies)
                        {
                            DamageStruct _damage = new DamageStruct(Name, damage.From, p, 1);
                            if (ai.GetDamageScore(_damage).Score > 6 && !ai.CanResist(p, _damage.Damage))
                            {
                                use.Card = tianxiangCard;
                                use.To.Add(p);
                                return use;
                            }
                        }

                        foreach (Player p in enemies)
                        {
                            if (p.Hp <= 2)
                            {
                                use.Card = tianxiangCard;
                                use.To.Add(p);
                                return use;
                            }
                        }

                        enemies = ai.GetEnemies(player);
                        if (enemies.Count > 0)
                        {
                            ai.SortByDefense(ref enemies, false);
                            use.Card = tianxiangCard;
                            use.To.Add(enemies[0]);
                            return use;
                        }
                    }
                }
            }

            return use;
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            Room room = ai.Room;
            Player target = room.FindPlayer((string)player.GetTag("tianxiang_target"));
            if (room.GetTag("TianxiangDamage") is DamageStruct damage)
            {
                DamageStruct _damage = new DamageStruct(Name, damage.From, target);
                if (ai.IsFriend(target))
                {
                    if (ai.HasSkill("zhaxiang", target) && !ai.WillSkipPlayPhase(target) && target.Hp > 1)
                        return "losehp";

                    return "damage";
                }
                else
                {
                    if (ai.GetDamageScore(_damage).Score > 6 && !ai.CanResist(target, _damage.Damage))
                        return "damage";
                }
            }

            return "losehp";
        }
    }

    public class PoluSJAI : SkillEvent
    {
        public PoluSJAI() : base("polu_sj")
        { }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> targets, int min, int max)
        {
            List<Player> result = new List<Player>();
            ai.SortByDefense(ref targets, false);
            foreach (Player p in targets)
                if (ai.IsFriend(p)) result.Add(p);

            return result;
        }
    }

    public class ShuangxiongJXAI : SkillEvent
    {
        public ShuangxiongJXAI() : base("shuangxiong_jx")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (!ai.WillShowForAttack() || ai.WillSkipPlayPhase(player) || player.HandcardNum + player.GetPile("wooden_ox").Count < 3)
                return false;

            return true;
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (player.GetMark(Name) > 0)
            {
                Room room = ai.Room;
                List<int> ids = player.GetCards("h");
                ids.AddRange(player.GetHandPile());
                ai.SortByUseValue(ref ids, false);
                foreach (int id in ids)
                {
                    WrappedCard card = room.GetCard(id);
                    if ((player.GetMark(Name) == 1 && WrappedCard.IsBlack(card.Suit)) || (player.GetMark(Name) == 2 && WrappedCard.IsRed(card.Suit)))
                    {
                        WrappedCard duel = new WrappedCard(Duel.ClassName);
                        duel.AddSubCard(card);
                        duel.Skill = Name;
                        duel.ShowSkill = Name;
                        duel = RoomLogic.ParseUseCard(room, duel);
                        return new List<WrappedCard> { duel };
                    }
                }
            }

            return null;
        }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, bool isUse, Player.Place place)
        {
            if (card.Id <= 0) return 0;
            if (player.GetMark(Name) == 1)
                return WrappedCard.IsBlack(RoomLogic.GetCardSuit(ai.Room, card)) ? 1 : 0;
            if (player.GetMark(Name) == 2)
                return WrappedCard.IsRed(RoomLogic.GetCardSuit(ai.Room, card)) ? 1 : 0;

            if (ai.HasSkill(Name, player))
                return 0.5;

            return 0;
        }

        public override AskForMoveCardsStruct OnMoveCards(TrustedAI ai, Player player, List<int> ups, List<int> downs, int min, int max)
        {
            AskForMoveCardsStruct result = new AskForMoveCardsStruct
            {
                Top = new List<int>(),
                Bottom = new List<int>(),
                Success = true
            };
            Room room = ai.Room;
            int red = 0, black = 0;
            foreach (int id in player.GetCards("h"))
            {
                WrappedCard card = room.GetCard(id);
                if (WrappedCard.IsBlack(card.Suit))
                    black++;
                else
                    red++;
            }
            List<int> wanted = new List<int>();
            if (red > black)
            {
                foreach (int id in ups)
                {
                    WrappedCard card = room.GetCard(id);
                    if (!WrappedCard.IsBlack(card.Suit))
                        wanted.Add(id);
                }
            }
            else if (red < black)
            {
                foreach (int id in ups)
                {
                    WrappedCard card = room.GetCard(id);
                    if (WrappedCard.IsBlack(card.Suit))
                        wanted.Add(id);
                }
            }

            if (wanted.Count == 0) wanted = ups;
            if (wanted.Count > 1)
                ai.SortByUseValue(ref wanted);

            result.Bottom.Add(wanted[0]);
            foreach (int id in ups)
                if (!result.Bottom.Contains(id))
                    result.Top.Add(id);

            return result;
        }
    }

    public class GuhuoAI : SkillEvent
    {
        public GuhuoAI() : base("guhuo")
        {
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            if (data is Player yuji && ai.IsEnemy(yuji))
            {
                if (player.HasFlag("guhuo_doubt")) return "doubt";
                string card_name = yuji.GetTag(Name).ToString();
                int count = 0;
                Room room = ai.Room;
                foreach (int id in room.DiscardPile)
                    if (room.GetCard(id).Name == card_name)
                        count++;

                List<int> ids = player.GetCards("h");
                ids.AddRange(player.GetHandPile());
                foreach (int id in ids)
                    if (Engine.GetRealCard(id).Name == card_name)
                        count++;

                if (ai.CardCounts[card_name] == count)
                    return "doubt";
            }

            return "cancel";
        }
    }

    public class JiuchiAI : SkillEvent
    {
        public JiuchiAI() : base("jiuchi")
        { }

        public override double TargetValueAdjust(TrustedAI ai, WrappedCard card, Player from, List<Player> targets, Player to)
        {
            if (card != null && card.Name == Analeptic.ClassName && ai.HasSkill(Name, to))
                return 1;

            return 0;
        }

        public override WrappedCard ViewAs(TrustedAI ai, Player player, int id, bool current, Player.Place place)
        {
            if ((ai.Room.GetCard(id).Suit) == WrappedCard.CardSuit.Spade)
            {
                WrappedCard dismantlement = new WrappedCard(Analeptic.ClassName)
                {
                    Skill = Name,
                    ShowSkill = Name
                };
                dismantlement.AddSubCard(id);
                dismantlement = RoomLogic.ParseUseCard(ai.Room, dismantlement);
                return dismantlement;
            }

            return null;
        }

        public override double CardValue(TrustedAI ai, Player player, WrappedCard card, bool isUse, Player.Place place)
        {
            if (ai.HasSkill(Name, player) && !RoomLogic.IsVirtualCard(ai.Room, card) && place != Player.Place.PlaceEquip)
                return RoomLogic.GetCardSuit(ai.Room, card) == WrappedCard.CardSuit.Spade ? 0.5 : 0;

            return 0;
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!ai.WillShowForAttack()) return null;
            Room room = ai.Room;
            List<int> ids = player.GetCards("h");
            ids.AddRange(player.GetHandPile());
            ai.SortByKeepValue(ref ids, false);
            foreach (int id in ids)
            {
                WrappedCard card = room.GetCard(id);
                if (WrappedCard.IsBlack(card.Suit))
                {
                    double keep = ai.GetKeepValue(id, player);
                    if (keep < 0)
                    {
                        WrappedCard dismantlement = new WrappedCard(Analeptic.ClassName)
                        {
                            Skill = Name,
                            ShowSkill = Name
                        };
                        dismantlement.AddSubCard(card);
                        dismantlement = RoomLogic.ParseUseCard(room, dismantlement);
                        return new List<WrappedCard> { dismantlement };
                    }

                    List<WrappedCard> cards = ai.GetViewAsCards(player, id);
                    double value = 0;
                    WrappedCard _card = null;
                    foreach (WrappedCard _c in cards)
                    {
                        double card_value = ai.GetUseValue(_c, player, room.GetCardPlace(id));
                        if (card_value > value)
                        {
                            value = card_value;
                            _card = _c;
                        }
                    }

                    if (_card != null && _card.Name == Analeptic.ClassName && _card.Skill == Name) return new List<WrappedCard> { _card };
                }
            }

            return null;
        }
    }

    public class BaonueAI : SkillEvent
    {
        public BaonueAI() : base("baonue")
        {
            key = new List<string> { "skillInvoke" };
        }

        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str && player != ai.Self)
            {
                List<string> strs = new List<string>(str.Split(':'));
                if (strs[1] == Name)
                {
                    Room room = ai.Room;
                    Player target = RoomLogic.FindPlayerBySkillName(room, Name);
                    bool friendly = strs[2] == "yes";
                    if (ai.GetPlayerTendency(target) != "unknown")
                        ai.UpdatePlayerRelation(player, target, friendly);
                }
            }
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            Room room = ai.Room;
            Player dz = RoomLogic.FindPlayerBySkillName(room, Name);
            if (ai.IsFriend(dz))
                return true;
            if (player.GetRoleEnum() == Player.PlayerRole.Renegade && !ai.IsEnemy(dz) && dz.Hp == 1)
                return true;

            return false;
        }
    }

    public class QiangxiJXAI : SkillEvent
    {
        public QiangxiJXAI() : base("qiangxi_jx")
        {
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            WrappedCard card = new WrappedCard(QiangxiJXCard.ClassName)
            {
                Skill = Name,
                ShowSkill = Name
            };
            return new List<WrappedCard> { card };
        }
    }

    public class QiangxiJXCardAI : UseCard
    {
        public QiangxiJXCardAI() : base(QiangxiJXCard.ClassName)
        {
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return player.GetWeapon() ? 3 : 0;
        }
        public override void OnEvent(TrustedAI ai, TriggerEvent triggerEvent, Player player, object data)
        {
            if (ai.Self == player) return;
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {
                foreach (Player p in use.To)
                    ai.UpdatePlayerRelation(player, p, false);
            }
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            List<Player> enemies = ai.GetEnemies(player);
            ai.SortByDefense(ref enemies, false);

            if (player.GetWeapon())
            {
                int hand_weapon = -1;
                foreach (WrappedCard _card in RoomLogic.GetPlayerHandcards(room, player))
                {
                    FunctionCard fcard = Engine.GetFunctionCard(_card.Name);
                    if (fcard is Weapon)
                    {
                        hand_weapon = _card.Id;
                        break;
                    }
                }

                foreach (Player enemy in enemies)
                {
                    if (player.HasFlag(string.Format("qiangxi-{0}", enemy.Name))) continue;
                    if (ai.PlayersLevel[enemy] > 3)
                    {
                        DamageStruct damage = new DamageStruct("qiangxi", player, enemy);
                        if (RoomLogic.InMyAttackRange(room, player, enemy) && hand_weapon >= 0 && ai.GetDamageScore(damage).Score > 4)
                        {
                            card.AddSubCard(hand_weapon);
                            use.Card = card;
                            use.To = new List<Player> { enemy };
                            return;
                        }
                        else if (RoomLogic.InMyAttackRange(room, player, enemy, room.GetCard(player.Weapon.Key)) && ai.GetDamageScore(damage).Score > 4)
                        {
                            card.AddSubCard(player.Weapon.Key);
                            use.Card = card;
                            use.To = new List<Player> { enemy };
                            return;
                        }
                    }
                }
            }
            else
            {
                bool analeptic = player.Hp == 1 && ai.GetCards(Analeptic.ClassName, player).Count > 0;
                bool peach = ai.GetCards(Peach.ClassName, player).Count > 0;
                foreach (Player enemy in enemies)
                {
                    if (player.HasFlag(string.Format("qiangxi-{0}", enemy.Name))) continue;
                    if (ai.PlayersLevel[enemy] > 3)
                    {
                        DamageStruct damage = new DamageStruct("qiangxi", player, enemy);
                        if (RoomLogic.InMyAttackRange(room, player, enemy)
                            && ((ai.GetDamageScore(damage).Score > 4 && (!player.IsWounded() && peach || analeptic))
                            || (ai.GetDamageScore(damage).Score > 7 && (player.Hp > 1 || peach || analeptic))))
                        {
                            use.Card = card;
                            use.To = new List<Player> { enemy };
                            return;
                        }
                    }
                }
            }
        }
    }


    public class GuixinAI : SkillEvent
    {
        public GuixinAI() : base("guixin")
        {
        }

        public override ScoreStruct GetDamageScore(TrustedAI ai, DamageStruct damage)
        {
            ScoreStruct score = new ScoreStruct
            {
                Score = 0
            };
            if (ai.HasSkill(Name, damage.To) && damage.Damage <= damage.To.Hp)
            {
                double value = 0;
                if (!damage.To.FaceUp && damage.Damage == 1) value += 4;
                Room room = ai.Room;
                foreach (Player p in room.GetOtherPlayers(damage.To))
                {
                    if (RoomLogic.CanGetCard(room, damage.To, p, "hej"))
                    {
                        value += 0.3;
                        if (ai.IsEnemy(damage.To, p))
                        {
                            if (!p.IsNude()) value += 0.5;
                            else
                                value -= 1;
                        }
                    }
                }
                if (damage.Damage > 1) value *= (0.6 * (damage.Damage - 1)) + 1;
                if (value > 0 && damage.Damage >= damage.To.Hp)
                    value /= 2;

                if (!ai.IsFriend(damage.To))
                    value = -value;

                score.Score = value;
            }

            return score;
        }
    }
}