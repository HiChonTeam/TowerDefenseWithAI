using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Tower
{
    // Start is called before the first frame update
    private void Start()
    {
        baseAtk = 800; 
        baseSpd = 0.8f;
        range = new int[] {4, 4, 4};
        canAtkGround = true;
        canAtkFly = true;
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
                type = "all";
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