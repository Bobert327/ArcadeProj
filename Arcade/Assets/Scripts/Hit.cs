using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Hit : MonoBehaviour
{
    public UnityEvent  overlap;
    public UnityEvent endOverlap;
    public UnityEvent  hitEvent;
    public GameObject intruder;
    private bool inside = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(inside&&Input.GetKeyDown(KeyCode.E)){
            Debug.Log("Hit");
            hitEvent.Invoke();
        }
    }

    //Overlap with intruder function
    void OnTriggerEnter(Collider other)
    {
      
        if (other.gameObject == intruder)
        {
              overlap.Invoke();
            inside = true;
            Debug.Log("Inside");
            
        }
    }
    void OnTriggerExit(Collider other)
    {
        
        if (other.gameObject == intruder)
        {
            endOverlap.Invoke();
            inside = false;
            Debug.Log("Outside");
        }
    }
}
