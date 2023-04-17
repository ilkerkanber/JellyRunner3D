using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Experimental.GraphView.GraphView;

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

    public bool IsJumping { get; private set; }
    Vector3 firstPos;
    float difInputPosX;
    float resultTempPosX;
    
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
        if(IsJumping) { return; }
        if (_gameManager.state == GameManager.GameState.Playing)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            if (Input.GetMouseButtonDown(0))
            {
                firstPos = Input.mousePosition;
            }
            if (Input.GetMouseButton(0))
            {
                difInputPosX = ((firstPos.x - Input.mousePosition.x) / -swerveSensitivity) / Screen.width;
                resultTempPosX = Mathf.Clamp(transform.position.x + difInputPosX, boundLimitsX.x, boundLimitsX.y);
                transform.position = new Vector3(resultTempPosX, transform.position.y, transform.position.z);
                firstPos = Input.mousePosition;
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (mod == Mod.Small)
                {
                    StartCoroutine(SetBigMod());
                }
                else
                {
                    StartCoroutine(SetSmallMod());
                }
            }
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.EulerRotation(Vector3.up * difInputPosX * 8f), Time.deltaTime * 5f);
        }
    }
    IEnumerator SetSmallMod()
    {
        float durationBigJelly = 0.3f;
        float durationSmallJelly = 0.15f;
        bigJelly.SmallModActivated(durationBigJelly);
        yield return new WaitForSeconds(durationBigJelly);
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
            float rndPosZ = Random.Range(transform.position.z - 2f, transform.position.z);
            rndPosX = Mathf.Clamp(rndPosX, navMeshBoundLimitX.x, navMeshBoundLimitX.y);
            if (centerPoint == 0)
            {
                rndPosX = transform.position.x;
                rndPosZ = transform.position.z;

            }
            item.SeperateMod(new Vector3(rndPosX, 0, rndPosZ), durationSmallJelly);
            centerPoint++;
        }
        mod = Mod.Small;
    }
    IEnumerator SetBigMod()
    {
        float duration = 0.5f;
        foreach (Jelly item in jellyList)
        {
            item.MergeMod(duration);
        }
        yield return new WaitForSeconds(duration);
        BigMod.gameObject.SetActive(true);
        SmallMod.gameObject.SetActive(false);
        bigJelly.BigModActivated(duration/2f);
        mod = Mod.Big;
    }
    #region Jumps
    public void StartedJumpMod_Small(Vector3 sPoint)
    {
        Sequence seq = DOTween.Sequence();
        IsJumping = true;
        foreach (Jelly item in jellyList)
        {
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
    }
    public void StartedJumpMod_Big(Vector3 tPoint,NavMeshAgent bJellAgent)
    {
        IsJumping = true;
        bJellAgent.enabled = false;
        transform.DOJump(tPoint, 9, 1, 2f).SetEase(Ease.Linear).OnComplete(() =>EndJumpMod_Big(bJellAgent));
    }
    void EndJumpMod_Big(NavMeshAgent bJellAgent)
    {
        bJellAgent.enabled = true;
        IsJumping = false;
    }
    #endregion

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
