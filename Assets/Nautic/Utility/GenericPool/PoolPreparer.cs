using UnityEngine;

namespace Utility.Pooling
{
    // Struct to make an entry in Poolpreparer.
    [System.Serializable]
    struct PoolEntry
    {
        public PooledMonobehaviour m_Prefab;
        public int m_PoolSize;
    }

    /* *
     * This class can prewarm pools for the in m_PrewarmedObjects given prefabs.
     * */
    public class PoolPreparer : MonoBehaviour
    {
        [Header("Objects to prewarm the pool for")]
        [SerializeField] PoolEntry[] m_PrewarmedObjects;

        private void Awake()
        {
            foreach (PoolEntry entry in m_PrewarmedObjects)
            {
                if (entry.m_Prefab == null)
                {
                    Debug.LogError("Null prefab in PoolPreparer");
                }
                else
                {
                    PooledMonobehaviour poolablePrefab = entry.m_Prefab.GetComponent<PooledMonobehaviour>();
                    if (poolablePrefab == null)
                    {
                        Debug.LogError("Prefab does not contain a PooledMonobehaviour and cant be pooled");
                    }
                    else
                    {
                        Pool.GetPool(poolablePrefab).GrowPool(entry.m_PoolSize);
                    }
                }
            }
        }
    }
}