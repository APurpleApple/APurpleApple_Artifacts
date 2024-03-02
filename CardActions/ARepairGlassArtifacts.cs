using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APurpleApple.GenericArtifacts.VFXs;
namespace APurpleApple.GenericArtifacts.CardActions
{
    internal class ARepairGlassArtifacts : CardAction
    {
        public override void Begin(G g, State s, Combat c)
        {
            foreach(ArtifactGlass artifact in s.EnumerateAllArtifacts().Where((a)=> a is ArtifactGlass))
            {
                artifact.isBroken = false;
            }
        }
    }
}
