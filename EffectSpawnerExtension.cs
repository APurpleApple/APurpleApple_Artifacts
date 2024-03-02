using FMOD;
using FSPRO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APurpleApple.GenericArtifacts.VFXs;

namespace APurpleApple.GenericArtifacts
{
    internal class EffectSpawnerExtension
    {
        public static void CannonAngled(G g, bool targetPlayer, int originX, RaycastResult ray, DamageDone dmg)
        {
            if (!(g.state.route is Combat combat))
            {
                return;
            }

            Vec start = FxPositions.Cannon(originX, !targetPlayer);
            Vec end = (!ray.hitDrone && !ray.hitShip) ? FxPositions.Miss(ray.worldX, targetPlayer) : (ray.hitDrone ? FxPositions.Drone(ray.worldX) : (dmg.hitHull ? FxPositions.Hull(ray.worldX, targetPlayer) : FxPositions.Shield(ray.worldX, targetPlayer)));
            combat.fx.Add(new CannonBeamAngled
            {
                start = start,
                end = end,
                w = 1
            });
            
            GUID? gUID = null;
            if (ray.hitShip)
            {
                ParticleBursts.HullImpact(g, end, targetPlayer, !ray.hitDrone, ray.fromDrone);
            }

            if (dmg.hitShield && !dmg.hitHull)
            {
                combat.fx.Add(new ShieldHit
                {
                    pos = FxPositions.Shield(ray.worldX, targetPlayer)
                });
                ParticleBursts.ShieldImpact(g, FxPositions.Shield(ray.worldX, targetPlayer), targetPlayer);
            }

            if (dmg.poppedShield)
            {
                combat.fx.Add(new ShieldPop
                {
                    pos = FxPositions.Shield(ray.worldX, targetPlayer)
                });
            }

            if (dmg.poppedShield)
            {
                gUID = Event.Hits_ShieldPop;
            }
            else if (dmg.hitShield)
            {
                gUID = Event.Hits_ShieldHit;
            }

            if (!ray.hitDrone && !ray.hitShip)
            {
                gUID = Event.Hits_Miss;
            }
            else if (dmg.hitHull)
            {
                gUID = ((!targetPlayer) ? new GUID?(Event.Hits_OutgoingHit) : new GUID?(Event.Hits_HitHurt));
            }
            else if (ray.hitDrone)
            {
                gUID = Event.Hits_HitDrone;
            }

            if (gUID.HasValue)
            {
                Audio.Play(gUID.Value);
            }
        }
    }
}
