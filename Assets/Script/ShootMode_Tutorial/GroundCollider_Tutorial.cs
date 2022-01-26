using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ShootMode;
using SoundFunction;
using static ShootMode.ShootModeDefine;

namespace ShootMode_Tutorial
{
    public class GroundCollider_Tutorial : MonoBehaviour
    {
        ScoreManager_Tutorial scoreMan;
        BlockManager_Tutorial blockMan;
        SoundManager soundMan;
        string[] vegName;

        void Start()
        {
            scoreMan = GameObject.FindWithTag("ScoreManager").GetComponent<ScoreManager_Tutorial>();
            blockMan = GameObject.FindWithTag("BlockManager").GetComponent<BlockManager_Tutorial>();
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
            string objTag  = obj.tag;
            if (Array.IndexOf(vegName, objTag) >= 0)
            {
                if (FEVER_START)
                {
                    //ÉçÉXÉg
                    Destroy(obj);
                }
                else
                {
                    //SE
                    soundMan.HarvestSE_Shoot_Shoot();

                    //é˚änäÆóπ
                    scoreMan.HarvestVegetable(objTag);
                }
                blockMan.fallCompleteCount++;
            }
        }
    }
}