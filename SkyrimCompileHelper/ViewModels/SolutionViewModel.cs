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
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;

    using Caliburn.Micro;

    using Microsoft.Practices.EnterpriseLibrary.Logging;

    using PapyrusCompiler;

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

        /// <summary>Initialises a new instance of the <see cref="SolutionViewModel"/> class.</summary>
        public SolutionViewModel()
        {
            if (Execute.InDesignMode)
            {
                this.SolutionName = "Outfits of Skyrim";
                this.Version = new SemVersion(0, 1);
                this.FlagsFile = "Flags";
                this.SolutionPath = @"C:\Test";
                this.Configurations = new List<CompileConfiguration>
                {
                    new CompileConfiguration { Name = "Debug" },
                    new CompileConfiguration { Name = "Release" },
                    new CompileConfiguration { Name = Constants.EditConst }
                };
            }
        }

        /// <summary>Initialises a new instance of the <see cref="SolutionViewModel"/> class.</summary>
        /// <param name="windowManager">The window manager.</param>
        /// <param name="settingsRepository">The settings repository.</param>
        /// <param name="solutionRepository">The solution repository.</param>
        /// <param name="solution">The solution to work with.</param>
        /// <param name="logWriter">The log writer.</param>
        public SolutionViewModel(IWindowManager windowManager, ISettingsRepository settingsRepository, ISolutionRepository solutionRepository, Solution solution, LogWriter logWriter)
        {
            // Initilaize readonly fields
            this.windowManager = windowManager;
            this.settingsRepository = settingsRepository;
            this.solutionRepository = solutionRepository;
            this.logWriter = logWriter;

            // Init the parameters
            this.SolutionName = solution.Name;
            this.SolutionPath = solution.Path;
            this.Version = solution.Version;
            this.CompilerAll = true;

            // Initialize the configuration parameters
            this.IntConfigParameter(solution);
        }

        /// <summary>Gets or sets the solution name.</summary>
        public string SolutionName { get; set; }

        /// <summary>Gets or sets the solution path.</summary>
        public string SolutionPath { get; set; }

        /// <summary>Gets or sets the solution version.</summary>
        public SemVersion Version { get; set; }

        /// <summary>Gets or sets the compile configurations.</summary>
        public IList<CompileConfiguration> Configurations { get; set; }

        /// <summary>Gets or sets the selected configuration.</summary>
        public CompileConfiguration SelectedConfiguration { get; set; }

        /// <summary>Gets or sets the compiler flags.</summary>
        public string FlagsFile { get; set; }

        /// <summary>Gets or sets a value indicating whether the compiler should compile a whole directory.</summary>
        public bool CompilerAll { get; set; }

        /// <summary>Gets or sets a value indicating whether the compiler should supress output.</summary>
        public bool CompilerQuiet { get; set; }

        /// <summary>Gets or sets a value indicating whether the compiler should print debug information.</summary>
        public bool CompilerDebug { get; set; }

        /// <summary>Gets or sets a value indicating whether the source files should be copied after a compile.</summary>
        public bool CopySourceFiles { get; set; }

        /// <summary>Gets or sets a value indicating whether the compiler should optimize the script files.</summary>
        public bool CompilerOptimize { get; set; }

        /// <summary>Gets or sets the selected assembly option.</summary>
        public AssemblyOption SelectedAssemblyOption { get; set; }

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

            this.SaveSolution();
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

                this.Configurations.Add(new CompileConfiguration { Name = Constants.EditConst });

                this.SaveSolution();

                return;
            }

            CompileConfiguration configuration = this.Configurations.SingleOrDefault(c => c.Name == configurationName);
            if (configuration != null)
            {
                this.CompilerAll = configuration.All;
                this.CompilerDebug = configuration.Debug;
                this.CompilerOptimize = configuration.Optimize;
                this.SelectedAssemblyOption = configuration.AssemblyOption;
                this.CompilerQuiet = configuration.Quiet;
                this.FlagsFile = configuration.FlagFile;
            }

            this.SelectedConfiguration = configuration;

            this.SaveSolution();
        }

        /// <summary>Saves a configuration to the repository.</summary>
        public void SaveConfiguration()
        {
            CompileConfiguration compConfig = this.SelectedConfiguration;

            compConfig.All = this.CompilerAll;
            compConfig.AssemblyOption = this.SelectedAssemblyOption;
            compConfig.Debug = this.CompilerDebug;
            compConfig.FlagFile = this.FlagsFile;
            compConfig.Optimize = this.CompilerOptimize;
            compConfig.Quiet = this.CompilerQuiet;

            this.SelectedConfiguration = compConfig;
            this.Configurations.Remove(compConfig);
            this.Configurations.Add(compConfig);
            this.SaveSolution();
        }

        /// <summary>Compiles the source files with the selected configuration.</summary>
        /// <exception cref="NotImplementedException">Not yet implemented.</exception>
        public void Compile()
        {
            string skyrimPath = this.settingsRepository.Read()["SkyrimPath"].ToString();

            IEnumerable<string> inputFolders = new List<string>
            {
                 Path.Combine(skyrimPath, @"Data\Scripts\Source")
            };

            ICompilerFactory compilerFactory = new CompilerFactory(this.settingsRepository.Read()["SkyrimPath"].ToString(), this.logWriter)
            {
                Flags = this.FlagsFile,
                ImportFolders = inputFolders.ToList(),
                CompilerTarget = Path.Combine(this.SolutionPath, "src"),
                OutputFolder = Path.Combine(this.SolutionPath, "bin", this.SelectedConfiguration.Name, "scripts"),
                All = this.CompilerAll,
                Quiet = this.CompilerQuiet,
                Debug = this.CompilerDebug,
                Optimize = this.CompilerOptimize,
                AssemblyOptions = this.SelectedAssemblyOption
            };

            int build = Convert.ToInt32(string.IsNullOrEmpty(this.Version.Build) ? "0" : this.Version.Build) + 1;
            this.Version = this.Version.Change(build: build.ToString());

            compilerFactory.Compile();
            this.MoveCompileFiles();
        }

        /// <summary>Cleans the output folder from any leftover files.</summary>
        public void CleanOutputFolders()
        {
            string modOrganizerSolutionPath = Path.Combine(this.settingsRepository.Read()["ModOrganizerPath"].ToString(), "mods", this.SolutionName);
            if (Directory.Exists(modOrganizerSolutionPath))
            {
                Directory.Delete(modOrganizerSolutionPath, true);
            }

            Directory.CreateDirectory(modOrganizerSolutionPath);

            string binPath = Path.Combine(this.SolutionPath, "bin", this.SelectedConfiguration.Name);
            if (Directory.Exists(binPath))
            {
                Directory.Delete(binPath, true);
            }

            Directory.CreateDirectory(binPath);
        }

        /// <summary>Handles the message.</summary>
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
            // Move the optimize files from, the application folder to the solution folder where they belong.
            string appPath = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));

            if (appPath != null)
            {
                foreach (string file in Directory.GetFiles(appPath, "*.dot", SearchOption.TopDirectoryOnly))
                {
                    string fileName = Path.GetFileName(file);

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        string destination = Path.Combine(this.SolutionPath, "bin", this.SelectedConfiguration.Name, fileName);

                        if (File.Exists(destination))
                        {
                            File.Delete(destination);
                        }

                        File.Move(file, destination);
                    }
                }
            }

            // Now we check if the user wants to copy the script source files and copy them if necessary.
            if (this.CopySourceFiles)
            {
                this.CopyFilesWithSubFolders(Path.Combine(this.SolutionPath, @"src\scripts"), Path.Combine(this.SolutionPath, "bin", this.SelectedConfiguration.Name, @"scripts\source"), true);
            }

            // Lastly we copy the whole configuration folder to the ModOrganizerFolder
            this.CopyFilesWithSubFolders(Path.Combine(this.SolutionPath, "bin", this.SelectedConfiguration.Name), Path.Combine(this.settingsRepository.Read()["ModOrganizerPath"].ToString(), "mods", this.SolutionName), true);
        }

        /// <summary>Copies the content of a folder with the contents of all the sub-folders from one path to another.</summary>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="destinationPath">The destination path.</param>
        /// <param name="overwrite">True, if existing files should be overwritten, otherwise false.</param>
        private void CopyFilesWithSubFolders(string sourcePath, string destinationPath, bool overwrite = false)
        {
            foreach (string directoryPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(directoryPath.Replace(sourcePath, destinationPath));
            }

            foreach (string filePath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(filePath, filePath.Replace(sourcePath, destinationPath), overwrite);
            }
        }

        /// <summary>Saves the selected solution to the solution repository.</summary>
        private void SaveSolution()
        {
            IDictionaryRange<string, Solution> solutions = new DictionaryRange<string, Solution>
            {
                { 
                    this.SolutionName, new Solution
                    {
                        Name = this.SolutionName,
                        Path = this.SolutionPath,
                        Version = this.Version,
                        CompileConfigurations = this.Configurations.Where(c => c.Name != Constants.EditConst).ToList(),
                        SelectedConfiguration = this.SelectedConfiguration != null ? this.SelectedConfiguration.Name : string.Empty
                    }
                }
            };

            this.solutionRepository.Update(solutions);
        }

        /// <summary>Initialises the config parameters.</summary>
        /// <param name="solution">The selected solution.</param>
        private void IntConfigParameter(Solution solution)
        {
            this.Configurations = solution.CompileConfigurations ?? new List<CompileConfiguration>();
            this.Configurations.Add(new CompileConfiguration { Name = Constants.EditConst });
            this.SelectedConfiguration = this.Configurations.SingleOrDefault(c => c.Name == solution.SelectedConfiguration);

            // If there is no selected configuration, we do nothing.
            if (this.SelectedConfiguration == null)
            {
                return;
            }

            this.CompilerAll = this.SelectedConfiguration.All;
            this.CompilerDebug = this.SelectedConfiguration.Debug;
            this.CompilerOptimize = this.SelectedConfiguration.Optimize;
            this.CompilerQuiet = this.SelectedConfiguration.Quiet;
            this.FlagsFile = this.SelectedConfiguration.FlagFile;
            this.SelectedAssemblyOption = this.SelectedConfiguration.AssemblyOption;
        }
    }
}
