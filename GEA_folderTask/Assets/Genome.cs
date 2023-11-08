using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using Random = System.Random;

public class Genome : MonoBehaviour, IEnumerable
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

  public void SaveGenomeToFile(string filepath)
  {
    StringBuilder builder = new StringBuilder();
    foreach (float parameter in _parameters)
      builder.Append(parameter.ToString()).Append(";");
    builder.Remove(builder.Length - 1, 1);
    File.WriteAllText(filepath,builder.ToString());
  }
  
  
}
//sources:https://github.com/ArztSamuel/Applying_EANNs/tree/master  
