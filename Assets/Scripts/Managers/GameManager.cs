using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class GameManager : MonoBehaviour
{
    public GameState state;
    public enum GameState { 
        Start,Playing,End
    }
    void OnEnable()
    {
        EventManager.WinGame += StopGame;
        EventManager.LoseGame += StopGame;
    }
    void OnDisable()
    {
        EventManager.WinGame -= StopGame;
        EventManager.LoseGame -= StopGame;
    }
    void Awake()
    {
        ObjectManager.GameManager = this;
    }
    void Update()
    {
        if(state == GameState.Start)
        {
            if(Input.GetMouseButtonDown(0)) 
            {
                state = GameState.Playing;
            }
        }
    }
    public void FailControl()
    {
        if (ObjectManager.Player.jellyList.Count==0) 
        {
            state = GameState.End;
        }
    }
    void StopGame()
    {
       state=GameState.End;
    }
}
