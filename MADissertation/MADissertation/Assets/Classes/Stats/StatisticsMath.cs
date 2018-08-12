using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A static class that can be used to calculate various
/// statistacle values
/// </summary>
public static class StatisticsMath
{
    /// <summary>
    /// Calculates the standard deviation of a data set
    /// </summary>
    /// <param name="_dataSet"></param>
    /// <returns></returns>
    public static float CalculateStandardDeviation(int[] _dataSet)
    {
        int n = _dataSet.Length;
        float avg = CalculateAverage(_dataSet);
        float sum = 0;
        float sd = 0;
        for (int index = 0; index < n; index++)
        {
            float xPlusAvg = _dataSet[index] + avg;
            sum += Mathf.Pow(xPlusAvg, 2);
        }
        int nMinusOne = n - 1;
        float division = sum / nMinusOne;
        sd = Mathf.Sqrt(division);
        return sd;
    }

    /// <summary>
    /// Calculates the standard deviation of a data set
    /// </summary>
    /// <param name="_dataSet"></param>
    /// <returns></returns>
    public static float CalculateStandardDeviation(float[] _dataSet)
    {
        int n = _dataSet.Length;
        float avg = CalculateAverage(_dataSet);
        float sum = 0;
        float sd = 0;
        for (int index = 0; index < n; index++)
        {
            float xPlusAvg = _dataSet[index] + avg;
            sum += Mathf.Pow(xPlusAvg, 2);
        }
        int nMinusOne = n - 1;
        float division = sum / nMinusOne;
        sd = Mathf.Sqrt(division);
        return sd;
    }

    /// <summary>
    /// Calculates the average of a data set
    /// </summary>
    /// <param name="_dataSet"></param>
    /// <returns></returns>
    public static float CalculateAverage(int[] _dataSet)
    {
        float sum = 0;
        int n = _dataSet.Length;
        float avg = 0;
        for (int index = 0; index < n; index++)
        {
            sum += _dataSet[index];
        }
        avg = sum / n;
        return avg;
    }

    /// <summary>
    /// Calculates the average of a data set
    /// </summary>
    /// <param name="_dataSet"></param>
    /// <returns></returns>
    public static float CalculateAverage(float[] _dataSet)
    {
        float sum = 0;
        int n = _dataSet.Length;
        float avg = 0;
        for (int index = 0; index < n; index++)
        {
            sum += _dataSet[index];
        }
        avg = sum / n;
        return avg;
    }

    /// <summary>
    /// Calculates the average of a data set
    /// </summary>
    /// <param name="_dataSet"></param>
    /// <returns></returns>
    public static double CalculateAverage(double[] _dataSet)
    {
        double sum = 0;
        int n = _dataSet.Length;
        double avg = 0;
        for (int index = 0; index < n; index++)
        {
            sum += _dataSet[index];
        }
        avg = sum / n;
        return avg;
    }
}