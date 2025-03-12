using System.Linq;
using TankGuns;
using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class AmmoCounterTallyUi : MonoBehaviour
{
    [field: SerializeField] public GameObject AmmoTallyPrefab;

    private List<GameObject> _tallies = new List<GameObject>();

    private AutoLoadingCannon _autoLoadingCannon;

    void Start()
    {
        var localPlayerCannon = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponentInChildren<BaseCannon>();

        if (localPlayerCannon is AutoLoadingCannon autoLoadingCannon)
        {
            _autoLoadingCannon = autoLoadingCannon;

            for (int i = 0; i < autoLoadingCannon.MagazineCapacity; i++)
            {
                var instance = Instantiate(AmmoTallyPrefab);
                instance.SetActive(false);
                instance.transform.SetParent(transform, false);

                _tallies.Add(instance);
            }

            autoLoadingCannon.FireClientEvent.AddListener(AutoLoadingCannon_FireClientEvent);
            autoLoadingCannon.ReloadStartEvent.AddListener(AutoLoadingCannon_ReloadStartEvent);
            autoLoadingCannon.ReloadEndEvent.AddListener(AutoLoadingCannon_ReloadEndEvent);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void AutoLoadingCannon_FireClientEvent()
    {
        for (int i = 0; i < _autoLoadingCannon.MagazineCapacity; i++)
        {
            _tallies[i].gameObject.SetActive(i < _autoLoadingCannon.MagazineCount - 1);
        }
    }

    private void AutoLoadingCannon_ReloadStartEvent()
    {
        foreach (var tally in _tallies)
        {
            tally.gameObject.SetActive(false);
        }
    }

    private void AutoLoadingCannon_ReloadEndEvent()
    {
        foreach (var tally in _tallies)
        {
            tally.gameObject.SetActive(true);
        }
    }
}
