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
    public NeuralNetwork Brain
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
    //called when all agents are inactive
    public event Action<Agent> AgentDied; 
    #endregion
    #region Constructors
 
    // Initialises a new agent from given genome, constructing a new feedfoward neural network from its genome parameters
    public Agent(Genome genome, NeuralLayer.ActivationFunction defaultActivation, params uint[] topology) 
    {
        IsAlive = false;
        Genome = genome;
        Brain = new NeuralNetwork(topology);
        foreach (NeuralLayer layer in Brain.Layers)
            layer.NeuronActivationFunction = defaultActivation;
    
        //Check if topology is valid
        if (Brain.WeightCount != genome.ParameterCount)
        {
            Debug.Log("Brain:"+Brain.WeightCount+" vs"+ "genome"+ genome.ParameterCount);
            throw new ArgumentException("The given genome's parameter count must match the neural network topology's weight count.");
        }
    
        //Construct FNN from genotype
        IEnumerator<float> parameters = genome.GetEnumerator();
        foreach (NeuralLayer layer in Brain.Layers) //Loop over all layers
        {
            for (int i = 0; i < layer.Weights.GetLength(0); i++) //Loop over all nodes of current layer
            {
                for (int j = 0; j < layer.Weights.GetLength(1); j++) //Loop over all nodes of next layer
                {
                    layer.Weights[i,j] = parameters.Current;
                    parameters.MoveNext();
                }
            }
        }
    }
    #endregion
    #region Methods
    // Resets agent to alive again.
    public void Reset()
    {
        Genome.Evaluation = 0;
        Genome.Fitness = 0;
        IsAlive = true;
    }


    // Kills this agent
    public void Kill()
    {
        IsAlive = false;
    }
    #endregion

    #region IComparable
    // Compares this agent to another agent, by comparing their underlying genotypes.
    // returns the result of comparing the underlying genotypes of this agent and the given agent
    public int CompareTo(Agent other)
    {
        return this.Genome.CompareTo(other.Genome);
    }
    #endregion
}


