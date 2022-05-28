using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm : MonoBehaviour
{
    [SerializeField] private int populationSize = 50;
    [SerializeField] private int maxGeneration = 100;
    [SerializeField] private float mutationRate = 0.15f;

    private int maxCost;
    private List<GameObject> availableMonster;

    private int round = 0;

    Problem prob = new Problem();

    private GameObject populationObject;

    private List<GAPopulation> pops = new List<GAPopulation>(); /* current top fitness pops */
    private List<GAPopulation> trialPops = new List<GAPopulation>(); /* trial pops where considering to take a place of pops */
    private List<GameObject> allPops = new List<GameObject>(); /* all pop object used for dump data */
    
    public void init(int maxCost, List<GameObject> availableMonster, int round, GameObject populationObject)
    {
        this.maxCost = maxCost;
        this.availableMonster = availableMonster;
        this.round = round;
        this.populationObject = populationObject;

        for(int i = 1; i <= populationSize; i++) /* first generation */
        {
            GameObject popObj = Instantiate(populationObject, new Vector3(0, 0, 0), transform.rotation);
            popObj.AddComponent<GAPopulation>();
            GAPopulation gap = popObj.GetComponent<GAPopulation>(); /* first generation */
            gap.initialize(maxCost, availableMonster, prob);
            trialPops.Add(gap);
            allPops.Add(popObj);
        }
        selection(1);
        trialPops = new List<GAPopulation>(); /* clear trial pops */
        for(int gen = 2; gen <= maxGeneration; gen++) /* run loop each generations */
        {
            crossover();
            selection(gen);
            trialPops = new List<GAPopulation>(); /* clear trial pops */
        }
    }    

    public void InsertionSort(GAPopulation p)
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
        // List<GAPopulation> newPops = new List<GAPopulation>();
        // List<GAPopulation> currentPops = new List<GAPopulation>(pops);
        // for(int i = 0; i < trialPops.Count; i++)
        // {
        //     currentPops.Add(trialPops[i]);
        // }
        // while(newPops.Count <= populationSize && currentPops.Count > 0) /* tournament selection */
        // {
        //     float sumFitness = 0.0f;
        //     for(int i = 0; i < currentPops.Count; i++)
        //     {
        //         sumFitness += currentPops[i].getFitness();
        //     }
        //     float rand = Random.Range(0.0f, sumFitness);
        //     for(int i = currentPops.Count - 1; i >= 0; i--)
        //     {
        //         rand -= currentPops[i].getFitness();
        //         if(rand <= 0)
        //         {
        //             newPops.Add(currentPops[i]);
        //             currentPops.RemoveAt(i);
        //             break;
        //         }
        //     }
        // }
        for(int i = 0; i < trialPops.Count; i++)
        {
            InsertionSort(trialPops[i]);
        }

        Debug.Log("Round: " + round + " Generation: " + generation + " Fitness: " + pops[0].getFitness());
        trialPops.Clear();
    }

    public void crossover()
    {
        for(int i = 0; i < pops.Count; i++)
        {
            int rand = 0;
            while(true)
            {
                rand = Random.Range(0, pops.Count);
                if(rand != i)
                {
                    break;
                }
            }
            
            List<int> parent1 = pops[i].getGene();
            List<int> parent2 = pops[rand].getGene();
            
            int cut1 = (int)(parent1.Count / 2); /* cross by cut half */
            int cut2 = (int)(parent2.Count / 2);

            List<int> child1 = new List<int>();
            List<int> child2 = new List<int>();

            for(int c = 0; c < cut1; c++)
            {
                child1.Add(parent1[c]);
            }
            for(int c = 0; c < cut2; c++)
            {
                child2.Add(parent2[c]);
            }
            for(int c = cut1; c < parent1.Count; c++)
            {
                child2.Add(parent1[c]);
            }
            for(int c = cut2; c < parent2.Count; c++)
            {
                child1.Add(parent2[c]);
            }

            GameObject popObj = Instantiate(populationObject, new Vector3(0, 0, 0), transform.rotation);
            popObj.AddComponent<GAPopulation>();
            GAPopulation gap = popObj.GetComponent<GAPopulation>(); /* first generation */
            gap.initializeCrossed(child1, mutationRate, maxCost, availableMonster, prob);
            trialPops.Add(gap);
            allPops.Add(popObj);

            GameObject popObj2 = Instantiate(populationObject, new Vector3(0, 0, 0), transform.rotation);
            popObj2.AddComponent<GAPopulation>();
            GAPopulation gap2 = popObj2.GetComponent<GAPopulation>(); /* first generation */
            gap2.initializeCrossed(child2,  mutationRate, maxCost, availableMonster, prob);
            trialPops.Add(gap2);
            allPops.Add(popObj2);
        }
    }
}

public class GAPopulation : MonoBehaviour
{
    private List<int> gene = new List<int>();
    private List<GameObject> availableMonster = new List<GameObject>();
    private float fitness = 0.0f;
    private Problem prob;

    public void initialize(int maxCost, List<GameObject> availableMonster, Problem prob)
    {
        int costLeft = maxCost;
        this.availableMonster = availableMonster;
        this.prob = prob;

        for(int iter = 0; costLeft >= 0; iter++)
        {
            List<int> affordableMonster = findAffordableMonster(availableMonster, costLeft);

            if(affordableMonster.Count == 0) /* if not have any affordable then break loop */
            {
                break;
            }

            int rand = Random.Range(0, affordableMonster.Count);
            if(rand < 0) /* temporary stop release monster on this path */
            {
                gene.Add(-1);
            }
            else /* add deploy order on gene [path number] */
            {
                gene.Add(affordableMonster[rand]);
                costLeft -= availableMonster[affordableMonster[rand]].GetComponent<Enemy>().getCost();
            }
        }
        evaluate();
    }

    public void initializeCrossed(List<int> initialGene, float mutationRate, int maxCost, List<GameObject> availableMonster, Problem prob)
    {
        int costLeft = maxCost;
        this.availableMonster = availableMonster;
        this.prob = prob;

        List<int> mutatedGene = mutation(initialGene, mutationRate, availableMonster);
        for(int i = 0; i < mutatedGene.Count; i++)
        {

            if(availableMonster[mutatedGene[i]].GetComponent<Enemy>().getCost() >= costLeft)
            {
                mutatedGene.RemoveRange(i, mutatedGene.Count - i);
                break;
            }
            costLeft -= availableMonster[mutatedGene[i]].GetComponent<Enemy>().getCost();

        }
        while(findAffordableMonster(availableMonster, costLeft).Count > 0)
        {
            List<int> affordableMonster = findAffordableMonster(availableMonster, costLeft);
            int randMon = Random.Range(0, affordableMonster.Count);
            mutatedGene.Add(randMon);
            costLeft -= availableMonster[randMon].GetComponent<Enemy>().getCost();
        }

        this.gene = mutatedGene;
        evaluate();
    }

    private void evaluate()
    {
        fitness = prob.NoTowerSinglePath(gene);
    }

    private List<int> mutation(List<int> initialGene, float mutationRate, List<GameObject> availableMonster)
    {
        List<int> mutatedGene = new List<int>(initialGene);
        for(int i = 0; i < mutatedGene.Count; i++)
        {
            float rand = Random.Range(0.0f, 1.0f);
            if(rand < mutationRate)
            {
                int randMon = Random.Range(0, availableMonster.Count);
                mutatedGene[i] = randMon;
            }
        }
        return mutatedGene;
    }

    public float getFitness()
    {
        return fitness;
    }

    public List<int> getGene()
    {
        return gene;
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
}

public class Problem
{
    public float NoTowerSinglePath(List<int> gene)
    {
        return gene.Count;
    }
}
