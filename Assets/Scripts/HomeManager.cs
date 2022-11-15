using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;


public enum _regionCodes
{
    AUTO,
    CAE,
    EU,
    US,
    USW,
    SA
}
public class HomeManager : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    [SerializeField] string _gameVersion = "1";
    [SerializeField] private string _regionCode = null;
    [SerializeField] private Text _txtBtnConnect;
    [SerializeField] private GameObject _btnConnect;
    [SerializeField] private GameObject _panelRoom;
    [SerializeField] private GameObject _panelConnect;
    [SerializeField] private Text _username;
    [SerializeField] private Text _playersCount;
    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void SetBtn(bool state, string msg)
    {
        _txtBtnConnect.text = msg;
        //_btnConnect.GetComponentInChildren<Button>().SetEnabled(state);
        _btnConnect.SetActive(state);
    }

    public void SetRegion(int index)
    {
        _regionCodes region = (_regionCodes) index;
        if (region == _regionCodes.AUTO)
        {
            _regionCode = null;
        }
        else
        {
            _regionCode = region.ToString();
        }
    }

    void ShowRoomPanel()
    {
        _panelConnect.SetActive(false);
        _panelRoom.SetActive(true);
        if (PhotonNetwork.NickName != null)
        {
            
            Debug.Log(PhotonNetwork.NickName);
            _username.text = (string)PhotonNetwork.NickName;
            _playersCount.text = PhotonNetwork.PlayerList[0].ToString() + "\n & \n " + PhotonNetwork.PlayerList[1].ToString();
        }
    }
    public void SetReady()
    {
        var propsToSet = new ExitGames.Client.Photon.Hashtable() {{"ready",true}};
        PhotonNetwork.LocalPlayer.SetCustomProperties(propsToSet);
    }
    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = _gameVersion;
        }

        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = _regionCode;

    }
    #region MonoBehaviourPunCallbacks Callbacks


    public override void OnConnectedToMaster()
    {
        Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
        SetBtn(true, "Buscar sala");
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        PhotonNetwork.CreateRoom(null, new RoomOptions());
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
        SetBtn(false, "Esperando Jugadores");

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            Debug.Log("Room is ready.");
            ShowRoomPanel();
            /*PhotonNetwork.LoadLevel("Game");*/
        }
        //PhotonNetwork.LoadLevel("Game");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " Se ha unido a la sala: " + PhotonNetwork.CurrentRoom.PlayerCount);
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            Debug.Log("Room is full");
            ShowRoomPanel();
            /*PhotonNetwork.LoadLevel("Game");*/
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changeProps)
    {

        if (changeProps.ContainsKey("ready"))
        {
            int playersReady = 0;
            foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
            {
                bool ready = (bool) player.CustomProperties["ready"];
                Debug.Log(player.NickName + "is ready? ... " + ready);
                if (ready)
                {
                    playersReady++;
                }

                if (playersReady == 2)
                {
                    PhotonNetwork.LoadLevel("BicycleStandardSetup");
                }
            }
            
        }
    }


    #endregion
}
