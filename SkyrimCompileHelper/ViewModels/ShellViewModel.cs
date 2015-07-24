// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShellViewModel.cs" company="Robert Logiewa">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Robert Logiewa
// </copyright>
// <summary>
//   Provides methods and properties to show the shell view.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SkyrimCompileHelper.ViewModels
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.Composition;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

    using Caliburn.Micro;

    using PropertyChanged;

    using Semver;

    using SkyrimCompileHelper.Common;

    /// <summary>Provides methods and properties to show the shell view.</summary>
    [ImplementPropertyChanged]
    [Export(typeof(ShellViewModel))]
    public sealed class ShellViewModel : PropertyChangedBase
    {
        private readonly ModRepository addRepositoryBlueprint = new ModRepository { Name = "Add New Repository..." };

        private readonly IWindowManager windowManager;

        private readonly SettingsRepository generalSettings;

        private string organizerPath;

        /// <summary>Initializes a new instance of the <see cref="ShellViewModel"/> class.</summary>
        public ShellViewModel()
        {
            if (Execute.InDesignMode)
            {
                this.SkyrimPath = @"C:\Skyrim";
                this.OrganizerPath = @"C:Organizer";
                this.SelectedRepository = new ModRepository
                {
                    Name = "Outfits of Skyrim",
                    Version = "0.1.0",
                    Path = @"C:\Outfits"
                };
            }
        }

        /// <summary>Initializes a new instance of the <see cref="ShellViewModel"/> class.</summary>
        /// <param name="windowManager">The window Manager.</param>
        /// <param name="settingsRepository">The settings repository.</param>
        [ImportingConstructor]
        public ShellViewModel(IWindowManager windowManager, SettingsRepository settingsRepository)
        {
            this.windowManager = windowManager;
            this.generalSettings = settingsRepository;
            this.SkyrimPath = this.generalSettings.SkyrimPath;
            this.OrganizerPath = this.generalSettings.ModOrganizerPath;

            this.Repositories = new ObservableCollection<ModRepository> { this.addRepositoryBlueprint };
        }

        /// <summary>Gets or sets skyrims path.</summary>
        public string SkyrimPath { get; set; }

        /// <summary>Gets or sets mod organizers path.</summary>
        public string OrganizerPath { get; set; }

        public ObservableCollection<ModRepository> Repositories { get; set; }

        public ModRepository SelectedRepository { get; set; }

        /// <summary>Adds a repository to the list.</summary>
        public void AddRepository()
        {
            if (this.SelectedRepository.Equals(this.addRepositoryBlueprint))
            {
                NewRepositoryViewModel viewModel = new NewRepositoryViewModel(this.windowManager);

                Dictionary<string, object> settingsDictionary = new Dictionary<string, object>
            {
                { "ResizeMode", ResizeMode.NoResize } 
            };

                bool? answer = this.windowManager.ShowDialog(viewModel, null, settingsDictionary);

                if (answer.HasValue && answer.Value)
                {
                    this.Repositories.Add(viewModel.GetRepository());
                }
            }
        }

        /// <summary>Compiles the repository.</summary>
        /// <exception cref="NotImplementedException" />
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1627:DocumentationTextMustNotBeEmpty", Justification = "Reviewed. Suppression is OK here.")]
        public void Compile()
        {
            throw new NotImplementedException();
        }

        public void ChangeVersion()
        {
            throw new NotImplementedException();
        }

        /// <summary>Saves the settings to the repository.</summary>
        public void SaveSettings()
        {
            this.generalSettings.SkyrimPath = this.SkyrimPath;
            this.generalSettings.ModOrganizerPath = this.OrganizerPath;
        }
    }
}
