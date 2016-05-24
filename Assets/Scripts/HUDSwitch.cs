using UnityEngine;
using System.Collections;
using DG.Tweening;

public class HUDSwitch : MonoBehaviour
{
    public bool isPushButton;

    public Transform head;
    public Sprite switchSprite;
    public Vector3 upPosition;
    public Vector3 downPosition;
    public float tweenSpeed = 0.2f;

    [System.Serializable]
    public class Message
    {
        public GameObject sendTo;
        public string methodName;
        public int sendValue;
    }

    public Message[] messages;

    bool isOn;

    public void Toggle()
    {
        if (isPushButton)
        {
            Set(1);
            StartCoroutine(PushButton());
            return;
        }

        isOn = !isOn;

        if (isOn) Set(1);
        else Set(0);

    }

    Sprite originalSprite;

    void Start()
    {
        originalSprite = GetComponent<UnityEngine.UI.Image>().sprite;
    }

    public void Set(int i)
    {

        if (head)
        {
            if (i == 0)
                head.DOLocalMove(upPosition, tweenSpeed);
            else
                head.DOLocalMove(downPosition, tweenSpeed);
        }

        if (switchSprite)
        {
            if (i == 0)
                GetComponent<UnityEngine.UI.Image>().sprite = originalSprite;
            else
                GetComponent<UnityEngine.UI.Image>().sprite = switchSprite;
        }

        SendTo(i);
    }

    IEnumerator PushButton()
    {
        yield return new WaitForSeconds(1);

        Set(0);
    }

    void SendTo(int i)
    {
        if (messages == null) return;
        if (messages.Length == 0) return;
        if (i >= messages.Length) return;
        if (messages[i] == null) return;
        if (messages[i].sendTo == null) return;

        messages[i].sendTo.SendMessage(messages[i].methodName, messages[i].sendValue);
    }
}