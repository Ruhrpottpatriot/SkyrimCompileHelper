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

namespace SkyrimCompileHelper.Common
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Newtonsoft.Json;

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
            string environmentPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            this.settingsFilePath = Path.Combine(environmentPath, @"SHC\settings.json");
            this.settingsFolderPath = Path.Combine(environmentPath, "SHC");
        }

        /// <summary>Creates a new set of items inside the repository.</summary>
        /// <param name="items">An <see cref="IDictionaryRange{TKey,TValue}"/> of items to add to the repository.</param>
        public void Create(IDictionaryRange<string, object> items)
        {
            string json = JsonConvert.SerializeObject(items, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Include });

            File.WriteAllText(this.settingsFilePath, json);
        }

        /// <summary>Reads the complete repository and returns it to the user.</summary>
        /// <returns>An <see cref="IDictionaryRange{TKey,TValue}"/> containing all items in the repository.</returns>
        public IDictionaryRange<string, object> Read()
        {
            if (!Directory.Exists(this.settingsFolderPath))
            {
                return new DictionaryRange<string, object>();
            }

            using (StreamReader fileReader = File.OpenText(this.settingsFilePath))
            {
               return new JsonSerializer().Deserialize<DictionaryRange<string, object>>(new JsonTextReader(fileReader));
            }
        }

        /// <summary>Updates a set of items in the repository.</summary>
        /// <param name="items">An <see cref="IDictionaryRange{TKey,TValue}"/> containing the items to update.</param>
        public void Update(IDictionaryRange<string, object> items)
        {
            if (!Directory.Exists(this.settingsFolderPath))
            {
                Directory.CreateDirectory(this.settingsFolderPath);
            }

            if (File.Exists(this.settingsFilePath))
            {
                File.Delete(this.settingsFilePath);
            }

            this.Create(items);
        }

        /// <summary>Deletes a set of items from the repository.</summary>
        /// <param name="identifiers">An <see cref="IEnumerable{T}"/> listing the identifiers of the items to delete.</param>
        public void Delete(IEnumerable<string> identifiers)
        {
            var settings = this.Read().Where(s => identifiers.All(i => s.Key != i)).ToDictionary(s => s.Key, s => s.Value);
            
            this.Update(new DictionaryRange<string, object>(settings));
        }
    }
}
