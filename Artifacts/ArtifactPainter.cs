using APurpleApple;
using FMOD;
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
    public class ArtifactPainter : Artifact, IModArtifact
    {
        public static Dictionary<Deck, IAppleArtifactApi.SingleActionProvider> deckToActions = new();
        public static Dictionary<Deck, Tooltip> pigmentTooltips = new();
        int maxColors = 3;

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
                Sprite = PMod.sprites["ArtifactPainter"].Sprite,
                Name = PMod.Instance.AnyLocalizations.Bind(["artifact", "Painter", "name"]).Localize,
                Description = PMod.Instance.AnyLocalizations.Bind(["artifact", "Painter", "description"]).Localize
            });
        }

        public static void AfterDBInit(IModHelper helper)
        {
            deckToActions.Add(Deck.peri, (State state) => new AAttack() { damage = Card.GetActualDamage(state, 0), targetPlayer = false });
            pigmentTooltips.Add(Deck.peri, new TTText(String.Format(PMod.Instance.Localizations.Localize(["glossary", "Pigment", "peri"]), 1)));

            deckToActions.Add(Deck.riggs, (State state) => new AMove() { targetPlayer = true, isRandom = true, dir = 1 });
            pigmentTooltips.Add(Deck.riggs, new TTText(String.Format(PMod.Instance.Localizations.Localize(["glossary", "Pigment", "riggs"]), 1)));

            deckToActions.Add(Deck.dizzy, (State state) => new AStatus() { targetPlayer = true, status = Status.tempShield, statusAmount = 1 });
            pigmentTooltips.Add(Deck.dizzy, new TTText(String.Format(PMod.Instance.Localizations.Localize(["glossary", "Pigment", "dizzy"]), 1)));

            deckToActions.Add(Deck.goat, (State state) => new AStatus() { targetPlayer = true, status = Status.droneShift, statusAmount = 1 });
            pigmentTooltips.Add(Deck.goat, new TTText(String.Format(PMod.Instance.Localizations.Localize(["glossary", "Pigment", "isaac"]), 1)));

            deckToActions.Add(Deck.eunice, (State state) => new AStatus() { targetPlayer = false, status = Status.heat, statusAmount = 1 });
            pigmentTooltips.Add(Deck.eunice, new TTText(String.Format(PMod.Instance.Localizations.Localize(["glossary", "Pigment", "drake"]), 1)));

            deckToActions.Add(Deck.hacker, (State state) => new ADrawCard() { count = 1 });
            pigmentTooltips.Add(Deck.hacker, new TTText(String.Format(PMod.Instance.Localizations.Localize(["glossary", "Pigment", "max"]), 1)));

            deckToActions.Add(Deck.shard, (State state) => new AStatus() { targetPlayer = true, status = Status.shard, statusAmount = 1 });
            pigmentTooltips.Add(Deck.shard, new TTText(String.Format(PMod.Instance.Localizations.Localize(["glossary", "Pigment", "books"]), 1)));
        }

        public List<Deck> colors = new List<Deck>();

        public override void OnPlayerPlayCard(int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition, int handCount)
        {
            Deck color = card.GetMeta().deck;
            int pigmentCount = GetCurrentPigmentCount();
            if (pigmentCount < maxColors && isColorValid(color))
            {
                Pulse();
                AddPigment(color);
            }
            if (pigmentCount > 0 && color == Deck.colorless) 
            {
                Pulse();
                colors.Clear();
            }
        }

        public CardAction GetColorAction(State state, Deck color)
        {
            if (deckToActions.TryGetValue(color, out IAppleArtifactApi.SingleActionProvider? action)){
                if (action != null)
                {
                    return action.Invoke(state);
                }
            }
            return new AStatus() { targetPlayer = true, status = Status.energyFragment, statusAmount = 1 };
        }

        public int GetCurrentPigmentCount()
        {
            return colors.Count;
        }

        public void AddPigment(Deck color)
        {
            colors.Add(color);
        }

        public override List<Tooltip>? GetExtraTooltips()
        {
            List<Tooltip> tooltips = new List<Tooltip>();
            /*tooltips.Add(new CustomTTGlossary(
                CustomTTGlossary.GlossaryType.action,
                () => PMod.sprites["StatusPaint"].Sprite,
                () => PMod.Instance.Localizations.Localize(["glossary", "Pigment", "name"]),
                () => PMod.Instance.Localizations.Localize(["glossary", "Pigment", "description"])
                ));*/

            State s = MG.inst.g.state;

            foreach (Deck? item in s.characters.Select((c) => c.deckType))
            {
                if (pigmentTooltips.TryGetValue(item ?? Deck.colorless, out Tooltip? tooltip))
                {
                    if (tooltip != null)
                    {
                        tooltips.Add(tooltip);
                    }
                }
                else
                {
                    
                }
            }
            tooltips.Add(new TTText(String.Format(PMod.Instance.Localizations.Localize(["glossary", "Pigment", "noone"]), 1)));

            return tooltips;
        }

        public Color DeckToColor(Deck deck)
        {
            return DB.decks[deck].color;
        }

        public bool isColorValid(Deck color)
        {
            return color != Deck.colorless;
        }
    }
}
