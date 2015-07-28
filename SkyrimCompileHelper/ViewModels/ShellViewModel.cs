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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    using Caliburn.Micro;

    using PropertyChanged;


    using SkyrimCompileHelper.Common;

    /// <summary>Provides methods and properties to show the shell view.</summary>
    [ImplementPropertyChanged]
    [Export(typeof(ShellViewModel))]
    public sealed class ShellViewModel : PropertyChangedBase
    {
        /// <summary>The window manager.</summary>
        private readonly IWindowManager windowManager;

        /// <summary>The general settings.</summary>
        private readonly SettingsRepository generalSettings;

        /// <summary>Initializes a new instance of the <see cref="ShellViewModel"/> class.</summary>
        public ShellViewModel()
        {
            if (Execute.InDesignMode)
            {
                this.SkyrimPath = @"C:\Skyrim";
                this.OrganizerPath = @"C:Organizer";

                this.Solutions = new ObservableCollection<Solution>
                {
                    new Solution { Name = "Outfits of Skyrim", Version = "0.1.0", Path = @"C:\Outfits" }
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

            this.Solutions = new ObservableCollection<Solution> { new Solution { Name = Constants.AddConst } };
        }

        /// <summary>Gets or sets skyrims path.</summary>
        public string SkyrimPath { get; set; }

        /// <summary>Gets or sets mod organizers path.</summary>
        public string OrganizerPath { get; set; }

        /// <summary>Gets or sets the repositories.</summary>
        public ObservableCollection<Solution> Solutions { get; set; }

        /// <summary>Gets or sets the selected solution.</summary>
        public SolutionViewModel SelectedSolution { get; set; }

        /// <summary>Changes a solution based on the users selection.</summary>
        /// <param name="sender">The ComboBox containing the selection.</param>
        public void ChangeSolution(ComboBox sender)
        {
            if (sender.SelectedItem == null)
            {
                return;
            }

            string solutionName = ((Solution)sender.SelectedItem).Name;

            if (solutionName == Constants.AddConst)
            {
                this.Solutions.Add(this.CreateSolution());

                return;
            }

            Solution solution = this.Solutions.Single(s => s.Name == solutionName);
            this.SelectedSolution = new SolutionViewModel(this.windowManager, solution);
        }

        /// <summary>Opens a new window to create a new solution.</summary>
        /// <returns>The <see cref="Solution"/> created by the user.</returns>
        private Solution CreateSolution()
        {
            NewSolutionViewModel viewModel = new NewSolutionViewModel(this.windowManager);

            Dictionary<string, object> settingsDictionary = new Dictionary<string, object>
            {
                { "ResizeMode", ResizeMode.NoResize } 
            };

            bool? answer = this.windowManager.ShowDialog(viewModel, null, settingsDictionary);

            return answer.HasValue && answer.Value ? viewModel.GetSolution() : null;
        }

        /// <summary>Saves the settings to the repository.</summary>
        public void SaveSettings()
        {
            this.generalSettings.SkyrimPath = this.SkyrimPath;
            this.generalSettings.ModOrganizerPath = this.OrganizerPath;
        }
    }
}
