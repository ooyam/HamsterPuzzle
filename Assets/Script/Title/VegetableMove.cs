using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VegetableMove : MonoBehaviour
{
    private RectTransform tra;
    private float moveSpeed = 3.5f;
    private float maxRot = 10.0f;
    private bool leftMove = true;
    private bool moveStop = false;

    // Start is called before the first frame update
    void Start()
    {
        tra = GetComponent<RectTransform>();
        float startRot = Random.Range(0.0f, 20.0f);
        tra.localRotation = Quaternion.Euler(0.0f, 0.0f, startRot - 10.0f);
        StartCoroutine(MoveStop());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!moveStop)
        {
            float rotZ = tra.localRotation.eulerAngles.z;
            rotZ = (rotZ >= 180.0f) ? rotZ - 360.0f : rotZ;
            if (leftMove)
            {
                tra.Rotate(0.0f, 0.0f, moveSpeed);
                if (rotZ > maxRot)
                {
                    leftMove = false;
                }
            }
            else
            {
                tra.Rotate(0.0f, 0.0f, -moveSpeed);
                if (rotZ < -maxRot)
                {
                    leftMove = true;
                }
            }
        }
    }

    IEnumerator MoveStop()
    {
        float waitTime = 0.8f;
        while (true)
        {
            moveStop = false;
            yield return new WaitForSeconds(waitTime);
            moveStop = true;
            yield return new WaitForSeconds(waitTime);
        }
    }
}