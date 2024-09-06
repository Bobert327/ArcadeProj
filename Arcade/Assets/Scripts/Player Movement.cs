using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.XR;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    public MeshRenderer mesh;
    public Material idle;
    public Material runLeft;
    public Material runRight;
    public float moveSpeed = 5f;
    public float rotationSpeed;
    public float materialSwitchTime;
    private bool canRotate = true;
  
    float mst;
    private float yrot;
    private Vector3 moveDirection;

    bool matswitch = true;
      bool matframe = true;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        mst = materialSwitchTime;
yrot = transform.rotation.y;
        
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
       //character controller moves forward
        moveDirection = new Vector3(horizontal, 0.0f, vertical);

        controller.Move(transform.forward * vertical * moveSpeed* Time.deltaTime);
        
        if(canRotate){
            this.transform.Rotate(0, horizontal*rotationSpeed, 0);
            }
        if(Input.GetAxis("Horizontal") > 0|| Input.GetAxis("Vertical") > 0|| Input.GetAxis("Horizontal") < 0|| Input.GetAxis("Vertical") < 0&&canRotate){
       
        
            if(matframe){
                if(matswitch){
                    mesh.material = runLeft;
                    matswitch = !matswitch;
                }
                else{
                    mesh.material = runRight;
                    matswitch = !matswitch;
                }
                matframe = false;
            }

            materialSwitchTime = materialSwitchTime - Time.deltaTime%1;
                if(materialSwitchTime <= 0){
                    if(matswitch){
                        mesh.material = runLeft;
                        matswitch = false;
                        materialSwitchTime = mst;
                    }else{
                        mesh.material = runRight;
                        matswitch = true;
                        materialSwitchTime = mst;
                    }
            }

        }
        else{
            mesh.material = idle;
            materialSwitchTime = mst;
            matframe = true;
            
        }
    }
    public void MovePlayerTo(GameObject moveto){
        //move player to object
        while(Vector3.Distance(this.transform.position, moveto.transform.position) > 0.1){
            this.transform.position = Vector3.MoveTowards(this.transform.position, moveto.transform.position, moveSpeed*Time.deltaTime);
        }
    }
    public void stopControl(){
        //stop player from moving
        controller.enabled = false;
        canRotate = false;
    }
    public void startControl(){
        //start player from moving
        controller.enabled = true;
        canRotate = true;
    }
}

