using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : MonoBehaviour
{
    [SerializeField] float rotateSpeed;
    [SerializeField] Transform fan;
    public Transform smallJelly_LastPoint;
    public Transform bigJelly_LastPoint;
    public Transform LavaPoint;

    void Update()
    {
        fan.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }
}
