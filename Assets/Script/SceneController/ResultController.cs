using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultController : MonoBehaviour
{
    public void TitlBackButtonDown()
    {
        GameObject SoundManObj = GameObject.FindWithTag("SoundManager");
        GetComponent<AudioSource>().Play();
        Destroy(SoundManObj);
        SceneNavigator.Instance.Change("TitleScene", 0.5f);
    }
    public void RetryButtonDown()
    {
        GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>().YesTapSE();
        SceneNavigator.Instance.Change(SceneManager.GetActiveScene().name, 0.5f);
    }
}
