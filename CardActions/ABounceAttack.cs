using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace APurpleApple.GenericArtifacts.CardActions
{
    internal class ABounceAttack : CardAction
    {
        public int bounceTargetX = 0;
        public int bounceOriginX = 0;
        public int bounceCount = 1;
        public bool targetPlayer = false;

        public int damage;

        public int moveEnemy;

        public bool stunEnemy;

        public bool weaken;

        public bool brittle;

        public bool armorize;

        public bool piercing;

        public Status? status;

        public int statusAmount;

        public int paybackCounter;

        public override void Begin(G g, State s, Combat c)
        {
            Ship target = (targetPlayer ? s.ship : c.otherShip);
            Ship origin = (targetPlayer ? c.otherShip : s.ship);
            if (target == null || origin == null || target.hull <= 0)
            {
                return;
            }

            RaycastResult raycastResult = CombatUtils.RaycastGlobal(c, target, fromDrone: false, bounceTargetX);
            if (raycastResult != null && ApplyAutododge(c, target, raycastResult))
            {
                return;
            }

            if (raycastResult == null)
            {
                return;
            }

            if (raycastResult.hitDrone)
            {
                bool flag2 = c.stuff[raycastResult.worldX].Invincible();
                foreach (Artifact item5 in s.EnumerateAllArtifacts())
                {
                    if (item5.ModifyDroneInvincibility(s, c, c.stuff[raycastResult.worldX]) == true)
                    {
                        flag2 = true;
                        item5.Pulse();
                    }
                }

                if (c.stuff[raycastResult.worldX].bubbleShield && !piercing)
                {
                    c.stuff[raycastResult.worldX].bubbleShield = false;
                }
                else if (flag2)
                {
                    c.QueueImmediate(c.stuff[raycastResult.worldX].GetActionsOnShotWhileInvincible(s, c, !targetPlayer, damage));
                }
                else
                {
                    c.QueueImmediate(c.stuff[raycastResult.worldX].GetActionsOnDestroyed(s, c, !targetPlayer, raycastResult.worldX));
                    if (c.stuff.TryGetValue(raycastResult.worldX, out StuffBase? value))
                    {
                        value.DoDestroyedEffect(s, c);
                    }

                    c.stuff.Remove(raycastResult.worldX);
                    if (!targetPlayer)
                    {
                        foreach (Artifact item6 in s.EnumerateAllArtifacts())
                        {
                            item6.OnPlayerDestroyDrone(s, c);
                        }
                    }
                }
            }

            timer = 0.2;
            DamageDone dmg = new DamageDone();
            if (raycastResult.hitShip)
            {
                dmg = target.NormalDamage(s, c, damage, raycastResult.worldX, piercing);
                Part? partAtWorldX = target.GetPartAtWorldX(raycastResult.worldX);

                if (partAtWorldX != null && partAtWorldX.GetDamageModifier() == PDamMod.armor)
                {
                    ABounceAttack attack = new ABounceAttack() {
                        bounceCount = bounceCount + 1,
                        bounceOriginX = bounceTargetX,
                        bounceTargetX = bounceTargetX + (int)Math.Round(s.rngActions.Next() * 2 * bounceCount - bounceCount),
                        damage = damage,
                        targetPlayer = !targetPlayer,
                        stunEnemy = stunEnemy,
                        status = status,
                        statusAmount = statusAmount,
                        piercing = piercing,
                        armorize = armorize,
                        brittle = brittle,
                        weaken = weaken,
                        moveEnemy = moveEnemy,
                        paybackCounter = paybackCounter,
                    };
                    c.QueueImmediate(attack);
                }

                if (partAtWorldX != null && partAtWorldX.stunModifier == PStunMod.stunnable)
                {
                    stunEnemy = true;
                }

                if ((target.Get(Status.payback) > 0 || target.Get(Status.tempPayback) > 0) && paybackCounter < 100)
                {
                    c.QueueImmediate(new AAttack
                    {
                        paybackCounter = paybackCounter + 1,
                        damage = Card.GetActualDamage(s, target.Get(Status.payback) + target.Get(Status.tempPayback), !targetPlayer),
                        targetPlayer = !targetPlayer,
                        fast = true,
                        storyFromPayback = true
                    });
                }

                if (moveEnemy != 0)
                {
                    c.QueueImmediate(new AMove
                    {
                        dir = moveEnemy,
                        targetPlayer = targetPlayer
                    });
                }

                if (status.HasValue)
                {
                    c.QueueImmediate(new AStatus
                    {
                        status = status.Value,
                        statusAmount = statusAmount,
                        targetPlayer = targetPlayer
                    });
                }

                if (stunEnemy)
                {
                    c.QueueImmediate(new AStunPart
                    {
                        worldX = raycastResult.worldX
                    });
                }

                if (weaken)
                {
                    c.QueueImmediate(new AWeaken
                    {
                        worldX = raycastResult.worldX
                    });
                }

                if (brittle)
                {
                    c.QueueImmediate(new ABrittle
                    {
                        worldX = raycastResult.worldX
                    });
                }

                if (target.Get(Status.reflexiveCoating) >= 1)
                {
                    c.QueueImmediate(new AArmor
                    {
                        worldX = raycastResult.worldX,
                        targetPlayer = targetPlayer,
                        justTheActiveOverride = true
                    });
                }

                if (armorize)
                {
                    c.QueueImmediate(new AArmor
                    {
                        worldX = raycastResult.worldX,
                        targetPlayer = targetPlayer
                    });
                }
            }

            if (!targetPlayer)
            {
                Input.Rumble(0.5);
            }

            if (targetPlayer)
            {
                if (!raycastResult.hitShip && !raycastResult.hitDrone)
                {
                    g.state.storyVars.enemyShotJustMissed = true;
                }

                if (raycastResult.hitShip)
                {
                    g.state.storyVars.enemyShotJustHit = true;
                }

                if (!raycastResult.hitShip && !raycastResult.hitDrone)
                {
                    foreach (Artifact item8 in s.EnumerateAllArtifacts())
                    {
                        item8.OnPlayerDodgeHit(s, c);
                    }
                }
            }
            else
            {
                if (raycastResult.hitDrone)
                {
                    g.state.storyVars.playerJustShotAMidrowObject = true;
                }

                if (!raycastResult.hitShip && !raycastResult.hitDrone)
                {
                    g.state.storyVars.playerShotJustMissed = true;
                }
                else
                {
                    g.state.storyVars.playerShotJustMissed = false;
                }

                if (raycastResult.hitShip)
                {
                    g.state.storyVars.playerShotJustHit = true;
                }

                if (raycastResult.hitShip)
                {
                    foreach (Artifact item10 in s.EnumerateAllArtifacts())
                    {
                        item10.OnEnemyGetHit(s, c, c.otherShip.GetPartAtWorldX(raycastResult.worldX));
                    }
                }

                if (!raycastResult.hitShip && !raycastResult.hitDrone)
                {
                    foreach (Artifact item11 in s.EnumerateAllArtifacts())
                    {
                        item11.OnEnemyDodgePlayerAttack(s, c);
                    }
                }

                if (!raycastResult.hitShip && !raycastResult.hitDrone)
                {
                    bool flag3 = false;
                    for (int i = -1; i <= 1; i += 2)
                    {
                        if (CombatUtils.RaycastGlobal(c, target, fromDrone: true, raycastResult.worldX + i).hitShip)
                        {
                            flag3 = true;
                        }
                    }

                    if (flag3)
                    {
                        foreach (Artifact item12 in s.EnumerateAllArtifacts())
                        {
                            item12.OnEnemyDodgePlayerAttackByOneTile(s, c);
                        }
                    }
                }
            }

            EffectSpawnerExtension.CannonAngled(g, targetPlayer, bounceOriginX, raycastResult, dmg);
        }

        private bool ApplyAutododge(Combat c, Ship target, RaycastResult ray)
        {
            if (ray.hitShip)
            {
                if (target.Get(Status.autododgeRight) > 0)
                {
                    target.Add(Status.autododgeRight, -1);
                    int dir = ray.worldX - target.x + 1;
                    c.QueueImmediate(new List<CardAction>
                {
                    new AMove
                    {
                        targetPlayer = targetPlayer,
                        dir = dir
                    },
                    this
                });
                    timer = 0.0;
                    return true;
                }

                if (target.Get(Status.autododgeLeft) > 0)
                {
                    target.Add(Status.autododgeLeft, -1);
                    int dir2 = ray.worldX - target.x - target.parts.Count;
                    c.QueueImmediate(new List<CardAction>
                {
                    new AMove
                    {
                        targetPlayer = targetPlayer,
                        dir = dir2
                    },
                    this
                });
                    timer = 0.0;
                    return true;
                }
            }

            return false;
        }
    }
}
