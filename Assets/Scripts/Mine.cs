using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Mine : MonoBehaviour
{
    List<GameObject> detectingJellies;
    public float bomDuration;
    bool startedBombCounter;
    void Awake()
    {
        detectingJellies = new List<GameObject>();
    }
    void StartBomb()
    {
        Vector3 fScale = transform.localScale;
        transform.DOScale(fScale * 2f, bomDuration).OnComplete(() => Bom());
    }
    void Bom()
    {
        ObjectManager.ParticleManager.MineBombParticle(transform.position);
        foreach (GameObject go in detectingJellies)
        {
            if(go.TryGetComponent<Jelly>(out Jelly jelly))
            {
                if (jelly != null)
                {
                    jelly.DumpDie();
                }
            }
            else if(go.TryGetComponent<BigJelly>(out BigJelly bigJelly))
            {
                if(bigJelly != null)
                {
                    bigJelly.Damaged();
                    break;
                }
            }
        }
        gameObject.SetActive(false);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Jelly"))
        {
            if (!startedBombCounter)
            {
                startedBombCounter = true;
                StartBomb();
            }
            if (!detectingJellies.Contains(other.gameObject))
            {
                detectingJellies.Add(other.gameObject);
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Jelly"))
        {
            if (!detectingJellies.Contains(other.gameObject))
            {
                detectingJellies.Remove(other.gameObject);
            }
        }
    }
}
