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
    internal class ArtifactPainter : Artifact, IModArtifact
    {
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
                /*foreach (Deck item in colors)
                {
                    CardAction? action = GetColorAction(state, item);
                    if (action != null)
                    {
                        combat.Queue(action);
                    }
                }*/
                colors.Clear();
            }
        }

        public CardAction GetColorAction(State state, Deck color)
        {
            switch (color)
            {
                case Deck.dizzy:
                    return new AStatus() { targetPlayer = true, status = Status.tempShield, statusAmount = 1 };
                case Deck.riggs:
                    return new AMove() { targetPlayer = true, isRandom = true, dir = 1 };
                case Deck.peri:
                    return new AAttack() { damage = Card.GetActualDamage(state, 0), targetPlayer = false };
                case Deck.goat:
                    return new AStatus() { targetPlayer = true, status = Status.droneShift, statusAmount = 1 };
                case Deck.eunice:
                    return new AStatus() { targetPlayer = false, status = Status.heat, statusAmount = 1 };
                case Deck.hacker:
                    return new ADrawCard() { count = 1 };
                case Deck.shard:
                    return new AStatus() { targetPlayer = true, status = Status.shard, statusAmount = 1 };
            }
            return new AStatus() { targetPlayer = true, status = Status.energyFragment, statusAmount = 1};
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
            tooltips.Add(new CustomTTGlossary(
                CustomTTGlossary.GlossaryType.action,
                () => PMod.sprites["StatusPaint"].Sprite,
                () => PMod.Instance.Localizations.Localize(["glossary", "Pigment", "name"]),
                () => PMod.Instance.Localizations.Localize(["glossary", "Pigment", "description"])
                ));

            foreach (Deck item in colors)
            {
                switch (item)
                {
                    case Deck.dizzy:
                        tooltips.Add(new TTText(String.Format(PMod.Instance.Localizations.Localize(["glossary", "Pigment", "dizzy"]), 1)));
                        break;
                    case Deck.riggs:
                        tooltips.Add(new TTText(String.Format(PMod.Instance.Localizations.Localize(["glossary", "Pigment", "riggs"]), 1)));
                        break;
                    case Deck.peri:
                        tooltips.Add(new TTText(String.Format(PMod.Instance.Localizations.Localize(["glossary", "Pigment", "peri"]), 1)));
                        break;
                    case Deck.goat:
                        tooltips.Add(new TTText(String.Format(PMod.Instance.Localizations.Localize(["glossary", "Pigment", "isaac"]), 1)));
                        break;
                    case Deck.eunice:
                        tooltips.Add(new TTText(String.Format(PMod.Instance.Localizations.Localize(["glossary", "Pigment", "drake"]), 1)));
                        break;
                    case Deck.hacker:
                        tooltips.Add(new TTText(String.Format(PMod.Instance.Localizations.Localize(["glossary", "Pigment", "max"]), 1)));
                        break;
                    case Deck.shard:
                        tooltips.Add(new TTText(String.Format(PMod.Instance.Localizations.Localize(["glossary", "Pigment", "books"]), 1)));
                        break;
                    default:
                        tooltips.Add(new TTText(String.Format(PMod.Instance.Localizations.Localize(["glossary", "Pigment", "noone"]), 1)));
                        break;
                }
            }

            return tooltips;
        }

        public Color DeckToColor(Deck deck)
        {
            return DB.decks[deck].color;
        }

        public bool isColorValid(Deck color)
        {
            return color != Deck.colorless;
            switch (color)
            {
                case Deck.dizzy:
                case Deck.riggs:
                case Deck.peri:
                case Deck.goat:
                case Deck.eunice:
                case Deck.hacker:
                case Deck.shard:
                    return true;
                default:
                    return false;
            }
        }
    }
}
