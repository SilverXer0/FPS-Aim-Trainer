using UnityEngine;
using System.Collections;
using TMPro;

public class PreTaskCountdown : MonoBehaviour
{
    public float countdownDuration = 3f;
    public TMP_Text countdownText;

    [Tooltip("Drag in any GameObject whose scripts you want fully disabled")]
    public GameObject[] objectsToDisable;

    private Behaviour[]  _singleComps;

    void Start()
    {
        if (_singleComps != null)
            foreach(var c in _singleComps) c.enabled = false;

        foreach(var go in objectsToDisable)
        {
            foreach(var comp in go.GetComponents<Behaviour>())
                comp.enabled = false;
        }

        countdownText.gameObject.SetActive(true);
        StartCoroutine(RunCountdown());
    }

    IEnumerator RunCountdown()
    {
        float t = countdownDuration;
        while(t>0f)
        {
            countdownText.text = Mathf.Ceil(t).ToString();
            t -= Time.deltaTime;
            yield return null;
        }
        countdownText.text = "GO!";
        yield return new WaitForSeconds(0.5f);
        countdownText.gameObject.SetActive(false);
        
        if (_singleComps != null)
            foreach (var c in _singleComps) c.enabled = true;

        foreach(var go in objectsToDisable)
        {
            foreach(var comp in go.GetComponents<Behaviour>())
                comp.enabled = true;
        }
    }
}
