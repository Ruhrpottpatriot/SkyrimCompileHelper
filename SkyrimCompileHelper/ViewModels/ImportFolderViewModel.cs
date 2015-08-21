namespace SkyrimCompileHelper.ViewModels
{
    using System.Collections.ObjectModel;

    using Caliburn.Micro;

    using SkyrimCompileHelper.Core;

    public class ImportFolderViewModel
    {
        /// <summary>The settings repository.</summary>
        private readonly ISettingsRepository settingsRepository;

        /// <summary>The window manager.</summary>
        private readonly IWindowManager windowManager;

        /// <summary>Initialises a new instance of the <see cref="ImportFolderViewModel"/> class.</summary>
        public ImportFolderViewModel()
        {
            if (Execute.InDesignMode)
            {
                this.ImportFolders = new ObservableCollection<ImportFolder>
                {
                    new ImportFolder(@"C:\Test"),
                    new ImportFolder(@"D:\Mods")
                };
            }
        }

        /// <summary>Initialises a new instance of the <see cref="ImportFolderViewModel"/> class.</summary>
        /// <param name="settingsRepository">The settings repository.</param>
        /// <param name="windowManager">The window manager.</param>
        public ImportFolderViewModel(ISettingsRepository settingsRepository, IWindowManager windowManager)
        {
            this.settingsRepository = settingsRepository;
            this.windowManager = windowManager;
        }

        /// <summary>Gets or sets the import folders.</summary>
        public ObservableCollection<ImportFolder> ImportFolders { get; set; }

        /// <summary>Gets or sets the selected import folder.</summary>
        public ImportFolder SelectedImportFolder { get; set; }
        
        /// <summary>Edits the selected import folder.</summary>
        public void EditImportFolder()
        {
            if (this.SelectedImportFolder == null)
            {
                return;
            }

            ImportFolderManagerViewModel viewModel = new ImportFolderManagerViewModel(this.SelectedImportFolder);

            bool? answer = this.windowManager.ShowDialog(viewModel);

            if (answer.HasValue && answer.Value)
            {
                this.ImportFolders.Remove(this.SelectedImportFolder);
                this.ImportFolders.Add(viewModel.Folder);
                this.SelectedImportFolder = null;
            }

            // this.SaveSolution();
        }

        /// <summary>Adds an import folder to the list.</summary>
        public void AddImportFolder()
        {
            ImportFolderManagerViewModel viewModel = new ImportFolderManagerViewModel();

            bool? answer = this.windowManager.ShowDialog(viewModel);
            if (answer.HasValue && answer.Value)
            {
                this.ImportFolders.Add(viewModel.Folder);
            }

           // this.SaveConfiguration();
        }

        /// <summary>Removes an import folder from the list.</summary>
        public void RemoveImportFolder()
        {
            if (this.SelectedImportFolder == null)
            {
                return;
            }

            this.ImportFolders.Remove(this.SelectedImportFolder);
            // this.SaveConfiguration();
        }
    }
}
