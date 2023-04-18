using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BigJelly : AJelly
{
    public int extraPoint { get; private set; }
    public float size;
    SkinnedMeshRenderer meshRenderer;
    Color firstColor;
    bool InFinishMod;
    void Awake()
    {
        GetBasicComponents();
    }
    void Start()
    {
        meshRenderer=transform.GetChild(0).GetComponent<SkinnedMeshRenderer>();
        _gameManager = ObjectManager.GameManager;
        _particleManager = ObjectManager.ParticleManager;
        _player = ObjectManager.Player;
        mod = Mod.Run;
        firstColor=meshRenderer.material.color;
    }
    void OnEnable()
    {
        EventManager.LoseGame += DieScalerY;
    }
    void OnDisable()
    {
        EventManager.LoseGame -= DieScalerY;
    }
    void Update()
    {
        if (_gameManager.state == GameManager.GameState.Playing)
        {
            if (mod == Mod.Run)
            {
                RunAnim();
            }
            if (InFinishMod)
            {
                size -= Time.deltaTime;
                transform.localScale = Vector3.one * size;

                if (size < 1f)
                {
                    DieScalerY();
                    EventManager.WinGame();
                }
            }
            else
            {
                size = 1.5f + _player.jellyList.Count*0.3f;
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * size, Time.deltaTime * 10f);
            }
        }
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
        col.enabled = false;
        DamageColor();
        Big_DeadThornAnim();
        EventManager.LoseGame();
    }
    public void ResetLocalRotation()
    {
        transform.DOLocalRotateQuaternion(Quaternion.Euler(Vector3.zero), 0.2f);
    }
    void DieScalerY()
    {
        animator.enabled = false;
        transform.DOScaleY(0.1f, 0.4f);
    }
    public void Damaged()
    {
        DamageColor();
        _player.Damaged(1);
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
            Damaged();
        }
        else if (targetCol.CompareTag("ThornObstacle"))
        {
            DeadThorn();
        }
        else if (targetCol.CompareTag("Knife")|| targetCol.CompareTag("Axe"))
        {
            _particleManager.BloodDamageParticle(transform.position);
            Damaged();
        }
        else if (targetCol.CompareTag("IsNearDetector"))
        {
            _player.IsNearObstacle = true;
        }
        else if(targetCol.TryGetComponent<FinishCube>(out FinishCube finishCube))
        {
            DieScalerY();
            extraPoint = finishCube.value;
            finishCube.bloodImage.SetActive(true);
            EventManager.WinGame();
        }
        else if(targetCol.TryGetComponent<BadJelly>(out BadJelly badJelly))
        {
            badJelly.DieScalerY();
        }
        else if (targetCol.CompareTag("StartFinishMod"))
        {
            InFinishMod = true;
            EventManager.StartFinishMod();
        }
    }
    void OnTriggerExit(Collider targetCol)
    {
        if (targetCol.CompareTag("WallDetector_Big"))
        {
            Root_AnimEnd();
        }
        else if (targetCol.CompareTag("IsNearDetector"))
        {
            _player.IsNearObstacle = false;
        }
    }

}
