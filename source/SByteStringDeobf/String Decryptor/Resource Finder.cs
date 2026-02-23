using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SByteStringDeobf
{
    internal class ResourceFinder
    {
        public static string FindResName(ModuleDefMD module)
        {
            foreach (var type in module.GetTypes())
            {
                foreach (var method in type.Methods.Where(m => m.HasBody))
                {
                    var instructions = method.Body.Instructions;
                    for (int i = 0; i < instructions.Count; i++)
                    {
                        if (
                            instructions[i].OpCode == OpCodes.Newarr &&
                            instructions[i + 1].OpCode == OpCodes.Stsfld &&
                            instructions[i + 2].OpCode == OpCodes.Ldtoken &&
                            instructions[i + 3].OpCode == OpCodes.Call &&
                            instructions[i + 4].OpCode == OpCodes.Callvirt &&
                            instructions[i + 5].OpCode == OpCodes.Ldstr &&
                            instructions[i + 6].OpCode == OpCodes.Callvirt &&
                            instructions[i + 6].Operand is IMethod methodOperand &&
                            methodOperand.Name == "GetManifestResourceStream")
                        {
                            string resName = instructions[i + 5].Operand.ToString();
                            return resName;
                        }
                    }
                }
            }
            return null;
        }
    }
}
