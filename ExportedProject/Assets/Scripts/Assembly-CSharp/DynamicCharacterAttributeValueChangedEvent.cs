using System;
using UnityEngine.Events;

[Serializable]
public class DynamicCharacterAttributeValueChangedEvent : UnityEvent<float, DynamicCharacterAttributeValueChangedType>
{
}
