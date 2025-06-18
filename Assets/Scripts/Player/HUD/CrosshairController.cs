using System.Collections;
using UnityEngine;

namespace Player.HUD
{
    public enum CrosshairScale
    {
        Default,
        Cast
    }

    public class CrosshairController : MonoBehaviour
    {
        [SerializeField] private Vector3 defaultScale = new(0.01f, 0.01f, 0.01f);
        [SerializeField] private Vector3 castScale = new(0.02f, 0.02f, 0.02f);
        [SerializeField] private float scaleSpeed = 2f;

        private CrosshairScale currentScale;

        private void Start()
        {
            transform.localScale = defaultScale;
            currentScale = CrosshairScale.Default;
        }

        private void Update()
        {
            var newScale = currentScale switch
            {
                CrosshairScale.Cast => castScale,
                _ => defaultScale
            };

            transform.localScale = Vector3.Lerp(transform.localScale, newScale, scaleSpeed * Time.deltaTime);
        }

        public void SetScale(CrosshairScale scale)
        {
            currentScale = scale;
        }

        public void SetScale(CrosshairScale scale, float duration)
        {
            if (!isActiveAndEnabled) return;
            currentScale = scale;
            StartCoroutine(ResetCrosshair(duration));
        }

        private IEnumerator ResetCrosshair(float duration)
        {
            yield return new WaitForSeconds(duration);
            currentScale = CrosshairScale.Default;
        }
    }
}