using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class HTTPClient : MonoBehaviour
{

    public delegate void PostDelegate(string responseCode);

    private static string baseUrl = "http://" + BaseClient.url + "/api";

    public static IEnumerator Post(string endpointUrl, string bodyJsonString, PostDelegate callback)
    {
        var request = new UnityWebRequest(baseUrl + endpointUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.Send();

        Debug.Log("Status Code: " + request.responseCode);
        callback?.Invoke(request.responseCode.ToString());
    }
}
