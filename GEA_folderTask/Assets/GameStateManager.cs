using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    #region Members
    //implemented commented out classes ( )
    // [SerializeField] private CamerMovemennt Camera
    [SerializeField] public string trackName;

    private CinemachineVirtualCamera _cameraRef;
    // public UIController UIController
    // {
    //     get;
    //     set;
    // }
    public static GameStateManager Instance
    {
        get;
        private set;
    }

    private CarController _prevbest, _prevSecondBest;
    #endregion

    #region Constructors
    //AWake is called while scene is loading
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("More than one game manager in the scene!");
        }

        Instance = this;
        
        // //Load gui scene
        // SceneManager.LoadScene("GUI", LoadSceneMode.Additive);
        //
        // //Load track
        // SceneManager.LoadScene(TrackName, LoadSceneMode.Additive);
    }

    // Start is called before the first frame update
    void Start()
    {
        _cameraRef = GameObject.Find("Camera").GetComponent<CinemachineVirtualCamera>();
        TrackManager.Instance.BestCarChanged += OnBestCarChanged;
        GeneticManager.Instance.StartEvolution();
    }
    #endregion

    #region Methods 
    //BM:using other camera implementation than that of the sample project, im using cinemachine framework, might give problems check
    private void OnBestCarChanged(CarController bestCar)
    {
        if (bestCar == null) _cameraRef.LookAt = null; //if no best car look at null/default camera orientation
        
        else _cameraRef.LookAt = bestCar.transform; //look at best car
        
        // if (UIController != null)                //BM: need to implement
        //     UIController.SetDisplayTarget(bestCar);
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        
    }
}
