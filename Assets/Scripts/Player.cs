using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Threading.Tasks;

struct SpriteSet
{
    public SpriteSet(GameObject walk, GameObject idle)
    {
        this.walk = walk;
        this.idle = idle;
    }

    public GameObject walk { get; }
    public GameObject idle { get; }
}

public enum MorphTypes
{
    normal,
    liver,
    legs,
    eyes,
    heart
}

public class Player : MonoBehaviour
{

    [SerializeField]
    GameObject normalSprite_idle;

    [SerializeField]
    GameObject normalSprite_walk;

    SpriteSet normalSprites;

    [SerializeField]
    GameObject heartSprite;

    [SerializeField]
    GameObject liverSprite_idle;

    [SerializeField]
    GameObject liverSprite_walk;

    SpriteSet liverSprites;

    [SerializeField]
    GameObject legsSprite;

    public List<GameObject> sprites = new();

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

    [SerializeField]
    GameObject poofAnim;

    [Header("HP Changed (healed or damaged)!")]
    public UnityEvent hpChanged;

    [Header("Respawning at checkpoint!")]
    public UnityEvent respawn;

    int hp = 3;

    float shakeRadius = 0.0f;

    string activeSprite = "";

    bool removingPoison = false;

    public bool movingLeft = false;
    public bool movingRight = false;

    [SerializeField]
    float heavyJumpModifier = 0.75f;

    [SerializeField]
    float legsJumpModifier = 1.5f;

    bool moving = false;

    GameObject checkpoint;

    bool poisonTimerRunning = false;

    bool poofing = false;

    async void TestLog()
    {
        bool stuff = await Gmtk2024_config.GetBoolValue("isDebugEnabled");

        Debug.Log("Is Debug Enabled: " + stuff);
    }

    void Start()
    {
        Gmtk2024_analytics.TrackTest();
        StartCoroutine("TestLog");

        normalSprites = new SpriteSet(normalSprite_walk, normalSprite_idle);
        liverSprites = new SpriteSet(liverSprite_walk, liverSprite_idle);

        FillSprites();

        SwitchSprite("normal");

        poofAnim.SetActive(false);

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");
        foreach (GameObject enemy in enemies)
        {
            //Debug.Log("Enemy " + enemy.gameObject.name);
            respawn.AddListener(enemy.GetComponent<Enemy>().ResetEnemy);
        }
    }

    void FillSprites()
    {
        for (int x = 0; x < transform.childCount; x++)
        {
            if (transform.GetChild(x).gameObject.tag == "sprite")
            {
                sprites.Add(transform.GetChild(x).gameObject);
            }
        }
    }

    void SwitchSprite(string newSprite)
    {
        bool regenOn = false;
        bool switchedOffLiver = true;
        poofAnim.SetActive(true);
        poofAnim.GetComponent<Animator>().Play("Poof", -1, 0.0f);
        poofing = true;

        DisableAllSprites();
        if (GetSpriteSetByName(newSprite) != null)
        {
            SpriteSet spritesheet = (SpriteSet)GetSpriteSetByName(newSprite);
            if (moving)
            {
                spritesheet.walk.SetActive(true);
            }
            else
            {
                spritesheet.idle.SetActive(true);
            }
        }
        else
        {
            if (newSprite == "heart")
            {
                heartSprite.SetActive(true);
                regenOn = true;
            }
            else if (newSprite == "legs")
            {
                legsSprite.SetActive(true);
            }
        }

        if (newSprite == "liver")
        {
            switchedOffLiver = false;
            StopPoison();
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
        foreach (GameObject g in sprites)
        {
            g.SetActive(false);
        }
    }

    SpriteSet? GetSpriteSetByName(string name)
    {
        if (name == "normal")
        {
            return normalSprites;
        }
        if (name == "liver")
        {
            return liverSprites;
        }
        return null;
    }

    public void DealDamage(int dmg)
    {
        shakeRadius += 0.5f;
        hp -= dmg;
        GameState.hp = hp;
        hpChanged.Invoke();
        if (hp <= 0)
        {
            Gmtk2024_analytics.TrackPlayerDeath(activeSprite, GameState.maxHP, poisonTimerRunning, 0);
            if (checkpoint is not null)
            {
                gameObject.transform.position = checkpoint.transform.position;
                respawn.Invoke();
                StopPoison();
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

        if (!poisonTimerRunning)
        {
            StartCoroutine("PoisonCountdown");
        }
        StopCoroutine("FadeoutPoison");
    }

    public void StopPoison()
    {
        poisonTimerRunning = false;

        GameState.IsPoisoned = false;

        StopCoroutine("PoisonCountdown");
        StartCoroutine("FadeoutPoison");
    }

    public void AssignCheckpoint(GameObject g)
    {
        checkpoint = g;
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




    // Update is called once per frame
    void Update()
    {
        if (poofing)
        {
            if (poofAnim.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f)
            {
                //Debug.Log("under 1");
            }
            else
            {
                //Debug.Log("over 1");
                poofAnim.SetActive(false);
                poofing = false;
            }
        }

        if (shakeRadius > 0.0f)
        {
            Vector2 playerPos = new Vector2(transform.position.x, transform.position.y);
            Vector2 randomPos = Random.insideUnitCircle * shakeRadius + playerPos;
            Vector3 newCameraPos = new Vector3(randomPos.x, randomPos.y + 1.5f, -10.0f);
            Camera.main.transform.position = newCameraPos;
            shakeRadius -= Time.deltaTime * 2.0f;
        }
        else
        {
            Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y + 1.5f, -10.0f);
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
            moving = true;
            movingRight = true;
            movingLeft = false;
            FlipSpritesAround();
            //SwitchSprite("normal");
        }
        else if (movement.x < 0)
        {
            moving = true;
            movingLeft = true;
            movingRight = false;
            FlipSpritesAround();
            //SwitchSprite("normal");
        }
        else
        {
            moving = false;
            movingLeft = false;
            movingRight = false;
        }

        HandleAnimStateSwitching();

        //movement.y = Input.GetAxis("Vertical");
        if (activeSprite == "liver" || activeSprite == "heart")
        {
            transform.Translate(movement * Time.deltaTime * speed * 0.5f);
        }
        else
        {
            transform.Translate(movement * Time.deltaTime * speed);
        }

        if (Input.GetKeyDown(KeyCode.Space) && IsOnGround())
        {
            Vector2 jumpVelo = GetComponent<Rigidbody2D>().velocity;
            if (activeSprite == "legs")
            {
                jumpVelo.y = jumpStrength * legsJumpModifier;
            }
            else if (activeSprite == "liver" || activeSprite == "heart")
            {
                jumpVelo.y = jumpStrength * heavyJumpModifier;
            }
            else
            {
                jumpVelo.y = jumpStrength;
            }

            GetComponent<Rigidbody2D>().velocity = jumpVelo;
        }

        HandleMorphInput();

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
        Debug.DrawRay(transform.position + new Vector3(0.0f, -0.5f, 0.0f), -Vector3.up * 0.1f, Color.red);
    }

    void HandleAnimStateSwitching()
    {
        if (GetSpriteSetByName(activeSprite) != null)
        {
            SpriteSet spritesheet = (SpriteSet)GetSpriteSetByName(activeSprite);
            if (moving)
            {
                spritesheet.walk.SetActive(true);
                spritesheet.idle.SetActive(false);
            }
            else
            {
                spritesheet.idle.SetActive(true);
                spritesheet.walk.SetActive(false);
            }
        }
    }

    void HandleMorphInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchSprite("normal");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (GameState.CheckForm("liver"))
            {
                SwitchSprite("liver");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (GameState.CheckForm("heart"))
            {
                SwitchSprite("heart");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (GameState.CheckForm("legs"))
            {
                SwitchSprite("legs");
            }
        }
    }

    IEnumerator PoisonCountdown()
    {
        WaitForSeconds wait = new WaitForSeconds(2f);
        //Debug.Log("Poison Countdown initiated!");

        poisonTimerRunning = true;

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

    public void Heal(int x)
    {
        if (hp + x > GameState.maxHP)
        {
            hp = GameState.maxHP;
        }
        else
        {
            hp += x;
        }
        GameState.hp = hp;
        hpChanged.Invoke();
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
        else if (collision.gameObject.tag == "health_pickup")
        {
            Heal(1);
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "max_health_pickup")
        {
            Heal(1);
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "killbox")
        {
            DealDamage(GameState.maxHP);
        }
        else if (collision.gameObject.tag == "projectile")
        {
            DealDamage(1);
            Destroy(collision.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("Player collision entered with " + collision.gameObject.name);
    }
}
