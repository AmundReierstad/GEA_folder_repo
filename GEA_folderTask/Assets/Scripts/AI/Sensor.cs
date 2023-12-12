using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Sensor : MonoBehaviour
{
    #region Members
    [SerializeField]
    private LayerMask LayerToSense;
    //The crosshair of the sensor, to be set in Unity editor.
    [SerializeField] private MeshFilter SensorCastMesh; // BM:fix for 3d implementation
    // Range to read
    private const float MAX_DIST = 10f;
    private const float MIN_DIST = 0.01f;
    // The current sensor readings in percent of maximum distance.
    public float Output
    {
        get;
        private set;
    }
    // Start is called before the first frame update
    #endregion
    
    void Start ()
    {
        SensorCastMesh = GetComponentInChildren<MeshFilter>();
        SensorCastMesh.gameObject.SetActive(true);
    }

    #region Methods

    // Update is called once per frame
    void FixedUpdate()
    {
        //calculate direction of sensor
        Vector3 direction = SensorCastMesh.transform.position - this.transform.position;
        direction.Normalize();
        
        //raycast into direction of sensor
        RaycastHit hit;
        Physics.Raycast(transform.position, direction, out hit, MAX_DIST, LayerToSense);
       
        //Check distance and clamp for min and max value
        if (hit.collider == null)
            hit.distance = MAX_DIST;
        else if (hit.distance < MIN_DIST)
            hit.distance = MIN_DIST;

        Output = hit.distance ; // to percentage of max distance //BM: Might be wrong own mod()
        //render sprite at hitpoint/ projection
        SensorCastMesh.transform.position =  this.transform.position + direction * hit.distance; //Set position of visual cross to current reading
    }
    
    public void Hide()
    {
        SensorCastMesh.gameObject.SetActive(false);
    }

    public void Show()
    {
        SensorCastMesh.gameObject.SetActive(true);
    }
    
    #endregion
    
}

