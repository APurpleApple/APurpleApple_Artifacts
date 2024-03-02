using APurpleApple;
using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.GenericArtifacts
{
    [ArtifactMeta(owner = Deck.colorless, pools = new ArtifactPool[] { ArtifactPool.Common }, unremovable = false)]
    internal class ArtifactGlassShield : ArtifactGlass, IModArtifact
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
                Sprite = PMod.sprites["ArtifactGlassShield"].Sprite,
                Name = PMod.Instance.AnyLocalizations.Bind(["artifact", "GlassShield", "name"]).Localize,
                Description = PMod.Instance.AnyLocalizations.Bind(["artifact", "GlassShield", "description"]).Localize
            });
        }

        public override void OnTurnStart(State state, Combat combat)
        {
            if (!isBroken)
            {
                combat.Queue(new AStatus() { targetPlayer = true, status = Status.tempShield, statusAmount = 1, artifactPulse = this.Key() });
            }
        }

        public override void OnPlayerLoseHull(State state, Combat combat, int amount)
        {
            if (!isBroken)
            {
                Pulse();
                isBroken = true;
            }
        }

        public override Spr GetSprite()
        {
            if (isBroken)
            {
                return PMod.sprites["ArtifactGlassShieldOff"].Sprite;
            }
            else
            {
                return PMod.sprites["ArtifactGlassShield"].Sprite;
            }
        }

        public override List<Tooltip>? GetExtraTooltips()
        {
            List<Tooltip>? tooltips = new List<Tooltip>();

            tooltips.Add(new TTGlossary("status.tempShieldAlt"));

            return tooltips;
        }
    }
}
