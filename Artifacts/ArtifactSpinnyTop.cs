using APurpleApple;
using APurpleApple.GenericArtifacts.CardActions;
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
    internal class ArtifactSpinnyTop : Artifact, IModArtifact
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
                Sprite = PMod.sprites["ArtifactSpinnyTop"].Sprite,
                Name = PMod.Instance.AnyLocalizations.Bind(["artifact", "SpinnyTop", "name"]).Localize,
                Description = PMod.Instance.AnyLocalizations.Bind(["artifact", "SpinnyTop", "description"]).Localize
            });
        }

        public override void OnReceiveArtifact(State state)
        {
            state.ship.baseEnergy++;
        }

        public override void OnPlayerPlayCard(int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition, int handCount)
        {
            if (card.GetDataWithOverrides(state).temporary) return;
            if (!state.characters.Any(c => c.deckType == card.GetMeta().deck)) return;

            state.RemoveCardFromWhereverItIs(card.uuid);
            Card newCard = DB.releasedCards.Where(delegate (Card c)
            {
                CardMeta meta = c.GetMeta();

                if (card.GetMeta().deck != meta.deck)
                {
                    return false;
                }

                if (meta.dontOffer)
                {
                    return false;
                }

                if (meta.unreleased)
                {
                    return false;
                }

                return true;
            }).Shuffle(state.rngActions).First().CopyWithNewId();

            newCard.upgrade = card.upgrade;
            combat.Queue(new ARemoveCardFromDeck() { uuid = card.uuid});
            combat.Queue(new AAddCard() { card = newCard, artifactPulse = this.Key() , destination = CardDestination.Hand});
        }

    }
}
