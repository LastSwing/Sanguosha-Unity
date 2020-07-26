using CommonClass;
using CommonClass.Game;
using SanguoshaServer.Game;
using SanguoshaServer.Package;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using static SanguoshaServer.Package.FunctionCard;

namespace SanguoshaServer.AI
{
    public class GuanduAI : TrustedAI
    {
        public GuanduAI(Room room, Player player) : base(room, player)
        {
        }
        public override void Event(TriggerEvent triggerEvent, Player player, object data)
        {
            if (!self.Alive) return;

            base.Event(triggerEvent, player, data);

            if (triggerEvent == TriggerEvent.EventPhaseStart || triggerEvent == TriggerEvent.BuryVictim)
                UpdatePlayers();

            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move)
            {
                bool open = false;
                bool pile_open = false;
                Player from = move.From;
                Player to = move.To;
                
                if ((from != null && (from == self || self.IsSameCamp(from))) || (to != null && (to == self || self.IsSameCamp(to)) && move.To_place != Player.Place.PlaceSpecial))
                    open = true;

                if (!open && to != null && !string.IsNullOrEmpty(move.To_pile_name) && !move.To_pile_name.StartsWith("#") && move.To != null)
                {
                    if (move.To.GetPileOpener(move.To_pile_name).Count == room.GetAllPlayers(true).Count)
                        pile_open = true;
                    else
                    {
                        foreach (string name in move.To.GetPileOpener(move.To_pile_name))
                        {
                            Player who = room.FindPlayer(name, true);
                            if (who != null && (who == self || self.IsSameCamp(who)))
                            {
                                open = true;
                                break;
                            }
                        }
                    }
                }

                if (to != null && move.To_place == Player.Place.PlaceHand)
                {
                    foreach (int id in move.Card_ids)
                    {
                        int index = move.Card_ids.IndexOf(id);
                        WrappedCard card = room.GetCard(id);
                        if (card.HasFlag("visible") || pile_open
                                || move.From_places[index] == Player.Place.PlaceEquip || move.From_places[index] == Player.Place.PlaceDelayedTrick
                                || move.From_places[index] == Player.Place.DiscardPile || move.From_places[index] == Player.Place.PlaceTable)
                        {
                            public_handcards[to].Add(id);
                            private_handcards[to].Add(id);
                            ClearCardLack(to, id);
                        }
                        else if (open)
                        {
                            private_handcards[to].Add(id);
                            ClearCardLack(to, id);
                        }
                        else
                        {
                            ClearCardLack(to);
                        }
                    }
                }

                if (to != null && move.To_place == Player.Place.PlaceSpecial && move.To_pile_name == "wooden_ox")
                {
                    foreach (int id in move.Card_ids)
                    {
                        if (open)
                            wooden_cards[to].Add(id);
                    }
                }

                if (from != null && move.From_places.Contains(Player.Place.PlaceHand))
                {
                    foreach (int id in move.Card_ids)
                    {
                        if (room.GetCard(id).HasFlag("visible") || pile_open || move.To_place == Player.Place.PlaceEquip
                                || move.To_place == Player.Place.PlaceDelayedTrick || move.To_place == Player.Place.DiscardPile
                                || move.To_place == Player.Place.PlaceTable)
                        {
                            public_handcards[from].RemoveAll(t => t == id);
                            private_handcards[from].RemoveAll(t => t == id);
                        }
                        else
                        {
                            public_handcards[from].Clear();
                            if (open)
                                private_handcards[from].RemoveAll(t => t == id);
                            else
                                private_handcards[from].Clear();
                        }
                    }
                }

                if (from != null && move.From_places.Contains(Player.Place.PlaceSpecial) && move.From_pile_names.Contains("wooden_ox"))
                {
                    foreach (int id in move.Card_ids)
                    {
                        int index = move.Card_ids.IndexOf(id);
                        if (open && move.From_pile_names[index] == "wooden_ox" && move.From_places[index] == Player.Place.PlaceSpecial)
                            wooden_cards[move.From].RemoveAll(t => t == id);
                    }
                }

                foreach (int id in move.Card_ids)
                {
                    int index = move.Card_ids.IndexOf(id);
                    WrappedCard card = room.GetCard(id);
                    if (move.From_places[index] == Player.Place.DrawPile)
                    {
                        if (move.To != null && move.To_place == Player.Place.PlaceHand && card.HasFlag("visible2" + self.Name))
                            private_handcards[move.To].Add(id);

                        if (guanxing.Key != null && guanxing.Value.Contains(id))
                        {
                            if (guanxing.Value[0] != id)
                            {
                                List<int> top_cards = new List<int>(guanxing.Value);
                                for (int y = top_cards.Count - 1; y >= top_cards.IndexOf(id); y--)
                                    guanxing.Value.RemoveAt(y);
                            }
                            else
                                guanxing.Value.RemoveAll(t => t == id);
                            if (guanxing.Value.Count == 0) guanxing = new KeyValuePair<Player, List<int>>();
                        }
                    }
                }
            }

            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                string class_name = fcard.Name;
                if (fcard is Slash) class_name = Slash.ClassName;
                UseCard e = Engine.GetCardUsage(class_name);
                if (e != null)
                    e.OnEvent(this, triggerEvent, player, data);
            }

            if (triggerEvent == TriggerEvent.ChoiceMade && data is string str)
            {
                List<string> choices = new List<string>(str.Split(':'));
                foreach (SkillEvent e in skill_events.Values)
                {
                    foreach (string key in e.Key)
                        if (str.StartsWith(key))
                            e.OnEvent(this, triggerEvent, player, data);
                }

                foreach (UseCard e in Engine.GetCardUsages())
                {
                    foreach (string key in e.Key)
                        if (str.StartsWith(key))
                            e.OnEvent(this, triggerEvent, player, data);
                }

                if (choices[0] == "viewCards")
                {
                    List<int> ids = new List<int>();
                    if (choices[choices.Count - 1] == "all")
                        player.GetCards("h");
                    else
                    {
                        List<string> card_str = new List<string>(choices[choices.Count - 1].Split('+'));
                        ids = JsonUntity.StringList2IntList(card_str);
                    }
                    if (choices[choices.Count - 2] == "all")
                    {
                        foreach (int id in ids)
                            SetPublicKnownCards(player, id);
                    }
                    else if (choices[choices.Count - 2] == self.Name)
                        foreach (int id in ids)
                            SetPrivateKnownCards(player, id);
                }
                else if (choices[0] == "showCards")
                {
                    List<int> ids = JsonUntity.StringList2IntList(new List<string>(choices[2].Split('+')));
                    if (choices[choices.Count - 1] == "all")
                    {
                        foreach (int id in ids)
                        {
                            if (!public_handcards[player].Contains(id)) public_handcards[player].Add(id);
                            if (!private_handcards[player].Contains(id)) private_handcards[player].Add(id);
                        }
                    }
                    else if (choices[choices.Count - 1] == self.Name)
                        foreach (int id in ids)
                            if (!private_handcards[player].Contains(id))
                                private_handcards[player].Add(id);
                }
                else if (choices[0] == "cardShow")
                {
                    int id = int.Parse(choices[choices.Count - 1].Substring(1, choices[choices.Count - 1].Length - 2));
                    if (!public_handcards[player].Contains(id)) public_handcards[player].Add(id);
                    if (!private_handcards[player].Contains(id)) private_handcards[player].Add(id);
                }
                else if (choices[0] == "ViewTopCards" || choices[0] == "ViewBottomCards")
                {
                    bool open = choices[choices.Count - 1] == "open";
                    List<int> drawpile = new List<int>(room.DrawPile);
                    List<int> moves = JsonUntity.StringList2IntList(new List<string>(choices[2].Split('+')));
                    if (choices[0] == "ViewTopCards")
                    {
                        guanxing = new KeyValuePair<Player, List<int>>();
                        if (open)
                        {
                            for (int index = 0; index < moves.Count; index++)
                            {
                                int id = moves[index];
                                room.SetCardFlag(id, "visible");
                            }
                        }
                        else
                        {
                            foreach (int id in moves)
                                if (player == self || player.IsSameCamp(self))
                                    room.SetCardFlag(id, "visible2" + self.Name);

                            guanxing = new KeyValuePair<Player, List<int>>(player, moves);
                        }
                    }
                    else
                    {
                        if (open)
                        {
                            for (int index = 0; index < moves.Count; index++)
                            {
                                int id = moves[index];
                                room.SetCardFlag(id, "visible");
                            }
                        }
                        else
                        {
                            foreach (int id in moves)
                                room.SetCardFlag(id, "visible2" + choices[1]);
                        }
                    }
                }
            }
            
        }

        public override ScoreStruct GetDamageScore(DamageStruct _damage, DamageStruct.DamageStep step = DamageStruct.DamageStep.Caused)
        {
            DamageStruct damage = new DamageStruct(_damage.Card, _damage.From, _damage.To, _damage.Damage, _damage.Nature)
            {
                Reason = _damage.Reason,
                Steped = _damage.Steped,
                Transfer = _damage.Transfer,
                TransferReason = _damage.TransferReason,
                Chain = _damage.Chain
            };

            if (!HasSkill("gangzhi", damage.To))
                damage.Damage = DamageEffect(damage, step);

            Player from = damage.From;
            Player to = damage.To;
            if (damage.Steped < DamageStruct.DamageStep.Caused)
                damage.Steped = DamageStruct.DamageStep.Caused;

            ScoreStruct result_score = new ScoreStruct
            {
                Damage = damage,
                DoDamage = true
            };

            if (!HasSkill("gangzhi", damage.To))
                damage.Damage = DamageEffect(damage, DamageStruct.DamageStep.Done);
            damage.Steped = DamageStruct.DamageStep.Done;
            result_score.Damage = damage;

            List<ScoreStruct> scores = new List<ScoreStruct>();
            bool deadly = false;
            if (damage.Damage > 0 && !to.Removed)
            {
                double value = 0;
                value = Math.Min(damage.Damage, to.Hp) * 3.5;
                if (IsWeak(to))
                {
                    value += 4;
                    if (damage.Damage > to.Hp)
                        if (!CanSave(to, damage.Damage - to.Hp + 1))
                        {
                            int over_damage = damage.Damage - to.Hp;
                            for (int i = 1; i <= over_damage; i++)
                            {
                                double x = HasSkill("buqu", to) ? 1 / Math.Pow(i, 2) : (double)8 / Math.Pow(i, 2);
                                value += x;
                            }
                        }
                        else
                            deadly = true;
                }

                //刚直应该这里就停止计算
                if (HasSkill("gangzhi", to))
                {
                    if (IsFriend(to)) value = -value;
                    if (priority_enemies.Contains(to) && value > 0)
                        value *= 1.5;
                    if (to.GetRoleEnum() == Player.PlayerRole.Lord) value *= 1.2;
                    result_score.DoDamage = false;
                    result_score.Score = value;
                    return result_score;
                }

                if (!to.Removed && CanResist(to, damage.Damage)) result_score.Score = 3;
                if (IsFriend(to))
                {
                    value = -value;
                    if (deadly && damage.From != null && damage.From.Alive)
                    {
                        if (RoomLogic.IsFriendWith(room, damage.From, damage.To) && !damage.From.IsNude())
                            value -= 3;
                        else if (!RoomLogic.WillBeFriendWith(room, damage.From, damage.To))
                            value -= 2;
                    }
                }
                else
                {
                    if (deadly && damage.From != null && damage.From.Alive)
                    {
                        if (RoomLogic.IsFriendWith(room, damage.From, damage.To) && !damage.From.IsNude())
                            value += 2.5;
                        else if (IsFriend(damage.From) && !RoomLogic.WillBeFriendWith(room, damage.From, damage.To))
                            value += 4;
                    }
                }

                foreach (SkillEvent e in skill_events.Values)
                {
                    if (e.Name != damage.Reason)
                        value += e.GetDamageScore(this, damage).Score;
                }
                if (priority_enemies.Contains(to) && value > 0)
                    value *= 1.5;

                //ai debug log
                /*
                if ((RoomLogic.IsFriendWith(room, self, to) || IsFriend(to)) && value > 0)
                {
                    string damage_from = damage.From != null ? string.Format("{0},{1} has skills {2}", damage.From.General1Showed ? damage.From.ActualGeneral1 : "hidden head",
                        damage.From.General2Showed ? damage.From.ActualGeneral2 : "hidden deputy", string.Join(",", GetKnownSkills(damage.From))) : "no damage from";
                    string damage_str = string.Format("nature {0} count {1} reason {2}", damage.Nature == DamageStruct.DamageNature.Normal ? "normal" : damage.Nature == DamageStruct.DamageNature.Fire ?
                        "fire" : "thunder", damage.Damage, damage.Card != null ? damage.Card.Name : damage.Reason);
                    string damage_to = string.Format("{0},{1} has skills {2}", damage.To.General1Showed ? damage.To.ActualGeneral1 : "hidden head",
                        damage.To.General2Showed ? damage.To.ActualGeneral2 : "hidden deputy", string.Join(",", GetKnownSkills(damage.To)));
                    string self_str = string.Format("{0},{1}", self.ActualGeneral1, self.ActualGeneral2);

                    File.AppendAllText("ai_damage_log.txt", string.Format("{0} judge damage {1} against {2} {6} and value is {3} and ai judge target is my {4} and I'm {5}\r\n",
                        damage_from, damage_str, damage_to, value, IsFriend(self, to) ? "friend" : "enemy", self_str, to.Chained ? "chained" : string.Empty));
                }
                */

                result_score.Score = value;
            }
            scores.Add(result_score);

            if (damage.Card != null && from != null && !damage.Transfer)
            {
                if (damage.Card.Name.Contains(Slash.ClassName) && from.HasWeapon(IceSword.ClassName) && !to.IsNude())
                {
                    ScoreStruct score = FindCards2Discard(from, to, string.Empty, "he", HandlingMethod.MethodDiscard, 2, true);
                    scores.Add(score);
                }
            }

            CompareByScore(ref scores);
            return scores[0];
        }

        public override bool IsGeneralExpose()
        {
            return true;
        }


        public void CompareByLevel(ref List<Player> players)
        {
            List<Player> alives = room.GetAllPlayers();
            players.Sort((x, y) =>
            {
                if (PlayersLevel[x] > PlayersLevel[y])
                    return -1;
                else
                    return alives.IndexOf(x) < alives.IndexOf(y) ? -1 : 1;
            });
        }

        public void UpdatePlayers()
        {
            friends.Clear();
            enemies.Clear();
            priority_enemies.Clear();
            
            UpdatePlayerLevel();
            
            foreach (Player p1 in room.GetAlivePlayers())
            {
                friends[p1] = new List<Player>();
                enemies[p1] = new List<Player>();
                foreach (Player p2 in room.GetAlivePlayers())
                {
                    if (p1.Camp == p2.Camp)
                        friends[p1].Add(p2);
                    else
                        enemies[p1].Add(p2);
                }
            }
            foreach (Player p in enemies[self])
                if (PlayersLevel[p] > 3)
                    priority_enemies.Add(p);

            CompareByLevel(ref priority_enemies);
            List<Player> players = enemies[self];
            CompareByLevel(ref players);
            enemies[self] = players;
        }
 
        //更新敌我识别
        public void UpdatePlayerLevel()
        {
            foreach (Player p in room.Players)
            {
                if (p.Camp == self.Camp)
                    PlayersLevel[p] = -1;
                else
                {
                    if (p.GetRoleEnum() == Player.PlayerRole.Lord)
                        PlayersLevel[p] = 4;
                    else if (p.Alive && p.GetMark("@gd_liang") > 0)
                        PlayersLevel[p] = 5;
                    else
                        PlayersLevel[p] = 3;
                }
            }
        }

        //更新玩家关系
        public override void UpdatePlayerRelation(Player from, Player to, bool friendly)
        {
        }
        //更新玩家身份的倾向
        public override void UpdatePlayerIntention(Player player, string kingdom, int intention)
        {
        }

        //服务器操作响应
        public override void Activate(ref CardUseStruct card_use)
        {
            UpdatePlayers();
            to_use = GetTurnUse();

            to_use.Sort((x, y) => { return GetDynamicUsePriority(x) > GetDynamicUsePriority(y) ? -1 : 1; });

            foreach (CardUseStruct use in to_use)
            {
                WrappedCard card = use.Card;
                if (!RoomLogic.IsCardLimited(room, self, card, HandlingMethod.MethodUse)
                    || (card.CanRecast && !RoomLogic.IsCardLimited(room, self, card, HandlingMethod.MethodRecast)))
                {
                    string class_name = card.Name.Contains(Slash.ClassName) ? Slash.ClassName : card.Name;
                    UseCard _use = Engine.GetCardUsage(class_name);
                    if (_use != null)
                    {
                        _use.Use(this, self, ref card_use, card);
                        if (card_use.Card != null)
                        {
                            to_use.Clear();
                            return;
                        }
                    }
                }
            }

            to_use.Clear();
        }

        public override WrappedCard AskForCard(string reason, string pattern, string prompt, object data)
        {
            UseCard card = Engine.GetCardUsage(reason);
            if (card != null)
                return card.OnResponding(this, self, pattern, prompt, data).Card;

            SkillEvent skill = Engine.GetSkillEvent(reason);
            if (skill != null)
                return skill.OnResponding(this, self, pattern, prompt, data).Card;

            return base.AskForCard(reason, pattern, prompt, data);
        }
        public override WrappedCard AskForCardShow(Player requestor, string reason, object data)
        {
            UseCard card = Engine.GetCardUsage(reason);
            WrappedCard result = null;
            if (card != null)
            {
                result = card.OnCardShow(this, self, requestor, data);
                if (result != null)
                    return result;
            }

            SkillEvent skill = Engine.GetSkillEvent(reason);
            if (skill != null)
            {
                result = skill.OnCardShow(this, self, requestor, data);
                if (result != null)
                    return result;
            }

            return base.AskForCardShow(requestor, reason, data);
        }

        public override WrappedCard AskForSinglePeach(Player dying, DyingStruct dying_struct)
        {
            FunctionCard f_peach = Peach.Instance;
            FunctionCard f_ana = Analeptic.Instance;
            if (self != dying & IsFriend(dying) && (CanSave(dying, 1 - dying.Hp) || dying.GetRoleEnum() == Player.PlayerRole.Lord))
            {
                if (dying.GetRoleEnum() != Player.PlayerRole.Lord && dying.IsNude() && !MaySave(dying) && !HasSkill(MasochismSkill))
                    return null;

                List<WrappedCard> peaches = GetCards(Peach.ClassName, self);
                foreach (WrappedCard card in peaches)
                {
                    if (f_peach.IsAvailable(room, self, card) && Engine.IsProhibited(room, self, dying, card) == null)
                        return card;
                }
            }
            else if (self == dying)
            {
                List<WrappedCard> peaches = new List<WrappedCard>();
                foreach (WrappedCard card in GetCards(Peach.ClassName, self))
                    if (f_peach.IsAvailable(room, self, card) && Engine.IsProhibited(room, self, dying, card) == null)
                        peaches.Add(card);
                foreach (WrappedCard card in GetCards(Analeptic.ClassName, self))
                    if (f_ana.IsAvailable(room, self, card) && Engine.IsProhibited(room, self, dying, card) == null)
                        peaches.Add(card);

                double best = -1000;
                WrappedCard result = null;
                foreach (WrappedCard card in peaches)
                {
                    double value = GetUseValue(card, self);
                    if (card.Name == Peach.ClassName) value -= 2;
                    if (value > best)
                    {
                        best = value;
                        result = card;
                    }
                }

                return result;
            }

            return null;
        }

        public override string AskForChoice(string skill_name, string choice, object data)
        {
            bool trigger_skill = false;
            if (skill_name == "GameRule:TurnStart")
            {
                string[] choices = choice.Split('+');
                List<string> new_choices = new List<string>();
                foreach (string cho in choices)
                {
                    if (!cho.Contains("GameRule_AskForGeneralShow") && cho != "cancel")
                    {
                        trigger_skill = true;
                        new_choices.Add(cho);
                    }
                }
                if (trigger_skill)
                    choice = string.Join("+", new_choices);
            }

            if (skill_name == "GameRule:TriggerOrder" || trigger_skill)
            {
                if (choice.Contains("qianxi")) return "qianxi";
                if (choice.Contains("duanbing")) return "duanbing";
                if (choice.Contains("jieming")) return "jieming";
                if (choice.Contains("fankui") && choice.Contains("ganglie")) return "fankui";
                if (choice.Contains("fangzhu") && data is DamageStruct damage)
                {
                    Player from = damage.From;
                    if (choice.Contains("wangxi"))
                    {
                        if (from != null && from.IsNude())
                            return "wangxi";
                    }

                    if (choice.Contains("fankui"))
                    {
                        if (from != null && from == Self && HasArmorEffect(Self, SilverLion.ClassName))
                        {
                            bool friend = false;
                            foreach (Player p in FriendNoSelf)
                            {
                                if (!p.FaceUp)
                                {
                                    friend = true;
                                    break;
                                }
                            }
                            if (!friend)
                                return "fankui";
                        }
                    }

                    return "fangzhu";
                }

                if (choice.Contains("wangxi") && choice.Contains("ganglie")) return "ganglie";
                if (choice.Contains("jiangxiong")) return "jianxiong";

                if (choice.Contains("qianxi") && choice.Contains("guanxing"))
                {
                    if (self.JudgingArea.Count > 0 && room.AliveCount() <= 4)
                    {
                        return "qianxi";
                    }
                    return "guanxing";
                }

                if (choice.Contains("tiandu") && data is JudgeStruct judge)
                {
                    int id = judge.Card.Id;
                    if (IsCard(id, Peach.ClassName, self) || IsCard(id, Analeptic.ClassName, Self))
                        return "tiandu";
                }
                if (choice.Contains("yiji")) return "yiji";
                if (choice.Contains("hunshang")) return "hunshang";
                if (choice.Contains("yinghun_sunjian")) return "yinghun_sunjian";
                if (choice.Contains("yinghun_sunce")) return "yinghun_sunce";
                if (choice.Contains("yingzi_zhouyu")) return "yingzi_zhouyu";
                if (choice.Contains("yingzi_sunce")) return "yingzi_sunce";
                if (choice.Contains("yingziextra")) return "yingziextra";
                if (choice.Contains("jieyue")) return "jieyue";
                if (choice.Contains("tianxiang")) return "tianxiang";
                string[] skillnames = choice.Split('+');
                return skillnames[0];
            }

            if (skill_name == HegNullification.ClassName)
            {
                if (!string.IsNullOrEmpty(Choice[HegNullification.ClassName]))
                    return Choice[HegNullification.ClassName];

                return "single";
            }

            UseCard card = Engine.GetCardUsage(skill_name);
            if (card != null)
                return card.OnChoice(this, self, choice, data);

            SkillEvent skill = Engine.GetSkillEvent(skill_name);
            if (skill != null)
                return skill.OnChoice(this, self, choice, data);

            return base.AskForChoice(skill_name, choice, data);
        }

        public override bool AskForSkillInvoke(string skill_name, object data)
        {
            UseCard card = Engine.GetCardUsage(skill_name);
            if (card != null)
                return card.OnSkillInvoke(this, self, data);

            SkillEvent skill = Engine.GetSkillEvent(skill_name);
            if (skill != null)
                return skill.OnSkillInvoke(this, self, data);

            return base.AskForSkillInvoke(skill_name, data);
        }

        public override List<int> AskForDiscard(List<int> ids, string reason, int discard_num, int min_num, bool optional)
        {
            List<int> result;
            SkillEvent skill = Engine.GetSkillEvent(reason);
            if (skill != null)
            {
                result = skill.OnDiscard(this, self, ids, min_num, discard_num, optional);
                if (result != null)
                    return result;
            }

            result = new List<int>();
            if (optional) return result;

           SortByKeepValue(ref ids, false);
            for (int i = 0; i < min_num; i++)
                result.Add(ids[i]);

            if (result.Count < discard_num)
            {
                for (int i = result.Count - 1; i < Math.Min(result.Count, ids.Count); i++)
                {
                    if (ids[i] < 0)
                        result.Add(ids[i]);
                    else
                        break;
                }
            }

            return result;

        }

        public override int AskForCardChosen(Player who, string flags, string reason, HandlingMethod method, List<int> disabled_ids)
        {
            SkillEvent e = Engine.GetSkillEvent(reason);
            if (e != null)
            {
                List<int> result = e.OnCardsChosen(this, self, who, flags, 1, 1, disabled_ids);
                if (result != null && result.Count == 1)
                    return result[0];
            }

            UseCard c = Engine.GetCardUsage(reason);
            if (c != null)
            {
                List<int> result = c.OnCardsChosen(this, self, who, flags, 1, 1, disabled_ids);
                if (result != null && result.Count == 1)
                    return result[0];
            }

            ScoreStruct score = FindCards2Discard(self, who, string.Empty, flags, method, 1, false, disabled_ids);
            if (score.Ids != null && score.Ids.Count == 1)
                return score.Ids[0];

            return -1;
        }

        private readonly Dictionary<string, string> prompt_keys = new Dictionary<string, string> {
            { "collateral-slash", Collateral.ClassName },
            { "@tiaoxin-slash", "tiaoxin" },
            { "@luanwu-slash", "luanwu" },
            { "@yuanlue", "yuanlue" },
        };

        public override CardUseStruct AskForUseCard(string pattern, string prompt, FunctionCard.HandlingMethod method)
        {
            const string rx_pattern = @"@?@?([_A-Za-z]+)(\d+)?!?";
            if (!string.IsNullOrEmpty(pattern) && pattern.StartsWith("@"))
            {
                Match result = Regex.Match(pattern, rx_pattern);
                if (result.Length > 0)
                {
                    string skill_name = result.Groups[1].ToString();
                    UseCard card = Engine.GetCardUsage(skill_name);
                    if (card != null)
                    {
                        CardUseStruct use = card.OnResponding(this, self, pattern, prompt, method);
                        return use;
                    }

                    SkillEvent skill = Engine.GetSkillEvent(skill_name);
                    if (skill != null)
                    {
                        CardUseStruct use = skill.OnResponding(this, self, pattern, prompt, method);
                        return use;
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(room.GetRoomState().GetCurrentResponseSkill()))
                {
                    string skill_name = room.GetRoomState().GetCurrentResponseSkill();
                    UseCard card = Engine.GetCardUsage(skill_name);
                    if (card != null)
                    {
                        CardUseStruct use = card.OnResponding(this, self, pattern, prompt, method);
                        return use;
                    }

                    SkillEvent skill = Engine.GetSkillEvent(skill_name);
                    if (skill != null)
                    {
                        CardUseStruct use = skill.OnResponding(this, self, pattern, prompt, method);
                        return use;
                    }
                }

                foreach (string key in prompt_keys.Keys)
                {
                    if (prompt.StartsWith(key))
                    {
                        string skill_name = prompt_keys[key];
                        UseCard card = Engine.GetCardUsage(skill_name);
                        if (card != null)
                        {
                            CardUseStruct use = card.OnResponding(this, self, pattern, prompt, method);
                            return use;
                        }

                        SkillEvent skill = Engine.GetSkillEvent(skill_name);
                        if (skill != null)
                        {
                            CardUseStruct use = skill.OnResponding(this, self, pattern, prompt, method);
                            return use;
                        }
                    }
                }
            }

            return base.AskForUseCard(pattern, prompt, method);
        }

        public override Player AskForYiji(List<int> cards, string reason, ref int card_id)
        {
            SkillEvent e = Engine.GetSkillEvent(reason);
            if (e != null)
            {
                Player result = e.OnYiji(this, self, cards, ref card_id);
                if (result != null)
                    return result;
            }

            return null;
        }

        public override List<int> AskForExchange(string reason, string pattern, int max_num, int min_num, string expand_pile)
        {
            SkillEvent e = Engine.GetSkillEvent(reason);
            if (e != null)
            {
                List<int> result = e.OnExchange(this, self, pattern, min_num, max_num, expand_pile);
                if (result != null)
                    return result;
            }

            return base.AskForExchange(reason, pattern, max_num, min_num, expand_pile);
        }

        public override List<Player> AskForPlayersChosen(List<Player> targets, string reason, int max_num, int min_num)
        {
            SkillEvent e = Engine.GetSkillEvent(reason);
            if (e != null)
            {
                List<Player> result = e.OnPlayerChosen(this, self, new List<Player>(targets), min_num, max_num);
                if (result != null)
                    return result;
            }
            UseCard u = Engine.GetCardUsage(reason);
            if (u != null)
            {
                List<Player> result = u.OnPlayerChosen(this, self, new List<Player>(targets), min_num, max_num);
                if (result != null)
                    return result;
            }

            return base.AskForPlayersChosen(targets, reason, max_num, min_num);
        }
        public override WrappedCard AskForNullification(CardEffectStruct effect, bool positive, CardEffectStruct real)
        {
            Player from = effect.From, to = effect.To;
            WrappedCard trick = effect.Card;
            Choice[HegNullification.ClassName] = null;
            if (!to.Alive) return null;

            List<WrappedCard> nullcards = GetCards(Nullification.ClassName, self);
            if (nullcards.Count == 0)
                return null;

            int null_num = nullcards.Count;
            SortByUseValue(ref nullcards);
            WrappedCard null_card = null;
            foreach (WrappedCard c in nullcards)
                if (!RoomLogic.IsCardLimited(room, self, c, HandlingMethod.MethodUse))
                    null_card = c;

            if (null_card == null) return null;

            if (null_num > 1)
            {
                foreach (WrappedCard card in nullcards)
                {
                    if (card.Name != HegNullification.ClassName && !RoomLogic.IsCardLimited(room, self, card, HandlingMethod.MethodUse))
                    {
                        null_card = card;
                        break;
                    }
                }
            }
            if (RoomLogic.IsCardLimited(room, self, null_card, FunctionCard.HandlingMethod.MethodUse)) return null;

            if (null_num == 1 && HasSkill("kanpo") && self.Phase == Player.PlayerPhase.NotActive && self.IsLastHandCard(null_card))
            {
                foreach (Player p in GetFriends(self))
                {
                    if (HasSkill("shouchen", p))
                    {
                        null_num = 2;
                        break;
                    }
                }
            }
            bool keep = false;
            if (null_num == 1)
            {
                bool only = true;
                foreach (Player p in FriendNoSelf)
                {
                    if (GetKnownCardsNums(Nullification.ClassName, "he", p, self) > 0)
                    {
                        only = false;
                        break;
                    }
                }

                if (only)
                {
                    foreach (Player p in GetFriends(self))
                    {
                        if (RoomLogic.PlayerContainsTrick(room, p, Indulgence.ClassName) && !HasSkill("guanxing|yizhi|shensu|qiaobian") && p.HandcardNum >= p.Hp
                            && (trick.Name != Indulgence.ClassName) || p.Name != to.Name)
                        {
                            keep = true;
                            break;
                        }
                    }
                }
            }
            UseCard use = Engine.GetCardUsage(trick.Name);
            if (use != null)
            {
                UseCard.NulliResult result = use.OnNullification(this, effect, positive, keep);
                if (result.Null)
                {
                    if (result.Heg)
                    {
                        foreach (WrappedCard card in nullcards)
                        {
                            if (card.Name == HegNullification.ClassName && !RoomLogic.IsCardLimited(room, self, card, HandlingMethod.MethodUse))
                            {
                                Choice[HegNullification.ClassName] = "all";
                                null_card = card;
                                break;
                            }
                        }
                    }
                    return null_card;
                }
            }
            return null;
        }

        public override AskForMoveCardsStruct AskForMoveCards(List<int> upcards, List<int> downcards, string reason, int min_num, int max_num)
        {
            SkillEvent e = Engine.GetSkillEvent(reason);
            if (e != null)
                return e.OnMoveCards(this, self, new List<int>(upcards), new List<int>(downcards), min_num, max_num);

            return base.AskForMoveCards(upcards, downcards, reason, min_num, max_num);
        }
    }
}
