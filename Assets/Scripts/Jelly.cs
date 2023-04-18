using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

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
    public void SeperateMod(float posX,float posY,float localPosZ, float duration)
    {
        transform.DOMoveX(posX,duration);
        transform.DOMoveY(posY, duration);
        transform.DOLocalMoveZ(localPosZ, duration+0.02f).OnComplete(()=>SeperateCompleted());
    }
    void SeperateCompleted()
    {
        ResetLocalRotation();
        agent.enabled = true;
        col.enabled = true;
    }
    public void MergeMod(float duration)
    {
        agent.enabled = false;
        col.enabled = false;
        transform.DOLocalJump(new Vector3(0,transform.localPosition.y,0f), 1, 1, duration);
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
            ResetLocalRotation();
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
    void DieScalerY()
    {
        animator.enabled = false;
        transform.DOScaleY(0.1f, 0.4f);
    }
    public void Dead()
    {
        col.enabled = false;
        InPlayer = false;
        transform.parent = null;
        rb.isKinematic = true;
        agent.enabled = false;
        _player.jellyList.Remove(this);
        _gameManager.FailControl();
    }
    public void SetNavMeshEnable(bool st)
    {
        agent.enabled = st;
    }
    public void RFeetSplash()
    {
        if (_player.IsJumping || _player.IsDropping)
        {
            return;
        }
        _particleManager.GetFeetSplash(RFeetTransform.position);
    }
    public void LFeetSplash()
    {
        if (_player.IsJumping || _player.IsDropping)
        {
            return;
        }
        _particleManager.GetFeetSplash(LFeetTransform.position);
    }
    public void ResetLocalRotation()
    {
        transform.DOLocalRotateQuaternion(Quaternion.Euler(Vector3.zero), 0.2f);
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
        else if (col.CompareTag("FanJumperDetector"))
        {
            if (!_player.IsJumping)
            {
                col.transform.root.TryGetComponent<Fan>(out Fan fan);
                _player.StartedJumpMod_Small(fan.smallJelly_LastPoint.position);
            }
        }
        else if (col.CompareTag("Knife") || col.CompareTag("Axe"))
        {
            Dead();
            DieScalerY();
            _particleManager.BloodDamageParticle(transform.position);
        }
    }
}
