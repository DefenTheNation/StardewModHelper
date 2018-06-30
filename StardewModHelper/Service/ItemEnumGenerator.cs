using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewModHelper.Service
{
    public enum InstallPathType
    {
        Custom,
        Steam,
        GOG
    }

    public class ItemEnumGenerator
    {
        public const string SteamInstallPath = @"";
        public const string GOGInstallPath = @"C:\Program Files (x86)\GOG Galaxy\Games\Stardew Valley";

        public string UnpackedDirectory { get; set; }
        public Dictionary<string, string> Translations { get; set; }

        public ItemEnumGenerator()
        {
            setupTranslations();
        }

        public ItemEnumGenerator(string fullpath)
        {
            UnpackedDirectory = fullpath;
            setupTranslations();
        }

        public ItemEnumGenerator(InstallPathType type)
        {
            switch(type)
            {
                case InstallPathType.Steam:
                    UnpackedDirectory = Path.Combine(SteamInstallPath, "UnPacked");
                    break;
                case InstallPathType.GOG:
                    UnpackedDirectory = Path.Combine(GOGInstallPath, "UnPacked");
                    break;
                case InstallPathType.Custom: default:
                    break;
            }

            setupTranslations();
        }

        private void setupTranslations()
        {
            Translations = new Dictionary<string, string>()
            {
                { "Stone", "DiamondStone" },
                { "Stone2", "RubyStone" },
                { "Stone3", "GeodeStone" },
                { "Stone4", "FrozenGeodeStone" },
                { "Stone5", "MagmaGeodeStone" },
                { "Stone6", "IronOreStone" },
                { "Stone7", "WeirdStone" },
                { "Stone8", "Stone" },
                { "Stone9", "StoneMines" },
                { "Stone10", "StoneOre1" },
                { "Stone11", "StoneOre2" },
                { "Stone12", "CopperOreStone" },
                { "Stone13", "StoneMinesDark1" },
                { "Stone14", "StoneMinesDark2" },
                { "Stone15", "GoldOreStone" },
                { "Stone16", "IridiumOreStone" },
                { "WarpTotem", "WarpTotemFarm" },
                { "WarpTotem2", "WarpTotemMountains" },
                { "WarpTotem3", "WarpTotemBeach" },
                { "Egg", "WhiteEgg" },
                { "LargeEgg", "LargeWhiteEgg" },
                { "Egg2", "BrownEgg" },
                { "LargeEgg2", "LargeBrownEgg" },
            };
        }

        public void GenerateEnumFromAll()
        {
            string output = "";

            output += @"
public enum ItemType
{
    Object = 0,
    BigCraftable = 1,
    Weapon = 2,
    SpecialItem = 3,
    RegularObjectRecipe = 4,
    BigCraftableRecipe = 5,
}

public enum Quality
{
    Regular = 0,
    Silver = 1,
    Gold = 2,
    Iridium = 4
}

";


            output += generateIfExists("Boots", "Boots.yaml");
            output += generateIfExists("Hats", "hats.yaml");
            output += generateIfExists("Weapons", "weapons.yaml");
            output += generateIfExists("BigCraftables", "BigCraftablesInformation.yaml");
            output += generateIfExists("Objects", "ObjectInformation.yaml");

            File.WriteAllText("ItemCodes.cs", output);
        }

        private string generateIfExists(string enumName, string YAMLName)
        {
            string fullPath = Path.Combine(UnpackedDirectory, YAMLName);

            if (File.Exists(fullPath)) return GenerateEnumFromYAML(enumName, fullPath);
            else return "";
        }

        public string GenerateEnumFromYAML(string enumName, string YAMLPath)
        {
            bool isContent = false;
            int id = -1, duplicateCount = -1;
            string name = "";

            Dictionary<string, int> duplicates = new Dictionary<string, int>();

            string output = getEnumStart(enumName);
            string[] fileData = File.ReadAllLines(YAMLPath);           

            foreach(var lineData in fileData)
            {
                if (!isContent && lineData.StartsWith("content:"))
                {
                    isContent = true;
                }
                else if(isContent)
                {
                    // 1. Parse
                    // 2. Find duplicates
                    // 3. Translate to readable name, if applicable

                    var test = lineData.Split(':');
                    if(!int.TryParse(test[0].Trim(), out id))
                    {
                        Debug.WriteLine("Unable to parse id " + test[0]);
                        continue;
                    }

                    var data = test[1].Replace("\"", "").Replace("'", "").Trim().Split('/');

                    name = data[0].Replace("\"", "").Replace("?", "").Replace("-", "").Replace(".", "").Replace(" ", "").Trim();
                   
                    if (duplicates.TryGetValue(name, out duplicateCount))
                    {
                        duplicateCount++;
                        duplicates[name] = duplicateCount;
                        name += duplicateCount;
                    }
                    else
                    {
                        duplicates.Add(name, 1);
                    }

                    // Translate the name from 
                    if (Translations.TryGetValue(name, out string translation))
                    {
                        name = translation;
                    }

                    output += name + " = " + id + "," + Environment.NewLine;
                }
            }

            output += getEnumEnd();
            return output;
        }

        private string getEnumStart(string enumName)
        {
            return "public enum " + enumName + Environment.NewLine + "{" + Environment.NewLine;
        }

        private string getEnumEnd()
        {
            return "}" + Environment.NewLine + Environment.NewLine;
        }
    }
}
