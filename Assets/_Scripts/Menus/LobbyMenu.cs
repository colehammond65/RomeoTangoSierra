using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject LobbyUI = null;
    [SerializeField] private GameObject JoinButton = null;
    [SerializeField] private Button StartGameButton = null;
    [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[4];

    private void Start(){
        RTSNetworkManager.ClientOnConnected += HandleClientConnected;
        RTSPlayer.AuthorityOnPartyOwnerStateUpdated += AuthorityHandlePartyOwnerStateUpdated;
        RTSPlayer.ClientOnInfoUpdated += ClientHandleInfoUpdated;
    }

    private void OnDestroy() {
        RTSNetworkManager.ClientOnConnected -= HandleClientConnected;
        RTSPlayer.AuthorityOnPartyOwnerStateUpdated -= AuthorityHandlePartyOwnerStateUpdated;
        RTSPlayer.ClientOnInfoUpdated -= ClientHandleInfoUpdated;
    }

    public void HandleClientConnected(){
        LobbyUI.SetActive(true);
        JoinButton.SetActive(false);
    }

    public void ClientHandleInfoUpdated(){
        List<RTSPlayer> players = ((RTSNetworkManager)NetworkManager.singleton).Players;

        for(int i = 0; i < players.Count; i++){
            playerNameTexts[i].text = players[i].GetDisplayName();
        }

        for(int i = players.Count; i < playerNameTexts.Length; i++){
            playerNameTexts[i].text = "Waiting for player...";
        }

        StartGameButton.interactable = players.Count > 1;
    }

    private void AuthorityHandlePartyOwnerStateUpdated(bool state){
        StartGameButton.gameObject.SetActive(state);
    }

    public void StartGame(){
        NetworkClient.connection.identity.GetComponent<RTSPlayer>().CmdStartGame();
    }

    public void LeaveLobby(){
        if(NetworkServer.active && NetworkClient.isConnected){
            NetworkManager.singleton.StopHost();
        }
        else{
            NetworkManager.singleton.StopClient();

            SceneManager.LoadScene(0);
        }
    }
}
