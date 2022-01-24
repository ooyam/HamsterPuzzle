using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShootMode_Tutorial;
using System;
using ShootMode;

public class BlockController_Tutorial : MonoBehaviour
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
            GameObject connectObj = col.gameObject;
            string connectObjTag  = connectObj.tag;
            int tagIndex = Array.IndexOf(blockTag, connectObjTag);
            if (0 <= tagIndex)
            {
                //���u���b�N�ڐG
                GameObject.FindWithTag("BlockManager").GetComponent<BlockManager_Tutorial>().BlockConnect(connectObj);
                fastContact = true;
                Destroy(this);
            }
            else if (connectObjTag == "UpperLimit")
            {
                //����ڐG
                GameObject.FindWithTag("BlockManager").GetComponent<BlockManager_Tutorial>().UpperLimitConnect();
                fastContact = true;
                Destroy(this);
            }
        }
    }
}