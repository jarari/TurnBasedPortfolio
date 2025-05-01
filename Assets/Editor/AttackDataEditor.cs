using TurnBased.Battle;
using UnityEditor;

namespace TurnBased.Editor {
    [CustomEditor(typeof(AttackData))]
    public class AttackDataEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            serializedObject.Update();
            DrawDefaultInspector();

            var data = (AttackData)target;
            if (data.damageMult?.Count != data.toughnessDamage?.Count) {
                EditorGUILayout.HelpBox(
                    "데미지 배율과 강인도 데미지의 개수가 일치하지 않습니다!",
                    MessageType.Warning
                );
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
