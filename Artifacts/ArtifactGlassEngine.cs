﻿using APurpleApple;
using APurpleApple.GenericArtifacts;
using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.GenericArtifacts
{
    internal class ArtifactGlassEngine : ArtifactGlass, IModArtifact
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
                Sprite = PMod.sprites["ArtifactGlassEngine"].Sprite,
                Name = PMod.Instance.AnyLocalizations.Bind(["artifact", "GlassEngine", "name"]).Localize,
                Description = PMod.Instance.AnyLocalizations.Bind(["artifact", "GlassEngine", "description"]).Localize
            });
        }

        public override void OnTurnStart(State state, Combat combat)
        {
            if (!isBroken)
            {
                combat.Queue(new AStatus() { targetPlayer = true, status = Status.evade, statusAmount = 1, artifactPulse = this.Key() });
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
                return PMod.sprites["ArtifactGlassEngineOff"].Sprite;
            }
            else
            {
                return PMod.sprites["ArtifactGlassEngine"].Sprite;
            }
        }

        public override List<Tooltip>? GetExtraTooltips()
        {
            List<Tooltip>? tooltips = new List<Tooltip>();

            tooltips.Add(new TTGlossary("status.evade",1));

            return tooltips;
        }
    }
}
