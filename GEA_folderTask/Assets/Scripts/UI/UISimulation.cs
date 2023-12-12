using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISimulation : MonoBehaviour
{
    #region Members
    //car to track data for
    private CarController target;
    public CarController Target
    {
        get { return target; }
        set
        {
            if (target != value)
            {
                target = value;

                // if (target != null)
                //     NeuralNetPanel.Display(target.Agent.FNN);
            }
        }
    }

    // GUI element references 
    [SerializeField]
    private TextMeshProUGUI[] InputTexts;
    [SerializeField]
    private TextMeshProUGUI Evaluation;
    [SerializeField]
    private TextMeshProUGUI GenerationCount;
    // private UINeuralNetworkPanel NeuralNetPanel;
    #endregion

    #region Constructors
    void Awake()
    {

    }
    void Start()
    {
        
    }
    #endregion

    #region Methods
    void Update()
    {
        if (Target != null)
        {
            //Display controls
            if (Target.CurrentControlInputs != null)
            {
                for (int i = 0; i < InputTexts.Length; i++)
                {
                    var tmp = Target.CurrentControlInputs[i].Round(2);
                    InputTexts[i].text = tmp.ToString();
                }
            }

            //Display evaluation and generation count
            Evaluation.text = Target.Agent.Genome.Evaluation.ToString();
            GenerationCount.text = GeneticManager.Instance.GenerationCount.ToString();
        }
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    #endregion
}
