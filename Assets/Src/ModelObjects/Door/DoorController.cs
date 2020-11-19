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
    private ParticleSystem[] hits;

    // Делегаты для реакций на события с этим врагом
    public Action onDestroy;
    public Action<string> onHit;
    public DoorModel Model { get => model; set => model = value; }

    void Awake()
    {
        colorOverlaper = GetComponentInChildren<ColorOverlaper>();
        hits = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem hit in hits) {
            hit.Stop();
        }
        Model = new DoorModel(50f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Hit(BaseModel attacker)
    {
        // Изменение данных модели в связи с попаданием.
        Model.inflictDamage(attacker.Damage);
        //Debug.Log("Door HP: " + Model.getCurrentHitPoints());
        if (Model.getCurrentHitPoints() > 0)
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
        onDestroy?.Invoke();
    }

    public string GetHpText() {
        return "" + Model.getCurrentHitPoints() + "/" + Model.getMaxHitPoints();
    }
    // Активирует всех делегатов подписанных на событие удара в дверь
    private void SendHitEvent()
    {
        onHit?.Invoke(GetHpText());
    }
}
