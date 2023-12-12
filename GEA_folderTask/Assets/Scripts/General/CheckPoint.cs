using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
#region Members

    // The radius in which the checkpoint may be captured
    public float CaptureRadius = 3;
    //assign a mesh so its easier to tinker with it in the editor
    private MeshRenderer _meshRenderer;
  
    // The reward value earned by capturing this checkpoint.
    public float RewardValue
    {
        get;
        set;
    }

   
    // The distance to the previous checkpoint (by index)
    public float DistanceToPrevious
    {
        get;
        set;
    }


    // Distance from first checkpoint ([0]) to this checkpoint [i]
    public float AccumulatedDistance
    {
        get;
        set;
    }

   
    // The accumulated reward earned for capturing this checkpoint and its priors
    public float AccumulatedReward
    {
        get;
        set;
    }


    // boolean determining if checkpoint (mesh) is visible on screen
    public bool IsVisible
    {
        get => _meshRenderer.enabled;
        set => _meshRenderer.enabled = value;
    }
    #endregion

    #region Constructors
    void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }
    #endregion

    #region Methods
  
    // Calculates the reward earned for the given distance to this checkpoint.
    /// currentDistance= distance to this checkpoint
    public float GetRewardValue(float currentDistance)
    {
        //Calculate how close the distance is to capturing this checkpoint, relative to the distance from the previous checkpoint
        float completePerc = (DistanceToPrevious - currentDistance) / DistanceToPrevious; 

        //Reward according to capture percentage
        if (completePerc < 0)
            return 0; 
        return completePerc * RewardValue;
    }
    
    //visualises capture radius in editor when selecting the game object
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(gameObject.transform.position,CaptureRadius);
    }

    #endregion
}
