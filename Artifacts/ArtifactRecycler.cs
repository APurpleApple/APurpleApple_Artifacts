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
    internal class ArtifactRecycler : Artifact, IModArtifact
    {
        public bool enabled = true;
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
                Sprite = PMod.sprites["ArtifactRecycler"].Sprite,
                Name = PMod.Instance.AnyLocalizations.Bind(["artifact", "Recycler", "name"]).Localize,
                Description = PMod.Instance.AnyLocalizations.Bind(["artifact", "Recycler", "description"]).Localize
            });
        }

        public override void OnPlayerPlayCard(int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition, int handCount)
        {

            if (enabled)
            {
                combat.Queue(new ARecycleCard() { card = card });
                enabled = false;
            }
        }

        public override void OnTurnStart(State state, Combat combat)
        {
            enabled = true;
        }

        public override Spr GetSprite()
        {
            return this.enabled ? PMod.sprites["ArtifactRecycler"].Sprite : PMod.sprites["ArtifactRecyclerOff"].Sprite;
        }
    }
}
