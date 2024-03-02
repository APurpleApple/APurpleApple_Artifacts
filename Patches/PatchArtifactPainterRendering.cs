using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HarmonyLib.Code;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace APurpleApple.GenericArtifacts.HarmonyPatches
{
    [HarmonyPatch]
    internal class PatchArtifactPainterRendering
    {
        public static readonly string[] stains = { "CardOverPaintStainA", "CardOverPaintStainB", "CardOverPaintStainC" };
        [HarmonyPatch(typeof(Artifact)), HarmonyPatch("Render"), HarmonyPostfix]
        public static void RenderTicks(G g, Vec restingPosition, bool showAsUnknown, bool autoFocus, bool showCount, Artifact __instance)
        {
            if (__instance is ArtifactPainter artifactPainter)
            {
                Rect rect = new Rect(0.0, 0.0, 11.0, 11.0) + restingPosition;
                Vec vec = g.Peek(rect).xy;
                int i = 0;
                foreach (Deck item in artifactPainter.colors)
                {
                    Draw.Sprite((Spr)(PMod.sprites["ArtifactPainterTick"].Sprite), vec.x + 6 + i * 2, vec.y + 5, color: artifactPainter.DeckToColor(item));
                    i++;
                }
            }
        }

        [HarmonyPatch(typeof(Card)), HarmonyPatch("GetActionsOverridden"), HarmonyPostfix]
        public static void AddPaintedActions(ref List<CardAction> __result, Card __instance, State s)
        {
            if (__instance.GetMeta().deck == Deck.colorless)
            {
                foreach (Artifact artifact in s.artifacts)
                {
                    if (artifact is ArtifactPainter artifactPainter)
                    {
                        if (artifactPainter.colors.Count > 0)
                        {
                            foreach (Deck deck in artifactPainter.colors)
                            {
                                __result.Add(artifactPainter.GetColorAction(s, deck));
                            }
                        }
                        break;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Card)), HarmonyPatch("Render"), HarmonyPostfix]
        public static void RenderPaint(G g, Vec? posOverride, State? fakeState, double? overrideWidth, Card __instance)
        {
            if (__instance.GetMeta().deck == Deck.colorless)
            {
                State state = fakeState ?? g.state;

                foreach (Artifact artifact in state.artifacts)
                {
                    if (artifact is ArtifactPainter artifactPainter)
                    {
                        if (artifactPainter.colors.Count > 0)
                        {
                            Vec vec = posOverride ?? __instance.pos;
                            Rect rect = (__instance.GetScreenRect() + vec + new Vec(0.0, __instance.hoverAnim * -2.0 + Mutil.Parabola(__instance.flipAnim) * -10.0 + Mutil.Parabola(Math.Abs(__instance.flopAnim)) * -10.0 * (double)Math.Sign(__instance.flopAnim))).round();
                            Rect value = rect;
                            if (overrideWidth.HasValue)
                            {
                                rect.w = overrideWidth.Value;
                            }
                            Vec vec2 = g.Peek(rect).xy;

                            double x = vec2.x;
                            double y = vec2.y;
                            int i = 0;

                            foreach (Deck deck in artifactPainter.colors)
                            {
                                Spr? id = PMod.sprites[stains[i]].Sprite;

                                    Draw.Sprite(id, x, y, flipX: false, flipY: false, 0.0, color: DB.decks[deck].color, blend: PMod.multiplyBlend);
                                i++;
                            }
                        }
                        break;
                    }
                }
            }
        }
    }
}
