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

    void OnEnable()
    {
        if (tex == null) ima.color = colArray[0];
        else tex.color = colArray[0];
        StartCoroutine(BlinkingStart());
    }

    IEnumerator BlinkingStart()
    {
        yield return new WaitForSecondsRealtime(waitTime);
        if (ima != null) StartCoroutine(ImagePaletteChange(ima, changeSpeed, colArray, compArray, chengeCount));
        if (tex != null) StartCoroutine(TextPaletteChange(tex, changeSpeed, colArray, compArray, chengeCount));
        if(endTime > 0)
        {
            yield return new WaitForSecondsRealtime(endTime);
            COLOR_CHANGE_INFINITE_END = true;
        }
    }
}
