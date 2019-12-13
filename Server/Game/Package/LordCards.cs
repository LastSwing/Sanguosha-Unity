using CommonClass.Game;
using SanguoshaServer.Game;
using System.Collections.Generic;

namespace SanguoshaServer.Package
{
    public class LordCards : CardPackage
    {
        public LordCards() : base("LordCards")
        {
            cards = new List<FunctionCard>
            {
                new DragonPhoenix(),
                new PeaceSpell(),
                new LuminouSpearl(),
                new DragonCarriage(),
            };
            skills = new List<Skill>
            {
                new DragonPhoenixSkill(),
                //new DragonPhoenixSkill2(),
                new PeaceSpellSkill(),
                new PeaceSpellSkillMaxCards(),
                new LuminouSpearlSkill(),
                //new ZhihengVH(),
                new DragonCarriageSkill(),
                new DragonCarriageDistanceSkill()
            };
        }
    }

    public class DragonPhoenix : Weapon
    {
        public static string ClassName = "DragonPhoenix";
        public DragonPhoenix() : base(ClassName, 2)
        {
        }
    }

    public class DragonPhoenixSkill : WeaponSkill
    {
        public DragonPhoenixSkill() : base(DragonPhoenix.ClassName)
        {
            events = new List<TriggerEvent> { TriggerEvent.TargetChosen, TriggerEvent.Dying };
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.TargetChosen && data is CardUseStruct use)
            {
                FunctionCard fcard = use.Card != null ? Engine.GetFunctionCard(use.Card.Name) : null;
                if (base.Triggerable(player, room) && use.Card != null && fcard is Slash)
                {
                    List<Player> targets = new List<Player>();
                    foreach (Player to in use.To)
                    {
                        if (RoomLogic.CanDiscard(room, to, to, "he"))
                            targets.Add(to);
                    }
                    if (targets.Count > 0)
                        return new TriggerStruct(Name, player, targets);
                }
            }
            else if (triggerEvent == TriggerEvent.Dying && data is DyingStruct dying && dying.Damage.Card != null && dying.Damage.Card.Name.Contains(Slash.ClassName)
                && !dying.Damage.Transfer && !dying.Damage.Chain
                && dying.Damage.From != null && dying.Damage.From.Alive && base.Triggerable(dying.Damage.From, room) && !player.IsKongcheng())
            {
                if (RoomLogic.CanGetCard(room, dying.Damage.From, player, "h"))
                    return new TriggerStruct(Name, dying.Damage.From);
            }

            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            bool invoke = false;
            if (triggerEvent == TriggerEvent.TargetChosen && room.AskForSkillInvoke(ask_who, Name, player))
                invoke = true;
            else if (triggerEvent == TriggerEvent.Dying && data is DyingStruct dying && dying.Damage.From != null
                && dying.Damage.From.Alive && !player.IsKongcheng() && RoomLogic.CanGetCard(room, ask_who, player, "h"))
            {
                ask_who.SetFlags(Name);
                invoke = room.AskForSkillInvoke(ask_who, Name, player);
                ask_who.SetFlags("-DragonPhoenix");
            }

            if (invoke)
            {
                room.SetEmotion(ask_who, "dragonphoenix");
                return info;
            }

            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.TargetChosen)
                room.AskForDiscard(player, Name, 1, 1, false, true, "@dragonphoenix-discard");
            else if (data is DyingStruct dying && dying.Damage.From != null && dying.Damage.From.Alive && !player.IsKongcheng() && RoomLogic.CanGetCard(room, ask_who, player, "h"))
            {
                int id = room.AskForCardChosen(ask_who, player, "h", Name, false, FunctionCard.HandlingMethod.MethodGet);
                CardMoveReason reason = new CardMoveReason(MoveReason.S_REASON_EXTRACTION, ask_who.Name, Name, Name);
                room.ObtainCard(ask_who, room.GetCard(id), reason, false);
            }

            return false;
        }
    }

    /*
    public class DragonPhoenixSkill : WeaponSkill
    {
        public DragonPhoenixSkill() : base(DragonPhoenix.ClassName)
        {
            events.Add(TriggerEvent.TargetChosen);
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            CardUseStruct use = (CardUseStruct)data;
            FunctionCard fcard = use.Card != null ? Engine.GetFunctionCard(use.Card.Name) : null;
            if (base.Triggerable(player, room) && use.Card != null && fcard is Slash)
            {
                List<Player> targets = new List<Player>();
                foreach (Player to in use.To) {
                    if (RoomLogic.CanDiscard(room, to, to, "he"))
                        targets.Add(to);
                }
                if (targets.Count > 0)
                    return new TriggerStruct(Name, player, targets);
            }
            return new TriggerStruct();
        }
        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(ask_who, Name, player))
            {
                room.SetEmotion(ask_who, "dragonphoenix");
                return info;
            }
            return new TriggerStruct();
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.AskForDiscard(player, Name, 1, 1, false, true, "@dragonphoenix-discard");
            return false;
        }
    }
    
    public class DragonPhoenixSkill2 : WeaponSkill
    {
        public DragonPhoenixSkill2() : base("#DragonPhoenix")
        {
            events.Add(TriggerEvent.BuryVictim);
        }
        public override int GetPriority() => -4;
        public override bool Triggerable(Player target, Room room)
        {
            return target != null;
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.Setting.GameMode != "Hegemony") return false;

            Player dfowner = null;
            foreach (Player p in room.GetAlivePlayers()) {
                if (p.HasWeapon(DragonPhoenix.ClassName))
                {
                    dfowner = p;
                    break;
                }
            }
            if (dfowner == null || !dfowner.HasShownOneGeneral() || dfowner.GetRoleEnum() == Player.PlayerRole.Careerist)
                return false;

            DeathStruct death = (DeathStruct)data;
            DamageStruct damage = death.Damage;
            if (damage.From != null || damage.From != dfowner) return false;
            if (damage.Card == null) return false;
            FunctionCard fcard = Engine.GetFunctionCard(damage.Card.Name);
            if (!(fcard is Slash)) return false;

            List<string> kingdom_list = new List<string> { "wei", "shu", "wu", "qun" };
            kingdom_list.Remove(dfowner.Kingdom);
            kingdom_list.Add("careerist");

            int n = RoomLogic.GetPlayerNumWithSameKingdom(room, dfowner);           //could be canceled later
            foreach (string kingdom in kingdom_list) {
                int other_num = RoomLogic.GetPlayerNumWithSameKingdom(room, dfowner, kingdom);
                if (other_num > 0 && other_num < n)
                {
                    return false;
                }
            }

            List<string> generals = room.Generals;
            Shuffle.shuffle<string>(ref generals);
            foreach (string name in room.UsedGeneral)
            if (generals.Contains(name)) generals.Remove(name);
            List<string> avaliable_generals = new List<string>();
            foreach (string general in generals) {
                if (Engine.GetGeneral(general).Kingdom != dfowner.Kingdom)
                    continue;
                avaliable_generals.Add(general);
                if (avaliable_generals.Count >= 3) break;
            }

            if (avaliable_generals.Count == 0)
                return false;

            bool invoke = room.AskForSkillInvoke(dfowner, DragonPhoenix.ClassName, data) && room.AskForSkillInvoke(player, DragonPhoenix.ClassName, "#DragonPhoenix-revive:::" + dfowner.Kingdom);

            if (invoke)
            {
                room.SetEmotion(dfowner, "dragonphoenix");
                player.DuanChang = string.Empty;
                room.BroadcastProperty(player, "DuanChang");
                string to_change = room.AskForGeneral(player, avaliable_generals, null, true, "dpRevive", dfowner.Kingdom, true, true);

                if (!string.IsNullOrEmpty(to_change))
                {
                    room.DoDragonPhoenix(player, to_change, null, false, dfowner.Kingdom, true, "h");
                    player.Hp = 2;
                    room.BroadcastProperty(player, "Hp");

                    room.SetPlayerChained(player, false, false);
                    player.FaceUp = true;
                    room.BroadcastProperty(player, "FaceUp");

                    LogMessage l = new LogMessage
                    {
                        Type = "#DragonPhoenixRevive",
                        From = player.Name,
                        Arg = to_change
                    };
                    room.SendLog(l);

                    room.DrawCards(player, 1, DragonPhoenix.ClassName);
                }
            }
            return false;
        }
    }
    */

    public class PeaceSpell : Armor
    {
        public static string ClassName = "PeaceSpell";
        public PeaceSpell() : base(ClassName) { }
        public override void OnUninstall(Room room, Player player, WrappedCard card)
        {
            if (player.Alive && RoomLogic.HasArmorEffect(room, player, Name, false))
                player.SetFlags("peacespell_throwing");
            base.OnUninstall(room, player, card);
        }
    }
    public class PeaceSpellSkill : ArmorSkill
    {
        public PeaceSpellSkill() : base(PeaceSpell.ClassName)
        {
            events = new List<TriggerEvent> { TriggerEvent.DamageDefined, TriggerEvent.CardsMoveOneTime };
            frequency = Frequency.Compulsory;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.DamageDefined && data is DamageStruct damage && base.Triggerable(player, room) && damage.Nature != DamageStruct.DamageNature.Normal)
            {
                if (player.ArmorIsNullifiedBy(damage.From)) return new TriggerStruct();
                return new TriggerStruct(Name, player);
            }
            else if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.From != null
                && move.From_places.Contains(Player.Place.PlaceEquip) && move.From.HasFlag("peacespell_throwing"))
            {
                for (int i = 0; i < move.Card_ids.Count; i++)
                {
                    if (move.From_places[i] != Player.Place.PlaceEquip) continue;
                    WrappedCard card = Engine.GetRealCard(move.Card_ids[i]);
                    if (card.Name == Name)
                    {
                        Player source = room.FindPlayer(move.Reason.PlayerId);
                        if (move.From.ArmorIsNullifiedBy(source))
                        {
                            move.From.SetFlags("-peacespell_throwing");
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
            return base.Cost(room, ref data, info);
        }
        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.DamageDefined && data is DamageStruct damage)
            {
                LogMessage log = new LogMessage
                {
                    Type = "#damaged-prevent",
                    From = player.Name,
                    Arg = Name
                };
                room.SendLog(log);

                room.SetEmotion(damage.To, "peacespell");
                return true;
            }
            else if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move)
            {
                move.From.SetFlags("-peacespell_throwing");
                LogMessage l = new LogMessage
                {
                    Type = "#PeaceSpellLost",
                    From = move.From.Name
                };
                room.SendLog(l);

                if (move.From.Alive)
                    room.DrawCards(move.From, 2, Name);
                if (move.From.Hp > 1)
                    room.LoseHp(move.From);
            }
            return false;
        }
    }
    public class PeaceSpellSkillMaxCards : MaxCardsSkill
    {
        public PeaceSpellSkillMaxCards() : base("#PeaceSpell-max")
        {
        }
        public override int GetExtra(Room room, Player target)
        {
            if (RoomLogic.HasArmorEffect(room, target, PeaceSpell.ClassName))
            {
                int count = 1;
                foreach (Player p in room.GetOtherPlayers(target))
                    if (RoomLogic.IsFriendWith(room, target, p))
                        count++;

                count += target.GetPile("heavenly_army").Count;

                return count;
            }

            return 0;
        }
    }
    
    public class LuminouSpearl : Treasure
    {
        public static string ClassName = "LuminouSpearl";
        public LuminouSpearl() : base(ClassName)
        {
        }
        public override void OnUninstall(Room room, Player player, WrappedCard card)
        {
            if (!RoomLogic.PlayerHasShownSkill(room, player, "zhiheng"))
                player.ClearHistory("ZhihengCard");
            base.OnUninstall(room, player, card);
        }
    }
    public class LuminouSpearlSkill : ViewAsSkill
    {
        public LuminouSpearlSkill() : base(LuminouSpearl.ClassName)
        {
        }
        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return !RoomLogic.IsJilei(room, player, to_select) && selected.Count < player.MaxHp && to_select.Id != player.Treasure.Key;
        }
        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count == 0)
                return null;

            WrappedCard zhiheng_card = new WrappedCard("ZhihengCard")
            {
                Skill = "zhiheng",
                Mute = true
            };
            zhiheng_card.AddSubCards(cards);
            return zhiheng_card;
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            return RoomLogic.CanDiscard(room, player, player, "he") && !player.HasUsed("ZhihengCard")
                    && !RoomLogic.PlayerHasShownSkill(room, player, "zhiheng");
        }
    }

    /*
    public class ZhihengVH : ViewHasSkill
    {
        public ZhihengVH() : base("zhiheng-viewhas")
        {
            global = true;
            viewhas_skills.Add("zhiheng");
        }
        public override bool ViewHas(Room room, Player player, string skill_name) => player.HasTreasure(LuminouSpearl.ClassName);
    }
    */

    public class DragonCarriage : SpecialEquip
    {
        public static string ClassName = "DragonCarriage";
        public DragonCarriage() : base(ClassName)
        {
        }
    }

    public class DragonCarriageSkill : TriggerSkill
    {
        public DragonCarriageSkill() : base(DragonCarriage.ClassName)
        {
            global = true;
            events = new List<TriggerEvent> { TriggerEvent.CardsMoveOneTime };
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (data is CardsMoveOneTimeStruct move && move.To != null && move.To_place == Player.Place.PlaceEquip && (move.To.GetOffensiveHorse() || move.To.GetDefensiveHorse()))
            {
                bool cart = false;
                foreach (int id in move.Card_ids)
                {
                    if (room.GetCard(id).Name == Name)
                    {
                        cart = true;
                        break;
                    }
                }
                if (cart)
                    return new TriggerStruct(Name, move.To);
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is CardsMoveOneTimeStruct move && move.To != null && (move.To.GetOffensiveHorse() || move.To.GetDefensiveHorse()))
            {
                room.SendCompulsoryTriggerLog(move.To, Name, false);
                List<int> ids = new List<int>();
                if (move.To.GetDefensiveHorse())
                    ids.Add(move.To.DefensiveHorse.Key);
                if (move.To.GetOffensiveHorse())
                    ids.Add(move.To.OffensiveHorse.Key);

                room.ThrowCard(ref ids, move.To, move.To);
            }

            return false;
        }
    }

    public class DragonCarriageDistanceSkill : DistanceSkill
    {
        public DragonCarriageDistanceSkill() : base("DragonCarriage-Distance")
        {
        }

        public override int GetCorrect(Room room, Player from, Player to, WrappedCard card = null)
        {
            int correct = 0;
            if (from.GetSpecialEquip() && from.GetMark("Equips_nullified_to_Yourself") == 0
                    && (card == null || !card.SubCards.Contains(from.Special.Key)))
            {
                correct -= 1;
            }
            if (to.GetSpecialEquip() && to.GetMark("Equips_nullified_to_Yourself") == 0
                    && (card == null || !card.SubCards.Contains(to.Special.Key)))
            {
                correct += 1;
            }

            return correct;
        }
    }
}
