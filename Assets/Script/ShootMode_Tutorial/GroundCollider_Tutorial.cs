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
        //�ڐG�I��
        //========================================================================
        void OnTriggerExit2D(Collider2D col)
        {
            if (FEVER_START)
            {
                //���X�g
                Destroy(col.gameObject);
            }
            else
            {
                //���n����
                scoreMan.HarvestVegetable(col.gameObject.tag);
            }
            blockMan.fallCompleteCount++;
        }
    }
}