using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wizard.Log;
using Wizard.Events;
using Wizard.GameField;


namespace Wizard
{
    public class MainSystem : MonoBehaviour
    {
        private static bool isPaused = false;
        public static bool IsPaused { get => isPaused; }

        private EventSystem events;
        private EnemiesMainController enemies;
        private GameFieldController field;
        private MageMainController mage;

        void Awake()
        {
            isPaused = true;           
        }

        void Start()
        {
            events = GetComponent<EventSystem>();
            enemies = GetComponentInChildren<EnemiesMainController>();
            field = GetComponentInChildren<GameFieldController>();
            mage = GetComponentInChildren<MageMainController>();
        }

        void Update()
        {
            enemies.UpdateStep();
        }
    }
}

