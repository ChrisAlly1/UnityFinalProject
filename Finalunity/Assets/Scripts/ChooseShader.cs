using UnityEngine;
using UnityEngine.Networking;

public class ChooseShader : NetworkBehaviour {
    [SyncVar]
    Color gunColor;

    bool enabledThis;

    // Use this for initialization
    public override void OnStartClient() {
        enabledThis = true;
    }

    // Update is called once per frame
    void Update () {
        if (!enabledThis) {
            gameObject.SetActive(false);
        }
	}

    void ChangeToGreen() {
    }
}
