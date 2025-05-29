using ModKit;
using System;
using UnityEngine;
using UnityModManagerNet;

namespace AlarmClock
{
    // ReSharper disable InconsistentNaming
    public static class UIElements
    {
        // STYLES MADE FOR ALARMCLOCK MOD START HERE ===
        public static void LabelTextField(string label, ref string text, int labelWidth = 100, int fieldWidth = 200)
        {
            using (UI.HorizontalScope())
            {
                Label(label, labelWidth);
                TextField(ref text, fieldWidth);
            }
        }

        public static void ToggleWithLabel(string label, ref bool value)
        {
            using (UI.HorizontalScope())
            {
                UI.Toggle("", ref value, GUILayout.Width(20));
                UI.Label(label);
            }
        }

        private static readonly GUIStyle LabelStyleRight = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleRight,
            richText = true,
            fixedHeight = 20,
            margin = new RectOffset(0, 0, 0, 0),
            padding = new RectOffset(0, 5, 0, 0)
        };


        private static readonly GUIStyle ToggleStyle = new GUIStyle(GUI.skin.toggle)
        {
            alignment = TextAnchor.MiddleLeft,
            richText = true,
            fixedHeight = 20,
            margin = new RectOffset(0, 0, 0, 0),
            padding = new RectOffset(0, 0, 0, 0)
        };

        public static void Toggle(string text, ref bool value, int width = 200)
        {
            //UI.Toggle(text, ref value, UI.ChecklyphOn, UI.CheckGlyphOff, width, ToggleStyle);

            GUILayout.BeginHorizontal();
            UI.Toggle("", ref value);
            Label(text, width);
            GUILayout.EndHorizontal();
        }

        // STYLES MADE FOR ALARMCLOCK MOD END HERE ===

        private static readonly GUIStyle IconButtonStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            richText = true,
            fixedHeight = 20,
            contentOffset = new Vector2(0.0f, UnityModManager.UI.Scale(-2)),
            margin = new RectOffset(0, 0, 0, 0)
        };

        private static readonly GUIStyle IconTextFieldStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleLeft,
            richText = true,
            fixedHeight = 20,
            contentOffset = new Vector2(0.0f, UnityModManager.UI.Scale(-2)),
            margin = new RectOffset(0, 0, 0, 0),
            padding = new RectOffset(5, 5, 0, 0)
        };

        public static readonly GUIStyle ElementBoxStyle = new GUIStyle(UI.textBoxStyle)
        {
            richText = true,
            fixedHeight = 20,
            alignment = TextAnchor.MiddleLeft,
            padding = new RectOffset(10, 10, 0, 0)
        };

        private static readonly GUIStyle TabButtonStyle = new GUIStyle(GUI.skin.label)
        {
            richText = true,
            fixedHeight = 20,
            alignment = TextAnchor.MiddleCenter,
            margin = new RectOffset(10, 10, 0, 0),
            padding = new RectOffset(0, 5, 0, 0)
        };

        private static readonly GUIStyle ClearBoxStyle = new GUIStyle(GUI.skin.label)
        {
            richText = true,
            fixedHeight = 20,
            alignment = TextAnchor.MiddleLeft
        };

        private static readonly GUIStyle TextFieldStyle = new GUIStyle(ElementBoxStyle)
        {
            alignment = TextAnchor.MiddleLeft,
            contentOffset = new Vector2(0.0f, UnityModManager.UI.Scale(-2)),
            margin = new RectOffset(0, 0, 0, 0),
            padding = new RectOffset(5, 5, 0, 0)
        };

        private static readonly GUIStyle HeaderStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleLeft,
            richText = true,
            fixedHeight = 20,
            fontSize = 16.point(),
            margin = new RectOffset(0, 0, 0, 0),
            padding = new RectOffset(0, 5, 0, 0)
        };

        private static readonly GUIStyle LabelStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleLeft,
            richText = true,
            fixedHeight = 20,
            margin = new RectOffset(0, 0, 0, 0),
            padding = new RectOffset(0, 5, 0, 0)
        };

        private static readonly GUIStyle IconLabelStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleLeft,
            richText = true,
            fixedHeight = 20,
            contentOffset = new Vector2(0.0f, UnityModManager.UI.Scale(-2)),
            margin = new RectOffset(0, 0, 0, 0),
            padding = new RectOffset(0, 5, 0, 0)
        };

        public static void Icon(Sprite sprite, int scale = 24)
        {
            var widerThanTaller = sprite.textureRect.width > sprite.textureRect.height;
            var height = widerThanTaller ? scale * (sprite.textureRect.height / sprite.textureRect.width) : scale;

            var topPadding = (int)((scale - height) / 2f + (40 - scale) / 2f) - 1;

            var style = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                normal = new GUIStyleState
                {
                    background = sprite.texture
                },
                margin = new RectOffset(0, 0, topPadding, 0),
                padding = new RectOffset(0, 0, 0, 0),
                fixedWidth = scale,
                fixedHeight = scale
            };

            UI.Label(" ", style);
        }

        public static void TabButton(string title, Sprite icon, Action action, float width = 200)
        {
            using (UI.HorizontalScope(ElementBoxStyle))
            {
                Icon(icon, 32);
                UI.Space(10);
                UI.ActionButton(title.ToUpper().bold(), action, TabButtonStyle, UI.MinWidth(158));
            }
        }

        public static void IconTextActionButton(string title, Sprite icon, Action action, float width = 200)
        {
            var buttonWidth = width - 42;

            using (UI.HorizontalScope(ElementBoxStyle, UI.MinWidth(width), UI.MaxWidth(width)))
            {
                Icon(icon);
                UI.Space(10);
                UI.ActionButton(title.ToUpper().bold(), action, IconButtonStyle, UI.MinWidth(buttonWidth), UI.MaxWidth(buttonWidth));
            }
        }

        public static void IconActionButton(Sprite icon, Action action, float size = 40)
        {
            var style = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.LowerCenter,
                normal = new GUIStyleState
                {
                    background = icon.texture,
                },
                fixedWidth = size,
                fixedHeight = size,
                margin = new RectOffset(5, 5, 10, 0)
            };

            UI.ActionButton("  ", action, style);
        }

        public static void TextField(ref string text, int width)
        {
            text = GUILayout.TextField(text, TextFieldStyle, UI.MinWidth(width), UI.MaxWidth(width));
        }

        public static void IconTextField(ref string text, Sprite icon, int width)
        {
            var textFieldWidth = width - 42;
            using (UI.HorizontalScope(ElementBoxStyle, UI.MinWidth(width), UI.MaxWidth(width)))
            {
                Icon(icon, 32);
                UI.Space(10);
                text = GUILayout.TextField(text, IconTextFieldStyle, UI.MinWidth(textFieldWidth),
                    UI.MaxWidth(textFieldWidth));
            }
        }

        public static void Label(string text, int width = 200)
        {
            UI.Label(text, LabelStyle, UI.MinWidth(width), UI.MaxWidth(width));
        }

        public static void LabelRight(string text, int width = 200)
        {
            UI.Label(text, LabelStyleRight, UI.MinWidth(width), UI.MaxWidth(width));
        }

        public static void Header(string text, int width = 200)
        {
            UI.Label(text, HeaderStyle, UI.MinWidth(width), UI.MaxWidth(width));
        }

        public static void IconLabel(string text, Sprite icon, int width = 200, int iconSize = 40)
        {
            var textFieldWidth = width - iconSize - 10;
            using (UI.HorizontalScope(ClearBoxStyle, UI.MinWidth(width), UI.MaxWidth(width)))
            {
                Icon(icon, iconSize);
                UI.Space(10);
                UI.Label(text, IconLabelStyle, UI.MinWidth(textFieldWidth), UI.MaxWidth(textFieldWidth));
            }
        }
    }
}