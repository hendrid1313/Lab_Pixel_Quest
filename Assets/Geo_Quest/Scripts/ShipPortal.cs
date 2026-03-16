using UnityEngine;

public class ShipPortal : MonoBehaviour
{
    public GameObject cubePlayer;
    public GameObject shipPlayer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == cubePlayer)
        {
            cubePlayer.SetActive(false);

            shipPlayer.transform.position = cubePlayer.transform.position;
            shipPlayer.SetActive(true);

            Camera.main.transform.parent = shipPlayer.transform;
        }
    }
}