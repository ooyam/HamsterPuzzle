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