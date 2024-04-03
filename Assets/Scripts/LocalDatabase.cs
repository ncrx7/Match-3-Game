using UnityEngine;

public static class LocalDatabase
{
    public static bool welcomePanelIsActive
    {
        get => PlayerPrefs.GetInt("WelcomePanelIsActive", 1) == 1;
        set => PlayerPrefs.SetInt("WelcomePanelIsActive", value ? 1 : 0);
    }
    
    public static bool testUser
    {
        get => PlayerPrefs.GetInt("TestUser") == 1;
        set => PlayerPrefs.SetInt("TestUser", value ? 1 : 0);
    }
}