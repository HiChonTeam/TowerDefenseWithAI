using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Witch : Tower
{
    // Start is called before the first frame update
    private void Start()
    {
        baseAtk = 200;
        baseSpd = 1.0f;
        range = new int[] {1, 1, 1};
        exrange = new int[] {0, 1, 1};
        canAtkArmoured = true;
        canAtkCamou = true;
        canAtkFly = true;
        target = -1;
        type = "magic";
    }

    public void BuffAlly(GameObject targetTower)
    {
        Tower targetComp = targetTower.GetComponent<Tower>();
        if(skillUpgraded[0] == 1 && skillUpgraded[1] != 4 && enemyInRange.Count == 0)
        {
            targetComp.AtkBuff(0.2f);
            targetComp.SpdBuff(0.2f);
        }
        if(skillUpgraded[0] >= 2 && skillUpgraded[1] != 4)
        {
            targetComp.AtkBuff(0.4f);
            targetComp.SpdBuff(0.4f);
        }
        if(skillUpgraded[0] >= 3)
        {
            targetComp.DiscountBuff(0.3f);
        }
        if(skillUpgraded[0] == 4)
        {
            targetComp.RangeBuff(1);
            targetComp.AtkBuff(1.0f);
            targetComp.SpdBuff(1.0f);
        }
        if(skillUpgraded[1] >= 3)
        {
            targetComp.AtkBuff(1.0f);
        }
        if(skillUpgraded[2] >= 1)
        {
            targetComp.MagicDmgBuff(0.4f);
        }
        if(skillUpgraded[2] >= 2)
        {
            targetComp.CanAtkAllBuff(true);
        }
    }

    private void AwokeReceiveBuff()
    {
        foreach(GameObject tower in TowerOnMap.towersOnMap)
        {
            Tower towerComp = tower.GetComponent<Tower>();
            towerComp.ReceiveBuff();
        }
    }

    public override void UpgradeSkill(int skillNumber)
    {
        if(skillNumber == 1 && skillUpgraded[1] == 3)
        {
            bool isHaveFrostArcher = false;
            bool isHaveDragonBreathSorcerer = false;
            foreach(GameObject tower in TowerOnMap.towersOnMap)
            {
                if(tower.GetComponent<Archer>() != null)
                {
                    if(tower.GetComponent<Archer>().GetSkillUpgraded(1) == 4)
                    {
                        isHaveFrostArcher = true;
                    }
                }
                else if(tower.GetComponent<Sorcerer>() != null)
                {
                    if(tower.GetComponent<Sorcerer>().GetSkillUpgraded(0) == 4)
                    {
                        isHaveDragonBreathSorcerer = true;
                    }
                }
            }
            if(isHaveFrostArcher == true && isHaveDragonBreathSorcerer == true)
            {
                skillUpgraded[1] += 1;
                UpdateStatBySkill(1, 4);
            }
        }
        else{
            if(skillUpgraded[skillNumber] < 4)
            {
                skillUpgraded[skillNumber] += 1;
                UpdateStatBySkill(skillNumber, skillUpgraded[skillNumber]);
            }
        }
        
    }

    protected override void UpdateStatBySkill(int skillNumber, int skillLevel) //skill coding will override this function
    {
        // Debug.Log("upgraded lv" + skillLevel);
        if(skillNumber == 0)
        {
            if(skillLevel == 1)
            {
                //buff
                if(skillUpgraded[1] == 4)
                {
                    IncreaseAtk(0.4f);
                }
            }
            else if(skillLevel == 2)
            {   //buff
                if(skillUpgraded[1] == 4)
                {
                    IncreaseAtk(0.6f);
                }
                else
                {
                    timeToNextAttack = float.MaxValue; //never attack again
                }
            }
            else if(skillLevel == 3)
            {
                //buff
            }
            else if(skillLevel == 4)
            {
                //buff
            }
        }

        else if(skillNumber == 1)
        {
            if(skillLevel == 1)
            {
                IncreaseRange(new int[] {1, 1, 1});
            }
            else if(skillLevel == 2)
            {
                IncreaseRange(new int[] {1, 1, 1, 1, 1});
                IncreaseEXRange(new int[] {0, 0, 1, 1});
            }
            else if(skillLevel == 3)
            {
                //buff
            }
            else if(skillLevel == 4)
            {
                IncreaseSpd(10.0f);
                if(skillUpgraded[0] >= 1)
                {
                    IncreaseAtk(0.4f);
                }
                if(skillUpgraded[0] == 2)
                {
                    timeToNextAttack = 0; //remove unable to
                    IncreaseAtk(0.6f);
                }
            }
        }

        else if(skillNumber == 2)
        {
            if(skillLevel == 1)
            {
                //buff
            }
            else if(skillLevel == 2)
            {
                //buff
            }
            else if(skillLevel == 3)
            {
                foreach(GameObject tower in TowerOnMap.towersOnMap)
                {
                    if(tower.GetComponent<Witch>() != null)
                    {
                        income += 50;
                    }
                }
            }
            else if(skillLevel == 4)
            {
                income += 500;
            }
        }
        AwokeReceiveBuff();
    } 
}
