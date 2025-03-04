using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.GenericArtifacts.HarmonyPatches
{
    [HarmonyPatch]
    public static class PatchArtifactFilter
    {
        [HarmonyPatch(typeof(ArtifactReward), nameof(ArtifactReward.GetBlockedArtifacts)), HarmonyPostfix]
        public static void AddBlockedArtifactsFromSettings(State s, ref HashSet<Type> __result)
        {
            foreach (var item in PMod.Registered_Artifact_Types)
            {
                if (PMod.Instance.Settings.ProfileBased.Current.BlacklistedArtifacts.Contains(item.Name ))
                {
                    __result.Add(item);
                }
            }
        }
    }
}
