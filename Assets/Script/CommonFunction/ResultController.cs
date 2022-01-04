using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundFunction;
using UnityEngine.SceneManagement;

public class ResultController : MonoBehaviour
{
    private string titleBackTag = "TitleBack";
    private string retryTag = "Retry";
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit2d = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction);
            if (hit2d)
            {
                if (hit2d.transform.gameObject.tag == titleBackTag)
                    TitlBackButtonDown();
                else if (hit2d.transform.gameObject.tag == retryTag)
                    RetryButtonDown();
            }
        }
    }
    void TitlBackButtonDown()
    {
        GameObject SoundManObj = GameObject.FindWithTag("SoundManager");
        if (EnvironmentalSetting.se) GetComponent<AudioSource>().Play();
        Destroy(SoundManObj);
        SceneNavigator.Instance.Change("TitleScene", 0.5f);
    }
    void RetryButtonDown()
    {
        GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>().YesTapSE();
        SceneNavigator.Instance.Change(SceneManager.GetActiveScene().name, 0.5f);
    }
}
