#region Headers
using System;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

#endregion

#region KeyDetector

namespace SecureByteResourceDecompressor.Decompressor
{
    internal class KeyDetector
    {
        public static int DetectKey(ModuleDefMD module)
        {
            foreach (var type in module.Types)
            {
                foreach (var method in type.Methods)
                {
                    if (!method.HasBody)
                        continue;

                    var instrs = method.Body.Instructions;

                    for (int i = 0; i < instrs.Count - 1; i++)
                    {
                        if (instrs[i].IsLdcI4())
                        {
                            if (instrs[i + 1].OpCode == OpCodes.Call &&
                                instrs[i + 1].Operand is IMethod methodRef &&
                                methodRef.FullName.Contains("System.BitConverter::GetBytes"))
                            {
                                int key = instrs[i].GetLdcI4Value();
                                Logger.Success("Key found in method: " + method.FullName);
                                return key;
                            }
                        }
                    }
                }
            }

            Logger.Error("Key not found!");
            throw new Exception("Key not found!");
            Console.Read();
        }
    }
}

#endregion