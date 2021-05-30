using System;
using System.Collections.Generic;
using System.IO;

namespace BSPLuaChecker
{
    class Program
    {
        static void CheckMap(string path)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine($"Could not find file '{path}'");
                return;
            }

            Console.Write($"Checked {path}: ");

            BSP bsp = new BSP(path);
            List<KeyValueGroup> ents = bsp.GetEntities();

            if (ents.Count == 0)
            {
                Console.WriteLine("This map contains 0 entities.");
                return;
            }

            bool found = false;
            foreach(KeyValueGroup ent in ents)
            {
                if(ent.name == "lua_run")
                {
                    if (!found)
                        Console.WriteLine("Found lua_run.");

                    Console.WriteLine($"hammerid: {ent.hammerid}. KeyValues: {ent.raw}");
                    found = true;
                }
            }

            if (found)
                return;

            if(bsp.GetEntityString().Contains("lua_run"))
                Console.WriteLine("Found lua_run but was unable to parse KVP data.");
            else
                Console.WriteLine("No lua_run found.");
        }

        static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Please drag a file onto this executable.");
                Console.ReadLine();
                return;
            }
            
            for(int i = 0; i < args.Length; i++)
                CheckMap(args[i]);

            Console.WriteLine("All files checked.");
            Console.ReadLine();
        }
    }
}