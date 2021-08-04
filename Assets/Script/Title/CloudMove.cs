using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMove : MonoBehaviour
{
    private RectTransform tra;
    private float moveSpeed = 0.01f;
    private float maxScale = 1.1f;
    private float minScale = 0.9f;
    private float scale;
    private bool expansion = true;

    // Start is called before the first frame update
    void Start()
    {
        tra = GetComponent<RectTransform>();
        scale = Random.Range(minScale, maxScale);
        tra.localScale = new Vector2(scale, scale);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (expansion)
        {
            scale += moveSpeed;
            if (scale > maxScale)
                expansion = false;
        }
        else
        {
            scale -= moveSpeed;
            if (scale < minScale)
                expansion = true;
        }
        tra.localScale = new Vector2(scale, scale);
    }
}