using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.GenericArtifacts.CardActions
{
    internal class ACopyCardTo : CardAction
    {
        public CardDestination destination = CardDestination.Hand;
        public override void Begin(G g, State s, Combat c)
        {
            Card? selectedCard = this.selectedCard;
            if (selectedCard == null)
                return;
            Card card = selectedCard.CopyWithNewId();
            c.QueueImmediate((CardAction)new AAddCard()
            {
                card = card,
                destination = this.destination,
                amount = 1
            });
        }

        public override List<Tooltip> GetTooltips(State s)
        {
            List<Tooltip> list = new List<Tooltip>();
            switch (destination)
            {
                case CardDestination.Discard:
                    list.Add(new TTGlossary("cardtrait.discard"));
                    break;
                case CardDestination.Exhaust:
                    list.Add(new TTGlossary("cardtrait.exhaust"));
                    break;
            }
            return list;
        }
    }
}
