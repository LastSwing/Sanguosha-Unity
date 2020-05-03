using CommonClass;
using CommonClass.Game;
using SanguoshaServer.Game;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using static CommonClass.Game.Player;

namespace SanguoshaServer.Package
{
    public class StrategicAdvantageCards : CardPackage
    {
        public StrategicAdvantageCards() : base("StrategicAdvantageCards")
        {
            skills = new List<Skill>
            {
                new Transfer(),         //new transfer card
                new BladeSkill(),
                new HalberdSkill(),
                new HalberdTM(),
                new HalberdTrigger(),
                new BreastPlateSkill(),
                new IronArmorSkill(),
                new IronArmorProhibit(),
                new WoodenOxSkill(),
                new WoodenOxTriggerSkill(),
                new JadeSealSkill(),
                new LureTigerSkill(),
                new LureTigerProhibit(),
                new ThreatenEmperorSkill(),
            };
            cards = new List<FunctionCard>
            {
                new TransferCard(),
                new WoodenOxCard(),
                new JadeSealCard(),

                new Blade(),
                new Halberd(),
                new BreastPlate(),
                new IronArmor(),
                new WoodenOx(),
                new JadeSeal(),

                new Drowning(),
                new BurningCamps(),
                new LureTiger(),
                new FightTogether(),
                new AllianceFeast(),
                new ThreatenEmperor(),
                new Edict(),
                new OffensiveHorse("Jingfan")
            };
        }
    }
}

namespace SanguoshaServer.Package
{
    #region 装备
    public class Blade : Weapon
    {
        public static string ClassName = "Blade";
        public Blade() : base(ClassName, 3) { }
    }
    public class BladeSkill : WeaponSkill
    {
        public BladeSkill() : base(Blade.ClassName)
        {
            events = new List<TriggerEvent> { TriggerEvent.CardUsed, TriggerEvent.CardFinished };
            frequency = Frequency.Compulsory;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.CardFinished && data is CardUseStruct use)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is Slash)
                {
                    string card_str = RoomLogic.CardToString(room, use.Card);
                    foreach (Player p in use.To)
                    {
                        List<string> blade_use = p.ContainsTag("blade_use") ? (List<string>)p.GetTag("blade_use") : new List<string>();
                        if (!blade_use.Contains(card_str))
                            return;

                        blade_use.Remove(card_str);
                        p.SetTag("blade_use", blade_use);

                        if (blade_use.Count == 0)
                        {
                            room.RemovePlayerDisableShow(p, Blade.ClassName);
                        }
                    }
                }
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            CardUseStruct use = (CardUseStruct)data;
            if (triggerEvent == TriggerEvent.CardUsed)
            {
                if (!base.Triggerable(player, room))
                    return new TriggerStruct();
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is Slash)
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            CardUseStruct use = (CardUseStruct)data;
            bool play_animation = false;
            foreach (Player p in use.To) {
                if (p.GetMark("Equips_of_Others_nullified_to_You") > 0)
                    continue;
                string card_str = RoomLogic.CardToString(room, use.Card);
                List<string> blade_use = p.ContainsTag("blade_use") ? (List<string>)p.GetTag("blade_use") : new List<string>();
                if (blade_use.Contains(card_str))
                    return false;

                blade_use.Add(card_str);
                p.SetTag("blade_use", blade_use);

                if (!p.HasShownAllGenerals())
                    play_animation = true;

                room.SetPlayerDisableShow(p, "hd", Blade.ClassName); // this effect should always make sense.
            }

            if (play_animation)
            {
                room.SetEmotion(player, Blade.ClassName);
                Thread.Sleep(400);
            }

            return false;
        }
    }
    public class Halberd : Weapon
    {
        public static string ClassName = "Halberd";
        public Halberd() : base(ClassName, 4) { }
    }
    public class HalberdSkill : WeaponSkill
    {
        public HalberdSkill() : base("Halberd-trigger")
        {
            events.Add(TriggerEvent.CardTargetAnnounced);
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            CardUseStruct use = (CardUseStruct)data;
            FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
            if (fcard is Slash)
            {
                List <Player> selected = new List<Player>();
                foreach (Player p in use.To)
                    selected.Add(p);
                TargetModSkill skill = (TargetModSkill)Engine.GetSkill(Halberd.ClassName);
                foreach (Player p in room.GetOtherPlayers(player))
                if (!use.To.Contains(p) && skill.CheckExtraTargets(room, player, p, use.Card, selected) && fcard.ExtratargetFilter(room, selected, p, player, use.Card))
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            CardUseStruct use = (CardUseStruct)data;
            room.SetTag("extra_target_skill", data);                   //for AI
            List<Player> targets = room.AskForExtraTargets(player, use.To, use.Card,
                                                                     Halberd.ClassName, "@halberd-target:::" + use.Card.Name, true);
            room.RemoveTag("extra_target_skill");
            if (targets.Count > 0)
            {
                List<string> players = new List<string>();
                foreach (Player p in targets)
                    players.Add(p.Name);
                player.SetTag("extra_targets", players);
                return info;
            }


            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            CardUseStruct use = (CardUseStruct)data;
            List<string> players = (List<string>)player.GetTag("extra_targets");
            player.RemoveTag("extra_targets");
            List<Player> targets = new List<Player>();
            foreach (string name in players)
                targets.Add(room.FindPlayer(name, true));

            if (targets.Count > 0)
            {
                targets.AddRange(use.To);
                room.SortByActionOrder(ref targets);
                use.To = targets;
                use.Card.SetFlags(Halberd.ClassName);
                data = use;
                room.SetEmotion(player, Halberd.ClassName);
            }

            return false;
        }
    }
    public class HalberdTM : TargetModSkill
    {
        public HalberdTM() : base(Halberd.ClassName)
        {
            skill_type = SkillType.Attack;
        }
        public override bool CheckExtraTargets(Room room, Player from, Player to, WrappedCard card, List<Player> previous_targets, List<Player> selected_targets = null)
        {
            if (!Engine.MatchExpPattern(room, pattern, from, card) || from.GetMark("Equips_nullified_to_Yourself") > 0 || !from.HasWeapon(Name)
                || card.SubCards.Contains(from.Weapon.Key) || to.GetMark("Equips_of_Others_nullified_to_You") > 0)
                return false;

            List<string> kingdoms = new List<string>();
            List <Player> targets = new List<Player>(previous_targets);
            if (selected_targets != null)
                targets.AddRange(selected_targets);
            foreach (Player p in targets) {
                if (p.HasShownOneGeneral() && p.Role != "careerist")
                    kingdoms.Add(p.Kingdom);
            }

            return !targets.Contains(to) && (!to.HasShownOneGeneral() || to.GetRoleEnum() == Player.PlayerRole.Careerist || !kingdoms.Contains(to.Kingdom));
        }
    }
    public class HalberdTrigger : WeaponSkill
    {
        public HalberdTrigger() : base("Halberd-slashmiss")
        {
            events = new List<TriggerEvent> { TriggerEvent.SlashMissed, TriggerEvent.SlashEffected };
            global = true;
            frequency = Frequency.Compulsory;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.SlashMissed && data is SlashEffectStruct effect)
            {
                if (effect.Slash.HasFlag(Halberd.ClassName))
                    effect.Slash.SetFlags("halberd_slash_missed");
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.SlashEffected && data is SlashEffectStruct slash)
            {
                if (slash.Slash.HasFlag("halberd_slash_missed"))
                    return new TriggerStruct(Name, player);
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            SlashEffectStruct effect = (SlashEffectStruct)data;
            LogMessage log = new LogMessage
            {
                Type = "#Halberdnullified",
                From = effect.From.Name,
                Arg = Halberd.ClassName,
                Arg2 = effect.Slash.Name,
                To = new List<string> { effect.To.Name }
            };
            room.SendLog(log);
            return true;
        }
    }
    public class BreastPlate : Armor
    {
        public static string ClassName = "BreastPlate";
        public BreastPlate() : base(ClassName) { }
    }
    public class BreastPlateSkill : ArmorSkill
    {
        public BreastPlateSkill() : base(BreastPlate.ClassName)
        {
            events.Add(TriggerEvent.DamageDefined);
            frequency = Frequency.Compulsory;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is DamageStruct damage && base.Triggerable(player, room) && damage.Damage >= player.Hp && player.GetArmor()
                && !player.ArmorIsNullifiedBy(damage.From))
            {
                return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            return room.AskForSkillInvoke(player, Name) ? info : new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_NATURAL_ENTER, player.Name, Name, null);
            room.MoveCardTo(room.GetCard(player.Armor.Key), null, Place.DiscardPile, reason, true);
            LogMessage log = new LogMessage
            {
                Type = "#damaged-prevent",
                From = player.Name,
                Arg = Name
            };
            room.SendLog(log);

            return true;
        }
    }
    public class IronArmor : Armor
    {
        public static string ClassName = "IronArmor";
        public IronArmor() : base(ClassName) { }
    }
    public class IronArmorSkill : ArmorSkill
    {
        public IronArmorSkill() : base(IronArmor.ClassName)
        {
            events.Add(TriggerEvent.TargetConfirming);
            frequency = Frequency.Compulsory;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is CardUseStruct use && use.Card != null && use.To.Contains(player)
                && player.GetMark("Equips_of_Others_nullified_to_You") == 0 && !player.ArmorIsNullifiedBy(use.From))
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is FireAttack || fcard is FireSlash || fcard is BurningCamps)
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            return base.Cost(room, ref data, info); ;
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            CardUseStruct use = (CardUseStruct)data;
            LogMessage log2 = new LogMessage
            {
                Type = "#IronArmor",
                From = player.Name,
                Arg = Name
            };
            room.SendLog(log2);

            room.CancelTarget(ref use, player); // Room::cancelTarget(use, player);

            data = use;
            return false;
        }
    }

    public class IronArmorProhibit : ProhibitSkill
    {
        public IronArmorProhibit() : base("#IronArmor-pro")
        {
        }

        public override bool IsProhibited(Room room, Player from, Player to, ProhibitType type)
        {
            if (type == ProhibitType.Chain && RoomLogic.HasArmorEffect(room, to, IronArmor.ClassName) && !to.ArmorIsNullifiedBy(from))
            {
                List<string> big_kingdoms = RoomLogic.GetBigKingdoms(room);
                if (big_kingdoms.Count > 0)
                {
                    string kingdom = (to.HasShownOneGeneral() ? (to.GetRoleEnum() == PlayerRole.Careerist ? to.Name : to.Kingdom) : string.Empty);
                    if (!big_kingdoms.Contains(kingdom))
                        return true;
                }
            }

            return false;
        }
    }

    public class WoodenOx : Treasure
    {
        public static string ClassName = "WoodenOx";
        public WoodenOx() : base(ClassName) { }
        public override void OnUninstall(Room room, Player player, WrappedCard card)
        {
            player.AddHistory(WoodenOxCard.ClassName, 0);
            base.OnUninstall(room, player, card);
        }
    }
    public class WoodenOxCard : SkillCard
    {
        public static string ClassName = "WoodenOxCard";
        public WoodenOxCard() : base(ClassName)
        {
            target_fixed = true;
            will_throw = false;
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            WrappedCard card = card_use.Card;
            room.AddToPile(card_use.From, "wooden_ox", card.SubCards, false);

            WrappedCard treasure = room.GetCard(card_use.From.Treasure.Key);

            if (treasure != null)
            {
                List<Player> targets = new List<Player>();
                foreach (Player p in room.GetOtherPlayers(card_use.From))
                {
                    if (!p.GetTreasure() && RoomLogic.CanPutEquip(p, treasure))
                        targets.Add(p);
                }
                if (targets.Count == 0)
                    return;
                Player target = room.AskForPlayerChosen(card_use.From, targets, WoodenOx.ClassName, "@wooden_ox-move", true);
                if (target != null)
                {
                    room.MoveCardTo(treasure, card_use.From, target, Place.PlaceEquip,
                        new CardMoveReason(MoveReason.S_REASON_TRANSFER, card_use.From.Name, WoodenOx.ClassName, null));
                }
            }
        }
    }
    public class WoodenOxSkill : OneCardViewAsSkill
    {
        public WoodenOxSkill() : base(WoodenOx.ClassName)
        {
            filter_pattern = ".|.|.|hand";
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed(WoodenOxCard.ClassName);
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard ox = new WrappedCard(WoodenOxCard.ClassName);
            ox.AddSubCard(card);
            ox.Skill = WoodenOx.ClassName;
            return ox;
        }
    }
    public class WoodenOxTriggerSkill : TreasureSkill
    {
        public WoodenOxTriggerSkill() : base("WoodenOx_trigger")
        {
            events.Add(TriggerEvent.CardsMoveOneTime);
            global = true;
            frequency = Frequency.Compulsory;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player target, ref object data, Player ask_who)
        {
            CardsMoveOneTimeStruct move = (CardsMoveOneTimeStruct)data;
            if (move.From == null || !move.From.Alive) return new TriggerStruct();

            Player player = move.From;
            if (player.HasTreasure(WoodenOx.ClassName))
            {
                int count = 0;
                for (int i = 0; i < move.Card_ids.Count; i++)
                    if (move.From_pile_names[i] == "wooden_ox") count++;

                if (count > 0) return new TriggerStruct(Name, player);
            }
            else if (player.GetPile("wooden_ox").Count > 0)
            {
                for (int i = 0; i < move.Card_ids.Count; i++)
                {
                    if (move.From_places[i] != Place.PlaceEquip && move.From_places[i] != Place.PlaceTable) continue;
                    WrappedCard card = Engine.GetRealCard(move.Card_ids[i]);
                    if (card?.Name == WoodenOx.ClassName)
                        return new TriggerStruct(Name, player);
                }
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player target, ref object data, Player ask_who, TriggerStruct info)
        {
            CardsMoveOneTimeStruct move = (CardsMoveOneTimeStruct)data;
            Player player = move.From;
            if (player.HasTreasure(WoodenOx.ClassName))
            {
                int count = 0;
                for (int i = 0; i < move.Card_ids.Count; i++)
                {
                    if (move.From_pile_names[i] == "wooden_ox") count++;
                }
                if (count > 0)
                {
                    LogMessage log = new LogMessage
                    {
                        Type = "#WoodenOx",
                        From = player.Name,
                        Arg = count.ToString(),
                        Arg2 = "wooden_ox"
                    };
                    room.SendLog(log);
                }
            }
            else if (player.GetPile("wooden_ox").Count > 0)
            {
                for (int i = 0; i < move.Card_ids.Count; i++)
                {
                    if (move.From_places[i] != Place.PlaceEquip && move.From_places[i] != Place.PlaceTable) continue;
                    WrappedCard card = Engine.GetRealCard(move.Card_ids[i]);
                    if (card?.Name == WoodenOx.ClassName)
                    {
                        Player to = move.To;
                        if (to != null && to.GetTreasure() && to.Treasure.Value == WoodenOx.ClassName
                            && move.To_place ==  Place.PlaceEquip && move.Reason.Reason == MoveReason.S_REASON_TRANSFER)
                        {
                            List<Player> p_list = new List<Player>{ to };
                            room.AddToPile(to, "wooden_ox", player.GetPile("wooden_ox"), false, p_list,
                                new CardMoveReason(MoveReason.S_REASON_TRANSFER, player.Name));
                        }
                        else
                        {
                            room.ClearOnePrivatePile(player, "wooden_ox");
                        }
                        return false;
                    }
                }
            }

            return false;
        }
    }
    public class JadeSeal : Treasure
    {
        public static string ClassName = "JadeSeal";
        public JadeSeal() : base(ClassName) { }
    }
    public class JadeSealCard : SkillCard
    {
        public static string ClassName = "JadeSealCard";
        public JadeSealCard() : base(ClassName)
        {
            will_throw = false;
        }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            WrappedCard kb = new WrappedCard(KnownBoth.ClassName);
            FunctionCard fcard = Engine.GetFunctionCard(kb.Name);
            return fcard.TargetFilter(room, targets, to_select, Self, kb);
        }
        public override WrappedCard Validate(Room room, CardUseStruct use)
        {
            WrappedCard kb = new WrappedCard(KnownBoth.ClassName)
            {
                Skill = "_JadeSeal"
            };
            return kb;
        }
    }
    public class JadeSealViewAsSkill : ZeroCardViewAsSkill
    {
        public JadeSealViewAsSkill() : base(JadeSeal.ClassName)
        {
        }

        public override bool IsAvailable(Room room, Player invoker, CardUseStruct.CardUseReason reason, string pattern, string position = null)
        {
            return reason == CardUseStruct.CardUseReason.CARD_USE_REASON_RESPONSE_USE && pattern == "@@JadeSeal!";
        }

        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard(JadeSealCard.ClassName);
        }
    }
    public class JadeSealSkill : TreasureSkill
    {
        public JadeSealSkill() : base(JadeSeal.ClassName)
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseProceeding, TriggerEvent.EventPhaseStart };
            view_as_skill = new JadeSealViewAsSkill();
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (!RoomLogic.HasTreasureEffect(room, player, Name) || !player.HasShownOneGeneral())
                return new TriggerStruct();
            if (triggerEvent == TriggerEvent.EventPhaseProceeding && player.Phase == PlayerPhase.Draw && (int)data >= 0)
            {
                return new TriggerStruct(Name, player);
            }
            else if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Play)
            {
                WrappedCard kb = new WrappedCard(KnownBoth.ClassName)
                {
                    Skill = "_JadeSeal"
                };
                FunctionCard fcard = Engine.GetFunctionCard(kb.Name);
                if (fcard?.IsAvailable(room, player, kb) == true)
                    return new TriggerStruct(Name, player);
            }
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.EventPhaseProceeding)
                return info;
            if (room.AskForUseCard(player, "@@JadeSeal!", "@JadeSeal", null) == null)
            {
                WrappedCard kb = new WrappedCard(KnownBoth.ClassName)
                {
                    Skill = "_JadeSeal"
                };
                FunctionCard fcard = Engine.GetFunctionCard(kb.Name);
                List<Player> targets = new List<Player>();
                foreach (Player p in room.GetOtherPlayers(player)) {
                    if (RoomLogic.IsProhibited(room, player, p, kb) == null && (!p.IsKongcheng() || !p.HasShownAllGenerals()))
                        targets.Add(p);
                }
                if (targets.Count > 0)
                {
                    Shuffle.shuffle(ref targets);
                    Player target = targets[0];
                    room.UseCard(new CardUseStruct(kb, player, target), false);
                }
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            data = (int)data + 1;
            return false;
        }
    }
    #endregion

    #region 锦囊
    public class Drowning : SingleTargetTrick
    {
        public static string ClassName = "Drowning";
        public Drowning() : base(Drowning.ClassName) { }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            int total_num = 1 + Engine.CorrectCardTarget(room, TargetModSkill.ModType.ExtraMaxTarget, Self, card);
            if (targets.Count >= total_num || !base.TargetFilter(room, targets, to_select, Self, card))
                return false;

            return to_select.HasEquip() && to_select != Self;
        }
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            if (effect.To.GetEquips().Count > 0
                && room.AskForChoice(effect.To, Name, "throw+damage", null, effect) == "throw")
            {
                room.ThrowAllEquips(effect.To);
            }
            else
            {
                room.Damage(new DamageStruct(effect.Card, effect.From.Alive ? effect.From : null, effect.To, 1, DamageStruct.DamageNature.Thunder));
            }
        }
        public override bool IsAvailable(Room room, Player player, WrappedCard card)
        {
            bool canUse = false;
            List<Player> players = room.GetAlivePlayers();
            foreach (Player p in players) {
                if (p == player) continue;
                if (RoomLogic.IsProhibited(room, player, p, card) != null)
                    continue;
                if (!p.HasEquip())
                    continue;
                canUse = true;
                break;
            }

            return canUse && base.IsAvailable(room, player, card);
        }
    }
    class BurningCamps : AOE
    {
        public static string ClassName = "BurningCamps";
        public BurningCamps() : base(ClassName) { }
        public override bool IsAvailable(Room room, Player player, WrappedCard card)
        {
            bool canUse = false;
            List <Player> players = RoomLogic.GetFormation(room, room.GetNextAlive(player));
            foreach (Player p in players) {
                if (RoomLogic.IsProhibited(room, player, p, card) != null)
                    continue;
                canUse = true;
                break;
            }

            return canUse && base.IsAvailable(room, player, card);
        }
        public override void OnUse(Room room, CardUseStruct card_use)
        {
            Player from = card_use.From;
            List <Player> targets = RoomLogic.GetFormation(room, room.GetNextAlive(from));
            foreach (Player player in targets) {
                Skill skill = RoomLogic.IsProhibited(room, from, player, card_use.Card);
                if (skill != null)
                {
                    skill = Engine.GetMainSkill(skill.Name);
                    LogMessage log = new LogMessage
                    {
                        Type = "#SkillAvoid",
                        From = player.Name,
                        Arg = skill.Name,
                        Arg2 = Name
                    };
                    room.SendLog(log);
                    if (RoomLogic.PlayerHasShownSkill(room, player, skill))
                    {
                        room.NotifySkillInvoked(player, skill.Name);
                        GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, skill.Name);
                        string genral = gsk.General;
                        int skin_id = gsk.SkinId;
                        string skill_name = skill.Name;
                        int audio = -1;
                        skill.GetEffectIndex(room, player, card_use.Card, ref audio, ref skill_name, ref genral, ref skin_id);
                        if (audio >= -1)
                            room.BroadcastSkillInvoke(skill_name, "male", audio, genral, skin_id);
                    }
                }
                else
                   card_use.To.Add(player);
            }
            base.OnUse(room, card_use);
        }
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            room.Damage(new DamageStruct(effect.Card, effect.From, effect.To, 1, DamageStruct.DamageNature.Fire));
        }
    }
    public class LureTiger : TrickCard
    {
        public static string ClassName = "LureTiger";
        public LureTiger() : base(ClassName) { }
        public override string GetSubtype()=> "lure_tiger";
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            int total_num = 2 + Engine.CorrectCardTarget(room, TargetModSkill.ModType.ExtraMaxTarget, Self, card);
            if (targets.Count >= total_num || Engine.IsProhibited(room, Self, to_select, card, targets) != null)
                return false;
            if (RoomLogic.IsCardLimited(room, Self, card, HandlingMethod.MethodUse))
                return false;

            return to_select != Self;
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            List<Player> targets = new List<Player>(card_use.To);
            for (int index = 0; index < targets.Count; index++)
            {
                Player target = targets[index];
                CardEffectStruct effect = new CardEffectStruct
                {
                    Card = card_use.Card,
                    From = card_use.From,
                    To = target,
                    Multiple = (targets.Count > 1),
                    Drank = card_use.Drank,
                    ExDamage = 0,
                    BasicEffect = card_use.EffectCount[index]
                };

                List<Player> players = new List<Player>();
                for (int i = index; i < targets.Count; i++)
                {
                    if (card_use.EffectCount.Count <= i || !card_use.EffectCount[i].Nullified)
                        players.Add(targets[i]);
                }
                effect.StackPlayers = players;

                room.CardEffect(effect);
            }
            //room.DrawCards(card_use.From, 1, Name);

            List<int> table_cardids = room.GetCardIdsOnTable(room.GetSubCards(card_use.Card));
            if (table_cardids.Count > 0)
            {
                CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_USE, card_use.From.Name, null, card_use.Card.Skill, null)
                {
                    Card = card_use.Card
                };
                if (targets.Count == 1) reason.TargetId = targets[0].Name;
                CardsMoveStruct move = new CardsMoveStruct(table_cardids, card_use.From, null, Place.PlaceTable, Place.DiscardPile, reason);
                room.MoveCardsAtomic(new List<CardsMoveStruct> { move }, true);
            }
        }
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            effect.To.Removed = true;
            room.BroadcastProperty(effect.To, "Removed");
            effect.From.SetFlags("LureTigerUser");
        }
    }
    public class LureTigerSkill : TriggerSkill
    {
        public LureTigerSkill() : base("lure_tiger_effect")
        {
            events = new List<TriggerEvent> { TriggerEvent.Death, TriggerEvent.EventPhaseChanging, TriggerEvent.HpChanging };
            global = true;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.HpChanging || !player.HasFlag("LureTigerUser"))
                return;
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change)
            {
                if (change.To != PlayerPhase.NotActive)
                    return;
            }

            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (p.Removed)
                {
                    p.Removed = false;
                    room.BroadcastProperty(p, "Removed");
                }
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.HpChanging && player.Removed)
            {
                return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name, false);
            LogMessage log = new LogMessage
            {
                Type = "#lure_tiger",
                From = player.Name
            };
            room.SendLog(log);

            return true;
        }
    }

    /*
    public class LureTigerSkill : TriggerSkill
    {
        public LureTigerSkill() : base("lure_tiger_effect")
        {
            events = new List<TriggerEvent> { TriggerEvent.Death, TriggerEvent.EventPhaseChanging };
            global = true;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (!player.HasFlag("LureTigerUser"))
                return;
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change)
            {
                if (change.To != PlayerPhase.NotActive)
                    return;
            }

            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (p.Removed)
                {
                    p.Removed = false;
                    room.BroadcastProperty(p, "Removed");
                }
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            return new TriggerStruct();
        }
    }
    */
    public class LureTigerProhibit : ProhibitSkill
    {
        public LureTigerProhibit() : base("#lure_tiger-prohibit")
        {
        }
        public override bool IsProhibited(Room room, Player from, Player to, WrappedCard card, List<Player> others = null)
        {
            if (card != null)
            {
                FunctionCard fcard = Engine.GetFunctionCard(card.Name);
                return to != null && to.Removed && fcard.TypeID != FunctionCard.CardType.TypeSkill;
            }
            return false;
        }
    }

    class FightTogether : TrickCard
    {
        public static string ClassName = "FightTogether";
        public FightTogether() : base(ClassName) { }
        public override string GetSubtype()=> "fight_together";
        public override bool IsAvailable(Room room, Player player, WrappedCard card)
        {
            List<string> big_kingdoms = RoomLogic.GetBigKingdoms(room);
            return big_kingdoms.Count > 0 && base.IsAvailable(room, player, card) || CanRecast(room, player, card);
        }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            bool big = RoomLogic.GetBigKingdoms(room).Count > 0;
            if (!big || targets.Count > 0 || RoomLogic.IsCardLimited(room, Self, card, HandlingMethod.MethodUse) || Engine.IsProhibited(room, Self, to_select, card, targets) != null)
                return false;

            return true;
        }
        public override bool TargetsFeasible(Room room, List<Player> targets, Player Self, WrappedCard card)
        {
            if (targets.Count == 0)
                return CanRecast(room, Self, card);
            else
            {
                List<string> big_kingdoms = RoomLogic.GetBigKingdoms(room);
                return big_kingdoms.Count > 0 && !RoomLogic.IsCardLimited(room, Self, card, HandlingMethod.MethodUse);
            }
        }
        public override void OnUse(Room room, CardUseStruct use)
        {
            if (use.To.Count == 0 && string.IsNullOrEmpty(use.Pattern))
            {
                DoRecast(room, use);
            }
            else
            {
                List<string> big_kingdoms = RoomLogic.GetBigKingdoms(room);
                if (big_kingdoms.Count > 0 && use.Pattern != "unknown")
                {
                    bool big = (use.Pattern == "big");
                    List<Player> targets = new List<Player>(), prohibites = new List<Player>();
                    foreach (Player p in room.GetAlivePlayers())
                    {
                        string kingdom = (p.HasShownOneGeneral() ? (p.GetRoleEnum() == Player.PlayerRole.Careerist ? p.Name : p.Kingdom) : string.Empty);
                        if (big_kingdoms.Contains(kingdom) == big)
                        {
                            Skill skill = RoomLogic.IsProhibited(room, use.From, p, use.Card);
                            if (skill != null)
                            {
                                prohibites.Add(p);
                            }
                            else
                                targets.Add(p);
                        }
                    }

                    if (use.Card.Skill == "qice" && targets.Count > use.Card.SubCards.Count)       //check for qice cause of stupid rules
                        targets.Clear();

                    if (targets.Count > 0)
                    {
                        foreach (Player player in prohibites)
                        {
                            Skill skill = RoomLogic.IsProhibited(room, use.From, player, use.Card);
                            if (skill != null)
                            {
                                skill = Engine.GetMainSkill(skill.Name);
                                LogMessage log = new LogMessage
                                {
                                    Type = "#SkillAvoid",
                                    From = player.Name,
                                    Arg = skill.Name,
                                    Arg2 = Name
                                };
                                room.SendLog(log);
                                if (RoomLogic.PlayerHasShownSkill(room, player, skill))
                                {
                                    room.NotifySkillInvoked(player, skill.Name);
                                    GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, skill.Name);
                                    string genral = gsk.General;
                                    int skin_id = gsk.SkinId;
                                    string skill_name = skill.Name;
                                    int audio = -1;
                                    skill.GetEffectIndex(room, player, use.Card, ref audio, ref skill_name, ref genral, ref skin_id);
                                    if (audio >= -1)
                                        room.BroadcastSkillInvoke(skill_name, "male", audio, genral, skin_id);
                                }
                            }
                        }
                    }

                    room.SortByActionOrder(ref targets);
                    use.To = targets;
                }
                base.OnUse(room, use);
            }
        }
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            if (!effect.To.Chained)
            {
                if (!RoomLogic.CanBeChainedBy(room, effect.To, effect.From))
                    return;
                room.SetPlayerChained(effect.To, true);
            }
            else
                room.DrawCards(effect.To, 1, Name);
        }
    }
    class AllianceFeast : TrickCard
    {
        public static string ClassName = "AllianceFeast";
        public AllianceFeast() : base(ClassName) { }
        public override string GetSubtype() => "alliance_feast";
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            if (!base.TargetFilter(room, targets, to_select, Self, card)) return false;

            return to_select.HasShownOneGeneral() && !RoomLogic.IsFriendWith(room, Self, to_select);
        }
        public override void OnUse(Room room, CardUseStruct card_use)
        {
            Player source = card_use.From;
            WrappedCard card = card_use.Card;
            List<Player> targets = new List<Player>();
            if (card_use.To.Count == 1)
            {
                Player target = card_use.To[0];
                List<Player> other_players = room.GetOtherPlayers(source);
                foreach (Player p in other_players)
                {
                    if (!RoomLogic.IsFriendWith(room, target, p))
                        continue;
                    Skill skill = RoomLogic.IsProhibited(room, source, p, card);
                    if (skill != null)
                    {
                        skill = Engine.GetMainSkill(skill.Name);
                        LogMessage _log = new LogMessage
                        {
                            Type = "#SkillAvoid",
                            From = p.Name,
                            Arg = skill.Name,
                            Arg2 = Name
                        };
                        room.SendLog(_log);
                        if (RoomLogic.PlayerHasShownSkill(room, p, skill))
                        {
                            room.NotifySkillInvoked(p, skill.Name);
                            GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, p, skill.Name);
                            string genral = gsk.General;
                            int skin_id = gsk.SkinId;
                            string skill_name = skill.Name;
                            int audio = -1;
                            skill.GetEffectIndex(room, p, card, ref audio, ref skill_name, ref genral, ref skin_id);
                            if (audio >= -1)
                                room.BroadcastSkillInvoke(skill_name, "male", audio, genral, skin_id);
                        }
                    }
                    else
                        targets.Add(p);
                }
                room.SortByActionOrder(ref targets);
            }
            targets.Add(source);
            card_use.To = targets;
            
            Player player = source;
            LogMessage log = new LogMessage("#UseCard")
            {
                From = player.Name,
                To = new List<string>(),
                Card_str = RoomLogic.CardToString(room, card_use.Card)
            };
            if (!TargetFixed(card_use.Card) || card_use.To.Count > 1 || !card_use.To.Contains(card_use.From))
                log.SetTos(card_use.To);

            List<int> used_cards = new List<int>();
            List<CardsMoveStruct> moves = new List<CardsMoveStruct>();
            used_cards.AddRange(card_use.Card.SubCards);

            RoomThread thread = room.RoomThread;
            object data = card_use;
            thread.Trigger(TriggerEvent.PreCardUsed, room, player, ref data);

            card_use = (CardUseStruct)data;

            CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_USE, player.Name, null, card_use.Card.Skill, null)
            {
                Card = card_use.Card,
                General = RoomLogic.GetGeneralSkin(room, player, card_use.Card.Skill, card_use.Card.SkillPosition)
            };
            if (card_use.To.Count == 1) reason.TargetId = card_use.To[0].Name;

            if (used_cards.Count == 0)
            {
                CardMoveReasonStruct virtual_reason = new CardMoveReasonStruct
                {
                    Reason = reason.Reason,
                    PlayerId = reason.PlayerId,
                    TargetId = reason.TargetId,
                    SkillName = reason.SkillName,
                    EventName = reason.EventName,
                    CardString = RoomLogic.CardToString(room, card_use.Card),
                    General = reason.General
                };
                ClientCardsMoveStruct move = new ClientCardsMoveStruct(-1, player, Place.PlaceTable, virtual_reason)              //show virtual card on table
                {
                    From_place = Place.PlaceUnknown,
                    From = player.Name,
                    Is_last_handcard = false,
                };
                room.NotifyUsingVirtualCard(RoomLogic.CardToString(room, card_use.Card), move);
            }
            else
            {
                room.RecordSubCards(card_use.Card);
                foreach (int id in used_cards)
                {
                    CardsMoveStruct move = new CardsMoveStruct(id, null, Place.PlaceTable, reason);
                    moves.Add(move);
                }
                room.MoveCardsAtomic(moves, true);
            }
            

            room.SendLog(log);

            room.RoomThread.Trigger(TriggerEvent.CardUsedAnnounced, room, player, ref data);
            room.RoomThread.Trigger(TriggerEvent.CardTargetAnnounced, room, player, ref data);

            room.RoomThread.Trigger(TriggerEvent.CardUsed, room, player, ref data);
            room.RoomThread.Trigger(TriggerEvent.CardFinished, room, player, ref data);
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            List<Player> targets = new List<Player>(card_use.To);

            for (int index = 0; index < targets.Count; index++)
            {
                Player target = targets[index];
                CardEffectStruct effect = new CardEffectStruct
                {
                    Card = card_use.Card,
                    From = card_use.From,
                    To = target,
                    Multiple = (targets.Count > 1),
                    Drank = card_use.Drank,
                    ExDamage = 0,
                    BasicEffect = card_use.EffectCount.Count > index ? card_use.EffectCount[index] : new CardBasicEffect(target, 0, 0, 0)
                };

                List<Player> players = new List<Player>();
                for (int i = index; i < targets.Count; i++)
                {
                    if (card_use.EffectCount.Count <= i || !card_use.EffectCount[i].Nullified)
                        players.Add(targets[i]);
                }
                effect.StackPlayers = players;

                if (target == card_use.From)
                {
                    int n = 0;
                    Player enemy = targets[0];
                    foreach (Player p in room.GetOtherPlayers(card_use.From))
                    {
                        if (RoomLogic.IsFriendWith(room, enemy, p))
                            ++n;
                    }
                    target.SetMark(Name, n);
                }
                room.CardEffect(effect);
                target.SetMark(Name, 0);
            }

            List<int> table_cardids = room.GetCardIdsOnTable(room.GetSubCards(card_use.Card));
            if (table_cardids.Count > 0)
            {
                CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_USE, card_use.From.Name, null, card_use.Card.Skill, null)
                {
                    Card = card_use.Card
                };
                CardsMoveStruct move = new CardsMoveStruct(table_cardids, card_use.From, null, Place.PlaceTable, Place.DiscardPile, reason);
                room.MoveCardsAtomic(new List<CardsMoveStruct> { move }, true);
            }
        }
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            if (effect.To.GetMark(Name) > 0)
            {
                if (effect.To.IsWounded())
                {
                    List<string> prompts = new List<string> { string.Empty };
                    List<string> choices = new List<string>();
                    for (int i = Math.Min(effect.To.GetMark(Name), effect.To.GetLostHp()); i > 0; i--)
                    {
                        choices.Add(string.Format("recover{0}", i));
                        if (i == effect.To.GetMark(Name) && effect.To.GetMark(Name) <= effect.To.GetLostHp())
                            prompts.Add(string.Format("@AllianceFeast-recover:::{0}:", i));
                        else
                            prompts.Add(string.Format("@AllianceFeast-recover-draw:::{0}:{1}", i, effect.To.GetMark(Name) - i));
                    }
                    choices.Add("draw");
                    prompts.Add(string.Format("@AllianceFeast-draw:::{0}:", effect.To.GetMark(Name)));
                    string result = room.AskForChoice(effect.To, Name, string.Join("+", choices), prompts);

                    const string rx_pattern = @"recover(\d+)";
                    Match match = Regex.Match(result, rx_pattern);

                    int draw = effect.To.GetMark(Name);
                    if (match.Success && match.Length > 0)
                    {
                        int recover = int.Parse(match.Groups[1].ToString());
                        draw = effect.To.GetMark(Name) - recover;
                        RecoverStruct re = new RecoverStruct
                        {
                            Recover = recover,
                            Who = effect.From
                        };
                        room.Recover(effect.To, re, true);
                    }
                    if (draw > 0)
                        room.DrawCards(effect.To, draw, Name);
                }
                else
                    room.DrawCards(effect.To, effect.To.GetMark(Name), Name);
            }
            else
            {
                room.DrawCards(effect.To, new DrawCardStruct(1, effect.From, Name));
                if (effect.To.Chained)
                    room.SetPlayerChained(effect.To, false);
            }
        }
        public override bool IsAvailable(Room room, Player player, WrappedCard card)
        {
            return player.HasShownOneGeneral() && RoomLogic.IsProhibited(room, player, player, card) == null && base.IsAvailable(room, player, card);
        }
    }

    /*
    class AllianceFeast : TrickCard
    {
        public AllianceFeast() : base(AllianceFeast.ClassName) { }
        public override string GetSubtype()=> "alliance_feast";
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            if (!base.TargetFilter(room, targets, to_select, Self, card)) return false;

            return to_select.HasShownOneGeneral() && !RoomLogic.IsFriendWith(room, Self, to_select);
        }
        public override void OnUse(Room room, CardUseStruct card_use)
        {
            Player source = card_use.From;
            WrappedCard card = card_use.Card;
            List<Player> targets = new List<Player>();
            if (RoomLogic.IsProhibited(room, source, source, card) == null)
                targets.Add(source);
            if (card_use.To.Count == 1)
            {
                Player target = card_use.To[0];
                List<Player> other_players = room.GetOtherPlayers(source);
                foreach (Player player in other_players) {
                    if (!RoomLogic.IsFriendWith(room, target, player))
                        continue;
                    Skill skill = RoomLogic.IsProhibited(room, source, player, card);
                    if (skill != null)
                    {
                        skill = Engine.GetMainSkill(skill.Name);
                        LogMessage log = new LogMessage
                        {
                            Type = "#SkillAvoid",
                            From = player.Name,
                            Arg = skill.Name,
                            Arg2 = Name
                        };
                        room.SendLog(log);
                        room.BroadcastSkillInvoke(skill.Name, player);
                    }
                    else
                        targets.Add(player);
                }
            }

            card_use.To = targets;
            base.OnUse(room, card_use);
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
                room.SetTag("targets" + RoomLogic.CardToString(room, card_use.Card), players);

                if (target == card_use.From)
                {
                    int n = 0;
                    Player enemy = targets[targets.Count - 1];
                    foreach (Player p in room.GetOtherPlayers(card_use.From)) {
                        if (RoomLogic.IsFriendWith(room, enemy, p))
                            ++n;
                    }
                    target.SetMark(Name, n);
                }
                room.CardEffect(effect);
                target.SetMark(Name, 0);
            }

            List<int> table_cardids = room.GetCardIdsOnTable(card_use.Card);
            if (table_cardids.Count > 0)
            {
                CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_USE, card_use.From.Name, null, card_use.Card.Skill, null)
                {
                    CardString = RoomLogic.CardToString(room, card_use.Card)
                };
                CardsMoveStruct move = new CardsMoveStruct(table_cardids, card_use.From, null, Place.PlaceTable, Place.DiscardPile, reason);
                room.MoveCardsAtomic(new List<CardsMoveStruct> { move }, true);
            }
        }
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            if (effect.To.GetMark(Name) > 0)
            {
                room.DrawCards(effect.To, effect.To.GetMark(Name), Name);
            }
            else
            {
                List<string> choices = new List<string>();
                if (effect.To.IsWounded())
                    choices.Add("recover");
                choices.Add("draw");
                string choice = room.AskForChoice(effect.To, Name, string.Join("+", choices));
                if (choice == "recover")
                {
                    RecoverStruct recover = new RecoverStruct
                    {
                        Recover = 1,
                        Who = effect.From
                    };
                    room.Recover(effect.To, recover);
                }
                else
                {
                    room.DrawCards(effect.To, 1, Name);
                    if (effect.To.Chained)
                        room.SetPlayerChained(effect.To, false);
                }
            }
        }
        public override bool IsAvailable(Room room, Player player, WrappedCard card)
        {
            return player.HasShownOneGeneral() && RoomLogic.IsProhibited(room, player, player, card) == null && base.IsAvailable(room, player, card);
        }
    }
    */
    public class ThreatenEmperor : SingleTargetTrick
    {
        public static string ClassName = "ThreatenEmperor";
        public ThreatenEmperor() : base(ClassName) { target_fixed = true; }
        public override void OnUse(Room room, CardUseStruct use)
        {
            if (use.To.Count == 0)
                use.To.Add(use.From);
            base.OnUse(room, use);
        }
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            if (effect.From.Phase == PlayerPhase.Play)
                effect.From.SetFlags("Global_PlayPhaseTerminated");
            effect.To.SetMark("ThreatenEmperorExtraTurn", 1);
        }
        public override bool IsAvailable(Room room, Player player, WrappedCard card)
        {
            if (!player.HasShownOneGeneral())
                return false;
            List<string> big_kingdoms = RoomLogic.GetBigKingdoms(room);
            bool invoke = big_kingdoms.Count > 0;
            if (invoke)
            {
                if (big_kingdoms.Count == 1 && big_kingdoms[0].StartsWith("SGS")) // for JadeSeal
                    invoke = big_kingdoms.Contains(player.Name);
                else if (player.GetRoleEnum() == PlayerRole.Careerist)
                    invoke = false;
                else
                    invoke = big_kingdoms.Contains(player.Kingdom);
            }
            return invoke && RoomLogic.IsProhibited(room, player, player, card) == null && base.IsAvailable(room, player, card);
        }
    }
    public class ThreatenEmperorSkill : TriggerSkill
    {
        public ThreatenEmperorSkill() : base(ThreatenEmperor.ClassName)
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseEnd, TriggerEvent.EventPhaseChanging };
            global = true;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == PlayerPhase.NotActive && player.GetMark("ThreatenEmperorExtraTurn") > 0)
                player.SetMark("ThreatenEmperorExtraTurn", 0);
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.EventPhaseEnd && player.Phase == PlayerPhase.Discard && player.Alive && player.GetMark("ThreatenEmperorExtraTurn") > 0)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            ask_who.RemoveMark("ThreatenEmperorExtraTurn");
            if (room.AskForCard(ask_who, Name, ".", "@threaten_emperor", data, Name) != null)
                return info;
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            LogMessage l = new LogMessage
            {
                Type = "#Fangquan",
                To = new List<string> { ask_who.Name }
            };
            room.SendLog(l);
            room.GainAnExtraTurn(ask_who);
            return false;
        }
    }

    public class Edict : GlobalEffect
    {
        public static string ClassName = "Edict";
        public Edict() : base(ClassName) { }
        public override bool IsAvailable(Room room, Player player, WrappedCard card)
        {
            bool canUse = false;
            List<Player> players = room.GetAlivePlayers();
            foreach (Player p in players)
            {
                if (p.HasShownOneGeneral() || RoomLogic.IsProhibited(room, player, p, card) != null)
                    continue;

                canUse = true;
                break;
            }

            return canUse && !RoomLogic.IsCardLimited(room, player, card, handling_method)
                || CanRecast(room, player, card);
        }
        public override void OnUse(Room room, CardUseStruct card_use)
        {
            Player source = card_use.From;
            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetAllPlayers()) {
                if (p.HasShownOneGeneral())
                    continue;
                Skill skill = RoomLogic.IsProhibited(room, source, p, card_use.Card);
                if (skill != null)
                {
                    skill = Engine.GetMainSkill(skill.Name);
                    LogMessage log = new LogMessage
                    {
                        Type = "#SkillAvoid",
                        From = p.Name,
                        Arg = skill.Name,
                        Arg2 = Name
                    };
                    room.SendLog(log);
                    if (RoomLogic.PlayerHasShownSkill(room, p, skill))
                    {
                        room.NotifySkillInvoked(p, skill.Name);
                        GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, p, skill.Name);
                        string genral = gsk.General;
                        int skin_id = gsk.SkinId;
                        string skill_name = skill.Name;
                        int audio = -1;
                        skill.GetEffectIndex(room, p, card_use.Card, ref audio, ref skill_name, ref genral, ref skin_id);
                        if (audio >= -1)
                            room.BroadcastSkillInvoke(skill_name, "male", audio, genral, skin_id);
                    }
                }
                else
                    targets.Add(p);
            }
            if (targets.Count == 0) return;

            card_use.To = targets;
            base.OnUse(room, card_use);
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            room.SetCardFlag(card_use.Card, "edict_normal_use");
            base.Use(room, card_use);
        }
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            if (room.AskForCard(effect.To, Name, "EquipCard", "@edict-equip") != null)
                return;
            List<string> choices = new List<string> { "losehp" };
            if (!effect.To.HasShownAllGenerals() && ((!effect.To.General1Showed && effect.To.CanShowGeneral("h"))
                || (!string.IsNullOrEmpty(effect.To.General2) && !!effect.To.General2Showed && effect.To.CanShowGeneral("d"))))
                choices.Add("show");
            string choice = room.AskForChoice(effect.To, Name, string.Join("+", choices));
            if (choice == "show")
            {
                room.AskForGeneralShow(effect.To, Name);
                room.DrawCards(effect.To, 1, Name);
            }
            else
            {
                room.LoseHp(effect.To);
            }
        }
    }

    #region old
    /*
    public class TransferCard : SkillCard
    {
        public TransferCard() : base(TransferCard.ClassName)
        {
            will_throw = false;
        }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            if (targets.Count == 0 && to_select != Self)
            {
                if (!Self.HasShownOneGeneral())
                    return !to_select.HasShownOneGeneral();
                return !to_select.HasShownOneGeneral() || !RoomLogic.IsFriendWith(room, to_select, Self);
            }
            return false;
        }
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            bool draw = effect.To.HasShownOneGeneral();
            CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_GIVE, effect.From.Name, effect.To.Name, "transfer", null);
            room.ObtainCard(effect.To, effect.Card, reason);
            if (draw)
                room.DrawCards(effect.From, 1, "transfer");
        }
    }
    */
    #endregion

    #region new
    public class TransferCard : SkillCard
    {
        public static string ClassName = "TransferCard";
        public TransferCard() : base(ClassName)
        {
            will_throw = false;
        }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            if (targets.Count == 0 && to_select != Self)
            {
                if (!Self.HasShownOneGeneral())
                    return !to_select.HasShownOneGeneral();
                return !to_select.HasShownOneGeneral() || !RoomLogic.IsFriendWith(room, to_select, Self);
            }
            return false;
        }
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            effect.From.SetFlags("transfer");
            bool draw = effect.To.HasShownOneGeneral();
            CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_GIVE, effect.From.Name, effect.To.Name, "transfer", null);
            room.ObtainCard(effect.To, effect.Card, reason);
            if (draw)
                room.DrawCards(effect.From, effect.Card.SubCards.Count, "transfer");
        }
    }

    public class Transfer : ViewAsSkill
    {
        public Transfer() : base("transfer")
        {
        }

        public override bool IsAvailable(Room room, Player invoker, CardUseStruct.CardUseReason reason, string pattern, string position = null)
        {
            return reason == CardUseStruct.CardUseReason.CARD_USE_REASON_PLAY && !invoker.IsKongcheng() && !invoker.HasFlag(Name);
        }

        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return selected.Count <= 2 && player.HandCards.Contains(to_select.Id) && to_select.Transferable;
        }

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count > 0)
            {
                WrappedCard card = new WrappedCard(TransferCard.ClassName)
                {
                    Skill = Name,
                    Mute = true
                };
                card.AddSubCards(cards);
                return card;
            }

            return null;
        }
    }
    #endregion
    #endregion
}
