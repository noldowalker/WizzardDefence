﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GridTools.TilemapWithGameData;

public class EnemiesMainController : MonoBehaviour
{
    protected BaseSceneFinder scene;
    protected GameFieldController tilemap;
    protected LogController log;
 

    void Awake()
    {
        scene = GetComponentInParent<BaseSceneFinder>();
        if (scene == null)
        {
            LogController.ShowError(LogController.Errors.BaseSceneFinderIsNull);
            return;
        }

        tilemap = scene.GetComponentInChildren<GameFieldController>();
        if (tilemap == null)
        {
            LogController.ShowError(LogController.Errors.GameFieldControllerIsNull);
            return;
        }

        List<GameDataTile> occupiedTiles = new List<GameDataTile>();
        foreach (DummyController enemy in GetComponentsInChildren<DummyController>())
        {
            enemy.onEnemyDestroy += this.HandleEnemyDeath;
            enemy.onMoveEnded += this.HandleEnemyWithoutTarget;
            Vector3 enemyPosition = enemy.gameObject.transform.position;
            Vector3Int enemyTilePosition = tilemap.GetTilePositionByWorldCoords(enemyPosition);
            GameDataTile enemyTileData = tilemap.GetTileDataByPosition(enemyTilePosition);
            occupiedTiles.Add(enemyTileData);
        }

        if (occupiedTiles.Count > 0) {
            tilemap.ChangeTilesOccupationAccorgingTileData(occupiedTiles);
        }
        
    }

    // Действия которые запускаются каждый кадр
    void Update()
    {
        List<GameDataTile> occupiedTiles = new List<GameDataTile>();
        foreach (DummyController enemy in GetComponentsInChildren<DummyController>())
        {
            occupiedTiles.Add(this.EnemyUpdate(enemy));            
        }
        tilemap.ChangeTilesOccupationAccorgingTileData(occupiedTiles);
    }

    private GameDataTile EnemyUpdate(DummyController enemy) {
        Vector3 enemyPosition = enemy.gameObject.transform.position;
        Vector3Int enemyTilePosition = tilemap.GetTilePositionByWorldCoords(enemyPosition);
        GameDataTile enemyTileData = tilemap.GetTileDataByPosition(enemyTilePosition);

        if (enemy.isIdle()) {
            if (tilemap.IsDoorTile(enemyTileData)) {
                if (tilemap.door != null) {
                    enemy.Attack(tilemap.door);
                }
                else {
                    //TODO: реакция на проигрыш
                }
            } else {
                this.IdleUpdate(enemy, enemyTileData);
            }
        }

        return enemyTileData;
    }

    private void IdleUpdate(DummyController enemy, GameDataTile enemyTileData) {
        GameDataTile nextTile = tilemap.FindPathToEntrance(enemyTileData);

        if (nextTile != null)
        {
            tilemap.MarkTileAsTarget(nextTile);
            enemy.Move(nextTile.CenterWorldPlace);
        }
    }

    private void HandleEnemyDeath(Vector3 deathPoint, Vector3 targetPoint)
    {
        Vector3Int enemyTilePosition = tilemap.GetTilePositionByWorldCoords(deathPoint);
        GameDataTile enemyTileData = tilemap.GetTileDataByPosition(enemyTilePosition);

        tilemap.MarkTileFree(enemyTileData);

        if (targetPoint != null)
        {
            this.HandleEnemyWithoutTarget(targetPoint);
        }
        
    }

    private void HandleEnemyWithoutTarget(Vector3 targetPoint)
    {
            Vector3Int targetTilePosition = tilemap.GetTilePositionByWorldCoords(targetPoint);
            GameDataTile targetTileData = tilemap.GetTileDataByPosition(targetTilePosition);

            tilemap.UnmarkTileAsTarget(targetTileData);
    }


}