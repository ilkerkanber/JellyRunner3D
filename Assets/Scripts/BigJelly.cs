using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BigJelly : AJelly
{
    public float size;
    SkinnedMeshRenderer meshRenderer;
    Color firstColor;
    void Start()
    {
        meshRenderer=transform.GetChild(0).GetComponent<SkinnedMeshRenderer>();
        _gameManager = ObjectManager.GameManager;
        _particleManager = ObjectManager.ParticleManager;
        _player = ObjectManager.Player;
        mod = Mod.Run;
        firstColor=meshRenderer.material.color;
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
            size = 1.5f + _player.jellyList.Count*0.3f;
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
        size = 1.5f + _player.jellyList.Count * 0.3f;
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
    void DamageColor()
    {
        if(_gameManager.state == GameManager.GameState.Playing) 
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(meshRenderer.material.DOColor(Color.black, 0.3f));
            seq.Append(meshRenderer.material.DOColor(firstColor, 0.3f));
        }
    }
    void DeadThorn()
    {
        DamageColor();
        Big_DeadThornAnim();
        EventManager.LoseGame();
    }
    public void ResetLocalRotation()
    {
        transform.DOLocalRotateQuaternion(Quaternion.Euler(Vector3.zero), 0.2f);
    }
    void OnTriggerEnter(Collider targetCol)
    {
        if (targetCol.CompareTag("WallDetector_Big"))
        {
            JumpAnim();
            Root_AnimStarted();
        }
        else if (targetCol.CompareTag("FanJumperDetector"))
        {
            Vector3 targetPos = targetCol.transform.root.GetComponent<Fan>().bigJelly_LastPoint.position;
            _player.StartedJumpMod_Big(targetPos,agent);
            FallAnim();
        }
        else if (targetCol.CompareTag("Lava"))
        {
            _particleManager.LavaParticle(targetCol.ClosestPoint(transform.position)+Vector3.up);
            _player.Damaged(1);
            DamageColor();
        }
        else if (targetCol.CompareTag("ThornObstacle"))
        {
            DeadThorn();
        }
        else if (targetCol.CompareTag("Knife")|| targetCol.CompareTag("Axe"))
        {
            DamageColor();
            _player.Damaged(1);
            _particleManager.BloodDamageParticle(transform.position);
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
