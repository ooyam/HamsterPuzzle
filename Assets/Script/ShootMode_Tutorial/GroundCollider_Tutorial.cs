using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ShootMode.ShootModeDefine;

namespace ShootMode_Tutorial
{
    public class GroundCollider_Tutorial : MonoBehaviour
    {
        ScoreManager_Tutorial scoreMan;
        BlockManager_Tutorial blockMan;

        void Start()
        {
            scoreMan = GameObject.FindWithTag("ScoreManager").GetComponent<ScoreManager_Tutorial>();
            blockMan = GameObject.FindWithTag("BlockManager").GetComponent<BlockManager_Tutorial>();
        }

        //========================================================================
        //ê⁄êGèIóπ
        //========================================================================
        void OnTriggerExit2D(Collider2D col)
        {
            if (FEVER_START)
            {
                //ÉçÉXÉg
                Destroy(col.gameObject);
            }
            else
            {
                //é˚änäÆóπ
                scoreMan.HarvestVegetable(col.gameObject.tag);
            }
            blockMan.fallCompleteCount++;
        }
    }
}