using CommonClass;
using CommonClass.Game;
using CommonClassLibrary;
using SanguoshaServer.Game;
using System;
using System.Collections.Generic;
using static CommonClass.Game.Player;

namespace SanguoshaServer.Package
{
    public class ClassicSpecial : GeneralPackage
    {
        public ClassicSpecial() : base("ClassicSpecial")
        {
            skills = new List<Skill>
            {
                new Shefu(),
                new ShefuClear(),
                new Benyu(),
                new Mashu("guanyu_sp"),
                new Nuzhan(),
                new NuzhanTM(),
                new Danji(),
                new Yuanhu(),
                new Bifa(),
                new Songci(),
                new Xianfu(),
                new Chouce(),

                new Shenxian(),
                new Qiangwu(),
                new QiangwuTar(),
                new Xueji(),
                new Huxiao(),
                new HuxiaoTar(),
                new Wuji(),
                new Liangzhu(),
                new Fanxiang(),

                new Hongyuan(),
                new Huanshi(),
                new Mingzhe(),
                new Aocai(),
                new Duwu(),
            };

            skill_cards = new List<FunctionCard>
            {
                new ShefuCard(),
                new BenyuCard(),
                new BifaCard(),
                new SongciCard(),
                new QiangwuCard(),
                new XuejiCard(),
                new AocaiCard(),
                new DuwuCard(),
            };

            related_skills = new Dictionary<string, List<string>>
            {
                { "shefu", new List<string>{ "#shefu-clear"} },
                { "nuzhan", new List<string>{ "#nuzhan" } },
                { "qiangwu", new List<string>{ "#qiangwu-tar" } },
                { "huxiao", new List<string>{ "#huxiao-tar" } },
            };
        }
    }

    public class Shefu : TriggerSkill
    {
        public Shefu() : base("shefu")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardUsed, TriggerEvent.EventPhaseStart, TriggerEvent.JinkEffect };
            view_as_skill = new ShefuVS();
            skill_type = SkillType.Wizzard;
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == Player.PlayerPhase.Finish && base.Triggerable(player, room)
                && !player.IsKongcheng() && ShefuVS.GuhuoCards(room, player).Count > 0)
            {
                triggers.Add(new TriggerStruct(Name, player));
            }
            else if (triggerEvent == TriggerEvent.CardUsed && data is CardUseStruct use && use.Card != null && use.To.Count > 0 && use.IsHandcard)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is SkillCard) return triggers;

                List<Player> chengyus = RoomLogic.FindPlayersBySkillName(room, Name);
                string card_name = fcard.Name;
                if (fcard is Slash) card_name = Slash.ClassName;
                foreach (Player p in chengyus)
                {
                    if (p != player && p.Phase == Player.PlayerPhase.NotActive && p.ContainsTag(string.Format("shefu_{0}", card_name))
                        && p.GetTag(string.Format("shefu_{0}", card_name)) is int id && p.GetPile("ambush").Contains(id))
                        triggers.Add(new TriggerStruct(Name, p));
                }
            }
            else if (triggerEvent == TriggerEvent.JinkEffect && data is CardResponseStruct resp && resp.Handcard)
            {
                List<Player> chengyus = RoomLogic.FindPlayersBySkillName(room, Name);
                foreach (Player p in chengyus)
                {
                    if (p != player && p.Phase == Player.PlayerPhase.NotActive && p.ContainsTag("shefu_Jink")
                        && p.GetTag("shefu_Jink") is int id && p.GetPile("ambush").Contains(id))
                        triggers.Add(new TriggerStruct(Name, p));
                }
            }

            return triggers;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player p, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart)
                room.AskForUseCard(player, "@@shefu", "@shefu", -1, FunctionCard.HandlingMethod.MethodUse, true, info.SkillPosition);
            else
            {
                string card_name;
                if (triggerEvent == TriggerEvent.CardUsed && data is CardUseStruct use)
                {
                    if (use.To.Count == 0) return new TriggerStruct();
                    FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                    card_name = fcard.Name;
                    if (fcard is Slash) card_name = Slash.ClassName;
                }
                else
                    card_name = Jink.ClassName;

                string key = string.Format("shefu_{0}", card_name);
                if (p.ContainsTag(key) && p.GetTag(key) is int id && p.GetPile("ambush").Contains(id))
                {
                    room.SetTag("shefu_data", data);
                    List<int> ids = room.AskForExchange(p, Name, 1, 0, string.Format("@shefu-cancel:::{0}", card_name),
                        "ambush", string.Format("{0}|.|.|ambush", id.ToString()), info.SkillPosition);
                    room.RemoveTag("shefu_data");
                    if (ids.Count == 1)
                    {
                        p.RemoveTag(key);
                        GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, p, Name, info.SkillPosition);
                        CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_REMOVE_FROM_PILE, p.Name, string.Empty, Name, string.Empty)
                        {
                            General = gsk
                        };
                        room.MoveCardTo(room.GetCard(id), p, null, Player.Place.DiscardPile, string.Empty, reason, true);

                        room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);
                        return info;
                    }
                }
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.CardUsed && data is CardUseStruct use)
            {
                LogMessage log = new LogMessage
                {
                    Type = "#ShefuEffect",
                    From = ask_who.Name,
                    To = new List<string> { player.Name },
                    Arg = Name,
                    Arg2 = use.Card.Name
                };
                room.SendLog(log);

                List<Player> targets = new List<Player>(use.To);
                foreach (Player p in targets)
                    room.CancelTarget(ref use, p);

                data = use;
            }
            else
            {
                LogMessage log = new LogMessage
                {
                    Type = "#ShefuEffect",
                    From = ask_who.Name,
                    To = new List<string> { player.Name },
                    Arg = Name,
                    Arg2 = Jink.ClassName
                };
                room.SendLog(log);
                return true;
            }

            return false;
        }
    }

    public class ShefuClear : DetachEffectSkill
    {
        public ShefuClear() : base("shefu", "ambush") { }
    }

    public class ShefuVS : ViewAsSkill
    {
        public ShefuVS() : base("shefu")
        {
            response_pattern = "@@shefu";
        }

        public override GuhuoType GetGuhuoType() => GuhuoType.PopUpBox;

        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return selected.Count == 0 && room.GetCardPlace(to_select.Id) == Player.Place.PlaceHand;
        }
        public override List<WrappedCard> GetGuhuoCards(Room room, List<WrappedCard> cards, Player player)
        {
            List<WrappedCard> result = new List<WrappedCard>();
            foreach (string name in GuhuoCards(room, player))
            {
                FunctionCard fcard = Engine.GetFunctionCard(name);
                if (fcard is Slash && name != Slash.ClassName) continue;
                if (player.ContainsTag(string.Format("shefu_{0}", name))) continue;

                WrappedCard card = new WrappedCard(name);
                card.AddSubCards(cards);
                result.Add(card);
            }

            return result;
        }

        public static List<string> GuhuoCards(Room room, Player player)
        {
            List<string> guhuos = GetGuhuoCards(room, "btd");
            List<string> result = new List<string>();
            foreach (string name in guhuos)
            {
                FunctionCard fcard = Engine.GetFunctionCard(name);
                if (fcard is Slash && name != Slash.ClassName) continue;
                if (fcard is Nullification && name != Nullification.ClassName) continue;
                if (player.ContainsTag(string.Format("shefu_{0}", name))) continue;

                result.Add(name);
            }

            return result;
        }

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count == 1 && cards[0].SubCards.Count == 1)
            {
                WrappedCard shefu = new WrappedCard(ShefuCard.ClassName) { Skill = Name, UserString = cards[0].Name };
                shefu.AddSubCards(cards);
                return shefu;
            }

            return null;
        }
    }

    public class ShefuCard : SkillCard
    {
        public static string ClassName = "ShefuCard";
        public ShefuCard() : base(ClassName)
        {
            target_fixed = true;
            will_throw = false;
        }

        public override void OnUse(Room room, CardUseStruct card_use)
        {
            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, card_use.From, "shefu", card_use.Card.SkillPosition);
            room.BroadcastSkillInvoke("shefu", "male", 1, gsk.General, gsk.SkinId);

            int id = card_use.Card.GetEffectiveId();
            string card_name = card_use.Card.UserString;
            room.AddToPile(card_use.From, "ambush", id, false);
            card_use.From.SetTag(string.Format("shefu_{0}", card_name), id);

            LogMessage log = new LogMessage
            {
                Type = "$ShefuRecord",
                From = card_use.From.Name,
                Card_str = id.ToString(),
                Arg = card_name
            };
            room.SendLog(log, card_use.From);
        }
    }

    public class BenyuCard : SkillCard
    {
        public static string ClassName = "BenyuCard";
        public BenyuCard() : base(ClassName)
        {
            will_throw = true;
            target_fixed = true;
        }

        public override void OnUse(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            List<int> ids = new List<int>(card_use.Card.SubCards);
            CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_DISCARD, player.Name, "benyu", string.Empty)
            {
                General = RoomLogic.GetGeneralSkin(room, player, "benyu", card_use.Card.SkillPosition)
            };
            room.ThrowCard(ref ids, reason, player, null, "benyu");
        }
    }

    public class BenyuVS : ViewAsSkill
    {
        public BenyuVS() : base("benyu")
        {
            response_pattern = "@@benyu";
        }
        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return !player.HasEquip(to_select.Name) && RoomLogic.CanDiscard(room, player, player, to_select.Id);
        }
        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            DamageStruct damage = (DamageStruct)room.GetTag(Name);
            int count = damage.From.HandcardNum;
            if (cards.Count > count)
            {
                WrappedCard dummy = new WrappedCard(BenyuCard.ClassName) { Skill = Name };
                dummy.AddSubCards(cards);
                return dummy;
            }

            return null;
        }
    }


    public class Benyu : MasochismSkill
    {
        public Benyu() : base("benyu")
        {
            view_as_skill = new BenyuVS();
            skill_type = SkillType.Defense;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is DamageStruct damage && damage.From != null && damage.From.Alive)
            {
                if (damage.From.HandcardNum > player.HandcardNum && player.HandcardNum < 5)
                    return new TriggerStruct(Name, player);
                else if (player.HandcardNum > damage.From.HandcardNum)
                {
                    int count = 0;
                    foreach (int id in player.GetCards("h"))
                        if (RoomLogic.CanDiscard(room, player, player, id)) count++;
                    if (count > damage.From.HandcardNum)
                        return new TriggerStruct(Name, player);
                }
            }

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is DamageStruct damage)
            {
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                bool invoke = false;
                if (damage.From.HandcardNum > player.HandcardNum && player.HandcardNum < 5)
                {
                    if (room.AskForSkillInvoke(player, Name, "@benyu-draw", info.SkillPosition))
                    {
                        room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                        player.SetTag("benyu_type", true);
                        invoke = true;
                    }
                }
                else if (player.HandcardNum > damage.From.HandcardNum)
                {
                    int count = 0;
                    foreach (int id in player.GetCards("h"))
                        if (RoomLogic.CanDiscard(room, player, player, id)) count++;
                    if (count > damage.From.HandcardNum)
                    {
                        room.SetTag(Name, data);
                        WrappedCard card = room.AskForUseCard(player, "@@benyu", string.Format("@benyu::{0}:{1}", damage.From.Name, damage.From.HandcardNum + 1),
                            -1, FunctionCard.HandlingMethod.MethodUse, true, info.SkillPosition);
                        room.RemoveTag(Name);

                        if (card != null)
                        {
                            room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);
                            player.SetTag("benyu_type", false);
                            invoke = true;
                        }
                    }
                }
                if (invoke) return info;
            }

            return new TriggerStruct();
        }
        public override void OnDamaged(Room room, Player target, DamageStruct damage, TriggerStruct info)
        {
            bool type = (bool)target.GetTag("benyu_type");
            target.RemoveTag("benyu_type");
            if (type)
            {
                int count = Math.Min(5 - target.HandcardNum, damage.From.HandcardNum - target.HandcardNum);
                if (count > 0)
                    room.DrawCards(target, count, Name);
            }
            else if (damage.From.Alive)
                room.Damage(new DamageStruct(Name, target, damage.From));
        }
    }

    //sp关羽
    public class Danji : PhaseChangeSkill
    {
        public Danji() : base("danji")
        {
            frequency = Frequency.Wake;
            skill_type = SkillType.Attack;
        }
        public override bool Triggerable(Player player, Room room)
        {
            foreach (Player p in room.GetAlivePlayers())
                if (p.GetRoleEnum() == PlayerRole.Lord && p.Name.Contains("liubei")) return false;

            return base.Triggerable(player, room) && player.Phase == PlayerPhase.Start
                    && player.HandcardNum > player.Hp && player.GetMark(Name) == 0;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SetPlayerMark(player, Name, 1);
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            room.DoSuperLightbox(player, info.SkillPosition, Name);
            return info;
        }
        public override bool OnPhaseChange(Room room, Player player, TriggerStruct info)
        {
            room.LoseMaxHp(player);
            List<string> skills = new List<string>();
            if (info.SkillPosition == "head")
            {
                skills.Add("mashu_guanyu_sp");
                skills.Add("nuzhan");
            }
            else
            {
                skills.Add("mashu_guanyu_sp!");
                skills.Add("nuzhan!");
            }
            room.HandleAcquireDetachSkills(player, skills);

            return false;
        }
    }

    public class Nuzhan : TriggerSkill
    {
        public Nuzhan() : base("nuzhan")
        {
            events.Add(TriggerEvent.ConfirmDamage);
            frequency = Frequency.Compulsory;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is DamageStruct damage && damage.Card != null && !damage.Chain && !damage.Transfer && damage.ByUser)
            {
                FunctionCard fcard = Engine.GetFunctionCard(damage.Card.Name);
                if (fcard is Slash && RoomLogic.IsVirtualCard(room, damage.Card) && damage.Card.SubCards.Count == 1)
                {
                    if (Engine.GetFunctionCard(room.GetCard(damage.Card.GetEffectiveId()).Name) is EquipCard)
                        return new TriggerStruct(Name, player);
                }
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
            room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);
            DamageStruct damage = (DamageStruct)data;
            LogMessage log = new LogMessage
            {
                Type = "#NuzhanBuff",
                From = player.Name,
                To = new List<string> { damage.To.Name },
                Arg = damage.Damage.ToString(),
                Arg2 = (++damage.Damage).ToString()
            };
            room.SendLog(log);

            data = damage;

            return false;
        }
    }

    public class NuzhanTM : TargetModSkill
    {
        public NuzhanTM() : base("#nuzhan")
        {
        }
        public override bool IgnoreCount(Room room, Player from, WrappedCard card)
        {
            if (RoomLogic.PlayerHasSkill(room, from, "nuzhan") && Engine.MatchExpPattern(room, pattern, from, card)
                    && RoomLogic.IsVirtualCard(room, card) && card.SubCards.Count == 1)
            {
                if (Engine.GetFunctionCard(room.GetCard(card.GetEffectiveId()).Name) is TrickCard)
                    return true;
            }

            return false;
        }
        public override void GetEffectIndex(Room room, Player player, WrappedCard card, ModType type, ref int index, ref string skill_name, ref string general_name, ref int skin_id)
        {
            if (type == ModType.History)
                index = 1;
            else
                index = -1;
        }
    }


    public class YuanhuViewAsSkill : OneCardViewAsSkill
    {
        public YuanhuViewAsSkill() : base("yuanhu")
        {
            filter_pattern = "EquipCard";
            response_pattern = "@@yuanhu";
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard first = new WrappedCard(HuyuanCard.ClassName) { Skill = Name, Mute = true };
            first.AddSubCard(card);
            return first;
        }
    }

    public class Yuanhu : PhaseChangeSkill
    {
        public Yuanhu() : base("yuanhu")
        {
            view_as_skill = new YuanhuViewAsSkill();
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            return base.Triggerable(player, room) && player.Phase == PlayerPhase.Finish && !player.IsNude()
                ? new TriggerStruct(Name, player)
                : new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            player.RemoveTag("huyuan_equip");
            player.RemoveTag("huyuan_target");
            WrappedCard card = room.AskForUseCard(player, "@@yuanhu", "@huyuan-equip", -1, FunctionCard.HandlingMethod.MethodNone, true, info.SkillPosition);
            if (card != null && player.ContainsTag("huyuan_target"))
            {
                player.SetMark(Name, card.GetEffectiveId());
                return info;
            }

            return new TriggerStruct();
        }
        public override bool OnPhaseChange(Room room, Player caohong, TriggerStruct info)
        {
            Player target = room.FindPlayer((string)caohong.GetTag("huyuan_target"));
            FunctionCard fcard = Engine.GetFunctionCard(room.GetCard(caohong.GetMark(Name)).Name);

            if (fcard != null)
            {
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, caohong, Name, info.SkillPosition);
                if (fcard is Weapon)
                {
                    room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                    List<Player> targets = new List<Player>();
                    foreach (Player p in room.GetAlivePlayers())
                    {
                        if (RoomLogic.DistanceTo(room, target, p) == 1 && RoomLogic.CanDiscard(room, caohong, p, "he"))
                            targets.Add(p);
                    }
                    if (targets.Count > 0)
                    {
                        Player to_dismantle = room.AskForPlayerChosen(caohong, targets, Name, "@huyuan-discard:" + target.Name, true, false, info.SkillPosition);
                        if (to_dismantle != null)
                        {
                            int card_id = room.AskForCardChosen(caohong, to_dismantle, "he", Name, false, FunctionCard.HandlingMethod.MethodDiscard);
                            room.ThrowCard(card_id, to_dismantle, caohong);
                        }
                    }
                }
                else if (fcard is Armor && target != null && target.Alive)
                {
                    room.BroadcastSkillInvoke(Name, "male", 2, gsk.General, gsk.SkinId);
                    room.DrawCards(target, new DrawCardStruct(1, caohong, Name));
                }
                else if (fcard is Horse && target != null && target.Alive && target.IsWounded())
                {
                    room.BroadcastSkillInvoke(Name, "male", 3, gsk.General, gsk.SkinId);
                    RecoverStruct recover = new RecoverStruct
                    {
                        Recover = 1,
                        Who = caohong
                    };
                    room.Recover(target, recover, true);
                }
            }

            return false;
        }
    }

    //陈琳
    public class BifaCard : SkillCard
    {
        public static string ClassName = "BifaCard";
        public BifaCard() : base(ClassName)
        {
            will_throw = false;
            handling_method = HandlingMethod.MethodNone;
        }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select.GetPile("bifa").Count == 0 && to_select != Self;
        }
        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            return targets.Count == 1 && card.SubCards.Count == 1;
        }
        public override void OnUse(Room room, CardUseStruct card_use)
        {
            Player target = card_use.To[0];
            card_use.From.SetTag("bifa_target", target.Name);
        }
    }
    public class BifaViewAsSkill : OneCardViewAsSkill
    {
        public BifaViewAsSkill() : base("bifa")
        {
            filter_pattern = ".";
            response_pattern = "@@bifa";
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard bf = new WrappedCard(BifaCard.ClassName) { Skill = Name };
            bf.AddSubCard(card);
            return bf;
        }
    }

    public class Bifa : TriggerSkill
    {
        public Bifa() : base("bifa")
        {
            events.Add(TriggerEvent.EventPhaseStart);
            view_as_skill = new BifaViewAsSkill();
            skill_type = SkillType.Attack;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (player.Phase == Player.PlayerPhase.RoundStart && player.GetPile("bifa").Count > 0)
            {
                int card_id = player.GetPile("bifa")[0];
                Player chenlin = room.FindPlayer(player.GetTag(string.Format("BifaSource{0}", card_id)).ToString());
                player.RemoveTag(string.Format("BifaSource{0}", card_id));
                List<int> ids = new List<int>
                {
                    card_id
                };

                LogMessage log = new LogMessage
                {
                    Type = "$BifaView",
                    From = player.Name,
                    Card_str = card_id.ToString(),
                    Arg = Name
                };
                room.SendLog(log, player);

                room.FillAG(Name, ids, player);
                FunctionCard cd = Engine.GetFunctionCard(room.GetCard(card_id).Name);
                string pattern = string.Empty;
                if (cd is BasicCard)
                    pattern = "BasicCard";
                else if (cd is TrickCard)
                    pattern = "TrickCard";
                else if (cd is EquipCard)
                    pattern = "EquipCard";
                pattern += "|.|.|hand";
                List<int> to_give = new List<int>();
                if (!player.IsKongcheng() && chenlin != null && chenlin.Alive)
                {
                    to_give = room.AskForExchange(player, Name, 1, 0, "@bifa-give:" + chenlin, string.Empty, pattern, string.Empty);
                    if (to_give.Count == 1)
                    {
                        CardMoveReason reasonG = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_GIVE, player.Name, chenlin.Name, Name, string.Empty);
                        room.ObtainCard(chenlin, ref to_give, reasonG, false);
                        CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_EXCHANGE_FROM_PILE, player.Name, Name, string.Empty);
                        List<int> card_ids = new List<int> { card_id };
                        room.ObtainCard(player, ref card_ids, reason, false);
                    }
                }

                if (to_give.Count == 0)
                {
                    CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_REMOVE_FROM_PILE, string.Empty, Name, string.Empty);
                    List<int> dis = player.GetPile("bifa");
                    room.ThrowCard(ref dis, reason, null);
                    room.LoseHp(player);
                }
                room.ClearAG(player);
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && player.Phase == Player.PlayerPhase.Finish && !player.IsKongcheng())
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            WrappedCard card = room.AskForUseCard(player, "@@bifa", "@bifa-remove", -1, FunctionCard.HandlingMethod.MethodNone, true, info.SkillPosition);
            if (card != null)
            {
                player.SetMark(Name, card.GetEffectiveId());
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player target = room.FindPlayer(player.GetTag("bifa_target").ToString());
            player.RemoveTag("bifa_target");
            if (target != null && target.Alive)
            {
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, target.Name);
                int id = player.GetMark(Name);
                target.SetTag(string.Format("BifaSource{0}", id), player.Name);
                room.AddToPile(target, Name, id, false);
            }

            return false;
        }
    }

    public class SongciCard : SkillCard
    {
        public static string ClassName = "SongciCard";
        public SongciCard() : base(ClassName)
        {
        }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select.GetMark(string.Format("songci_{0}", Self.Name)) == 0 && to_select.HandcardNum != to_select.Hp;
        }
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            int handcard_num = effect.To.HandcardNum;
            int hp = effect.To.Hp;
            room.SetPlayerMark(effect.To, "@songci", 1);
            effect.To.SetMark(string.Format("songci_{0}", effect.From.Name), 1);
            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, effect.From, "songci", effect.Card.SkillPosition);
            if (handcard_num > hp)
            {
                room.BroadcastSkillInvoke("songci", "male", 2, gsk.General, gsk.SkinId);
                room.AskForDiscard(effect.To, "songci", 2, 2, false, true);
            }
            else if (handcard_num < hp)
            {
                room.BroadcastSkillInvoke("songci", "male", 1, gsk.General, gsk.SkinId);
                room.DrawCards(effect.To, new DrawCardStruct(2, effect.From, "songci"));
            }
        }
    }

    public class Songci : ZeroCardViewAsSkill
    {
        public Songci() : base("songci")
        {
            skill_type = SkillType.Wizzard;
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            foreach (Player sib in room.GetAlivePlayers())
                if (sib.GetMark("songci_" + player.Name) == 0 && sib.HandcardNum != sib.Hp)
                    return true;
            return false;
        }
        public override WrappedCard ViewAs(Room room, Player player)
        {
            WrappedCard c = new WrappedCard(SongciCard.ClassName) { Skill = Name, Mute = true };
            return c;
        }
    }


    public class Xianfu : TriggerSkill
    {
        public Xianfu() : base("xianfu")
        {
            events = new List<TriggerEvent> { TriggerEvent.GameStart, TriggerEvent.Damaged, TriggerEvent.HpRecover };
            skill_type = SkillType.Wizzard;
            frequency = Frequency.Compulsory;
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.GameStart && base.Triggerable(player, room))
                triggers.Add(new TriggerStruct(Name, player));
            else if (triggerEvent == TriggerEvent.Damaged && player.ContainsTag(Name))
            {
                List<string> xizhicai = (List<string>)player.GetTag(Name);
                foreach (string name in xizhicai)
                {
                    Player p = room.FindPlayer(name);
                    if (p != null && base.Triggerable(p, room)) triggers.Add(new TriggerStruct(Name, p));
                }
            }
            else if (triggerEvent == TriggerEvent.HpRecover && player.ContainsTag(Name))
            {
                List<string> xizhicai = (List<string>)player.GetTag(Name);
                foreach (string name in xizhicai)
                {
                    Player p = room.FindPlayer(name);
                    if (p != null && base.Triggerable(p, room) && p.IsWounded()) triggers.Add(new TriggerStruct(Name, p));
                }
            }
            return triggers;
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Random ra = new Random();
            if (triggerEvent == TriggerEvent.GameStart)
            {
                Player target = room.AskForPlayerChosen(player, room.GetOtherPlayers(player), Name, "@xianfu-target", false, false, info.SkillPosition);
                List<string> froms = target.ContainsTag(Name) ? (List<string>)target.GetTag(Name) : new List<string>();
                if (!froms.Contains(player.Name))
                    froms.Add(player.Name);
                target.SetTag(Name, froms);

                List<string> arg = new List<string>
                {
                    target.Name,
                    "@fu",
                    "1"
                };
                room.DoNotify(room.GetClient(player), CommandType.S_COMMAND_SET_MARK, arg);

                room.NotifySkillInvoked(player, Name);
                GeneralSkin general = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                int result = ra.Next(1, 3);
                room.BroadcastSkillInvoke(Name, "male", result, general.General, general.SkinId);
            }
            else if (triggerEvent == TriggerEvent.Damaged && data is DamageStruct damage && ask_who.Alive)
            {
                if (player.GetMark("@fu") == 0)
                    room.SetPlayerMark(player, "@fu", 1);

                room.NotifySkillInvoked(ask_who, Name);
                GeneralSkin general = RoomLogic.GetGeneralSkin(room, ask_who, Name, info.SkillPosition);
                int result = ra.Next(3, 5);
                room.BroadcastSkillInvoke(Name, "male", result, general.General, general.SkinId);

                room.Damage(new DamageStruct(Name, null, ask_who, damage.Damage));
            }
            else if (triggerEvent == TriggerEvent.HpRecover && data is RecoverStruct recover && ask_who.IsWounded())
            {
                room.NotifySkillInvoked(ask_who, Name);
                GeneralSkin general = RoomLogic.GetGeneralSkin(room, ask_who, Name, info.SkillPosition);
                int result = ra.Next(5, 7);
                room.BroadcastSkillInvoke(Name, "male", result, general.General, general.SkinId);

                RecoverStruct _recover = new RecoverStruct
                {
                    Recover = recover.Recover,
                    Who = player
                };
                room.Recover(ask_who, _recover, true);
            }
            return false;
        }
    }

    public class Chouce : TriggerSkill
    {
        public Chouce() : base("chouce")
        {
            events = new List<TriggerEvent> { TriggerEvent.Damaged };
            skill_type = SkillType.Masochism;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is DamageStruct damage)
            {
                TriggerStruct trigger = new TriggerStruct(Name, player)
                {
                    Times = damage.Damage
                };
                return trigger;
            }
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(player, Name, data, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            JudgeStruct judge = new JudgeStruct
            {
                Good = true,
                PlayAnimation = false,
                Who = player,
                Reason = Name
            };
            room.Judge(ref judge);

            if (WrappedCard.IsBlack(judge.Card.Suit))
            {
                List<Player> targets = new List<Player>();
                foreach (Player p in room.GetAlivePlayers())
                    if (RoomLogic.CanDiscard(room, player, p, "hej"))
                        targets.Add(p);

                if (targets.Count > 0)
                {
                    Player target = room.AskForPlayerChosen(player, targets, Name, "@chouce-discard", false, true, info.SkillPosition);
                    int id = room.AskForCardChosen(player, target, "hej", Name, false, FunctionCard.HandlingMethod.MethodDiscard);
                    room.ThrowCard(id, target, player != target ? player : null);
                }
            }
            else
            {
                Player target = room.AskForPlayerChosen(player, room.GetAlivePlayers(), Name, "@chouce-draw", false, true, info.SkillPosition);
                DrawCardStruct draw = new DrawCardStruct(1, player, Name);
                if (target.ContainsTag("xianfu") && target.GetTag("xianfu") is List<string> names && names.Contains(player.Name))
                {
                    if (target.GetMark("@fu") == 0)
                        room.SetPlayerMark(target, "@fu", 1);
                    draw.Draw = 2;
                }

                room.DrawCards(target, draw);
            }

            return false;
        }
    }

    public class Shenxian : TriggerSkill
    {
        public Shenxian() : base("shenxian")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseChanging, TriggerEvent.CardsMoveOneTime };
            skill_type = SkillType.Replenish;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive)
                foreach (Player p in room.GetAlivePlayers())
                    p.SetFlags(string.Format("-{0}", Name));
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.From != null && move.From_places.Contains(Place.PlaceHand)
                && move.To_place == Place.PlaceTable && (move.Reason.Reason & CardMoveReason.MoveReason.S_MASK_BASIC_REASON) == CardMoveReason.MoveReason.S_REASON_DISCARD)
            {
                bool basic = false;
                foreach (int id in move.Card_ids)
                {
                    if (Engine.GetFunctionCard(room.GetCard(id).Name) is BasicCard)
                    {
                        basic = true;
                        break;
                    }
                }
                if (basic)
                {
                    List<Player> xincai = RoomLogic.FindPlayersBySkillName(room, Name);
                    foreach (Player p in xincai)
                        if (move.From != p && !p.HasFlag(Name))
                            triggers.Add(new TriggerStruct(Name, p));
                }
            }

            return triggers;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(ask_who, Name, null, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            ask_who.SetFlags(Name);
            room.DrawCards(ask_who, 1, Name);
            return false;
        }
    }

    public class Qiangwu : TriggerSkill
    {
        public Qiangwu() : base("qiangwu")
        {
            events.Add(TriggerEvent.EventPhaseChanging);
            skill_type = SkillType.Attack;
            view_as_skill = new QiangwuVS();
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive && player.GetMark(Name) > 0)
                player.SetMark(Name, 0);
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }
    public class QiangwuVS : ZeroCardViewAsSkill
    {
        public QiangwuVS() : base("qiangwu")
        {
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(QiangwuCard.ClassName);
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            WrappedCard card = new WrappedCard(QiangwuCard.ClassName)
            {
                Skill = Name
            };
            return card;
        }
    }

    public class QiangwuCard : SkillCard
    {
        public static string ClassName = "QiangwuCard";
        public QiangwuCard() : base(ClassName)
        {
            target_fixed = true;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            JudgeStruct judge = new JudgeStruct
            {
                Good = true,
                PlayAnimation = false,
                Who = player,
                Reason = "qiangwu"
            };

            room.Judge(ref judge);

            player.SetMark("qiangwu", judge.Card.Number);
        }
    }

    public class QiangwuTar : TargetModSkill
    {
        public QiangwuTar() : base("#qiangwu-tar", false) { }

        public override bool IgnoreCount(Room room, Player from, WrappedCard card)
        {
            return from.GetMark("qiangwu") > 0 && RoomLogic.GetCardNumber(room, card) > from.GetMark("qiangwu");
        }

        public override bool GetDistanceLimit(Room room, Player from, Player to, WrappedCard card)
        {
            return from.GetMark("qiangwu") > 0 && RoomLogic.GetCardNumber(room, card) < from.GetMark("qiangwu");
        }
    }

    public class Xueji : OneCardViewAsSkill
    {
        public Xueji() : base("xueji")
        {
            filter_pattern = ".|red";
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.IsNude() && !player.HasUsed(XuejiCard.ClassName);
        }

        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard xj = new WrappedCard(XuejiCard.ClassName)
            {
                Skill = Name
            };
            xj.AddSubCard(card);
            return xj;
        }
    }

    public class XuejiCard : SkillCard
    {
        public static string ClassName = "XuejiCard";
        public XuejiCard() : base(ClassName)
        {
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            int count = Math.Max(1, Self.GetLostHp());
            return targets.Count < count && RoomLogic.InMyAttackRange(room, Self, to_select, card);
        }

        public override void OnUse(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            List<Player> targets = new List<Player>(card_use.To);
            room.SortByActionOrder(ref targets);

            bool hidden = (TypeID == CardType.TypeSkill && !WillThrow);
            LogMessage log = new LogMessage("#UseCard")
            {
                From = player.Name,
                To = new List<string>(),
                Card_str = RoomLogic.CardToString(room, card_use.Card)
            };
            log.SetTos(targets);

            List<int> used_cards = new List<int>();
            List<CardsMoveStruct> moves = new List<CardsMoveStruct>();
            used_cards.AddRange(card_use.Card.SubCards);

            RoomThread thread = room.RoomThread;
            object data = card_use;
            thread.Trigger(TriggerEvent.PreCardUsed, room, player, ref data);

            card_use = (CardUseStruct)data;

            CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_THROW, card_use.From.Name, null,
                card_use.Card.Skill, null)
            {
                CardString = RoomLogic.CardToString(room, card_use.Card),
                General = RoomLogic.GetGeneralSkin(room, player, card_use.Card.Skill, card_use.Card.SkillPosition)
            };

            room.MoveCardTo(card_use.Card, card_use.From, null, Place.PlaceTable, reason, true);
            room.SendLog(log);

            List<int> table_cardids = room.GetCardIdsOnTable(card_use.Card);
            if (table_cardids.Count > 0)
            {
                CardsMoveStruct move = new CardsMoveStruct(table_cardids, player, null, Place.PlaceTable, Place.DiscardPile, reason);
                room.MoveCardsAtomic(new List<CardsMoveStruct> { move }, true);
            }

            room.RoomThread.Trigger(TriggerEvent.CardUsedAnnounced, room, player, ref data);
            room.RoomThread.Trigger(TriggerEvent.CardTargetAnnounced, room, player, ref data);

            room.RoomThread.Trigger(TriggerEvent.CardUsed, room, player, ref data);
            room.RoomThread.Trigger(TriggerEvent.CardFinished, room, player, ref data);
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            List<Player> targets = new List<Player>(card_use.To);
            foreach (Player p in targets)
                if (!p.Chained && RoomLogic.CanBeChainedBy(room, p, player))
                    room.SetPlayerChained(p, true);

            Player target = card_use.To[0];
            if (target.Alive)
                room.Damage(new DamageStruct("xeji", player, target, 1, DamageStruct.DamageNature.Fire));
        }
    }

    public class Huxiao : TriggerSkill
    {
        public Huxiao() : base("huxiao")
        {
            events = new List<TriggerEvent> { TriggerEvent.Damage };
            skill_type = SkillType.Attack;
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is DamageStruct damage && damage.To.Alive && base.Triggerable(player, room) && damage.Nature == DamageStruct.DamageNature.Fire)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name);
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            if (data is DamageStruct damage && damage.To.Alive)
            {
                room.DrawCards(damage.To, new DrawCardStruct(1, player, Name));
                if (room.Current == player)
                    player.SetFlags(string.Format("{0}_{1}", Name, damage.To.Name));
            }

            return false;
        }
    }

    public class HuxiaoTar : TargetModSkill
    {
        public HuxiaoTar() : base("#huxiao-tar", false)
        {
            pattern = ".Basic";
        }

        public override bool CheckSpecificAssignee(Room room, Player from, Player to, WrappedCard card)
        {
            return from != null && to != null && from.HasFlag(string.Format("huxiao_{0}", to.Name));
        }
    }

    public class Wuji : TriggerSkill
    {
        public Wuji() : base("wuji")
        {
            frequency = Frequency.Wake;
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart, TriggerEvent.Damage, TriggerEvent.EventPhaseChanging };
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.Damage && base.Triggerable(player, room) && player.GetMark(Name) == 0 && data is DamageStruct damage)
                player.AddMark("wuji_count", damage.Damage);
            else if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive && player.GetMark("wuji_count") > 0)
                player.AddMark("wuji_count", 0);
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Finish && player.GetMark(Name) == 0 && player.GetMark("wuji_count") >= 3)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            room.DoSuperLightbox(player, info.SkillPosition, Name);
            room.SetPlayerMark(player, Name, 1);

            player.MaxHp++;
            room.BroadcastProperty(player, "MaxHp");

            LogMessage log = new LogMessage
            {
                Type = "$GainMaxHp",
                From = player.Name,
                Arg = "1"
            };
            room.SendLog(log);

            room.RoomThread.Trigger(TriggerEvent.MaxHpChanged, room, player);
            RecoverStruct recover = new RecoverStruct
            {
                Who = player,
                Recover = 1
            };
            room.Recover(player, recover, true);

            room.HandleAcquireDetachSkills(player, "-huxiao");

            foreach (Player p in room.GetAlivePlayers())
            {
                if (p.Weapon.Value == ClassicBlade.ClassName)
                {
                    room.ObtainCard(player, p.Weapon.Key);
                    return false;
                }
            }
            foreach (int id in room.DrawPile)
            {
                if (room.GetCard(id).Name == ClassicBlade.ClassName)
                {
                    room.ObtainCard(player, id);
                    return false;
                }
            }
            foreach (int id in room.DiscardPile)
            {
                if (room.GetCard(id).Name == ClassicBlade.ClassName)
                {
                    room.ObtainCard(player, id);
                    break;
                }
            }

            return false;
        }
    }

    public class Liangzhu : TriggerSkill
    {
        public Liangzhu() : base("liangzhu")
        {
            events.Add(TriggerEvent.HpRecover);
            skill_type = SkillType.Replenish;
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            List<Player> ssx = RoomLogic.FindPlayersBySkillName(room, Name);
            foreach (Player p in ssx)
                triggers.Add(new TriggerStruct(Name, player));

            return triggers;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(ask_who, Name, data, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player target = null;
            if (player == ask_who)
            {
                target = player;
            }
            else
            {
                List<string> prompts = new List<string> { string.Empty, "@liangzhu-let:" + player.Name };
                string choice = room.AskForChoice(ask_who, Name, "let_draw+draw_self", prompts, data);
                if (choice == "let_draw")
                    target = player;
                else
                    target = ask_who;
            }

            room.DrawCards(target, new DrawCardStruct(target == player ? 2 : 1, ask_who, Name));
            if (target == player)
                target.SetMark(Name, 1);
            return false;
        }
    }

    public class Fanxiang : PhaseChangeSkill
    {
        public Fanxiang() : base("fanxiang")
        {
            frequency = Frequency.Wake;
            skill_type = SkillType.Recover;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player.Phase == PlayerPhase.Start && base.Triggerable(player, room) && player.GetMark(Name) == 0)
            {
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (p.GetMark("liangzhu") > 1 && p.IsWounded())
                        return new TriggerStruct(Name, player);
                }
            }

            return new TriggerStruct();
        }

        public override bool OnPhaseChange(Room room, Player player, TriggerStruct info)
        {
            room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
            room.DoSuperLightbox(player, info.SkillPosition, Name);
            room.SetPlayerMark(player, Name, 1);
            room.SendCompulsoryTriggerLog(player, Name);
            if (player.Alive)
            {
                player.MaxHp++;
                room.BroadcastProperty(player, "MaxHp");

                LogMessage log = new LogMessage
                {
                    Type = "$GainMaxHp",
                    From = player.Name,
                    Arg = "1"
                };
                room.SendLog(log);

                room.RoomThread.Trigger(TriggerEvent.MaxHpChanged, room, player);
                RecoverStruct recover = new RecoverStruct
                {
                    Who = player,
                    Recover = 1
                };
                room.Recover(player, recover, true);

                room.HandleAcquireDetachSkills(player, "-liangzhu|xiaoji", false);
            }

            return false;
        }
    }

    public class Hongyuan : DrawCardsSkill
    {
        public Hongyuan() : base("hongyuan")
        {
            skill_type = SkillType.Replenish;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<Player> targets = room.AskForPlayersChosen(player, room.GetOtherPlayers(player), Name, 0, 2, "@hongyuan", true, info.SkillPosition);
            if (targets.Count > 0 && data is int count)
            {
                count--;
                data = count;
                room.SetTag(Name, targets);
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }
        public override int GetDrawNum(Room room, Player player, int n)
        {
            List<Player> targets = (List<Player>)room.GetTag(Name);
            room.RemoveTag(Name);
            foreach (Player p in targets)
                room.DrawCards(p, new DrawCardStruct(1, player, Name));

            return n;
        }
    }

    public class Huanshi : TriggerSkill
    {
        public Huanshi() : base("huanshi")
        {
            events.Add(TriggerEvent.AskForRetrial);
            skill_type = SkillType.Wizzard;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && !player.IsKongcheng())
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is JudgeStruct judge && judge.Who.Alive && !player.IsKongcheng() && room.AskForSkillInvoke(player, Name, judge.Who, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                int id = room.AskForCardChosen(judge.Who, player, "he", Name, true, FunctionCard.HandlingMethod.MethodResponse);
                room.Retrial(room.GetCard(id), player, ref judge, Name, false, info.SkillPosition);
                data = judge;
                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            JudgeStruct judge = (JudgeStruct)data;
            room.UpdateJudgeResult(ref judge);
            data = judge;
            return false;
        }
    }

    public class Mingzhe : TriggerSkill
    {
        public Mingzhe() : base("mingzhe")
        {
            events.Add(TriggerEvent.CardsMoveOneTime);
            skill_type = SkillType.Replenish;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is CardsMoveOneTimeStruct move && move.From != null && (move.From_places.Contains(Place.PlaceEquip) || move.From_places.Contains(Place.PlaceHand))
                && base.Triggerable(move.From, room) && (move.To != move.From || move.To_place != Place.PlaceHand)
                && ((move.Reason.Reason & CardMoveReason.MoveReason.S_MASK_BASIC_REASON) == CardMoveReason.MoveReason.S_REASON_RESPONSE
                ||(move.Reason.Reason & CardMoveReason.MoveReason.S_MASK_BASIC_REASON) == CardMoveReason.MoveReason.S_REASON_USE
                || (move.Reason.Reason & CardMoveReason.MoveReason.S_MASK_BASIC_REASON) == CardMoveReason.MoveReason.S_REASON_DISCARD))
            {
                for (int i = 0; i < move.Card_ids.Count; i++)
                {
                    if ((move.From_places[i] == Place.PlaceEquip || move.From_places[i] == Place.PlaceHand) && WrappedCard.IsRed(room.GetCard(move.Card_ids[i]).Suit))
                        return new TriggerStruct(Name, move.From);
                }
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(ask_who, Name, null, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.DrawCards(ask_who, 1, Name);
            return false;
        }
    }

    public class Aocai : TriggerSkill
    {
        public Aocai() : base("aocai")
        {
            skill_type = SkillType.Defense;
            view_as_skill = new AocaiVS();
            events.Add(TriggerEvent.DrawPileChanged);
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<Player> zhugeke = RoomLogic.FindPlayersBySkillName(room, Name);
            foreach (Player p in zhugeke)
                p.Piles["#aocai"] = new List<int>();
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class AocaiVS : ViewAsSkill
    {
        public AocaiVS() : base("aocai")
        {
            expand_pile = "#aocai";
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return false;
        }
        public override bool IsEnabledAtResponse(Room room, Player player, string pattern)
        {
            if (player.Phase != PlayerPhase.NotActive)
                return false;

            foreach (FunctionCard fcard in room.AvailableFunctionCards)
            {
                if (!(fcard is BasicCard)) continue;
                WrappedCard card = new WrappedCard(fcard.Name);
                if (Engine.MatchExpPattern(room, pattern, player, card))
                    return true;
            }

            return false;
        }

        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return selected.Count == 0 && player.GetPile(expand_pile).Contains(to_select.Id)
                && Engine.MatchExpPattern(room, room.GetRoomState().GetCurrentCardUsePattern(), player, to_select);
        }

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (player.GetPile(expand_pile).Count == 0)
                return new WrappedCard(AocaiCard.ClassName) { Skill = Name, Mute = true };
            else if (cards.Count == 1)
                return cards[0];

            return null;
        }
    }

    public class AocaiCard : SkillCard
    {
        public static string ClassName = "AocaiCard";
        public AocaiCard() : base(ClassName)
        {
            target_fixed = true;
        }
        public override WrappedCard Validate(Room room, CardUseStruct use)
        {
            View(room, use.From, use.Card.SkillPosition);
            return null;
        }

        public override WrappedCard ValidateInResponse(Room room, Player player, WrappedCard card)
        {
            View(room, player, card.SkillPosition);
            return null;
        }

        private void View(Room room, Player player, string head)
        {
            List<int> guanxing = room.GetNCards(2, false);
            room.NotifySkillInvoked(player, "aocai");
            room.BroadcastSkillInvoke("aocai", player, head);
            LogMessage log = new LogMessage
            {
                Type = "$ViewDrawPile",
                From = player.Name,
                Card_str = string.Join("+", JsonUntity.IntList2StringList(guanxing))
            };
            room.SendLog(log, player);
            log.Type = "$ViewDrawPile2";
            log.Arg = guanxing.Count.ToString();
            log.Card_str = null;
            room.SendLog(log, new List<Player> { player });

            player.Piles["#aocai"] = guanxing;
            room.SetPromotSkill(player, "aocai", head, room.GetRoomState().GetCurrentCardUsePattern(), room.GetRoomState().GetCurrentCardUseReason());
        }
    }

    public class Duwu : TriggerSkill
    {
        public Duwu() : base("duwu")
        {
            events.Add(TriggerEvent.QuitDying);
            skill_type = SkillType.Attack;
            view_as_skill = new DuwuVS();
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (data is DeathStruct death && !string.IsNullOrEmpty(death.Damage.Reason) && death.Damage.Reason == Name && death.Damage.From.Alive
                && !death.Damage.Transfer && !death.Damage.Chain)
            {
                LogMessage log = new LogMessage
                {
                    Type = "#duwu-lose",
                    From = death.Damage.From.Name
                };

                death.Damage.From.SetFlags(Name);
                room.LoseHp(death.Damage.From);
            }
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class DuwuVS : ViewAsSkill
    {
        public DuwuVS() : base("duwu")
        {
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasFlag(Name);
        }

        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return RoomLogic.CanDiscard(room, player, player, to_select.Id);
        }

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            WrappedCard dw = new WrappedCard(DuwuCard.ClassName)
            {
                Skill = Name
            };
            dw.AddSubCards(cards);
            return dw;
        }
    }

    public class DuwuCard : SkillCard
    {
        public static string ClassName = "DuwuCard";
        public DuwuCard() : base(ClassName)
        {
            will_throw = true;
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select != null && RoomLogic.InMyAttackRange(room, Self, to_select, card) && to_select.Hp == card.SubCards.Count;
        }

        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            return targets.Count == 1;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            room.Damage(new DamageStruct("duwu", card_use.From, card_use.To[0]));
        }
    }
}
