using UnityEditor;

namespace OneDevApp
{
    /// <summary>
    /// Override the Unity UI custom editor for DynamicOrientationGroupEditor
    /// and do exactly nothing. The only purpose of this class is to expose the
    /// public 'VerticalWhen'. The inherited editor prevents editing new
    /// publics in descendant classes without doing a full widget layout here
    /// (too much work!).
    /// </summary>
    [CustomEditor(typeof(DynamicOrientationGroup), true)]
    [CanEditMultipleObjects]
    public class DynamicOrientationGroupEditor : Editor
    {
    }
}
