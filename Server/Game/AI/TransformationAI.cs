using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                new YiguiAI(),
                new JihunAI(),
                new XuanlueAI(),
                new YongjinAI(),
                new DiaoduAI(),
                new DiancaiAI(),
                new LianziAI(),
                new FlamemapChoiceAI(),
                new FlamemapAI(),
                new YingziExtraAI(),
                new HaoshiExtraAI(),
                new DuoshiEAI(),
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
            double value = ai.GetGeneralStength(player, false, true);
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

        public override List<int> OnDiscard(TrustedAI ai, Player player, List<int> cards, int min, int max, bool option)
        {
            Room room = ai.Room;
            List<int> ids = new List<int>();
            foreach (int id in player.GetCards("h"))
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
            if (!player.IsKongcheng() && !player.HasUsed(QiceCard.ClassName) && ai.WillShowForAttack())
            {
                Room room = ai.Room;
                foreach (int id in player.GetCards("h"))
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
                FunctionCard fcard = Engine.GetFunctionCard(QiceCard.ClassName);
                WrappedCard qice = new WrappedCard(QiceCard.ClassName);
                qice.AddSubCards(player.GetCards("h"));

                List<WrappedCard> guhuos = QiceVS.GetGuhuoCards(room, player);
                double good = -1;
                foreach (WrappedCard card in guhuos)
                {
                    if (card.Name == GodSalvation.ClassName)
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
                        if (card.Name == AllianceFeast.ClassName)
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

                            if (best > good && (room.BloodBattle || best >= 2) && target != null)
                            {
                                ai.Target[Name] = target;
                                return new List<WrappedCard> { qice };
                            }
                        }
                    }
                }

                if (good > 0 && (room.BloodBattle || good > 1))
                {
                    qice.UserString = GodSalvation.ClassName;
                    return new List<WrappedCard> { qice };
                }

                List<WrappedCard> result = new List<WrappedCard>();
                foreach (WrappedCard card in guhuos)
                {
                    if (card.Name == BefriendAttacking.ClassName)
                    {
                        foreach (Player p in ai.FriendNoSelf)
                        {
                            if (!ai.IsCancelTarget(card, p, player) && p.HasShownOneGeneral()
                                && RoomLogic.IsProhibited(room, player, p, card) == null && !RoomLogic.IsFriendWith(room, player, p))
                            {
                                qice.UserString = card.Name;
                                card.UserString = RoomLogic.CardToString(room, qice);
                                result.Add(card);
                                return result;
                            }
                        }
                    }
                }
                foreach (WrappedCard card in guhuos)
                {

                    if (card.Name == ExNihilo.ClassName)
                    {
                        qice.UserString = card.Name;
                        card.UserString = RoomLogic.CardToString(room, qice);
                        result.Add(card);
                    }
                    return result;
                }
            }

            return new List<WrappedCard>();
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, CardUseStruct use)
        {
            if (use.Card.Skill == Name)
                return -10;

            return 0;
        }
    }

    public class QiceCardAI : UseCard
    {
        public QiceCardAI() : base(QiceCard.ClassName)
        { }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            use.Card = card;
            if (ai.Target["qice"] != null)
                use.To = new List<Player> { ai.Target["qice"] };
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return -1;
        }
    }

    public class YuejianAI : SkillEvent
    {
        public YuejianAI() : base("yuejian") { }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (player.HandcardNum > player.Hp)
                return true;

            return false;
        }
    }

    public class WanweiAI : SkillEvent
    {
        public WanweiAI() : base("wanwei") { }
        /*
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
            if (cards.Count == 0) return use;

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
        */
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data) => true;
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
                ScoreStruct get = ai.FindCards2Discard(player, damage.To, string.Empty, "ej", HandlingMethod.MethodGet);
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
            if (!player.IsNude() && !player.HasUsed(SanyaoCard.ClassName))
             {
                WrappedCard first = new WrappedCard(SanyaoCard.ClassName)
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
        public SanyaoCardAI() : base(SanyaoCard.ClassName) { }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
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

        public static int FindXiongsuanCard(TrustedAI ai, Player player)
        {
            int result = -1;

            Room room = ai.Room;
            List<int> ids = new List<int>();
            foreach (int id in player.GetCards("h"))
            {
                if (RoomLogic.CanDiscard(room, player, player, id))
                    ids.Add(id);
            }
            if (ids.Count > 0)
            {
                ai.SortByKeepValue(ref ids, false);
                if (ai.GetKeepValue(ids[0], player) < 0)
                    return ids[0];

                Dictionary<int, double> values = new Dictionary<int, double>();
                List<int> equips = new List<int>();
                foreach (int id in ids)
                {
                    WrappedCard _c = room.GetCard(id);
                    if (Engine.GetFunctionCard(_c.Name) is EquipCard)
                        equips.Add(id);
                    else
                        values[id] = ai.GetKeepValue(id, player);
                }
                List<int> weapons = new List<int>(), armors = new List<int>(), ohorse = new List<int>(), dhorse = new List<int>(), speacial = new List<int>(), treasure = new List<int>();
                foreach (int id in equips)
                {
                    WrappedCard _c = room.GetCard(id);
                    double basic = Engine.GetCardUseValue(_c.Name);
                    UseCard card_event = Engine.GetCardUsage(_c.Name);
                    if (card_event != null)
                        basic += card_event.CardValue(ai, player, true, _c, Player.Place.PlaceEquip);
                    values[id] = basic;
                    if (Engine.GetFunctionCard(_c.Name) is Weapon)
                        weapons.Add(id);
                    else if (Engine.GetFunctionCard(_c.Name) is Armor)
                        armors.Add(id);
                    else if (Engine.GetFunctionCard(_c.Name) is OffensiveHorse)
                        ohorse.Add(id);
                    else if (Engine.GetFunctionCard(_c.Name) is DefensiveHorse)
                        dhorse.Add(id);
                    else if (Engine.GetFunctionCard(_c.Name) is SpecialEquip)
                        speacial.Add(id);
                    else
                        treasure.Add(id);
                }

                if (speacial.Count > 0)
                {
                    if (ohorse.Count > 0)
                        return ohorse[0];
                    if (dhorse.Count > 0)
                        return dhorse[0];
                }
                else
                {
                    if (ohorse.Count > 1)
                    {
                        ohorse.Sort((x, y) => { return values[x] > values[y] ? 1 : -1; });
                        return ohorse[0];
                    }
                    if (dhorse.Count > 1)
                    {
                        dhorse.Sort((x, y) => { return values[x] > values[y] ? 1 : -1; });
                        return dhorse[0];
                    }
                }

                if (weapons.Count > 1)
                {
                    weapons.Sort((x, y) => { return values[x] > values[y] ? 1 : -1; });
                    return weapons[0];
                }

                if (treasure.Count > 1)
                {
                    treasure.Sort((x, y) => { return values[x] > values[y] ? 1 : -1; });
                    return treasure[0];
                }

                ids.Sort((x, y) => { return values[x] > values[y] ? 1 : -1; });
                if (ai.GetUseValue(ids[0], player) < 6)
                    return ids[0];
            }

            return result;
        }
        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (ai.WillShowForAttack() && !player.IsKongcheng()  && RoomLogic.CanDiscard(ai.Room, player, player, "h") && player.GetMark("@xiong") > 0)
            {
                WrappedCard first = new WrappedCard(XiongsuanCard.ClassName)
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
        public XiongsuanCardAI() : base(XiongsuanCard.ClassName) { }
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

            int sub = XiongSuanAI.FindXiongsuanCard(ai, player);
            if (target != null && sub >= 0)
            {
                card.AddSubCard(sub);
                use.Card = card;
                use.To = new List<Player> { target };
                return;
            }
            
            DamageStruct damage_self = new DamageStruct("xiongsuan", player, player);
            if (ai.GetDamageScore(damage_self).Score >= 0 || player.GetLostHp() == 0 || (player.IsWounded() && player.HasArmor(SilverLion.ClassName))
                || (player.Hp == 1 && ai.GetKnownCardsNums(Analeptic.ClassName, "he", player) > 0 && player.GetCards("he").Count > 1))
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

    public class YiguiAI : SkillEvent
    {
        public YiguiAI() : base("yigui")
        { }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            List<WrappedCard> result = new List<WrappedCard>();
            if (!player.ContainsTag("spirit")) return result;
            Room room = ai.Room;
            bool slash = !player.HasFlag("yigui_Slash");
            foreach (string general in (List<string>)player.GetTag("spirit"))
            {
                string kingdom = Engine.GetGeneral(general, room.Setting.GameMode).Kingdom;
                if (kingdom != "qun" && slash)
                {
                    WrappedCard _Slash = new WrappedCard(Slash.ClassName)
                    {
                        Skill = Name,
                        UserString = general
                    };
                    result.Add(_Slash);
                    slash = false;
                }
                if (kingdom == "qun")
                {
                    int count = 0;
                    foreach (Player p in ai.GetFriends(player))
                        if (RoomLogic.IsFriendWith(room, p, player) && p.IsWounded())
                            count++;

                    if (count > 1 && !player.HasFlag("yigui_GodSalvation"))
                    {
                        WrappedCard god = new WrappedCard(GodSalvation.ClassName)
                        {
                            Skill = Name,
                            UserString = general
                        };
                        result.Add(god);
                    }
                    else if (player.IsWounded() && !player.HasFlag("yigui_Peach"))
                    {
                        WrappedCard peach = new WrappedCard(Peach.ClassName)
                        {
                            Skill = Name,
                            UserString = general
                        };
                        result.Add(peach);
                    }

                    if (!player.HasFlag("yigui_Analeptic"))
                    {
                        WrappedCard peach = new WrappedCard(Analeptic.ClassName)
                        {
                            Skill = Name,
                            UserString = general
                        };

                        result.Add(peach);
                    }
                }
                else
                {
                    if (!player.HasFlag("yigui_SavageAssault"))
                    {
                        WrappedCard card = new WrappedCard(SavageAssault.ClassName)
                        {
                            Skill = Name,
                            UserString = general
                        };
                        result.Add(card);
                    }
                    if (!player.HasFlag("yigui_ArcheryAttack"))
                    {
                        WrappedCard card = new WrappedCard(ArcheryAttack.ClassName)
                        {
                            Skill = Name,
                            UserString = general
                        };
                        result.Add(card);
                    }
                    if (!player.HasFlag("yigui_BefriendAttacking"))
                    {
                        WrappedCard card = new WrappedCard(BefriendAttacking.ClassName)
                        {
                            Skill = Name,
                            UserString = general
                        };
                        result.Add(card);
                    }
                    if (!player.HasFlag("yigui_BurningCamps"))
                    {
                        WrappedCard card = new WrappedCard(BurningCamps.ClassName)
                        {
                            Skill = Name,
                            UserString = general
                        };
                        result.Add(card);
                    }
                }
            }

            return result;
        }

        public override double UseValueAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            if (card.Name == Analeptic.ClassName)
                return -4;

            return 0;
        }

        public override List<WrappedCard> GetViewAsCards(TrustedAI ai, string pattern, Player player)
        {
            List<WrappedCard> result = new List<WrappedCard>();
            if (!player.ContainsTag("spirit")) return result;

            if (pattern == Slash.ClassName && !player.HasFlag("yigui_Slash")
                && (ai.Room.GetRoomState().GetCurrentCardUseReason() == CardUseStruct.CardUseReason.CARD_USE_REASON_PLAY
                || ai.Room.GetRoomState().GetCurrentCardUseReason() == CardUseStruct.CardUseReason.CARD_USE_REASON_RESPONSE_USE))
            {
                foreach (string general in (List<string>)player.GetTag("spirit"))
                {
                    string kingdom = Engine.GetGeneral(general, ai.Room.Setting.GameMode).Kingdom;
                    if (kingdom != "qun")
                    {
                        WrappedCard slash = new WrappedCard(Slash.ClassName)
                        {
                            Skill = Name,
                            UserString = general
                        };
                        WrappedCard fslash = new WrappedCard(FireSlash.ClassName)
                        {
                            Skill = Name,
                            UserString = general
                        };
                        WrappedCard tslash = new WrappedCard(ThunderSlash.ClassName)
                        {
                            Skill = Name,
                            UserString = general
                        };
                        result.Add(slash);
                        result.Add(fslash);
                        result.Add(tslash);
                    }
                }
            }
            else if (pattern == Peach.ClassName && !player.HasFlag("yigui_Peach"))
            {
                foreach (string general in (List<string>)player.GetTag("spirit"))
                {
                    WrappedCard card = new WrappedCard(Peach.ClassName)
                    {
                        Skill = Name,
                        UserString = general
                    };
                    result.Add(card);
                }
            }
            else if (pattern == Analeptic.ClassName && !player.HasFlag("yigui_Analeptic"))
            {
                foreach (string general in (List<string>)player.GetTag("spirit"))
                {
                    string kingdom = Engine.GetGeneral(general, ai.Room.Setting.GameMode).Kingdom;
                    if (kingdom == "qun")
                    {
                        WrappedCard card = new WrappedCard(Analeptic.ClassName)
                        {
                            Skill = Name,
                            UserString = general
                        };
                        result.Add(card);
                    }
                }
            }

            return result;
        }
    }
    public class JihunAI : SkillEvent
    {
        public JihunAI() : base("jihun")
        {
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            return true;
        }

        public override ScoreStruct GetDamageScore(TrustedAI ai, DamageStruct damage)
        {
            ScoreStruct score = new ScoreStruct
            {
                Score = 0
            };

            if (damage.To != null && ai.HasSkill(Name, damage.To) && ai.IsEnemy(damage.To) && damage.Damage == 1)
                score.Score = -1;

            return score;
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
                ScoreStruct score = ai.FindCards2Discard(player, p, string.Empty, "he", FunctionCard.HandlingMethod.MethodDiscard);
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
                        if (RoomLogic.CanPutEquip(_p, room.GetCard(id)) && ai.GetSameEquip(room.GetCard(id), _p) == null)
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
                            if (ai.IsFriend(_p) && RoomLogic.CanPutEquip(_p, room.GetCard(id)) && ai.GetSameEquip(room.GetCard(id), _p) == null
                                && (!player_location.ContainsKey(_p) || !player_location[_p].Contains(location)))
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

        public override CardUseStruct OnResponding(TrustedAI ai, Player player, string pattern, string prompt, object data)
        {
            CardUseStruct use = new CardUseStruct
            {
                From = player,
                To = new List<Player>()
            };
            Room room = ai.Room;
            List<int> ids = new List<int>();
            foreach (int id in player.GetCards("he"))
            {
                if (Engine.GetFunctionCard(room.GetCard(id).Name) is EquipCard)
                    ids.Add(id);
            }
            if (ids.Count > 0)
            {
                ai.SortByKeepValue(ref ids);
                if (ai.GetKeepValue(ids[0], player) < 0)
                {
                    foreach (Player p in room.GetOtherPlayers(player))
                    {
                        if (ai.HasSkill(TrustedAI.LoseEquipSkill, p) && ai.IsFriend(p))
                        {
                            use.Card = new WrappedCard(DiaoduCard.ClassName) { Skill = Name, ShowSkill = Name };
                            use.Card.AddSubCard(ids[0]);
                            use.To.Add(p);
                            return use;
                        }
                    }

                    foreach (Player p in room.GetOtherPlayers(player))
                    {
                        if (ai.IsFriend(p) && ai.GetSameEquip(room.GetCard(ids[0]), p) == null)
                        {
                            use.Card = new WrappedCard(DiaoduCard.ClassName) { Skill = Name, ShowSkill = Name };
                            use.Card.AddSubCard(ids[0]);
                            use.To.Add(p);
                            return use;
                        }
                    }
                }
            }

            double best_value = 0;
            Player target = null;
            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (RoomLogic.IsFriendWith(room, p, player) && RoomLogic.CanGetCard(room, player, p, "e"))
                    targets.Add(p);
            }
            foreach (Player p in targets)
            {
                foreach (int card_id in player.GetEquips())
                {
                    if (RoomLogic.CanGetCard(room, player, p, card_id))
                    {
                        double v = ai.GetKeepValue(card_id, p, Player.Place.PlaceEquip);
                        if (v < best_value)
                        {
                            best_value = v;
                            target = p;
                        }
                    }
                }
            }
            if (target != null)
            {
                use.Card = new WrappedCard(DiaoduCard.ClassName) { Skill = Name, ShowSkill = Name };
                use.To.Add(target);
                return use;
            }

            foreach (Player p in targets)
            {
                foreach (int card_id in player.GetEquips())
                {
                    if (!RoomLogic.CanGetCard(room, player, p, card_id)) continue;
                    FunctionCard fcard = Engine.GetFunctionCard(room.GetCard(card_id).Name);
                    if (fcard is Weapon || fcard is OffensiveHorse)
                    {
                        use.Card = new WrappedCard(DiaoduCard.ClassName) { Skill = Name, ShowSkill = Name };
                        use.To.Add(p);
                        return use;
                    }
                }
            }

            foreach (Player p in targets)
            {
                foreach (int card_id in player.GetEquips())
                {
                    if (!RoomLogic.CanGetCard(room, player, p, card_id)) continue;
                    FunctionCard fcard = Engine.GetFunctionCard(room.GetCard(card_id).Name);
                    if ((fcard is Armor || fcard is DefensiveHorse || fcard is SpecialEquip) && ai.GetEnemisBySeat(p) == 0)
                    {
                        use.Card = new WrappedCard(DiaoduCard.ClassName) { Skill = Name, ShowSkill = Name };
                        use.To.Add(p);
                        return use;
                    }
                }
            }

            foreach (Player p in targets)
            {
                foreach (int card_id in player.GetEquips())
                {
                    if (!RoomLogic.CanGetCard(room, player, p, card_id)) continue;
                    FunctionCard fcard = Engine.GetFunctionCard(room.GetCard(card_id).Name);
                    if (fcard is Treasure)
                    {
                        if (fcard is WoodenOx && p.GetPile("wooden_ox").Count > 0 || ai.GetSameEquip(room.GetCard(card_id), player) != null) continue;
                        List<int> self_ids = player.GetCards("h");
                        self_ids.AddRange(player.GetHandPile());
                        foreach (int _id in self_ids)
                            if (Engine.GetFunctionCard(room.GetCard(_id).Name) is Treasure)
                                continue;

                        use.Card = new WrappedCard(DiaoduCard.ClassName) { Skill = Name, ShowSkill = Name };
                        use.To.Add(p);
                        return use;
                    }
                }
            }

            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (ai.HasSkill(TrustedAI.LoseEquipSkill, p) && p.GetEquips().Count > 0 && RoomLogic.IsFriendWith(room, player, p))
                {
                    if (p.GetOffensiveHorse() && RoomLogic.CanGetCard(room, player, p, p.OffensiveHorse.Key))
                    {
                        use.Card = new WrappedCard(DiaoduCard.ClassName) { Skill = Name, ShowSkill = Name };
                        use.To.Add(p);
                        return use;
                    }
                    if (p.GetWeapon() && RoomLogic.CanGetCard(room, player, p, p.Weapon.Key))
                    {
                        use.Card = new WrappedCard(DiaoduCard.ClassName) { Skill = Name, ShowSkill = Name };
                        use.To.Add(p);
                        return use;
                    }

                    foreach (Player p2 in ai.GetFriends(player))
                    {
                        if (p2 != p && ai.HasSkill(TrustedAI.LoseEquipSkill, p2))
                        {
                            use.Card = new WrappedCard(DiaoduCard.ClassName) { Skill = Name, ShowSkill = Name };
                            use.To.Add(p);
                            return use;
                        }
                    }
                }
            }

            return use;
        }

        public override List<Player> OnPlayerChosen(TrustedAI ai, Player player, List<Player> target, int min, int max)
        {
            if (ai.HasSkill(TrustedAI.LoseEquipSkill)) return new List<Player>();
            foreach (Player p in target)
                if (ai.IsFriend(p) && (ai.HasSkill(TrustedAI.LoseEquipSkill, p) || ai.HasSkill(TrustedAI.NeedEquipSkill, p)))
                    return new List<Player> { p };

            Room room = ai.Room;
            int id = (int)player.GetTag(Name);
            if (Engine.GetFunctionCard(room.GetCard(id).Name) is EquipCard fequip)
            {
                WrappedCard equip = ai.GetSameEquip(room.GetCard(id), player);
                if (equip != null)
                {
                    foreach (Player p in target)
                        if (ai.IsFriend(p) && ai.GetSameEquip(room.GetCard(id), p) == null)
                            return new List<Player> { p };
                }
                List<int> ids = player.GetCards("h");
                ids.AddRange(player.GetHandPile());
                foreach (int card_id in ids)
                {
                    if (card_id == id) continue;
                    if (Engine.GetFunctionCard(room.GetCard(card_id).Name) is EquipCard _fequip)
                    {
                        if (_fequip.EquipLocation() == fequip.EquipLocation()
                            || (fequip.EquipLocation() == EquipCard.Location.SpecialLocation && _fequip.EquipLocation() == EquipCard.Location.DefensiveHorseLocation)
                            || (_fequip.EquipLocation() == EquipCard.Location.SpecialLocation && (fequip.EquipLocation() == EquipCard.Location.DefensiveHorseLocation
                            || fequip.EquipLocation() == EquipCard.Location.OffensiveHorseLocation)))
                        {
                            foreach (Player p in target)
                                if (ai.IsFriend(p) && ai.GetSameEquip(room.GetCard(id), p) == null)
                                    return new List<Player> { p };
                        }
                    }
                }
            }

            return new List<Player>();
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
                    foreach (int id in player.GetCards("h"))
                    {
                        FunctionCard fcard = Engine.GetFunctionCard(ai.Room.GetCard(id).Name);
                        if (fcard is BasicCard && RoomLogic.CanDiscard(ai.Room, player, player, id))
                            ids.Add(id);
                    }

                    if (ids.Count > 0)
                    {
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
            }

            return null;
        }
    }

    public class LianziCardAI : UseCard
    {
        public LianziCardAI() : base("LianziCard")
        {
        }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
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
                WrappedCard slash = new WrappedCard("FlamemapCard")
                {
                    Mute = true,
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
                if ((lord_liubei != null && card.Name == DragonPhoenix.ClassName) || (lord_jiaozhu != null && card.Name == PeaceSpell.ClassName))
                    ids.Remove(id);

                if (card.Name == LuminouSpearl.ClassName)
                    return id;
            }

            if (ids.Count > 0) return ids[0];

            return card_ids[0];
        }
    }

    public class FlamemapCardAI : UseCard
    {
        public FlamemapCardAI() : base("FlamemapCard") { }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return (ai.HasSkill("zhiheng") || player.HasTreasure(LuminouSpearl.ClassName) && !player.HasUsed("ZhihengCard")) ? 4.5 : 1.5;
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
                foreach (int id in ids)
                {
                    WrappedCard eq = room.GetCard(id);
                    if (eq.Name !=  LuminouSpearl.ClassName && ai.GetKeepValue(id, player) < 0)
                    {
                        card.AddSubCard(ids[0]);
                        use.Card = card;
                        return;
                    }
                }

                ai.SortByUseValue(ref ids, false);
                foreach (int id in ids)
                {
                    WrappedCard eq = room.GetCard(id);
                    if (eq.Name == LuminouSpearl.ClassName) continue;
                    if (ai.GetUseValue(ids[0], player) < 5)
                    {
                        card.AddSubCard(ids[0]);
                        use.Card = card;
                        return;
                    }
                    else if (room.GetCardPlace(id) == Player.Place.PlaceHand)
                    {
                        if (ai.GetSameEquip(eq, player) != null)
                        {
                            card.AddSubCard(ids[0]);
                            use.Card = card;
                            return;
                        }
                    }
                }

                Player lord = ai.FindPlayerBySkill("jiahe");
                if (lord != null && lord.GetPile("flame_map").Count < 5)
                {
                    foreach (int id in ids)
                    {
                        WrappedCard eq = room.GetCard(id);
                        if (eq.Name != LuminouSpearl.ClassName && ai.GetKeepValue(id, player) < 7)
                        {
                            card.AddSubCard(id);
                            use.Card = card;
                            return;
                        }
                    }
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
            if (player.HasTreasure(JadeSeal.ClassName)) n++;

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

            if (choice.Contains("duoshiextra")) return "duoshiextra";

            return "cancel"; 
        }
    }

    public class YingziExtraAI : SkillEvent
    {
        public YingziExtraAI() : base("yingziextra") { }
        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            Room room = ai.Room;
            if (ai.HasSkill("haoshi+haoshiextra")) return true;

            if (ai.HasSkill("haoshi|haoshiextra"))
            {
                int count = player.HandcardNum + 4;
                if (player.HasTreasure(JadeSeal.ClassName))
                    count++;

                if (count <= 5 && count + 1 > 5)
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
            }

            return true;
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
            if (player.HasTreasure(JadeSeal.ClassName))
                count++;

            if (count > 5)
            {
                int least = 1000;
                foreach (Player p in room.GetOtherPlayers(player))
                    least = Math.Min(p.HandcardNum, least);

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

    public class DuoshiEAI : SkillEvent
    {
        public DuoshiEAI() : base("duoshiextra")
        {
        }

        public override WrappedCard ViewAs(TrustedAI ai, Player player, int id, bool current, Player.Place place)
        {
            if (WrappedCard.IsRed(ai.Room.GetCard(id).Suit) && (player.GetHandPile().Contains(id) || place == Player.Place.PlaceHand))
            {
                WrappedCard await = new WrappedCard(AwaitExhausted.ClassName);
                await.AddSubCard(id);
                await.Skill = Name;
                await.ShowSkill = Name;
                await = RoomLogic.ParseUseCard(ai.Room, await);
                return await;
            }

            return null;
        }

        public override List<WrappedCard> GetTurnUse(TrustedAI ai, Player player)
        {
            if (player.UsedTimes("ViewAsSkill_duoshiextraCard") < 4 && (ai.WillShowForAttack() || ai.WillShowForDefence()))
            {
                Room room = ai.Room;
                List<int> ids= player.GetCards("h");
                ids.AddRange(player.GetHandPile());
                ai.SortByKeepValue(ref ids, false);

                bool need_lose_equip = false;
                foreach (int id in player.GetEquips())
                {
                    if (ai.GetKeepValue(id, player) < 0)
                    {
                        need_lose_equip = true;
                        break;
                    }
                }
                if (ai.IsWeak() && (player.GetWeapon() || player.GetOffensiveHorse()))
                    need_lose_equip = true;

                foreach (int id in ids)
                {
                    if (WrappedCard.IsRed(ai.Room.GetCard(id).Suit) && room.GetCard(id).Name != AwaitExhausted.ClassName
                        && (ai.GetKeepValue(id, player) < 3 || ai.GetOverflow(player) > 0 || need_lose_equip))
                    {
                        WrappedCard await = new WrappedCard(AwaitExhausted.ClassName)
                        {
                            Skill = Name,
                            ShowSkill = Name
                        };
                        await.AddSubCard(id);
                        await = RoomLogic.ParseUseCard(ai.Room, await);
                        return new List<WrappedCard> { await };
                    }
                }
            }

            return null;
        }
        public override double UsePriorityAdjust(TrustedAI ai, Player player, CardUseStruct use)
        {
            if (use.Card.Skill == Name)
            {
                bool friend = false;
                foreach (Player p in ai.Room.GetAlivePlayers())
                {
                    if (RoomLogic.IsFriendWith(ai.Room, p, player))
                    {
                        friend = true;
                        break;
                    }
                }

                if (!friend) return -3.5;
            }

            return 0;
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