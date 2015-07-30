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
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    using Caliburn.Micro;

    using PropertyChanged;

    using SkyrimCompileHelper.Common;
    using SkyrimCompileHelper.Repositories;

    /// <summary>Provides methods and properties to show the shell view.</summary>
    [ImplementPropertyChanged]
    [Export(typeof(ShellViewModel))]
    public sealed class ShellViewModel : PropertyChangedBase
    {
        /// <summary>The window manager.</summary>
        private readonly IWindowManager windowManager;

        /// <summary>The settings repository.</summary>
        private readonly ISettingsRepository settingsRepository;

        /// <summary>Initializes a new instance of the <see cref="ShellViewModel"/> class.</summary>
        public ShellViewModel()
        {
            if (Execute.InDesignMode)
            {
                this.SkyrimPath = @"C:\Skyrim";
                this.OrganizerPath = @"C:\Organizer";

                this.Solutions = new List<Solution>
                {
                    new Solution { Name = "Outfits of Skyrim", Version = "0.1.0", Path = @"C:\Outfits" }
                };
            }
        }

        /// <summary>Initializes a new instance of the <see cref="ShellViewModel"/> class.</summary>
        /// <param name="windowManager">The window Manager.</param>
        /// <param name="settingsRepository">The settings repository.</param>
        [ImportingConstructor]
        public ShellViewModel(IWindowManager windowManager, ISettingsRepository settingsRepository)
        {
            this.windowManager = windowManager;
            this.settingsRepository = settingsRepository;

            this.SkyrimPath = settingsRepository.Read()["SkyrimPath"].ToString();
            this.OrganizerPath = settingsRepository.Read()["ModOrganizerPath"].ToString();
            
            this.Solutions = new List<Solution> { new Solution { Name = Constants.EditConst } };
        }

        /// <summary>Gets or sets skyrims path.</summary>
        public string SkyrimPath { get; set; }

        /// <summary>Gets or sets mod organizers path.</summary>
        public string OrganizerPath { get; set; }

        /// <summary>Gets or sets the repositories.</summary>
        public IList<Solution> Solutions { get; set; }

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

            if (solutionName == Constants.EditConst)
            {
                // Create a copy of the solution list excluding the edit item.
                IList<Solution> solutions = this.Solutions.Where(s => s.Name != Constants.EditConst).ToList();

                SolutionManagerViewModel viewModel = new SolutionManagerViewModel(this.windowManager, solutions);

                Dictionary<string, object> settingsDictionary = new Dictionary<string, object>
                {
                    { "ResizeMode", ResizeMode.NoResize } 
                };

                bool? answer = this.windowManager.ShowDialog(viewModel, null, settingsDictionary);

                if (answer.HasValue && answer.Value)
                {
                    this.Solutions = viewModel.GetSolutions();
                    this.Solutions.Add(new Solution { Name = Constants.EditConst });
                }

                return;
            }

            Solution selectedSolution = this.Solutions.Single(s => s.Name == solutionName);
            this.SelectedSolution = new SolutionViewModel(this.windowManager, selectedSolution);
        }

        /// <summary>Saves the settings to the repository.</summary>
        public void SaveSettings()
        {
            var settings = new DictionaryRange<string, object>
            {
                { "SkyrimPath", this.SkyrimPath },
                { "ModOrganizerPath", this.OrganizerPath }
            };

            this.settingsRepository.Update(settings);
        }
    }
}
