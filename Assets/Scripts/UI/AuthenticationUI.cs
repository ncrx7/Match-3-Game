using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;
using System.Threading.Tasks;
using Services.Firebase;
using UnityUtils.BaseClasses;

public class AuthenticationUI : SingletonBehavior<AuthenticationUI>
{
    //Panel variables
    [Header("Panels")]
    public GameObject loginUI;
    public GameObject registerUI;

    //Login variables
    [Header("Login Field")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;
    public TMP_Text confirmLoginText;

    //Register variables
    [Header("Register Field")]
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;
    public TMP_Text warningRegisterText;
    
    //Function for the login button
    public async void LoginButton()
    {
        string result = await Authentication.Login(emailLoginField.text, passwordLoginField.text);

        if (result == "Success")
        {
            warningLoginText.text = "";
            confirmLoginText.text = "Logged In";
            return;
        }
        warningLoginText.text = result;
        confirmLoginText.text = "";
    }
    
    //Function for the register button
    public async void RegisterButton()
    {
        if (passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            //If the password does not match show a warning
            warningRegisterText.text = "Password Does Not Match!";
            return;
        }

        string result = await Authentication.Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text);

        if (result == "Success")
        {
            warningRegisterText.text = "";
            confirmLoginText.text = "Register Successful";
            
            loginUI.SetActive(true);
            registerUI.SetActive(false);
            return;
        }
        warningRegisterText.text = result;
        confirmLoginText.text = "";
    }
}
