--Main module for managing UserCFrames from Enigma.
--!strict

local LAST_INPUT_TIMEOUT_SECONDS = 0.5

local TrackerRole = require(script:WaitForChild("Data"):WaitForChild("TrackerRole"))
local CombinedInput = require(script:WaitForChild("Input"):WaitForChild("CombinedInput"))
local CompanionPluginInput = require(script:WaitForChild("Input"):WaitForChild("CompanionPluginInput"))
local TextBoxInput = require(script:WaitForChild("Input"):WaitForChild("TextBoxInput"))
local DeserializeOpenVrInputs = require(script:WaitForChild("Serialization"):WaitForChild("DeserializeOpenVrInputs"))
local CalculateRolloff = require(script:WaitForChild("Util"):WaitForChild("CalculateRolloff"))

local Enigma = {
    TrackerRoles = TrackerRole,
    Input = nil :: CombinedInput.CombinedInput?,
    LastInputData = "",
    LastInputTime = tick(),
    LastInputs = {} :: {[TrackerRole]: {DeserializeOpenVrInputs.RawOpenVRInput}},
}

export type TrackerRole = TrackerRole.TrackerRole
export type Enigma = typeof(Enigma)



--[[
Updates the stored UserCFrames.
--]]
function Enigma.UpdateUserCFrames(self: Enigma): ()
    --Return if the input was not initialized.
    if not self.Input then
        warn("Enigma is not enabled. Call Enigma:Enable() before calling.")
        return
    end

    --Return if the data is unchanged.
    --Clear the inputs if it has been enough time since the last update.
    local NewInputData = self.Input:GetCurrentText()
    if NewInputData == self.LastInputData then
        if tick() - self.LastInputTime >= LAST_INPUT_TIMEOUT_SECONDS then
            self.LastInputs = {}
        end
        return
    end

    --Parse and store the inputs.
    --Done in a pcall in case the text can't be parsed.
    pcall(function()
        local NewInputs = {}
        for _, NewRawInput in DeserializeOpenVrInputs(NewInputData) do
            if not NewInputs[NewRawInput.TrackerRole] then NewInputs[NewRawInput.TrackerRole] = {} end
            table.insert(NewInputs[NewRawInput.TrackerRole], NewRawInput)
        end
        self.LastInputData = NewInputData
        self.LastInputTime = tick()
        self.LastInputs = NewInputs
    end)
end

--[[
Returns if a UserCFrame is active.
An optional index can be passed in for when a user has multiple trackers for the same role.
--]]
function Enigma.GetUserCFrameEnabled(self: Enigma, UserCFrame: TrackerRole, Index: number?): boolean
    self:UpdateUserCFrames()
    local UserCFrameGroup = self.LastInputs[UserCFrame]
    return UserCFrameGroup ~= nil and UserCFrameGroup[Index or 1] ~= nil
end

--[[
Returns the CFrame for a UserCFrame.
The result will be nil if the user does not have a tracker for the UserCFrame.
An optional index can be passed in for when a user has multiple trackers for the same role.
--]]
function Enigma.GetUserCFrame(self: Enigma, UserCFrame: TrackerRole, Index: number?): CFrame?
    self:UpdateUserCFrames()
    local UserCFrameGroup = self.LastInputs[UserCFrame]
    if not UserCFrameGroup then return nil end
    local RawInputData = UserCFrameGroup[Index or 1]
    if not RawInputData then return nil end
    local TimeOffset = CalculateRolloff(0.03, 0.03 * 3, tick() - self.LastInputTime)
    return CFrame.new(RawInputData.HeadRelativeVelocity * TimeOffset) * RawInputData.HeadRelativeCFrame
end

--[[
Enables inputs for Enigma.
--]]
function Enigma.Enable(self: Enigma): ()
    if self.Input then return end
    self.Input = CombinedInput.new({TextBoxInput.new(), CompanionPluginInput.new()} :: {any})
end



return Enigma