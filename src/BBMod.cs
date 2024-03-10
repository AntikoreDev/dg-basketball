using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;

// The title of your mod, as displayed in menus
[assembly: AssemblyTitle("Basketball Mod")]

// The author of the mod
[assembly: AssemblyCompany("Antikore + some other guy")]

// The description of the mod
[assembly: AssemblyDescription("Basketball gamemode for DG")]

// The mod's version
[assembly: AssemblyVersion("1.1.0.0")]

namespace DuckGame.BBMod
{
    public class BBMod : Mod
    {
        internal static string AssemblyName { get; private set; }

        // The mod's priority; this property controls the load order of the mod.
        public override Priority priority
		{
			get { return base.priority; }
		}

		// This function is run before all mods are finished loading.
		protected override void OnPreInitialize()
		{
			base.OnPreInitialize();
		}

		// This function is run after all mods are loaded.
		protected override void OnPostInitialize()
		{
			base.OnPostInitialize();
            BBMod.UpdateNetmessageTypes();
            this.AddTeams();
            //this.CreateBasketLevelPlaylist();
        }

        public static void UpdateNetmessageTypes()
        {
            IEnumerable<System.Type> subclasses = Editor.GetSubclasses(typeof(NetMessage));
            Network.typeToMessageID.Clear();
            ushort key = 1;
            foreach (System.Type type in subclasses)
            {
                if (type.GetCustomAttributes(typeof(FixedNetworkID), false).Length != 0)
                {
                    FixedNetworkID customAttribute = (FixedNetworkID)type.GetCustomAttributes(typeof(FixedNetworkID), false)[0];
                    if (customAttribute != null)
                        Network.typeToMessageID.Add(type, customAttribute.FixedID);
                }
            }
            foreach (System.Type type in subclasses)
            {
                if (!Network.typeToMessageID.ContainsValue(type))
                {
                    while (Network.typeToMessageID.ContainsKey(key))
                        ++key;
                    Network.typeToMessageID.Add(type, key);
                    ++key;
                }
            }
        }
        /*
        private void CreateBasketLevelPlaylist()
        {
            string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "DuckGame\\Levels\\Basket\\");
            if (!Directory.Exists(text))
            {
                Directory.CreateDirectory(text);
            }
            IList<string> list = new List<string>();
            foreach (string text2 in Directory.GetFiles(Mod.GetPath<BBMod>("levels")))
            {
                string text3 = BBMod.AssemblyName + "\\content\\levels\\";
                string text4 = text2.Replace('/', '\\');
                string text5 = text + text4.Substring(text4.IndexOf(text3) + text3.Length);
                if (!File.Exists(text5) || !this.GetMD5Hash(File.ReadAllBytes(text4)).SequenceEqual(this.GetMD5Hash(File.ReadAllBytes(text5))))
                {
                    File.Copy(text4, text5, true);
                }
                list.Add(text5);
            }
            string text6 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "DuckGame\\Levels\\Basket Levels.play");
            XElement xelement = new XElement("playlist");
            foreach (string text7 in list)
            {
                XElement content = new XElement("element", text7.Replace('\\', '/'));
                xelement.Add(content);
            }
            XDocument xdocument = new XDocument();
            xdocument.Add(xelement);
            string text8 = xdocument.ToString();
            if (string.IsNullOrWhiteSpace(text8))
            {
                throw new Exception("Blank XML (" + text6 + ")");
            }
            if (!File.Exists(text6) || !File.ReadAllText(text6).Equals(text8))
            {
                File.WriteAllText(text6, text8);
                this.SaveCloudFile(text6, "Levels/Basket Levels.play");
            }
        }
        
        private void SaveCloudFile(string path, string localPath)
        {
            if (MonoMain.disableCloud || MonoMain.cloudNoSave || !Steam.IsInitialized())
            {
                return;
            }
            byte[] array = File.ReadAllBytes(path);
            Steam.FileWrite(localPath, array, array.Length);
        }
        */

        private byte[] GetMD5Hash(byte[] sourceBytes)
        {
            return new MD5CryptoServiceProvider().ComputeHash(sourceBytes);
        }

        private void AddTeams()
        {
            Teams.core.teams.Add(new Team("Blue Caps", Mod.GetPath<BBMod>("blueCaps"), false, false, default(Vec2)));
            Teams.core.teams.Add(new Team("Pink Caps", Mod.GetPath<BBMod>("pinkCaps"), false, false, default(Vec2)));
            Teams.core.teams.Add(new Team("Yellow Caps", Mod.GetPath<BBMod>("yellowCaps"), false, false, default(Vec2)));
            Teams.core.teams.Add(new Team("Orange Caps", Mod.GetPath<BBMod>("orangeCaps"), false, false, default(Vec2)));
            Teams.core.teams.Add(new Team("Green Caps", Mod.GetPath<BBMod>("greenCaps"), false, false, default(Vec2)));
        }
    }
}
