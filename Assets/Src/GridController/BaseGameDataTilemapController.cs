using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using System.Linq;

public class BaseGameDataTilemapController : MonoBehaviour
{
    private Grid grid; // Грид к которому привязан данный тайлмап
    private Tilemap interfaceTilemap; // Сам тайлмап
    // Дополнительные данные к каждому тайлу, ключ - имя тайла по принципу х_у_interface_tile
    private Dictionary<string, GameDataTile> tilesData;
    // Список тайлов на которых могут появитсья монстры
    private List<GameDataTile> spawnTiles;
    // Место куда устремятся монстры
    private GameDataTile doorTile;
    // Минимальное и максимальное значения сетки
    private int minX = 0, minY = 0, maxX = 0, maxY = 0;
    // Экземпляр класса который осуществляет поиск пути по тайлмапу.
    private GameDataTilemapPathFinder pathFinder;

    private int safeIterator = 0;
    // Событие клика по тайлмапу.
    public Action<Vector3> onTileClick;
    // место последнего клика (не используется нуно было для отладки)
    private Vector3Int lastSelectedTilePos;
    // тайлы (сами плиточки) которые можно принудительно присваивать ячейкам (нужно было для отладки)
    public Tile notSelectedTile, selectedTile, tileInGrid;

    // Запускается на старте
    void Awake()
    {
        grid = GetComponentInParent<Grid>();
        interfaceTilemap = GetComponent<Tilemap>();
        lastSelectedTilePos = new Vector3Int(100, 100, 100);
        tilesData = new Dictionary<string, GameDataTile>();
        spawnTiles = new List<GameDataTile>();
        CreateTileInfo();
        pathFinder = new GameDataTilemapPathFinder(this);
        // HideTilemaps();
    }

    // Обработчик клика по колайдеру тайлмапа.
    public void OnMouseDown()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 worldPoint = ray.GetPoint(-ray.origin.z / ray.direction.z);
        Vector3Int position = grid.WorldToCell(worldPoint);

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
        if (onTileClick != null)
        {
            onTileClick(worldPoint);
        }
    }

    // Скрывает спрайты тайлов, оставляя только их коллайдеры.
    private void HideTilemaps()
    {
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
                        currentTileData.Occupied = true;
                        break;
                    case "tile_move_to":
                        doorTile = currentTileData;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    // Вспомогательная функция для дебага. Рисует счет алгоритма по клеткам.
    void OnDrawGizmos()
    {
        if (tilesData != null)
            foreach (KeyValuePair<string, GameDataTile> record in tilesData)
            {
                GameDataTile tile = record.Value;
                Handles.Label(tile.CenterWorldPlace, tile.count.ToString());
            }
    }

    // Гетер для списка тайлов.
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
    public GameDataTile GetDoorTile()
    {
        return doorTile;
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
    }

    // Помечает конкретный тайл как занятый.
    public void DrawTileVisited(GameDataTile tile)
    {
        ChangeTileSprite(tile.LocalPlace, selectedTile);
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

    // Ищет путь ко входу
    public List<GameDataTile> FindPathToEntrance(GameDataTile tileFrom)
    {
        if (tilesData.ContainsKey(tileFrom.Name) && doorTile != null)
        {
            ResetTileCount();
            ResetTileVisited();

            List<GameDataTile> result = pathFinder.SearchForTile(tileFrom, doorTile);

            if (result != null)
            {
                foreach (GameDataTile record in result)
                {
                    ChangeTileSprite(record.LocalPlace, tileInGrid);
                }
            }           

            return result;
        } else {
            return null;
        }
    }    
}
