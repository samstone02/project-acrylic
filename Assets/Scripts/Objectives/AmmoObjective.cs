using Unity.Netcode;
using UnityEngine;

public class AmmoObjective : BaseObjective
{
    protected override void OnCapture()
    {
        NetworkLog.LogInfoServer("Captured!");
    }

    protected override void StartObjective()
    {
        NetworkLog.LogInfoServer("Objective started!");
    }
}