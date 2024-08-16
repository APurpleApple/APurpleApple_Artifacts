using Nickel;
using System;
using System.Collections.Generic;
using System.Reflection;
using APurpleApple.GenericArtifacts.CardActions;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
                upgradesTo = [Upgrade.A, Upgrade.B],
                dontOffer = true
            },
            Art = PMod.sprites["CardRamm"].Sprite,
            Name = PMod.Instance.AnyLocalizations.Bind(["card", "BumpersRamm", "name"]).Localize
        });
    }
    public override List<CardAction> GetActions(State s, Combat c)
    {
        var list = new List<CardAction>();

        switch (this.upgrade)
        {
            case Upgrade.None:
                list.Add(new ARamAnim() { hurtAmount = 1 });
                break;

            case Upgrade.A:
                list.Add(new AMove() { dir = 2, targetPlayer = true });
                list.Add(new ARamAnim() { hurtAmount = 1 });
                break;

            case Upgrade.B:
                list.Add(new ARamAnim() { hurtAmount = 2 });
                break;
        }
        return list;
    }

    public override CardData GetData(State state)
    {
        CardData data = new CardData();
        switch (this.upgrade)
        {
            case Upgrade.None:
                data.cost = 1;
                data.exhaust = true;
                break;

            case Upgrade.A:
                data.cost = 1;
                data.flippable = true;
                data.exhaust = true;
                break;

            case Upgrade.B:
                data.cost = 2;
                data.retain = true;
                data.exhaust = true;
                break;
        }
        return data;
    }
}
