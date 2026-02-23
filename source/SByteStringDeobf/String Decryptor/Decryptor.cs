using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SByteStringDeobf
{
    internal class Decryptor
    {
        public static void DecryptStrings(Dictionary<int, string> encStrings,ModuleDefMD module,string input)
        {
            Assembly runtimeAssembly = Assembly.LoadFile(input);
            string[] cachedStrings = encStrings.Values.ToArray();

            foreach (var type in module.GetTypes())
            {
                foreach (var method in type.Methods)
                {
                    if (!method.HasBody || !method.Body.HasInstructions)
                        continue;
                    var instructions = method.Body.Instructions;
                    for (int i = 0; i < instructions.Count; i++)
                    {
                        if (instructions[i].OpCode == OpCodes.Ldsfld &&
                           instructions[i + 1].IsLdcI4() &&
                           instructions[i + 2].OpCode == OpCodes.Call)
                        {
                            if (instructions[i + 2].Operand is MethodDef decryptionMethod)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Logger.Info($"Found decryption method: {decryptionMethod.FullName}");
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                Type runtimeType = runtimeAssembly.GetType(decryptionMethod.DeclaringType.FullName);
                                MethodInfo decryptionMethodInfo = runtimeType.GetMethod(decryptionMethod.Name, BindingFlags.Public | BindingFlags.Static);

                                object[] argss =
                                    {
                                            cachedStrings,
                                            instructions[i + 1].GetLdcI4Value()
                                    };

                                try
                                {
                                    object result = decryptionMethodInfo.Invoke(null, argss);
                                    if (result is string decryptedString)
                                    {

                                        Logger.Success($"Decrypted string: {decryptedString}");
                                        instructions[i].OpCode = OpCodes.Ldstr;
                                        instructions[i].Operand = decryptedString;
                                        instructions[i + 1].OpCode = OpCodes.Nop;
                                        instructions[i + 2].OpCode = OpCodes.Nop;

                                    }

                                }
                                catch (Exception ex)
                                {
                                    Logger.Error($"Error invoking decryption method: {ex.Message}");
                                }

                            }
                            else
                            {
                                Logger.Error("Error: Operand at i + 2 is not a MethodDef.");
                            }
                        }
                    }
                }
            }
        }
    }
}
