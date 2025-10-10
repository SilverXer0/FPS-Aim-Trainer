using System.Collections;
using UnityEngine;

public class MultiShotManager : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Drag the GrowingTarget prefab here (with GrowingTarget component).")]
    [SerializeField] private GameObject growingTargetPrefab;

    [Header("Spawn Settings")]
    [Tooltip("Seconds between spawning each new target.")]
    [SerializeField] private float spawnInterval = 1f;

    [Tooltip("Initial uniform scale for each target (overrides prefab default).")]
    [SerializeField] private float startScale = 0.5f;

    [Tooltip("Final uniform scale at which the target disappears (overrides prefab default).")]
    [SerializeField] private float maxScale = 3f;

    [Tooltip("Seconds it takes a target to grow from startScale to maxScale (overrides prefab default).")]
    [SerializeField] private float growTime = 5f;

    private bool _isSpawning = false;

    void Start()
    {
        _isSpawning = true;
        StartCoroutine(SpawnLoop());
    }

    void OnDestroy()
    {
        _isSpawning = false;
    }

    private IEnumerator SpawnLoop()
    {
        while (_isSpawning && !Timer.GameEnded)
        {
            SpawnOneTarget();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnOneTarget()
    {
        Vector3 randomPos = TargetBounds.Instance.GetRandomPosition();

        GameObject go = Instantiate(growingTargetPrefab, randomPos, Quaternion.identity);

        GrowingTarget gt = go.GetComponent<GrowingTarget>();
        if (gt != null)
        {
            gt.GetType().GetField("startScale", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(gt, startScale);
            gt.GetType().GetField("maxScale", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(gt, maxScale);
            gt.GetType().GetField("growTime", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(gt, growTime);

            gt.OnEnable();
        }
    }
}
