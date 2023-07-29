using System.Reflection;
using System;

namespace ModSettings
{
    public class ControlStructs
    {
        public struct Label
        {
            public Label(string text)
            {
                Text = text;
                DynamicText = null;
            }
            public Label(FieldInfo dynamicText)
            {
                DynamicText = dynamicText;
                Text = null;
            }

            public FieldInfo? DynamicText { get; }
            public string? Text { get; }
        }

        public struct Button
        {
            public Button(string text, MethodInfo buttonMethod, object[] methodParameters)
            {
                Text = text;
                ButtonMethod = buttonMethod;
                MethodParameters = methodParameters;
            }
            public Button(string text, MethodInfo buttonMethod)
            {
                Text = text;
                ButtonMethod = buttonMethod;
                MethodParameters = new object[0];
            }

            public string Text { get; }
            public MethodInfo ButtonMethod { get; }
            public object[] MethodParameters { get; }
        }

        public struct RepeatButton
        {
            public RepeatButton(string text, MethodInfo buttonMethod, object[] methodParameters)
            {
                Text = text;
                ButtonMethod = buttonMethod;
                MethodParameters = methodParameters;
            }
            public RepeatButton(string text, MethodInfo buttonMethod)
            {
                Text = text;
                ButtonMethod = buttonMethod;
                MethodParameters = new object[0];
            }

            public string Text { get; }
            public MethodInfo ButtonMethod { get; }
            public object[] MethodParameters { get; }
        }

        public struct TextField
        {
            public TextField(FieldInfo field)
            {
                if (field.FieldType == typeof(string))
                {
                    Field = field;
                }
                else
                {
                    throw new ArgumentException($"Field passed into TextField is {field.FieldType}, expected string");
                }
            }

            public FieldInfo Field { get; }
        }

        public struct TextArea
        {
            public TextArea(FieldInfo field)
            {
                if (field.FieldType == typeof(string))
                {
                    Field = field;
                }
                else
                {
                    throw new ArgumentException($"Field passed into TextArea is {field.FieldType}, expected string");
                }
            }

            public FieldInfo Field { get; }
        }

        public struct Toggle
        {
            public Toggle(FieldInfo field, string name)
            {
                if (field.FieldType == typeof(bool))
                {
                    Field = field;
                }
                else
                {
                    throw new ArgumentException($"Field passed into Toggle is {field.FieldType}, expected bool");
                }

                Name = name;
            }

            public FieldInfo Field { get; }
            public string Name { get; }
        }

        public struct Toolbar
        {
            public Toolbar(FieldInfo field, string[] toolbarStrings)
            {
                if (field.FieldType == typeof(int) || field.FieldType.IsEnum)
                {
                    Field = field;
                }
                else
                {
                    throw new ArgumentException($"Field passed into Toolbar is {field.FieldType}, expected int or enum object");
                }

                ToolbarStrings = toolbarStrings;
            }

            public FieldInfo Field { get; }
            public string[] ToolbarStrings { get; }
        }

        public struct SelectionGrid
        {
            public SelectionGrid(FieldInfo field, string[] selectionStrings, int columnNumber)
            {
                if (field.FieldType == typeof(int) || field.FieldType.IsEnum)
                {
                    Field = field;
                }
                else
                {
                    throw new ArgumentException($"Field passed into SelectionGrid is {field.FieldType}, expected int or enum object");
                }

                SelectionStrings = selectionStrings;
                ColumnNumber = columnNumber;
            }

            public FieldInfo Field { get; }
            public string[] SelectionStrings { get; }
            public int ColumnNumber { get; }
        }

        public struct HorizontalSlider
        {
            public HorizontalSlider(FieldInfo field, float minValue, float maxValue)
            {
                if (field.FieldType == typeof(float))
                {
                    Field = field;
                    IsInt = false;
                }
                else if (field.FieldType == typeof(int))
                {
                    Field = field;
                    IsInt = true;
                }
                else
                {
                    throw new ArgumentException($"Field passed into HorizontalSlider is {field.FieldType}, expected float or int");
                }

                MinValue = minValue;
                MaxValue = maxValue;
                MinValueField = null;
                MaxValueField = null;
            }
            public HorizontalSlider(FieldInfo field, FieldInfo minValueField, float maxValue)
            {
                if (field.FieldType == typeof(float))
                {
                    Field = field;
                    IsInt = false;
                }
                else if (field.FieldType == typeof(int))
                {
                    Field = field;
                    IsInt = true;
                }
                else
                {
                    throw new ArgumentException($"Field passed into HorizontalSlider is {field.FieldType}, expected float or int");
                }

                MinValue = null;
                MaxValue = maxValue;
                if (minValueField.FieldType == typeof(float))
                {
                    MinValueField = minValueField;
                }
                else
                {
                    throw new ArgumentException($"MinValueField passed into HorizontalSlider is {minValueField.FieldType}, expected float");
                }
                MaxValueField = null;
            }
            public HorizontalSlider(FieldInfo field, float minValue, FieldInfo maxValueField)
            {
                if (field.FieldType == typeof(float))
                {
                    Field = field;
                    IsInt = false;
                }
                else if (field.FieldType == typeof(int))
                {
                    Field = field;
                    IsInt = true;
                }
                else
                {
                    throw new ArgumentException($"Field passed into HorizontalSlider is {field.FieldType}, expected float or int");
                }

                MinValue = minValue;
                MaxValue = null;
                MinValueField = null;
                if (maxValueField.FieldType == typeof(float))
                {
                    MaxValueField = maxValueField;
                }
                else
                {
                    throw new ArgumentException($"MaxValueField passed into HorizontalSlider is {maxValueField.FieldType}, expected float");
                }
            }
            public HorizontalSlider(FieldInfo field, FieldInfo minValueField, FieldInfo maxValueField)
            {
                if (field.FieldType == typeof(float))
                {
                    Field = field;
                    IsInt = false;
                }
                else if (field.FieldType == typeof(int))
                {
                    Field = field;
                    IsInt = true;
                }
                else
                {
                    throw new ArgumentException($"Field passed into HorizontalSlider is {field.FieldType}, expected float or int");
                }

                MinValue = null;
                MaxValue = null;
                if (minValueField.FieldType == typeof(float))
                {
                    MinValueField = minValueField;
                }
                else
                {
                    throw new ArgumentException($"MinValueField passed into HorizontalSlider is {minValueField.FieldType}, expected float");
                }
                if (maxValueField.FieldType == typeof(float))
                {
                    MaxValueField = maxValueField;
                }
                else
                {
                    throw new ArgumentException($"MaxValueField passed into HorizontalSlider is {maxValueField.FieldType}, expected float");
                }
            }

            public FieldInfo Field { get; }
            public float? MinValue { get; }
            public float? MaxValue { get; }
            public FieldInfo? MinValueField { get; }
            public FieldInfo? MaxValueField { get; }
            public bool IsInt { get; }
        }

        public struct VerticalSlider
        {
            public VerticalSlider(FieldInfo field, float minValue, float maxValue)
            {
                if (field.FieldType == typeof(float))
                {
                    Field = field;
                    IsInt = false;
                }
                else if (field.FieldType == typeof(int))
                {
                    Field = field;
                    IsInt = true;
                }
                else
                {
                    throw new ArgumentException($"Field passed into VerticalSlider is {field.FieldType}, expected float or int");
                }

                MinValue = minValue;
                MaxValue = maxValue;
                MinValueField = null;
                MaxValueField = null;
            }
            public VerticalSlider(FieldInfo field, FieldInfo minValueField, float maxValue)
            {
                if (field.FieldType == typeof(float))
                {
                    Field = field;
                    IsInt = false;
                }
                else if (field.FieldType == typeof(int))
                {
                    Field = field;
                    IsInt = true;
                }
                else
                {
                    throw new ArgumentException($"Field passed into VerticalSlider is {field.FieldType}, expected float or int");
                }

                MinValue = null;
                MaxValue = maxValue;
                if (minValueField.FieldType == typeof(float))
                {
                    MinValueField = minValueField;
                }
                else
                {
                    throw new ArgumentException($"MinValueField passed into VerticalSlider is {minValueField.FieldType}, expected float");
                }
                MaxValueField = null;
            }
            public VerticalSlider(FieldInfo field, float minValue, FieldInfo maxValueField)
            {
                if (field.FieldType == typeof(float))
                {
                    Field = field;
                    IsInt = false;
                }
                else if (field.FieldType == typeof(int))
                {
                    Field = field;
                    IsInt = true;
                }
                else
                {
                    throw new ArgumentException($"Field passed into VerticalSlider is {field.FieldType}, expected float or int");
                }

                MinValue = minValue;
                MaxValue = null;
                MinValueField = null;
                if (maxValueField.FieldType == typeof(float))
                {
                    MaxValueField = maxValueField;
                }
                else
                {
                    throw new ArgumentException($"MaxValueField passed into VerticalSlider is {maxValueField.FieldType}, expected float");
                }
            }
            public VerticalSlider(FieldInfo field, FieldInfo minValueField, FieldInfo maxValueField)
            {
                if (field.FieldType == typeof(float))
                {
                    Field = field;
                    IsInt = false;
                }
                else if (field.FieldType == typeof(int))
                {
                    Field = field;
                    IsInt = true;
                }
                else
                {
                    throw new ArgumentException($"Field passed into VerticalSlider is {field.FieldType}, expected float or int");
                }

                MinValue = null;
                MaxValue = null;
                if (minValueField.FieldType == typeof(float))
                {
                    MinValueField = minValueField;
                }
                else
                {
                    throw new ArgumentException($"MinValueField passed into VerticalSlider is {minValueField.FieldType}, expected float");
                }
                if (maxValueField.FieldType == typeof(float))
                {
                    MaxValueField = maxValueField;
                }
                else
                {
                    throw new ArgumentException($"MaxValueField passed into VerticalSlider is {maxValueField.FieldType}, expected float");
                }
            }

            public FieldInfo Field { get; }
            public float? MinValue { get; }
            public float? MaxValue { get; }
            public FieldInfo? MinValueField { get; }
            public FieldInfo? MaxValueField { get; }
            public bool IsInt { get; }
        }

        //This is called Spacing instead of Space because if it was space there would be ambiguity with UnityEngine.Space
        public struct Spacing
        {
            public Spacing(int pixels)
            {
                Pixels = pixels;
            }

            public int Pixels { get; }
        }
    }
}
