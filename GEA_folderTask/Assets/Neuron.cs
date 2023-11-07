using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Neuron 
    {
      public List<Synapse> IngoingSynapses;
      public List<Synapse> OutgoingSynapses;
      private double Bias { get; set; }
      private double BiasDelta { get; set; }
      private double Gradient { get; set; }
      public double Value { get; set; }
      
      public Neuron()
      {
        IngoingSynapses = new List<Synapse>();
        OutgoingSynapses = new List<Synapse>();
        Bias = NeuralNetwork.GetRandom();
      }
      
      public Neuron(IEnumerable<Neuron> inputNeurons) : this()
      {
        foreach (var inputNeuron in inputNeurons)
        {
          var synapse = new Synapse(inputNeuron, this);
          inputNeuron.OutgoingSynapses.Add(synapse);
          IngoingSynapses.Add(synapse);
        }
      }
      public virtual double CalculateValue()
      {
        return Value = Utilities.Sigmoid.Squish(IngoingSynapses.Sum(synapse => synapse.Weight * synapse.InputNeuron.Value) + Bias);
      }
      
      public double CalculateError(double target)
      {
        return target - Value;
      }
      
      public double CalculateGradient(double? target = null) //syntax might be wrong here, check *
      {
        if (target == null)
          return Gradient = OutgoingSynapses.Sum(a => a.OutputNeuron.Gradient * a.Weight) * Utilities.Sigmoid.Derivative(Value);

        return Gradient = CalculateError(target.Value) * Utilities.Sigmoid.Derivative(Value);
      }
      
      public void UpdateWeights(double learnRate, double momentum)
      {
        var prevDelta = BiasDelta;
        BiasDelta = learnRate * Gradient;
        Bias += BiasDelta + momentum * prevDelta;

        foreach (var synapse in IngoingSynapses)
        {
          prevDelta = synapse.WeightDelta;
          synapse.WeightDelta = learnRate * Gradient * synapse.InputNeuron.Value;
          synapse.Weight += synapse.WeightDelta + momentum * prevDelta;
        }
      }
      
      
      public class Synapse
      {
        public Neuron InputNeuron { get; set; }
        public Neuron OutputNeuron { get; set; }
        public double Weight { get; set; }
        public double WeightDelta { get; set; }

        public Synapse(Neuron inputNeuron, Neuron outputNeuron)
        {
          InputNeuron = inputNeuron;
          OutputNeuron = outputNeuron;
          Weight = NeuralNetwork.GetRandom();
        }
      }
      public class DataSet
      {
        public double[] Values { get; set; }
        public double[] Targets { get; set; }

        public DataSet(double[] values, double[] targets)
        {
          Values = values;
          Targets = targets;
        }
      }

    }

