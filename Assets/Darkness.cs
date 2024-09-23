using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Darkness : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer sr;

    [SerializeField]
    float seeThroughA = 0.25f;

    Color dark;
    Color seeThrough;

    float currentA;

    [SerializeField]
    float deDark = 0.5f;

    private void Start()
    {
        dark = sr.color;
        currentA = dark.a;
        seeThrough = sr.color;
        seeThrough.a = seeThroughA;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameState.EyesOpen)
        {
            sr.color = seeThrough;
            currentA = sr.color.a;
        }
        else
        {
            if (currentA < dark.a)
            {
                Color c = sr.color;
                currentA += Time.deltaTime * deDark;
                c.a = currentA;
                sr.color = c;
            }
            else
            {
                sr.color = dark;
            }
        }
    }
}
