using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Wizard.GameField
{
    public class GameFieldMediator : MonoBehaviour
    {
        private GameFieldController tilemap;

       
        void Start()
        {
            tilemap = GetComponent<GameFieldController>();
        }

        public Vector3 GetTileCenterByPosition(Vector3 positionWithinTile) {
            GameDataTile tile = GetTileByPosition(positionWithinTile);

            if (tile == null)
                return Vector3.negativeInfinity;

            return tile.CenterWorldPlace;
        }

        public bool IsPointInDoorOutsideZone(Vector3 point) => tilemap.IsDoorOutsideTile(GetTileByPosition(point));
        public bool IsPointInInterierEnterZone(Vector3 point) => tilemap.IsHouseInterierTile(GetTileByPosition(point));
        public bool IsPointInDoorInsideZone(Vector3 point) => tilemap.IsDoorInsideTile(GetTileByPosition(point));
        public bool IsDoorBroken() => tilemap.IsDoorBroken();
        public Vector3 GetNextWaypointToTheExit(Vector3 currentPoint) {
            GameDataTile nextTile = tilemap.ToTheExit(GetTileByPosition(currentPoint));
            if (nextTile != null) {
                tilemap.MarkTileAsTarget(nextTile);
                return nextTile.CenterWorldPlace;
            }

            return Vector3.negativeInfinity;
        }

        public Vector3 GetNextWaypointToTheTreasure(Vector3 currentPoint) {
            GameDataTile currentTile = GetTileByPosition(currentPoint);
            if (currentTile == null)
                return Vector3.negativeInfinity;

            GameDataTile nextTile = tilemap.ToTheTreasure(currentTile);
            if (nextTile != null) {
                tilemap.MarkTileAsTarget(nextTile);
                return nextTile.CenterWorldPlace;
            }

            return Vector3.negativeInfinity;
        } 
        

        // Проходит по списку позиций, находит соответствующие им тайлы и отмечает их как занятые, 
        // далее по всем тайлам из списка прочих проставляет что они не занятые.
        public void MarkTilesOccupationByPositions(List<Vector3> occupationPoints) {
            List<GameDataTile> occupiedTiles = new List<GameDataTile>();

            foreach (Vector3 point in occupationPoints) {
                GameDataTile currentTile = GetTileByPosition(point);
                if (currentTile != null) {
                    tilemap.MarkTileOccupied(currentTile);
                    occupiedTiles.Add(currentTile);
                }
            }

            List<GameDataTile> freeTiles = tilemap.GetOtherTiles().Except(occupiedTiles).ToList<GameDataTile>();
            MarkTilesAsFree(freeTiles);
            tilemap.CheckTransparancy();
        }

        // Проходит по списку тайлов и помечает их как незанятые
        private void MarkTilesAsFree(List<GameDataTile> freeTiles)
        {
            foreach (GameDataTile tile in freeTiles)
            {
                tilemap.MarkTileFree(tile);
            }
        }
        
        // Ищет тайл котором принадлежит точка и если таковой найден - отмечает его как незанятый.
        public void MarkTileAtPointAsFree(Vector3 point)
        {
            GameDataTile tile = GetTileByPosition(point);
            if(tile != null)
                tilemap.MarkTileFree(tile);
        }
        
        // Ищет тайл котором принадлежит точка и если таковой найден - убирает с него флаг целевого.
        public void MarkTileAtPointAsUntargeted(Vector3 point)
        {
            GameDataTile tile = GetTileByPosition(point);
            if(tile != null)
                tilemap.UnmarkTileAsTarget(tile);
        }

        // Гетер центра для любого свободного тайла из ведущих внутрь здания
        public Vector3 GetCenterOfAnyFreeTileFromHouseInterierTiles()
        {
            GameDataTile freeTile = tilemap.GetAnyFreeTileFromHouseInterierTiles();
            if (freeTile == null)
                return Vector3.negativeInfinity;

            return freeTile.CenterWorldPlace;
        }

        // Гетер центра для любого свободного тайла из ведущих внутрь здания
        public Vector3 GetCenterOfAnyFreeTileForSpawn()
        {
            GameDataTile freeTile = tilemap.GetAnyFreeTileForSpawn();
            if (freeTile == null)
                return Vector3.negativeInfinity;

            return freeTile.CenterWorldPlace;
        }

        private GameDataTile GetTileByPosition(Vector3 position) {
            Vector3Int enemyTilePosition = tilemap.GetTilePositionByWorldCoords(position);
            return tilemap.GetTileDataByPosition(enemyTilePosition);
        }

        public DoorController GetDoorActor() => tilemap.door;
    }
}
