using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class DataManager
{
    public static void SaveData(GameData gameData)
    {
        string dataString = JsonUtility.ToJson(gameData);
        PlayerPrefs.SetString("data", dataString);
    }
    public static void LoadData(GameData gameData)
    {
        if (!PlayerPrefs.HasKey("data"))
        {
            SaveData(gameData);
            return;
        }

        string dataString = PlayerPrefs.GetString("data");
        JsonUtility.FromJsonOverwrite(dataString, gameData);
    }
}
