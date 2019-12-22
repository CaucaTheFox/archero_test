using Core.IoC;
using UnityEngine;

namespace Features.Heroes
{
    public class Hero : InjectableBehaviour
    {
        #region Unity Serialized Fields
        [SerializeField] private float walkingSpeed;
        [SerializeField] private Animator animator;
        #endregion

        #region Dependencies
        #endregion

        #region Properties
        public HeroSettings Settings {get;set;}
        private Vector3 refVelocity;
        #endregion

        #region Lifecycle
        private void Start()
        {

        }
        private void OnDestroy()
        {

        }
        #endregion

        #region Public
        public void MoveCharacter(Vector3 joyStickInput)
        {
            transform.Translate(joyStickInput * 7f * Time.deltaTime, Space.World);
            transform.forward = joyStickInput;
            animator.SetTrigger("Run");
        }

        public void Shoot()
        {
            animator.SetTrigger("Idle");
            animator.SetTrigger("Arrow Attack");
        }
        #endregion

        #region Private
        #endregion
    }
}
