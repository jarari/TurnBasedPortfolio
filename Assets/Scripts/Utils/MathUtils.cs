using UnityEngine;

namespace TurnBased.Utils {
    public static class MathUtils {
        public static float EaseInOutSine(float x) {
            return -(Mathf.Cos(Mathf.PI * x) - 1) / 2;
        }

        public static float EaseOutSine(float x) {
            return Mathf.Sin((x * Mathf.PI) / 2);
        }
    }
}
