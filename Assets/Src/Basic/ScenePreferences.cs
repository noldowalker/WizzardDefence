using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenePreferences : MonoBehaviour
{
    public int framerate = 60;
    // Start is called before the first frame update
    void Awake()
    {
        Application.targetFrameRate = framerate;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
