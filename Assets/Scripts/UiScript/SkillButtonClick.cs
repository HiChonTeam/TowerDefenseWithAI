using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillButtonClick : MonoBehaviour
{
    private int skillCost = 0;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI costText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI descText;
    [SerializeField] int thisSkillNumber = 0;
    private TowerPlacement tp;
    private Tower tower;
    private bool levelmax = false;

    public void Click(){
        tp.upgradeTower(tower, thisSkillNumber);
    }

    public void setSkill(string name, int cost, string details, string level, Tower tower)
    {
        nameText.GetComponentInChildren<TextMeshProUGUI>().text = name;
        costText.GetComponentInChildren<TextMeshProUGUI>().text = cost + "$";
        descText.GetComponentInChildren<TextMeshProUGUI>().text = details;
        levelText.GetComponentInChildren<TextMeshProUGUI>().text = level;
        skillCost = cost;
        this.tower = tower;
        if(level == "max") levelmax = true;
        else levelmax = false;
    }

    public void setupButton(TowerPlacement tpsetup)
    {
        tp = tpsetup;
    }

    private void Update()
    {
        if(StatusController.userMoney < skillCost || levelmax || RoundController.AIsimulate)
        {
            gameObject.GetComponent<Button>().interactable = false;
        }
        else
        {
            gameObject.GetComponent<Button>().interactable = true;
        }
    }
}
