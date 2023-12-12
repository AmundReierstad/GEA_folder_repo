using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Perceptron: MonoBehaviour
{
    [SerializeField] private Material correctMaterial;
    [SerializeField] private Material wrongMaterial;
    private float[] _weights;
    private int _numWeights;
    private float _bias = 1;
    private GameObject _gridRef;
    private GameObject _gridRef2;
    private List<GameObject>  _refArraySpheres;
    private List<int>  _refArraySpheresLabels;
    [SerializeField] private float _learningRate=0.1f;

    void Awake()
    {
        _weights = new float[3];
        for (var i = 0; i < _weights.Length; i++)
        {
            _weights[i] = Random.Range(-1f, 1f);
        }
        
    }

    void Start()
    {
        _gridRef2 = GameObject.Find("Grid");
        _gridRef = GameObject.Find("Manager");
        _refArraySpheres = _gridRef.GetComponent<ArraySpheres>().SphereArray;
        _refArraySpheresLabels = _gridRef.GetComponent<ArraySpheres>().SphereLabels;
        CheckGueeses();
    }

     void Update()
    {
    }
    // Perceptron()
    // {
    //     for (var i = 0; i < _weights.Length; i++)
    //     {
    //         _weights[i] = Random.Range(-1f, 1f);
    //     }
    // }

    int Guees(float[] inputs)
    {
        float sum = 0;
        for (var i = 0; i < _weights.Length; i++)
        {
            sum += inputs[i] * _weights[i];
        }

        var output = Utilities.Sign.ReturnSign(sum);
        return output;
    }

    void Train(float[] inputs, int target)
    {
        int guees = Guees(inputs);
        int error = target - guees;
        
        //tune alle the weights according to the error
        for (int i = 0; i < inputs.Length; i++)
        {
            _weights[i]+=error * inputs[i]*_learningRate;
        }
    }

    public void Trainer()
    {
        for (var i = 0; i < _refArraySpheres.Count; i++)
        {
            float[] inputs = { _refArraySpheres[i].transform.position.x, _refArraySpheres[i].transform.position.y ,_bias};
            int target = _refArraySpheresLabels[i];
            Train(inputs,target);
        }
        CheckGueeses();
    }
    

     void CheckGueeses()
    {
        for (var i = 0; i < _refArraySpheres.Count; i++)
        {
            float[] inputs = { _refArraySpheres[i].transform.position.x, _refArraySpheres[i].transform.position.y , _bias};
            int target = _refArraySpheresLabels[i];
            int guess = Guees(inputs);
            if (guess == target)
            {
                _refArraySpheres[i].GetComponent<MeshRenderer>().material = correctMaterial;
            }
            else _refArraySpheres[i].GetComponent<MeshRenderer>().material = wrongMaterial;
        }
    }
}
