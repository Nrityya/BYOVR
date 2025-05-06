using UnityEngine;
using Fusion;
public class HelpMenuControler : NetworkBehaviour
{
    public GameObject canvas;
    private bool isMenuOpen = false;

    //public void close_menu()
    //{
    //    Debug.Log("Close Menu");
    //    canvas.SetActive(false);
    //    isMenuOpen = false;
    //}
    //public void open_menu()
    //{
    //    Debug.Log("Open Menu");
    //    canvas.SetActive(true);
    //    isMenuOpen = true;
    //}
    void OpenMenuLocal()
    {
        canvas.SetActive(true);
        isMenuOpen = true;
        Debug.Log("Menu opened locally");
    }

    void CloseMenuLocal()
    {
        canvas.SetActive(false);
        isMenuOpen = false;
        Debug.Log("Menu closed locally");
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RpcOpenMenu()
    {
        OpenMenuLocal();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RpcCloseMenu()
    {
        CloseMenuLocal();
    }
    // Call this from the EventTrigger → pointer click
    public void open_menu()
    {
        RpcOpenMenu();
    }

    // Call this from the button inside the menu → button onClick
    public void close_menu()
    {
        RpcCloseMenu();
    }
}
