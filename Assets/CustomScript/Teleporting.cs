using UnityEngine;
using Fusion;

public class Teleporting : NetworkBehaviour
{
    public GameObject billiard_teleport;
    public GameObject beerpong_teleport;
    public GameObject karaoke_teleport;
    public GameObject uno_teleport;

    public void teleport_to_billiards()
    {
        PlayerNetworkController.localPlayer.Teleport(billiard_teleport.transform.position);
    }

    public void teleport_to_beer_pong()
    {
        PlayerNetworkController.localPlayer.Teleport(beerpong_teleport.transform.position);
    }

    public void teleport_to_karaoke()
    {
        PlayerNetworkController.localPlayer.Teleport(karaoke_teleport.transform.position);
    }

    public void teleport_to_uno()
    {
        PlayerNetworkController.localPlayer.Teleport(uno_teleport.transform.position);
    }
}
