using UnityEngine;
using Fusion;

[RequireComponent(typeof(NetworkObject))]
public class DrinkMenuControler : NetworkBehaviour
{
    enum Drink
    {
        JUICE,
        BEER,
        COFFEE,
        TEA,
    }

    public Vector3 spawnOffset = Vector3.zero;

    public GameObject juicePrefab;
    public GameObject beerPrefab;
    public GameObject coffeePrefab;
    public GameObject teaPrefab;

    public void open_juice()
    {
        RpcOrderDrink(Drink.JUICE);
    }

    public void open_beer()
    {
        RpcOrderDrink(Drink.BEER);
    }

    public void open_coffee()
    {
        RpcOrderDrink(Drink.COFFEE);
    }

    public void open_tea()
    {
        RpcOrderDrink(Drink.TEA);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RpcOrderDrink(Drink drinkId)
    {
        GameObject prefab = drinkId switch
        {
            Drink.BEER => beerPrefab,
            Drink.COFFEE => coffeePrefab,
            Drink.JUICE => juicePrefab,
            Drink.TEA => teaPrefab,
            _ => beerPrefab,
        };
        Runner.Spawn(prefab, transform.position + spawnOffset, Quaternion.identity);
    }
}
