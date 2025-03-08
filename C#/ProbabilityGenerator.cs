using System;
using System.Collections.Generic;

public static class ProbabilityGenerator
{
    public enum CurveType
    {
        Exponential,  // Default
        Linear,
        Logarithmic,
        Quadratic,
        Cubic,
        Sigmoid,  // S-curve for smooth transition
        InverseQuadratic,  // Rapid drop-off after a slow start
        Custom // User-defined function
    }

    /// <summary>
    /// Predefined probability drop-off curves for quick access.
    /// </summary>
    public static class Curves
    {
        public static Func<double, double> HalfLife => t => Math.Pow(0.5, t * 5); // Quick drop-off
        public static Func<double, double> SteepDrop => t => 1 - Math.Pow(t, 2); // Steep early drop
        public static Func<double, double> SoftDrop => t => Math.Sqrt(1 - t); // Slow initial drop
        public static Func<double, double> BellCurve => t => Math.Exp(-Math.Pow((t - 0.5) * 5, 2)); // Gaussian bell curve
        public static Func<double, double> SCurve => t => 1 / (1 + Math.Exp(-10 * (t - 0.5))); // Smooth sigmoid S-curve
        public static Func<double, double> SharpEdge => t => Math.Pow(t, 5); // Slow start, then rapid drop
    }

    /// <summary>
    /// Generates an array of percentage chances decreasing from start to end over a given number of instances.
    /// </summary>
    public static double[] GenerateChancesArray(int numInstances, double startPercentage, double endPercentage, bool wholeNumbers = true, CurveType curve = CurveType.Exponential, Func<double, double>? customFunc = null)
    {
        return GenerateChances(numInstances, startPercentage, endPercentage, wholeNumbers, curve, customFunc).ToArray();
    }

    /// <summary>
    /// Generates a list of percentage chances decreasing from start to end over a given number of instances.
    /// </summary>
    public static List<double> GenerateChancesList(int numInstances, double startPercentage, double endPercentage, bool wholeNumbers = true, CurveType curve = CurveType.Exponential, Func<double, double>? customFunc = null)
    {
        return GenerateChances(numInstances, startPercentage, endPercentage, wholeNumbers, curve, customFunc);
    }

    /// <summary>
    /// Assigns probability percentages to a given list of objects based on their position and chosen probability curve.
    /// </summary>
    public static List<(T item, double probability)> AssignProbabilities<T>(IEnumerable<T> items, double startPercentage, double endPercentage, bool wholeNumbers = true, CurveType curve = CurveType.Exponential, Func<double, double>? customFunc = null)
    {
        var itemList = new List<T>(items);
        int count = itemList.Count;
        if (count < 2) throw new ArgumentException("Item list must contain at least 2 elements.", nameof(items));

        var probabilities = GenerateChances(count, startPercentage, endPercentage, wholeNumbers, curve, customFunc);
        var result = new List<(T item, double probability)>();

        for (int i = 0; i < count; i++)
        {
            result.Add((itemList[i], probabilities[i]));
        }

        return result;
    }

    /// <summary>
    /// Internal function that generates a list of percentage chances with the specified curve type.
    /// </summary>
    private static List<double> GenerateChances(int numInstances, double startPercentage, double endPercentage, bool wholeNumbers, CurveType curve, Func<double, double>? customFunc)
    {
        if (numInstances < 2)
            throw new ArgumentException("Number of instances must be at least 2.", nameof(numInstances));

        if (wholeNumbers)
        {
            startPercentage = Math.Round(startPercentage);
            endPercentage = Math.Max(1, Math.Round(endPercentage));
        }

        List<double> chances = new List<double>();

        for (int i = 0; i < numInstances; i++)
        {
            double t = (double)i / (numInstances - 1);
            double value = curve switch
            {
                CurveType.Linear => startPercentage - t * (startPercentage - endPercentage),
                CurveType.Logarithmic => startPercentage / (1 + t * (startPercentage / endPercentage - 1)),
                CurveType.Quadratic => startPercentage * Math.Pow(1 - t, 2) + endPercentage * (1 - Math.Pow(1 - t, 2)),
                CurveType.Cubic => startPercentage * Math.Pow(1 - t, 3) + endPercentage * (1 - Math.Pow(1 - t, 3)),
                CurveType.Sigmoid => startPercentage / (1 + Math.Exp(-10 * (t - 0.5))) + endPercentage * (1 - 1 / (1 + Math.Exp(-10 * (t - 0.5)))),
                CurveType.InverseQuadratic => startPercentage / (1 + Math.Pow(t * (startPercentage / endPercentage), 2)),
                CurveType.Custom when customFunc != null => startPercentage + (endPercentage - startPercentage) * customFunc(t),
                _ => startPercentage * Math.Pow(endPercentage / startPercentage, t), // Exponential by default
            };

            chances.Add(wholeNumbers ? Math.Round(value) : value);
        }

        return chances;
    }
}
