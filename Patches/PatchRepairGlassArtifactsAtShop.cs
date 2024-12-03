using HarmonyLib;
using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APurpleApple.GenericArtifacts.CardActions;

namespace APurpleApple.GenericArtifacts.Patches
{
    [HarmonyPatch]
    public static class PatchRepairGlassArtifactsAtShop
    {
        [HarmonyPatch(typeof(Events), nameof(Events.NewShop)), HarmonyPostfix]
        public static void AddRepairChoice(State s, ref List<Choice> __result)
        {
            if (s.EnumerateAllArtifacts().Any((a) => a is ArtifactGlass g && g.isBroken))
            {
                __result.Insert(0,new Choice() { 
                    label = PMod.Instance.Localizations.Localize(["choice", "Shop", "RepairGlass"]),
                    key = ".shopRepairGlassArtifacts",
                    actions = new List<CardAction>()
                    {
                        new ARepairGlassArtifacts()
                    }
                });
            }
        }
    }
}
