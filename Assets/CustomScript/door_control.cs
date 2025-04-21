using TMPro;
using UnityEngine;

public class door_control : MonoBehaviour
{
    public GameObject door;
    Vector3 colse_position = new Vector3(2f, 0.0f, -1f);
    Vector3 colse_rotation = new Vector3(0.0f, -90.0f, 0.0f);
    Vector3 open_position = new Vector3(2.5f, 0.0f, -1.4f);
    Vector3 open_rotation = new Vector3(0.0f, 0.0f, 0.0f);

    bool door_open = false;

    public void door_controler()
    {
        Debug.Log("Door Control");
            if (door_open == false)
            {
                open_door();
                door_open = true;
            }
            else
            {
                close_door();
                door_open = false;
            }
    }
    void open_door()
    {
        Debug.Log("Opening door");
        door.transform.position = open_position;
        door.transform.rotation = Quaternion.Euler(open_rotation);
    }
    void close_door()
    {
        Debug.Log("Closing door");
        door.transform.position = colse_position;
        door.transform.rotation = Quaternion.Euler(colse_rotation);
    }
}
