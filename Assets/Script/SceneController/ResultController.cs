using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultController : MonoBehaviour
{
    public void TitlBackButtonDown()
    {
        SceneNavigator.Instance.Change("TitleScene", 0.5f);
    }
    public void RetryButtonDown()
    {
        SceneNavigator.Instance.Change(SceneManager.GetActiveScene().name, 0.5f);
    }
}
