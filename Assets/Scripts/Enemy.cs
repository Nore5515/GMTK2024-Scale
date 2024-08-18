using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Enemy trigger entered with " + collision.gameObject.name);
        if (collision.gameObject.tag == "swooshie")
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Enemy collision entered with " + collision.gameObject.name);
        if (collision.gameObject.tag == "player")
        {
            collision.gameObject.GetComponent<Player>().DealDamage(1);
        }
    }
}
