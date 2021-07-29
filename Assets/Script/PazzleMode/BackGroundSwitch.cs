using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackGroundSwitch : MonoBehaviour
{
    [Header("îwåi")]
    public Sprite[] backGroundSpr;

    // Start is called before the first frame update
    void Start()
    {
        Image ima = GetComponent<Image>();
        int stageNum = PuzzleMainController.stageNum;
        switch (stageNum)
        {
            case 0:
            case 1:
            case 2:
            case 3:
            case 4:
                ima.sprite = backGroundSpr[0];
                break;
            case 5:
            case 6:
            case 7:
            case 8:
                ima.sprite = backGroundSpr[1];
                break;
            case 9:
            case 10:
            case 11:
            case 12:
                ima.sprite = backGroundSpr[2];
                break;
            case 13:
            case 14:
            case 15:
            case 16:
                ima.sprite = backGroundSpr[3];
                break;
        }
    }
}
