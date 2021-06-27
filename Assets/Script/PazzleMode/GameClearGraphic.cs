using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameClearGraphic : MonoBehaviour
{
    private RectTransform tra;
    private float moveSpeed = 0.0f;
    private Vector2 startScale = new Vector2(0.0f, 0.0f);
    private bool moveEnd = false;

    void OnEnable()
    {
        if (tra == null) tra = GetComponent<RectTransform>();
        tra.localScale = startScale;
        moveSpeed = 0.0f;
        moveEnd = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!moveEnd)
        {
            moveSpeed += Time.deltaTime;
            tra.localScale = new Vector2(moveSpeed, moveSpeed);
            if (tra.localScale.y >= 1.0f)
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
        SceneNavigator.Instance.Change("TitleScene", 1.0f);
    }
}
