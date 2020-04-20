local UIWindowManager = class("UIWindowManager")

UIWindowManager.CommandType = {
    EnterWindow = 1,
    ExitWindow = 2,
    DestroyWindow = 3,
}

function UIWindowManager:initialize()
    self.windowStack = {}
    self.enteredWindowDict = {}
    self.cacheWindowDict = {}
    self.commandQueue = {}
    self.curCommand = nil
    self.curWindow = nil
    self.hintRootDict = {}
end

function UIWindowManager:Update()
    if self.curWindow then
        local windowName = self.curWindow.windowName
        local state = self.curWindow.state
        if state == UIWindow.WindowType.Entering or state == UIWindow.WindowType.Exiting or 
            state == UIWindow.WindowType.Loading then
            return
        end

        if state == UIWindow.WindowType.Exited and
            if self.curCommand.cType == UIWindowManager.WindowType.DestroyWindow then
                self.curWindow:Destroy()
                self.curWindow = nil
            else
                self.cacheWindowDict[windowName] = self.curWindow
                self.curWindow = nil
            end
        end

        self.curWindow = nil
    end

    local commandLen = #self.commandQueue
    if commandLen == 0 then
        return
    end

    local command = self.commandQueue[commandLen]
    table.remove(self.commandQueue, commandLen)
    self:RunCommand(command)
end

function UIWindowManager:EnterWindow(name)
    local command = {}
    command.cType = UIWindowManager.CommandType.EnterWindow
    command.windowName = name

    if not self.curWindow then
        self:RunCommand(command)
    else
        table.insert(self.commandQueue, 0, command)
    end
end

function UIWindowManager:ExitWindow(name)
    local command = {}
    command.cType = UIWindowManager.CommandType.ExitWindow
    command.windowName = name

    if not self.curWindow then
        self:RunCommand(command)
    else
        table.insert(self.commandQueue, 0, command)
    end
end

function UIWindowManager:RunCommand(command)
    local windowName = command.windowName
    self.curCommand = command

    if command.cType == UIWindowManager.CommandType.EnterWindow then
        self:EnterWindowImmediately(windowName)
    elseif command.cType == UIWindowManager.CommandType.ExitWindow then
        self:ExitWindowImmediately()
    elseif command.cType == UIWindowManager.CommandType.DestroyWindow then
        self:ExitWindowImmediately()
    end
end

function UIWindowManager:EnterWindowImmediately(name)
    local uiWindow = nil
    local onWindowLoaded = function(name)
        local uiSetting = WindowSetting[name]
        if uiSetting.windowType == UIWindow.WindowType.Stack then
            local index = self:GetStackIndex()
            table.remove(self.windowStack, index)
            tbale.insert(self.windowStack, name)

            self.enteredWindowDict[name].hintGo:SetAsLastSibling()
            self:AdjustStackWindows()
        end
    end

    --如果是已经打开的界面
    if self.enteredWindowDict[name] then
        uiWindow = self.enteredWindowDict[name]
        uiWindow.hintGo:SetActive(true)
        self.curWindow = uiWindow

        onWindowLoaded(name)
        return
    end

    if self.cacheWindowDict[name] then
        --如果是有缓存的界面
        uiWindow = self.cacheWindowDict[name]
        self.enteredWindowDict[name] = uiWindow
        self.cacheWindowDict[name] = nil
        uiWindow.hintGo:SetActive(true)
        self.curWindow = uiWindow

        onWindowLoaded(name)
        uiWindow:Enter()
    else
        --如果是 需要重新创建的界面
        self:CreateUIWindow(name, function(uwd)
            self.enteredWindowDict[name] = uwd
            onWindowLoaded(name)
            uwd:Enter()
        end)
    end
end

function UIWindowManager:ExitWindowImmediately(name)
    local uiWindow = nil
    local index = 0

    if not self.enteredWindowDict[name] then
        return
    end

    index = self:GetStackIndex(name)
    table.remove(self.windowStack, index)

    uiWindow = self.enteredWindowDict[name]
    self.enteredWindowDict[name] = nil
    self.curWindow = uiWindow
    uiWindow:Exit()
end

function UIWindowManager:CreateUIWindow(name, onLoadedCallBack)
    local uiSetting = WindowSetting[name]
    local ui = require(string.format("Game/UI/%s", name))
    local uiWindow = ui:new(name)
    local hintTransform = nil
    
    uiWindow.hintGo = UGameObject(name)
    hintTransform = uiWindow.hintGo.transform
    hintTransform.parent = self.hintRootDict[uiSetting.windowType]
    hintTransform.localPosition = UVector3(0, 0, 0)
    hintTransform.localScale = UVector3(1, 1, 1)

    uiWindow.state = UIWindow.WindowType.Loading
    self.curWindow = curWindow

    ResourceManager.LoadAsync(name, 
        function(res)
            local insGo = UObjectInstantiate(res)
            local insTransform = insGo.transform
            insGo:SetActive(true)
            insTransform.parent = uiWindow.hintGo.transform
            insTransform.localPosition = UVector3(0, 0, 0)
            insTransform.localScale = UVector3(1, 1, 1)
            uiWindow.mainGo = insGo
            uiWindow.simpleAnimations = {}

			local animations = insGo:GetComponentsInChildren(typeof(CS.SimpleAnimation))
			if not IsNull(animations) then
				local len = animations.Length - 1
				for index = 0, len do
					local simpleAnimation = animations[index]
					simpleAnimation.playAutomatically = false
					simpleAnimation:Stop()
					table.insert(uiWindow.simpleAnimations, simpleAnimation)
				end
			end
             
            uiWindow:Loaded()
            if onLoadedCallBack then
                onLoadedCallBack(uiWindow)
            end
        end
    )
end

function UIWindowManager:AdjustStackWindows()
    local stackCount = #self.windowStack
    local isPreFullScreen = false
    local windowName = nil
    local uiSetting = nil
    local uiWindow = nil
    for i=stackCount, 1, -1 do
        windowName = self.windowStack[i]
        uiSetting = WindowSetting[windowName]
        uiWindow = self.enteredWindowDict[windowName]

        if isPreFullScreen then
            uiWindow.hintGo:SetActive(false)
        else
            uiWindow.hintGo:SetActive(true)
        end

        if uiSetting.isFullScreen then
            isPreFullScreen = true
        end
    end
end

function UIWindowManager:GetStackIndex(name)
    local index = 0
    for i=1, #self.windowStack then
        if self.windowStack[i] == name then
            index = i
        end
    end
    return index
end

function UIWindowManager