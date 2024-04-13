using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Database;
using UnityEngine;

namespace Services.Firebase.Database
{
    /// <summary>
    /// Provides functionality to retrieve leaderboard data from the Firebase database.
    /// </summary>
    public static class Leaderboard
    {
        /// <summary>
        /// Retrieves the leaderboard data from the Firebase database, consisting of usernames and their corresponding high scores.
        /// </summary>
        /// <returns>A FirebaseResult containing the success status and the leaderboard data if successful, or an error message if unsuccessful.</returns>
        public static async Task<FirebaseResult<List<LeaderboardUserModel>>> GetLeaderboard()
        {
            try
            {
                // List to store the leaderboard entries
                List<LeaderboardUserModel> leaderboard = new List<LeaderboardUserModel>();

                // Query to get all users
                var queryResult = await Database.DbReference.Child("users").GetValueAsync();

                if (queryResult is not { ChildrenCount: > 0 })
                {
                    return new FirebaseResult<List<LeaderboardUserModel>>()
                    {
                        Success = false,
                        Cause = "No leaderboard entries found."
                    };
                }

                foreach (var childSnapshot in queryResult.Children)
                {
                    // Get the user's high score from the snapshot
                    var highScoreData = childSnapshot.Child("HighScore").GetValue(true);
                    if (highScoreData != null)
                    {
                        int highScore = int.Parse(highScoreData.ToString());

                        // Get the user's username from the snapshot
                        string userName = childSnapshot.Child("UserName").Value.ToString();

                        // Create a new LeaderboardUserModel instance
                        LeaderboardUserModel leaderboardUser = new LeaderboardUserModel()
                        {
                            UserName = userName,
                            HighScore = highScore
                        };

                        // Add the leaderboard user to the list
                        leaderboard.Add(leaderboardUser);
                    }
                }

                // Sort the leaderboard by HighScore in descending order
                leaderboard.Sort((a, b) => b.HighScore.CompareTo(a.HighScore));

                return new FirebaseResult<List<LeaderboardUserModel>>()
                {
                    Success = true,
                    Item = leaderboard
                };
            }
            catch (Exception exception)
            {
                return new FirebaseResult<List<LeaderboardUserModel>>()
                {
                    Success = false,
                    Cause = exception.Message
                };
            }
        }

    }
}