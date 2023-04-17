using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Jelly : AJelly
{
    public bool InPlayer;

    void Start()
    {
        _gameManager = ObjectManager.GameManager;
        _particleManager = ObjectManager.ParticleManager;
        _player = ObjectManager.Player;
        if (InPlayer)
        {
            mod = Mod.Run;
        }
    }
    void Update()
    {
        if(_gameManager.state == GameManager.GameState.Playing && InPlayer)
        {
            if(mod == Mod.Run) 
            {
                RunAnim();
            }
            else if(mod == Mod.Jump)
            {
                JumpAnim();
            }
        }
    }
    public void SeperateMod(Vector3 localPos,float duration)
    {
        agent.enabled = true;
        col.enabled = true;
        transform.DOLocalMove(localPos, duration);
    }
    public void MergeMod(float duration)
    {
        agent.enabled = false;
        col.enabled = false;
        transform.DOLocalJump(Vector3.zero, 1, 1, duration);
    }
    void JoinToPlayer()
    {
        if (_player.mod== Player.Mod.Small)
        {
            _player.jellyList.Add(this);
            InPlayer = true;
            transform.parent = ObjectManager.Player.SmallMod;
            mod = Mod.Run;
            Blob();
        }
    }
    void Blob()
    {
        Vector3 fScale = transform.localScale;
        Sequence blobSequence = DOTween.Sequence();
        blobSequence.Append(transform.DOScale(fScale * 1.5f, 0.3f));
        blobSequence.Append(transform.DOScale(fScale, 0.3f));
    }
   
    void Dead()
    {
        transform.parent = null;
        rb.isKinematic = true;
        agent.enabled = false;
        col.isTrigger = true;
        _player.jellyList.Remove(this);
        _gameManager.FailControl();
    }
    void OnCollisionEnter(Collision collision)
    {
        Collider col = collision.collider;
        if (col.CompareTag("Jelly"))
        {
            if (_player.GetListControl(this, collision.collider.GetComponent<Jelly>()))
            {
                JoinToPlayer();
            }
        }
        else if (col.CompareTag("Wall"))
        {
            _particleManager.WallDamageParticle(transform.position + Vector3.up);
            WallDeadAnim();
            Dead();
        }
    }
}
