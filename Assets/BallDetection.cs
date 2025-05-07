using Unity.VisualScripting;
using UnityEngine;

public class BallDetection : MonoBehaviour
{  
    public enum Type{
        Cup,
        Pool
    }
    public Type type;
    Collider c;
    ParticleSystem ps;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        c=GetComponent<SphereCollider>();
        ps = GetComponent<ParticleSystem>();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BeerPongBall") && type==Type.Cup)
        {   
            ps.Play();
            Destroy(other.gameObject);
            
            SendMessageUpwards("RpcFlipCup", SendMessageOptions.DontRequireReceiver);
        }
        // else if (other.CompareTag("PoolBall") && type==Type.Pool)
        // {

        // }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
