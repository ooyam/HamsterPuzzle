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

    private GameObject soundManObj; //SoundManager
    private SoundManager soundMan;  //SoundManager

    void OnEnable()
    {
        tra = GetComponent<RectTransform>();
        soundManObj = GameObject.FindWithTag("SoundManager");
        soundMan = soundManObj.GetComponent<SoundManager>();
        soundMan.BGM_Stop();
        soundMan.GameClearSE();
        tra.localScale = startScale;
        moveSpeed = 0.0f;
        moveEnd = false;
        GameObject.FindWithTag("SaveDataManager").GetComponent<SaveDataManager>().PuzzleModeSaveData(PuzzleMainController.stageNum + 1);
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
        Destroy(soundManObj);
        SceneNavigator.Instance.Change("TitleScene", 1.0f);
    }
}
