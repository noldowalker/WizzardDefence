﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Wizard.GameField;
using Wizard.Log;
using Wizard.Events;

public class EnemiesMainController : MonoBehaviour, ISubscribable
{
    protected BaseSceneFinder scene;
    protected GameFieldMediator fieldMediator;
    protected LogController log;
 

    void Start()
    {
        scene = GetComponentInParent<BaseSceneFinder>();
        if (scene == null)
        {
            LogController.ShowError(LogController.Errors.BaseSceneFinderIsNull);
            return;
        }

        fieldMediator = scene.GetComponentInChildren<GameFieldMediator>();
        if (fieldMediator == null)
        {
            LogController.ShowError(LogController.Errors.GameFieldMediatorIsNull);
            return;
        }

        List<Vector3> occupationPoints = new List<Vector3>();
        foreach (DummyController enemy in GetComponentsInChildren<DummyController>())
        {
            EventSystem.Instance.SubscribeActorBasedEvent(EventTypes.ActorBased.Destroy, HandleEnemyDeath);
            EventSystem.Instance.SubscribeActorBasedEvent(EventTypes.ActorBased.MoveEnd, MarkCurrentEnemyTileAsUntargeted);

            Vector3 enemyPosition = enemy.GetPosition();
            Vector3 newPosition = fieldMediator.GetTileCenterByPosition(enemy.GetPosition());
            enemy.AlignToCoords(newPosition);
            occupationPoints.Add(enemyPosition);
        }

        if (occupationPoints.Count > 0) {
            fieldMediator.MarkTilesOccupationByPositions(occupationPoints);
        }
        
    }

    public void Unsubscribe() {
        EventSystem.Instance.UnsubscribeActorBasedEvent(EventTypes.ActorBased.Destroy, HandleEnemyDeath);
        EventSystem.Instance.UnsubscribeActorBasedEvent(EventTypes.ActorBased.MoveEnd, MarkCurrentEnemyTileAsUntargeted);
    }

    void OnDestroy()
    {
        Unsubscribe();
    }

    // Действия которые запускаются каждый кадр
    public void UpdateStep()
    {
        List<Vector3> occupationPoints = new List<Vector3>();
        foreach (DummyController enemy in GetComponentsInChildren<DummyController>())
        {
            EnemyUpdate(enemy);
            occupationPoints.Add(enemy.GetPosition());            
        }

        fieldMediator.MarkTilesOccupationByPositions(occupationPoints);
    }

    private void EnemyUpdate(DummyController enemy) {
        if (enemy.isIdle())
            IdleUpdate(enemy);

        if (enemy.IsReadyToTurnBack())
            GoBackUpdate(enemy);
    }

    private void IdleUpdate(DummyController enemy) {
        if (
            fieldMediator.IsPointInDoorOutsideZone(enemy.GetPosition()) 
            && !fieldMediator.IsDoorBroken()
        ) {
            //TODO: меньше связности, сделай через IAttakable интерфейс или типа того.
            enemy.Attack(fieldMediator.GetDoorActor());
            return;
        }
        
        if (
            fieldMediator.IsPointInInterierEnterZone(enemy.GetPosition()) 
            && !enemy.IsGoingBack()
        ) {
            // TODO: вынести в экшен... наверное?
            enemy.FindAndStealTreasure(scene.GetTreasureController());
            return;
        }

        Vector3 nextPoint = (enemy.IsGoingBack()) ? ToTheExit(enemy) : ToTheTreasure(enemy);

        if (nextPoint.x != Vector3.negativeInfinity.x)
            enemy.Move(nextPoint);
        
    }

    private void GoBackUpdate(DummyController enemy) {
        Vector3 newPoint = fieldMediator.GetCenterOfAnyFreeTileFromHouseInterierTiles();
        if (newPoint.x == Vector3.negativeInfinity.x)
            return;

        enemy.AlignToCoords(newPoint);
        enemy.SetReadyToMove();
    }
    
    private Vector3 ToTheTreasure(DummyController enemy)
    {
        if (
            fieldMediator.IsPointInDoorOutsideZone(enemy.GetPosition()) 
            && enemy.isVisible()
        )
            enemy.makeInvisible();

        if (
            fieldMediator.IsPointInDoorInsideZone(enemy.GetPosition()) 
            && !enemy.isVisible()
        ) {
            enemy.makeVisible();
            enemy.setInsideOrder();
        }

        return fieldMediator.GetNextWaypointToTheTreasure(enemy.GetPosition());
    }

    private Vector3 ToTheExit(DummyController enemy)
    {
        if (
            fieldMediator.IsPointInDoorInsideZone(enemy.GetPosition()) 
            && enemy.isVisible()
        )
            enemy.makeInvisible();

        if (
            fieldMediator.IsPointInDoorOutsideZone(enemy.GetPosition()) 
            && !enemy.isVisible()
        ) {
            enemy.makeVisible();
            enemy.setOutsideOrder();
        }

        return fieldMediator.GetNextWaypointToTheExit(enemy.GetPosition());
    }

    private void HandleEnemyDeath(BaseEnemyController dyingEnemy)
    {
        fieldMediator.MarkTileAtPointAsFree(dyingEnemy.GetPosition());

        if (dyingEnemy.TargetForMoving != null)
        {
            MarkCurrentEnemyTileAsUntargeted(dyingEnemy);
        }
    }

    private void MarkCurrentEnemyTileAsUntargeted(BaseEnemyController dyingEnemy)
    {
        fieldMediator.MarkTileAtPointAsUntargeted(dyingEnemy.GetPosition());
    }


}
