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
    internal class ArtifactPrism : Artifact, IModArtifact
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
                    pools = [ArtifactPool.Boss],
                    unremovable = false,
                },
                Sprite = PMod.sprites["ArtifactPrism"].Sprite,
                Name = PMod.Instance.AnyLocalizations.Bind(["artifact", "Prism", "name"]).Localize,
                Description = PMod.Instance.AnyLocalizations.Bind(["artifact", "Prism", "description"]).Localize
            });
        }

    }
}
