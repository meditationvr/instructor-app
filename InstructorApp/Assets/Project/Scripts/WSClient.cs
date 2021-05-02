using UnityEngine;

public class WSClient : MonoBehaviour
{
    public static string baseUrl = "ws://" + BaseClient.url + "/socket.io/?EIO=4&transport=websocket";
}
