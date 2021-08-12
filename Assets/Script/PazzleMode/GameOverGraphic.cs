using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverGraphic : MonoBehaviour
{
    private RectTransform tra;
    private float moveSpeed = 1000.0f;
    private Vector2 endPos = Vector2.zero;
    private bool moveEnd = false;
    private float scaleSpeed = 0.03f;
    private float[] size = new float[] { 0.7f, 1.0f };
    private bool[] scale = new bool[] { false, false };
    private GameObject resultScreen;
    [System.NonSerialized]
    public bool turnOver = false;

    private SoundManager soundMan;  //SoundManager

    void OnEnable()
    {
        tra = GetComponent<RectTransform>();
        soundMan = GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>();
        soundMan.BGM_Stop();
        soundMan.GameOverSE(0);
        Vector2 startPos = new Vector2(0.0f, 1700.0f);
        tra.anchoredPosition = startPos;
        tra.localScale = new Vector3(size[1], size[1], size[1]);
        moveEnd = false;
    }

    void Start()
    {
        resultScreen = PuzzleMainController.resultScreenObj;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!moveEnd)
        {
            float speed = moveSpeed * Time.deltaTime;
            tra.anchoredPosition = Vector2.MoveTowards(tra.anchoredPosition, endPos, speed);
            if (tra.anchoredPosition.y <= endPos.y)
            {
                soundMan.GameOverSE(1);
                moveEnd = true;
            }
        }
        else if (!scale[0])
        {
            float nowScale = (scale[1]) ? tra.localScale.y + scaleSpeed : tra.localScale.y - scaleSpeed;
            tra.localScale = new Vector3(size[1], nowScale, size[1]);
            if (!scale[1] && nowScale <= size[0])
            {
                scale[1] = true;
            }
            else if(scale[1] && nowScale >= size[1])
            {
                scale[0] = true;
                StartCoroutine(ResultScreenDisplay());
            }
        }
    }

    IEnumerator ResultScreenDisplay()
    {
        float waitTime = 1.0f;
        yield return new WaitForSeconds(waitTime);
        resultScreen.SetActive(true);
        int textIndex = (turnOver) ? 1 : 0;
        GameObject resScreenTextObj = resultScreen.transform.GetChild(0).gameObject;
        resScreenTextObj.SetActive(true);
        resScreenTextObj.transform.GetChild(textIndex).gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
