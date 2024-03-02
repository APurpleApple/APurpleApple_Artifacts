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

        [HarmonyPatch(typeof(MG), nameof(MG.DrawLoadingScreen)), HarmonyPrefix]
        private static void DrawLoadingScreen_Prefix(MG __instance, ref int __state)
        => __state = __instance.loadingQueue?.Count ?? 0;

        [HarmonyPatch(typeof(MG), nameof(MG.DrawLoadingScreen)), HarmonyPostfix]
        private static void DrawLoadingScreen_Postfix(MG __instance, ref int __state)
        {
            if (__state <= 0)
                return;
            if ((__instance.loadingQueue?.Count ?? 0) > 0)
                return;
            
            DB.story.all["ShopRepairGlassArtifacts_A"] = new StoryNode()
            {
                lookup = new HashSet<string>() { "shopRepairGlassArtifacts" },
                type = NodeType.@event,
                bg = "BGShop",
                lines = new List<Instruction>()
                {
                    new CustomSay()
                    {
                        flipped = true,
                        who = "nerd",
                        Text = PMod.Instance.Localizations.Localize(["event", "ShopRepairGlass", "A"]),
                    },
                }
            };

            DB.story.all["ShopRepairGlassArtifacts_B"] = new StoryNode()
            {
                lookup = new HashSet<string>() { "shopRepairGlassArtifacts" },
                type = NodeType.@event,
                bg = "BGShop",
                lines = new List<Instruction>()
                {
                    new CustomSay()
                    {
                        flipped = true,
                        who = "nerd",
                        Text = PMod.Instance.Localizations.Localize(["event", "ShopRepairGlass", "B"]),
                    },
                }
            };

            DB.story.all["ShopRepairGlassArtifacts_C"] = new StoryNode()
            {
                lookup = new HashSet<string>() { "shopRepairGlassArtifacts" },
                type = NodeType.@event,
                bg = "BGShop",
                lines = new List<Instruction>()
                {
                    new CustomSay()
                    {
                        flipped = true,
                        who = "nerd",
                        Text = PMod.Instance.Localizations.Localize(["event", "ShopRepairGlass", "C"]),
                    },
                }
            };
        }
    }
}
