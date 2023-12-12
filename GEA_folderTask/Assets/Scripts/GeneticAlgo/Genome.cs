using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using Random = System.Random;

public class Genome :  IComparable<Genome>, IEnumerable<float>
{
  private Random _randomizer = new Random();
  // private Random randomizer = new Random();
  public float Evaluation;
  public float Fitness;
  private float[] _parameters;

  public int ParameterCount
  {
    get
    {
      if (_parameters == null) return 0;
      return _parameters.Length;
    }
  }

  //overrideden indexer
  public float this [int index]
  {
    get { return _parameters[index]; }
    set { _parameters[index] = value; }
  }
  //constructor
  public Genome(float[] parameters)
  {
    _parameters = parameters;
    Fitness = 0;
  }
  
   //-n = greater, n= lower, 0 =even
  #region IComparable
  public int  CompareTo(Genome other)
  {
    return other.Fitness.CompareTo(Fitness);
  }
  #endregion
  #region IEnumerable
  //enumerators
  public IEnumerator<float> GetEnumerator()
  {
    for (var i = 0; i < _parameters.Length; i++)
    {
      yield return _parameters[i];
    }
  }
  IEnumerator IEnumerable.GetEnumerator()
  {
    foreach (var t in _parameters)
      yield return t;
  }
  #endregion

  //set parameter of genome random initially
  public void SetRandomParameters(float minVal, float maxVal)
  {

    float range = maxVal - minVal;
    
    for (var i = 0; i<_parameters.Length; i++)
    {
      _parameters[i] = (float)((_randomizer.NextDouble() * range) + minVal);
    }
  }
  public float[] GetParameterCopy()
  {
    float[] copy = new float[ParameterCount];
    for (int i = 0; i < _parameters.Length; i++)
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
    if (parameterCount == 0) return new Genome(Array.Empty<float>());

    Genome randomGenome = new Genome(new float[parameterCount]);
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

    return new Genome(parameters.ToArray());
  }
  
}
//sources:https://github.com/ArztSamuel/Applying_EANNs/tree/master  
