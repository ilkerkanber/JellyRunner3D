using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadJelly : MonoBehaviour
{
    public BadJellyHand handClass;
    Animator animator;
    Collider col;
    bool IsDead;
    void Awake()
    {
        animator = GetComponent<Animator>();    
        col = GetComponent<Collider>();
    }
    public void DieScalerY()
    {
        IsDead = true;
        ObjectManager.ParticleManager.BloodDamageParticle_Black(transform.position+Vector3.up/2f);
        col.enabled = false;
        animator.enabled = false;
        transform.DOScaleY(0.1f, 0.4f);
    }
    void Update()
    {
        if (IsDead) { return; }
        if(Physics.Raycast(transform.position,transform.forward*2f,out RaycastHit hit))
        {
            if (hit.collider.GetComponent<Jelly>()){
                animator.SetTrigger("Punch");   
            }
        }    
    }
    public void PunchedNow()
    {
        if (IsDead)
        {
            return;
        }
        handClass.PunchActivated();
    }

}
