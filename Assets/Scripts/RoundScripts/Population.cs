using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population : MonoBehaviour
{
    private List<List<int>> gene = new List<List<int>>();
    private int numberOfPaths = 0; /* number of enemies paths in map*/
    private int activeGeneNumber = 0; /* number of gene that is not -1 (release monster) */
    private int numberOfFitnessCount = 0; /* number of moster in this gene that already count fitness */
    private List<GameObject> availableMonster = new List<GameObject>();
    private float fitness = 0.0f;
    private int acceleration;

    public void initialize(List<List<int>> gene, List<GameObject> availableMonster, int numberOfPaths, int acceleration, int activeGeneNumber)
    {
        this.gene = gene;
        this.acceleration = acceleration;
        this.availableMonster = availableMonster;
        this.activeGeneNumber = activeGeneNumber;
        this.numberOfPaths = numberOfPaths;

        evaluate();
    }

    public int getLongestGene()
    {
        int geneLong = 0;

        for(int i = 0; i < numberOfPaths; i++)
        {
            geneLong = geneLong < gene[i].Count ? gene[i].Count : geneLong;
        }

        return geneLong; /* return number of slots in gene */
    }

    public void evaluate()
    {
        StartCoroutine("ISimulateSpawnEnemies");
        // for(int path = 0; path < gene.Count; path++) /* for test */
        // {
        //     for(int c = 0; c < gene[path].Count; c++)
        //     {
        //         if(gene[path][c] != -1) setFitness(1.0f);
        //     }
        // }
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

