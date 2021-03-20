using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wizard.GameField;

namespace Wizard.GameField.PathFinding
{
    public interface IFindPathStrategy
    {
        List<GameDataTile> GetNeigboursForTile(GameDataTile currentTile, GameDataTile endTile, GameFieldController tilemap);
    }
}