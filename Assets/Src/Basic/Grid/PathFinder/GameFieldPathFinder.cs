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
    public class GameFieldPathFinder
    {
        private GameFieldController tilemap;
        private Dictionary<string, IFindPathStrategy> strategies = new Dictionary<string, IFindPathStrategy>
        {
            {"StraitWithDiagonalFindPathStrategy", new StraitWithDiagonalFindPathStrategy()},
            {"AroundWithDiagonalFindPathStrategy", new AroundWithDiagonalFindPathStrategy()},
            {"BackwardStraitWithDiagonalFindPathStrategy", new BackwardStraitWithDiagonalFindPathStrategy()},
            {"BackwardAroundWithDiagonalFindPathStrategy", new BackwardAroundWithDiagonalFindPathStrategy()}
        };

        // Входной параметр - тайлмап соответствующего класса, по которому будет происходить поиск.
        public GameFieldPathFinder(GameFieldController initTilemap)
        {
            tilemap = initTilemap;
        }

        // Поиск стратегией котоаря будет строить путь к сокровищу игнорируя занятые другими противниками клетки.
        public List<GameDataTile> AroundWithDiagonalSearchForTile(GameDataTile startTile, GameDataTile endTile)
        {
            return BaseAlhoritm(startTile, endTile, strategies["AroundWithDiagonalFindPathStrategy"]); ;
        }

        // Поиск стратегией котоаря будет строить путь  к сокровищу игнорируя занятые другими противниками клетки.
        public List<GameDataTile> StraitWithDiagonalSearchForTile(GameDataTile startTile, GameDataTile endTile)
        {
            return BaseAlhoritm(startTile, endTile, strategies["StraitWithDiagonalFindPathStrategy"]); ;
        }

        // Поиск стратегией котоаря будет строить путь к выходу с карты игнорируя занятые другими противниками клетки.
        public List<GameDataTile> BackwardAroundWithDiagonalSearchForTile(GameDataTile startTile, GameDataTile endTile)
        {
            return BaseAlhoritm(startTile, endTile, strategies["BackwardAroundWithDiagonalFindPathStrategy"]); ;
        }

        // Поиск стратегией котоаря будет строить путь выходу с карты игнорируя занятые другими противниками клетки.
        public List<GameDataTile> BackwardStraitWithDiagonalSearchForTile(GameDataTile startTile, GameDataTile endTile)
        {
            return BaseAlhoritm(startTile, endTile, strategies["BackwardStraitWithDiagonalFindPathStrategy"]); ;
        }

        //На вход подаются 3 параметра - откуда и до куда искать путь, а так же стратегия поиска.
        private List<GameDataTile> BaseAlhoritm(GameDataTile startTile, GameDataTile endTile, IFindPathStrategy currentStrategy)
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
                    List<GameDataTile> tempArea = currentStrategy.GetNeigboursForTile(currentTile, endTile, tilemap);

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
    }
}