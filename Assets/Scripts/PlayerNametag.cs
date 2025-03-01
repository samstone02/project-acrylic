using TMPro;
using Unity.Collections;
using Unity.Netcode;

public class PlayerNametag : NetworkBehaviour
{
    public FixedString32Bytes PlayerName
    {
        get => PlayerDisplayNameNetVar.Value;
        set => PlayerDisplayNameNetVar.Value = value;
    }

    private readonly NetworkVariable<FixedString32Bytes> PlayerDisplayNameNetVar = new NetworkVariable<FixedString32Bytes>("Default");

    public override void OnNetworkSpawn()
    {
        if (!IsClient)
        {
            return;
        }

        var nametagText = GetComponent<TextMeshPro>();
        PlayerDisplayNameNetVar.OnValueChanged += (_, nextValue) =>
        {
            nametagText.text = nextValue.ToString();
        };
        nametagText.text = PlayerName.ToString();
    }
}
