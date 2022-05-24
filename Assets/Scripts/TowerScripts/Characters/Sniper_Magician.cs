using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper_Magician : Tower
{
    float destroyerBuff = 0.0f;
    // Start is called before the first frame update
    private void Start()
    {
        baseAtk = 2500; 
        baseSpd = 0.5f;
        range = new int[] {5};
        SetTarget(newTarget : 2);
        canAtkGround = true;
        canAtkArmoured = true;
        type = "magic";
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
