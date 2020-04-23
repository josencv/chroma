using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Chroma.ColorSystem.UI
{
    [RequireComponent(typeof(Image))]
    public class SelectedColorBackgroundUI : MonoBehaviour
    {
        private Image image;

        [Inject]
        private void Inject(Image image)
        {
            this.image = image;
        }

        public void ChangeBackgroundColor(Color color)
        {
            image.color = GetFillColor(color);
        }

        private UnityEngine.Color GetFillColor(Color color)
        {
            UnityEngine.Color fillColor;

            switch(color)
            {
                case Color.Red:
                    fillColor = UnityEngine.Color.red;
                    break;
                case Color.Yellow:
                    fillColor = UnityEngine.Color.yellow;
                    break;
                case Color.Green:
                    fillColor = UnityEngine.Color.green;
                    break;
                case Color.Cyan:
                    fillColor = UnityEngine.Color.cyan;
                    break;
                case Color.Blue:
                    fillColor = UnityEngine.Color.blue;
                    break;
                case Color.Magenta:
                    fillColor = UnityEngine.Color.magenta;
                    break;
                default:
                    fillColor = UnityEngine.Color.white;
                    break;
            }

            return fillColor;
        }
    }
}
