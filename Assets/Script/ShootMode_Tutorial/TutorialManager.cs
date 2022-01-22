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
        bool handShow;     //手表示フラグ
        bool handNowMove;  //手動作中フラグ

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

        int descriptionNum    = 0;     //説明番号
        float minDispalyTime  = 3.0f;  //説明最低表示時間
        float imaFadeSpeed    = 0.2f;  //フェード速度
        float destroyWaitTime = 0.3f;  //フェードアウト待機時間
        int[] alphaFadeComp = new int[] { 3, 3 };  //比較番号指定配列(0:R 1:G 2:B 3:A)

        Color[] appearance  = new Color[] { new Color(1, 1, 1, 0), Color.white };               //透明 → 白
        Color[] transparent = new Color[] { Color.white, new Color(1, 1, 1, 0) };               //白 → 透明
        Color[] filterColor = new Color[] { new Color(0, 0, 0, 160.0f / 255.0f), Color.clear }; //黒半透明 → 透明

        [System.NonSerialized]
        public bool throwWait; //投擲待機フラグ
        [System.NonSerialized]
        public bool throwBlockChangeWait; //nextブロックタップ待機フラグ

        void Start()
        {
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

            //説明開始
            StartCoroutine(Description());
        }

        //========================================================================
        //タップを待つ
        //========================================================================
        IEnumerator WaitTap()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(0.02f);
                if (Input.GetMouseButtonDown(0))
                {
                    NextDescriptionStart();
                    break;
                }
            }
        }

        //========================================================================
        //次説明へ
        //========================================================================
        public void NextDescriptionStart()
        {
            descriptionNum++;
            StartCoroutine(Description());
        }

        //========================================================================
        //説明
        //========================================================================
        IEnumerator Description()
        {
            switch (descriptionNum)
            {
                case 0:  //シュートモードへようこそ～
                    SoundMan.YesTapSE();
                    FullFilterSwitch(true, true);                     //全面フィルターフェード表示
                    TextShow(0);                                      //テキスト[0]表示
                    yield return new WaitForSeconds(minDispalyTime);  //最低表示時間待機
                    StartCoroutine(WaitTap());                        //タップ待機
                    break;

                case 1:  //画面の上の方に野菜ブロックが見えるかな～
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                       //テキスト非表示
                    yield return new WaitForSeconds(destroyWaitTime); //テキスト非表示待機
                    FullFilterSwitch(true, false);                    //全面フィルターフェード非表示
                    FilterShow(0, true);                              //フィルター[0]表示
                    TextShow(1);                                      //テキスト[1]表示
                    yield return new WaitForSeconds(minDispalyTime);  //最低表示時間待機
                    StartCoroutine(WaitTap());                        //タップ待機
                    break;

                case 2:  //まずはオラフくんをタップして～
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                       //テキスト非表示
                    StartCoroutine(FilterHide());                     //フィルター非表示
                    yield return new WaitForSeconds(destroyWaitTime); //テキスト非表示待機
                    StartCoroutine(HandMove(0));                      //手表示[0]
                    TextShow(2);                                      //テキスト[2]表示
                    FilterShow(1, true);                              //フィルター[1]表示
                    yield return new WaitForSeconds(minDispalyTime);  //最低表示時間待機
                    StartCoroutine(WaitTap());                        //タップ待機
                    break;

                case 3:  //パプリカ投擲待機
                    StartCoroutine(TextHide());                       //テキスト非表示
                    yield return new WaitForSeconds(destroyWaitTime); //テキスト非表示待機
                    throwWait = true;                                 //投擲可能フラグtrue
                    break;

                case 4:  //パプリカがつながったね～
                    SoundMan.YesTapSE();
                    throwWait = false;                                        //投擲可能フラグfalse
                    HandHide();                                               //手非表示
                    StartCoroutine(FilterHide());                             //フィルター非表示
                    yield return new WaitForSecondsRealtime(destroyWaitTime); //テキスト非表示待機
                    FilterShow(2, true);                                      //フィルター[2]表示
                    TextShow(3);                                              //テキスト[3]表示
                    yield return new WaitForSecondsRealtime(minDispalyTime);  //最低表示時間待機
                    StartCoroutine(WaitTap());                                //タップ待機
                    break;

                case 5:  //収穫待機
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                               //テキスト非表示
                    yield return new WaitForSecondsRealtime(destroyWaitTime); //テキスト非表示待機
                    Time.timeScale = 1;                                       //時間を戻す
                    StartCoroutine(FilterHide());                             //フィルター非表示
                    break;

                case 6:  //収穫した野菜は～
                    SoundMan.YesTapSE();
                    StartCoroutine(HandMove(1));                      //手表示[1]
                    FilterShow(3, true);                              //フィルター[3]表示
                    TextShow(4);                                      //テキスト[4]表示
                    yield return new WaitForSeconds(minDispalyTime);  //最低表示時間待機
                    StartCoroutine(WaitTap());                        //タップ待機
                    break;

                case 7:  //画面上部で～
                    SoundMan.YesTapSE();
                    HandHide();                                       //手非表示
                    StartCoroutine(TextHide());                       //テキスト非表示
                    yield return new WaitForSeconds(destroyWaitTime); //テキスト非表示待機
                    StartCoroutine(HandMove(2));                      //手表示[2]
                    TextShow(5);                                      //テキスト[5]表示
                    yield return new WaitForSeconds(minDispalyTime);  //最低表示時間待機
                    StartCoroutine(WaitTap());                        //タップ待機
                    break;

                case 8:  //次に、その他の収穫方法を～
                    SoundMan.YesTapSE();
                    HandHide();                                       //手非表示
                    StartCoroutine(TextHide());                       //テキスト非表示
                    yield return new WaitForSeconds(destroyWaitTime); //テキスト非表示待機
                    StartCoroutine(FilterHide());                     //フィルター非表示
                    FullFilterSwitch(true, true);                     //全面フィルターフェード表示
                    TextShow(6);                                      //テキスト[6]表示
                    yield return new WaitForSeconds(minDispalyTime);  //最低表示時間待機
                    StartCoroutine(WaitTap());                        //タップ待機
                    break;

                case 9:  //キャベツ投擲待機
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                       //テキスト非表示
                    yield return new WaitForSeconds(destroyWaitTime); //テキスト非表示待機
                    throwWait = true;                                 //投擲可能フラグtrue
                    StartCoroutine(HandMove(3));                      //手表示[3]
                    FullFilterSwitch(true, false);                    //全面フィルターフェード非表示
                    FilterShow(4, true);                              //フィルター[4]表示
                    break;

                case 10:  //キャベツを収穫した時～
                    SoundMan.YesTapSE();
                    throwWait = false;                                //投擲可能フラグfalse
                    HandHide();                                       //手非表示
                    StartCoroutine(FilterHide());                     //フィルター非表示
                    FullFilterSwitch(true, true);                     //全面フィルターフェード表示
                    TextShow(7);                                      //テキスト[7]表示
                    yield return new WaitForSeconds(minDispalyTime);  //最低表示時間待機
                    StartCoroutine(WaitTap());                        //タップ待機
                    break;

                case 11:  //画面右下のオラフくんが～
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                       //テキスト非表示
                    yield return new WaitForSeconds(destroyWaitTime); //テキスト非表示待機
                    StartCoroutine(HandMove(4));                      //手表示[4]
                    FullFilterSwitch(true, false);                    //全面フィルターフェード非表示
                    FilterShow(5, true);                              //フィルター[5]表示
                    TextShow(8);                                      //テキスト[8]表示
                    break;

                case 12:  //スペシャルオラフ待機
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                       //テキスト非表示
                    yield return new WaitForSeconds(destroyWaitTime); //テキスト非表示待機
                    StartCoroutine(FilterHide());                     //フィルター非表示
                    break;

                case 13:  //スペシャルオラフくんは～
                    SoundMan.YesTapSE();
                    FullFilterSwitch(true, true);                     //全面フィルターフェード表示
                    TextShow(9);                                      //テキスト[9]表示
                    yield return new WaitForSeconds(minDispalyTime);  //最低表示時間待機
                    StartCoroutine(WaitTap());                        //タップ待機
                    break;

                case 14:  //おっと、次に持っているブロックは～
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                       //テキスト非表示
                    yield return new WaitForSeconds(destroyWaitTime); //テキスト非表示待機
                    StartCoroutine(HandMove(5));                      //手表示[5]
                    FullFilterSwitch(true, false);                    //全面フィルターフェード非表示
                    FilterShow(6, true);                              //フィルター[6]表示
                    TextShow(10);                                     //テキスト[10]表示
                    yield return new WaitForSeconds(minDispalyTime);  //最低表示時間待機
                    throwBlockChangeWait = true;                      //nextブロックタップ待機フラグtrue
                    break;

                case 15:  //手持ちのブロックが入れ替わったね～
                    SoundMan.YesTapSE();
                    throwBlockChangeWait = false;                     //nextブロックタップ待機フラグfalse
                    HandHide();                                       //手非表示
                    StartCoroutine(TextHide());                       //テキスト非表示
                    yield return new WaitForSeconds(destroyWaitTime); //テキスト非表示待機
                    TextShow(11);                                     //テキスト[11]表示
                    yield return new WaitForSeconds(minDispalyTime);  //最低表示時間待機
                    StartCoroutine(WaitTap());                        //タップ待機
                    break;

                case 16:  //次にフィーバータイムについて～
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                       //テキスト非表示
                    yield return new WaitForSeconds(destroyWaitTime); //テキスト非表示待機
                    StartCoroutine(FilterHide());                     //フィルター非表示
                    FullFilterSwitch(true, true);                     //全面フィルターフェード表示
                    TextShow(12);                                     //テキスト[12]表示
                    yield return new WaitForSeconds(minDispalyTime);  //最低表示時間待機
                    StartCoroutine(WaitTap());                        //タップ待機
                    break;

                case 17:  //ブロッコリー投擲待機
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                       //テキスト非表示
                    yield return new WaitForSeconds(destroyWaitTime); //テキスト非表示待機
                    FullFilterSwitch(true, false);                    //全面フィルターフェード非表示
                    throwWait = true;                                 //投擲可能フラグtrue
                    StartCoroutine(HandMove(6));                      //手表示[6]
                    break;

                case 18:  //ブロックをすべて収穫出来たね～
                    SoundMan.YesTapSE();
                    Time.timeScale = 0;                                       //時間停止
                    throwWait = false;                                        //投擲可能フラグfalse
                    HandHide();                                               //手非表示
                    FullFilterSwitch(true, true);                             //全面フィルターフェード表示
                    TextShow(13);                                             //テキスト[13]表示
                    yield return new WaitForSecondsRealtime(minDispalyTime);  //最低表示時間待機
                    StartCoroutine(WaitTap());                                //タップ待機
                    break;

                case 19:  //フィーバー開始演出待機
                    SoundMan.YesTapSE();
                    Time.timeScale = 1;                               //時間を戻す
                    StartCoroutine(TextHide());                       //テキスト非表示
                    yield return new WaitForSeconds(destroyWaitTime); //テキスト非表示待機
                    FullFilterSwitch(true, false);                    //全面フィルターフェード非表示
                    break;

                case 20:  //フィーバータイムは～
                    SoundMan.YesTapSE();
                    Time.timeScale = 0;                                       //時間停止
                    FullFilterSwitch(true, true);                             //全面フィルターフェード表示
                    TextShow(14);                                             //テキスト[14]表示
                    yield return new WaitForSecondsRealtime(minDispalyTime);  //最低表示時間待機
                    StartCoroutine(WaitTap());                                //タップ待機
                    break;

                case 21:  //地面に落ちたブロックは収穫はできないよ～
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                               //テキスト非表示
                    yield return new WaitForSecondsRealtime(destroyWaitTime); //テキスト非表示待機
                    TextShow(15);                                             //テキスト[15]表示
                    yield return new WaitForSecondsRealtime(minDispalyTime);  //最低表示時間待機
                    StartCoroutine(WaitTap());                                //タップ待機
                    break;

                case 22:  //フィーバー待機
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                               //テキスト非表示
                    yield return new WaitForSecondsRealtime(destroyWaitTime); //テキスト非表示待機
                    Time.timeScale = 1;                                       //時間を戻す
                    StartCoroutine(HandMove(7));                              //手表示[7]
                    FullFilterSwitch(true, false);                            //全面フィルターフェード非表示
                    break;

                case 23:  //うまくキャッチできたかな～
                    SoundMan.YesTapSE();
                    HandHide();                                       //手非表示
                    FullFilterSwitch(true, true);                     //全面フィルターフェード表示
                    TextShow(16);                                     //テキスト[16]表示
                    yield return new WaitForSeconds(minDispalyTime);  //最低表示時間待機
                    StartCoroutine(WaitTap());                        //タップ待機
                    break;

                case 24:  //このラインより下にブロックが来たら～
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                       //テキスト非表示
                    yield return new WaitForSeconds(destroyWaitTime); //テキスト非表示待機
                    StartCoroutine(HandMove(8));                      //手表示[8]
                    FullFilterSwitch(true, false);                    //全面フィルターフェード非表示
                    FilterShow(7, true);                              //フィルター[7]表示
                    TextShow(17);                                     //テキスト[17]表示
                    yield return new WaitForSeconds(minDispalyTime);  //最低表示時間待機
                    StartCoroutine(WaitTap());                        //タップ待機
                    break;

                case 25:  //操作方法は分かったかな～
                    SoundMan.YesTapSE();
                    HandHide();                                       //手非表示
                    StartCoroutine(TextHide());                       //テキスト非表示
                    yield return new WaitForSeconds(destroyWaitTime); //テキスト非表示待機
                    StartCoroutine(FilterHide());                     //フィルター非表示
                    FullFilterSwitch(true, true);                     //全面フィルターフェード表示
                    TextShow(18);                                     //テキスト[18]表示
                    yield return new WaitForSeconds(minDispalyTime);  //最低表示時間待機
                    StartCoroutine(WaitTap());                        //タップ待機
                    break;

                case 26:  //強制クリア
                    StartCoroutine(TextHide());                       //テキスト非表示
                    yield return new WaitForSeconds(destroyWaitTime); //テキスト非表示待機
                    FullFilterSwitch(true, false);                    //全面フィルターフェード非表示
                    GameObject.FindWithTag("ScoreManager").GetComponent<ScoreManager_Tutorial>().ForcedClear();
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
            StartCoroutine(PaletteChange(fadeTextFilter[displayTextIndex], null, imaFadeSpeed, transparent, alphaFadeComp, 1));
        }

        //========================================================================
        //テキスト非表示
        //========================================================================
        IEnumerator TextHide()
        {
            StartCoroutine(PaletteChange(fadeTextFilter[displayTextIndex], null, imaFadeSpeed, appearance, alphaFadeComp, 1));
            yield return new WaitForSecondsRealtime(destroyWaitTime);
            textObj[displayTextIndex].SetActive(false);
        }

        //========================================================================
        //フィルター表示
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
                if (fade) StartCoroutine(PaletteChange(filter, null, imaFadeSpeed, new Color[] { filterColor[1], filterColor[0] }, alphaFadeComp, 1));
                else filter.color = filterColor[0];
            }
        }

        //========================================================================
        //フィルター非表示
        //========================================================================
        public IEnumerator FilterHide()
        {
            foreach (Image filter in fadeFilter[displayFilterIndex])
            {
                StartCoroutine(PaletteChange(filter, null, 0.02f, filterColor, alphaFadeComp, 1));
            }
            yield return new WaitForSecondsRealtime(destroyWaitTime);
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
                StartCoroutine(PaletteChange(fullFilterIma, null, imaFadeSpeed, ColArray, alphaFadeComp, 1));
            }
            else
            {
                fullFilterIma.color = (display) ? filterColor[0] : filterColor[1];
            }
        }


        //========================================================================
        //手を動かす
        //========================================================================
        //moveNum; 動作番号
        //========================================================================
        IEnumerator HandMove(int moveNum)
        {
            //手他動作中は終了まで待機
            yield return new WaitWhile(() => handNowMove == true);

            //手の表示
            handObj.SetActive(true);
            Vector2[] handPos = new Vector2[2];
            float pointFingerSpeed = 1.0f;
            switch (moveNum)
            {
                case 0:  //パプリカ投擲
                case 3:  //キャベツ投擲
                case 6:  //ブロッコリー投擲
                    StartCoroutine(ThrowingInstructions()); //投擲指示動作
                    break;

                case 1:  //スコア指差し
                    handTra.rotation = Quaternion.Euler(0.0f, 0.0f, 180.0f);   //角度設定
                    handPos[0] = new Vector2(-230.0f, -560.0f);                //指差し動作開始座標設定
                    handPos[1] = new Vector2(-230.0f, -540.0f);                //指差し動作折返し座標設定
                    StartCoroutine(HandRoundTrip(handPos, pointFingerSpeed));  //指差し動作
                    break;

                case 2:  //ターゲット指差し
                    handTra.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);     //角度指定
                    handPos[0] = new Vector2(20.0f, 590.0f);                   //指差し動作開始座標設定
                    handPos[1] = new Vector2(20.0f, 570.0f);                   //指差し動作折返し座標設定
                    StartCoroutine(HandRoundTrip(handPos, pointFingerSpeed));  //投擲指示動作
                    break;

                case 4:  //スペシャルオラフ指差し
                    handTra.rotation = Quaternion.Euler(0.0f, 0.0f, 180.0f);   //角度指定
                    handPos[0] = new Vector2(400.0f, -630.0f);                 //指差し動作開始座標設定
                    handPos[1] = new Vector2(400.0f, -610.0f);                 //指差し動作折返し座標設定
                    StartCoroutine(HandRoundTrip(handPos, pointFingerSpeed));  //投擲指示動作
                    break;

                case 5:  //nextブロック指差し
                    handTra.rotation = Quaternion.Euler(0.0f, 0.0f, 180.0f);   //角度指定
                    handPos[0] = new Vector2(295.0f, -550.0f);                 //指差し動作開始座標設定
                    handPos[1] = new Vector2(295.0f, -530.0f);                 //指差し動作折返し座標設定
                    StartCoroutine(HandRoundTrip(handPos, pointFingerSpeed));  //投擲指示動作
                    break;

                case 7:  //フィーバー中スワイプ
                    handTra.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);                  //角度指定
                    handPos[0] = new Vector2(-400.0f, -650.0f);                             //指差し動作開始座標設定
                    handPos[1] = new Vector2(420.0f, -650.0f);                              //指差し動作折返し座標設定
                    handTra.anchoredPosition = new Vector2(20.0f, -650.0f);                 //画面中央へ
                    handIma.color   = appearance[1];                                        //alpha値戻し
                    float moveSpeed = 7.0f;                                                 //手の移動速度
                    float moveTime  = GetMoveTime(handTra, moveSpeed, 1.0f, handPos[0]);    //動作時間取得
                    StartCoroutine(MoveMovement(handTra, moveSpeed, 1.0f, handPos[0]));     //中央から左へ
                    yield return new WaitForSeconds(moveTime);                              //動作待機
                    StartCoroutine(HandRoundTrip(handPos, moveSpeed));                      //投擲指示動作
                    break;

                case 8:  //ゲームオーバーライン指差し
                    handTra.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);     //角度指定
                    handPos[0] = new Vector2(400.0f, -550.0f);                 //指差し動作開始座標設定
                    handPos[1] = new Vector2(400.0f, -570.0f);                 //指差し動作折返し座標設定
                    StartCoroutine(HandRoundTrip(handPos, pointFingerSpeed));  //投擲指示動作
                    break;
            }
        }

        //========================================================================
        //手投擲指示動作
        //========================================================================
        IEnumerator ThrowingInstructions()
        {
            //各値指定
            Vector2[] handPos = new Vector2[] { new Vector2(20.0f, -600.0f), new Vector2(20.0f, -840.0f) };
            float handMoveSpeed = 7.0f;
            Vector3[] handScalingSpeed = new Vector3[] { new Vector3(0.02f, 0.02f, 0.0f), new Vector3(-0.02f, -0.02f, 0.0f) };
            float[] handSize = new float[] { 1.0f, 1.2f };

            //初期値設定
            handTra.anchoredPosition = handPos[0];
            handTra.localScale       = new Vector3(handSize[1], handSize[1], handSize[1]);
            handTra.rotation         = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            handIma.color            = appearance[0];

            //動作時間計算
            float moveTime  = GetMoveTime(handTra, handMoveSpeed, 1.0f, handPos[1]);
            float scaleTime = GetScaleChangeTime(handTra, handScalingSpeed[1], handSize[0], handSize[0], 1);
            float stopTime  = 0.2f;
            float interval  = 1.0f;

            //動作開始
            handNowMove = true;
            handShow    = true;
            while (handShow)
            {
                yield return new WaitForSeconds(interval);                                                  //インターバル
                handTra.anchoredPosition = handPos[0];                                                      //手座標指定
                StartCoroutine(PaletteChange(handIma, null, imaFadeSpeed, appearance, alphaFadeComp, 1));   //出現
                StartCoroutine(ScaleChange(handTra, handScalingSpeed[1], handSize[0], handSize[0], 1));     //縮小
                yield return new WaitForSeconds(scaleTime);                                                 //縮小待機
                yield return new WaitForSeconds(stopTime);                                                  //一旦停止
                StartCoroutine(MoveMovement(handTra, handMoveSpeed, 1.0f, handPos[1]));                     //下に移動
                yield return new WaitForSeconds(moveTime);                                                  //移動待機
                yield return new WaitForSeconds(stopTime);                                                  //一旦停止
                StartCoroutine(PaletteChange(handIma, null, imaFadeSpeed, transparent, alphaFadeComp, 1));  //透過
                StartCoroutine(ScaleChange(handTra, handScalingSpeed[0], handSize[1], handSize[1], 1));     //拡大
                yield return new WaitForSeconds(scaleTime);                                                 //拡大待機
            }

            //動作終了
            handObj.SetActive(false);
            handIma.color = appearance[1];
            handTra.localScale = Vector3.one;
            handNowMove = false;
        }

        //========================================================================
        //往復動作
        //========================================================================
        //handPos;       手の座標 1:開始座標 2:終了座標
        //handMoveSpeed; 手の移動速度
        //========================================================================
        IEnumerator HandRoundTrip(Vector2[] handPos, float handMoveSpeed)
        {
            //初期値設定
            handTra.anchoredPosition = handPos[0];
            handIma.color = appearance[1];
            float moveTime = GetMoveTime(handTra, handMoveSpeed, 1.0f, handPos[1]);

            //動作開始
            handNowMove = true;
            handShow    = true;
            while (handShow)
            {
                StartCoroutine(MoveMovement(handTra, handMoveSpeed, 1.0f, handPos[1])); //目標座標へ
                yield return new WaitForSeconds(moveTime);
                StartCoroutine(MoveMovement(handTra, handMoveSpeed, 1.0f, handPos[0])); //元座標へ
                yield return new WaitForSeconds(moveTime);
            }

            //動作終了
            handObj.SetActive(false);
            handTra.localScale = Vector3.one;
            handNowMove = false;
        }

        //========================================================================
        //手非表示(動作終了)
        //========================================================================
        public void HandHide()
        {
            handShow = false;
            handObj.SetActive(false);
        }
    }
}