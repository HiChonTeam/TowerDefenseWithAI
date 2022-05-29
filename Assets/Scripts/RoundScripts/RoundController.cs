using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundController : MonoBehaviour
{
    // public GameObject basicEnemy;
    // public GameObject eliteEnemy;

    private EnemiesList enemieslist;
    public List<GameObject> allMonster;

    public bool isRoundGoing;
    public bool isSimulating;
    public bool isInterMission;
    public bool isWaitingForUser;

    public int round = 1;

    public float timeForNextRound = 5.0f;
    // public float timeForNextRound = 0f;

    public static List<List<int>> monsterReleaseThisRound = new List<List<int>>();
    public static List<GameObject> monsterAvailable = new List<GameObject>();

    // public static List<List<GameObject>> enemiesOnMap = new List<List<GameObject>>();
    //public static List<List<int>> MonsterOnThisRound = new List<List<int>>();
    public GameObject edaObject;

    private void Start()
    {
        
        enemieslist = GetComponent<EnemiesList>();
        isRoundGoing = false;
        isSimulating = false;
        isInterMission = false;
        isWaitingForUser = true;
        monsterAvailable = new List<GameObject>(){allMonster[0]};
        enemieslist.CheckSlotIsFull(monsterAvailable);

        InvokeRepeating("SecondPassed", 1f, 1f);  //1s delay, repeat every 1s
    }

    private void SecondPassed()
    {
        if(isRoundGoing)
        {
            foreach(GameObject tower in TowerOnMap.towersOnMap)
            {   
                tower.GetComponent<Tower>().SecondPassed();
            }
        }
    }

    private void SimulateSpawnEnemies()
    {
        // GameObject edaObj = new GameObject("edaObj");
        GameObject edaObj = Instantiate(edaObject, new Vector3(0, 0, 0), transform.rotation);
        edaObj.AddComponent<EstimationOfDistributionAlgorithm>();
        EstimationOfDistributionAlgorithm eda = edaObj.GetComponent<EstimationOfDistributionAlgorithm>(); 
        eda.init( /* use eda algorithm here */
            (int)(round * (1 + round)), 
            monsterAvailable, 
            MapGenerator.pathsTiles.Count,
            round,
            this,
            edaObject
        ); 
        StartCoroutine(WaitForEDAResult(eda));
    }

    IEnumerator WaitForEDAResult(EstimationOfDistributionAlgorithm eda)
    {
        yield return new WaitUntil(() => eda.getIsFinishedSimulate());
        monsterReleaseThisRound = eda.getBestGene();
        //Debug.Log("BestGene =" , monsterReleaseThisRound[1]);
        //MonsterOnThisRound = eda.getBestGene();
        isInterMission = true;
        eda.DumpData();
    }

    private void SpawnEnemies()
    {
        StartCoroutine("ISpawnEnemies");
    }

    IEnumerator ISpawnEnemies()
    {
        foreach(GameObject tower in TowerOnMap.towersOnMap)
        {
            tower.GetComponent<Tower>().EndSimulate();
        }
        for(int c = 0; c < monsterReleaseThisRound[0].Count; c++)
        {
            for(int p = 0; p < monsterReleaseThisRound.Count; p++)
            {
                try
                {
                    if(monsterReleaseThisRound[p][c] != -1)
                    {
                        GameObject newEnemy = Instantiate(monsterAvailable[monsterReleaseThisRound[p][c]], MapGenerator.pathsTiles[p][0].transform.position, Quaternion.identity);
                        newEnemy.GetComponent<Enemy>().setPath(p);
                        EnemiesOnMap.enemiesOnMap[p].Add(newEnemy);
                    }
                }
                catch
                {
                    /* do nothing since this chromosome unavailable */
                }
                yield return new WaitForSeconds(0.5f);
            }
        }

        isRoundGoing = true;
    }

    

    private void Update()
    {
        if(isSimulating)
        {
            isSimulating = false;
            SimulateSpawnEnemies();
        }
        else if(isInterMission)
        {
            isInterMission = false;
            SpawnEnemies();
        }
        else if(isRoundGoing)
        {
            bool isStillHaveEnemy = false;
            foreach(List<GameObject> pathOfEnemies in EnemiesOnMap.enemiesOnMap)
            {
                if(pathOfEnemies.Count > 0) /* check if still have enemy on map */
                {
                    isStillHaveEnemy = true;
                }
            }
            if(!isStillHaveEnemy)
            {
                enemieslist.CheckSlotIsFull(monsterAvailable);
                foreach(GameObject tower in TowerOnMap.towersOnMap)
                {
                    tower.GetComponent<Tower>().NoticeEndOfWave(); /* trigger tower end of wave event */
                }
                StatusController.userMoney += 100 + (round * 10);
                isWaitingForUser = true;
                isRoundGoing = false;
                timeForNextRound = Time.time + 5.0f;
                round++;
                Debug.Log("Round is" + round);
                
                if(round % 2 == 0 && monsterAvailable.Count < allMonster.Count)
                {
                    monsterAvailable.Add(allMonster[monsterAvailable.Count]);
                    enemieslist.CheckSlotIsFull(monsterAvailable);
                    //enemieslist.CheckSlotIsFull(monsterAvailable);
                }
            }
        }
        else if(isWaitingForUser && round <= 30)
        {
            if(Time.time >= timeForNextRound)
            {
                isWaitingForUser = false;
                isSimulating = true;
            }
        }
    }
}
