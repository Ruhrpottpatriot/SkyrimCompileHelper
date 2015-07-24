namespace SkyrimCompileHelper.Common
{
    using System;
    using System.IO;

    using Newtonsoft.Json;

    public class SettingsRepository
    {
        private readonly Lazy<JsonSerializer> serializer;

        private readonly string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        private Settings settings;

        public SettingsRepository()
        {
            this.serializer = new Lazy<JsonSerializer>();
            this.settings = new Settings();
        }

        public string SkyrimPath
        {
            get
            {
                return this.settings.SkyrimPath;
            }

            set
            {
                if (this.settings.SkyrimPath != value)
                {
                    this.settings.SkyrimPath = value;
                    this.Save();
                }
            }
        }

        public string ModOrganizerPath
        {
            get
            {
                return this.settings.ModOrganizerPath;
            }

            set
            {
                if (this.settings.ModOrganizerPath != value)
                {
                    this.settings.ModOrganizerPath = value;
                    this.Save();
                }
            }
        }

        public string CompilerFlags
        {
            get
            {
                return this.settings.CompilerFlags;
            }

            set
            {
                if (this.settings.CompilerFlags != value)
                {
                    this.settings.CompilerFlags = value;
                    this.Save();
                }
            }
        }

        public void ReloadSettings()
        {
            this.settings = this.Load();
        }

        private Settings Load()
        {
            string settingsFile = Path.Combine(this.appdata, @"SHC\settings.json");

            if (File.Exists(settingsFile))
            {
                return null;
            }

            // deserialize JSON directly from a file
            using (StreamReader file = File.OpenText(settingsFile))
            {
                return (Settings)this.serializer.Value.Deserialize(file, typeof(Settings));
            }
        }

        private void Save()
        {
            string settingsDir = Path.Combine(this.appdata, "SHC");

            if (!Directory.Exists(settingsDir))
            {
                Directory.CreateDirectory(settingsDir);
            }

            // serialize JSON directly to a file
            using (StreamWriter file = File.CreateText(Path.Combine(settingsDir, "settings.json")))
            {
                this.serializer.Value.Serialize(file, this.settings);
            }
        }
    }
}