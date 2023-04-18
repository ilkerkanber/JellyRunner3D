using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] float lerpSmoothValue;
    GameObject player;
    Vector3 difStartToPlayer;
    public Camera cam { get; private set; }
    private void Awake()
    {
        ObjectManager.CameraManager = this;
    }
    void Start()
    {
        player = ObjectManager.Player.gameObject;
        difStartToPlayer = player.transform.position - transform.position;
        cam=transform.GetChild(0).GetComponent<Camera>();
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
