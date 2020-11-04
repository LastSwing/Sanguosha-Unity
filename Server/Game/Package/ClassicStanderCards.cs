using CommonClass.Game;
using CommonClassLibrary;
using SanguoshaServer.Game;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace SanguoshaServer.Package
{
    public class ClassicStanderCards : CardPackage
    {
        public ClassicStanderCards() : base("ClassicStanderCards")
        {
            skills = new List<Skill>
            {
                new ClassicBladeSkill(),
                new BladeTag(),
                new ClassicHalberdSkill(),
                new SaberSkill(),  //七宝刀
                new LanceSkill(),
                new LightningSummonerSkill(),
                new PosionedDaggerSkill(),
                new QuenchedKnifeSkill(),
                new QuenchedKnifeTM(),
                new WaterSwordSkill(),
            };
            cards = new List<FunctionCard> {
                new ClassicBlade(),
                new ClassicHalberd(),
                new Saber(),            //七宝刀
                new HiddenDagger(),     //笑里藏刀
                new HoneyTrap(),        //美人计
                new Lance(),            //红缎枪
                new LightningSummoner(),//天雷刃
                new PosionedDagger(),   //混毒弯匕
                new QuenchedKnife(),    //烈淬刀
                new WaterSword(),       //水波剑
            };

            related_skills = new Dictionary<string, List<string>>
            {
                { ClassicBlade.ClassName, new List<string> { "#blade-target-mod" } },
                { QuenchedKnife.ClassName, new List<string> { "#QuenchedKnife-tm" } },
            };
        }
    }

    public class ClassicBlade : Weapon
    {
        public static string ClassName = "ClassicBlade";
        public ClassicBlade() : base(ClassName, 3) { }
    }

    public class ClassicBladeSkill : WeaponSkill
    {
        public ClassicBladeSkill() : base(ClassicBlade.ClassName)
        {
            events = new List<TriggerEvent> { TriggerEvent.SlashMissed };
            frequency = Frequency.Compulsory;
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            SlashEffectStruct effect = (SlashEffectStruct)data;
            if (!effect.To.Alive || effect.To.GetMark("Equips_of_Others_nullified_to_You") > 0)
                return false;

            player.SetFlags("slashTargetFix");
            player.SetFlags("slashTargetFixToOne");
            room.SetCardFlag(player.Weapon.Key, "using");
            effect.To.SetFlags("SlashAssignee");

            WrappedCard slash = room.AskForUseCard(player, "Slash:ClassicBlade", "@blade:" + effect.To.Name, null, -1, FunctionCard.HandlingMethod.MethodUse, false);
            if (slash == null)
            {
                player.SetFlags("-slashTargetFix");
                player.SetFlags("-slashTargetFixToOne");
                effect.To.SetFlags("-SlashAssignee");
                if (player.HasWeapon(Name))
                    room.SetCardFlag(player.Weapon.Key, "-using");
            }

            return false;
        }
    }

    public class BladeTag : TargetModSkill
    {
        public BladeTag() : base("#blade-target-mod", false) {}
        public override bool GetDistanceLimit(Room room, Player from, Player to, WrappedCard card, CardUseStruct.CardUseReason reason, string pattern)
        {
            if (from.HasWeapon("ClassicBlade") && reason == CardUseStruct.CardUseReason.CARD_USE_REASON_RESPONSE_USE
                && (room.GetRoomState().GetCurrentResponseSkill() == "ClassicBlade" || pattern == "Slash:ClassicBlade"))
                return true;

            return false;
        }
    }

    public class ClassicHalberd : Weapon
    {
        public static string ClassName = "ClassicHalberd";
        public ClassicHalberd() : base(ClassName, 4) { }
    }

    public class ClassicHalberdSkill : TargetModSkill
    {
        public ClassicHalberdSkill() : base(ClassicHalberd.ClassName) { }

        public override int GetExtraTargetNum(Room room, Player from, WrappedCard card)
        {
            if (from.HasWeapon(Name) && card.SubCards.Count > 0 && from.IsLastHandCard(card, true) && !card.SubCards.Contains(from.Weapon.Key))
                return 2;

            return 0;
        }
    }

    public class Saber : Weapon
    {
        public static string ClassName = "Saber";
        public Saber() : base(ClassName, 2) { }
    }
    public class SaberSkill : WeaponSkill
    {
        public SaberSkill() : base(Saber.ClassName)
        {
            events = new List<TriggerEvent> { TriggerEvent.TargetChosen, TriggerEvent.DamageCaused };
            frequency = Frequency.Compulsory;
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.TargetChosen && data is CardUseStruct use && base.Triggerable(player, room) && use.Card.Name.Contains(Slash.ClassName) && use.To.Count > 0)
                return new TriggerStruct(Name, player, use.To);
            else if (triggerEvent == TriggerEvent.DamageCaused && data is DamageStruct damage && base.Triggerable(player, room) && !damage.To.IsWounded()
                && damage.To.Alive && damage.Card != null && damage.Card.Name.Contains(Slash.ClassName) && !damage.Chain && !damage.Transfer)
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SetEmotion(ask_who, "saber");
            Thread.Sleep(400);
            return info;
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player target, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.TargetChosen && data is CardUseStruct use)
            {
                target.AddQinggangTag(RoomLogic.CardToString(room, use.Card));
            }
            else if (triggerEvent == TriggerEvent.DamageCaused && data is DamageStruct damage)
            {
                room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, ask_who.Name, damage.To.Name);
                LogMessage log = new LogMessage
                {
                    Type = "#AddDamage",
                    From = ask_who.Name,
                    To = new List<string> { damage.To.Name },
                    Arg = Name,
                    Arg2 = (++damage.Damage).ToString()
                };

                room.SendLog(log);
                data = damage;
            }

            return false;
        }
    }

    public class HiddenDagger : TrickCard
    {
        public static string ClassName = "HiddenDagger";
        public override string GetSubtype() => "single_target_trick";
        public HiddenDagger() : base(ClassName)
        {
        }

        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            Player player = effect.From, target = effect.To;
            int count = target.GetLostHp();
            if (count > 0) room.DrawCards(target, new DrawCardStruct(count, player, Name));
            if (player.Alive && target.Alive)
                room.Damage(new DamageStruct(effect.Card, player, target));
        }
    }

    public class HoneyTrap : TrickCard
    {
        public static string ClassName = "HoneyTrap";
        public override string GetSubtype() => "single_target_trick";
        public HoneyTrap() : base(ClassName)
        {
        }

        public override bool TargetFilter(Room room, List<Player> targets, Player to_select, Player Self, WrappedCard card)
        {
            return base.TargetFilter(room, targets, to_select, Self, card) && to_select.IsMale() && !to_select.IsKongcheng();
        }

        public override void OnEffect(Room room, CardEffectStruct effect)
        {
            Player player = effect.From, target = effect.To;
            List<Player> players = new List<Player>();
            foreach (Player p in room.GetAlivePlayers())
                if (p.IsFemale()) players.Add(p);

            if (players.Count > 0)
            {
                room.SortByActionOrder(ref players);
                foreach (Player p in players)
                {
                    if (target.Alive && !target.IsKongcheng() && RoomLogic.CanGetCard(room, p, target, "h"))
                    {
                        int id = room.AskForCardChosen(p, target, "h", Name, false, HandlingMethod.MethodGet);
                        List<int> ids = new List<int> { id };
                        room.ObtainCard(p, ref ids, new CardMoveReason(MoveReason.S_REASON_EXTRACTION, p.Name, target.Name, Name, string.Empty), false);

                        if (p != player && p.Alive && player.Alive)
                        {
                            ids = p.GetCards("h");
                            if (ids.Count > 1)
                                ids = room.AskForExchange(p, Name, 1, 1, "@HoneyTrap:" + player.Name, string.Empty, ".", string.Empty);

                            if (ids.Count > 0)
                                room.ObtainCard(player, ref ids, new CardMoveReason(MoveReason.S_REASON_GIVE, p.Name, player.Name, Name, string.Empty), false);
                        }
                    }
                }
            }

            if (player.Alive && target.Alive && player.HandcardNum != target.HandcardNum)
            {
                if (player.HandcardNum < target.HandcardNum)
                    room.Damage(new DamageStruct(effect.Card, player, target));
                else
                    room.Damage(new DamageStruct(effect.Card, target, player));
            }
        }
    }

    public class Lance : Weapon
    {
        public static string ClassName = "Lance";
        public Lance() : base(ClassName, 3) { }
        public override void OnUninstall(Room room, Player player, WrappedCard card)
        {
            player.SetFlags("-Lance");
        }
    }

    public class LanceSkill : WeaponSkill
    {
        public LanceSkill() : base(Lance.ClassName)
        {
            events= new List<TriggerEvent> { TriggerEvent.Damage, TriggerEvent.EventPhaseChanging };
            skill_type = SkillType.Recover;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == Player.PlayerPhase.NotActive)
            {
                foreach (Player p in room.GetAlivePlayers())
                    if (p.HasFlag(Name))
                        p.SetFlags("-Lance");
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && !player.HasFlag(Name) && data is DamageStruct damage && damage.Card != null && damage.Card.Name.Contains(Slash.ClassName))
                return new TriggerStruct(Name, player);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (room.AskForSkillInvoke(player, Name))
                return info;

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            player.SetFlags(Name);
            JudgeStruct judge = new JudgeStruct
            {
                Good = true,
                //Pattern = ".|red",
                PlayAnimation = false,
                Reason = Name,
                Who = player,
                Negative = false
            };
            
            room.SetCardFlag(player.Weapon.Key, "using");
            room.Judge(ref judge);
            if (player.HasWeapon(Name))
                room.SetCardFlag(player.Weapon.Key, "-using");

            if (player.Alive)
            {
                if (WrappedCard.IsRed(judge.Card.Suit))
                {
                    if (player.IsWounded())
                    {
                        RecoverStruct recover = new RecoverStruct
                        {
                            Recover = 1,
                            Who = player
                        };
                        room.Recover(player, recover, true);
                    }
                }
                else
                {
                    room.DrawCards(player, 2, Name);
                }
            }

            return false;
        }
    }

    public class LightningSummoner : Weapon
    {
        public static string ClassName = "LightningSummoner";
        public LightningSummoner() : base(ClassName, 4) { }
    }

    public class LightningSummonerSkill : WeaponSkill
    {
        public LightningSummonerSkill() : base(LightningSummoner.ClassName)
        {
            events.Add(TriggerEvent.TargetChosen);
            skill_type = SkillType.Attack;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is CardUseStruct use && use.Card.Name.Contains(Slash.ClassName))
                return new TriggerStruct(Name, player, use.To);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player target, ref object data, Player ask_who, TriggerStruct info)
        {
            if (target.Alive)
            {
                bool invoke = room.AskForSkillInvoke(ask_who, Name, target);
                if (invoke) return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player target, ref object data, Player ask_who, TriggerStruct info)
        {
            if (target.Alive && ask_who.Alive)
            {
                JudgeStruct judge = new JudgeStruct
                {
                    Good = true,
                    Pattern = ".|spade#club",
                    PlayAnimation = true,
                    Reason = Name,
                    Who = target,
                    Negative = true
                };

                room.SetCardFlag(ask_who.Weapon.Key, "using");
                room.Judge(ref judge);
                if (ask_who.HasWeapon(Name))
                    room.SetCardFlag(ask_who.Weapon.Key, "-using");

                if (judge.IsEffected() && target.Alive)
                {
                    if (judge.Card.Suit == WrappedCard.CardSuit.Spade)
                        room.Damage(new DamageStruct(Name, null, target, 3, DamageStruct.DamageNature.Thunder));
                    else
                    {
                        room.Damage(new DamageStruct(Name, null, target, 1, DamageStruct.DamageNature.Thunder));
                        if (ask_who.Alive && ask_who.IsWounded())
                        {
                            RecoverStruct recover = new RecoverStruct
                            {
                                Recover = 1
                            };
                            room.Recover(ask_who, recover, true);
                        }
                        if (ask_who.Alive)
                            room.DrawCards(ask_who, 1, Name);
                    }
                }
            }

            return false;
        }
    }

    public class PosionedDagger : Weapon
    {
        public static string ClassName = "PosionedDagger";
        public PosionedDagger() : base(ClassName, 1) { }
        public override void OnUninstall(Room room, Player player, WrappedCard card)
        {
            player.SetMark(Name, 0);
        }
    }

    public class PosionedDaggerSkill : WeaponSkill
    {
        public PosionedDaggerSkill() : base(PosionedDagger.ClassName)
        {
            events = new List<TriggerEvent> { TriggerEvent.TargetChosen, TriggerEvent.EventPhaseChanging };
            skill_type = SkillType.Attack;
        }
        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == Player.PlayerPhase.NotActive)
            {
                foreach (Player p in room.GetAlivePlayers())
                    if (p.GetMark(Name) > 0)
                        p.SetMark(Name, 0);
            }
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && data is CardUseStruct use && use.Card.Name.Contains(Slash.ClassName) && player.GetMark(Name) < 5)
                return new TriggerStruct(Name, player, use.To);

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player target, ref object data, Player ask_who, TriggerStruct info)
        {
            if (ask_who.GetMark(Name) < 5 && ask_who.Alive && target.Alive)
            {
                bool invoke = room.AskForSkillInvoke(ask_who, Name, target);
                if (invoke) return info;
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player target, ref object data, Player ask_who, TriggerStruct info)
        {
            ask_who.AddMark(Name);
            if (target.Alive) room.LoseHp(target);

            return false;
        }
    }
    public class QuenchedKnife : Weapon
    {
        public static string ClassName = "QuenchedKnife";
        public QuenchedKnife() : base(ClassName, 2) { }
        public override void OnUninstall(Room room, Player player, WrappedCard card)
        {
            player.SetMark(Name, 0);
        }
    }

    public class QuenchedKnifeSkill : WeaponSkill
    {
        public QuenchedKnifeSkill() : base(QuenchedKnife.ClassName)
        {
            events = new List<TriggerEvent> { TriggerEvent.DamageCaused, TriggerEvent.EventPhaseChanging };
            skill_type = SkillType.Attack;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == Player.PlayerPhase.NotActive)
            {
                foreach (Player p in room.GetAlivePlayers())
                    if (p.GetMark(Name) > 0)
                        p.SetMark(Name, 0);
            }
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.DamageCaused && base.Triggerable(player, room) && data is DamageStruct damage && damage.Card != null
                && damage.Card.Name.Contains(Slash.ClassName) && !damage.Transfer && !damage.Chain && !player.IsNude() && player.GetMark(Name) < 2)
            {
                return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (data is DamageStruct damage)
            {
                room.SetCardFlag(player.Weapon.Key, "using");
                room.SetTag(Name, data);
                //List<int> ids = room.AskForExchange(player, Name, 1, 0, "@QuenchedKnife:" + damage.To.Name, string.Empty, "Slash,Weapon!", string.Empty);
                if (room.AskForDiscard(player, Name, 1, 1, true, true, "@QuenchedKnife:" + damage.To.Name, true, null))
                {
                    room.RemoveTag(Name);
                    room.SetCardFlag(player.Weapon.Key, "-using");

                    return info;
                }
                room.SetCardFlag(player.Weapon.Key, "-using");
            }

            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            player.AddMark(Name);
            DamageStruct damage = (DamageStruct)data;
            LogMessage log = new LogMessage
            {
                Type = "#AddDamage",
                From = player.Name,
                To = new List<string> { damage.To.Name },
                Arg = Name,
                Arg2 = (++damage.Damage).ToString()
            };
            room.SendLog(log);
            data = damage;

            return false;
        }
    }

    public class QuenchedKnifeTM : TargetModSkill
    {
        public QuenchedKnifeTM() : base("#QuenchedKnife-tm", false)
        {
        }
        public override int GetResidueNum(Room room, Player from, WrappedCard card)
        {
            return from.HasWeapon("QuenchedKnife") ? 1 : 0;
        }
    }

    public class WaterSword : Weapon
    {
        public static string ClassName = "WaterSword";
        public WaterSword() : base(ClassName, 2) { }
        public override void OnUninstall(Room room, Player player, WrappedCard card)
        {
            if (player.Alive && player.HasWeapon(Name))
                player.SetFlags("WaterSwordRecover");

            player.SetMark(Name, 0);
        }
    }

    public class WaterSwordSkill : WeaponSkill
    {
        public WaterSwordSkill() : base(WaterSword.ClassName)
        {
            events = new List<TriggerEvent> { TriggerEvent.EventPhaseChanging, TriggerEvent.CardTargetAnnounced, TriggerEvent.CardsMoveOneTime };
            skill_type = SkillType.Wizzard;
        }

        public override void Record(TriggerEvent triggerEvent, Room room, Player player, ref object data)
        {
            if (triggerEvent == TriggerEvent.EventPhaseChanging && data is PhaseChangeStruct change && change.To == Player.PlayerPhase.NotActive)
                foreach (Player p in room.GetAlivePlayers())
                    if (p.GetMark(Name) > 0)
                        p.SetMark(Name, 0);
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && base.Triggerable(player, room) && player.GetMark(Name) < 2
                && data is CardUseStruct use && use.To.Count > 0 && use.Card.Name != Collateral.ClassName)
            {
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                if (fcard is Slash || fcard.IsNDTrick())
                    return new TriggerStruct(Name, player);
            }
            else if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move && move.From != null
                && move.From_places.Contains(Player.Place.PlaceEquip) && move.From.HasFlag("WaterSwordRecover"))
            {
                for (int i = 0; i < move.Card_ids.Count; i++)
                {
                    if (move.From_places[i] != Player.Place.PlaceEquip) continue;
                    WrappedCard card = Engine.GetRealCard(move.Card_ids[i]);
                    if (card.Name == Name)
                    {
                        if (!move.From.IsWounded())
                        {
                            move.From.SetFlags("-WaterSwordRecover");
                            return new TriggerStruct();
                        }
                        return new TriggerStruct(Name, move.From);
                    }
                }
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (triggerEvent == TriggerEvent.CardTargetAnnounced && data is CardUseStruct use)
            {
                player.AddMark(Name);
                List<Player> targets = new List<Player>();
                FunctionCard fcard = Engine.GetFunctionCard(use.Card.Name);
                foreach (Player p in room.GetAlivePlayers())
                {
                    if (!use.To.Contains(p))
                    {
                        if (fcard.TargetFixed(use.Card) && fcard.Name == ExNihilo.ClassName)
                            targets.Add(p);
                        else if (!fcard.TargetFixed(use.Card) && fcard.TargetFilter(room, new List<Player>(), p, player, use.Card))
                            targets.Add(p);
                    }
                }

                if (targets.Count > 0)
                {
                    room.SetTag(Name, data);
                    Player target = room.AskForPlayerChosen(player, targets, Name, string.Format("@WaterSword:::{0}", use.Card.Name), true, true, info.SkillPosition);
                    room.RemoveTag(Name);
                    if (target != null)
                    {
                        use.To.Add(target);
                        LogMessage log = new LogMessage
                        {
                            Type = "$extra_target",
                            From = player.Name,
                            To = new List<string> { target.Name },
                            Card_str = RoomLogic.CardToString(room, use.Card),
                            Arg = Name
                        };
                        room.SendLog(log);
                        room.SortByActionOrder(ref use);
                        data = use;
                    }
                }
            }
            else if (triggerEvent == TriggerEvent.CardsMoveOneTime && data is CardsMoveOneTimeStruct move)
            {
                for (int i = 0; i < move.Card_ids.Count; i++)
                {
                    if (move.From_places[i] != Player.Place.PlaceEquip) continue;
                    WrappedCard card = Engine.GetRealCard(move.Card_ids[i]);
                    if (card.Name == Name)
                    {
                        move.From.SetFlags("-WaterSwordRecover");

                        //room.SetEmotion(move.From, "silverlion");
                        //Thread.Sleep(400);
                        RecoverStruct recover = new RecoverStruct
                        {
                            Recover = 1,
                            Card = card
                        };
                        room.Recover(move.From, recover, true);

                        return false;
                    }
                }
            }

            return false;
        }
    }
}