using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusController : MonoBehaviour
{
    [SerializeField] private Text HealthText;
    [SerializeField] private Text MoneyText;
    

    public static int userMoney = 1000;
    public static int userLife = 20;


    private void Update(){
        HealthText.text = "Health:"+ userLife;
        MoneyText.text  = "Money:" + userMoney;
    }

    // void Haelther
}
