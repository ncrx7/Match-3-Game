using System.Threading.Tasks;
using Firebase;
using Firebase.Crashlytics;
using Firebase.Extensions;
using Managers;
using Services.Firebase;
using Services.Firebase.Database;
using UnityEngine;
using Utils.BaseClasses;

namespace Core
{
    public static class FirebaseMemory
    {
        public static DependencyStatus DependencyStatus = DependencyStatus.UnavailableOther;
        public static bool FirebaseInitialized = false;
        public static FirebaseManager FirebaseManager;
        public static FirebaseApp App;

        public static async Task Initialize()
        {
            await FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
                DependencyStatus = task.Result;
                if (DependencyStatus == DependencyStatus.Available)
                {
                    App = FirebaseApp.DefaultInstance;

                    // When this property is set to true, Crashlytics will report all
                    // uncaught exceptions as fatal events. This is the recommended behavior.
                    Crashlytics.ReportUncaughtExceptionsAsFatal = true;

                    Database.Initialize();
                    Authentication.Initialize();
                    Analytics.Initialize();

                    FirebaseInitialized = true;
                }
                else
                {
                    Debug.LogError("Could not resolve all Firebase dependencies: " + DependencyStatus);
                }
            });
        }

        public static void Reset()
        {
            DependencyStatus = DependencyStatus.UnavailableDisabled;
            FirebaseInitialized = false;
            FirebaseManager = null;
            App = null;
            
            Database.Reset();
            Authentication.Reset();
            Analytics.Reset();
        }
    }
}