using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class BuildingButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Building building = null;
    [SerializeField] private Image iconImage = null;
    [SerializeField] private TMP_Text priceText = null;
    [SerializeField] private LayerMask floorMask = new LayerMask();

    private Camera mainCamera;
    private RTSPlayer player;
    private GameObject buildingPreviewInstance;
    //private List<Renderer> buildingRendererInstances = new List<Renderer>();
    private Renderer[] buildingRendererInstances;
    private BoxCollider buildingCollider;

    private void Start(){
        mainCamera = Camera.main;
        
        iconImage.sprite = building.GetIcon();
        priceText.text = building.GetPrice().ToString();

        player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();

        buildingCollider = building.GetComponent<BoxCollider>();
    }    
    
    private void Update()
    {
        if(buildingPreviewInstance == null){
            return;
        }

        UpdateBuildingPreview();
    }

    public void OnPointerDown(PointerEventData eventData){
        if(eventData.button != PointerEventData.InputButton.Left){
            return;
        }

        if(player.GetResources() < building.GetPrice()){
            return;
        }

        buildingPreviewInstance = Instantiate(building.GetBuildingPreview());
        //buildingRendererInstance = buildingPreviewInstance.GetComponentInChildren<Renderer>();

        buildingRendererInstances = buildingPreviewInstance.GetComponentsInChildren<Renderer>();

        buildingPreviewInstance.SetActive(false);
    }

    public void OnPointerUp(PointerEventData eventData){
        if(buildingPreviewInstance == null){
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask)){
            player.CmdTryPlaceBuilding(building.GetId(), hit.point);
        }

        Destroy(buildingPreviewInstance);
    }

    private void UpdateBuildingPreview()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if(!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask)){
            return;
        }

        buildingPreviewInstance.transform.position = hit.point;

        if(!buildingPreviewInstance.activeSelf){
            buildingPreviewInstance.SetActive(true);
        }     

        Color color = player.CanPlaceBuilding(buildingCollider, hit.point) ? Color.green : Color.red; 

        //buildingRendererInstance.material.SetColor("_BaseColor", color);

        foreach(Renderer renderer in buildingRendererInstances){
            renderer.material.SetColor("_BaseColor", color);
        }
    }
}
