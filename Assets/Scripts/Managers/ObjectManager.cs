using DG.Tweening.Core.Easing;
using System;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.Playables;

public static class ObjectManager
{
    #region Managers
    public static CameraManager CameraManager;
    public static GameManager GameManager;
    public static CanvasManager CanvasManager;
    public static ParticleManager ParticleManager;
    #endregion

    #region GameElements
    public static Player Player;
    #endregion
}
