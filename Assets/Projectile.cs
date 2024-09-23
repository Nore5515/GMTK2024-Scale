using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    float speed = 5.0f;

    [SerializeField]
    GameObject sprite;

    [SerializeField]
    GameObject playerFlippedSprite;

    public Vector3 dir;

    public bool playerFlipped = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (dir != null)
        {
            transform.Translate(dir * speed * Time.deltaTime);
        }
    }

    public void TeamFlip(Vector3 swooshPos)
    {
        Vector3 dist = swooshPos - transform.position;
        dist *= -1.0f;
        dist.Normalize();
        dir = dist;
        speed *= 1.5f;
        playerFlipped = true;
        sprite.SetActive(false);
        playerFlippedSprite.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Proj trigger entered with " + collision.gameObject.name);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("Proj collision entered with " + collision.gameObject.name);
    }
}
