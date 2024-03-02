using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.GenericArtifacts.VFXs
{
    public class CannonBeamAngled : FX
    {
        public Vec start;
        public Vec end;
        public double w;

        public static Color cannonBeam = new Color("ff8866");

        public static Color cannonBeamCore = new Color("ffffff");

        public override void Render(G g, Vec v)
        {
            Rect r = Rect.FromPoints(start, end);
            double num = 0.1;
            if (age < num)
            {
                double num2 = 2.0 * (1.0 - age / num);

                Draw.Line(v.x + start.x, v.y + start.y, v.x + end.x, v.y + end.y, w + 2.0 * (num2 + 1.0), cannonBeam, BlendMode.Screen);
                Draw.Line(v.x + start.x, v.y + start.y, v.x + end.x, v.y + end.y, w + num2 * 2.0, cannonBeamCore);
            }
        }
    }
}
