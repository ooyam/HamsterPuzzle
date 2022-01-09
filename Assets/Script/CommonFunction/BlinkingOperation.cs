using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static MoveFunction.ObjectMove;

public class BlinkingOperation : MonoBehaviour
{
    [Header("Iamge")]
    public Image ima;

    [Header("Text")]
    public Text tex;

    [Header("�ύX���x")]
    public float changeSpeed;

    [Header("�ύX�F�̔z��")]
    public Color[] colArray;

    [Header("��r�ԍ��w��z��(0:R 1:G 2:B 3:A)")]
    public int[] compArray;

    [Header("���[�v��(�z��1����1�J�E���g,-1�ŃJ�E���g��~����)")]
    public int chengeCount;

    [Header("�����x������")]
    public float waitTime;

    [Header("��~����(-1�Ŏ��Ԓ�~����)")]
    public float endTime;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSecondsRealtime(waitTime);
        StartCoroutine(PaletteChange(ima, tex, changeSpeed, colArray, compArray, chengeCount));
        if(endTime > 0)
        {
            yield return new WaitForSecondsRealtime(endTime);
            COLOR_CHANGE_INFINITE_END = true;
        }
    }
}
