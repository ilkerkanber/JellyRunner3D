using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceObstacle : MonoBehaviour
{
    public float targetY;
    public float duration;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Jelly>(out Jelly jelly) && !ObjectManager.Player.IsJumping)
        {
            ObjectManager.Player.StartedDroppingMOD(targetY, duration);
        }
    }
}
