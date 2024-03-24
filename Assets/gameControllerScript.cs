using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class gameControllerScript : MonoBehaviour
{
    public enum gamestates { 
        menu = 0
    };

    public GameObject playerPrefab;
    public GameObject clientPrefab;
    public GameObject enemyPrefab;

    public Camera menuCamera;

    public gamestates gameState = gamestates.menu;

    public void startSinglePlayer() { 
        
        
    }

    List<GameObject> menuEnemys = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 15; i++) {
            menuEnemys.Add(Instantiate(enemyPrefab, new Vector3(Random.Range(-120, 120), 0, Random.Range(-100, 100)), Quaternion.identity));
            menuEnemys[menuEnemys.Count - 1].GetComponent<gingeyMovement>().targetPos = (new Vector3(Random.Range(-120, 120), 0, Random.Range(-100, 100)));
        }
    }

    // Update is called once per frame
    void Update()
    {
        //one in 50 chance of changing enemys position is in menu state
        if (gameState == gamestates.menu && Random.Range(0, 100) == 0) {
            print("changed enemys position");
            menuEnemys[Random.Range(0, menuEnemys.Count-1)].GetComponent<gingeyMovement>().targetPos = (new Vector3(Random.Range(-120, 120), 0, Random.Range(-100, 100)));
        }


    }
}
