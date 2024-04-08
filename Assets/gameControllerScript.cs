using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class gameControllerScript : MonoBehaviour
{
    public enum gamestates { 
        menu = 0,
        start,
        running
    };

    public GameObject playerPrefab;
    public GameObject clientPrefab;
    public GameObject enemyPrefab;

    public GameObject menuCamera;

    public TMP_InputField ip;
    public TMP_InputField port;

    public float PlayerMouseSens = 1;

    public gamestates gameState = gamestates.menu;

    public void startSinglePlayer() {
        //set gamestate
        gameState = gamestates.running;

        //destory all menu items used (enemys and camera)
        for (int i = 0; i < enemyList.Count; i++) {
            Destroy(enemyList[i]);
        }
        Destroy(menuCamera);
        enemyList.Clear();
        //create player

        GameObject pla = Instantiate(playerPrefab, new Vector3(0,0,0), Quaternion.identity);
        pla.name="player";
        pla.GetComponent<playercameraLook>().mouseSensitivity = PlayerMouseSens;
    }

    public void connectToServer(GameObject connectmenu)
    {
        GameObject clientNetwork = Instantiate(clientPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        clientNetwork.name = "NETWORKER";
        clientNetwork.GetComponent<clientSide>().HostIP = ip.text;
        clientNetwork.GetComponent<clientSide>().port = int.Parse(port.text);
        if (clientNetwork.GetComponent<clientSide>().connect() == 0) {
            startSinglePlayer();
            clientNetwork.GetComponent<clientSide>().start();
            connectmenu.SetActive(false);
        }
    }

    //used for title screen
    int maxEnemyCount = 20;
    List<GameObject> enemyList = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {

        for (int i = 0; i < 15; i++) {
            enemyList.Add(Instantiate(enemyPrefab, new Vector3(Random.Range(-120, 120), 0, Random.Range(-100, 100)), Quaternion.identity));
            enemyList[enemyList.Count - 1].GetComponent<gingeyMovement>().targetPos = (new Vector3(Random.Range(-120, 120), 0, Random.Range(-100, 100)));
        }
    }

    // Update is called once per frame
    void Update()
    {
        //limits the framerate to 60
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;

        //one in 50 chance of changing enemys position is in menu state
        if (gameState == gamestates.menu && Random.Range(0, 100) == 0) {
            print("changed enemys position");
            enemyList[Random.Range(0, enemyList.Count-1)].GetComponent<gingeyMovement>().targetPos = (new Vector3(Random.Range(-120, 120), 0, Random.Range(-100, 100)));
        }

        //if game is running
        if (gameState == gamestates.running && Random.Range(0, 50) == 0) {
            int randNumEnemy = Random.Range(0, 5);
            int randHouse = Random.Range(0, 103);
            for (int i = 0; i < Mathf.Min(randNumEnemy, maxEnemyCount-enemyList.Count); i++) {
                Vector3 spawnPos = GameObject.Find("gingerbreadHouse" + randHouse.ToString()).transform.position;
                enemyList.Add(Instantiate(enemyPrefab, spawnPos+new Vector3(0,10, 0), Quaternion.identity)); //enemy currently only targets the single player
            }

        }
    }
}
