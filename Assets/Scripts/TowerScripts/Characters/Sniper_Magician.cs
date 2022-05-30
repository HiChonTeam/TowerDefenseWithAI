using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper_Magician : Tower
{
    private Animator animator;

    float destroyerBuff = 0.0f;
    // Start is called before the first frame update
    private void Start()
    {
        SkillCost = new int[,] {{800,1200,2000,6000},{1000,1500,3000,6000},{1200,1200,2400,4500}};
        SkillName = new string[,] {
            {
            "Stopping magic",
            "Larger maniber",
            "Mana detector scope",
            "Killing machine",
            }
            ,
            {
            "Piercing shot",
            "Anti-materiel magic",
            "Giant killer",
            "Eraser",
            }
            ,
            {
            "2-cast mode",
            "3-cast mode",
            "Full auto magic",
            "Destroyer",
            }
        };
        SkillDetail = new string[,] {
            {
            "Sniper’s attack increase damage dealt with non-armoured target by 100%.",
            "Sniper’s attack power + 100%.",
            "Sniper’s now can target camouflaged enemy. Also, Sniper’s attack speed + 50%.",
            "Each target Sniper attacked, Sniper’s attack power will increase by 10% and attack speed will increase by 2%. Also, now Sniper can target all types of enemy. These effects will reset every time wave end."
            }
            ,
            {
            "Sniper’s attack can pierce up to 5 targets.",
            "Sniper’s attack can pierce all targets in range. Additional, increase damage dealt to armoured target by 100%.",
            "The first target each Sniper’s shot will receive more damage by 1000%.",
            "Sniper’s shot able to target all types of enemy and increase attack power by 150%. Also, significantly increase Sniper’s attack range."
            }
            ,
            {
            "Sniper’s attack deal double hit.",
            "Sniper’s attack deal triple hit instead of double.",
            "Sniper’s now deal only single hit instead of triple but increase Sniper’s attack speed by 500%.",
            "Sniper’s shot will no more pierce but increase Sniper’s attack power by X%. X equal to 200 * numbers of enemies in range. Upgrade Piercing shot and Anti-materiel magic will increase Sniper’s attack range instead."
            }
        };
        baseAtk = 2500; 
        baseSpd = 0.5f;
        range = new int[] {5};
        SetTarget(newTarget : 2);
        canAtkGround = true;
        canAtkArmoured = true;
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
        if(skillUpgraded[0] >= 4)
        {
            IncreaseAtkUntilEndOfWave(0.1f);
            IncreaseSpdUntilEndOfWave(0.02f);
        }
        if(skillUpgraded[2] >= 4)
        {
            destroyerBuff = enemyInRange.Count * 2.0f;
            atkModifier += destroyerBuff;
        }
    }

    protected override void AfterAttack(GameObject targetEnemy)
    {
        if(skillUpgraded[2] >= 4)
        {
            atkModifier -= destroyerBuff;
            destroyerBuff = 0.0f;
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
                IncreaseDmgToNonArmoured(1.0f);
            }
            else if(skillLevel == 2)
            {
                IncreaseAtk(1.0f);
            }
            else if(skillLevel == 3)
            {
                CanAtkCamou(true);
                IncreaseSpd(0.5f);

            }
            else if(skillLevel == 4)
            {
                //stack buff when attack
            }
        }
        else if(skillNumber == 1)
        {
            if(skillLevel == 1)
            {
                if(skillUpgraded[2] < 4)
                {
                    SetTarget(newTarget : 5);
                }
                else 
                {
                    IncreaseRange(new int[] {1});
                }
            }
            else if(skillLevel == 2)
            {
                if(skillUpgraded[2] < 4)
                {
                    SetTarget(newTarget : -1);
                }
                else 
                {
                    IncreaseRange(new int[] {1});
                }
                IncreaseDmgToArmoured(1.0f);
            }
            else if(skillLevel == 3)
            {
                IncreaseDmgToFirst(10.0f);
            }
            else if(skillLevel == 4)
            {
                IncreaseRange(new int[] {3});
                CanAtkFly(true);
                CanAtkGround(true);
                CanAtkArmoured(true);
                CanAtkCamou(true);
                IncreaseAtk(1.5f);
            }
        }
        else if(skillNumber == 2)
        {
            if(skillLevel == 1)
            {
                SetNumberOfHits(2);
            }
            else if(skillLevel == 2)
            {
                SetNumberOfHits(3);
            }
            else if(skillLevel == 3)
            {
                SetNumberOfHits(1);
                IncreaseSpd(5.0f);
            }
            else if(skillLevel == 4)
            {
                SetTarget(newTarget : 1);
                if(skillUpgraded[1] == 1)
                {
                    IncreaseRange(new int[] {1});
                }
                else if(skillUpgraded[1] >= 2)
                {
                    IncreaseRange(new int[] {2});
                }
            }
        }
    }
}
