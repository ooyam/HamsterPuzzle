using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShootMode;
using System;

public class BlockController : MonoBehaviour
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
            fastContact = true;
            GameObject connectObj = col.gameObject;
            string connectObjTag  = connectObj.tag;
            int tagIndex = Array.IndexOf(blockTag, connectObjTag);
            if (0 <= tagIndex)
            {
                GameObject.FindWithTag("BlockManager").GetComponent<BlockManager>().BlockConnect(connectObj);
                Destroy(this.GetComponent<BlockController>());
            }
        }
    }
}