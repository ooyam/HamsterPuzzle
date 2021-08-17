using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VegetableMove : MonoBehaviour
{
    private RectTransform tra;
    private Image ima;
    private Collider2D coll;
    private Transform parentTra;
    private Vector2 startPos;
    private float moveSpeed = 3.5f;
    private float maxRot = 10.0f;
    private bool leftMove = true;
    private bool moveStop = false;
    [System.NonSerialized]
    public bool hamCatch = false;

    // Start is called before the first frame update
    void Start()
    {
        tra = GetComponent<RectTransform>();
        ima = GetComponent<Image>();
        coll = GetComponent<Collider2D>();
        parentTra = tra.parent.gameObject.transform;
        startPos = tra.anchoredPosition;
        float startRot = Random.Range(0.0f, 20.0f);
        tra.localRotation = Quaternion.Euler(0.0f, 0.0f, startRot - 10.0f);
        StartCoroutine(MoveStop());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!moveStop && !hamCatch)
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

    public void RegenerationStart()
    {
        tra.SetParent(parentTra, false);
        StartCoroutine(Regeneration());
    }

    IEnumerator Regeneration()
    {
        hamCatch = false;
        float alpha = 0.0f;
        ima.color = new Color(1, 1, 1, alpha);
        coll.enabled = false;
        tra.anchoredPosition = startPos;
        float stealthTime = 3.0f;
        yield return new WaitForSeconds(stealthTime);

        float oneFrametime = 0.02f;
        while (true)
        {
            alpha += oneFrametime;
            ima.color = new Color(1.0f, 1.0f, 1.0f, alpha);
            if (alpha >= 1.0f) break;
            yield return new WaitForSeconds(oneFrametime);
        }
        coll.enabled = true;
    }
}