using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class NeuralLayer 
{
    #region Members

    private static Random _random = new Random();
    // Delegate for the activation function 
    public delegate double ActivationFunction(double xValue);
    // Activation function used by the neurons of this layer.
    public ActivationFunction NeuronActivationFunction = Utilities.Sigmoid.Squish;
    // Amount of neurons in layer
    public uint NeuronCount
    {
        get;
        private set;
    }

    // The amount of neurons this layer is connected to / the amount of neurons of the next layer.
    public uint OutputCount
    {
        get;
        private set;
    }
    
    // The weights of the connections of this layer to the next layer.
    // E.g., weight [i, j] is the weight of the connection from the i-th weight
    // of this layer to the j-th weight of the next layer.
    public double[,] Weights
    {
        get;
        private set;
    }
    #endregion

    #region Constructors
    // Initialises a new neural layer for a fully connected feedforward neural network with given 
    // amount of node and with connections to the given amount of nodes of the next layer.
    //NodeCount: n nodes in this layer, outputCount: n nodes in next layer
    // All weights of the connections from this layer to the next are initialised with the default double value.
    public NeuralLayer(uint nodeCount, uint outputCount)
    {
        this.NeuronCount = nodeCount;
        this.OutputCount = outputCount;

        Weights = new double[nodeCount + 1, outputCount]; // + 1 for bias node
    }
    #endregion

    #region Methods
   
    // Sets the weights of this layer to the given values.
    // The values are ordered in neuron order. E.g., in a layer with two neurons with a next layer of three neurons 
    // the values [0-2] are the weights from neuron 0 of this layer to neurons 0-2 of the next layer respectively and 
    // the values [3-5] are the weights from neuron 1 of this layer to neurons 0-2 of the next layer respectively.
    public void SetWeights(double[] weights)
    {
        //Check arguments
        if (weights.Length != this.Weights.Length)
            throw new ArgumentException("Input weights do not match layer weight count.");

        // Copy weights from given value array
        int k = 0;
        for (int i = 0; i < this.Weights.GetLength(0); i++)
            for (int j = 0; j < this.Weights.GetLength(1); j++)
                this.Weights[i, j] = weights[k++];
    }
    
    // Processes the given inputs using the current weights to the next layer, and returns the calculated outputs
    public double[] ProcessInputs(double[] inputs)
    {
        //Check arguments
        if (inputs.Length != NeuronCount)
            throw new ArgumentException("Given xValues do not match layer input count.");

        //Calculate sum for each neuron from weighted inputs and bias
        double[] sums = new double[OutputCount];
        //Add bias (always on) neuron to inputs
        double[] biasedInputs = new double[NeuronCount + 1];
        inputs.CopyTo(biasedInputs, 0);
        biasedInputs[inputs.Length] = 1.0;

        for (int j = 0; j < this.Weights.GetLength(1); j++)
            for (int i = 0; i < this.Weights.GetLength(0); i++)
                sums[j] += biasedInputs[i] * Weights[i, j];

        //Apply activation function to sum, if set
        if (NeuronActivationFunction != null)
        {
            for (int i = 0; i < sums.Length; i++)
                sums[i] = NeuronActivationFunction(sums[i]);
        }

        return sums;
    }


    // Copies this NeuralLayer including its weights.
    public NeuralLayer DeepCopy()
    {
        //Copy weights
        double[,] copiedWeights = new double[this.Weights.GetLength(0), this.Weights.GetLength(1)];

        for (int x = 0; x < this.Weights.GetLength(0); x++)
            for (int y = 0; y < this.Weights.GetLength(1); y++)
                copiedWeights[x, y] = this.Weights[x, y];

        //Create copy
        NeuralLayer newLayer = new NeuralLayer(this.NeuronCount, this.OutputCount);
        newLayer.Weights = copiedWeights;
        newLayer.NeuronActivationFunction = this.NeuronActivationFunction;

        return newLayer;
    }
    
    // Sets the weights of the connection from this layer to the next to random values in given range.
    public void SetRandomWeights(double minValue, double maxValue)
    {
        double range = Math.Abs(minValue - maxValue);
        for (int i = 0; i < Weights.GetLength(0); i++)
            for (int j = 0; j < Weights.GetLength(1); j++)
                Weights[i, j] = minValue + (_random.NextDouble() * range); //random double between minValue and maxValue
    }


    // Returns a string representation of this layer's connection weights.
    public override string ToString()
    {
        string output = "";

        for (int x = 0; x < Weights.GetLength(0); x++)
        {
            for (int y = 0; y < Weights.GetLength(1); y++)
                output += "[" + x + "," + y + "]: " + Weights[x, y];

            output += "\n";
        }

        return output;
    }
    #endregion
}
