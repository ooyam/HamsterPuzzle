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
    private float moveSpeed = 5.0f;
    private float desPos = 1300.0f;
    private float posX;
    private float posY;

    // Start is called before the first frame update
    void Start()
    {
        tra = GetComponent<RectTransform>();
        float width = 1080.0f / 2.0f;
        posX = Random.Range(-width, width);
        posY = -desPos;
        tra.anchoredPosition = new Vector2(posX, posY);

        int balloonIndex = Random.Range(0, balloonSpr.Length);
        tra.GetChild(0).gameObject.GetComponent<Image>().sprite = balloonSpr[balloonIndex];
        int hamsterIndex = Random.Range(0, hamsterSpr.Length);
        tra.GetChild(1).gameObject.GetComponent<Image>().sprite = hamsterSpr[hamsterIndex];
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
