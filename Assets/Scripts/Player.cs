using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField]
    float speed = 1.0f;

    [SerializeField]
    float jumpStrength = 90.0f;

    [SerializeField]
    GameObject swooshie;

    [SerializeField]
    GameObject pivotPoint;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
            Debug.Log(angle);
        }
    }

    bool IsOnGround()
    {
        return true;
    }
}
