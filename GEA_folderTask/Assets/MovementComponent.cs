using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovementComponent : MonoBehaviour
{
    private Rigidbody _rigidbody;
    public double[] currentInputs;
    [SerializeField]private float forceStrength=1;
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponentInParent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            Accelerate();
        }
        if (Input.GetKey(KeyCode.D))
        {
            TurnRight();
        }
        if (Input.GetKey(KeyCode.A))
        {
            TurnLeft();
        }
    }
    void Accelerate()
    {
        _rigidbody.AddForce(transform.forward*forceStrength);
    }

    void TurnRight()
    {
        transform.Rotate(0,1,0);
    }

    void TurnLeft()
    {
        transform.Rotate(0,-1,0);
    }
}
