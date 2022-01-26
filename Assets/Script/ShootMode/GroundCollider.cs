using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SoundFunction;
using static ShootMode.ShootModeDefine;

namespace ShootMode
{
    public class GroundCollider : MonoBehaviour
    {
        ScoreManager scoreMan;
        BlockManager blockMan;
        SoundManager soundMan;
        string[] vegName;

        void Start()
        {
            scoreMan = GameObject.FindWithTag("ScoreManager").GetComponent<ScoreManager>();
            blockMan = GameObject.FindWithTag("BlockManager").GetComponent<BlockManager>();
            soundMan = GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>();
            var vegetableType = Enum.GetValues(typeof(VegetableType));
            int vegTypeNum = vegetableType.Length;
            vegName = new string[vegTypeNum];
            foreach (VegetableType vegeValue in vegetableType)
            { vegName[(int)vegeValue] = Enum.GetName(typeof(VegetableType), vegeValue); }
        }

        //========================================================================
        //ê⁄êGèIóπ
        //========================================================================
        void OnTriggerExit2D(Collider2D col)
        {
            GameObject obj = col.gameObject;
            string objTag = obj.tag;
            if (Array.IndexOf(vegName, objTag) >= 0)
            {
                if (FEVER_START)
                {
                    //ÉçÉXÉg
                    Destroy(col.gameObject);
                }
                else
                {
                    //SE
                    soundMan.HarvestSE_Shoot_Shoot();

                    //é˚änäÆóπ
                    scoreMan.HarvestVegetable(col.gameObject.tag);
                }
                blockMan.fallCompleteCount++;
            }
        }
    }
}