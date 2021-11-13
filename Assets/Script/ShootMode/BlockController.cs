using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShootMode;
using System;

public class BlockController : MonoBehaviour
{
    string[] blockTag;
    void Start()
    {
        //��؏��̎擾
        System.Array vegetableType = Enum.GetValues(typeof(VegetableType));
        blockTag = new string[vegetableType.Length];
        foreach (VegetableType value in vegetableType)
        { blockTag[(int)value] = Enum.GetName(typeof(VegetableType), value); }
    }

    //========================================================================
    //�ڐG
    //========================================================================
    void OnTriggerEnter2D(Collider2D col)
    {
        GameObject connectObj = col.gameObject;
        int tagIndex = Array.IndexOf(blockTag, connectObj.tag);
        if (0 <= tagIndex)
        {
            GameObject.FindWithTag("BlockManager").GetComponent<BlockManager>().BlockConnect(connectObj);
            Destroy(this.GetComponent<BlockController>());
        }
    }
}