using APurpleApple;
using FMOD;
using Microsoft.Build.Framework;
using Microsoft.Xna.Framework;
using Nickel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.GenericArtifacts
{
    internal class ArtifactPingPong : Artifact, IModArtifact
    {
        public static void Register(IModHelper helper)
        {
            Type type = MethodBase.GetCurrentMethod()!.DeclaringType!;
            helper.Content.Artifacts.RegisterArtifact(type.Name, new()
            {
                ArtifactType = type,
                Meta = new()
                {
                    owner = Deck.colorless,
                    pools = [ArtifactPool.Common],
                    unremovable = false,
                },
                Sprite = PMod.sprites["ArtifactPingPong"].Sprite,
                Name = PMod.Instance.AnyLocalizations.Bind(["artifact", "PingPong", "name"]).Localize,
                Description = PMod.Instance.AnyLocalizations.Bind(["artifact", "PingPong", "description"]).Localize
            });
        }

        public override void OnReceiveArtifact(State state)
        {
            foreach (Part part in state.ship.parts)
            {
                if (part.type == PType.cannon)
                {
                    part.damageModifier = PDamMod.armor;
                    part.damageModifierOverrideWhileActive = PDamMod.armor;
                }
            }
        }

        public override List<Tooltip>? GetExtraTooltips()
        {
            List<Tooltip> list = new List<Tooltip>();
            list.Add(new TTGlossary("parttrait.armor"));
            return list;
        }
    }
}
