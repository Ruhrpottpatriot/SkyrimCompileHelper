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
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;

    using Caliburn.Micro;

    using Microsoft.Practices.EnterpriseLibrary.Logging;

    using PropertyChanged;

    using Semver;

    using SkyrimCompileHelper.Compiler;
    using SkyrimCompileHelper.Core;
    using SkyrimCompileHelper.Core.EventHandles;

    /// <summary>ViewModel containing methods and properties to work with a solution.</summary>
    [ImplementPropertyChanged]
    public class SolutionViewModel : PropertyChangedBase, IHandle<SaveSolutionEvenHandle>
    {
        /// <summary>The window manager.</summary>
        private readonly IWindowManager windowManager;

        /// <summary>The settings repository.</summary>
        private readonly ISettingsRepository settingsRepository;

        /// <summary>The solution repository.</summary>
        private readonly ISolutionRepository solutionRepository;

        /// <summary>The log writer.</summary>
        private readonly LogWriter logWriter;

        /// <summary>The event aggregator.</summary>
        private readonly IEventAggregator eventAggregator;

        /// <summary>Stores the old solution name, so the repository is able to properly delete it after a rename.</summary>
        private string oldSolutionName;

        /// <summary>Initialises a new instance of the <see cref="SolutionViewModel"/> class.</summary>
        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Used for Caliburn.Micro design time support.")]
        public SolutionViewModel()
        {
            if (Execute.InDesignMode)
            {
                this.SolutionName = "Outfits of Skyrim";
                this.Version = new SemVersion(0, 1);
                this.SolutionPath = @"C:\Test";
                this.Configurations = new List<CompileConfiguration>
                {
                    new CompileConfiguration { Name = "Debug" },
                    new CompileConfiguration { Name = "Release" },
                    new CompileConfiguration { Name = Constants.EditConst }
                };

                this.ConfigurationView = new ConfigurationViewModel();
                this.ImportFolderView = new ImportFolderViewModel();
            }
        }

        /// <summary>Initialises a new instance of the <see cref="SolutionViewModel"/> class.</summary>
        /// <param name="windowManager">The window manager.</param>
        /// <param name="settingsRepository">The settings repository.</param>
        /// <param name="solutionRepository">The solution repository.</param>
        /// <param name="solution">The solution to work with.</param>
        /// <param name="logWriter">The log writer.</param>
        /// <param name="eventAggregator">The event Aggregator.</param>
        public SolutionViewModel(IWindowManager windowManager, ISettingsRepository settingsRepository, ISolutionRepository solutionRepository, Solution solution, LogWriter logWriter, IEventAggregator eventAggregator)
        {
            // Subscribe to the event aggregator
            eventAggregator.Subscribe(this);
            this.eventAggregator = eventAggregator;

            // Initialise readonly fields
            this.windowManager = windowManager;
            this.settingsRepository = settingsRepository;
            this.solutionRepository = solutionRepository;
            this.logWriter = logWriter;

            // Set the solution parameters
            this.SolutionName = solution.Name;
            this.oldSolutionName = solution.Name;
            this.SolutionPath = solution.Path;
            this.Version = solution.Version;

            // Set the configuration parameters
            this.Configurations = solution.CompileConfigurations ?? new List<CompileConfiguration>();
            this.Configurations.Add(new CompileConfiguration { Name = Constants.EditConst });
            this.SelectedConfiguration = solution.SelectedConfiguration;

            // Intialise sub ViewModels
            this.ImportFolderView = new ImportFolderViewModel(windowManager, eventAggregator);
            this.ConfigurationView = new ConfigurationViewModel(eventAggregator, this.Configurations.SingleOrDefault(c => c.Name == this.SelectedConfiguration));
        }

        /// <summary>Gets or sets the import folder view.</summary>
        public ImportFolderViewModel ImportFolderView { get; set; }

        /// <summary>Gets or sets the configuration view.</summary>
        public ConfigurationViewModel ConfigurationView { get; set; }

        /// <summary>Gets or sets the solution name.</summary>
        public string SolutionName { get; set; }

        /// <summary>Gets or sets the solution path.</summary>
        public string SolutionPath { get; set; }

        /// <summary>Gets or sets the solution version.</summary>
        public SemVersion Version { get; set; }

        /// <summary>Gets or sets the compile configurations.</summary>
        public IList<CompileConfiguration> Configurations { get; set; }

        /// <summary>Gets or sets the selected configuration.</summary>
        public string SelectedConfiguration { get; set; }

        /// <summary>Changes the compile configuration for the current solution.</summary>
        /// <param name="sender">The <see cref="ComboBox"/> that holds the selected item.</param>
        public void ChangeConfiguration(ComboBox sender)
        {
            // If the selected item is null, we do nothing.
            if (sender.SelectedItem == null)
            {
                return;
            }

            // Get the name of the selected compile configuration
            string configurationName = ((CompileConfiguration)sender.SelectedItem).Name;

            // If then name is equal to <edit...> then we open the configuration manager
            if (configurationName == Constants.EditConst)
            {
                // Filter out the <edit...> item so we don't pass it over to the manager
                var filteredConfigurations = this.Configurations.Where(c => c.Name != Constants.EditConst).ToList();

                // Create a new ViewModel
                var viewModel = new ConfigurationManagerViewModel(this.windowManager, filteredConfigurations);

                // Get how the window was closed
                bool? answer = this.windowManager.ShowDialog(viewModel);

                // If the window was closed with "true" we want to handle the data coming back
                if (answer.HasValue && answer.Value)
                {
                    this.Configurations = viewModel.Configurations;
                }

                // Since we just replaced the whole configurations list, we need to re add the <edit...> item
                this.Configurations.Add(new CompileConfiguration { Name = Constants.EditConst });

                // After adding a new configuration to the solution we need to save it.
                this.SaveSolution();

                // We don't want to do anything more, so we return.
                return;
            }

            // If the user didn't select the <edit...> item, we simply set the new selected configuration
            this.SelectedConfiguration = configurationName;

            // After that we want to show the user the configuration view
            this.ConfigurationView = new ConfigurationViewModel(this.eventAggregator, this.Configurations.Single(c => c.Name == this.SelectedConfiguration));

            // Again, we want to save the changes in the solution.
            this.SaveSolution();
        }

        /// <summary>Saves the selected solution to the solution repository.</summary>
        public void SaveSolution()
        {
            // Generate solution object with the most basic details we know we have.
            Solution sln = new Solution
            {
                Path = this.SolutionPath,
                Name = this.SolutionName,
                Version = this.Version
            };

            // If there is no configuration selected the user didn't change anything,
            // thus we can savely assume that nothing changed
            if (string.IsNullOrEmpty(this.SelectedConfiguration))
            {
                sln.CompileConfigurations = this.Configurations;
            }
            else
            {
                sln.SelectedConfiguration = this.SelectedConfiguration;

                var config = this.GenerateConfigurationDetails();
                config.Name = this.SelectedConfiguration;
                this.Configurations.Remove(config);
                this.Configurations.Add(config);
                sln.CompileConfigurations = this.Configurations;
            }

            // Now we need to pass the solution to the repository. Since it only accepts DictionaryRanges
            // we need to wrap the solution.
            IDictionaryRange<string, Solution> solutions = new DictionaryRange<string, Solution>();
            solutions.Add(this.SolutionName, sln);

            // We also need to check if the solution ame changed and handle it properly.
            if (this.oldSolutionName != this.SolutionName)
            {
                this.solutionRepository.Delete(new List<string> { this.oldSolutionName });
                this.solutionRepository.Create(solutions);
                this.oldSolutionName = sln.Name;
            }
            else
            {
                this.solutionRepository.Update(solutions);
            }
        }

        /// <summary>Changes the version of a solution.</summary>
        public void ChangeVersion()
        {
            // Create the ViewModel
            ChangeVersionViewModel viewModel = new ChangeVersionViewModel(this.Version);

            // Get how the window was closed
            bool? answer = this.windowManager.ShowDialog(viewModel);

            // Do a switch based on the closed value.
            if (answer.HasValue && answer.Value)
            {
                this.Version = viewModel.GetVersion();
            }

            // Since the version changed we need to save the solution.
            this.SaveSolution();
        }

        /// <summary>Opens the solution folder in the windows explorer.</summary>
        /// <exception cref="NotImplementedException">Not yet implemented</exception>
        public void OpenSolutionFolder()
        {
            // Since we don't know if the path is valid, we have to be careful of exceptions
            try
            {
                Process.Start(this.SolutionPath);
            }
            catch (Exception)
            {
                MessageBox.Show("Could not open solution directory. Please check, if the path is valid.");
            }
        }

        /// <summary>Cleans the output folder from any leftover files.</summary>
        public void CleanOutputFolders()
        {
            // Get the path of the mod inside the ModOrganizer installation folder,
            string modOrganizerSolutionPath = Path.Combine(this.settingsRepository.Read()["ModOrganizerPath"].ToString(), "mods", this.SolutionName);
            
            // If the directory exists, we delete it.
            if (Directory.Exists(modOrganizerSolutionPath))
            {
                Directory.Delete(modOrganizerSolutionPath, true);
            }

            // Since the directory either does not exist, or was recently deleted we need to recreate it
            Directory.CreateDirectory(modOrganizerSolutionPath);

            // Now the only folder remaining is the bin folder inside the repository folder.
            string binPath = Path.Combine(this.SolutionPath, "bin", this.SelectedConfiguration);

            // Again, check if the folder does exist, we delete it
            if (Directory.Exists(binPath))
            {
                Directory.Delete(binPath, true);
            }

            // And again, recreate the folder
            Directory.CreateDirectory(binPath);
        }

        /// <summary>Compiles the source files with the selected configuration.</summary>
        /// <exception cref="NotImplementedException">Not yet implemented.</exception>
        public void Compile()
        {
            // Get the list of input folders the user added himself
            IList<string> inputFolders = new List<string>(this.ImportFolderView.ImportFolders.Select(f => f.FolderPath));
            
            // We always want to add skyrims data folder, if only for the flags file.
            inputFolders.Add(Path.Combine(this.settingsRepository.Read()["SkyrimPath"].ToString(), @"Data\Scripts\Source"));

            // Create a new Compiler factory and pass down the parameters
            ICompilerFactory compilerFactory = new CompilerFactory(this.settingsRepository.Read()["SkyrimPath"].ToString(), this.logWriter)
            {
                Flags = this.ConfigurationView.FlagsFile,
                ImportFolders = inputFolders,
                CompilerTarget = Path.Combine(this.SolutionPath, "src"),
                OutputFolder = Path.Combine(this.SolutionPath, "bin", this.SelectedConfiguration, "scripts"),
                All = this.ConfigurationView.All,
                Quiet = this.ConfigurationView.Quiet,
                Debug = this.ConfigurationView.Debug,
                Optimize = this.ConfigurationView.Optimize,
                AssemblyOptions = this.ConfigurationView.SelectedAssemblyOption
            };

            // Increase the build by one per compile
            int build = Convert.ToInt32(string.IsNullOrEmpty(this.Version.Build) ? "0" : this.Version.Build) + 1;
            this.Version = this.Version.Change(build: build.ToString());

            // Compile and move
            compilerFactory.Compile();
            this.MoveCompileFiles();
        }
        
        /// <summary>Handle the save solution messages.</summary>
        /// <param name="message">The message.</param>
        public void Handle(SaveSolutionEvenHandle message)
        {
            this.SaveSolution();
        }

        /// <summary>Moves the compiled script and remaining source files from the compilation folder to the ModOrganizer folder.</summary>
        /// <remarks>This method copies the content of the selected binary folder of the selected configuration to the mod organizer folder.
        /// To copy all relevant files this method first copies the optimize files (if existing) to the bin folder of the selected configuration.
        /// After that the script source files are copied to the bin folder (if selected). At the end the content of the folder is copied to the
        /// ModOrganizer folder.</remarks>
        private void MoveCompileFiles()
        {
            // Since the compiler places the optimise files in the application folder, we need to get the path of it
            string appPath = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));

            // Since Path.GetDirectoryName() can be null, we need watch out for that
            if (appPath != null)
            {
                // Get all the files with the .dot extension inside the app topmost folder so we can move it
                foreach (string file in Directory.GetFiles(appPath, "*.dot", SearchOption.TopDirectoryOnly))
                {
                    string fileName = Path.GetFileName(file);

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        // Get the complete path for the destination
                        string destination = Path.Combine(this.SolutionPath, "bin", this.SelectedConfiguration, fileName);

                        // If the file exists, we want to delete it first
                        if (File.Exists(destination))
                        {
                            File.Delete(destination);
                        }

                        // After we made sure, that the destinationfile does not exist, 
                        // we can safely mofe the file we want to move
                        File.Move(file, destination);
                    }
                }
            }

            // Check if the user wants to copy the source script files
            if (this.ConfigurationView.CopySourceFiles)
            {
                // Copy the contents of the src folder
                this.CopyFilesWithSubFolders(Path.Combine(this.SolutionPath, @"src\scripts"), Path.Combine(this.SolutionPath, "bin", this.SelectedConfiguration, @"scripts\source"), true);
            }

            // Lastly we copy the whole configuration folder to the ModOrganizerFolder
            this.CopyFilesWithSubFolders(Path.Combine(this.SolutionPath, "bin", this.SelectedConfiguration), Path.Combine(this.settingsRepository.Read()["ModOrganizerPath"].ToString(), "mods", this.SolutionName), true);
        }
        
        /// <summary>Copies the content of a folder with the contents of all the sub-folders from one path to another.</summary>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="destinationPath">The destination path.</param>
        /// <param name="overwrite">True, if existing files should be overwritten, otherwise false.</param>
        private void CopyFilesWithSubFolders(string sourcePath, string destinationPath, bool overwrite = false)
        {
            // Since File.Copy() does not create any foders, we need to create them ourselves.
            foreach (string directoryPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(directoryPath.Replace(sourcePath, destinationPath));
            }

            // Now we copy all files from their origin to destination, overwriting if the flag is set.
            foreach (string filePath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(filePath, filePath.Replace(sourcePath, destinationPath), overwrite);
            }
        }

        /// <summary>Generates the configuration details from the selected configuration.</summary>
        /// <returns>A <see cref="CompileConfiguration" />.</returns>
        private CompileConfiguration GenerateConfigurationDetails()
        {
            return new CompileConfiguration
            {
                All = this.ConfigurationView.All,
                AssemblyOption = this.ConfigurationView.SelectedAssemblyOption,
                Debug = this.ConfigurationView.Debug,
                FlagFile = this.ConfigurationView.FlagsFile,
                Optimize = this.ConfigurationView.Optimize,
                Quiet = this.ConfigurationView.Quiet,
                ImportFolders = this.ImportFolderView.ImportFolders
            };
        }
    }
}
