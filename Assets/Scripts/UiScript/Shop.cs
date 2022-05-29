using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// [RequireComponent(typeof(Button))]

public class Shop : TowerPlacement
{
    private List<int> Cost = new List<int>(){500, 1200, 700, 500, 1500};
    public static bool canBuy = true;
    [SerializeField] private List<Button> buttons;
    private bool simulating = false;

    public bool CheckMoneyEnough(int cost){
        if(StatusController.userMoney >= cost ){
            return true;
        }
        else{
            return false;
        }
    }

    public void OnclickButton1(){
        if(canBuy)
        {
            if(CheckMoneyEnough(Cost[0]) == true){
                PurchaseTurrent(1, Cost[0]);
            }
            else{
                Debug.Log("Not Enough Money");
            }
        }
        
    }
    public void OnclickButton2(){
        if(canBuy)
        {
            if(CheckMoneyEnough(Cost[1]) == true){
                PurchaseTurrent(2, Cost[1]);
            }
            else{
                Debug.Log("Not Enough Money");
            }
        }
        
    }
    public void OnclickButton3(){
        if(canBuy)
        {
            if(CheckMoneyEnough(Cost[2]) == true){
                PurchaseTurrent(3, Cost[2]);
            }
            else{
                Debug.Log("Not Enough Money");
            } 
        }
    }
    public void OnclickButton4(){
        if(canBuy)
        {
            if(CheckMoneyEnough(Cost[3]) == true){

                PurchaseTurrent(4, Cost[3]);
            }
            else{
                Debug.Log("Not Enough Money");
            }
        }
        
    }
    public void OnclickButton5(){
        if(canBuy)
        {
            if(CheckMoneyEnough(Cost[4]) == true){
                PurchaseTurrent(5, Cost[4]);
            }
            else{
                Debug.Log("Not Enough Money");
            }
        }
    }

    private void Update()
    {
        if(!RoundController.AIsimulate)
        {
            for(int i = 0; i < buttons.Count; i++)  
            {
                if(CheckMoneyEnough(Cost[i]))
                {
                    buttons[i].GetComponent<Button>().interactable = true;
                }
                else
                {
                    buttons[i].GetComponent<Button>().interactable = false;
                }
            }
        }
        else
        {
            foreach(Button button in buttons)
            {
                button.GetComponent<Button>().interactable = false;
            }
        }
    }
}
