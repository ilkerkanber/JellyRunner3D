using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class GameManager : MonoBehaviour
{
    public GameState state;
    public enum GameState { 
        Tutorial,Playing,End
    }
    void OnEnable()
    {
        EventManager.StartGame += StartGame;
        EventManager.WinGame += StopGame;
        EventManager.LoseGame += StopGame;
    }
    void OnDisable()
    {
        EventManager.StartGame -= StartGame;
        EventManager.WinGame -= StopGame;
        EventManager.LoseGame -= StopGame;
    }
    void Awake()
    {
        ObjectManager.GameManager = this;
    }
    void Update()
    {
        if(state == GameState.Tutorial)
        {
            if(Input.GetMouseButtonDown(0)) 
            {
                EventManager.StartGame();
            }
        }
    }
    public void FailControl()
    {
        if (ObjectManager.Player.jellyList.Count==0) 
        {
            EventManager.LoseGame();
        }
    }
    void StartGame()
    {
       state = GameState.Playing;
    }
    void StopGame()
    {
       state = GameState.End;
    }
}
