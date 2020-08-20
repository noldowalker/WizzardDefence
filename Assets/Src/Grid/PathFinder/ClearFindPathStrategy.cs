using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridTools.TilemapWithGameData;

namespace GridTools.PathFinding
{
    public class ClearFindPathStrategy : IFindPathStrategy
    {
        public List<GameDataTile> GetNeigboursForTile(GameDataTile currentTile, BaseGameDataTilemapController tilemap)
        {
            List<GameDataTile> tempCollection = new List<GameDataTile>();
            tempCollection.Add(tilemap.GetTileDataByXY(currentTile.LocalPlace.x, currentTile.LocalPlace.y + 1));
            tempCollection.Add(tilemap.GetTileDataByXY(currentTile.LocalPlace.x, currentTile.LocalPlace.y - 1));
            tempCollection.Add(tilemap.GetTileDataByXY(currentTile.LocalPlace.x + 1, currentTile.LocalPlace.y));
            tempCollection.Add(tilemap.GetTileDataByXY(currentTile.LocalPlace.x - 1, currentTile.LocalPlace.y));

            List<GameDataTile> tempArea = new List<GameDataTile>();
            foreach (GameDataTile nextTile in tempCollection)
            {
                if (nextTile != null && !nextTile.Visited && !nextTile.Blocked)
                {
                    tempArea.Add(nextTile);
                }
            }

            foreach (GameDataTile nextTile in tempArea)
            {
                nextTile.CameFrom = currentTile;
            }

            return tempArea;
        }
    }
}
