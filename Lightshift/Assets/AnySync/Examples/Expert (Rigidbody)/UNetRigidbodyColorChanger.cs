//using AnySync;
//using AnySync.Examples;
//using UnityEngine;
//using UnityEngine.Networking;

///// <summary>
///// This is an example of using InvocationSync in a separate independent script.
///// InvocationSync instance could be reused, depending on the project setup.
///// </summary>
//public class UNetRigidbodyColorChanger : NetworkBehaviour
//{
//    private InvocationSync _invocationSync;
//    private void Awake()
//    {
//        // Creates a new instance that will automatically update together with the specified MotionGenerator.
//        _invocationSync = new InvocationSync(GetComponent<UNetRigidbodySync>().MotionGenerator);
//    }

//    private void Update()
//    {
//        if (!hasAuthority)
//            return;

//        // For this example, assign a random color every time the object is teleporting.
//        if (Input.GetKeyDown(KeyCode.Space))
//        {
//            var randomColor = new Color(Random.value, Random.value, Random.value);
//            ChangeColor(randomColor);
//            CmdChangeColor(randomColor);
//        }
//    }

//    private void ChangeColor(Color color)
//    {
//        GetComponent<Renderer>().material.color = color;
//    }

//    [Command]
//    private void CmdChangeColor(Color color)
//    {
//        RpcChangeColor(color);
//    }

//    [ClientRpc]
//    private void RpcChangeColor(Color color)
//    {
//        if (hasAuthority)
//            return;

//        _invocationSync.AddAction(() => ChangeColor(color));
//    }
//}
