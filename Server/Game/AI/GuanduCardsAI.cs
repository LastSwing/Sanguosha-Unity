using System;
using System.Collections.Generic;
using CommonClass.Game;
using SanguoshaServer.Game;
using SanguoshaServer.Package;

namespace SanguoshaServer.AI
{
    class GuanduCardsAI : AIPackage
    {
        public GuanduCardsAI() : base("GuanduCards")
        {
            use_cards = new List<UseCard>
            {
                new CatapultAI(),
                new HoardUpAI(),
                new ReinforcementAI(),
                new GDFighttogetherAI(),
            };
        }
    }

    public class CatapultAI : UseCard
    {
        public CatapultAI() : base(Catapult.ClassName) { }

        public override double UsePriorityAdjust(TrustedAI ai, Player player, List<Player> targets, WrappedCard card)
        {
            return ai.GetEquipPriorityAdjust(card);
        }
        public override double CardValue(TrustedAI ai, Player player, bool use, WrappedCard card, Player.Place place)
        {
            return ai.AjustWeaponRangeValue(player, card);
        }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            ai.UseEquipCard(ref use, card);
        }

        public override bool OnSkillInvoke(TrustedAI ai, Player player, object data)
        {
            if (data is Player to && ai.IsEnemy(to))
                return true;

            return false;
        }
    }

    public class HoardUpAI : UseCard
    {
        public HoardUpAI() : base(HoardUp.ClassName)
        { }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            List<Player> targets = new List<Player>();
            Room room = ai.Room;

            List<Player> friends = ai.GetFriends(player);
            ai.SortByDefense(ref friends, false);
            foreach (Player p in friends)
                if (HoardUp.Instance.TargetFilter(room, targets, p, player, card))
                    targets.Add(p);

            if (targets.Count > 0)
            {
                use.Card = card;
                use.To = targets;
            }
        }
    }

    public class ReinforcementAI : UseCard
    {
        public ReinforcementAI() : base(Reinforcement.ClassName)
        {}

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            List<Player> targets = new List<Player>();
            Room room = ai.Room;
            foreach (Player p in ai.FriendNoSelf)
                if (Reinforcement.Instance.TargetFilter(room, targets, p, player, card))
                    targets.Add(p);

            if (targets.Count > 0)
            {
                use.Card = card;
                use.To = targets;
            }
        }

        public override NulliResult OnNullification(TrustedAI ai, CardEffectStruct effect, bool positive, bool keep)
        {
            NulliResult result = new NulliResult();
            Room room = ai.Room;
            Player player = ai.Self;
            Player from = effect.From, to = effect.To;
            WrappedCard trick = effect.Card;
            List<Player> delete = new List<Player>(effect.StackPlayers);
            List<Player> targets = new List<Player>(delete);
            foreach (Player p in delete)
                if (delete.IndexOf(p) < delete.IndexOf(to))
                    targets.Remove(p);

            double good = 0;
            if (positive)
            {
                if (ai.IsEnemy(to))
                {
                    good += 4;
                    if (ai.HasSkill(TrustedAI.MasochismSkill, to))
                        good += 3;
                    if (ai.IsWeak(to) && to.Hp <= 1)
                        good += 5;

                    if (good > 8)
                        result.Null = true;
                }
            }
            else
            {
                if (ai.IsFriend(to))
                {
                    good += 4;
                    if (ai.HasSkill(TrustedAI.MasochismSkill, to))
                        good += 3;
                    if (ai.IsWeak(to) && to.Hp <= 1)
                        good += 5;

                    if (good > 8)
                        result.Null = true;
                }
            }
            return result;
        }
    }

    public class GDFighttogetherAI : UseCard
    {
        public GDFighttogetherAI() : base(GDFighttogether.ClassName) { }

        public override void Use(TrustedAI ai, Player player, ref CardUseStruct use, WrappedCard card)
        {
            int self_count = 0, enemies = 0;
            Room room = ai.Room;
            foreach (Player p in ai.GetFriends(player))
                if (p.Chained && GDFighttogether.Instance.TargetFilter(room, new List<Player>(), p, player, card))
                    self_count++;

            List<Player> _enemies = ai.GetEnemies(player);
            bool vine = false;
            foreach (Player p in room.GetAlivePlayers())
            {
                if (p.HasArmor(Vine.ClassName) && (p.Chained || _enemies.Contains(p)) && !ai.HasSkill("gangzhi", p))
                {
                    vine = true;
                    break;
                }
            }

            foreach (Player p in _enemies)
            {
                if (!p.Chained && GDFighttogether.Instance.TargetFilter(room, new List<Player>(), p, player, card))
                {
                    enemies++;
                    if (RoomLogic.PlayerHasSkill(room, p, "gangzhi"))
                        enemies++;
                }
            }

            if (enemies > 0 && vine) enemies++;

            use.Card = card;
            if (self_count < enemies && _enemies.Count > 1)
            {
                use.To.Add(ai.GetEnemies(player)[0]);
            }
            else if (self_count > 1)
                use.To.Add(player);

            if (use.To.Count == 0 && !GDFighttogether.Instance.CanRecast(room, player, card))
                use.Card = null;
        }

        public override NulliResult OnNullification(TrustedAI ai, CardEffectStruct effect, bool positive, bool keep)
        {
            NulliResult result = new NulliResult();
            Room room = ai.Room;
            Player from = effect.From, to = effect.To;
            WrappedCard trick = effect.Card;
            if (positive && !keep)
            {
                if (ai.IsFriend(to) && ai.HasSkill("gangzhi", to) && !to.Chained)
                    result.Null = true;
            }

            return result;
        }
    }
}
