// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImportFolder.cs" company="Robert Logiewa">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Robert Logiewa
// </copyright>
// <summary>
//   Represents an import folder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SkyrimCompileHelper.Core
{
    using System.IO;

    using Newtonsoft.Json;

    using PropertyChanged;

    /// <summary>Represents an import folder.</summary>
    [ImplementPropertyChanged]
    public class ImportFolder
    {
        /// <summary>Gets the folder name.</summary>
        [JsonIgnore]
        public string Name
        {
            get
            {
                if (this.FolderPath == null)
                {
                    return string.Empty;
                }

                string path = Path.GetFullPath(this.FolderPath).TrimEnd(Path.DirectorySeparatorChar);
                return Path.GetFileName(path);
            }
        }

        /// <summary>Gets or sets the path of the current <see cref="ImportFolder"/>.</summary>
        public string FolderPath { get; set; }
    }
}
