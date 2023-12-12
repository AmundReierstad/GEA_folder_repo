using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class GeneticManager : MonoBehaviour
{
    #region Class Members
    private static System.Random _randomizer = new System.Random();
    public static GeneticManager Instance
    {
        get;
        private set;
    }
    //Serialized fields are set in the unity editor
    [SerializeField]  private bool saveStatistics = false;   // Whether or not the results of each generation shall be written to file
    private string _statisticsFileName;
    [SerializeField] private int SaveFirstNGenotype = 0;     // How many of the first to finish the course should be saved to file
    private int genomesSaved = 0;
    [SerializeField] private int PopulationSize = 30;
    [SerializeField] private int RestartAfter = 100;         // After how many generations should the genetic algorithm be restart (0 -> never)
    [SerializeField] private bool ElitistSelection = false;  // Whether to use elitist selection or remainder stochastic sampling
    [SerializeField] private uint[] FNNTopology; // Topology of the agent's feed forward neutralnetwork
    [SerializeField] private int nInputsNN;
    [SerializeField] private int nHiddenLayersNN;
    [SerializeField] private int nOutputsNN;
    // The current population of agents.
    private List<Agent> agents = new List<Agent>();
    /// The amount of agents that are currently alive.
    public int AgentsAliveCount
    {
        get;
        private set;
    }
    public event System.Action AllAgentsDied; //trigger when all agents are dead
    private GeneticAlgo geneticAlgorithm;
    public int GenerationCount //returns age of current generation/ n prior generations
    {
        get { return geneticAlgorithm.GenerationCount; }
    }
    #endregion
    #region Constructor
    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one EvolutionManager in the Scene.");
            return;
        }
        Instance = this;
    }
    #endregion
    #region Methods
    //her forsøker jeg å bruke eget NN, kan gi problemer
    public void StartEvolution()  // Starts the evolutionary process.
    {
       
        //Create neural network to determine parameter count
        NeuralNetwork nn = new NeuralNetwork(FNNTopology);
        //Setup genetic algorithm
        geneticAlgorithm = new GeneticAlgo((int) nn.WeightCount,  PopulationSize);
        genomesSaved = 0;

        geneticAlgorithm.Evaluation = StartEvaluation;

        if (ElitistSelection)
        {
            //Second configuration
            geneticAlgorithm.Selection = GeneticAlgo.DefaultSelectionOperator;
            geneticAlgorithm.Recombination = RandomRecombination;
            geneticAlgorithm.Mutation = MutateAllButBestTwo;
        }
        else
        {   
            //First configuration
            geneticAlgorithm.Selection = RemainderStochasticSampling;
            geneticAlgorithm.Recombination = RandomRecombination;
            geneticAlgorithm.Mutation = MutateAllButBestTwo;
        }
        
        AllAgentsDied += geneticAlgorithm.EvaluationFinished;

        //Statistics
        if (saveStatistics)
        {
            _statisticsFileName = "Evaluation - " + GameStateManager.Instance.trackName + " " + DateTime.Now.ToString("yyyy_MM_dd_HH-mm-ss");
            WriteStatisticsFileStart();
            geneticAlgorithm.FitnessCalculationFinished += WriteStatisticsToFile;
        }
        geneticAlgorithm.FitnessCalculationFinished += CheckForTrackFinished;

        //Restart logic
        if (RestartAfter > 0)
        {
            geneticAlgorithm.TerminationCriterion += CheckGenerationTermination;
            geneticAlgorithm.AlgorithmTermination += OnGATermination;
        }

        geneticAlgorithm.Start();
    }

    // Writes the starting line to the statistics file, stating all genetic algorithm parameters.
    private void WriteStatisticsFileStart()
    {
        File.WriteAllText(_statisticsFileName + ".txt", "Evaluation of a Population with size " + PopulationSize + 
                ", on Track \"" + GameStateManager.Instance.trackName + "\", using the following GA operators: " + Environment.NewLine +
                "Selection: " + geneticAlgorithm.Selection.Method.Name + Environment.NewLine +
                "Recombination: " + geneticAlgorithm.Recombination.Method.Name + Environment.NewLine +
                "Mutation: " + geneticAlgorithm.Mutation.Method.Name + Environment.NewLine + 
                "FitnessCalculation: " + geneticAlgorithm.FitnessCalculationMethod.Method.Name + Environment.NewLine + Environment.NewLine);
    }

    // Appends the current generation count and the evaluation of the best genotype to the statistics file.
    private void WriteStatisticsToFile(IEnumerable<Genome> currentPopulation)
    {
        foreach (Genome genotype in currentPopulation)
        {
            File.AppendAllText(_statisticsFileName + ".txt", geneticAlgorithm.GenerationCount + "\t" + genotype.Evaluation + Environment.NewLine);
            break; //Only write first
        }
    }

    // Checks the current population and saves genotypes to a file if their evaluation is greater than or equal to 1
    private void CheckForTrackFinished(IEnumerable<Genome> currentPopulation)
    {
        if (genomesSaved >= SaveFirstNGenotype) return;

        string saveFolder = _statisticsFileName + "/";

        foreach (Genome genome in currentPopulation)
        {
            if (genome.Evaluation >= 1)
            {
                if (!Directory.Exists(saveFolder))
                    Directory.CreateDirectory(saveFolder);

                genome.SaveGenomeToFile(saveFolder + "Genome - Finished as " + (++genomesSaved) + ".txt");

                if (genomesSaved >= SaveFirstNGenotype) return;
            }
            else
                return; //List should be sorted, so we can exit here
        }
    }
    // Checks whether the termination criterion of generation count was met.
    private bool CheckGenerationTermination(IEnumerable<Genome> currentPopulation)
    {
        return geneticAlgorithm.GenerationCount >= RestartAfter;
    }
    // To be called when the genetic algorithm is terminated
    private void OnGATermination(GeneticAlgo ga)
    {
        AllAgentsDied -= ga.EvaluationFinished;

        RestartAlgorithm(5.0f);
    }
    // Restarts the algorithm after a specific wait time second wait
    private void RestartAlgorithm(float wait)
    {
        Invoke("StartEvolution", wait);
    }
    // Starts the evaluation by first creating new agents from the current population and then restarting the track manager.
    private void StartEvaluation(IEnumerable<Genome> currentPopulation)
    {
        //Create new agents from currentPopulation
        agents.Clear();
        AgentsAliveCount = 0;
        foreach (Genome genome in
                 currentPopulation) 
        {
            agents.Add(new Agent(genome, Utilities.SoftSignFunction, FNNTopology));
        }

        TrackManager.Instance.SetCarAmount(agents.Count);
        IEnumerator<CarController> carsEnum = TrackManager.Instance.GetCarEnumerator();
        for (int i = 0; i < agents.Count; i++)
        {
            if (!carsEnum.MoveNext())
            {
                Debug.LogError("Cars enum ended before agents.");
                break;
            }

            carsEnum.Current.Agent = agents[i];
            AgentsAliveCount++;
            agents[i].AgentDied += OnAgentDied;
        }

        TrackManager.Instance.Restart();
    }

    // Callback for when an agent died.
    private void OnAgentDied(Agent agent)
    {
        AgentsAliveCount--;

        if (AgentsAliveCount == 0 && AllAgentsDied != null)
            AllAgentsDied();
    }
    #endregion
    
    #region Genetic Algorithm Operators
    // Selection operator for the genetic algorithm, using a method called remainder stochastic sampling.
    private List<Genome> RemainderStochasticSampling(List<Genome> currentPopulation)
    {
        List<Genome> intermediatePopulation = new List<Genome>();
        //Put integer portion of genotypes into intermediatePopulation
        //Assumes that currentPopulation is already sorted
        foreach (Genome genome in currentPopulation)
        {
            if (genome.Fitness < 1)
                break;
            else
            {
                for (int i = 0; i < (int) genome.Fitness; i++)
                    intermediatePopulation.Add(new Genome(genome.GetParameterCopy()));
            }
        }

        //Put remainder portion of genotypes into intermediatePopulation
        foreach (Genome genome in currentPopulation)
        {
            float remainder = genome.Fitness - (int)genome.Fitness;
            if (_randomizer.NextDouble() < remainder)
                intermediatePopulation.Add(new Genome(genome.GetParameterCopy()));
        }

        return intermediatePopulation;
    }
    // Recombination operator for the genetic algorithm, recombining random genotypes of the intermediate population
    private List<Genome> RandomRecombination(List<Genome> intermediatePopulation, int newPopulationSize)
    {
        //Check input
        if (intermediatePopulation.Count < 2)
        {
            Debug.Log("count:"+intermediatePopulation.Count);
            throw new System.ArgumentException("The intermediate population has to be at least of size 2 for this operator.");
        }

        List<Genome> newPopulation = new List<Genome>();
        //Always add best two (unmodified)
        newPopulation.Add(intermediatePopulation[0]);
        newPopulation.Add(intermediatePopulation[1]);
        
        while (newPopulation.Count < newPopulationSize)
        {
            //Get two random indices that are not the same
            int randomIndex1 = _randomizer.Next(0, intermediatePopulation.Count), randomIndex2;
            do
            {
                randomIndex2 = _randomizer.Next(0, intermediatePopulation.Count);
            } while (randomIndex2 == randomIndex1);

            GeneticAlgo.CompleteCrossover(intermediatePopulation[randomIndex1], intermediatePopulation[randomIndex2], 
                GeneticAlgo.CrossProbability, out var offspring1, out var offspring2);

            newPopulation.Add(offspring1);
            if (newPopulation.Count < newPopulationSize)
                newPopulation.Add(offspring2);
        }

        return newPopulation;
    }
    // Mutates all members of the new population with the default probability, while leaving the first 2 genotypes in the list untouched.
    private void MutateAllButBestTwo(List<Genome> newPopulation)
    {
        for (int i = 2; i < newPopulation.Count; i++)
        {
            if (_randomizer.NextDouble() < GeneticAlgo.MutationPercentage)
                GeneticAlgo.MutateGenotype(newPopulation[i], GeneticAlgo.MutateProbability, GeneticAlgo.MutateAmount);
        }
    }
    // Mutates all members of the new population with the default parameters
    private void MutateAll(List<Genome> newPopulation)
    {
        foreach (Genome genome in newPopulation)
        {
            if (_randomizer.NextDouble() < GeneticAlgo.MutationPercentage)
                GeneticAlgo.MutateGenotype(genome, GeneticAlgo.MutateProbability, GeneticAlgo.MutateAmount);
        }
    }
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
