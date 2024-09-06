using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Money : MonoBehaviour
{
    public string score;
    public int money = 0;
    public retro retro;


    // Start is called before the first frame update
    

    // Update is called once per frame
    void Update()
    {
    
    }
    public void AddMoney(){
        //convert score to int
            score = retro.bigEnd;
        int Score = int.Parse(score);
    money = money + Score;
    }
}
