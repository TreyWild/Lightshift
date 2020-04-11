using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ForceField : MonoBehaviour 
{
    public bool Active = true;

    private GameObject _forceField;
    [Range(0.1f, 30)]
    public float fieldSize = 15;
    public Vector3 reflectionSpeed;
    public bool solidForceField = false;

    private CircleCollider2D _collider;
    private RotateGameObject _rotationObject;

    public event Action<GameObject> OnTriggerEnter;
    public event Action<GameObject> OnTriggerLeave;
    private void Start()
    {
        _forceField = Instantiate(PrefabManager.Instance.forceFieldPrefab, transform);
        _forceField.layer = gameObject.layer;
        _collider = _forceField.AddComponent<CircleCollider2D>();
        _collider.isTrigger = !solidForceField;

        _rotationObject = _forceField.GetComponent<RotateGameObject>();
    }

    private void Update()
    {
        if (!Active)
        {
            if (_forceField.activeInHierarchy)
                _forceField.SetActive(false);
            return;
        }

        _rotationObject.Speed = reflectionSpeed;
        _collider.isTrigger = !solidForceField;
        _forceField.transform.localScale = new Vector3(fieldSize, fieldSize, fieldSize / 1.5f);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        OnTriggerEnter?.Invoke(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnTriggerLeave?.Invoke(collision.gameObject);
    }
}