using System;
using System.Collections.Generic;
using CommonClass.Game;
using SanguoshaServer.Game;
using SanguoshaServer.Package;
using static SanguoshaServer.Package.FunctionCard;

namespace SanguoshaServer.AI
{
    public class TransformationAI : AIPackage
    {
        public TransformationAI() : base("Transformation")
        {
            events = new List<SkillEvent>
            {
                new TransformAI(),

                new ZhiyuAI(),
                new QiceAI(),
                new WanweiAI(),
                new YuejianAI(),
                new ZhimanAI(),
                new SanyaoAI(),
                new JiliAI(),
                new XiongSuanAI(),
                new HuashenAI(),
                new XinshengAI(),
                new XuanlueAI(),
                new YongjinAI(),
                new DiaoduAI(),
                new DiancaiAI(),
                new LianziAI(),
                new FlamemapChoiceAI(),
                new FlamemapAI(),
                new HaoshiExtraAI(),
                new ShelieAI(),
            };

            use_cards = new List<UseCard>
            {
                new QiceCardAI(),
                new SanyaoCardAI(),
                new XiongsuanCardAI(),
                new YongjinCardAI(),
                new LianziCardAI(),
                new FlamemapCardAI(),
            };
        }
    }

    public class TransformAI : SkillEvent
    {
        public TransformAI() : base("transform") { }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            string g2 = player.General2;
            if (g2.Contains("sujiang")) return true;
            double value = Engine.GetGeneralValue(g2, "Hegemony");
            if (value < 5) return true;
            return false;
        }
    }

    public class ZhiyuAI : SkillEvent
    {
        public ZhiyuAI() : base("zhiyu") { }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (ai.WillShowForDefence())
            {
                if (data is DamageStruct damage && damage.From != null && ai.IsFriend(damage.From) && player.IsKongcheng())
                    return ai.GetOverflow(damage.From) > 1;

                return true;
            }

            return false;
        }

        public override ScoreStruct GetDamageScore(TrustedAI ai, DamageStruct damage)
        {
            double value = 0;
            if (ai.HasSkill(Name, damage.To))
            {
                Room room = ai.Room;
                value = ai.IsFriend(damage.To) ? 1.5 : -1.5;
                if (damage.From != null && damage.From != null)
                {
                    int count = damage.From.HandcardNum;
                    if (damage.Card != null)
                    {
                        foreach (int id in damage.Card.SubCards)
                            if (room.GetCardPlace(id) == Player.Place.PlaceHand && room.GetCardOwner(id) == damage.From)
                                count--;
                    }

                    if (count > 0)
                    {
                        if (damage.To.HandcardNum <= 1)
                        {
                            if (ai.IsEnemy(damage.To))
                            {
                                value -= 2;
                            }
                            else if (!ai.HasSkill("jieming|yiji|jianxiong", damage.To))
                            {
                                if (ai.GetOverflow(damage.From) > 1)
                                    value = 1;
                                else
                                    value = 0;
                            }
                        }
                    }

                    if (damage.Damage >= damage.To.Hp) value /= 2;
                }
            }
            ScoreStruct score = new ScoreStruct
            {
                Score = value
            };
            return score;
        }

        public override List<int> OnDiscard(TrustedAI ai, Player player, int min, int max, bool option, bool include_equip)
        {
            Room room = ai.Room;
            List<int> ids = new List<int>();
            foreach (int id in player.HandCards)
                if (RoomLogic.CanDiscard(room, player, player, id))
                    ids.Add(id);

            if (ids.Count > 0)
            {

                if (room.Current == player)
                    ai.SortByKeepValue(ref ids, false);
                else
                    ai.SortByKeepValue(ref ids, false);

                return new List<int> { ids[0] };
            }

            return new List<int>();
        }
    }

    public class QiceAI : SkillEvent
    {
        public QiceAI() : base("qice") { }
        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            ai.Target[Name] = null;
            if (!player.IsKongcheng() && !player.HasUsed("QiceCard") && ai.WillShowForAttack())
            {
                Room room = ai.Room;
                foreach (int id in player.HandCards)
                {
                    WrappedCard card = room.GetCard(id);
                    if (RoomLogic.IsCardLimited(room, player, card, HandlingMethod.MethodUse))
                        return new List<WrappedCard>();
                    else
                    {
                        UseCard usage = Engine.GetCardUsage(card.Name);
                        if (usage != null)
                        {
                            CardUseStruct use = new CardUseStruct
                            {
                                From = player,
                                To = new List<Player>(),
                                IsDummy = true
                            };
                            usage.Use(ai, player, ref use, card);
                            if (use.Card != null)
                                return new List<WrappedCard>();
                        }
                    }
                }
                FunctionCard fcard = Engine.GetFunctionCard("QiceCard");
                WrappedCard qice = new WrappedCard("QiceCard");
                qice.AddSubCards(player.HandCards);

                List<WrappedCard> guhuos = QiceVS.GetGuhuoCards(room, player);
                double good = -1;
                foreach (WrappedCard card in guhuos)
                {
                    if (card.Name == "GodSalvation")
                    {
                        good = 0;
                        foreach (Player p in room.GetAlivePlayers())
                        {
                            if (p.Removed) continue;
                            if (p.IsWounded())
                            {
                                if (ai.IsFriend(p))
                                    good++;
                                else
                                    good--;
                            }
                        }
                    }
                }

                if (player.IsWounded())
                {
                    foreach (WrappedCard card in guhuos)
                    {
                        if (card.Name == "AllianceFeast")
                        {
                            double best = 0;
                            Player target = null;
                            qice.UserString = card.Name;
                            foreach (Player p in room.GetOtherPlayers(player))
                            {
                                if (fcard.TargetFilter(room, new List<Player>(), p, player, qice))
                                {
                                    int count = 0;
                                    foreach (Player _p in room.GetAlivePlayers())
                                        if (RoomLogic.IsFriendWith(room, p, _p))
                                            count++;

                                    count = Math.Min(count, player.GetLostHp()) + count - player.GetLostHp() > 0 ? (count - player.GetLostHp()) / 2 : 0;
                                    if (count > best)
                                    {
                                        best = count;
                                        target = p;
                                    }
                                }
                            }

                            if (best > good && (room.BloodBattle || best >= 2))
                            {
                                ai.Target[Name] = target;
                                return new List<WrappedCard> { qice };
                            }
                        }
                    }
                }

                if (good > 0 && (room.BloodBattle || good > 1))
                {
                    qice.UserString = "GodSalvation";
                    return new List<WrappedCard> { qice };
                }

                List<WrappedCard> result = new List<WrappedCard>();
                foreach (WrappedCard card in guhuos)
                {
                    if (card.Name == "ExNihilo")
                    {
                        qice.UserString = card.Name;
                        card.UserString = RoomLogic.CardToString(room, qice);
                        result.Add(card);
                    }
                    else if (card.Name == "BefriendAttacking")
                    {
                        qice.UserString = card.Name;
                        card.UserString = RoomLogic.CardToString(room, qice);
                        result.Add(card);
                    }
                }

                return result;
            }

            return new List<WrappedCard>();
        }
    }

    public class QiceCardAI : UseCard
    {
        public QiceCardAI() : base("QiceCard")
        { }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            use.Card = card;
            if (ai.Target["qice"] != null)
                use.To = new List<Player> { ai.Target["qice"] };
        }
    }

    public class YuejianAI : SkillEvent
    {
        public YuejianAI() : base("yuejian") { }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is Player target && target.HandcardNum > target.Hp)
                return true;

            return false;
        }
    }

    public class WanweiAI : SkillEvent
    {
        public WanweiAI() : base("wanwei") { }

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct
            {
                From = player,
                To = new List<Player>()
            };
            if (!ai.WillShowForDefence()) return use;

            Room room = ai.Room;
            string[] strs = prompt.Split(':');
            Player target = room.FindPlayer(strs[1]);

            List<int> cards = new List<int>();
            foreach (int id in player.GetCards("he"))
                if (room.GetCard(id).HasFlag("can_wanwei"))
                    cards.Add(id);

            if (target != null && ai.IsFriend(target) && room.ContainsTag("wanwei_data") && room.GetTag("wanwei_data") is List<int> moves)
            {
                int count = moves.Count;
                List<int> subs = new List<int>();
                foreach (int id in moves)
                {
                    if (room.GetCardPlace(id) == Player.Place.PlaceHand)
                        count--;
                    else
                        subs.Add(id);
                }

                while (count > 0)
                {
                    double best = -100;
                    int result = -1;
                    foreach (int id in cards)
                    {
                        if (subs.Contains(id)) continue;
                        double value = ai.GetUseValue(id, target, Player.Place.PlaceHand);
                        if (value > best)
                        {
                            best = value;
                            result = id;
                        }
                    }

                    if (result >= 0)
                    {
                        subs.Add(result);
                        count--;
                    }
                }

                WrappedCard card = new WrappedCard("WanweiCard")
                {
                    ShowSkill = Name,
                    Skill = Name
                };
                card.AddSubCards(subs);
                use.Card = card;
            }
            else
            {
                WrappedCard card = new WrappedCard("WanweiCard")
                {
                    ShowSkill = Name,
                    Skill = Name
                };
                int count = player.GetMark(Name);
                ai.SortByKeepValue(ref cards, false);
                for (int i = 0; i < count; i++)
                    card.AddSubCard(cards[i]);

                use.Card = card;
            }

            return use;
        }
    }

    public class ZhimanAI : SkillEvent
    {
        public ZhimanAI() : base("zhiman") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            Room room = ai.Room;
            if ((ai.WillShowForAttack() || ai.WillShowForDefence())
                && room.ContainsTag("zhiman_data") && room.GetTag("zhiman_data") is DamageStruct damage)
            {
                ScoreStruct get = ai.FindCards2Discard(player, damage.To, "ej", HandlingMethod.MethodGet);
                ScoreStruct score = ai.GetDamageScore(damage);

                return get.Score >= score.Score;
            }

            return false;
        }
    }

    public class SanyaoAI : SkillEvent
    {
        public SanyaoAI() : base("sanyao") { }
        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (player.IsNude() && !player.HasUsed("SanyaoCard"))
             {
                WrappedCard first = new WrappedCard("SanyaoCard")
                {
                    Skill = Name,
                    ShowSkill = Name
                };
                return new List<WrappedCard> { first };
            }

            return new List<WrappedCard>();
        }
    }

    public class SanyaoCardAI : UseCard
    {
        public SanyaoCardAI() : base("SanyaoCard") { }
        public override double UsePriorityAjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 3;
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            int max = 0;
            foreach (Player p in room.GetAlivePlayers())
                if (p.Hp > max)
                    max = p.Hp;

            List<ScoreStruct> scores = new List<ScoreStruct>();
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (p.Hp == max)
                {
                    DamageStruct damage = new DamageStruct(Name, player, p);
                    ScoreStruct score = ai.GetDamageScore(damage);
                    score.Players = new List<Player> { p };
                    scores.Add(score);
                }
            }

            if (scores.Count > 0)
            {
                ai.CompareByScore(ref scores);
                List<int> ids = new List<int>();
                foreach (int id in player.GetCards("he"))
                    if (RoomLogic.CanDiscard(room, player, player, id))
                        ids.Add(id);

                if (ids.Count > 0)
                {
                    ai.SortByKeepValue(ref ids, false);
                    if (ai.GetKeepValue(ids[0], player) < 0 && scores[0].Score >= 0)
                    {
                        card.AddSubCard(ids[0]);
                        use.Card = card;
                        use.To = scores[0].Players;
                        return;
                    }

                    ai.SortByUseValue(ref ids, false);
                    double value = ai.GetUseValue(ids[0], player);
                    if (ai.GetOverflow(player) > 0)
                        value /= 3;

                    if (scores[0].Score > 0 && scores[0].Score > value)
                    {
                        card.AddSubCard(ids[0]);
                        use.Card = card;
                        use.To = scores[0].Players;
                        return;
                    }
                }
            }
        }
    }

    public class JiliAI : SkillEvent
    {
        public JiliAI() : base("jili")
        { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return ai.WillShowForAttack() || ai.WillShowForDefence();
        }
    }

    public class XiongSuanAI : SkillEvent
    {
        public XiongSuanAI() : base("xiongsuan") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (ai.WillShowForAttack() && RoomLogic.CanDiscard(ai.Room, player, player, "he") && player.GetMark("@xiong") > 0)
            {
                WrappedCard first = new WrappedCard("XiongsuanCard")
                {
                    Skill = Name,
                    ShowSkill = Name,
                    Mute = true
                };
                return new List<WrappedCard> { first };
            }

            return new List<WrappedCard>();
        }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            if (choice.Contains("xiongyi")) return "xiongyi";
            if (choice.Contains("xiongsuan")) return "xiongsuan";

            return choice.Split('+')[0];
        }
    }

    public class XiongsuanCardAI : UseCard
    {
        public XiongsuanCardAI() : base("XiongsuanCard") { }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            Player target = null;
            foreach (Player p in room.GetAlivePlayers())
            {
                if (RoomLogic.WillBeFriendWith(room, player, p, "xiongyi") && ai.HasSkill("xiongyi", p) && p.GetMark("@arise") == 0)
                {
                    DamageStruct damage = new DamageStruct("xiongsuan", player, p);
                    if (ai.GetDamageScore(damage).Score > -5)
                    {
                        target = p;
                        break;
                    }
                }
            }

            int sub = LijianAI.FindLijianCard(ai, player);
            if (target != null && sub >= 0)
            {
                card.AddSubCard(sub);
                use.Card = card;
                use.To = new List<Player> { target };
                return;
            }
            
            DamageStruct damage_self = new DamageStruct("xiongsuan", player, player);
            if (ai.GetDamageScore(damage_self).Score >= 0 || player.GetLostHp() == 0 || (player.IsWounded() && player.HasArmor("SilverLion"))
                || (player.Hp == 1 && ai.GetKnownCardsNums("Analeptic", "he", player) > 0 && player.GetCards("he").Count > 1))
            {
                if (sub >= 0)
                {
                    card.AddSubCard(sub);
                    use.Card = card;
                    use.To = new List<Player> { player };
                }
            }
        }
    }

    public class HuashenAI : SkillEvent
    {
        public HuashenAI() : base("huashen")
        { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }
    public class XinshengAI : SkillEvent
    {
        public XinshengAI() : base("xinsheng")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class XuanlueAI : SkillEvent
    {
        public XuanlueAI() : base("xuanlue") { }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> target, int min, int max)
        {
            Room room = ai.Room;
            List<ScoreStruct> scores = new List<ScoreStruct>();

            foreach (Player p in room.GetOtherPlayers(player))
            {
                ScoreStruct score = ai.FindCards2Discard(player, p, "he", FunctionCard.HandlingMethod.MethodDiscard);
                scores.Add(score);
            }
            if (scores.Count > 0)
            {
                scores.Sort((x, y) => { return x.Score > y.Score ? -1 : 1; });
                if (scores[0].Score > 0)
                {
                    return scores[0].Players;
                }
            }

            return null;
        }
    }

    public class YongjinAI : SkillEvent
    {
        public YongjinAI() : base("yongjin") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (ai.WillShowForAttack() && player.GetMark("@yong") > 0)
            {
                WrappedCard card = new WrappedCard("YongjinCard")
                {
                    Skill = Name,
                    ShowSkill = Name,
                    Mute = true
                };

                return new List<WrappedCard> { card };
            }

            return new List<WrappedCard>();
        }

        Player FindTarget(TrustedAI ai, Player player)
        {
            Room room = ai.Room;
            List<Player> targets = new List<Player>();
            int result = -1;
            double best = -100;
            foreach (Player p in room.GetAlivePlayers())
            {
                foreach (int id in p.GetEquips())
                {
                    double v = ai.GetKeepValue(id, player);
                    double value = ai.IsFriend(p) ? -v : v;
                    foreach (Player _p in room.GetOtherPlayers(p))
                    {
                        if (RoomLogic.CanPutEquip(_p, room.GetCard(id)))
                        {
                            double _v = ai.GetUseValue(id, _p);
                            double _value = value + (ai.IsFriend(_p) ? _v : -_v);
                            if (_value > best)
                            {
                                targets = new List<Player> { p, _p };
                                best = _value;
                                result = id;
                            }
                        }
                    }
                }
            }

            if (targets.Count == 2 && result > -1)
            {
                ai.Target[Name] = targets[1];
                ai.Choice[Name] = result.ToString();
                return targets[0];
            }

            return null;
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> target, int min, int max)
        {
            if (player.HasFlag(Name))
            {
                ai.Target[Name] = null;
                ai.Choice[Name] = string.Empty;
                Player p = FindTarget(ai, player);
                if (p != null) return new List<Player> { p };
            }
            else if (ai.Target[Name] != null)
            {
                return new List<Player> { ai.Target[Name] };
            }

            return null;
        }

        public override List<int> OnCardsChosen(TrustedAI ai, Player from, Player to, string flags, int min, int max, List<int> disable_ids)
        {
            if (!string.IsNullOrEmpty(ai.Choice[Name]) && int.TryParse(ai.Choice[Name], out int id) && !disable_ids.Contains(id))
                return new List<int> { id };

            return new List<int>();
        }
    }

    public class YongjinCardAI : UseCard
    {
        public YongjinCardAI() : base("YongjinCard") { }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            int count = 0;
            Dictionary<Player, List<int>> player_location = new Dictionary<Player, List<int>>();
            foreach (Player p in room.GetAlivePlayers())
            {
                foreach (int id in p.GetEquips())
                {
                    if (ai.IsEnemy(p) && ai.GetKeepValue(id, p) > 0)
                    {
                        bool found = false;
                        foreach (Player _p in room.GetOtherPlayers(p))
                        {
                            EquipCard fcard = (EquipCard)Engine.GetFunctionCard(room.GetCard(id).Name);
                            int location = (int)fcard.EquipLocation();
                            if (ai.IsFriend(_p) && RoomLogic.CanPutEquip(_p, room.GetCard(id)) && (!player_location.ContainsKey(_p) || !player_location[_p].Contains(location)))
                            {
                                count++;
                                List<int> locations = player_location.ContainsKey(_p) ? new List<int>(player_location[_p]) : new List<int>();
                                locations.Add(location);
                                player_location[_p] = locations;
                                found = true;
                                break;
                            }
                        }
                        if (found)
                            break;
                    }
                    if (count >= 3)
                    {
                        use.Card = card;
                        return;
                    }
                }
            }
            
        }
    }


    public class DiaoduAI : SkillEvent
    {
        public DiaoduAI() : base("diaodu") { }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class DiancaiAI : SkillEvent
    {
        public DiancaiAI() : base("diancai") { }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }
    }

    public class LianziAI : SkillEvent
    {
        public LianziAI() : base("lianzi")
        { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (RoomLogic.CanDiscard(ai.Room, player, player, "h") && !player.HasUsed("LianziCard"))
            {
                int count = player.GetPile("flame_map").Count + player.GetEquips().Count;

                if (count >= 3)
                {
                    List<int> ids = new List<int>();
                    foreach (int id in player.HandCards)
                    {
                        FunctionCard fcard = Engine.GetFunctionCard(ai.Room.GetCard(id).Name);
                        if (fcard is BasicCard && RoomLogic.CanDiscard(ai.Room, player, player, id))
                            ids.Add(id);
                    }

                    ai.SortByUseValue(ref ids, false);
                    if (ai.GetUseValue(ids[0], player) <= 5)
                    {
                        WrappedCard first = new WrappedCard("LianziCard");
                        first.AddSubCard(ids[0]);
                        first.Skill = Name;
                        first.ShowSkill = Name;
                        return new List<WrappedCard> { first };
                    }
                }
            }

            return null;
        }
    }

    public class LianziCardAI : UseCard
    {
        public LianziCardAI() : base("LianziCard")
        {
        }

        public override double UsePriorityAjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return 3;
        }
        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            use.Card = card;
        }
    }

    public class FlamemapAI : SkillEvent
    {
        public FlamemapAI() : base("flamemap") { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (!player.HasUsed("FlamemapCard"))
            {
                WrappedCard slash = new WrappedCard("FlameMapCard")
                {
                    Mute = true,
                    ShowSkill = "showforviewhas"
                };
                return new List<WrappedCard> { slash };
            }

            return null;
        }

        public override int OnPickAG(TrustedAI ai, Player player, List<int> card_ids, bool refusable)
        {
            Player lord_liubei = ai.FindPlayerBySkill("zhangwu");
            Player lord_jiaozhu = ai.FindPlayerBySkill("wendao");

            List<int> ids = new List<int>(card_ids);
            foreach (int id in card_ids)
            {
                WrappedCard card = ai.Room.GetCard(id);
                if ((lord_liubei != null && card.Name == "DragonPhoenix") || (lord_jiaozhu != null && card.Name == "PeaceSpell"))
                    ids.Remove(id);

                if (card.Name == "LuminouSpearl")
                    return id;
            }

            if (ids.Count > 0) return ids[0];

            return card_ids[0];
        }
    }

    public class FlamemapCardAI : UseCard
    {
        public FlamemapCardAI() : base("FlamemapCard") { }

        public override double UsePriorityAjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return (ai.HasSkill("zhiheng") || player.HasTreasure("LuminouSpearl") && !player.HasUsed("ZhihengCard")) ? 4.5 : 1.5;
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            Room room = ai.Room;
            List<int> ids = new List<int>();
            foreach (int id in player.GetCards("he"))
            {
                FunctionCard fcard = Engine.GetFunctionCard(room.GetCard(id).Name);
                if (fcard is EquipCard)
                    ids.Add(id);
            }

            if (ids.Count > 0)
            {
                ai.SortByKeepValue(ref ids, false);
                if (ai.GetKeepValue(ids[0], player) < 0)
                {
                    card.AddSubCard(ids[0]);
                    use.Card = card;
                    return;
                }

                ai.SortByUseValue(ref ids, false);
                foreach (int id in ids)
                {
                    if (ai.GetUseValue(ids[0], player) < 5)
                    {
                        card.AddSubCard(ids[0]);
                        use.Card = card;
                        return;
                    }
                    else if (room.GetCardPlace(id) == Player.Place.PlaceHand)
                    {
                        WrappedCard equip = room.GetCard(id);
                        if (ai.GetSameEquip(equip, player) != null)
                        {
                            card.AddSubCard(ids[0]);
                            use.Card = card;
                            return;
                        }
                    }
                }

                Player lord = ai.FindPlayerBySkill("jiahe");
                if (lord.GetPile("flame_map").Count < 5 && ai.GetKeepValue(ids[0], player) < 7)
                {
                    card.AddSubCard(ids[0]);
                    use.Card = card;
                }
            }
        }
    }

    public class FlamemapChoiceAI : SkillEvent
    {
        public FlamemapChoiceAI() : base("flamemapskill") { }

        public override string OnChoice(TrustedAI ai, Player player, string choice, object data)
        {
            Room room = ai.Room;
            int check = 1000;

            int n = 2;
            if (ai.HasSkill("yingzi_sunce")) n++;
            if (ai.HasSkill("yingzi_zhouyu")) n++;
            if (ai.HasSkill("yingziextra")) n++;
            if (player.HasTreasure("JadeSeal")) n++;

            check = 5 - n - 2 - player.HandcardNum;
            if (check < 0)
            {
                int least = 1000;
                foreach (Player p in room.GetOtherPlayers(player))
                    least = Math.Min(player.HandcardNum, least);


                foreach (Player p in ai.FriendNoSelf)
                {
                    if (p.HandcardNum == least)
                    {
                        check = 1000;
                        break;
                    }
                }
            }

            if (choice.Contains("haoshi"))
                if (check >= 0 && !ai.HasSkill("haoshi") || check > 1) return "haoshiextra";

            if (choice.Contains("shelie"))
            {
                if (n == 2 && (check < 0 || !ai.HasSkill("haoshi"))) return "shelie"; 
            }

            if (choice.Contains("yingziextra"))
            {
                if (check > 0) return "yingziextra";
            }

            if (choice.Contains("duoshi")) return "duoshi";

            return "cancel"; 
        }
    }

    public class HaoshiExtraAI : SkillEvent
    {
        public HaoshiExtraAI() : base("haoshiextra")
        {
        }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (!ai.WillShowForAttack() && !ai.WillShowForDefence()) return false;

            Room room = ai.Room;
            int draw = (int)data;
            int count = player.HandcardNum + draw + 2;
            if (player.HasTreasure("JadeSeal"))
                count++;

            if (count > 5)
            {
                int least = 1000;
                foreach (Player p in room.GetOtherPlayers(player))
                    least = Math.Min(player.HandcardNum, least);

                bool check = false;
                foreach (Player p in ai.FriendNoSelf)
                {
                    if (p.HandcardNum == least)
                    {
                        check = true;
                        break;
                    }
                }
                return check;
            }
            else
                return true;
        }
    }

    public class ShelieAI : SkillEvent
    {
        public ShelieAI() : base("shelie")
        {}

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
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
            List<int> h = new List<int>(), s = new List<int>(), d = new List<int>(), c = new List<int>();
            foreach (int id in ups)
            {
                WrappedCard card = room.GetCard(id);
                if (card.Suit == WrappedCard.CardSuit.Heart)
                    h.Add(id);
                else if (card.Suit == WrappedCard.CardSuit.Spade)
                    s.Add(id);
                else if (card.Suit == WrappedCard.CardSuit.Diamond)
                    d.Add(id);
                else if (card.Suit == WrappedCard.CardSuit.Club)
                    c.Add(id);
            }

            if (h.Count > 1)
            {
                ai.SortByUseValue(ref h);
                result.Bottom.Add(h[0]);
            }
            else if (h.Count == 1)
                result.Bottom.Add(h[0]);

            if (c.Count > 1)
            {
                ai.SortByUseValue(ref c);
                result.Bottom.Add(c[0]);
            }
            else if (c.Count == 1)
                result.Bottom.Add(c[0]);

            if (d.Count > 1)
            {
                ai.SortByUseValue(ref d);
                result.Bottom.Add(d[0]);
            }
            else if (d.Count == 1)
                result.Bottom.Add(d[0]);

            if (s.Count > 1)
            {
                ai.SortByUseValue(ref s);
                result.Bottom.Add(s[0]);
            }
            else if (s.Count == 1)
                result.Bottom.Add(s[0]);

            foreach (int id in ups)
                if (!result.Bottom.Contains(id))
                    result.Top.Add(id);

            return result;
        }
    }
}