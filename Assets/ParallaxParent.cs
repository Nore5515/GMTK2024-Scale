using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[System.Serializable]
class ParallaxObject
{
    public GameObject obj;
    public float speed;
}

public class ParallaxParent : MonoBehaviour
{
    [SerializeField]
    Player player;

    [SerializeField]
    List<ParallaxObject> pObjs;

    [SerializeField]
    GameObject cameraObj;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        foreach (ParallaxObject pobj in pObjs)
        {
            Vector3 newMove = pobj.obj.transform.position;
            float parallaxMoveSpeed = pobj.speed * Time.deltaTime;
            if (MovingLeft())
            {
                newMove.x += parallaxMoveSpeed;
            }
            if (MovingRight())
            {
                newMove.x -= parallaxMoveSpeed;
            }
            if (MovingDown())
            {
                newMove.y += parallaxMoveSpeed;
            }
            if (MovingUp())
            {
                newMove.y -= parallaxMoveSpeed;
            }
            pobj.obj.transform.position = newMove;
        }
    }

    bool MovingUp()
    {
        return (player.GetComponent<Rigidbody2D>().velocity.y > 0);
    }

    bool MovingDown()
    {
        return (player.GetComponent<Rigidbody2D>().velocity.y < 0);
    }

    bool MovingLeft()
    {
        return (player.GetComponent<Player>().movingLeft);
    }

    bool MovingRight()
    {
        return (player.GetComponent<Player>().movingRight);
    }
}

