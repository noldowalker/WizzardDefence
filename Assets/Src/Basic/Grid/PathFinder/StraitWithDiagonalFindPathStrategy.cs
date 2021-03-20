using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wizard.GameField;

namespace Wizard.GameField.PathFinding
{
    public class StraitWithDiagonalFindPathStrategy : IFindPathStrategy
    {
        public List<GameDataTile> GetNeigboursForTile(GameDataTile currentTile, GameDataTile endTile, GameFieldController tilemap)
        {
            List<GameDataTile> tempCollection = createTileCheckSequence(currentTile, tilemap);

            List<GameDataTile> tempArea = new List<GameDataTile>();
            foreach (GameDataTile nextTile in tempCollection)
            {
                if (nextTile != null && !nextTile.Visited && !nextTile.Blocked && (!nextTile.Occupied || nextTile == endTile))
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

        private List<GameDataTile> createTileCheckSequence(GameDataTile currentTile, GameFieldController tilemap) {
            List<GameDataTile> tempCollection = new List<GameDataTile>();

            GameDataTile forward = tilemap.GetTileDataByXY(currentTile.LocalPlace.x - 1, currentTile.LocalPlace.y);
            GameDataTile forwardLeft = tilemap.GetTileDataByXY(currentTile.LocalPlace.x - 1, currentTile.LocalPlace.y - 1);
            GameDataTile forwardRight = tilemap.GetTileDataByXY(currentTile.LocalPlace.x - 1, currentTile.LocalPlace.y + 1);
            GameDataTile Right = tilemap.GetTileDataByXY(currentTile.LocalPlace.x, currentTile.LocalPlace.y + 1);
            GameDataTile Left = tilemap.GetTileDataByXY(currentTile.LocalPlace.x, currentTile.LocalPlace.y + 1);
            GameDataTile backwardLeft = tilemap.GetTileDataByXY(currentTile.LocalPlace.x + 1, currentTile.LocalPlace.y - 1);
            GameDataTile backwardRight = tilemap.GetTileDataByXY(currentTile.LocalPlace.x + 1, currentTile.LocalPlace.y + 1);
            GameDataTile backward = tilemap.GetTileDataByXY(currentTile.LocalPlace.x + 1, currentTile.LocalPlace.y);

            tempCollection.Add(forward);

            bool isGoingLeft = (Random.Range(0, 2) == 0);
            if (isGoingLeft)
            {
                tempCollection.Add(forwardLeft);
                tempCollection.Add(forwardRight);
                tempCollection.Add(Left);
                tempCollection.Add(Right);
                tempCollection.Add(backwardLeft);
                tempCollection.Add(backwardRight);
            } else {
                tempCollection.Add(forwardRight);
                tempCollection.Add(forwardLeft);
                tempCollection.Add(Right);
                tempCollection.Add(Left);
                tempCollection.Add(backwardRight);
                tempCollection.Add(backwardLeft);
            }

            tempCollection.Add(backward);

            return tempCollection;
        }
    }
}
