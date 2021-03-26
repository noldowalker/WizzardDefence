using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wizard.Log
{
    public class LogController : MonoBehaviour
    {
        public static bool mute;

        public enum Errors {
            BaseSceneFinderIsNull,
            GameFieldMediatorIsNull,
            NoHitRegistratorInDummy,
            NoSpriteControllerInDummy
        };

        public static void ShowError(Errors reason)
        {
            if (mute == false) {
                switch (reason) {
                    case Errors.BaseSceneFinderIsNull:
                        Debug.Log("You forgot add BaseSceneFinder in Root Node");
                        break;
                    case Errors.GameFieldMediatorIsNull:
                        Debug.Log("You forgot add GameFieldMediator in GameField Node");
                        break;
                    case Errors.NoHitRegistratorInDummy:
                        Debug.Log("You forgot add Hit Registrator in Dummy's childen sprite");
                        break;
                    case Errors.NoSpriteControllerInDummy:
                        Debug.Log("You forgot add Sprite Controller in Dummy's childen sprite");
                        break;
                }
            }
        }
    }
}
