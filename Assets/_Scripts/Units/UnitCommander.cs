﻿using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommander : MonoBehaviour
{
    
    [SerializeField] private UnitSelectionHandler unitSelectionHandler = null;
    [SerializeField] private LayerMask layerMask = new LayerMask();

    private Camera mainCamera;
    private void Start(){
        mainCamera = Camera.main;

        GameOverHandler.ClientOnGameOver += ClientOnGameOver;
    }

    private void Update(){
        if(!Mouse.current.rightButton.wasPressedThisFrame){
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if(!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)){
            return;
        }

        if(hit.collider.TryGetComponent<Targetable>(out Targetable target)){
            if(target.hasAuthority){
                TryMove(hit.point);
                return;
            }
            TryTarget(target);
            return;
        }

        TryMove(hit.point);
    }

    private void OnDestroy(){
        GameOverHandler.ClientOnGameOver -= ClientOnGameOver;
    }

    private void TryMove(Vector3 point)
    {
        foreach(Unit unit in unitSelectionHandler.SelectedUnits){
            unit.GetUnitMovement().CmdMove(point);
        }
    }

    private void TryTarget(Targetable target)
    {
        foreach(Unit unit in unitSelectionHandler.SelectedUnits){
            unit.GetTargeter().CmdSetTarget(target.gameObject);
        }
    }

    private void ClientOnGameOver(string winnerName){
        enabled = false;
    }
}
