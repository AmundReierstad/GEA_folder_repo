using System;
using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class MovementComponent : MonoBehaviour
{
    #region Members
    //movement restrictions
    [SerializeField] private float maxSpeed = 20f;
    [SerializeField] private float acceleration = 8f;
    private const float Friction = 2f;
    [SerializeField] private float turnSpeed = 300;
    //car members
    private CarController _controller;
    private Rigidbody _rigidbody;
    private GameObject _forwardRef;
    public float Velocity
    {
        get;
        private set;
    }
    public Quaternion Rotation
    {
        get;
        private set;
    }
    private double _turnInput, _accelerationInput;
    public double[] CurrentInputs
    {
        get { return new double[] { _turnInput, _accelerationInput }; }
    }
   
    public event Action HitWall;
    #endregion

    #region Constructor
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _controller = GetComponent<CarController>();
        _forwardRef = transform.Find("Forward").gameObject;
    }
    #endregion

    #region Methods
    // FixedUpdate is called once per frame in physics step
    void FixedUpdate()
    {
        //check for player input, if enabled and input is given override prev inputs
        if (_controller != null && _controller.enableUserInput)
        {
            DetectInput();
        }
        else
        {
            
            ApplyInput();

            ApplyVelocity();

            ApplyFriction();
        }
    }
    private void ApplyInput()
    {
        //Cap inputs
        if (_turnInput > 1)
            _turnInput = 1;
        else if (_turnInput < -1)
            _turnInput = -1;
        
        if (_accelerationInput > 1)
            _accelerationInput = 1;
        else if (_accelerationInput < -1)
            _accelerationInput = -1;
        
        //Car can only accelerate further if velocity is lower than max speed
        bool canAccelerate = false;
        if (_accelerationInput < 0)
            canAccelerate = Velocity > _accelerationInput * maxSpeed;
        else if (_accelerationInput > 0)
            canAccelerate = Velocity < _accelerationInput * maxSpeed;

        //apply velocity
        if (canAccelerate)
        {
            Velocity += (float)_accelerationInput * acceleration * Time.deltaTime;
        }
        //Cap velocity
        if (Velocity > maxSpeed)
            Velocity = maxSpeed;
        else if (Velocity < -maxSpeed)
            Velocity = -maxSpeed;
        
        //calc and cache rotation
        Rotation = transform.rotation;
        Rotation *= Quaternion.AngleAxis((float)-_turnInput * turnSpeed * Time.deltaTime, Vector3.up); //BM: axis might be wrong
    }
    public void SetInputs(double[] input)
    {
        _turnInput = input[0];
        _accelerationInput = input[1];
    }

    private Vector3  GetForwardDirection()
    {
        return _forwardRef.transform.position - transform.position;
    }
    private void ApplyVelocity()
    {
        //apply rotation
        var t = transform;
        t.rotation = Rotation;
        
        //get forward vector
        var direction = GetForwardDirection();
        direction.y = 0; // car is not a plane
        //apply velocity in forward direction
        t.position += direction * (Velocity * Time.deltaTime);
        Debug.DrawLine(transform.position+(direction*3),transform.position); //shows direction in editor

    }
    //applies friction opposite to velocity direction
    private void ApplyFriction()
    {
        if (_accelerationInput == 0)
        {
            if (Velocity > 0)
            {
                Velocity -= Friction * Time.deltaTime;
                if (Velocity < 0)
                    Velocity = 0;
            }
            else if (Velocity < 0)
            {
                Velocity += Friction * Time.deltaTime;
                if (Velocity > 0)
                    Velocity = 0;            
            }
        }
    }
    //trigger on collision
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer==3)
            HitWall?.Invoke();
    }

    //stops further movement of the car
    public void Stop()
    {
        Velocity = 0;
        Rotation=Quaternion.AngleAxis(0,new Vector3(0,1,0));
    }
    #region PlayerInput
    private void DetectInput()
    {
        //gamepad
        _turnInput = Input.GetAxis("Horizontal");
        _accelerationInput = Input.GetAxis("Vertical");
        //keyboard
        if (Input.GetKey(KeyCode.W))
        {
            _accelerationInput += 0.1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            _turnInput += 0.1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            _turnInput -= 0.1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            _accelerationInput -= 0.1f;
        }
    }
    #endregion
    
    #endregion

    
}
