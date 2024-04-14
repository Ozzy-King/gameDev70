using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

/// <summary>
/// full controller for player movemnet and camera movemnt
/// </summary>
public class playercameraLook : MonoBehaviour
{
    //cameras is child if the head joint
    public GameObject headJoint;
    public GameObject player;
    public float currentXRot = 0f;
    public float currentYRot = 0f;

    //placeholder for later
    public float mouseSensitivity = 1f;
    public float moveSpeed = 1f;

    public Animator playerAnimator;

    public NavMeshAgent agent;

    bool dieTrig = false;
    bool attackTrig = false;

    float walkSpeed = 20.98f;
    float runSpeed = 28.1f;

    public bool dead = false;
    public int health = 100;

    public GameObject pausedMenu;
    public bool isPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.acceleration = 100000;
        pausedMenu = GameObject.Find("pauseScreen");
    }

    public void takeDamage(int amount) {
        health -= amount;
    }
    public void healDamage(int amount)
    {
        health += amount;
    }
    bool stepping = false;

    // Update is called once per frame
    void LateUpdate()
    {
        //player movement in world
        //needs to update first to the head rotateion sync to body movement porperly else the body move and the head get left behuind till next update
        float temp =0;
        float leftRight=0;
        float UpDown = 0;
        if (health > 0 && isPaused == false)
        {

            //forawrd1 backward-1
            if ((temp = Input.GetAxis("Vertical")) != 0)
            {
                playerAnimator.SetBool("moveingBool", true);
                playerAnimator.SetBool("reverseBool", (temp == -1 ? true : false));
                agent.speed = walkSpeed;

                if (Input.GetKey(KeyCode.LeftShift))
                { //for if running
                    playerAnimator.SetBool("runningBool", true);
                    agent.speed = runSpeed;
                }
                else
                {
                    playerAnimator.SetBool("runningBool", false);
                    agent.speed = walkSpeed;
                }


                player.transform.Rotate(0, currentYRot, 0);
                currentYRot = 0;
                if (temp == 1)
                {
                    agent.Move(transform.forward * (agent.speed == runSpeed ? 2 : 1));//moves the agent as character instead of npc
                }
                else if(temp == -1)
                {
                    agent.Move(-transform.forward * (agent.speed == runSpeed ? 2 : 1));//moves the agent as character instead of npc    
                }
                if (!stepping) {
                    GameObject.Find("menuScreens").GetComponent<soundController>().playRandomStep();
                    stepping = true;
                }
                else if(!GameObject.Find("menuScreens").GetComponent<soundController>().soundSource.isPlaying) {
                    stepping = false;
                }
            }
            else
            {
                playerAnimator.SetBool("moveingBool", false);
                playerAnimator.SetBool("runningBool", false);
                agent.Stop();
                //agent.isStopped = true;
            }





            //camera movement along with head movement
            // gathered mouse values 
            leftRight = Input.GetAxis("Mouse X") * mouseSensitivity;
            UpDown = Input.GetAxis("Mouse Y") * mouseSensitivity;

            //add values to cameras up down abnd clamp when needed
            currentXRot += -UpDown;
            currentXRot = Mathf.Clamp(currentXRot, -36, 36);

            //add values to left right and sortout overlfow by rotating the body and maxing out head rotation
            currentYRot += leftRight;
            if (currentYRot > 33)
            {
                player.transform.Rotate(0, currentYRot - 33, 0);
                //currentYRot = 36;
            }
            else if (currentYRot < -33)
            {
                player.transform.Rotate(0, currentYRot + 33, 0);
                //currentYRot = -36;
            }
            currentYRot = Mathf.Clamp(currentYRot, -33, 33);

            //resert position of head so its level with world(its not originally due to animation)
            headJoint.transform.eulerAngles = player.transform.eulerAngles;// new Vector3(0f,0f,0f);
                                                                           //rotate to correct look poisiton
            headJoint.transform.Rotate(currentXRot, 0f, 0f);
            headJoint.transform.Rotate(0f, currentYRot, 0f, Space.World);

            //if left mouse button is clicked and not in attack animation
            if (Input.GetMouseButtonDown(0) && !playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("attack"))
            {
                //set state to attacking will send state to other player if online
                attackTrig = true;
                playerAnimator.SetTrigger("attackTrig");
                temp = 1;

                //checks if enemy is in front, if so damage the enemy
                //currently onehit kills enemys
                RaycastHit hitEnemy;
                if (Physics.Raycast(player.transform.position, transform.forward, out hitEnemy, 10f, 1<<6))
                {
                    hitEnemy.transform.gameObject.GetComponent<gingeyMovement>().health -= 10;
                }

            }

            if (Input.GetKey(KeyCode.P)) {
                GameObject.Find("GameController").GetComponent<gameControllerScript>().pause(true);
                isPaused = true;
            }

        }
        //for deteting when death animation should be played and sent to other clients
        if (health <= 0)
        {
            float stateTime = playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if (!dead)
            {
                GameObject.Find("menuScreens").GetComponent<soundController>().playDeathSound();
                playerAnimator.SetTrigger("dieTrig");
                dead = true;
                dieTrig = true;
                temp = 1;
            }
            //if in the animtion is in die and is at the end of the animation destory object
            else if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("die") && ((int)stateTime) == 1)
            {
                GameObject.Find("GameController").GetComponent<gameControllerScript>().BackMainMenu();
            }
            
            
        }

        //set hlaeth bar to current health
        GameObject.Find("GameController").GetComponent<gameControllerScript>().userMenu.transform.GetChild(0).GetComponent<Slider>().value = health;


        GameObject networkObj;

        if ((leftRight != 0 || UpDown != 0 || temp != 0) && (networkObj=GameObject.Find("NETWORKER") ) != null) {
            clientSide networkObjScript = networkObj.GetComponent<clientSide>();
            basePacket packet = new movePacket();
            movePacket pack = packet as movePacket;
            pack.runningBool = playerAnimator.GetBool("runningBool");
            pack.reverseBool = playerAnimator.GetBool("reverseBool");
            pack.moveingBool = playerAnimator.GetBool("moveingBool");

            pack.posx = player.transform.position.x;
            pack.posy = player.transform.position.y;
            pack.posz = player.transform.position.z;

            pack.rotx = player.transform.eulerAngles.x;
            pack.roty = player.transform.eulerAngles.y;
            pack.rotz = player.transform.eulerAngles.z;

            pack.rothx = headJoint.transform.eulerAngles.x;
            pack.rothy = headJoint.transform.eulerAngles.y;
            pack.rothz = headJoint.transform.eulerAngles.z;

            pack.attackTrigger = attackTrig;
            attackTrig = false;
            pack.dieTrigger = dieTrig;
            dieTrig = false;


            networkObjScript.sendPacket(packet);
        
        }

        //float forward = Input.GetAxis("Vertical") * moveSpeed;
        //float side = Input.GetAxis("Horizontal") * moveSpeed;

        //player.transform.position += new Vector3(side, 0, forward);
    }
}
