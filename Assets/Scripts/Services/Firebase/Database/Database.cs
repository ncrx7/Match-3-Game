using System.Threading.Tasks;
using Core;
using Firebase.Database;
using UnityEngine;

namespace Services.Firebase.Database
{
    /// <summary>
    /// Controls the Firebase database operations such as saving and retrieving user data.
    /// </summary>
    public static class Database
    {
        public static UserModel User;
        public static DatabaseReference DbReference;

        private static readonly FirebaseResult<UserModel> _authRequiredResultUser = new FirebaseResult<UserModel>()
        {
            Success = false,
            Cause = "User authentication required!"
        };

        private static readonly FirebaseResult<int> _authRequiredResultInt = new FirebaseResult<int>()
        {
            Success = false,
            Cause = "User authentication required!"
        };

        /// <summary>
        /// Initializes the Firebase database reference.
        /// </summary>
        public static void Initialize()
        {
            DbReference = FirebaseDatabase.DefaultInstance.RootReference;
        }

        /// <summary>
        /// Resets the Firebase database reference.
        /// </summary>
        public static void Reset()
        {
            DbReference = null;
        }

        /// <summary>
        /// Retrieves the high score data from the Firebase database.
        /// </summary>
        /// <returns>A FirebaseResult containing the success status and the retrieved high score data if successful, or an error message if unsuccessful.</returns>
        public static async Task<FirebaseResult<int>> GetHighScore()
        {
            try
            {
                // Check if the user is authenticated
                if (!Authentication.IsAuth)
                    return _authRequiredResultInt;

                // Retrieve the high score data from the database
                var highScoreData = await DbReference.Child("users").Child(Authentication.User.UserId).Child("HighScore").GetValueAsync();

                // Check if the high score data exists
                if (!highScoreData.Exists)
                    return new FirebaseResult<int>()
                    {
                        Success = false,
                        Cause = "High score data not found"
                    };

                // Parse the high score data and return it
                int highScore = int.Parse(highScoreData.Value.ToString());
                return new FirebaseResult<int>()
                {
                    Success = true,
                    Item = highScore
                };
            }
            catch (DatabaseException e)
            {
                // In case of an error, return the appropriate error message
                return new FirebaseResult<int>()
                {
                    Success = false,
                    Cause = CatchError(e)
                };
            }
        }

        /// <summary>
        /// Saves the high score data to the Firebase database.
        /// </summary>
        /// <param name="highScore">The high score data to be saved.</param>
        /// <returns>A FirebaseResult containing the success status and the saved high score data if successful, or an error message if unsuccessful.</returns>
        public static async Task<FirebaseResult<int>> SaveHighScore(int highScore)
        {
            try
            {
                // Check if the user is authenticated
                if (!Authentication.IsAuth)
                    return _authRequiredResultInt;

                // Save the high score data to the database
                await DbReference.Child("users").Child(Authentication.User.UserId).Child("HighScore").SetValueAsync(highScore);

                // Return a result indicating successful saving
                return new FirebaseResult<int>()
                {
                    Success = true,
                    Item = highScore
                };
            }
            catch (DatabaseException e)
            {
                // In case of an error, return the appropriate error message
                return new FirebaseResult<int>()
                {
                    Success = false,
                    Cause = CatchError(e)
                };
            }
        }

        /// <summary>
        /// Retrieves the level data from the Firebase database.
        /// </summary>
        /// <returns>A FirebaseResult containing the success status and the retrieved level data if successful, or an error message if unsuccessful.</returns>
        public static async Task<FirebaseResult<int>> GetLevel()
        {
            try
            {
                // Check if the user is authenticated
                if (!Authentication.IsAuth)
                    return _authRequiredResultInt;

                // Retrieve the level data from the database
                var levelData = await DbReference.Child("users").Child(Authentication.User.UserId).Child("Level").GetValueAsync();

                // Check if the level data exists
                if (!levelData.Exists)
                    return new FirebaseResult<int>()
                    {
                        Success = false,
                        Cause = "Level data not found"
                    };

                // Parse the level data and return it
                int level = int.Parse(levelData.Value.ToString());
                return new FirebaseResult<int>()
                {
                    Success = true,
                    Item = level
                };
            }
            catch (DatabaseException e)
            {
                // In case of an error, return the appropriate error message
                return new FirebaseResult<int>()
                {
                    Success = false,
                    Cause = CatchError(e)
                };
            }
        }

        /// <summary>
        /// Saves the level data to the Firebase database.
        /// </summary>
        /// <param name="level">The level data to be saved.</param>
        /// <returns>A FirebaseResult containing the success status and the saved level data if successful, or an error message if unsuccessful.</returns>
        public static async Task<FirebaseResult<int>> SaveLevel(int level)
        {
            try
            {
                // Check if the user is authenticated
                if (!Authentication.IsAuth)
                    return _authRequiredResultInt;

                // Save the level data to the database
                await DbReference.Child("users").Child(Authentication.User.UserId).Child("Level").SetValueAsync(level);

                User.Level = level;
                // Return a result indicating successful saving
                return new FirebaseResult<int>()
                {
                    Success = true,
                    Item = level
                };
            }
            catch (DatabaseException e)
            {
                // In case of an error, return the appropriate error message
                return new FirebaseResult<int>()
                {
                    Success = false,
                    Cause = CatchError(e)
                };
            }
        }

        /// <summary>
        /// Saves the user data to the Firebase database.
        /// </summary>
        /// <param name="user">The UserModel object containing user data to be saved.</param>
        /// <returns>A FirebaseResult containing the success status and the saved UserModel object if successful, or an error message if unsuccessful.</returns>
        public static async Task<FirebaseResult<UserModel>> SaveUser(UserModel user, bool authFilter = true)
        {
            // Check if the user is authenticated
            if (!Authentication.IsAuth && authFilter)
                return _authRequiredResultUser;

            try
            {
                // Convert the UserModel object to JSON string
                string json = JsonUtility.ToJson(user);

                // Save the JSON string to the database under the current user's node
                await DbReference.Child("users").Child(Authentication.User.UserId).SetRawJsonValueAsync(json);

                // Set the local user object to the saved user
                User = user;

                // Return a success result with the saved user data
                return new FirebaseResult<UserModel>()
                {
                    Success = true,
                    Item = user
                };
            }
            catch (DatabaseException e)
            {
                // Return an error result with the error message
                return new FirebaseResult<UserModel>()
                {
                    Success = false,
                    Cause = CatchError(e)
                };
            }
        }

        /// <summary>
        /// Loads the user data from the Firebase database.
        /// </summary>
        /// <returns>A FirebaseResult containing the success status and the loaded UserModel object if successful, or an error message if unsuccessful.</returns>
        public static async Task<FirebaseResult<UserModel>> LoadUser()
        {
            try
            {
                // Check if the user is authenticated
                if (!Authentication.IsAuth)
                    return _authRequiredResultUser;

                // Get the user data from the Firebase database
                var serverData = await DbReference.Child("users").Child(Authentication.User.UserId).GetValueAsync();
                var jsonData = serverData.GetRawJsonValue();

                // Check if the user data is null
                if (jsonData == null)
                    // Return an error result if the user data is null
                    return new FirebaseResult<UserModel>()
                    {
                        Success = false,
                        Cause = "Load user operation failed: No data found"
                    };

                // Parse the JSON data to a UserModel object
                User = JsonUtility.FromJson<UserModel>(jsonData);

                // Return a success result with the loaded user data
                return new FirebaseResult<UserModel>()
                {
                    Success = true,
                    Item = User
                };
            }
            catch (DatabaseException e)
            {
                // Return an error result with the error message
                return new FirebaseResult<UserModel>()
                {
                    Success = false,
                    Cause = CatchError(e)
                };
            }
        }
        /// <summary>
        /// For Editor. Reset test user.
        /// </summary>
        public static async void ResetTestUser()
        {
            await FirebaseMemory.Initialize();
            await Authentication.LoginWithTestUser();
            await SaveUser(new UserModel()
            {
                UserName = "TestUser",
                Level = 1,
                HighScore = 0
            }, false);
            FirebaseMemory.Reset();
        }

        /// <summary>
        /// Handles the database exception and returns the error message.
        /// </summary>
        /// <param name="exception">The DatabaseException object representing the error.</param>
        /// <returns>The error message extracted from the DatabaseException object.</returns>
        private static string CatchError(DatabaseException exception)
        {
            // Log a warning message with the details of the database operation failure
            Debug.LogWarning($"Failed to perform Firebase Database operation with error message: {exception.Message}, Exception: {exception}");

            // Return the error message from the DatabaseException object
            return exception.Message;
        }
    }
}