using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using APurpleApple.GenericArtifacts.VFXs;
using System.Collections;
namespace APurpleApple.GenericArtifacts.HarmonyPatches
{
    [HarmonyPatch()]
    public static class PatchArtifactPrism
    {
        [HarmonyPatch(typeof(CardReward), nameof(CardReward.GetOffering)), HarmonyPostfix]
        public static void AddReward(State s, int count, Deck? limitDeck, BattleType battleType, bool? overrideUpgradeChances, bool makeAllCardsTemporary, bool inCombat, ref List<Card> __result)
        {
            if (limitDeck.HasValue || makeAllCardsTemporary || inCombat || !s.EnumerateAllArtifacts().Any((a) => a is ArtifactPrism)) return;


            List<Deck> list = s.storyVars.GetUnlockedChars().Where((d) => !s.characters.Any((Character ch) => ch.deckType == d)).ToList();

            Deck foundCharacter = list.Random(s.rngCardOfferings);
            Rarity rarity = CardReward.GetRandomRarity(s.rngCardOfferings, battleType);

            List<Card> validCards = DB.releasedCards.Where(delegate (Card c)
            {
                CardMeta meta = c.GetMeta();
                if (meta.rarity != rarity)
                {
                    return false;
                }

                if (meta.deck != foundCharacter) return false;

                if (meta.dontOffer)
                {
                    return false;
                }

                if (meta.unreleased)
                {
                    return false;
                }

                return (!meta.weirdCard) ? true : false;
            }).ToList();
            if (validCards.Count() != 0)
            {
                Card? card = Activator.CreateInstance(validCards.Random(s.rngCardOfferings).GetType()) as Card;

                if (card != null)
                {
                    card.drawAnim = 1.0;
                    card.upgrade = CardReward.GetUpgrade(s, s.rngCardOfferings, s.map, card, (s.GetDifficulty() >= 1) ? 0.5 : 1.0, overrideUpgradeChances);
                    card.flipAnim = 1.0;
                    __result.RemoveAt(__result.Count - 1);
                    __result.Add(card);
                }
            }
        }
    }
}
