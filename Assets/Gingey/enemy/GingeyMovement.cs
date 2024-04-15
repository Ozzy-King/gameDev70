using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]
public class gingeyMovement : MonoBehaviour
{

    public GameObject[] gems = new GameObject[5];

    private NavMeshAgent agent;
    private Animator ModelAnimator;
    private Camera mainCamera;

    //used for deteriming movement aspects
    private Vector3 oldPos; //move?
    public Vector3 targetPos; //to where
    public int moveStyle = 0; //how to move
    public int idleStyle = 0; //how to stand
    private float[] moveSpeeds = { 5.6f, 8.7f, 24.98f }; //at what speed

    public float distToWalk = 0.005f;

    public int health = 100;
    public bool dead = false;
    public bool attack = false;

    public bool paused = false;

    void Start() {
        agent = GetComponent<NavMeshAgent>();
        ModelAnimator = GetComponent<Animator>();
        mainCamera = Camera.main;

        //choose idle style
        idleStyle = Random.Range(0, 2);
        ModelAnimator.SetInteger("idleStyle", idleStyle);
        //choose move style
        moveStyle = Random.Range(0, 3); 
        ModelAnimator.SetInteger("moveStyle", moveStyle);
        agent.speed = moveSpeeds[moveStyle];

        health = Random.Range(10, 20);
        targetPos = gameObject.transform.position;
    }

    public void targetOBJ(GameObject obj)
    {
            targetPos = obj.transform.position;
    }
    public void stopMove(bool stop) {
        paused = stop;
    }

    public void attackplayer(GameObject obj)
    {
        ModelAnimator.SetTrigger("attackTrig");
        //if withing 10 units
        playercameraLook test1;
        //if the obj attack has the playerCameraLook script minus health
        if ((test1 = obj.GetComponent<playercameraLook>()) != null)
        {
            test1.health -= 3;
        }
        agent.isStopped = true;
        
        attack = true;
    }

    bool stepping = false;
    void Update()
    {
        //stops subsiquence attacks till the current one is done
        if (paused)
        {
            agent.SetDestination(gameObject.transform.position);
        }
        else if (attack) {
            float stateTime = ModelAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if (!ModelAnimator.GetNextAnimatorStateInfo(0).IsName("attack") && !ModelAnimator.GetCurrentAnimatorStateInfo(0).IsName("attack"))
            {
                attack = false;
                agent.isStopped = false;
            }
        }
        //if still alive and not attacking
        else if (health > 0 && !attack)
        {
            agent.SetDestination(targetPos);

            //based on distance from previous and new position set walking annimation
            if (Vector3.Distance(oldPos, gameObject.transform.position) < distToWalk)
            {
                ModelAnimator.SetBool("moveingBool", false);
            }
            else
            {
                ModelAnimator.SetBool("moveingBool", true);
                if (!stepping)
                {
                    playRandomStep();
                    stepping = true;
                }
                else if (!soundSource.isPlaying)
                {
                    stepping = false;
                }
            }
            oldPos = gameObject.transform.position;

            //when the ray hits the clinets player the enemy will attack the client and reduce helth if it one of the the other players it will just show the attack animation
            RaycastHit playerHit;
            if (Physics.Raycast(gameObject.transform.position, transform.forward, out playerHit, 10f, 1 << 7))
            {
                attackplayer(playerHit.transform.gameObject);
            }

            Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + (gameObject.transform.forward * 10), Color.red);
        }
        //else if dead
        else if (health <= 0) {
            float stateTime = ModelAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if (!dead)
            {
                ModelAnimator.SetTrigger("dieTrig");
                dead = true;
            }
            //if in the animtion is in die and is at the end of the animation destory object
            else if (ModelAnimator.GetCurrentAnimatorStateInfo(0).IsName("die") && ((int)stateTime) == 1) {
                Instantiate(gems[Random.Range(0, 4)], gameObject.transform.position, Quaternion.identity).transform.rotation = Quaternion.Euler(-89.98f,Random.Range(0f, 360f),0);
                Destroy(transform.gameObject);
            }
        }
    }
    public AudioClip[] snowStep = new AudioClip[3];
    public AudioSource soundSource;
    public void playRandomStep()
    {
        soundSource.volume = ((-Mathf.Min(Vector3.Distance(gameObject.transform.position, targetPos), 200)) + 200)/200;
        int randomStep = (int)Random.Range(0, 3);
        soundSource.clip = snowStep[randomStep];
        soundSource.Play();
    }
}