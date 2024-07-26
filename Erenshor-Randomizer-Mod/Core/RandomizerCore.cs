using Erenshor_Randomizer_Mod.Core.Classes;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Erenshor_Randomizer_Mod.Core
{
    internal class RandomizerCore
    {
        public void start(GameObject plyr)
        {
            // Create new Player Object
            Player ply = new Player(plyr);
        }
    }
}
