using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackManager : MonoBehaviour
{
    #region Members
    public static TrackManager Instance
    {
        get;
        private set;
    }
    private CheckPoint[] _checkpoints;
    [SerializeField]
    private Material bestCarMaterial;
    [SerializeField]
    private Material secondBestCarMaterial;
    [SerializeField]
    private Material normalCarMaterial;
    public CarController prototypeCar; //prefab ver car
    //Start transform components
    private Vector3 _startPos;
    private Quaternion _startRot;
    
    //struct to store car data
    private class RaceCar
    {
        public RaceCar(CarController car = null, uint checkpointIndex = 1)
        {
            this.Car = car;
            this.CheckpointIndex = checkpointIndex;
        }
        public CarController Car;
        public uint CheckpointIndex;
    }
    private List<RaceCar> _cars = new List<RaceCar>();

    public int CarCount => _cars.Count;

    #region LeadCars
    private CarController _bestCar = null;

    public CarController BestCar
    {
        get { return _bestCar; }
        private set
        {
            if (_bestCar != value)
            {
                {
                    //update materials/apperance
                    if (BestCar != null) BestCar.CarMaterial = normalCarMaterial;
                    if (value != null) value.CarMaterial = bestCarMaterial;
                    
                    //update new best car
                    CarController prevBest = _bestCar;
                    _bestCar = value;
                    if (BestCarChanged != null)
                        BestCarChanged(BestCar);

                    SecondBestCar = prevBest;
                }
            }
        }
    }

    private CarController _secondBestCar = null;
    public CarController SecondBestCar
    {
        get { return _secondBestCar;}
        private set
        {
            if (_secondBestCar != value)
            {
                //update materials/apperance
                if (SecondBestCar != null && SecondBestCar != BestCar)
                    SecondBestCar.CarMaterial = normalCarMaterial;
                if (value != null)
                    value.CarMaterial = secondBestCarMaterial;
                
                
                _secondBestCar = value;
                if (SecondBestCarChanged != null)
                    SecondBestCarChanged(SecondBestCar);
            }
        }
    }
    #region LeadCarEvents
    public event System.Action<CarController> BestCarChanged;
    public event System.Action<CarController> SecondBestCarChanged;
    #endregion
    #endregion
    public float TrackLength
    {
        get;
        private set;
    }
    #endregion

    #region Constructors

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple instance of TrackManager detected!");
            return;
        }

        Instance = this;

        //Get all checkpoints
        _checkpoints = GetComponentsInChildren<CheckPoint>();  //BM: implement checkpoints

        //Set start position and hide prototype
        var transform1 = prototypeCar.transform;
        _startPos = transform1.position;
        _startRot = transform1.rotation;
        prototypeCar.gameObject.SetActive(false);

        // CalculateCheckpointPercentages();
    }

    // Start is called before the first frame update
    void Start()
    {
        //Hide checkpoints
         foreach (CheckPoint check in _checkpoints)
                    check.IsVisible = false;
    }
    #endregion

    #region Methods
    // Update is called once per frame
    void Update()
    {
        //Update reward for each enabled car on the track
        for (int i = 0; i < _cars.Count; i++)
        {
            RaceCar car = _cars[i];
            if (car.Car.enabled)
            {
                car.Car.CurrentCompletionReward = GetCompletePerc(car.Car, ref car.CheckpointIndex);
                
                //Update best
                if (BestCar == null || car.Car.CurrentCompletionReward >= BestCar.CurrentCompletionReward)
                    BestCar = car.Car;
                else if (SecondBestCar == null || car.Car.CurrentCompletionReward >= SecondBestCar.CurrentCompletionReward)
                    SecondBestCar = car.Car;
            }
        }
    }

    public void SetCarAmount(int amount)
    {
        //Check arguments
        if (amount < 0) throw new ArgumentException("Amount may not be less than zero.");

        if (amount == CarCount) return;

        if (amount > _cars.Count)
        {
            //Add new cars
            for (int toBeAdded = amount - _cars.Count; toBeAdded > 0; toBeAdded--)
            {
                GameObject carCopy = Instantiate(prototypeCar.gameObject);
                carCopy.transform.position = _startPos;
                carCopy.transform.rotation = _startRot;
                CarController controllerCopy = carCopy.GetComponent<CarController>();
                _cars.Add(new RaceCar(controllerCopy, 1));
                carCopy.SetActive(true);
            }
        }
        else if (amount < _cars.Count)
        {
            //Remove existing cars
            for (int toBeRemoved = _cars.Count - amount; toBeRemoved > 0; toBeRemoved--)
            {
                RaceCar last = _cars[_cars.Count - 1];
                _cars.RemoveAt(_cars.Count - 1);

                Destroy(last.Car.gameObject);
            }
        }
    }
    //restarts all cars and set them to starting positons
    public void Restart()
    {
        foreach (RaceCar car in _cars)
        {
            car.Car.transform.position = _startPos;
            car.Car.transform.rotation = _startRot;
            car.Car.Restart();
            car.CheckpointIndex = 1;
        }
        BestCar = null;
        SecondBestCar = null;
    }
    public IEnumerator<CarController> GetCarEnumerator()
    {
        for (int i = 0; i < _cars.Count; i++)
            yield return _cars[i].Car;
    }
    //calculates the percentage each checkpoints account for in regards to the total distance of the track
    private void CalculateCheckpointPercentages()
    {
        _checkpoints[0].AccumulatedDistance = 0; //First checkpoint is start
        //Iterate over remaining checkpoints and set distance to previous and accumulated track distance.
        for (int i = 1; i < _checkpoints.Length; i++)
        {
            _checkpoints[i].DistanceToPrevious = Vector2.Distance(_checkpoints[i].transform.position, _checkpoints[i - 1].transform.position);
            _checkpoints[i].AccumulatedDistance = _checkpoints[i - 1].AccumulatedDistance + _checkpoints[i].DistanceToPrevious;
        }

        //Set track length to accumulated distance of last checkpoint
        TrackLength = _checkpoints[_checkpoints.Length - 1].AccumulatedDistance;
        
        //Calculate reward value for each checkpoint
        for (int i = 1; i < _checkpoints.Length; i++)
        {
            _checkpoints[i].RewardValue = (_checkpoints[i].AccumulatedDistance / TrackLength) - _checkpoints[i-1].AccumulatedReward;
            _checkpoints[i].AccumulatedReward = _checkpoints[i - 1].AccumulatedReward + _checkpoints[i].RewardValue;
        }
    }
    //calculates the completion percentage of a given car in regards to the last checkpoints reached and total tracklength
    private float GetCompletePerc(CarController car, ref uint curCheckpointIndex)
    {
        //Already all checkpoints captured
        if (curCheckpointIndex >= _checkpoints.Length)
            return 1;

        //Calculate distance to next checkpoint
        float checkPointDistance = Vector2.Distance(car.transform.position, _checkpoints[curCheckpointIndex].transform.position);

        //Check if checkpoint can be captured
        if (checkPointDistance <= _checkpoints[curCheckpointIndex].CaptureRadius)
        {
            curCheckpointIndex++;
            car.ReachedCheckPoint(); //Inform car that it captured a checkpoint
            return GetCompletePerc(car, ref curCheckpointIndex); //Recursively check next checkpoint
        }
        else
        {
            //Return accumulated reward of last checkpoint + reward of distance to next checkpoint
            return _checkpoints[curCheckpointIndex - 1].AccumulatedReward + _checkpoints[curCheckpointIndex].GetRewardValue(checkPointDistance);
        }
    }
    #endregion
}

