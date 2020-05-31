using UnityEngine;
using UnityEngine.UI;

namespace OneDevApp
{
    /// <summary>
    /// Layout group switches orientation automatically between landscape and
    /// portrait layouts so it either acts like a VerticalLayoutGroup or a
    /// HorizontalLayoutGroup.
    /// </summary>
    [AddComponentMenu("Layout/Dynamic Orientation Group", 150)]
    public class DynamicOrientationGroup : HorizontalOrVerticalLayoutGroup
    {

        /// <summary>
        /// When is the layout vertical? In portrait or landscape?
        /// </summary>
        [SerializeField]
        public ScreenOrientation verticalWhen = ScreenOrientation.Portrait;

        public bool IsVertical { get { return GetIsVertical(); } }

        private bool GetIsVertical()
        {
            bool isVertical;

            if (Screen.width > Screen.height)
            {
                isVertical = (verticalWhen == ScreenOrientation.Landscape) ? true : false;
            }
            else
            {
                isVertical = (verticalWhen == ScreenOrientation.Portrait) ? true : false;
            }
            return isVertical;
        }

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            CalcAlongAxis(0, isVertical: IsVertical);
        }

        public override void CalculateLayoutInputVertical()
        {
            CalcAlongAxis(1, isVertical: IsVertical);
        }

        public override void SetLayoutHorizontal()
        {
            SetChildrenAlongAxis(0, isVertical: IsVertical);
        }

        public override void SetLayoutVertical()
        {
            SetChildrenAlongAxis(1, isVertical: IsVertical);
        }
    }
}

