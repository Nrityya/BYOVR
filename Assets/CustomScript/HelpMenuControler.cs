using UnityEngine;
using Fusion;
public class HelpMenuControler : MonoBehaviour
{
    public GameObject canvas;
    private bool isMenuOpen = false;

    void OpenMenuLocal()
    {
        canvas.SetActive(true);
        isMenuOpen = true;
    }

    void CloseMenuLocal()
    {
        canvas.SetActive(false);
        isMenuOpen = false;
    }

    // Call this from the EventTrigger → pointer click
    public void open_menu()
    {
        OpenMenuLocal();
    }

    // Call this from the button inside the menu → button onClick
    public void close_menu()
    {
        CloseMenuLocal();
    }
}
