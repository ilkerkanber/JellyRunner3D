using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "GameFiles/Game Data")]
public class GameData : ScriptableObject
{
    public int Level;
    public int Money;
}