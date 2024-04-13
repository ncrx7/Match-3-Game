using System;
using System.Threading.Tasks;
using Services.Firebase;
using Services.Firebase.Database;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class MatchThreeSampleWindow : EditorWindow
    {
        private Texture2D _logo;
        private const int _minSizeX = 300;
        private const int _minSizeY = 500;
        private const int _maxSizeX = 480;
        private const int _maxSizeY = 800;

        [InitializeOnLoadMethod]
        private static async void OnLoad()
        {
            if (!LocalDatabase.WelcomePanelIsActive)
                return;

            await Task.Delay(10);
            
            if (!Application.isPlaying)
                ShowWindow();   
        }
        
        [MenuItem("MatchThreeSample/Welcome", false, 0)]
        public static void ShowWindow()
        {
            MatchThreeSampleWindow window = (MatchThreeSampleWindow)GetWindow(typeof(MatchThreeSampleWindow));
            window.minSize = new Vector2(_minSizeX, _minSizeY);
            window.maxSize = new Vector2(_maxSizeX, _maxSizeY);
            window.titleContent = new GUIContent("Welcome M3S");
            window.ShowUtility();
        }
        
        private async void OnGUI()
        {
            if (_logo == null) 
                _logo = Resources.Load("velo_blue_logo") as Texture2D;

            GUILayout.BeginVertical();
            
            LeftAndRightAlignment(() => {
                    GUILayout.BeginVertical();
                    Header("Welcome to Match Three Sample");
            
                    GUILayout.Label(@"This project has been developed by interns as part 
of the Velo Games internship program. 
It is a Match Three Sample Project 
where the game mechanics of Match Three 
have been enhanced and implemented 
as an example using Firebase services.");
                    GUILayout.EndVertical();
                },
                () => { GUILayout.Label(_logo, GUILayout.MaxHeight(70)); });
                
            GUILayout.Space(50);
            
            Button(LocalDatabase.TestUser, LocalDatabase.TestUser ? "Login Without Test User" : "Login With Test User",
                "Test User", () => {LocalDatabase.TestUser = !LocalDatabase.TestUser;});
            
            Button(LocalDatabase.WelcomePanelIsActive, LocalDatabase.WelcomePanelIsActive ? "Welcome window does not open with InitializeOnLoad"
                    : "Welcome window does open with InitializeOnLoad",
                "Welcome window opens automatically", () => {LocalDatabase.WelcomePanelIsActive = !LocalDatabase.WelcomePanelIsActive;});
            
            GUILayout.Space(15);
            if (GUILayout.Button("Reset Test User"))
                Database.ResetTestUser();
            
            GUILayout.Space(10);
            
            GUILayout.FlexibleSpace();
            Header("Developed By");
            GUILayout.Label("Backend & Editor Developer : Umutcan Bağcı");
            GUILayout.Label("In Game Developer & UI Designer : Hazar ..");
            GUILayout.Label("Game Developer : Batuhan Uysal");
            
            GUILayout.EndVertical();
        }

        private void LeftAndRightAlignment(Action leftContentDraw, Action rightContentDraw)
        {
            GUILayout.BeginHorizontal();
            leftContentDraw?.Invoke();
            GUILayout.FlexibleSpace();
            rightContentDraw?.Invoke();
            GUILayout.EndHorizontal();
        }
        
        private void MiddleAlignment(Action ContentDraw)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            ContentDraw.Invoke();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        
        private void Header(string header)
        {
            GUILayout.Space(10);
            GUILayout.Label(header, GUIStyles.Header);
            GUILayout.Space(5);
        }

        private void Button(bool condition, string buttonText, string explanation, Action onClickButton)
        {
            GUILayout.Space(15);
            if (GUILayout.Button(buttonText))
                onClickButton.Invoke();
            
            MiddleAlignment(() => {
                GUILayout.Label($"{explanation} : {BooleanColoredText(condition)}", GUIStyles.Rich);
            });
            GUILayout.Space(10);
        }
        
        private string BooleanColoredText(bool state) => state ? GreenText("True") : RedText("False");
        private string BooleanColoredText(string text, bool state) => state ? GreenText(text) : RedText(text);
        private string RedText(string text) => $"<color=red>({text})</color>";
        private string GreenText(string text) => $"<color=green>({text})</color>";
    }
}