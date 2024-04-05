using Core;
using Firebase.Auth;
using Services.Firebase;
using Services.Firebase.Database;
using UI;
using UnityEngine;
using UnityUtils.BaseClasses;
using Utils.Extensions;

namespace Managers
{
    public class FirebaseManager : SingletonBehavior<FirebaseManager>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Main()
        {
            // Create an empty GameObject
            GameObject gameObject = new GameObject("Firebase");
            // Add this Component
            gameObject.AddComponent<FirebaseManager>();
        }

        protected async void Awake()
        {
            if (!gameObject.DontDestroyOnLoadIfSingle<FirebaseManager>())
                return;
            FirebaseMemory.FirebaseManager = this;
            
            await FirebaseMemory.Initialize();
            ShowAuthentication();
        }
        
        private void OnApplicationQuit()
        {
            FirebaseMemory.Reset();
        }

        public static async void ShowAuthentication()
        {
            if (LocalDatabase.TestUser)
            {
                FirebaseResult<FirebaseUser> loginResult = await Authentication.LoginWithTestUser();
                if (loginResult.Success)
                    return;
                
                Debug.LogError("Login With Test User Operation is Failed!!!");
                LocalDatabase.TestUser = false;
                ShowAuthentication();
            }
            else
            {
                if (!AuthenticationUI.Instance)
                {
                    var auth = Resources.Load("AuthenticationUI") as GameObject;
                    Instantiate(auth, Instance.transform);
                }
                
                AuthenticationUI.Instance.Activate(true);
            }
        }
    }
}