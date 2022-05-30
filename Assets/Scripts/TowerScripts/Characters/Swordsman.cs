using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swordsman : Tower
{

    private Animator animator;

    // Start is called before the first frame update
    private void Start()
    {
        SkillCost = new int[,] {{100,500,1000,5000},{1000,1500,1000,4500},{500,100,200,4500}};
        SkillName = new string[,] {
            {
            "Throwing knife",
            "Throwing master",
            "Shuriken thrower",
            "Ninja",
            }
            ,
            {
            "Heavy slash",
            "Overwhelming power",
            "Swordsmanship",
            "Divine Punishment",
            }
            ,
            {
            "Double hit",
            "Full swing",
            "Extra long sword",
            "True Silver blade",
            }
        };
        SkillDetail = new string[,] {
            {
            "Swordsman now able to throw a further range knife. Throwing knife deal only 80% of attack power.",
            "Swordsman now throws knife at all ranges instead of swing sword. Throwing knife deal 100% of attack power and able to attack aerial enemies.",
            "Swordsman now throws twice Shurikens instead of knives.",
            "Swordsman warps to attack enemies with his Katana instead of throwing Shurikens with astonished speed. Increase Swordsman’s attack speed by 100%. Each time he swings his Katana will deal another following damage 5 times, each deal 80% of basic attack. Also, Swordsman now can attack all types of enemies."
            }
            ,
            {
            "Swordsman’s attack power +100%.",
            "Swordsman’s attack power +100% and able to damage armoured enemies.",
            "Swordsman’s attack speed + 50%.",
            "Every time Swordsman attack will cast Divine Punishment but his attack speed will reduce by 50%. Divine Punishment will deliver instant death to target enemy. Also, Swordsman will priority attack most HP enemy in range first."
            }
            ,
            {
            "Swordsman’s attacks now can hit 2 targets at once.",
            "Increase each side Swordsman’s attack width by 1.",
            "Increase Swordsman’s attack range by 4 grid forward in cone shape. ",
            "Swordsman now can attack all enemies in range at once and his attack can target camouflage enemy. Also, Swordsman’s attack power +50% and attack speed + 50%."
            }
        };
        baseAtk = 1200; 
        baseSpd = 1.0f;
        range = new int[] {1, 1, 1};

        animator = GetComponent<Animator>();
    }

    protected override void setAnimationAttack(){

        // if (!animator.GetCurrentAnimatorStateInfo(0).IsName("isAttackAnimation"))
        // {
            Debug.Log("+++++++++++++++++++++++++++++ isAttack Animation work");
            animator.SetBool("isAttackAnimation", true);
        // }
    }

    protected override void setAnimationIdle(){
  

        // if (animator.GetCurrentAnimatorStateInfo(0).IsName("isAttackAnimation"))
        // {
            // Debug.Log("------ ------- ------- ------- Idle Animation work" );
            animator.SetBool("isAttackAnimation", false);
        // }
        
    } 


    protected override void UpdateStatBySkill(int skillNumber, int skillLevel) //skill coding will override this function
    {
        // Debug.Log("upgraded lv" + skillLevel);
        if(skillNumber == 0)
        {
            if(skillLevel == 1)
            {
                Debug.Log(SkillDetail[skillNumber,skillLevel]);
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
