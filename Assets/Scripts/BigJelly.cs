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
        if(Input.GetKeyDown(KeyCode.Space))
        {
            JumpAnim();
        }
        if (_gameManager.state == GameManager.GameState.Playing)
        {
            if (mod == Mod.Run)
            {
                RunAnim();
            }
            size = _player.jellyList.Count*0.5f;
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * size, Time.deltaTime * 10f);
        }
    }
    public void SmallModActivated(float scaleDuration)
    {
        transform.DOScale(Vector3.one, scaleDuration);
    }
    public void BigModActivated(float scaleDuration)
    {
        transform.localScale = Vector3.one;
        size = _player.jellyList.Count*0.5f;
        transform.DOScale(size, scaleDuration);
    }
    void Root_AnimStarted()
    {
        agent.enabled = false;
        animator.applyRootMotion = true;
        col.isTrigger = false;
    }
    void Root_AnimEnd()
    {
        animator.applyRootMotion = false;
        col.isTrigger = true;
        agent.enabled = true;
    }
    void OnTriggerEnter(Collider targetCol)
    {
        if (targetCol.CompareTag("WallDetector_Big"))
        {
            JumpAnim();
            Root_AnimStarted();
        }
        else if (targetCol.CompareTag("FanDetector"))
        {
            Vector3 targetPos = targetCol.transform.root.GetComponent<Fan>().LavaPoint.position;
            _player.StartedJumpMod_Big(targetPos,agent);
            FallAnim();
        }
        else if (targetCol.CompareTag("Lava"))
        {
            _particleManager.LavaParticle(targetCol.ClosestPoint(transform.position)+Vector3.up);
            _player.Damaged(1);
        }
    }
    void OnTriggerExit(Collider targetCol)
    {
        if (targetCol.CompareTag("WallDetector_Big"))
        {
            Root_AnimEnd();
        }
    }

}
