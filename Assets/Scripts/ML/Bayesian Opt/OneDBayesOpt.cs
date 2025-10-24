using System;
using System.Collections.Generic;
using UnityEngine;

// Simple 1D Gaussian Process with RBF kernel + Expected Improvement acquisition.
// Uses a tiny Cholesky solver (good for small N ~ up to 200).
public class OneDBayesOpt
{
    // Public knobs
    public float sigmaF = 1.0f;     // signal std
    public float length = 18.0f;    // RBF length scale (in sensitivity units)
    public float sigmaN = 0.15f;    // noise std on utility
    public float minSens = 10f, maxSens = 200f;

    private readonly List<double> xs = new();
    private readonly List<double> ys = new();

    // RBF kernel
    private double K(double a, double b)
    {
        double d = a - b;
        return sigmaF * sigmaF * Math.Exp(-(d * d) / (2.0 * length * length));
    }

    public void Clear() { xs.Clear(); ys.Clear(); }
    public int Count => xs.Count;
    public void Add(double x, double y) { xs.Add(x); ys.Add(y); }

    // Solve K alpha = y using Cholesky (K is SPD)
    private void SolveCholesky(double[,] Kmat, double[] y, out double[] alpha)
    {
        int n = y.Length;
        // Cholesky: K = L L^T
        double[,] L = new double[n, n];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j <= i; j++)
            {
                double sum = Kmat[i, j];
                for (int k = 0; k < j; k++) sum -= L[i, k] * L[j, k];
                if (i == j) L[i, j] = Math.Sqrt(Math.Max(sum, 1e-9));
                else         L[i, j] = sum / L[j, j];
            }
        }
        // Solve L z = y
        double[] z = new double[n];
        for (int i = 0; i < n; i++)
        {
            double sum = y[i];
            for (int k = 0; k < i; k++) sum -= L[i, k] * z[k];
            z[i] = sum / L[i, i];
        }
        // Solve L^T alpha = z
        alpha = new double[n];
        for (int i = n - 1; i >= 0; i--)
        {
            double sum = z[i];
            for (int k = i + 1; k < n; k++) sum -= L[k, i] * alpha[k];
            alpha[i] = sum / L[i, i];
        }
    }

    // Also need (K^-1 kstar) for variance; implement via two triangular solves
    private void SolveKInvTimes(double[,] Kmat, double[] v, out double[] outVec)
    {
        int n = v.Length;
        // Re-Cholesky (small N so OK)
        double[,] L = new double[n, n];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j <= i; j++)
            {
                double sum = Kmat[i, j];
                for (int k = 0; k < j; k++) sum -= L[i, k] * L[j, k];
                if (i == j) L[i, j] = Math.Sqrt(Math.Max(sum, 1e-9));
                else         L[i, j] = sum / L[j, j];
            }
        }
        // Solve L y = v
        double[] y = new double[n];
        for (int i = 0; i < n; i++)
        {
            double sum = v[i];
            for (int k = 0; k < i; k++) sum -= L[i, k] * y[k];
            y[i] = sum / L[i, i];
        }
        // Solve L^T x = y
        outVec = new double[n];
        for (int i = n - 1; i >= 0; i--)
        {
            double sum = y[i];
            for (int k = i + 1; k < n; k++) sum -= L[k, i] * outVec[k];
            outVec[i] = sum / L[i, i];
        }
    }

    private (double mu, double var) Posterior(double xstar)
    {
        int n = xs.Count;
        if (n == 0) return (0.0, sigmaF * sigmaF);

        // Build K
        double[,] Kmat = new double[n, n];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++) Kmat[i, j] = K(xs[i], xs[j]);
            Kmat[i, i] += sigmaN * sigmaN;
        }
        // alpha = K^-1 y
        double[] y = ys.ToArray();
        SolveCholesky(Kmat, y, out var alpha);

        // kstar
        double[] kstar = new double[n];
        for (int i = 0; i < n; i++) kstar[i] = K(xs[i], xstar);

        // mu = k*^T alpha
        double mu = 0.0;
        for (int i = 0; i < n; i++) mu += kstar[i] * alpha[i];

        // var = k(x*,x*) - k*^T K^-1 k*
        double[] Kinvkstar;
        SolveKInvTimes(Kmat, kstar, out Kinvkstar);
        double q = 0.0;
        for (int i = 0; i < n; i++) q += kstar[i] * Kinvkstar[i];
        double var = Math.Max(1e-9, K(xstar, xstar) - q);
        return (mu, var);
    }

    private static double StdNormalPdf(double z) => Math.Exp(-0.5 * z * z) / Math.Sqrt(2.0 * Math.PI);
    private static double StdNormalCdf(double z) => 0.5 * (1.0 + Erf(z / Math.Sqrt(2.0)));

    private static double Erf(double x)
    {
        // Abramowitz-Stegun approximation
        double t = 1.0 / (1.0 + 0.5 * Math.Abs(x));
        double tau = t * Math.Exp(-x*x - 1.26551223 +
                   1.00002368*t + 0.37409196*t*t + 0.09678418*Math.Pow(t,3) -
                   0.18628806*Math.Pow(t,4) + 0.27886807*Math.Pow(t,5) -
                   1.13520398*Math.Pow(t,6) + 1.48851587*Math.Pow(t,7) -
                   0.82215223*Math.Pow(t,8) + 0.17087277*Math.Pow(t,9));
        return x >= 0 ? 1 - tau : tau - 1;
    }

    private double EI(double x)
    {
        if (xs.Count == 0) return 1e3;
        double best = double.NegativeInfinity;
        foreach (var v in ys) if (v > best) best = v;

        var (mu, var) = Posterior(x);
        double sigma = Math.Sqrt(var);
        if (sigma < 1e-6) return 0.0;

        double z = (mu - best) / sigma;
        return (mu - best) * StdNormalCdf(z) + sigma * StdNormalPdf(z);
    }

    public double SuggestNext(double current, float maxStep = 12f, int grid = 61)
    {
        double left  = Math.Max(minSens, current - maxStep);
        double right = Math.Min(maxSens, current + maxStep);

        // Wider exploration if very little data:
        if (xs.Count < 4) { left = minSens; right = maxSens; }

        double bestX = current, bestEI = -1.0;
        for (int i = 0; i < grid; i++)
        {
            double x = left + (right - left) * i / (grid - 1.0);
            double ei = EI(x);
            if (ei > bestEI) { bestEI = ei; bestX = x; }
        }
        return bestX;
    }

    public void FitFromHistory(TaskHistory h)
    {
        Clear();
        foreach (var d in h.data) Add(d.sens, d.utility);
    }
}
