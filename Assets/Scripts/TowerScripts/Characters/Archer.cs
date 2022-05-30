using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Tower
{

    private Animator animator;

    // Start is called before the first frame update
    private void Start()
    {
        SkillCost = new int[,] {{700,1200,1800,5000},{300,1000,1000,1000},{500,1500,2000,4000}};
        SkillName = new string[,] {
            {
            "Longer bow",
            "Even longer bow",
            "Concentration",
            "White Death",
            }
            ,
            {
            "Hunter’s eye",
            "Two types arrow",
            "Flare magic",
            "Frost barrage",
            }
            ,
            {
            "Magic arrow",
            "Time fuse arrow",
            "Rapid fire",
            "Aerial solver",
            }
        };
        SkillDetail = new string[,] {
            {
            "Slightly increase Archer’s attack range and increase Archer’s attack power by 100%.",
            "Significantly increase Archer’s attack range and increase Archer’s attack power by 100%.",
            "Farther target is more damage arrow does. Increase damage dealt to an enemy by X%.X equal to the distance between Archer and enemy * 50.",
            "Archer priority farthest target in range first. Each time Archer attack enemy, increase attack power by X% until the end of the wave. X equal to to the distance between Archer and enemy * 2.Also, each enemy eliminated by Archer will give additional 50$."
            }
            ,
            {
            "Archer’s now can deal damage to camouflage enemy.",
            "When Archer attacks an aerial target, deal damage to all aerial enemies in range.When Archer attacks a ground target, that attack can deal damage to armoured enemy.",
            "Remove all enemies camouflage in range.",
            "Call a dragon for barrage frost stones on the entire map, deal damage to all enemies in entire map 25% of their remaining HP every second and increase damage all enemies receive by 10%. This damage effect can dealt to all enemy types."
            }
            ,
            {
            "Archer’s attack deal additional 100% of attack power as magic damage.",
            "Increase Archer’s attack power by 500%, but can’t target ground enemies.",
            "Increase Archer’s attack speed by 100% ",
            "Archer’s attack now can dealt damage to all types of enemy except ground enemy. Also, increase Archer’s attack power and attack speed by 50%."
            }
        };
        baseAtk = 800; 
        baseSpd = 0.8f;
        range = new int[] {4, 4, 4};
        canAtkGround = true;
        canAtkFly = true;

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
        int distance = (int)(Vector2.Distance(gameObject.transform.position, targetEnemy.transform.position));
        if(skillUpgraded[0] == 4)
        {
            IncreaseAtkUntilEndOfWave(0.02f * distance);
        }
        if(skillUpgraded[1] >= 2)
        {
            if(targetEnemy.GetComponent<Enemy>().getIsFly() == true)
            {
                CanAtkGround(false);
                target = -1;
            }
            else
            {
                CanAtkArmoured(true);
            }
        }
    }

    protected override void AfterAttack(GameObject targetEnemy)
    {
        if(skillUpgraded[0] == 4)
        {
            if(targetEnemy == null)
            {
                StatusController.userMoney += 50;
            }
        }
        if(skillUpgraded[1] >= 2)
        {
            target = 1;
            if(skillUpgraded[2] < 2)
            {
                CanAtkGround(true);
            }
        }
    }

    protected override void ActionToFirstMetEnemy(GameObject targetEnemy)
    {
        if(skillUpgraded[1] >= 3)
        {
            targetEnemy.GetComponent<Enemy>().setIsCamou(false);
        }
    }

    public override void SecondPassed()
    {
        if(skillUpgraded[1] == 4)
        {
            foreach(List<GameObject> enemyPath in EnemiesOnMap.enemiesOnMap)
            {
                foreach(GameObject enemy in enemyPath)
                {
                    Enemy enemyComp = enemy.GetComponent<Enemy>();
                    enemyComp.takeConstantDamage(0.25f);
                    enemyComp.increasePhysicDamageReceive(0.1f);
                    enemyComp.increaseMagicDamageReceive(0.1f);
                }
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
                IncreaseRange(new int[] {1});
                IncreaseAtk(1.0f);
            }
            else if(skillLevel == 2)
            {
                IncreaseRange(new int[] {1,1,1});
                IncreaseAtk(1.0f);
            }
            else if(skillLevel == 3)
            {
                IncreaseAtkModifierByFartherRange(0.5f);
            }
            else if(skillLevel == 4)
            {
                SetPriority("last");
            }
        }

        else if(skillNumber == 1)
        {
            if(skillLevel == 1)
            {
                CanAtkCamou(true);
            }
            else if(skillLevel == 2)
            {
                //attacking
            }
            else if(skillLevel == 3)
            {
                //debuff
            }
            else if(skillLevel == 4)
            {
                //debuff & constant dmg
            }
        }

        else if(skillNumber == 2)
        {
            if(skillLevel == 1)
            {
                type = "both";
            }
            else if(skillLevel == 2)
            {
                canAtkGround = false;
                IncreaseAtk(5.0f);
            }
            else if(skillLevel == 3)
            {
                IncreaseSpd(1.0f);
            }
            else if(skillLevel == 4)
            {
                CanAtkArmoured(true);
                CanAtkCamou(true);
                IncreaseAtk(0.5f);
                IncreaseSpd(0.5f);
            }
        }
    } 
}