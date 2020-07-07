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

        model = new DummyModel(5f);
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
        Destroy(gameObject);
    }

    private void MoveStep()
    {
        if (transform.position != targetForMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetForMoving, model.getSpeed());
        } else {
            model.state.SetIdle();
            Debug.Log("End Moving");
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
        Debug.Log("Move to :" + positionTo);
        Debug.Log("Current position :" + transform.position);
        Debug.Log("Start Moving");
    }

    public bool isMoving() {
        return model.state.IsMoving();
    }

    public bool isIdle()
    {
        return model.state.IsIdle();
    }
}
