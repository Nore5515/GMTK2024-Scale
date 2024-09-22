using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swooshie : MonoBehaviour
{
    public Vector3 playerPos;

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "projectile")
        {
            collision.gameObject.GetComponent<Projectile>().TeamFlip(playerPos);
            //Destroy(collision.gameObject);
        }
        //Debug.Log("Swooshie trigger entered with " + collision.gameObject.name);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("Swooshie collision entered with " + collision.gameObject.name);
    }
}
