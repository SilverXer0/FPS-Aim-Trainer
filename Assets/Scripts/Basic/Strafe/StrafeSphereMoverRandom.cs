using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Transform))]
public class StrafeSphereMoverRandom : MonoBehaviour
{
    [Header("Horizontal Movement Bounds")]
    [Tooltip("Half the total distance (in world units) the sphere travels left/right from its start X.")]
    [SerializeField] private float halfWidth = 3f;

    [Header("Speed Settings")]
    [Tooltip("Minimum horizontal speed in units/sec.")]
    [SerializeField] private float minSpeed = 1f;

    [Tooltip("Maximum horizontal speed in units/sec.")]
    [SerializeField] private float maxSpeed = 4f;

    [Tooltip("Seconds between picking a new random speed.")]
    [SerializeField] private float speedChangeInterval = 2f;

    private float _direction = 1f;    
    private float _currentSpeed;   
    private Vector3 _startPos;      

    void Start()
    {

        _startPos = transform.position;

        _currentSpeed = Random.Range(minSpeed, maxSpeed);
        StartCoroutine(RandomizeSpeedRoutine());
    }

    void Update()
    {
        float dx = _direction * _currentSpeed * Time.deltaTime;
        transform.position += new Vector3(dx, 0f, 0f);

        float offsetX = transform.position.x - _startPos.x;
        if (offsetX >= halfWidth)
        {

            _direction = -1f;
        }
        else if (offsetX <= -halfWidth)
        {

            _direction = +1f;
        }
    }

    private IEnumerator RandomizeSpeedRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(speedChangeInterval);
            _currentSpeed = Random.Range(minSpeed, maxSpeed);
        }
    }
}
