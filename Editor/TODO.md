﻿## Refactor project handling 

- [x] Remove GraphWindow.CanOpenAnotherWindow
- [x] Prevent last project window from being closed
- [x] Show project after NewProjectDialog -> ProjectManager.TryOpenInstancePath().
- [x] Test AppMenu -> New Graph Window
- [x] fix view regions when navigating in/out
- [ ] unload projects from project list
- [x] somewhat presentable project list
- [ ] Load last project from user settings
- [x] focus graph imgui-window after creation
- [x] Fix parameter window
- [x] Update view after TimeClip Split

Graph:
- [x] Fix initial view 
- [x] Show all content of initial view scope is empty
- [x] Add input parameters
- [x] Add output parameters
- [x] Remove input output params
- [x] Remove output params
- [ ] Fix background control in mag graph
- [x] Fix fence selection for timeline and preset view
- [ ] Add annotations
- [x] Fix MultiInput on disconnect re-layout
- [ ] Add options for hidden secondary outputs
- [x] Fix overlapping outputs in RevisionPanic
- [x] Rename children with Return
- [ ] Fix dragging from parameter window
- [x] Fix extracting from parameter window

- [x] Fix tab search result output type filtering
- [ ] Improve connecting from additional (hidden) outputs
- [ ] Fix additional output warning
- [ ] Allow dragging connection from horizontal input slot
- [ ] Allow clicking vertical input slot
- [ ] Allow dragging connections from vertical output slot
- [ ] Split Connections
- [ ] Rewiring of vertical connection lines

General fixes:
- [ ] Default gradients are not loaded?
- [ ] Fix Scaling for Curve select with ALT-Key
- [ ] Deleting last output will cause crash

General UX-ideas:
- [ ] StatusProvideIcon should support non-warning indicator
- [ ] Separate Value Clamping for lower and upper values 

Long-Term ideas:
- [ ] Render-Settings should be a connection type, including texture sampling, culling, z-depth 


Feats:
- [x] Copy and Paste Values
- [ ] Maybe show tags in symbolBrowser / Placeholder

Ops
- [ ] Sort out obsolete pixtur examples
- [ ] Remove Time 2nd output
- [ ] Fix SnapToPoints
- [ ] Fix BoxGradient

Refactoring
- Remove ICanvas
- Refactor to use Scopes