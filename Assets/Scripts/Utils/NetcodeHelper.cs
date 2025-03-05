using Unity.Netcode;

public static class NetcodeHelper
{
    public static Tank GetLocalClientTankOrNull()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening && NetworkManager.Singleton.IsClient)
        {
            return NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Tank>();
        }

        return null;
    }
}
