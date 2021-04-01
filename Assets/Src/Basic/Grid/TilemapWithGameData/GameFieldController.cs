using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Wizard.Events;
using Wizard.GameField.PathFinding;
using System.Linq;

namespace Wizard.GameField
{
    public class GameFieldController : MonoBehaviour
    {
        private Grid grid; // Грид к которому привязан данный тайлмап
        private Tilemap interfaceTilemap; // Сам тайлмап
                                          // Дополнительные данные к каждому тайлу, ключ - имя тайла по принципу х_у_interface_tile
        private Dictionary<string, GameDataTile> tilesData;
        // Список тайлов на которых могут появитсья монстры
        private List<GameDataTile> spawnTiles;
        // Список непроходимых тайлов
        private List<GameDataTile> blockedTiles;
        // Список свободных от меток тайлов
        private List<GameDataTile> otherTiles;
        // Список тайлов при занятости которых стена становится прозрачной
        private List<GameDataTile> transparentDetectionTiles;
        // Внешние клетки двери
        private List<GameDataTile> doorOutsideTiles;
        // Внутренние клетки двери
        private List<GameDataTile> doorInsideTiles;
        // Вход в башню где лежат сокровища
        private List<GameDataTile> houseInterierTiles;
        // Минимальное и максимальное значения сетки
        private int minX = 0, minY = 0, maxX = 0, maxY = 0, spawnX = 0, doorOutsideX = 0, doorInsideX = 0, houseInterierX = 0;
        // Экземпляр класса который осуществляет поиск пути по тайлмапу.
        private GameFieldPathFinder pathFinder;

        // Событие клика по тайлмапу.
        public Action<Vector3> onTileClick;
        // Событие смены статуса прозрачности
        public Action<bool> onTransparencyChange;
        // место последнего клика (не используется нуно было для отладки)
        private Vector3Int lastSelectedTilePos;
        // тайлы (сами плиточки) которые можно принудительно присваивать ячейкам (нужно было для отладки)
        public Tile notSelectedTile, selectedTile, tileInGrid, occupiedTile, blockedTile, monsterGenerationTile, targetTile, errorTile;
        // Объект Двери
        public DoorController door;
        // Флаг занята ли хоть одна из клеток которые влияют на прозрачность
        private bool transparency = false;

        [SerializeField]
        private bool hideGameField = true;

        // Запускается на старте
        void Awake()
        {
            grid = GetComponentInParent<Grid>();
            door = GetComponentInParent<DoorController>();
            interfaceTilemap = GetComponent<Tilemap>();
            lastSelectedTilePos = new Vector3Int(100, 100, 100);
            tilesData = new Dictionary<string, GameDataTile>();
            //ToDO: поменять название на что-то типа WalkableTiles
            otherTiles = new List<GameDataTile>();
            spawnTiles = new List<GameDataTile>();
            blockedTiles = new List<GameDataTile>();
            doorOutsideTiles = new List<GameDataTile>();
            doorInsideTiles = new List<GameDataTile>();
            houseInterierTiles = new List<GameDataTile>();
            transparentDetectionTiles = new List<GameDataTile>();

            CreateTileInfo();
            pathFinder = new GameFieldPathFinder(this);
            if (door != null)
            {
                door.OnDestroy += onDoorDestroyed;
            }
            HideTilemaps();
        }

        // Обработчик клика по колайдеру тайлмапа.
        public void OnMouseDown()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 worldPoint = ray.GetPoint(-ray.origin.z / ray.direction.z);
            Vector3Int position = grid.WorldToCell(worldPoint);

            if (position.x >= doorOutsideX)
                SendEvent(worldPoint);
        }

        // Меняет плитку тайла (отладочная функция)
        public void ChangeTileSprite(Vector3Int position, Tile tileToChange)
        {
            interfaceTilemap.SetTile(position, tileToChange);
        }

        // Активирует всех делегатов подписанных на событие клика по тайлу.
        private void SendEvent(Vector3 worldPoint)
        {
            EventSystem.Instance.FireGameFieldEvent(EventTypes.GameFieldPointed.Click, worldPoint);
        }

        // Скрывает спрайты тайлов, оставляя только их коллайдеры.
        private void HideTilemaps()
        {
            if (!hideGameField)
                return;

            GetComponent<TilemapRenderer>().enabled = false;
        }

        // *** Методы работы с данными сетки *** \\

        // Инициализация сетки, создание дополнительных данных для каждой ячейки.
        private void CreateTileInfo()
        {
            List<Vector3> availablePlaces = new List<Vector3>();
            foreach (Vector3Int position in interfaceTilemap.cellBounds.allPositionsWithin)
            {
                if (position.x < minX) minX = position.x;
                if (position.y < minY) minY = position.y;
                if (position.x > maxX) maxX = position.x;
                if (position.y > maxY) maxY = position.y;

                TileBase currentTile = interfaceTilemap.GetTile(position);
                if (currentTile != null)
                {
                    GameDataTile currentTileData = new GameDataTile();
                    currentTileData.LocalPlace = position;
                    currentTileData.TileBase = currentTile;
                    currentTileData.Name = "" + position.x + "_" + position.y + "_interface_tile";
                    currentTileData.CenterWorldPlace = interfaceTilemap.GetCellCenterWorld(position);
                    tilesData.Add(currentTileData.Name, currentTileData);

                    switch (currentTile.name)
                    {
                        case "tile_monster":
                            spawnTiles.Add(currentTileData);
                            otherTiles.Add(currentTileData);
                            spawnX = position.x;
                            break;
                        case "tile_bloked":
                            blockedTiles.Add(currentTileData);
                            otherTiles.Add(currentTileData);
                            currentTileData.Blocked = true;
                            break;
                        case "tile_door_outside":
                            doorOutsideTiles.Add(currentTileData);
                            otherTiles.Add(currentTileData);
                            doorOutsideX = position.x;
                            break;
                        case "tile_door_inside":
                            doorInsideTiles.Add(currentTileData);
                            otherTiles.Add(currentTileData);
                            doorInsideX = position.x;
                            break;
                        case "tile_house_interier":
                            houseInterierTiles.Add(currentTileData);
                            otherTiles.Add(currentTileData);
                            houseInterierX = position.x;
                            break;
                        default:
                            otherTiles.Add(currentTileData);
                            break;
                    }
                }
            }

            if (doorOutsideTiles != null) {
                GetTilesForTransparent();
            }
        }

        private void GetTilesForTransparent() {
            foreach (GameDataTile doorTile in doorOutsideTiles) {
                Vector3Int doorCoords = doorTile.LocalPlace;
                transparentDetectionTiles.Add(doorTile);

                int y = doorTile.LocalPlace.y + 1;
                GameDataTile currentTile = GetTileDataByPosition(new Vector3Int(doorCoords.x, y, doorCoords.z));

                while (currentTile != null)
                {
                    if (!transparentDetectionTiles.Contains(currentTile))
                        transparentDetectionTiles.Add(currentTile);
                    y++;
                    currentTile = GetTileDataByPosition(new Vector3Int(doorCoords.x, y, doorCoords.z));
                }

                y = doorTile.LocalPlace.y - 1;
                currentTile = GetTileDataByPosition(new Vector3Int(doorCoords.x, y, doorCoords.z));
                while (currentTile != null)
                {
                    if (!transparentDetectionTiles.Contains(currentTile))
                        transparentDetectionTiles.Add(currentTile);
                    y--;
                    currentTile = GetTileDataByPosition(new Vector3Int(doorCoords.x, y, doorCoords.z));
                }
            }
            
        }

        // Гетер для списка тайлов в виде списка.
        public List<GameDataTile> GetOtherTiles()
        {
            return otherTiles;
        }

        // Гетер для списка тайлов в виде справочника.
        public Dictionary<string, GameDataTile> GetTilesData()
        {
            return tilesData;
        }

        // Гетер для списка тайлов на которых могут появиться монстры
        public List<GameDataTile> GetSpawnTiles()
        {
            return spawnTiles;
        }

        // Гетер для целевого тайла, за которым дверь. Все монстры побегут сюда.
        public List<GameDataTile> GetDoorOutsideTiles()
        {
            return doorOutsideTiles;
        }


        // Проверяет разрушена ли дверь
        public bool IsDoorBroken()
        {
            return door == null;
        }

        // Проверяет не является ли переданный тайл соседним извне по отношению к двери.
        public bool IsDoorOutsideTile(GameDataTile tileForCheck)
        {
            return doorOutsideTiles.Contains(tileForCheck);
        }

        // Проверяет не является ли переданный тайл соседним изнутри по отношению к двери.
        public bool IsDoorInsideTile(GameDataTile tileForCheck)
        {
            return doorInsideTiles.Contains(tileForCheck);
        }

        // Проверяет не является ли переданный тайл входом в сокровищницу.
        public bool IsHouseInterierTile(GameDataTile tileForCheck)
        {
            return houseInterierTiles.Contains(tileForCheck);
        }

        // Получив мировые координаты, вернет позицию тайла.
        public Vector3Int GetTilePositionByWorldCoords(Vector3 position) { return grid.WorldToCell(position); }

        // Получив позицию тайла вернет его данные.
        public GameDataTile GetTileDataByPosition(Vector3Int position)
        {
            string key = "" + position.x + "_" + position.y + "_interface_tile";
            if (tilesData.ContainsKey(key))
            {
                return tilesData[key];
            }
            else
            {
                return null;
            }
        }

        // Получив x и y тайла вернет его данные.
        public GameDataTile GetTileDataByXY(int x, int y)
        {
            string key = "" + x + "_" + y + "_interface_tile";
            if (tilesData.ContainsKey(key))
            {
                return tilesData[key];
            }
            else
            {
                return null;
            }
        }

        // Помечает конкретный тайл как занятый.
        public void MarkTileOccupied(GameDataTile tile)
        {
            tilesData[tile.Name].Occupied = true;
            RepaintTileByItStatus(tile);
        }

        // Помечает конкретный тайл как свободный.
        public void MarkTileFree(GameDataTile tile)
        {
            tilesData[tile.Name].Occupied = false;
            RepaintTileByItStatus(tile);
        }

        // Помечает конкретный тайл как цель движения.
        public void MarkTileAsTarget(GameDataTile tile)
        {
            tilesData[tile.Name].Target = true;
            RepaintTileByItStatus(tile);
        }

        // Убиреат марку цели движения.
        public void UnmarkTileAsTarget(GameDataTile tile)
        {
            tilesData[tile.Name].Target = false;
            RepaintTileByItStatus(tile);
        }

        private void RepaintTileByItStatus(GameDataTile tile) {
            if (tile.IsBlocked() == true)
                return;

            if (tile.IsFree() && tile.IsNotTarget())
            {
                ChangeTileSprite(tile.LocalPlace, notSelectedTile);
            }
            else if (tile.IsOccupied() && tile.IsTarget())
            { // Это косяк
                ChangeTileSprite(tile.LocalPlace, errorTile);
            }
            else if (tile.IsOccupied())
            {
                ChangeTileSprite(tile.LocalPlace, occupiedTile);
            }
            else if (tile.IsTarget()) {
                ChangeTileSprite(tile.LocalPlace, targetTile);
            }

            
        }

        // Функция которая сбрасывает счет по тайлам.
        public void ResetTileCount()
        {
            foreach (KeyValuePair<string, GameDataTile> record in tilesData)
            {
                record.Value.count = 0;
            }
        }

        // Функция которая сбрасывает счет по тайлам.
        public void ResetTileVisited()
        {
            foreach (KeyValuePair<string, GameDataTile> record in tilesData)
            {
                record.Value.Visited = false;
                record.Value.CameFrom = null;
            }
        }

        // Перебирает набор тайлов стоящих рядом с дверью, выбирая из них ближайший не занятый.
        private GameDataTile GetClosestFreeTileInList(GameDataTile tileFrom, List<GameDataTile> tileList) {
            float distance = 0f;
            GameDataTile resultTile = null;
            foreach (GameDataTile doorTile in tileList) {
                float tempDistance = Vector3Int.Distance(doorTile.LocalPlace, tileFrom.LocalPlace);
                if (
                    (doorTile.IsFree())
                    && (distance == 0 || tempDistance < distance)
                ) {
                    distance = tempDistance;
                    resultTile = doorTile;
                }
            }
            return resultTile;
        }

        // Перебирает набор тайлов стоящих рядом с дверью, выбирая из них ближайший не занятый.
        private GameDataTile GetClosestAnyTileInList(GameDataTile tileFrom, List<GameDataTile> tileList)
        {
            float distance = 0f;
            GameDataTile resultTile = null;
            foreach (GameDataTile doorTile in tileList)
            {
                float tempDistance = Vector3Int.Distance(doorTile.LocalPlace, tileFrom.LocalPlace);
                if (distance == 0 || tempDistance < distance)               
                {
                    distance = tempDistance;
                    resultTile = doorTile;
                }
            }
            return resultTile;
        }


        // Гетер для любого свободного тайла из набора
        private GameDataTile GetAnyFreeTileFrom(List<GameDataTile> tileList)
        {
            List<GameDataTile> freeTiles = new List<GameDataTile>();
            foreach (GameDataTile tile in tileList)
            {
                if (tile.IsFree() && tile.IsNotBlocked() && tile.IsNotTarget()) {
                    freeTiles.Add(tile);
                }
            }

            if(freeTiles.Count > 0)
                return freeTiles[UnityEngine.Random.Range(0, freeTiles.Count-1)];
            else
                return null;
        }

        // Гетер для любого свободного тайла из ведущих внутрь здания
        public GameDataTile GetAnyFreeTileFromHouseInterierTiles() {
            return GetAnyFreeTileFrom(houseInterierTiles);
        }

        // Гетер для любого свободного тайла на которых генерируются монстры
        public GameDataTile GetAnyFreeTileForSpawn()
        {
            return GetAnyFreeTileFrom(spawnTiles);
        }

        public GameDataTile ToTheTreasure(GameDataTile tileFrom) {
            GameDataTile tileTo = null;

            switch (tileFrom.LocalPlace.x) {
                case int x when tileFrom.LocalPlace.x > this.doorOutsideX:
                    tileTo = FindPathTo(
                        tileFrom,
                        GetClosestFreeTileInList(tileFrom, doorOutsideTiles),
                        GetClosestAnyTileInList(tileFrom, doorOutsideTiles)
                    );
                    break;
                case int x when tileFrom.LocalPlace.x > this.doorInsideX:
                    tileTo = FindPathTo(
                        tileFrom,
                        GetClosestFreeTileInList(tileFrom, doorInsideTiles),
                        GetClosestAnyTileInList(tileFrom, doorInsideTiles)
                    );
                    break;
                case int x when tileFrom.LocalPlace.x > this.houseInterierX:
                    tileTo = FindPathTo(
                        tileFrom,
                        GetClosestFreeTileInList(tileFrom, houseInterierTiles),
                        GetClosestAnyTileInList(tileFrom, houseInterierTiles)
                    );
                    break;
                default:
                    break;
                ;
            }

            return tileTo;
        }

        public GameDataTile ToTheExit(GameDataTile tileFrom)
        {
            GameDataTile tileTo = null;

            switch (tileFrom.LocalPlace.x)
            {

                case int x when tileFrom.LocalPlace.x < this.doorInsideX:
                    tileTo = FindBackwardPathTo(
                        tileFrom,
                        GetClosestFreeTileInList(tileFrom, doorInsideTiles),
                        GetClosestAnyTileInList(tileFrom, doorInsideTiles)
                    );
                    break;
                case int x when tileFrom.LocalPlace.x < this.doorOutsideX:
                    tileTo = FindBackwardPathTo(
                        tileFrom,
                        GetClosestFreeTileInList(tileFrom, doorOutsideTiles),
                        GetClosestAnyTileInList(tileFrom, doorOutsideTiles)
                    );
                    break;
                case int x when tileFrom.LocalPlace.x < spawnX:
                    tileTo = FindBackwardPathTo(
                        tileFrom,
                        GetClosestFreeTileInList(tileFrom, spawnTiles),
                        GetClosestAnyTileInList(tileFrom, spawnTiles)
                    );
                    break;
                default:
                    break;
                    ;
            }

            return tileTo;
        }

        // Ищет путь ко входу 2мя разными способами. Один в обход занятых клеток, второй по кратчайшему без занятых.
        public GameDataTile FindPathTo(GameDataTile tileFrom, GameDataTile tileToFree, GameDataTile tileToAny)
        {
            
            if (tilesData.ContainsKey(tileFrom.Name) && doorOutsideTiles.Count != 0)
            {
                ResetTileCount();
                ResetTileVisited();

                GameDataTile nextTile = null;

                if (tileToFree != null)
                {
                    nextTile = checkPathForNexTile(pathFinder.StraitWithDiagonalSearchForTile(tileFrom, tileToFree));

                    if (nextTile != null)
                        return nextTile;
                }

                ResetTileCount();
                ResetTileVisited();

                nextTile = checkPathForNexTile(pathFinder.AroundWithDiagonalSearchForTile(tileFrom, tileToAny));

                return nextTile;
            }
            else
            {
                return null;
            }
        }

        // Ищет путь ко выходу 2мя разными способами. Один в обход занятых клеток, второй по кратчайшему без занятых.
        public GameDataTile FindBackwardPathTo(GameDataTile tileFrom, GameDataTile tileToFree, GameDataTile tileToAny)
        {

            if (tilesData.ContainsKey(tileFrom.Name) && doorOutsideTiles.Count != 0)
            {
                ResetTileCount();
                ResetTileVisited();

                GameDataTile nextTile = null;

                if (tileToFree != null)
                {
                    nextTile = checkPathForNexTile(pathFinder.BackwardStraitWithDiagonalSearchForTile(tileFrom, tileToFree));

                    if (nextTile != null)
                        return nextTile;
                }

                ResetTileCount();
                ResetTileVisited();

                nextTile = checkPathForNexTile(pathFinder.BackwardAroundWithDiagonalSearchForTile(tileFrom, tileToAny));

                return nextTile;
            }
            else
            {
                return null;
            }
        }

        // Проверяет можно ли двигаться к следующем на полученом пути тайлу, если нет возвращает нулл.
        private GameDataTile checkPathForNexTile(List<GameDataTile> path)
        {
            GameDataTile nextTile = (path != null && path[path.Count - 1] != null) ? path[path.Count - 1] : null;

            if (nextTile != null && nextTile.IsFree() && nextTile.IsNotTarget())
                return nextTile;

            return null;
        }

        // Проходит по списку тайлов и помечает их как незанятые
        public void MarkTilesAsFree(List<GameDataTile> freeTiles)
        {
            foreach (GameDataTile tile in freeTiles)
            {
                this.MarkTileFree(tile);
            }

        }

        // Проходит по списку тайлов и отмечает их как занятые, 
        // далее по всем тайлам из списка прочих проставляет что они не занятые.
        public void ChangeTilesOccupationAccorgingTileData(List<GameDataTile> occupiedTiles)
        {
            foreach (GameDataTile tile in occupiedTiles)
            {
                if (tile.IsFree())
                {
                    this.MarkTileOccupied(tile);
                    
                }
            }

            List<GameDataTile> freeTiles = this.GetOtherTiles().Except(occupiedTiles).ToList<GameDataTile>();
            this.MarkTilesAsFree(freeTiles);
            CheckTransparancy();
        }

        public void CheckTransparancy() {
            bool result = false;
            foreach (GameDataTile tile in transparentDetectionTiles) {
                result = tile.IsOccupied() || tile.IsTarget();
                if (result)
                    break;
            }

            if (result != transparency) {
                transparency = result;
                onTransparencyChange(transparency);
            }
        }

        private void onDoorDestroyed()
        {
            door = null;
        }

        // Вспомогательная функция для дебага. Рисует счет алгоритма по клеткам.
        /*void OnDrawGizmos()
        {
            if (tilesData != null)
                foreach (KeyValuePair<string, GameDataTile> record in tilesData)
                {
                    GameDataTile tile = record.Value;
                    UnityEditor.Handles.Label(tile.CenterWorldPlace, "" + tile.LocalPlace.x + "," + tile.LocalPlace.y);
                }
        }*/
    }
}