using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundFunction;
using UnityEngine.SceneManagement;

public class GameClearGraphic : MonoBehaviour
{
    [Header("インタースティシャル広告")]
    [SerializeField]
    GameObject interstitialObj;

    private RectTransform tra;
    private float moveSpeed = 0.0f;
    private Vector2 startScale = new Vector2(0.0f, 0.0f);
    private bool moveEnd = false;

    void OnEnable()
    {
        tra = GetComponent<RectTransform>();
        SoundManager soundMan = GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>();
        soundMan.BGM_Volume(0.0f);
        soundMan.GameClearSE();
        tra.localScale = startScale;
        moveSpeed = 0.0f;
        moveEnd = false;
        GameObject.FindWithTag("SaveDataManager").GetComponent<SaveDataManager>().WritePuzzleModeSaveData(PuzzleMainController.stageNum);
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
        float waitTime = 3.0f;
        yield return new WaitForSeconds(waitTime);

        //インタースティシャル広告表示
        if (!PuzzleMainController.tutorial) Instantiate(interstitialObj);
        else SceneNavigator.Instance.Change("TitleScene", 1.0f);
    }
}
