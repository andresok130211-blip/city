using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonPlayer : MonoBehaviour
{
    public float walkSpeed = 4.2f, runSpeed = 6.5f, jumpHeight = 1.25f, gravity = -22f;
    public Transform cameraTransform;
    CharacterController controller;
    Vector2 mobileMove;
    float verticalVelocity;
    bool jumpQueued;
    void Awake() { controller = GetComponent<CharacterController>(); }
    public void SetMove(Vector2 value) { mobileMove = Vector2.ClampMagnitude(value, 1f); }
    public void QueueJump() { jumpQueued = true; }
    void Update()
    {
        Vector2 input = mobileMove;
#if UNITY_EDITOR || UNITY_STANDALONE
        Vector2 keyboard = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (keyboard.sqrMagnitude > input.sqrMagnitude) input = Vector2.ClampMagnitude(keyboard, 1f);
#endif
        Vector3 forward = cameraTransform ? cameraTransform.forward : transform.forward;
        Vector3 right = cameraTransform ? cameraTransform.right : transform.right;
        forward.y = 0; right.y = 0; forward.Normalize(); right.Normalize();
        Vector3 move = forward * input.y + right * input.x;
        if (move.sqrMagnitude > .01f) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(move), 14f * Time.deltaTime);
        float speed = input.magnitude > .75f ? runSpeed : walkSpeed;
        if (controller.isGrounded && verticalVelocity < 0) verticalVelocity = -2f;
        if (jumpQueued && controller.isGrounded) verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        jumpQueued = false;
        verticalVelocity += gravity * Time.deltaTime;
        Vector3 velocity = move * speed; velocity.y = verticalVelocity;
        controller.Move(velocity * Time.deltaTime);
    }
}