using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SLC.RetroHorror.Core
{
    [System.Serializable]
    public class Item
    {
        public enum Type
        {
            RangeWeapon = 0,
            MeleeWeapon,
            Ammo,
            Consumable,
            Note
        }

        /// <summary>
        /// This should be unique for each different item type!
        /// </summary>
        public string ItemId;
        public string ItemName;
        [TextArea(2, 10)] public string ItemDescription;
        public Type ItemType;
        public bool UsesAmmo;
        public string AmmoId;
        public int ItemMaxStack;
        public float ItemWeight;
        public Mesh ItemMesh;
        public Material ItemMaterial;
        
        //TODO : HIDE USESAMMO AND AMMOID IF NOT RANGEWEAPON
    }

    [CustomPropertyDrawer(typeof(Item))]
    public class ItemDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            Foldout container = new();

            //Fetch properties used for determining visual element
            SerializedProperty idProperty = property.FindPropertyRelative("ItemId");
            SerializedProperty typeProperty = property.FindPropertyRelative("ItemType");
            SerializedProperty usesAmmoProperty = property.FindPropertyRelative("UsesAmmo");
            SerializedProperty ammoIdProperty = property.FindPropertyRelative("AmmoId");

            //Create property fields to add to container
            PropertyField idField = new(idProperty);
            PropertyField nameField = new(property.FindPropertyRelative("ItemName"));
            PropertyField descriptionField = new(property.FindPropertyRelative("ItemDescription"));
            PropertyField typeField = new(typeProperty);
            PropertyField usesAmmoField = new(usesAmmoProperty);
            PropertyField ammoIdField = new(ammoIdProperty);
            PropertyField maxStackField = new(property.FindPropertyRelative("ItemMaxStack"));
            PropertyField weightField = new(property.FindPropertyRelative("ItemWeight"));
            PropertyField meshField = new(property.FindPropertyRelative("ItemMesh"));
            PropertyField materialField = new(property.FindPropertyRelative("ItemMaterial"));
            Item.Type type = (Item.Type)typeProperty.enumValueIndex;
            container.text = idProperty.stringValue;

            //Add fields to foldout
            container.Add(idField);
            container.Add(nameField);
            container.Add(descriptionField);
            container.Add(typeField);

            typeField.RegisterValueChangeCallback((callback) =>
                {
                    bool canUseAmmo = (Item.Type)callback.changedProperty.enumValueIndex == Item.Type.RangeWeapon;
                    ToggleFieldVisibility(canUseAmmo, ref usesAmmoField);
                    if (!canUseAmmo) usesAmmoProperty.boolValue = false;
                    property.serializedObject.ApplyModifiedProperties();
                });

            usesAmmoField.RegisterValueChangeCallback((callback) =>
            {
                ToggleFieldVisibility(callback.changedProperty.boolValue, ref ammoIdField);
                if (!callback.changedProperty.boolValue) ammoIdProperty.stringValue = "";
                property.serializedObject.ApplyModifiedProperties();
            });

            container.Add(usesAmmoField);
            container.Add(ammoIdField);

            //Final fields to add
            container.Add(maxStackField);
            container.Add(weightField);
            container.Add(meshField);
            container.Add(materialField);

            return container;
        }

        private void ToggleFieldVisibility(bool _toggledOn, ref PropertyField _fieldToToggle)
        {
            if (_toggledOn) _fieldToToggle.style.display = DisplayStyle.Flex;
            else _fieldToToggle.style.display = DisplayStyle.None;
        }
    }
}