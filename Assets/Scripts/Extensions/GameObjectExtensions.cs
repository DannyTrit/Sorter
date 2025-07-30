using UnityEngine;

namespace Sorter
{
    public static class GameObjectExtensions
    {
        public static void Show(this GameObject self) => self.SetActive(true);
        public static void Hide(this GameObject self) => self.SetActive(false);
        
        public static void Show(this Component self) => self.SetActive(true);
        public static void Hide(this Component self) => self.SetActive(false);

        public static void SetActive(this Component self, bool isActive) => self.gameObject.SetActive(isActive);

        public static void DisableComponent(this Behaviour behaviour)
        {
            behaviour.SetComponentEnabled(false);
        }

        public static void EnableComponent(this Behaviour behaviour)
        {
            behaviour.SetComponentEnabled(true);
        }

        public static void SetComponentEnabled(this Behaviour behaviour, bool isEnabled)
        {
            behaviour.enabled = isEnabled;
        }
        
        public static void DisableComponent(this Collider collider)
        {
            collider.SetComponentEnabled(false);
        }
        
        public static void EnableComponent(this Collider collider)
        {
            collider.SetComponentEnabled(true);
        }
        
        public static void SetComponentEnabled(this Collider collider, bool isEnabled)
        {
            collider.enabled = isEnabled;
        }

        public static void SetSprite(this SpriteRenderer spriteRenderer, Sprite sprite)
        {
            spriteRenderer.sprite = sprite;
        }
    }
}