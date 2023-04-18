using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadJellyHand : MonoBehaviour
{
    public List<Jelly> trigJellys;
    void Awake()
    {
        trigJellys = new List<Jelly>();
    }
    public void PunchActivated()
    {
        foreach (Jelly item in trigJellys)
        {
            ObjectManager.ParticleManager.BadJellyPunchParticle(item.transform.position);
            item.DumpDie();
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Jelly>(out Jelly jelly))
        {
            if (!trigJellys.Contains(jelly))
            {
                trigJellys.Add(jelly);
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Jelly>(out Jelly jelly))
        {
            if (trigJellys.Contains(jelly))
            {
                trigJellys.Remove(jelly);
            }
        }
    }
}
