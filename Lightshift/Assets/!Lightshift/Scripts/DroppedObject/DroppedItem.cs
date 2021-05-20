using Mirror;
using SharedModels.Models.Game;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DroppedItem : NetworkBehaviour
{
    [SyncVar]
    public ResourceType resourceType;

    [SyncVar]
    public int Amount;

    [SerializeField] SpriteRenderer _renderer;
    [SerializeField] TextMeshProUGUI _amountText;
    [SerializeField] TextMeshProUGUI _labelText;
    [SerializeField] GameObject _infoTooltip;
    private float _delay = 5;
    private float _lifeTime = 100;

    private List<PlayerShip> _players;
    private PlayerShip _dropper;

    private Transform _iconTransform;
    public void Init(ResourceObject resource, PlayerShip dropper)
    {
        _dropper = dropper;
        resourceType = resource.Type;
        Amount = resource.Amount;

        _players = new List<PlayerShip>();

        InitClient();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        InitClient();
    }

    private void InitClient() 
    {
        _infoTooltip.SetActive(false);
        var resObj = ItemService.GetResourceItem(resourceType);
        _renderer.sprite = resObj.Sprite;
        _iconTransform = _renderer.gameObject.transform;
        _iconTransform.transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 360));
        _labelText.text = resourceType.ToString();
        _amountText.text = Amount.ToString();
    }

    private void OnMouseOver()
    {
        _infoTooltip.SetActive(true);
    }

    private void OnMouseExit()
    {
        _infoTooltip.SetActive(false);
    }
    private void Update()
    {
        if (_iconTransform != null)
            _iconTransform.Rotate(new Vector3(0, 0, 10 * Time.deltaTime));

        if (!isServer || _players == null)
            return;

        _delay -= Time.deltaTime;
        _lifeTime -= Time.deltaTime;

        if (_lifeTime < 0)
        {
            Destroy(gameObject);
            return;
        }

        if (_delay < 0 && _players.Count > 0 || (_delay > 4.8 && _players.Count > 0))
        {
            var amount = (int)(Amount / _players.Count);
            foreach (var player in _players)
                player.Player.PickupResource(resourceType, amount);

            Destroy(gameObject);
            _players = null;
        }
    }


    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isServer)
            return;

        var player = collision.GetComponentInParent<PlayerShip>();
        if (player == null)
            return;

        if (player.Id == _dropper.Id && _delay > 4.7)
            return;
        if (!_players.Contains(player))
            _players.Add(player);
    }

    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (!isServer)
            return;

        var player = collision.GetComponentInParent<PlayerShip>();
        if (player == null)
            return;

        if (_players.Contains(player))
            _players.Remove(player);
    }
}
