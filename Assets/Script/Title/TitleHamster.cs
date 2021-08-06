using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleHamster : MonoBehaviour
{
    private RectTransform tra;
    [Header("ハムスターsprite")]
    public Sprite[] hamsterSpr;
    [Header("風船sprite")]
    public Sprite[] balloonSpr;
    private float moveSpeed = 3.0f;
    private float desPos = 1300.0f;
    private float posX;
    private float posY;

    // Start is called before the first frame update
    void Start()
    {
        tra = GetComponent<RectTransform>();
        float width = 400.0f;
        posX = Random.Range(-width, width);
        posY = -desPos;
        tra.anchoredPosition = new Vector2(posX, posY);

        int balloonIndex = Random.Range(0, balloonSpr.Length);
        Image balloonObj = tra.GetChild(0).gameObject.GetComponent<Image>();
        balloonObj.sprite = balloonSpr[balloonIndex];
        if (balloonIndex == 0) balloonObj.color = Color.yellow;
        else if (balloonIndex == 3) balloonObj.color = new Color(1.0f, 0.0f, 1.0f, 1.0f);
        else balloonObj.color = Color.white;

        float[] hamsterPosX = new float[] { -30.0f, 10.0f, 0.0f, -20.0f, -20.0f, -20.0f, -10.0f, -30.0f, -25.0f, -20.0f };
        int hamsterIndex = Random.Range(0, hamsterSpr.Length);
        GameObject hamsterObj = tra.GetChild(1).gameObject;
        hamsterObj.GetComponent<Image>().sprite = hamsterSpr[hamsterIndex];
        RectTransform hamsterTra = hamsterObj.GetComponent<RectTransform>();
        hamsterTra.anchoredPosition = new Vector2(hamsterPosX[hamsterIndex], 0.0f);
        switch (hamsterIndex)
        {
            case 6:
                hamsterTra.sizeDelta = new Vector2(130.0f, 150.0f);
                hamsterTra.SetSiblingIndex(0);
                break;
            case 7:
            case 8:
            case 9:
                hamsterTra.sizeDelta = new Vector2(200.0f, 150.0f);
                break;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        posY += moveSpeed;
        tra.anchoredPosition = new Vector2(posX, posY);
        if(posY > desPos)
        {
            Destroy(this.gameObject);
        }
    }
}
