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

    using PropertyChanged;

    /// <summary>Represents an import folder.</summary>
    [ImplementPropertyChanged]
    public class ImportFolder
    {
        /// <summary>Initialises a new instance of the <see cref="ImportFolder"/> class. </summary>
        /// <param name="path">The path to the import folder.</param>
        public ImportFolder(string path)
        {
            this.FolderPath = path;
            this.Name = Path.GetFileName(Path.GetFullPath(this.FolderPath).TrimEnd(Path.DirectorySeparatorChar));
        }

        /// <summary>Initialises a new instance of the <see cref="ImportFolder"/> class. </summary>
        /// <param name="path">The path to the import folder.</param>
        /// <param name="name">The display name of the import folder.</param>
        public ImportFolder(string path, string name)
        {
            this.FolderPath = path;
            this.Name = name;
        }

        /// <summary>Gets or sets the folder name.</summary>
        public string Name { get; set; }

        /// <summary>Gets or sets the path of the current <see cref="ImportFolder"/>.</summary>
        public string FolderPath { get; set; }
    }
}
