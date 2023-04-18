using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gold : MonoBehaviour
{
    [SerializeField] float rotateSpeed;
    bool collected;
    void Update()
    {
        if (!collected)
        {
            transform.Rotate(Vector3.up*rotateSpeed);
        }    
    }
    void OnTriggerEnter(Collider other)
    {
        if (collected)
        {
            return;
        }
        if(other.GetComponent<Jelly>() || other.GetComponent<BigJelly>())
        {
            collected = true;
            EventManager.EarnGold(transform.position);
            gameObject.SetActive(false);
        }
    }
}
