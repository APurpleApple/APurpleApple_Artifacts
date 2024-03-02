using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using APurpleApple.GenericArtifacts.CardActions;
using Nickel;

namespace APurpleApple.GenericArtifacts.Cards
{
    internal class CardPrinter : Card, IModCard
    {
        public static void Register(IModHelper helper)
        {
            Type type = MethodBase.GetCurrentMethod()!.DeclaringType!;
            helper.Content.Cards.RegisterCard(type.Name, new()
            {
                CardType = type,
                Meta = new()
                {
                    deck = Deck.trash,
                    rarity = Rarity.common,
                    upgradesTo = [Upgrade.A, Upgrade.B],
                    dontOffer = true
                },
                Art = PMod.sprites["CardPrinter"].Sprite,
                Name = PMod.Instance.AnyLocalizations.Bind(["card", "BlankCard", "name"]).Localize,
            });
        }
        public override List<CardAction> GetActions(State s, Combat c)
        {
            var list = new List<CardAction>();
            list.Add(new ACardSelect() { browseSource = CardBrowse.Source.Hand, browseAction = new ACopyCardTo() { destination = CardDestination.Exhaust} });
            return list;
        }
        public override CardData GetData(State state)
        {
            CardData data = new CardData();
            data.cost = 1;
            data.temporary = true;
            data.singleUse = true;
            data.description = PMod.Instance.Localizations.Localize(["card", "BlankCard", "description"]);
            return data;
        }

    }
}
