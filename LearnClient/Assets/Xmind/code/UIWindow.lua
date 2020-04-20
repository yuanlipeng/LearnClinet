local UIWindow = class("UIWindow")

UIWindow.WindowState = {
	Invaild = 0,
	Loading = 1,
	Entering = 2,
	Entered = 3,
	Exiting = 4,
	Exited = 5
}

function UIWindow:initialize()
	self.hintGo = nil
	self.mainGo = nil
	self.windowName = nil
	self.state = nil
	self.initParams = nil
	self.simpleAnimations = nil
	self.rootCanvasGroup = nil
end

function UIWindow:Load(go)
	self.hintGo = go

	self.state = UIWindow.WindowState.Loading
end

function UIWindow:Loaded(go)
	self.mainGo = go

	self:OnLoaded()
end

function UIWindow:Destroy()
	self:OnDestroy()

	GameObject.Destroy(self.mainGo)
	GameObject.Destroy(self.hintGo)
end

function UIWindow:Enter()
	local animationLength = 0
	self.state = UIWindow.WindowState.Entering
	self:OnEnter()

	animationLength = self:PlayAnimation("Enter")
	if animationLength <= 0 then
		GetGame().unityTimer:CreateTimer(animationLength,
			function()
				self:OnEnterFinished()
				self.state = UIWindow.WindowState.Entered
			end
		)
	else
		self:OnEnterFinished()
		self.state = UIWindow.WindowState.Entered
	end
end

function UIWindow:Exit()
	local animationLength = 0
	self.state = UIWindow.WindowState.Exiting
	self:OnExit()

	animationLength = self:PlayAnimation("Exit")
	if animationLength <= 0 then
		GetGame().unityTimer:CreateTimer(animationLength,
			function()
				self:OnExitFinished()
				self.state = UIWindow.WindowState.Exited
			end
		)
	else
		self:OnExitFinished()
		self.state = UIWindow.WindowState.Exited
	end
end

function UIWindow:PlayAnimation(clipName)
	if not self.simpleAnimations then
		return 0
	end

	local playList = {}
	local maxClipTime = 0
	for index = 1, #self.simpleAnimations do
		local animation = self.simpleAnimations[index]
		if not IsNull(animation) and animation.enabled then
			local clip = animation:GetClip(clipName)
			if not IsNull(clip) then
				if clip.length > maxClipTime then
					maxClipTime = clip.length
				end
				table.insert(playList, index)
			end
		end
	end

	if maxClipTime <= 0 then
		return 0
	end

	for i=1, #playList do
		self.simpleAnimations[playList[i]]:Play(clipName)
		self.simpleAnimations[playList[i]]:Sample()
	end

	return maxClipTime
end

--override
function UIWindow:OnLoaded()
end

function UIWindow:OnEnter()
end

function UIWindow:OnEnterFinished()
end


function UIWindow:OnExit()
end

function UIWindow:OnExitFinished()
end

function UIWindow:OnDestroy()
end