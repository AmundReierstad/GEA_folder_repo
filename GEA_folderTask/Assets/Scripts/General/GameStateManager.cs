using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    //singleton class managing the overall simulation
    #region Members
    //name of track to be loaded
    [SerializeField] public string trackName;

    private CinemachineVirtualCamera _cameraRef;
    public UIController UIController;
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
        
    }

    // Start is called before the first frame update
    void Start()
    {
        _cameraRef = GameObject.Find("Camera").GetComponent<CinemachineVirtualCamera>();
        UIController = GameObject.Find("UI").GetComponent<UIController>();
        TrackManager.Instance.BestCarChanged += OnBestCarChanged;
        GeneticManager.Instance.StartEvolution();
    }
    #endregion

    #region Methods 
    private void OnBestCarChanged(CarController bestCar)
    {
        if (bestCar == null) _cameraRef.LookAt = null; //if no best car look at null/default camera orientation
        
        else _cameraRef.LookAt = bestCar.transform; //look at best car

        if (UIController != null)
        {
            UIController.SetDisplayTarget(bestCar);
        }
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        
    }
}
