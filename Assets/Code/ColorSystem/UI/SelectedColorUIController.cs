using UnityEngine;
using Zenject;

namespace Chroma.ColorSystem.UI
{
    public class SelectedColorUIController : MonoBehaviour
    {
        private SelectedColorBackgroundUI background;

        [Inject]
        private void Inject(SelectedColorBackgroundUI background)
        {
            this.background = background;
        }

        public void ChangeSelectedColor(Color color)
        {
            background.ChangeBackgroundColor(color);
        }
    }
}
