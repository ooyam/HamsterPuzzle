using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VegetablePanelController : MonoBehaviour
{
    [Header("HarvestBox")]
    public Transform HarvestBoxTra;

    private PanelManager PanelManagerScr;      //PanelManager取得
    private RectTransform HamsterTra;          //ハムスターのRectTransform取得
    private HamsterPanelController HamsterScr; //ハムスターのRectTransform取得

    private RectTransform Tra;  //RectTransform取得
    private Image Ima;          //Image取得
    private BoxCollider2D Coll; //Collider取得
    [System.NonSerialized]
    public int VegPosNum;               //自身の位置番号
    private bool HarvestStart;          //収穫開始？
    private float HarvestSpeed = 10.0f; //収穫速度
    private bool ImageChange = false;   //Image変更
    private bool Generate = true;       //生成直後
    private bool RotStart = false;      //回転開始？
    private float RotSpeed = 8.0f;      //回転速度

    // Start is called before the first frame update
    IEnumerator Start()
    {
        PanelManagerScr = GameObject.FindWithTag("PanelManager").GetComponent<PanelManager>();
        Tra = GetComponent<RectTransform>();
        Ima = GetComponent<Image>();
        Coll = GetComponent<BoxCollider2D>();
        Tra.rotation = Quaternion.Euler(0.0f, 270.0f, 0.0f);
        yield return null;
        GameObject HamsterObj = GameObject.FindWithTag("Hamster");
        HamsterTra = HamsterObj.GetComponent<RectTransform>();
        HamsterScr = HamsterObj.GetComponent<HamsterPanelController>();
    }

    //パネル位置変更
    public IEnumerator PanelPosChange(int panelPosNum)
    {
        VegPosNum = panelPosNum;
        if (Coll != null)
        {
            Coll.enabled = false;
            yield return null;
            Coll.enabled = true;
        }
    }

    //収穫
    public void Harvest()
    {
        RotStart = true;
    }

    void FixedUpdate()
    {
        if (Generate)
        {
            Tra.Rotate(0.0f, RotSpeed, 0.0f);
            if (Tra.localEulerAngles.y >= 350.0f || Tra.localEulerAngles.y <= 10.0f)
            {
                Generate = false;
                Tra.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            }
        }

        if (RotStart)
        {
            Tra.Rotate(0.0f, RotSpeed, 0.0f);
            if (!ImageChange)
            {
                if (Tra.localEulerAngles.y >= 90.0f)
                {
                    ImageChange = true;
                    Ima.enabled = false;
                }
            }
            else
            {
                if (Tra.localEulerAngles.y >= 179.0f)
                {
                    RotStart = false;
                    HarvestStart = true;
                }
            }
        }
        if (HarvestStart)
        {
            Tra.anchoredPosition = Vector3.MoveTowards(Tra.anchoredPosition, HamsterTra.anchoredPosition, HarvestSpeed);
            if (HamsterTra.anchoredPosition == Tra.anchoredPosition)
            {
                Destroy(this.gameObject);
                PanelManagerScr.HarvestComplete(this.gameObject.tag);
            }
        }
    }
}
