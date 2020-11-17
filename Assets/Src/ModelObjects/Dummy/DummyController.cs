using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameModels;

public class DummyController : MonoBehaviour
{
    // Ссылки на используемые объекты
    // Другие части врага
    private ColorOverlaper colorOverlaper;
    private DummyAnimationController dummyAnimationController;
    private DummyModel model;
    private HitRegistrator hitRegitrator;
    
    // Текущая цель для движения в виде глобальных координат (если есть)
    private Vector3 targetForMoving;
    private DoorController targetForAttack;

    // переменные механики шока от попадания
    private bool isShockedByHit;
    private float shockTime = 0f;
    private float shockTimeStep = 0.1f;

    // Делегаты для реакций на события с этим врагом
    public Action<Vector3, Vector3> onEnemyDestroy;
    public Action<Vector3> onMoveEnded;

    void Awake()
    {
        hitRegitrator = GetComponentInChildren<HitRegistrator>();
        if (hitRegitrator != null) {
            hitRegitrator.hitListeners += onGetingHit;
        } else {
            LogController.ShowError(LogController.Errors.NoHitRegistratorInDummy);
        }

        colorOverlaper = GetComponentInChildren<ColorOverlaper>();
        dummyAnimationController = GetComponent<DummyAnimationController>();

        model = new DummyModel(2f, 1f);
        isShockedByHit = false;

        targetForAttack = null;
    }

    void Update()
    {
        if (model.state.IsMoving() && !isShockedByHit) {
            MoveStep();
        }
    }

    private void onGetingHit(GameObject hitedObject) {
        if (hitedObject != null && hitedObject.tag != "ProjectileTag") {
            return;
        }

        // Изменение данных модели в связи с попаданием.
        model.inflictDamage(1f);
        if (model.getCurrentHitPoints() > 0) {
            // Проигрыш анимации попадания.
            float animationTime = dummyAnimationController.PlayHitedAnimation(1f);
            
            // Закрашивание красным от попадания.
            if (colorOverlaper != null)
            {
                colorOverlaper.makeRed(animationTime);
            }
            else
            {
                LogController.ShowError(LogController.Errors.NoSpriteControllerInDummy);
            }

            // Останавливаем действия на время проигрыша анимации попадания.
            if (!isShockedByHit)
            {
                
                StartCoroutine(HitShock(animationTime));
            } else {
                shockTime += animationTime;
            }
            
        } else {
            float deathTime = dummyAnimationController.PlayDeathAnimation();
            this.isShockedByHit = true;
            StartCoroutine(DeleteThis(deathTime));
        }
    }

    // Корутина запускающаяся при удалении.
    private IEnumerator DeleteThis(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        SendDeathEvent();
        Destroy(gameObject);        
    }

    // Корутина запускающаяся при попадании.
    private IEnumerator HitShock(float timeToWait)
    {
        shockTime += timeToWait;
        this.isShockedByHit = true;

        while (shockTime > 0) {
            shockTime -= shockTimeStep;
            yield return new WaitForSeconds(shockTimeStep);
        }

        shockTime = 0;
        this.isShockedByHit = false;
    }

    // Функция которая делает очередной шажок к текущей цели (клетке), запускается каждое обновление.
    private void MoveStep()
    {
        if (transform.position != targetForMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetForMoving, model.getSpeed());
        } else {
            SendEndMoveEvent();
            model.state.SetIdle();
        }
        
    }

    // Выравнивает игровой объект врага по координата
    public void AlignToCoords(Vector3 position)
    {
        gameObject.transform.position = new Vector3(position.x, position.y, gameObject.transform.position.z);
    }

    // Переводит цель в статус двигающейся и задает цель к которой надо идти в мировых координатах.
    public void Move(Vector3 positionTo)
    {
        model.state.SetMoving();
        targetForMoving = new Vector3 (positionTo.x, positionTo.y, transform.position.z);
    }

    public bool isMoving() {
        return model.state.IsMoving();
    }

    public bool isIdle()
    {
        return model.state.IsIdle();
    }

    // Активирует всех делегатов подписанных на событие уничтожения противника.
    private void SendDeathEvent()
    {
        if (onEnemyDestroy != null)
        {
            onEnemyDestroy(this.gameObject.transform.position, this.targetForMoving);
        }
    }

    // Активирует всех делегатов подписанных на событие окончания движения к заданной цели (как праивло тайл).
    private void SendEndMoveEvent()
    {
        if (onMoveEnded != null)
        {
            onMoveEnded(this.targetForMoving);
        }
    }

    public void Attack(DoorController target) {
        model.state.SetAttacking();
        targetForAttack = target;
        target.onDestroy += onDoorDestroy;
        StartCoroutine(Strike(model.AtackSpeed));
    }

    // Корутина запускающаяся при атаке.
    private IEnumerator Strike(float timeToWait)
    {
        while (targetForAttack != null)
        {
            targetForAttack.Hit(model);
            yield return new WaitForSeconds(timeToWait);
        }

        model.state.SetIdle();
    }

    // Срабатывает на событии разрушения текущией цели - убираем цель.
    private void onDoorDestroy() {
        targetForAttack = null;
    }
}
