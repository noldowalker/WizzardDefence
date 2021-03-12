using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GridTools.TilemapWithGameData
{
    public class GameGridController : MonoBehaviour
    {
        private GameFieldController gameTilemap;
        private WallsTilemapController wallsTilemap;
        private RoomTilemapController roomTilemap;
        private DoorController door;


        void Awake()
        {
            gameTilemap = GetComponentInChildren<GameFieldController>();
            wallsTilemap = GetComponentInChildren<WallsTilemapController>();
            roomTilemap = GetComponentInChildren<RoomTilemapController>();
            door = GetComponent<DoorController>();
            if (door != null)
            {
                door.onDestroy += roomTilemap.DestroyDoor;
            }
            gameTilemap.onTransparencyChange += wallsTilemap.changeTransparensyOn;

            BaseSceneFinder finder = GetComponentInParent<BaseSceneFinder>();
            InterfaceController ui = finder.GetInterfaceController();
        }

        void Update()
        {

        }

        public DoorController GetDoorController() {
            return door;
        }

        public void SetAction(Action<String> function)
        {
            door.onHit += function;
        }
    }
}
