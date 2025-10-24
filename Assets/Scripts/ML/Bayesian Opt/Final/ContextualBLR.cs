using System;
using UnityEngine;


public class ContextualBLR
{
    private readonly int d;
    private float[,] Sigma;  // d x d
    private float[]  mu;     // d
    private System.Random rng = new System.Random();

    // Tunables
    private float priorVar = 400f;  // prior on β
    private float noiseVar = 25f;   // observation noise on sens

    public ContextualBLR(int featureDim, float priorVar = 400f, float noiseVar = 25f)
    {
        this.d = featureDim;
        this.priorVar = priorVar;
        this.noiseVar = noiseVar;

        Sigma = new float[d,d];
        mu    = new float[d];
        for (int i=0;i<d;i++) Sigma[i,i] = priorVar;
    }

    // φ is length-d feature vector
    public void Observe(float[] phi, float sens)
    {
        // Sphi = Sigma * phi
        var Sphi = new float[d];
        for (int i=0;i<d;i++)
        {
            float s = 0f;
            for (int j=0;j<d;j++) s += Sigma[i,j] * phi[j];
            Sphi[i] = s;
        }
        // denom = noise + phi^T Sigma phi
        float denom = noiseVar;
        for (int j=0;j<d;j++) denom += phi[j] * Sphi[j];

        // Kalman gain K = Sphi / denom
        var K = new float[d];
        for (int i=0;i<d;i++) K[i] = Sphi[i] / denom;

        // innovation (sens - phi^T mu)
        float pred = 0f; for (int i=0;i<d;i++) pred += phi[i] * mu[i];
        float innov = sens - pred;

        // mu = mu + K * innov
        for (int i=0;i<d;i++) mu[i] += K[i] * innov;

        // Sigma = Sigma - K (Sphi)^T
        for (int i=0;i<d;i++)
            for (int j=0;j<d;j++)
                Sigma[i,j] -= K[i] * Sphi[j];
    }

    public float SamplePredict(float[] phi)
    {
        var L = Chol(Sigma);
        var z = new float[d];
        for (int i=0;i<d;i++) z[i] = (float)StdNorm(rng); // N(0,1)

        var beta = new float[d];
        for (int i=0;i<d;i++)
        {
            float s = mu[i];
            for (int k=0;k<=i;k++) s += L[i,k]*z[k];
            beta[i] = s;
        }

        float y = 0f; for (int i=0;i<d;i++) y += beta[i]*phi[i];
        return y;
    }

    public float PredictMAP(float[] phi)
    {
        float y = 0f; for (int i=0;i<d;i++) y += mu[i]*phi[i];
        return y;
    }

    public (float[] mu, float[,] Sigma) GetParams() => (mu, Sigma);
    public void SetParams(float[] muIn, float[,] SigmaIn)
    {
        Array.Copy(muIn, mu, d);
        for (int i=0;i<d;i++) for (int j=0;j<d;j++) Sigma[i,j] = SigmaIn[i,j];
    }

    // --- helpers ---
    private static double StdNorm(System.Random r)
    {
        // Box-Muller
        double u1 = 1.0 - r.NextDouble();
        double u2 = 1.0 - r.NextDouble();
        return Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
    }

    private static float[,] Chol(float[,] A)
    {
        int n = A.GetLength(0);
        var L = new float[n,n];
        for (int i=0;i<n;i++)
        {
            for (int j=0;j<=i;j++)
            {
                double sum = A[i,j];
                for (int k=0;k<j;k++) sum -= L[i,k]*L[j,k];
                if (i==j) L[i,j] = (float)Math.Sqrt(Math.Max(sum, 1e-6));
                else       L[i,j] = (float)(sum / L[j,j]);
            }
        }
        return L;
    }
}
