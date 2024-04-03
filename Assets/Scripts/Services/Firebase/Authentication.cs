using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using UnityEngine;

namespace Services.Firebase
{
    public static class Authentication
    {
        public static FirebaseAuth auth;
        public static FirebaseUser User;

        private const string _testUsername = "TestUser";
        private const string _testEmail = "TestUser@mail.com";
        private const string _testPassword = "test_User1234*";
        
        public static void Initialize()
        {

#if !UNITY_EDITOR
// TR: Olası beklenmeyen hatalara karşı önlem amaçlıdır. Gerekliliği öngörülmez
// EN: It is for precautionary purposes against possible unexpected errors. Necessity is not foreseen
            testUser = false;
#endif

            //Set the authentication instance object
            auth = FirebaseAuth.DefaultInstance;
        }

        public static void Reset()
        {
            //Set the authentication instance object
            auth = null;
        }

        public static async Task<string> LoginWithTestUser() => await Login(_testEmail, _testPassword);
        
        public static async Task<string> Login(string _email, string _password)
        {
            try
            {
                //Call the Firebase auth signin function passing the email and password
                var loginResult = await auth.SignInWithEmailAndPasswordAsync(_email, _password);

                //User is now logged in
                //Now get the result
                User = loginResult.User;
                Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
                return "Success";
            }
            catch (Exception exception)
            {
                FirebaseException firebaseEx = exception.GetBaseException() as FirebaseException;
                
                if (firebaseEx != null)
                {
                    AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                    Debug.LogWarning(message: $"Failed to login with ({(AuthError)firebaseEx.ErrorCode}) {exception}");

                    return errorCode switch
                    {
                        AuthError.MissingEmail => "Missing Email",
                        AuthError.MissingPassword => "Missing Password",
                        AuthError.WrongPassword => "Wrong Password",
                        AuthError.InvalidEmail => "Invalid Email",
                        AuthError.UserNotFound => "Account does not exist",
                        _ => "Login Failed!"
                    };
                }
                return "Login Failed!";
            }
        }

        public static async Task<string> Register(string _email, string _password, string _username)
        {
            try
            {
                //If the username field is blank show a warning
                if (_username == "")
                    return "Missing Username";
                
                //Call the Firebase auth signin function passing the email and password
                var registerResult = await auth.CreateUserWithEmailAndPasswordAsync(_email, _password);

                //User has now been created
                //Now get the result
                User = registerResult.User;

                if (User == null)
                    return "Register Failed!";
                
                //Create a user profile and set the username
                var profile = new UserProfile { DisplayName = _username };

                try
                {
                    //Call the Firebase auth update user profile function passing the profile with the username
                    await User.UpdateUserProfileAsync(profile);
                    return "Success";
                }
                catch (Exception ex)
                {
                    //If there are errors handle them
                    Debug.LogWarning(message: $"Failed to register task with {ex}");
                    var firebaseEx = ex.GetBaseException() as FirebaseException;
                    var errorCode = (AuthError)firebaseEx.ErrorCode;
                    return "Username Set Failed!";
                }
            }
            catch (Exception exception)
            {
                //If there are errors handle them
                Debug.LogWarning(message: $"Failed to register task with {exception}");
                var firebaseEx = exception.GetBaseException() as FirebaseException;
                var errorCode = (AuthError)firebaseEx.ErrorCode;

                return errorCode switch
                {
                    AuthError.MissingEmail => "Missing Email",
                    AuthError.MissingPassword => "Missing Password",
                    AuthError.WeakPassword => "Weak Password",
                    AuthError.EmailAlreadyInUse => "Email Already In Use",
                    _ => "Register Failed!"
                };
            }
        }
    }
}