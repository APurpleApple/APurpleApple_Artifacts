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
    internal class ArtifactSpikeBumper : Artifact, IModArtifact
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
                Sprite = PMod.sprites["ArtifactBumper"].Sprite,
                Name = PMod.Instance.AnyLocalizations.Bind(["artifact", "Bumpers", "name"]).Localize,
                Description = PMod.Instance.AnyLocalizations.Bind(["artifact", "Bumpers", "description"]).Localize
            });
        }

        public override void OnReceiveArtifact(State state)
        {
            state.GetCurrentQueue().QueueImmediate(new AAddCard()
            {
                card = new CardBumpersRamm(),
                showCardTraitTooltips = true,
            });
        }


        public override List<Tooltip>? GetExtraTooltips()
        {
            List<Tooltip> tooltips = new List<Tooltip>();

            tooltips.Add(new TTCard() { card = new CardBumpersRamm() });

            return tooltips;
        }
    }
}
