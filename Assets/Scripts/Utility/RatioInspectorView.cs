using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonoBehaviour), true)]
public class RatioInspectorEditor : Editor
{
    private const float COLOR_BAR_HEIGHT = 50f;
    private static readonly Color TEXT_COLOR = new Color(140, 140, 140, 1);

    private bool _isDragging;
    private bool _showSliders;
    private int _draggedIndex = -1;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty property = serializedObject.GetIterator();
        property.Next(true);

        while (property.NextVisible(false))
        {
            if (HasRatioAttribute(property))
            {
                DrawRatioBar(property);
            }
            else
            {
                EditorGUILayout.PropertyField(property, true);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    private bool HasRatioAttribute(SerializedProperty property)
    {
        FieldInfo field = serializedObject.targetObject.GetType().GetField(property.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        return field != null && field.GetCustomAttribute<RatioAttribute>() != null;
    }

    private void DrawRatioBar(SerializedProperty property)
    {
        FieldInfo field
            = serializedObject
            .targetObject
            .GetType()
            .GetField
            (
                property.name,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );
        RatioAttribute ratioAttribute
            = (RatioAttribute)field
            .GetCustomAttribute(typeof(RatioAttribute));

        // �z��̒l���擾
        float[] values = (float[])field.GetValue(serializedObject.targetObject);
        float total = Mathf.Max(1, values.Sum());

        // �t�B�[���h����\��
        EditorGUILayout.LabelField
        (
            ObjectNames.NicifyVariableName(property.name),
            EditorStyles.boldLabel
        );

        // �J���[�o�[�̍�����30�ɐݒ�
        Rect barRect = GUILayoutUtility.GetRect(0, COLOR_BAR_HEIGHT);
        float currentX = barRect.x;
        float width = barRect.width;

        Event currentEvent = Event.current;
        Vector2 mousePosition = currentEvent.mousePosition;

        for (int i = 0; i < values.Length; i++)
        {
            float ratioValue = values[i] / total;
            float elementWidth = width * ratioValue;

            Color color = Color.HSVToRGB(i / (float)values.Length, 1f, 1f);
            Rect elementRect = new Rect(currentX, barRect.y, elementWidth, barRect.height);
            EditorGUI.DrawRect(elementRect, color);

            // �J���[�o�[�Ɍ��݂̊�����\��
            string percentageText = (values[i] * 100f / total).ToString("F1") + "%";
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.LowerCenter,
                normal = new GUIStyleState { textColor = Color.black }
            };

            // �F��ݒ�
            labelStyle.normal.textColor = TEXT_COLOR;

            Rect textRect = new Rect(currentX, barRect.y + COLOR_BAR_HEIGHT * 0.5f, elementWidth, barRect.height);
            EditorGUI.LabelField(textRect, percentageText, labelStyle);

            // �J���[�o�[��I�����ē�������悤�ɂ���
            if (i < values.Length - 1)
            {
                Rect dragRect = new Rect(currentX + elementWidth - 2f, barRect.y, 4f, barRect.height);
                EditorGUIUtility.AddCursorRect(dragRect, MouseCursor.ResizeHorizontal);

                if (currentEvent.type == EventType.MouseDown && dragRect.Contains(mousePosition))
                {
                    _draggedIndex = i;
                    _isDragging = true;
                    currentEvent.Use();
                }
            }

            if (currentEvent.type == EventType.MouseDrag && _isDragging && _draggedIndex == i)
            {
                float delta = currentEvent.delta.x / barRect.width * total;

                values[_draggedIndex] = Mathf.Clamp(values[_draggedIndex] + delta, ratioAttribute.Min, ratioAttribute.Max);
                values[_draggedIndex + 1] = Mathf.Clamp(values[_draggedIndex + 1] - delta, ratioAttribute.Min, ratioAttribute.Max);

                field.SetValue(serializedObject.targetObject, values);
                serializedObject.Update();

                currentEvent.Use();
            }

            if (currentEvent.type == EventType.MouseUp)
            {
                _isDragging = false;
                _draggedIndex = -1;
            }

            currentX += elementWidth;
        }

        EditorGUILayout.Space(COLOR_BAR_HEIGHT);

        // �ڍׂ̐ݒ�
        _showSliders = EditorGUILayout.Foldout(_showSliders, "�����̏ڍ�");

        // �J����Ă���Ƃ��Ɍv�Z������
        if (_showSliders)
        {
            EditorGUI.indentLevel++;
            float[] originalValues = (float[])field.GetValue(serializedObject.targetObject);

            // ���v���v�Z
            float totalValues = originalValues.Sum();
            if (totalValues <= 0f)
                totalValues = 1f;

            for (int i = 0; i < values.Length; i++)
            {
                // ���ׂẴX���C�_�[��\��
                float newValue
                    = EditorGUILayout.Slider
                    (
                        ObjectNames.NicifyVariableName(property.name)
                            + " " + (i + 1), originalValues[i],
                        ratioAttribute.Min,
                        ratioAttribute.Max
                    );
                originalValues[i] = newValue;
            }

            // 100%�ɂȂ�悤�ɒ���
            float newTotal = originalValues.Sum();
            if (newTotal > 0f)
            {
                for (int i = 0; i < originalValues.Length; i++)
                {
                    originalValues[i] = Mathf.Clamp
                    (
                        (originalValues[i] / newTotal) * 100f,
                        ratioAttribute.Min,
                        ratioAttribute.Max
                    );
                }
            }

            field.SetValue(serializedObject.targetObject, originalValues);
            serializedObject.Update();

            EditorGUI.indentLevel--;
        }

        // �ǉ��ƍ폜�̃{�^����\��
        if (ratioAttribute.AllowEdit)
        {
            EditorGUILayout.Space();
            if (GUILayout.Button("�ǉ�"))
            {
                AddElement(property, ratioAttribute);
            }
            if (GUILayout.Button("�폜"))
            {
                RemoveElement(property, ratioAttribute);
            }
        }
    }

    /// <summary>
    /// �v�f��ǉ�����{�^��
    /// </summary>
    private void AddElement(SerializedProperty property, RatioAttribute ratioAttribute)
    {
        FieldInfo field
            = serializedObject
            .targetObject
            .GetType()
            .GetField
            (
                property.name,
        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );
        float[] values = (float[])field.GetValue(serializedObject.targetObject);
        float total = Mathf.Max(1, values.Sum());

        // ���������ʂ̗v�f��������Ă��炤
        float[] newValues = new float[values.Length + 1];
        float newElementRatio = 10f; // MN=10%
        float adjustment = newElementRatio / 100f * total;

        for (int i = 0; i < values.Length; i++)
        {
            newValues[i]
                = Mathf.Clamp
                (
                    values[i] - adjustment,
                    ratioAttribute.Min,
                    ratioAttribute.Max
                );
        }
        newValues[values.Length] = newElementRatio;

        field.SetValue(serializedObject.targetObject, newValues);
        serializedObject.Update();
    }

    private void RemoveElement(SerializedProperty property, RatioAttribute ratioAttribute)
    {
        FieldInfo field
            = serializedObject
            .targetObject
            .GetType()
            .GetField
            (
                property.name,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );
        float[] values = (float[])field.GetValue(serializedObject.targetObject);

        if (values.Length <= 1) return;

        // �v�f���폜���Ďc���100%�ɂȂ�悤�ɖ��߂�
        float[] newValues = new float[values.Length - 1];
        float removedRatio = values[values.Length - 1];
        float distribution = removedRatio / (newValues.Length > 0 ? newValues.Length : 1);

        for (int i = 0; i < newValues.Length; i++)
        {
            newValues[i] = Mathf.Clamp(values[i] + distribution, ratioAttribute.Min, ratioAttribute.Max);
        }

        field.SetValue(serializedObject.targetObject, newValues);
        serializedObject.Update();
    }
}