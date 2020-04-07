using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PlayerMenuItem : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI _usernameLabel;

    public void SetUsername(string username) => _usernameLabel.text = username;
}
