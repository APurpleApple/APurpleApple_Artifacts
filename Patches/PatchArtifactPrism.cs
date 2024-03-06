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
        public static void AddReward(State __0,Deck? __2,BattleType __3,bool? __5, bool __7,bool __9, ref List<Card> __result)
        {
            if (__2.HasValue || __7 || __9 || !__0.EnumerateAllArtifacts().Any((a) => a is ArtifactPrism)) return;


            List<Deck> list = __0.storyVars.GetUnlockedChars().Where((d) => !__0.characters.Any((Character ch) => ch.deckType == d)).ToList();

            Deck foundCharacter = list.Random(__0.rngCardOfferings);
            Rarity rarity = CardReward.GetRandomRarity(__0.rngCardOfferings, __3);

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
                Card? card = Activator.CreateInstance(validCards.Random(__0.rngCardOfferings).GetType()) as Card;

                if (card != null)
                {
                    card.drawAnim = 1.0;
                    card.upgrade = CardReward.GetUpgrade(__0.rngCardOfferings, __0.map, card, (__0.GetDifficulty() >= 1) ? 0.5 : 1.0, __5);
                    card.flipAnim = 1.0;
                    __result.RemoveAt(__result.Count - 1);
                    __result.Add(card);
                }
            }
        }
    }
}
