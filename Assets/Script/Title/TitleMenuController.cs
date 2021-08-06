using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleMenuController : MonoBehaviour
{
    public enum TargetVegetable
    {
        Broccoli,
        Cabbage,
        Paprika,
        Carrot,
        Pumpkin,
        Corn
    }

    [Header("パズルモード開始ボタン")]
    public GameObject pouzzleModeObject;
    private Button pouzzleModeButton;
    public GameObject[] pouzzleStageObject;
    private Button[] pouzzleStageButton;
    [Header("選択画面に戻るボタン")]
    public GameObject selectBuckObject;
    private Button selectBackButton;
    [Header("SoundManager")]
    public SoundManager soundMan;
    private SaveDataManager saveMan;
    [Header("ハムスターの親オブジェクト")]
    public Transform hamsterBox;
    [Header("風船ハムスター")]
    public GameObject hamsterPre;

    private int displayStageNum;  //表示するステージ番号

    // Start is called before the first frame update
    void Start()
    {
        saveMan = GameObject.FindWithTag("SaveDataManager").GetComponent<SaveDataManager>();
        saveMan.PuzzleModeLoadData();
        displayStageNum = saveMan.puzzelModeStageNum;
        pouzzleModeButton = pouzzleModeObject.GetComponent<Button>();
        selectBackButton = selectBuckObject.GetComponent<Button>();
        pouzzleStageButton = new Button[pouzzleStageObject.Length];
        for (int i = 0; i < pouzzleStageObject.Length; i++)
        {
            pouzzleStageButton[i] = pouzzleStageObject[i].GetComponent<Button>();
        }

        //ボタンに関数を追加
        pouzzleStageButton[0].onClick.AddListener(() =>
        OnClickGameStart(new TargetVegetable[] { TargetVegetable.Broccoli, TargetVegetable.Carrot }, 2, new int[] { 6, 5 }, 50, 0));
        pouzzleStageButton[1].onClick.AddListener(() =>
        OnClickGameStart(new TargetVegetable[] { TargetVegetable.Carrot }, 1, new int[] { 6 }, 20, 1));
        pouzzleStageButton[2].onClick.AddListener(() =>
        OnClickGameStart(new TargetVegetable[] { TargetVegetable.Cabbage, TargetVegetable.Paprika }, 2, new int[] { 8, 8 }, 20, 2));
        pouzzleStageButton[3].onClick.AddListener(() =>
        OnClickGameStart(new TargetVegetable[] { TargetVegetable.Cabbage }, 1, new int[] { 15 }, 10, 3));
        pouzzleStageButton[4].onClick.AddListener(() =>
        OnClickGameStart(new TargetVegetable[] { TargetVegetable.Broccoli, TargetVegetable.Paprika, TargetVegetable.Cabbage }, 3, new int[] { 10, 10, 20 }, 15, 4));
        pouzzleStageButton[5].onClick.AddListener(() =>
        OnClickGameStart(new TargetVegetable[] { TargetVegetable.Carrot, TargetVegetable.Paprika, TargetVegetable.Broccoli }, 3, new int[] { 20, 30, 35 }, 25, 5));
        pouzzleStageButton[6].onClick.AddListener(() =>
        OnClickGameStart(new TargetVegetable[] { TargetVegetable.Broccoli, TargetVegetable.Carrot }, 2, new int[] { 30, 20 }, 13, 6));
        pouzzleStageButton[7].onClick.AddListener(() =>
        OnClickGameStart(new TargetVegetable[] { TargetVegetable.Cabbage, TargetVegetable.Broccoli, TargetVegetable.Carrot }, 3, new int[] { 25, 25, 25 }, 15, 7));
        pouzzleStageButton[8].onClick.AddListener(() =>
        OnClickGameStart(new TargetVegetable[] { TargetVegetable.Paprika }, 1, new int[] { 30 }, 8, 8));
        pouzzleStageButton[9].onClick.AddListener(() =>
        OnClickGameStart(new TargetVegetable[] { TargetVegetable.Paprika, TargetVegetable.Cabbage, TargetVegetable.Broccoli, TargetVegetable.Pumpkin }, 4, new int[] { 25, 25, 25, 25 }, 20, 9));
        pouzzleStageButton[10].onClick.AddListener(() =>
        OnClickGameStart(new TargetVegetable[] { TargetVegetable.Carrot, TargetVegetable.Cabbage, TargetVegetable.Pumpkin }, 3, new int[] { 10, 10, 10 }, 5, 10));
        pouzzleStageButton[11].onClick.AddListener(() =>
        OnClickGameStart(new TargetVegetable[] { TargetVegetable.Broccoli, TargetVegetable.Carrot }, 2, new int[] { 50, 50 }, 20, 11));
        pouzzleStageButton[12].onClick.AddListener(() =>
        OnClickGameStart(new TargetVegetable[] { TargetVegetable.Pumpkin }, 1, new int[] { 80 }, 30, 12));
        pouzzleStageButton[13].onClick.AddListener(() =>
        OnClickGameStart(new TargetVegetable[] { TargetVegetable.Pumpkin, TargetVegetable.Corn, TargetVegetable.Cabbage, TargetVegetable.Paprika }, 4, new int[] { 40, 40, 40, 40 }, 20, 13));
        pouzzleStageButton[14].onClick.AddListener(() =>
        OnClickGameStart(new TargetVegetable[] { TargetVegetable.Corn, TargetVegetable.Pumpkin }, 2, new int[] { 20, 20 }, 5, 14));
        pouzzleStageButton[15].onClick.AddListener(() =>
        OnClickGameStart(new TargetVegetable[] { TargetVegetable.Pumpkin, TargetVegetable.Broccoli, TargetVegetable.Cabbage, TargetVegetable.Corn }, 4, new int[] { 35, 50, 35, 50 }, 10, 15));
        pouzzleStageButton[16].onClick.AddListener(() =>
        OnClickGameStart(new TargetVegetable[] { TargetVegetable.Carrot }, 1, new int[] { 50 }, 2, 16));

        pouzzleModeButton.onClick.AddListener(() => OnClickSelectMode(true));
        selectBackButton.onClick.AddListener(() => OnClickSelectMode(false));

        StartCoroutine(HamsterGenerate());
    }

    void OnClickGameStart(TargetVegetable[] targetVeg, int vegetableNum, int[] targetNum, int turnNum, int stageNum)
    {
        soundMan.YesTapSE();
        SceneNavigator.Instance.Change("PuzzleMode", 0.5f);

        PuzzleMainController.tartgetVeg = new PuzzleMainController.TargetVegetable[vegetableNum];
        PuzzleMainController.targetNum = new int[vegetableNum];
        for (int i = 0; i < vegetableNum; i++)
        {
            PuzzleMainController.tartgetVeg[i] = (PuzzleMainController.TargetVegetable)(int)targetVeg[i];
            PuzzleMainController.targetNum[i] = targetNum[i];
        }
        PuzzleMainController.vegetableNum = vegetableNum;
        PuzzleMainController.turnNum = turnNum;
        PuzzleMainController.stageNum = stageNum;
        PuzzleMainController.tutorial = (stageNum == 0) ? true : false;
    }
    void OnClickSelectMode(bool selectMode)
    {
        pouzzleModeObject.SetActive(!selectMode);
        displayStageNum = (displayStageNum == 0) ? 1 : displayStageNum;
        int stageTotal = pouzzleStageObject.Length;
        int loopTimes = (displayStageNum >= stageTotal) ? stageTotal : displayStageNum;
        for (int i = 0; i < loopTimes + 1; i++)
        {
            pouzzleStageObject[i].SetActive(selectMode);
        }
        selectBuckObject.SetActive(selectMode);
        if(selectMode) soundMan.YesTapSE();
        else soundMan.NoTapSE();
    }

    IEnumerator HamsterGenerate()
    {
        while (true)
        {
            float waitTime = Random.Range(5.0f, 12.0f);
            yield return new WaitForSeconds(waitTime);
            GameObject hamsterObj = Instantiate(hamsterPre);
            hamsterObj.transform.SetParent(hamsterBox, false);
        }
    }
}
