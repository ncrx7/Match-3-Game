using System;
using System.Threading.Tasks;
using Core;
using Services.Firebase.Database;
using UnityEngine;

namespace UI
{
    public class LeaderboardUI : MonoBehaviour
    {
        private async void Start()
        {
            // FirebaseMemory.Initialize() metodunun tamamlanmasını bekleyelim
            await WaitUntilFirebaseInitialized();
            
            var result = await Leaderboard.GetLeaderboard();

            for (int i = 0; i < result.Item.Count; i++)
            {
                Debug.Log($"{i}. {result.Item[i].UserName}, Score {result.Item[i].HighScore}");
            }
        }
        
        private async Task WaitUntilFirebaseInitialized()
        {
            while (!FirebaseMemory.FirebaseInitialized)
                await Task.Delay(100);
        }
    }
}