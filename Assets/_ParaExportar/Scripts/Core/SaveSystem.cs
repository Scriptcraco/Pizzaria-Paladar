using UnityEngine;

public static class SaveSystem
{
    private const string KeyPlayerName = "pp_playerName";
    private const string KeyGender = "pp_gender";

    public static void NewProfile(string playerName, string gender)
    {
        Save(playerName, gender);
    }

    public static void Save(string playerName, string gender)
    {
        PlayerPrefs.SetString(KeyPlayerName, playerName);
        PlayerPrefs.SetString(KeyGender, gender);
        PlayerPrefs.Save();
    }

    public static SaveData Load()
    {
        if (!PlayerPrefs.HasKey(KeyPlayerName)) return null;
        return new SaveData
        {
            playerName = PlayerPrefs.GetString(KeyPlayerName, ""),
            gender = PlayerPrefs.GetString(KeyGender, "")
        };
    }
}

public class SaveData
{
    public string playerName;
    public string gender;
}
