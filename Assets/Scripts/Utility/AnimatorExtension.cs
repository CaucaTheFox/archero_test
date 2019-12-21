using System.Linq;
using UnityEngine;

namespace Utility
{
    public static class AnimatorExtension
    {
        public static float AnimationLength(this RuntimeAnimatorController animatorController, string animationName) =>
            animatorController.animationClips
                .Where(clip => clip.name == animationName)
                .Select(clip => clip.length)
                .FirstOrDefault();

        public static float AnimationLength(this RuntimeAnimatorController animatorController, int animationHash) =>
            animatorController.animationClips
                .Where(clip => Animator.StringToHash(clip.name) == animationHash)
                .Select(clip => clip.length)
                .FirstOrDefault();
    }
}