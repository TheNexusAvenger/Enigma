--Reads data from the StringValue created by the Enigma companion plugin.
--!strict

local DATA_VALUE_NAME = "__EnigmaPluginData"

local ReplicatedStorage = game:GetService("ReplicatedStorage")

local CompanionPluginInput = {}
CompanionPluginInput.__index = CompanionPluginInput

export type CompanionPluginInput = typeof(setmetatable({}, CompanionPluginInput))



--[[
Creates a new CompanionPluginInput.
--]]
function CompanionPluginInput.new(): CompanionPluginInput
    return setmetatable({}, CompanionPluginInput) :: CompanionPluginInput
end

--[[
Returns the current data in the TextBox.
Any data may be stored, including an empty string.
--]]
function CompanionPluginInput.GetCurrentText(self: CompanionPluginInput): string
    local DataValue = ReplicatedStorage:FindFirstChild(DATA_VALUE_NAME) :: StringValue
    if not DataValue then return "" end
    return DataValue.Value
end



return CompanionPluginInput