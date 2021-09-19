using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BlocController : MonoBehaviour
{
    string[] blocTag = new string[] { "Broccoli", "Cabbage", "Carrot", "Paprika", "Pumpkin", "Corn" };

    //========================================================================
    //ê⁄êG
    //========================================================================
    void OnCollisionEnter2D(Collision2D col)
    {
        GameObject connectObj = col.gameObject;
        int tagIndex = Array.IndexOf(blocTag, connectObj.tag);
        if (0 <= tagIndex)
        {
            Vector3 connectPos = col.contacts[0].point;
            GameObject.FindWithTag("BlocManager").GetComponent<BlocManager>().BlocConnect(connectObj, connectPos);
        }
    }
}