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
    private int spriteIndex;
    private Collider2D col;
    private float moveSpeed = 3.0f;
    private float desPos = 1300.0f;
    private float posX;
    private float posY;
    private bool vegCatch = false;
    private VegetableMove vegScr;

    // Start is called before the first frame update
    void Start()
    {
        tra = GetComponent<RectTransform>();
        col = GetComponent<Collider2D>();
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
        spriteIndex = Random.Range(0, hamsterSpr.Length);
        GameObject hamsterObj = tra.GetChild(1).gameObject;
        hamsterObj.GetComponent<Image>().sprite = hamsterSpr[spriteIndex];
        RectTransform hamsterTra = hamsterObj.GetComponent<RectTransform>();
        hamsterTra.anchoredPosition = new Vector2(hamsterPosX[spriteIndex], 0.0f);
        switch (spriteIndex)
        {
            case 0:
            case 1:
            case 2:
                col.enabled = true;
                break;
            case 6:
                hamsterTra.sizeDelta = new Vector2(130.0f, 150.0f);
                hamsterTra.SetSiblingIndex(0);
                col.enabled = true;
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
            if (vegCatch) vegScr.RegenerationStart();
            Destroy(this.gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        GameObject collObj = collider.gameObject;
        if (!vegCatch && collObj.tag == "Vegetable")
        {
            RectTransform vegTra = collObj.GetComponent<RectTransform>();
            vegScr = collObj.GetComponent<VegetableMove>();
            vegTra.SetParent(tra, false);
            float posX = (spriteIndex == 2) ? -30.0f : 0.0f;
            float posY = (spriteIndex == 2) ? 0.0f : -30.0f;
            vegTra.anchoredPosition = new Vector2(posX, posY);
            vegCatch = true;
            vegScr.hamCatch = true;
        }
    }
}
