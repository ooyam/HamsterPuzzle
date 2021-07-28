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

        int vegetableMaxNum = 4;
        RectTransform Tra = GetComponent<RectTransform>();

        float posX = 180.0f;
        float posXFix = posX + (0.5f * (vegetableNum - 1) * posX);
        float posY = -20.0f;
        Vector2[] displayPos = new Vector2[vegetableNum];
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

                if (vegetableNum < vegetableMaxNum)
                {
                    displayPos[i] = new Vector2(posX * (i + 1) - posXFix, posY);
                    targetTra[i].anchoredPosition = displayPos[i];
                }
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
