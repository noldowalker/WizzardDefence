using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameModels;

public class MageMainController : MonoBehaviour
{

    public InterfaceTilemapController tilemapForClicksClass;
    public GameObject defaultPtojectilePrefub;
    public float shootingSpeed = 10f;

    private MageAnimationController animationController;
    private Vector3 projectileLaunchingPoint;
    private State state;

    void Awake()
    {
        tilemapForClicksClass.onTileClick += HandleTileClick;
        animationController = GetComponentInChildren<MageAnimationController>();
        projectileLaunchingPoint = transform.Find("LaunchingPoint").position;
        state = new State();
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
