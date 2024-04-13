using Firebase.Auth;
using Services.Firebase;
using TMPro;
using UnityEngine;
using UnityUtils.BaseClasses;
using Utils.Extensions;

namespace UI
{
    public class AuthenticationUI : SingletonBehavior<AuthenticationUI>
    {
        //Panel variables
        [Header("Panels")]
        [SerializeField] private GameObject _loginUI;
        [SerializeField] private GameObject _registerUI;

        //Login variables
        [Header("Login Field")]
        [SerializeField] private TMP_InputField _emailLoginField;
        [SerializeField] private TMP_InputField _passwordLoginField;
        [SerializeField] private TMP_Text _warningLoginText;
        [SerializeField] private TMP_Text _confirmLoginText;

        //Register variables
        [Header("Register Field")]
        [SerializeField] private TMP_InputField _usernameRegisterField;
        [SerializeField] private TMP_InputField _emailRegisterField;
        [SerializeField] private TMP_InputField _passwordRegisterField;
        [SerializeField] private TMP_InputField _passwordRegisterVerifyField;
        [SerializeField] private TMP_Text _warningRegisterText;

        private Transform Root => transform.GetChild(0);

        public void Activate(bool active) => Root.SetActivate(active);
    
        //Function for the login button
        public async void LoginButton()
        {
            FirebaseResult<FirebaseUser> result = await Authentication.Login(_emailLoginField.text, _passwordLoginField.text);

            if (result.Success)
            {
                _warningLoginText.text = "";
                _confirmLoginText.text = "Logged In";
                Activate(false);
                return;
            }
            _warningLoginText.text = result.Cause;
            _confirmLoginText.text = "";
        }
    
        //Function for the register button
        public async void RegisterButton()
        {
            if (_passwordRegisterField.text != _passwordRegisterVerifyField.text)
            {
                //If the password does not match show a warning
                _warningRegisterText.text = "Password Does Not Match!";
                return;
            }

            FirebaseResult<FirebaseUser> result = await Authentication.Register
                (_emailRegisterField.text, _passwordRegisterField.text, _usernameRegisterField.text);

            if (result.Success)
            {
                _warningRegisterText.text = "";
                _confirmLoginText.text = "Register Successful";
            
                _loginUI.SetActive(true);
                _registerUI.SetActive(false);
                return;
            }
            _warningRegisterText.text = result.Cause;
            _confirmLoginText.text = "";
        }
        
        //Function for the logout button
        public static void LogOutButton()
        {
            if (!Instance)
            {
                var auth = Resources.Load("AuthenticationUI") as GameObject;
                Instantiate(auth, Instance.transform);
            }
            
            FirebaseResult<bool> result = Authentication.LogOut();

            if (result.Success)
            {
                Instance._warningRegisterText.text = "";
                Instance._confirmLoginText.text = "Log out Successful";
                
                Instance.Activate(true);
                Instance._loginUI.SetActive(true);
                Instance._registerUI.SetActive(false);
                return;
            }
            Instance._warningRegisterText.text = result.Cause;
            Instance._confirmLoginText.text = "";
        }
    }
}
