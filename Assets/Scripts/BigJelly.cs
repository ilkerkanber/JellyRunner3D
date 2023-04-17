using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BigJelly : AJelly
{
    public float size;
    void Start()
    {
        _gameManager = ObjectManager.GameManager;
        _particleManager = ObjectManager.ParticleManager;
        _player = ObjectManager.Player;
        mod = Mod.Run;
    }
    void Update()
    {
        if (_gameManager.state == GameManager.GameState.Playing)
        {
            if (mod == Mod.Run)
            {
                RunAnim();
            }
        }
    }
    public void SmallModActivated(float scaleDuration)
    {
        transform.DOScale(Vector3.one, scaleDuration);
    }
    public void BigModActivated(int jellyCount,float scaleDuration)
    {
        transform.localScale = Vector3.one;
        size = jellyCount * 0.5f;
        transform.DOScale(size, scaleDuration);
    }
    void OnCollisionEnter(Collision collision)
    {
        Collider col = collision.collider;

        if (col.CompareTag("Wall"))
        {
            JumpAnim();
            col.transform.root.GetComponent<Wall>().Break();
        }
    }
}
