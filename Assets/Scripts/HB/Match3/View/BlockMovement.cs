using System;
using UnityEngine;

namespace HB.Match3.View
{
    public class BlockMovement : MonoBehaviour
    {
        public float velocity = 0.0f; // Current Travelling Velocity
        public float maxVelocity = 1.0f; // Maxima Velocity
        public float acceleration = 0.0f; // Current Acceleration
        public float speed = 0.1f; // Amount to increase Acceleration with.
        public float maxAcceleration = 1.0f; // Max Acceleration
        public float minAcceleration = -1.0f; // Min Acceleration


        private Vector3 _targetPosition;
        private Action _onMoveFinished;

        public void MoveTo(Vector3 position, Action onMoveFinished)
        {
            _targetPosition = position;
            _onMoveFinished = onMoveFinished;
        }

        void Update()
        {
            if ((transform.position - _targetPosition).magnitude < 0.1)
            {
                _onMoveFinished?.Invoke();
                _onMoveFinished = null;
                return;
            }

            acceleration += speed;


            if (acceleration > maxAcceleration)
                acceleration = maxAcceleration;
            else if (acceleration < minAcceleration)
                acceleration = minAcceleration;

            velocity += acceleration;

            if (velocity > maxVelocity)
                velocity = maxVelocity;
            else if (velocity < -maxVelocity)
                velocity = -maxVelocity;

       
            transform.Translate(Time.deltaTime * velocity * Vector3.forward);
      
        }
    }
}