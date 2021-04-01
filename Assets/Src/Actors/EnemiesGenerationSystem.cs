using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wizard.Events;

public class EnemiesGenerationSystem : MonoBehaviour
{
    [SerializeField]
    private List<EnemyContainerForGeneration> enemiesPool;

    private int totalEnemies = 0;

    void Awake()
    {
        if (enemiesPool == null)
            enemiesPool = new List<EnemyContainerForGeneration>();

        foreach (EnemyContainerForGeneration container in enemiesPool)
        {
            totalEnemies += container.Amount;
        }
    }

    public void StartEnemiesGeneration() {
        StartCoroutine(EnemiesGeneration());
    }

    IEnumerator EnemiesGeneration() {
        while (totalEnemies > 0)
        {
            yield return new WaitForSeconds(GetRandomGenerationTime());

            GameObject newEnemy = Instantiate(enemiesPool[0].Prefub);
            newEnemy.name = newEnemy.name + " #" + totalEnemies;

            BaseEnemyController enemyEntity = newEnemy.GetComponent<BaseEnemyController>();
            if (enemyEntity != null) {
                EventSystem.Instance.FireActorBasedEvent(EventTypes.ActorBased.Created, enemyEntity);
            } else {
                Destroy(newEnemy);
            }

            totalEnemies--;
        }

        yield return null;
    }

    private float GetRandomGenerationTime() {
        return Random.Range(0.1f, 1.3f);
    }

    void Update() { }
}
