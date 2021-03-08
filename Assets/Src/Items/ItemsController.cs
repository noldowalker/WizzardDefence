using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsController : MonoBehaviour
{
    protected BaseSceneFinder scene;
    protected GameObject gem;

    void Awake()
    {
        scene = GetComponentInParent<BaseSceneFinder>();
        // Assets/Resources/Prefubs/Treasures/Gem.prefab
        gem = Resources.Load<GameObject>("Prefubs/Treasures/Gem");
 
        Debug.Log("gem = " + gem.name);
    }

    public void CreateItem(BaseEnemyController dyingEnemy) {
        Debug.Log("Creating Items!");
        for (int i = 0; i < dyingEnemy.CurrentTreasures; i++) {
            scene.GetTreasureController().ReturnTreasure(1);
            Treasure currentGem = Instantiate(
                    gem,
                    new Vector3(
                        dyingEnemy.transform.position.x,
                        dyingEnemy.transform.position.y,
                        1
                    ),
                    Quaternion.identity
                ).GetComponent<Treasure>();

            Array.ForEach(
                currentGem.GetComponents<SpriteRenderer>(),
                (SpriteRenderer sr) => {
                    sr.sortingOrder = dyingEnemy.SortingOrder;
                }
            );
        };
    }
}
