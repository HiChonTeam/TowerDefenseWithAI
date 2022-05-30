using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Witch : Tower
{
    private Animator animator;

    // Start is called before the first frame update
    private void Start()
    {
        SkillCost = new int[,] {{1000,1200,2000,7500},{1200,1600,4444,9999},{800,2500,2000,5000}};
        SkillName = new string[,] {
            {
            "Support magic",
            "Focus on support",
            "Talent awakening",
            "Limit break",
            }
            ,
            {
            "Longer range magic",
            "Far sight",
            "Holy dragon aura",
            "Four Heavenly Kings",
            }
            ,
            {
            "Magic teaching",
            "Knowledge teaching",
            "Guild assembly",
            "Alchemist guild",
            }
        };
        SkillDetail = new string[,] {
            {
            "When not have any enemy in range, all allies within range increase attack power and attack speed by 20%.",
            "Support magic skill now effect all time and double its effect, but Witch will not attack enemy anymore.",
            "Reduce amount of money required to upgrade skill for all other allies within range by 30%.",
            "Increase attack power and attack speed of all allies in range by 100%. Also, increase their attack range in front directions by 1 grid. (included Witch)"
            }
            ,
            {
            "Increase Witch’s attack range.",
            "Increase Witch’s attack range even more.",
            "All allies in range increase attack power by 100%.",
            "Only upgrade this skill when Archer with Frost barrage upgraded and Sorcerer with Dragon’s breath upgraded are within battlefield.When this skill upgraded, with the power of three dragons combine into power of Chaturathep, the Witch can attack all enemies on the entire map and increase Witch’s attack speed by 1000%. Support magic and Focus on support upgraded will no longer effective but will increase Witch’s attack power by 40% and 60% respectively."
            }
            ,
            {
            "All allies in range increase magic damage dealt by 40%.",
            "All allies in range can deal damage to all enemy types.",
            "At the end of the wave, gain additional income by X$, X equal to the number of witches in battlefield before upgrade this skill * 50 (included self).",
            "At the end of the wave, gain additional income by 500$."
            }
        };
        baseAtk = 200;
        baseSpd = 1.0f;
        range = new int[] {1, 1, 1};
        exrange = new int[] {0, 1, 1};
        canAtkArmoured = true;
        canAtkCamou = true;
        canAtkFly = true;
        target = -1;
        type = "magic";

        animator = GetComponent<Animator>();
    }

    protected override void setAnimationAttack(){
        
        animator.SetBool("isAttackAnimation", true);
    }

    protected override void setAnimationIdle(){
        animator.SetBool("isAttackAnimation", false);
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
