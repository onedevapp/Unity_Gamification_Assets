using UnityEngine;
using UnityEngine.UI;

namespace OneDevApp
{
    /// <summary>
    /// Layout group aligns childrens like pile of cards
    /// </summary>
    [AddComponentMenu("Layout/Cards Layout", 152)]
    public class CardsLayout : LayoutGroup
    {
        [SerializeField] protected Vector2 m_CellSize = new Vector2(100, 100);
        public Vector2 cellSize { get { return m_CellSize; } set { SetProperty(ref m_CellSize, value); } }

        [SerializeField] protected Vector2 m_Spacing = Vector2.zero;
        public Vector2 spacing { get { return m_Spacing; } set { SetProperty(ref m_Spacing, value); } }

        [SerializeField] protected int m_VisibleCardsCount = -1;
        public int VisibleCardsCount { get { return m_VisibleCardsCount; } set { SetProperty(ref m_VisibleCardsCount, Mathf.Max(-1, value)); } }

        /// <summary>
        /// When is the layout vertical? In portrait or landscape?
        /// </summary>
        [Tooltip("Use unknown, to avoid AutoRotation")]
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

        protected CardsLayout()
        { }

    #if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
        }

    #endif

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();

            SetLayoutInputForAxis(
                padding.horizontal + (cellSize.x + (IsVertical ? spacing.x : spacing.y)) - (IsVertical ? spacing.x : spacing.y),
                padding.horizontal + (cellSize.x + (IsVertical ? spacing.x : spacing.y)) - (IsVertical ? spacing.x : spacing.y),
                -1, 0);
        }

        public override void CalculateLayoutInputVertical()
        {
            float minSpace = padding.vertical + (cellSize.y + (IsVertical ? spacing.y : spacing.x)) - (IsVertical ? spacing.y : spacing.x);
            SetLayoutInputForAxis(minSpace, minSpace, -1, 1);
        }

        public override void SetLayoutHorizontal()
        {
            SetCellsAlongAxis();
        }

        public override void SetLayoutVertical()
        {
            SetCellsAlongAxis();
        }

        private void SetCellsAlongAxis()
        {
            Vector2 startOffset = new Vector2(0, 0);

            for (int i = 0; i < rectChildren.Count; i++)
            {
                SetChildAlongAxis(rectChildren[i], 0, startOffset.x, cellSize[0]);
                SetChildAlongAxis(rectChildren[i], 1, startOffset.y, cellSize[1]);

                if (i > 0)
                {
                    if (m_VisibleCardsCount > 0)
                    {
                        if (i >= (rectChildren.Count - m_VisibleCardsCount))
                        {
                            startOffset.y += (IsVertical ? spacing.x : spacing.y);
                            startOffset.x += (IsVertical ? spacing.y : spacing.x);
                        }
                        else if (i >= (rectChildren.Count - m_VisibleCardsCount - 2))
                        {
                            startOffset.y += ((IsVertical ? spacing.y : spacing.x) > 0 ? 0f : 2f);
                            startOffset.x += ((IsVertical ? spacing.x : spacing.y) > 0 ? 0f : 2f);
                        }
                    }
                    else if (m_VisibleCardsCount < 0)
                    {
                        startOffset.y += (IsVertical ? spacing.y : spacing.x);
                        startOffset.x += (IsVertical ? spacing.x : spacing.y);
                    }
                }
            }
        }
    }
}