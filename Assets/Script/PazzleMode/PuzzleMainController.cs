using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundFunction;

public class PuzzleMainController : MonoBehaviour
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
    public static TargetVegetable[] tartgetVeg; //目標野菜
    public static int vegetableNum;             //目標野菜の個数
    public static int[] targetNum;              //種類ごとの目標個数
    public static int turnNum;                  //ターン数
    public static int stageNum;                 //ステージ番号
    public static bool tutorial;                //チュートリアル？
    public static GameObject resultScreenObj;   //リザルトスクリーン

    [Header("ResultScreen")]
    public GameObject resultScreenPre;

    public static PuzzleMainController instance = null;
    void Awake()
    {
        instance = this;

        resultScreenObj = Instantiate(resultScreenPre);
        resultScreenObj.SetActive(false);
        resultScreenObj.transform.SetParent(GameObject.FindWithTag("CanvasMain").transform.GetChild(1).gameObject.transform, false);

        //BGM開始
        int bgmIndex = 1;
        if (stageNum >= 8 && stageNum <= 14) bgmIndex = 2;
        else if (stageNum >= 15) bgmIndex = 3;
        GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>().BGM_Start(bgmIndex);
    }
}
