using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Jelly : AJelly
{
    public bool InPlayer;
    public Transform RFeetTransform;
    public Transform LFeetTransform;

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
    #region MODS
    public void SeperateMod(Vector3 pos,float duration)
    {
        transform.DOMove(pos, duration).OnComplete(()=>SeperateCompleted());
    }
    void SeperateCompleted()
    {
        transform.rotation = Quaternion.EulerRotation(Vector3.zero);
        agent.enabled = true;
        col.enabled = true;
    }
    public void MergeMod(float duration)
    {
        agent.enabled = false;
        col.enabled = false;
        transform.DOLocalJump(Vector3.zero, 1, 1, duration);
    }
    #endregion
    void JoinToPlayer()
    {
        if (_player.mod == Player.Mod.Small)
        {
            _player.jellyList.Add(this);
            gameObject.layer = 3;
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
    public void Dead()
    {
        transform.parent = null;
        rb.isKinematic = true;
        agent.enabled = false;
        col.isTrigger = true;
        _player.jellyList.Remove(this);
        _gameManager.FailControl();
    }
    public void SetNavMeshEnable(bool st)
    {
        agent.enabled = st;
    }
    public void RFeetSplash()
    {
        if (_player.IsJumping)
        {
            return;
        }
        _particleManager.GetFeetSplash(RFeetTransform.position);
    }
    public void LFeetSplash()
    {
        if (_player.IsJumping)
        {
            return;
        }
        _particleManager.GetFeetSplash(LFeetTransform.position);
    }
    void OnTriggerEnter(Collider col)
    {
        if (_gameManager.state == GameManager.GameState.End)
        {
            return;
        }
        if (col.CompareTag("Jelly"))
        {
            if (_player.GetListControl(this, col.GetComponent<Jelly>()))
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
        else if (col.CompareTag("FanDetector"))
        {
            if (!_player.IsJumping)
            {
                col.transform.root.TryGetComponent<Fan>(out Fan fan);
                _player.StartedJumpMod_Small(fan.JumpLastPoint.position);
            }
        }
    }
}
