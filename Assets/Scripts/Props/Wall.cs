using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField] Transform brickGroup;
    public void Break()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            brickGroup.GetChild(i).GetComponent<Rigidbody>().isKinematic = false;
        }    
    }
}
