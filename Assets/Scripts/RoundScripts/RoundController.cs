using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundController : MonoBehaviour
{

    private EnemiesList enemieslist;
    public List<GameObject> allMonster;

    public bool isRoundGoing;
    public bool isSimulating;
    public bool isInterMission;
    public bool isWaitingForUser;
    public bool isFinishedReleaseQueue;

    public int round = 1;

    public float timeForNextRound = 0f; 

    private List<List<int>> monsterReleaseThisRound = new List<List<int>>();
    private float candidateFitness = 0.0f;
    private List<List<int>> candidateMonsterReleaseThisRound = new List<List<int>>();
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
        isFinishedReleaseQueue = true;
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
        GameObject edaObj = Instantiate(edaObject, new Vector3(0, 0, 0), transform.rotation);
        edaObj.AddComponent<EstimationOfDistributionAlgorithm>();
        EstimationOfDistributionAlgorithm eda = edaObj.GetComponent<EstimationOfDistributionAlgorithm>(); 
        eda.init( /* use eda algorithm here */
            (int)((round) * Mathf.Ceil(round / 10.0F) + ((round - 1) % 10)), 
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
        (candidateMonsterReleaseThisRound, candidateFitness) = eda.getBestGene();
        SimulateGA();
        // isInterMission = true;
        // isRoundGoing = true; /* for test */
        eda.DumpData();
    }

    private void SimulateGA()
    {
        GameObject gaObj = Instantiate(edaObject, new Vector3(0, 0, 0), transform.rotation);
        gaObj.AddComponent<GeneticAlgorithm>();
        GeneticAlgorithm ga = gaObj.GetComponent<GeneticAlgorithm>(); 
        ga.init( /* use ga algorithm here */
            (int)((round) * Mathf.Ceil(round / 10.0F) + ((round - 1) % 10)), 
            monsterAvailable, 
            MapGenerator.pathsTiles.Count,
            round,
            this,
            edaObject
        ); 
        StartCoroutine(WaitForGAResult(ga));
    }

    IEnumerator WaitForGAResult(GeneticAlgorithm ga)
    {
        yield return new WaitUntil(() => ga.getIsFinishedSimulate());
        (List<List<int>> gaBestGene, float gaBestFitness) = ga.getBestGene();
        if(gaBestFitness > candidateFitness)
        {
            Debug.Log("GA win");
            monsterReleaseThisRound = gaBestGene;
        }
        else if(gaBestFitness < candidateFitness)
        {
            Debug.Log("EDA win");
            monsterReleaseThisRound = candidateMonsterReleaseThisRound;
        }
        else
        {
            Debug.Log("Draw");
            int rand = Random.Range(0, 2);
            monsterReleaseThisRound = (rand == 0) ? gaBestGene : candidateMonsterReleaseThisRound;
        }
        isInterMission = true;
        // isRoundGoing = true; /* for test */
        ga.DumpData();
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
        isFinishedReleaseQueue = true;
    }

    private void JustRandom()
    {
        GameObject obj = Instantiate(edaObject, new Vector3(0, 0, 0), transform.rotation);
        obj.AddComponent<JustRandom>();
        JustRandom jr = obj.GetComponent<JustRandom>(); 
        jr.init( /* use ga algorithm here */
            (int)((round) * Mathf.Ceil(round / 10.0F) + ((round - 1) % 10)), 
            monsterAvailable, 
            round
        ); 
        isRoundGoing = true;
    }

    private void Update()
    {
        if(isSimulating)
        {
            // JustRandom(); /* for test random */
            isSimulating = false;
            SimulateSpawnEnemies(); /* EDA -> GA */
        }
        else if(isInterMission)
        {
            isInterMission = false;
            isFinishedReleaseQueue = false;
            // isFinishedReleaseQueue = true; /* for test */
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
            
            if(!isStillHaveEnemy && isFinishedReleaseQueue)
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
                // timeForNextRound = Time.time; /* for test */
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
