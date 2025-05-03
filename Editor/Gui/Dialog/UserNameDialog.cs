﻿using ImGuiNET;
using T3.Editor.Gui.Styling;
using T3.Editor.Gui.UiHelpers;
using GraphUtils = T3.Editor.UiModel.Helpers.GraphUtils;

namespace T3.Editor.Gui.Dialog;

internal sealed class UserNameDialog : ModalDialog
{
    private string _userName = string.Empty;
        
    protected override void OnShowNextFrame() => _userName = UserSettings.Config.UserName;

    internal void Draw()
    {
        if (BeginDialog("Edit username"))
        {
            ImGui.PushFont(Fonts.FontSmall);
            ImGui.TextUnformatted("Nickname");
            ImGui.PopFont();

            ImGui.SetNextItemWidth(150);

            if (ImGui.IsWindowAppearing())
                ImGui.SetKeyboardFocusHere();

            ImGui.InputText("##name", ref _userName, 32);

            CustomComponents
               .HelpText("Tooll will use this to group your projects into a namespace.\n\nIt should be short and not contain spaces or special characters.");
            ImGui.Spacing();

            if (CustomComponents.DisablableButton("Okay", GraphUtils.IsValidProjectName(_userName)))
            {
                try
                {
                    UserSettings.Config.UserName = _userName;
                }
                catch (Exception e)
                {
                    Log.Error($"Error while renaming user {e}");
                }

                ImGui.CloseCurrentPopup();
            }

            EndDialogContent();
        }

        EndDialog();
    }
}