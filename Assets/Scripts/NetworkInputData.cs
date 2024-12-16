using UnityEngine;

public class NetworkInputData : MonoBehaviour
{
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
     var data = new NetworkInputData();

     if (Input.GetKey(KeyCode.W))
         data.direction += Vector3.forward;

     if (Input.GetKey(KeyCode.S))
         data.direction += Vector3.back;

     if (Input.GetKey(KeyCode.A))
         data.direction += Vector3.left;

     if (Input.GetKey(KeyCode.D))
        data.direction += Vector3.right;

    input.Set(data);
}
}
