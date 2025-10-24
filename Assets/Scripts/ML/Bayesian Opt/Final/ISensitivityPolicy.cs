using UnityEngine;

public struct TaskContext
{
    public string taskKey;       
    public float targetSizeNorm;  
    public float avgZNorm;        
    public float trackingSpeedNorm;
    public float dpiNorm, hzNorm; 
    public float lastAcc;         
    public float lastAbsErr;      
    public float lastSignedErr;   
}

public interface ISensitivityPolicy
{
    void Observe(string taskKey, float sens, float utility, TaskContext ctx);

    float ProposeNext(string taskKey, float currentSens, TaskHistory hist, TaskContext ctx);
}
