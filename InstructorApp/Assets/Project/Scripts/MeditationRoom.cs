using UnityEngine;
using SocketIO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using agora_gaming_rtc;
using UnityEngine.Android;
using System;

[Serializable]
public class RoomData
{
    public string agoraIoVoiceKey;
    public string roomId;
}

[Serializable]
public class PredictedLabels
{
    public string MLPerceptron;
    public string NaiveBayes;
    public string RandomForest;
}

[Serializable]
public class UserData
{
    public string userId;
    public string name;
    [SerializeField]
    public PredictedLabels predictedLabels;
}

[Serializable]
public class UserDataCollection
{
    [SerializeField]
    public UserData[] users;
}

public class MeditationRoom : MonoBehaviour
{
    public GameObject waitingForStudentsText;
    public GameObject instructionButtonText;

    public GameObject eegTable;
    public GameObject eegTableContent;
    public GameObject[] userRows = new GameObject[6];

    private SocketIOComponent socket;
    private bool meditationSessionStarted;
    private IRtcEngine mRtcEngine = null;

    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;

        foreach (GameObject userRow in userRows)
        {
            userRow.transform.GetChild(0).GetComponent<Text>().text = "";
            userRow.transform.GetChild(1).GetComponent<Text>().text = "";
            userRow.transform.GetChild(2).GetComponent<Text>().text = "";
            userRow.transform.GetChild(3).GetComponent<Text>().text = "";
            userRow.SetActive(false);
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        #if (UNITY_2018_3_OR_NEWER)
        if (Permission.HasUserAuthorizedPermission(Permission.Microphone)) {

        } else {
            Permission.RequestUserPermission(Permission.Microphone);
        }
        #endif

        meditationSessionStarted = false;

        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();

        socket.On("open", OpenSocket);
        socket.On("error", ErrorSocket);
        socket.On("close", CloseSocket);
        socket.On("authenticate", AuthenticateSocket);
        socket.On("authenticated", AuthenticatedSocket);
        socket.On("created-meditation-room", CreatedMeditationRoomSocket);
        socket.On("meditation-room-not-full", MeditationRoomNotFullSocket);
        socket.On("meditation-room-is-full", MeditationRoomIsFullSocket);
        socket.On("got-student-data", GotStudentDataSocket);
        socket.On("instructor-left-room", InstructorLeftRoomSocket);

        socket.Connect();
    }

    void Update()
    {
       if (!meditationSessionStarted) {
            instructionButtonText.GetComponent<Text>().text = "Stop search";
            waitingForStudentsText.SetActive(true);
            eegTable.SetActive(false);
       } else {
            instructionButtonText.GetComponent<Text>().text = "Stop Meditation Instruction";
            waitingForStudentsText.SetActive(false);
            eegTable.SetActive(true);
        }
    }

    void OnDestroy()
    {
        Debug.Log("Scene got destroyed!");
        LeaveChannel();
        socket.Emit("disconnect");
    }

    void LeaveChannel()
    {       
        if (mRtcEngine != null)
            mRtcEngine.LeaveChannel();
    }

    void JoinChannel(string channelName)
    {
        Debug.Log(string.Format("tap joinChannel with channel name {0}", channelName));

        if (string.IsNullOrEmpty(channelName))
        {
            return;
        }

        mRtcEngine.JoinChannel(channelName, "extra", 9527);
    }

    public void stopMeditationInstruction()
    {
        SceneManager.LoadScene("SetupMeditationRoom");
    }

    void InstructorLeftRoomSocket(SocketIOEvent e)
    {
        stopMeditationInstruction();
    }

    void GotStudentDataSocket(SocketIOEvent e)
    {
        UserDataCollection userData = JsonUtility.FromJson<UserDataCollection>(e.data.ToString());

        for (int i = 0; i < userData.users.Length; i++)
        {
            UserData userDataEntry = userData.users[i];

            GameObject row = userRows[i];

            // Update Row
            row.transform.GetChild(0).GetComponent<Text>().text = userDataEntry.name;
            row.transform.GetChild(1).GetComponent<Text>().text = userDataEntry.predictedLabels.RandomForest;
            row.transform.GetChild(2).GetComponent<Text>().text = userDataEntry.predictedLabels.MLPerceptron;
            row.transform.GetChild(3).GetComponent<Text>().text = userDataEntry.predictedLabels.NaiveBayes;
            row.SetActive(true);
        }
        
        socket.Emit("get-students-data");
    }

    void MeditationRoomIsFullSocket(SocketIOEvent e)
    {
        meditationSessionStarted = true;

        RoomData roomData = JsonUtility.FromJson<RoomData>(e.data.ToString());
        mRtcEngine = IRtcEngine.GetEngine(roomData.agoraIoVoiceKey);
        JoinChannel(roomData.roomId);

        socket.Emit("get-students-data");
    }

    void MeditationRoomNotFullSocket(SocketIOEvent e)
    {
        socket.Emit("check-meditation-room-status");
    }

    void CreatedMeditationRoomSocket(SocketIOEvent e)
    {
        socket.Emit("check-meditation-room-status");
    }

    void AuthenticateSocket(SocketIOEvent e)
    {
        Debug.Log("[SocketIO] Authenticate received: " + e.name + " " + e.data);
        FBAccessToken fbAccessToken = new FBAccessToken();
        fbAccessToken.TokenString = Facebook.Unity.AccessToken.CurrentAccessToken.TokenString;
        fbAccessToken.UserType = "Instructor";
        socket.Emit("authenticate", new JSONObject(JsonUtility.ToJson(fbAccessToken)));
    }

    void AuthenticatedSocket(SocketIOEvent e)
    {
        Debug.Log("[SocketIO] Authenticated received: " + e.name + " " + e.data);
        MeditationRoomSetupData meditationRoomSetupData = new MeditationRoomSetupData();
        meditationRoomSetupData.NumberOfStudents = MeditationRoomSetupData.RoomSize;

        socket.Emit("create-meditation-room", new JSONObject(JsonUtility.ToJson(meditationRoomSetupData)));
    }

    void OpenSocket(SocketIOEvent e)
    {
        Debug.Log("[SocketIO] Open received: " + e.name + " " + e.data);
    }

    void ErrorSocket(SocketIOEvent e)
    {
        Debug.Log("[SocketIO] Error received: " + e.name + " " + e.data);
    }

    void CloseSocket(SocketIOEvent e)
    {
        Debug.Log("[SocketIO] Close received: " + e.name + " " + e.data);
    }
}
