using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThornObstacle : MonoBehaviour
{
    [SerializeField] float rotateSpeed;
    [SerializeField] Transform thorn;

    void Update()
    {
        thorn.Rotate(Vector3.right * rotateSpeed * Time.deltaTime);
    }
}
