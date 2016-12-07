using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
public class ChatBoxFunctions : NetworkBehaviour {
    [SerializeField] ContentSizeFitter contentSizeFitter1;
    [SerializeField]  Text showHideButtonText;
    [SerializeField] Transform messageParentPanel;
    [SerializeField] GameObject newMessagePrefab;
    bool isChatShowing = false;
    string message = " ";
    void Start()
    {
        ToggleChat();
    }
    public void ToggleChat()
    {
        isChatShowing = !isChatShowing;
        if(isChatShowing)
        {
            contentSizeFitter1.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            showHideButtonText.text = "Hide Chat";
        }else
        {
            contentSizeFitter1.verticalFit = ContentSizeFitter.FitMode.MinSize;
            showHideButtonText.text = "Show Chat";
        }
    }

    public void SetMessage(string message)
    {
        this.message = message;
    }

    public void ShowMessage() {
        if (message != "aaaa") {
            GameObject clone = Instantiate(newMessagePrefab);
            clone.transform.SetParent(messageParentPanel);
            clone.transform.SetSiblingIndex(messageParentPanel.childCount - 2);
            clone.GetComponent<MessageFunction>().ShowMessage(message);

            NetworkServer.Spawn(clone);
        }
    }

}
