using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GeneticAlgorithm : MonoBehaviour
{
    [SerializeField] private int populationSize = 10;
    [SerializeField] private int maxGeneration = 5;
    [SerializeField] private float mutationRate = 0.15f;
    [SerializeField] private int acceleration = 50;

    private int maxCost;
    private int numberOfPaths;
    private List<GameObject> availableMonster;

    private int round = 0;

    private int longestGene = 0;

    private bool selectioned = false;
    private bool isFinishedSimulate = false;

    private GameObject populationObject;
    private TextMeshPro loadingText;

    private List<Population> pops = new List<Population>(); /* current top fitness pops */
    private List<Population> trialPops = new List<Population>(); /* trial pops where considering to take a place of pops */
    private List<GameObject> allPops = new List<GameObject>(); /* all pop object used for dump data */
    
    public void init(int maxCost, List<GameObject> availableMonster, int numberOfPaths, int round, TextMeshPro loadingText, GameObject populationObject)
    {
        this.maxCost = maxCost;
        this.numberOfPaths = numberOfPaths;
        this.availableMonster = availableMonster;
        this.round = round;
        this.populationObject = populationObject;
        this.loadingText = loadingText;

        StartCoroutine(LifeCycle());
    }    

    IEnumerator LifeCycle()
    {
        for(int i = 1; i <= populationSize; i++) /* create initial populations */
        {
            (Population p, GameObject popObj) = generateInitialPopulation();
            int c = p.getLongestGene();
            longestGene = c > longestGene ? c : longestGene;
            trialPops.Add(p);
            allPops.Add(popObj);
            yield return new WaitUntil(() => p.isFinishedEvaluate() == true);
        }
        selection(1);
        yield return new WaitUntil(() => selectioned); 
        trialPops = new List<Population>(); /* clear trial pops */
        for(int gen = 2; gen <= maxGeneration; gen++) /* run loop each generations */
        {
            List<int> unmarriagePopNum = new List<int>(); /* address pop that not crossovered */
            for(int i = 0; i < pops.Count; i++)
            {
                unmarriagePopNum.Add(i);
            }
            int j = 0;
            while(unmarriagePopNum.Count > 1)
            {
                int rand1 = Random.Range(0, unmarriagePopNum.Count);
                int rand2 = 0;
                while(true)
                {
                    rand2 = Random.Range(0, unmarriagePopNum.Count); /* random until parent 1 and 2 isn't same */
                    if(rand2 != rand1)
                    {
                        break;
                    }
                }
                (List<List<int>> child1, List<List<int>> child2, List<int> activeGeneNumber) = crossover(unmarriagePopNum[rand1], unmarriagePopNum[rand2], longestGene * numberOfPaths);
                unmarriagePopNum.RemoveAt(rand1);
                if(rand1 < rand2)
                {
                    unmarriagePopNum.RemoveAt(rand2 - 1);
                }
                else
                {
                    unmarriagePopNum.RemoveAt(rand2);
                }
                
                List<List<List<int>>> children = new List<List<List<int>>>(){child1, child2};
                
                for(int cn = 0; cn < children.Count; cn++)
                {
                    /* add trial populations */
                    GameObject popObj = Instantiate(populationObject, new Vector3(0, 0, 0), transform.rotation);
                    popObj.AddComponent<Population>();
                    Population tp = popObj.GetComponent<Population>();
                    tp.initialize(children[cn], availableMonster, numberOfPaths, acceleration, activeGeneNumber[cn]);
                    int ch = tp.getLongestGene();
                    longestGene = ch > longestGene ? ch : longestGene;
                    trialPops.Add(tp);
                    yield return new WaitUntil(() => trialPops[j].isFinishedEvaluate() == true);
                    j++;
                }
                
            }
            selection(gen);
            yield return new WaitUntil(() => selectioned);
            trialPops = new List<Population>(); /* clear trial pops */
        }
        isFinishedSimulate = true;
    }

    private (Population, GameObject) generateInitialPopulation()
    {
        List<List<int>> gene  = new List<List<int>>();
        int costLeft = maxCost;
        int activeGeneNumber = 0;

        for(int i = 0; i < numberOfPaths; i++)
        {
            gene.Add(new List<int>());
        }
        
        for(int iter = 0; costLeft >= 0; iter++)
        {
            List<int> affordableMonster = findAffordableMonster(costLeft);

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

        GameObject popObj = Instantiate(populationObject, new Vector3(0, 0, 0), transform.rotation);
        popObj.AddComponent<Population>();
        Population p = popObj.GetComponent<Population>(); /* first generation */
        p.initialize(gene, availableMonster, numberOfPaths, acceleration, activeGeneNumber);

        return (p, popObj); /* return population */
    }

    public List<int> findAffordableMonster(int costLeft)
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

    private void InsertionSort(Population p)
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

    private void selection(int generation)
    {
        selectioned = false;
        StartCoroutine(WaitForEvaluate(generation));
    }

    IEnumerator WaitForEvaluate(int generation)
    {
        for(int i = 0; i < trialPops.Count; i++)
        {
            yield return new WaitUntil(() => trialPops[i].isFinishedEvaluate() == true);
            // InsertionSort(trialPops[i]);
        }
        float bestFitness = TournamentSelection();
        
        Debug.Log("Round: " + round + " Generation: " + generation + " Fitness: " + bestFitness);
        loadingText.SetText("Simulating... " + ((generation * 10) + 50) + "%");
        
        trialPops.Clear();
        selectioned = true;
    }

    private float TournamentSelection()
    {
        List<Population> popsInGen = pops;
        List<Population> newPops = new List<Population>();
        float sumFitness = 0.0f;
        float bestFitness = 0.0f;
        foreach(Population trialPop in trialPops)
        {
            popsInGen.Add(trialPop);
        }
        for(int i = 0; i < populationSize; i++)
        {
            sumFitness = 0.0f;
            foreach(Population pop in popsInGen)
            {
                sumFitness += pop.getFitness();
            }
            float rand = Random.Range(0.0f, sumFitness);
            for(int j = 0; j < popsInGen.Count; j++)
            {
                rand -= popsInGen[j].getFitness();
                if(rand <= 0)
                {
                    newPops.Add(popsInGen[j]);
                    bestFitness = popsInGen[j].getFitness() > bestFitness ? popsInGen[j].getFitness() : bestFitness;
                    popsInGen.RemoveAt(j);
                    break;
                }
            }
        }
        pops = newPops; /* replace with new gene from tournament */
        return bestFitness;
    }

    private (List<List<int>>, List<List<int>>, List<int>) crossover(int parentNum1, int parentNum2, int ChromosomeNum)
    {
        List<List<int>> parent1 = pops[parentNum1].getGene();
        List<List<int>> parent2 = pops[parentNum2].getGene();
            
        List<int> cut1 = new List<int>();
        List<int> cut2 = new List<int>();
        for(int i = 0; i < numberOfPaths; i++) /* cross by cut half */
        {
            cut1.Add((int)(parent1[i].Count / 2));
            cut2.Add((int)(parent2[i].Count / 2));
        }

        List<List<int>> child1 = new List<List<int>>();
        List<List<int>> child2 = new List<List<int>>();

        for(int i = 0; i < numberOfPaths; i++)
        {
            child1.Add(new List<int>());
            child2.Add(new List<int>());
            for(int c = 0; c < cut1[i]; c++)
            {
                child1[i].Add(parent1[i][c]);
            }
            for(int c = 0; c < cut2[i]; c++)
            {
                child2[i].Add(parent2[i][c]);
            }
            for(int c = cut1[i]; c < parent1[i].Count; c++)
            {
                child2[i].Add(parent1[i][c]);
            }
            for(int c = cut2[i]; c < parent2[i].Count; c++)
            {
                child1[i].Add(parent2[i][c]);
            }
        }
        child1 = mutation(child1);
        child2 = mutation(child2);
        List<List<List<int>>> children = new List<List<List<int>>>(){child1, child2};
        List<int> activeGeneNumber = new List<int>(){0, 0};

        for(int i = 0; i < children.Count; i++)
        {
            List<List<int>> child = children[i];
            int costLeft = maxCost;
            int longestPath = 0;
            for(int path = 0; path < child.Count; path++)
            {
                longestPath = child[path].Count > longestPath ? child[path].Count : longestPath; /* count chromosome of each path */
                for(int c = 0; c < child[path].Count; c++)
                {
                    if(child[path][c] != -1)
                    {
                        if(availableMonster[child[path][c]].GetComponent<Enemy>().getCost() > costLeft)
                        {
                            child[path][c] = -1;
                        }
                        else
                        {
                            costLeft -= availableMonster[child[path][c]].GetComponent<Enemy>().getCost();
                            activeGeneNumber[i]++;
                        }
                    }
                }
            }


            if(findAffordableMonster(costLeft).Count > 0) /* fill gene for maximum cost */
            {
                bool onGoing = true;
                for(int c = 0; c < longestPath; c++)
                {
                    for(int pathNum = 0; pathNum < child.Count; pathNum++)
                    {
                        try /* when gene is feasible solution and still have afforable then rand add them in empty chromosome */
                        {
                            List<int> affordableMonsters = findAffordableMonster(costLeft);
                            if(affordableMonsters.Count == 0)
                            {
                                onGoing = false;
                                break;
                            }

                            if(child[pathNum][c] == -1)
                            {
                                int rand = Random.Range((int)(Mathf.Ceil(-1 * (affordableMonsters.Count / 3))), affordableMonsters.Count);

                                if(rand >= 0)
                                {
                                    child[pathNum][c] = affordableMonsters[rand];
                                    costLeft -= availableMonster[affordableMonsters[rand]].GetComponent<Enemy>().getCost();
                                    activeGeneNumber[i]++;
                                }
                                else
                                {
                                    child[pathNum][c] = -1;
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

                List<int> affordableMonster = findAffordableMonster(costLeft);
                while(affordableMonster.Count != 0)
                {
                    /* if gene already full but still have affordable monster then */
                    for(int path = 0; path < child.Count; path++)
                    {
                        if(affordableMonster.Count == 0)
                        {
                            child[path].Add(-1);
                            continue;
                        }

                        int rand = Random.Range((int)(Mathf.Ceil(-1 * (affordableMonster.Count / 3))), affordableMonster.Count);

                        if(rand >= 0)
                        {
                            child[path].Add(affordableMonster[rand]);
                            costLeft -= availableMonster[affordableMonster[rand]].GetComponent<Enemy>().getCost();
                            activeGeneNumber[i]++;
                            affordableMonster = findAffordableMonster(costLeft);
                        }
                        else
                        {
                            child[path].Add(-1);
                        }
                    }
                }
            }
        } 

        return (child1, child2, activeGeneNumber);
    }

    private List<List<int>> mutation(List<List<int>> gene)
    {
        for(int path = 0; path < gene.Count; path++)
        {
            for(int c = 0; c < gene[path].Count; c++)
            {
                float rand = Random.Range(0.0f, 1.0f);
                if(rand < mutationRate)
                {
                    int randMon = Random.Range(0, availableMonster.Count);
                    gene[path][c] = randMon;
                }
            }
        }
        return gene;
    }

    public (List<List<int>>, float) getBestGene()
    {
        return (pops[0].getGene(), pops[0].getFitness()); /* return best fitness population's gene */
    }

    public bool getIsFinishedSimulate()
    {
        return isFinishedSimulate;
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
}