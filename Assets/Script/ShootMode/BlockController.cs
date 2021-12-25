using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShootMode;
using System;

public class BlockController : MonoBehaviour
{
    bool fastContact = false; //�ڐG����d���h�~�p
    string[] blockTag;        //�u���b�N�^�O���X�g

    void Start()
    {
        //�u���b�N�^�O�擾
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
        if (!fastContact)
        {
            fastContact = true;
            GameObject connectObj = col.gameObject;
            string connectObjTag  = connectObj.tag;
            int tagIndex = Array.IndexOf(blockTag, connectObjTag);
            if (0 <= tagIndex)
            {
                //���u���b�N�ڐG
                GameObject.FindWithTag("BlockManager").GetComponent<BlockManager>().BlockConnect(connectObj);
                Destroy(this);
            }
            else if (connectObjTag == "UpperLimit")
            {
                //����ڐG
                GameObject.FindWithTag("BlockManager").GetComponent<BlockManager>().UpperLimitConnect();
                Destroy(this);
            }
        }
    }
}