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
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    using Caliburn.Micro;

    using Microsoft.Practices.EnterpriseLibrary.Logging;

    using PropertyChanged;

    using SkyrimCompileHelper.Core;

    /// <summary>Provides methods and properties to show the shell view.</summary>
    [ImplementPropertyChanged]
    [Export(typeof(ShellViewModel))]
    public sealed class ShellViewModel : Screen
    {
        /// <summary>The window manager.</summary>
        private readonly IWindowManager windowManager;

        /// <summary>The settings repository.</summary>
        private readonly ISettingsRepository settingsRepository;

        /// <summary>The solution repository.</summary>
        private readonly ISolutionRepository solutionRepository;

        /// <summary>The writer.</summary>
        private readonly LogWriter logWriter;

        /// <summary>The event aggregator.</summary>
        private readonly IEventAggregator eventAggregator;

        /// <summary>Initialises a new instance of the <see cref="ShellViewModel"/> class.</summary>
        public ShellViewModel()
        {
            if (Execute.InDesignMode)
            {
                this.Solutions = new List<Solution>
                {
                    new Solution { Name = "Outfits of Skyrim", Version = "0.1.0", Path = @"C:\Outfits" }
                };
            }
        }

        /// <summary>Initialises a new instance of the <see cref="ShellViewModel"/> class.</summary>
        /// <param name="windowManager">The window Manager.</param>
        /// <param name="settingsRepository">The settings repository.</param>
        /// <param name="solutionRepository">The solution repository.</param>
        /// <param name="logWriter">The log writer.</param>
        /// <param name="eventAggregator">The event Aggregator.</param>
        [ImportingConstructor]
        public ShellViewModel(IWindowManager windowManager, ISettingsRepository settingsRepository, ISolutionRepository solutionRepository, LogWriter logWriter, IEventAggregator eventAggregator)
        {
            this.windowManager = windowManager;
            this.settingsRepository = settingsRepository;
            this.solutionRepository = solutionRepository;
            this.logWriter = logWriter;
            this.eventAggregator = eventAggregator;
            this.DisplayName = "Skyrim Compile Helper";
            this.Settings = new SettingsViewModel(settingsRepository);
            this.Solutions = new List<Solution>();

            foreach (Solution solution in solutionRepository.Read().Values)
            {
                this.Solutions.Add(solution);
            }

            this.Solutions.Add(Constants.EditSolution);
        }

        /// <summary>Gets or sets the repositories.</summary>
        public IList<Solution> Solutions { get; set; }

        /// <summary>Gets or sets the selected solution.</summary>
        public SolutionViewModel SelectedSolution { get; set; }

        /// <summary>Gets or sets the settings.</summary>
        public SettingsViewModel Settings { get; set; }

        /// <summary>Changes a solution based on the users selection.</summary>
        /// <param name="sender">The ComboBox containing the selection.</param>
        public void ChangeSolution(ComboBox sender)
        {
            if (sender.SelectedItem == null)
            {
                return;
            }

            string solutionName = ((Solution)sender.SelectedItem).Name;

            if (solutionName == Constants.EditSolution.Name)
            {
                // Create a copy of the solution list excluding the edit item.
                IEnumerable<Solution> solutions = this.Solutions.Where(s => s != Constants.EditSolution);

                SolutionManagerViewModel viewModel = new SolutionManagerViewModel(this.windowManager, solutions);

                Dictionary<string, object> settingsDictionary = new Dictionary<string, object>
                {
                    { "ResizeMode", ResizeMode.NoResize } 
                };

                bool? answer = this.windowManager.ShowDialog(viewModel, null, settingsDictionary);

                if (answer.HasValue && answer.Value)
                {
                    // Get the solutions from the view model
                    this.Solutions = viewModel.GetSolutions();

                    // Write to the log, that we are updating the solutions
                    LogEntry updatingSolutionsEntry = new LogEntry
                    {
                        EventId = 00207,
                        Title = "Updating Repository",
                        Message = "Updating solutions in the repository with changed information.",
                        Categories = { LoggingConstants.CategoryGeneralConst },
                        Severity = TraceEventType.Information
                    };
                    this.logWriter.Write(updatingSolutionsEntry);

                    // Actually update the solutions
                    this.solutionRepository.Update(new DictionaryRange<string, Solution>(this.Solutions.ToDictionary(s => s.Name, s => s)));

                    // Get the names of the solutions we want to delete
                    IList<string> deletedSolutions = viewModel.DeletedSolutions;

                    // Write to the log, that we are deleting solutions
                    LogEntry deletingSolutionsEntry = new LogEntry
                    {
                        EventId = 00207,
                        Title = "Deleting Solutions",
                        Message = string.Format("Deleting {0} solutions from the file system. Additional information below.", deletedSolutions.Count),
                        Categories = { LoggingConstants.CategoryGeneralConst },
                        Severity = TraceEventType.Information
                    };
                    this.logWriter.Write(deletingSolutionsEntry);

                    foreach (string deletedSolution in deletedSolutions)
                    {
                        LogEntry deletingSolutionEntry = new LogEntry
                        {
                            EventId = 00207,
                            Title = "Deleting Solution",
                            Message = string.Format("Deleting solution \"{0}\" from the file system", deletedSolution),
                            Categories = { LoggingConstants.CategoryGeneralConst },
                            Severity = TraceEventType.Information
                        };
                        this.logWriter.Write(deletingSolutionEntry);
                    }

                    this.solutionRepository.Delete(deletedSolutions);

                    this.Solutions.Add(Constants.EditSolution);
                }

                return;
            }

            LogEntry changedSolutionEntry = new LogEntry
            {
                EventId = 00201,
                Title = "Changed Solution",
                Message = string.Format("The solution was changed to \"{0}\"", solutionName),
                Categories = { LoggingConstants.CategoryGeneralConst },
                Severity = TraceEventType.Information
            };
            this.logWriter.Write(changedSolutionEntry);

            Solution selectedSolution = this.Solutions.Single(s => s.Name == solutionName);
            this.SelectedSolution = new SolutionViewModel(this.windowManager, this.settingsRepository, this.solutionRepository, selectedSolution, this.logWriter, this.eventAggregator);
        }
    }
}
