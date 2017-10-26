using Level;
using UnityEngine;
using Utils;

namespace Player
{
    [RequireComponent(typeof(BlockInteractor))]
    public class BlockCursor : MonoBehaviour
    {
        [SerializeField] GameObject blockCursorPrefab;

        GameObject blockCursor;
        MeshFilter meshFilter;
        BlockInteractor interactor;
        BlockBuilder builder;

        void OnEnable()
        {
            if (!blockCursor)
            {
                blockCursor = Instantiate(blockCursorPrefab);
                blockCursor.name = "BlockCursor";
                blockCursor.SetActive(false);
                meshFilter = blockCursor.GetComponentInChildren<MeshFilter>();
                MeshBuilder.BuildBlock(new Vector3(-0.5f, -0.5f, -0.5f), meshFilter.mesh);
            }
            interactor = GetComponent<BlockInteractor>();
            interactor.selectedBlockChanged += OnSelectedBlockChanged;
            
            builder = GetComponent<BlockBuilder>();
            if (builder)
            {
                builder.blockTypeSelected += OnSelectedBlockChanged;
            }
        }

        void OnDisable()
        {
            if (blockCursor) blockCursor.SetActive(false);
            interactor.selectedBlockChanged -= OnSelectedBlockChanged;
            
            if (builder)
            {
                builder.blockTypeSelected -= OnSelectedBlockChanged;
            }
        }

        void OnSelectedBlockChanged()
        {
            if (interactor.isBlockSelected)
            {
                blockCursor.SetActive(true);
                blockCursor.transform.position = interactor.selectedChunk.chunkPosition + interactor.selectedPosition;

                var blockType = builder
                    ? builder.blockTypeToBuild
                    : interactor.selectedChunk.GetBlock(interactor.selectedPosition).type;

                MeshBuilder.ChangeBlockUVs(meshFilter.mesh, Block.tiles[blockType]);
            }
            else
            {
                blockCursor.SetActive(false);
            }
        }
    }
}