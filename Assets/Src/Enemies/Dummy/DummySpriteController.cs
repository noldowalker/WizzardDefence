using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummySpriteController : MonoBehaviour
{
    private Renderer currentRenderer;

    void Awake()
    {
        currentRenderer = GetComponent<Renderer>();
    }

    public void makeRed() {
        StartCoroutine(changeColorTo(Color.red));
    }

    private IEnumerator changeColorTo(Color color) {
        currentRenderer.material.color = color;

        float ElapsedTime = 0.0f;
        float TotalTime = 0.2f;
        while (ElapsedTime < TotalTime) {
            ElapsedTime += Time.deltaTime;
            currentRenderer.material.color = Color.Lerp(color, Color.white, (ElapsedTime / TotalTime));
            yield return null;
        }
    }
}
