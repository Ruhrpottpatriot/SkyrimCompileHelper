// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SolutionManagerViewModel.cs" company="Robert Logiewa">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Robert Logiewa
// </copyright>
// <summary>
//   ViewModel containing methods and properties to add new solutions to the program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SkyrimCompileHelper.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows;

    using Caliburn.Micro;

    using SkyrimCompileHelper.Common;

    /// <summary>ViewModel containing methods and properties to add new solutions to the program.</summary>
    public sealed class SolutionManagerViewModel : Screen
    {
        /// <summary>The window manager.</summary>
        private readonly IWindowManager windowManager;

        /// <summary>Initializes a new instance of the <see cref="SolutionManagerViewModel"/> class.</summary>
        public SolutionManagerViewModel()
        {
            if (Execute.InDesignMode)
            {
                this.DisplayName = "Solution Manger";
            }
        }

        /// <summary>Initializes a new instance of the <see cref="SolutionManagerViewModel"/> class.</summary>
        /// <param name="windowManager">The window manager.</param>
        /// <param name="solutions">The solutions.</param>
        public SolutionManagerViewModel(IWindowManager windowManager, IList<Solution> solutions)
        {
            this.DisplayName = "Solution Manager";

            this.windowManager = windowManager;
            this.Solutions = new ObservableCollection<Solution>(solutions);
        }

        /// <summary>Gets or sets the selected solutions.</summary>
        public Solution SelectedSolutions { get; set; }

        /// <summary>Gets or sets the solutions.</summary>
        public ObservableCollection<Solution> Solutions { get; set; }

        /// <summary>Adds a new solution to the list.</summary>
        public void AddSolution()
        {
            NewSolutionViewModel viewModel = new NewSolutionViewModel(this.windowManager);

            Dictionary<string, object> settingsDictionary = new Dictionary<string, object>
            {
                { "ResizeMode", ResizeMode.NoResize }
            };

            bool? answer = this.windowManager.ShowDialog(viewModel, null, settingsDictionary);

            if (answer.HasValue && answer.Value)
            {
                this.Solutions.Add(viewModel.GetSolution());
            }
        }

        /// <summary>Deletes a solution from the list.</summary>
        public void DeleteSolution()
        {
            this.Solutions.Remove(this.SelectedSolutions);
        }

        /// <summary>Compiles and returns the solutions to the user.</summary>
        /// <returns>An <see cref="IList{T}"/> containing the compiled <see cref="Solution"/>.</returns>
        public IList<Solution> GetSolutions()
        {
            return this.Solutions;
        }

        /// <summary>Closes the window.</summary>
        public void Close()
        {
            this.TryClose(true);
        }
    }
}
