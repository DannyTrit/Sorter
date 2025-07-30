using System;
using UnityEngine;
using UnityEngine.UI;

namespace Sorter
{
    public static class UIExtensions
    {
        public static void SetColor(this Graphic image, Color color)
        {
            image.color = color;
        }
        
        public static void SetSprite(this Image img, Sprite sprite) => img.sprite = sprite;
        
        public static void Bind(this Button button, Action onClick)
        {
            button.onClick.AddListener(() => onClick?.Invoke());
        }

        public static void Clear(this Button button)
        {
            button.onClick.RemoveAllListeners();
        }

        public static void ClearBind(this Button button, Action onClick)
        {
            button.Clear();
            button.Bind(onClick);
        }
        
        public static void Enable(this Selectable selectable)
        {
            selectable.SetInteractable(true);
        }

        public static void Disable(this Selectable selectable)
        {
            selectable.SetInteractable(false);
        }

        public static void SetInteractable(this Selectable selectable, bool isEnabled)
        {
            selectable.interactable = isEnabled;
        }
    }
}