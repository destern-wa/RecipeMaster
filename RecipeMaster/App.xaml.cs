using RecipeMaster.Database;
using RecipeMaster.Interface;
using System;
using System.Windows;

namespace RecipeMaster
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// MSSQL database connection string
        /// </summary>
        public static string msSqlConnStr = @"Data Source=(localdb)\ProjectsV13;Initial Catalog=Recipes;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        /// <summary>
        /// Read-only database instance (singleton)
        /// </summary>
        private static readonly IRecipeDatabase database;

        /// <summary>
        /// Database instance
        /// </summary>
        public static IRecipeDatabase Database { get => database; }

        /// <summary>
        /// Static constructor to initialise database singleton
        /// </summary>
        static App()
        {
            try
            {
                database = MsSqlDatabase.Instance;
            }
            catch (Exception e)
            {
                string errorMessage = string.Format("An unexpected error occurred:\n\n{0}", e.Message);
                MessageBox.Show(errorMessage, "Could not connect to database", MessageBoxButton.OK, MessageBoxImage.Error);
                if (Application.Current != null) Application.Current?.Shutdown();
                else throw e;
            }
        }
    }
}
