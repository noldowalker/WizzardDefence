using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameModels;

public class DoorController : MonoBehaviour
{

    // Ссылки на используемые объекты
    // Другие части врага
    private DoorModel model;
    private HitRegistrator hitRegitrator;
    private ColorOverlaper colorOverlaper;
    private ParticleSystem hit;

    // Делегаты для реакций на события с этим врагом
    public Action onDestroy;
    public DoorModel Model { get => model; set => model = value; }

    void Awake()
    {
        colorOverlaper = GetComponentInChildren<ColorOverlaper>();
        hit = GetComponentInChildren<ParticleSystem>();
        hit.Stop();
        Model = new DoorModel(4f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Hit(BaseModel attacker)
    {
        // Изменение данных модели в связи с попаданием.
        Model.inflictDamage(attacker.Damage);
        Debug.Log("Door HP: " + Model.getCurrentHitPoints());
        if (Model.getCurrentHitPoints() > 0)
        {
            hit.Play();
            StartCoroutine(Stop(hit.main.duration));
        }
        else
        {
            StartCoroutine(DeleteThis(0f));
        }
    }

    // Корутина запускающаяся при удалении.
    private IEnumerator DeleteThis(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        SendDeathEvent();
    }

    // Корутина запускающаяся при удалении.
    private IEnumerator Stop(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        hit.Stop();
    }

    // Активирует всех делегатов подписанных на событие уничтожения противника.
    private void SendDeathEvent()
    {
        if (onDestroy != null)
        {
            onDestroy();
        }
    }
}
