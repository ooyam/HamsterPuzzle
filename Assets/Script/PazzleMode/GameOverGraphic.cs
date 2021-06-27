using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverGraphic : MonoBehaviour
{
    private RectTransform tra;
    private float moveSpeed = 1000.0f;
    private Vector2 startPos = new Vector2(0.0f, 1700.0f);
    private Vector2 endPos = Vector2.zero;
    private bool moveEnd = false;
    private GameObject resultScreen; 

    void OnEnable()
    {
        if (tra == null) tra = GetComponent<RectTransform>();
        tra.anchoredPosition = startPos;
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
                moveEnd = true;
                StartCoroutine(ResultScreenDisplay());
            }
        }
    }

    IEnumerator ResultScreenDisplay()
    {
        float waitTime = 1.0f;
        yield return new WaitForSeconds(waitTime);
        resultScreen.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
