using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushNotificator : MonoBehaviour
{
    void Start()
    {
        Pushwoosh.ApplicationCode = "E7AE2-93A5F";
        Pushwoosh.FcmProjectNumber = "426012666184";
        Pushwoosh.Instance.OnRegisteredForPushNotifications += OnRegisteredForPushNotifications;
        Pushwoosh.Instance.OnFailedToRegisteredForPushNotifications += OnFailedToRegisteredForPushNotifications;
        Pushwoosh.Instance.OnPushNotificationsReceived += OnPushNotificationsReceived;
        Pushwoosh.Instance.RegisterForPushNotifications();
    }

    void OnRegisteredForPushNotifications(string token)
    {
        // handle here
        Debug.Log("Received token: \n" + token);
    }

    void OnFailedToRegisteredForPushNotifications(string error)
    {
        // handle here
        Debug.Log("Error ocurred while registering to push notifications: \n" + error);
    }

    void OnPushNotificationsReceived(string payload)
    {
        // handle here
        Debug.Log("Received push notificaiton: \n" + payload);
    }
}
