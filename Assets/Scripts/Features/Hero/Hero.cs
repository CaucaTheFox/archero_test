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
            transform.Translate(joyStickInput * walkingSpeed * Time.deltaTime, Space.World);
            if (joyStickInput != Vector3.zero)
            {
                transform.forward = joyStickInput;
            }
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
