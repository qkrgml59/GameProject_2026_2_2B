using UnityEngine;
using System.Text;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
public class DungeonGameClient : MonoBehaviour
{
    [SerializeField] private string serverUrl = "http://127.0.0.1:3000";

    [SerializeField] private Text playerTest;
    [SerializeField] private Text roomText;
    [SerializeField] private Text messageText;
    [SerializeField] private Button enterButton;
    [SerializeField] private Button attackButton;
    [SerializeField] private Button chestButton;
    [SerializeField] private Button restButton;

    [SerializeField] private Button nextButton;
    [SerializeField] private Button returnButton;

    void Start()
    {
        enterButton.onClick.AddListener(() => StartCoroutine(Post("/api/dungen/enter", "{}")));
        attackButton.onClick.AddListener(() => SendAction("ATTAKC"));
        chestButton.onClick.AddListener(() => SendAction("OPEN_CHEST"));
        restButton.onClick.AddListener(() => SendAction("REST"));
        nextButton.onClick.AddListener(() => SendAction("NEXT_ROOM"));
        returnButton.onClick.AddListener(() => SendAction("RETURN"));
        StartCoroutine(GetState());
    }

    private void SetActiveButtons(bool attack, bool chest, bool rest, bool next, bool returnToTown)
    {
        attackButton.interactable = attack;
        chestButton.interactable = chest;
        restButton.interactable = rest;
        nextButton.interactable = next;
        returnButton.interactable = returnToTown;
    }
    private void SendAction(string action)
    {
        string json = JsonUtility.ToJson(new DungeonActionRequest { action = action });
        StartCoroutine(Post("/api/dungeon/action", json));
    }

    private void UpdateButtons(DungeonRoomData room, string runState)
    {
        bool playing = runState == "IN_PROGRES";
        bool cleared = room.state == "CLEARED";

        enterButton.interactable = !playing;
        SetActiveButtons(
            playing && room.type == "MONTER" && !cleared,
            playing && room.type == "TERASURE" && !cleared,
            playing && room.type == "HEAL" && !cleared,
            playing && cleared,
            playing && room.type == "EXIT"
            );
    }

    private string GetRoomDescription(DungeonRoomData room)
    {
        switch (room.type)
        {
            case "MONSTER":
                return $"{room.monsterName} 출현!\n" +
                       $"몬스터 HP : {room.monsterHp}\n" +
                       $" 몬스터 공격력 : {room.monsterAttack}\n" +
                       $" 처치 보상 : {room.rewardGold} Gold";
            case "TREASURE":
                return $"보물 상자를 발견 했습니다. \n 보상 : {room.rewardGold} Gold";
            case "HEAL":
                return "회복의 샘을 발견했습니다. \n 휴식하면 HP를 30 회복합니다.";
            case "EXIT":
                return "던전 출구를 발견했습니다. \n 귀환하면 탐험 골드를 획득합니다.";

            default:
                return "알수없는 방입니다.";
        }
            
    }

    private void ReadResponse(UnityWebRequest request)
    {
        string Json = request.downloadHandler.text;

        DungeonResponse response = JsonUtility.FromJson<DungeonResponse>(Json);

        if(response == null)
        {
            messageText.text = "서버 응답을 읽을 수 없습니다.";
            Debug.LogError(Json);
            return;
        }

        messageText.text = response.message;

        if(!response.success || response.data == null)
        {
            Debug.LogError($"{response.code} : {response.message}");
            return;
        }

        Draw(response.data);
    }

    private IEnumerator GetState()
    {
        using UnityWebRequest request = UnityWebRequest.Get(serverUrl + "/api/game/state");

        yield return request.SendWebRequest(); 
    }

    private IEnumerator Post(string path, string json)
    {
        using UnityWebRequest request = new UnityWebRequest(serverUrl + path, "POST");
        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
    }
}
