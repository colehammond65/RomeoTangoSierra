using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using System;

public class ResourcesDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text resourcesText = null;

    private RTSPlayer player;

    private void Start() {
        player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
    }

    private void Update(){
        if(player != null){
            ClientOnResourcesUpdated(player.GetResources());
            player.ClientOnResourcesUpdated += ClientOnResourcesUpdated;
        }
    }

    private void OnDestroy() {
        player.ClientOnResourcesUpdated -= ClientOnResourcesUpdated;
    }

    private void ClientOnResourcesUpdated(int resources)
    {
        resourcesText.text = $"Resources: {resources}";
    }
}
