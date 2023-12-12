using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    //controls overall GUI
    #region Members
    public Canvas Canvas
    {
        get;
        private set;
    }
    private UISimulation simulationUI;
    // private UIStartMenuController startMenuUI;
    

    #endregion

    #region Constructor

    void Awake()
    {
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.UIController = this;
        Canvas = GetComponent<Canvas>();
        
        simulationUI = GetComponentInChildren<UISimulation>(true);
        // startMenuUI = GetComponentInChildren<UIStartMenuController>(true);
        
        simulationUI.Show();
    }

    #endregion

    #region Methods

    public void SetDisplayTarget(CarController target)
    {
        simulationUI.Target = target;
    }

    #endregion

}
