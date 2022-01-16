using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ShootMode.ShootModeDefine;

namespace ShootMode
{
    public class GroundCollider : MonoBehaviour
    {
        ScoreManager scoreMan;
        BlockManager blockMan;

        void Start()
        {
            scoreMan = GameObject.FindWithTag("ScoreManager").GetComponent<ScoreManager>();
            blockMan = GameObject.FindWithTag("BlockManager").GetComponent<BlockManager>();
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