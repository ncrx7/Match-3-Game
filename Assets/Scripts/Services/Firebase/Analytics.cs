using Firebase.Analytics;
using Utils.BaseClasses;

namespace Services.Firebase
{
    public class Analytics : Singleton<Analytics>
    {
        public static void Initialize()
        {
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
        }
        
        public static void Reset()
        {
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(false);
        }
    }
}