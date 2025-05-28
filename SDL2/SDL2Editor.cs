using System;
using T3.SystemUi;

namespace T3.SDL2;

public class SDL2Editor : IEditorSystemUiService
{
    public void OpenWithDefaultApplication(string uri)
    {
        throw new NotImplementedException();
    }

    public void ExitApplication()
    {
        throw new NotImplementedException();
    }

    public void ExitThread()
    {
        throw new NotImplementedException();
    }

    public ICursor Cursor { get; }
    public void SetUnhandledExceptionMode(bool throwException)
    {
        throw new NotImplementedException();
    }

    public void EnableDpiAwareScaling()
    {
        Console.WriteLine("TODO: DPI not implemented!!");
    }

    public void SetClipboardText(string text)
    {
        throw new NotImplementedException();
    }

    public string GetClipboardText()
    {
        throw new NotImplementedException();
    }

    public IFilePicker CreateFilePicker()
    {
        throw new NotImplementedException();
    }

    public IReadOnlyList<IScreen> AllScreens { get; }
}