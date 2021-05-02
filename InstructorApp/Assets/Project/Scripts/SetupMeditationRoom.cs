using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SetupMeditationRoom : MonoBehaviour
{
    public Dropdown numberOfStudents;

    public void createMeditationRoom()
    {
        MeditationRoomSetupData.RoomSize = int.Parse(numberOfStudents.options[numberOfStudents.value].text);
        SceneManager.LoadScene("MeditationRoom");
    }
}
