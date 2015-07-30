// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsRepository.cs" company="Robert Logiewa">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Robert Logiewa
// </copyright>
// <summary>
//   Contains methods and properties to manipulate the application settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SkyrimCompileHelper.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Newtonsoft.Json;

    using SkyrimCompileHelper.Common;

    /// <summary>Contains methods and properties to manipulate the application settings.</summary>
    public class SettingsRepository : ISettingsRepository
    {
        /// <summary>The path to the settings folder.</summary>
        private readonly string settingsFolderPath;

        /// <summary>The path to the settings file.</summary>
        private readonly string settingsFilePath;

        /// <summary>Initializes a new instance of the <see cref="SettingsRepository"/> class.</summary>
        public SettingsRepository()
        {
            this.Settings = this.ReadSettingsFile();
            string environmentPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            this.settingsFilePath = Path.Combine(environmentPath, @"SHC\settings.json");
            this.settingsFolderPath = Path.Combine(environmentPath, "SHC");
        }

        /// <summary>Gets or sets the settings.</summary>
        private Settings Settings { get; set; }

        /// <summary>Creates a new set of items inside the repository.</summary>
        /// <param name="items">An <see cref="IDictionaryRange{TKey,TValue}"/> of items to add to the repository.</param>
        public void Create(IDictionaryRange<string, object> items)
        {
            throw new NotImplementedException();
        }

        /// <summary>Reads the complete repository and returns it to the user.</summary>
        /// <returns>An <see cref="IDictionaryRange{TKey,TValue}"/> containing all items in the repository.</returns>
        public IDictionaryRange<string, object> Read()
        {
            throw new NotImplementedException();
        }

        /// <summary>Updates a set of items in the repository.</summary>
        /// <param name="items">An <see cref="IDictionaryRange{TKey,TValue}"/> containing the items to update.</param>
        public void Update(IDictionaryRange<string, object> items)
        {
            throw new NotImplementedException();
        }

        /// <summary>Deletes a set of items from the repository.</summary>
        /// <param name="identifiers">An <see cref="IEnumerable{T}"/> listing the identifiers of the items to delete.</param>
        public void Delete(IEnumerable<string> identifiers)
        {
            throw new NotImplementedException();
        }

        /// <summary>Reads and parses the settings file.</summary>
        /// <returns>The parsed <see cref="Settings"/>.</returns>
        private Settings ReadSettingsFile()
        {
            if (!Directory.Exists(this.settingsFolderPath))
            {
                Directory.CreateDirectory(this.settingsFolderPath);
                return new Settings();
            }
            
            using (StreamReader fileReader = File.OpenText(this.settingsFilePath))
            {
                return new JsonSerializer().Deserialize<Settings>(new JsonTextReader(fileReader));
            }
        }

        /// <summary>Writes the settings to the file system.</summary>
        private void WriteSettingsFile()
        {
            if (!Directory.Exists(this.settingsFolderPath))
            {
                Directory.CreateDirectory(this.settingsFolderPath);
            }

            if (File.Exists(this.settingsFilePath))
            {
                File.Delete(this.settingsFilePath);
            }

            using (StreamWriter file = File.CreateText(this.settingsFilePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, this.Settings);
            }
        }
    }
}
