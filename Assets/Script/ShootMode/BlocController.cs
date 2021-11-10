using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShootMode;
using System;

public class BlocController : MonoBehaviour
{
    string[] blocTag;
    void Start()
    {
        //��؏��̎擾
        System.Array vegetableType = Enum.GetValues(typeof(VegetableType));
        blocTag = new string[vegetableType.Length];
        foreach (VegetableType value in vegetableType)
        { blocTag[(int)value] = Enum.GetName(typeof(VegetableType), value); }
    }

    //========================================================================
    //�ڐG
    //========================================================================
    void OnTriggerEnter2D(Collider2D col)
    {
        GameObject connectObj = col.gameObject;
        int tagIndex = Array.IndexOf(blocTag, connectObj.tag);
        if (0 <= tagIndex)
        {
            GameObject.FindWithTag("BlocManager").GetComponent<BlocManager>().BlocConnect(connectObj);
            Destroy(this.GetComponent<BlocController>());
        }
    }
}