using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShootMode
{
    public class GroundCollider : MonoBehaviour
    {
        ScoreManager scoreMan;

        void Start()
        {
            scoreMan = GameObject.FindWithTag("ScoreManager").GetComponent<ScoreManager>();
        }

        //========================================================================
        //ê⁄êG
        //========================================================================
        void OnTriggerEnter2D(Collider2D col)
        {
            scoreMan.HarvestVegetable(col.gameObject.tag);
        }
    }
}