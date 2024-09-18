using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Object), true)]
public class RatioInspectorEditor : Editor
{
    private const float COLOR_BAR_HEIGHT = 50f;
    private static readonly Color TEXT_COLOR = new Color(0.55f, 0.55f, 0.55f, 1f);

    private bool _showThisItem;
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
        SceneView.RepaintAll();
        
    }

    private bool HasRatioAttribute(SerializedProperty property)
    {
        FieldInfo field = serializedObject.targetObject.GetType().GetField(property.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        return field != null && field.GetCustomAttribute<RatioAttribute>() != null;
    }

    private void DrawRatioBar(SerializedProperty property)
    {
        FieldInfo field = serializedObject.targetObject.GetType().GetField(property.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        RatioAttribute ratioAttribute = (RatioAttribute)field.GetCustomAttribute(typeof(RatioAttribute));

        // �z��̒l���擾���A0-1�͈̔͂ɐ��K��
        float[] values = (float[])field.GetValue(serializedObject.targetObject);
        if (values == null)
        {
            values = new float[0];
        }
        float[] normalizedValues = NormalizeValues(values, ratioAttribute.Min, ratioAttribute.Max);
        float total = Mathf.Max(1f, normalizedValues.Sum());

        // �t�B�[���h����\��
        _showThisItem = EditorGUILayout.Foldout(_showThisItem, ObjectNames.NicifyVariableName(property.name));

        if (!_showThisItem) { return; }

        // �J���[�o�[�̍�����ݒ�
        Rect barRect = GUILayoutUtility.GetRect(0, COLOR_BAR_HEIGHT);
        float currentX = barRect.x;
        float width = barRect.width;

        Event currentEvent = Event.current;
        Vector2 mousePosition = currentEvent.mousePosition;

        for (int i = 0; i < normalizedValues.Length; i++)
        {
            float ratioValue = normalizedValues[i] / total;
            float elementWidth = width * ratioValue;

            Color color = Color.HSVToRGB(i / (float)normalizedValues.Length, 1f, 1f);
            Rect elementRect = new Rect(currentX, barRect.y, elementWidth, barRect.height);
            EditorGUI.DrawRect(elementRect, color);

            // �J���[�o�[�Ɍ��݂̊�����\�� (100%�\�L)
            string percentageText = (normalizedValues[i] * 100f / total).ToString("F1") + "%";
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.LowerCenter,
                normal = new GUIStyleState { textColor = TEXT_COLOR }
            };

            Rect textRect = new Rect(currentX, barRect.y + COLOR_BAR_HEIGHT * 0.5f, elementWidth, barRect.height);
            EditorGUI.LabelField(textRect, percentageText, labelStyle);

            // �J���[�o�[��I�����ē�������悤�ɂ���
            if (i < normalizedValues.Length - 1)
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

                normalizedValues[_draggedIndex] = Mathf.Clamp01(normalizedValues[_draggedIndex] + delta);
                normalizedValues[_draggedIndex + 1] = Mathf.Clamp01(normalizedValues[_draggedIndex + 1] - delta);

                field.SetValue(serializedObject.targetObject, DenormalizeValues(normalizedValues, ratioAttribute.Min, ratioAttribute.Max));
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
            if (originalValues == null)
            {
                originalValues = new float[0];
            }

            float[] normalizedOriginalValues = NormalizeValues(originalValues, ratioAttribute.Min, ratioAttribute.Max);

            // ���v���v�Z
            float totalValues = normalizedOriginalValues.Sum();
            if (totalValues <= 0f)
                totalValues = 1f;

            for (int i = 0; i < normalizedOriginalValues.Length; i++)
            {
                // ���ׂẴX���C�_�[��\�� (100%�\�L)
                float newValue = EditorGUILayout.Slider(
                    ObjectNames.NicifyVariableName(property.name) + " " + (i + 1),
                    normalizedOriginalValues[i] * 100f,
                    0f,
                    100f
                );
                normalizedOriginalValues[i] = newValue / 100f;
            }

            // 100%�ɂȂ�悤�ɒ���
            float newTotal = normalizedOriginalValues.Sum();
            if (newTotal > 0f)
            {
                for (int i = 0; i < normalizedOriginalValues.Length; i++)
                {
                    normalizedOriginalValues[i] = Mathf.Clamp01(normalizedOriginalValues[i] / newTotal);
                }
            }

            field.SetValue(serializedObject.targetObject, DenormalizeValues(normalizedOriginalValues, ratioAttribute.Min, ratioAttribute.Max));
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

    private void AddElement(SerializedProperty property, RatioAttribute ratioAttribute)
    {
        FieldInfo field = serializedObject.targetObject.GetType().GetField(property.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        float[] values = (float[])field.GetValue(serializedObject.targetObject);
        if (values == null)
        {
            values = new float[0];
        }
        float[] normalizedValues = NormalizeValues(values, ratioAttribute.Min, ratioAttribute.Max);
        float total = Mathf.Max(1f, normalizedValues.Sum());

        // ���������ʂ̗v�f��������Ă��炤
        float[] newNormalizedValues = new float[normalizedValues.Length + 1];
        float newElementRatio = 0.1f; // 10%
        float adjustment = newElementRatio / (normalizedValues.Length > 0 ? normalizedValues.Length : 1);

        for (int i = 0; i < normalizedValues.Length; i++)
        {
            newNormalizedValues[i] = Mathf.Clamp01(normalizedValues[i] - adjustment);
        }
        newNormalizedValues[normalizedValues.Length] = newElementRatio;

        field.SetValue(serializedObject.targetObject, DenormalizeValues(newNormalizedValues, ratioAttribute.Min, ratioAttribute.Max));
        serializedObject.Update();
    }

    private void RemoveElement(SerializedProperty property, RatioAttribute ratioAttribute)
    {
        FieldInfo field = serializedObject.targetObject.GetType().GetField(property.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        float[] values = (float[])field.GetValue(serializedObject.targetObject);
        if (values == null)
        {
            values = new float[0];
        }
        float[] normalizedValues = NormalizeValues(values, ratioAttribute.Min, ratioAttribute.Max);

        if (normalizedValues.Length <= 1) return;

        // �v�f���폜���Ďc���100%�ɂȂ�悤�ɖ��߂�
        float[] newNormalizedValues = new float[normalizedValues.Length - 1];
        float removedRatio = normalizedValues[normalizedValues.Length - 1];
        float distribution = removedRatio / (newNormalizedValues.Length > 0 ? newNormalizedValues.Length : 1);

        for (int i = 0; i < newNormalizedValues.Length; i++)
        {
            newNormalizedValues[i] = Mathf.Clamp01(normalizedValues[i] + distribution);
        }

        field.SetValue(serializedObject.targetObject, DenormalizeValues(newNormalizedValues, ratioAttribute.Min, ratioAttribute.Max));
        serializedObject.Update();
    }

    // �l��0-1�͈̔͂ɐ��K������
    private float[] NormalizeValues(float[] values, float min, float max)
    {
        return values.Select(v => Mathf.InverseLerp(min, max, v)).ToArray();
    }

    // ���K�����ꂽ�l�����͈̔͂ɖ߂�
    private float[] DenormalizeValues(float[] normalizedValues, float min, float max)
    {
        return normalizedValues.Select(v => Mathf.Lerp(min, max, v)).ToArray();
    }
}