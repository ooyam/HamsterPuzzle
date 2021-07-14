using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TagetController : MonoBehaviour
{
    public enum TargetVegetable
    {
        Broccoli,
        Cabbage,
        Paprika,
        Carrot
    }
    private TargetVegetable[] targetVeg; //目標野菜(enumである必要はないがenumを配列で使う練習を行ってみた)
    private int vegetableNum;            //目標野菜の個数
    private int[] targetNum;             //種類ごとの目標個数

    [Header("数字のスプライト")]
    public Sprite[] numbers;
    [Header("野菜のスプライト")]
    public Sprite[] vegetables;

    private GameObject[] targetObj;  //ターゲットオブジェクト 1～3

    // Start is called before the first frame update
    void Start()
    {
        vegetableNum = PuzzleMainController.vegetableNum;
        targetNum = PuzzleMainController.targetNum;
        targetVeg = new TargetVegetable[vegetableNum];
        for (int i = 0; i < vegetableNum; i++)
        {
            targetVeg[i] = (TargetVegetable)(int)PuzzleMainController.tartgetVeg[i];
        }

        int vegetableMaxNum = 3;
        Transform Tra = transform;

        float posYFix = 100.0f;
        Vector2[] displayPos = new Vector2[vegetableNum];
        switch (vegetableNum)
        {
            case 1:
                displayPos[0] = Vector2.zero;
                break;
            case 2:
                displayPos[0] = new Vector2(0.0f, posYFix / 2.0f);
                displayPos[1] = new Vector2(0.0f, -posYFix / 2.0f);
                break;
            case 3:
                displayPos[0] = new Vector2(0.0f, posYFix);
                displayPos[1] = Vector2.zero;
                displayPos[2] = new Vector2(0.0f, -posYFix);
                break;
        }
        targetObj = new GameObject[vegetableMaxNum];
        RectTransform[] targetTra = new RectTransform[vegetableMaxNum];
        for (int i = 0; i < vegetableMaxNum; i++)
        {
            targetObj[i] = Tra.GetChild(i).gameObject;
            if (i <= vegetableNum - 1)
            {
                targetTra[i] = targetObj[i].GetComponent<RectTransform>();
                targetTra[i].GetChild(0).gameObject.GetComponent<Image>().sprite = vegetables[(int)targetVeg[i]]; //ターゲット指定
                Transform numbersTra = targetTra[i].GetChild(1).gameObject.transform;
                for (int a = 0; a < 2; a++)
                {
                    numbersTra.GetChild(a).gameObject.GetComponent<Image>().sprite = numbers[TargetNumCalculation(a, targetNum[i])];
                }
                targetTra[i].anchoredPosition = displayPos[i];
            }
            else
                targetObj[i].SetActive(false);
        }
    }

    //ターン計算
    private int TargetNumCalculation(int digits, int referenceNum)
    {
        int ten = 10;
        if (digits == 0)
        {
            return referenceNum % ten;
        }
        else
        {
            if(referenceNum < ten)
            {
                return ten;
            }
            else
            {
                return (referenceNum / ten) % ten;
            }
        }
    }
}
