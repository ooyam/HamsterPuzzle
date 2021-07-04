using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("BGM")]
    public AudioClip[] bgm;
    [Header("Yesタップ")]
    public AudioClip yesTap;
    [Header("noタップ")]
    public AudioClip noTap;
    [Header("ハムスター移動")]
    public AudioClip panelChange;
    [Header("収穫")]
    public AudioClip[] harvest;  //通常:0  列(横):1  行(縦):2
    [Header("咀嚼")]
    public AudioClip eat;
    [Header("ゲームオーバー")]
    public AudioClip[] gameOver;
    [Header("ゲームクリア")]
    public AudioClip gameClear;

    private AudioSource audio_SE;  //SE_AudioSource    
    private AudioSource audio_BGM; //BGM_AudioSource

    public static SoundManager instance = null;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            audio_SE = GetComponent<AudioSource>();
            audio_BGM = transform.GetChild(0).gameObject.GetComponent<AudioSource>();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public IEnumerator BGM_Start(int seIndex)
    {
        audio_BGM.clip = bgm[seIndex];
        audio_BGM.volume = 0.0f;
        audio_BGM.Play();
        while (true)
        {
            float oneFlameTime = 0.02f;
            audio_BGM.volume += oneFlameTime;
            yield return new WaitForSeconds(oneFlameTime);
            if(audio_BGM.volume >= 0.48)
            {
                audio_BGM.volume = 0.5f;
                break;
            }
        }
    }
    public void BGM_Stop()
    {
        audio_BGM.volume = 0.0f;
    }

    public void YesTapSE()
    {
        audio_SE.PlayOneShot(yesTap);
    }
    public void NoTapSE()
    {
        audio_SE.PlayOneShot(noTap);
    }
    public void PanelChangeSE()
    {
        audio_SE.PlayOneShot(panelChange);
    }
    public void HarvestSE(int seIndex)
    {
        audio_SE.PlayOneShot(harvest[seIndex]);
    }
    public void EatSE()
    {
        audio_SE.PlayOneShot(eat);
    }
    public void GameOverSE(int seIndex)
    {
        audio_SE.PlayOneShot(gameOver[seIndex]);
    }
    public void GameClearSE()
    {
        audio_SE.PlayOneShot(gameClear);
    }
}
