using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitRegistrator : MonoBehaviour
{

    public Action<GameObject> hitListeners;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (hitListeners != null)
        {
            hitListeners(col.gameObject);
        }
    }
}
