﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Wizard.Events;

public class EventTestButtonController : MonoBehaviour
{
    private Button button;

    void Awake()
    {

    }

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(onClickEvent);
    }

    void onClickEvent()
    {
        EventSystem.Instance.FireUiEvent(EventTypes.UI.Test, "CLIK BY "+this.gameObject.name);
    }
}
