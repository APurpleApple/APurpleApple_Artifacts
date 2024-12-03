using Newtonsoft.Json;
using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.GenericArtifacts;
internal sealed class Settings
{
    [JsonProperty]
    public ProfileSettings Global = new();

    [JsonIgnore]
    public ProfileBasedValue<IModSettingsApi.ProfileMode, ProfileSettings> ProfileBased;

    public Settings()
    {
        this.ProfileBased = ProfileBasedValue.Create(
            () => PMod.Instance.Helper.ModData.GetModDataOrDefault(MG.inst.g?.state ?? DB.fakeState, "ActiveProfile", IModSettingsApi.ProfileMode.Slot),
            profile => PMod.Instance.Helper.ModData.SetModData(MG.inst.g?.state ?? DB.fakeState, "ActiveProfile", profile),
            profile => profile switch
            {
                IModSettingsApi.ProfileMode.Global => this.Global,
                IModSettingsApi.ProfileMode.Slot => PMod.Instance.Helper.ModData.ObtainModData<ProfileSettings>(MG.inst.g?.state ?? DB.fakeState, "ProfileSettings"),
                _ => throw new ArgumentOutOfRangeException(nameof(profile), profile, null)
            },
            (profile, data) =>
            {
                switch (profile)
                {
                    case IModSettingsApi.ProfileMode.Global:
                        this.Global = data;
                        break;
                    case IModSettingsApi.ProfileMode.Slot:
                        PMod.Instance.Helper.ModData.SetModData(MG.inst.g?.state ?? DB.fakeState, "ProfileSettings", data);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        );
    }
}