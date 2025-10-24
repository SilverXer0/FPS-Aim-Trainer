using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskMetricOverlay : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RawImage plotImage;

    [Header("Task")]
    [SerializeField] private string taskKey = "Gridshot";

    [Header("Plot")]
    [SerializeField] private int width = 900;
    [SerializeField] private int height = 420;
    [SerializeField] private int margin = 40;
    [Tooltip("If score/acc not logged, use utility (scaled to 0..100).")]
    [SerializeField] private bool fallbackToUtility = true;

    private Texture2D _tex;

    void Awake()
    {
        _tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        _tex.filterMode = FilterMode.Point;
        plotImage.texture = _tex;
        Hide();
    }

    public void SetTaskKey(string key) { taskKey = key; }

    public void Show(string overrideTaskKey = null)
    {
        if (!string.IsNullOrEmpty(overrideTaskKey)) taskKey = overrideTaskKey;
        Redraw();
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    private void Redraw()
    {
        var bg = new Color32(15,15,18,220);
        var px = new Color32[width*height];
        for (int i=0;i<px.Length;i++) px[i] = bg;
        _tex.SetPixels32(px);

        var hist = HistoryIO.Load($"HIST_{taskKey}");
        var data = hist?.data;
        int n = (data == null) ? 0 : data.Count;
        if (n == 0) { _tex.Apply(); return; }

        // Extract series: prefer score -> accuracy -> utility
        var series = new List<float>(n);
        bool usedAcc = false, usedUtil = false;

        // choose metric
        bool hasScore = false;
        for (int i=0;i<n;i++) if (data[i].score != 0f) { hasScore = true; break; }
        if (hasScore) {
            for (int i=0;i<n;i++) series.Add(data[i].score);
        } else {
            bool hasAcc = false;
            for (int i=0;i<n;i++) if (data[i].acc01 > 0f) { hasAcc = true; break; }
            if (hasAcc) {
                usedAcc = true;
                for (int i=0;i<n;i++) series.Add(100f * Mathf.Clamp01(data[i].acc01)); // %
            } else if (fallbackToUtility) {
                usedUtil = true;
                for (int i=0;i<n;i++) series.Add(100f * Mathf.InverseLerp(-1f,1f, data[i].utility)); // %
            } else {
                _tex.Apply(); return;
            }
        }

        // bounds
        float vMin=float.PositiveInfinity, vMax=float.NegativeInfinity;
        for (int i=0;i<n;i++){ vMin=Mathf.Min(vMin, series[i]); vMax=Mathf.Max(vMax, series[i]); }
        if (Mathf.Approximately(vMin,vMax)) { vMin -= 1f; vMax += 1f; }

        int W = width - 2*margin;
        int H = height - 2*margin;
        DrawRect(margin, margin, W, H, new Color32(90,90,95,255));

        // plot
        Vector2? prev = null;
        for (int i=0;i<n;i++)
        {
            float x01 = (n>1) ? (float)i/(n-1) : 0.5f;
            float y01 = Mathf.InverseLerp(vMin, vMax, series[i]);
            int x = margin + Mathf.RoundToInt(x01*(W-1));
            int y = margin + Mathf.RoundToInt(y01*(H-1));
            DrawDisc(x,y,2,new Color32(240,240,255,255));
            if (prev.HasValue) DrawLine((int)prev.Value.x,(int)prev.Value.y, x,y, new Color32(180,200,255,255));
            prev = new Vector2(x,y);
        }

        _tex.Apply();
    }

    // --- tiny raster helpers ---
    void SetPixelSafe(int x,int y, Color32 c){ if (x<0||x>=width||y<0||y>=height) return; _tex.SetPixel(x,y,c); }
    void DrawRect(int x,int y,int w,int h, Color32 c){
        for(int i=0;i<w;i++){ SetPixelSafe(x+i,y,c); SetPixelSafe(x+i,y+h-1,c); }
        for(int j=0;j<h;j++){ SetPixelSafe(x,y+j,c); SetPixelSafe(x+w-1,y+j,c); }
    }
    void DrawLine(int x0,int y0,int x1,int y1, Color32 c){
        int dx=Mathf.Abs(x1-x0), sx=x0<x1?1:-1; int dy=-Mathf.Abs(y1-y0), sy=y0<y1?1:-1; int err=dx+dy;
        while(true){ SetPixelSafe(x0,y0,c); if(x0==x1&&y0==y1) break; int e2=2*err; if(e2>=dy){err+=dy; x0+=sx;} if(e2<=dx){err+=dx; y0+=sy;} }
    }
    void DrawDisc(int cx,int cy,int r, Color32 c){
        int r2=r*r; for(int y=-r;y<=r;y++) for(int x=-r;x<=r;x++) if(x*x+y*y<=r2) SetPixelSafe(cx+x,cy+y,c);
    }
}
