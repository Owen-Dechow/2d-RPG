using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class TreeSplitView : TwoPaneSplitView
{
    public new class UxmlFactory : UxmlFactory<TreeSplitView, TwoPaneSplitView.UxmlTraits> { }
    public TreeSplitView() { }
}
