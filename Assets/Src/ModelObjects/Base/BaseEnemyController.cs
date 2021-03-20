using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameModels;
using Wizard.Log;

public class BaseEnemyController : MonoBehaviour
{
    // Ссылки на используемые объекты
    // Другие части врага
    private ColorOverlaper colorOverlaper;
    private DummyAnimationController dummyAnimationController;
    private DummyModel model;
    private HitRegistrator hitRegitrator;
    private int sortingOrder = 1;
    
    // Текущая цель для движения в виде глобальных координат (если есть)
    private Vector3 targetForMoving;
    private DoorController targetForAttack;

    // переменные механики шока от попадания
    private bool isShockedByHit;
    private float shockTime = 0f;
    private float shockTimeStep = 0.2f;

    // Делегаты для реакций на события с этим врагом
    public Action<BaseEnemyController> onEnemyDestroy;
    public Action<Vector3> onMoveEnded;
    public Vector3 TargetForMoving { get => targetForMoving; }
    public int CurrentTreasures { get => model.TreasureBagCurrentTreasures; }
    public int SortingOrder { get => sortingOrder; }

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

        model = new DummyModel(1f, 1f);
        isShockedByHit = false;

        targetForAttack = null;
    }

    void Update()
    {
        if (model.EnemyState.IsMoving() && !isShockedByHit) {
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
            SendDeathEvent();
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
        sortingOrder = 10;
        SetSortingOrder(sortingOrder);
    }

    public void setOutsideOrder()
    {
        sortingOrder = 1;
        SetSortingOrder(sortingOrder);
    }

    private void SetSortingOrder(int order) {
        Array.ForEach (
            GetComponentsInChildren<SpriteRenderer>(),
            (SpriteRenderer sr) => {
                sr.sortingOrder = order;
            }
        );
    }
    // Корутина запускающаяся при удалении.
    private IEnumerator DeleteThis(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
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
            transform.position = Vector3.MoveTowards(transform.position, targetForMoving, model.GetSpeed());
        } else {
            SendEndMoveEvent();
            model.EnemyState.SetIdle();
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
        model.EnemyState.SetMoving();
        targetForMoving = new Vector3 (positionTo.x, positionTo.y, transform.position.z);
    }

    public bool isMoving() {
        return model.EnemyState.IsMoving();
    }

    public bool isIdle()
    {
        return model.EnemyState.IsIdle();
    }

    public bool IsReadyToTurnBack()
    {
        return model.EnemyState.IsReadyToTurnBack();
    }
        
    // Активирует всех делегатов подписанных на событие уничтожения противника.
    private void SendDeathEvent()
    {
        Debug.Log("SendDeathEvent");
        onEnemyDestroy?.Invoke(this);
    }

    // Активирует всех делегатов подписанных на событие окончания движения к заданной цели (как праивло тайл).
    private void SendEndMoveEvent()
    {
        if (onMoveEnded != null)
        {
            onMoveEnded(targetForMoving);
        }
    }

    // ToDo: сделай через интерфейс
    public void Attack(DoorController target) {
        model.EnemyState.SetAttacking();
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

        model.EnemyState.SetIdle();
    }
         
    // Срабатывает на событии разрушения текущией цели - убираем цель.
    private void onTargetDestroy() {
        targetForAttack = null;
    }

    // Запускает процесс кражи сокровища
    public void FindAndStealTreasure(IStealable target) {
        float stealingSpeed = 1f;
        makeInvisible();
        model.EnemyState.SetStealing();
        AwayFromField();
        StartCoroutine(StealProcess(stealingSpeed, target));
        
    }

    // Корутина кражи
    private IEnumerator StealProcess(float timeToWait, IStealable target)
    {
        yield return new WaitForSeconds(timeToWait);
        int treasuresTaken = target.TryTakeTreasure(model.TreasureBagCapacity);
        int treasuresNotFitTheBag = model.PutTreasuresInTheBag(treasuresTaken);
        if (treasuresNotFitTheBag > 0) target.ReturnTreasure(treasuresNotFitTheBag);
        model.EnemyState.SetReadyToTurnBack();
    }

    // Смещает объект вне поля
    private void AwayFromField() {
        this.gameObject.transform.position = new Vector3(10000f, 10000f, this.gameObject.transform.position.z);
    }

    public void SetReadyToMove() {
        makeVisible();
        model.GoingBack = true;
        model.EnemyState.SetIdle();
    }

    public bool IsGoingBack() {
        return model.GoingBack;
    }
}
