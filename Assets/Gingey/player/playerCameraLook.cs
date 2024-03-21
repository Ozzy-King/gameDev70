using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    float walkSpeed = 13f;
    float runSpeed = 26.1f;

    // Start is called before the first frame update
    void Start()
    {
        playerAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //player movement in world
        //needs to update first to the head rotateion sync to body movement porperly else the body move and the head get left behuind till next update

        //forawrd1 backward-1
        float temp;
        if ((temp = Input.GetAxis("Vertical")) != 0)
        {
            playerAnimator.SetBool("moveingBool", true);
            playerAnimator.SetBool("reverseBool", (temp == -1 ? true : false)) ;
            moveSpeed = walkSpeed;

            if (Input.GetKey(KeyCode.LeftShift))
            { //for if running
                playerAnimator.SetBool("runningBool", true);
                moveSpeed = runSpeed;
            }
            else {
                playerAnimator.SetBool("runningBool", false);
                moveSpeed = walkSpeed;
            }


            player.transform.Rotate(0, currentYRot, 0);
            currentYRot = 0;
            if (temp == 1)
            {
                transform.position += transform.forward * Time.deltaTime * moveSpeed;
            }
            if (temp == -1)
            {
                transform.position -= transform.forward * Time.deltaTime * moveSpeed;
            }
        }
        else {
            playerAnimator.SetBool("moveingBool", false);
            playerAnimator.SetBool("runningBool", false);
        }





        //camera movement along with head movement
        // gathered mouse values 
        float leftRight = Input.GetAxis("Mouse X") * mouseSensitivity;
        float UpDown = Input.GetAxis("Mouse Y") * mouseSensitivity;

        //add values to cameras up down abnd clamp when needed
        currentXRot += -UpDown;
        currentXRot = Mathf.Clamp(currentXRot, -36, 36);

        //add values to left right and sortout overlfow by rotating the body and maxing out head rotation
        currentYRot += leftRight;
        if (currentYRot > 33) {
            player.transform.Rotate(0, currentYRot - 33, 0);
            //currentYRot = 36;
        }
        else if (currentYRot < -33) {
            player.transform.Rotate(0, currentYRot + 33, 0);
            //currentYRot = -36;
        }
        currentYRot = Mathf.Clamp(currentYRot, -33, 33);

        //resert position of head so its level with world(its not originally due to animation)
        headJoint.transform.eulerAngles = player.transform.eulerAngles ;// new Vector3(0f,0f,0f);
        //rotate to correct look poisiton
        headJoint.transform.Rotate(currentXRot, 0f, 0f);
        headJoint.transform.Rotate(0f, currentYRot, 0f, Space.World);

        //float forward = Input.GetAxis("Vertical") * moveSpeed;
        //float side = Input.GetAxis("Horizontal") * moveSpeed;

        //player.transform.position += new Vector3(side, 0, forward);
    }
}
