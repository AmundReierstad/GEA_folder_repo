using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class CarController : MonoBehaviour
{
    #region Members
    private static int _idGenerator = 0;
    private static int NextID => _idGenerator++;

    [SerializeField] float maxCheckPointDelayBeforeTermination=7;
    
    //AI agent of the car
    public Agent Agent
    {
        get;
        set;
    }
    
    public float CurrentCompletionReward
    {
        get => Agent.Genome.Evaluation;
        set => Agent.Genome.Evaluation = value;
    }
    //wheter the car can be controlled by user input
    public bool enableUserInput = false;

    public MovementComponent MovementComponent
    {
        get;
    }
    //current inputs for controlloing the movement component
    public double[] CurrentControlInputs => MovementComponent.currentInputs;

    public Material CarMaterial
    {
        get;
        set;
    }

    // private Sensor[] _sensors; //BM:need implemenation
    private float _timeSinceLastCheckpoint; //need this to kill lazy cars
    #endregion

    #region Constructors

    public void Restart()
    {
        MovementComponent.enabled = true;
        _timeSinceLastCheckpoint = 0;

        // foreach (Sensor s in sensors) BM:same as previous BM
        //     s.Show();
        Agent.Reset();
        enabled = true;
    }
    

    #endregion
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _timeSinceLastCheckpoint += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        // if (!enableUserInput)
        // {
        //     double sensorOutput = new double[sensors.length];
        //     for (int i = 0; i < sensors.length; i++)
        //     {
        //         sensorOutput[i] = sensors[i].Output;
        //     }
        //
        //     double[] controlInputs = Agent.Brain.processInputs(sensorOutput);  //BM: need to implement this method in the NeuralNet class
        // }

        if (_timeSinceLastCheckpoint > maxCheckPointDelayBeforeTermination) //lazy cars get killed
        {
            Disable();
        }
    }
    
    //disables the car, by making it immovable and stops the brain/NN from further calculation
    private void Disable()
    {
        enabled = false;
        // MovementComponent.Stop();
        MovementComponent.enabled = false;

        // foreach (var Sensor s in sensors)
        // {
        //     s.Hide();
        // }
        Agent.Kill();
    }
    
    public void ReachedCheckPoint() //former CheckPointCaptured()
    {
        _timeSinceLastCheckpoint = 0;
    }
}
