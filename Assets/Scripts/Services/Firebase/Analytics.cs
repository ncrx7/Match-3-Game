using System.Diagnostics;
using Firebase.Analytics;
using Utils.BaseClasses;

namespace Services.Firebase
{
    public static class Analytics
    {
        public static void Initialize()
        {
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
        }
        
        public static void Reset()
        {
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(false);
        }
        
        public static bool NullCheckAndLog(this object value, string header = "")
        {
            if (value != null)
                return true;

            // En. Getting error details and stack traces
            // Tr. Hata detayları ve geri izleme (stack trace) alınması
            StackTrace stackTrace = new StackTrace();
            StackFrame frame = stackTrace.GetFrame(1);

            string methodName = frame.GetMethod().Name;
            string className = frame.GetMethod().DeclaringType?.Name;

            string errorMessage = $"{header}{(string.IsNullOrEmpty(header) ? "" : " / ")} " +
                                  $"NullCheckAndLog : {className}.{methodName}: {nameof(value)} is null";
            
            //Logging...
            UnityEngine.Debug.LogError(errorMessage);
            FirebaseAnalytics.LogEvent("NullCheckAndLog", new Parameter("errorMessage", errorMessage));
            
            return false;
        }
    }
}