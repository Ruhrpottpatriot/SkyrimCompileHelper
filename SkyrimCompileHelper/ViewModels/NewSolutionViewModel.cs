// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NewSolutionViewModel.cs" company="Robert Logiewa">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Robert Logiewa
// </copyright>
// <summary>
//   ViewModel containing methods and properties to create a new solution.
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

    using SkyrimCompileHelper.Core;

    using Screen = Caliburn.Micro.Screen;

    /// <summary>ViewModel containing methods and properties to create a new solution.</summary>
    [ImplementPropertyChanged]
    public class NewSolutionViewModel : Screen
    {
        /// <summary>The window manager.</summary>
        private readonly IWindowManager windowManager;

        /// <summary>Initializes a new instance of the <see cref="NewSolutionViewModel"/> class.</summary>
        public NewSolutionViewModel()
        {
            if (Execute.InDesignMode)
            {
                this.SolutionName = "Test Modification";
                this.Version = new SemVersion(0, 1);
                this.Path = @"C:\SkyrimMod";
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NewSolutionViewModel"/> class.
        /// </summary>
        /// <param name="windowManager">
        /// The window manager.
        /// </param>
        public NewSolutionViewModel(IWindowManager windowManager)
        {
            this.windowManager = windowManager;
            this.Version = new SemVersion(0);
        }

        /// <summary>Gets or sets the name.</summary>
        public string SolutionName { get; set; }

        /// <summary>Gets or sets the path.</summary>
        public string Path { get; set; }

        /// <summary>Gets or sets the version.</summary>
        public SemVersion Version { get; set; }

        /// <summary>Gets a value indicating whether the user can create a solution.</summary>
        /// <remarks>This property tells if a user should be able to create a new solution. Name and Path are required to be able to crate a new solution.
        /// This property however does not check whether the path is valid altogether! This means an otherwise invalid path, such as c:\\directory\filename is accepted.</remarks>
        public bool CanCreateSolution
        {
            get
            {
                return !string.IsNullOrEmpty(this.SolutionName) && !string.IsNullOrEmpty(this.Path);
            }
        }

        /// <summary>Chooses a path for the solution.</summary>
        public void ChoosePath()
        {
            // Hack: Not implemented in MVVM pattern.
            FolderBrowserDialog fileDialog = new FolderBrowserDialog();

            fileDialog.ShowDialog();

            this.Path = fileDialog.SelectedPath;
        }

        /// <summary>Compiles the solution from the view model.</summary>
        /// <returns>A <see cref="Solution" /> containing the view models information.</returns>
        public Solution GetSolution()
        {
            return new Solution { Name = this.SolutionName, Path = this.Path, Version = this.Version };
        }

        /// <summary>Changes the version of the solution.</summary>
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

        /// <summary>Creates a solution and closes the screen.</summary>
        public void CreateSolution()
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
