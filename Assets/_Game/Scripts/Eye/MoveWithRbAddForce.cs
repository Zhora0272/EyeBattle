using UnityEngine;

public class MoveWithRbAddForce : IMoveableRigidbody
{
    void IMoveableRigidbody.Move(Rigidbody rb, Vector3 direction, float speed)
    {
        rb.AddForce(
            new Vector3(direction.x, 0, direction.y)
            * speed,
            ForceMode.Acceleration);
    }
}