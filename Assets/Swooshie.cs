using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swooshie : MonoBehaviour
{
    void Start()
    {
        //Start the coroutine we define below named DeathWait.
        StartCoroutine(DeathWait());
    }

    IEnumerator DeathWait()
    {

        //yield on a new YieldInstruction that waits for 0.1s seconds.
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }
}
