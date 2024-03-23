using System.Threading.Tasks;
using Firebase;
using Firebase.Crashlytics;
using Firebase.Extensions;
using Managers;
using Services.Firebase;
using UnityEngine;
using Utils.BaseClasses;

namespace Core
{
    public static class FirebaseMemory
    {
        public static DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
        public static bool firebaseInitialized = false;
        public static FirebaseManager FirebaseManager;
        public static FirebaseApp App;

        public static async Task Initialize()
        {
            await FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
                dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    App = FirebaseApp.DefaultInstance;

                    // When this property is set to true, Crashlytics will report all
                    // uncaught exceptions as fatal events. This is the recommended behavior.
                    Crashlytics.ReportUncaughtExceptionsAsFatal = true;

                    Database.Initialize();
                    Authentication.Initialize();
                    Analytics.Initialize();

                    firebaseInitialized = true;
                }
                else
                {
                    Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
                }
            });
        }

        public static void Reset()
        {
            dependencyStatus = DependencyStatus.UnavailableDisabled;
            firebaseInitialized = false;
            FirebaseManager = null;
            App = null;
        }
    }
}