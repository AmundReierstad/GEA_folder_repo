using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class GeneticAlgo : MonoBehaviour
{
    #region Members
    [SerializeField] public const float PopulationMinParameterValue = -1f;
    [SerializeField] public const float PopulationMaxParameterValue = 1f;
    [SerializeField] public const float CrossProbability = 0.6f;
    [SerializeField] public const float MutateProbability = 0.3f;
    [SerializeField] public const float MutateAmount = 2f;
    [SerializeField] public const float MutationPercentage = 2f;
    private static Random _random = new Random();
    private List<Genome> _currentPopulation;
    //shorthand declaration of fields
    public int PopulationSize
    {
        get;
        private set;
    }
    public int GenerationCount
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
    #endregion
    
    #region Events
    public event System.Action<GeneticAlgo> AlgorithmTermination;
    public event System.Action<IEnumerable<Genome>> FitnessCalculationFinished;
    #endregion
    
    #region Operator Delegates
    /// <summary>
    /// Method template for methods used to initialise the initial population.
    /// </summary>
    /// <param name="initialPopulation">The population to be initialised.</param>
    public delegate void InitialisationOperator(IEnumerable<Genome> initialPopulation);
    /// <summary>
    /// Method template for methods used to evaluate (or start the evluation process of) the current population.
    /// </summary>
    /// <param name="currentPopulation">The current population.</param>
    public delegate void EvaluationOperator(IEnumerable<Genome> currentPopulation);
    /// <summary>
    /// Method template for methods used to calculate the Fitness value of each genotype of the current population.
    /// </summary>
    /// <param name="currentPopulation"></param>
    public delegate void FitnessCalculation(IEnumerable<Genome> currentPopulation);
    /// <summary>
    /// Method template for methods used to select genotypes of the current population and create the intermediate population.
    /// </summary>
    /// <param name="currentPopulation">The current population,</param>
    /// <returns>The intermediate population.</returns>
    public delegate List<Genome> SelectionOperator(List<Genome> currentPopulation);
    /// <summary>
    /// Method template for methods used to recombine the intermediate population to generate a new population.
    /// </summary>
    /// <param name="intermediatePopulation">The intermediate population.</param>
    /// <returns>The new population.</returns>
    public delegate List<Genome> RecombinationOperator(List<Genome> intermediatePopulation, int newPopulationSize);
    /// <summary>
    /// Method template for methods used to mutate the new population.
    /// </summary>
    /// <param name="newPopulation">The mutated new population.</param>
    public delegate void MutationOperator(List<Genome> newPopulation);
    /// <summary>
    /// Method template for method used to check whether any termination criterion has been met.
    /// </summary>
    /// <param name="currentPopulation">The current population.</param>
    /// <returns>Whether the algorithm shall be terminated.</returns>
    public delegate bool CheckTerminationCriterion(IEnumerable<Genome> currentPopulation);
    #endregion

    #region Operator Methods
    /// <summary>
    /// Method used to initialise the initial population.
    /// </summary>
    public InitialisationOperator InitialisePopulation = DefaultPopulationInitialisation;
    /// <summary>
    /// Method used to evaluate (or start the evaluation process of) the current population.
    /// </summary>
    public EvaluationOperator Evaluation = AsyncEvaluation;
    /// <summary>
    /// Method used to calculate the Fitness value of each genotype of the current population.
    /// </summary>
    public FitnessCalculation FitnessCalculationMethod = DefaultFitnessCalculation;
    /// <summary>
    /// Method used to select genotypes of the current population and create the intermediate population.
    /// </summary>
    public SelectionOperator Selection = DefaultSelectionOperator;
    /// <summary>
    /// Method used to recombine the intermediate population to generate a new population.
    /// </summary>
    public RecombinationOperator Recombination = DefaultRecombinationOperator;
    /// <summary>
    /// Method used to mutate the new population.
    /// </summary>
    public MutationOperator Mutation = DefaultMutationOperator;
    /// <summary>
    /// Method used to check whether any termination criterion has been met.
    /// </summary>
    public CheckTerminationCriterion TerminationCriterion = null;
    #endregion

    #region Constructor
    public GeneticAlgo(int genomeParameterCount, int populationSize)
    {
        PopulationSize = populationSize;
        _currentPopulation = new List<Genome>(populationSize);
        for (int i = 0; i < populationSize; i++)
        {
            _currentPopulation.Add(new Genome(new List<float>(genomeParameterCount)));
        }

        GenerationCount = 1;
        SortPopulationBeforeDestruction = true;
        Running = false;
    }
    #endregion
    
    #region Methods
    public void PopulationInitialisation(IEnumerable<Genome> population)
    {
        //Set parameters to random values in set range
        foreach (Genome genome in population)
            genome.SetRandomParameters(PopulationMinParameterValue, PopulationMaxParameterValue);
    }
    public void Start()
    {
        PopulationInitialisation(_currentPopulation);
        
        Running = true;
        
    }

    public void EvaluationFinished()
    {
        FitnessCalculationMethod(_currentPopulation);
        
        if (SortPopulationBeforeDestruction)
            _currentPopulation.Sort();
        
        if (FitnessCalculationFinished != null)
            FitnessCalculationFinished(_currentPopulation);
        
        if (TerminationCriterion != null && TerminationCriterion(_currentPopulation))
        {
            Terminate();
            return;
        }
        //apply selection
        List<Genome> intermediatePopulation = Selection(_currentPopulation);
        
        //Apply Recombination
        List<Genome> newPopulation = Recombination(intermediatePopulation, PopulationSize);
        
        //Apply Mutation
        Mutation(newPopulation);
        //Set current population to newly generated one and start evaluation again
        _currentPopulation = newPopulation;
        GenerationCount++;
        
        Evaluation(_currentPopulation);
        
    }
    private void Terminate()
    {
        Running = false;
        AlgorithmTermination?.Invoke(this);
    }
    #endregion
    
    #region Static Methods
    public static void DefaultPopulationInitialisation(IEnumerable<Genome> population)
    {
        //Set parameters to random values in set range
        foreach (Genome genome in population)
            genome.SetRandomParameters(PopulationMinParameterValue, PopulationMaxParameterValue);
    }

    public static void AsyncEvaluation(IEnumerable<Genome> currentPopulation)
    {
        //At this point the async evaluation should be started and after it is finished EvaluationFinished should be called
    }

    //Calculates the Fitness of each genotype by : Fitness = evaluation / averageEvaluation.
    public static void DefaultFitnessCalculation(IEnumerable<Genome> currentPopulation)
    {
        //First calculate average evaluation of whole population
        int populationSize = 0;
        float overallEvaluation = 0;
        var population = currentPopulation as Genome[] ?? currentPopulation.ToArray();
        foreach (Genome genotype in population)
        {
            overallEvaluation += genotype.Evaluation;
            populationSize++;
        }

        float averageEvaluation = overallEvaluation / populationSize;

        //Now assign Fitness with formula Fitness = evaluation / averageEvaluation
        foreach (Genome genotype in population)
            genotype.Fitness = genotype.Evaluation / averageEvaluation;
    }

    /// Selects the best three genotypes of the current population and copies them to the intermediate population.
    public static List<Genome> DefaultSelectionOperator(List<Genome> currentPopulation)
    {
        List<Genome> intermediatePopulation = new List<Genome>();
        intermediatePopulation.Add(currentPopulation[0]);
        intermediatePopulation.Add(currentPopulation[1]);
        intermediatePopulation.Add(currentPopulation[2]);

        return intermediatePopulation;
    }
    // Crosses the first with the second genotype of the intermediate population until the new population is of the desired size.
    public static List<Genome> DefaultRecombinationOperator(List<Genome> intermediatePopulation, int newPopulationSize)
    {
        if (intermediatePopulation.Count < 2) throw new ArgumentException("Intermediate population size must be greater than 2 for this operator.");
        List<Genome> newPopulation = new List<Genome>();
        while (newPopulation.Count < newPopulationSize)
        {
            Genome offspring1, offspring2;
            CompleteCrossover(intermediatePopulation[0], intermediatePopulation[1], CrossProbability, out offspring1, out offspring2);

            newPopulation.Add(offspring1);
            if (newPopulation.Count < newPopulationSize)
                newPopulation.Add(offspring2);
        }
        
        return newPopulation;
    }
    
    public static void DefaultMutationOperator(List<Genome> newPopulation)
    {
        foreach (Genome genotype in newPopulation)
        {
            if (_random.NextDouble() < MutationPercentage )
                MutateGenotype(genotype, MutateProbability, MutateAmount);
        }
    }
    
    public static void CompleteCrossover(Genome parent1, Genome parent2, float swapChance, out Genome offspring1, out Genome offspring2)
    {
        //Initialise new parameter vectors
        int parameterCount = parent1.ParameterCount;
        List<float> off1Parameters = new List<float>();
        List<float> off2Parameters = new List<float>();

        //Iterate over all parameters randomly swapping
        for (int i = 0; i < parameterCount; i++)
        {
            if (_random.Next() < swapChance)
            {
                //Swap parameters
                off1Parameters[i] = parent2[i];
                off2Parameters[i] = parent1[i];
            }
            else
            {
                //Don't swap parameters
                off1Parameters[i] = parent1[i];
                off2Parameters[i] = parent2[i];
            }
        }

        offspring1 = new Genome(off1Parameters);
        offspring2 = new Genome(off2Parameters);
    }
    public static void MutateGenotype(Genome genotype, float mutationProb, float mutationAmount)
    {
        for (int i = 0; i < genotype.ParameterCount; i++)
        {
            if (_random.NextDouble() < mutationProb)
            {
                //Mutate by random amount in range [-mutationAmount, mutationAmoun]
                genotype[i] += (float)(_random.NextDouble() * (mutationAmount * 2) - mutationAmount);
            }    
        } 
    }
    #endregion
}
