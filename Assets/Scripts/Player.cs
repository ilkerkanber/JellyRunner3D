using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    public List<Jelly> jellyList;
    [SerializeField] BigJelly bigJelly;
    public Mod mod;
    public enum Mod { Small,Big }

    [Header("Options")]
    [SerializeField] float speed;
    [SerializeField] Vector2 boundLimitsX;
    [SerializeField] Vector2 navMeshBoundLimitX;
    [SerializeField] float swerveSensitivity;

    [Header("Requireds")]
    public Transform SmallMod;
    public Transform BigMod;

    GameManager _gameManager;
    public bool IsNearObstacle;
    public bool IsJumping { get; private set; }
    public bool IsDropping{ get; set; }
    bool IsFinishMod;

    Vector3 firstPos;
    float difInputPosX;
    float resultTempPosX;
    bool IsConverting;
    bool clicked;
    void OnEnable()
    {
        EventManager.StartFinishMod += SetFinishMod;    
    }
    void OnDisable()
    {
        EventManager.StartFinishMod -= SetFinishMod;
    }
    void Awake()
    {
        ObjectManager.Player = this;
    }
    void Start()
    {
        _gameManager = ObjectManager.GameManager;
    }
    void Update()
    {
        InputController();
    }
    void InputController()
    {
        if(IsJumping) {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.EulerRotation(Vector3.zero), Time.deltaTime * 10f);
            clicked = false;
            return; }
        if (_gameManager.state == GameManager.GameState.Playing)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            if (!IsDropping)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    clicked = true;
                    firstPos = Input.mousePosition;
                }
                if (Input.GetMouseButton(0))
                {
                    if (!clicked)
                    {
                        clicked = true;
                        firstPos = Input.mousePosition;
                    }
                    difInputPosX = ((firstPos.x - Input.mousePosition.x) / -swerveSensitivity) / Screen.width;
                    resultTempPosX = Mathf.Clamp(transform.position.x + difInputPosX, boundLimitsX.x, boundLimitsX.y);
                    transform.position = new Vector3(resultTempPosX, transform.position.y, transform.position.z);
                    firstPos = Input.mousePosition;
                    if (mod == Mod.Big && !IsConverting && !IsFinishMod)
                    {
                        StartCoroutine(SetSmallMod());
                    }
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.EulerRotation(Vector3.up * difInputPosX * 5f), Time.deltaTime * 10f);
                }
                if (Input.GetMouseButtonUp(0) && !IsConverting)
                {
                    clicked = false;
                    difInputPosX = firstPos.x;
                    if (!IsFinishMod)
                    {
                        StartCoroutine(SetBigMod());
                    }
                }
                if (!clicked)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.EulerRotation(Vector3.zero), Time.deltaTime * 10f);
                }
            }
        }
    }
    IEnumerator SetSmallMod()
    {
        if (!IsNearObstacle)
        {
            IsConverting = true;
            mod = Mod.Small;
            float durationSmallJelly = 0.1f;
            ObjectManager.ParticleManager.JellyMergeParticle(transform.position);
            BigMod.gameObject.SetActive(false);
            SmallMod.gameObject.SetActive(true);
            int centerPoint = 0;
            foreach (Jelly item in jellyList)
            {
                float boundLeftX = navMeshBoundLimitX.x - transform.position.x;
                float boundRightX = navMeshBoundLimitX.y + transform.position.x;

                boundLeftX = Mathf.Clamp(boundLeftX, transform.position.x - 2f, transform.position.x);
                boundRightX = Mathf.Clamp(boundRightX, transform.position.x, transform.position.x + 2f);

                float rndPosX = Random.Range(boundLeftX, boundRightX);
                float rndPosZ = Random.Range(-1f, 0.3f);
                rndPosX = Mathf.Clamp(rndPosX, navMeshBoundLimitX.x, navMeshBoundLimitX.y);
                if (centerPoint == 0)
                {
                    item.SeperateMod(transform.position.x, transform.position.y, 0f, durationSmallJelly);
                }
                item.SeperateMod(rndPosX, transform.position.y, rndPosZ, durationSmallJelly);
                centerPoint++;
            }
            yield return new WaitForSeconds(durationSmallJelly);
            IsConverting = false;
        }
    }
    IEnumerator SetBigMod()
    {
        if(!IsNearObstacle)
        {
            IsConverting = true;
            mod = Mod.Big;
            float duration = 0.1f;
            foreach (Jelly item in jellyList)
            {
                item.MergeMod(duration);
            }
            yield return new WaitForSeconds(duration);
            ObjectManager.ParticleManager.JellyMergeParticle(transform.position);
            BigMod.gameObject.SetActive(true);
            SmallMod.gameObject.SetActive(false);
            bigJelly.transform.localScale = Vector3.one;
            IsConverting = false;
        }
    }
    #region Jumps
    public void StartedJumpMod_Small(Vector3 sPoint)
    {
        Sequence seq = DOTween.Sequence();
        IsJumping = true;
        foreach (Jelly item in jellyList)
        {
            item.ResetLocalRotation();
            item.SetNavMeshEnable(false);
        }
        transform.DOJump(sPoint, 20f, 2, 2f).OnComplete(()=>EndJumpMod_Small());
      
    }
    void EndJumpMod_Small()
    {
        IsJumping = false;
        foreach (Jelly item in jellyList)
        {
            item.SetNavMeshEnable(true);
        }
        IsNearObstacle = false;
    }
    public void StartedJumpMod_Big(Vector3 tPoint, NavMeshAgent bJellAgent)
    {
        bigJelly.ResetLocalRotation();
        IsJumping = true;
        bJellAgent.enabled = false;
        transform.DOJump(tPoint, 9, 1, 2f).SetEase(Ease.Linear).OnComplete(() =>EndJumpMod_Big(bJellAgent));
    }
    void EndJumpMod_Big(NavMeshAgent bJellAgent)
    {
        bJellAgent.enabled = true;
        IsJumping = false;
        IsNearObstacle = false;
    }
    public void StartedDroppingMOD(float targetY,float duration)
    {
        transform.rotation = Quaternion.Euler(Vector3.zero);
        IsDropping = true;
        foreach (Jelly item in jellyList)
        {
            item.ResetLocalRotation();
            item.SetNavMeshEnable(false);
        }
        transform.DOMoveY(targetY, duration).OnComplete(() =>EndDroppingMOD());
    }
    void EndDroppingMOD()
    {
        IsDropping = false;
        foreach (Jelly item in jellyList)
        {
            item.SetNavMeshEnable(true);
        }
        IsNearObstacle = false;
    }
    #endregion

    void SetFinishMod()
    {
        IsFinishMod = true;
        StartCoroutine(SetBigMod());
    }
    public void Damaged(int damageCount)
    {
        for (int i = 0; i < damageCount; i++)
        {
            Jelly damagedJelly = jellyList[0];
            damagedJelly.Dead();
            damagedJelly.gameObject.SetActive(false);
        }
    }
    public bool GetListControl(Jelly currentJelly, Jelly collisionJelly)
    {
        if (!jellyList.Contains(currentJelly) && jellyList.Contains(collisionJelly))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
