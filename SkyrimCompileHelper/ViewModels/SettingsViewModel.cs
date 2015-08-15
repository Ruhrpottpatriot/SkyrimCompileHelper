namespace SkyrimCompileHelper.ViewModels
{
    using System;
    using System.IO;
    using System.Windows.Forms;

    using Caliburn.Micro;

    using PropertyChanged;

    using SkyrimCompileHelper.Common;

    [ImplementPropertyChanged]
    public class SettingsViewModel
    {
        private readonly ISettingsRepository settingsRepository;

        public SettingsViewModel()
        {
            if (Execute.InDesignMode)
            {
                this.SkyrimPath = @"C:\Skyrim";
                this.OrganizerPath = @"C:\Organizer";
            }
        }

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

                return Directory.Exists(Path.Combine(this.SkyrimPath, "Papyrus Compiler"));
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

        public void ChangeOrganizerPath()
        {
            this.OrganizerPath = this.SelectFolder();
        }

        public void ChangeSkyrimPath()
        {
            this.SkyrimPath = this.SelectFolder();
        }

        private string SelectFolder()
        {
            FolderBrowserDialog dialogue = new FolderBrowserDialog();

            return dialogue.ShowDialog() == DialogResult.OK ? dialogue.SelectedPath : string.Empty;
        }
    }
}