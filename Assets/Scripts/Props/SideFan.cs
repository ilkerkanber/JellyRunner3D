using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SideFan : MonoBehaviour
{
    public Vector3 forceDirection;
    List<Rigidbody> impactList;
    [SerializeField] float rotateSpeed;
    [SerializeField] Transform fan;
    float z;
    float forceTimer = 0;
    void Awake()
    {
        impactList = new List<Rigidbody>();
    }
    void Update()
    {
        z += Time.deltaTime * rotateSpeed;
        fan.transform.localRotation = Quaternion.EulerRotation(Vector3.forward * z);
        if (impactList.Count > 0)
        {
            forceTimer+= Time.deltaTime;    
            if(forceTimer > 0.5f)
            {
                foreach(Rigidbody item in impactList) 
                {
                    item.AddForce(forceDirection,ForceMode.Impulse);
                }
                forceTimer = 0f;
            }
        }
    }
    void OnTriggerEnter(Collider other)
    {
       if(other.TryGetComponent<Jelly>(out Jelly jelly))
        {
            Rigidbody rbJelly = jelly.GetComponent<Rigidbody>();
            Collider rbCol=jelly.GetComponent<Collider>();
            rbCol.isTrigger = false;
            rbJelly.isKinematic = false;
            rbJelly.constraints = RigidbodyConstraints.None;
            rbJelly.useGravity = true;
            jelly.SetNavMeshEnable(false);
            impactList.Add(rbJelly);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Jelly>(out Jelly jelly))
        {
            impactList.Remove(jelly.GetComponent<Rigidbody>());
            ObjectManager.Player.jellyList.Remove(jelly);
            ObjectManager.GameManager.FailControl();

        }
    }
}
