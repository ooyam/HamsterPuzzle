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
        if(stageNum > 15)
            ima.sprite = backGroundSpr[3];
        else if(stageNum > 10)
            ima.sprite = backGroundSpr[2];
        else if(stageNum > 5)
            ima.sprite = backGroundSpr[2];
        else
            ima.sprite = backGroundSpr[0];

    }
}
