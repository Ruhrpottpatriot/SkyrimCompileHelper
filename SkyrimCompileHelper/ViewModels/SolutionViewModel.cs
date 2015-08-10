// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SolutionViewModel.cs" company="Robert Logiewa">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Robert Logiewa
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SkyrimCompileHelper.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    using Caliburn.Micro;

    using Microsoft.Practices.EnterpriseLibrary.Logging;

    using PropertyChanged;

    using Semver;

    using SkyrimCompileHelper.Common;
    using SkyrimCompileHelper.Compiler;

    /// <summary>ViewModel containing methods and properties to work with a solution.</summary>
    [ImplementPropertyChanged]
    public class SolutionViewModel : PropertyChangedBase
    {
        /// <summary>The window manager.</summary>
        private readonly IWindowManager windowManager;

        /// <summary>The settings repository.</summary>
        private readonly ISettingsRepository settingsRepository;

        /// <summary>The solution repository.</summary>
        private readonly ISolutionRepository solutionRepository;

        /// <summary>The log writer.</summary>
        private readonly LogWriter logWriter;

        /// <summary>Initializes a new instance of the <see cref="SolutionViewModel"/> class.</summary>
        public SolutionViewModel()
        {
            if (Execute.InDesignMode)
            {
                this.SolutionName = "Outfits of Skyrim";
                this.Version = new SemVersion(0, 1);
                this.CompilerFlags = "Flags";
                this.SolutionPath = @"C:\Test";
                this.Configurations = new List<CompileConfiguration>
                {
                    new CompileConfiguration { Name = "Debug" },
                    new CompileConfiguration { Name = "Release" },
                    new CompileConfiguration { Name = Constants.EditConst }
                };
            }
        }

        /// <summary>Initializes a new instance of the <see cref="SolutionViewModel"/> class.</summary>
        /// <param name="windowManager">The window manager.</param>
        /// <param name="settingsRepository">The settings repository.</param>
        /// <param name="solutionRepository">The solution repository.</param>
        /// <param name="solution">The solution to work with.</param>
        /// <param name="logWriter">The log writer.</param>
        public SolutionViewModel(IWindowManager windowManager, ISettingsRepository settingsRepository, ISolutionRepository solutionRepository, Solution solution, LogWriter logWriter)
        {
            this.windowManager = windowManager;
            this.settingsRepository = settingsRepository;
            this.solutionRepository = solutionRepository;
            this.logWriter = logWriter;
            this.Configurations = solution.CompileConfigurations ?? new List<CompileConfiguration>();
            this.Configurations.Add(new CompileConfiguration { Name = Constants.EditConst });
            this.SolutionName = solution.Name;
            this.SolutionPath = solution.Path;
            this.Version = solution.Version;
        }

        /// <summary>Gets or sets the solution name.</summary>
        public string SolutionName { get; set; }

        /// <summary>Gets or sets the solution path.</summary>
        public string SolutionPath { get; set; }

        /// <summary>Gets or sets the solution version.</summary>
        public SemVersion Version { get; set; }

        /// <summary>Gets or sets the compiler flags.</summary>
        public string CompilerFlags { get; set; }

        /// <summary>Gets or sets the compile configurations.</summary>
        public IList<CompileConfiguration> Configurations { get; set; }

        /// <summary>Gets or sets the selected configuration.</summary>
        public CompileConfiguration SelectedConfiguration { get; set; }

        /// <summary>Opens the solution folder in the windows explorer.</summary>
        /// <exception cref="NotImplementedException">Not yet implemented</exception>
        public void OpenSolutionFolder()
        {
            try
            {
                Process.Start(this.SolutionPath);
            }
            catch (Exception)
            {
                MessageBox.Show("Could not open solution directory. Please check, if the path is valid.");
            }
        }

        /// <summary>Changes the version of a solution.</summary>
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
                this.Version = viewModel.GetVersion();
            }
        }

        /// <summary>Changes the compile configuration for the current solution.</summary>
        /// <param name="sender">The <see cref="ComboBox"/> that holds the selected item.</param>
        /// <exception cref="NotImplementedException">Thrown when the configuration manger is selected.</exception>
        public void ChangeConfiguration(ComboBox sender)
        {
            if (sender.SelectedItem == null)
            {
                return;
            }

            string configurationName = ((CompileConfiguration)sender.SelectedItem).Name;

            if (configurationName == Constants.EditConst)
            {
                this.OpenConfigurationManager();

                if (this.Configurations.All(c => c.Name != Constants.EditConst))
                {
                    this.Configurations.Add(new CompileConfiguration { Name = Constants.EditConst });
                }

                return;
            }

            CompileConfiguration configuration = this.Configurations.SingleOrDefault(c => c.Name == configurationName);
            this.CompilerFlags = configuration != null ? configuration.CompilerFlags : string.Empty;
            this.SaveSolution();
        }

        /// <summary>Compiles the source files with the selected configuration.</summary>
        /// <exception cref="NotImplementedException">Not yet implemented.</exception>
        public void Compile()
        {
            IEnumerable<string> inputFolders = new List<string>
            {
                 Path.Combine(this.SolutionPath, "src")   
            };

            CompilerFactory compilerFactory = new CompilerFactory(this.settingsRepository.Read()["SkyrimPath"].ToString(), this.logWriter)
            {
                Flags = this.CompilerFlags,
                InputFolders = inputFolders.ToList(),
                OutputFolder = Path.Combine(this.SolutionPath, "bin", this.SelectedConfiguration.Name),
                All = true
            };

            int build = Convert.ToInt32(string.IsNullOrEmpty(this.Version.Build) ? "0" : this.Version.Build) + 1;
            this.Version = this.Version.Change(build: build.ToString());

            compilerFactory.Compile();
        }

        /// <summary>Saves the selected solution to the solution repository.</summary>
        private void SaveSolution()
        {
            IDictionaryRange<string, Solution> solutions = new DictionaryRange<string, Solution>
            {
                {
                    this.SolutionName,
                    new Solution
                    {
                        Name = this.SolutionName,
                        Path = this.SolutionPath,
                        Version = this.Version,
                        CompileConfigurations = this.Configurations.Where(c => c.Name != Constants.EditConst).ToList(),
                        SelectedConfiguration = this.SelectedConfiguration
                    }
                }
            };

            this.solutionRepository.Update(solutions);
        }

        /// <summary>Opens the configuration manager.</summary>
        private void OpenConfigurationManager()
        {
            IEnumerable<CompileConfiguration> configurations = this.Configurations.Where(c => c.Name != Constants.EditConst);

            ConfigurationManagerViewModel viewModel = new ConfigurationManagerViewModel(this.windowManager, configurations.ToList());

            Dictionary<string, object> settingsDictionary = new Dictionary<string, object>
            {
                { "ResizeMode", ResizeMode.NoResize }
            };

            bool? answer = this.windowManager.ShowDialog(viewModel, null, settingsDictionary);

            if (answer.HasValue && answer.Value)
            {
                this.Configurations = viewModel.Configurations;
            }
        }
    }
}
