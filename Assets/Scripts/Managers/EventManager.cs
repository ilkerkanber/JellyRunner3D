using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager 
{
    public static Action StartGame;
    public static Action LoseGame;
    public static Action WinGame;

    public static Action StartFinishMod;
    public static Action<Vector3> EarnGold;
}
