using UnityEngine;

namespace Chroma.Tests
{
    public class TestWalkRunBlendTree : MonoBehaviour
    {
        #region Private fields

        [SerializeField, Tooltip("Pess 'A' to toggle Maintain Speed.")] private Animator anim;
        [SerializeField] private bool maintainSpeed = false;
        [SerializeField, Tooltip("Press the Right arrow key to play animation."), Range(0.0f, 1.0f)] private float speed = 0.0f;

        #endregion

        #region Unity

        private void Start()
        {
            Debug.Log("To test the animation, press the <b>RIGHT</b> arrow key.");
            Debug.Log("To toggle Maintain Speed, press the <b>A</b> key.");

            if(anim == null)
            {
                anim = GetComponent<Animator>();
            }
        }

        private void Update()
        {
            SetSpeed();
            ToggleMaintainSpeed();
            anim.SetFloat(Constants.blend, speed);
        }

        #endregion

        #region Private methods

        private void SetSpeed()
        {
            float input = Input.GetAxis(Constants.horizontal);
            
            if(maintainSpeed)
            {
                speed += input / 100f;
                speed = Mathf.Clamp01(speed);
            }
            else
            {
                speed = Input.GetAxis(Constants.horizontal);
            }
        }

        private void ToggleMaintainSpeed()
        {
            if(Input.GetKeyDown(KeyCode.A))
            {
                maintainSpeed = !maintainSpeed;
            }
        }

        #endregion
    }

    #region Dont Worry About This :D
    internal class Constants
    {
        public const string blend = "Blend";
        public const string horizontal = "LeftStickHorizontal";
    }
    #endregion
}
