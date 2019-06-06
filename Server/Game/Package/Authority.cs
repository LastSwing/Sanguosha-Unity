using CommonClass;
using CommonClass.Game;
using CommonClassLibrary;
using SanguoshaServer.Game;
using SanguoshaServer.Scenario;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using static CommonClass.Game.Player;

namespace SanguoshaServer.Package
{
    public class Authority : GeneralPackage
    {
        public Authority() : base("Authority")
        {
            skills = new List<Skill>
            {
                new Zhengpi(),
                new ZhengpiTar(),
                new Fengying(),
                new Fudi(),
                new Congjian(),
            };
            skill_cards = new List<FunctionCard>
            {
                new FengyingCard(),
            };
            related_skills = new Dictionary<string, List<string>>
            {
                { "zhengpi", new List<string> { "#zhengpi-target" } },
            };
        }
    }

    //cuimao
    public class Zhengpi : TriggerSkill
    {
        public Zhengpi() : base("zhengpi")
        {
            events.Add(TriggerEvent.EventPhaseStart);
            skill_type = SkillType.Attack;
        }

        public override bool Triggerable(Player target, Room room)
        {
            if (target.Phase == PlayerPhase.Play && base.Triggerable(target, room))
            {
                bool check = false;
                foreach (Player p in room.GetOtherPlayers(target))
                {
                    if (!p.HasShownOneGeneral())
                    {
                        check = true;
                        break;
                    }
                }
                return check;
            }

            return false;
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            List<Player> targets = new List<Player>();
            foreach (Player p in room.GetOtherPlayers(player))
            {
                if (!p.HasShownOneGeneral())
                {
                    targets.Add(p);
                }
            }

            if (targets.Count > 0)
            {
                Player target = room.AskForPlayerChosen(player, targets, Name, "zhengpi-target", true, false, info.SkillPosition);
                if (target != null)
                {
                    room.SetTag(Name, target);
                    room.DoAnimate(AnimateType.S_ANIMATE_INDICATE, player.Name, target.Name);
                    GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                    room.BroadcastSkillInvoke(Name, "male", 1, gsk.General, gsk.SkinId);
                    return info;
                }
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            Player target = room.ContainsTag(Name) ? (Player)room.GetTag(Name) : null;
            room.RemoveTag(Name);
            if (target != null)
            {
                player.SetFlags(Name);
                target.SetFlags(Name + "target");
            }
            return false;
        }

        public override int GetEffectIndex(Room room, Player player, WrappedCard card)
        {
            return 2;
        }
    }

    public class ZhengpiTar : TargetModSkill
    {
        public ZhengpiTar() : base("#zhengpi-target")
        {
        }

        public override bool CheckSpecificAssignee(Room room, Player from, Player to, WrappedCard card)
        {
            if (from.HasFlag("zhengpi") && to != null && to.HasFlag("zhengpi_target"))
            {
                return true;
            }

            return false;
        }

        public override bool GetDistanceLimit(Room room, Player from, Player to, WrappedCard card)
        {
            if (from.HasFlag("zhengpi") && to != null && to.HasFlag("zhengpi_target"))
            {
                return true;
            }

            return false;
        }
    }

    public class Fengying : ViewAsSkill
    {
        public Fengying() : base("fengying")
        {
            limit_mark = "@lord";
        }
        public override bool IsEnabledAtPlay(Room room, Player player)
        {
            bool big = false;
            List<string> bigs = RoomLogic.GetBigKingdoms(room);
            if (player.HasShownOneGeneral())
            {
                if ((player.Role == "careerist" && bigs.Contains(player.Name)) || (player.Role != "careerist" && bigs.Contains(player.Kingdom)))
                    big = true;
            }
            else
            {
                string role = Hegemony.WillbeRole(room, player);
                if (player.HasTreasure("JadeSeal") || bigs.Contains(role))
                    big = true;
                else if (role != "careerist")
                {
                    List<Player> players = room.GetOtherPlayers(player);
                    Player jade_seal_owner = null;
                    foreach (Player p in players)
                    {
                        if (p.HasTreasure("JadeSeal") && p.HasShownOneGeneral())
                        {
                            jade_seal_owner = p;
                            break;
                        }
                    }
                    if (jade_seal_owner == null)
                    {
                        Dictionary<string, int> kingdom_map = new Dictionary<string, int>
                        {
                            { "wei", 0 },
                            { "shu", 0 },
                            { "wu", 0 },
                            { "qun", 0 }
                        };
                        foreach (Player p in players)
                        {
                            if (!p.HasShownOneGeneral())
                                continue;
                            if (p.Role != "careerist")
                                ++kingdom_map[p.Kingdom];
                        }
                        kingdom_map[role]++;
                        List<string> kingdoms = new List<string>(kingdom_map.Keys);
                        kingdoms.Sort((x, y) => { return kingdom_map[x] > kingdom_map[y] ? -1 : 1; });
                        if (kingdom_map[role] == kingdom_map[kingdoms[0]] && kingdom_map[role] >= 2)
                            big = true;
                    }
                }
            }

            return big && player.GetMark("@lord") > 0 && !player.IsKongcheng();
        }

        public override bool ViewFilter(Room room, List<WrappedCard> selected, WrappedCard to_select, Player player)
        {
            return false;
        }

        public override WrappedCard ViewAs(Room room, List<WrappedCard> cards, Player player)
        {
            if (cards.Count == 0)
            {
                WrappedCard card = new WrappedCard("FengyingCard")
                {
                    Skill = Name,
                    ShowSkill = Name,
                };

                return card;
            }

            return null;
        }
    }

    public class FengyingCard : SkillCard
    {
        public FengyingCard() : base("FengyingCard")
        {
            target_fixed = true;
        }

        public override void Use(Room room, CardUseStruct card_use)
        {
            Player player = card_use.From;
            WrappedCard card = new WrappedCard("ThreatenEmperor")
            {
                Skill = "_fengying"
            };
            card.AddSubCards(player.HandCards);

            CardUseStruct use = new CardUseStruct(card, player, new List<Player>());
            room.UseCard(use);

            List<string> bigs = RoomLogic.GetBigKingdoms(room);
            List<Player> drawers = new List<Player>();
            foreach (Player p in room.GetAlivePlayers())
                if (p.HasShownOneGeneral() && ((p.Role == "careerist" && bigs.Contains(p.Name)) || (p.Role != "careerist" && bigs.Contains(p.Kingdom))))
                    drawers.Add(p);

            if (drawers.Count > 0)
            {
                room.SortByActionOrder(ref drawers);
                foreach (Player p in drawers)
                {
                    int i = p.MaxHp - p.HandcardNum;
                    if (i > 0)
                        room.DrawCards(p, new DrawCardStruct(i, player, "fengying"));
                }
            }
        }
    }

    //zhangxiu
    public class Fudi : MasochismSkill
    {
        public Fudi() : base("fudi")
        {
        }
        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room) && !player.IsKongcheng() && data is DamageStruct damage && damage.From != null && damage.From.Alive)
            {
                return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (!player.IsKongcheng() && data is DamageStruct damage && damage.From != null && damage.From.Alive)
            {
                List<int> ids = room.AskForExchange(player, Name, 1, 0, "@fudi::" + damage.From.Name, string.Empty, ".", info.SkillPosition);
                if (ids.Count == 1)
                {
                    CardMoveReason reason = new CardMoveReason(CardMoveReason.MoveReason.S_REASON_GIVE, player.Name, damage.From.Name, Name, null);
                    room.BroadcastSkillInvoke(Name, player, info.SkillPosition);
                    room.ObtainCard(damage.From, ids, reason);
                    return info;
                }
            }

            return new TriggerStruct();
        }

        public override void OnDamaged(Room room, Player player, DamageStruct damage, TriggerStruct info)
        {
            Player from = damage.From;
            List<Player> targets = new List<Player>();
            int max = player.Hp;
            foreach (Player p in room.GetAlivePlayers())
            {
                if (RoomLogic.IsFriendWith(room, p, from) && p.Hp >= max)
                {
                    max = p.Hp;
                    targets.Add(p);
                }
            }

            targets.RemoveAll(t => (t.Hp < max));

            if (targets.Count > 0)
            {
                Player target = room.AskForPlayerChosen(player, targets, Name, "@fudi-target", false, false, info.SkillPosition);
                if (target != null)
                    room.Damage(new DamageStruct(Name, player, target));
            }
        }
    }

    public class Congjian : TriggerSkill
    {
        public Congjian() : base("congjian")
        {
            events = new List<TriggerEvent> { TriggerEvent.DamageCaused, TriggerEvent.DamageInflicted };
            frequency = Frequency.Compulsory;
        }

        public override TriggerStruct Triggerable(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who)
        {
            if (base.Triggerable(player, room))
            {
                if ((triggerEvent == TriggerEvent.DamageCaused && player.Phase == PlayerPhase.NotActive)
                    || (triggerEvent == TriggerEvent.DamageInflicted && room.Current == player))
                    return new TriggerStruct(Name, player);
            }

            return new TriggerStruct();
        }

        public override TriggerStruct Cost(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            if (RoomLogic.PlayerHasShownSkill(room, player, Name) || room.AskForSkillInvoke(player, Name, data, info.SkillPosition))
            {
                GeneralSkin gsk = RoomLogic.GetGeneralSkin(room, player, Name, info.SkillPosition);
                room.BroadcastSkillInvoke(Name, "male", triggerEvent == TriggerEvent.DamageCaused ? 1 : 2, gsk.General, gsk.SkinId);
                return info;
            }
            return new TriggerStruct();
        }

        public override bool Effect(TriggerEvent triggerEvent, Room room, Player player, ref object data, Player ask_who, TriggerStruct info)
        {
            room.SendCompulsoryTriggerLog(player, Name);
            DamageStruct damage = (DamageStruct)data;
            if (triggerEvent == TriggerEvent.DamageCaused)
            {
                LogMessage log = new LogMessage
                {
                    Type = "#CongjianBuff",
                    From = player.Name,
                    To = new List<string> { damage.To.Name },
                    Arg = damage.Damage.ToString(),
                    Arg2 = (++damage.Damage).ToString()
                };
                room.SendLog(log);

                data = damage;

            }
            else
            {
                LogMessage log = new LogMessage
                {
                    Type = "#CongjianDamage",
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
}
