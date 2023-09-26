using UnityEngine;
using System;

public class MoveWithRbAddForce : IMoveableRigidbody
{
    void IMoveableRigidbody.Move(Rigidbody rb, Vector3 direction, float speed)
    {
        _moveAction.Invoke(rb, direction.normalized, speed);
    }

    public MoveWithRbAddForce(bool xyzState = true)
    {
        InitDirectionMove(xyzState);
    }

    private void InitDirectionMove(bool xyzState)
    {
        if (xyzState)
        {
            _moveAction = (rb, normalizeDir, speed) =>
            {
                AddForce(rb, new Vector3(normalizeDir.x, 0, normalizeDir.z), speed);
            };
        }
        else
        {
            _moveAction = (rb, normalizeDir, speed) =>
            {
                AddForce(rb, new Vector3(normalizeDir.x, 0, normalizeDir.y), speed);
            };
        }
    }

    private void AddForce(Rigidbody rb, Vector3 direction, float speed)
    {
        rb.AddForce(direction * speed, ForceMode.VelocityChange);
    }

    private Action<Rigidbody, Vector3, float> _moveAction;
}