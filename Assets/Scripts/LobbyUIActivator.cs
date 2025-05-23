using UnityEngine;

public class LobbyUIActivator : MonoBehaviour
{
    void OnEnable()
    {
        NetworkManager nm = Object.FindFirstObjectByType<NetworkManager>();
        if (nm != null)
        {
            nm.TryUpdateLobbyUI();
        }
    }

}
