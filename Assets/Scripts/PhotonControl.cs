using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;


public class PhotonControl : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    [SerializeField] Text Nombre;
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private Transform _player1Spawn;
    [SerializeField] private Transform _player2Spawn;
    
    void Start()
    {
        if (_playerPrefab == null)
        {
            Debug.Log("Falta la referencia la player pref");
        }
        else
        {
            
            Transform spawnPoint = (PhotonNetwork.IsMasterClient) ? _player1Spawn : _player2Spawn;
            object[] initData = new object[1];
            initData[0] = "Data instaciation";
            PhotonNetwork.Instantiate(_playerPrefab.name, spawnPoint.position, Quaternion.identity, 0, initData);    
        }

        ShowNickname();
    }

    void ShowNickname()
    {
        if (PhotonNetwork.NickName != null)
        {

            Debug.Log(PhotonNetwork.NickName);
            Nombre.text = (string)PhotonNetwork.NickName;
        }
    }
}
