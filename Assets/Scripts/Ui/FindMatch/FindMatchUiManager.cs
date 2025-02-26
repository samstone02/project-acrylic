using System;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FindMatchUiManager : MonoBehaviour
{
    public class HostSessionData
    {
        // TODO: player name here?
        public ushort PortNumber;
    }

    public class JoinSessionData
    {
        // TODO: player name here?
        public string Ipv4Address;
        public ushort PortNumber;
    }

    public event Action<HostSessionData> HostSessionEvent;
    public event Action<JoinSessionData> JoinSessionEvent;

    private TMP_Text displayNameInputText;
    private TMP_Text addressInputText;

    private void Awake()
    {
        displayNameInputText = GameObject.Find("DisplayNameInputField").transform.Find("Text Area").Find("Text").GetComponentInChildren<TMP_Text>();
        addressInputText = GameObject.Find("IpAddressInputField").transform.Find("Text Area").Find("Text").GetComponentInChildren<TMP_Text>();

        var hostSessionButton = GameObject.Find("HostSessionButton").GetComponent<Button>();
        var joinSessionButton = GameObject.Find("JoinSessionButton").GetComponent<Button>();
        hostSessionButton.onClick.AddListener(HandleClick_HostSession);
        joinSessionButton.onClick.AddListener(HandleClick_JoinSession);
    }

    public FixedString32Bytes GetPlayerName()
    {
        var playerName = displayNameInputText.text;
        return new FixedString32Bytes(playerName);
    }

    private void HandleClick_HostSession()
    {
        HostSessionData data = new HostSessionData
        {
            PortNumber = 7777
        };
        HostSessionEvent?.Invoke(data);
    }

    private void HandleClick_JoinSession()
    {
        JoinSessionData data = new JoinSessionData
        {
            Ipv4Address = addressInputText.text.Substring(0, addressInputText.text.Length - 1),
            PortNumber = 7777
        };
        JoinSessionEvent?.Invoke(data);
    }
}