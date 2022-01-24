using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShootMode_Tutorial;
using System;
using ShootMode;

public class BlockController_Tutorial : MonoBehaviour
{
    bool fastContact = false; //接触判定重複防止用
    string[] blockTag;        //ブロックタグリスト

    void Start()
    {
        //ブロックタグ取得
        System.Array vegetableType = Enum.GetValues(typeof(VegetableType));
        blockTag = new string[vegetableType.Length];
        foreach (VegetableType value in vegetableType)
        { blockTag[(int)value] = Enum.GetName(typeof(VegetableType), value); }
    }

    //========================================================================
    //接触
    //========================================================================
    void OnTriggerEnter2D(Collider2D col)
    {
        if (!fastContact)
        {
            GameObject connectObj = col.gameObject;
            string connectObjTag  = connectObj.tag;
            int tagIndex = Array.IndexOf(blockTag, connectObjTag);
            if (0 <= tagIndex)
            {
                //他ブロック接触
                GameObject.FindWithTag("BlockManager").GetComponent<BlockManager_Tutorial>().BlockConnect(connectObj);
                fastContact = true;
                Destroy(this);
            }
            else if (connectObjTag == "UpperLimit")
            {
                //上限接触
                GameObject.FindWithTag("BlockManager").GetComponent<BlockManager_Tutorial>().UpperLimitConnect();
                fastContact = true;
                Destroy(this);
            }
        }
    }
}