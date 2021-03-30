using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameModels;
using Wizard.Events;

public class DoorController : MonoBehaviour, IAttackable
{

    // Ссылки на используемые объекты
    // Другие части врага
    private DoorModel model;
    private HitRegistrator hitRegitrator;
    private ColorOverlaper colorOverlaper;
    private ParticleSystem[] hits;

    // Делегаты для реакций на события с этим врагом
    public Action OnDestroy { get; set; }
    public Action<string> onHit;
    public DoorModel Model { get => model; set => model = value; }
    [SerializeField]
    private float DoorMaxStructure = 5f;

    void Awake()
    {
        Model = Resources.Load<DoorModel>("ScriptableObjects/PlayerSide/Door");
        Model.MaxHitPoints = DoorMaxStructure;
    }

    private void Start()
    {
        colorOverlaper = GetComponentInChildren<ColorOverlaper>();
        hits = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem hit in hits) {
            hit.Stop();
        }

        SendHitEvent();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Hit(IAttacker attacker)
    {
        // Изменение данных модели в связи с попаданием.
        Model.inflictDamage(attacker.Damage);
        if (Model.CurrentHitPoints > 0)

        {
            ParticleSystem hit = getRandomHit();
            if (hit != null) {
                hit.Play();
                StartCoroutine(Stop(hit.main.duration, hit));
            }
            SendHitEvent();
        }
        else
        {
            SendHitEvent();
            StartCoroutine(DeleteThis(0f));
        }
    }

    private ParticleSystem getRandomHit() {
        int length = hits.Length;
        if (length == 0)
            return null;

        return hits[UnityEngine.Random.Range(0, hits.Length)];
    }

    // Корутина запускающаяся при удалении.
    private IEnumerator DeleteThis(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        SendDeathEvent();
    }

    // Корутина запускающаяся при удалении.
    private IEnumerator Stop(float timeToWait, ParticleSystem hit)
    {
        yield return new WaitForSeconds(timeToWait);
        hit.Stop();
    }

    // Активирует всех делегатов подписанных на событие уничтожения противника.
    private void SendDeathEvent()
    {
        OnDestroy?.Invoke();
    }

    public string GetHpText() {
        return "Door structure: " + Model.CurrentHitPoints + "/" + Model.MaxHitPoints;
    }

    // Активирует всех делегатов подписанных на событие удара в дверь
    private void SendHitEvent()
    {
        Debug.Log("Hit!");
        EventSystem.Instance.FireUiEvent(EventTypes.UI.DoorHPChanged, GetHpText());
    }
}
