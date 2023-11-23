using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.IO;
using UnityEngine;

public class Agent : IComparable<Agent>
{
    #region Members

    //agents genome
    public Genome Genome
    {
        get;
        private set;
    }
    // FNN derived from this agents genome
    public NeuralNet Brain
    {
        get;
        private set;
    }
    //boolean signifying wheter or not the agents is alive/particpating in the current simulation
    private bool _isAlive = false;
    public bool IsAlive
    {
        get { return _isAlive; }
        private set
        {
            if (_isAlive != value)
            {
                _isAlive = value;

                if (!_isAlive && AgentDied != null)
                    AgentDied(this);
            }
        }
    }
    #endregion

    #region Events
    public event Action<Agent> AgentDied; 
    #endregion
    #region Constructors
    /// <summary>
    /// Initialises a new agent from given genotype, constructing a new feedfoward neural network from
    /// the parameters of the genotype.
    /// </summary>
    /// <param name="genotype">The genotpye to initialise this agent from.</param>
    /// <param name="topology">The topology of the feedforward neural network to be constructed from given genotype.</param>
    // public Agent(Genome genome, NeuralLayer.ActivationFunction defaultActivation, params uint[] topology) //refactor to fit my NN
    // {
    //     IsAlive = false;
    //     this.Genome = genome;
    //     Brain = new NeuralNet(topology);
    //     foreach (NeuralLayer layer in FNN.Layers)
    //         layer.NeuronActivationFunction = defaultActivation;
    //
    //     //Check if topology is valid
    //     if (Brain.numberOfWeights!= genome.ParameterCount)
    //         throw new ArgumentException("The given genotype's parameter count must match the neural network topology's weight count.");
    //
    //     //Construct FNN from genotype
    //     IEnumerator<float> parameters = genome.GetEnumerator();
    //     foreach (NeuralLayer layer in FNN.Layers) //Loop over all layers
    //     {
    //         for (int i = 0; i < layer.Weights.GetLength(0); i++) //Loop over all nodes of current layer
    //         {
    //             for (int j = 0; j < layer.Weights.GetLength(1); j++) //Loop over all nodes of next layer
    //             {
    //                 layer.Weights[i,j] = parameters.Current;
    //                 parameters.MoveNext();
    //             }
    //         }
    //     }
    // }
    #endregion
    #region Methods
    // Resets this agent to be alive again.
    public void Reset()
    {
        Genome.Evaluation = 0;
        Genome.Fitness = 0;
        IsAlive = true;
    }


    // Kills this agent (sets IsAlive to false).
    public void Kill()
    {
        IsAlive = false;
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
    #region IComparable
    /// <summary>
    /// Compares this agent to another agent, by comparing their underlying genotypes.
    /// </summary>
    /// <param name="other">The agent to compare this agent to.</param>
    /// <returns>The result of comparing the underlying genotypes of this agent and the given agent.</returns>
    public int CompareTo(Agent other)
    {
        return this.Genome.CompareTo(other.Genome);
    }
    #endregion
}


