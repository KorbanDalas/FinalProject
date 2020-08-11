using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LobbyButton : MonoBehaviour
{
    RoomInfo Info;

    public TMP_InputField NameField;

    public void Init(RoomInfo info)
    {
        Info = info;
        GetComponentInChildren<TextMeshProUGUI>().text = info.Name;
    }

    public void Join()
    {

        PhotonNetwork.LocalPlayer.NickName = NameField.text;
        PhotonNetwork.JoinRoom(Info.Name);

    }
}
