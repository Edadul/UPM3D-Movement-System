using UnityEngine;

namespace Edadul.MovementSystem
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CharacterController))]
    public class MovementController : MonoBehaviour
    {
        [SerializeField] private CharacterData _data;

        public bool IsGrounded => _cc.isGrounded;
        public int JumpsLeft { get; private set; }

        private Rigidbody _rb;
        private CharacterController _cc;

        private float _verticalVelocity;

        private Vector3 _externalForce;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.isKinematic = true;
            _rb.freezeRotation = true;

            _cc = GetComponent<CharacterController>();

            JumpsLeft = _data.maxJumps;
        }

        private void Update()
        {
            if (_cc.isGrounded) JumpsLeft = _data.maxJumps;
        }

        public void Move(Vector2 input, bool isSprinting)
        {
            float speed = _data.moveSpeed * (isSprinting ? _data.sprintMultiplier : 1f);
            Vector3 direction = new Vector3(input.x, 0f, input.y).normalized;

            float accel = _cc.isGrounded ? _data.acceleration : _data.acceleration * 0.4f;

            Vector3 horizontal = Vector3.MoveTowards(
                _currentHorizontal,
                direction * speed,
                accel * Time.fixedDeltaTime);

            _currentHorizontal = horizontal;

            Vector3 motion = horizontal + Vector3.up * _verticalVelocity + _externalForce;
            _cc.Move(motion * Time.fixedDeltaTime);

            _rb.position = transform.position;

            if (direction.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation, targetRot, _data.rotationSpeed * Time.fixedDeltaTime);
            }

            _externalForce = Vector3.MoveTowards(_externalForce, Vector3.zero, _data.deceleration * Time.fixedDeltaTime);
        }

        public void Decelerate()
        {
            _currentHorizontal = Vector3.MoveTowards(
                _currentHorizontal, Vector3.zero, _data.deceleration * Time.fixedDeltaTime);

            Vector3 motion = _currentHorizontal + Vector3.up * _verticalVelocity + _externalForce;
            _cc.Move(motion * Time.fixedDeltaTime);

            _rb.position = transform.position;

            _externalForce = Vector3.MoveTowards(_externalForce, Vector3.zero, _data.deceleration * Time.fixedDeltaTime);
        }

        public void ApplyGravity()
        {
            if (_cc.isGrounded && _verticalVelocity < 0f)
            {
                _verticalVelocity = -2f;
                return;
            }

            float gravity = _verticalVelocity < 0f
                ? _data.gravity * _data.fallMultiplier
                : _data.gravity;

            _verticalVelocity += gravity * Time.fixedDeltaTime;
        }

        public void Jump()
        {
            if (JumpsLeft <= 0) return;
            _verticalVelocity = Mathf.Sqrt(-2f * _data.gravity * _data.jumpHeight);
            JumpsLeft--;
        }

        /// <summary>
        /// Push objects with Rigidbody when colliding with them, except if they are below (ground).
        /// </summary>
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody hitRb = hit.collider.attachedRigidbody;
            if (hitRb == null || hitRb.isKinematic) return;

            if (hit.moveDirection.y < -0.3f) return;

            Vector3 pushDir = new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z);
            hitRb.AddForce(pushDir * _data.pushForce, ForceMode.Force);
        }

        /// <summary>
        /// Recieve external forces from other scripts (like explosions, wind, etc.) and apply them to the character.
        /// </summary>
        public void AddExternalForce(Vector3 force)
        {
            _externalForce += force;
        }

        private Vector3 _currentHorizontal;
    }
}