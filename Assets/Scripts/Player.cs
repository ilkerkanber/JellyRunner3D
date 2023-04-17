using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public List<Jelly> jellyList;
    [SerializeField] BigJelly bigJelly;
    public Mod mod;
    public enum Mod { Small,Big }

    [Header("Options")]
    [SerializeField] float speed;
    [SerializeField] Vector2 boundLimitsX;
    [SerializeField] float swerveSensitivity;

    [Header("Requireds")]
    public Transform SmallMod;
    public Transform BigMod;

    GameManager _gameManager;

    Vector3 firstPos;
    float difInputPosX;
    float resultTempPosX;
    
    void OnEnable()
    {
    }
    void OnDisable()
    {
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
        }

    }
    IEnumerator SetSmallMod()
    {
        float durationBigJelly = 0.5f;
        float durationSmallJelly = 0.3f;
        bigJelly.SmallModActivated(durationBigJelly);
        yield return new WaitForSeconds(durationBigJelly);
        BigMod.gameObject.SetActive(false);
        SmallMod.gameObject.SetActive(true);
        int centerPoint = 0;
        foreach (Jelly item in jellyList)
        {
            if (centerPoint == 0)
            {
                item.SeperateMod(Vector3.zero, durationSmallJelly);
            }
            else
            {
                float clambXL = Mathf.Clamp(boundLimitsX.x - transform.position.x, -2f, 2f);
                float clambXR= Mathf.Clamp(boundLimitsX.y - transform.position.x, -2f, 2f);
                float rndLocalPosX = Random.Range(clambXL,clambXR);
                float rndLocalPosZ = Random.Range(0, 1f);
                item.SeperateMod(new Vector3(rndLocalPosX, 0, rndLocalPosZ),durationSmallJelly);
            }
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
        bigJelly.BigModActivated(jellyList.Count,duration/2f);
        mod = Mod.Big;
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
