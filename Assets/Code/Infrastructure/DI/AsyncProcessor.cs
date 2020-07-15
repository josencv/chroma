using UnityEngine;

namespace Chroma.Infrastructure.DI
{
    /// <summary>
    /// Used to initialize objects in Zenject that do not inherit from MonoBehaviour, but requires
    /// an async initialization.
    /// The technique is describes in Zenject's FAQ section under "How do I use Unity style Coroutines in normal C# classes?".
    /// See https://github.com/modesttree/Zenject#frequently-asked-questions
    /// </summary>
    public class AsyncProcessor : MonoBehaviour
    {
    }
}
