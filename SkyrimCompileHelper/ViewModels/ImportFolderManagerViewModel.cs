// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImportFolderManagerViewModel.cs" company="Robert Logiewa">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Robert Logiewa
// </copyright>
// <summary>
//   Provides methods and properties to create and change import folders.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SkyrimCompileHelper.ViewModels
{
    using Caliburn.Micro;

    using PropertyChanged;

    using SkyrimCompileHelper.Core;

    /// <summary>Provides methods and properties to create and change import folders.</summary>
    [ImplementPropertyChanged]
    public sealed class ImportFolderManagerViewModel : Screen
    {
        /// <summary>Initialises a new instance of the <see cref="ImportFolderManagerViewModel"/> class.</summary>
        public ImportFolderManagerViewModel()
        {
            this.DisplayName = "Edit Import Folder";
            this.Folder = new ImportFolder(string.Empty);
            if (Execute.InDesignMode)
            {
                this.Folder = new ImportFolder(@"C:\SomePath");
            }
        }

        /// <summary>Initialises a new instance of the <see cref="ImportFolderManagerViewModel"/> class.</summary>
        /// <param name="folder">The import folder to change.</param>
        public ImportFolderManagerViewModel(ImportFolder folder)
        {
            this.DisplayName = "Edit Import Folder";
            this.Folder = folder;
        }

        /// <summary>Gets the folder name.</summary>
        public string FolderName
        {
            get
            {
                return this.Folder.Name;
            }
        }

        /// <summary>Gets or sets the folder path.</summary>
        [AlsoNotifyFor("FolderName")]
        public string Path
        {
            get
            {
                return this.Folder.FolderPath;
            }

            set
            {
                this.Folder.FolderPath = value;
            }
        }

        /// <summary>Gets the import folder.</summary>
        public ImportFolder Folder { get; private set; }

        /// <summary>Orderly closes the window.</summary>
        public void Close()
        {
            this.TryClose(true);
        }
    }
}
