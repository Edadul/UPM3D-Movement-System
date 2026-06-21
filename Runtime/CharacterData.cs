using UnityEngine;

namespace Edadul.MovementSystem
{
    [CreateAssetMenu(menuName = "Edadul/Movement System/Character Data", fileName = "CharacterData")]
    public class CharacterData : ScriptableObject
    {
        [Header("Movement")]
        public float moveSpeed = 6f;
        public float sprintMultiplier = 1.6f;
        public float acceleration = 15f;
        public float deceleration = 20f;

        [Header("Jump")]
        public float jumpHeight = 1.8f;
        public float gravity = -20f;
        public float fallMultiplier = 2.2f;
        public int maxJumps = 1;

        [Header("Ground Detection")]
        public float groundCheckRadius = 0.3f;
        public float groundCheckDistance = 0.05f;
        public LayerMask groundLayers = ~0;

        [Header("Rotation")]
        public float rotationSpeed = 10f;

        [Header("Physics Interaction")]
        public float pushForce = 3f;
    }
}