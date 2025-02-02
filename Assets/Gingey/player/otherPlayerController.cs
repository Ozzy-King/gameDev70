using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class otherPlayerController : MonoBehaviour
{
    public GameObject headJoint;
    public GameObject player;

    public Animator playerAnimator;
    public movePacket PositionInfo;

    public bool inDie = false;
    public bool inAttack = false;

    // Start is called before the first frame update
    void Start()
    {
        playerAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        player.transform.position = new Vector3(PositionInfo.posx, PositionInfo.posy, PositionInfo.posz);
        player.transform.eulerAngles = new Vector3(PositionInfo.rotx, PositionInfo.roty, PositionInfo.rotz);
        headJoint.transform.eulerAngles = new Vector3(PositionInfo.rothx, PositionInfo.rothy, PositionInfo.rothz);
        playerAnimator.SetBool("moveingBool", PositionInfo.moveingBool);
        playerAnimator.SetBool("reverseBool", PositionInfo.reverseBool);
        playerAnimator.SetBool("runningBool", PositionInfo.runningBool);
        if (PositionInfo.attackTrigger == true && inAttack == false) { 
            playerAnimator.SetTrigger("attackTrig");
        }
        inAttack = PositionInfo.attackTrigger;
        if (PositionInfo.dieTrigger == true && inDie == false)
        {
            playerAnimator.SetTrigger("dieTrig");
        }
        inDie = PositionInfo.dieTrigger;
    }
}
