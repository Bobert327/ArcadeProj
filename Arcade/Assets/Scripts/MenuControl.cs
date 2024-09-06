using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;
public class MenuControl : MonoBehaviour
{
    public float offset;
    private TextMeshProUGUI text;
    public Boolean Genesis;
    public Boolean Atari;
    public Boolean Build;
    private int buttonCount;
    private int gameCount;
    public Canvas canvas;
    public GameObject button;
    private Vector2 buttorigin;
    //public GameObject array based on the number of buttons
    public GameObject[] buttonArray;
    public GameObject[] gameArray;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        
    }
    public void SpawnButtons(){
        buttonCount = buttonArray.Length;

        Debug.Log("SpawnButtons");
        //get size of array buttons
        int size = buttonArray.Length;
        for(int i = 0; i<buttonCount; i++){
            Debug.Log("Button "+ i);
            buttonArray[i].SetActive(true);
        }
    }
    public void gameDisplay(GameObject Console){
        Debug.Log(Console.GetComponent<ConsoleInfo>().numGames);
    for(int i = 0;i<Console.GetComponent<ConsoleInfo>().numGames;i++){
        //text equals text chile of gameArray[i]
        TextMeshProUGUI text = gameArray[i].GetComponentInChildren<TextMeshProUGUI>();
        Debug.Log(text.text);
        gameArray[i].gameObject.SetActive(true);
        text.text = Console.GetComponent<ConsoleInfo>().gameName[i];
    }
    }
    public void buttonHide(){
        for(int i = 0;i<buttonArray.Length;i++){
            buttonArray[i].SetActive(false);
        }
    }
    public void gameHide(){
gameCount = gameArray.Length;
        for(int i = 0;i<gameCount;i++){

        gameArray[i].gameObject.SetActive(false);
    }
    }

    
}
