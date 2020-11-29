using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Lightshift.Scripts
{
    public class Delegator : MonoBehaviour
    {
        private static Delegator _instance { get; set; }
        private List<Action> _actions = new List<Action>();
        private bool _waitingForFrame = false;
        private void Awake()
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        public static void WaitForEndOfFrame(Action callback) 
        {
            _instance._actions.Add(callback);
            if (!_instance._waitingForFrame)
            {
                _instance._waitingForFrame = true;
                _instance.StartCoroutine(_instance.WaitForEndOfFrame());
            }
        }

        private IEnumerator WaitForEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            for (int i = 0; i < _actions.Count; i++) 
                _actions[i]?.Invoke();

            _actions.Clear();
            _waitingForFrame = false;
        }
    }
}
