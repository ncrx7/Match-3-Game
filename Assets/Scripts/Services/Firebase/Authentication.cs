using Firebase.Auth;

namespace Services.Firebase
{
    public static class Authentication
    {
        public static FirebaseAuth auth;
        public static FirebaseUser User;
        public static void Initialize()
        {
            //Set the authentication instance object
            auth = FirebaseAuth.DefaultInstance;
        }
    }
}