using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class genScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {        
        print("testing");
        if (collision.gameObject.name == "player")
        {
            collision.gameObject.GetComponent<playercameraLook>().health += 10;
            Destroy(transform.gameObject);
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        print("testing");
        if (other.gameObject.name == "player")
        {
            other.gameObject.GetComponent<playercameraLook>().health += 10;
            Destroy(transform.gameObject);
        }
    }

}
