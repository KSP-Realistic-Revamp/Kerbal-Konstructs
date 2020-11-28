using UnityEngine;
using UnityEngine.UI;
using KodeUI;

namespace KerbalKonstructs.UI {

	public class HorizontalLayout : Layout
	{
		public override void CreateUI ()
		{
			this.Horizontal ()
				.ControlChildSize (true, true)
				.ChildForceExpand (false,false)
				;
		}
	}

	public class HorizontalSep : UIImage, ILayoutElement
	{
		Vector2 minSize;
		Vector2 preferredSize;

		public void CalculateLayoutInputHorizontal()
		{
			float width = 0;
			if (image.sprite) {
				width = image.sprite.rect.width;
			}
			preferredSize.x = width;
			minSize.x = width;
		}

		public void CalculateLayoutInputVertical()
		{
			float height = 0;
			if (image.sprite) {
				height = image.sprite.rect.height;
			}
			preferredSize.y = height;
			minSize.y = height;
		}

		public int layoutPriority { get { return 0; } }
		public float minWidth { get { return minSize.x; } }
		public float preferredWidth { get { return preferredSize.x; } }
		public float flexibleWidth  { get { return -1; } }
		public float minHeight { get { return minSize.y; } }
		public float preferredHeight { get { return preferredSize.y; } }
		public float flexibleHeight  { get { return -1; } }


		public override void CreateUI ()
		{
			base.CreateUI();
			this.FlexibleLayout(false, false)
				.PreferredHeight(4);
			DebugLayout();
		}
	}

	public class FlexibleSpace : UIEmpty
	{
		public override void CreateUI ()
		{
			var parent = rectTransform.parent.GetComponent<Layout>();
			if (parent != null) {
				if (parent.layoutGroup is HorizontalLayoutGroup) {
					this.FlexibleLayout (true, false);
				} else if (parent.layoutGroup is VerticalLayoutGroup) {
					this.FlexibleLayout (false, true);
				} else {
					Debug.LogError("don't know how to fiex");
				}
			}
		}
	}

	public class FixedSpace : UIEmpty
	{
		bool horizontal;

		public override void CreateUI ()
		{
			base.CreateUI();
			var parent = rectTransform.parent.GetComponent<Layout>();
			if (parent != null) {
				if (parent.layoutGroup is HorizontalLayoutGroup) {
					horizontal = true;
				} else if (parent.layoutGroup is VerticalLayoutGroup) {
					horizontal = false;
				} else {
					Debug.LogError("don't know how to fiex");
				}
			}
		}

		public FixedSpace Size(float size)
		{
			if (horizontal) {
				PreferredWidth(size);
			} else {
				PreferredHeight(size);
			}
			return this;
		}
	}

	public class VerticalLayout : Layout
	{
		public override void CreateUI ()
		{
			this.Vertical ()
				.ControlChildSize (true, true)
				.ChildForceExpand (false,false)
				;
		}
	}
}
