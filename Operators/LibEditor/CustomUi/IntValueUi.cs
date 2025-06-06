using ImGuiNET;
using T3.Core.Operator;
using T3.Editor.Gui.Styling;
using T3.Editor.Gui.UiHelpers;
using T3.Editor.UiModel;
using Types.Values;
using Vector2 = System.Numerics.Vector2;

namespace libEditor.CustomUi;

public static class IntValueUi
{
    public static SymbolUi.Child.CustomUiResult DrawChildUi(Instance instance, ImDrawListPtr drawList, ImRect selectableScreenRect, Vector2 canvasScale)
    {
        if (!(instance is IntValue intValueInstance))
            return SymbolUi.Child.CustomUiResult.None;

        var symbolChild = instance.SymbolChild;
        ImGui.PushClipRect(selectableScreenRect.Min, selectableScreenRect.Max, true);
            
        var h = selectableScreenRect.GetHeight();
        var font = h > 40
                       ? Fonts.FontLarge
                       : (h > 25
                              ? Fonts.FontNormal
                              : Fonts.FontSmall);

        ImGui.PushFont(font);
        ImGui.SetCursorScreenPos(selectableScreenRect.Min + new Vector2(10,0));
        ImGui.BeginGroup();
        if (!string.IsNullOrWhiteSpace(symbolChild.Name))
        {
            ImGui.TextUnformatted(symbolChild.Name);
        }

        var isAnimated = instance.Parent?.Symbol.Animator.IsInputSlotAnimated(intValueInstance.Int)??false;

        var value = (isAnimated || intValueInstance.Int.HasInputConnections) 
                        ? intValueInstance.Int.Value 
                        : intValueInstance.Int.TypedInputValue.Value;
            
        ImGui.TextUnformatted($"{value:0}");
        ImGui.EndGroup();
        ImGui.PopFont();
            
        ImGui.PopClipRect();
        return SymbolUi.Child.CustomUiResult.Rendered 
               | SymbolUi.Child.CustomUiResult.PreventOpenSubGraph 
               | SymbolUi.Child.CustomUiResult.PreventInputLabels
               | SymbolUi.Child.CustomUiResult.PreventTooltip;
    }
}