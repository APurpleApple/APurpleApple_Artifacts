using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APurpleApple.GenericArtifacts.VFXs;
namespace APurpleApple.GenericArtifacts.CardActions
{
    internal class ARecycleCard: CardAction
    {
        public Card? card;
        public override void Begin(G g, State s, Combat c)
        {
            if (card == null) return;

            s.RemoveCardFromWhereverItIs(card.uuid);
            s.SendCardToDeck(card, true);
        }
    }
}
