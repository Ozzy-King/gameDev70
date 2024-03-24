using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]
public class gingeyMovement : MonoBehaviour
{
   
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

    }

    void Update()
    {

        //targetPos = hit.point;
        if (GameObject.Find("player") != null) {
            targetPos = GameObject.Find("player").transform.position;
        }

        agent.SetDestination(targetPos);


        //based on distance from previous and new position set walking annimation
        if (Vector3.Distance(oldPos, gameObject.transform.position) < distToWalk) {
            ModelAnimator.SetBool("moveingBool", false);
        }
        else {
            ModelAnimator.SetBool("moveingBool", true);
        }
        oldPos = gameObject.transform.position;

        // Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        // Debug.DrawLine(ray.origin, ray.direction * 20, Color.red);
    }
}