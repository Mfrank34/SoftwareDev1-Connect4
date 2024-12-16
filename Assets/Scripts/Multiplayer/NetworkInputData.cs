using UnityEngine;
using Fusion;

public class NetworkInputHandler : MonoBehaviour
{
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        if (Input.GetKey(KeyCode.W))
        {
            data.direction += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            data.direction += Vector3.back;
        }
        if (Input.GetKey(KeyCode.A))
        {
            data.direction += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            data.direction += Vector3.right;
        }

        // Normalize to prevent faster diagonal movement
        data.direction = data.direction.normalized;

        input.Set(data);
    }
}

public struct NetworkInputData : INetworkInput
{
    public Vector3 direction;
}
