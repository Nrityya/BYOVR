using UnityEngine;

public class HelpMenuControler : MonoBehaviour
{
    public GameObject canves;
    public void close_menu()
    {
        Debug.Log("Close Menu");
        canves.SetActive(false);
    }
    public void open_menu()
    {
        Debug.Log("Open Menu");
        canves.SetActive(true);
    }
}
