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
using APurpleApple.GenericArtifacts.CardActions;

namespace APurpleApple.GenericArtifacts
{
    

    internal class ArtifactPrinter : Artifact, IModArtifact
    {
        public bool isEnabled = true;
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

        public override void OnPlayerPlayCard(int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition, int handCount)
        {
            if (isEnabled)
            {
                combat.Queue(new ACopyCardTo() { destination = CardDestination.Exhaust, artifactPulse = Key(), selectedCard = card } );
                isEnabled = false;
            }
        }

        public override Spr GetSprite()
        {
            if (!isEnabled)
            {
                return PMod.sprites["ArtifactPrinterOff"].Sprite;
            }
            else
            {
                return PMod.sprites["ArtifactPrinter"].Sprite;
            }
        }

        public override void OnCombatEnd(State state)
        {
            isEnabled = true;
        }
    }
}
