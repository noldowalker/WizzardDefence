using System;
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
            enemy.onEnemyDestroy += HandleEnemyDeath;
            enemy.onEnemyDropTreasure += GetComponentInParent<BaseSceneFinder>().GetTreasureController().ReturnTreasure;
            enemy.onMoveEnded += HandleEnemyWithoutTarget;
            Vector3 enemyPosition = enemy.gameObject.transform.position;
            Vector3Int enemyTilePosition = tilemap.GetTilePositionByWorldCoords(enemyPosition);
            GameDataTile enemyTileData = tilemap.GetTileDataByPosition(enemyTilePosition);
            enemy.AlignToCoords(enemyTileData.CenterWorldPlace);
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
            GameDataTile currentTile = EnemyUpdate(enemy);
            if (currentTile != null)
                occupiedTiles.Add(currentTile);            
        }
        tilemap.ChangeTilesOccupationAccorgingTileData(occupiedTiles);
    }

    private GameDataTile EnemyUpdate(DummyController enemy) {
        Vector3 enemyPosition = enemy.gameObject.transform.position;
        Vector3Int enemyTilePosition = tilemap.GetTilePositionByWorldCoords(enemyPosition);
        GameDataTile enemyTileData = tilemap.GetTileDataByPosition(enemyTilePosition);

        if (enemy.isIdle())
            IdleUpdate(enemy, enemyTileData);

        if (enemy.IsReadyToTurnBack())
            GoBackUpdate(enemy);

        return enemyTileData;
    }

    private void IdleUpdate(DummyController enemy, GameDataTile enemyTileData) {
        if (tilemap.IsDoorOutsideTile(enemyTileData) && !tilemap.IsDoorBroken())
        {
            enemy.Attack(tilemap.door);
            return;
        }
        
        if (tilemap.IsHouseInterierTile(enemyTileData) && !enemy.IsGoingBack()) {
            enemy.FindAndStealTreasure(scene.GetTreasureController());
            return;
        }

        GameDataTile nextTile = null;
        
        nextTile = (enemy.IsGoingBack()) ? ToTheExit(enemy, enemyTileData) : ToTheTreasure(enemy, enemyTileData);

        if (nextTile != null)
        {
            tilemap.MarkTileAsTarget(nextTile);
            enemy.Move(nextTile.CenterWorldPlace);
        }
    }

    private void GoBackUpdate(DummyController enemy) {
        GameDataTile tile = tilemap.GetAnyFreeTileFromHouseInterierTiles();
        enemy.AlignToCoords(tile.CenterWorldPlace);
        enemy.SetReadyToMove();
    }
    
    private GameDataTile ToTheTreasure(DummyController enemy, GameDataTile enemyTileData)
    {
        if (tilemap.IsDoorOutsideTile(enemyTileData) && enemy.isVisible())
            enemy.makeInvisible();

        if (tilemap.IsDoorInsideTile(enemyTileData) && !enemy.isVisible())
        {
            enemy.makeVisible();
            enemy.setInsideOrder();
        }

        return tilemap.ToTheTreasure(enemyTileData);
    }

    private GameDataTile ToTheExit(DummyController enemy, GameDataTile enemyTileData)
    {
        if (tilemap.IsDoorInsideTile(enemyTileData) && enemy.isVisible())
            enemy.makeInvisible();

        if (tilemap.IsDoorOutsideTile(enemyTileData) && !enemy.isVisible())
        {
            enemy.makeVisible();
            enemy.setOutsideOrder();
        }

        return tilemap.ToTheExit(enemyTileData);
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
