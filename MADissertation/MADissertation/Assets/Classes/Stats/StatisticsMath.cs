using System.Linq;
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
    /// <param name="_dataSet">Data set</param>
    /// <returns>Standard Deviation</returns>
    public static float CalculateStandardDeviation(int[] _dataSet)
    {
        // Set the count of the data set
        int n = _dataSet.Length;

        // Calculate the average of the data set
        float avg = CalculateAverage(_dataSet);

        // Set sum and standard deviation to 0
        float sum = 0;
        float sd = 0;

        // Loop through the elements of the data set
        for (int index = 0; index < n; index++)
        {
            // Calculate the difference between the element and the average
            float xPlusAvg = _dataSet[index] - avg;
            // Add the difference squared to the sum
            sum += Mathf.Pow(xPlusAvg, 2);
        }

        // Calculate n - 1
        int nMinusOne = n - 1;

        // Divide the sum by n - 1
        float division = sum / nMinusOne;

        // Square root the division value
        sd = Mathf.Sqrt(division);

        // Return the standard deviation
        return sd;
    }

    /// <summary>
    /// Calculates the standard deviation of a data set
    /// </summary>
    /// <param name="_dataSet">Data set</param>
    /// <returns>Standard Deviation</returns>
    public static float CalculateStandardDeviation(float[] _dataSet)
    {
        // Set the count of the data set
        int n = _dataSet.Length;

        // Calculate the average of the data set
        float avg = CalculateAverage(_dataSet);

        // Set sum and standard deviation to 0
        float sum = 0;
        float sd = 0;

        // Loop through the elements of the data set
        for (int index = 0; index < n; index++)
        {
            // Calculate the difference between the element and the average
            float xPlusAvg = _dataSet[index] - avg;
            // Add the difference squared to the sum
            sum += Mathf.Pow(xPlusAvg, 2);
        }

        // Calculate n - 1
        int nMinusOne = n - 1;

        // Divide the sum by n - 1
        float division = sum / nMinusOne;

        // Square root the division value
        sd = Mathf.Sqrt(division);

        // Return the standard deviation
        return sd;
    }

    /// <summary>
    /// Calculates the standard deviation of a data set
    /// </summary>
    /// <param name="_n">Number of elements</param>
    /// <param name="_avg">Average value of data set</param>
    /// <param name="_dataSet">Data set</param>
    /// <returns>Standard Deviation</returns>
    public static float CalculateStandardDeviation(int _n, float _avg, float[] _dataSet)
    {
        // Set the count of the data set
        int n = _dataSet.Length;

        // Calculate the average of the data set
        float avg = CalculateAverage(_dataSet);

        // Set sum and standard deviation to 0
        float sum = 0;
        float sd = 0;

        // Loop through the elements of the data set
        for (int index = 0; index < n; index++)
        {
            // Calculate the difference between the element and the average
            float xPlusAvg = _dataSet[index] - avg;
            // Add the difference squared to the sum
            sum += Mathf.Pow(xPlusAvg, 2);
        }

        // Calculate n - 1
        int nMinusOne = n - 1;

        // Divide the sum by n - 1
        float division = sum / nMinusOne;

        // Square root the division value
        sd = Mathf.Sqrt(division);

        // Return the standard deviation
        return sd;
    }

    /// <summary>
    /// Calculates the average of a data set
    /// </summary>
    /// <param name="_dataSet">Data set</param>
    /// <returns>Average value</returns>
    public static float CalculateAverage(int[] _dataSet)
    {
        // Set sum to 0
        float sum = 0;

        // Set the count of elements within the data set
        int n = _dataSet.Length;

        // Set the average to 0
        float avg = 0;

        // Loop through elements of the data set and add to sum
        for (int index = 0; index < n; index++)
        {
            sum += _dataSet[index];
        }

        // Calculate the average
        avg = sum / n;

        // Return the average
        return avg;
    }

    /// <summary>
    /// Calculates the average of a data set
    /// </summary>
    /// <param name="_dataSet">Data set</param>
    /// <returns>Average value</returns>
    public static float CalculateAverage(float[] _dataSet)
    {
        // Set sum to 0
        float sum = 0;

        // Set the count of elements within the data set
        int n = _dataSet.Length;

        // Set the average to 0
        float avg = 0;

        // Loop through elements of the data set and add to sum
        for (int index = 0; index < n; index++)
        {
            sum += _dataSet[index];
        }

        // Calculate the average
        avg = sum / n;

        // Return the average
        return avg;
    }

    /// <summary>
    /// Calculates the average of a data set
    /// </summary>
    /// <param name="_dataSet">Data set</param>
    /// <returns>Average value</returns>
    public static double CalculateAverage(double[] _dataSet)
    {
        // Set sum to 0
        double sum = 0;

        // Set the count of elements within the data set
        int n = _dataSet.Length;

        // Set the average to 0
        double avg = 0;

        // Loop through elements of the data set and add to sum
        for (int index = 0; index < n; index++)
        {
            sum += _dataSet[index];
        }

        // Calculate the average
        avg = sum / n;

        // Return the average
        return avg;
    }
}