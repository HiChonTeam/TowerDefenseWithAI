using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] public GameObject rangeGrid;
    protected List<GameObject> atkGrid = new List<GameObject>();

    protected List<GameObject> currentRange = new List<GameObject>();
    protected List<GameObject> enemyInRange = new List<GameObject>();
    protected float timeToNextAttack = 0.0f;
    protected int[] skillUpgraded = {0, 0, 0}; 
    protected string towerRotate;

    protected int baseAtk = 0;  //base attack power
    protected float baseSpd = 1.0f; //base attack speed

    protected int[] range = {1};
    /** line of array compare to real range when turn tower to left
        ex [3, 2, 2, 1, 1]
        .....
            5
          3 3
        1 1 1 tower
          2 2
            4
        .....
    */
    protected int[] exrange = {0};
    /** line of array compare to real range when turn tower to left
        ex [0, 1, 1, 1, 1]

        ... 5
        ... 3
        ... tower
        ... 2
        ... 4

    */

    protected float atkModifier = 1.0f; //change of attack power by skill, should += when increased but *= when decreased
    protected float spdModifier = 1.0f; //change of attack speed by skill, should += when increased but *= when decreased

    protected float dmgToFly = 1.0f; //damage to fly enemies, should += when increased but *= when decreased 
    protected float dmgToGround = 1.0f; //damage to ground enemies, should += when increased but *= when decreased 
    protected float dmgToArmoured = 1.0f; //damage to armoured enemies, should += when increased but *= when decreased 
    protected float dmgToNonArmoured = 1.0f; //damage to non armoured enemies, should += when increased but *= when decreased 
    protected float dmgToCamou = 1.0f; //damage to camouflaged enemies, should += when increased but *= when decreased 
    protected float dmgToFirst = 1.0f; //damage to nearest enemies, should += when increased but *= when decreased 
    protected float dmgToAll = 1.0f; //damage to all enemies, should += when increased but *= when decreased 

    protected float atkModifierByFartherRange = 0.0f; //change of attack power when enemy is farther range

    protected float atkModifierUntilEndWave = 1.0f; //change of attack power by skill that stack until end of wave
    protected float spdModifierUntilEndWave = 1.0f; //change of attack speed by skill that stack until end of wave

    protected int spdModifyBySimulate = 1; //change of attack speed when simulating

    protected float numberOfHits = 1; //number of hit each attack for one target (ex. double = 2 etc.)

    protected bool canAtkFly = false; //can this tower attack fly enemies or not
    protected bool canAtkGround = true; //can this tower attack ground enemies or not
    protected bool canAtkArmoured = false; //can this tower attack armoured enemies or not
    protected bool canAtkCamou = false; //can this tower attack camouflaged enemies or not

    protected int target = 1; //most target this tower can attack at once, -1 if can attack AoE
    protected bool isInstantDeath = false; //can this tower bring enemy without barrier to death with only one punch?
    protected string priority = "first"; 
    /** 
        priority this tower attack enemy
        "first" = nearest enemy
        "strongest" = most HP enemy
        "last" = farthest enemy
    **/

    protected string type = "physic"; //type of attack "physic" or "magic" or "all"

    protected float atkBuff = 1.0f; //attack power increased by buff
    protected float spdBuff = 1.0f; //attack speed increased by buff
    protected float physicDmgBuff = 1.0f; //physic damage increased by buff
    protected float magicDmgBuff = 1.0f; //magic damage increased by buff
    protected int rangeBuff = 0; //range increased by buff
    protected bool canAtkAllBuff = false; //can attack all types of enemies by buff
    protected float discountBuff = 0.0f; //upgrade discounted by buff
    protected int income = 0; //money generate each time wave ended


    public void Attack()
    {
        int targets = target == -1 ? enemyInRange.Count : target; //if target = -1 meaning attack AoE
        bool isFirstTarget = true; 
        // foreach(GameObject enemy in enemyInRange)
        for(int i=0; i < enemyInRange.Count; i++)
        {
            float modifier = atkModifier * dmgToAll;
            if(targets != 0)
            {
                Enemy enemyScript = enemyInRange[i].GetComponent<Enemy>();
                int distance = (int)(Vector2.Distance(gameObject.transform.position, enemyInRange[i].transform.position));

                if( //check if can attack or not
                    !(enemyScript.getIsFly() && (!canAtkFly && !canAtkAllBuff)) &&
                    !(!enemyScript.getIsFly() && (!canAtkGround && !canAtkAllBuff)) &&
                    !(enemyScript.getIsArmoured() && (!canAtkArmoured && !canAtkAllBuff)) &&
                    !(enemyScript.getIsCamou() && (!canAtkCamou && !canAtkAllBuff))
                )
                {
                    BeforeAttack(enemyInRange[i]);

                    modifier *= enemyScript.getIsFly() ? dmgToFly : dmgToGround;
                    modifier *= enemyScript.getIsArmoured() ? dmgToArmoured : dmgToNonArmoured;
                    modifier *= enemyScript.getIsCamou() ? dmgToCamou : 1.0f;
                    modifier *= isFirstTarget ? dmgToFirst : 1.0f;
                    modifier *= atkModifierUntilEndWave;
                    modifier += (atkModifierByFartherRange * Mathf.Abs(distance));
                    for(int noh = 0; noh < numberOfHits; noh++)
                    {
                        if(type == "both") //deal two type damage in the same time
                        {
                            enemyScript.takeDamage(amount : (int)(baseAtk * modifier * magicDmgBuff), type : "physic", isInstantDeath : isInstantDeath);
                            enemyScript.takeDamage(amount : (int)(baseAtk * modifier * physicDmgBuff), type : "magic", isInstantDeath : isInstantDeath);
                        }
                        else
                        {
                            enemyScript.takeDamage(amount : (int)(baseAtk * modifier * (type == "physic" ? physicDmgBuff : magicDmgBuff)), type : type, isInstantDeath : isInstantDeath);
                        }
                    }
                    isFirstTarget = false;
                    targets--;

                    AfterAttack(enemyInRange[i]);
                }
            }
        }
    }

    protected virtual void BeforeAttack(GameObject targetEnemy)
    {
        //activate before attack target enemy
    }

    protected virtual void AfterAttack(GameObject targetEnemy)
    {
        //activate after attack target enemy
    }

    protected virtual void ActionToFirstMetEnemy(GameObject targetEnemy)
    {
        //activate after enemy come into this tower range first time
    }

    public virtual void SecondPassed()
    {
        //activate every second
    }

    public void StartSimulate(int acceleration)
    {
        timeToNextAttack = Time.time;
        spdModifyBySimulate = acceleration;
    }

    public void EndSimulate()
    {
        timeToNextAttack = Time.time;
        spdModifyBySimulate = 1;
    }

    public void ReceiveBuff()
    {
        ResetBuff();
        foreach(GameObject tower in TowerOnMap.towersOnMap)
        {
            if(tower.GetComponent<Witch>() != null)
            {
                Witch witchComp = tower.GetComponent<Witch>();
                if(GameObject.ReferenceEquals(gameObject, tower))
                {
                    witchComp.BuffAlly(gameObject); //buff self (for witch)
                    continue;
                }
                for(int i=0; i < witchComp.currentRange.Count; i++)
                {
                    if(witchComp.currentRange[i].transform.position.Equals(gameObject.transform.position))
                    {
                        //can receive buff
                        witchComp.BuffAlly(gameObject);
                        break;
                    }
                }
            }
        }
    }

    protected void ResetBuff()
    {
        atkBuff = 1.0f;
        spdBuff = 1.0f;
        physicDmgBuff = 1.0f;
        magicDmgBuff = 1.0f;
        rangeBuff = 0;
        canAtkAllBuff = false;
        discountBuff = 0.0f;
    }

    public void AtkBuff(float value = 0.0f)
    {
        if(value > 0)
        {
            atkBuff += value;
        }
        else if(value < 0)
        {
            atkBuff *= (1.0f + value);
        }
    }

    public void SpdBuff(float value = 0.0f)
    {
        if(value > 0)
        {
            spdBuff += value;
        }
        else if(value < 0)
        {
            spdBuff *= (1.0f + value);
        }
    }

    public void PhysicDmgBuff(float value = 0.0f)
    {
        if(value > 0)
        {
            physicDmgBuff += value;
        }
        else if(value < 0)
        {
            physicDmgBuff *= (1.0f + value);
        }
    }

    public void MagicDmgBuff(float value = 0.0f)
    {
        if(value > 0)
        {
            magicDmgBuff += value;
        }
        else if(value < 0)
        {
            magicDmgBuff *= (1.0f + value);
        }
    }

    public void RangeBuff(int value = 0)
    {
        rangeBuff += value;
    }

    public void CanAtkAllBuff(bool value = false)
    {
        canAtkAllBuff = value;
    }

    public void DiscountBuff(float value = 0.0f)
    {
        discountBuff += value;
    }

    public List<Vector2> UpdateRange() //update current range
    // protected void UpdateRange()
    {
        currentRange = new List<GameObject>();
        Vector2 pos = gameObject.transform.position;
        List<Vector2> atkRange = new List<Vector2>();

        //vector for multiply of tower rotate (normal range)
        Vector2 initialVector = new Vector2();
        if(towerRotate == "up")
        {
            initialVector = new Vector2(0, 1);
        }
        else if(towerRotate == "down")
        {
            initialVector = new Vector2(0, -1);
        }
        else if(towerRotate == "left")
        {
            initialVector = new Vector2(-1, 0);
        }
        else if(towerRotate == "right")
        {
            initialVector = new Vector2(1, 0);
        }

        //vector for multiply of tower rotate (ex range)
        Vector2 initialEXVector = new Vector2();
        if(towerRotate == "up")
        {
            initialEXVector = new Vector2(1, 0);
        }
        else if(towerRotate == "down")
        {
            initialEXVector = new Vector2(-1, 0);
        }
        else if(towerRotate == "left")
        {
            initialEXVector = new Vector2(0, 1);
        }
        else if(towerRotate == "right")
        {
            initialEXVector = new Vector2(0, -1);
        }

        for(int i = 0; i < range.Length; i++) //decode array of range to real range
        {
            atkRange.AddRange(RangeLine(pos, initialVector, i)); //concat lists of Vector2
        }
        for(int i = 0; i < exrange.Length; i++)
        {
            if(exrange[i] == 1)
            {
                atkRange.Add(AddExRange(pos, initialEXVector, i));
            }
        }
        foreach(Vector2 rangeTile in atkRange) //check each path is in attack range or not
        {
            foreach(GameObject mapTile in MapGenerator.mapTiles){ 
                Vector2 maptTileV2 = new Vector2(mapTile.transform.position.x, mapTile.transform.position.y);
                if(maptTileV2.Equals(rangeTile))
                {
                    currentRange.Add(mapTile);
                }
            }  
            
        }
        return atkRange;
    }

    protected List<Vector2> RangeLine(Vector2 towerPosition, Vector2 initialVector, int line) 
    {   //add each position of tiles in range line
        List<Vector2> rangeLine = new List<Vector2>();
        for(int i = 1; i <= range[line] + rangeBuff; i++)
        {
            int side;
            if(line == 0)
            {
                side = 0;
            }
            else if(line % 2 == 0)
            {
                side = 1;
            }
            else 
            {
                side = -1;
            }
            rangeLine.Add(towerPosition + (initialVector * i) + (new Vector2(Mathf.Abs(initialVector.y), Mathf.Abs(initialVector.x)) * side * (int)((line + 1) / 2)));
        }
        return rangeLine;
    }

    protected Vector2 AddExRange(Vector2 towerPosition, Vector2 initialVector, int line)
    {
        int side;
        if(line == 0)
        {
            side = 0;
        }
        if(line % 2 == 0)
        {
            side = 1;
        }
        else
        {
            side = -1;
        }
        return towerPosition + (side * (int)((line + 1) / 2) * initialVector);
    }

    protected List<GameObject> CheckEnemyInRange()
    {
        List<GameObject> enemiesInRange = new List<GameObject>();
        List<float> enemiesDistance = new List<float>();
        foreach(List<GameObject> pathOfEnemies in EnemiesOnMap.enemiesOnMap)
        {
            foreach(GameObject enemy in pathOfEnemies)
            {
                //check if enemy is in range or not
                for(int i=0; i < currentRange.Count; i++)
                {
                    Collider2D enemyCollider = enemy.GetComponent<Collider2D>();
                    Collider2D gridCollider = currentRange[i].GetComponent<Collider2D>();
                    if(gridCollider.bounds.Intersects(enemyCollider.bounds))
                    {
                        if(enemy.GetComponent<Enemy>().isFirstTimeInRange(gameObject))
                        {
                            ActionToFirstMetEnemy(enemy);
                        }
                        enemiesInRange.Add(enemy);
                        float distance = Vector2.Distance(gameObject.transform.position, enemy.transform.position);
                        enemiesDistance.Add(distance);
                        break;
                    }
                }
            }
        }

        return InsertionSort(enemiesInRange, enemiesDistance);
    }

    protected List<GameObject> InsertionSort(List<GameObject> enemiesInRange, List<float> enemiesDistance)
    {
        if(priority == "first")
        {
            for(int i = 1; i < enemiesInRange.Count; i++)
            {
                if(enemiesDistance[i-1] <= enemiesDistance[i])
                {
                    continue;
                }
                GameObject enemySorting = enemiesInRange[i];
                float distanceSorting = enemiesDistance[i];
                for(int j = i - 1; j >= 0; j--)
                {
                    if(enemiesDistance[j] > distanceSorting) //if j distance still more than i
                    {
                        enemiesDistance[j+1] = enemiesDistance[j];
                        enemiesInRange[j+1] = enemiesInRange[j];
                        if(j == 0)
                        {
                            enemiesDistance[j] = distanceSorting;
                            enemiesInRange[j] = enemySorting;
                            break;
                        }
                    }
                    else
                    {
                        enemiesDistance[j+1] = distanceSorting;
                        enemiesInRange[j+1] = enemySorting;
                        break;
                    }
                }
            } 
        }
        else if(priority == "last")
        {
            for(int i = 1; i < enemiesInRange.Count; i++)
            {
                if(enemiesDistance[i-1] >= enemiesDistance[i])
                {
                    continue;
                }
                GameObject enemySorting = enemiesInRange[i];
                float distanceSorting = enemiesDistance[i];
                for(int j = i - 1; j >= 0; j--)
                {
                    if(enemiesDistance[j] < distanceSorting) //if j distance still less than i
                    {
                        enemiesDistance[j+1] = enemiesDistance[j];
                        enemiesInRange[j+1] = enemiesInRange[j];
                        if(j == 0)
                        {
                            enemiesDistance[j] = distanceSorting;
                            enemiesInRange[j] = enemySorting;
                            break;
                        }
                    }
                    else
                    {
                        enemiesDistance[j+1] = distanceSorting;
                        enemiesInRange[j+1] = enemySorting;
                        break;
                    }
                }
            }
        }
        else if(priority == "strongest")
        {
            List<int> enemiesHP = new List<int>();
            for(int i = 0; i < enemiesInRange.Count; i++)
            {
                enemiesHP.Add(enemiesInRange[i].GetComponent<Enemy>().getHP());
            }
            for(int i = 1; i < enemiesInRange.Count - 1; i++)
            {
                if(enemiesHP[i-1] >= enemiesHP[i])
                {
                    continue;
                }
                GameObject enemySorting = enemiesInRange[i];
                int hpSorting = enemiesHP[i];
                for(int j = i - 1; j >= 0; j--)
                {
                    if(enemiesHP[i] < hpSorting) //if j hp still less than i hp
                    {
                        enemiesHP[j+1] = enemiesHP[j];
                        enemiesInRange[j+1] = enemiesInRange[j];
                        if(j == 0)
                        {
                            enemiesHP[j] = hpSorting;
                            enemiesInRange[j] = enemySorting;
                            break;
                        }
                    }
                    else
                    {
                        enemiesHP[j+1] = hpSorting;
                        enemiesInRange[j+1] = enemySorting;
                        break;
                    }
                }
            }
        }
        return enemiesInRange;
    }

    public void IncreaseRange(int[] value)
    {
        int rangeCount = range.Length;
        int valueCount = value.Length;
        int count = rangeCount >= valueCount ? rangeCount : valueCount; 
        int[] newRange = new int[count];
        for(int i = 0; i < rangeCount; i++)
        {
            newRange[i] = range[i];
        }
        for(int i = 0; i < valueCount; i++)
        {
            newRange[i] += value[i];
        }
        range = newRange;
        UpdateRange();
    }

    public void IncreaseEXRange(int[] value)
    {
        int exrangeCount = exrange.Length;
        int valueCount = value.Length;
        int count = exrangeCount >= valueCount ? exrangeCount : valueCount; 
        int[] newEXRange = new int[count];
        for(int i = 0; i < exrangeCount; i++)
        {
            newEXRange[i] = exrange[i];
        }
        for(int i = 0; i < valueCount; i++)
        {
            newEXRange[i] += value[i];
        }
        exrange = newEXRange;
        UpdateRange();
    }

    public void IncreaseBaseAtk(int value = 0)
    {
        baseAtk += value;
    }

    public void IncreaseBaseSpd(int value = 0)
    {
        baseSpd += value;
    }

    public void ChangeBaseAtk(int value = 0)
    {
        baseAtk = value;
    }

    public void ChangeBaseSpd(int value = 0)
    {
        baseSpd = value;
    }

    public void IncreaseAtk(float value = 0.0f)
    {
        if(value > 0)
        {
            atkModifier += value;
        }
        else if(value < 0)
        {
            atkModifier *= (1.0f + value);
        }
    }

    public void IncreaseSpd(float value = 0.0f)
    {
        if(value > 0)
        {
            spdModifier += value;
        }
        else if(value < 0)
        {
            spdModifier *= (1.0f + value);
        }
    }

    public void IncreaseAtkModifierByFartherRange(float value = 0.0f)
    {
        atkModifierByFartherRange += value;
    }

    public void IncreaseAtkUntilEndOfWave(float value = 0.0f)
    {
        atkModifierUntilEndWave += value;
    }

    public void IncreaseSpdUntilEndOfWave(float value = 0.0f)
    {
        spdModifierUntilEndWave += value;
    }

    public void NoticeEndOfWave()
    {
        atkModifierUntilEndWave = 1.0f;
        spdModifierUntilEndWave = 1.0f;
        StatusController.userMoney += income;
    }

    public void IncreaseDmgToFly(float value = 0.0f)
    {
        if(value > 0)
        {
            dmgToFly += value;
        }
        else if(value < 0)
        {
            dmgToFly *= (1.0f + value);
        }
    }

    public void IncreaseDmgToGround(float value = 0.0f)
    {
        if(value > 0)
        {
            dmgToGround += value;
        }
        else if(value < 0)
        {
            dmgToGround *= (1.0f + value);
        }
    }

    public void IncreaseDmgToArmoured(float value = 0.0f)
    {
        if(value > 0)
        {
            dmgToArmoured += value;
        }
        else if(value < 0)
        {
            dmgToArmoured *= (1.0f + value);
        }
    }

    public void IncreaseDmgToNonArmoured(float value = 0.0f)
    {
        if(value > 0)
        {
            dmgToNonArmoured += value;
        }
        else if(value < 0)
        {
            dmgToNonArmoured *= (1.0f + value);
        }
    }

    public void IncreaseDmgToCamou(float value = 0.0f)
    {
        if(value > 0)
        {
            dmgToCamou += value;
        }
        else if(value < 0)
        {
            dmgToCamou *= (1.0f + value);
        }
    }

    public void IncreaseDmgToFirst(float value = 0.0f)
    {
        if(value > 0)
        {
            dmgToFirst += value;
        }
        else if(value < 0)
        {
            dmgToFirst *= (1.0f + value);
        }
    }

    public void IncreaseDmgToAll(float value = 0.0f)
    {
        if(value > 0)
        {
            dmgToAll += value;
        }
        else if(value < 0)
        {
            dmgToAll *= (1.0f + value);
        }
    }

    public void CanAtkFly(bool change = true)
    {
        canAtkFly = change;
    }

    public void CanAtkGround(bool change = true)
    {
        canAtkGround = change;
    }

    public void CanAtkArmoured(bool change = true)
    {
        canAtkArmoured = change;
    }

    public void CanAtkCamou(bool change = true)
    {
        canAtkCamou = change;
    }

    public void SetNumberOfHits(int value = 0)
    {
        numberOfHits = value;
    }

    public void SetTarget(int newTarget = 0, int add = 0)
    {
        if(add != 0)
        {
            target += add;
        }
        else
        {
            target = newTarget;
        }
    }

    public void SetInstantDeath(bool isIn = true)
    {
        isInstantDeath = isIn;
    }

    public void SetPriority(string newP = "first")
    {
        priority = newP;
    }

    public void SetTowerRotate(string rotate)
    {
        towerRotate = rotate;
    }

    public virtual void UpgradeSkill(int skillNumber)
    {
        if(skillUpgraded[skillNumber] < 4)
        {
            skillUpgraded[skillNumber] += 1;
            UpdateStatBySkill(skillNumber, skillUpgraded[skillNumber]);
        }
    }

    public int GetSkillUpgraded(int skillNumber)
    {
        return skillUpgraded[skillNumber];
    }

    protected virtual void UpdateStatBySkill(int skillNumber, int skillLevel) //skill coding will override this function
    {
        if(skillNumber == 0)
        {
            if(skillLevel == 1)
            {

            }
            else if(skillLevel == 2)
            {

            }
            else if(skillLevel == 3)
            {
                
            }
            else if(skillLevel == 4)
            {
                
            }
        }

        else if(skillNumber == 1)
        {
            if(skillLevel == 1)
            {

            }
            else if(skillLevel == 2)
            {

            }
            else if(skillLevel == 3)
            {
                
            }
            else if(skillLevel == 4)
            {
                
            }
        }

        else if(skillNumber == 2)
        {
            if(skillLevel == 1)
            {

            }
            else if(skillLevel == 2)
            {

            }
            else if(skillLevel == 3)
            {
                
            }
            else if(skillLevel == 4)
            {
                
            }
        }
    } 

    protected void OnMouseEnter()
    {
        if(!StatusController.isGameOver)
        {
            List<Vector2> atkRange = UpdateRange();
            foreach(Vector2 grid in atkRange)
            {
                GameObject newAtkGrid = Instantiate(rangeGrid, new Vector3(grid.x, grid.y, 0), Quaternion.identity);
                atkGrid.Add(newAtkGrid);
            }
        }
    }

    protected void OnMouseExit()
    {
        foreach(GameObject grid in atkGrid)
        {
            Destroy(grid);
        }
        atkGrid.Clear();
    }

    // Start is called before the first frame update
    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        if(!StatusController.isGameOver)
        {
            UpdateRange();
        }
        enemyInRange = CheckEnemyInRange(); 
        if(enemyInRange.Count > 0)
        {
            if(Time.time >= timeToNextAttack)
            {
                Attack();
                timeToNextAttack = Time.time + (1.0f / (baseSpd * spdModifier * spdBuff * spdModifierUntilEndWave * spdModifyBySimulate));
            }
        }
    }
}
