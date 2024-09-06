using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Textdissolver : MonoBehaviour
{
    public float drate;
    public TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
        
        if(text.text != ""&& text.color.a < 0){
            text.text = "";
            Debug.Log("text is empty");
            Color textColor = text.color;
            textColor.a = 255;
        }
        
        if(text.text != ""){
            Color textColor = text.color;
            textColor.a -= drate * Time.deltaTime;
            Debug.Log(textColor.a);
            text.color = textColor;
        }
        
    }
}
