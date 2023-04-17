using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] ParticleSystem wallDamageParticle;
    [SerializeField] ParticleSystem jellyMergeParticle;

    void Awake()
    {
        ObjectManager.ParticleManager = this;
    }
    public void WallDamageParticle(Vector3 pos)
    {
        ExecuteParticle(pos, wallDamageParticle);
    }
    public void JellyMergeParticle(Vector3 pos)
    {
        ExecuteParticle(pos, jellyMergeParticle);
    }
    void ExecuteParticle(Vector3 pos,ParticleSystem targetParticle)
    {
        Instantiate(targetParticle, pos, Quaternion.identity, transform);
    }
}
