// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NewRepositoryViewModel.cs" company="Robert Logiewa">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Robert Logiewa
// </copyright>
// <summary>
//   ViewModel containing methods and properties to create a new mod repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SkyrimCompileHelper.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Forms;

    using Caliburn.Micro;

    using PropertyChanged;

    using Semver;

    using SkyrimCompileHelper.Common;

    using Screen = Caliburn.Micro.Screen;

    /// <summary>ViewModel containing methods and properties to create a new mod repository.</summary>
    [ImplementPropertyChanged]
    public class NewRepositoryViewModel : Screen
    {
        /// <summary>The window manager.</summary>
        private readonly IWindowManager windowManager;

        /// <summary>Initializes a new instance of the <see cref="NewRepositoryViewModel"/> class.</summary>
        public NewRepositoryViewModel()
        {
            if (Execute.InDesignMode)
            {
                this.Name = "Test Modification";
                this.Version = new SemVersion(0, 1);
                this.Path = @"C:\SkyrimMod";
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NewRepositoryViewModel"/> class.
        /// </summary>
        /// <param name="windowManager">
        /// The window manager.
        /// </param>
        public NewRepositoryViewModel(IWindowManager windowManager)
        {
            this.windowManager = windowManager;
            this.Version = new SemVersion(0);
        }

        /// <summary>Gets or sets the name.</summary>
        public string Name { get; set; }

        /// <summary>Gets or sets the path.</summary>
        public string Path { get; set; }

        /// <summary>Gets or sets the version.</summary>
        public SemVersion Version { get; set; }

        /// <summary>Gets a value indicating whether the user can create a repository.</summary>
        /// <remarks>This property tells if a user should be able to create a new repository. Name and Path are required to be able to crate a new repository.
        /// This property however does not check whether the path is valid altogether! This means an otherwise invalid path, such as c:\\directory\filename is accepted.</remarks>
        public bool CanCreateRepository
        {
            get
            {
                return !string.IsNullOrEmpty(this.Name) && !string.IsNullOrEmpty(this.Path);
            }
        }

        /// <summary>Chooses a path for the repository.</summary>
        public void ChoosePath()
        {
            // Hack: Not implemented in MVVM pattern.
            FolderBrowserDialog fileDialog = new FolderBrowserDialog();

            fileDialog.ShowDialog();

            this.Path = fileDialog.SelectedPath;
        }

        /// <summary>Compiles the repository from the view model.</summary>
        /// <returns>A <see cref="ModRepository" /> containing the view models information.</returns>
        public ModRepository GetRepository()
        {
            return new ModRepository { Name = this.Name, Path = this.Path, Version = this.Version };
        }

        /// <summary>Changes the version of the mod repository.</summary>
        /// <exception cref="NotImplementedException" />
        public void ChangeVersion()
        {
            ChangeVersionViewModel viewModel = new ChangeVersionViewModel(this.Version);

            Dictionary<string, object> settingsDictionary = new Dictionary<string, object>
            {
                { "ResizeMode", ResizeMode.NoResize } 
            };

            bool? answer = this.windowManager.ShowDialog(viewModel, null, settingsDictionary);

            if (answer.HasValue && answer.Value)
            {
                this.Version = viewModel.GetVersion().ToString();
            }
        }

        /// <summary>Creates a repository and closes the screen.</summary>
        public void CreateRepository()
        {
            this.TryClose(true);
        }

        /// <summary>Closes the screen without further processing of information.</summary>
        public void Cancel()
        {
            this.TryClose(false);
        }
    }
}
