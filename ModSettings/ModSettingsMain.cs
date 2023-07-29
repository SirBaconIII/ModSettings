using MelonLoader;
using ModSettings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using static ModSettings.ControlStructs;

[assembly: MelonInfo(typeof(ModSettingsMain), "Mod Settings", "1.0.0", "SirBaconIII")]
namespace ModSettings
{
    public class ModSettingsMain : MelonMod
    {
        private KeyCode renderGuiKey = KeyCode.F5;
        public static bool renderGui = false;
        private int pageIndex = -1;

        private Rect backgroundRect = new Rect(10, 10, Screen.width / 4, Screen.height - 20);
        private Rect layoutRect = new Rect(20, 40, (Screen.width / 4) - 20, Screen.height - 60);

        private static List<ModSetting> registeredMods = new List<ModSetting>();

        private string registeredModNames = "Registered Mods:";
        private static string path = "";

        public override void OnLateUpdate()
        {
            if (Input.GetKeyDown(renderGuiKey))
            {
                renderGui = !renderGui;
            }
        }

        public override void OnLateInitializeMelon()
        {
            registeredMods.Sort((setting1, setting2) => string.Compare(setting1.Name, setting2.Name));

            path = Path.GetFullPath(".") + "/Mods/config";
            Directory.CreateDirectory(path);

            foreach (ModSetting modSetting in registeredMods)
            {
                registeredModNames += " " + modSetting.Name + ",";
                modSetting.CreatePreferences();
                modSetting.LoadPreferences();
            }
            registeredModNames = registeredModNames.Remove(registeredModNames.Length - 1);

            MelonLogger.Msg($"Press {renderGuiKey} to open the settings menu.");
            MelonLogger.Msg(registeredModNames);
        }

        public override void OnApplicationQuit()
        {
            foreach (ModSetting modSetting in registeredMods)
            {
                modSetting.SavePreferences();
            }
        }

        public override void OnGUI()
        {
            if (renderGui) 
            {
                if (pageIndex == -1)
                {
                    GUI.Box(backgroundRect, "Mod Settings");
                    GUILayout.BeginArea(layoutRect);

                    for (int i = 0; i < registeredMods.Count; i++)
                    {
                        if (GUILayout.Button(registeredMods[i].FormattedName))
                        {
                            pageIndex = i;
                        }
                    }
                    
                    GUILayout.EndArea();
                }
                else
                {
                    ModSetting currentPage = registeredMods[pageIndex];

                    GUI.Box(backgroundRect, currentPage.FormattedName);
                    if (GUI.Button(new Rect(20, 15, Screen.width / 24, 15), "Back"))
                    {
                        pageIndex = -1;
                    }
                    GUILayout.BeginArea(layoutRect);

                    foreach (object control in currentPage.Controls)
                    {
                        CreateControl(control, currentPage);
                    }
                    if (currentPage.ResetButton)
                    {
                        GUILayout.Space(30);
                        if (GUILayout.Button("Reset default settings"))
                        {
                            currentPage.ResetPreferences();
                            currentPage.LoadPreferences();
                        }
                    }
                    GUILayout.EndArea();
                }
            }
            else
            {
                pageIndex = -1;
            }
        }

        public static void RegisterMod(string modName, object modClass, object[] controls)
        {
            registeredMods.Add(new ModSetting(modName, controls, modClass));
        }
        public static void RegisterMod(string modName, object modClass, object[] controls, bool resetButton)
        {
            registeredMods.Add(new ModSetting(modName, controls, modClass, resetButton));
        }

        private void CreateControl(object control, ModSetting currentPage)
        {
            if (control is Label label)
            {
                if (label.DynamicText != null)
                {
                    GUILayout.Label(label.DynamicText.GetValue(currentPage.ModClass).ToString());
                }
                else
                {
                    GUILayout.Label(label.Text);
                }
            }
            else if (control is Button button)
            {
                if (GUILayout.Button(button.Text))
                {
                    button.ButtonMethod.Invoke(currentPage.ModClass, button.MethodParameters);
                }
            }
            else if (control is RepeatButton repeatButton)
            {
                if (GUILayout.RepeatButton(repeatButton.Text))
                {
                    repeatButton.ButtonMethod.Invoke(currentPage.ModClass, repeatButton.MethodParameters);
                }
            }
            else if (control is TextField textField)
            {
                textField.Field.SetValue(currentPage.ModClass, GUILayout.TextField(textField.Field.GetValue(currentPage.ModClass).ToString()));
            }
            else if (control is TextArea textArea)
            {
                textArea.Field.SetValue(currentPage.ModClass, GUILayout.TextArea(textArea.Field.GetValue(currentPage.ModClass).ToString()));
            }
            else if (control is Toggle toggle)
            {
                toggle.Field.SetValue(currentPage.ModClass, GUILayout.Toggle((bool)toggle.Field.GetValue(currentPage.ModClass), toggle.Name));
            }
            else if (control is Toolbar toolbar)
            {
                Type enumType = toolbar.Field.FieldType;
                if (enumType.IsEnum)
                {
                    int enumValue = GUILayout.Toolbar((int)toolbar.Field.GetValue(currentPage.ModClass), toolbar.ToolbarStrings);
                    toolbar.Field.SetValue(currentPage.ModClass, Enum.ToObject(enumType, enumValue));
                }
                else
                {
                    toolbar.Field.SetValue(currentPage.ModClass, GUILayout.Toolbar((int)toolbar.Field.GetValue(currentPage.ModClass), toolbar.ToolbarStrings));
                }
            }
            else if (control is SelectionGrid selectionGrid)
            {
                Type enumType = selectionGrid.Field.FieldType;
                if (enumType.IsEnum)
                {
                    int enumValue = GUILayout.SelectionGrid((int)selectionGrid.Field.GetValue(currentPage.ModClass), selectionGrid.SelectionStrings, selectionGrid.ColumnNumber);
                    selectionGrid.Field.SetValue(currentPage.ModClass, Enum.ToObject(enumType, enumValue));
                }
                else
                {
                    selectionGrid.Field.SetValue(currentPage.ModClass, GUILayout.SelectionGrid((int)selectionGrid.Field.GetValue(currentPage.ModClass), selectionGrid.SelectionStrings, selectionGrid.ColumnNumber));
                }
            }
            else if (control is HorizontalSlider horizontalSlider)
            {
                float minValue;
                float maxValue;

                if (horizontalSlider.MinValue == null)
                {
                    minValue = (float)horizontalSlider.MinValueField.GetValue(currentPage.ModClass);
                }
                else
                {
                    minValue = (float)horizontalSlider.MinValue;
                }
                if (horizontalSlider.MaxValue == null)
                {
                    maxValue = (float)horizontalSlider.MaxValueField.GetValue(currentPage.ModClass);
                }
                else
                {
                    maxValue = (float)horizontalSlider.MaxValue;
                }

                if (horizontalSlider.IsInt)
                {
                    //the fact that i have to cast GetValue to int and then to float is stupid
                    horizontalSlider.Field.SetValue(currentPage.ModClass, (int)GUILayout.HorizontalSlider((float)((int)horizontalSlider.Field.GetValue(currentPage.ModClass)), minValue, maxValue));
                }
                else
                {
                    horizontalSlider.Field.SetValue(currentPage.ModClass, GUILayout.HorizontalSlider((float)horizontalSlider.Field.GetValue(currentPage.ModClass), minValue, maxValue));
                }
            }
            else if (control is VerticalSlider verticalSlider)
            {
                float minValue;
                float maxValue;

                if (verticalSlider.MinValue == null)
                {
                    minValue = (float)verticalSlider.MinValueField.GetValue(currentPage.ModClass);
                }
                else
                {
                    minValue = (float)verticalSlider.MinValue;
                }
                if (verticalSlider.MaxValue == null)
                {
                    maxValue = (float)verticalSlider.MaxValueField.GetValue(currentPage.ModClass);
                }
                else
                {
                    maxValue = (float)verticalSlider.MaxValue;
                }

                if (verticalSlider.IsInt)
                {
                    //the fact that i have to cast GetValue to int and then to float is stupid
                    verticalSlider.Field.SetValue(currentPage.ModClass, (int)GUILayout.HorizontalSlider((float)((int)verticalSlider.Field.GetValue(currentPage.ModClass)), minValue, maxValue));
                }
                else
                {
                    verticalSlider.Field.SetValue(currentPage.ModClass, GUILayout.HorizontalSlider((float)verticalSlider.Field.GetValue(currentPage.ModClass), minValue, maxValue));
                }
            }
            else if (control is Spacing space)
            {
                GUILayout.Space(space.Pixels);
            }
            else
            {
                throw new ArgumentException($"Object passed into controls array of registered mod {currentPage.Name} is not a valid control");
            }
        }

        public static string SeparateWords(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            StringBuilder result = new StringBuilder();
            result.Append(input[0]);

            for (int i = 1; i < input.Length; i++)
            {
                if (char.IsUpper(input[i]))
                {
                    result.Append(' ');
                }
                result.Append(input[i]);
            }

            return result.ToString();
        }

        //Mod Setting struct has all the info for each mod
        private struct ModSetting
        {
            public ModSetting(string name, object[] controls, object modClass, bool resetButton)
            {
                Name = name;
                FormattedName = SeparateWords(name);
                Controls = controls;
                ModClass = modClass;
                ResetButton = resetButton;
            }
            public ModSetting(string name, object[] controls, object modClass)
            {
                Name = name;
                FormattedName = SeparateWords(name);
                Controls = controls;
                ModClass = modClass;
                ResetButton = true;
            }

            public string Name { get; }
            public string FormattedName { get; }
            public object[] Controls { get; }
            public object? ModClass { get; }
            public bool ResetButton { get; }

            public void CreatePreferences()
            {
                MelonPreferences_Category category = MelonPreferences.CreateCategory(Name);
                category.SetFilePath($"{path}/{Name}.cfg");

                foreach (object control in Controls)
                {
                    if (control is TextField textField)
                    {
                        category.CreateEntry(textField.Field.Name, (string)textField.Field.GetValue(ModClass));
                    }
                    else if (control is TextArea textArea)
                    {
                        category.CreateEntry(textArea.Field.Name, (string)textArea.Field.GetValue(ModClass));
                    }
                    else if (control is Toggle toggle)
                    {
                        category.CreateEntry(toggle.Field.Name, (bool)toggle.Field.GetValue(ModClass));
                    }
                    else if (control is Toolbar toolbar)
                    {
                        category.CreateEntry(toolbar.Field.Name, (int)toolbar.Field.GetValue(ModClass));
                    }
                    else if (control is SelectionGrid selectionGrid)
                    {
                        category.CreateEntry(selectionGrid.Field.Name, (int)selectionGrid.Field.GetValue(ModClass));
                    }
                    else if (control is HorizontalSlider horizontalSlider)
                    {
                        if (horizontalSlider.IsInt)
                        {
                            category.CreateEntry(horizontalSlider.Field.Name, (int)horizontalSlider.Field.GetValue(ModClass));
                        }
                        else
                        {
                            category.CreateEntry(horizontalSlider.Field.Name, (float)horizontalSlider.Field.GetValue(ModClass));
                        }
                    }
                    else if (control is VerticalSlider verticalSlider)
                    {
                        category.CreateEntry(verticalSlider.Field.Name, (float)verticalSlider.Field.GetValue(ModClass));
                    }
                }
            }
            
            public void LoadPreferences() 
            {
                MelonPreferences_Category category = MelonPreferences.GetCategory(Name);

                foreach (object control in Controls)
                {
                    if (control is TextField textField)
                    {
                        textField.Field.SetValue(ModClass, category.GetEntry(textField.Field.Name).BoxedEditedValue);
                    }
                    else if (control is TextArea textArea)
                    {
                        textArea.Field.SetValue(ModClass, category.GetEntry(textArea.Field.Name).BoxedEditedValue);
                    }
                    else if (control is Toggle toggle)
                    {
                        toggle.Field.SetValue(ModClass, category.GetEntry(toggle.Field.Name).BoxedEditedValue);
                    }
                    else if (control is Toolbar toolbar)
                    {
                        toolbar.Field.SetValue(ModClass, category.GetEntry(toolbar.Field.Name).BoxedEditedValue);
                    }
                    else if (control is SelectionGrid selectionGrid)
                    {
                        selectionGrid.Field.SetValue(ModClass, category.GetEntry(selectionGrid.Field.Name).BoxedEditedValue);
                    }
                    else if (control is HorizontalSlider horizontalSlider)
                    {
                        if (horizontalSlider.IsInt)
                        {
                            horizontalSlider.Field.SetValue(ModClass, (int)category.GetEntry(horizontalSlider.Field.Name).BoxedEditedValue);
                        }
                        else
                        {
                            horizontalSlider.Field.SetValue(ModClass, (float)category.GetEntry(horizontalSlider.Field.Name).BoxedEditedValue);
                        }
                        
                    }
                    else if (control is VerticalSlider verticalSlider)
                    {
                        verticalSlider.Field.SetValue(ModClass, category.GetEntry(verticalSlider.Field.Name).BoxedEditedValue);
                    }
                }
            }

            public void SavePreferences()
            {
                MelonPreferences_Category category = MelonPreferences.GetCategory(Name);

                foreach (object control in Controls)
                {
                    if (control is TextField textField)
                    {
                        category.GetEntry(textField.Field.Name).BoxedEditedValue = textField.Field.GetValue(ModClass);
                    }
                    else if (control is TextArea textArea)
                    {
                        category.GetEntry(textArea.Field.Name).BoxedEditedValue = textArea.Field.GetValue(ModClass);
                    }
                    else if (control is Toggle toggle)
                    {
                        category.GetEntry(toggle.Field.Name).BoxedEditedValue = toggle.Field.GetValue(ModClass);
                    }
                    else if (control is Toolbar toolbar)
                    {
                        category.GetEntry(toolbar.Field.Name).BoxedEditedValue = toolbar.Field.GetValue(ModClass);
                    }
                    else if (control is SelectionGrid selectionGrid)
                    {
                        category.GetEntry(selectionGrid.Field.Name).BoxedEditedValue = selectionGrid.Field.GetValue(ModClass);
                    }
                    else if (control is HorizontalSlider horizontalSlider)
                    {
                        category.GetEntry(horizontalSlider.Field.Name).BoxedEditedValue = horizontalSlider.Field.GetValue(ModClass);
                    }
                    else if (control is VerticalSlider verticalSlider)
                    {
                        category.GetEntry(verticalSlider.Field.Name).BoxedEditedValue = verticalSlider.Field.GetValue(ModClass);
                    }
                }
            }

            public void ResetPreferences()
            {
                MelonPreferences_Category category = MelonPreferences.GetCategory(Name);
                foreach (MelonPreferences_Entry entry in category.Entries)
                {
                    entry.ResetToDefault();
                }
            }
        }
    }
}