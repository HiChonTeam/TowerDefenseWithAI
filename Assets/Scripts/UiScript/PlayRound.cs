using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayRound : MonoBehaviour
{
    public void OnclickButton(){
        
        RoundController.isStartPressed = true;
        gameObject.GetComponent<Button>().interactable = false;
    
    }
}
