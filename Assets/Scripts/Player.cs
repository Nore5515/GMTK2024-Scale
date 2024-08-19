using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    [SerializeField]
    GameObject normalSprite;

    [SerializeField]
    GameObject heartSprite;

    [SerializeField]
    GameObject liverSprite;

    List<GameObject> sprites = new();

    [SerializeField]
    float speed = 1.0f;

    [SerializeField]
    float jumpStrength = 90.0f;

    [SerializeField]
    GameObject swooshie;

    [SerializeField]
    TextMeshProUGUI hpText;

    [SerializeField]
    GameObject healingEffect;

    [SerializeField]
    GameObject poisonAnim;

    [Header("HP Changed (healed or damaged)!")]
    public UnityEvent hpChanged;

    int hp = 3;

    string activeSprite = "";

    bool removingPoison = false;

    bool movingLeft = false;
    bool movingRight = false;

    GameObject checkpoint;

    public void DealDamage(int dmg)
    {
        shakeRadius += 0.5f;
        hp -= dmg;
        GameState.hp = hp;
        hpChanged.Invoke();
        if (hp <= 0)
        {
            if (checkpoint is not null)
            {
                gameObject.transform.position = checkpoint.transform.position;
                Heal(GameState.maxHP);
            }
        }
        else
        {
            // Reload scene!
        }
    }

    public void GetPoisonedIdiot()
    {
        //Debug.Log("Get poison!");

        if (activeSprite == "liver")
        {
            return;
        }
        poisonAnim.SetActive(true);
        removingPoison = false;
        GameState.IsPoisoned = true;

        Color c = poisonAnim.GetComponent<Image>().color;
        c.a = 1.0f;
        poisonAnim.GetComponent<Image>().color = c;

        StartCoroutine("PoisonCountdown");
        StopCoroutine("FadeoutPoison");
    }

    public void StopPoison()
    {
        //Debug.Log("Stopping poison!");

        GameState.IsPoisoned = false;

        StopCoroutine("PoisonCountdown");
        StartCoroutine("FadeoutPoison");
    }

    public void AssignCheckpoint(GameObject g)
    {
        checkpoint = g;
    }

    void SwitchSprite(string newSprite)
    {
        bool regenOn = false;
        bool switchedOffLiver = true;

        DisableAllSprites();
        if (newSprite == "normal")
        {
            normalSprite.SetActive(true);
        }
        else if (newSprite == "liver")
        {
            liverSprite.SetActive(true);
            switchedOffLiver = false;
            StopPoison();
        }
        else if (newSprite == "heart")
        {
            heartSprite.SetActive(true);
            regenOn = true;
        }

        if (switchedOffLiver && removingPoison)
        {
            //Debug.Log("Tried to sneak off poison ehh?");
            GetPoisonedIdiot();
        }

        ToggleRegen(regenOn);
        activeSprite = newSprite;
    }

    void DisableAllSprites()
    {
        normalSprite.SetActive(false);
        liverSprite.SetActive(false);
        heartSprite.SetActive(false);
    }

    void FlipSpritesAround()
    {
        foreach (GameObject sprite in sprites)
        {
            if (movingLeft)
            {
                sprite.GetComponent<SpriteRenderer>().flipX = true;
            }
            else
            {
                sprite.GetComponent<SpriteRenderer>().flipX = false;
            }
        }
    }

    void ToggleRegen(bool regenOn)
    {
        if (regenOn)
        {
            healingEffect.SetActive(true);
            StartCoroutine("HealWait");
        }
        else
        {
            healingEffect.SetActive(false);
            StopCoroutine("HealWait");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SwitchSprite("normal");

        sprites.Add(normalSprite);
        sprites.Add(liverSprite);
        sprites.Add(heartSprite);
    }

    float shakeRadius = 0.0f;

    // Update is called once per frame
    void Update()
    {
        if (shakeRadius > 0.0f)
        {
            Vector2 playerPos = new Vector2(transform.position.x, transform.position.y);
            Vector2 randomPos = Random.insideUnitCircle * shakeRadius + playerPos;
            Vector3 newCameraPos = new Vector3(randomPos.x, randomPos.y, -10.0f);
            Camera.main.transform.position = newCameraPos;
            shakeRadius -= Time.deltaTime * 2.0f;
        }
        else
        {
            Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10.0f);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            GetPoisonedIdiot();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            StopPoison();
        }

        hpText.text = hp.ToString();
        Vector2 movement = new Vector2();
        movement.x = Input.GetAxis("Horizontal");
        if (movement.x > 0)
        {
            movingRight = true;
            movingLeft = false;
            FlipSpritesAround();
        }
        else if (movement.x < 0)
        {
            movingLeft = true;
            movingRight = false;
            FlipSpritesAround();
        }
        else
        {
            movingLeft = false;
            movingRight = false;
        }
        movement.y = Input.GetAxis("Vertical");
        transform.Translate(movement * Time.deltaTime * speed);

        if (Input.GetKeyDown(KeyCode.Space) && IsOnGround())
        {
            Vector2 jumpVelo = GetComponent<Rigidbody2D>().velocity;
            jumpVelo.y = jumpStrength;
            GetComponent<Rigidbody2D>().velocity = jumpVelo;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchSprite("normal");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchSprite("liver");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchSprite("heart");
        }


        if (Input.GetMouseButtonDown(0))
        {
            Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            cursorPosition.z = 0;

            GameObject inst = Instantiate(swooshie);
            inst.transform.position = transform.position;
            Vector3 direction = cursorPosition - transform.position;

            direction.Normalize();
            inst.transform.position += direction;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            inst.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, angle));
        }
    }

    IEnumerator PoisonCountdown()
    {
        WaitForSeconds wait = new WaitForSeconds(2f);
        //Debug.Log("Poison Countdown initiated!");

        while (true)
        {
            yield return wait;
            DealDamage(1);
        }
    }

    IEnumerator FadeoutPoison()
    {
        //Debug.Log("Fadeout Poison");

        WaitForSeconds wait = new WaitForSeconds(0.15f);
        removingPoison = true;

        while (poisonAnim.GetComponent<Image>().color.a > 0)
        {
            yield return wait;
            Color c = poisonAnim.GetComponent<Image>().color;
            c.a -= 0.05f;
            poisonAnim.GetComponent<Image>().color = c;
        }

        poisonAnim.SetActive(false);
        removingPoison = false;

        //Debug.Log("Fadeout Poison Complete! Free to go!");
    }

    IEnumerator HealWait()
    {
        WaitForSeconds wait = new WaitForSeconds(3f);

        while (true)
        {
            yield return wait;
            Heal(1);
        }
    }

    void Heal(int x)
    {
        hp += x;
        GameState.hp = hp;
        hpChanged.Invoke();
    }

    bool IsOnGround()
    {
        return true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Player trigger entered with " + collision.gameObject.name);
        if (collision.gameObject.tag == "poison_giver")
        {
            GetPoisonedIdiot();
        }
        else if (collision.gameObject.tag == "poison_remover")
        {
            StopPoison();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("Player collision entered with " + collision.gameObject.name);
    }
}
