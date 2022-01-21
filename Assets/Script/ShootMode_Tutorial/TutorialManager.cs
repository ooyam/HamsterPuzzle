using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SoundFunction;
using static MoveFunction.ObjectMove;

namespace ShootMode_Tutorial
{
    public class TutorialManager : MonoBehaviour
    {
        [Header("エフェクト")]
        [SerializeField]
        GameObject pointEffPre;

        [Header("フィルターボックス")]
        [SerializeField]
        Transform filterBoxTra;

        [Header("テキストボックス")]
        [SerializeField]
        Transform textBoxTra;

        [Header("エフェクトボックス")]
        [SerializeField]
        Transform effBoxTra;

        [Header("手")]
        [SerializeField]
        RectTransform handTra;
        GameObject handObj;
        Image handIma;

        HamsterController_Tutorial hamsterCon;   //HamsterController_Tutorial
        BlockManager_Tutorial blockMan;          //BlockManager_Tutorial
        SoundManager SoundMan;                   //SoundManager

        GameObject[] filterObj;       //フィルターObj
        Image fullFilterIma;          //全面フィルターgImae
        Image[][] fadeFilter;         //フェードImage
        int displayFilterIndex;       //表示中のImage番号
        GameObject[] textObj;         //テキストObj
        Image[] fadeTextFilter;       //フェードText用フィルター
        int displayTextIndex;         //表示中のText番号

        [System.NonSerialized]
        public int descriptionNum = 0;  //説明番号
        float minDispalyTime   = 3.0f;  //説明最低表示時間
        float alphaChangeSpeed = 0.2f;  //フェード速度
        float destroyWaitTime  = 0.3f;  //フェードアウト待機時間
        Color[] handColor   = new Color[] { Color.white, new Color(1, 1, 1, 0) };               //白 → 透明
        Color[] textColor   = new Color[] { Color.white, new Color(1, 1, 1, 0) };               //白 → 透明
        Color[] filterColor = new Color[] { new Color(0, 0, 0, 160.0f / 255.0f), Color.clear }; //黒半透明 → 透明
        int[] alphaFadeComp = new int[] { 3, 3 };  //比較番号指定配列(0:R 1:G 2:B 3:A)

        //手の位置 0･1:オラフ移動 2:体力ゲージ 3:ターン 4:目標
        Vector2[] handStartPos =
            new Vector2[] {
            new Vector2(0.0f, -30.0f),
            new Vector2(0.0f, -200.0f),
            new Vector2(100.0f, -600.0f),
            new Vector2(475.0f, 580.0f),
            new Vector2(400.0f, 770.0f)
            };
        Vector2[] handEndPos =
            new Vector2[] {
            new Vector2(0.0f, -200.0f),
            new Vector2(-360.0f, -200.0f),
            new Vector2(100.0f, -650.0f),
            new Vector2(475.0f, 530.0f),
            new Vector2(350.0f, 770.0f)
            };

        void Start()
        {
            //Time.timeScale = 1.0f;
            hamsterCon = GameObject.FindWithTag("Hamster").GetComponent<HamsterController_Tutorial>();
            blockMan   = GameObject.FindWithTag("BlockManager").GetComponent<BlockManager_Tutorial>();
            SoundMan   = GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>();
            handObj    = handTra.gameObject;
            handIma    = handObj.GetComponent<Image>();

            //フィルター取得
            int filterCount = filterBoxTra.childCount;
            filterObj       = new GameObject[filterCount];
            fadeFilter      = new Image[filterCount][];
            for (int filterInd = 0; filterInd < filterCount; filterInd++)
            {
                filterObj[filterInd]   = filterBoxTra.GetChild(filterInd).gameObject;
                Transform filterObjTra = filterObj[filterInd].transform;
                int filterCount_1      = filterObjTra.childCount;
                fadeFilter[filterInd]  = new Image[filterCount_1];
                for (int filterInd_1 = 0; filterInd_1 < filterCount_1; filterInd_1++)
                { fadeFilter[filterInd][filterInd_1] = filterObjTra.GetChild(filterInd_1).GetComponent<Image>(); }
            }
            fullFilterIma = filterBoxTra.GetChild(filterCount - 1).gameObject.GetComponent<Image>();

            //テキストText取得
            int textCount  = textBoxTra.childCount;
            textObj        = new GameObject[textCount];
            fadeTextFilter = new Image[textCount];
            for (int textInd = 0; textInd < textCount; textInd++)
            {
                textObj[textInd] = textBoxTra.GetChild(textInd).gameObject;
                fadeTextFilter[textInd] = textObj[textInd].transform.Find("TextFilter").GetComponent<Image>();
            }

            StartCoroutine(Description());
        }

        //========================================================================
        //タップを待つ
        //========================================================================
        IEnumerator WaitTap()
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();
                if (Input.GetMouseButtonDown(0))
                {
                    descriptionNum++;
                    StartCoroutine(Description());
                    break;
                }
            }
        }

        //========================================================================
        //説明
        //========================================================================
        IEnumerator Description()
        {
            switch (descriptionNum)
            {
                case 0:
                    //シュートモードへようこそ～
                    SoundMan.YesTapSE();
                    FullFilterSwitch(true, true);                     //全面フィルターフェード表示
                    TextShow(0);                                      //テキスト[0]表示
                    yield return new WaitForSeconds(minDispalyTime);  //最低表示時間待機
                    StartCoroutine(WaitTap());                        //タップ待機
                    break;

                case 1:
                    //画面の上の方に野菜ブロックが見えるかな～
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                       //テキスト非表示
                    yield return new WaitForSeconds(destroyWaitTime); //テキスト非表示待機
                    FullFilterSwitch(true, false);                    //全面フィルターフェード非表示
                    FilterShow(0, true);                              //フィルター[0]表示
                    TextShow(1);                                      //テキスト[1]表示
                    yield return new WaitForSeconds(minDispalyTime);  //最低表示時間待機
                    StartCoroutine(WaitTap());                        //タップ待機
                    break;

                case 2:
                    //まずはオラフくんをタップして～
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                       //テキスト非表示
                    StartCoroutine(FilterHide());                     //フィルター非表示
                    yield return new WaitForSeconds(destroyWaitTime); //テキスト非表示待機
                    TextShow(2);                                      //テキスト[2]表示
                    FilterShow(1, true);                              //フィルター[1]表示
                    yield return new WaitForSeconds(minDispalyTime);  //最低表示時間待機
                    StartCoroutine(WaitTap());                        //タップ待機
                    break;

                case 3:
                    //パプリカ投擲待機
                    StartCoroutine(TextHide());                       //テキスト非表示
                    yield return new WaitForSeconds(destroyWaitTime); //テキスト非表示待機
                    yield return new WaitForSeconds(minDispalyTime);  //最低表示時間待機
                    StartCoroutine(WaitTap());                        //タップ待機
                    break;

                case 4:
                    //パプリカがつながったね～
                    SoundMan.YesTapSE();
                    StartCoroutine(FilterHide());                     //フィルター非表示
                    yield return new WaitForSeconds(destroyWaitTime); //テキスト非表示待機
                    FilterShow(2, true);                              //フィルター[2]表示
                    TextShow(3);                                      //テキスト[3]表示
                    yield return new WaitForSeconds(minDispalyTime);  //最低表示時間待機
                    StartCoroutine(WaitTap());                        //タップ待機
                    break;

                case 5:
                    //収穫待機
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                       //テキスト非表示
                    yield return new WaitForSeconds(destroyWaitTime); //テキスト非表示待機
                    StartCoroutine(FilterHide());                     //フィルター非表示
                    yield return new WaitForSeconds(minDispalyTime);  //最低表示時間待機
                    StartCoroutine(WaitTap());                        //タップ待機
                    break;

                case 6:
                    //収穫した野菜は～
                    SoundMan.YesTapSE();
                    FilterShow(3, true);                              //フィルター[3]表示
                    TextShow(4);                                      //テキスト[4]表示
                    yield return new WaitForSeconds(minDispalyTime);  //最低表示時間待機
                    StartCoroutine(WaitTap());                        //タップ待機
                    break;

                case 7:
                    //収穫した野菜は～
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                       //テキスト非表示
                    yield return new WaitForSeconds(destroyWaitTime); //テキスト非表示待機
                    TextShow(5);                                      //テキスト[5]表示
                    yield return new WaitForSeconds(minDispalyTime);  //最低表示時間待機
                    StartCoroutine(WaitTap());                        //タップ待機
                    break;

                case 8:
                    //次に、その他の収穫方法を～
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                       //テキスト非表示
                    yield return new WaitForSeconds(destroyWaitTime); //テキスト非表示待機
                    StartCoroutine(FilterHide());                     //フィルター非表示
                    FullFilterSwitch(true, true);                     //全面フィルターフェード表示
                    TextShow(6);                                      //テキスト[6]表示
                    yield return new WaitForSeconds(minDispalyTime);  //最低表示時間待機
                    StartCoroutine(WaitTap());                        //タップ待機
                    break;

                case 9:
                    //キャベツ投擲待機
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                       //テキスト非表示
                    yield return new WaitForSeconds(destroyWaitTime); //テキスト非表示待機
                    FullFilterSwitch(true, false);                    //全面フィルターフェード非表示
                    FilterShow(4, true);                              //フィルター[4]表示
                    yield return new WaitForSeconds(minDispalyTime);  //最低表示時間待機
                    StartCoroutine(WaitTap());                        //タップ待機
                    break;

                case 10:
                    //キャベツを収穫した時～
                    SoundMan.YesTapSE();
                    StartCoroutine(FilterHide());                     //フィルター非表示
                    FullFilterSwitch(true, true);                     //全面フィルターフェード表示
                    TextShow(7);                                      //テキスト[7]表示
                    yield return new WaitForSeconds(minDispalyTime);  //最低表示時間待機
                    StartCoroutine(WaitTap());                        //タップ待機
                    break;

                case 11:
                    //画面右下のオラフくんが～
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                       //テキスト非表示
                    yield return new WaitForSeconds(destroyWaitTime); //テキスト非表示待機
                    FullFilterSwitch(true, false);                    //全面フィルターフェード非表示
                    FilterShow(5, true);                              //フィルター[5]表示
                    TextShow(8);                                      //テキスト[8]表示
                    yield return new WaitForSeconds(minDispalyTime);  //最低表示時間待機
                    StartCoroutine(WaitTap());                        //タップ待機
                    break;

                case 12:
                    //スペシャルオラフ待機
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                       //テキスト非表示
                    yield return new WaitForSeconds(destroyWaitTime); //テキスト非表示待機
                    StartCoroutine(FilterHide());                     //フィルター非表示
                    yield return new WaitForSeconds(minDispalyTime);  //最低表示時間待機
                    StartCoroutine(WaitTap());                        //タップ待機
                    break;

                case 13:
                    //スペシャルオラフくんは～
                    SoundMan.YesTapSE();
                    FullFilterSwitch(true, true);                     //全面フィルターフェード表示
                    TextShow(9);                                      //テキスト[9]表示
                    yield return new WaitForSeconds(minDispalyTime);  //最低表示時間待機
                    StartCoroutine(WaitTap());                        //タップ待機
                    break;

                case 14:
                    //おっと、次に持っているブロックは～
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                       //テキスト非表示
                    yield return new WaitForSeconds(destroyWaitTime); //テキスト非表示待機
                    FullFilterSwitch(true, false);                    //全面フィルターフェード非表示
                    FilterShow(6, true);                              //フィルター[6]表示
                    TextShow(10);                                     //テキスト[10]表示
                    yield return new WaitForSeconds(minDispalyTime);  //最低表示時間待機
                    StartCoroutine(WaitTap());                        //タップ待機
                    break;

                case 15:
                    //手持ちのブロックが入れ替わったね～
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                       //テキスト非表示
                    yield return new WaitForSeconds(destroyWaitTime); //テキスト非表示待機
                    TextShow(11);                                     //テキスト[11]表示
                    yield return new WaitForSeconds(minDispalyTime);  //最低表示時間待機
                    StartCoroutine(WaitTap());                        //タップ待機
                    break;

                case 16:
                    //次にフィーバータイムについて～
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                       //テキスト非表示
                    yield return new WaitForSeconds(destroyWaitTime); //テキスト非表示待機
                    StartCoroutine(FilterHide());                     //フィルター非表示
                    FullFilterSwitch(true, true);                     //全面フィルターフェード表示
                    TextShow(12);                                     //テキスト[12]表示
                    yield return new WaitForSeconds(minDispalyTime);  //最低表示時間待機
                    StartCoroutine(WaitTap());                        //タップ待機
                    break;

                case 17:
                    //ブロッコリー投擲待機
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                       //テキスト非表示
                    yield return new WaitForSeconds(destroyWaitTime); //テキスト非表示待機
                    FullFilterSwitch(true, false);                    //全面フィルターフェード非表示
                    yield return new WaitForSeconds(minDispalyTime);  //最低表示時間待機
                    StartCoroutine(WaitTap());                        //タップ待機
                    break;

                case 18:
                    //ブロックをすべて収穫出来たね～
                    SoundMan.YesTapSE();
                    FullFilterSwitch(true, true);                     //全面フィルターフェード表示
                    TextShow(13);                                     //テキスト[13]表示
                    yield return new WaitForSeconds(minDispalyTime);  //最低表示時間待機
                    StartCoroutine(WaitTap());                        //タップ待機
                    break;

                case 19:
                    //フィーバー開始演出待機
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                       //テキスト非表示
                    yield return new WaitForSeconds(destroyWaitTime); //テキスト非表示待機
                    FullFilterSwitch(true, false);                    //全面フィルターフェード非表示
                    yield return new WaitForSeconds(minDispalyTime);  //最低表示時間待機
                    StartCoroutine(WaitTap());                        //タップ待機
                    break;

                case 20:
                    //フィーバータイムは～
                    SoundMan.YesTapSE();
                    FullFilterSwitch(true, true);                     //全面フィルターフェード表示
                    TextShow(14);                                     //テキスト[14]表示
                    yield return new WaitForSeconds(minDispalyTime);  //最低表示時間待機
                    StartCoroutine(WaitTap());                        //タップ待機
                    break;

                case 21:
                    //地面に落ちたブロックは収穫はできないよ～
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                       //テキスト非表示
                    yield return new WaitForSeconds(destroyWaitTime); //テキスト非表示待機
                    TextShow(15);                                     //テキスト[15]表示
                    yield return new WaitForSeconds(minDispalyTime);  //最低表示時間待機
                    StartCoroutine(WaitTap());                        //タップ待機
                    break;

                case 22:
                    //フィーバー待機
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                       //テキスト非表示
                    yield return new WaitForSeconds(destroyWaitTime); //テキスト非表示待機
                    FullFilterSwitch(true, false);                    //全面フィルターフェード非表示
                    yield return new WaitForSeconds(minDispalyTime);  //最低表示時間待機
                    StartCoroutine(WaitTap());                        //タップ待機
                    break;

                case 23:
                    //うまくキャッチできたかな～
                    SoundMan.YesTapSE();
                    FullFilterSwitch(true, true);                     //全面フィルターフェード表示
                    TextShow(16);                                     //テキスト[16]表示
                    yield return new WaitForSeconds(minDispalyTime);  //最低表示時間待機
                    StartCoroutine(WaitTap());                        //タップ待機
                    break;

                case 24:
                    //このラインより下にブロックが来たら～
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                       //テキスト非表示
                    yield return new WaitForSeconds(destroyWaitTime); //テキスト非表示待機
                    FullFilterSwitch(true, false);                    //全面フィルターフェード非表示
                    FilterShow(7, true);                              //フィルター[7]表示
                    TextShow(17);                                     //テキスト[17]表示
                    yield return new WaitForSeconds(minDispalyTime);  //最低表示時間待機
                    StartCoroutine(WaitTap());                        //タップ待機
                    break;

                case 25:
                    //操作方法は分かったかな～
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                       //テキスト非表示
                    yield return new WaitForSeconds(destroyWaitTime); //テキスト非表示待機
                    StartCoroutine(FilterHide());                     //フィルター非表示
                    FullFilterSwitch(true, true);                     //全面フィルターフェード表示
                    TextShow(18);                                     //テキスト[18]表示
                    yield return new WaitForSeconds(minDispalyTime);  //最低表示時間待機
                    StartCoroutine(WaitTap());                        //タップ待機
                    break;
            }
        }

        //========================================================================
        //テキスト表示
        //========================================================================
        //textIndex;  生成するテキスト番号
        //========================================================================
        void TextShow(int textIndex)
        {
            //テキスト生成
            textObj[textIndex].SetActive(true);
            displayTextIndex = textIndex;

            //テキストフェード(対象Image, 対象Text, 変更速度, 変更色の配列(0:現在の色), 比較番号(0:R 1:G 2:B 3:A), 動作回数)
            StartCoroutine(PaletteChange(fadeTextFilter[displayTextIndex], null, alphaChangeSpeed, textColor, alphaFadeComp, 1));
        }

        //========================================================================
        //テキスト非表示
        //========================================================================
        IEnumerator TextHide()
        {
            StartCoroutine(PaletteChange(fadeTextFilter[displayTextIndex], null, alphaChangeSpeed, new Color[] { textColor[1], textColor[0] }, alphaFadeComp, 1));
            yield return new WaitForSeconds(destroyWaitTime);
            textObj[displayTextIndex].SetActive(false);
        }

        //========================================================================
        //フィルター生成
        //========================================================================
        //filterIndex;  生成するフィルター番号
        //fade;         フェードあり？
        //========================================================================
        void FilterShow(int filterIndex, bool fade)
        {
            //テキスト生成
            filterObj[filterIndex].SetActive(true);
            displayFilterIndex = filterIndex;

            foreach (Image filter in fadeFilter[filterIndex])
            {
                if (fade) StartCoroutine(PaletteChange(filter, null, alphaChangeSpeed, new Color[] { filterColor[1], filterColor[0] }, alphaFadeComp, 1));
                else filter.color = filterColor[0];
            }
        }

        //========================================================================
        //フィルター非表示
        //========================================================================
        IEnumerator FilterHide()
        {
            foreach (Image filter in fadeFilter[displayFilterIndex])
            {
                StartCoroutine(PaletteChange(filter, null, 0.02f, filterColor, alphaFadeComp, 1));
            }
            yield return new WaitForSeconds(destroyWaitTime);
            filterObj[displayFilterIndex].SetActive(false);
        }

        //========================================================================
        //全面フィルターのフェード
        //========================================================================
        //fade;     フェードあり？
        //display;  表示？
        //========================================================================
        void FullFilterSwitch(bool fade, bool display)
        {
            if (fade)
            {
                //テキストフェード(対象Image, 対象Text, 変更速度, 変更色の配列(0:現在の色), 比較番号(0:R 1:G 2:B 3:A), 動作回数)
                Color[] ColArray = (display) ? new Color[] { filterColor[1], filterColor[0] } : filterColor;
                StartCoroutine(PaletteChange(fullFilterIma, null, alphaChangeSpeed, ColArray, alphaFadeComp, 1));
            }
            else
            {
                fullFilterIma.color = (display) ? filterColor[0] : filterColor[1];
            }
        }
    }
}