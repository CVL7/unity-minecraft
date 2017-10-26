using System;
using Level;
using UnityEngine;
using Utils;

namespace Player
{
    public class BlockInteractor : MonoBehaviour
    {
        const int interationDistance = 4;
        
        public event Action selectedBlockChanged = delegate { };
        public Vector3i selectedPosition { get; private set; }
        public Vector3i placeblePosition { get; private set; }
        public Chunk selectedChunk { get; private set; }     
        public bool isBlockSelected { get; private set; }
        
        [SerializeField] World world;
        [SerializeField] Camera playerCamera;
        Transform cameraTransform;
        RaycastHit hit; 

        void Awake()
        {
            cameraTransform = playerCamera.transform;
        }

        void Update()
        {
            var selectedState = isBlockSelected;

            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, interationDistance)
                && hit.collider.GetComponent<Chunk>())
            {
                isBlockSelected = SelectBlock();
            }
            else
            {
                isBlockSelected = false;
            }

            if (selectedState != isBlockSelected) selectedBlockChanged();
        }

        bool SelectBlock()
        {
            var hitPosition = hit.collider.transform.position;
            var chunkPosition = World.GetChunkPosition(hitPosition);
            selectedChunk = world.GetChunk(chunkPosition);
            if (!selectedChunk) return false;

            var outsideHit = Vector3i.FloorToInt(hit.point + hit.normal / 2);
            var insideHit = Vector3i.FloorToInt(hit.point - hit.normal / 2);

            placeblePosition = outsideHit - chunkPosition;
            var insidePosition = insideHit - chunkPosition;
            if (selectedPosition != insidePosition)
            {
                selectedPosition = insidePosition;
                selectedBlockChanged();
            }
            return true;
        }
    }
}