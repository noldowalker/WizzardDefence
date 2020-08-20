using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridTools.TilemapWithGameData;

namespace GridTools.PathFinding
{
    /*
     *  Ищет по тайлмапу под это приспособленному путь от клетки до клетки 
     *  С помощью модифицированного волнового алгоритма. 
     * 
     * */
    public class GameDataTilemapPathFinder
    {
        private BaseGameDataTilemapController tilemap;
        private Dictionary<string, IFindPathStrategy> strategies = new Dictionary<string, IFindPathStrategy>
        {
            {"ClearPathFinding", new ClearFindPathStrategy()}
        };

        // Входной параметр - тайлмап соответствующего класса, по которому будет происходить поиск.
        public GameDataTilemapPathFinder(BaseGameDataTilemapController initTilemap)
        {
            tilemap = initTilemap;
        }

        public List<GameDataTile> ClearSearchForTile(GameDataTile startTile, GameDataTile endTile) {
            return SearchForTile(startTile, endTile, strategies["ClearPathFinding"]);
        }

        //На вход подаются 2 параметра - откуда и до куда искать путь.
        private List<GameDataTile> SearchForTile(GameDataTile startTile, GameDataTile endTile, IFindPathStrategy currentStrategy)
        {
            List<GameDataTile> result = new List<GameDataTile>();
            List<GameDataTile> currentArea = new List<GameDataTile>();

            startTile.Visited = true;
            currentArea.Add(startTile);
            int visitCounter = 0;
            bool pathFounded = false;

            while (!pathFounded)
            {
                List<GameDataTile> nextArea = new List<GameDataTile>();
                foreach (GameDataTile currentTile in currentArea)
                {
                    List<GameDataTile> tempArea = currentStrategy.GetNeigboursForTile(currentTile, tilemap);

                    foreach (GameDataTile nextTile in tempArea)
                    {
                        nextTile.Visited = true;
                        nextTile.count = visitCounter += 1;
                        if (nextTile.Name == endTile.Name)
                        {
                            pathFounded = true;
                            result.Add(nextTile);
                            GameDataTile previousTile = nextTile.CameFrom;
                            while (previousTile != startTile)
                            {
                                result.Add(previousTile);
                                previousTile = previousTile.CameFrom;
                            }
                        }
                        else
                        {
                            nextArea.Add(nextTile);
                        }
                    }
                }

                if (nextArea.Count > 0)
                {
                    currentArea = nextArea;
                }
                else
                {
                    return null;
                }
            }

            return result;
        }

        private List<GameDataTile> GetNeigboursForTile(GameDataTile currentTile)
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