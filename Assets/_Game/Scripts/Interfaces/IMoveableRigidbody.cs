using UnityEngine;

public interface IMoveableRigidbody
{
    public void Move(Rigidbody rb, Vector3 direction, float speed);
}