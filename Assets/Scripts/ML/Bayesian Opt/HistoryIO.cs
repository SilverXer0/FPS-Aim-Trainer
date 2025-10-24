using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RunDatum
{
    public float sens;
    public float utility;
    public long  t;

    public float score; 
    public float acc01;
}

[Serializable] 
public class TaskHistory { 
    public string taskName; 
    public List<RunDatum> data = new(); 
}

public static class HistoryIO
{
    public static TaskHistory Load(string key)
    {
        string json = PlayerPrefs.GetString(key, "");
        if (string.IsNullOrEmpty(json)) return new TaskHistory();
        return JsonUtility.FromJson<TaskHistory>(json) ?? new TaskHistory();
    }

    public static void Save(string key, TaskHistory hist)
    {
        string json = JsonUtility.ToJson(hist);
        PlayerPrefs.SetString(key, json);
        PlayerPrefs.Save();
    }

    public static void Add(TaskHistory h, float sens, float utility, int keepLast = 30)
    {
        h.data.Add(new RunDatum {
            sens = sens,
            utility = utility,
            t = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        });
        if (h.data.Count > keepLast) h.data.RemoveRange(0, h.data.Count - keepLast);
    }
}
