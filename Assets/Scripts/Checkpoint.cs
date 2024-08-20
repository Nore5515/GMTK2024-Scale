using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField]
    GameObject dullTorch;

    [SerializeField]
    GameObject igniteTorch;

    [SerializeField]
    GameObject litTorch;

    bool flagged = false;

    public void IgniteCheckpoint()
    {
        flagged = true;
        dullTorch.SetActive(false);
        igniteTorch.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        igniteTorch.SetActive(false);
        litTorch.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (flagged)
        {
            if (igniteTorch.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f)
            {
                Debug.Log("WOO");
            }
            else
            {
                Debug.Log("BOO");
                igniteTorch.SetActive(false);
                litTorch.SetActive(true);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "player")
        {
            IgniteCheckpoint();
            collision.gameObject.GetComponent<Player>().AssignCheckpoint(this.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
    }
}
