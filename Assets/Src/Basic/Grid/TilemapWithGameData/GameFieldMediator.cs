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


        // тайлы (сами плиточки) которые можно принудительно присваивать ячейкам (нужно было для отладки)
        public Tile notSelectedTile, selectedTile, tileInGrid, occupiedTile, blockedTile, monsterGenerationTile;

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

        // Проходит по списку позиций, находит соответствующие им тайлы и отмечает их как занятые, 
        // далее по всем тайлам из списка прочих проставляет что они не занятые.
        public void MarkTilesOccupationByPositions(List<Vector3> occupationPoints) {
            List<GameDataTile> occupiedTiles = new List<GameDataTile>();

            foreach (Vector3 point in occupationPoints) {
                GameDataTile currentTile = GetTileByPosition(point);
                tilemap.MarkTileOccupied(currentTile);
                occupiedTiles.Add(currentTile);
                // Включи для отладки если надо будет tilemap.ChangeTileSprite(currentTile.LocalPlace, occupiedTile);
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
                tilemap.ChangeTileSprite(tile.LocalPlace, notSelectedTile);
            }

        }

        private GameDataTile GetTileByPosition(Vector3 position) {
            Vector3Int enemyTilePosition = tilemap.GetTilePositionByWorldCoords(position);
            return tilemap.GetTileDataByPosition(enemyTilePosition);
        }
    }
}
