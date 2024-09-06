using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcadeSpawner : MonoBehaviour
{ 
    public GameObject FakeMachine;
    public GameObject Machine;
    private bool placed = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      if(placed ==false){
        if(Input.GetKeyDown(KeyCode.Space)){
            spawnMachine();
            placed = true;
        
        //destroy fake machine
            Destroy(this.transform.GetChild(2).gameObject);
        }
      }

        
    }
    public void spawnFake(){
       //spawn machine
        Vector3 position = this.transform.position;
        //spawn machine at position of this
      
      Quaternion newrot = this.transform.rotation;
        //newrot = opposite of current rotation;

        //Insantiate FakeMachine as child of this
        Instantiate(FakeMachine, position + transform.forward * 2, newrot, this.transform);
      placed = false;
    }
    public void spawnMachine(){
        //spawn machine
        Vector3 position = this.transform.position;
        //spawn machine at position of this
      
      Quaternion newrot = this.transform.rotation;
        //newrot = opposite of current rotation;

        Instantiate(Machine, position + transform.forward * 2, newrot);
    }
}
