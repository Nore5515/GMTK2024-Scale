using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    bool isBigEye = false;

    [SerializeField]
    bool isBigBrain = false;

    [Header("Means nothing if not big brain!")]
    [SerializeField]
    float SECONDS_BETWEEN_SHOTS = 0.75f;
    [SerializeField]
    float amp = 1.0f;
    [SerializeField]
    float freq = 1.0f;
    float initialY;

    float cooldownTime = 0.0f;

    bool walkingLeft = true;

    [SerializeField]
    float speed = 5.0f;

    [SerializeField]
    List<SpriteRenderer> sprites;

    [SerializeField]
    GameObject bulletPrefab;

    Vector3 startPoint;

    List<GameObject> targets = new();

    // Start is called before the first frame update
    void Start()
    {
        initialY = this.transform.position.y;
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
            float sineMove = initialY + Mathf.Sin(Time.time * freq) * amp;
            transform.position = new Vector3(transform.position.x, sineMove, transform.position.z);
            if (cooldownTime > 0.0f)
            {
                cooldownTime -= Time.deltaTime;
            }
            else
            {
                if (targets.Count > 0)
                {
                    FireAtGameObject(targets[0]);
                    cooldownTime = SECONDS_BETWEEN_SHOTS;
                }
            }
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

    void FireAtGameObject(GameObject player)
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
            targets.Add(collision.gameObject);
        }
        else if (collision.gameObject.tag == "projectile")
        {
            if (collision.gameObject.GetComponent<Projectile>().playerFlipped)
            {
                Destroy(collision.gameObject);
                gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "player" && isBigBrain)
        {
            targets.Remove(collision.gameObject);
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
