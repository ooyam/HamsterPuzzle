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
        Carrot
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

    // Start is called before the first frame update
    void Start()
    {
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
        OnClickGameStart(new TargetVegetable[] { TargetVegetable.Carrot, TargetVegetable.Paprika, TargetVegetable.Broccoli }, 3, new int[] { 20, 30, 35 }, 18, 5));
        pouzzleStageButton[6].onClick.AddListener(() =>
        OnClickGameStart(new TargetVegetable[] { TargetVegetable.Broccoli, TargetVegetable.Carrot }, 2, new int[] { 20, 20 }, 5, 6));
        pouzzleStageButton[7].onClick.AddListener(() =>
        OnClickGameStart(new TargetVegetable[] { TargetVegetable.Cabbage, TargetVegetable.Broccoli, TargetVegetable.Carrot }, 3, new int[] { 25, 25, 25 }, 10, 7));
        pouzzleStageButton[8].onClick.AddListener(() =>
        OnClickGameStart(new TargetVegetable[] { TargetVegetable.Paprika }, 1, new int[] { 30 }, 2, 8));

        pouzzleModeButton.onClick.AddListener(() => OnClickSelectMode(true));
        selectBackButton.onClick.AddListener(() => OnClickSelectMode(false));
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
        foreach (var ButtonObj in pouzzleStageObject)
        {
            ButtonObj.SetActive(selectMode);
        }
        selectBuckObject.SetActive(selectMode);
        if(selectMode) soundMan.YesTapSE();
        else soundMan.NoTapSE();
    }
}
