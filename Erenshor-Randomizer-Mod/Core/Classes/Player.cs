using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MelonLoader;
using HarmonyLib;

namespace Erenshor_Randomizer_Mod.Core.Classes
{
    internal class Player
    {
        private static System.Random rnd = new System.Random();

        public Player(GameObject ply) 
        {
            gObject = ply;

            randomizeNewCharacter();
        }

        private static GameObject gObject;

        private void randomizeNewCharacter()
        {
            // Check if it's a new Character
            if (gObject.GetComponent<Stats>().Level != 1)
                return;

            if(Randomizer.config.GetBool("RandomizeStats"))
            {
                // Give the Player random Stats
                randomStats();
            }

            if (Randomizer.config.GetBool("RandomizeRunSpeed"))
            {
                // Runspeed
                randomRunSpeed();
            }

            if (Randomizer.config.GetBool("RandomizeItems"))
            {
                // Set the Players Item in the Inventory to random Items
                randomItems(Randomizer.config.GetBool("OnlyForMyClass"));
            }

            if (Randomizer.config.GetBool("RandomizeGold"))
            {
                // Randomize Gold
                randomGold();
            }

            if (Randomizer.config.GetBool("RandomizeScale"))
            {
                // Random Scale
                randomScale();
            }

            if (Randomizer.config.GetBool("RandomizeAuras"))
            {
                // Random Auras
                randomAuras();
            }

            if (Randomizer.config.GetBool("RandomizeSkills"))
            {
                // Random Skills
                randomSkills();
            }

            if(Randomizer.config.GetBool("RandomizePlayerSpells"))
            {
                // Random Spells
                randomSpells();
            }

            // When finished levelup!
            gObject.GetComponent<Stats>().DoLevelUp();
        }

        private void randomSpells()
        {
            var SpellsDB = GameData.GM.GetComponent<SpellDB>().SpellDatabase;

            for (int i = 0; i < rnd.Next(0, 5); i++)
            {
                gObject.GetComponent<CastSpell>().KnownSpells.Add(SpellsDB[rnd.Next(0, SpellsDB.Length - 1)]);
            }
        }

        private void randomSkills()
        {
            var SkillDB = GameData.GM.GetComponent<SkillDB>().SkillDatabase;

            for (int i = 0; i < rnd.Next(0, 5); i++)
            {
                gObject.GetComponent<UseSkill>().KnownSkills.Add(SkillDB[rnd.Next(0, SkillDB.Length - 1)]);
            }
        }

        private void randomAuras()
        {
            var spellDB = GameData.GM.GetComponent<SpellDB>().SpellDatabase;

            for (int i = 0; i < rnd.Next(0, 15); i++)
            {
                Spell applyingSpell = spellDB[rnd.Next(0, spellDB.Length - 1)];

                if (applyingSpell.TargetDamage < gObject.GetComponent<Stats>().CurrentHP && applyingSpell.SpellDurationInTicks > 0 && applyingSpell.StunTarget == false)
                {
                    try
                    {
                        gObject.GetComponent<Stats>().AddAuras(applyingSpell);
                        Melon<Randomizer>.Logger.Msg($"{applyingSpell.SpellName} has been applied to the player");
                    }
                    catch (Exception e)
                    {
                        Melon<Randomizer>.Logger.Warning($"{applyingSpell.SpellName} couldn't be applied: {e.Message}");
                    }
                }
            }
        }

        private void randomScale()
        {
            var x = rnd.Next(1, 4);
            var y = rnd.Next(1, 4);
            var z = rnd.Next(1, 4);

            gObject.transform.localScale = new Vector3(x, y, z);
            Melon<Randomizer>.Logger.Msg($"New Scale {x}, {y}, {z} has been applied to the player");
        }

        private void randomRunSpeed()
        {
            var newSpeed = rnd.Next(25, 100);
            gObject.GetComponent<Stats>().actualRunSpeed = newSpeed;
            Melon<Randomizer>.Logger.Msg($"Players new Run Speed is {newSpeed}");
        }

        private void randomStats()
        {
            // Get all Stats
            var stats = gObject.GetComponent<Stats>();
            List<float> statsValues = new List<float>() { stats.BaseAC, stats.BaseAgi, stats.BaseCha, stats.BaseDex, stats.BaseEnd, stats.BaseER, stats.BaseHP, stats.BaseInt, stats.BaseLifesteal, stats.BaseMana, stats.BaseMHAtkDelay, stats.BaseMR, stats.BaseOHAtkDelay, stats.BasePR, stats.BaseRes, stats.BaseStr, stats.BaseVR, stats.BaseWis, stats.PercentLifesteal };

            // Loop through all Stats and give random Values
            foreach (var stat in statsValues)
            {
                randomizeStat(stat);
            }

            stats.CurrentHP = stats.CurrentMaxHP;
            stats.CurrentMana = stats.GetCurrentMaxMana();
        }

        private void randomItems(bool onlyForMyClass)
        {
            // Get Inventory
            var inventory = gObject.GetComponent<Inventory>();

            // Get All Items Ingame
            var itemDB = GameData.ItemDB.ItemDB;

            foreach (var item in inventory.ALLSLOTS)
            {
                if(item.MyItem.ItemName != "Empty")
                {
                    swapItem(item, itemDB, onlyForMyClass);
                }
            }
        }

        private void swapItem(ItemIcon newItem, Item[] itemDB, bool onlyForMyClass)
        {
            if (onlyForMyClass)
            {
                var oldItemName = newItem.MyItem.ItemName;
                newItem.MyItem = itemDB[rnd.Next(0, itemDB.Length - 1)];

                var plyClass = gObject.GetComponent<Stats>().CharacterClass;

                while (newItem.MyItem.Classes.Count > 0 && !(newItem.MyItem.Classes.Contains(plyClass)))
                {
                    swapItem(newItem, itemDB, onlyForMyClass);
                }

                Melon<Randomizer>.Logger.Msg($"{oldItemName} has been changed into {newItem.MyItem.ItemName}");
            } else
            {
                var oldItemName = newItem.MyItem.ItemName;
                newItem.MyItem = itemDB[rnd.Next(0, itemDB.Length - 1)];

                Melon<Randomizer>.Logger.Msg($"{oldItemName} has been changed into {newItem.MyItem.ItemName}");
            }
            Melon<Randomizer>.Logger.Msg($"FUNCTION WORKS!");
        }

        private static void swapEquippedItem(ItemIcon newItem, Item[] itemDB)
        {
            var oldItemName = newItem.MyItem.ItemName;
            var plyClass = gObject.GetComponent<Stats>().CharacterClass;
            var requiredSlot = newItem.MyItem.RequiredSlot;

            // Funktion zum Swappen des Items, das zu Klasse und Slot passt
            Item GetRandomItem(Item[] items, Class playerClass, Item.SlotType slot)
            {
                Item selectedItem;
                do
                {
                    selectedItem = items[rnd.Next(0, items.Length)];
                } while (!(selectedItem.Classes.Contains(playerClass) && selectedItem.RequiredSlot == slot));

                return selectedItem;
            }

            // Neues Item auswählen, das von der Klasse tragbar ist und zum Slot passt
            newItem.MyItem = GetRandomItem(itemDB, plyClass, requiredSlot);

            Melon<Randomizer>.Logger.Msg($"{oldItemName} has been changed into {newItem.MyItem.ItemName}");
        }

        private void randomGold()
        {
            gObject.GetComponent<Inventory>().Gold = rnd.Next(0, 500);
        }

        private void randomizeStat(float stat)
        {
            var randomValue = rnd.Next(-100, 100);
            stat = randomValue;
            Melon<Randomizer>.Logger.Msg($"Randomized {nameof(stat)} to value {randomValue}");
        }

        [HarmonyPatch(typeof(Stats), "DoLevelUp")]
        private static class Patch
        {
            private static void Postfix()
            {
                if (Randomizer.config.GetBool("RandomizePlayerGearOnLevelUp"))
                {
                    // Get Inventory
                    var inventory = gObject.GetComponent<Inventory>();

                    // Get All Items Ingame
                    var itemDB = GameData.ItemDB.ItemDB;

                    foreach (var item in inventory.EquipmentSlots)
                    {
                        if (item.MyItem.ItemName != "Empty")
                        {
                            swapEquippedItem(item, itemDB);
                        }
                    }
                }
            }
        }
    }
}
