using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swordsman : Tower
{

    // Start is called before the first frame update
    private void Start()
    {
        baseAtk = 1200; 
        baseSpd = 1.0f;
        range = new int[] {1, 1, 1};
    }

    protected override void UpdateStatBySkill(int skillNumber, int skillLevel) //skill coding will override this function
    {
        // Debug.Log("upgraded lv" + skillLevel);
        if(skillNumber == 0)
        {
            if(skillLevel == 1)
            {
                IncreaseRange(new int[] {3});
                IncreaseDmgToAll(-0.2f);
            }
            else if(skillLevel == 2)
            {
                CanAtkFly(true);
                IncreaseDmgToAll(0.2f);
            }
            else if(skillLevel == 3)
            {
                SetNumberOfHits(2);
            }
            else if(skillLevel == 4)
            {
                IncreaseSpd(1.0f);
                SetNumberOfHits(5);
                IncreaseDmgToAll(-0.2f);
                CanAtkArmoured(true);
                CanAtkCamou(true);
            }
        }

        else if(skillNumber == 1)
        {
            if(skillLevel == 1)
            {
                IncreaseAtk(1.0f);
            }
            else if(skillLevel == 2)
            {
                IncreaseAtk(1.0f);
                CanAtkArmoured(true);
            }
            else if(skillLevel == 3)
            {
                IncreaseSpd(0.5f);
            }
            else if(skillLevel == 4)
            {
                IncreaseSpd(-0.5f);
                SetInstantDeath(true);
                SetPriority("strongest");
            }
        }

        else if(skillNumber == 2)
        {
            if(skillLevel == 1)
            {
                SetTarget(newTarget : 2);
            }
            else if(skillLevel == 2)
            {
                IncreaseRange(new int[] {0, 0, 0, 1, 1});
            }
            else if(skillLevel == 3)
            {
                IncreaseRange(new int[] {2, 1, 1});
            }
            else if(skillLevel == 4)
            {
                SetTarget(newTarget : -1);
                CanAtkCamou(true);
                IncreaseAtk(0.5f);
                IncreaseSpd(0.5f);
            }
        }
    } 
}
