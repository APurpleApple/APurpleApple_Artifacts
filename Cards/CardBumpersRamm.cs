using Nickel;
using System;
using System.Collections.Generic;
using System.Reflection;
using APurpleApple.GenericArtifacts.CardActions;

namespace APurpleApple.GenericArtifacts;

internal sealed class CardBumpersRamm : Card, IModCard
{
    public static void Register(IModHelper helper)
    {
        Type type = MethodBase.GetCurrentMethod()!.DeclaringType!;
        helper.Content.Cards.RegisterCard(type.Name, new()
        {
            CardType = type,
            Meta = new()
            {
                deck = PMod.decks["Ramm"].Deck,
                rarity = Rarity.common,
                upgradesTo = [],
                dontOffer = true
            },
            Art = PMod.sprites["CardRamm"].Sprite,
            Name = PMod.Instance.AnyLocalizations.Bind(["card", "BumpersRamm", "name"]).Localize
        });
    }
    public override List<CardAction> GetActions(State s, Combat c)
    {
        var list = new List<CardAction>();

        list.Add(new ARamAnim());
        list.Add(new ARamAttack() { hurtAmount = 3});
        return list;
    }

    public override CardData GetData(State state)
    {
        CardData data = new CardData();
        switch (this.upgrade)
        {
            case Upgrade.None:
                data.cost = 1;
                break;

            case Upgrade.A:
                data.cost = 0;
                break;

            case Upgrade.B:
                data.cost = 1;
                break;
        }
        return data;
    }
}
