using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;
using MelonLoader;

namespace Erenshor_Randomizer_Mod.Core.Classes
{
    internal class NPCRandomizer
    {
        public NPCRandomizer() 
        {
            if (Randomizer.config.GetBool("RandomizeNPCLevels"))
            {
                randomLevels();
            }

            if (Randomizer.config.GetBool("RandomizeNPCSizes"))
            {
                randomSizes();
            }
        }

        private static System.Random rnd = new System.Random();

        public static void randomLevels()
        {
            foreach (var npc in NPCTable.LiveNPCs)
            {
                npc.GetComponent<Stats>().Level = rnd.Next(1, 10);
            }

            Melon<Randomizer>.Logger.Msg("Randomized NPC Levels");
        }

        public static void randomSizes()
        {
            foreach (var npc in NPCTable.LiveNPCs)
            {
                var x = rnd.Next(1, 5);
                var y = rnd.Next(1, 5);
                var z = rnd.Next(1, 5);

                npc.gameObject.transform.localScale = new Vector3(x, y, z);
            }

            Melon<Randomizer>.Logger.Msg("Randomized NPC Sizes");
        }
    }
}
