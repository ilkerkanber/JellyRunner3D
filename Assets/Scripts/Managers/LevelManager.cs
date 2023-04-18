using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public GameData GameData;
    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex != GameData.Level)
        {
            SceneManager.LoadScene(GameData.Level);
        }
    }
}
