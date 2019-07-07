using CommonClass;
using CommonClass.Game;
using CommonClassLibrary;
using SanguoshaServer.Game;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using static CommonClass.Game.CardUseStruct;
using static CommonClass.Game.Player;

namespace SanguoshaServer.Package
{
    public class StanderCards : CardPackage
    {
        public StanderCards() : base("StanderCards")
        {
            skills = new List<Skill>
            {
                new DoubleSwordSkill(),
                new QinggangSwordSkill(),
                new SpearSkill(),
                new AxeSkill(),
                new KylinBowSkill(),
                new EightDiagramSkill(),
                new IceSwordSkill(),
                new RenwangShieldSkill(),
                new FanSkill(),
                new SixSwordsSkill(),
                new TribladeSkill(),
                new VineSkill(),
                new SilverLionSkill(),
                new HorseSkill(),
                new Companion(),
                new Megatama(),
                new MegatamaMax(),
                new Pioneer()
            };
            cards = new List<FunctionCard>
            {
                new DummyCard(),
                new TribladeSkillCard(),

                new Slash(),
                new FireSlash(),
                new ThunderSlash(),
                new Analeptic(),
                new Jink(),
                new Peach(),

                new GodSalvation(),
                new AmazingGrace(),
                new SavageAssault(),
                new ArcheryAttack(),
                new Collateral(),
                new ExNihilo(),
                new Indulgence(),
                new SupplyShortage(),
                new Lightning(),
                new Nullification(),
                new HegNullification(),
                new Snatch(),
                new Dismantlement(),
                new Duel(),
                new IronChain(),
                new FireAttack(),
                new KnownBoth(),
                new AwaitExhausted(),
                new BefriendAttacking(),

                new CrossBow(),
                new DoubleSword(),
                new QinggangSword(),
                new Spear(),
                new Axe(),
                new KylinBow(),
                new EightDiagram(),
                new IceSword(),
                new RenwangShield(),
                new Fan(),
                new SixSwords(),
                new Triblade(),
                new Vine(),
                new SilverLion(),
                new DefensiveHorse("Jueying"),
                new DefensiveHorse("Dilu"),
                new DefensiveHorse("Zhuahuangfeidian"),
                new OffensiveHorse("Chitu"),
                new OffensiveHorse("Dayuan"),
                new OffensiveHorse("Zixing"),

                new MegatamaCard(),
                new CompanionCard(),
                new PioneerCard(),
            };
        }
    }
}

namespace SanguoshaServer.Package
{
    #region 技能卡
    public class DummyCard : SkillCard
    {
        public DummyCard() : base("DummyCard")
        {
            target_fixed = true;
            handling_method = HandlingMethod.MethodNone;
            type_id = CardType.TypeSkill;
        }
        public override string GetSubtype() => "dummy_card";
    }
    #endregion

    #region 基本卡
    public class Slash : BasicCard
    {
        protected DamageStruct.DamageNature nature = DamageStruct.DamageNature.Normal;

        public Slash() : base("Slash")
        {
        }
        public DamageStruct.DamageNature Nature => nature;

        public override string GetSubtype() => "attack_card";

        public override void OnUse(Room room, CardUseStruct use)
        {
            Player player = use.From;

            if (player.HasFlag("slashTargetFix"))
            {
                player.SetFlags("-slashTargetFix");
                player.SetFlags("-slashTargetFixToOne");
                foreach (Player target in room.GetAlivePlayers())
                    if (target.HasFlag("SlashAssignee"))
                        target.SetFlags("-SlashAssignee");
            }

            /* actually it's not proper to put the codes here.
               considering the nasty design of the client and the convenience as well,
               I just move them here */
            WrappedCard card = use.Card;
            if (RoomLogic.IsVirtualCard(room, card) && player.HasWeapon(card.Skill))
            {
                room.SetEmotion(player, card.Skill.ToLower());
                Thread.Sleep(400);
                card.SetFlags(card.Skill);
            }

            string color = WrappedCard.IsRed(RoomLogic.GetCardSuit(room, card)) ? "red_" : "black_";
            string type_str = (this is ThunderSlash) ? "thunder_" : (this is FireSlash) ? "fire_" : string.Empty;
            room.SetEmotion(player, string.Format("{0}{1}slash", color, type_str));

            if (player.GetMark("drank") > 0)
            {
                use.Drank = player.GetMark("drank");
                room.SetPlayerMark(player, "drank", 0);
            }

            base.OnUse(room, use);
        }
        public override void OnEffect(Room room, CardEffectStruct card_effect)
        {
            SlashEffectStruct effect = new SlashEffectStruct
            {
                From = card_effect.From,
                Nature = nature,
                Slash = card_effect.Card,
                To = card_effect.To,
                Drank = card_effect.Drank,
                Nullified = card_effect.Nullified,
                Jink_num = 1
            };

            if (card_effect.EffectCount != null)
            {
                foreach (EffctCount count in card_effect.EffectCount)
                {
                    if (count.From == card_effect.From && count.To == card_effect.To)
                    {
                        effect.Jink_num = count.Count;
                        break;
                    }
                }
            }

            room.SlashEffect(effect);
        }
        public override void CheckTargetModSkillShow(Room room, CardUseStruct use)
        {
            if (use.Card == null) return;

            Player player = use.From;
            WrappedCard card = use.Card;
            List<TargetModSkill> targetModSkills = new List<TargetModSkill>();
            // for Paoxiao, Jili, CrossBow & etc
            if (player.Phase == PlayerPhase.Play && use.Reason == CardUseReason.CARD_USE_REASON_PLAY && player.GetSlashCount() > 1)
            {
                foreach (string name in room.Skills)
                {
                    if (Engine.GetSkill(name) is TargetModSkill skill)
                    {
                        if (player.GetSlashCount() <= Engine.CorrectCardTarget(room, TargetModSkill.ModType.Residue, player, card) + 1
                                && player.UsedTimes("Slash_" + name) < skill.GetResidueNum(room, player, card))
                            targetModSkills.Add(skill);
                        else
                        {
                            foreach (Player p in use.To)
                            {
                                if (skill.CheckSpecificAssignee(room, player, p, card))
                                {
                                    targetModSkills.Add(skill);
                                    break;
                                }
                            }
                        }
                    }
                }
                bool canSelectCrossBow = player.HasWeapon("CrossBow") && !card.SubCards.Contains(player.Weapon.Key);
                List<string> q = new List<string>();
                foreach (TargetModSkill skill in targetModSkills)
                {
                    if (skill.GetResidueNum(room, player, card) > 500 && RoomLogic.PlayerHasShownSkill(room, player, skill))
                    {
                        q.Add(skill.Name);
                        canSelectCrossBow = false;
                    }
                }
                if (canSelectCrossBow) q.Add("CrossBow");
                if (q.Count == 0)
                {
                    foreach (TargetModSkill skill in targetModSkills)
                        if (!q.Contains(skill.Name))
                            q.Add(skill.Name);
                }
                else
                {
                    foreach (TargetModSkill skill in targetModSkills)
                        if (skill.GetResidueNum(room, player, card) > 500 && !RoomLogic.PlayerHasShownSkill(room, player, skill))
                            q.Add(skill.Name);
                }

                List<TriggerStruct> skills = new List<TriggerStruct>();
                foreach (string skill in q)
                {
                    skills.Add(new TriggerStruct(skill, player));
                }
                TriggerStruct r = room.AskForSkillTrigger(player, "declare_skill_invoke", skills, false, null, true);

                if (r.SkillName.EndsWith("CrossBow"))
                {
                    room.SetEmotion(player, "crossbow");
                    Thread.Sleep(200);
                }
                else
                {
                    string skill_name = r.SkillName;
                    string position = r.SkillPosition;

                    string main = Engine.GetMainSkill(skill_name).Name;
                    TargetModSkill result_skill = (TargetModSkill)Engine.GetSkill(skill_name);
                    player.AddHistory("Slash_" + skill_name);

                    room.ShowSkill(player, main, position);
                    GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, main, position);
                    room.BroadcastSkillInvoke(main, "male", result_skill.GetEffectIndex(room, player, card, TargetModSkill.ModType.Residue), gsk.General, gsk.SkinId);

                    room.NotifySkillInvoked(player, main);
                }
            }

            List<Player> correct_targets = new List<Player>();
            foreach (Player p in use.To)
                if (!card.DistanceLimited && !RoomLogic.InMyAttackRange(room, player, p, card))
                    correct_targets.Add(p);

            if (correct_targets.Count > 0)
            {
                List<TargetModSkill> showed = new List<TargetModSkill>();
                targetModSkills.Clear();
                foreach (string name in room.Skills)
                {
                    if (Engine.GetSkill(name) is TargetModSkill tarmod)
                    {
                        foreach (Player p in new List<Player>(correct_targets))
                        {
                            if (tarmod.GetDistanceLimit(room, player, p, card))
                            {
                                Skill main_skill = Engine.GetMainSkill(tarmod.Name);
                                if (RoomLogic.PlayerHasShownSkill(room, player, main_skill) || !RoomLogic.PlayerHasSkill(room, player, main_skill.Name))
                                {
                                    correct_targets.Remove(p);
                                    if (!showed.Contains(tarmod))
                                        showed.Add(tarmod);
                                }
                                else if (!targetModSkills.Contains(tarmod))
                                    targetModSkills.Add(tarmod);
                            }
                        }
                    }
                }

                if (showed.Count > 0)
                {
                    foreach (TargetModSkill skill in showed)
                    {
                        Skill main_skill = Engine.GetMainSkill(skill.Name);
                        room.NotifySkillInvoked(player, main_skill.Name);
                        if (RoomLogic.PlayerHasSkill(room, player, main_skill.Name))
                        {
                            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, main_skill.Name);
                            room.BroadcastSkillInvoke(main_skill.Name, player.IsMale() ? "male" : "female",
                                                       skill.GetEffectIndex(room, player, card, TargetModSkill.ModType.DistanceLimit), gsk.General, gsk.SkinId);
                        }
                    }
                }

                if (targetModSkills.Count > 0)
                {
                    List<TargetModSkill> tarmods_copy;
                    while (correct_targets.Count > 0 && targetModSkills.Count > 0)
                    {
                        List<string> show = new List<string>();
                        tarmods_copy = new List<TargetModSkill>(targetModSkills);
                        List<Player> targets_copy = new List<Player>(correct_targets);
                        foreach (TargetModSkill tarmod in tarmods_copy)
                            foreach (Player p in targets_copy)
                                if (tarmod.GetDistanceLimit(room, player, p, card) && !show.Contains(tarmod.Name))
                                    show.Add(tarmod.Name);

                        if (show.Count == 0) break;
                        List<TriggerStruct> skills = new List<TriggerStruct>();
                        foreach (string skill in show)
                        {
                            skills.Add(new TriggerStruct(skill, player));
                        }
                        TriggerStruct r = room.AskForSkillTrigger(player, "declare_skill_invoke", skills, false, null, true);

                        string skill_name = r.SkillName;
                        string position = r.SkillPosition;

                        TargetModSkill result_skill = (TargetModSkill)Engine.GetSkill(skill_name);
                        string main = Engine.GetMainSkill(skill_name).Name;
                        targetModSkills.Remove(result_skill);
                        foreach (Player p in targets_copy)
                            if (result_skill.GetDistanceLimit(room, player, p, card))
                                correct_targets.Remove(p);

                        room.ShowSkill(player, main, position);
                        room.NotifySkillInvoked(player, main);
                        GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, main, position);
                        room.BroadcastSkillInvoke(main, player.IsMale() ? "male" : "female",
                            result_skill.GetEffectIndex(room, player, card, TargetModSkill.ModType.DistanceLimit), gsk.General, gsk.SkinId);
                    }
                }
            }
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            int slash_targets = 1 + (card.ExtraTarget ? Engine.CorrectCardTarget(room, TargetModSkill.ModType.ExtraMaxTarget, Self, card) : 0);

            bool has_specific_assignee = false;
            foreach (Player p in room.GetAlivePlayers())
            {
                if (p == Self) continue;
                if (IsSpecificAssignee(room, p, Self, card))
                {
                    has_specific_assignee = true;
                    break;
                }
            }

            if (has_specific_assignee)
            {
                if (targets.Count == 0)
                    return IsSpecificAssignee(room, to_select, Self, card) && RoomLogic.CanSlash(room, Self, to_select, card);
                else
                {
                    if (!card.ExtraTarget) return false;
                    bool canSelect = false;
                    foreach (Player p in targets)
                    {
                        if (IsSpecificAssignee(room, p, Self, card))
                        {
                            canSelect = true;
                            break;
                        }
                    }
                    if (!canSelect) return false;
                }
            }

            if (!RoomLogic.CanSlash(room, Self, to_select, card, 0, targets) || targets.Count >= slash_targets) return false;

            return true;
        }
        public override bool ExtratargetFilter(Room room, List<Player> selected, Player to_select, Player Self, WrappedCard card)
        {
            return card.ExtraTarget && RoomLogic.CanSlash(room, Self, to_select, card, 0, selected);
        }
        public override bool IsAvailable(Room room, Player player, WrappedCard card)
        {
            return IsAvailable(room, player, card);
        }

        public static bool IsAvailable(Room room, Player player, WrappedCard slash = null, bool considerSpecificAssignee = true)
        {
            WrappedCard newslash = new WrappedCard("Slash");
            newslash.SetFlags("Global_SlashAvailabilityChecker");
            slash = slash ?? newslash;
            if (RoomLogic.IsCardLimited(room, player, slash, HandlingMethod.MethodUse)) return false;

            CardUseReason reason = room.GetRoomState().GetCurrentCardUseReason();
            if (reason == CardUseReason.CARD_USE_REASON_PLAY
                    || (reason == CardUseReason.CARD_USE_REASON_RESPONSE_USE && room.GetRoomState().GetCurrentCardUsePattern(player) == "@@rende"))
            {
                List<int> ids = slash != null ? slash.SubCards : new List<int>();
                bool has_weapon = player.HasWeapon("CrossBow") && !ids.Contains(player.Weapon.Key);
                if (has_weapon || RoomLogic.CanSlashWithoutCrossBow(room, player, slash))
                    return true;

                if (considerSpecificAssignee)
                {
                    foreach (Player p in room.GetAlivePlayers())
                        if (Engine.CorrectCardTarget(room, TargetModSkill.ModType.SpecificAssignee, player, p, slash) && RoomLogic.CanSlash(room, player, p, slash))
                            return true;

                }
                return false;
            }
            else
                return true;
        }
        static bool IsSpecificAssignee(Room room, Player player, Player from, WrappedCard slash)
        {
            if (from.HasFlag("slashTargetFix") && player.HasFlag("SlashAssignee")) return true;
            else if (from.Phase == PlayerPhase.Play && room.GetRoomState().GetCurrentCardUseReason() == CardUseReason.CARD_USE_REASON_PLAY
                && !IsAvailable(room, from, slash, false))
            {
                if (Engine.CorrectCardTarget(room, TargetModSkill.ModType.SpecificAssignee, from, player, slash))
                    return true;
            }
            return false;
        }
    }
    public class FireSlash : Slash
    {
        public FireSlash()
        {
            card_name = "FireSlash";
            nature = DamageStruct.DamageNature.Fire;
        }
    }
    public class ThunderSlash : Slash
    {
        public ThunderSlash()
        {
            card_name = "ThunderSlash";
            nature = DamageStruct.DamageNature.Thunder;
        }
    }
    class Jink : BasicCard
    {
        public Jink() : base("Jink")
        {
            target_fixed = true;
        }
        public override string GetSubtype() => "defense_card";
        public override bool IsAvailable(Room room, Player player, WrappedCard card)
        {
            CardUseReason reason = room.GetRoomState().GetCurrentCardUseReason();
            HandlingMethod method = HandlingMethod.MethodNone;
            if (reason == CardUseReason.CARD_USE_REASON_RESPONSE_USE)
                method = HandlingMethod.MethodUse;
            else if (reason == CardUseReason.CARD_USE_REASON_RESPONSE)
                method = HandlingMethod.MethodResponse;

            return room.GetRoomState().GetCurrentCardUsePattern() == "jink" && RoomLogic.IsProhibited(room, player, null, card) == null
                && !RoomLogic.IsCardLimited(room, player, card, method);
        }
    }
    class Peach : BasicCard
    {

        public Peach() : base("Peach")
        {
            target_fixed = true;
        }
        public override string GetSubtype() => "recover_card";
        public override void OnUse(Room room, CardUseStruct use)
        {
            if (use.To.Count == 0)
                use.To.Add(use.From);
            base.OnUse(room, use);
        }
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            room.SetEmotion(effect.From, "peach");

            // recover hp
            RecoverStruct recover = new RecoverStruct
            {
                Recover = 1,
                Card = effect.Card,
                Who = effect.From
            };
            room.Recover(effect.To, recover);
        }

        public override bool IsAvailable(Room room, Player player, WrappedCard card)
        {
            if (!base.IsAvailable(room, player, card)) return false;
            //鏖战模式特殊判断
            if (room.BloodBattle && !RoomLogic.IsVirtualCard(room, card))
            {
                WrappedCard slash = new WrappedCard("Slash");
                slash.AddSubCard(card);
                slash = RoomLogic.ParseUseCard(room, card);
                if (Engine.GetFunctionCard("Slash").IsAvailable(room, player, slash))
                    return true;
                else
                {
                    WrappedCard jink = new WrappedCard("Jink");
                    jink.AddSubCard(card);
                    jink = RoomLogic.ParseUseCard(room, card);
                    return Engine.GetFunctionCard("Jink").IsAvailable(room, player, jink);
                }
            }
            else
            {
                if (room.GetRoomState().GetCurrentCardUseReason() == CardUseReason.CARD_USE_REASON_RESPONSE_USE)
                {
                    Player dying = room.GetRoomState().GetCurrentAskforPeachPlayer();
                    if (dying != null)
                    {
                        return RoomLogic.IsProhibited(room, player, dying, card) == null;
                    }
                }

                return RoomLogic.IsProhibited(room, player, player, card) == null && player.IsWounded()
                    && room.GetRoomState().GetCurrentCardUseReason() != CardUseReason.CARD_USE_REASON_RESPONSE;
            }
        }
    }
    public class Analeptic : BasicCard
    {

        public Analeptic() : base("Analeptic")
        {
            target_fixed = true;
        }
        public override string GetSubtype() => "buff_card";
        public static bool IsAnalepticAvailable(Room room, Player player, WrappedCard analeptic = null)
        {
            WrappedCard card = new WrappedCard("Analeptic");
            WrappedCard THIS_ANAL = analeptic ?? card;
            if (RoomLogic.IsCardLimited(room, player, THIS_ANAL, HandlingMethod.MethodUse) || RoomLogic.IsProhibited(room, player, player, THIS_ANAL) != null)
                return false;

            if (player.HasFlag("Global_Dying")) return true;

            if (player.UsedTimes("Analeptic") <= Engine.CorrectCardTarget(room, TargetModSkill.ModType.Residue, player, THIS_ANAL))
                return true;

            if (Engine.CorrectCardTarget(room, TargetModSkill.ModType.SpecificAssignee, player, player, THIS_ANAL))
                return true;

            return false;
        }

        public override bool IsAvailable(Room room, Player player, WrappedCard card)
        {
            return IsAnalepticAvailable(room, player, card) && base.IsAvailable(room, player, card);
        }

        public override void OnUse(Room room, CardUseStruct use)
        {
            if (use.To.Count == 0)
                use.To.Add(use.From);
            base.OnUse(room, use);
        }
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            room.SetEmotion(effect.To, "analeptic");

            if (effect.To.HasFlag("Global_Dying") && room.GetRoomState().GetCurrentCardUseReason() != CardUseReason.CARD_USE_REASON_PLAY)
            {
                // recover hp
                RecoverStruct recover = new RecoverStruct
                {
                    Recover = 1,
                    Card = effect.Card,
                    Who = effect.From
                };
                room.Recover(effect.To, recover);
            }
            else
            {
                room.AddPlayerMark(effect.To, "drank");
            }
        }
        public override void CheckTargetModSkillShow(Room room, CardUseStruct use)
        {
            if (use.Card == null) return;

            WrappedCard card = use.Card;
            Player player = use.From;
            if (player.Phase != PlayerPhase.NotActive && player.UsedTimes(Name) > 1)
            {
                List<string> q = new List<string>();
                foreach (string name in room.Skills)
                {
                    if (Engine.GetSkill(name) is TargetModSkill skill)
                    {
                        if (player.UsedTimes(Name) <= Engine.CorrectCardTarget(room, TargetModSkill.ModType.Residue, player, card) + 1
                                && player.UsedTimes("Analeptic_" + name) < skill.GetResidueNum(room, player, card))
                            q.Add(skill.Name);

                        if (skill.CheckSpecificAssignee(room, player, player, card))
                            q.Add(skill.Name);
                    }
                }
                List<TriggerStruct> skills = new List<TriggerStruct>();
                foreach (string skill in q)
                {
                    skills.Add(new TriggerStruct(skill, player));
                }
                TriggerStruct r = room.AskForSkillTrigger(player, "declare_skill_invoke", skills, false, null, true);

                string skill_name = r.SkillName;
                string position = r.SkillPosition;


                string main = Engine.GetMainSkill(skill_name).Name;
                TargetModSkill result_skill = (TargetModSkill)Engine.GetSkill(skill_name);
                player.AddHistory("Analeptic_" + skill_name);

                room.ShowSkill(player, main, position);
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, main, position);
                room.BroadcastSkillInvoke(main, player.IsMale() ? "male" : "female", result_skill.GetEffectIndex(room, player, card, TargetModSkill.ModType.Residue), gsk.General, gsk.SkinId);
                room.NotifySkillInvoked(player, main);
            }
        }
    }
    #endregion

    #region 锦囊卡
    public class GodSalvation : GlobalEffect
    {
        public GodSalvation() : base("GodSalvation")
        { }

        public override bool IsCancelable(Room room, CardEffectStruct effect)
        {
            return effect.To.IsWounded() && base.IsCancelable(room, effect);
        }
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            room.SetEmotion(effect.From, "god_salvation");
            if (effect.To.IsWounded())
            {
                RecoverStruct recover = new RecoverStruct
                {
                    Recover = 1,
                    Card = effect.Card,
                    Who = effect.From
                };
                room.Recover(effect.To, recover);
            }
        }
    }
    class AmazingGrace : GlobalEffect
    {
        public AmazingGrace() : base("AmazingGrace")
        {
            has_preact = true;
        }

        public override void DoPreAction(Room room, WrappedCard card)
        {
            List<int> card_ids = room.GetNCards(room.GetAllPlayers().Count);
            room.SetTag(Name, card_ids);
            room.SetTag("AmazingGraceOrigin", new List<int>(card_ids));
            room.FillAG(Name, card_ids);
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            base.Use(room, card_use);
            ClearRestCards(room);
        }

        private void ClearRestCards(Room room)
        {
            room.ClearAG();
            List<int> ag_list =  (List<int>)room.GetTag(Name);
            room.RemoveTag("AmazingGraceOrigin");
            room.RemoveTag(Name);
            if (ag_list.Count == 0) return;

            CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_NATURAL_ENTER, null, Name, null);
            room.ThrowCard(ref ag_list, reason, null);
        }
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            room.SetEmotion(effect.From, "amazing_grace");
            List<int> card_ids = (List<int>)room.GetTag(Name);

            int card_id = room.AskForAG(effect.To, new List<int>(card_ids), false, Name);
            card_ids.Remove(card_id);

            room.TakeAG(effect.To, card_id);

            room.SetTag(Name, card_ids);
        }
    }
    public class SavageAssault : AOE
    {
        public SavageAssault() : base("SavageAssault") { }
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            room.SetEmotion(effect.From, "savage_assault");
            WrappedCard slash = room.AskForCard(effect.To, Name, "slash", "savage-assault-slash:" + effect.From.Name, effect, HandlingMethod.MethodResponse,
                effect.From.Alive ? effect.From : null);
            if (slash != null)
            {
                if (slash.Skill == "Spear")
                {
                    room.SetEmotion(effect.To, "spear");
                    Thread.Sleep(400);
                }

                FunctionCard fcard = Engine.GetFunctionCard(slash.Name);
                if (fcard != null && fcard is Slash)
                {
                    string color = WrappedCard.IsRed(RoomLogic.GetCardSuit(room, slash)) ? "red_" : "black_";
                    string type_str = (fcard is ThunderSlash) ? "thunder_" : (fcard is FireSlash) ? "fire_" : string.Empty;
                    room.SetEmotion(effect.To, string.Format("{0}{1}slash", color, type_str));
                }
            }
            else
            {
                room.Damage(new DamageStruct(effect.Card, effect.From.Alive ? effect.From : null, effect.To));
                Thread.Sleep(500);
            }
        }
    }
    public class ArcheryAttack : AOE
    {
        public ArcheryAttack() : base("ArcheryAttack") { }
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            room.SetEmotion(effect.From, "archery_attack");
            WrappedCard jink = room.AskForCard(effect.To, Name, "jink", "archery-attack-jink:" + effect.From.Name, effect, HandlingMethod.MethodResponse,
                effect.From.Alive ? effect.From : null);
            if (jink != null && jink.Skill != "EightDiagram")
                room.SetEmotion(effect.To, "jink");

            if (jink == null)
            {
                room.Damage(new DamageStruct(effect.Card, effect.From.Alive ? effect.From : null, effect.To));
                Thread.Sleep(500);
            }
            else if (effect.Card.Skill == "luanji" && room.Setting.GameMode == "Hegemony" && RoomLogic.IsFriendWith(room, effect.To, effect.From)
                && room.AskForSkillInvoke(effect.To, "luanji-draw", "#luanji-draw"))
            {
                room.DrawCards(effect.To, 1, "luanji");
            }
        }
    }
    public class Collateral : SingleTargetTrick
    {
        public Collateral() : base("Collateral") { }
        public override bool IsAvailable(Room room, Player player, WrappedCard card)
        {
            bool canUse = false;
            foreach (Player p in room.GetAlivePlayers())
            {
                if (p == player) continue;
                if (p.GetWeapon())
                {
                    canUse = true;
                    break;
                }
            }
            return canUse && base.IsAvailable(room, player, card);
        }

        public override bool TargetsFeasible(Room room, List<Player> selected, Player to_select, WrappedCard card)
        {
            return selected.Count == 2;
        }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            if (targets.Count > 0)
            {
                // @todo: fix this. We should probably keep the codes here, but change the code in
                // roomscene such that if it is collateral, then targetFilter's result is overriden
                if (targets.Count == 2) return false;
                Player slashFrom = targets[0];
                return RoomLogic.CanSlash(room, slashFrom, to_select);
            }
            else
            {
                if (!to_select.GetWeapon() || to_select == Self || Engine.IsProhibited(room, Self, to_select, card, targets) != null)
                    return false;

                foreach (Player p in room.GetAllPlayers())
                {
                    if (p == to_select) continue;
                    if (RoomLogic.CanSlash(room, to_select, p))
                        return true;
                }
            }
            return false;
        }

        public override void OnUse(Room room, CardUseStruct card_use)
        {
            Player killer = card_use.To[0];
            Player victim = card_use.To[1];

            card_use.To.RemoveAt(1);
            killer.SetTag("collateralVictim", victim.Name);

            base.OnUse(room, card_use);
        }

        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            Player source = effect.From;
            room.SetEmotion(source, "collateral");
            Player killer = effect.To;
            Player victim = room.FindPlayer((string)effect.To.GetTag("collateralVictim"), true);
            effect.To.RemoveTag("collateralVictim");
            if (victim == null) return;
            WrappedCard weapon = room.GetCard(killer.Weapon.Key);

            string prompt = string.Format("collateral-slash:{0}:{1}", victim.Name, source.Name);

            if (!victim.Alive)
            {
                if (source.Alive && killer.Alive && killer.GetWeapon())
                {
                    CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_GIVE, killer.Name);
                    room.ObtainCard(source, weapon, reason);
                }
            }
            else if (!source.Alive)
            {
                if (killer.Alive)
                    DoCollateral(room, killer, victim, prompt);
            }
            else
            {
                if (!killer.Alive)
                {
                    ; // do nothing
                }
                else if (!killer.GetWeapon())
                {
                    DoCollateral(room, killer, victim, prompt);
                }
                else
                {
                    if (!DoCollateral(room, killer, victim, prompt))
                    {
                        if (killer.GetWeapon())
                        {
                            CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_GIVE, killer.Name);
                            room.ObtainCard(source, weapon, reason);
                        }
                    }
                }
            }
        }

        private bool DoCollateral(Room room, Player killer, Player victim, string prompt)
        {
            bool useSlash = false;
            if (RoomLogic.CanSlash(room, killer, victim))
                useSlash = room.AskForUseSlashTo(killer, victim, prompt) != null;
            return useSlash;
        }
    }
    public class ExNihilo : SingleTargetTrick
    {

        public ExNihilo() : base("ExNihilo")
        {
            target_fixed = true;
        }

        public override void OnUse(Room room, CardUseStruct use)
        {
            if (use.To.Count == 0)
                use.To.Add(use.From);
            base.OnUse(room, use);
        }

        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            room.SetEmotion(effect.To, "ex_nihilo");
            room.DrawCards(effect.To, 2, Name);
        }
        public override bool IsAvailable(Room room, Player player, WrappedCard card)
        {
            return RoomLogic.IsProhibited(room, player, player, card) == null && base.IsAvailable(room, player, card);
        }
    }
    public class Duel : SingleTargetTrick
    {

        public Duel() : base("Duel") { }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            int total_num = 1 + Engine.CorrectCardTarget(room, TargetModSkill.ModType.ExtraMaxTarget, Self, card);
            if (targets.Count >= total_num)
                return false;
            if (to_select == Self)
                return false;

            return base.TargetFilter(room, targets, to_select, Self, card);
        }

        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            Player first = effect.To;
            Player second = effect.From;

            room.SetEmotion(first, "duel");
            room.SetEmotion(second, "duel");
            Thread.Sleep(400);

            Dictionary<Player, int> counts = new Dictionary<Player, int> { { first, 1 }, { second, 1 } };
            if (effect.EffectCount != null)
            {
                foreach (EffctCount count in effect.EffectCount)
                {
                    if (count.From == second && count.To == first)
                        counts[first] = count.Count;
                    if (count.From == first && count.To == second)
                        counts[second] = count.Count;
                }
            }

            bool stop = false;
            do
            {
                if (!first.Alive)
                    break;
                int count = counts[first];
                while (count > 0)
                {
                    WrappedCard slash = room.AskForCard(first, Name, "slash", string.Format("duel-slash:{0}::{1}", second.Name, count), effect, HandlingMethod.MethodResponse, second);
                    count--;
                    if (slash == null)
                    {
                        stop = true;
                        break;
                    }
                }
                if (!stop)
                    Swap(ref first, ref second);
            }
            while (!stop);

            DamageStruct damage = new DamageStruct(effect.Card, second.Alive ? second : null, first);
            if (second != effect.From)
                damage.ByUser = false;
            room.Damage(damage);
        }

        private void Swap(ref Player a, ref Player b)
        {
            Player c = a;
            a = b;
            b = c;
        }
    }
    public class Indulgence : DelayedTrick
    {
        public Indulgence() : base("Indulgence")
        {
            judge.Pattern = ".|heart";
            judge.Good = false;
            judge.PlayAnimation = true;
            judge.Reason = "Indulgence";
        }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return !RoomLogic.PlayerContainsTrick(room, to_select, Name) && base.TargetFilter(room, targets, to_select, Self, card);
        }
        public override void TakeEffect(Room room, Player target, WrappedCard card)
        {
            target.ClearHistory();
            room.SetEmotion(target, "indulgence");
            Thread.Sleep(400);
            room.SkipPhase(target, PlayerPhase.Play);
        }
    }
    public class SupplyShortage : DelayedTrick
    {
        public SupplyShortage() : base("SupplyShortage")
        {
            judge.Pattern = ".|club";
            judge.Good = false;
            judge.PlayAnimation = true;
            judge.Reason = "SupplyShortage";
        }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            if (RoomLogic.PlayerContainsTrick(room, to_select, Name) || !base.TargetFilter(room, targets, to_select, Self, card))
                return false;

            bool correct_target = (!card.DistanceLimited || Engine.CorrectCardTarget(room, TargetModSkill.ModType.DistanceLimit, Self, to_select, card));
            return correct_target || RoomLogic.DistanceTo(room, Self, to_select, card) == 1;
        }

        public override void TakeEffect(Room room, Player target, WrappedCard card)
        {
            room.SetEmotion(target, "supplyshortage");
            Thread.Sleep(400);
            room.SkipPhase(target, PlayerPhase.Draw);
        }

        public override void CheckTargetModSkillShow(Room room, CardUseStruct use)
        {
            WrappedCard card = use.Card;
            if (card == null || !card.DistanceLimited) return;

            Player player = use.From;

            if (RoomLogic.DistanceTo(room, player, use.To[0], card) > 1)
            {
                List<string> tarmods = new List<string>();
                foreach (string name in room.Skills)
                {
                    if (Engine.GetSkill(name) is TargetModSkill tarmod)
                    {
                        if (tarmod.GetDistanceLimit(room, player, use.To[0], card))
                        {
                            Skill main_skill = Engine.GetMainSkill(tarmod.Name);
                            if (RoomLogic.PlayerHasShownSkill(room, player, main_skill) || !RoomLogic.PlayerHasSkill(room, player, main_skill.Name))
                            {
                                room.NotifySkillInvoked(player, main_skill.Name);
                                if (RoomLogic.PlayerHasSkill(room, player, main_skill.Name))
                                {
                                    GeneralSkin general = RoomLogic.GetGeneralSkin(room, player, main_skill.Name);
                                    room.BroadcastSkillInvoke(main_skill.Name, player.IsMale() ? "male" : "female",
                                                               tarmod.GetEffectIndex(room, player, card, TargetModSkill.ModType.DistanceLimit), general.General, general.SkinId);
                                }
                                return;
                            }
                            else if (!tarmods.Contains(tarmod.Name))
                                tarmods.Add(tarmod.Name);
                        }
                    }
                }
                if (tarmods.Count > 0)
                {
                    List<TriggerStruct> skills = new List<TriggerStruct>();
                    foreach (string skill in tarmods)
                    {
                        skills.Add(new TriggerStruct(skill, player));
                    }
                    TriggerStruct r = room.AskForSkillTrigger(player, "declare_skill_invoke", skills, false, null, true);
                    string skill_name = r.SkillName;
                    TargetModSkill result_skill = (TargetModSkill)Engine.GetSkill(skill_name);
                    string position = r.SkillPosition;
                    Skill main = Engine.GetMainSkill(skill_name);
                    room.ShowSkill(player, main.Name, position);
                    room.NotifySkillInvoked(player, main.Name);
                    GeneralSkin general = RoomLogic.GetGeneralSkin(room, player, main.Name);
                    room.BroadcastSkillInvoke(main.Name, player.IsMale() ? "male" : "female",
                                               result_skill.GetEffectIndex(room, player, card, TargetModSkill.ModType.DistanceLimit), general.General, general.SkinId);
                }
            }
        }
    }
    public class Lightning : DelayedTrick
    {
        public Lightning() : base("Lightning", true)
        {
            target_fixed = true;
            judge.Pattern = ".|spade|2~9";
            judge.Good = true;
            judge.PlayAnimation = true;
            judge.Reason = "Lightning";
        }

        public override void OnUse(Room room, CardUseStruct use)
        {
            if (use.To.Count == 0)
                use.To = new List<Player> { use.From };
            base.OnUse(room, use);
        }
        public override bool IsAvailable(Room room, Player player, WrappedCard card)
        {
            return RoomLogic.IsProhibited(room, player, player, card) == null && base.IsAvailable(room, player, card);
        }

        public override void TakeEffect(Room room, Player target, WrappedCard card)
        {
            room.SetEmotion(target, "lightning");
            Thread.Sleep(400);
            room.Damage(new DamageStruct(card, null, target, 3, DamageStruct.DamageNature.Thunder));
        }
    }
    public class Nullification : SingleTargetTrick
    {

        public Nullification() : base("Nullification")
        {
            target_fixed = true;
        }
        public override void OnUse(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            string[] infos = Regex.Split(card_use.Pattern, "->", RegexOptions.IgnoreCase);

            Player from = room.FindPlayer(infos[0], true);
            Player to = room.FindPlayer(infos[2]);
            WrappedCard trick = RoomLogic.ParseCard(room, infos[1]);

            List<int> used_cards = new List<int>(card_use.Card.SubCards);
            List<CardsMoveStruct> moves = new List<CardsMoveStruct>();

            object data = card_use;
            RoomThread thread = room.RoomThread;
            thread.Trigger(TriggerEvent.PreCardUsed, room, player, ref data);
            card_use = (CardUseStruct)data;

            CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_USE, player.Name, null, card_use.Card.Skill, null)
            {
                CardString = RoomLogic.CardToString(room, card_use.Card),
                General = RoomLogic.GetGeneralSkin(room, player, card_use.Card.Skill, card_use.Card.SkillPosition)
            };

            if (card_use.To.Count == 1)
                reason.TargetId = card_use.To[0].Name;

            foreach (int id in used_cards)
            {
                CardsMoveStruct move = new CardsMoveStruct(id, null, Place.PlaceTable, reason);
                moves.Add(move);
            }
            room.MoveCardsAtomic(moves, true);
            if (used_cards.Count == 0)
            {                                                                                 //show virtual card on table
                CardsMoveStruct move = new CardsMoveStruct(-1, card_use.From, Place.PlaceTable, reason);
                room.NotifyUsingVirtualCard(RoomLogic.CardToString(room, card_use.Card), move);
            }

            List<Player> tos = new List<Player>();
            FunctionCard ftrick = Engine.GetFunctionCard(trick.Name);
            if (this is HegNullification && !(ftrick is Nullification) && ftrick.IsNDTrick() && to.HasShownOneGeneral() && to.Role != "careerist")
            {
                string trick_str = RoomLogic.CardToString(room, trick);
                string key = "targets" + trick_str;
                List<Player> targets = room.ContainsTag(key) ? (List<Player>)room.GetTag(key) : new List<Player>();
                List<Player> targets_copy = new List<Player>(targets);
                targets.Remove(to);
                foreach (Player p in targets_copy)
                    if (targets_copy.IndexOf(p) < targets_copy.IndexOf(to))
                        targets.Remove(p);

                foreach (Player p in targets)
                    if (p.HasShownOneGeneral() && p.Role != "careerist" && p.Kingdom == to.Kingdom)
                        tos.Add(p);

                if (tos.Count > 0)
                {
                    CardEffectStruct trickEffect = new CardEffectStruct
                    {
                        Card = trick,
                        From = from,
                        To = to
                    };
                    object _data = trickEffect;
                    List<string> des = new List<string> { string.Format("@HegNullification:::{0}",
                        trick.Name), "@HegNullification-single:" + to.Name,
                        string.Format("@HegNullification-all:{0}::{1}", to.Name, to.Kingdom) };
                    string heg_nullification_selection = room.AskForChoice(player, "HegNullification", "single+all", des, _data);

                    if (heg_nullification_selection.Contains("all"))
                        room.HegNull = true;
                    else
                        tos.Clear();
                }
            }

            LogMessage log = new LogMessage
            {
                Type = "#nullification",
                From = player.Name,
                Card_str = RoomLogic.CardToString(room, card_use.Card),
                Arg = RoomLogic.CardToString(room, trick),
                Arg2 = infos[1]
            };
            if (!(ftrick is Nullification))
            {
                tos.Add(to);
                log.To = new List<string>();
                foreach (Player p in tos)
                    log.To.Add(p.Name);
            }
            room.SendLog(log);

            thread.Trigger(TriggerEvent.CardUsedAnnounced, room, player, ref data);
            thread.Trigger(TriggerEvent.CardTargetAnnounced, room, player, ref data);

            thread.Trigger(TriggerEvent.CardUsed, room, player, ref data);
            thread.Trigger(TriggerEvent.CardFinished, room, player, ref data);
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            //notify focus
            room.FocusAll(1000);

            // does nothing, just throw it
            List<int> table_cardids = room.GetCardIdsOnTable(card_use.Card);
            if (table_cardids.Count > 0)
            {
                CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_USE, card_use.From.Name)
                {
                    CardString = RoomLogic.CardToString(room, card_use.Card)
                };
                CardsMoveStruct move = new CardsMoveStruct(table_cardids, card_use.From, null, Place.PlaceTable, Place.DiscardPile, reason);
                room.MoveCardsAtomic(new List<CardsMoveStruct> { move }, true);
            }
        }
        public override bool IsAvailable(Room room, Player player, WrappedCard card)
        {
            if (room.GetRoomState().GetCurrentCardUseReason() == CardUseReason.CARD_USE_REASON_RESPONSE_USE)
                return room.GetRoomState().GetCurrentCardUsePattern().Contains("Nullification") && RoomLogic.IsProhibited(room, player, null, card) == null;

            return false;
        }
    }
    public class HegNullification : Nullification
    {
        public HegNullification()
        {
            card_name = "HegNullification";
        }
    }
    public class Snatch : SingleTargetTrick
    {
        public Snatch() : base("Snatch") { }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            int total_num = 1 + Engine.CorrectCardTarget(room, TargetModSkill.ModType.ExtraMaxTarget, Self, card);
            if (targets.Count >= total_num)
                return false;

            if (to_select.IsAllNude() || !RoomLogic.CanGetCard(room, Self, to_select, "hej") || !base.TargetFilter(room, targets, to_select, Self, card))
                return false;

            bool correct_target = (!card.DistanceLimited || Engine.CorrectCardTarget(room, TargetModSkill.ModType.DistanceLimit, Self, to_select, card));

            return correct_target || RoomLogic.DistanceTo(room, Self, to_select, card) == 1;
        }
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            if (!effect.From.Alive)
                return;
            if (effect.To.IsAllNude())
                return;

            room.SetEmotion(effect.To, "snatch");
            if (!RoomLogic.CanGetCard(room, effect.From, effect.To, "hej"))
                return;

            int card_id = room.AskForCardChosen(effect.From, effect.To, "hej", Name, false, HandlingMethod.MethodGet);
            CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_EXTRACTION, effect.From.Name, Name, Name);
            room.ObtainCard(effect.From, room.GetCard(card_id), reason, false);
        }

        public override void CheckTargetModSkillShow(Room room, CardUseStruct use)
        {
            WrappedCard card = use.Card;
            if (card == null) return;

            Player player = use.From;
            List<string> tarmods = new List<string>();

            if (card.DistanceLimited && RoomLogic.DistanceTo(room, player, use.To[0], card) > 1)
            {
                foreach (string name in room.Skills)
                {
                    if (Engine.GetSkill(name) is TargetModSkill tarmod)
                    {
                        if (tarmod.GetDistanceLimit(room, player, use.To[0], card))
                        {
                            Skill main_skill = Engine.GetMainSkill(tarmod.Name);
                            if (RoomLogic.PlayerHasShownSkill(room, player, main_skill) || !RoomLogic.PlayerHasSkill(room, player, main_skill.Name))
                            {
                                room.NotifySkillInvoked(player, main_skill.Name);
                                if (RoomLogic.PlayerHasSkill(room, player, main_skill.Name))
                                {
                                    GeneralSkin general = RoomLogic.GetGeneralSkin(room, player, main_skill.Name);
                                    room.BroadcastSkillInvoke(main_skill.Name, player.IsMale() ? "male" : "female",
                                                               tarmod.GetEffectIndex(room, player, card, TargetModSkill.ModType.DistanceLimit), general.General, general.SkinId);
                                }
                                return;
                            }
                            else if (!tarmods.Contains(tarmod.Name))
                                tarmods.Add(tarmod.Name);
                        }
                    }
                }
                if (tarmods.Count > 0)
                {
                    List<TriggerStruct> skills = new List<TriggerStruct>();
                    foreach (string skill in tarmods)
                    {
                        skills.Add(new TriggerStruct(skill, player));
                    }
                    TriggerStruct r = room.AskForSkillTrigger(player, "declare_skill_invoke", skills, false, null, true);
                    string skill_name = r.SkillName;
                    TargetModSkill result_skill = (TargetModSkill)Engine.GetSkill(skill_name);
                    string position = r.SkillPosition;
                    Skill main = Engine.GetMainSkill(skill_name);
                    room.ShowSkill(player, main.Name, position);
                    room.NotifySkillInvoked(player, main.Name);
                    GeneralSkin general = RoomLogic.GetGeneralSkin(room, player, main.Name);
                    room.BroadcastSkillInvoke(main.Name, player.IsMale() ? "male" : "female",
                                               result_skill.GetEffectIndex(room, player, card, TargetModSkill.ModType.DistanceLimit), general.General, general.SkinId);
                }
            }
        }
    }
    public class Dismantlement : SingleTargetTrick
    {
        public Dismantlement() : base("Dismantlement") { }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            int total_num = 1 + Engine.CorrectCardTarget(room, TargetModSkill.ModType.ExtraMaxTarget, Self, card);
            if (targets.Count >= total_num)
                return false;

            if (to_select.IsAllNude())
                return false;

            if (to_select == Self)
                return false;

            if (!RoomLogic.CanDiscard(room, Self, to_select, "hej"))
                return false;
            return base.TargetFilter(room, targets, to_select, Self, card);
        }
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            if (!effect.From.Alive)
                return;

            room.SetEmotion(effect.From, "dismantlement");
            if (!RoomLogic.CanDiscard(room, effect.From, effect.To, "hej"))
                return;

            int card_id = room.AskForCardChosen(effect.From, effect.To, "hej", Name, false, HandlingMethod.MethodDiscard);
            room.ThrowCard(card_id, room.GetCardPlace(card_id) == Place.PlaceDelayedTrick ? null : effect.To, effect.From);
        }
    }
    class IronChain : TrickCard
    {
        public IronChain() : base("IronChain") { }
        public override string GetSubtype() => "damage_spread";
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            int total_num = 2 + Engine.CorrectCardTarget(room, TargetModSkill.ModType.ExtraMaxTarget, Self, card);
            if (targets.Count >= total_num)
                return false;
            if (RoomLogic.IsCardLimited(room, Self, card, HandlingMethod.MethodUse))
                return false;

            return Engine.IsProhibited(room, Self, to_select, card, targets) == null;
        }
        public override bool TargetsFeasible(Room room, List<Player> selected, Player Self, WrappedCard card)
        {
            bool rec = CanRecast(room, Self, card);

            if (rec && RoomLogic.IsCardLimited(room, Self, card, HandlingMethod.MethodUse))
                return selected.Count == 0;
            int total_num = 2 + Engine.CorrectCardTarget(room, TargetModSkill.ModType.ExtraMaxTarget, Self, card);
            if (selected.Count > total_num)
                return false;
            return rec || selected.Count > 0;
        }
        public override void OnUse(Room room, CardUseStruct use)
        {
            if (use.To == null || use.To.Count == 0)
                DoRecast(room, use);
            else
                base.OnUse(room, use);
        }
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            if (!RoomLogic.CanBeChainedBy(room, effect.To, effect.From))
                return;

            room.SetPlayerChained(effect.To, !effect.To.Chained);
        }
    }
    public class FireAttack : SingleTargetTrick
    {
        public FireAttack() : base("FireAttack") { }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            int total_num = 1 + Engine.CorrectCardTarget(room, TargetModSkill.ModType.ExtraMaxTarget, Self, card);
            if (targets.Count >= total_num || !base.TargetFilter(room, targets, to_select, Self, card) || to_select.IsKongcheng())
                return false;

            if (to_select == Self)
                return !Self.IsLastHandCard(card, true);
            else
                return true;
        }
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            if (effect.To.IsKongcheng()) return;

            int id = room.AskForCardShow(effect.To, effect.From, Name, effect);
            room.SetEmotion(effect.From, "fire_attack");
            room.ShowCard(effect.To, id, Name);

            string suit_str = WrappedCard.GetSuitString(RoomLogic.GetCardSuit(room, room.GetCard(id)));
            string pattern = string.Format(".{0}", suit_str.Substring(0, 1).ToUpper());
            string prompt = string.Format("@fire-attack:{0}::{1}", effect.To.Name, suit_str);
            if (effect.From.Alive)
            {
                WrappedCard card_to_throw = room.AskForCard(effect.From, Name, pattern, prompt, effect);
                if (card_to_throw != null)
                {
                    room.Damage(new DamageStruct(effect.Card, effect.From, effect.To, 1, DamageStruct.DamageNature.Fire));
                }
                else
                {
                    effect.From.SetFlags("FireAttackFailedPlayer_" + effect.To.Name); // For AI
                    effect.From.SetFlags("FireAttackFailed_" + suit_str); // For AI
                }
            }
        }
    }
    public class AwaitExhausted : TrickCard
    {
        public AwaitExhausted() : base("AwaitExhausted")
        {
            target_fixed = true;
        }
        public override string GetSubtype() => "await_exhausted";
        public override bool IsAvailable(Room room, Player player, WrappedCard card)
        {
            bool canUse = false;
            if (RoomLogic.IsProhibited(room, player, player, card) == null)
                canUse = true;
            if (!canUse)
            {
                List<Player> players = room.GetAlivePlayers();
                foreach (Player p in players)
                {
                    if (p == player) continue;
                    if (RoomLogic.IsProhibited(room, player, p, card) != null)
                        continue;
                    if (RoomLogic.IsFriendWith(room, player, p))
                    {
                        canUse = true;
                        break;
                    }
                }
            }

            return canUse && base.IsAvailable(room, player, card);
        }
        public override void OnUse(Room room, CardUseStruct use)
        {
            if (RoomLogic.IsProhibited(room, use.From, use.From, use.Card) == null)
                use.To.Add(use.From);
            foreach (Player p in room.GetOtherPlayers(use.From))
            {
                if (RoomLogic.IsFriendWith(room, p, use.From))
                {
                    Skill skill = RoomLogic.IsProhibited(room, use.From, p, use.Card);
                    if (skill != null)
                    {
                        LogMessage log = new LogMessage
                        {
                            Type = "#SkillAvoid",
                            From = p.Name,
                            Arg = skill.Name,
                            Arg2 = Name
                        };
                        room.SendLog(log);

                        room.BroadcastSkillInvoke(skill.Name, p);
                    }
                    else
                    {
                        use.To.Add(p);
                    }
                }
            }
            base.OnUse(room, use);
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            List<Player> targets = card_use.To;
            List<string> nullified_list = room.ContainsTag("CardUseNullifiedList") ? (List<string>)room.GetTag("CardUseNullifiedList") : new List<string>();
            bool all_nullified = nullified_list.Contains("_ALL_TARGETS");
            foreach (Player target in targets)
            {
                CardEffectStruct effect = new CardEffectStruct
                {
                    Card = card_use.Card,
                    From = card_use.From,
                    To = target,
                    Multiple = (targets.Count > 1),
                    Nullified = (all_nullified || nullified_list.Contains(target.Name))
                };

                List<Player> players = new List<Player>();
                for (int i = targets.IndexOf(target); i < targets.Count; i++)
                {
                    if (!nullified_list.Contains(targets[i].Name) && !all_nullified)
                        players.Add(targets[i]);
                }

                room.CardEffect(effect);
            }

            foreach (Player target in targets)
            {
                if (target.HasFlag("AwaitExhaustedEffected"))
                {
                    target.SetFlags("-AwaitExhaustedEffected");
                    room.AskForDiscard(target, Name, 2, 2, false, true);
                }
            }

            List<int> table_cardids = room.GetCardIdsOnTable(card_use.Card);
            if (table_cardids.Count > 0)
            {
                CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_USE, card_use.From.Name, null, card_use.Card.Skill, null)
                {
                    CardString = RoomLogic.CardToString(room, card_use.Card)
                };
                if (targets.Count == 1) reason.TargetId = targets[0].Name;

                CardsMoveStruct move = new CardsMoveStruct(table_cardids, card_use.From, null, Place.PlaceTable, Place.DiscardPile, reason);
                room.MoveCardsAtomic(new List<CardsMoveStruct> { move }, true);
            }
        }
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            room.DrawCards(effect.To, new DrawCardStruct(2, effect.From, Name));
            effect.To.SetFlags("AwaitExhaustedEffected");
        }
    }
    public class KnownBoth : SingleTargetTrick
    {
        public KnownBoth() : base("KnownBoth") { }
        public override bool IsAvailable(Room room, Player player, WrappedCard card)
        {
            bool can_use = false;
            foreach (Player p in room.GetAlivePlayers())
            {
                if (p == player) continue;
                if (RoomLogic.IsProhibited(room, player, p, card) != null)
                    continue;
                if (p.IsKongcheng() && p.HasShownAllGenerals())
                    continue;
                can_use = true;
                break;
            }

            return (can_use && !RoomLogic.IsCardLimited(room, player, card, HandlingMethod.MethodUse)) || CanRecast(room, player, card);
        }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            int total_num = 1 + Engine.CorrectCardTarget(room, TargetModSkill.ModType.ExtraMaxTarget, Self, card);
            if (targets.Count >= total_num || to_select == Self || !base.TargetFilter(room, targets, to_select, Self, card))
                return false;

            return !to_select.IsKongcheng() || !to_select.HasShownAllGenerals();
        }

        public override bool TargetsFeasible(Room room, List<Player> selected, Player to_select, WrappedCard card)
        {
            bool rec = (room.GetRoomState().GetCurrentCardUseReason() == CardUseReason.CARD_USE_REASON_PLAY) && card.CanRecast
                && !RoomLogic.IsCardLimited(room, to_select, card, HandlingMethod.MethodRecast);
            List<int> sub = card.SubCards, hand_cards = new List<int>(to_select.HandCards);
            foreach (int id in sub)
            {
                if (!hand_cards.Contains(id))
                {
                    rec = false;
                    break;
                }
            }

            if (RoomLogic.IsCardLimited(room, to_select, card, HandlingMethod.MethodUse))
                return rec && selected.Count == 0;
            int total_num = 1 + Engine.CorrectCardTarget(room, TargetModSkill.ModType.ExtraMaxTarget, to_select, card);
            if (selected.Count > total_num)
                return false;
            return selected.Count > 0 || rec;
        }
        public override void OnUse(Room room, CardUseStruct use)
        {
            if (use.To.Count == 0)
                DoRecast(room, use);
            else
                base.OnUse(room, use);
        }
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            List<string> choices = new List<string>();
            if (!effect.To.IsKongcheng())
                choices.Add("handcards");
            if (!effect.To.General1Showed)
                choices.Add("head_general");
            if (!string.IsNullOrEmpty(effect.To.General2) && !effect.To.General2Showed)
                choices.Add("deputy_general");

            effect.To.SetFlags("KnownBothTarget");// For AI
            string choice = room.AskForChoice(effect.From, Name, string.Join("+", choices), null, effect.To);
            effect.To.SetFlags("-KnownBothTarget");

            if (choice == "handcards")
                room.ShowAllCards(effect.To, effect.From, Name);
            else
            {
                string general = choice == "head_general" ? effect.To.ActualGeneral1 : effect.To.ActualGeneral2;
                LogMessage log = new LogMessage
                {
                    Type = "$KnownBothViewGeneral",
                    From = effect.From.Name,
                    To = new List<string> { effect.To.Name },
                    Arg = choice,
                    Arg2 = general
                };
                room.SendLog(log, effect.From);

                LogMessage log2 = new LogMessage
                {
                    Type = "#KnownBothView",
                    From = effect.From.Name,
                    To = new List<string> { effect.To.Name },
                    Arg = choice
                };
                room.SendLog(log2, new List<Player> { effect.From });

                room.ViewGenerals(effect.From, new List<string> { general }, Name);
            }
        }
    }
    public class BefriendAttacking : SingleTargetTrick
    {
        public BefriendAttacking() : base("BefriendAttacking") { }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            int total_num = 1 + Engine.CorrectCardTarget(room, TargetModSkill.ModType.ExtraMaxTarget, Self, card);
            if (targets.Count >= total_num || !base.TargetFilter(room, targets, to_select, Self, card))
                return false;

            return to_select.HasShownOneGeneral() && !RoomLogic.IsFriendWith(room, Self, to_select);
        }
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            room.DrawCards(effect.To, new DrawCardStruct(1, effect.From, Name));
            room.DrawCards(effect.From, new DrawCardStruct(3, effect.From, Name));
        }
        public override bool IsAvailable(Room room, Player player, WrappedCard card)
        {
            return player.HasShownOneGeneral() && base.IsAvailable(room, player, card);
        }
    }
    #endregion

    #region 装备卡
    public class CrossBow : Weapon
    {
        public CrossBow() : base("CrossBow", 1) { }
    }
    public class DoubleSword : Weapon
    {
        public DoubleSword() : base("DoubleSword", 2) { }
    }
    public class DoubleSwordSkill : WeaponSkill
    {
        public DoubleSwordSkill() : base("DoubleSwordSkill")
        {
            events = new List<TriggerEvent> { TriggerEvent.TargetChosen };
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            CardUseStruct use = (CardUseStruct)data;
            if (!base.Triggerable(player, room))
                return new TriggerStruct();

            if (use.Card != null)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is Slash)
                {
                    List<Player> targets = new List<Player>();
                    foreach (Player to in use.To)
                    {
                        if (GenderDiff(player, to))
                            targets.Add(to);
                    }
                    if (targets.Count > 0)
                        return new TriggerStruct(Name, player, targets);
                }
            }
            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (ask_who != null && room.AskForSkillInvoke(ask_who, Name, player))
            {
                room.SetEmotion(ask_who, "doublesword");
                Thread.Sleep(400);
                return info;
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player skill_target, ref object data, Player ask_who, TriggerStruct info)
        {
            bool draw_card = false;
            if (!RoomLogic.CanDiscard(room, skill_target, skill_target, "h"))
                draw_card = true;
            else
            {
                string prompt = "double-sword-card:" + ask_who.Name;
                if (!room.AskForDiscard(skill_target, Name, 1, 1, true, false, prompt))
                    draw_card = true;
            }
            if (draw_card)
                room.DrawCards(ask_who, 1, Name);
            return false;
        }

        private static bool GenderDiff(Player a, Player b)
        {
            return (a.IsMale() && b.IsFemale()) || (a.IsFemale() && b.IsMale());
        }
    }
    public class QinggangSword : Weapon
    {
        public QinggangSword() : base("QinggangSword", 2) { }
    }
    public class QinggangSwordSkill : WeaponSkill
    {
        public QinggangSwordSkill() : base("QinggangSword")
        {
            events = new List<TriggerEvent> { TriggerEvent.TargetChosen };
            frequency = Frequency.Compulsory;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            CardUseStruct use = (CardUseStruct)data;
            if (!base.Triggerable(player, room))
                return new TriggerStruct();

            if (use.Card != null)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is Slash && use.To.Count > 0)
                    return new TriggerStruct(Name, player, use.To);
            }
            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SetEmotion(ask_who, "qinggangsword");
            Thread.Sleep(400);
            return info;
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player target, ref object data, Player ask_who, TriggerStruct info)
        {
            CardUseStruct use = (CardUseStruct)data;
            target.AddQinggangTag(RoomLogic.CardToString(room, use.Card));
            return false;
        }
    }
    public class Spear : Weapon
    {
        public Spear() : base("Spear", 3) { }
    }
    public class SpearSkill : ViewAsSkill
    {
        public SpearSkill() : base("Spear")
        {
            response_or_use = true;
        }

        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return Slash.IsAvailable(room, player)
                && player.GetMark("Equips_nullified_to_Yourself") == 0;
        }
        public override bool IsEnabledAtResponse(Room room, Player player, string pattern)
        {
            return pattern == "Slash" && player.GetMark("Equips_nullified_to_Yourself") == 0;
        }
        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return selected.Count < 2 && !player.HasEquip(to_select.Name);
        }
        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count != 2)
                return null;

            WrappedCard slash = new WrappedCard("Slash");
            slash.AddSubCards(cards);
            slash.Skill = Name;
            slash = RoomLogic.ParseUseCard(room, slash);
            return slash;
        }
    }
    public class Axe : Weapon
    {
        public Axe() : base("Axe", 3) { }
    }
    public class AxeSkill : WeaponSkill
    {
        public AxeSkill() : base("Axe")
        {
            events = new List<TriggerEvent> { TriggerEvent.SlashMissed };
            view_as_skill = new AxeViewAsSkill();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            SlashEffectStruct effect = (SlashEffectStruct)data;

            if (!effect.To.Alive || effect.To.GetMark("Equips_of_Others_nullified_to_You") > 0)
                return false;

            WrappedCard card = null;
            if (player.GetCardCount(true) >= 3) // Need 2 more cards except from the weapon itself
                card = room.AskForCard(player, Name, "@@Axe", "@Axe:" + effect.To.Name, data, Name);
            if (card != null)
            {
                room.SetEmotion(player, "axe");
                Thread.Sleep(400);
                room.SlashResult(effect, null);
            }

            return false;
        }
    }
    public class AxeViewAsSkill : ViewAsSkill
    {
        public AxeViewAsSkill() : base("Axe")
        {
            response_pattern = "@@Axe";
        }
        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return selected.Count < 2 && to_select.Name != player.Weapon.Value && !RoomLogic.IsJilei(room, player, to_select);
        }
        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count != 2)
                return null;

            WrappedCard card = new WrappedCard("DummyCard")
            {
                Skill = Name
            };
            card.AddSubCards(cards);
            return card;
        }
    }
    public class KylinBow : Weapon
    {
        public KylinBow() : base("KylinBow", 5) { }
    }
    public class KylinBowSkill : WeaponSkill
    {
        public KylinBowSkill() : base("KylinBow")
        {
            events = new List<TriggerEvent> { TriggerEvent.DamageCaused };
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            DamageStruct damage = (DamageStruct)data;

            List<string> horses = new List<string>();
            if (damage.Card != null && damage.ByUser && !damage.Chain && !damage.Transfer
                && damage.To.GetMark("Equips_of_Others_nullified_to_You") == 0)
            {
                FunctionCard fcard = Engine.GetFunctionCard(damage.Card.Name);
                if (fcard is Slash)
                {
                    if (damage.To.GetDefensiveHorse() && RoomLogic.CanDiscard(room, damage.From, damage.To, damage.To.DefensiveHorse.Key))
                        horses.Add("dhorse");
                    if (damage.To.GetOffensiveHorse() && RoomLogic.CanDiscard(room, damage.From, damage.To, damage.To.OffensiveHorse.Key))
                        horses.Add("ohorse");
                    if (damage.To.GetSpecialEquip() && RoomLogic.CanDiscard(room, damage.From, damage.To, damage.To.Special.Key))
                        horses.Add("shorse");

                    if (horses.Count == 0)
                        return false;

                    if (player == null) return false;
                    if (!room.AskForSkillInvoke(player, Name, data))
                        return false;

                    room.SetEmotion(player, "kylinbow");
                    Thread.Sleep(400);

                    string horse_type = room.AskForChoice(player, Name, string.Join("+", horses));

                    if (horse_type == "dhorse")
                        room.ThrowCard(damage.To.DefensiveHorse.Key, damage.To, damage.From);
                    else if (horse_type == "ohorse")
                        room.ThrowCard(damage.To.OffensiveHorse.Key, damage.To, damage.From);
                    else
                        room.ThrowCard(damage.To.Special.Key, damage.To, damage.From);
                }
            }

            return false;
        }
    }
    public class EightDiagram : Armor
    {
        public EightDiagram() : base("EightDiagram") { }
    }
    public class EightDiagramSkill : ArmorSkill
    {
        public EightDiagramSkill() : base("EightDiagram")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardAsked };
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (!base.Triggerable(player, room)) return new TriggerStruct();
            string pattern = ((List<string>)data)[0];
            WrappedCard jink = new WrappedCard("Jink");
            FunctionCard fcard = Engine.GetFunctionCard(jink.Name);
            if (Engine.MatchExpPattern(room, pattern, player, jink) && fcard.IsAvailable(room, player, jink))
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            return base.Cost(room, ref data, info);
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            int armor_id = -1;
            if (player.GetArmor())
            {
                armor_id = player.Armor.Key;
                room.SetCardFlag(armor_id, "using");
            }
            room.SetEmotion(player, "eightdiagram");
            JudgeStruct judge = new JudgeStruct
            {
                Pattern = ".|red",
                Good = true,
                PlayAnimation = true,
                Reason = Name,
                Who = player
            };

            room.Judge(ref judge);
            Thread.Sleep(500);
            if (armor_id != -1)
                room.SetCardFlag(armor_id, "-using");

            if (judge.IsGood())
            {
                WrappedCard jink = new WrappedCard("Jink");
                room.Provide(jink);

                return true;
            }

            return false;
        }
    }
    public class IceSword : Weapon
    {
        public IceSword() : base("IceSword", 2) { }
    }
    public class IceSwordSkill : WeaponSkill
    {
        public IceSwordSkill() : base("IceSword")
        {
            events = new List<TriggerEvent> { TriggerEvent.DamageCaused };
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            DamageStruct damage = (DamageStruct)data;

            if (damage.Card != null && damage.To.GetMark("Equips_of_Others_nullified_to_You") == 0
                && !damage.To.IsNude() && damage.ByUser
                && !damage.Chain && !damage.Transfer)
            {
                FunctionCard fcard = Engine.GetFunctionCard(damage.Card.Name);
                if (fcard is Slash && room.AskForSkillInvoke(player, Name, data))
                {
                    room.SetEmotion(player, "icesword");
                    Thread.Sleep(400);
                    if (RoomLogic.CanDiscard(room, damage.From, damage.To, "he"))
                    {
                        int card_id = room.AskForCardChosen(player, damage.To, "he", Name, false, FunctionCard.HandlingMethod.MethodDiscard);
                        room.ThrowCard(card_id, damage.To, damage.From);

                        if (damage.From.Alive && damage.To.Alive && RoomLogic.CanDiscard(room, damage.From, damage.To, "he"))
                        {
                            card_id = room.AskForCardChosen(player, damage.To, "he", Name, false, FunctionCard.HandlingMethod.MethodDiscard);
                            room.ThrowCard(card_id, damage.To, damage.From);
                        }
                    }
                    return true;
                }
            }
            return false;
        }
    }
    public class RenwangShield : Armor
    {
        public RenwangShield() : base("RenwangShield") { }
    }
    public class RenwangShieldSkill : ArmorSkill
    {
        public RenwangShieldSkill() : base("RenwangShield")
        {
            events = new List<TriggerEvent> { TriggerEvent.SlashEffected };
            frequency = Frequency.Compulsory;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (!base.Triggerable(player, room)) return new TriggerStruct();
            SlashEffectStruct effect = (SlashEffectStruct)data;
            if (WrappedCard.IsBlack(RoomLogic.GetCardSuit(room, effect.Slash))) return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(Room room, ref object data, TriggerStruct info)
        {
            return base.Cost(room, ref data, info);
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            SlashEffectStruct effect = (SlashEffectStruct)data;
            LogMessage log = new LogMessage
            {
                Type = "#Armornullify",
                From = player.Name,
                Arg = Name,
                Arg2 = effect.Slash.Name
            };
            room.SendLog(log);

            room.SetEmotion(player, "renwangshield");
            Thread.Sleep(400);
            effect.To.SetFlags("Global_NonSkillnullify");
            return true;
        }
    }
    public class Fan : Weapon
    {
        public Fan() : base("Fan", 4) { }
    }
    public class FanSkill : WeaponSkill
    {
        public FanSkill() : base("Fan")
        {
            events = new List<TriggerEvent> { TriggerEvent.CardUsedAnnounced };
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room))
            {
                CardUseStruct use = (CardUseStruct)data;
                if (use.Card != null && use.Card.Name == "Slash")
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(player, Name, data))
                return info;

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            CardUseStruct use = (CardUseStruct)data;
            LogMessage log = new LogMessage
            {
                Type = "$Fan",
                From = use.From.Name,
                Arg = use.Card.Name
            };

            WrappedCard fire = new WrappedCard("FireSlash");
            fire.AddSubCard(use.Card);
            fire = RoomLogic.ParseUseCard(room, fire);
            use.Card = fire;
            data = use;
            room.SetEmotion(player, "fan");
            Thread.Sleep(400);

            log.Card_str = RoomLogic.CardToString(room, fire);
            room.SendLog(log);

            return false;
        }
    }
    public class SixSwords : Weapon
    {
        public SixSwords() : base("SixSwords", 2) { }
    }
    public class SixSwordsSkill : AttackRangeSkill
    {
        public SixSwordsSkill() : base("SixSwords")
        {
        }
        public override int GetExtra(Room room, Player target, bool include_weapon)
        {
            foreach (Player p in room.GetAlivePlayers())
            {
                if (p.HasWeapon("SixSwords") && RoomLogic.IsFriendWith(room, p, target) && p.GetMark("Equips_nullified_to_Yourself") == 0)
                    return 1;
            }

            return 0;
        }
    }
    public class Triblade : Weapon
    {
        public Triblade() : base("Triblade", 3) { }
    }
    public class TribladeSkill : WeaponSkill
    {
        public TribladeSkill() : base("Triblade")
        {
            events = new List<TriggerEvent> { TriggerEvent.Damage };
            view_as_skill = new TribladeSkillVS();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            DamageStruct damage = (DamageStruct)data;
            if (!player.IsKongcheng() && damage.To != null && damage.To.Alive && damage.Card != null
                && damage.ByUser && !damage.Chain && !damage.Transfer)
            {
                FunctionCard fcard = Engine.GetFunctionCard(damage.Card.Name);
                if (fcard is Slash)
                {
                    List<Player> players = new List<Player>();
                    foreach (Player p in room.GetOtherPlayers(player))
                    {
                        if (RoomLogic.DistanceTo(room, damage.To, p) == 1)
                        {
                            players.Add(p);
                            p.SetFlags("TribladeCanBeSelected");
                        }
                    }
                    if (players.Count == 0)
                        return false;
                    room.AskForUseCard(player, "@@Triblade", "@Triblade");
                }

                foreach (Player p in room.GetAllPlayers())
                    if (p.HasFlag("TribladeCanBeSelected"))
                        p.SetFlags("-TribladeCanBeSelected");
            }

            return false;
        }
    }
    public class TribladeSkillVS : OneCardViewAsSkill
    {
        public TribladeSkillVS() : base("Triblade")
        {
            response_pattern = "@@Triblade";
            filter_pattern = ".|.|.|hand!";
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard c = new WrappedCard("TribladeSkillCard")
            {
                Skill = Name
            };
            c.AddSubCard(card);
            return c;
        }
    }
    public class TribladeSkillCard : SkillCard
    {

        public TribladeSkillCard() : base("TribladeSkillCard") { }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return targets.Count == 0 && to_select.HasFlag("TribladeCanBeSelected");
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            room.SetEmotion(card_use.From, "triblade");
            Thread.Sleep(400);
            room.Damage(new DamageStruct("Triblade", card_use.From, card_use.To[0]));
        }
    }
    public class Vine : Armor
    {
        public Vine() : base("Vine") { }
    }
    public class VineSkill : ArmorSkill
    {
        public VineSkill() : base("Vine")
        {
            events = new List<TriggerEvent> { TriggerEvent.DamageInflicted, TriggerEvent.SlashEffected, TriggerEvent.CardEffected };
            frequency = Frequency.Compulsory;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (!base.Triggerable(player, room)) return new TriggerStruct();
            if (triggerEvent == TriggerEvent.SlashEffected && data is SlashEffectStruct effect)
            {
                if (effect.Nature == DamageStruct.DamageNature.Normal)
                    return new TriggerStruct(Name, player);
            }
            else if (triggerEvent == TriggerEvent.CardEffected && data is CardEffectStruct card_effect)
            {
                if (card_effect.Card.Name == "SavageAssault" || card_effect.Card.Name == "ArcheryAttack")
                    return new TriggerStruct(Name, player);
            }
            else if (triggerEvent == TriggerEvent.DamageInflicted && data is DamageStruct damage)
            {
                if (damage.Nature == DamageStruct.DamageNature.Fire)
                    return new TriggerStruct(Name, player);
            }
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(Room room, ref object data, TriggerStruct info)
        {
            return base.Cost(room, ref data, info);
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.SlashEffected && data is SlashEffectStruct effect)
            {
                room.SetEmotion(player, "vine");
                Thread.Sleep(400);
                LogMessage log = new LogMessage
                {
                    From = player.Name,
                    Type = "#Armornullify",
                    Arg = Name,
                    Arg2 = effect.Slash.Name
                };
                room.SendLog(log);

                effect.To.SetFlags("Global_NonSkillnullify");
                return true;
            }
            else if (triggerEvent == TriggerEvent.CardEffected && data is CardEffectStruct card_effect)
            {
                room.SetEmotion(player, "vine");
                Thread.Sleep(400);
                LogMessage log = new LogMessage
                {
                    From = player.Name,
                    Type = "#Armornullify",
                    Arg = Name,
                    Arg2 = card_effect.Card.Name
                };
                room.SendLog(log);

                card_effect.To.SetFlags("Global_NonSkillnullify");
                return true;
            }
            else if (triggerEvent == TriggerEvent.DamageInflicted && data is DamageStruct damage)
            {
                room.SetEmotion(player, "vineburn");
                Thread.Sleep(400);
                LogMessage log = new LogMessage
                {
                    Type = "#VineDamage",
                    From = player.Name,
                    Arg = damage.Damage.ToString(),
                    Arg2 = (++damage.Damage).ToString()
                };
                room.SendLog(log);

                data = damage;
            }
            return false;
        }
    }
    public class SilverLion : Armor
    {
        public SilverLion() : base("SilverLion") { }
        public override void OnUninstall(Room room, Player player, WrappedCard card)
        {
            if (player.Alive && RoomLogic.HasArmorEffect(room, player, Name, false))
                player.SetFlags("SilverLionRecover");
        }
    }
    public class SilverLionSkill : ArmorSkill
    {
        public SilverLionSkill() : base("SilverLion")
        {
            events = new List<TriggerEvent> { TriggerEvent.DamageInflicted, TriggerEvent.CardsMoveOneTime };
            frequency = Frequency.Compulsory;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.DamageInflicted && data is DamageStruct damage)
            {
                if (base.Triggerable(player, room) && damage.Damage > 1)
                    return new TriggerStruct(Name, player);
            }
            else if (data is CardsMoveOneTimeStruct move && move.From != null && move.From_places.Contains(Place.PlaceEquip) && move.From.HasFlag("SilverLionRecover"))
            {
                for (int i = 0; i < move.Card_ids.Count; i++)
                {
                    if (move.From_places[i] != Place.PlaceEquip) continue;
                    WrappedCard card = Engine.GetRealCard(move.Card_ids[i]);
                    if (card.Name == Name)
                    {
                        if (!move.From.IsWounded())
                        {
                            move.From.SetFlags("-SilverLionRecover");
                            return new TriggerStruct();
                        }
                        return new TriggerStruct(Name, move.From);
                    }
                }
            }
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.CardsMoveOneTime) return info;
            return base.Cost(triggerEvent, room, player, ref data, ask_who, info);
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.DamageInflicted && data is DamageStruct damage)
            {
                room.SetEmotion(player, "silverlion");
                Thread.Sleep(400);
                LogMessage log = new LogMessage
                {
                    Type = "#SilverLion",
                    From = player.Name,
                    Arg = damage.Damage.ToString(),
                    Arg2 = Name
                };
                room.SendLog(log);

                damage.Damage = 1;
                data = damage;
            }
            else if (data is CardsMoveOneTimeStruct move)
            {
                for (int i = 0; i < move.Card_ids.Count; i++)
                {
                    if (move.From_places[i] != Place.PlaceEquip) continue;
                    WrappedCard card = Engine.GetRealCard(move.Card_ids[i]);
                    if (card.Name == Name)
                    {
                        move.From.SetFlags("-SilverLionRecover");

                        room.SetEmotion(move.From, "silverlion");
                        Thread.Sleep(400);
                        RecoverStruct recover = new RecoverStruct
                        {
                            Recover = 1,
                            Card = card
                        };
                        room.Recover(move.From, recover);

                        return false;
                    }
                }

            }
            return false;
        }
    }

    public class HorseSkill : DistanceSkill
    {
        public HorseSkill() :base("Horse")
        {
        }
        public override int GetCorrect(Room room, Player from, Player to, WrappedCard card = null)
        {
            int correct = 0;
            Horse horse = null;
            if (from.GetOffensiveHorse() && from.GetMark("Equips_nullified_to_Yourself") == 0
                    && (card == null || !card.SubCards.Contains(from.OffensiveHorse.Key)))
            {
                horse = (Horse)Engine.GetFunctionCard(from.OffensiveHorse.Value);
                if (horse != null) correct += horse.Correct;
            }
            if (to.GetDefensiveHorse() && to.GetMark("Equips_nullified_to_Yourself") == 0
                    && (card == null || !card.SubCards.Contains(to.DefensiveHorse.Key)))
            {
                horse = (Horse)Engine.GetFunctionCard(to.DefensiveHorse.Value);
                if (horse != null) correct += horse.Correct;
            }

            return correct;
        }
    }

    public class CompanionCard : SkillCard
    {
        public CompanionCard() : base("CompanionCard")
        {
            target_fixed = true;
        }
        public override WrappedCard Validate(Room room, CardUseStruct use)
        {
            room.SetPlayerMark(use.From, "@companion", 0);
            room.DetachSkillFromPlayer(use.From, "companion");
            CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_ANNOUNCE, use.From.Name, null, use.Card.Skill, null)
            {
                CardString = RoomLogic.CardToString(room, use.Card),
                General = RoomLogic.GetGeneralSkin(room, use.From, "companion", "head")
            };
            //show virtual card on table
            CardsMoveStruct move = new CardsMoveStruct(-1, use.From, Place.PlaceTable, reason)
            {
                From_place = Place.PlaceUnknown,
                From = use.From.Name,
                Is_last_handcard = false,
            };
            room.NotifyUsingVirtualCard(RoomLogic.CardToString(room, use.Card), move);
            Thread.Sleep(1000);

            WrappedCard peach = new WrappedCard("Peach")
            {
                Skill = "_comapnion"
            };
            FunctionCard fcard = Engine.GetFunctionCard(peach.Name);
            string choice = "draw";
            if (fcard.IsAvailable(room, use.From, peach))
            {
                choice += "+peach";
            }
            string result = room.AskForChoice(use.From, "companion", choice);
            if (result == "draw")
                return use.Card;
            else
            {
                return peach;
            }
        }
        public override WrappedCard ValidateInResponse(Room room, Player player, WrappedCard card)
        {
            CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_ANNOUNCE, player.Name, null, card.Skill, null)
            {
                CardString = RoomLogic.CardToString(room, card),
                General = RoomLogic.GetGeneralSkin(room, player, "companion", "head")
            };
            //show virtual card on table
            CardsMoveStruct move = new CardsMoveStruct(-1, player, Place.PlaceTable, reason)
            {
                From_place = Place.PlaceUnknown,
                From = player.Name,
                Is_last_handcard = false,
            };
            room.NotifyUsingVirtualCard(RoomLogic.CardToString(room, card), move);
            Thread.Sleep(1000);

            room.SetPlayerMark(player, "@companion", 0);
            room.DetachSkillFromPlayer(player, "companion");
            WrappedCard peach = new WrappedCard("Peach")
            {
                Skill = "_comapnion"
            };
            return peach;
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            room.DrawCards(card_use.From, 2, "comapnion");
        }
    }
    public class Companion : ZeroCardViewAsSkill
    {
        public Companion() : base("companion")
        {
        }
        public override bool IsAvailable(Room room, Player invoker, CardUseReason reason, string pattern, string position = null)
        {
            if (invoker.GetMark("@companion") > 0)
            {
                if (reason == CardUseReason.CARD_USE_REASON_PLAY)
                    return true;
                else if (reason == CardUseReason.CARD_USE_REASON_RESPONSE_USE)
                {
                    WrappedCard peach = new WrappedCard("Peach");
                    FunctionCard fcard = Engine.GetFunctionCard(peach.Name);
                    if (fcard.IsAvailable(room, invoker, peach) && Engine.MatchExpPattern(room, pattern, invoker, peach))
                        return true;
                }
            }
            return false;
        }
        public override WrappedCard ViewAs(Room room, Player player)
        {
            WrappedCard card = new WrappedCard("CompanionCard")
            {
                Skill = Name
            };
            return card;
        }
    }
    public class MegatamaCard : SkillCard
    {
        public MegatamaCard() : base("MegatamaCard")
        {
            target_fixed = true;
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            WrappedCard card = card_use.Card;
            CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_ANNOUNCE, player.Name, null, card.Skill, null)
            {
                CardString = RoomLogic.CardToString(room, card),
                General = RoomLogic.GetGeneralSkin(room, player, "magatama", "head")
            };
            //show virtual card on table
            CardsMoveStruct move = new CardsMoveStruct(-1, player, Place.PlaceTable, reason)
            {
                From_place = Place.PlaceUnknown,
                From = player.Name,
                Is_last_handcard = false,
            };
            room.NotifyUsingVirtualCard(RoomLogic.CardToString(room, card), move);
            Thread.Sleep(1000);

            room.SetPlayerMark(player, "@megatama", 0);
            room.DetachSkillFromPlayer(player, "megatama");
            if (player.Phase == PlayerPhase.Play)
                room.DrawCards(player, 1, "megatama");
            else
                player.SetFlags("megatama");
        }
    }
    public class MegatamaVS : ZeroCardViewAsSkill
    {
        public MegatamaVS() : base("megatama")
        {
        }
        public override bool IsAvailable(Room room, Player invoker, CardUseReason reason, string pattern, string position = null)
        {
            if (invoker.GetMark("@megatama") > 0)
            {
                if (reason == CardUseReason.CARD_USE_REASON_PLAY)
                    return true;
                else if (reason == CardUseReason.CARD_USE_REASON_RESPONSE_USE && pattern == "@@megatama")
                    return true;
            }

            return false;
        }
        public override WrappedCard ViewAs(Room room, Player player)
        {
            WrappedCard card = new WrappedCard("MegatamaCard")
            {
                Skill = Name
            };
            return card;
        }
    }
    public class Megatama : PhaseChangeSkill
    {
        public Megatama() : base("megatama")
        {
            global = true;
            view_as_skill = new MegatamaVS();
        }
        public override int GetPriority()
        {
            return -1;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (player != null && player.GetMark("@megatama") > 0 && RoomLogic.GetMaxCards(room, player) < player.HandcardNum
                && player.Phase == PlayerPhase.Discard)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.AskForUseCard(player, "@@megatama", "@megatama-max");
            return new TriggerStruct();
        }
        public override bool OnPhaseChange(Room room, Player player, TriggerStruct info)
        {
            return false;
        }
    }
    public class MegatamaMax : MaxCardsSkill
    {
        public MegatamaMax() : base("megatama-max")
        {
        }
        public override int GetExtra(Room room, Player target)
        {
            if (target.HasFlag("megatama"))
                return 2;

            return 0;
        }
    }

    public class PioneerCard : SkillCard
    {
        public PioneerCard() : base("PioneerCard")
        {
            target_fixed = true;
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            WrappedCard card = card_use.Card;
            CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_ANNOUNCE, player.Name, null, card.Skill, null)
            {
                CardString = RoomLogic.CardToString(room, card),
                General = RoomLogic.GetGeneralSkin(room, player, "pioneer", "head")
            };
            //show virtual card on table
            CardsMoveStruct move = new CardsMoveStruct(-1, player, Place.PlaceTable, reason)
            {
                From_place = Place.PlaceUnknown,
                From = player.Name,
                Is_last_handcard = false,
            };
            room.NotifyUsingVirtualCard(RoomLogic.CardToString(room, card), move);
            Thread.Sleep(1000);

            room.SetPlayerMark(player, "@pioneer", 0);
            room.DetachSkillFromPlayer(player, "pioneer");
            if (player.HandcardNum < 4)
                room.DrawCards(player, 4 - player.HandcardNum, "pioneer");

            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (!p.HasShownAllGenerals())
                    targets.Add(p);
            }
            if (targets.Count > 0)
            {
                Player target = room.AskForPlayerChosen(player, targets, "pioneer", "@pioneer-view", false);
                if (target != null)
                {
                    room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, target.Name);
                    List<string> choices = new List<string>();
                    if (!target.General1Showed)
                        choices.Add("head_general");
                    if (!string.IsNullOrEmpty(target.General2) && !target.General2Showed)
                        choices.Add("deputy_general");


                    target.SetFlags("KnownBothTarget");// For AI
                    string choice = room.AskForChoice(player, "pioneer", string.Join("+", choices), null, target);
                    target.SetFlags("-KnownBothTarget");
                    string general = choice == "head_general" ? target.ActualGeneral1 : target.ActualGeneral2;
                    LogMessage log = new LogMessage
                    {
                        Type = "$KnownBothViewGeneral",
                        From = target.Name,
                        To = new List<string> { target.Name },
                        Arg = choice,
                        Arg2 = general
                    };
                    room.SendLog(log, player);

                    LogMessage log2 = new LogMessage
                    {
                        Type = "#KnownBothView",
                        From = player.Name,
                        To = new List<string> { target.Name },
                        Arg = choice
                    };
                    room.SendLog(log2, new List<Player> { player });

                    room.ViewGenerals(player, new List<string> { general }, "pioneer");
                }
            }
        }
    }
    public class Pioneer : ZeroCardViewAsSkill
    {
        public Pioneer() : base("pioneer")
        {
        }
        public override bool IsAvailable(Room room, Player invoker, CardUseReason reason, string pattern, string position = null)
        {
            if (invoker.GetMark("@pioneer") > 0 && reason == CardUseReason.CARD_USE_REASON_PLAY)
            {
                if (invoker.HandcardNum < 4)
                    return true;

                foreach (Player p in room.GetOtherPlayers(invoker))
                    if (!p.HasShownAllGenerals())
                        return true;
            }

            return false;
        }
        public override WrappedCard ViewAs(Room room, Player player)
        {
            WrappedCard card = new WrappedCard("PioneerCard")
            {
                Skill = Name
            };
            return card;
        }
    }

    #endregion
}
