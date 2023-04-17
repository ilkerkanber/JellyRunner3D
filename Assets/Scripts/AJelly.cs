using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class AJelly : MonoBehaviour
{
    public Mod mod;
    public enum Mod { Idle, Run, Jump, Die };
    protected Animator animator;
    protected Rigidbody rb;
    protected Collider col;
    protected NavMeshAgent agent;

    protected GameManager _gameManager;
    protected ParticleManager _particleManager;
    protected Player _player;
    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        agent = GetComponent<NavMeshAgent>();
    }
    #region Animations
    protected void IdleAnim()
    {
        animator.SetBool("Run", false);
    }
    protected void RunAnim()
    {
        animator.SetBool("Run", true);
    }
    protected void JumpAnim()
    {
        animator.SetBool("Jump", true);
    }
    protected void WallDeadAnim()
    {
        animator.SetTrigger("WallDead");
    }
    protected void FallAnim()
    {
        animator.SetTrigger("Fall");
    }
    #endregion
}
