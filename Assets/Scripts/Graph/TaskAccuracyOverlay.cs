using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TaskAccuracyOverlay : MonoBehaviour
{
    [Header("Wiring")]
    [SerializeField] private CanvasGroup canvasGroup;   // panel to show/hide (blocks raycasts)
    [SerializeField] private RawImage graphImage;       // RawImage to draw into

    [Header("Appearance")]
    [SerializeField] private int width = 640;
    [SerializeField] private int height = 320;
    [SerializeField] private int margin = 28;
    [SerializeField] private bool newestOnRight = true;
    [SerializeField] private bool invertY = false;      // set true if up/down looks flipped

    [Header("Labels (optional)")]
    [SerializeField] private Text headerText;           // e.g., "Gridshot – Accuracy"
    [SerializeField] private Text footerText;           // e.g., "Runs: N"

    private Texture2D _tex;
    private Color32[] _clear;
    private string _taskKey = "";

    void Awake()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        if (!graphImage) graphImage = GetComponentInChildren<RawImage>(true);

        _tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        _tex.wrapMode = TextureWrapMode.Clamp;
        graphImage.texture = _tex;

        _clear = new Color32[width * height];
        for (int i = 0; i < _clear.Length; i++) _clear[i] = new Color32(0, 0, 0, 0);

        HideImmediate();
    }

    // Show using an explicit key: e.g., "Gridshot"
    public void Show(string taskKey)
    {
        _taskKey = taskKey;
        Redraw();
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Convenience: use current scene name as the key
    public void ShowForCurrentScene()
    {
        Show(SceneManager.GetActiveScene().name);
    }

    public void Hide()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    private void HideImmediate() => Hide();

    public void Redraw()
    {
        // clear
        _tex.SetPixels32(_clear);

        if (string.IsNullOrEmpty(_taskKey))
        {
            _tex.Apply();
            return;
        }

        // Load history for this task
        var key = $"HIST_{_taskKey}";
        TaskHistory hist = HistoryIO.Load(key);
        List<RunDatum> src = hist?.data ?? new List<RunDatum>();

        // Pull acc01 points (skip missing / invalid)
        List<(long t, float a)> pts = new List<(long, float)>();
        foreach (var d in src)
        {
            if (d != null && d.acc01 > 0f && d.acc01 <= 1f)
                pts.Add((d.t, d.acc01));
        }

        if (pts.Count == 0)
        {
            _tex.Apply();
            if (headerText) headerText.text = $"{_taskKey} – Accuracy (no data)";
            if (footerText) footerText.text = "";
            return;
        }

        // Sort by time
        pts.Sort((x, y) => x.t.CompareTo(y.t));
        if (!newestOnRight) pts.Reverse();

        // Compute drawing rect
        int W = width - 2 * margin;
        int H = height - 2 * margin;
        DrawRect(margin, margin, W, H, new Color32(30, 35, 45, 255));

        // Axes min/max (acc01 is 0..1 by definition)
        float aMin = 0f, aMax = 1f;

        // Plot
        Vector2Int? prev = null;
        for (int i = 0; i < pts.Count; i++)
        {
            float x01 = (pts.Count > 1) ? (float)i / (pts.Count - 1) : 0.5f;
            float y01 = Mathf.InverseLerp(aMin, aMax, pts[i].a);
            if (invertY) y01 = 1f - y01;

            int x = margin + Mathf.RoundToInt(x01 * (W - 1));
            int y = margin + Mathf.RoundToInt(y01 * (H - 1));

            DrawDisc(x, y, 2, new Color32(240, 240, 255, 255));
            if (prev.HasValue)
                DrawLine(prev.Value.x, prev.Value.y, x, y, new Color32(180, 200, 255, 255));
            prev = new Vector2Int(x, y);
        }

        _tex.Apply();

        if (headerText) headerText.text = $"{_taskKey} – Accuracy";
        if (footerText) footerText.text = $"Runs: {pts.Count}     Latest: {Mathf.RoundToInt(pts[pts.Count-1].a * 100f)}%";
    }

    // --- tiny software drawing helpers ---

    private void DrawRect(int x, int y, int w, int h, Color32 c)
    {
        for (int yy = y; yy < y + h; yy++)
        {
            int idx = yy * width + x;
            for (int xx = 0; xx < w; xx++)
                _tex.SetPixel(xx + x, yy, c);
        }
    }

    private void DrawDisc(int cx, int cy, int r, Color32 c)
    {
        int r2 = r * r;
        for (int y = -r; y <= r; y++)
        {
            for (int x = -r; x <= r; x++)
            {
                if (x * x + y * y <= r2)
                    _tex.SetPixel(cx + x, cy + y, c);
            }
        }
    }

    private void DrawLine(int x0, int y0, int x1, int y1, Color32 c)
    {
        int dx = Mathf.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
        int dy = -Mathf.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
        int err = dx + dy;

        while (true)
        {
            _tex.SetPixel(x0, y0, c);
            if (x0 == x1 && y0 == y1) break;
            int e2 = 2 * err;
            if (e2 >= dy) { err += dy; x0 += sx; }
            if (e2 <= dx) { err += dx; y0 += sy; }
        }
    }
}