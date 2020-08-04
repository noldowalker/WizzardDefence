using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameModels;

public class DummyController : MonoBehaviour
{
    private HitRegistrator hitRegitrator;
    private DummySpriteController dummySpriteController;
    private DummyAnimationController dummyAnimationController;
    private DummyModel model;
    private Vector3 targetForMoving;
    public Action<Vector3, Vector3> onEnemyDestroy;
    public Action<Vector3> onMoveEnded;

    void Awake()
    {
        hitRegitrator = GetComponentInChildren<HitRegistrator>();
        if (hitRegitrator != null) {
            hitRegitrator.hitListeners += onGetingHit;
        } else {
            LogController.ShowError(LogController.Errors.NoHitRegistratorInDummy);
        }

        dummySpriteController = GetComponentInChildren<DummySpriteController>();
        dummyAnimationController = GetComponent<DummyAnimationController>();

        model = new DummyModel(1f);
    }

    void Update()
    {
        if (model.state.IsMoving()) {
            MoveStep();
        }
    }

    private void onGetingHit(GameObject hitedObject) {
        if (hitedObject != null && hitedObject.tag != "ProjectileTag") {
            return;
        }

        if (dummySpriteController != null) {
            dummySpriteController.makeRed();
        } else {
            LogController.ShowError(LogController.Errors.NoSpriteControllerInDummy);
        }

        model.inflictDamage(1f);
        if (model.getCurrentHitPoints() > 0) {
            dummyAnimationController.PlayHitedAnimation();
        } else {
            float deathTime = dummyAnimationController.PlayDeathAnimation();
            StartCoroutine(DeleteThis(deathTime));
        }
    }

    private IEnumerator DeleteThis(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        SendDeathEvent();
        Destroy(gameObject);        
    }

    private void MoveStep()
    {
        if (transform.position != targetForMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetForMoving, model.getSpeed());
        } else {
            SendEndMoveEvent();
            model.state.SetIdle();
        }
        
    }

    public void AlignToCoords(Vector3 position)
    {
        gameObject.transform.position = new Vector3(position.x, position.y, gameObject.transform.position.z);
    }

    public void Move(Vector3 positionTo)
    {
        model.state.SetMoving();
        targetForMoving = new Vector3 (positionTo.x, positionTo.y, transform.position.z);
    }

    public bool isMoving() {
        return model.state.IsMoving();
    }

    public bool isIdle()
    {
        return model.state.IsIdle();
    }

    // Активирует всех делегатов подписанных на событие уничтожения противника.
    private void SendDeathEvent()
    {
        if (onEnemyDestroy != null)
        {
            onEnemyDestroy(this.gameObject.transform.position, this.targetForMoving);
        }
    }

    // Активирует всех делегатов подписанных на событие уничтожения противника.
    private void SendEndMoveEvent()
    {
        if (onMoveEnded != null)
        {
            onMoveEnded(this.targetForMoving);
        }
    }
}
