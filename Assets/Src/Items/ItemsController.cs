using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wizard.Events;

public class ItemsController : MonoBehaviour, ISubscribable
{
    protected TreasuryController treasury;
    protected GameObject gem;

    void Awake()
    {
        gem = Resources.Load<GameObject>("Prefubs/Treasures/Gem");
    }

    void Start() {
        treasury = GetComponentInParent<BaseSceneFinder>().GetTreasureController();

        EventSystem.Instance.SubscribeActorBasedEvent(EventTypes.ActorBased.Destroy, CreateItem);
    }

    public void Unsubscribe() {
        EventSystem.Instance.UnsubscribeActorBasedEvent(EventTypes.ActorBased.Destroy, CreateItem);
    }

    void OnDestroy() {
        Unsubscribe();
    }

    public void CreateItem(BaseEnemyController dyingEnemy) {
        Debug.Log("Creating Items!");
        for (int i = 0; i < dyingEnemy.CurrentTreasures; i++) {
            treasury.ReturnTreasure(1);
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
