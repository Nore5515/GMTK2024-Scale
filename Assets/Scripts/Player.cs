using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class Player : MonoBehaviour
{

    [SerializeField]
    GameObject normalSprite;

    [SerializeField]
    GameObject heartSprite;

    [SerializeField]
    GameObject liverSprite;

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

    [Header("HP Changed (healed or damaged)!")]
    public UnityEvent hpChanged;

    int hp = 3;

    string activeSprite = "";

    public void DealDamage(int dmg)
    {
        hp -= dmg;
        GameState.hp = hp;
        hpChanged.Invoke();
    }

    void SwitchSprite(string newSprite)
    {
        bool regenOn = false;

        DisableAllSprites();
        if (newSprite == "normal")
        {
            normalSprite.SetActive(true);
        }
        else if (newSprite == "liver")
        {
            liverSprite.SetActive(true);
        }
        else if (newSprite == "heart")
        {
            heartSprite.SetActive(true);
            regenOn = true;
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

    void ToggleRegen(bool regenOn)
    {
        if (regenOn)
        {
            Debug.Log("Start the heals");
            healingEffect.SetActive(true);
            StartCoroutine("HealWait");
        }
        else
        {
            Debug.Log("Stop the heals");
            healingEffect.SetActive(false);
            StopCoroutine("HealWait");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SwitchSprite("normal");
    }

    // Update is called once per frame
    void Update()
    {
        hpText.text = hp.ToString();
        Vector2 movement = new Vector2();
        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");
        transform.Translate(movement * Time.deltaTime * speed);

        if (Input.GetKeyDown(KeyCode.Space) && IsOnGround())
        {
            Vector2 jumpVelo = new Vector2();
            jumpVelo = GetComponent<Rigidbody2D>().velocity;
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

    IEnumerator HealWait()
    {
        WaitForSeconds wait = new WaitForSeconds(3f);
        Debug.Log("Healing...");

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
}
