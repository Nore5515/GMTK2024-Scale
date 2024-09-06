using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartRenderer : MonoBehaviour
{
    [SerializeField]
    GameObject heartContainer;

    List<GameObject> containers = new();

    int currentHeartsOnScreen;

    public void UpdateHearts()
    {
        if (currentHeartsOnScreen != GameState.maxHP)
        {
            while (currentHeartsOnScreen < GameState.maxHP)
            {
                GameObject inst = Instantiate(heartContainer);
                inst.transform.parent = transform;
                Vector3 newSpot = transform.position;
                newSpot.x += currentHeartsOnScreen * 68 + 32;
                newSpot.y -= 32;
                inst.transform.position = newSpot;
                inst.GetComponent<HeartContainer>().SetFullness(true);
                containers.Add(inst);
                currentHeartsOnScreen++;
            }
        }
        for (int x = 0; x < GameState.hp; x++)
        {
            containers[x].GetComponent<HeartContainer>().SetFullness(true);
        }
        for (int x = Mathf.Max(0, GameState.hp); x < GameState.maxHP; x++)
        {
            Debug.Log(x);
            containers[x].GetComponent<HeartContainer>().SetFullness(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int x = 0; x < GameState.maxHP; x++)
        {
            GameObject inst = Instantiate(heartContainer);
            inst.transform.parent = transform;
            Vector3 newSpot = transform.position;
            newSpot.x += x * 68 + 32;
            newSpot.y -= 32;
            inst.transform.position = newSpot;
            inst.GetComponent<HeartContainer>().SetFullness(true);
            containers.Add(inst);
        }
        currentHeartsOnScreen = GameState.maxHP;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
