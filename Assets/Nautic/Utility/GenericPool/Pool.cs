using System.Collections.Generic;
using UnityEngine;

/* *
 * Static pool class. 
 * Dictionary Pools is holder for all pools in the szene. 
 * Objects to pool need to derivate from PooledMonobehaviour.
 * */

namespace Utility.Pooling
{
    public class Pool : MonoBehaviour
    {
        // Static holder for all Pools. Prefab as key for a queue that stores his instances.
        private static Dictionary<PooledMonobehaviour, Pool> Pools = new Dictionary<PooledMonobehaviour, Pool>();
        // All instances for own prefab.
        private Queue<PooledMonobehaviour> m_Objects = new Queue<PooledMonobehaviour>();
        // All instances of the prefab that were in use and need to redirect in hierachy.
        private List<PooledMonobehaviour> m_DisabledObjects = new List<PooledMonobehaviour>();
        // The pooled prefab.
        private PooledMonobehaviour m_Prefab;
        // The partentobject for all created pools in hierachy.
        private static GameObject m_PoolParent;

        private void Update()
        {
            //MakeDisabledObjectsChildren();
        }

        /* *
         * Ask for the pool from given prefab. If pool dont exist create pool, name it and redirect it to pool parentobject.
         * If there is no pool pool parentobject need to get initialized.
         * */
        public static Pool GetPool(PooledMonobehaviour prefab)
        {
            if (Pools.Count == 0)
            {
                m_PoolParent = GameObject.Find("Pool");
                if (m_PoolParent == null)
                    m_PoolParent = new GameObject("Pool");
            }

            if (Pools.ContainsKey(prefab))
                return Pools[prefab];


            Pool pool = new GameObject("Pool - " + prefab.name).AddComponent<Pool>();
            pool.transform.SetParent(m_PoolParent.transform);
            pool.m_Prefab = prefab;

            Pools.Add(prefab, pool);
            return pool;
        }

        /* *
         * Get a instance ob the prefab from this pool.
         * If there is no instance, let the pool grow up to in prefab given size.
         * */
        public T Get<T>() where T : PooledMonobehaviour
        {
            if (m_Objects.Count == 0)
            {
                GrowPool();
            }

            PooledMonobehaviour obj = m_Objects.Dequeue();
            return (T)obj;
        }

        /* *
         * Let pool grow up to from prefab given growsize.
         * */
        public void GrowPool()
        {
            GrowPool(m_Prefab.m_GrowSize);
        }

        /* *
         * Let pool grow up to the given poolGrow.
         * */
        public void GrowPool(int poolGrow)
        {
            for (int i = 0; i < poolGrow; i++)
            {
                PooledMonobehaviour pooledObject = Instantiate(m_Prefab, transform);

                pooledObject.OnDestroyEvent += () => AddObjectToAvailable(pooledObject);

                pooledObject.Release();
            }
        }

        /* *
         * Set instance of prefab back to usable prefabs.
         * */
        private void AddObjectToAvailable(PooledMonobehaviour pooledObject)
        {
            m_DisabledObjects.Add(pooledObject);
            m_Objects.Enqueue(pooledObject);
        }

        /* *
         * Redirect instances of prefab back to pool parent after they were in use.
         * */
        private void MakeDisabledObjectsChildren()
        {
            if (m_DisabledObjects.Count > 0)
            {
                foreach (PooledMonobehaviour pooledObject in m_DisabledObjects)
                {
                    if (pooledObject.gameObject.activeInHierarchy == false)
                    {
                        pooledObject.transform.SetParent(transform);
                    }
                }
                m_DisabledObjects.Clear();
            }
        }

    }
}
