using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EstimationOfDistributionAlgorithm : MonoBehaviour
{
    [SerializeField] private int populationSize = 5;
    [SerializeField] private int maxGeneration = 5;
    [SerializeField] private int acceleration = 50;

    private int maxCost;
    private int numberOfPaths;
    [SerializeField] private List<GameObject> availableMonster;

    private int round = 0;
    
    private int longestGene = 0;

    private bool selectioned = false;
    private bool isFinishedSimulate = false;

    private List<Population> pops = new List<Population>();
    private List<Population> trialPops = new List<Population>();
    private List<GameObject> allPops = new List<GameObject>();

    public GameObject populationObject;

    public void init(int maxCost, List<GameObject> availableMonster, int numberOfPaths, int round, RoundController roundController, GameObject populationObject)
    {
        this.maxCost = maxCost;
        this.numberOfPaths = numberOfPaths;
        this.availableMonster = availableMonster;
        this.round = round;

        this.populationObject = populationObject;

        StartCoroutine(LifeCycle());
    }

    IEnumerator LifeCycle()
    {
        for(int i = 1; i <= populationSize; i++) /* create initial populations */
        {
            GameObject popObj = Instantiate(populationObject, new Vector3(0, 0, 0), transform.rotation);
            popObj.AddComponent<Population>();
            Population p = popObj.GetComponent<Population>(); /* first generation */
            int c = p.initialize(maxCost, availableMonster, numberOfPaths, acceleration);
            longestGene = c > longestGene ? c : longestGene;
            trialPops.Add(p);
            allPops.Add(popObj);
            yield return new WaitUntil(() => p.isFinishedEvaluate() == true);
            // List<List<int>> g = p.getGene();
            // string sg = "";
            // foreach(List<int> path in g)
            // {
            //     foreach(int ch in path)
            //     {
            //         sg += ch + " ";
            //     }
            //     sg += " | ";
            // }
            // Debug.Log("gen : 1" + ", pop : " + i + ", fitness : " + p.getFitness() + "\ngene : " + sg);
        }
        selection(1);
        yield return new WaitUntil(() => selectioned); 
        trialPops = new List<Population>(); /* clear trial pops */
        for(int gen = 2; gen <= maxGeneration; gen++) /* run loop each generations */
        {
            for(int i = 1; i <= populationSize; i++)
            {
                sampling(longestGene * numberOfPaths);
                yield return new WaitUntil(() => trialPops[i - 1].isFinishedEvaluate() == true);
                // List<List<int>> g = trialPops[i - 1].getGene();
                // string sg = "";
                // foreach(List<int> path in g)
                // {
                //     foreach(int ch in path)
                //     {
                //         sg += ch + " ";
                //     }
                //     sg += " | ";
                // }
                // Debug.Log("gen : " + gen + ", pop : " + i + ", fitness : " + trialPops[i - 1].getFitness() + "\ngene : " + sg);
            }
            selection(gen);
            yield return new WaitUntil(() => selectioned);
            trialPops = new List<Population>(); /* clear trial pops */
        }
        isFinishedSimulate = true;
    }

    public bool getIsFinishedSimulate()
    {
        return isFinishedSimulate;
    }

    public List<List<int>> getBestGene()
    {
        return pops[0].getGene(); /* return best fitness population's gene */
    }

    public void sampling(int ChromosomeNum)
    {
        int[,] probabilisticSamplingArray = new int[populationSize, ChromosomeNum]; /* create table for store genes */

        for(int p = 0; p < populationSize; p++)
        {
            for(int c = 0; c < ChromosomeNum; c++)
            {
                probabilisticSamplingArray[p, c] = -1; /* fill array with -1 */
            }
        }

        for(int i = 0; i < pops.Count; i++) /* O(gen * pop * chro * path) */
        {
            List<List<int>> gene = pops[i].getGene();

            for(int p = 0; p < gene.Count; p++)
            {
                for(int c = 0; c < gene[p].Count; c++)
                {
                    probabilisticSamplingArray[i, (c + ((p) * (ChromosomeNum / (p + 1))))] = gene[p][c]; /* write each cromosome into table */
                }
            }
        }

        Dictionary<int, float>[] prob = new Dictionary<int, float>[ChromosomeNum]; /* probability of monster in timestamp (chromosome) */

        for(int c = 0; c < ChromosomeNum; c++)
        {
            prob[c] = new Dictionary<int, float>();
            for(int i = 0; i < populationSize; i++)
            {
                try 
                {
                    prob[c][probabilisticSamplingArray[i, c]] += (1.0f / populationSize); /* add probability of that monster in timestamp (chromosome) */
                }
                catch
                {
                    prob[c].Add(probabilisticSamplingArray[i, c], 1.0f / populationSize); /* in case of monster in this timestamp never released */
                }
            }
        }        

        List<List<int>> trialGene = new List<List<int>>();

        for(int i = 0; i < numberOfPaths; i++)
        {
            trialGene.Add(new List<int>());
        }

        for(int c = 0; c < ChromosomeNum; c++)
        {
            float rands = Random.Range(0.0f, 1.0f); /* random each trial gene's chromosome */
            float rangeP = 0.0f;

            int path = (int)(c / longestGene);

            for(int m = -1; m < availableMonster.Count; m++)
            {
                try
                {
                    rangeP += prob[c][m]; /* if this is first prob then add rangeP as prob */
                    if(rangeP >= rands)
                    {
                        trialGene[path].Add(m); /* if rand in prob range then select this chromosome as trial gene */
                        break;
                    }
                }
                catch
                {
                    /* no prob on this monster then ignore */
                }
            }
        }

        /* add trial populations */
        GameObject popObj = Instantiate(populationObject, new Vector3(0, 0, 0), transform.rotation);
        popObj.AddComponent<Population>();
        Population Tp = popObj.GetComponent<Population>();
        int ch = Tp.initializeTrial(maxCost, availableMonster, numberOfPaths, trialGene, acceleration);
        longestGene = ch > longestGene ? ch : longestGene;
        trialPops.Add(Tp);
    }

    public void InsertionSort(Population p)
    {
        bool isInserted = false;
        for(int i = 0; i < pops.Count; i++)
        {
            if(p.getFitness() > pops[i].getFitness()) 
            {
                pops.Insert(i, p); /* insert population at index i */
                isInserted = true;
                break;
            }
        }
        if(!isInserted)
        {
            pops.Add(p);
        }
        if(pops.Count > populationSize)
        {
            pops.RemoveRange(populationSize - 1, pops.Count - populationSize); /* remove overflow population */
        }
    }

    public void selection(int generation)
    {
        selectioned = false;
        StartCoroutine(WaitForEvaluate(generation));
    }

    IEnumerator WaitForEvaluate(int generation)
    {
        for(int i = 0; i < trialPops.Count; i++)
        {
            yield return new WaitUntil(() => trialPops[i].isFinishedEvaluate() == true);
            InsertionSort(trialPops[i]);
        }
        
        Debug.Log("Round: " + round + " Generation: " + generation + " Fitness: " + pops[0].getFitness());
        // if(generation == maxGeneration)
        // {
        //     List<List<int>> g = pops[0].getGene();
        //     string sg = "| ";
        //     foreach(List<int> path in g)
        //     {
        //         foreach(int ch in path)
        //         {
        //             sg += ch + " ";
        //         }
        //         sg += " | ";
        //     }
        //     Debug.Log("best gene : " + sg);
        // }
        
        trialPops.Clear();
        selectioned = true;
    }

    public void DumpData()
    {
        for(int i = allPops.Count - 1; i >= 0; i--)
        {
            GameObject p = allPops[i];
            allPops.Remove(p);
            GameObject.Destroy(p);
        }
        Destroy(gameObject);
    }

    // private void Start()
    // {
    //     float func_time = Time.realtimeSinceStartup;
    //     for(int i = 10000; i <= 80000; i *= 2)
    //     {
    //         init(i, availableMonster, 5);
    //         func_time = Time.realtimeSinceStartup - func_time;
    //         Debug.Log("EDA Run time on " + i.ToString() + " maximum deploy cost is " + func_time.ToString("F2") + "s");
    //     }
        
    // }
}

public class Population : MonoBehaviour
{
    private List<List<int>> gene = new List<List<int>>();
    private int activeGeneNumber = 0; /* number of gene that is not -1 (release monster) */
    private int numberOfFitnessCount = 0;
    private List<GameObject> availableMonster = new List<GameObject>();
    private float fitness = 0.0f;
    private int acceleration;

    public int initialize(int maxCost, List<GameObject> availableMonster, int numberOfPaths, int acceleration)
    {
        int costLeft = maxCost;
        this.acceleration = acceleration;
        this.availableMonster = availableMonster;

        for(int i = 0; i < numberOfPaths; i++)
        {
            gene.Add(new List<int>());
        }
        
        for(int iter = 0; costLeft >= 0; iter++)
        {
            List<int> affordableMonster = findAffordableMonster(availableMonster, costLeft);

            if(affordableMonster.Count == 0) /* if not have any affordable then break loop */
            {
                break;
            }

            int rand = Random.Range((int)(Mathf.Ceil(-1 * (affordableMonster.Count / 3))), affordableMonster.Count);
            if(rand < 0) /* temporary stop release monster on this path */
            {
                gene[iter % numberOfPaths].Add(-1);
            }
            else /* add deploy order on gene [path number] */
            {
                gene[iter % numberOfPaths].Add(affordableMonster[rand]);
                costLeft -= availableMonster[affordableMonster[rand]].GetComponent<Enemy>().getCost();
                activeGeneNumber++;
            }
        }

        int geneLong = 0;

        for(int i = 0; i < numberOfPaths; i++)
        {
            geneLong = geneLong < gene[i].Count ? gene[i].Count : geneLong;
        }

        evaluate();

        return geneLong; /* return number of slots in gene */
    }

    public int initializeTrial(int maxCost, List<GameObject> availableMonster, int numberOfPaths, List<List<int>> trialGene, int acceleration)
    {
        this.acceleration = acceleration;
        this.availableMonster = availableMonster;

        int costLeft = maxCost;
        List<int> pathLong = new List<int>();
        int longestPath = 0;

        for(int path = 0; path < trialGene.Count; path++)
        {
            pathLong.Add(trialGene[path].Count); /* count chromosome of each path */
            longestPath = trialGene[path].Count > longestPath ? trialGene[path].Count : longestPath;
        }

        for(int c = 0; c < longestPath; c++)
        {
            for(int path = 0; path < trialGene.Count; path++)
            {
                try /* make gene is feasible solution (can afford all monster in chromosome, if cant then replace those overflow chromosome with -1) */
                {
                    if(trialGene[path][c] != -1)
                    {
                        int monsterCost = availableMonster[trialGene[path][c]].GetComponent<Enemy>().getCost();
                        if(costLeft >= monsterCost)
                        {
                            costLeft -= monsterCost;
                            activeGeneNumber++;
                        }
                        else
                        {
                            trialGene[path][c] = -1;
                        }
                    }
                }
                catch
                {
                    /* do nothing since this chromosome unavailable */
                }
            }
        }

        if(findAffordableMonster(availableMonster, costLeft).Count > 0)
        {
            bool onGoing = true;
            for(int c = 0; c < longestPath; c++)
            {
                for(int pathNum = 0; pathNum < trialGene.Count; pathNum++)
                {
                    try /* when gene is feasible solution and still have afforable then rand add them in empty chromosome */
                    {
                        List<int> affordableMonsters = findAffordableMonster(availableMonster, costLeft);
                        if(affordableMonsters.Count == 0)
                        {
                            onGoing = false;
                            break;
                        }

                        if(trialGene[pathNum][c] == -1)
                        {
                            int rand = Random.Range((int)(Mathf.Ceil(-1 * (affordableMonsters.Count / 3))), affordableMonsters.Count);

                            if(rand >= 0)
                            {
                                trialGene[pathNum][c] = affordableMonsters[rand];
                                costLeft -= availableMonster[affordableMonsters[rand]].GetComponent<Enemy>().getCost();
                                activeGeneNumber++;
                            }
                            else
                            {
                                trialGene[pathNum][c] = -1;
                            }
                        }
                    }
                    catch
                    {
                        /* do nothing since this chromosome unavailable */
                    }
                }

                if(onGoing == false)
                {
                    break;
                }
            }

            List<int> affordableMonster = findAffordableMonster(availableMonster, costLeft);
            while(affordableMonster.Count != 0)
            {
                /* if gene already full but still have affordable monster then */
                for(int path = 0; path < trialGene.Count; path++)
                {
                    if(affordableMonster.Count == 0)
                    {
                        trialGene[path].Add(-1);
                        continue;
                    }

                    int rand = Random.Range((int)(Mathf.Ceil(-1 * (affordableMonster.Count / 3))), affordableMonster.Count);

                    if(rand >= 0)
                    {
                        trialGene[path].Add(affordableMonster[rand]);
                        costLeft -= availableMonster[affordableMonster[rand]].GetComponent<Enemy>().getCost();
                        activeGeneNumber++;
                        affordableMonster = findAffordableMonster(availableMonster, costLeft);
                    }
                    else
                    {
                        trialGene[path].Add(-1);
                    }
                }
            }
        }

        gene = trialGene;

        int geneLong = 0;

        for(int i = 0; i < numberOfPaths; i++)
        {
            geneLong = geneLong < gene[i].Count ? gene[i].Count : geneLong;
        }

        evaluate();

        return geneLong;
    }

    public List<int> findAffordableMonster(List<GameObject> availableMonster, int costLeft)
    {
        List<int> affordableMonster = new List<int>();
        for(int i = 0; i < availableMonster.Count; i++) /* check which enemies are affordable */
        {
            if(costLeft >= availableMonster[i].GetComponent<Enemy>().getCost())
            {
                affordableMonster.Add(i);
            }
        }
        return affordableMonster;
    }

    public void evaluate()
    {
        StartCoroutine("ISimulateSpawnEnemies");
    }

    IEnumerator ISimulateSpawnEnemies()
    {
        foreach(GameObject tower in TowerOnMap.towersOnMap)
        {   
            tower.GetComponent<Tower>().StartSimulate(acceleration);
        }

        for(int c = 0; c < gene[0].Count; c++)
        {
            for(int path = 0; path < gene.Count; path++)
            {
                try
                {
                    List<int> pathList = gene[path];
                    if(pathList[c] >= 0)
                    {
                        GameObject newEnemy = Instantiate(availableMonster[pathList[c]], MapGenerator.pathsTiles[path][0].transform.position, Quaternion.identity);
                        newEnemy.GetComponent<Enemy>().Simulating(acceleration: acceleration, population: this);
                        newEnemy.GetComponent<Enemy>().setPath(path);
                        EnemiesOnMap.enemiesOnMap[path].Add(newEnemy);
                    }
                }
                catch
                {
                    /* do nothing since index out of range */
                }
                yield return new WaitForSeconds(0.5f / acceleration);
            }
        }
    }

    public void setFitness(float fitness)
    {
        numberOfFitnessCount++;
        // if(fitness >= 1.0f)
        // {
        //     this.fitness = (float)(Mathf.Floor(this.fitness + fitness));  /* if enemy can reach goal then add 1 to fitness */
        // }
        // else if(this.fitness < fitness)
        // {
        //     this.fitness = fitness; /* if enemy can't reach goal and can go farthest from record */
        // }
        if(fitness >= 1.0f)
        {
            this.fitness += fitness;  /* if enemy can reach goal then add 1 to fitness */
        }
        else if((this.fitness % 1) < fitness)
        {
            this.fitness = (float)(Mathf.Floor(this.fitness)) + fitness; /* if enemy can't reach goal and can go farthest from record */
        }
    }

    public float getFitness()
    {
        return fitness;
    }

    public bool isFinishedEvaluate()
    {
        return activeGeneNumber == numberOfFitnessCount;
    }

    public List<List<int>> getGene()
    {
        return gene;
    }
}
