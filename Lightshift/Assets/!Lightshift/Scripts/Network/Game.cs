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
        NetworkClient.RegisterHandler<YouKilledEntityMessage>(OnKillEntity);
        NetworkServer.RegisterHandler<RespawnMessage>(OnPlayerRespawn);
    }

    private void OnPlayerRespawn(NetworkConnection client, RespawnMessage m)
    {
        GameUIManager.Instance.HandleRespawnScreen(false);
    }

    private void OnKillEntity(NetworkConnection arg1, YouKilledEntityMessage message)
    {
        GameUIManager.Instance.ShowAnnouncementText($"You killed {message.username}!");
    }

    private void OnDeathMessage(NetworkConnection arg1, YouWereKilledMessage message)
    {
        GameUIManager.Instance.HandleRespawnScreen(showRespawn: true, message.killerName);
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
