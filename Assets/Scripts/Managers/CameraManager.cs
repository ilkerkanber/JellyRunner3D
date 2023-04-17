using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] float lerpSmoothValue;
    GameObject player;
    Vector3 difStartToPlayer;
    void Start()
    {
        player = ObjectManager.Player.gameObject;
        difStartToPlayer = player.transform.position - transform.position;
    }
    
    void Update()
    {
        WatchToPlayer(); 
    }
    void WatchToPlayer()
    {
        transform.position = Vector3.Lerp(transform.position, player.transform.position - difStartToPlayer, Time.deltaTime * lerpSmoothValue);
    }
}
