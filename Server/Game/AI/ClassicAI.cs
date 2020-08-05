using CommonClass;
using CommonClass.Game;
using SanguoshaServer.Game;
using SanguoshaServer.Package;
using SanguoshaServer.Scenario;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using static CommonClass.Game.Player;
using static SanguoshaServer.Package.FunctionCard;

namespace SanguoshaServer.AI
{
    public class StupidAI : TrustedAI
    {
        private readonly List<string> kingdoms = new List<string> { "wei", "shu", "wu", "qun" };
        private Dictionary<PlayerRole, int> roles = new Dictionary<PlayerRole, int>();
        public StupidAI(Room room, Player player) : base(room, player)
        {
        }

        private readonly List<string> intention_roles = new List<string> { "loyalist", "rebel", "renegade" };

        //根据将面设置初始身份倾向
        private void InitRoleTendency()
        {
            Player lord = null;
            foreach (Player p in room.GetAlivePlayers())
            {
                if (p.GetRoleEnum() == PlayerRole.Lord)
                {
                    id_tendency[p] = "lord";
                    lord = p;
                }
                else if (p == self)
                    id_tendency[p] = p.Role;
                else
                {
                    id_tendency[p] = "unknown";
                    Dictionary<string, int> role_tendency = new Dictionary<string, int>
                    {
                        { "loyalist", Engine.GetRoleTendency(p.General1, PlayerRole.Loyalist) },
                        { "rebel", Engine.GetRoleTendency(p.General1, PlayerRole.Rebel) },
                        { "renegade", Engine.GetRoleTendency(p.General1, PlayerRole.Renegade) }
                    };
                    player_intention[p] = role_tendency;
                    PlayersLevel[p] = 0;
                }
            }

            id_public[self] = self.GetRoleEnum() == PlayerRole.Lord ? "lord" : "unknown";
            if (self.GetRoleEnum() != PlayerRole.Lord)
            {
                foreach (string role in intention_roles)
                    player_intention_public[self][role] = 0;
            }
            General lord_general = Engine.GetGeneral(lord.General1, room.Setting.GameMode);
            //对将面国籍、技能设置身份倾向
            foreach (Player p in room.GetOtherPlayers(lord))
            {
                if (self == p) continue;

                if (lord_general.IsLord())
                {
                    if (p.Kingdom == lord.Kingdom)
                    {
                        player_intention[p]["loyalist"] += 5;

                        if (HasSkill("jijiang", lord) && HasSkill("yajiao", p))      //主公刘备，赵云偏忠
                            player_intention[p]["loyalist"] += 5;
                    }

                    if (lord.General1 == "liubei" && p.General1 == "xizhicai") player_intention[p]["loyalist"] += 10;

                    General general = Engine.GetGeneral(p.General1, room.Setting.GameMode);
                    if (general.Kingdom == "god" && p.Kingdom != lord.Kingdom)
                        player_intention[p]["rebel"] += 20;                         //神将国籍与主公不同偏反
                }
                if (HasSkill(MasochismSkill, lord) && p.General1 == "zhangchunhua")             //主公卖血用春哥的明反
                    player_intention[p]["rebel"] += 80;
                if (lord.General1 == "xizhicai" && (p.General1 == "jvshou" || p.General1 == "caorui")) player_intention[p]["loyalist"] += 30;
                if (lord.IsFemale())
                {
                    if (p.General1 == "diaochan" || p.General1 == "diaochan_sp") player_intention[p]["loyalist"] += 20;
                    if (p.General1 == "xiahoushi") player_intention[p]["rebel"] += 20;
                    if (p.General1 == "xushi") player_intention[p]["rebel"] -= 10;
                }
                if (HasSkill(MasochismSkill, lord) && HasSkill("tieqi_jx", p) && RoomLogic.DistanceTo(room, p, lord) == 1 && room.Players.Count > 5)
                    player_intention[p]["rebel"] += 20;                                             //近位克制卖血的马超偏反

                if (lord.General1 == "wutugu" && (p.General1 == "guanyinping" || p.General1 == "wolong"
                    || p.General1 == "zhouyu_god" || p.General1 == "liubei_god"))                   //关银屏卧龙神周瑜神刘备对兀突骨偏反
                    player_intention[p]["rebel"] += 20;

                if (lord.General1 == "maliang" && (p.General1 == "lusu" || p.General1 == "diaochan_sp" || p.General1 == "wangji"))  //专克马良的偏反
                    player_intention[p]["rebel"] += 25;
                if (lord.General1 == "maliang" && (p.General1 == "caorui" || p.General1 == "liubei" || p.General1 == "zhangchangpu")) //不配合马良的偏反
                    player_intention[p]["rebel"] += 15;
                if ((lord.General1 == "liubei" || lord.General1 == "lusu" || lord.General1 == "caorui") && p.General1 == "maliang") player_intention[p]["rebel"] += 20;

                if (p.General1 == "quyi" && RoomLogic.DistanceTo(room, p, lord) == 1)
                    player_intention[p]["rebel"] += 10;                                             //近位麹义偏反
                if (lord.General1 == "caocao_god" && p.General1 == "haozhao")
                    player_intention[p]["rebel"] += 30;                                             //神曹操局郝昭偏反
            }
        }

        public override void Event(TriggerEvent triggerEvent, Player player, object data)
        {
            if (!self.Alive) return;

            base.Event(triggerEvent, player, data);

            if (player != null && player == self)
            {
                if (triggerEvent == TriggerEvent.GameStart)
                {
                    //根据场面选将决定初始身份倾向
                    InitRoleTendency();
                    UpdatePlayers();
                }
            }
            if (triggerEvent == TriggerEvent.EventPhaseStart || triggerEvent == TriggerEvent.RemoveStateChanged
                || triggerEvent == TriggerEvent.BuryVictim)
            {
                UpdatePlayers();
            }

            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move)
            {
                bool open = false;
                bool pile_open = false;
                Player from = move.From;
                Player to = move.To;

                if ((from != null && (from == self || self.IsSameCamp(from))) || (to != null && (to == self || self.IsSameCamp(to)) && move.To_place != Place.PlaceSpecial))
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

                if (move.To_place == Place.DrawPile)
                {
                    for (int i = 0; i < move.Card_ids.Count; i++)
                    {
                        if (move.Open[i])
                            room.GetCard(move.Card_ids[i]).SetFlags("visible");
                        else if (from != null)
                            room.GetCard(move.Card_ids[i]).SetFlags("visible2" + from.Name);
                    }
                }

                if (to != null && move.To_place == Place.PlaceHand)
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

                if (to != null && move.To_place == Place.PlaceSpecial && move.To_pile_name == "wooden_ox")
                {
                    foreach (int id in move.Card_ids)
                    {
                        if (open)
                            wooden_cards[to].Add(id);
                    }
                }

                if (from != null && move.From_places.Contains(Place.PlaceHand))
                {
                    foreach (int id in move.Card_ids)
                    {
                        if (room.GetCard(id).HasFlag("visible") || pile_open || move.To_place == Place.PlaceEquip
                                || move.To_place == Place.PlaceDelayedTrick || move.To_place == Place.DiscardPile
                                || move.To_place == Place.PlaceTable)
                        {
                            //蛊惑的真假判断
                            if (move.To_place == Place.PlaceTable && move.Reason.SkillName == "guhuo" && move.Reason.Reason == MoveReason.S_REASON_ANNOUNCE && !open
                                && private_handcards[from].Count > move.From.HandcardNum)
                            {
                                WrappedCard guhuo = move.Reason.Card;
                                bool doubt = true;
                                foreach (int hand in private_handcards[from])
                                {
                                    if (room.GetCard(hand).Name == guhuo.Name)
                                    {
                                        doubt = false;
                                        break;
                                    }
                                }
                                if (doubt) self.SetFlags("guhuo_doubt");
                            }

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

                if (from != null && move.From_places.Contains(Place.PlaceSpecial) && move.From_pile_names.Contains("wooden_ox"))
                {
                    //蛊惑的真假判断
                    if (move.To_place == Place.PlaceTable && move.Reason.SkillName == "guhuo" && move.Reason.Reason == MoveReason.S_REASON_ANNOUNCE && !open
                        && wooden_cards[move.From].Count > move.From.GetPile("wooden_ox").Count)
                    {
                        WrappedCard guhuo = move.Reason.Card;
                        bool doubt = true;
                        foreach (int hand in wooden_cards[move.From])
                        {
                            if (room.GetCard(hand).Name == guhuo.Name)
                            {
                                doubt = false;
                                break;
                            }
                        }
                        if (doubt) self.SetFlags("guhuo_doubt");
                    }
                    else if (open || move.To_place == Place.PlaceEquip || move.To_place == Place.PlaceDelayedTrick || move.To_place == Place.DiscardPile
                                || move.To_place == Place.PlaceTable)
                    {
                        foreach (int id in move.Card_ids)
                        {
                            int index = move.Card_ids.IndexOf(id);
                            if (move.From_pile_names[index] == "wooden_ox" && move.From_places[index] == Place.PlaceSpecial)
                                wooden_cards[move.From].RemoveAll(t => t == id);
                        }
                    }
                    else
                        wooden_cards[move.From].Clear();
                }

                foreach (int id in move.Card_ids)
                {
                    int index = move.Card_ids.IndexOf(id);
                    WrappedCard card = room.GetCard(id);
                    if (move.From_places[index] == Place.DrawPile)
                    {
                        if (move.To != null && move.To_place == Place.PlaceHand && card.HasFlag("visible2" + self.Name))
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

                List<string> choices = new List<string>(str.Split(':'));
                if (str.StartsWith("viewCards"))
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
                    List<int> drawpile = room.DrawPile;
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
                else if (choices[0] == "Nullification" && self != player)
                {
                    string trick = choices[1];
                    Player to = room.FindPlayer(choices[3], true);
                    Player from = null;
                    if (!string.IsNullOrEmpty(choices[2]))
                        from = room.FindPlayer(choices[2], true);
                    bool positive = bool.Parse(choices[4]);

                    if (player != to && GetPlayerTendency(to) != "unknown")
                    {
                        if (trick == Indulgence.ClassName || trick == SupplyShortage.ClassName || trick == Lightning.ClassName)
                            UpdatePlayerRelation(player, to, positive);
                        else if (trick == Duel.ClassName || trick == SavageAssault.ClassName || trick == ArcheryAttack.ClassName)
                        {
                            if (to.Hp == 1)
                                UpdatePlayerRelation(player, to, positive);
                        }
                        else if (trick == Snatch.ClassName || trick == Dismantlement.ClassName)
                        {
                            if (IsFriend(from, to))
                            {
                                if (RoomLogic.PlayerContainsTrick(room, to, Indulgence.ClassName) || RoomLogic.PlayerContainsTrick(room, to, SupplyShortage.ClassName))
                                    UpdatePlayerRelation(player, to, !positive);
                                else
                                {
                                    foreach (int id in to.GetEquips())
                                    {
                                        if (GetKeepValue(id, to, Place.PlaceEquip) < 0)
                                        {
                                            UpdatePlayerRelation(player, to, !positive);
                                            break;
                                        }
                                    }
                                }
                            }
                            else if (IsEnemy(from, to) && !to.IsNude())
                            {
                                UpdatePlayerRelation(player, to, positive);
                            }
                            else
                            {
                                if (to.JudgingArea.Count == 0)
                                    UpdatePlayerRelation(player, to, positive);
                                else if (to.IsNude())
                                    UpdatePlayerRelation(player, to, !positive);
                            }
                        }
                        else if (trick == IronChain.ClassName)
                        {
                            if (!to.Chained)
                                UpdatePlayerRelation(player, to, positive);
                            else
                                UpdatePlayerRelation(player, to, !positive);
                        }
                    }
                }
            }

            FilterEvent(triggerEvent, player, data);
        }

        private void CountPlayers()
        {
            roles.Clear();
            List<Player> players = room.GetAllPlayers(true);
            switch (players.Count)
            {
                case 4:
                    roles.Add(PlayerRole.Loyalist, 1);
                    roles.Add(PlayerRole.Rebel, 1);
                    roles.Add(PlayerRole.Renegade, 1);
                    break;
                case 5:
                    roles.Add(PlayerRole.Loyalist, 1);
                    roles.Add(PlayerRole.Rebel, 2);
                    roles.Add(PlayerRole.Renegade, 1);
                    break;
                case 6:
                    roles.Add(PlayerRole.Loyalist, 1);
                    roles.Add(PlayerRole.Rebel, 3);
                    roles.Add(PlayerRole.Renegade, 1);
                    break;
                case 7:
                    roles.Add(PlayerRole.Loyalist, 2);
                    roles.Add(PlayerRole.Rebel, 3);
                    roles.Add(PlayerRole.Renegade, 1);
                    break;
                case 8:
                    roles.Add(PlayerRole.Loyalist, 2);
                    roles.Add(PlayerRole.Rebel, 4);
                    roles.Add(PlayerRole.Renegade, 1);
                    break;
            }

            foreach (Player p in players)
            {
                if (!p.Alive)
                    roles[p.GetRoleEnum()]--;
            }
        }

        public int GetRolePitts(PlayerRole role)
        {
            return roles.ContainsKey(role) ? roles[role] : 0;
        }

        public override bool IsSituationClear()
        {
            if (self.GetRoleEnum() == PlayerRole.Loyalist || self.GetRoleEnum() == PlayerRole.Lord)
            {
                return friends[self].Count >= roles[PlayerRole.Loyalist] + 1;
            }
            else if (self.GetRoleEnum() == PlayerRole.Rebel)
            {
                return friends[self].Count >= roles[PlayerRole.Rebel];
            }
            else
            {
                return (roles[PlayerRole.Loyalist] == 0 || roles[PlayerRole.Rebel] == 0) || Process.Contains(">>") || Process.Contains("<<");
            }
        }

        public override bool IsGeneralExpose()
        {
            return true;
        }

        private void FilterEvent(TriggerEvent triggerEvent, Player player, object data)
        {
        }

        //评估玩家强度
        public double EvaluatePlayerStrength(Player who)
        {
            double point = Engine.GetGeneralValue(who.General1, "Classic");

            Player lord = null;
            foreach (Player p in room.Players)
            {
                if (p.GetRoleEnum() == PlayerRole.Lord)
                {
                    lord = p;
                    break;
                }
            }

            PlayerRole role = PlayerRole.Unknown;
            if (who != lord)
            {
                string str = GetPlayerTendency(who);
                switch (str)
                {
                    case "rebel":
                        role = PlayerRole.Rebel;
                        break;
                    case "loyalist":
                        role = PlayerRole.Loyalist;
                        break;
                    case "renegade":
                        role = PlayerRole.Renegade;
                        break;
                }
            }
            else
                role = PlayerRole.Lord;

            point += GeneralSelector.AdjustRolePoints(room, lord, who.General1, role, who);
            List<string> skills = GetKnownSkills(who, true, self, true);
            foreach (string skill in skills)
            {
                SkillEvent e = Engine.GetSkillEvent(skill);
                point += Engine.GetSkillValue(skill) + (e != null ? e.GetSkillAdjustValue(this, who) : 0);
                Skill _skill = Engine.GetSkill(skill);
                if (_skill.SkillFrequency == Skill.Frequency.Limited && who.GetMark(_skill.LimitMark) == 0)
                {
                    point -= 1.5;
                    if (skill == "xiongluan") point -= 2;
                }
                if (_skill.SkillFrequency == Skill.Frequency.Wake && who.GetMark(skill) > 0)
                    point += 1.5;
            }

            point += (who.MaxHp - 3) * 1;
            //todo: match player equip & handcard
            point += GetDefensePoint(who);

            // emit room->room_message(QString("%1 judge %2:%3,%4 's stength point is %5")
            //     .arg(self->objectName()).arg(who->objectName()).arg(who->getGeneralName()).arg(who->getGeneral2() ? who->getGeneral2Name() : QString()).arg(point));
            return point;
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
            CountPlayers();
            //分析身份
            List<Player> loyalists = new List<Player>(), rebels = new List<Player>(), rends = new List<Player>();
            List<Player> loyalists_n = new List<Player>(), rebels_n = new List<Player>();
            Player lord = null;

            if (self.GetRoleEnum() == PlayerRole.Renegade && room.Round > 1)        //内奸第二回合起自带身份透视
            {
                foreach (Player p in room.GetAlivePlayers())
                {
                    switch (p.GetRoleEnum())
                    {
                        case PlayerRole.Lord:
                            lord = p;
                            break;
                        case PlayerRole.Loyalist:
                            loyalists.Add(p);
                            break;
                        case PlayerRole.Rebel:
                            rebels.Add(p);
                            break;
                        case PlayerRole.Renegade:
                            rends.Add(p);
                            break;
                    }
                }
            }
            else
            {
                switch (self.GetRoleEnum())
                {
                    case PlayerRole.Loyalist:
                        loyalists.Add(self);
                        break;
                    case PlayerRole.Rebel:
                        rebels.Add(self);
                        break;
                    case PlayerRole.Renegade:
                        rends.Add(self);
                        break;
                }

                if (self.GetRoleEnum() == PlayerRole.Lord) lord = self;
                //首先选出已确定身份的角色
                foreach (Player p in room.GetOtherPlayers(self))
                {
                    if (p.GetRoleEnum() == PlayerRole.Lord)
                        lord = p;

                    if (id_tendency[p] != "unknown")
                    {
                        switch (id_tendency[p])
                        {
                            case "loyalist":
                                loyalists.Add(p);
                                break;
                            case "rebel":
                                rebels.Add(p);
                                break;
                            case "renegade":
                                rends.Add(p);
                                break;
                        }
                    }
                }
                //匹配剩余身份数量，如不符合，则重新识别
                if (loyalists.Count > roles[PlayerRole.Loyalist])
                {
                    List<Player> re = new List<Player>(loyalists);
                    foreach (Player p in re)
                    {
                        if (p == self) continue;
                        loyalists.Remove(p);
                        id_tendency[p] = "unknown";
                    }
                }
                if (rebels.Count > roles[PlayerRole.Rebel])
                {
                    List<Player> re = new List<Player>(rebels);
                    foreach (Player p in re)
                    {
                        if (p == self || p.GetRoleEnum() == PlayerRole.Rebel) continue;         //小作弊，已经确定身份的反贼不会再改变身份识别
                        rebels.Remove(p);
                        id_tendency[p] = "unknown";
                    }
                }
                if (rends.Count > roles[PlayerRole.Renegade])
                {
                    List<Player> re = new List<Player>(rends);
                    foreach (Player p in re)
                    {
                        if (p == self) continue;
                        rends.Remove(p);
                        id_tendency[p] = "unknown";
                    }
                }

                //填补剩余坑位
                int loyal_c = roles[PlayerRole.Loyalist] - loyalists.Count;
                int rebel_c = roles[PlayerRole.Rebel] - rebels.Count;
                int rends_c = roles[PlayerRole.Renegade] - rends.Count;

                int count = 0;
                if (loyal_c == 0) count++;
                if (rebel_c == 0) count++;
                if (rends_c == 0) count++;
                if (count == 2)                         //当其中2个身份已识别完毕，则自动将剩余玩家的身份归为未识别完成的那一个
                {
                    if (loyal_c > 0)
                    {
                        foreach (Player p in room.GetOtherPlayers(self))
                        {
                            if (id_tendency[p] == "unknown")
                            {
                                id_tendency[p] = "loyalist";
                                loyalists.Add(p);
                            }
                        }
                    }
                    else if (rebel_c > 0)
                    {
                        foreach (Player p in room.GetOtherPlayers(self))
                        {
                            if (id_tendency[p] == "unknown")
                            {
                                id_tendency[p] = "rebel";
                                rebels.Add(p);
                            }
                        }
                    }
                    else
                    {
                        foreach (Player p in room.GetOtherPlayers(self))
                        {
                            if (id_tendency[p] == "unknown")
                            {
                                id_tendency[p] = "renegade";
                                rends.Add(p);
                            }
                        }
                    }

                    count = 3;
                }

                foreach (Player p in room.GetOtherPlayers(self))
                {
                    if (p.GetRoleEnum() == PlayerRole.Lord || id_tendency[p] != "unknown") continue;

                    List<string> roles = GetPossibleId(p);
                    if (roles.Count > 0)
                    {
                        if (player_intention[p][roles[0]] >= 80)
                        {
                            switch (roles[0])
                            {
                                case "loyalist":
                                    loyalists_n.Add(p);
                                    break;
                                case "rebel":
                                    rebels_n.Add(p);
                                    break;
                            }
                        }
                    }
                }
            }

            //为主忠方与反贼方强度打分
            Dictionary<PlayerRole, double> roles_points = new Dictionary<PlayerRole, double>();
            Dictionary<PlayerRole, List<string>> side_skills = new Dictionary<PlayerRole, List<string>>();
            roles_points.Add(PlayerRole.Lord, EvaluatePlayerStrength(lord));
            side_skills.Add(PlayerRole.Lord, GetKnownSkills(lord));
            foreach (Player p in loyalists)
            {
                roles_points[PlayerRole.Lord] += EvaluatePlayerStrength(p);
                side_skills[PlayerRole.Lord].AddRange(GetKnownSkills(p));
            }
            roles_points.Add(PlayerRole.Rebel, 0);
            side_skills.Add(PlayerRole.Rebel, new List<string>());
            foreach (Player p in rebels)
            {
                roles_points[PlayerRole.Rebel] += EvaluatePlayerStrength(p);
                side_skills[PlayerRole.Rebel].AddRange(GetKnownSkills(p));
            }
            //当敌我均未完成识别时，势力方人数矫正
            if (loyalists.Count < roles[PlayerRole.Loyalist] && rebels.Count < roles[PlayerRole.Rebel])
            {
                roles_points[PlayerRole.Lord] += (roles[PlayerRole.Loyalist] + 1 - roles[PlayerRole.Rebel]) * 3;    //人数占优的一方额外获得3分优势
            }

            //势力方技能配合度分数矫正
            Dictionary<string, double> coop_skills = Engine.GetSkillCoopAdjust("Classic");
            foreach (PlayerRole role in side_skills.Keys)
            {
                foreach (string str in coop_skills.Keys)
                {
                    bool check = true;
                    foreach (string skill in str.Split('+'))
                    {
                        if (!side_skills[role].Contains(skill))
                        {
                            check = false;
                            break;
                        }
                    }
                    if (check) roles_points[role] += coop_skills[str];
                }
            }

            if (loyalists.Count < roles[PlayerRole.Loyalist])
            {
                int p_count = roles[PlayerRole.Loyalist] - loyalists.Count;
                double value = 0;
                foreach (Player p in loyalists_n)
                {
                    value += EvaluatePlayerStrength(p);
                }

                if (loyalists_n.Count <= p_count)
                {
                    roles_points[PlayerRole.Lord] += value;
                    roles_points[PlayerRole.Lord] += (p_count - loyalists_n.Count) * 7;
                }
                else
                    roles_points[PlayerRole.Lord] += value / loyalists_n.Count * p_count;
            }
            if (rebels.Count < roles[PlayerRole.Rebel])
            {
                int p_count = roles[PlayerRole.Rebel] - rebels.Count;
                double value = 0;
                foreach (Player p in rebels_n)
                {
                    value += EvaluatePlayerStrength(p);
                }

                if (rebels_n.Count <= p_count)
                {
                    roles_points[PlayerRole.Rebel] += value;
                    roles_points[PlayerRole.Rebel] += (p_count - rebels_n.Count) * 7;
                }
                else
                    roles_points[PlayerRole.Rebel] += value / rebels_n.Count * p_count;
            }

            Process = "=";
            double diff = roles_points[PlayerRole.Lord] - roles_points[PlayerRole.Rebel];
            if (diff > 0)
            {
                Process = ">";
                if (diff > 8)
                    Process += ">";
                if (diff > 12)
                    Process += ">";
            }
            else if (diff < 0)
            {
                Process = "<";
                if (diff < -8)
                    Process += "<";
                if (diff < -12)
                    Process += "<";
            }
            /*
#if DEBUG
            if (self.GetRoleEnum() == PlayerRole.Lord || self.GetRoleEnum() == PlayerRole.Renegade)
            {
                bool all_bot = true;
                foreach (Client client in room.Clients)
                {
                    if (client.UserID > 0)
                    {
                        all_bot = false;
                        break;
                    }
                }

                if (!all_bot)
                {
                    room.Speak(room.GetClient(Self),
                        string.Format("player {0}{1}, role {2} lord: {3} rebel: {4}, {5}",
                        self.SceenName, self.General1, self.Role, roles_points[PlayerRole.Lord], roles_points[PlayerRole.Rebel], Process));

                    foreach (Player p in loyalists)
                        room.Speak(room.GetClient(Self), string.Format("{0} {1} is loyalist", p.SceenName, p.General1));

                    foreach (Player p in loyalists_n)
                        room.Speak(room.GetClient(Self), string.Format("{0} {1} maybe loyalist", p.SceenName, p.General1));

                    foreach (Player p in rebels)
                        room.Speak(room.GetClient(Self), string.Format("{0} {1} is rebel", p.SceenName, p.General1));

                    foreach (Player p in rebels_n)
                        room.Speak(room.GetClient(Self), string.Format("{0} {1} maybe rebel", p.SceenName, p.General1));

                    Debug.Assert(true);
                }
            }

#endif
*/
            //根据场上形势识别敌我
            friends.Clear();
            enemies.Clear();
            priority_enemies.Clear();

            if (self.GetRoleEnum() == PlayerRole.Lord || self.GetRoleEnum() == PlayerRole.Loyalist)     //身份为主忠时
            {
                if (roles[PlayerRole.Rebel] == 0)       //反贼死光仅剩内奸
                {
                    if (rends.Count > 0)
                    {
                        List<Player> friends = new List<Player>(loyalists), enemies = new List<Player>(rends);
                        priority_enemies = enemies;
                        friends.Add(lord);
                        foreach (Player p in friends)
                        {
                            this.friends[p] = friends;
                            this.enemies[p] = rends;
                            PlayersLevel[p] = -1;
                        }

                        foreach (Player p in rends)
                        {
                            this.friends[p] = new List<Player>();
                            this.enemies[p] = loyalists;
                            PlayersLevel[p] = 5;
                        }
                    }
                    else                       //猜不出内奸时
                    {
                        if (self.GetRoleEnum() == PlayerRole.Lord)
                        {
                            PlayersLevel[self] = -1;
                            friends[self] = new List<Player> { self };
                            enemies[self] = new List<Player>();
                            List<Player> others = room.GetOtherPlayers(self);
                            SortByDefense(ref others);
                            if (!IsWeak(others[0]))                                             //主公会视强度最高的角色为敌人
                                enemies[self].Add(others[0]);

                            foreach (Player p in others)
                            {
                                friends[p] = new List<Player> { p };

                                PlayersLevel[p] = enemies[self].Contains(p) ? 1 : 0;
                                List<Player> _others = room.GetOtherPlayers(p);
                                _others.Remove(lord);
                                enemies[p] = _others;
                            }
                        }
                        else
                        {
                            foreach (Player p in room.GetAlivePlayers())                       //忠臣会将非主公玩家当作敌人进行攻击
                            {
                                if (p == self)
                                {
                                    PlayersLevel[p] = -1;
                                    friends[p] = new List<Player> { p, lord };
                                    List<Player> others = room.GetOtherPlayers(self);
                                    others.Remove(lord);
                                    enemies[p] = others;
                                }
                                else if (p.GetRoleEnum() == PlayerRole.Lord)
                                {
                                    PlayersLevel[p] = -1;
                                    friends[p] = new List<Player> { p };
                                    enemies[p] = new List<Player>();
                                }
                                else
                                {
                                    PlayersLevel[p] = 1;
                                    friends[p] = new List<Player> { p };
                                    List<Player> others = room.GetOtherPlayers(p);
                                    others.Remove(lord);
                                    enemies[p] = others;
                                }
                            }
                        }
                    }
                }
                else
                {
                    List<Player> friends = new List<Player>(loyalists), enemies = new List<Player>(rebels);
                    if (!friends.Contains(lord))
                        friends.Add(lord);
                    enemies.AddRange(rebels_n);
                    if (Process.Contains(">"))
                    {
                        enemies.AddRange(rends);        //主忠方优势将内奸当作敌人

                        foreach (Player p in rends)
                        {
                            PlayersLevel[p] = 1;
                            this.friends[p] = Process.Contains(">>") ? enemies : new List<Player> { p };    //主忠方优势大时内奸将反贼当作友方
                            this.enemies[p] = loyalists;
                        }
                        if (!Process.Contains(">>"))
                        {
                            foreach (Player p in loyalists_n)
                            {
                                if (!rebels_n.Contains(p))
                                {
                                    friends.Add(p);
                                    PlayersLevel[p] = 0;
                                    this.friends[p] = friends;
                                    this.enemies[p] = enemies;
                                }
                            }
                        }
                    }
                    else if (Process.Contains("<"))
                    {
                        if (Process.Contains("<<"))
                            friends.AddRange(rends);        //主忠方劣势大时将内奸当作友方

                        foreach (Player p in loyalists_n)
                        {
                            if (!rebels_n.Contains(p))
                            {
                                friends.Add(p);
                                PlayersLevel[p] = -1;
                                this.friends[p] = friends;
                                this.enemies[p] = enemies;
                            }
                        }

                        foreach (Player p in rends)
                        {
                            PlayersLevel[p] = Process.Contains("<<") ? -1 : 0;
                            this.friends[p] = Process.Contains("<<") ? friends : new List<Player> { p };    //反贼方优势大时内奸将主忠方当作友方
                            this.enemies[p] = enemies;
                        }
                    }
                    this.friends[lord] = friends;
                    this.enemies[lord] = enemies;
                    PlayersLevel[lord] = -1;

                    foreach (Player p in loyalists)
                    {
                        this.friends[p] = friends;
                        this.enemies[p] = enemies;
                        PlayersLevel[p] = -1;
                    }
                    foreach (Player p in rebels)
                    {
                        this.friends[p] = enemies;
                        this.enemies[p] = friends;
                        PlayersLevel[p] = 5;
                        priority_enemies.Add(p);
                    }
                    foreach (Player p in rebels_n)
                    {
                        this.enemies[p] = friends;
                        this.friends[p] = enemies;
                        PlayersLevel[p] = 3;
                    }
                    
                    if (loyalists.Count == roles[PlayerRole.Loyalist] && rebels.Count < roles[PlayerRole.Rebel])
                    {           //当友方全部识别完成而敌方尚未区分内奸与反贼时，则全部当作敌人处理
                        foreach (Player p in room.GetOtherPlayers(self))
                        {
                            if (!this.friends[self].Contains(p) && !this.enemies[self].Contains(p))
                            {
                                PlayersLevel[p] = 1;
                                this.enemies[self].Add(p);
                            }
                        }
                    }
                    else if (rebels.Count < roles[PlayerRole.Rebel] && loyalists.Count < roles[PlayerRole.Loyalist])
                    {               //敌我都未完成识别时，计算是反贼的几率
                        double rate = (roles[PlayerRole.Rebel] - rebels.Count) / (roles[PlayerRole.Loyalist] - loyalists.Count + roles[PlayerRole.Rebel] - rebels.Count);

                        foreach (Player p in room.GetOtherPlayers(self))
                        {
                            if (!this.friends[self].Contains(p) && !this.enemies[self].Contains(p))
                            {
                                PlayersLevel[p] = 0;
                                if (player_intention[p]["rebel"] / 10 + rate > 0.5)
                                {
                                    PlayersLevel[p] = 1;
                                    this.enemies[self].Add(p);
                                }
                            }
                        }
                    }
                }
            }
            else if (self.GetRoleEnum() == PlayerRole.Rebel)        //身份为反贼时
            {
                List<Player> friends = new List<Player>(rebels), enemies = new List<Player>(loyalists)
                {
                    lord
                };
                enemies.AddRange(loyalists_n);
                if (Process.Contains(">"))
                {
                    if (Process.Contains(">>"))
                        friends.AddRange(rends);        //主忠方优势大时将内奸当作友方

                    foreach (Player p in rebels_n)
                    {
                        if (!loyalists_n.Contains(p))
                        {
                            PlayersLevel[p] = -1;
                            friends.Add(p);
                            this.friends[p] = friends;
                            this.enemies[p] = enemies;
                        }
                    }

                    foreach (Player p in rends)
                    {
                        PlayersLevel[p] = Process.Contains(">>") ? -1 : 0;
                        this.friends[p] = Process.Contains(">>") ? friends : new List<Player> { p };    //主忠方优势大时将反贼当作友方
                        this.enemies[p] = loyalists;
                    }
                }
                else if (Process.Contains("<"))
                {
                    enemies.AddRange(rends);        //反贼方劣势大时将内奸当作敌方

                    foreach (Player p in rends)
                    {
                        PlayersLevel[p] = 1;
                        this.friends[p] = Process.Contains("<<") ? enemies : new List<Player> { p };    //反贼方优势大时将主忠方当作友方
                        this.enemies[p] = friends;
                    }
                }

                this.friends[lord] = enemies;
                this.enemies[lord] = friends;
                PlayersLevel[lord] = 5;
                priority_enemies.Add(lord);

                foreach (Player p in loyalists)
                {
                    this.friends[p] = enemies;
                    this.enemies[p] = friends;
                    PlayersLevel[p] = 3;
                }
                foreach (Player p in loyalists_n)
                {
                    this.friends[p] = enemies;
                    this.enemies[p] = friends;
                    PlayersLevel[p] = 3;
                }
                foreach (Player p in rebels)
                {
                    this.friends[p] = friends;
                    this.enemies[p] = enemies;
                    PlayersLevel[p] = -1;
                }
                
                if (rebels.Count == roles[PlayerRole.Rebel] && loyalists.Count < roles[PlayerRole.Loyalist])
                {           //当友方全部识别完成而敌方尚未区分内奸与忠臣时，则全部当作敌人处理
                    foreach (Player p in room.GetOtherPlayers(self))
                    {
                        if (!this.friends[self].Contains(p) && !this.enemies[self].Contains(p))
                        {
                            PlayersLevel[p] = 1;
                            this.enemies[self].Add(p);
                        }
                    }
                }
                else if (rebels.Count < roles[PlayerRole.Rebel] && loyalists.Count < roles[PlayerRole.Loyalist])
                {               //敌我都未完成识别时，计算是忠臣的几率
                    double rate = (roles[PlayerRole.Loyalist] - loyalists.Count) / (roles[PlayerRole.Loyalist] - loyalists.Count + roles[PlayerRole.Rebel] - rebels.Count);
                    foreach (Player p in room.GetOtherPlayers(self))
                    {
                        if (!this.friends[self].Contains(p) && !this.enemies[self].Contains(p))
                        {
                            PlayersLevel[p] = 0;
                            if (player_intention[p]["loyalist"] / 10 + rate > 0.5)
                            {
                                PlayersLevel[p] = 1;
                                this.enemies[self].Add(p);
                            }
                        }
                    }
                }
            }
            else               //身份为内奸时
            {
                if (roles[PlayerRole.Rebel] == 0 && roles[PlayerRole.Loyalist] == 0)    //主内sala时
                {
                    PlayersLevel[lord] = 5;
                    enemies[lord] = new List<Player> { self };
                    friends[lord] = new List<Player> { lord };
                    priority_enemies = new List<Player> { lord };
                    PlayersLevel[self] = -1;
                    enemies[self] = new List<Player> { lord };
                    friends[self] = new List<Player> { self };
                }
                else if (roles[PlayerRole.Rebel] == 0)                              //反贼死光，忠臣存在时
                {
                    List<Player> others = room.GetOtherPlayers(self);
                    others.Remove(lord);

                    PlayersLevel[self] = -1;
                    enemies[self] = others;
                    friends[self] = new List<Player> { self };
                    priority_enemies = new List<Player>(others);

                    foreach (Player p in room.GetOtherPlayers(self))
                    {
                        PlayersLevel[lord] = p.GetRoleEnum() == PlayerRole.Lord ? 0 : 5;
                        if (id_public[self] == "renegade")                      //内奸身份暴露时
                        {
                            enemies[p] = new List<Player> { self };
                            friends[p] = room.GetOtherPlayers(self);
                        }
                        else
                        {
                            if (p.GetRoleEnum() == PlayerRole.Lord)             //未暴露时主公应无明显倾向
                            {
                                enemies[p] = new List<Player>();
                                friends[p] = new List<Player> { p };
                            }
                            else
                            {
                                List<Player> _others = room.GetOtherPlayers(p); //未暴露时忠臣应互相攻击
                                _others.Remove(lord);
                                enemies[p] = _others;
                                friends[p] = new List<Player> { p, lord };
                            }
                        }
                    }
                }
                else
                {
                    List<Player> friends = new List<Player> { self }, enemies = new List<Player>();
                    if (Process.Contains(">"))
                    {
                        enemies.AddRange(loyalists);
                        enemies.AddRange(loyalists_n);
                        if (Process.Contains(">>"))
                        {
                            friends.AddRange(rebels);        //主忠方大优将反贼当作友方
                            foreach (Player p in rebels_n)
                            {
                                if (!loyalists_n.Contains(p))
                                    friends.Add(p);
                            }
                        }

                        List<Player> l_friends = new List<Player>(loyalists)
                        {
                            lord
                        };
                        l_friends.AddRange(loyalists_n);
                        List<Player> l_enemies = new List<Player>(rebels)
                        {
                            self
                        };

                        foreach (Player p in rebels_n)
                        {
                            if (!loyalists_n.Contains(p))
                                l_enemies.Add(p);
                        }

                        foreach (Player p in room.GetOtherPlayers(self))
                        {
                            PlayersLevel[p] = 0;
                            if (p.GetRoleEnum() == PlayerRole.Lord || loyalists.Contains(p) || loyalists_n.Contains(p))
                            {
                                PlayersLevel[p] = p.GetRoleEnum() == PlayerRole.Lord ? 0 : Process.Contains(">>") ? 5 : 1;
                                this.friends[p] = l_friends;
                                this.enemies[p] = l_enemies;
                            }
                            else if (rebels.Contains(p) || rebels_n.Contains(p))
                            {
                                List<Player> r_friends = new List<Player>(l_enemies);
                                if (!Process.Contains(">>")) r_friends.Remove(self);
                                this.friends[p] = r_friends;
                                this.enemies[p] = l_friends;
                                PlayersLevel[p] = Process.Contains(">>") ? -1 : 0;
                            }
                        }
                    }
                    else if (Process.Contains("<"))
                    {
                        enemies.AddRange(rebels);
                        enemies.AddRange(rebels_n);
                        if (Process.Contains("<<"))
                        {
                            friends.AddRange(loyalists);        //反贼方大优将主忠当作友方
                            if (!friends.Contains(lord)) friends.Add(lord);
                            foreach (Player p in loyalists_n)
                            {
                                if (!rebels_n.Contains(p))
                                    friends.Add(p);
                            }
                        }

                        List<Player> r_enemies = new List<Player>(loyalists)
                        {
                            lord,
                            self
                        };
                        r_enemies.AddRange(loyalists_n);
                        List<Player> r_friends = new List<Player>(rebels);
                        foreach (Player p in rebels_n)
                            if (!loyalists_n.Contains(p)) r_friends.Add(p);

                        foreach (Player p in room.GetOtherPlayers(self))
                        {
                            PlayersLevel[p] = 0;
                            if (rebels.Contains(p) || rebels_n.Contains(p))
                            {
                                PlayersLevel[p] = Process.Contains("<<") ? 5 : 1;
                                this.friends[p] = r_friends;
                                this.enemies[p] = r_enemies;
                            }
                            else if (p.GetRoleEnum() == PlayerRole.Lord || loyalists.Contains(p) || loyalists_n.Contains(p))
                            {
                                List<Player> l_friends = new List<Player>(r_enemies);
                                if (!Process.Contains("<<")) l_friends.Remove(self);
                                this.friends[p] = l_friends;
                                this.enemies[p] = r_friends;
                                PlayersLevel[p] = Process.Contains("<<") ? -1 : 0;
                            }
                        }
                    }

                    this.friends[self] = friends;
                    this.enemies[self] = enemies;
                    priority_enemies = enemies;
                    PlayersLevel[self] = -1;
                }
            }
            /*
#if DEBUG
            if (self.GetRoleEnum() == PlayerRole.Lord || self.GetRoleEnum() == PlayerRole.Renegade)
            {
                bool all_bot = true;
                foreach (Client client in room.Clients)
                {
                    if (client.UserID > 0)
                    {
                        all_bot = false;
                        break;
                    }
                }

                if (!all_bot)
                {
                    foreach (Player p in friends[Self])
                        room.Speak(room.GetClient(Self),
                            string.Format("{0} {1} is friend", p.SceenName, p.General1));

                    foreach (Player p in enemies[Self])
                        room.Speak(room.GetClient(Self),
                            string.Format("{0} {1} is enemy", p.SceenName, p.General1));

                    Debug.Assert(true);
                }
            }
#endif
        */
        }
        public override List<string> GetPossibleId(Player who)
        {
            if (id_tendency[who] != "unknown") return new List<string> { id_tendency[who] };

            Dictionary<string, double> sort = new Dictionary<string, double>();
            foreach (string role in player_intention[who].Keys)
            {
                int intention = player_intention[who][role];
                if (intention > 0)
                    sort.Add(role, intention);
            }

            List<string> result = new List<string>();
            CompareByStrength(sort, ref result);

            return result;
        }

        public override string GetPlayerTendency(Player player)
        {
            if (player.GetRoleEnum() == PlayerRole.Lord || player != Self)
                return id_tendency[player];
            else
                return id_public[self];
        }

        //更新玩家关系
        public override void UpdatePlayerRelation(Player from, Player to, bool friendly)
        {
            if (from == to) return;

            if (to != self || self.GetRoleEnum() == PlayerRole.Lord)
            {
                if (id_tendency[to] == "lord")
                {
                    UpdatePlayerIntention(from, friendly ? "loyalist" : "rebel", 80);
                }
                else if (id_tendency[to] == "loyalist" && from.GetRoleEnum() != PlayerRole.Lord)
                {
                    UpdatePlayerIntention(from, friendly ? "loyalist" : "rebel", 80);
                }
                else if (id_tendency[to] == "rebel" && from.GetRoleEnum() != PlayerRole.Lord)
                {
                    UpdatePlayerIntention(from, friendly ? "rebel" : "loyalist", 80);
                }
            }
            else if (self == to && id_public[self] == "rebel")
            {
                UpdatePlayerIntention(from, "loyalist", 60);
            }
            else if (self == to && id_public[self] == "loyalist")
            {
                UpdatePlayerIntention(from, "rebel", 60);
            }
        }

        //更新玩家身份的倾向
        //倾向上限值为150，超过100时即确定身份
        //AI作弊，已经确定反贼身份后若其真实身份为反贼就不会再变动
        //已确定角色身份后，身份倾向值增减减半
        public override void UpdatePlayerIntention(Player player, string role, int intention)
        {
            if (player.GetRoleEnum() == PlayerRole.Lord) return;
            if (player == self)         //记录自己的行为表现
            {
                if (player.GetRoleEnum() == PlayerRole.Rebel && id_public[player] == "rebel") return;

                if (id_public[player] != "unknown") intention /= 2;
                if ((role == "rebel" && roles[PlayerRole.Rebel] > 0) || (role == "loyalist" && roles[PlayerRole.Loyalist] > 0))
                {
                    player_intention_public[player][role] += intention;
                    player_intention_public[player][role] = Math.Min(player_intention_public[player][role], 150);
                    player_intention_public[player][role] = Math.Max(player_intention_public[player][role], 0);

                    if (role == "rebel")
                    {
                        player_intention_public[player]["loyalist"] -= intention;
                    }
                    else
                    {
                        player_intention_public[player]["rebel"] -= intention;
                    }
                }
                else if (role == "renegade")                //身份倾向不会直接赋值内奸，需要根据形势判断
                {
                    if (Process.Contains("<"))
                    {
                        player_intention_public[player]["rebel"] -= intention;
                        player_intention_public[player]["loyalist"] += intention;
                    }
                    else if (Process.Contains(">"))
                    {
                        player_intention_public[player]["rebel"] += intention;
                        player_intention_public[player]["loyalist"] -= intention;
                    }
                }

                player_intention_public[player]["loyalist"] = Math.Min(player_intention_public[player]["loyalist"], 150);
                player_intention_public[player]["loyalist"] = Math.Max(player_intention_public[player]["loyalist"], 0);

                player_intention_public[player]["rebel"] = Math.Min(player_intention_public[player]["rebel"], 150);
                player_intention_public[player]["rebel"] = Math.Max(player_intention_public[player]["rebel"], 0);

                if (player_intention_public[player]["rebel"] >= 60 && player_intention_public[player]["loyalist"] >= 60)
                {
                    if (roles[PlayerRole.Renegade] > 0)
                        id_public[player] = "renegade";
                    else
                        id_public[player] = "unknown";
                }
                else if (player_intention_public[player]["rebel"] >= 100)
                    id_public[player] = "rebel";
                else if (player_intention_public[player]["loyalist"] >= 100)
                    id_public[player] = "loyalist";
                else
                    id_public[player] = "unknown";
            }
            else
            {
                if (player.GetRoleEnum() == PlayerRole.Rebel && id_tendency[player] == "rebel") return;       //小作弊，已经确定反贼身份就不会再变动

                if (id_tendency[player] != "unknown") intention /= 2;
                if ((role == "rebel" && roles[PlayerRole.Rebel] > 0) || (role == "loyalist" && roles[PlayerRole.Loyalist] > 0))
                {
                    player_intention[player][role] += intention;
                    player_intention[player][role] = Math.Min(player_intention[player][role], 150);
                    player_intention[player][role] = Math.Max(player_intention[player][role], 0);

                    if (role == "rebel")
                    {
                        player_intention[player]["loyalist"] -= intention;
                    }
                    else
                    {
                        player_intention[player]["rebel"] -= intention;
                    }
                }
                else if (role == "renegade")                //身份倾向不会直接赋值内奸，需要根据形势判断
                {
                    if (Process.Contains("<"))
                    {
                        player_intention[player]["rebel"] -= intention;
                        player_intention[player]["loyalist"] += intention;
                    }
                    else if (Process.Contains(">"))
                    {
                        player_intention[player]["rebel"] += intention;
                        player_intention[player]["loyalist"] -= intention;
                    }
                }

                player_intention[player]["loyalist"] = Math.Min(player_intention[player]["loyalist"], 150);
                player_intention[player]["loyalist"] = Math.Max(player_intention[player]["loyalist"], 0);

                player_intention[player]["rebel"] = Math.Min(player_intention[player]["rebel"], 150);
                player_intention[player]["rebel"] = Math.Max(player_intention[player]["rebel"], 0);

                if (player_intention[player]["rebel"] >= 60 && player_intention[player]["loyalist"] >= 60)
                {
                    if (roles[PlayerRole.Renegade] > 0)
                        id_tendency[player] = "renegade";
                    else
                        id_tendency[player] = "unknown";
                }
                else if (player_intention[player]["rebel"] >= 100)
                    id_tendency[player] = "rebel";
                else if (player_intention[player]["loyalist"] >= 100)
                    id_tendency[player] = "loyalist";
                else
                    id_tendency[player] = "unknown";
            }

            UpdatePlayers();
        }

        public bool NeedDamage(DamageStruct _damage, DamageStruct.DamageStep step = DamageStruct.DamageStep.Caused)
        {
            DamageStruct damage = new DamageStruct(_damage.Card, _damage.From, _damage.To, _damage.Damage, _damage.Nature)
            {
                Reason = _damage.Reason,
                Steped = _damage.Steped,
                Transfer = _damage.Transfer,
                TransferReason = _damage.TransferReason,
                Chain = _damage.Chain
            };

            Player from = damage.From;
            Player to = damage.To;
            ScoreStruct result_score = new ScoreStruct
            {
                Damage = damage,
                DoDamage = true
            };

            if (!HasSkill("jueqing|gangzhi_classic", from))
            {
                damage.Damage = DamageEffect(damage, step);
                if (damage.Steped < DamageStruct.DamageStep.Caused)
                    damage.Steped = DamageStruct.DamageStep.Caused;

                damage.Damage = DamageEffect(damage, DamageStruct.DamageStep.Done);
                damage.Steped = DamageStruct.DamageStep.Done;
                result_score.Damage = damage;
            }
            else
                result_score.DoDamage = false;

            if (result_score.DoDamage && result_score.Damage.Damage < to.Hp && damage.Damage == 1)
            {
                if (HasSkill("yiji_jx|chouce", to)) return true;
                List<Player> friends = GetFriends(to);
                if (HasSkill("jieming_jx", to))
                {
                    foreach (Player p in friends)
                        if (p.MaxHp - p.HandcardNum <= 2) return true;
                }

                if (HasSkill("fangzhu", to))
                {
                    foreach (Player p in friends)
                        if (p != to && !p.FaceUp) return true;

                    if (!IsWeak(to))
                        foreach (Player p in GetEnemies(to))
                            if (!p.FaceUp) return true;
                }
            }

            if (HasSkill("hunzi", to) && to.GetMark("hunzi") == 0 && damage.Damage == 1 && to.Hp == 2)
            {
                bool save = true;
                Player next = room.GetNextAlive(room.Current, 1, false);
                while (next != to)
                {
                    if (!WillSkipPlayPhase(next) && !IsFriend(next, to))
                    {
                        save = false;
                        break;
                    }
                    next = room.GetNextAlive(next, 1, false);
                }

                if (!save) return false;
            }

            return false;
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
            if (damage.From == null || !HasSkill("jueqing|gangzhi_classic", damage.From))
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

            if (from != null && from.Alive && damage.To.HasFlag(string.Format("yinju_{0}", from.Name)))
            {
                result_score.DoDamage = false;
                double value = 4 * Math.Min(to.GetLostHp(), damage.Damage);
                if (IsFriend(to))
                    result_score.Score += value;
                else if (IsEnemy(to))
                    result_score.Score -= value;

                return result_score;
            }

            if (damage.Damage > 0 && NotHurtXiaoqiao(damage))
            {
                result_score.Score = -20;
                return result_score;
            }


            if (from == null || !HasSkill("jueqing|gangzhi_classic", from))
                damage.Damage = DamageEffect(damage, DamageStruct.DamageStep.Done);

            damage.Steped = DamageStruct.DamageStep.Done;
            result_score.Damage = damage;

            if (HasSkill("lixun", to) && to.GetMark("@zhu") >= 10 && (from == null || !HasSkill("jueqing|gangzhi_classic", from))) damage.Damage = 0;
            if (damage.To.GetMark("@tangerine") > 0)            //橘防止伤害时
            {
                double value = 3;
                //根据身份矫正分数
                if (self.GetRoleEnum() == PlayerRole.Lord || self.GetRoleEnum() == PlayerRole.Loyalist)
                {
                    if (IsFriend(to))
                        value = -value;
                    else if (IsEnemy(to))
                    {
                        if (PlayersLevel[to] < 2) value -= 0.5;
                    }
                    else
                    {
                        if (GetPlayerTendency(to) == "renegade")
                        {
                            if (Process.Contains("<"))
                                value = value * -0.7;
                        }
                        else
                        {
                            int loyalist = 0, rebel = 0;
                            foreach (Player p in enemies[self])
                                if (GetPlayerTendency(p) == "rebel") rebel++;
                            foreach (Player p in friends[self])
                                if (GetPlayerTendency(p) == "loyalist") loyalist++;

                            int count = roles[PlayerRole.Rebel] - rebel + roles[PlayerRole.Loyalist] - loyalist;
                            if (count > 0)
                            {
                                double rate = (roles[PlayerRole.Rebel] - rebel) / count;
                                if (rate > 0.5)
                                    value *= rate;
                                else
                                    value *= (rate - 1);
                            }
                            else
                            {                                               //出现分母为0的情况很可能是内奸
                                room.Debug(string.Format("player name {0} role {1} maybe {2}", to.ActualGeneral1, to.Role, GetPlayerTendency(to)));
                                if (Process.Contains("<"))
                                {
                                    value = value * -0.7;
                                }
                            }
                        }
                    }
                }
                else if (self.GetRoleEnum() == PlayerRole.Rebel)
                {
                    if (IsEnemy(to))
                    {
                        value += 1;
                    }
                    else if (IsFriend(to))
                    {
                        value = -value;
                    }
                    else
                    {
                        if (GetPlayerTendency(to) == "renegade")
                        {
                            if (Process.Contains(">"))
                                value = value * -0.7;
                        }
                        else
                        {
                            int loyalist = 0, rebel = 0;
                            foreach (Player p in enemies[self])
                                if (GetPlayerTendency(p) == "loyalist") loyalist++;
                            foreach (Player p in friends[self])
                                if (GetPlayerTendency(p) == "rebel") rebel++;

                            int count = roles[PlayerRole.Rebel] - rebel + roles[PlayerRole.Loyalist] - loyalist;
                            if (count > 0)
                            {
                                double rate = (roles[PlayerRole.Loyalist] - loyalist) / count;
                                if (rate > 0.5)
                                    value *= rate;
                                else
                                    value *= (rate - 1);
                            }
                            else
                            {                                               //出现分母为0的情况很可能是内奸
                                room.Debug(string.Format("player name {0} role {1} maybe {2}", to.ActualGeneral1, to.Role, GetPlayerTendency(to)));
                                if (Process.Contains(">"))
                                {
                                    value = value * -0.7;
                                }
                            }
                        }
                    }
                }
                else
                {
                    //内奸
                    if (IsEnemy(to))
                    {
                        value += 1;
                    }
                    else if (IsFriend(to))
                    {
                        value = -value;
                    }
                    else
                    {
                        if (to.GetRoleEnum() == PlayerRole.Lord)
                        {
                            if (to.Hp >= 4)
                                value = 0.5;
                            else if (to.Hp > 2)
                                value = 0;
                            else if (to.Hp <= 2)
                                value = -1;
                        }
                        else if ((GetPlayerTendency(to) == "rebel" && Process.Contains(">")) || (GetPlayerTendency(to) == "loyalist" && Process.Contains("<")))      //根据场面局势调整分数
                        {
                            value = -1;
                        }
                        else
                            value = 2;
                    }
                }

                result_score.Score = value;
            }

            List <ScoreStruct> scores = new List<ScoreStruct>();
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
                                double x = HasSkill("buqu_jx", to) ? 1 / Math.Pow(i, 2) : (double)8 / Math.Pow(i, 2);
                                value += x;
                            }
                        }
                        else
                            deadly = true;
                }

                if (!to.Removed && CanResist(to, damage.Damage) && (from == null || !HasSkill("jueqing|gangzhi_classic", damage.From))) result_score.Score = 3;

                //根据身份矫正分数
                if (self.GetRoleEnum() == PlayerRole.Lord || self.GetRoleEnum() == PlayerRole.Loyalist)
                {
                    if (IsFriend(to))
                    {
                        value = -value;
                        if (deadly)
                        {
                            if (to.GetRoleEnum() == PlayerRole.Lord)
                            {
                                value -= 20;
                            }
                            else
                            {
                                value -= 4;
                                if (GetPlayerTendency(to) == "loyalist" && from != null && from.GetRoleEnum() == PlayerRole.Lord && !from.IsNude() && !HasSkill("jueqing|gangzhi_classic", from))
                                    value -= from.GetCards("he").Count * 1.5;
                            }
                        }
                    }
                    else if (IsEnemy(to))
                    {
                        if (deadly)
                        {
                            value += 2;
                            if (IsEnemy(to) && enemies[self].Count == 1)
                            {
                                value += 5;
                            }
                            if (GetPlayerTendency(to) == "rebel" && damage.From != null && damage.From.Alive && !HasSkill("jueqing|gangzhi_classic", from) && !HasSkill("lixun", to))
                            {
                                value += IsFriend(from) ? 1.5 * 3 : -1.5 * 3;
                            }
                        }

                        if (PlayersLevel[to] < 2) value /= 2;
                    }
                    else
                    {
                        if (GetPlayerTendency(to) == "renegade")
                        {
                            if (Process.Contains("<"))
                            {
                                value = value * -0.7;
                                if (deadly) value -= 5;
                            }
                        }
                        else
                        {
                            int loyalist = 0, rebel = 0;
                            foreach (Player p in enemies[self])
                                if (GetPlayerTendency(p) == "rebel") rebel++;
                            foreach (Player p in friends[self])
                                if (GetPlayerTendency(p) == "loyalist") loyalist++;

                            int count = roles[PlayerRole.Rebel] - rebel + roles[PlayerRole.Loyalist] - loyalist;
                            if (count > 0)
                            {
                                double rate = (roles[PlayerRole.Rebel] - rebel) / count;
                                if (rate > 0.5)
                                    value *= rate;
                                else
                                    value *= (rate - 1);
                            }
                            else
                            {                                               //出现分母为0的情况很可能是内奸
                                room.Debug(string.Format("player name {0} role {1} maybe {2}", to.ActualGeneral1, to.Role, GetPlayerTendency(to)));
                                if (Process.Contains("<"))
                                {
                                    value = value * -0.7;
                                    if (deadly) value -= 5;
                                }
                            }
                        }
                    }

                    if (deadly && to.GetRoleEnum() != PlayerRole.Lord && !to.IsNude() && !HasSkill("lixun", to))
                    {
                        Player caopi = FindPlayerBySkill("xingshang");
                        if (caopi != null && to != caopi && GetPlayerTendency(caopi) != "renegade")
                            value += IsFriend(caopi) ? 0.5 * to.GetCardCount(true) : -0.5 * to.GetCardCount(true);
                    }
                }
                else if (self.GetRoleEnum() == PlayerRole.Rebel)
                {
                    if (IsEnemy(to))
                    {
                        value += 2;
                        if (deadly)
                        {
                            if (to.GetRoleEnum() == PlayerRole.Lord)
                            {
                                value += 20;
                            }
                            else
                            {
                                if (GetPlayerTendency(to) == "loyalist" && from != null && from.GetRoleEnum() == PlayerRole.Lord && !from.IsNude() && !HasSkill("jueqing|gangzhi_classic", from))
                                    value += from.GetCardCount(true) * 1.5;
                            }
                        }
                        if (PlayersLevel[to] < 2) value /= 2;
                    }
                    else if (IsFriend(to))
                    {
                        value = -value;
                        if (deadly)
                        {
                            value -= 2;
                            if (GetPlayerTendency(to) == "rebel" && damage.From != null && damage.From.Alive)
                            {
                                value += IsFriend(from) ? 1.5 * 3 : -1.5 * 3;
                                if (to.Hp == 1 && to.GetCardCount(true) < 2 && !MaySave(to) && GetPlayerTendency(from) == "rebel")      //无可救药的反贼同伴宁可被自己人收掉
                                {
                                    if (!HasSkill("jueqing|gangzhi_classic", from) && !HasSkill("lixun", to))
                                        value += 4;
                                    else
                                        value += 1;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (GetPlayerTendency(to) == "renegade")
                        {
                            if (Process.Contains(">"))
                            {
                                value = value * -0.7;
                                if (deadly) value -= 5;
                            }
                        }
                        else
                        {
                            int loyalist = 0, rebel = 0;
                            foreach (Player p in enemies[self])
                                if (GetPlayerTendency(p) == "loyalist") loyalist++;
                            foreach (Player p in friends[self])
                                if (GetPlayerTendency(p) == "rebel") rebel++;

                            int count = roles[PlayerRole.Rebel] - rebel + roles[PlayerRole.Loyalist] - loyalist;
                            if (count > 0)
                            {
                                double rate = (roles[PlayerRole.Loyalist] - loyalist) / count;
                                if (rate > 0.5)
                                    value *= rate;
                                else
                                    value *= (rate - 1);
                            }
                            else
                            {                                               //出现分母为0的情况很可能是内奸
                                room.Debug(string.Format("player name {0} role {1} maybe {2}", to.ActualGeneral1, to.Role, GetPlayerTendency(to)));
                                if (Process.Contains(">"))
                                {
                                    value = value * -0.7;
                                    if (deadly) value -= 5;
                                }
                            }
                        }
                    }

                    if (deadly && to.GetRoleEnum() != PlayerRole.Lord && !to.IsNude() && !HasSkill("lixun", to))
                    {
                        Player caopi = FindPlayerBySkill("xingshang");
                        if (caopi != null && caopi != to && GetPlayerTendency(caopi) != "renegade")
                            value += IsFriend(caopi) ? 0.5 * to.GetCardCount(true) : -0.5 * to.GetCardCount(true);
                    }
                }
                else
                {
                    //内奸
                    if (to.GetRoleEnum() == PlayerRole.Lord && deadly && (roles[PlayerRole.Loyalist] > 0 || roles[PlayerRole.Rebel] > 0))             //对主公的致命伤
                        value -= 100;
                    else if (IsEnemy(to))
                    {
                        value += 2;
                        if (deadly)
                        {
                            if (to.GetRoleEnum() == PlayerRole.Lord)
                            {
                                value += 20;
                            }
                            else
                            {
                                if (GetPlayerTendency(to) == "loyalist" && from != null && from.GetRoleEnum() == PlayerRole.Lord && !from.IsNude() && !HasSkill("jueqing|gangzhi_classic", from))
                                    value += from.GetCardCount(true) * 1.5;
                            }
                        }
                    }
                    else if (IsFriend(to))
                    {
                        value = -value;
                        if (deadly)
                        {
                            value -= 2;
                            if (to == self)
                                value -= 10;
                        }
                    }
                    else
                    {
                        if (to.GetRoleEnum() == PlayerRole.Lord)
                        {
                            if (to.Hp >= 4)
                                value = 0.5;
                            else if (to.Hp > 2)
                                value = 0;
                            else if (to.Hp <= 2)
                                value = -1;
                        }
                        else if ((GetPlayerTendency(to) == "rebel" && Process.Contains(">")) || (GetPlayerTendency(to) == "loyalist" && Process.Contains("<")))      //根据场面局势调整分数
                        {
                            value = -2;
                            if (deadly) value -= 4;
                        }
                        else
                            value = 2;
                    }

                    if (deadly && to.GetRoleEnum() != PlayerRole.Lord && !to.IsNude() && to != self)
                    {
                        if (GetPlayerTendency(to) == "rebel" && damage.From == self)
                            value += 1.5 * 3;

                        if (HasSkill("xingshang") && !HasSkill("lixun", to)) value += 0.5 * to.GetCardCount(true);
                    }
                }

                foreach (SkillEvent e in skill_events.Values)
                {
                    if (e.Name != damage.Reason)
                        value += e.GetDamageScore(this, damage).Score;
                }
                if (priority_enemies.Contains(to) && value > 0)
                    value *= 1.5;
                else if (!IsSituationClear() && value > 0)
                    value /= 2;

                result_score.Score = value;
            }
            scores.Add(result_score);

            if (from != null && HasSkill("zhiman|zhiman_jx", from) && RoomLogic.GetPlayerCards(room, to, "ej").Count > 0)
            {
                ScoreStruct score = FindCards2Discard(from, to, string.Empty, "ej", HandlingMethod.MethodGet);
                scores.Add(score);
            }
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

        public override void FindSlashandTarget(ref CardUseStruct use, Player player, List<WrappedCard> slashes = null)
        {
            List<ScoreStruct> values = CaculateSlashIncome(player, slashes);
            double best = 0;
            if (values.Count > 0)
                best = values[0].Score;

            if (HasSkill("keji_jx", player) && !player.HasFlag("KejiSlashInPlayPhase") && GetOverflow(player) > 2)
            {          //old keji
                bool multi_slash = false;
                if (GetKnownCardsNums(Slash.ClassName, "h", player, self) >= GetOverflow(player))
                {
                    WrappedCard slash = new WrappedCard(Slash.ClassName);
                    if (Engine.CorrectCardTarget(room, TargetModSkill.ModType.Residue, player, slash) >= GetOverflow(player))
                        multi_slash = true;
                    if (!multi_slash && GetCards(CrossBow.ClassName, player).Count > 0)
                    {
                        foreach (Player p in room.GetOtherPlayers(player))
                        {
                            if (RoomLogic.DistanceTo(room, player, p) == 1 && !IsFriend(p) && !HasSkill("fankui", p))
                            {
                                multi_slash = true;
                                break;
                            }
                        }
                    }

                    bool negative = true;
                    if (self.GetRoleEnum() == PlayerRole.Renegade && (Process.Contains(">>") || Process.Contains("<<")))        //内奸吕蒙会在局势崩塌时积极行动
                        negative = false;
                    else if (self.GetRoleEnum() == PlayerRole.Rebel && Process.Contains("<<"))                                  //当己方取得优势时不再蹲坑
                        negative = false;
                    else if ((self.GetRoleEnum() == PlayerRole.Lord || self.GetRoleEnum() == PlayerRole.Loyalist) && Process.Contains(">>"))
                        negative = false;

                    if (!multi_slash && negative && best < 8)
                        return;
                }
            }

            //check will use analeptic
            if (!use.IsDummy && player.Phase == PlayerPhase.Play && pre_drink == null && IsSituationClear()
                && room.GetRoomState().GetCurrentCardUseReason() == CardUseStruct.CardUseReason.CARD_USE_REASON_PLAY)
            {
                List<WrappedCard> analeptics = GetCards(Analeptic.ClassName, player);
                bool will_use = false;
                bool drink_hand = false;
                if (analeptics.Count > 0)
                {
                    foreach (int id in analeptics[0].SubCards)
                    {
                        if (room.GetCardPlace(id) == Place.PlaceHand && room.GetCardOwner(id) == player)
                        {
                            drink_hand = true;
                            break;
                        }
                    }
                }
                if (analeptics.Count > 1 || GetEnemies(player).Count == 1 || (GetOverflow(player) > 0 && drink_hand)) will_use = true;
                //room.OutPut(string.Format("{0} 检查酒的数量为 {1} 是否需要使用 {2}", player.SceenName, analeptics.Count, will_use.ToString()));

                FunctionCard fcard = Analeptic.Instance;
                foreach (WrappedCard analeptic in analeptics)
                {
                    if (fcard.IsAvailable(room, player, analeptic) && (will_use
                            || (analeptic.IsVirtualCard() && analeptic.SubCards.Count == 0 && GetUseValue(analeptic, player) > 0)))
                    {
                        pre_drink = analeptic;
                        double drank_value = 0;
                        List<ScoreStruct> drank_values = CaculateSlashIncome(player, slashes);
                        pre_drink = null;
                        if (drank_values.Count > 0)
                            drank_value = drank_values[0].Score;

                        foreach (string skill in GetKnownSkills(player))
                        {
                            SkillEvent skill_e = Engine.GetSkillEvent(skill);
                            if (skill_e != null)
                                drank_value += skill_e.TargetValueAdjust(this, analeptic, player, new List<Player> { player }, player);
                        }

                        //room.OutPut(string.Format("{0} 喝酒后的分数由 {1} 提高至 {2}", player.SceenName, best, drank_value));

                        if (drank_value - best >= 6 || (GetOverflow(player) > 0 && drink_hand && drank_value - best > 2))
                        {
                            use = new CardUseStruct(analeptic, player, new List<Player>());
                            return;
                        }
                    }
                }
            }

            if (values.Count > 0 && values[0].Score > 0)
            {
                bool will_slash = false;

                Player gn = RoomLogic.FindPlayerBySkillName(room, "jieying_gn");
                if (player.GetMark("@rob") > 0 && !HasSkill("jieying_gn") && gn != null && !IsFriend(gn))
                    will_slash = true;
                else if (values[0].Score >= 10)
                {
                    will_slash = true;
                }
                else if (GetEnemies(player).Count == 1 && GetOverflow(player) > 0)
                {
                    foreach (int id in values[0].Card.SubCards)
                    {
                        if (room.GetCardPlace(id) == Place.PlaceHand)
                        {
                            will_slash = true;
                            break;
                        }
                    }
                }
                else if (IsSituationClear())
                {
                    foreach (Player p in values[0].Players)
                    {
                        if (GetPrioEnemies().Contains(p))
                        {
                            will_slash = true;
                            break;
                        }
                    }
                }
                else if (self.GetRoleEnum() == PlayerRole.Renegade && (Process == "<" || Process == "=" || Process == ">") && values[0].Score <= 4)     //若没有大的利益或局势崩塌，内奸不会积极行动
                    return;
                else if (values[0].Score > 0 && values[0].Card.SubCards.Count == 0)
                    will_slash = true;

                if (!will_slash && values[0].Score >= 0 && player.ContainsTag("sidi") && player.GetTag("sidi") is List<string> caozhens)     //被司敌，要用杀，否则会被杀
                {
                    WrappedCard sd = new WrappedCard(Slash.ClassName);
                    foreach (string name in caozhens)
                    {
                        Player target = room.FindPlayer(name);
                        if (target != null && RoomLogic.IsProhibited(room, target, player, sd) != null && IsCardEffect(sd, player, target))
                        {
                            will_slash = true;
                            break;
                        }
                    }
                }
                if (!will_slash && values[0].Score >= 0 && HasSkill("tushe", player) && (player.JudgingArea.Count > 0 || player.HasWeapon(Spear.ClassName)))
                    will_slash = true;

                //todo: adjust ai personality
                if (!will_slash && GetOverflow(player) > 0)
                {
                    foreach (int id in values[0].Card.SubCards)
                    {
                        if (room.GetCardPlace(id) == Place.PlaceHand)
                        {
                            will_slash = true;
                            break;
                        }
                    }
                }

                if (will_slash)
                {
                    if (HasSkill("wuyuan") && values[0].Score < 7 && !values[0].Card.IsVirtualCard() && GetOverflow(player) == 0 && (FriendNoSelf.Count > 0
                        || (player.GetRoleEnum() == PlayerRole.Rebel && GetRolePitts(PlayerRole.Rebel) > 1)) && room.GetCardPlace(values[0].Card.GetEffectiveId()) == Place.PlaceHand)
                    {
                        bool other = false;
                        foreach (int id in player.GetCards("h"))
                        {
                            if (!values[0].Card.SubCards.Contains(id) && room.GetCard(id).Name.Contains(Slash.ClassName))
                            {
                                other = true;
                                break;
                            }
                        }
                        if (!other) return;
                    }

                    if (HasSkill("duanbing") && WillShowForAttack() && values[0].Players.Count == 1 && RoomLogic.DistanceTo(room, player, values[0].Players[0], values[0].Card) == 1
                        && values.Count > 1)
                    {
                        for (int i = 1; i < values.Count; i++)
                        {
                            if (values[i].Score > 0 && values[i].Card == values[0].Card && !values[i].Players.Contains(values[0].Players[0]))
                            {
                                use.From = player;
                                use.To = values[i].Players;
                                use.Card = values[0].Card;
                                return;
                            }
                        }
                    }
                    else
                    {
                        use.From = player;
                        use.To = values[0].Players;
                        use.Card = values[0].Card;
                    }
                }
            }
        }
        public override List<ScoreStruct> CaculateSlashIncome(Player player, List<WrappedCard> slashes = null, List<Player> targets = null, bool play = true)
        {
            List<ScoreStruct> values = new List<ScoreStruct>();
            if (slashes == null || slashes.Count == 0)
                slashes = GetCards(Slash.ClassName, player);

            List<WrappedCard> cards = new List<WrappedCard>();
            FunctionCard fcard = Slash.Instance;
            foreach (WrappedCard slash in slashes)
            {
                if (!slash.Name.Contains(Slash.ClassName) || (play && !fcard.IsAvailable(room, player, slash))) continue;
                cards.Add(slash);
                if (player.HasWeapon(Fan.ClassName) && slash.Name == Slash.ClassName && !slash.SubCards.Contains(player.Weapon.Key))
                {
                    WrappedCard fire_slash = new WrappedCard(FireSlash.ClassName)
                    {
                        DistanceLimited = slash.DistanceLimited
                    };
                    fire_slash.AddSubCard(slash);
                    fire_slash = RoomLogic.ParseUseCard(room, fire_slash);
                    fire_slash.UserString = RoomLogic.CardToString(room, slash);
                    cards.Add(fire_slash);
                }
                else if (!player.HasWeapon(Fan.ClassName) && slash.Name == Slash.ClassName && HasSkill("lihuo", player) && player.Hp > 1)
                {
                    WrappedCard fire_slash = new WrappedCard(FireSlash.ClassName)
                    {
                        DistanceLimited = slash.DistanceLimited
                    };
                    fire_slash.SetFlags("lihuo");
                    fire_slash.AddSubCard(slash);
                    fire_slash = RoomLogic.ParseUseCard(room, fire_slash);
                    fire_slash.UserString = RoomLogic.CardToString(room, slash);
                    cards.Add(fire_slash);
                }
            }

            if (targets == null || targets.Count == 0)
                targets = room.GetOtherPlayers(player);

            foreach (WrappedCard slash in cards)
            {
                //use value的比重应减小
                double value = HasSkill("wuyuan") ? 0 : (GetUseValue(slash, self) - Engine.GetCardUseValue(slash.Name)) / 2;

                int extra = Engine.CorrectCardTarget(room, TargetModSkill.ModType.ExtraMaxTarget, player, slash);
                Dictionary<Player, ScoreStruct> available_targets = new Dictionary<Player, ScoreStruct>();
                foreach (Player target in targets)
                {
                    ClearPreInfo();
                    if (!Slash.Instance.TargetFilter(room, new List<Player>(), target, player, slash)) continue;
                    available_targets.Add(target, SlashIsEffective(slash, target));
                }

                int target_count = Math.Min(available_targets.Keys.Count, extra + 1);
                List<List<Player>> uses = new List<List<Player>>();
                for (int i = 1; i <= target_count; i++)
                {
                    List<List<Player>> players = GetCombinationList(new List<Player>(available_targets.Keys), i);
                    uses.AddRange(players);
                }

                foreach (List<Player> use in uses)
                {
                    List<Player> _targets = use;
                    room.SortByActionOrder(ref _targets);

                    bool chain = false;
                    double use_value = 0;
                    foreach (Player target in _targets)
                    {
                        ScoreStruct score = available_targets[target];
                        if (score.Score == 0) continue;
                        use_value += score.Score;

                        foreach (string skill in GetKnownSkills(target))
                        {
                            SkillEvent ev = Engine.GetSkillEvent(skill);
                            if (ev != null)
                                use_value += ev.TargetValueAdjust(this, slash, player, _targets, target);
                        }

                        if (IsFriend(target) && target.Chained && chain && score.Damage.Damage > 0)
                            use_value -= 10;

                        if (score.DoDamage && !chain)
                        {
                            if (target.Chained && !IsSpreadStarter(score.Damage) && score.Damage.Nature != DamageStruct.DamageNature.Normal)
                                value -= 0.5;
                            else
                            {
                                double chain_value = ChainDamage(score.Damage);
                                if (IsSpreadStarter(score.Damage))
                                {
                                    if (chain_value < 0)
                                    {
                                        use_value += chain_value;
                                        chain = true;
                                    }
                                    else if (score.Info != "miss" && score.Rate > 0.5)
                                    {
                                        use_value += chain_value * score.Rate;
                                        chain = true;
                                    }
                                }
                            }
                        }
                    }
                    if (use_value >= 0)
                    {
                        use_value += value;
                        if (!string.IsNullOrEmpty(slash.Skill))
                        {
                            SkillEvent e = Engine.GetSkillEvent(slash.Skill);
                            if (e != null)
                                use_value += e.UseValueAdjust(this, player, _targets, slash);
                        }
                    }

                    if (_targets.Count == 1 && !IsFriend(_targets[0]) && use_value >= 4)
                    {            //yuji?
                        bool yuji = false;
                        foreach (Player p in room.GetAlivePlayers())
                            if (HasSkill("qianhuan", p) && RoomLogic.IsFriendWith(room, p, _targets[0]) && p.GetPile("sorcery").Count > 0)
                                yuji = true;

                        List<Player> targets_const = new List<Player>
                        {
                            _targets[0]
                        };
                        if (yuji)
                        {
                            bool extra_target = false;
                            if (HasSkill("duanbing", player))
                            {
                                List<Player> duanbings = new List<Player>();
                                foreach (Player p in room.GetOtherPlayers(player))
                                    if (!_targets.Contains(p) && fcard.ExtratargetFilter(room, targets_const, p, player, slash) && RoomLogic.DistanceTo(room, player, p, slash) == 1)
                                        duanbings.Add(p);

                                foreach (Player p in duanbings)
                                    if (SlashIsEffective(slash, p).Score >= 0)
                                        extra_target = true;
                            }
                            if (!extra_target && player.HasFlag("TianyiSuccess"))
                            {
                                List<Player> tianyis = new List<Player>();
                                foreach (Player p in room.GetOtherPlayers(player))
                                    if (!_targets.Contains(p) && fcard.ExtratargetFilter(room, targets_const, p, player, slash))
                                        tianyis.Add(p);

                                foreach (Player p in tianyis)
                                    if (SlashIsEffective(slash, p).Score >= 0)
                                        extra_target = true;
                            }
                            if (!extra_target && player.HasWeapon(Halberd.ClassName) && !slash.SubCards.Contains(player.Weapon.Key))
                            {
                                List<Player> halberds = new List<Player>();
                                foreach (Player p in room.GetOtherPlayers(player))
                                    if (!_targets.Contains(p) && fcard.ExtratargetFilter(room, targets_const, p, player, slash) && !RoomLogic.IsFriendWith(room, p, _targets[0]))
                                        halberds.Add(p);

                                foreach (Player p in halberds)
                                    if (SlashIsEffective(slash, p).Score >= 0)
                                        extra_target = true;
                            }

                            if (!extra_target) use_value = 3;
                        }
                    }

                    ScoreStruct result_score = new ScoreStruct
                    {
                        Card = slash,
                        Players = _targets,
                        Score = use_value
                    };


                    values.Add(result_score);
                }
            }

            CompareByScore(ref values);
            return values;
        }
        public override double ChainDamage(DamageStruct damage)
        {
            if (damage.To == null || !damage.To.Alive || !damage.To.Chained || damage.Chain || damage.Damage <= 0 || damage.Nature == DamageStruct.DamageNature.Normal
                || HasSkill("gangzhi", damage.To) || (damage.From != null && HasSkill("jueqing|gangzhi_classic", damage.From)))
                return 0;

            List<Player> players = GetSpreadTargets(damage);
            if (players.Count == 0) return 0;
            double good = 0;
            foreach (Player p in players)
            {
                DamageStruct spread = new DamageStruct(damage.Card, damage.From, damage.To, damage.Damage, damage.Nature);
                damage.Reason = damage.Reason;
                damage.Steped = damage.Steped;
                spread.Chain = true;
                spread.To = p;
                int cause = DamageEffect(spread, DamageStruct.DamageStep.Caused);
                bool caculat_break = false;
                if (cause > 0)
                {
                    if (IsFriend(p))
                    {
                        if (spread.From != null && HasSkill("zhiman|zhiman_jx", spread.From) && IsFriend(spread.From, spread.To))
                        {
                            if (spread.To.JudgingArea.Count > 0 || GetLostEquipEffect(spread.To) < 0)
                            {
                                caculat_break = true;
                                good += 4;
                            }
                        }
                    }
                    else if (IsEnemy(p))
                    {
                        if (spread.From != null && HasSkill("zhiman|zhiman_jx", spread.From) && IsFriend(spread.From, spread.To))
                        {
                            if (spread.To.JudgingArea.Count > 0 || GetLostEquipEffect(spread.To) < 0)
                            {
                                good -= 4;
                                caculat_break = true;
                            }
                        }
                    }
                }

                if (caculat_break) continue;

                int done = DamageEffect(spread, DamageStruct.DamageStep.Done);
                if (done > 0)
                {
                    ScoreStruct score = GetDamageScore(spread);
                    good += score.Score;
                }
            }

            return good;
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
                            //左慈技能还原
                            ZuociReturn(ref card_use);

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
            if (HasSkill("aocai") && Self.Phase == PlayerPhase.NotActive && self.GetPile("#aocai").Count == 0           //傲才耦合
                && (room.GetRoomState().GetCurrentCardUseReason() == CardUseStruct.CardUseReason.CARD_USE_REASON_RESPONSE
                || room.GetRoomState().GetCurrentCardUseReason() == CardUseStruct.CardUseReason.CARD_USE_REASON_RESPONSE_USE))
            {
                WrappedCard aocai = new WrappedCard(AocaiCard.ClassName) { Skill = "aocai" };
                WrappedCard slash = new WrappedCard(Slash.ClassName);
                if (Engine.MatchExpPattern(room, pattern, self, slash)) return aocai;
                WrappedCard jink = new WrappedCard(Jink.ClassName);
                if (Engine.MatchExpPattern(room, pattern, self, jink)) return aocai;
                WrappedCard ana = new WrappedCard(Analeptic.ClassName);
                if (Engine.MatchExpPattern(room, pattern, self, ana)) return aocai;
                WrappedCard peach = new WrappedCard(Peach.ClassName);
                if (Engine.MatchExpPattern(room, pattern, self, peach)) return aocai;
            }

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
            WrappedCard result = null;

            if (self != dying)
            {
                if ((HasSkill("niepan", dying) && dying.GetMark("@nirvana") > 0) || (HasSkill("fuli", dying) && dying.GetMark("@fuli") > 0)
                    || (HasSkill("jizhao", dying) && dying.GetMark("@jizhao") > 0)) return null;
                if (HasSkill("buqu|buqu_jx", dying) && dying.GetPile("buqu").Count <= 4 && room.GetFront(self, dying) == self)
                    return null;

                bool will_save = false;
                if (dying.GetRoleEnum() == PlayerRole.Lord)
                {
                    if (self.GetRoleEnum() == PlayerRole.Loyalist)              //忠臣一定会救主公
                    {
                        will_save = true;
                    }
                    else if (self.GetRoleEnum() == PlayerRole.Renegade && (roles[PlayerRole.Loyalist] > 0 || roles[PlayerRole.Rebel] > 0))      //内奸不会优先用自己的桃子
                    {
                        int count = 0;
                        List<WrappedCard> analeptics = GetCards(Analeptic.ClassName, dying);
                        foreach (WrappedCard card in analeptics)
                            if (!RoomLogic.IsCardLimited(room, dying, card, HandlingMethod.MethodUse) && RoomLogic.IsProhibited(room, dying, dying, card) == null)
                                count++;

                        foreach (Player p in GetFriends(dying))
                        {
                            if (self == p) continue;
                            List<WrappedCard> peach = GetCards(Peach.ClassName, p);
                            foreach (WrappedCard card in peach)
                                if (!RoomLogic.IsCardLimited(room, p, card, HandlingMethod.MethodUse) && RoomLogic.IsProhibited(room, p, dying, card) == null)
                                    count++;
                        }

                        if (count >= 1 - dying.Hp) return null;
                        will_save = true;
                    }
                }

                if (HasSkill("yechou", dying) && GetPlayerTendency(dying) == "rebel" && (GetRolePitts(PlayerRole.Rebel) > 1 || GetRolePitts(PlayerRole.Renegade) > 0))
                {
                    Player lord = null;
                    foreach (Player p in room.GetAlivePlayers())
                    {
                        if (p.GetRoleEnum() == PlayerRole.Lord && p.GetLostHp() > 1)
                        {
                            lord = p;
                            break;
                        }
                    }

                    if (lord != null)
                    {
                        int count = 1;
                        Player next = room.GetNextAlive(room.Current, 1, false);
                        while (next != lord)
                        {
                            if (next.FaceUp && next != dying)
                                count++;

                            next = room.GetNextAlive(next, 1, false);
                        }

                        if (count > lord.Hp)
                        {
                            switch (Self.GetRoleEnum())
                            {
                                case PlayerRole.Lord:
                                case PlayerRole.Loyalist:
                                case PlayerRole.Renegade:
                                    will_save = CanSave(dying, 1 - dying.Hp);
                                    break;
                                case PlayerRole.Rebel:
                                    return null;
                            }
                        }
                    }
                }

                if (!will_save && IsFriend(dying) && CanSave(dying, 1 - dying.Hp))
                {
                    if (dying.IsNude() && !MaySave(dying) && !HasSkill(MasochismSkill, dying)) return null;
                    if (dying_struct.Damage.From != null && IsFriend(dying_struct.Damage.From) && GetPlayerTendency(dying) == "rebel" && dying.GetCardCount(true) < 2)  //反贼杀反贼牌少的也不救
                        return null;

                    will_save = true;
                }

                if (will_save)
                {
                    WrappedCard ana = new WrappedCard(Analeptic.ClassName);
                    if (HasSkill("chunlao") && Analeptic.Instance.IsAvailable(room, dying, ana))                            //醇醪要在这里判断
                    {
                        List<int> ids = self.GetPile("chun");
                        if (ids.Count > 0)
                        {
                            result = new WrappedCard(ChunlaoCard.ClassName) { Skill = "chunlao", Mute = true, UserString = dying.Name };
                            result.AddSubCard(ids[0]);
                        }
                    }

                    if (result == null)
                    {
                        List<WrappedCard> peaches = GetCards(Peach.ClassName, self);
                        foreach (WrappedCard card in peaches)
                        {
                            if (f_peach.IsAvailable(room, self, card) && Engine.IsProhibited(room, self, dying, card) == null)
                            {
                                result = card;
                                break;
                            }
                        }
                    }
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

                if (result == null)
                {
                    WrappedCard ana = new WrappedCard(Analeptic.ClassName);
                    if (HasSkill("chunlao") && Analeptic.Instance.IsAvailable(room, dying, ana))                            //醇醪要在这里判断
                    {
                        List<int> ids = self.GetPile("chun");
                        if (ids.Count > 0)
                        {
                            result = new WrappedCard(ChunlaoCard.ClassName) { Skill = "chunlao", Mute = true, UserString = dying.Name };
                            result.AddSubCard(ids[0]);
                        }
                    }
                }
            }

            return result;
        }

        public override string AskForChoice(string skill_name, string choice, object data)
        {
            //主公选将
            if (skill_name == "gamerule" && self.GetRoleEnum() == PlayerRole.Lord)
            {
                List<string> choices = new List<string>(choice.Split('+'));
                return GeneralSelector.GetGeneral(room, choices, self.GetRoleEnum(), self);
            }
            //神将选国籍
            if (skill_name == "Kingdom" && self.GetRoleEnum() == PlayerRole.Lord)
            {
                List<string> choices = new List<string>(choice.Split('+'));
                Shuffle.shuffle(ref choices);
                return choices[0];
            }

            if (skill_name == "GameRule:TriggerOrder")
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
            if (skill_name == "userdefine:changetolord")
                return Shuffle.random(2, 3);

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

            List<double> values = SortByKeepValue(ref ids, false);
            for (int i = 0; i < min_num; i++)
                result.Add(ids[i]);

            if (result.Count < discard_num)
            {
                for (int i = result.Count - 1; i < Math.Min(discard_num, ids.Count); i++)
                {
                    if (values[i] < 0)
                        result.Add(ids[i]);
                    else
                        break;
                }
            }

            if (reason == "gamerule" && HasSkill("yinbing"))
            {
                bool trick = false;
                List<int> rest = new List<int>(self.GetCards("h"));
                rest.RemoveAll(t => result.Contains(t));
                foreach (int id in rest)
                {
                    WrappedCard card = room.GetCard(id);
                    if (Engine.GetFunctionCard(card.Name).TypeID != CardType.TypeBasic && card.Name != Nullification.ClassName)
                    {
                        trick = true;
                        break;
                    }
                }

                if (!trick && rest.Count > 2)
                {
                    int keep = -1;
                    foreach (int id in result)
                    {
                        WrappedCard card = room.GetCard(id);
                        if (Engine.GetFunctionCard(card.Name).TypeID != CardType.TypeBasic)
                        {
                            keep = id;
                            break;
                        }
                    }

                    if (keep > -1)
                    {
                        foreach (int id in ids)
                        {
                            if (!result.Contains(id))
                            {
                                result.Add(id);
                                result.Remove(keep);
                                break;
                            }
                        }
                    }
                }
            }

            return result;
        }

        public override AskForMoveCardsStruct AskForMoveCards(List<int> upcards, List<int> downcards, string reason, int min_num, int max_num)
        {
            SkillEvent e = Engine.GetSkillEvent(reason);
            if (e != null)
                return e.OnMoveCards(this, self, new List<int>(upcards), new List<int>(downcards), min_num, max_num);

            return base.AskForMoveCards(upcards, downcards, reason, min_num, max_num);
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
            { "@tiaoxin_jx-slash", "tiaoxin_jx" },
            { "@luanwu-slash", "luanwu" },
            { "@kill_victim", BeltsheChao.ClassName },
            { "@kangkai-use", "kangkai" },
            { "@lianji-slash", "lianji" },
            { "@jianji", "jianji" },
            { "@zhongyong-slash", "zhongyong" },
            { "@zhuhai-slash", "zhuhai" },
            { "@sheque-slash", "sheque" },
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
                        //左慈技能的复原
                        ZuociReturn(ref use);
                        return use;
                    }

                    SkillEvent skill = Engine.GetSkillEvent(skill_name);
                    if (skill != null)
                    {
                        CardUseStruct use = skill.OnResponding(this, self, pattern, prompt, method);
                        //左慈技能的复原
                        ZuociReturn(ref use);

                        return use;
                    }
                }
            }
            else
            {
                if (HasSkill("aocai") && Self.Phase == PlayerPhase.NotActive && self.GetPile("#aocai").Count == 0           //傲才耦合
                    && (room.GetRoomState().GetCurrentCardUseReason() == CardUseStruct.CardUseReason.CARD_USE_REASON_RESPONSE
                    || room.GetRoomState().GetCurrentCardUseReason() == CardUseStruct.CardUseReason.CARD_USE_REASON_RESPONSE_USE))
                {
                    WrappedCard aocai = new WrappedCard(AocaiCard.ClassName) { Skill = "aocai" };
                    WrappedCard slash = new WrappedCard(Slash.ClassName);
                    if (Engine.MatchExpPattern(room, pattern, self, slash)) return new CardUseStruct(aocai, self, new List<Player>());
                    WrappedCard jink = new WrappedCard(Jink.ClassName);
                    if (Engine.MatchExpPattern(room, pattern, self, jink)) return new CardUseStruct(aocai, self, new List<Player>());
                    WrappedCard ana = new WrappedCard(Analeptic.ClassName);
                    if (Engine.MatchExpPattern(room, pattern, self, ana)) return new CardUseStruct(aocai, self, new List<Player>());
                    WrappedCard peach = new WrappedCard(Peach.ClassName);
                    if (Engine.MatchExpPattern(room, pattern, self, peach)) return new CardUseStruct(aocai, self, new List<Player>());
                }

                if (!string.IsNullOrEmpty(room.GetRoomState().GetCurrentResponseSkill()))
                {
                    string skill_name = room.GetRoomState().GetCurrentResponseSkill();
                    UseCard card = Engine.GetCardUsage(skill_name);
                    if (card != null)
                    {
                        CardUseStruct use = card.OnResponding(this, self, pattern, prompt, method);
                        //左慈技能的复原
                        ZuociReturn(ref use);
                        return use;
                    }

                    SkillEvent skill = Engine.GetSkillEvent(skill_name);
                    if (skill != null)
                    {
                        CardUseStruct use = skill.OnResponding(this, self, pattern, prompt, method);
                        //左慈技能的复原
                        ZuociReturn(ref use);
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
                            //左慈技能的复原
                            ZuociReturn(ref use);
                            return use;
                        }

                        SkillEvent skill = Engine.GetSkillEvent(skill_name);
                        if (skill != null)
                        {
                            CardUseStruct use = skill.OnResponding(this, self, pattern, prompt, method);
                            //左慈技能的复原
                            ZuociReturn(ref use);
                            return use;
                        }
                    }
                }
            }

            return base.AskForUseCard(pattern, prompt, method);
        }

        private void ZuociReturn(ref CardUseStruct use)
        {
            if (use.Card != null && use.Card.Skill == "yigui")
            {
                use.Card = new WrappedCard(YiguiCard.ClassName)
                {
                    Skill = "yigui",
                    UserString = string.Format("{0}_{1}", use.Card.Name, use.Card.UserString)
                };
            }
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
            /*
            UseCard u = Engine.GetCardUsage(reason);
            if (u != null)
            {
                List<int> result
                if (result != null)
                    return result;
            }
            */
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

            if (real.From != null && HasSkill("funan", real.From) && !IsFriend(real.From) && (real.From.GetMark("funan") > 0 || room.GetSubCards(real.Card).Count > 0))
                return null;

            List<WrappedCard> nullcards = GetCards(Nullification.ClassName, self);
            if (nullcards.Count == 0)
                return null;

            if (trick.Name == SavageAssault.ClassName && IsFriend(to) && positive)
            {
                Player menghuo = FindPlayerBySkill("huoshou");
                if (menghuo != null && RoomLogic.PlayerHasShownSkill(room, menghuo, "huoshou") && IsFriend(to, menghuo) && HasSkill("zhiman|zhiman_jx", menghuo))
                    return null;
            }

            if (from != null && IsFriend(to, from) && IsFriend(to) && positive && HasSkill("zhiman|zhiman_jx"))
                return null;

            int null_num = nullcards.Count;
            SortByUseValue(ref nullcards);
            WrappedCard null_card = null;
            foreach (WrappedCard c in nullcards)
                if (!RoomLogic.IsCardLimited(room, self, c, HandlingMethod.MethodUse))
                    null_card = c;

            if (null_card == null) return null;

            FunctionCard fcard = Engine.GetFunctionCard(trick.Name);
            if (HasSkill("kongcheng|kongcheng_jx") && self.IsLastHandCard(null_card) && fcard is SingleTargetTrick)
            {
                //bool heg = (int)room.GetTag("NullifyingTimes") == 0 && null_card.Name == HegNullification.ClassName || (bool)room.GetTag("HegNullificationValid");
                if (positive && IsFriend(to) && IsEnemy(from))
                {
                    return null_card;
                }
                else if (!positive && IsFriend(from))
                {
                    return null_card;
                }
            }


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
            if (RoomLogic.IsCardLimited(room, self, null_card, HandlingMethod.MethodUse)) return null;

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
    }
}