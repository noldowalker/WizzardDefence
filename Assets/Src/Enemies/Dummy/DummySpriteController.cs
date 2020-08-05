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

    public void makeRed(float totalTime = 0.2f) {
        StartCoroutine(changeColorTo(Color.red, totalTime));
    }

    private IEnumerator changeColorTo(Color color, float totalTime) {
        currentRenderer.material.color = color;

        float elapsedTime = 0.0f;
        while (elapsedTime < totalTime) {
            elapsedTime += Time.deltaTime;
            currentRenderer.material.color = Color.Lerp(color, Color.white, (elapsedTime / totalTime));
            yield return null;
        }
    }
}
