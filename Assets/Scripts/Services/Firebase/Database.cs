using Firebase.Database;

namespace Services.Firebase
{
    public static class Database
    {
        private static DatabaseReference dbReference;

        public static void Initialize()
        {
            dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        }
    }
}