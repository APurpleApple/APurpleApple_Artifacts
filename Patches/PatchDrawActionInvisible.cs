using HarmonyLib;
using System.Reflection.Emit;
using System.Reflection;
using static HarmonyLib.Code;
using System.Collections;
using System.Collections.Generic;
using APurpleApple.GenericArtifacts.CardActions;
using System.Linq;

namespace APurpleApple.GenericArtifacts.HarmonyPatches
{
    [HarmonyPatch]
    public static class PatchDrawActionInvisible
    {
        [HarmonyPatch(typeof(Card), nameof(Card.MakeAllActionIcons)), HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            int workingIndex = 0;
            List<CodeInstruction> instrs = new List<CodeInstruction>(instructions);

            LocalBuilder skippedActions = generator.DeclareLocal(typeof(int));
            LocalBuilder AIInvisibleCount = generator.DeclareLocal(typeof(int));

            MethodInfo counter = SymbolExtensions.GetMethodInfo((List<CardAction> actions) => GetAIInvisibleCount(actions));

            for (; workingIndex < instrs.Count; workingIndex++)
            {
                if (instrs[workingIndex].opcode == OpCodes.Endfinally)
                {
                    workingIndex++;
                    workingIndex++;
                    instrs.Insert(workingIndex, new CodeInstruction(OpCodes.Ldc_I4_0));
                    workingIndex++;
                    instrs.Insert(workingIndex, new CodeInstruction(OpCodes.Stloc, skippedActions));
                    workingIndex++;
                    instrs.Insert(workingIndex, new CodeInstruction(OpCodes.Ldloc, 0));
                    workingIndex++;
                    instrs.Insert(workingIndex, new CodeInstruction(OpCodes.Call, counter));
                    workingIndex++;
                    instrs.Insert(workingIndex, new CodeInstruction(OpCodes.Stloc, AIInvisibleCount));
                    workingIndex++;
                    break;
                }
            }

            Label loopEndLabel = generator.DefineLabel();
            Label ifFalseJumpLabel = generator.DefineLabel();

            int stashedWorkingIndex = workingIndex;

            for (; workingIndex < instrs.Count; workingIndex++)
            {
                if (instrs[workingIndex].ToString() == "ldloc.s 10 (System.Int32)")
                {
                    if (instrs[workingIndex+1].opcode == OpCodes.Ldc_I4_1)
                    {
                        if (instrs[workingIndex + 2].opcode == OpCodes.Add)
                        {
                            if (instrs[workingIndex + 3].ToString() == "stloc.s 10 (System.Int32)")
                            {
                                instrs[workingIndex].labels.Add(loopEndLabel);
                            }
                        }
                    }
                }
            }
            workingIndex = stashedWorkingIndex;
            
            for (; workingIndex < instrs.Count; workingIndex++)
            {
                if (instrs[workingIndex].ToString() == "stloc.s 11 (CardAction)")
                {
                    workingIndex++;
                    instrs.Insert(workingIndex, new CodeInstruction(OpCodes.Ldloc, 11)); // push the current action onto the stack
                    workingIndex++;
                    instrs.Insert(workingIndex, new CodeInstruction(OpCodes.Isinst, typeof(IAInvisible))); // check if the current action is AIInvisible
                    workingIndex++;
                    instrs.Insert(workingIndex, new CodeInstruction(OpCodes.Brfalse, ifFalseJumpLabel)); // if false, jump the following
                    workingIndex++;
                    instrs.Insert(workingIndex, new CodeInstruction(OpCodes.Ldc_I4_1)); // push 1 onto the stack
                    workingIndex++;
                    instrs.Insert(workingIndex, new CodeInstruction(OpCodes.Ldloc, skippedActions)); // push skippedActions onto the stack
                    workingIndex++;
                    instrs.Insert(workingIndex, new CodeInstruction(OpCodes.Add)); // Add both
                    workingIndex++;
                    instrs.Insert(workingIndex, new CodeInstruction(OpCodes.Stloc, skippedActions)); // Push the result into skippedActions
                    workingIndex++;

                    instrs.Insert(workingIndex, new CodeInstruction(OpCodes.Br, loopEndLabel));
                    workingIndex++;
                    CodeInstruction ifJumpArrival = new CodeInstruction(OpCodes.Nop);
                    ifJumpArrival.labels.Add(ifFalseJumpLabel);
                    instrs.Insert(workingIndex, ifJumpArrival);
                    workingIndex++;
                    break;
                }
            }

            for (; workingIndex < instrs.Count; workingIndex++)
            {
                if (instrs[workingIndex].ToString() == "ldloc.s 10 (System.Int32)")
                {
                    if (instrs[workingIndex +1].ToString() == "ldc.i4.s 12")
                    {
                        workingIndex++;
                        instrs.Insert(workingIndex, new CodeInstruction(OpCodes.Ldloc, skippedActions));
                        workingIndex++;
                        instrs.Insert(workingIndex, new CodeInstruction(OpCodes.Sub));
                        workingIndex+=4;
                        instrs.Insert(workingIndex, new CodeInstruction(OpCodes.Ldloc, AIInvisibleCount));
                        workingIndex++;
                        instrs.Insert(workingIndex, new CodeInstruction(OpCodes.Ldc_I4_6));
                        workingIndex++;
                        instrs.Insert(workingIndex, new CodeInstruction(OpCodes.Mul));
                        workingIndex++;
                        instrs.Insert(workingIndex, new CodeInstruction(OpCodes.Add));
                        break;
                    }
                }
            }
            return instrs.AsEnumerable();
        }

        static public int GetAIInvisibleCount(List<CardAction> actions)
        {
            return actions.Where(x => x is IAInvisible).Count();
        }
    }
}
