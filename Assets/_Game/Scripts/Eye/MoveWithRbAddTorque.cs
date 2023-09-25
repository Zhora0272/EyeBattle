using UnityEngine;

public class MoveWithRbAddTorque : IMoveableRigidbody
{
    void IMoveableRigidbody.Move(Rigidbody rb, Vector3 direction, float speed)
    {
        rb.AddTorque(direction + Vector3.right
                     * speed,
            ForceMode.Acceleration);
    }
}