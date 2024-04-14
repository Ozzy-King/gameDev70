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

    public GameObject startScreen;
    public GameObject menuCamera;

    public TMP_InputField ip;
    public TMP_InputField port;

    public float PlayerMouseSens = 1;

    public gamestates gameState = gamestates.menu;
    public bool isSinglePlayer = false;
    bool isPaused = false;
    public GameObject pausemenu;

    public GameObject userMenu;

    public void BackMainMenu() {
        //destroy all enemys
        for (int i = 0; i < enemyList.Count; i++)
        {
            Destroy(enemyList[i]);
        }
        enemyList.Clear();
        Destroy(GameObject.Find("player")); //delete player
        
        menuCamera.SetActive(true);//set menu camera to true
        startScreen.SetActive(true);
        gameState = gamestates.menu;
        userMenu.SetActive(false);
    }
    public void pause(bool p)
    {
        isPaused = p;
        if (isPaused == false)
        {//if set to false give player control back
            GameObject.Find("player").GetComponent<playercameraLook>().isPaused = false;
        }
        else {
            pausemenu.SetActive(true);
            pausemenu.GetComponentInChildren<settingMenuScript>().setSensitivityValue(PlayerMouseSens.ToString());
        }
    }

    public void startSinglePlayer(bool singlePlayer = true) {
        //set gamestate
        gameState = gamestates.running;
        userMenu.SetActive(true);

        //destory all menu items used (enemys and camera)
        for (int i = 0; i < enemyList.Count; i++) {
            Destroy(enemyList[i]);
        }
        menuCamera.SetActive(false);
        enemyList.Clear();
        //create player

        GameObject pla = Instantiate(playerPrefab, new Vector3(0,0,0), Quaternion.identity);
        pla.name="player";
        pla.GetComponent<playercameraLook>().mouseSensitivity = PlayerMouseSens;
        isSinglePlayer = singlePlayer;

        userMenu.transform.GetChild(1).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Wave: " + waveCount.ToString();

    }

    public void connectToServer(GameObject connectmenu)
    {
        GameObject clientNetwork = Instantiate(clientPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        clientNetwork.name = "NETWORKER";
        clientNetwork.GetComponent<clientSide>().HostIP = ip.text;
        clientNetwork.GetComponent<clientSide>().port = int.Parse(port.text);
        if (clientNetwork.GetComponent<clientSide>().connect() == 0) {
            startSinglePlayer(false);
            clientNetwork.GetComponent<clientSide>().start();
            connectmenu.SetActive(false);
        }
    }

    //used for title screen
    int menuEnemyCount = 15;
    int maxEnemyCount = 5;
    bool spawnedAllEnemy = false;
    public int waveCount = 1;
    List<GameObject> enemyList = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //limits the framerate to 60
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;

        if (menuEnemyCount - enemyList.Count != 0 && gameState == gamestates.menu)
        {
            for (int i = 0; i < menuEnemyCount; i++)
            {
                enemyList.Add(Instantiate(enemyPrefab, new Vector3(Random.Range(-120, 120), 0, Random.Range(-100, 100)), Quaternion.identity));
                enemyList[enemyList.Count - 1].GetComponent<gingeyMovement>().targetPos = (new Vector3(Random.Range(-120, 120), 0, Random.Range(-100, 100)));
            }
        }
        //one in 50 chance of changing enemys position is in menu state
        if (gameState == gamestates.menu && Random.Range(0, 100) == 0) {
            print("changed enemys position");
            enemyList[Random.Range(0, enemyList.Count-1)].GetComponent<gingeyMovement>().targetPos = (new Vector3(Random.Range(-120, 120), 0, Random.Range(-100, 100)));
        }

        //if game is running
        if (gameState == gamestates.running && Random.Range(0, 50) == 0 && isSinglePlayer) {

            //spawn an enemy if needed

            if (spawnedAllEnemy == false)
            {
                while (maxEnemyCount - enemyList.Count != 0)
                {
                    int randNumEnemy = Random.Range(0, 5);
                    int randHouse = Random.Range(0, 103);
                    for (int i = 0; i < Mathf.Min(randNumEnemy, maxEnemyCount - enemyList.Count); i++)
                    {
                        Vector3 spawnPos = GameObject.Find("gingerbreadHouse" + randHouse.ToString()).transform.position;
                        enemyList.Add(Instantiate(enemyPrefab, spawnPos + new Vector3(0, 10, 0), Quaternion.identity)); //enemy currently only targets the single player
                    }
                }
                spawnedAllEnemy = true;
            }
            //if no emenys remain increase wave count by 1 and max enemy by 2, and set to create next wave
            if (enemyList.Count == 0) {
                spawnedAllEnemy = false;
                maxEnemyCount += 2;
                waveCount++;
                userMenu.transform.GetChild(1).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Wave: "+waveCount.ToString() ;
            }

            //make all the enemys target the player

            for (int i = enemyList.Count - 1; i >= 0; i--) {
                if (enemyList[i] != null)
                {
                    if (isPaused)
                    {
                        enemyList[i].GetComponent<gingeyMovement>().stopMove(true);
                    }
                    else
                    {
                        enemyList[i].GetComponent<gingeyMovement>().targetOBJ(GameObject.Find("player"));
                        enemyList[i].GetComponent<gingeyMovement>().stopMove(false);
                    }
                }
                else
                { //if is null remove from list
                    enemyList.RemoveAt(i);
                    print("removed i from thing");
                }
            }

        }
    }
}
