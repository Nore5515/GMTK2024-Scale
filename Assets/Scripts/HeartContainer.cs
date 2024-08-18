using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartContainer : MonoBehaviour
{
    [SerializeField]
    GameObject fullSprite;

    [SerializeField]
    GameObject emptySprite;

    bool isFull = true;

    public void SetFullness(bool newFull)
    {
        isFull = newFull;
        if (isFull)
        {
            fullSprite.SetActive(true);
            emptySprite.SetActive(false);
        }
        else
        {
            fullSprite.SetActive(false);
            emptySprite.SetActive(true);
        }
    }

    private void Start()
    {
        SetFullness(true);
    }
}
