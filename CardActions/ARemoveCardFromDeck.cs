using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.GenericArtifacts.CardActions
{
    internal class ARemoveCardFromDeck : CardAction
    {
        public int uuid;
        public override void Begin(G g, State s, Combat c)
        {
            timer = 0;
            s.RemoveCardFromWhereverItIs(uuid);
        }
    }
}
