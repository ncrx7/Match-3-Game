using System;
using System.Collections;
using Firebase.Database;
using UnityEngine;

namespace Test
{
    public class FirebaseDatabaseTest : MonoBehaviour
    {
        public UserModel user;
        public string userId;
        private DatabaseReference dbReference;

        private void Start()
        {
            dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        }

        public void Save()
        {
            string json = JsonUtility.ToJson(user);
            dbReference.Child("users").Child(userId).SetRawJsonValueAsync(json);

        }

        public async void Load()
        {
            var serverData = await dbReference.Child("users").Child(userId).GetValueAsync();
            Debug.Log("Process is complete!");

            var jsonData = serverData.GetRawJsonValue();

            if (jsonData != null)
            {
                user = JsonUtility.FromJson<UserModel>(jsonData);
                Debug.Log(user.userName + user.level);
            }
            else
            {
                Debug.Log("Fail");
            }
        }
    }
}