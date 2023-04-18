using DG.Tweening;
using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasManager : MonoBehaviour
{
    public GameData GameData;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] GameObject StateParent;
    [SerializeField] TextMeshProUGUI goldTmPro;
    [SerializeField] GameObject goldImage;
    [SerializeField] GameObject InstantiteGold2D;
    [Header("ForWin")]
    [SerializeField] TextMeshProUGUI bonusXTmPro;
    [SerializeField] TextMeshProUGUI resultTmPro;
    [SerializeField] Transform moneyGroupTransform;
    [SerializeField] List<GameObject> moneyImageList;
    Vector3 fScaleGoldTmPro;
    int goldCount;
    int lastPoint;
    void OnEnable()
    {
        EventManager.StartGame += InGame;
        EventManager.WinGame += WinGame;
        EventManager.LoseGame += FailGame;
        EventManager.EarnGold += GoldSend;
    }
    void OnDisable()
    {
        EventManager.StartGame -= InGame;
        EventManager.WinGame-= WinGame;
        EventManager.LoseGame -= FailGame;
        EventManager.EarnGold -= GoldSend;
    }
    void Awake()
    {
        fScaleGoldTmPro = goldTmPro.transform.localScale;   
    }
    void Start()
    {
        TutorialGame();
        DataManager.LoadData(GameData);
        goldCount = GameData.Money;
        goldTmPro.text = GameData.Money.ToString();
        levelText.text = GameData.Level.ToString();
    }
    void ActivateOnlyCanvas(int i)
    {
        StateParent.transform.GetChild(i).gameObject.SetActive(true);
    }
    void SetCanvas(int on)
    {
        for (int i = 0; i < StateParent.transform.childCount; i++)
        {
            StateParent.transform.GetChild(i).gameObject.SetActive(false);
        }
        StateParent.transform.GetChild(on).gameObject.SetActive(true);
    }
    void TutorialGame()
    {
        SetCanvas(0);
    }
    void InGame()
    {
        SetCanvas(1);
    }
    void WinGame()
    {
        SetCanvas(2);
        ActivateOnlyCanvas(1);
        StartCoroutine(WinGoldSend());
    }
    void FailGame()
    {
        SetCanvas(3);
    }
    IEnumerator WinGoldSend()
    {
        int extraBonus = ObjectManager.Player.bigJelly.extraPoint;
        bonusXTmPro.text ="x"+extraBonus.ToString();
        int totalPoint = ObjectManager.Player.jellyList.Count*extraBonus;
        resultTmPro.text=totalPoint.ToString();
        yield return new WaitForSeconds(1f);
        moneyGroupTransform.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        int child= moneyGroupTransform.childCount;
        for (int i = 0; i < child; i++)
        {
            Transform curTr = moneyGroupTransform.GetChild(0).transform;
            curTr.parent = goldImage.transform;
            curTr.DOLocalMove(Vector3.zero, 0.5f).OnComplete(()=>curTr.gameObject.SetActive(false));
        }
        yield return new WaitForSeconds(0.5f);
        lastPoint = goldCount + totalPoint;
        goldTmPro.text = lastPoint.ToString();
    }
    void GoldSend(Vector3 pos)
    {
        Vector3 canvasPos = ObjectManager.CameraManager.cam.WorldToScreenPoint(pos);
        GameObject createdGold2D = Instantiate(InstantiteGold2D, canvasPos, Quaternion.identity,transform);
        createdGold2D.transform.parent = goldImage.transform;
        createdGold2D.transform.localScale = Vector3.one;
        createdGold2D.transform.DOLocalMove(Vector3.zero, 0.25f).OnComplete(() => GoldArrived(createdGold2D));
    }
    void GoldArrived(GameObject gold)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(goldTmPro.transform.DOScale(fScaleGoldTmPro * 1.3f, 0.2f));
        seq.Append(goldTmPro.transform.DOScale(fScaleGoldTmPro, 0.2f));
        goldCount++;
        goldTmPro.text = goldCount.ToString();
        Destroy(gold);
    }
    public void NextButton()
    {
        GameData.Money = lastPoint;
        if (SceneManager.sceneCountInBuildSettings-1 == GameData.Level)
        {
            GameData.Level = 1;
        }
        else
        {
            GameData.Level++;
        }
        DataManager.SaveData(GameData);
        SceneManager.LoadScene(GameData.Level);
    }
    public void RestartButton()
    {
        SceneManager.LoadScene(GameData.Level);
    }
}
