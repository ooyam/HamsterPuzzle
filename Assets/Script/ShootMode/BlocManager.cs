using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlocManager : MonoBehaviour
{
    [Header("ブロックの取得")]
    public GameObject[] blocPre;
    List<GameObject> blocObj = new List<GameObject>();
    List<RectTransform> blocTra = new List<RectTransform>();

    int columnNum = 8;                   //1行の列数
    int nowLineNum = 0;                  //現在の行数
    int maxLineNum = 15;                 //最大行数
    float lineCoordinateX = 100.0f;      //1列の座標
    float lineCoordinateY = 50.0f;       //1行の座標
    float[][] blocPosX = new float[2][]; //ブロック生成位置X
    float[] blocPosY;                    //ブロック生成位置Y

    void Strat()
    {
        float posYFix = 900.0f;
        blocPosY = new float[maxLineNum];
        for (int posYInd = 0; posYInd < maxLineNum; posYInd++)
        {
            blocPosY[posYInd] = lineCoordinateY * posYInd + posYFix;
        }

        float[] posXFix = new float[] { 525.0f, 575.0f };
        for (int posXFirstInd = 0; posXFirstInd < blocPosX.Length; posXFirstInd++)
        {
            blocPosX[posXFirstInd] = new float[columnNum];
            for (int posXSecondInd = 0; posXSecondInd < columnNum; posXSecondInd++)
            {
                blocPosX[posXFirstInd][posXSecondInd] = lineCoordinateX * posXSecondInd - posXFix[posXFirstInd];
            }
        }
        int firstGenerateLinesNum = 3;
        BlocGenerate(firstGenerateLinesNum);
    }

    void BlocGenerate(int generatLineNum)
    {
        for (int lineIndex = 0; lineIndex < generatLineNum; lineIndex++)
        {
            int blocPosIndex = nowLineNum % 2;
            for (int colIndex = 0; colIndex < columnNum; colIndex++)
            {
                int blocPreIndex = Random.Range(0, blocPre.Length);
                GameObject blocObject = Instantiate(blocPre[blocPreIndex]);
                RectTransform blocRectTra = blocObject.GetComponent<RectTransform>();
                blocObj.Add(blocObject);
                blocTra.Add(blocRectTra);
                blocTra[blocTra.Count - 1].anchoredPosition = new Vector2(blocPosX[blocPosIndex][colIndex], blocPosY[nowLineNum]);
            }
            nowLineNum++;
        }
    }
}
