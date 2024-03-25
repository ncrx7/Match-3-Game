using Core;
using UnityEngine;
using Utils.Extensions;

namespace Managers
{
    public class FirebaseManager : MonoBehaviour
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
            
            //TO DO : Bu fonksyon asenkron olacak bitince load scene geçecek
            await FirebaseMemory.Initialize();
        }
        
        private void OnApplicationQuit()
        {
            FirebaseMemory.Reset();
        }
    }
}