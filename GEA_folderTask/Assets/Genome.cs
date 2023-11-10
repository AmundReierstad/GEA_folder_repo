using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using Random = System.Random;

public class Genome :  IEnumerable
{
  private Random _randomizer = new Random();
  // private Random randomizer = new Random();
  public float evaluation;
  public float fitness;
  private List<float> _parameters;

  //constructor
  public Genome(List<float> parameters)
  {
    _parameters = parameters;
    fitness = 0;
  }
  
   //-n = greater, n= lower, 0 =even
  int  CompareGenome(Genome other)
  {
    return other.fitness.CompareTo(fitness);
  }

  //enumerators
  public IEnumerator<float> GetEnumerator()
  {
    for (var i = 0; i < _parameters.Count; i++)
    {
      yield return _parameters[i];
    }
  }
  IEnumerator IEnumerable.GetEnumerator()
  {
    foreach (var t in _parameters)
      yield return t;
  }

  //set parameter of genome random initially
  public void SetRandomParameters(float minVal, float maxVal)
  {

    float range = maxVal - minVal;
    
    for (var i = 0; i<_parameters.Count; i++)
    {
      _parameters[i] = (float)((_randomizer.NextDouble() * range) + minVal);
    }
  }
  public float[] GetParameterCopy()
  {
    float[] copy = new float[_parameters.Count];
    for (int i = 0; i < _parameters.Count; i++)
      copy[i] = _parameters[i];
    return copy;
  }
  public void SaveGenomeToFile(string filepath)
  {
    StringBuilder builder = new StringBuilder();
    foreach (float parameter in _parameters)
      builder.Append(parameter.ToString()).Append(";");
    builder.Remove(builder.Length - 1, 1);
    File.WriteAllText(filepath,builder.ToString());
  }
  
  public static Genome GenerateRandom(int parameterCount, float minValue, float maxValue)
  {
    //Check arguments
    if (parameterCount == 0) return new Genome(new List<float>(0));

    Genome randomGenome = new Genome(new List<float>(parameterCount));
    randomGenome.SetRandomParameters(minValue, maxValue);

    return randomGenome;
  }

  
  public static Genome LoadFromFile(string filePath)
  {
    string data = File.ReadAllText(filePath);

    List<float> parameters = new List<float>();
    string[] paramStrings = data.Split(';');

    foreach (string parameter in paramStrings)
    {
      float parsed;
      if (!float.TryParse(parameter, out parsed)) throw new ArgumentException("The file at given file path does not contain a valid genotype serialisation.");
      parameters.Add(parsed);
    }

    return new Genome(parameters);
  }
}
//sources:https://github.com/ArztSamuel/Applying_EANNs/tree/master  
