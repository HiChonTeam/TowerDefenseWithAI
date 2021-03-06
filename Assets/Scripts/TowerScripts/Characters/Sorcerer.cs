using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sorcerer : Tower
{
    private Animator animator;

    private float pointBlankDmg = 0.0f;
    // Start is called before the first frame update
    private void Start()
    {
        SkillCost = new int[,] {{400,800,2000,3500},{300,500,500,4500},{400,800,1600,3200}};
        SkillName = new string[,] {
            {
            "Shotmagic",
            "Powerful magic",
            "Magic missile",
            "Dragon’s breath",
            }
            ,
            {
            "Stone bullet",
            "Mana eye",
            "Slug magic",
            "Meteorite",
            }
            ,
            {
            "Quick spell",
            "Without casting",
            "Endless mana",
            "Genius Sorcerer",
            }
        };
        SkillDetail = new string[,] {
            {
            "Sorcerer deal up to 150% more damage to enemy within point-bank range (1.5px).",
            "Sorcerer’s attack power + 100%",
            "Reduce Sorcerer’s attack range, but Sorcerer can deal damage to an enemy 5 times at once.",
            "Reduce Sorcerer’s attack range, but increase Sorcerer attack power by 500%. Also, when enemies arrived to Sorcerer’s range first time, they will receive damage equal to 50% of their current HP."
            }
            ,
            {
            "Cast magic as earth element. Sorcerer’s now can deal damage to armoured enemy.",
            "Sorcerer’s now can deal damage to camouflage enemy.",
            "Change Sorcerer’s attack range. Shotmagic upgrade has longer effective range (3px). ",
            "Increase Sorcerer’s attack range. Sorcerer’s magic now deals damage to all enemies and increase attack power by 400%, but decrease attack speed by 50%. The attack will cause burns on enemies, burning enemies will damage its HP by 100 constant damage every second and remove its camouflage and armoured."
            }
            ,
            {
            "Sorcerer’s attack speed +50%.",
            "Sorcerer’s attack speed + 100%",
            "Every time Sorcerer cast magic, Sorcerer’s attack speed will increase by 10%. This effect will reset every time wave end.",
            "Sorcerer’s magic will deal X true damage to all enemies in range instead of normal attack. Also, increase Sorcerer’s attack speed by 30%. X equal to 100 * current attack speed. Upgrade Shotmagic and Powerful magic will increase Sorcerer’s attack speed by 50% and 100% respectively."
            }
        };
        baseAtk = 1000; 
        baseSpd = 1.0f;
        range = new int[] {3, 2, 2};
        canAtkGround = true;
        type = "magic";

        animator = GetComponent<Animator>();
    }

    protected override void setAnimationAttack(){
        
        animator.SetBool("isAttackAnimation", true);
    }

    protected override void setAnimationIdle(){
        animator.SetBool("isAttackAnimation", false);
    } 

    protected override void BeforeAttack(GameObject targetEnemy)
    {
        Enemy enemyScript = targetEnemy.GetComponent<Enemy>();
        if(skillUpgraded[0] >= 1 && skillUpgraded[2] < 4)
        {
            if(Vector2.Distance(gameObject.transform.position, targetEnemy.transform.position) <= 1.5f)
            {
                if(skillUpgraded[1] >= 3)
                {
                    pointBlankDmg = 3.0f;
                }
                else
                {
                    pointBlankDmg = 1.5f;
                }
                
                IncreaseDmgToAll(pointBlankDmg);
            }
            else
            {
                pointBlankDmg = 0.0f;
            }
        }
        if(skillUpgraded[1] >= 4)
        {
            enemyScript.setIsArmoured(false);
            enemyScript.setIsCamou(false);
            enemyScript.takeConstantDamageEverySeconds(100, "burn");
        }
        if(skillUpgraded[2] >= 3)
        {
            IncreaseSpdUntilEndOfWave(0.1f);
        }
        if(skillUpgraded[2] >= 4)
        {
            if( //check if can attack or not
                !(enemyScript.getIsFly() && (!canAtkFly && !canAtkAllBuff)) &&
                !(!enemyScript.getIsFly() && (!canAtkGround && !canAtkAllBuff)) &&
                !(enemyScript.getIsArmoured() && (!canAtkArmoured && !canAtkAllBuff)) &&
                !(enemyScript.getIsCamou() && (!canAtkCamou && !canAtkAllBuff))
            )
            {
                enemyScript.takeTrueDamage((int)(baseSpd * spdModifier * spdBuff * spdModifierUntilEndWave * 100));
            }
        }

    }

    protected override void AfterAttack(GameObject targetEnemy)
    {
        dmgToAll -= pointBlankDmg;
    }

    protected override void ActionToFirstMetEnemy(GameObject targetEnemy)
    {
        if(skillUpgraded[0] >= 4)
        {
            targetEnemy.GetComponent<Enemy>().takeConstantDamage(0.5f);
        }
    }

    // Update is called once per frame
    protected override void UpdateStatBySkill(int skillNumber, int skillLevel)
    {
        // Debug.Log("upgraded lv" + skillLevel);
        if(skillNumber == 0)
        {
            if(skillLevel == 1)
            {
                //point bank damage
                if(skillUpgraded[2] >= 4)
                {
                    IncreaseSpd(0.5f);
                }
            }
            else if(skillLevel == 2)
            {
                if(skillUpgraded[2] >= 4)
                {
                    IncreaseSpd(0.5f);
                }
                else
                {
                    IncreaseAtk(1.0f);
                } 
            }
            else if(skillLevel == 3)
            {
                IncreaseRange(new int[] {-1});
                SetNumberOfHits(5);
            }
            else if(skillLevel == 4)
            {
                IncreaseRange(new int[] {0, -1, -1});
                IncreaseAtk(5.0f);
                //take constant damage to first met enemies
            }
        }
        else if(skillNumber == 1)
        {
            if(skillLevel == 1)
            {
                CanAtkArmoured(true);
            }
            else if(skillLevel == 2)
            {
                CanAtkCamou(true);
            }
            else if(skillLevel == 3)
            {
                //shot magic
                IncreaseRange(new int[] {2, -1, -1});
            }
            else if(skillLevel == 4)
            {
                IncreaseRange(new int[] {0, 4, 4});
                SetTarget(-1); 
                IncreaseAtk(4.0f);
                IncreaseSpd(-0.5f);
                //remove camouflage and armoured
                //damage overtime
            }
        }
        else if(skillNumber == 2)
        {
            if(skillLevel == 1)
            {
                IncreaseSpd(0.5f);
            }
            else if(skillLevel == 2)
            {
                IncreaseSpd(1.0f);
            }
            else if(skillLevel == 3)
            {
                //increase speed every time hit
            }
            else if(skillLevel == 4)
            {
                //deal true damage
                SetNumberOfHits(0);
                if(skillUpgraded[0] >= 2)
                {
                    IncreaseSpd(1.3f);
                }
                else if(skillUpgraded[0] == 1)
                {
                    IncreaseSpd(0.8f);
                }
                else
                {
                    IncreaseSpd(0.3f);
                }
            }
        }
    }
}
