using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridTools.TilemapWithGameData;

namespace GridTools.PathFinding
{
    public interface IFindPathStrategy
    {
        List<GameDataTile> GetNeigboursForTile(GameDataTile currentTile, GameDataTile endTile, BaseGameDataTilemapController tilemap);
    }
}