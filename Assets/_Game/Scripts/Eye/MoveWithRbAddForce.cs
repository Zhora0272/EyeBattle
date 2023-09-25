using System;
using UnityEngine;

public class MoveWithRbAddForce : IMoveableRigidbody
{
    void IMoveableRigidbody.Move(Rigidbody rb, Vector3 direction, float speed)
    {
        var normalizeDir = direction.normalized;

        _moveAction.Invoke(rb, normalizeDir, speed);
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
                rb.AddForce(new Vector3(normalizeDir.x, 0, normalizeDir.z)
                            * speed,
                    ForceMode.VelocityChange);
            };
        }
        else
        {
            _moveAction = (rb, normalizeDir, speed) =>
            {
                rb.AddForce(new Vector3(normalizeDir.x, 0, normalizeDir.y)
                            * speed,
                    ForceMode.VelocityChange);
            };
        }
    }

    private Action<Rigidbody, Vector3, float> _moveAction;
}