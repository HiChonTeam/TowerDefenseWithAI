using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Enemy : MonoBehaviour
{
    [SerializeField] private string enemyName;
    [SerializeField] private int enemyHealth;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float reducePhysicDamage;
    [SerializeField] private float reduceMagicDamage;
    [SerializeField] private int barrier;
    [SerializeField] private string barrierResistance;
    [SerializeField] private int cost; //cost to deploy this enemy

    [SerializeField] private bool isFly = false;
    [SerializeField] private bool isArmoured = false;
    [SerializeField] private bool isCamou = false;

    [SerializeField] private ParticleSystem deathParticle;
    [SerializeField] private ParticleSystem burnParticle;
    [SerializeField] private TextMeshPro popupDamage;
    [SerializeField] private TextMeshPro popupGameOver;
    [SerializeField] private TextMeshPro CountDown;

    private GameObject targetTile;

    private int path;

    private List<GameObject> passed = new List<GameObject>();

    private ParticleSystem bps;

    private bool simulating = false;
    private float fitness = 0f;
    private Population population;

    public void setPath(int pathNumber)
    {
        path = pathNumber;
        InitializeEnemy();
    }

    public int getHP()
    {
        return enemyHealth;
    }

    public bool getIsFly()
    {
        return isFly;
    }

    public bool getIsArmoured()
    {
        return isArmoured;
    }

    public bool getIsCamou()
    {
        return isCamou;
    }

    public int getCost()
    {
        return cost;
    }

    public void setIsArmoured(bool armoured = false)
    {
        isArmoured = armoured;
    }

    public void setIsCamou(bool camou = false)
    {
        isCamou = camou;
    }

    public void increasePhysicDamageReceive(float amount = 0.0f)
    {
        reducePhysicDamage -= amount;
    }

    public void increaseMagicDamageReceive(float amount = 0.0f)
    {
        reduceMagicDamage -= amount;
    }

    public bool isFirstTimeInRange(GameObject tower)
    {
        for(int i=0; i < passed.Count; i++)
        {
            if(GameObject.ReferenceEquals(tower, passed[i]))
            {
                return false;
            }
        }
        passed.Add(tower);
        return true;
    }

    public void Simulating(int acceleration, Population population)
    {
        gameObject.transform.localScale = new Vector3(0, 0, 0);
        simulating = true;
        movementSpeed *= acceleration;
        this.population = population;
    }

    private void InitializeEnemy()
    {
        targetTile = MapGenerator.pathsTiles[path][0];
    }

    public string getName()
    {
        return enemyName;
    }

    public void takeDamage(int amount, string type = "physic", bool isInstantDeath = false)
    {
        DamagePopup damagePopup = simulating ? null : Instantiate(popupDamage, gameObject.transform.position, Quaternion.identity).GetComponent<DamagePopup>();
        bool isBreakBarrier = false;
        int breakAmount = 0;
        
        if(isInstantDeath && barrier == 0)
        {
            if(!simulating) damagePopup.Setup(0, "instantDeath");
            die();
            return;
        }
        if(barrier > 0 && barrierResistance != type) //if have barrier with resistance and atk type not match
        {
            breakAmount = barrier; //store original barrier value
            barrier -= amount; //current barrier after damage
            if(barrier >= 0)
            {
                if(!simulating) damagePopup.Setup(amount, "barrier");
                amount = 0;
            }
            else
            {
                isBreakBarrier = true;
                amount = -barrier;
            }
        }
        else if(barrier > 0)
        {
            if(!simulating) damagePopup.Setup(0, "resist");
        }
        if(barrier <= 0)
        {
            if(type == "physic")
            {
                int dmg = (int)(amount * (1 - reducePhysicDamage));
                enemyHealth -= dmg;
                if(!simulating)
                {
                    if(isBreakBarrier)
                    {
                        damagePopup.Setup(dmg + breakAmount, "break");
                    }
                    else{
                        damagePopup.Setup(dmg, "physic");
                    }
                }
            }
            else if(type == "magic")
            {
                int dmg = (int)(amount * (1 - reduceMagicDamage));
                enemyHealth -= dmg;
                if(!simulating)
                {
                    if(isBreakBarrier)
                    {
                        damagePopup.Setup(dmg + breakAmount, "break");
                    }
                    else{
                        damagePopup.Setup(dmg, "magic");
                    }
                }
                
            }        
        }
        
        if(enemyHealth <= 0)
        {
            die();
        }
    }

    public void takeTrueDamage(int amount = 0)
    {
        DamagePopup damagePopup = simulating ? null : Instantiate(popupDamage, gameObject.transform.position, Quaternion.identity).GetComponent<DamagePopup>();
        if(!simulating) damagePopup.Setup(amount, "true");
        enemyHealth -= amount;
        if(enemyHealth <= 0)
        {
            die();
        }
    }

    public void takeConstantDamage(float amount = 0.0f)
    {
        DamagePopup damagePopup = simulating ? null : Instantiate(popupDamage, gameObject.transform.position, Quaternion.identity).GetComponent<DamagePopup>();
        if(!simulating) damagePopup.Setup((int)(amount * enemyHealth), "constant");
        int overpenetrate = 0;
        if(barrier > 0)
        {
            barrier -= (int)(amount * enemyHealth);
            if(barrier <= 0)
            {
                overpenetrate -= barrier;
            }
        }
        else
        {
            enemyHealth -= (int)(amount * enemyHealth);
        }
        if(overpenetrate > 0)
        {
            enemyHealth -= overpenetrate;
        }
        if(enemyHealth <= 0)
        {
            die();
        }
    }

    public void takeConstantDamageEverySeconds(int amount = 0, string damageType = "none")
    {
        StartCoroutine(ConstantDamagePerSecond(amount, damageType));
        if(damageType == "burn" && !simulating)
        {
            //burn effect
            bps = Instantiate(burnParticle, transform.position, Quaternion.identity);
            bps.transform.SetParent(gameObject.transform);
        }
    }

    IEnumerator ConstantDamagePerSecond(int amount = 0, string damageType = "none")
    {
        int backup_amount = amount;
        while(true)
        {
            amount = backup_amount;
            DamagePopup damagePopup = simulating ? null : Instantiate(popupDamage, gameObject.transform.position, Quaternion.identity).GetComponent<DamagePopup>();
            if(!simulating) damagePopup.Setup(amount, damageType);
            if(barrier > 0)
            {
                barrier -= amount;
                if(barrier >= 0)
                {
                    amount = 0;
                }
                else
                {
                    amount = -barrier;
                }
            }
            if(barrier <= 0)
            {
                enemyHealth -= amount;
                if(enemyHealth <= 0)
                {
                    die();
                    break;
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }

    private void die()
    {   
        EnemiesOnMap.enemiesOnMap[path].Remove(gameObject);
        if(bps != null)
        {
            Destroy(bps);
        }
        if(!simulating)
        {
            Instantiate(deathParticle, transform.position, Quaternion.identity);
        }
        else
        {
            population.setFitness((float)(fitness) / (float)(MapGenerator.pathsTiles[path].Count));
        }
        Destroy(transform.gameObject);
    }

    private void goal()
    {
        EnemiesOnMap.enemiesOnMap[path].Remove(gameObject);
        
        if(simulating)
        {
            population.setFitness(1f);
        }
        else
        {
            StatusController.userLife -= 1;
            if(StatusController.userLife <= 0)
            {
                StatusController.isGameOver = true;
                Instantiate(popupGameOver, new Vector3(-2, 0, 0), Quaternion.identity);
                Instantiate(CountDown, new Vector3(-2 ,-2, 0), Quaternion.identity);

            }
        }
        Destroy(transform.gameObject);
    }

    public bool getSimulating()
    {
        return simulating;
    }

    private void moveEnemy() 
    {
        transform.position = MoveTowards(transform.position, targetTile.transform.position, movementSpeed * Time.deltaTime);
    }

    private Vector3 MoveTowards(Vector3 current, Vector3 target, float maxDistance) 
    {
        Vector3 difference = (target - current);
        float distanceToMove = Mathf.Min(difference.magnitude, maxDistance);
        return current + difference.normalized * distanceToMove;
    }

    private void checkPosition()
    {
        float distance = (transform.position - targetTile.transform.position).magnitude;

        if(targetTile != null && targetTile != MapGenerator.pathsTiles[path][MapGenerator.pathsTiles[path].Count - 1]) 
        {
            if(distance < 0.001f) {
                int currentIndex = MapGenerator.pathsTiles[path].IndexOf(targetTile);
                targetTile = MapGenerator.pathsTiles[path][currentIndex + 1];
                fitness += 1.0f;
            }
        }
        else
        {
            if(distance < 0.001f) {
                goal();
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if(!StatusController.isGameOver)
        {
            checkPosition();
            moveEnemy();
        }
    }
}
