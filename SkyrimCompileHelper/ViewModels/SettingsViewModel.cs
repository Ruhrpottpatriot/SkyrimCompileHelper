// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsViewModel.cs" company="Robert Logiewa">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Robert Logiewa
// </copyright>
// <summary>
//   Provides methods and properties to show the settings view.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SkyrimCompileHelper.ViewModels
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Windows.Forms;

    using Caliburn.Micro;

    using PropertyChanged;

    using SkyrimCompileHelper.Core;

    /// <summary>Provides methods and properties to show the settings view.</summary>
    [ImplementPropertyChanged]
    public class SettingsViewModel
    {
        /// <summary>Holds a reference to the settings repository.</summary>
        private readonly ISettingsRepository settingsRepository;

        /// <summary>Initialises a new instance of the <see cref="SettingsViewModel"/> class.</summary>
        public SettingsViewModel()
        {
            if (Execute.InDesignMode)
            {
                this.SkyrimPath = @"C:\Skyrim";
                this.OrganizerPath = @"C:\Organizer";
            }
        }

        /// <summary>Initialises a new instance of the <see cref="SettingsViewModel"/> class.</summary>
        /// <param name="settingsRepository">The settings repository.</param>
        public SettingsViewModel(ISettingsRepository settingsRepository)
        {
            this.settingsRepository = settingsRepository;

            this.SkyrimPath = this.settingsRepository.Read()["SkyrimPath"].ToString();
            this.OrganizerPath = this.settingsRepository.Read()["ModOrganizerPath"].ToString();
        }

        /// <summary>Gets or sets skyrims path.</summary>
        public string SkyrimPath { get; set; }

        /// <summary>Gets a value indicating whether Skyrim is installed.</summary>
        public bool SkyrimInstalled
        {
            get
            {
                return File.Exists(Path.Combine(this.SkyrimPath, "SkyrimLauncher.exe"));
            }
        }

        /// <summary>Gets a value indicating whether the compiler is installed.</summary>
        public bool CompilerInstalled
        {
            get
            {
                if (!this.SkyrimInstalled)
                {
                    return false;
                }

                return File.Exists(Path.Combine(this.SkyrimPath, @"Papyrus Compiler\PCompiler.dll"));
            }
        }

        public bool AssemblerInstalled
        {
            get
            {
                if (!this.SkyrimInstalled)
                {
                    return false;
                }

                return File.Exists(Path.Combine(this.SkyrimPath, @"Papyrus Compiler\PapyrusAssembler.exe"));
            }
        }

        /// <summary>Gets or sets mod organizers path.</summary>
        public string OrganizerPath { get; set; }

        /// <summary>Gets a value indicating whether Mod Organizer is installed.
        /// </summary>
        public bool OrganizerInstalled
        {
            get
            {
                return File.Exists(Path.Combine(this.OrganizerPath, "ModOrganizer.exe"));
            }
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

        /// <summary>Changes the organizer path.</summary>
        public void ChangeOrganizerPath()
        {
            this.OrganizerPath = this.SelectFolder();
            this.SaveSettings();
        }

        /// <summary>Changes the Skyrim path.</summary>
        public void ChangeSkyrimPath()
        {
            this.SkyrimPath = this.SelectFolder();
            this.SaveSettings();
        }

        public void OpenAppDataFolder()
        {
            Process.Start(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SCH"));
        }

        /// <summary>Opens a <see cref="FolderBrowserDialog"/> returning the selected path.</summary>
        /// <returns>A <see cref="string"/> representing the path.</returns>
        private string SelectFolder()
        {
            FolderBrowserDialog dialogue = new FolderBrowserDialog();

            return dialogue.ShowDialog() == DialogResult.OK ? dialogue.SelectedPath : string.Empty;
        }
    }
}