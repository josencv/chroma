using UnityEngine;

namespace Chroma.Tests
{
    public class TestWalkRunBlendTree : MonoBehaviour
    {
        #region Private fields

        [SerializeField, Tooltip("Pess 'A' to toggle Maintain Speed.")] private Animator _anim;
        [SerializeField] private bool _maintainSpeed = false;
        [SerializeField, Tooltip("Press the Right arrow key to play animation."), Range(0.0f, 1.0f)] private float _speed = 0.0f;

        #endregion

        #region Unity

        private void Start()
        {
            Debug.Log("To test the animation, press the <b>RIGHT</b> arrow key.");
            Debug.Log("To toggle Maintain Speed, press the <b>A</b> key.");

            if(_anim == null)
            {
                _anim = GetComponent<Animator>();
            }
        }

        private void Update()
        {
            SetSpeed();
            ToggleMaintainSpeed();
            _anim.SetFloat(Constants.blend, _speed);
        }

        #endregion

        #region Private methods

        private void SetSpeed()
        {
            float input = Input.GetAxis(Constants.horizontal);
            
            if(_maintainSpeed)
            {
                _speed += input / 100f;
                _speed = Mathf.Clamp01(_speed);
            }
            else
            {
                _speed = Input.GetAxis(Constants.horizontal);
            }
        }

        private void ToggleMaintainSpeed()
        {
            if(Input.GetKeyDown(KeyCode.A))
            {
                _maintainSpeed = !_maintainSpeed;
            }
        }

        #endregion
    }

    #region Dont Worry About This :D
    internal class Constants
    {
        public const string blend = "Blend";
        public const string horizontal = "Horizontal";
    }
    #endregion
}
