local WindowSetting = {}

WindowSetting.WindowType = {
    RockBottom,
    Bottom,
    Stack,
    Top,
}


--[[
windowType : 界面挂载的层级
isFullScreen : 是否是全屏界面
handleNotch : 界面适配类型
isDestroyWhenExit：是否在关闭界面时 立马销毁资源
isDestroyWhenSceneChange：是否在场景切换时， 自动关闭界面并销毁资源。
]]--
WindowSetting = {
    UILogin = { windowType = WindowSetting.WindowType.Top, isFullScreen = false, isDestroyWhenExit = false, isDestroyWhenSceneChange = true, },

    UIBag = { windowType = WindowSetting.WindowType.Bottom, isFullScreen = false, isDestroyWhenExit = true, isDestroyWhenSceneChange = false, },

    UIEquip = { windowType = WindowSetting.WindowType.Top, isFullScreen = false, isDestroyWhenExit = true, isDestroyWhenSceneChange = true, },

    UITest = { windowType = WindowSetting.WindowType.Top, isFullScreen = true, isDestroyWhenExit = true, isDestroyWhenSceneChange = false, },
}

return WindowSetting