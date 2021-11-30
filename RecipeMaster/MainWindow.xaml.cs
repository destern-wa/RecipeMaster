using MahApps.Metro.Controls;
using RecipeMaster.View;
using RecipeMaster.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace RecipeMaster
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            // Instantiate and set view model
            rootViewModel = RootViewModel.Create(App.Database.DiscardChanges, SetChildView, App.Database.HasChanges, ConfirmPrompt);
            DataContext = rootViewModel;
            // Set the initial child view
            SetChildView();
        }
        /// <summary>
        /// Root view model
        /// </summary>
        private RootViewModel rootViewModel;

        /// <summary>
        /// Handles items being invoked on the hamburger menu
        /// </summary>
        /// <remarks>
        /// Binding commands on the actual items is unreliable and results in "Cannot find governing
        /// FrameworkElement or Framework..." errors. So instead we handled them here in the event handler
        /// (sometimes proper MVVM is just too hard!)
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HamburgerMenuControl_OnItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs e)
        {
            if (e.IsItemOptions)
            {
                // There only item in the options section is "About"
                rootViewModel.ShowAboutPage();
            }
            else switch (((HamburgerMenu)sender).SelectedIndex)
            {
                case 0: rootViewModel.ShowDashboard(); break;
                case 1: rootViewModel.AddRecipe(); break;
                case 2: rootViewModel.BrowseRecipes(); break;
                case 3: rootViewModel.Search(); break;
                case 4: rootViewModel.ViewFavourites(); break;
                default:
                    {
                        Console.WriteLine("[HamburgerMenuControl_OnItemInvoked] unknown SelectedIndex");
                        break;
                    }
            }

            // Binding also doesn't seem to work properly with SelectedIndex, so make sure here that the
            // control's value matches the view model's value (accounts for the user not wanting to
            // lose changes by switching to a different view)
            if (HamburgerMenuControl.SelectedIndex != rootViewModel.CurrentViewIndex)
            {
                HamburgerMenuControl.SelectedIndex = rootViewModel.CurrentViewIndex;
            }

            // Close the menu if an item was selected
            if (!e.IsItemOptions && HamburgerMenuControl.IsPaneOpen)
            {
                HamburgerMenuControl.IsPaneOpen = false;
            }
        }

        /// <summary>
        /// Functions to construct new views, keyed by the rootViewModel's
        /// ViewTitle enum
        /// </summary>
        private Dictionary<RootViewModel.ViewTitle, Func<UserControl>> viewConstrctors = new Dictionary<RootViewModel.ViewTitle, Func<UserControl>>()
        {
            [RootViewModel.ViewTitle.Browser] = () => new RecipeBrowserView(),
            [RootViewModel.ViewTitle.Dashboard] = () => new DashboardView(),
            [RootViewModel.ViewTitle.Editor] = () => new RecipeEditorView(),
            [RootViewModel.ViewTitle.Favourites] = () => new RecipeBrowserView(),
            [RootViewModel.ViewTitle.Searcher] = () => new RecipeSearcherView(),
            [RootViewModel.ViewTitle.About] = () => new AboutAppView()
        };

        /// <summary>
        /// Constructs a new view and attaches a view model
        /// </summary>
        /// <param name="viewType">Type of view to construct</param>
        /// <param name="viewModel">View model for the view</param>
        /// <returns>New view, with view model attached</returns>
        private UserControl CreateView(RootViewModel.ViewTitle viewType, object viewModel)
        {
            UserControl view = viewConstrctors[viewType]();
            view.DataContext = viewModel;
            return view;
        }

        /// <summary>
        /// Sets a child view (with its own view model) in the content viewer elemet
        /// </summary>
        private void SetChildView()
        {
            ContentViewer.Child = CreateView(
                rootViewModel.CurrentViewTitle,
                rootViewModel.CurrentViewModel
            );
        }

        /// <summary>
        /// Shows a confirmation prompt and returns the user's selection
        /// </summary>
        /// <param name="message">Message to display</param>
        /// <returns>True if OK was selected, false otherwise</returns>
        private bool ConfirmPrompt(string message)
        {
            MessageBoxResult result = MessageBox.Show(message, "Confirmation required", MessageBoxButton.OKCancel);
            return result == MessageBoxResult.OK;
        }

        /// <summary>
        /// When the window has loaded, loads the favourites
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadFavourites();
        }

        /// <summary>
        /// Loads favourites, with message boxes shown for errors and warnings encountered
        /// </summary>
        public void LoadFavourites()
        {
            (bool favsLoaded, string favsLoadMessage, Exception favsLoadEx) = Favourites.Load();
            if (!favsLoaded)
            {
                MessageBoxResult choice = MessageBox.Show(
                    this,
                    favsLoadMessage + "\nContinue?",
                    "Error loading favourites",
                    MessageBoxButton.YesNo
                );
                if (choice == MessageBoxResult.No) this.Close();
            }
            else if (favsLoadMessage != null)
            {
                MessageBox.Show(this, favsLoadMessage, "Warning", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// When the window is closing, disconnects from the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.Database.Disconnect();
        }

    }
}
