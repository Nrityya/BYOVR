using UnityEngine;

public class Teleporting : MonoBehaviour
{
    public GameObject billiard_teleport;
    public GameObject beerpong_teleport;
    public GameObject karaoke_teleport;
    public GameObject uno_teleport;
    public GameObject camera_gameobject;
   public void teleport_to_billiaer()
    {
        Debug.Log("teleporting to billiard");
        Vector3 vector3 = billiard_teleport.transform.position;
        vector3.y = camera_gameobject.transform.position.y;
        camera_gameobject.transform.position = vector3;
    }
    public void teleport_to_beerpong()
    {
        Debug.Log("teleporting to beerpong");
        Vector3 vector3 = beerpong_teleport.transform.position;
        vector3.y = camera_gameobject.transform.position.y;
        camera_gameobject.transform.position = vector3;
    }
    public void teleport_to_karaoke()
    {
        Debug.Log("teleporting to karaoke");
        Vector3 vector3 = karaoke_teleport.transform.position;
        vector3.y = camera_gameobject.transform.position.y;
        camera_gameobject.transform.position = vector3;

    }
    public void teleport_to_uno()
    {
        Debug.Log("teleporting to uno");
        Vector3 vector3 = uno_teleport.transform.position;
        vector3.y = camera_gameobject.transform.position.y;
        camera_gameobject.transform.position = vector3;
    }
}
