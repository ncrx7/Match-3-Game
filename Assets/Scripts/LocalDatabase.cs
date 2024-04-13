using UnityEngine;

public static class LocalDatabase
{
    public static bool WelcomePanelIsActive
    {
        get => PlayerPrefs.GetInt("WelcomePanelIsActive", 1) == 1;
        set => PlayerPrefs.SetInt("WelcomePanelIsActive", value ? 1 : 0);
    }
    
    public static bool TestUser
    {
        get => PlayerPrefs.GetInt("TestUser") == 1;
        set => PlayerPrefs.SetInt("TestUser", value ? 1 : 0);
    }
}