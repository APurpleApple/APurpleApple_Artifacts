using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.GenericArtifacts
{
    public sealed class ApiImplementation : IAppleArtifactApi
    {
        public void SetPaletteAction(Deck deck, IAppleArtifactApi.SingleActionProvider action, Tooltip description)
        {
            ArtifactPainter.deckToActions.Add(deck, action);
            ArtifactPainter.pigmentTooltips.Add(deck, description);
        }
    }
}
