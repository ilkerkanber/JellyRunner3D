using DG.Tweening;
using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] GameObject StateParent;
    [SerializeField] TextMeshProUGUI goldTmPro;
    [SerializeField] GameObject goldImage;
    [SerializeField] GameObject InstantiteGold2D;

    Vector3 fScaleGoldTmPro;
    int goldCount;

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
    }
    void FailGame()
    {
        SetCanvas(3);
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
    public void RestartButton()
    {
        SceneManager.LoadScene(0);
    }
}
