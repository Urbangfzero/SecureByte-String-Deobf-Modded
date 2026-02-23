using dnlib.DotNet;
using dnlib.DotNet.Writer;
using SByteStringDeobf;
using SecureByteResourceDecompressor.Decompressor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SecureByteToolkit
{
    internal class Program
    {
        private static string inputPath;
        private static string outputPath;

        static void Main(string[] args)
        {
            SetupConsole();

            if (!ResolveInput(args))
                return;

            ModuleDefMD module = ModuleDefMD.Load(inputPath);
            outputPath = BuildOutputPath(inputPath);

           
            TryDecompressResources(module);

           
            TryDecryptStrings(module);

            SaveModule(module);

            Logger.Success("Finished successfully.");
            Console.ReadKey();
        }

      

        private static void SetupConsole()
        {
            Console.Title = "SecureByte String Decryptor";
            Console.Clear();

            Logger.Custom(@"
 =============================================
    SecureByte String Decryptor
 =============================================
", System.Drawing.Color.Cyan);
        }

     

        private static bool ResolveInput(string[] args)
        {
            if (args.Length > 0)
                inputPath = args[0];

            while (string.IsNullOrEmpty(inputPath) || !File.Exists(inputPath))
            {
                Logger.Warn("Enter valid file path:");
                inputPath = Console.ReadLine();
            }

            Logger.Success("Loaded file: " + inputPath);
            return true;
        }

        private static string BuildOutputPath(string input)
        {
            return Path.Combine(
                Path.GetDirectoryName(input),
                Path.GetFileNameWithoutExtension(input) + "-Cleaned" +
                Path.GetExtension(input));
        }


        private static void TryDecompressResources(ModuleDefMD module)
        {
            Logger.Info("Checking for compressed resources...");

            var resource = module.Resources
                .OfType<EmbeddedResource>()
                .FirstOrDefault(r => r.Name.EndsWith(".resources"));

            if (resource == null)
            {
                Logger.Warn("No compressed resource found. Skipping decompression.");
                return;
            }

            Logger.Success("Encrypted resource found: " + resource.Name);

            try
            {
                int key = KeyDetector.DetectKey(module);
                Logger.Success("Detected Key: " + key);

                byte[] encrypted = resource.CreateReader().ToArray();
                byte[] decrypted = Decompressor.Decompress(encrypted, key);
                byte[] unpacked = QuickLZ.DecompressBytes(decrypted, 1);

                ModuleDefMD unpackedModule = ModuleDefMD.Load(unpacked);

                ReplaceResources(module, unpackedModule);

                Logger.Success("Resource decompression complete.");
            }
            catch (Exception ex)
            {
                Logger.Error("Resource decompression failed: " + ex.Message);
            }
        }

        private static void ReplaceResources(ModuleDefMD original, ModuleDefMD unpacked)
        {
            int replaced = 0;

            foreach (var res in unpacked.Resources)
            {
                var existing = original.Resources.FirstOrDefault(r => r.Name == res.Name);

                if (existing != null)
                {
                    original.Resources.Remove(existing);
                    replaced++;
                }

                original.Resources.Add(res);
            }

            Logger.Info("Resources replaced: " + replaced);
        }

       

        private static void TryDecryptStrings(ModuleDefMD module)
        {
            Logger.Info("Searching for encrypted string resource...");

            string resourceName = ResourceFinder.FindResName(module);

            if (resourceName == null)
            {
                Logger.Warn("No string resource found. Skipping string decryption.");
                return;
            }

            var embedded = module.Resources.Find(resourceName) as EmbeddedResource;

            if (embedded == null)
                return;

            try
            {
                byte[] resourceBytes;

                using (Stream stream = embedded.CreateReader().AsStream())
                using (MemoryStream memory = new MemoryStream())
                {
                    stream.CopyTo(memory);
                    resourceBytes = memory.ToArray();
                }

                byte[] decompressed = QuickLZ.DecompressBytes(resourceBytes, 2);
                Dictionary<int, string> strings = ReadStringsFromBytes(decompressed);

                Decryptor.DecryptStrings(strings, module, inputPath);

                Logger.Success("String decryption complete.");
            }
            catch (Exception ex)
            {
                Logger.Error("String decryption failed: " + ex.Message);
            }
        }

        private static Dictionary<int, string> ReadStringsFromBytes(byte[] data)
        {
            Dictionary<int, string> result = new Dictionary<int, string>();

            using (MemoryStream memory = new MemoryStream(data))
            using (StreamReader reader = new StreamReader(memory))
            {
                string line;
                int index = 0;

                while ((line = reader.ReadLine()) != null)
                    result[index++] = line;
            }

            return result;
        }

      
        private static void SaveModule(ModuleDefMD module)
        {
            try
            {
                ModuleWriterOptions options = new ModuleWriterOptions(module);
                options.MetadataOptions.Flags = MetadataFlags.PreserveAll;
                options.Logger = DummyLogger.NoThrowInstance;

                module.Write(outputPath, options);

                Logger.Success("Saved cleaned file: " + outputPath);
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to save cleaned EXE: " + ex.Message);
            }
        }
    }
}