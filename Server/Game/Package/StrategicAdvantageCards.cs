using CommonClass;
using CommonClass.Game;
using SanguoshaServer.Game;
using System.Collections.Generic;
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
                new BladeSkill(),
                new HalberdSkill(),
                new HalberdTM(),
                new HalberdTrigger(),
                new BreastplateSkill(),
                new IronArmorSkill(),
                new WoodenOxSkill(),
                new WoodenOxTriggerSkill(),
                new JadeSealSkill(),
                new LureTigerSkill(),
                new LureTigerProhibit(),
                new ThreatenEmperorSkill()
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
                new ImperialOrder(),
                new OffensiveHorse("Jingfan")
            };
        }
    }
}

namespace SanguoshaServer.Game
{
    #region 装备
    public class Blade : Weapon
    {
        public Blade() : base("Blade", 3) { }
    }
    public class BladeSkill : WeaponSkill
    {
        public BladeSkill() : base("Blade")
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
                            room.RemovePlayerDisableShow(p, "Blade");
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

                room.SetPlayerDisableShow(p, "hd", "Blade"); // this effect should always make sense.
            }

            if (play_animation)
            {
                room.SetEmotion(player, "blade");
                Thread.Sleep(400);
            }

            return false;
        }
    }
    public class Halberd : Weapon
    {
        public Halberd() : base("Halberd", 4) { }
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
                TargetModSkill skill = (TargetModSkill)Engine.GetSkill("Halberd");
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
                                                                     "Halberd", "@extra_targets1:" + use.Card.Name, true);
            player.RemoveTag("extra_target_skill");
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
                use.Card.SetFlags("Halberd");
                data = use;
                room.SetEmotion(player, "halberd");
            }

            return false;
        }
    }
    public class HalberdTM : TargetModSkill
    {
        public HalberdTM() : base("Halberd")
        {
            skill_type = SkillType.Attack;
        }
        public override bool CheckExtraTargets(Room room, Player from, Player to, WrappedCard card, List<Player> previous_targets, List<Player> selected_targets = null)
        {
            if (!Engine.MatchExpPattern(room, pattern, from, card) || from.GetMark("Equips_nullified_to_Yourself") > 0 || !from.HasWeapon(Name)
                || card.SubCards.Contains(from.Weapon.Key))
                return false;

            List<string> kingdoms = new List<string>();
            List <Player> targets = new List<Player>(previous_targets);
            if (selected_targets != null)
                targets.AddRange(selected_targets);
            foreach (Player p in targets) {
                if (!p.HasShownOneGeneral() || p.Role == "careerist")
                    continue;
                kingdoms.Add(p.Kingdom);
            }
            if (to.GetMark("Equips_of_Others_nullified_to_You") > 0)
                return false;
            if (to.HasShownOneGeneral() && to.Role == "careerist") // careerist!
                return false;
            if (to.HasShownOneGeneral() && kingdoms.Contains(to.Kingdom))
                return false;
            return !targets.Contains(to);
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
                if (effect.Slash.HasFlag("Halberd"))
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
                Arg = "Halberd",
                Arg2 = effect.Slash.Name,
                To = new List<string> { effect.To.Name }
            };
            room.SendLog(log);
            return true;
        }
    }
    public class BreastPlate : Armor
    {
        public BreastPlate() : base("BreastPlate") { }
    }
    public class BreastplateSkill : ArmorSkill
    {
        public BreastplateSkill() : base("Breastplate")
        {
            events.Add(TriggerEvent.DamageInflicted);
            frequency = Frequency.Compulsory;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            DamageStruct damage = (DamageStruct)data;
            if (base.Triggerable(player, room) && damage.Damage >= player.Hp && player.GetArmor())
                return new TriggerStruct(Name, player);
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            return room.AskForSkillInvoke(player, Name) ? info : new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_NATURAL_ENTER, player.Name, Name, null);
            room.MoveCardTo(room.GetCard(player.Armor.Key), null, Player.Place.DiscardPile, reason, true);
            DamageStruct damage = (DamageStruct)data;
            LogMessage log = new LogMessage
            {
                Type = "#Breastplate",
                From = player.Name,
                Arg = damage.Damage.ToString()
            };
            if (damage.From != null)
                log.To = new List<string> { damage.From.Name };
            if (damage.Nature == DamageStruct.DamageNature.Normal)
                log.Arg2 = "normal_nature";
            else if (damage.Nature == DamageStruct.DamageNature.Fire)
                log.Arg2 = "fire_nature";
            else if (damage.Nature == DamageStruct.DamageNature.Thunder)
                log.Arg2 = "thunder_nature";
            room.SendLog(log);
            return true;
        }
    }
    public class IronArmor : Armor
    {
        public IronArmor() : base("IronArmor") { }
    }
    public class IronArmorSkill : ArmorSkill
    {
        public IronArmorSkill() : base("IronArmor")
        {
            events.Add(TriggerEvent.TargetConfirming);
            frequency = Frequency.Compulsory;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (!base.Triggerable(player, room)) return new TriggerStruct();
            CardUseStruct use = (CardUseStruct)data;
            if (use.Card == null) return new TriggerStruct();
            if (!use.To.Contains(player) || player.GetMark("Equips_of_Others_nullified_to_You") > 0) return new TriggerStruct();
            FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
            if (fcard is FireAttack || fcard is FireSlash || fcard is BurningCamps)
                return new TriggerStruct(Name, player);
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
    public class WoodenOx : Treasure
    {
        public WoodenOx() : base("WoodenOx") { }
        public override void OnUninstall(Room room, Player player, WrappedCard card)
        {
            player.AddHistory("WoodenOxCard", 0);
            base.OnUninstall(room, player, card);
        }
    }
    public class WoodenOxCard : SkillCard
    {
        public WoodenOxCard() : base("WoodenOxCard")
        {
            target_fixed = true;
            will_throw = false;
            handling_method = HandlingMethod.MethodNone;
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            WrappedCard card = card_use.Card;
            room.AddToPile(card_use.From, "wooden_ox", card.SubCards, false);

            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetOtherPlayers(card_use.From)) {
                if (!p.GetTreasure())
                    targets.Add(p);
            }
            if (targets.Count == 0)
                return;
            Player target = room.AskForPlayerChosen(card_use.From, targets, "WoodenOx", "@wooden_ox-move", true);
            if (target != null)
            {
                WrappedCard treasure = room.GetCard(card_use.From.Treasure.Key);
                if (treasure != null)
                    room.MoveCardTo(treasure, card_use.From, target, Place.PlaceEquip,
                        new CardMoveReason(CardMoveReason.MoveReason.S_REASON_TRANSFER, card_use.From.Name, "WoodenOx", null));
            }
        }
    }
    public class WoodenOxSkill : OneCardViewAsSkill
    {
        public WoodenOxSkill() : base("WoodenOx")
        {
            filter_pattern = ".|.|.|hand";
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return !player.HasUsed("WoodenOxCard");
        }
        public override WrappedCard ViewAs(Room room, WrappedCard card, Player player)
        {
            WrappedCard ox = new WrappedCard("WoodenOxCard");
            ox.AddSubCard(card);
            ox.Skill = "WoodenOx";
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
            if (player.HasTreasure("WoodenOx"))
            {
                int count = 0;
                for (int i = 0; i < move.Card_ids.Count; i++)
                    if (move.From_pile_names[i] == "wooden_ox") count++;

                if (count > 0) return new TriggerStruct(Name, player);
            }
            else if (player.GetPile("wooden_ox").Count == 0)
            {
                for (int i = 0; i < move.Card_ids.Count; i++)
                {
                    if (move.From_places[i] != Place.PlaceEquip && move.From_places[i] != Place.PlaceTable) continue;
                    WrappedCard card = Engine.GetRealCard(move.Card_ids[i]);
                    if (card?.Name == "WoodenOx")
                        return new TriggerStruct(Name, player);
                }
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player target, ref object data, Player ask_who, TriggerStruct info)
        {
            CardsMoveOneTimeStruct move = (CardsMoveOneTimeStruct)data;
            Player player = move.From;
            if (player.HasTreasure("WoodenOx"))
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
                    if (card?.Name == "WoodenOx")
                    {
                        Player to = move.To;
                        if (to != null && to.GetTreasure() && to.Treasure.Value == "WoodenOx"
                            && move.To_place ==  Place.PlaceEquip && move.Reason.Reason == CardMoveReason.MoveReason.S_REASON_TRANSFER)
                        {
                            List<Player> p_list = new List<Player>{ to };
                            room.AddToPile(to, "wooden_ox", player.GetPile("wooden_ox"), false, p_list,
                                new CardMoveReason(CardMoveReason.MoveReason.S_REASON_TRANSFER, player.Name));
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
        public JadeSeal() : base("JadeSeal") { }
    }
    public class JadeSealCard : SkillCard
    {
        public JadeSealCard() : base("JadeSealCard")
        {
            will_throw = false;
            handling_method = HandlingMethod.MethodNone;
        }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            WrappedCard kb = new WrappedCard("KnownBoth");
            FunctionCard fcard = Engine.GetFunctionCard(kb.Name);
            return fcard.TargetFilter(room, targets, to_select, Self, kb);
        }
        public override WrappedCard Validate(Room room, CardUseStruct use)
        {
            WrappedCard kb = new WrappedCard("KnownBoth")
            {
                Skill = "_JadeSeal"
            };
            return kb;
        }
    }
    public class JadeSealViewAsSkill : ZeroCardViewAsSkill
    {
        public JadeSealViewAsSkill() : base("JadeSeal")
        {
            response_pattern = "@@JadeSeal!";
        }
        public override WrappedCard ViewAs(Room room, Player player)
        {
            return new WrappedCard("JadeSealCard");
        }
    }
    public class JadeSealSkill : TreasureSkill
    {
        public JadeSealSkill() : base("JadeSeal")
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseProceeding, TriggerEvent.EventPhaseStart };
            view_as_skill = new JadeSealViewAsSkill();
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (!base.Triggerable(player, room) || !player.HasShownOneGeneral())
                return new TriggerStruct();
            if (triggerEvent == TriggerEvent.EventPhaseProceeding && player.Phase == PlayerPhase.Draw && (int)data >= 0)
            {
                return new TriggerStruct(Name, player);
            }
            else if (triggerEvent == TriggerEvent.EventPhaseStart && player.Phase == PlayerPhase.Play)
            {
                WrappedCard kb = new WrappedCard("KnownBoth")
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
            if (room.AskForUseCard(player, "@@JadeSeal!", "@JadeSeal") == null)
            {
                WrappedCard kb = new WrappedCard("KnownBoth")
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
                    Shuffle.shuffle<Player>(ref targets);
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
        public Drowning() : base("Drowning") { }
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
            List<Player> players = room.AlivePlayers;
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
        public BurningCamps() : base("BurningCamps") { }
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

                    room.BroadcastSkillInvoke(skill.Name, player);
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
        public LureTiger() : base("LureTiger") { }
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
            List<Player> targets = card_use.To;
            List<string> nullified_list = room.ContainsTag("CardUseNullifiedList") ? (List<string>)room.GetTag("CardUseNullifiedList") : new List<string>();
            bool all_nullified = nullified_list.Contains("_ALL_TARGETS");
            foreach (Player target in targets) {
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

                room.CardEffect(effect);
            }
            room.DrawCards(card_use.From, 1, Name);

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
            effect.To.Removed = true;
            room.BroadcastProperty(effect.To, "Removed");
            effect.From.SetFlags("LureTigerUser");
        }
    }
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
        public FightTogether() : base("FightTogether") { }
        public override string GetSubtype()=> "fight_together";
        public override bool IsAvailable(Room room, Player player, WrappedCard card)
        {
            List<string> big_kingdoms = RoomLogic.GetBigKingdoms(room);
            return big_kingdoms.Count > 0 && base.IsAvailable(room, player, card) || CanRecast(room, player, card);
        }
        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            bool big = RoomLogic.GetBigKingdoms(room).Count > 0;
            if (!big || targets.Count > 0 || RoomLogic.IsCardLimited(room, Self, card, HandlingMethod.MethodUse)
                || Engine.IsProhibited(room, Self, to_select, card, targets) != null)
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
                    foreach (Player p in room.AlivePlayers) {
                        string kingdom = (p.HasShownOneGeneral() ? (p.Role == "careerist" ? p.Name : p.Kingdom) : string.Empty);
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
                                room.BroadcastSkillInvoke(skill.Name, player);
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
        public AllianceFeast() : base("AllianceFeast") { }
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
                CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_USE, card_use.From.Name, null, card_use.Card.Skill, null)
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
    public class ThreatenEmperor : SingleTargetTrick
    {
        public ThreatenEmperor() : base("ThreatenEmperor") { target_fixed = true; }
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
                if (big_kingdoms.Count == 1 && big_kingdoms[0].StartsWith("sgs")) // for JadeSeal
                    invoke = big_kingdoms.Contains(player.Name);
                else if (player.Role == "careerist")
                    invoke = false;
                else
                    invoke = big_kingdoms.Contains(player.Kingdom);
            }
            return invoke && RoomLogic.IsProhibited(room, player, player, card) == null && base.IsAvailable(room, player, card);
        }
    }
    public class ThreatenEmperorSkill : TriggerSkill
    {
        public ThreatenEmperorSkill() : base("threaten_emperor")
        {
            events.Add(TriggerEvent.EventPhaseChanging);
            global = true;
        }
        public override int GetPriority() => 1;
        public override List<TriggerStruct> Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            List<TriggerStruct> list = new List<TriggerStruct>();
            PhaseChangeStruct change = (PhaseChangeStruct)data;
            if (change.To != PlayerPhase.NotActive)
                return list;
            foreach (Player p in room.GetAllPlayers())
                if (p.GetMark("ThreatenEmperorExtraTurn") > 0)
                    list.Add(new TriggerStruct(Name, p));

            return list;
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            ask_who.RemoveMark("ThreatenEmperorExtraTurn");
            if (room.AskForCard(ask_who, "..", "@threaten_emperor", data, Name) != null)
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
    public class ImperialOrder : GlobalEffect
    {
        public ImperialOrder() : base("ImperialOrder") { }
        public override bool IsAvailable(Room room, Player player, WrappedCard card)
        {
            return base.IsAvailable(room, player, card);
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

                    room.BroadcastSkillInvoke(skill.Name, p);
                }
                else
                    targets.Add(p);
            }

            card_use.To = targets;
            base.OnUse(room, card_use);
        }
        public override void Use(Room room, CardUseStruct card_use)
        {
            room.SetCardFlag(card_use.Card, "imperial_order_normal_use");
            base.Use(room, card_use);
        }
        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            if (room.AskForCard(effect.To, "EquipCard", "@imperial_order-equip") != null)
                return;
            List<string> choices = new List<string>();
            if (!effect.To.HasShownAllGenerals()
                && ((!effect.To.General1Showed && effect.To.DisableShowList(true).Count == 0)
                || (!string.IsNullOrEmpty(effect.To.General2) && !effect.To.General2Showed && effect.To.DisableShowList(false).Count == 0)))
                choices.Add("show");
            choices.Add("losehp");
            string choice = room.AskForChoice(effect.To, Name, string.Join("+", choices));
            if (choice == "show")
            {
                room.AskForGeneralShow(effect.To);
                room.DrawCards(effect.To, 1, Name);
            }
            else
            {
                room.LoseHp(effect.To);
            }
        }
    }

    public class TransferCard : SkillCard
    {
        public TransferCard() : base("TransferCard")
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
            CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_GIVE, effect.From.Name, effect.To.Name, "transfer", null);
            room.ObtainCard(effect.To, effect.Card, reason);
            if (draw)
                room.DrawCards(effect.From, 1, "transfer");
        }
    }
    #endregion
}
