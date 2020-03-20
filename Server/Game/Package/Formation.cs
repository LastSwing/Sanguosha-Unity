using CommonClass.Game;
using CommonClassLibrary;
using SanguoshaServer.Game;
using System.Collections.Generic;
using static CommonClass.Game.Player;
using static SanguoshaServer.Package.FunctionCard;

namespace SanguoshaServer.Package
{
    public class Formation : GeneralPackage
    {
        public Formation() : base("Formation")
        {
            skills = new List<Skill>
            {
                new Tuntian(),
                new TuntianGotoField(),
                new TuntianDistance(),
                new TuntianClear(),
                new Jixi(),
                new Ziliang(),
                new Huyuan(),
                new Heyi(),
                new Feiying(),
                new FeiyingVH(),
                new Tiaoxin(),
                new Yizhi(),
                new Tianfu(),
                new Shengxi(),
                new ShengxiClear(),
                new Shoucheng(),
                new Shangyi(),
                new Niaoxiang(),
                new Yicheng(),
                new Qianhuan(),
                new QianhuanClear(),
                new Zhendu(),
                new Qiluan(),
                new QiluanClear(),
                new Zhangwu(),
                new Zhangwu_Draw(),
                new Shouyue(),
                new Jizhao()
            };
            skill_cards = new List<FunctionCard> {
                new ZiliangCard(),
                new HuyuanCard(),
                new HeyiSummon(),
                new TiaoxinCard(),
                new TianfuSummon(),
                new ShangyiCard(),
                new NiaoxiangSummon(),
                new QianhuanCard()

            };
            related_skills = new Dictionary<string, List<string>> {
                { "tuntian", new List<string>{"#tuntian-gotofield", "#tuntian-clear" } },
                { "shengxi", new List<string>{ "#shengxi-clear" } },
                { "qianhuan", new List<string>{ "#qianhuan-clear" } },
                { "zhangwu", new List<string>{ "#zhangwu-draw" } },
                { "qiluan", new List<string>{ "#qiluan" } }
            };
        }
    }

    public class Tuntian : TriggerSkill
    {
        public Tuntian() : base("tuntian")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardsMoveOneTime, TriggerEvent.FinishJudge };
            frequency = Frequency.Frequent;
            priority.Add(TriggerEvent.CardsMoveOneTime, 3);
            priority.Add(TriggerEvent.FinishJudge, -1);
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move
                && move.From != null && base.Triggerable(move.From, room) && move.From.Phase == PlayerPhase.NotActive
                && (move.From_places.Contains(Place.PlaceHand) || move.From_places.Contains(Place.PlaceEquip))
                    && !(move.To == move.From && (move.To_place == Place.PlaceHand || move.To_place == Place.PlaceEquip)))
                move.From.AddMark("tuntian_postpone");
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> skill_list = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.From != null
                && base.Triggerable(move.From, room) && move.From.Phase != PlayerPhase.NotActive && move.From.GetMark("tuntian_postpone") >0
                && (move.From_places.Contains(Place.PlaceHand) || move.From_places.Contains(Place.PlaceEquip))
                    && !(move.To == move.From && (move.To_place == Place.PlaceHand || move.To_place == Place.PlaceEquip)))
                {
                    if (!room.ContainsTag("judge") || (int)room.GetTag("judge") == 0)
                        skill_list.Add(new TriggerStruct(Name, move.From));
                }
            else
            {
                List<Player> dengais = RoomLogic.FindPlayersBySkillName(room, Name);
                foreach (Player dengai in dengais) {
                    int postponed = dengai.GetMark("tuntian_postpone");
                    if (postponed > 0)
                        skill_list.Add(new TriggerStruct(Name, dengai));
                }
            }

            return skill_list;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player dengai, TriggerStruct info)
        {
            dengai.RemoveMark("tuntian_postpone");
            if (room.AskForSkillInvoke(dengai, Name, data, info.SkillPosition))
            {
                return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player dengai, TriggerStruct info)
        {
            JudgeStruct judge = new JudgeStruct
            {
                Pattern = ".|heart",
                Good = false,
                Reason = "tuntian",
                Who = dengai
            };
            room.Judge(ref judge);
            return false;
        }
    }
    public class TuntianGotoField : TriggerSkill
    {
        public TuntianGotoField() : base("#tuntian-gotofield")
        {
            events.Add(TriggerEvent.FinishJudge);
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            JudgeStruct judge = (JudgeStruct)data;
            if (judge.Who != null && base.Triggerable(judge.Who, room))
            {
                if (judge.Reason == "tuntian" && judge.IsGood() && room.GetCardPlace(judge.Card.Id) == Place.PlaceJudge)
                {
                    player = judge.Who;
                    return new TriggerStruct(Name, player);
                }
            }
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            JudgeStruct judge  = (JudgeStruct)data;
            if (room.AskForSkillInvoke(judge.Who, "tuntian", "#gotofield"))
            {
                room.BroadcastSkillInvoke("tuntian", judge.Who, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            JudgeStruct judge = (JudgeStruct)data;
            room.AddToPile(judge.Who, "field", judge.Card);
            return false;
        }
    }
    public class TuntianClear : DetachEffectSkill
    {
        public TuntianClear() : base("tuntian", "field")
        {
            frequency = Frequency.Compulsory;
        }
    }
    public class TuntianDistance : DistanceSkill
    {
        public TuntianDistance() : base("#tuntian-dist")
        {
        }
        public override int GetCorrect(Room room, Player from, Player to, WrappedCard card = null)
        {
            if (RoomLogic.PlayerHasShownSkill(room, from, "tuntian"))
            {
                int correct = 0;
                if (card != null)
                    foreach (int id in card.SubCards)
                    if (from.GetPile("field").Contains(id))
                    correct += 1;
                return -from.GetPile("field").Count + correct;
            }
            else
                return 0;
        }
    }
    public class Jixi : OneCardViewAsSkill
    {
        public Jixi() : base("jixi")
        {
            relate_to_place = "head";
            filter_pattern = ".|.|.|field";
            expand_pile = "field";
            skill_type = SkillType.Attack;
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return player.GetPile("field").Count > 0;
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard shun = new WrappedCard(Snatch.ClassName)
            {
                Skill = Name,
                ShowSkill = Name
            };
            shun.AddSubCard(card);
            shun = RoomLogic.ParseUseCard(room, shun);
            return shun;
        }
    }
    public class ZiliangCard:SkillCard
    {
        public ZiliangCard() : base("ZiliangCard")
        {
            target_fixed = true;
            will_throw = false;
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            card_use.From.SetTag("ziliang", card_use.Card.SubCards[0]);

            ResultStruct result = card_use.From.Result;
            result.Assist += card_use.Card.SubCards.Count;
            card_use.From.Result = result;
        }
    }
    public class ZiliangVS : OneCardViewAsSkill
    {
        public ZiliangVS() : base("ziliang")
        {
            response_pattern = "@@ziliang";
            filter_pattern = ".|.|.|field";
            expand_pile = "field";
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard c = new WrappedCard("ZiliangCard")
            {
                Skill = Name,
                ShowSkill = Name
            };
            c.AddSubCard(card);
            return c;
        }
    }
    public class Ziliang : TriggerSkill
    {
        public Ziliang() : base("ziliang")
        {
            events.Add(TriggerEvent.Damaged);
            relate_to_place = "deputy";
            view_as_skill = new ZiliangVS();
            skill_type = SkillType.Replenish;
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> skill_list = new List<TriggerStruct>();
            if (player != null && player.Alive)
            {
                List<Player> dengais = RoomLogic.FindPlayersBySkillName(room, Name);
                foreach (Player dengai in dengais) {
                    if (dengai.GetPile("field").Count > 0 && RoomLogic.IsFriendWith(room, dengai, player))
                        skill_list.Add(new TriggerStruct(Name, dengai));
                }
            }
            return skill_list;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player p, ref object data, Player player, TriggerStruct info)
        {
            player.RemoveTag("ziliang");
            player.SetTag("ziliang_aidata", p.Name);
            if (room.AskForUseCard(player, "@@ziliang", "@ziliang-give", null, -1, HandlingMethod.MethodNone, true, info.SkillPosition) != null)
                return info;

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player dengai, TriggerStruct info)
        {
            int id = (int)dengai.GetTag("ziliang");
            if (player == dengai)
            {
                LogMessage log = new LogMessage
                {
                    Type = "$MoveCard",
                    From = player.Name,
                    To = new List<string> { player.Name },
                    Card_str = id.ToString()
                };
                room.SendLog(log);
            }
            else
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, dengai.Name, player.Name);
            room.ObtainCard(player, id);

            return false;
        }
    }
    public class HuyuanCard:SkillCard
    {
        public static string ClassName = "HuyuanCard";
        public HuyuanCard() : base(ClassName)
        {
            will_throw = false;
        }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            if (targets.Count > 0)
                return false;
            
            WrappedCard ecard = room.GetCard(card.SubCards[0]);
            EquipCard equip = (EquipCard)Engine.GetFunctionCard(ecard.Name);
            int equip_index = (int)equip.EquipLocation();
            return to_select.GetEquip(equip_index) < 0 && RoomLogic.CanPutEquip(to_select, ecard);
        }
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            ResultStruct result = effect.From.Result;
            result.Assist += 2;
            effect.From.Result = result;

            WrappedCard ecard = room.GetCard(effect.Card.SubCards[0]);

            effect.From.SetTag("huyuan_target", effect.To.Name);

            room.MoveCardTo(ecard, effect.From, effect.To, Place.PlaceEquip,
                new CardMoveReason(MoveReason.S_REASON_PUT, effect.From.Name, effect.Card.Skill, null));

            LogMessage log = new LogMessage
            {
                Type = "$ZhijianEquip",
                From = effect.To.Name,
                Card_str = ecard.Id.ToString()
            };
            room.SendLog(log);
        }
    }
    public class HuyuanViewAsSkill : OneCardViewAsSkill
    {
        public HuyuanViewAsSkill() : base("huyuan")
        {
            filter_pattern = "EquipCard";
            response_pattern = "@@huyuan";
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard first = new WrappedCard(HuyuanCard.ClassName)
            {
                Skill = Name,
                Mute = true
            };
            first.AddSubCard(card);
            return first;
        }
    }
    public class Huyuan : PhaseChangeSkill
    {
        public Huyuan() : base("huyuan")
        {
            view_as_skill = new HuyuanViewAsSkill();
        }
        public override bool CanPreShow() => true;
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player target, ref object data, Player ask_who)
        {
            if (base.Triggerable(target, room) && target.Phase == PlayerPhase.Finish && !target.IsNude())
                return new TriggerStruct(Name, target);
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player target, ref object data, Player ask_who, TriggerStruct info)
        {
            target.RemoveTag("huyuan_equip");
            target.RemoveTag("huyuan_target");
            bool invoke = room.AskForUseCard(target, "@@huyuan", "@huyuan-equip", null, -1, HandlingMethod.MethodNone, true, info.SkillPosition) != null;
            if (invoke && target.ContainsTag("huyuan_target"))
            {
                room.BroadcastSkillInvoke(Name, target, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }
        public override bool OnPhaseChange(Room room, Player caohong, TriggerStruct info)
        {
            Player target = room.FindPlayer((string)caohong.GetTag("huyuan_target"));

            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetAlivePlayers()) {
                if (RoomLogic.DistanceTo(room, target, p) == 1 && RoomLogic.CanDiscard(room, caohong, p, "he"))
                    targets.Add(p);
            }
            if (targets.Count > 0)
            {
                Player to_dismantle = room.AskForPlayerChosen(caohong, targets, "huyuan", "@huyuan-discard:" + target.Name, true, false, info.SkillPosition);
                if (to_dismantle != null)
                {
                    int card_id = room.AskForCardChosen(caohong, to_dismantle, "he", "huyuan", false, HandlingMethod.MethodDiscard);
                    room.ThrowCard(card_id, to_dismantle, caohong);
                }
            }
            return false;
        }
    }
    public class HeyiSummon : ArraySummonCard
    {
        public HeyiSummon() : base("heyi")
        {
        }
    }
    public class Heyi : BattleArraySkill
    {
        public Heyi() : base("heyi", ArrayType.Formation)
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart, TriggerEvent.GeneralShown };
        }
        public override bool CanPreShow() => false;
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (room.AliveCount() < 4) return;
            if (triggerEvent == TriggerEvent.EventPhaseStart)
            {
                if (player != null && player.Alive && player.Phase == PlayerPhase.RoundStart)
                {
                    Player caohong = RoomLogic.FindPlayerBySkillName(room, "heyi");
                    if (caohong != null && caohong.Alive && RoomLogic.PlayerHasShownSkill(room, caohong, "heyi") && RoomLogic.InFormationRalation(room, player, caohong))
                    {
                        room.DoBattleArrayAnimate(caohong);
                        room.BroadcastSkillInvoke(Name, caohong);
                    }
                }
            }
            else if (triggerEvent == TriggerEvent.GeneralShown)
            {
                if (base.Triggerable(player, room) && RoomLogic.PlayerHasShownSkill(room, player, Name) && (bool)data == RoomLogic.InPlayerHeadSkills(player, Name))
                {
                    room.DoBattleArrayAnimate(player);
                    room.BroadcastSkillInvoke(Name, player, (bool)data ? "head" : "deputy");
                }
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            return new TriggerStruct();
        }
    }
    public class FeiyingVH : ViewHasSkill
    {
        public FeiyingVH() : base("feiyingVH")
        {
            global = true;
            viewhas_skills.Add("feiying");
        }
        public override bool ViewHas(Room room, Player player, string skill_name)
        {
            if (room.AliveCount() < 4) return false;
            List <Player> caohongs = new List<Player>();

            foreach (Player p in room.GetOtherPlayers(player))
            if (RoomLogic.PlayerHasShownSkill(room, p, "heyi"))
                caohongs.Add(p);

            foreach (Player caohong in caohongs)
            if (RoomLogic.GetFormation(room, caohong).Contains(player))
                return true;

            return false;
        }
    }
    public class Feiying : DistanceSkill
    {
        public Feiying() : base("feiying")
        {
        }
        public override int GetCorrect(Room room, Player from, Player to, WrappedCard card = null)
        {
            if (RoomLogic.PlayerHasShownSkill(room, to, Name))
                return 1;
            else
                return 0;
        }
    }
    public class TiaoxinCard : SkillCard
    {
        public TiaoxinCard() : base("TiaoxinCard") { }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && RoomLogic.InMyAttackRange(room, to_select, Self) && to_select != Self;
        }
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            bool use_slash = false;
            if (RoomLogic.CanSlash(room, effect.To, effect.From))
                use_slash = room.AskForUseSlashTo(effect.To, effect.From, "@tiaoxin-slash:" + effect.From.Name, null) != null;
            if (!use_slash && RoomLogic.CanDiscard(room, effect.From, effect.To, "he"))
            {
                int id = room.AskForCardChosen(effect.From, effect.To, "he", "tiaoxin", false, HandlingMethod.MethodDiscard);
                room.ThrowCard(id, effect.To, effect.From);
            }
        }
    }
    public class Tiaoxin : ZeroCardViewAsSkill
    {
        public Tiaoxin() : base("tiaoxin")
        {
            skill_type = SkillType.Attack;
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed("TiaoxinCard");
        }
        public override WrappedCard ViewAs(Room room, Player player)
        {
            WrappedCard card = new WrappedCard("TiaoxinCard")
            {
                Skill = Name,
                ShowSkill = Name
            };
            return card;
        }
    }
    public class Yizhi : ViewHasSkill
    {
        public Yizhi() : base("yizhi")
        {
            relate_to_place = "deputy";
            frequency = Frequency.Compulsory;
            viewhas_skills.Add("guanxing");
        }
        public override bool ViewHas(Room room, Player player, string skill_name)
        {
            if (skill_name == "guanxing" && RoomLogic.PlayerHasSkill(room, player, Name) && !RoomLogic.InPlayerHeadSkills(player, skill_name))
                return true;
            return false;
        }
    }
    public class TianfuSummon : ArraySummonCard
    {
        public TianfuSummon() : base("tianfu") { }
    }
    public class Tianfu : BattleArraySkill
    {
        public Tianfu() : base("tianfu", ArrayType.Formation)
        {
            events = new List<TriggerEvent>{ TriggerEvent.EventPhaseStart, TriggerEvent.Death, TriggerEvent.EventLoseSkill , TriggerEvent.EventAcquireSkill,
        TriggerEvent.GeneralShown,TriggerEvent.GeneralHidden,TriggerEvent.RemoveStateChanged };
            relate_to_place = "head";
        }
        public override bool CanPreShow() => false;
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            Player current = room.Current;
            if (player == null || current == null) return;

            if (triggerEvent == TriggerEvent.EventPhaseStart)
            {
                if (player.Phase != PlayerPhase.RoundStart)
                    return;
            }
            else if (triggerEvent == TriggerEvent.Death)
            {
                if (player != current && !RoomLogic.IsFriendWith(room, player, player))
                    return;
            }
            else if (triggerEvent == TriggerEvent.GeneralShown)
            {
                if (player.HasShownAllGenerals() || !RoomLogic.InFormationRalation(room, current, player))
                    return;
            }
            else if (triggerEvent == TriggerEvent.EventLoseSkill)
            {
                if (((InfoStruct)data).Info != "tianfu" || !RoomLogic.InFormationRalation(room, current, player))
                    return;
            }
            else if (triggerEvent == TriggerEvent.GeneralHidden && player.HasShownOneGeneral())
            {
                return;
            }

            foreach (Player p in room.Players) {
                if (p.GetMark("tianfu_kanpo") > 0 && p.GetAcquiredSkills("head").Contains("kanpo"))
                {
                    p.SetMark("tianfu_kanpo", 0);
                    room.DetachSkillFromPlayer(p, "kanpo", true, true);
                }
            }

            if (room.AliveCount() < 4)
                return;

            if (current != null && current.Alive && current.Phase != PlayerPhase.NotActive)
            {
                List<Player> jiangweis = RoomLogic.FindPlayersBySkillName(room, Name);
                foreach (Player jiangwei in jiangweis) {
                    if (RoomLogic.PlayerHasShownSkill(room, jiangwei, Name) && RoomLogic.InFormationRalation(room, jiangwei, current) && !RoomLogic.PlayerHasSkill(room, jiangwei, "kanpo"))
                    {
                        room.DoBattleArrayAnimate(jiangwei);
                        jiangwei.SetMark("tianfu_kanpo", 1);
                        room.AttachSkillToPlayer(jiangwei, "kanpo");
                    }
                }
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            return new TriggerStruct();
        }
    }
    public class Shengxi : TriggerSkill
    {
        public Shengxi() : base("shengxi")
        {
            events = new List<TriggerEvent> { TriggerEvent.DamageDone, TriggerEvent.EventPhaseEnd, TriggerEvent.EventPhaseStart };
            frequency = Frequency.Frequent;
            skill_type = SkillType.Replenish;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.DamageDone && data is DamageStruct damage && damage.From != null && !damage.From.HasFlag("ShengxiDamageInPlayPhase"))
            {
                damage.From.SetFlags("ShengxiDamageInPlayPhase");
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && base.Triggerable(player, room)
                && player.Phase == PlayerPhase.Discard && !player.HasFlag("ShengxiDamageInPlayPhase"))
                    return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(player, Name, null, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.DrawCards(player, 2, Name);

            return false;
        }
    }
    public class ShengxiClear : TriggerSkill
    {
        public ShengxiClear() : base("#shengxi-clear")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart };
        }
        public override int GetPriority() => -1;
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Discard && player.HasFlag("ShengxiDamageInPlayPhase"))
                player.SetFlags("-ShengxiDamageInPlayPhase");
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            return new TriggerStruct();
        }
    }
    public class Shoucheng : TriggerSkill
    {
        public Shoucheng() : base("shoucheng")
        {
            events.Add(TriggerEvent.CardsMoveOneTime);
            frequency = Frequency.Frequent;
            skill_type = SkillType.Replenish;
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            CardsMoveOneTimeStruct move  = (CardsMoveOneTimeStruct)data;
            if (move.From != null && move.From.Alive && move.From.Phase == PlayerPhase.NotActive
                && move.From_places.Contains(Place.PlaceHand) && move.Is_last_handcard)
            {
                List<Player> jfs = RoomLogic.FindPlayersBySkillName(room, Name);
                foreach (Player p in jfs)
                    if (RoomLogic.WillBeFriendWith(room, p, move.From, Name))
                        triggers.Add(new TriggerStruct(Name, p));
            }
            return triggers;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardsMoveOneTimeStruct move && move.From.Alive && room.AskForSkillInvoke(ask_who, Name, move.From, info.SkillPosition))
            {
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, ask_who.Name, ((CardsMoveOneTimeStruct)data).From.Name);
                room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardsMoveOneTimeStruct move && move.From.Alive)
                room.DrawCards(move.From, new DrawCardStruct(1, ask_who, Name));
            return false;
        }
    }
    public class ShangyiCard:SkillCard
    {
        public ShangyiCard() : base("ShangyiCard") { }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && (!to_select.IsKongcheng() || !to_select.HasShownAllGenerals()) && to_select != Self;
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            room.ShowAllCards(card_use.From, card_use.To[0], "shangyi", card_use.Card.SkillPosition);
            base.Use(room, card_use);
        }
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            List<string> choices = new List<string>();
            if (!effect.To.IsKongcheng())
                choices.Add("handcards");
            if (!effect.To.HasShownAllGenerals())
                choices.Add("hidden_general");

            effect.To.SetFlags("shangyiTarget");        //for AI
            string choice = room.AskForChoice(effect.From, "shangyi",  string.Join("+", choices), new List<string> { string.Format("@to-player:{0}", effect.To.Name) }, effect.To);
            effect.To.SetFlags("-shangyiTarget");

            LogMessage log = new LogMessage
            {
                Type = "#KnownBothView",
                From = effect.From.Name,
                To = new List<string> { effect.To.Name },
                Arg = choice
            };
            room.SendLog(log, new List<Player> { effect.From });

            if (choice.Contains("handcards"))
            {
                List<int> blacks = new List<int>();
                foreach (int card_id in effect.To.GetCards("h"))
                {
                    if (WrappedCard.IsBlack(room.GetCard(card_id).Suit))
                        blacks.Add(card_id);
                }
                effect.To.SetFlags("shangyi_target");
                int to_discard = room.DoGongxin(effect.From, effect.To, effect.To.GetCards("h"), blacks, "shangyi", "@shangyi:" + effect.To.Name, effect.Card.SkillPosition);
                effect.To.SetFlags("-shangyi_target");

                if (to_discard == -1) return;
                
                CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_DISMANTLE, effect.From.Name, effect.To.Name, "shangyi", null)
                {
                    General = RoomLogic.GetGeneralSkin(room, effect.From, "shangyi", effect.Card.SkillPosition)
                };
                List<int> ints = new List<int> { to_discard };
                room.ThrowCard(ref ints, reason, effect.To, effect.From);
            }
            else
            {
                List<string> list = new List<string>(), list2 = new List<string>();
                if (!effect.To.General1Showed)
                {
                    list.Add("head_general");
                    list2.Add(effect.To.ActualGeneral1);
                }
                if (!effect.To.General2Showed)
                {
                    list.Add("deputy_general");
                    list2.Add(effect.To.ActualGeneral2);
                }
                foreach (string name in list) {
                    log = new LogMessage
                    {
                        Type = "$KnownBothViewGeneral",
                        From = effect.From.Name,
                        To = new List<string> { effect.To.Name },
                        Arg = name,
                        Arg2 = (name == "head_general" ? effect.To.ActualGeneral1 : effect.To.ActualGeneral2)
                    };
                    room.SendLog(log, effect.From);

                    LogMessage log2 = new LogMessage
                    {
                        Type = "#KnownBothView",
                        From = effect.From.Name,
                        To = new List<string> { effect.To.Name },
                        Arg = name
                    };
                    room.SendLog(log2, new List<Player> { effect.From });
                }
                
                room.ViewGenerals(effect.From, list2, "shangyi", effect.Card.SkillPosition);
            }
        }
    }
    public class Shangyi : ZeroCardViewAsSkill
    {
        public Shangyi() : base("shangyi")
        {
            skill_type = SkillType.Attack;
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed("ShangyiCard") && !player.IsKongcheng();
        }
        public override WrappedCard ViewAs(Room room, Player player)
        {
            WrappedCard c = new WrappedCard("ShangyiCard")
            {
                Skill = Name,
                ShowSkill = Name
            };
            return c;
        }
    }
    public class NiaoxiangSummon : ArraySummonCard
    {
        public NiaoxiangSummon() : base("niaoxiang") { }
    }
    public class Niaoxiang : BattleArraySkill
    {
        public Niaoxiang() : base("niaoxiang", ArrayType.Siege)
        {
            events.Add(TriggerEvent.TargetChosen);
            frequency = Frequency.Compulsory;
            skill_type = SkillType.Attack;
        }
        public override bool CanPreShow() => false;
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> skill_list = new List<TriggerStruct>();
            CardUseStruct use = (CardUseStruct)data;
            List<Player> skill_owners = RoomLogic.FindPlayersBySkillName(room, Name);
            FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
            if (fcard is Slash)
            {
                foreach (Player skill_owner in skill_owners) {
                    if (base.Triggerable(skill_owner, room) && RoomLogic.PlayerHasShownSkill(room, skill_owner, Name))
                    {
                        List<Player> targets = new List<Player>();
                        foreach (Player to in use.To) {
                            if (RoomLogic.InSiegeRelation(room, player, skill_owner, to))
                                targets.Add(to);
                        }
                        if (targets.Count > 0)
                        {
                            if (RoomLogic.InPlayerHeadSkills(skill_owner, Name))
                            {
                                TriggerStruct trigger = new TriggerStruct(Name, skill_owner, targets)
                                {
                                    SkillPosition = "head"
                                };
                                skill_list.Add(trigger);
                            }
                            if (RoomLogic.InPlayerDeputykills(skill_owner, Name))
                            {
                                TriggerStruct trigger = new TriggerStruct(Name, skill_owner, targets)
                                {
                                    SkillPosition = "deputy"
                                };
                                skill_list.Add(trigger);
                            }
                        }
                    }
                }
            }
            return skill_list;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player skill_target, ref object data, Player ask_who, TriggerStruct info)
        {
            room.DoBattleArrayAnimate(ask_who, skill_target);
            room.BroadcastSkillInvoke(Name, ask_who, info.SkillPosition);
            return info;
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player skill_target, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(ask_who, Name, true);
            CardUseStruct use = (CardUseStruct)data;
            int x = use.To.IndexOf(skill_target);

            CardBasicEffect effect = use.EffectCount[x];
            effect.Effect2 = 0;
            use.EffectCount[x] = effect;
            data = use;

            return false;
        }
    }
    public class Yicheng : TriggerSkill
    {
        public Yicheng() : base("yicheng")
        {
            events.Add(TriggerEvent.TargetConfirmed);
            frequency = Frequency.Frequent;
            skill_type = SkillType.Defense;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            CardUseStruct use = (CardUseStruct)data;
            FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
            if (fcard is Slash && use.To.Contains(player))
            {
                foreach (Player p in RoomLogic.FindPlayersBySkillName(room, Name))
                    if (RoomLogic.IsFriendWith(room, player, p))
                        return new TriggerStruct(Name, player, p);
            }
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player owner = room.FindPlayer(info.SkillOwner);
            if (owner != null && room.AskForSkillInvoke(ask_who, Name, data, info.SkillPosition))
            {
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, owner.Name, ask_who.Name);
                room.BroadcastSkillInvoke(Name, owner, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player owner = room.FindPlayer(info.SkillOwner);
            room.DrawCards(ask_who, new DrawCardStruct(1, owner, Name));
            if (ask_who.Alive && RoomLogic.CanDiscard(room, ask_who, ask_who, "he"))
                room.AskForDiscard(ask_who, Name, 1, 1, false, true, null, false, info.SkillPosition);
            return false;
        }
    }
    public class QianhuanCard:SkillCard
    {
        public QianhuanCard() : base("QianhuanCard")
        {
            target_fixed = true;
            will_throw = false;
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_REMOVE_FROM_PILE, null, "qianhuan", null);
            List<int> ids = new List<int> { card_use.Card.SubCards[0] };
            room.ThrowCard(ref ids, reason, null);
        }
    }
    public class QianhuanVS : OneCardViewAsSkill
    {
        public QianhuanVS() : base("qianhuan")
        {
            filter_pattern = ".|.|.|sorcery";
            response_pattern = "@@qianhuan";
            expand_pile = "sorcery";
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard c = new WrappedCard("QianhuanCard")
            {
                Mute = true,
                Skill = Name
            };
            c.AddSubCard(card);
            return c;
        }
    }
    public class Qianhuan : TriggerSkill
    {
        public Qianhuan() : base("qianhuan")
        {
            events = new List<TriggerEvent> { TriggerEvent.Damaged, TriggerEvent.TargetConfirming, TriggerEvent.CardsMoveOneTime };
            view_as_skill = new QianhuanVS();
            skill_type = SkillType.Wizzard;
        }
        public override bool CanPreShow() => true;
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> skill_list = new List<TriggerStruct>();
            List<Player> yujis = RoomLogic.FindPlayersBySkillName(room, Name);
            if (player != null && triggerEvent == TriggerEvent.Damaged && player.Alive)
            {
                foreach (Player yuji in yujis) {
                    if (RoomLogic.WillBeFriendWith(room, yuji, player) && yuji.GetPile("sorcery").Count < 4)
                        skill_list.Add(new TriggerStruct(Name, yuji));
                }
            }
            else if (player != null && triggerEvent == TriggerEvent.TargetConfirming && data is CardUseStruct use && use.Card != null)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (!(fcard is SkillCard) && !(fcard is EquipCard) && use.To.Contains(player) && use.To.Count == 1)
                {
                    foreach (Player yuji in yujis)
                    {
                        if (yuji.GetPile("sorcery").Count > 0 && RoomLogic.WillBeFriendWith(room, yuji, use.To[0], Name))
                            skill_list.Add(new TriggerStruct(Name, yuji));
                    }
                }
            }
            else if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.To != null && move.To_place == Place.PlaceDelayedTrick
                && move.Reason.Reason == MoveReason.S_REASON_TRANSFER)
            {
                WrappedCard card = move.Reason.Card;
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                if (fcard is DelayedTrick && room.GetCardPlace(move.Card_ids[0]) == Place.PlaceDelayedTrick)
                {
                    foreach (Player yuji in yujis)
                    {
                        if (yuji.GetPile("sorcery").Count > 0 && RoomLogic.WillBeFriendWith(room, yuji, move.To, Name))
                            skill_list.Add(new TriggerStruct(Name, yuji));
                    }
                }
            }

            return skill_list;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player yuji, TriggerStruct info)
        {
           room.SetTag("qianhuan_data", data);

            bool invoke = false;

            if (triggerEvent == TriggerEvent.Damaged)
            {
                List<string> patterns = new List<string> { "spade", "heart", "club", "diamond" };
                foreach (int id in yuji.GetPile("sorcery"))
                    patterns.Remove(WrappedCard.GetSuitString(room.GetCard(id).Suit));
                List<int> ints = room.AskForExchange(yuji, Name, 1, 0, "@qianhuan", string.Empty, ".|" + string.Join(",", patterns), info.SkillPosition);
                if (ints.Count > 0)
                {
                    room.AddToPile(yuji, "sorcery", ints);
                    invoke = true;
                }
            }
            else if (triggerEvent == TriggerEvent.TargetConfirming && data is CardUseStruct use)
            {
                List<string> prompt_list = new List<string> { "@qianhuan-cancel", string.Empty, use.To[0].Name, use.Card.Name };
                string prompt = string.Join(":", prompt_list);
                if (room.AskForUseCard(yuji, "@@qianhuan", prompt, null, -1, HandlingMethod.MethodNone, true, info.SkillPosition) != null)
                {
                    if (yuji != use.To[0])
                    {
                        ResultStruct result = yuji.Result;
                        result.Assist++;
                        yuji.Result = result;
                    }

                    room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, yuji.Name, use.To[0].Name);
                    invoke = true;
                }
            }
            else if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.To_place == Place.PlaceDelayedTrick
                && room.GetCardPlace(move.Card_ids[0]) == Place.PlaceDelayedTrick)
            {
                WrappedCard card = move.Reason.Card;
                List<string> prompt_list = new List<string> { "@qianhuan-cancel", string.Empty, move.To.Name, card.Name };
                string prompt = string.Join(":", prompt_list);
                if (room.AskForUseCard(yuji, "@@qianhuan", prompt, null, -1, HandlingMethod.MethodNone, true, info.SkillPosition) != null)
                {
                    room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, yuji.Name, move.To.Name);
                    invoke = true;
                }
            }

            room.RemoveTag("qianhuan_data");
            if (invoke)
            {
                room.BroadcastSkillInvoke(Name, yuji, info.SkillPosition);
                room.NotifySkillInvoked(yuji, Name);
                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.TargetConfirming && data is CardUseStruct use)
            {
                room.CancelTarget(ref use, use.To[0]); // Room::cancelTarget(use, player);
                data = use;
                return true;
            }
            else if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && room.GetCardPlace(move.Card_ids[0]) == Place.PlaceDelayedTrick)
            {
                room.SetEmotion(ask_who, "cancel");
                System.Threading.Thread.Sleep(400);

                CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_NATURAL_ENTER, string.Empty)
                {
                    Card = move.Reason.Card
                };

                CardsMoveStruct move2 = new CardsMoveStruct(move.Card_ids, null, Place.DiscardPile, reason);
                room.MoveCardsAtomic(move2, true);
            }

            return false;
        }
    }
    public class QianhuanClear : DetachEffectSkill
    {
        public QianhuanClear() : base("qianhuan", "sorcery")
        {
            frequency = Frequency.Compulsory;
        }
    }
    public class Zhendu : TriggerSkill
    {
        public Zhendu() : base("zhendu")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart, TriggerEvent.CardEffectConfirmed };
            skill_type = SkillType.Attack;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardEffectConfirmed)
            {
                CardEffectStruct effect = (CardEffectStruct)data;
                if (effect.Card != null && effect.Card.Skill == "_zhendu")
                    effect.To.SetFlags(Name);
            }
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> skill_list = new List<TriggerStruct>();
            if (player != null && triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Play)
            {
                List<Player> hetaihous = RoomLogic.FindPlayersBySkillName(room, Name);
                foreach (Player hetaihou in hetaihous) {
                    if (RoomLogic.CanDiscard(room, hetaihou, hetaihou, "h") && hetaihou != player)
                        skill_list.Add(new TriggerStruct(Name, hetaihou));
                }
            }
            return skill_list;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player hetaihou, TriggerStruct info)
        {
            if (room.AskForDiscard(hetaihou, Name, 1, 1, true, false, "@zhendu-discard", true, info.SkillPosition))
            {
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, hetaihou.Name, player.Name);
                room.BroadcastSkillInvoke(Name, hetaihou, info.SkillPosition);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player hetaihou, TriggerStruct info)
        {
            WrappedCard analeptic = new WrappedCard(Analeptic.ClassName)
            {
                Skill = "_zhendu"
            };
            if (room.UseCard(new CardUseStruct(analeptic, player, new List<Player>(), true), true, true))
            {
                if (player.Alive && player.HasFlag(Name))
                    room.Damage(new DamageStruct(Name, hetaihou, player));
            }
            player.SetFlags("-" + Name);

            return false;
        }
    }
    public class Qiluan : TriggerSkill
    {
        public Qiluan() : base("qiluan")
        {
            events = new List<TriggerEvent> { TriggerEvent.Death, TriggerEvent.EventPhaseStart };
            skill_type = SkillType.Replenish;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.Death && data is DeathStruct death && death.Damage.From != null && death.Damage.From.Alive)
            {
                Player current = room.Current;
                if (current != null && (current.Alive || death.Who == current) && current.Phase != PlayerPhase.NotActive)
                    death.Damage.From.SetMark(Name , 1);
            }
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> skill_list = new List<TriggerStruct>();
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.NotActive)
            {
                foreach (Player p in RoomLogic.FindPlayersBySkillName(room, Name))
                    if (p.GetMark(Name) > 0) skill_list.Add(new TriggerStruct(Name, p));
            }

            return skill_list;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player hetaihou, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(hetaihou, Name, null, info.SkillPosition))
            {
                room.BroadcastSkillInvoke(Name, hetaihou, info.SkillPosition);
                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.DrawCards(ask_who, 3, Name);
            return false;
        }
    }

    public class QiluanClear : TriggerSkill
    {
        public QiluanClear() : base("#qiluan")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseStart };
        }
        public override int GetPriority() => 2;
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.NotActive)
            {
                foreach (Player p in room.GetAlivePlayers())
                    if (p.GetMark("qiluan") > 0) p.SetMark("qiluan", 0);
            }
        }

        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            return new List<TriggerStruct>();
        }
    }

    public class Zhangwu : TriggerSkill
    {
        public Zhangwu() : base("zhangwu")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardsMoveOneTime, TriggerEvent.BeforeCardsMove };
            frequency = Frequency.Compulsory;
        }
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> triggers = new List<TriggerStruct>();
            if (data is CardsMoveOneTimeStruct move && move.To_place != Place.DrawPileBottom)
            {
                int fldfid = -1;
                foreach (int id in move.Card_ids)
                {
                    if (room.GetCard(id).Name == DragonPhoenix.ClassName)
                    {
                        fldfid = id;
                        break;
                    }
                }

                if (fldfid >= 0)
                {
                    if (triggerEvent == TriggerEvent.CardsMoveOneTime && room.GetCardPlace(fldfid) == move.To_place
                        && (move.To_place == Place.DiscardPile || move.To_place == Place.PlaceEquip))
                    {
                        List<Player> lord_liubeis = RoomLogic.FindPlayersBySkillName(room, Name);
                        foreach (Player p in lord_liubeis)
                        {
                            if (base.Triggerable(p, room) && (move.To_place == Place.DiscardPile || p != move.To))
                                triggers.Add(new TriggerStruct(Name, p));
                        }
                    }
                    else if (triggerEvent == TriggerEvent.BeforeCardsMove && move.From != null && base.Triggerable(move.From, room))
                    {
                        Place from = move.From_places[move.Card_ids.IndexOf(fldfid)];
                        Place to = move.To_place;
                        {
                            if ((from == Place.PlaceHand || from == Place.PlaceEquip) && (move.To != move.From || (to != Place.PlaceHand && to != Place.PlaceEquip)))
                            {
                                if (move.From_places[move.Card_ids.IndexOf(fldfid)] == Place.PlaceHand && move.To_place == Place.PlaceTable &&
                                        move.Reason.Reason == MoveReason.S_REASON_USE && move.Reason.Card.Name == DragonPhoenix.ClassName)
                                    return triggers;

                                triggers.Add(new TriggerStruct(Name, move.From));
                            }
                        }
                    }
                }
            }

            return triggers;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            bool invoke = RoomLogic.PlayerHasShownSkill(room, ask_who, Name) ? true : room.AskForSkillInvoke(ask_who, Name, null, "head");
            if (invoke)
            {
                if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move)
                {
                    if (move.To != null)
                        room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, ask_who.Name, move.To.Name);
                }

                room.BroadcastSkillInvoke(Name, "male", (triggerEvent == TriggerEvent.BeforeCardsMove) ? 1 : 2, ask_who.ActualGeneral1, ask_who.HeadSkinId);
                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(ask_who, Name);
            CardsMoveOneTimeStruct move = (CardsMoveOneTimeStruct)data;
            WrappedCard dragonPhoenix = null;
            int dragonPhoenixId = -1;
            foreach (int id in move.Card_ids) {
                WrappedCard card = room.GetCard(id);
                if (card.Name == DragonPhoenix.ClassName)
                {
                    dragonPhoenixId = id;
                    dragonPhoenix = card;
                    break;
                }
            }

            if (triggerEvent == TriggerEvent.CardsMoveOneTime)
            {
                room.ObtainCard(ask_who, dragonPhoenix);
            }
            else
            {
                room.ShowCard(ask_who, dragonPhoenixId, Name);
                ask_who.SetFlags("fldf_removing");
                move.From_places.RemoveAt(move.Card_ids.IndexOf(dragonPhoenixId));
                move.Card_ids.Remove(dragonPhoenixId);
                data = move;

                room.MoveCardTo(dragonPhoenix, null, Place.DrawPileBottom);
            }
            return false;
        }
    }
    public class Zhangwu_Draw : TriggerSkill
    {
        public Zhangwu_Draw() : base("#zhangwu-draw")
        {
            frequency = Frequency.Compulsory;
            events.Add(TriggerEvent.CardsMoveOneTime);
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is CardsMoveOneTimeStruct move && move.From != null && base.Triggerable(move.From, room) && move.To_place == Place.DrawPileBottom)
            {
                int fldfid = -1;
                foreach (int id in move.Card_ids) {
                    if (room.GetCard(id).Name == DragonPhoenix.ClassName)
                    {
                        fldfid = id;
                        break;
                    }
                }

                if (fldfid == -1)
                    return new TriggerStruct();

                if (move.From.HasFlag("fldf_removing"))
                    return new TriggerStruct(Name, move.From);

            }

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            ask_who.SetFlags("-fldf_removing");
            return RoomLogic.PlayerHasShownSkill(room, ask_who, Name) ? info : new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.DrawCards(ask_who, 2, "zhangwu");
            return false;
        }
    }
    public class Shouyue : TriggerSkill
    {
        public Shouyue() : base("shouyue")
        {
            lord_skill = true;
            frequency = Frequency.Compulsory;
        }
        public override bool CanPreShow() => false;
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            return new TriggerStruct();
        }
    }
    public class Jizhao : TriggerSkill
    {
        public Jizhao() : base("jizhao")
        {
            events.Add(TriggerEvent.AskForPeaches);
            frequency = Frequency.Limited;
            limit_mark = "@jizhao";
            skill_type = SkillType.Recover;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is DyingStruct dying && dying.Who == player && base.Triggerable(player, room) && player.GetMark("@jizhao") > 0 && player.Hp <= 0)
                return new TriggerStruct(Name, player);
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(player, Name, data))
            {
                room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                room.DoSuperLightbox(player, info.SkillPosition, Name);
                room.SetPlayerMark(player, limit_mark, 0);
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (player.HandcardNum < player.MaxHp)
                room.DrawCards(player, player.MaxHp - player.HandcardNum, Name);

            if (player.Hp < 2)
            {
                RecoverStruct rec = new RecoverStruct
                {
                    Recover = 2 - player.Hp,
                    Who = player
                };
                room.Recover(player, rec);
            }

            room.HandleAcquireDetachSkills(player, "-shouyue|rende");
            return false; //return player.getHp() > 0 || player.isDead();
        }
    }
}
