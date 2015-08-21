// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImportFolderViewModel.cs" company="Robert Logiewa">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Robert Logiewa
// </copyright>
// <summary>
//   Contains methods and properties to show and manipulate a collection of <see cref="ImportFolder">Import Folders</see>.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SkyrimCompileHelper.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;

    using Caliburn.Micro;

    using SkyrimCompileHelper.Core;
    using SkyrimCompileHelper.Core.EventHandles;

    /// <summary>Contains methods and properties to show and manipulate a collection of <see cref="ImportFolder">Import Folders</see>.</summary>
    public class ImportFolderViewModel
    {
        /// <summary>The window manager.</summary>
        private readonly IWindowManager windowManager;

        /// <summary>The event aggregator.</summary>
        private readonly IEventAggregator eventAggregator;

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
        /// <param name="windowManager">The window manager.</param>
        /// <param name="eventAggregator">The event aggregator.</param>
        public ImportFolderViewModel(IWindowManager windowManager, IEventAggregator eventAggregator)
        {
            this.windowManager = windowManager;
            this.eventAggregator = eventAggregator;

            this.ImportFolders = new ObservableCollection<ImportFolder>();
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

            this.SaveSolution();
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

            this.SaveSolution();
        }

        /// <summary>Removes an import folder from the list.</summary>
        public void RemoveImportFolder()
        {
            if (this.SelectedImportFolder == null)
            {
                return;
            }

            this.ImportFolders.Remove(this.SelectedImportFolder);
            this.SaveSolution();
        }

        /// <summary>Publishes a message to the event aggregator telling all subscribers to save the solution.</summary>
        private void SaveSolution()
        {
            this.eventAggregator.Publish(new SaveSolutionEvenHandle(), action => { Task.Factory.StartNew(action); });
        }
    }
}
