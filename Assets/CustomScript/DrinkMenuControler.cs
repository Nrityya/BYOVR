using UnityEngine;

public class DrinkMenuControler : MonoBehaviour
{
    public GameObject  juice;
    public GameObject beer;
    public GameObject coffee;
    public GameObject tee;

    public void open_juice()
    {
        Debug.Log("Open Juice");
        juice.SetActive(true);
        beer.SetActive(false);
        coffee.SetActive(false);
        tee.SetActive(false);
    }
    public void open_beer()
    {
        Debug.Log("Open Beer");
        juice.SetActive(false);
        beer.SetActive(true);
        coffee.SetActive(false);
        tee.SetActive(false);
    }
    public void open_coffee()
    {
        Debug.Log("Open Coffee");
        juice.SetActive(false);
        beer.SetActive(false);
        coffee.SetActive(true);
        tee.SetActive(false);
    }
    public void open_tee()
    {
        Debug.Log("Open Tee");
        juice.SetActive(false);
        beer.SetActive(false);
        coffee.SetActive(false);
        tee.SetActive(true);
    }
}
