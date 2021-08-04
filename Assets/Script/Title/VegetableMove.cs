using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VegetableMove : MonoBehaviour
{
    private RectTransform tra;
    private float moveSpeed = 3.0f;
    private float maxRot = 10.0f;
    private bool leftMove = true;

    // Start is called before the first frame update
    void Start()
    {
        tra = GetComponent<RectTransform>();
        float startRot = Random.Range(0.0f, 20.0f);
        tra.localRotation = Quaternion.Euler(0.0f, 0.0f, startRot - 10.0f);
    }

    // Update is called once per frame
    void FixedUpdate()
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