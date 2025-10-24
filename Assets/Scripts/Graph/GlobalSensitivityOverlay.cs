using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalSensitivityOverlay : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RawImage plotImage;

    [Header("Plot")]
    [SerializeField] private int  width = 900;
    [SerializeField] private int  height = 420;
    [SerializeField] private int  margin = 40;

    [Header("Tasks Included")]
    [Tooltip("Which task histories to merge; leave empty to include common defaults.")]
    [SerializeField] private string[] taskKeys = new string[] {
        "Gridshot","Sixshot","Headshot","Spidershot","Flickshot",
        "StrafeTrackBasic","StrafeTrackRandom","Entry","MultiShot","Zapshot"
    };

    private Texture2D _tex;

    void Awake()
    {
        _tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        _tex.filterMode = FilterMode.Point;
        plotImage.texture = _tex;
        Hide();
    }

    public void Show()
    {
        Redraw();
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private void Redraw()
    {
        // background
        var bg = new Color32(15,15,18,220);
        var px = new Color32[width*height];
        for (int i=0;i<px.Length;i++) px[i] = bg;
        _tex.SetPixels32(px);

        // merge all runs
        var all = new List<RunDatum>();
        foreach (var key in taskKeys)
        {
            var h = HistoryIO.Load($"HIST_{key}");
            if (h?.data != null) all.AddRange(h.data);
        }
        // sort by time
        all.Sort((a,b) => a.t.CompareTo(b.t));

        int n = all.Count;
        if (n == 0) { _tex.Apply(); return; }

        // bounds
        float sMin = float.PositiveInfinity, sMax = float.NegativeInfinity;
        for (int i=0;i<n;i++){ sMin = Mathf.Min(sMin, all[i].sens); sMax = Mathf.Max(sMax, all[i].sens); }
        if (Mathf.Approximately(sMin,sMax)) { sMin -= 1f; sMax += 1f; }

        int W = width - 2*margin;
        int H = height - 2*margin;

        // frame
        DrawRect(margin, margin, W, H, new Color32(90,90,95,255));

        // line plot
        Vector2? prev = null;
        for (int i=0;i<n;i++)
        {
            float x01 = (n>1) ? (float)i/(n-1) : 0.5f;
            float y01 = Mathf.InverseLerp(sMin, sMax, all[i].sens);
            int x = margin + Mathf.RoundToInt(x01*(W-1));
            int y = margin + Mathf.RoundToInt(y01*(H-1));
            DrawDisc(x,y,2,new Color32(240,240,255,255));
            if (prev.HasValue) DrawLine((int)prev.Value.x,(int)prev.Value.y, x,y, new Color32(180,200,255,255));
            prev = new Vector2(x,y);
        }

        _tex.Apply();
    }

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
