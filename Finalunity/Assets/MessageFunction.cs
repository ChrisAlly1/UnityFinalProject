using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
public class MessageFunction : NetworkBehaviour {
    [SerializeField] Text text;
public void ShowMessage(string message)
    {
        text.text = message;
    }
    public void HideMessaage()
    {
        Destroy(gameObject);
    }
}
