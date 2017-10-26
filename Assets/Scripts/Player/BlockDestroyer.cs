using Level;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(BlockInteractor))]
    public class BlockDestroyer : MonoBehaviour
    {
        [SerializeField] float damage = 1f;
        
        BlockInteractor interactor;
        
        public float blockHeath { get; private set; }
        public float healthRemains { get; private set; }
        public bool isDestroying { get; private set; }
        
        void OnEnable()
        {
            interactor = GetComponent<BlockInteractor>();
            interactor.selectedBlockChanged += OnSelectedBlockChanged;
        }

        void OnDisable()
        {
            interactor.selectedBlockChanged -= OnSelectedBlockChanged;
        }
        
        void OnSelectedBlockChanged()
        {
            isDestroying = false;
        }

        void Hit()
        {
            healthRemains -= Time.deltaTime*damage;
            if (healthRemains <= 0)
            {
                interactor.selectedChunk.DestroyBlock(interactor.selectedPosition);
                isDestroying = false;
            }
        }
        
        void Update()
        {
            if (Input.GetMouseButton(0) && interactor.isBlockSelected)
            {
                if (isDestroying)
                {
                    Hit();
                    return;
                }
                isDestroying = true;
                var block = interactor.selectedChunk.GetBlock(interactor.selectedPosition);
                blockHeath = Block.health[block.type];
                healthRemains = blockHeath;
            }
            else
            {
                isDestroying = false;
            }
            
        }

    }
}