using System;
using UnityEngine;

/* *
 * Components that derivate from this, are poolable.
 * */

namespace Utility.Pooling
{
    public class PooledMonobehaviour : MonoBehaviour
    {
        [Header("Poolingoptions")]
        [Tooltip("If the pool is empty, how much should it grow?")]
        [SerializeField] private int GrowSize = 1;

        private bool m_ReadyToReuse;
        public int m_GrowSize { get { return GrowSize; } }
        // From pool class given destroyevent to trigger pooling by disabling Gameobject. 
        public event Action OnDestroyEvent;


        /* *
         * Mark this instance as reusable
         * */
        public void Release(bool enable = false)
        {
            if (!enable)
            {
                gameObject.SetActive(false);
            }
            OnDestroyEvent?.Invoke();
        }

        /* *
         * Get an instance of this prefab from pool.
         * If enable is set, the gameobject will be set to active otherwise it will be inactive.
         * */
        public T Get<T>(bool enable = true) where T : PooledMonobehaviour
        {
            Pool pool = Pool.GetPool(this);
            PooledMonobehaviour pooledObject = pool.Get<T>();

            if (enable)
            {
                pooledObject.gameObject.SetActive(true);
            }

            return (T)pooledObject;
        }


        /* *
         * Get an instance of this prefab from pool.
         * Object will be redirected in hierachy under given parentobject. If resetTransform is set prefab will be and position and rotation zero.
         * */
        public T Get<T>(Transform parent, bool resetTransform = false) where T : PooledMonobehaviour
        {
            PooledMonobehaviour pooledObject = Get<T>(true);
            pooledObject.transform.SetParent(parent);

            if (resetTransform)
            {
                pooledObject.transform.position = Vector3.zero;
                pooledObject.transform.rotation = Quaternion.identity;
            }

            return (T)pooledObject;
        }

        /* *
         * Get an instance of this prefab from pool.
         * Object will be redirected in hierachy under given parentobject. Position and rotation set to given parameters.
         * */
        public T Get<T>(Transform parent, Vector3 relativePosition, Quaternion relativeRotation) where T : PooledMonobehaviour
        {
            PooledMonobehaviour pooledObject = Get<T>(true);
            pooledObject.transform.SetParent(parent);

            pooledObject.transform.localPosition = relativePosition;
            pooledObject.transform.localRotation = relativeRotation;

            return (T)pooledObject;
        }
    }
}
