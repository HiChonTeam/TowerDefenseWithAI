using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : TowerPlacement
{
    private int Cost = 0;
    // TowerPlacement towerplacement;
    // protected override void PurchaseTurrent(string TowerType){
    // }   

    public bool CheckMoneyEnough(int cost){
        if(StatusController.userMoney >= cost ){
            StatusController.userMoney -= cost;
            // TowerPlacement.PurchaseTurrent(1);
            return true;
        }
        else{
            Debug.Log("Not Enough Money");
            return false;
        }
    }

    public void OnclickButton1(){
        Cost = 500;
        //Debug.Log("Shoppppppppppppppppppppppppppppppppppppppppppppppp");
        if(CheckMoneyEnough(Cost) == true){
            PurchaseTurrent(1);
        }
        else{
            Debug.Log("Not Enough Money");
        }
    }
    public void OnclickButton2(){
        Cost = 1200;
        if(CheckMoneyEnough(Cost) == true){
            PurchaseTurrent(2);
        }
        else{
            Debug.Log("Not Enough Money");
        }
    }
    public void OnclickButton3(){
        Cost = 700;
        if(CheckMoneyEnough(Cost) == true){
            PurchaseTurrent(3);
        }
        else{
            Debug.Log("Not Enough Money");
        }
    }
    public void OnclickButton4(){
        Cost = 500;
        if(CheckMoneyEnough(Cost) == true){

            PurchaseTurrent(4);
        }
        else{
            Debug.Log("Not Enough Money");
        }
    }
    public void OnclickButton5(){
        Cost = 1500;
        if(CheckMoneyEnough(Cost) == true){
            PurchaseTurrent(5);
        }
        else{
            Debug.Log("Not Enough Money");
        }
    }
}
