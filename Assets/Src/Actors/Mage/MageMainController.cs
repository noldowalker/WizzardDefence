using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameModels;
using Wizard.GameField;
using Wizard.Events;

public class MageMainController : MonoBehaviour, ISubscribable
{

    public GameFieldController tilemapForClicksClass;
    public GameObject defaultPtojectilePrefub;
    public float shootingSpeed = 10f;

    private MageAnimationController animationController;
    private Vector3 projectileLaunchingPoint;
    private State state;
    private EventSystem events;

    void Start()
    {
        EventSystem.Instance.SubscribeGameFieldEvent(EventTypes.GameFieldPointed.Click, HandleTileClick);
        tilemapForClicksClass.onTileClick += HandleTileClick;
        animationController = GetComponentInChildren<MageAnimationController>();
        projectileLaunchingPoint = transform.Find("LaunchingPoint").position;
        state = new State();
    }

    public void Unsubscribe() {
        EventSystem.Instance.UnsubscribeGameFieldEvent(EventTypes.GameFieldPointed.Click, HandleTileClick);
    }

    private void HandleTileClick(Vector3 clickPosition){
        if (state.IsIdle())
        {
            state.SetAttacking();
            float time = animationController.PlayAttackAnimation(shootingSpeed / 100f);
            StartCoroutine(shootProjectile(clickPosition, time));
        }
    }

    private IEnumerator shootProjectile(Vector3 clickPosition, float time) {
        yield return new WaitForSeconds(.66f * time);       
        DefaultProjectileController newProjectile = Instantiate(defaultPtojectilePrefub, projectileLaunchingPoint, transform.rotation).GetComponent<DefaultProjectileController>();
        newProjectile.target = clickPosition;
        yield return new WaitForSeconds(.25f * time);
        state.SetIdle();
    }
}
