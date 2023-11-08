using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra; //Matrix library

public class NeuralNet : MonoBehaviour
{
    [SerializeField] private int numberOfInputs;
    [SerializeField] private int numberOfHiddenLayers;
    [SerializeField] private int numberOfOutputs;
    // NeuralNet(int nInputs, int nHiddenLayers, int nOutputs)
    // {
    //     
    // }
    private Matrix<double> _input;
    private Matrix<double> _weightsInpHiddenLayer;
    private Matrix<double> _weightsHiddenOutLayer;
    private Matrix<double> _targets;
    private Matrix<double> _biasInputLayer;
    private Matrix<double> _biasOutputLayer;
    private float _learningRate=0.1f;
    List<Matrix<double>> FeedForward(Matrix<double> inputs)
    {
        //store matrices in list
        var r = new List<Matrix<double>> ();
        //generating hidden outputs
        var hiddenLayer = _weightsInpHiddenLayer * _input;
        hiddenLayer += _biasInputLayer;
        hiddenLayer=hiddenLayer.Map(Utilities.Sigmoid.SigmoidOnMatrix);
        r.Add(hiddenLayer);

        //generating final output
        var output = _weightsHiddenOutLayer * hiddenLayer;
        output += _biasOutputLayer;
        output.Map(Utilities.Sigmoid.SigmoidOnMatrix);
        //format as array
        var outputAsArray=output.ToArray();
        r.Add(output);
        return r;
    }

    void Train(Matrix<double> target, Matrix<double> inputs)
    {
        //feedforward to get values of output and hidden layers
        var tmp=FeedForward(inputs);
        var outputY = tmp[1];
        var hiddenLayerY = tmp[0];
        //calculate errors
        //error is computed: ERROR= target-Y , Y= output
            //output layer:
        var errorOutputs = _targets - inputs;
            //hidden layer:
        var hiddenWeightsTransposed = _weightsHiddenOutLayer.Transpose();
        var errorHiddenLayer = hiddenWeightsTransposed * errorOutputs;
        //calculate gradients
            //output layer:
       
        /*calculate deltas:
        DeltaW(layer)(ij)=Lr*E*dSigmoid*P || (gradient*P) 
             Lr=learning rate,
             E=errors,
             dSigmoid=derivative of the sigmoid (of the output),
             P=output (y) (of the layer Prior to the weights, transposed)
             (gradient = LR*E*dSigmoid)
         */
        var dSigmoidOuter = outputY.Map(Utilities.Sigmoid.DerivativeSigmoidOnMatrix);
        var yHidden = hiddenLayerY.Transpose();
        var gradientHiddenOuter = _learningRate * errorOutputs * dSigmoidOuter;
        var deltaWeightsHiddenOuterLayer = gradientHiddenOuter * yHidden;
        //apply deltas to weights and biases (just the gradient for bias)
        _weightsHiddenOutLayer += deltaWeightsHiddenOuterLayer;
        _biasOutputLayer += gradientHiddenOuter;
        //same for hidden layer
        var dSigmoidHidden = hiddenLayerY.Map(Utilities.Sigmoid.DerivativeSigmoidOnMatrix);
        var yInner = inputs.Transpose();
        var gradientInnerHidden = _learningRate * errorHiddenLayer * dSigmoidHidden;
        var deltaWeightsHiddenInputLayer =gradientInnerHidden * yInner;
        _weightsInpHiddenLayer += deltaWeightsHiddenInputLayer;
        _biasInputLayer += gradientInnerHidden;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
    //sources: videoseries on neural networks by the Coding Train
    // https://www.youtube.com/playlist?list=PLRqwX-V7Uu6aCibgK1PTWWu9by6XFdCfh
}
