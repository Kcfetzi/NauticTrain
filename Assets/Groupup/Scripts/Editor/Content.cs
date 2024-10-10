using UnityEngine;

namespace Groupup
{
    public abstract class Content
    {
        protected Rect WindowPosition;

        public abstract void DrawContent(Rect position);
    }
}
