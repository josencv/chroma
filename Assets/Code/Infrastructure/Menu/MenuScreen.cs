using UnityEngine;

namespace Chroma.Infrastructure.Menu
{
    [RequireComponent(typeof(Canvas))]
    public abstract class MenuScreen : MonoBehaviour
    {
        protected new string name;

        public string Name { get { return name; } }
    }
}
