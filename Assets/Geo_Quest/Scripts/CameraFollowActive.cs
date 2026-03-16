using UnityEngine;

public class CameraFollowActive : MonoBehaviour
{
    public GameObject cubePlayer;
    public GameObject shipPlayer;

    void LateUpdate()
    {
        if (cubePlayer.activeSelf)
        {
            transform.position = new Vector3(
                cubePlayer.transform.position.x + 3,
                cubePlayer.transform.position.y,
                -10
            );
        }
        else if (shipPlayer.activeSelf)
        {
            transform.position = new Vector3(
                shipPlayer.transform.position.x + 3,
                shipPlayer.transform.position.y,
                -10
            );
        }
    }
}