using UnityEditor;

namespace OneDevApp
{
    /// <summary>
    /// Override the Unity UI custom editor for CardsLayoutEditor
    /// and do exactly nothing. The only purpose of this class is to expose the
    /// public 'VerticalWhen'
    /// </summary>
    [CustomEditor(typeof(CardsLayout), true)]
    [CanEditMultipleObjects]
    public class CardsLayoutEditor : Editor
    {
    }
}
