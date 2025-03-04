using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.GenericArtifacts
{
    internal sealed class ProfileSettings
    {
        [JsonProperty]
        public HashSet<string> BlacklistedArtifacts = [];
    }

    internal static class ArtifactBlacklist
    {
        public static IModSettingsApi.IModSetting MakeSettings(IModSettingsApi api)
        => new ArtifactsModSetting
        {
            Title = () => "Offered Artifacts",
            AllArtifacts = () => PMod.Registered_Artifact_Types.ToList(),
            IsEnabled = artType => !PMod.Instance.Settings.ProfileBased.Current.BlacklistedArtifacts.Contains(artType.Name),
            SetEnabled = (route, artType, value) =>
            {
                var oldValue = !PMod.Instance.Settings.ProfileBased.Current.BlacklistedArtifacts.Contains(artType.Name);

                if (value)
                    PMod.Instance.Settings.ProfileBased.Current.BlacklistedArtifacts.Remove(artType.Name);
                else
                    PMod.Instance.Settings.ProfileBased.Current.BlacklistedArtifacts.Add(artType.Name);
            }
        };

    }
}
