using System.ComponentModel;
using System.Drawing;
using System.IO;
using ImGuiNET;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
using T3.Core.DataTypes.Vector;
using T3.Core.Resource;
using T3.Core.SystemUi;
using T3.Editor.Gui.Styling;
using T3.SDL2;
using Device = SharpDX.Direct3D11.Device;
using Icon = System.Drawing.Icon;
using Rectangle = System.Drawing.Rectangle;
using Resource = SharpDX.Direct3D11.Resource;
using Vector2 = System.Numerics.Vector2;

namespace T3.Editor.App;

/// <summary>
/// Functions and properties related to rendering DX11 content into  RenderForm windows
/// </summary>
internal sealed class AppWindow
{
    public IntPtr HwndHandle => SDLWindow;
    public Int2 Size => new(Width, Height);

    public int Width {
        get {
            int width, height;
            SDL.SDL_GetWindowSize(SDLWindow, out width, out height);
            return width;
        }
    }

    public int Height {
        get {
            int width, height;
            SDL.SDL_GetWindowSize(SDLWindow, out width, out height);
            return height;
        }
    }

    public bool IsFullScreen => (SDL.SDL_GetWindowFlags(SDLWindow) & (uint)SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN) != 0;

    internal SwapChain SwapChain { get => _swapChain; private set => _swapChain = value; }
    internal RenderTargetView RenderTargetView { get => _renderTargetView; private set => _renderTargetView = value; }
    internal nint SDLWindow { get; private set; }

    internal SwapChainDescription SwapChainDescription => new()
                                                              {
                                                                  BufferCount = 3,
                                                                  ModeDescription = new ModeDescription(Width,
                                                                                                        Height,
                                                                                                        new Rational(60, 1),
                                                                                                        Format.R8G8B8A8_UNorm),
                                                                  IsWindowed = true,
                                                                  OutputHandle = SDLWindow,
                                                                  SampleDescription = new SampleDescription(1, 0),
                                                                  SwapEffect = SwapEffect.Discard,
                                                                  Usage = Usage.RenderTargetOutput
                                                              };

    internal bool IsMinimized => (SDL.SDL_GetWindowFlags(SDLWindow) & (uint)SDL.SDL_WindowFlags.SDL_WINDOW_MINIMIZED) != 0;
    internal bool IsCursorOverWindow => (SDL.SDL_GetWindowFlags(SDLWindow) & (uint)SDL.SDL_WindowFlags.SDL_WINDOW_MOUSE_FOCUS) != 0;
    public Texture2D Texture { get; set; }

    internal AppWindow(string windowTitle, bool disableClose)
    {
        CreateWindow(windowTitle, disableClose);
    }

    public void SetVisible(bool isVisible)
    {
        // TODO: uuhh how to sdl this?
    }

    public void SetSizeable()
    {
        SDL.SDL_SetWindowResizable(SDLWindow, SDL.SDL_bool.SDL_TRUE);
        if (_boundsBeforeFullscreen.Height != 0 && _boundsBeforeFullscreen.Width != 0)
        {
            SDL.SDL_SetWindowSize(SDLWindow, _boundsBeforeFullscreen.Width, _boundsBeforeFullscreen.Height);
            SDL.SDL_SetWindowPosition(SDLWindow, _boundsBeforeFullscreen.X, _boundsBeforeFullscreen.Y);
        }
    }

    public void Show() => SDL.SDL_ShowWindow(SDLWindow);

    public Vector2 GetDpi()
    {
        float ddpi, hdpi, vdpi;
        // TODO: which display?
        SDL.SDL_GetDisplayDPI(0, out ddpi, out hdpi, out vdpi);
        Vector2 dpi = new(hdpi, vdpi);
        return dpi;
    }

    internal void SetFullScreen(int screenIndex)
    {
        int x, y, width, height;
        SDL.SDL_GetWindowPosition(SDLWindow, out x, out y);
        SDL.SDL_GetWindowSize(SDLWindow, out width, out height);
        _boundsBeforeFullscreen.X = x;
        _boundsBeforeFullscreen.Y = y;
        _boundsBeforeFullscreen.Width = width;
        _boundsBeforeFullscreen.Height = height;
        SDL.SDL_SetWindowFullscreen(SDLWindow, (uint)SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN);
    }

    internal void InitViewSwapChain(Factory factory)
    {
        SwapChain = new SwapChain(factory, _device, SwapChainDescription);
        SwapChain.ResizeBuffers(bufferCount: 3, Width, Height,
                                SwapChain.Description.ModeDescription.Format, SwapChain.Description.Flags);
    }

    internal void PrepareRenderingFrame()
    {
        _deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
        _deviceContext.Rasterizer.SetViewport(new Viewport(0, 0, Width, Height, 0.0f, 1.0f));
        _deviceContext.OutputMerger.SetTargets(RenderTargetView);

        var color = UiColors.WindowBackground.ToByte4();
        var sharpDxColor = new SharpDX.Color(color.X, color.Y, color.Z, color.W);
        _deviceContext.ClearRenderTargetView(RenderTargetView, sharpDxColor);
    }

    internal void RunRenderLoop(Action callback)
    {
        bool running = true;
        while (running)
        {
            var io = ImGui.GetIO();
            while (SDL.SDL_PollEvent(out SDL.SDL_Event e) == 1)
            {
                switch (e.type)
                {
                case SDL.SDL_EventType.SDL_QUIT:
                    running = false;
                    break;
                case SDL.SDL_EventType.SDL_MOUSEMOTION:
                case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN:
                case SDL.SDL_EventType.SDL_MOUSEBUTTONUP:
                    int x, y;
                    uint buttons = SDL.SDL_GetMouseState(out x, out y);
                    io.MousePos = new Vector2(x, y);
                    io.MouseDown[0] = (buttons & SDL.SDL_BUTTON_LMASK) != 0;
                    io.MouseDown[1] = (buttons & SDL.SDL_BUTTON_RMASK) != 0;
                    io.MouseDown[2] = (buttons & SDL.SDL_BUTTON_MMASK) != 0;
                    break;
                case SDL.SDL_EventType.SDL_KEYDOWN:
                    io.KeyCtrl = (e.key.keysym.mod & SDL.SDL_Keymod.KMOD_CTRL) != 0;
                    io.KeyShift = (e.key.keysym.mod & SDL.SDL_Keymod.KMOD_SHIFT) != 0;
                    io.KeyAlt = (e.key.keysym.mod & SDL.SDL_Keymod.KMOD_ALT) != 0;
                    io.KeysDown[(int)e.key.keysym.scancode] = true;
                    break;
                case SDL.SDL_EventType.SDL_KEYUP:
                    io.KeyCtrl = (e.key.keysym.mod & SDL.SDL_Keymod.KMOD_CTRL) != 0;
                    io.KeyShift = (e.key.keysym.mod & SDL.SDL_Keymod.KMOD_SHIFT) != 0;
                    io.KeyAlt = (e.key.keysym.mod & SDL.SDL_Keymod.KMOD_ALT) != 0;
                    io.KeysDown[(int)e.key.keysym.scancode] = false;
                    break;
                }
            }

            callback();
        }
    }

    internal void SetSize(int width, int height) => SDL.SDL_SetWindowSize(SDLWindow, width, height);

    internal void SetBorderStyleSizable() => SDL.SDL_SetWindowResizable(SDLWindow, SDL.SDL_bool.SDL_TRUE);

    internal void InitializeWindow(CancelEventHandler handleClose, bool handleKeys)
    {
        InitRenderTargetsAndEventHandlers();

        // if (handleKeys)
        // {
        //     MsForms.MsForms.TrackKeysOf(Form);
        // }
        //     
        // MsForms.MsForms.TrackMouseOf(Form);

        // if (handleClose != null)
        //     Form.Closing += handleClose;

        // Form.WindowState = windowState;
    }

    internal void SetDevice(Device device, DeviceContext deviceContext, SwapChain swapChain = null)
    {
        if (_hasSetDevice)
            throw new InvalidOperationException("Device has already been set");

        _hasSetDevice = true;
        _device = device;
        _deviceContext = deviceContext;
        _swapChain = swapChain;
    }

    internal void Release()
    {
        _renderTargetView.Dispose();
        _backBufferTexture.Dispose();
        _swapChain.Dispose();
    }

    private void CreateWindow(string windowTitle, bool disableClose)
    {
        SDLWindow = SDL.SDL_CreateWindow(windowTitle, SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, 1280, 720, SDL.SDL_WindowFlags.SDL_WINDOW_VULKAN);
        // TODO: SDL.SDL_AddEventWatch();
    }

    private void InitRenderTargetsAndEventHandlers()
    {
        var device = _device;
        _backBufferTexture = Resource.FromSwapChain<Texture2D>(SwapChain, 0);
        RenderTargetView = new RenderTargetView(device, _backBufferTexture);

        // Form.ResizeBegin += (sender, args) => _isResizingRightNow = true;
        // Form.ResizeEnd += (sender, args) =>
        //                   {
        //                       RebuildBackBuffer(SDLWindow, device, ref _renderTargetView, ref _backBufferTexture, ref _swapChain);
        //                       _isResizingRightNow = false;
        //                   };
        // Form.ClientSizeChanged += (sender, args) =>
        //                           {
        //                               if (_isResizingRightNow)
        //                                   return;

        //                               RebuildBackBuffer(SDLWindow, device, ref _renderTargetView, ref _backBufferTexture, ref _swapChain);
        //                           };
    }

    private static void RebuildBackBuffer(nint window, Device device, ref RenderTargetView rtv, ref Texture2D buffer, ref SwapChain swapChain)
    {
        rtv.Dispose();
        buffer.Dispose();
        int width, height;
        SDL.SDL_GetWindowSize(window, out width, out height);
        swapChain.ResizeBuffers(3, width, height, Format.Unknown, 0);
        buffer = Resource.FromSwapChain<Texture2D>(swapChain, 0);
        rtv = new RenderTargetView(device, buffer);
    }

    private bool _hasSetDevice;
    private Device _device;
    private DeviceContext _deviceContext;
    private SwapChain _swapChain;
    private RenderTargetView _renderTargetView;
    private Texture2D _backBufferTexture;
    public Texture2D BackBufferTexture => _backBufferTexture;
    private bool _isResizingRightNow;
    private Rectangle _boundsBeforeFullscreen;

    public void SetTexture(Texture2D texture)
    {
        Texture = texture;
    }
}