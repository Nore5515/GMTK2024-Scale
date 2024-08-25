using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    bool isBigEye = false;

    [SerializeField]
    bool isBigBrain = false;

    bool walkingLeft = true;

    [SerializeField]
    float speed = 5.0f;

    [SerializeField]
    List<SpriteRenderer> sprites;

    [SerializeField]
    GameObject bulletPrefab;

    Vector3 startPoint;

    // Start is called before the first frame update
    void Start()
    {
        startPoint = transform.position;
    }

    public void ResetEnemy()
    {
        transform.position = startPoint;
        gameObject.SetActive(true);
    }

    void FlipLeft()
    {
        foreach (SpriteRenderer s in sprites)
        {
            s.flipX = false;
        }
    }

    void FlipRight()
    {
        foreach (SpriteRenderer s in sprites)
        {
            s.flipX = true;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (isBigEye)
        {
            if (walkingLeft)
            {
                transform.Translate(Vector3.left * Time.deltaTime * speed);
                if (IsEdgeLeft())
                {
                    walkingLeft = false;
                    FlipRight();
                }
            }
            else
            {
                transform.Translate(Vector3.right * Time.deltaTime * speed);
                if (IsEdgeRight())
                {
                    walkingLeft = true;
                    FlipLeft();
                }
            }
        }
        if (isBigBrain)
        {

        }
    }

    // wall or cliff
    bool IsEdgeLeft()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(-0.5f, 0.0f, 0.0f), Vector3.left, 0.1f, LayerMask.GetMask("Default"));
        if (hit.collider != null)
        {
            return true;
        }
        return false;
    }

    bool IsEdgeRight()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0.5f, 0.0f, 0.0f), Vector3.right, 0.1f, LayerMask.GetMask("Default"));
        if (hit.collider != null)
        {
            return true;
        }
        return false;
    }

    bool IsOnGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0.0f, -0.5f, 0.0f), -Vector3.up, 0.1f, LayerMask.GetMask("Default"));
        if (hit.collider != null)
        {
            return true;
        }
        return false;
    }

    void FireAtPlayer(GameObject player)
    {
        GameObject inst = Instantiate(bulletPrefab);
        inst.transform.position = this.transform.position;
        Vector3 dist = player.transform.position - transform.position;
        dist.Normalize();
        inst.GetComponent<Projectile>().dir = dist;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Enemy trigger entered with " + collision.gameObject.name);
        if (collision.gameObject.tag == "swooshie")
        {
            gameObject.SetActive(false);
            //Destroy(gameObject);
        }
        else if (collision.gameObject.tag == "player" && isBigBrain)
        {
            FireAtPlayer(collision.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("Enemy collision entered with " + collision.gameObject.name);
        if (collision.gameObject.tag == "player")
        {
            collision.gameObject.GetComponent<Player>().DealDamage(1);
        }
    }
}
