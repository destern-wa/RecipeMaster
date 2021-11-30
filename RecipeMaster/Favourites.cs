using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace RecipeMaster
{
    /// <summary>
    /// Manages the storage and retrieval of favourite recipe IDs
    /// </summary>
    public static class Favourites
    {
        /// <summary>
        /// IDs of favourite recipes
        /// </summary>
        private static List<int> recipeIds;
        /// <summary>
        /// Path to the binary file used for saving/loading
        /// </summary>
        private static readonly string FilePath = "favourites.bin";

        /// <summary>
        /// Initilaises the favourites list by deleting the file (which will the be recreated in the Load method).
        /// Should only be called when initialising/refreshing the database.
        /// </summary>
        public static void Initialise() {
            File.Delete(FilePath);
        }

        /// <summary>
        /// Tries to load favourite recipe IDs from file
        /// </summary>
        /// <returns>(Load succeded), (Short message or null), (Exception or null)</returns>
        public static (bool, string, Exception) Load()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            Stream filestream = null;
            try
            {
                filestream = File.Open(FilePath, FileMode.Open);
                recipeIds = formatter.Deserialize(filestream) as List<int>;
                return (true, null, null);
            }
            catch (FileNotFoundException)
            {
                recipeIds = new List<int>();
                return (true, "Favourites file not found; initialising empty list", null);
            }
            catch (IOException e)
            {
                return (false, "Error reading from file", e);
            }
            catch(Exception e)
            {
                return (false, "An unexpected error occured", e);
            }
            finally
            {
                filestream?.Close();
            }
        }

        /// <summary>
        /// Tries to save favourite recipe IDs to file
        /// </summary>
        /// <returns>(Save succeded), (Short message or null), (Exception or null)</returns>
        private static (bool, string, Exception) Save()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            Stream filestream = null;
            try
            {
                filestream = File.Open(FilePath, FileMode.Create);
                formatter.Serialize(filestream, recipeIds);
                return (true, null, null);
            }
            catch (IOException e)
            {
                return (false, "Error writing favourites to file", e);
            }
            catch (Exception e)
            {
                return (false, "An unexpected error occured", e);
            }
            finally
            {
                filestream?.Close();
            }
        }

        /// <summary>
        /// Checks if a recipe is a favourite
        /// </summary>
        /// <param name="recipeId">ID of recipe</param>
        /// <returns>True if it is a favourite, false otherwise</returns>
        public static bool IsFavourite(int recipeId)
        {
            if (recipeIds == null) Load();

            return (bool)(recipeIds?.Contains(recipeId));
        }

        /// <summary>
        /// Gets the favourite recipes' IDs as an array
        /// </summary>
        /// <returns>Favourite recipes' IDs</returns>
        public static int[] GetFavourites()
        {
            if (recipeIds == null) Load();

            return recipeIds?.ToArray();
        }

        /// <summary>
        /// Add a recipe to the favourites, and saves favourites to file
        /// </summary>
        /// <param name="id">Recipe's id</param>
        /// <returns>(Save succeded), (Short message or null), (Exception or null)</returns>
        public static (bool, string, Exception) AddFavourite(int id)
        {
            if (recipeIds.Contains(id)) return (true, $"ID {id} was already a favourite", null);

            recipeIds.Add(id);
            return Save();
        }

        /// <summary>
        /// Removes a recipe from the favourites, and saves favourites to file
        /// </summary>
        /// <param name="id">Recipe's id</param>
        /// <returns>(Save succeded), (Short message or null), (Exception or null)</returns>
        public static (bool, string, Exception) RemoveFavourite(int id)
        {
            if (!recipeIds.Contains(id)) return (true, $"ID {id} was already not a favourite", null);

            recipeIds.Remove(id);
            return Save();
        }
    }
}
