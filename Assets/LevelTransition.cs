using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTransition : MonoBehaviour
{
    [SerializeField]
    string level;

    bool playerInRange = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)))
        {
            SwitchLevel();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Level trigger entered with " + collision.gameObject.name);
        if (collision.gameObject.tag == "player")
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Level trigger exited with " + collision.gameObject.name);
        if (collision.gameObject.tag == "player")
        {
            playerInRange = false;
        }
    }

    void SwitchLevel()
    {
        // TODO
        Debug.Log("Switch level!");
    }
}
