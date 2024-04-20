using System;
using System.Threading.Tasks;
using Core;
using Services.Firebase.Database;
using TMPro;
using UnityEngine;

namespace UI
{
    public class LeaderboardUI : MonoBehaviour
    {
        [SerializeField] private Transform _leaderboardUserParentContainer;
        [SerializeField] private GameObject _leaderboardUserPrefab;
        private async void Start()
        {
            // FirebaseMemory.Initialize() metodunun tamamlanmasını bekleyelim
            await WaitUntilFirebaseInitialized();
            
            var result = await Leaderboard.GetLeaderboard();

            for (int i = 0; i < result.Item.Count; i++)
            {
                Debug.Log($"{i}. {result.Item[i].UserName}, Score {result.Item[i].HighScore}");
                GameObject userLeaderboard = Instantiate(_leaderboardUserPrefab, _leaderboardUserParentContainer.position, Quaternion.identity, _leaderboardUserParentContainer);
                LeaderboardReferenceHolder leaderboardReferenceHolder = userLeaderboard.GetComponent<LeaderboardReferenceHolder>();
                leaderboardReferenceHolder.GetUserNameTextReference().text = result.Item[i].UserName;
                leaderboardReferenceHolder.GetHighScoreTextReference().text = result.Item[i].HighScore.ToString();
            }
        }
        
        private async Task WaitUntilFirebaseInitialized()
        {
            while (!FirebaseMemory.FirebaseInitialized)
                await Task.Delay(100);
        }
    }
}