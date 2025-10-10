using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable] public class BetaArm { public float sens; public float alpha=1f, beta=1f;
    public float SampleScore(){ float m = alpha/(alpha+beta); return m + UnityEngine.Random.Range(-0.02f,0.02f); }
    public void Update(float r){ alpha += Mathf.Clamp01(r); beta += Mathf.Clamp01(1f-r); } }

[Serializable] public class TaskBandit { public string taskName; public List<BetaArm> arms = new(); public float lastChosen=-1f; }
[Serializable] public class SensitivityModelSave { public List<TaskBandit> tasks = new(); }

public static class SensitivityModel
{
    public static float[] DefaultBins = { 60f, 80f, 100f, 120f, 140f, 160f, 180f };

    static readonly Dictionary<string,TaskBandit> _bandits = new();
    static readonly string _path = Path.Combine(Application.persistentDataPath, "sensitivity_model.json");

    static SensitivityModel(){ Load(); }

    public static float Select(string taskName)
    {
        var tb = GetOrCreate(taskName);
        int best = 0; float bestScore = float.NegativeInfinity;
        for (int i=0;i<tb.arms.Count;i++){ float s = tb.arms[i].SampleScore(); if (s>bestScore){bestScore=s; best=i;} }
        tb.lastChosen = tb.arms[best].sens; Save();
        return tb.lastChosen;
    }

    public static void Update(string taskName, float reward01)
    {
        if(!_bandits.TryGetValue(taskName,out var tb)) return;
        for (int i=0;i<tb.arms.Count;i++)
            if (Mathf.Approximately(tb.arms[i].sens, tb.lastChosen)){ tb.arms[i].Update(Mathf.Clamp01(reward01)); break; }
        Save();
    }

    static TaskBandit GetOrCreate(string taskName)
    {
        if (_bandits.TryGetValue(taskName, out var tb)) return tb;
        tb = new TaskBandit{ taskName = taskName };
        foreach (var v in DefaultBins) tb.arms.Add(new BetaArm{ sens=v });
        _bandits[taskName] = tb; return tb;
    }

    static void Load(){ try{ if(!File.Exists(_path))return; var save=JsonUtility.FromJson<SensitivityModelSave>(File.ReadAllText(_path));
        _bandits.Clear(); if(save?.tasks!=null) foreach(var tb in save.tasks) _bandits[tb.taskName]=tb; } catch{} }
    static void Save(){ try{ var save=new SensitivityModelSave{ tasks=new List<TaskBandit>(_bandits.Values) };
        File.WriteAllText(_path, JsonUtility.ToJson(save)); } catch{} }
}
