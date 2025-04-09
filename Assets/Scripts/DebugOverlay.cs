using UnityEngine;
using TMPro;

public class DebugOverlay: MonoBehaviour
{
    public TMP_Text debugText;

    void Start()
    {
        debugText.text = "Test 123";
    }
}
