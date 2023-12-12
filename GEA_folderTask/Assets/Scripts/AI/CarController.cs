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
    //required for changing materials
    public MeshRenderer CarMesh { get; private set; }
    public MovementComponent MovementComponent { get; private set; }
    //maximum delay between checkpoints before a car is killed/terminated
    [SerializeField] float maxCheckPointDelayBeforeTermination=7;
    private float _timeSinceLastCheckpoint; //tracks time between checkpoints
    //AI agent of the car
    public Agent Agent { get; set; }
    public float CurrentCompletionReward
    {
        get => Agent.Genome.Evaluation;
        set => Agent.Genome.Evaluation = value;
    }
    //wheter the car can be controlled by user input
    public bool enableUserInput = false;
    //current inputs for controlling the movement component
    public double[] CurrentControlInputs => MovementComponent.CurrentInputs;
    private Sensor[] _sensors; 
    #endregion

    #region Constructors
    public void Awake()
    {
        MovementComponent = GetComponent<MovementComponent>();
        CarMesh = GetComponent<MeshRenderer>();
        _sensors = GetComponentsInChildren<Sensor>();
    }
    void Start()
    {
        MovementComponent.HitWall += Disable; //subscribe to listener, disables car who collides with wall
        
        //give uniq name
        name = "Car (" + NextID + ")";
    }
    #endregion

    #region Methods
    //resets car parameters
    public void Restart()
    {
        MovementComponent.enabled = true;
        _timeSinceLastCheckpoint = 0;

        foreach (Sensor s in _sensors)
            s.Show();
        Agent.Reset();
        enabled = true;
    }
  
    // increases time since last checkpoint
    void Update()
    {
        _timeSinceLastCheckpoint += Time.deltaTime;
    }
    
    //gets input from player or from FFNN if player inputs are disabled, disables car exceeding the checkpoint time threshold
    private void FixedUpdate()
    {
        if (!enableUserInput)
        {
            double[] sensorOutput = new double[_sensors.Length];
            for (int i = 0; i < _sensors.Length; i++)
            {
                sensorOutput[i] = _sensors[i].Output;
            }
            double[] controlInputs = Agent.Brain.ProcessInputs(sensorOutput); 
            MovementComponent.SetInputs(controlInputs);
        }

        if (_timeSinceLastCheckpoint > maxCheckPointDelayBeforeTermination) //lazy cars get killed
        {
            Disable();
        }
    }
    
    //disables the car, by making it immovable and stops the brain/NN from further calculation (agent.kill() )
    private void Disable()
    {
        enabled = false;
        // MovementComponent.Stop();
        MovementComponent.enabled = false;

        foreach (Sensor s in _sensors)
        {
            s.Hide();
        }
        Agent.Kill();
    }
    
    //called when checkpoint is reached, resets the checkpoint timer
    public void ReachedCheckPoint() 
    {
        _timeSinceLastCheckpoint = 0;
    }
    #endregion
}
