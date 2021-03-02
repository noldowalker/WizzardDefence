using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameModels;

public class BaseEnemyController : MonoBehaviour
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
                colorOverlaper.makeHitColoration(animationTime);
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

    public void  makeInvisible() {
        colorOverlaper.makeInvisible(Color.white);
        model.Visible = false;
    }

    public void makeVisible()
    {
        colorOverlaper.makeVisible(Color.white);
        model.Visible = true;
    }

    public bool isVisible() {
        return model.Visible;
    }

    public void setInsideOrder()
    {
        SetSortingOrder(10);
    }

    public void setOutsideOrder()
    {
        SetSortingOrder(1);
    }

    private void SetSortingOrder(int order) {
        Array.ForEach (
            this.GetComponentsInChildren<SpriteRenderer>(),
            (SpriteRenderer sr) => {
                sr.sortingOrder = 10;
            }
        );
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

    public bool IsReadyToTurnBack()
    {
        return model.state.IsReadyToTurnBack();
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

    // ToDo: сделай через интерфейс
    public void Attack(DoorController target) {
        model.state.SetAttacking();
        targetForAttack = target;
        target.onDestroy += onTargetDestroy;
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
    private void onTargetDestroy() {
        targetForAttack = null;
    }

    // Запускает процесс кражи сокровища
    public void FindAndStealTreasure(IStealable target) {
        float stealingSpeed = 1f;
        makeInvisible();
        model.state.SetStealing();
        AwayFromField();
        StartCoroutine(StealProcess(stealingSpeed, target));
        
    }

    // Корутина кражи
    private IEnumerator StealProcess(float timeToWait, IStealable target)
    {
        yield return new WaitForSeconds(timeToWait);
        target.tryTakeTreasure(1);

        model.state.SetReadyToTurnBack();
    }

    // Смещает объект вне поля
    private void AwayFromField() {
        this.gameObject.transform.position = new Vector3(10000f, 10000f, this.gameObject.transform.position.z);
    }

    public void SetReadyToMove() {
        makeVisible();
        model.GoingBack = true;
        model.state.SetIdle();
    }

    public bool IsGoingBack() {
        Debug.Log("GoingBack = " + model.GoingBack);
        return model.GoingBack;
    }
}
