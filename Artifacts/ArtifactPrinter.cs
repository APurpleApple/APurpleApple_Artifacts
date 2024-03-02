using APurpleApple;
using APurpleApple.GenericArtifacts;
using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using APurpleApple.GenericArtifacts.Cards;

namespace APurpleApple.GenericArtifacts
{
    

    internal class ArtifactPrinter : Artifact, IModArtifact
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
                Sprite = PMod.sprites["ArtifactPrinter"].Sprite,
                Name = PMod.Instance.AnyLocalizations.Bind(["artifact", "Printer", "name"]).Localize,
                Description = PMod.Instance.AnyLocalizations.Bind(["artifact", "Printer", "description"]).Localize
            });
        }

        public override void OnCombatStart(State state, Combat combat)
        {
            combat.Queue(new AAddCard() { card = new CardPrinter(), amount = 1, artifactPulse = this.Key(), destination = CardDestination.Hand });
        }

        public override List<Tooltip>? GetExtraTooltips()
        {
            List<Tooltip> list = new List<Tooltip>();
            list.Add(new TTCard() { card = new CardPrinter() });
            return list;
        }
    }
}
