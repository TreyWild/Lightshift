using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    private TextMeshProUGUI _text;
    private Kinematic _kinematic;
    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _kinematic = GetComponent<Kinematic>();
    }

    private void OnEnable()
    {
        _kinematic.AddForce(Vector2.down * 5);
    }

    private void Update()
    {
        var color = _text.color;
        _text.color = new Color(color.r, color.b, color.g, color.a - Time.deltaTime);

        if (_text.color.a <= 0)
        {
            gameObject.SetActive(false);
            _text.text = "";
            _text.color = new Color(color.r, color.b, color.g, 1);
        }
    }
}