using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPCountController : MonoBehaviour
{
    private Text text;
    // Start is called before the first frame update
    void Awake()
    {
        text = GetComponent<Text>();
        text.color = Color.white;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeTextOn(string textValue) {
        GetComponent<Text>().text = textValue;
    }
}
