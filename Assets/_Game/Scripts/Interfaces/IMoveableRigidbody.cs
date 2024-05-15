using UnityEngine;

public interface IMoveableRigidbody
{
    public void Move(Rigidbody rb, Vector3 direction, float speed);
}

public class MoveWithRbAddMove : IMoveableRigidbody
{
    public void Move(Rigidbody rb, Vector3 direction, float speed)
    {
        rb.MovePosition(direction);
    }
}