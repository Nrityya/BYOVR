//using UnityEngine;

//public class DrinkMenuControler : MonoBehaviour
//{
//    public GameObject  juice;
//    public GameObject beer;
//    public GameObject coffee;
//    public GameObject tee;

//    public void open_juice()
//    {
//        Debug.Log("Open Juice");
//        juice.SetActive(true);
//        beer.SetActive(false);
//        coffee.SetActive(false);
//        tee.SetActive(false);
//    }
//    public void open_beer()
//    {
//        Debug.Log("Open Beer");
//        juice.SetActive(false);
//        beer.SetActive(true);
//        coffee.SetActive(false);
//        tee.SetActive(false);
//    }
//    public void open_coffee()
//    {
//        Debug.Log("Open Coffee");
//        juice.SetActive(false);
//        beer.SetActive(false);
//        coffee.SetActive(true);
//        tee.SetActive(false);
//    }
//    public void open_tee()
//    {
//        Debug.Log("Open Tee");
//        juice.SetActive(false);
//        beer.SetActive(false);
//        coffee.SetActive(false);
//        tee.SetActive(true);
//    }
//}


using UnityEngine;
using Fusion;

[RequireComponent(typeof(NetworkObject))]
public class DrinkMenuControler : NetworkBehaviour
{
    public GameObject juice;
    public GameObject beer;
    public GameObject coffee;
    public GameObject tee;

    [Networked]
    private int activeDrink { get; set; }  // 0 = none, 1 = juice, 2 = beer, 3 = coffee, 4 = tee

    private int lastDrink = -1;

    public override void Spawned()
    {
        ApplyDrink(activeDrink);
        lastDrink = activeDrink;
    }

    public void open_juice()
    {
        RpcOrderDrink(1);
    }

    public void open_beer()
    {
        RpcOrderDrink(2);
    }

    public void open_coffee()
    {
        RpcOrderDrink(3);
    }

    public void open_tee()
    {
        RpcOrderDrink(4);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RpcOrderDrink(int drinkId)
    {
        activeDrink = drinkId;
    }

    public override void FixedUpdateNetwork()
    {
        if (activeDrink != lastDrink)
        {
            ApplyDrink(activeDrink);
            lastDrink = activeDrink;
        }
    }

    private void ApplyDrink(int drinkId)
    {
        juice.SetActive(false);
        beer.SetActive(false);
        coffee.SetActive(false);
        tee.SetActive(false);

        switch (drinkId)
        {
            case 1:
                Debug.Log("Show Juice");
                juice.SetActive(true);
                break;
            case 2:
                Debug.Log("Show Beer");
                beer.SetActive(true);
                break;
            case 3:
                Debug.Log("Show Coffee");
                coffee.SetActive(true);
                break;
            case 4:
                Debug.Log("Show Tee");
                tee.SetActive(true);
                break;
            default:
                Debug.Log("No drink selected");
                break;
        }
    }
}
