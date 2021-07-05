using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDestroy : MonoBehaviour
{
    // Start is called before the first frame update
    IEnumerator Start()
    {
        float destroyTime = 1.0f;
        yield return new WaitForSeconds(destroyTime);
        Destroy(this.gameObject);
    }
}
