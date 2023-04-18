using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    Queue<GameObject> feetSplashPool;
    [SerializeField] ParticleSystem wallDamageParticle;
    [SerializeField] ParticleSystem jellyMergeParticle;
    [SerializeField] ParticleSystem lavaParticle;
    [SerializeField] ParticleSystem punchParticle;
    [SerializeField] ParticleSystem bloodParticle;
    [SerializeField] ParticleSystem goldEarnParticle;
    [SerializeField] ParticleSystem mineParticle;

    [SerializeField] GameObject feetSplash;
    [Space]
    [SerializeField] ParticleSystem failParticle;
    void OnEnable()
    {
        EventManager.LoseGame += FailParticle;
        EventManager.EarnGold += EarnGoldParticle;
    }
    void OnDisable()
    {
        EventManager.LoseGame -= FailParticle;
        EventManager.EarnGold -= EarnGoldParticle;
    }
    void Awake()
    {
        ObjectManager.ParticleManager = this;
    }
    void Start()
    {
        feetSplashPool= new Queue<GameObject>();
        InstantiateFeetSplash(200);
    }
    public void WallDamageParticle(Vector3 pos)
    {
        ExecuteParticle(pos, wallDamageParticle);
    }
    public void JellyMergeParticle(Vector3 pos)
    {
        ExecuteParticle(pos, jellyMergeParticle);
    }
    public void LavaParticle(Vector3 pos)
    {
        ExecuteParticle(pos, lavaParticle);
    }
    public void PunchParticle(Vector3 pos)
    {
        ExecuteParticle(pos, punchParticle);
    }
    public void BloodDamageParticle(Vector3 pos)
    {
        ExecuteParticle(pos, bloodParticle);
    }
    public void MineBombParticle(Vector3 pos)
    {
        ExecuteParticle(pos, mineParticle);
    }
    void EarnGoldParticle(Vector3 pos)
    {
        ExecuteParticle(pos, goldEarnParticle);
    }
    void FailParticle()
    {
        ExecuteParticle(ObjectManager.Player.transform.position + Vector3.up*2f, failParticle);
    }
    void InstantiateFeetSplash(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject cr = Instantiate(feetSplash, Vector3.zero, Quaternion.identity, transform);
            feetSplashPool.Enqueue(cr);
            cr.SetActive(false);
        }
    }
    public void GetFeetSplash(Vector3 pos)
    {
        if(feetSplashPool.Count == 0)
        {
            InstantiateFeetSplash(50);
        }
        GameObject get = feetSplashPool.Dequeue();
        get.SetActive(true);
        get.transform.position = pos+Vector3.up*0.2f;
        StartCoroutine(SetFeetSplashToPool(get));
    }
    IEnumerator SetFeetSplashToPool(GameObject feetObj)
    {
        yield return new WaitForSeconds(3f);
        feetObj.SetActive(false);
        feetSplashPool.Enqueue(feetObj);
    }
    void ExecuteParticle(Vector3 pos,ParticleSystem targetParticle)
    {
        Instantiate(targetParticle, pos, Quaternion.identity, transform);
    }
}
