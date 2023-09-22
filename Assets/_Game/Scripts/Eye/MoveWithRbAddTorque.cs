using UnityEngine;

public class MoveWithRbAddTorque : IMoveableRigidbody
{
    void IMoveableRigidbody.Move(Rigidbody rb, Vector3 direction, float speed)
    {
        rb.AddTorque(
            new Vector3(direction.x, 0, direction.y)
            * speed,
            ForceMode.Acceleration);
    }
}