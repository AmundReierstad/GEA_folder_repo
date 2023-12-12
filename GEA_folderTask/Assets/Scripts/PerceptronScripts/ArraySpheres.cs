using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArraySpheres : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float numberOfSpheres;
    private GameObject _sphereRef;
    public List<GameObject> SphereArray=null;
    public List<int> SphereLabels;
    void Awake()
    {
        _sphereRef = GameObject.Find("Sphere");
        for (int i = 0; i < numberOfSpheres; i++)
        {
            GameObject newSphere=Instantiate(_sphereRef, new Vector3(Random.Range(-5f,5f),Random.Range(-5f,5f),0),Quaternion.identity);
            if (newSphere.transform.position.x > newSphere.transform.position.y) // assign label depending on x-y relation / for proof reading gueeses of the perceptron/ fasit
                SphereLabels.Add(1);    
            else
                SphereLabels.Add(-1);
            SphereArray.Add(newSphere);
        }
 
        
    }

     public List<GameObject> GetSpheresVec ()
    {
        return SphereArray;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
