using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{

    public static class Sigmoid
    {
        public static double Squish(double x)
        {
            return x < -45.0 ? 0.0 : x > 45.0 ? 1.0 : 1.0 / (1.0 + Mathf.Exp((float)-x));
        }

        public static double Derivative(double x)
        {
            return x * (1 - x);
        }

        public static Func<double, double> SigmoidOnMatrix = Squish;
        public static Func<double, double> DerivativeSigmoidOnMatrix = Derivative;
        // public static Func<float, float> Squish() 
        // {
        //     return x < -45.0 ? 0.0 : x > 45.0 ? 1.0 : 1.0 / (1.0 + Mathf.Exp((float)-x));
        //     throw new NotImplementedException();
        // }
    }

    public static class Sign //activation function for perceptron
    {
        public static int ReturnSign(float x)
        {
            if (x>=0) return 1;
            return -1;
        }
    }
    public static double SoftSignFunction(double xValue)
    {
        return xValue / (1 + Math.Abs(xValue));
    }
}
