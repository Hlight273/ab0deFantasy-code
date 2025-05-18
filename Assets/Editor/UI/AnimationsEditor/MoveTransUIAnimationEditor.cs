using UnityEngine;
using UnityEditor;
using HFantasy.Script.UI.MainMenu.Animations;

namespace HFantasy.Editor.UI
{
    [CustomEditor(typeof(MoveTransUIAnimation))]
    public class MoveTransUIAnimationEditor : UnityEditor.Editor
    {
        private void OnSceneGUI()
        {
            MoveTransUIAnimation animation = (MoveTransUIAnimation)target;
            var configs = animation.GetType().GetField("buttonConfigs", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(animation) as MoveTransUIAnimation.ButtonAnimationConfig[];
            
            if (configs == null) return;

            Handles.color = Color.green;
            foreach (var config in configs)
            {
                if (config.rectTransform == null) continue;

                Vector3 position = config.rectTransform.position;
                float angle = config.moveAngle;
                
                //���Ʒ���ָʾ��
                Handles.DrawWireArc(position, Vector3.forward, Vector3.right, angle, 50);
                Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.right;
                Handles.DrawLine(position, position + direction * 100);
                
                //��ӽǶȿ����ֱ�
                EditorGUI.BeginChangeCheck();
                angle = Handles.RadiusHandle(Quaternion.identity, position, 50, false);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(animation, "Change Move Angle");
                    config.moveAngle = angle;
                    EditorUtility.SetDirty(animation);
                }
            }
        }
    }
}