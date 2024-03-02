using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using APurpleApple.GenericArtifacts.VFXs;
namespace APurpleApple.GenericArtifacts.HarmonyPatches
{
    [HarmonyPatch()]
    public static class PatchRammingAction
    {
        [HarmonyPrefix(), HarmonyPatch(typeof(Ship), nameof(Ship.DrawTopLayer))]
        public static void DrawShipOverPrefix(G g, ref Vec worldPos, Ship __instance)
        {
            if (__instance.isPlayerShip)
            {
                if (g.state.route is Combat c)
                {
                    foreach(FX fx in c.fx)
                    {
                        if (fx is ShipRamm shipRamm)
                        {
                            if (shipRamm.age < .2)
                            {
                                double percent = Ease.InElastic(shipRamm.age) / .2;
                                worldPos.y -= 70 * percent;
                            }
                            else if (shipRamm.age < .6)
                            {
                                double percent = Ease.OutSin(1 - ((shipRamm.age - .2) / .4));
                                worldPos.y -= 70 * percent;
                            }
                            break;
                        }
                    }
                    /*
                    if (c.cardActions.Count > 0 && c.cardActions[0] is ARamAttack ramAttack)
                    {
                        double duration = 0.2;
                        double percent = Ease.InElastic(g.state.time - ramAttack.startTime) / duration;
                        v.y -= 70 * percent;
                    }
                    if (c.cardActions.Count > 0 && c.cardActions[0] is ARamAftermath ramAftermath)
                    {
                        double duration = 0.4;
                        double percent = Ease.OutSin(1 - ((g.state.time - ramAftermath.startTime) / duration));
                        v.y -= 70 * percent;
                    }*/
                }
            }
        }

        [HarmonyPrefix(), HarmonyPatch(typeof(Ship), nameof(Ship.DrawBottomLayer))]
        public static void DrawShipUnderPrefix(G g, ref Vec worldPos, Ship __instance)
        {
            if (__instance.isPlayerShip)
            {
                if (g.state.route is Combat c)
                {
                    foreach (FX fx in c.fx)
                    {
                        if (fx is ShipRamm shipRamm)
                        {
                            if (shipRamm.age < .2)
                            {
                                double percent = Ease.InElastic(shipRamm.age) / .2;
                                worldPos.y -= 70 * percent;
                            }
                            else if (shipRamm.age < .6)
                            {
                                double percent = Ease.OutSin(1 - ((shipRamm.age - .2) / .4));
                                worldPos.y -= 70 * percent;
                            }
                            break;
                        }
                    }
                    /*
                    if (c.cardActions.Count > 0 && c.cardActions[0] is ARamAttack ramAttack)
                    {
                        double duration = 0.2;
                        double percent = Ease.InElastic(g.state.time - ramAttack.startTime) / duration;
                        v.y -= 70 * percent;
                    }
                    if (c.cardActions.Count > 0 && c.cardActions[0] is ARamAftermath ramAftermath)
                    {
                        double duration = 0.4;
                        double percent = Ease.OutSin(1 - ((g.state.time - ramAftermath.startTime) / duration));
                        v.y -= 70 * percent;
                    }*/
                }
            }
        }

        //[HarmonyPostfix(), HarmonyPatch(typeof(Combat), nameof(Combat.Update))]
        public static void DebugPostfix(Combat __instance)
        {
            if (__instance.cardActions.Count != lastCount)
            {
                lastCount = __instance.cardActions.Count;
                lastType = null;
                Console.WriteLine(__instance.cardActions.Count);
            }

            if (__instance.cardActions.Count > 0)
            {
                if (__instance.cardActions[0].GetType() != lastType)
                {
                    lastType = __instance.cardActions[0].GetType();
                    Console.WriteLine(__instance.cardActions[0].GetType().Name);
                }
            }
        }


        public static int lastCount = 0;
        public static Type? lastType;
    }
}
