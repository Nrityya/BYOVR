using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerAnimatorController : MonoBehaviour
{
    public float speed = 5f;
    private Animator animator;
    private CharacterController controller;

    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(h, 0, v);

        bool isMoving = direction.magnitude > 0.1f;
        animator.SetBool("isMoving", isMoving);

        controller.Move(direction * speed * Time.deltaTime);
    }
}
