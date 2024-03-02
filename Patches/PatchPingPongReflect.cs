using APurpleApple.GenericArtifacts.CardActions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace APurpleApple.GenericArtifacts.Patches
{
    [HarmonyPatch]
    internal static class PatchPingPongReflect
    {
        [HarmonyPatch(typeof(AAttack), nameof(AAttack.Begin)), HarmonyPostfix]
        public static void AfterAttackTryBouncePatch(AAttack __instance, bool __runOriginal, State s, Combat c, G g)
        {
            if (__runOriginal)
            {
                if (!s.artifacts.Any(a => a is ArtifactPingPong)) { return; }

                Ship ship = (__instance.targetPlayer ? s.ship : c.otherShip);
                Ship ship2 = (__instance.targetPlayer ? c.otherShip : s.ship);
                if (ship == null || ship2 == null || ship.hull <= 0 || (__instance.fromDroneX.HasValue && !c.stuff.ContainsKey(__instance.fromDroneX.Value)))
                {
                    return;
                }

                int? num = GetFromX(__instance, s, c);
                RaycastResult? raycastResult = __instance.fromDroneX.HasValue ? CombatUtils.RaycastGlobal(c, ship, fromDrone: true, __instance.fromDroneX.Value) : (num.HasValue ? CombatUtils.RaycastFromShipLocal(s, c, num.Value, __instance.targetPlayer) : null);

                if (raycastResult == null) { return; }
                if (!raycastResult.hitShip) { return; }

                Part? partAtWorldX = ship.GetPartAtWorldX(raycastResult.worldX);
                if (partAtWorldX == null) { return; }

                if (partAtWorldX.GetDamageModifier() != PDamMod.armor) { return; }

                ABounceAttack attack = new ABounceAttack()
                {
                    bounceOriginX = raycastResult.worldX,
                    bounceTargetX = raycastResult.worldX + (int)Math.Round(s.rngActions.Next() * 2 - 1),
                    damage = __instance.damage,
                    targetPlayer = !__instance.targetPlayer,
                    stunEnemy = __instance.stunEnemy,
                    status = __instance.status,
                    statusAmount = __instance.statusAmount,
                    piercing = __instance.piercing,
                    armorize = __instance.armorize,
                    brittle = __instance.brittle,
                    weaken = __instance.weaken,
                    moveEnemy = __instance.moveEnemy,
                    paybackCounter = __instance.paybackCounter,
                };
                c.QueueImmediate(attack);
            }
        }

        private static int? GetFromX(AAttack instance, State s, Combat c)
        {
            if (instance.fromX.HasValue)
            {
                return instance.fromX;
            }

            int num = (instance.targetPlayer ? c.otherShip : s.ship).parts.FindIndex((Part p) => p.type == PType.cannon && p.active);
            if (num != -1)
            {
                return num;
            }

            return null;
        }
    }
}
