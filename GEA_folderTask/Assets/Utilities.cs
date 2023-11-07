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
    }

    public static class Sign //activation function for perceptron
    {
        public static int ReturnSign(float x)
        {
            if (x>=0) return 1;
            return -1;
        }
    }
}
