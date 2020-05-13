using UnityEngine;
using Mirror;

public class Game : MonoBehaviour
{
    public static Game Instance { get; set; }
    public void Awake()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }
    public void Start()
    {
        InitializeMessageHandlers();
    }

    public void InitializeMessageHandlers() 
    {
        NetworkClient.RegisterHandler<AlertMessage>(OnAlert);
        NetworkClient.RegisterHandler<ChatMessage>(OnChatMessage);
        NetworkClient.RegisterHandler<YouWereKilledMessage>(OnDeathMessage);

        NetworkServer.RegisterHandler(delegate (YouKilledEntityMessage message)
        {
            GameUIManager.Instance.ShowAnnouncementText($"You killed {message.username}!");
        });
    }

    private void OnDeathMessage(NetworkConnection client, YouWereKilledMessage message)
    {
        var entity = EntityManager.GetEntity(message.killerEntityId);
        if (entity != null)
            CameraFollow.Instance.SetTarget(entity.transform);
    }

    private void OnAlert(NetworkConnection connection, AlertMessage message)
    {
        if (message.IsPopup)
        {
            DialogManager.ShowMessage(message.Message);
            return;
        }
        else ChatBox.Instance.AddMessage($"<color=red>SYSTEM: {message.Message}</color>");
    }

    private void OnChatMessage(NetworkConnection connection, ChatMessage chatObj)
    {
        ChatBox.Instance.AddMessage($"{chatObj.username}: {chatObj.message}");
    }
}
