using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sorcerer : Tower
{
    private float pointBlankDmg = 0.0f;
    // Start is called before the first frame update
    private void Start()
    {
        baseAtk = 1000; 
        baseSpd = 1.0f;
        range = new int[] {3, 2, 2};
        canAtkGround = true;
        type = "magic";
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
