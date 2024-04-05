using System;
using System.Threading.Tasks;
using Core;
using Firebase;
using Firebase.Auth;
using Services.Firebase.Database;
using UnityEngine;

namespace Services.Firebase
{
    /// <summary>
    /// Controls authentication with Firebase.
    /// </summary>
    public static class Authentication
    {
        public static FirebaseAuth Auth;
        public static FirebaseUser User;

        private const string _testUsername = "TestUser";
        private const string _testEmail = "TestUser@mail.com";
        private const string _testPassword = "test_User1234*";

        /// <summary>
        /// Gets a value indicating whether the user is authenticated.
        /// </summary>
        /// <value><c>true</c> if the user is authenticated; otherwise, <c>false</c>.</value>
        public static bool IsAuth
        {
            get
            {
                if (!FirebaseMemory.FirebaseInitialized)
                    return false;
                
                if (Auth == null)
                    return false;

                return User != null;
            }
        }
        
        /// <summary>
        /// Initializes the Firebase authentication system.
        /// </summary>
        public static void Initialize()
        {
#if !UNITY_EDITOR
// TR: Olası beklenmeyen hatalara karşı önlem amaçlıdır. Gerekliliği öngörülmez
// EN: It is for precautionary purposes against possible unexpected errors. Necessity is not foreseen
            LocalDatabase.TestUser = false;
#endif
            
            //Set the authentication instance object
            Auth = FirebaseAuth.DefaultInstance;
        }

        /// <summary>
        /// Resets the Firebase authentication system.
        /// </summary>
        public static void Reset()
        {
            //Set the authentication instance object
            Auth = null;
            User = null;
        }

        /// <summary>
        /// Logs in the user with the test email and password.
        /// </summary>
        /// <returns>A FirebaseResult containing the success status and the logged-in user if successful, or an error message if unsuccessful.</returns>
        public static async Task<FirebaseResult<FirebaseUser>> LoginWithTestUser() => await Login(_testEmail, _testPassword);
        
        /// <summary>
        /// Logs in the user with the provided email and password.
        /// </summary>
        /// <param name="email">The email of the user to log in.</param>
        /// <param name="password">The password of the user to log in.</param>
        /// <returns>A FirebaseResult containing the success status and the logged-in user if successful, or an error message if unsuccessful.</returns>
        public static async Task<FirebaseResult<FirebaseUser>> Login(string email, string password)
        {
            try
            {
                //Call the Firebase auth signin function passing the email and password
                var loginResult = await Auth.SignInWithEmailAndPasswordAsync(email, password);

                //User is now logged in. Now get the result
                User = loginResult.User;
                Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
                await Database.Database.LoadUser();
                Debug.LogFormat("User load successfully: {0}", Database.Database.User.UserName);
                
                return new FirebaseResult<FirebaseUser>() {Success = true, Item = User};
            }
            catch (Exception exception)
            {
                return new FirebaseResult<FirebaseUser>() {Success = false, Cause = CatchError(exception)};
            }
        }

        /// <summary>
        /// Registers a new user with the provided email, password, and username.
        /// </summary>
        /// <param name="email">The email of the user to be registered.</param>
        /// <param name="password">The password of the user to be registered.</param>
        /// <param name="username">The username of the user to be registered.</param>
        /// <returns>A FirebaseResult containing the success status and the registered user if successful, or an error message if unsuccessful.</returns>
        public static async Task<FirebaseResult<FirebaseUser>> Register(string email, string password, string username)
        {
            try
            {
                //If the username field is blank show a warning
                if (username == "")
                    return new FirebaseResult<FirebaseUser>() {Success = false, Cause = "Missing Username"};
                
                //Call the Firebase auth signin function passing the email and password
                var registerResult = await Auth.CreateUserWithEmailAndPasswordAsync(email, password);

                //User has now been created
                //Now get the result
                User = registerResult.User;

                if (User == null)
                    return new FirebaseResult<FirebaseUser>() {Success = false, Cause = "Register Failed!"};
                
                //Create a user profile and set the username
                var profile = new UserProfile { DisplayName = username };

                try
                {
                    //Call the Firebase auth update user profile function passing the profile with the username
                    await User.UpdateUserProfileAsync(profile);
                    await Database.Database.SaveUser(new UserModel() {UserName = "TestUser", Level = 1, HighScore = 0});
                    return new FirebaseResult<FirebaseUser>() {Success = true, Item = User};
                }
                catch (Exception ex)
                {
                    //If there are errors handle them
                    Debug.LogWarning(message: $"Failed to register task with {ex}");
                    
                    if (ex.GetBaseException() is FirebaseException firebaseEx)
                        return new FirebaseResult<FirebaseUser>() {Success = false, Cause = 
                            "Username Set Failed, Error Code : " + (AuthError)firebaseEx.ErrorCode};
                    
                    return new FirebaseResult<FirebaseUser>() {Success = false, Cause = "Username Set Failed"};
                }
            }
            catch (Exception exception)
            {
                return new FirebaseResult<FirebaseUser>() {Success = false, Cause = CatchError(exception)};
            }
        }

        /// <summary>
        /// Handles the exception thrown during Firebase authentication and returns the corresponding error message.
        /// </summary>
        /// <param name="exception">The exception thrown during authentication.</param>
        /// <returns>The error message corresponding to the exception.</returns>
        public static string CatchError(Exception exception)
        {
            if (exception.GetBaseException() is not FirebaseException firebaseEx)
                return "Authentication Failed!";
                
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
            Debug.LogWarning(message: $"Failed to Authentication with ({(AuthError)firebaseEx.ErrorCode}) {exception}");

            return errorCode switch
            {
                AuthError.MissingEmail => "Missing Email",
                AuthError.MissingPassword => "Missing Password",
                AuthError.WeakPassword => "Weak Password",
                AuthError.WrongPassword => "Wrong Password",
                AuthError.InvalidEmail => "Invalid Email",
                AuthError.UserNotFound => "Account does not exist",
                AuthError.EmailAlreadyInUse => "Email Already In Use",
                AuthError.None => "No error",
                AuthError.InvalidUserToken => "Invalid user token",
                AuthError.TooManyRequests => "Too many requests",
                AuthError.UserTokenExpired => "User token expired",
                AuthError.InvalidCustomToken => "Invalid custom token",
                AuthError.CustomTokenMismatch => "Custom token mismatch",
                AuthError.InvalidCredential => "Invalid credential",
                AuthError.UserDisabled => "User disabled",
                AuthError.AccountExistsWithDifferentCredentials => "Account exists with different credentials",
                AuthError.OperationNotAllowed => "Operation not allowed",
                _ => "Authentication Failed!"
            };
        }
    }
}