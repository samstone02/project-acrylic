using System.Collections.Generic;
using UnityEngine;

public abstract class BaseObjectiveType : MonoBehaviour
{
    public abstract void OnStart();

    public abstract void OnCapture(IEnumerable<ulong> teamMembersClientIds);
}