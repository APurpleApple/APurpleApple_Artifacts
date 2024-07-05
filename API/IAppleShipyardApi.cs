using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.GenericArtifacts;
public interface IAppleArtifactApi
{
    public delegate CardAction SingleActionProvider(State state);
    public void SetPaletteAction(Deck deck, SingleActionProvider action, Tooltip description);
}
