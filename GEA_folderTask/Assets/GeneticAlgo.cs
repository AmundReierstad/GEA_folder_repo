using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class GeneticAlgo : MonoBehaviour
{
    [SerializeField]private float populationMinParameterValue = -1f;
    [SerializeField] private float populationMaxParameterValue = 1f;
    [SerializeField] private float crossProbability = 0.6f;
    [SerializeField] private float mutateProbability = 0.3f;
    [SerializeField] private float mutateAmount = 2f;
    [SerializeField] private float mutationPercentage = 2f;
    private static Random _random = new Random();
    private List<Genome> _currentPopulation;
    //shorthand declaration of fields
    public uint PopulationSize
    {
        get;
        private set;
    }
    public uint GenerationCount
    {
        get;
        private set;
    }
    public bool SortPopulationBeforeDestruction
    {
        get;
        private set;
    }
    public bool Running
    {
        get;
        private set;
    }
    //events
    public event System.Action<GeneticAlgo> AlgorithmEnded;
    public event System.Action<IEnumerable<Genome>> FitnessCalculationFinished;
    //methods
    public GeneticAlgo(int genomeParameterCount, int populationSize)
    {
        PopulationSize = (uint)populationSize;
        _currentPopulation = new List<Genome>(populationSize);
        for (int i = 0; i < populationSize; i++)
        {
            _currentPopulation.Add(new Genome(new List<float>(genomeParameterCount)));
        }
    }

    public void PopulationInitialisation(IEnumerable<Genome> population)
    {
        //Set parameters to random values in set range
        foreach (Genome genome in population)
            genome.SetRandomParameters(populationMinParameterValue, populationMaxParameterValue);
    }
    public void Start()
    {
        PopulationInitialisation(_currentPopulation);
        
        Running = true;
        
    }
    
    
}
