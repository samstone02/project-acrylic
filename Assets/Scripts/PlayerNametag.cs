using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerNametag : MonoBehaviour
{
    private Tank _playerTank;
    private TextMeshPro _nametag2;

    private void OnEnable()
    {
        if (!NetworkManager.Singleton.IsClient)
        {
            return;
        }

        NetworkLog.LogInfoServer("HELLO!");

        _playerTank ??= GetComponentInParent<Tank>();
        _nametag2 ??= GetComponent<TextMeshPro>();

        if (_nametag2 == null) return;

        _nametag2.text = _playerTank.PlayerName.ToString();
    }
}
