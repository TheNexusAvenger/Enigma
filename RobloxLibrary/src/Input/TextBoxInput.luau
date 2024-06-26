--Creates a TextBox that is locked to being focused to receive data.
--!strict

local HEARTBEAT_TIMEOUT_SECONDS = 0.5

local Players = game:GetService("Players")
local RunService = game:GetService("RunService")
local UserInputService = game:GetService("UserInputService")

local TextBoxInput = {}
TextBoxInput.__index = TextBoxInput

export type TextBoxInput = {
    WindowFocused: boolean,
    LastHeartbeatTime: number,
    Events: {RBXScriptConnection},
    TextBoxEvents: {RBXScriptConnection},
    ScreenGui: ScreenGui?,
    TextBox: TextBox?,
} & typeof(setmetatable({}, TextBoxInput))



--[[
Creates a new TextBoxInput.
--]]
function TextBoxInput.new(Parent: Instance?): TextBoxInput
    --Create the object.
    local self = setmetatable({
        WindowFocused = true,
        LastHeartbeatTime = 0,
        Events = {},
        TextBoxEvents = {},
    }, TextBoxInput) :: TextBoxInput
    self:CreateTextBox(Parent or Players.LocalPlayer:WaitForChild("PlayerGui"))
    
    --Force the focusing of the TextBox.
    table.insert(self.Events, UserInputService.InputBegan:Connect(function(Input)
        if Input.KeyCode ~= Enum.KeyCode.F13 then return end
        self.LastHeartbeatTime = tick()
        self:TryCaptureFocus()
    end))
    table.insert(self.Events, UserInputService.WindowFocused:Connect(function()
        self.WindowFocused = true
        self:TryCaptureFocus()
    end))
    table.insert(self.Events, UserInputService.WindowFocusReleased:Connect(function()
        self.WindowFocused = false
        self:TryReleaseFocus()
    end))
    table.insert(self.Events, RunService.Stepped:Connect(function()
        self:TryCaptureFocus()
    end))

    --Return the object.
    return self
end

--[[
Creates the ScreenGui and TextBox.
This may be called multiple times it is destroyed.
--]]
function TextBoxInput.CreateTextBox(self: TextBoxInput, Parent: Instance): ()
    --Clear the previous elements.
    for _, Event in self.TextBoxEvents do
        Event:Disconnect()
    end
    self.TextBoxEvents = {}
    if self.ScreenGui then
        self.ScreenGui:Destroy()
    end
    if self.TextBox then
        self.TextBox:Destroy()
    end

    --Create the ScreenGui and TextBox.
    local ScreenGui = Instance.new("ScreenGui")
    ScreenGui.Name = "EnigmaTextBoxInput"
    ScreenGui.ResetOnSpawn = false
    ScreenGui.Parent = Parent
    self.ScreenGui = ScreenGui

    local TextBox = Instance.new("TextBox")
    TextBox.BackgroundTransparency = 0.99
    TextBox.BorderSizePixel = 0
    TextBox.Size = UDim2.new(0, 1, 0, 1)
    TextBox.Selectable = false
    TextBox.ClipsDescendants = true
    TextBox.ClearTextOnFocus = true
    TextBox.TextTransparency = 0.99
    TextBox.Parent = ScreenGui
    self.TextBox = TextBox

    --Connect the TextBox becoming inaccessible.
    table.insert(self.TextBoxEvents, TextBox.AncestryChanged:Connect(function()
        self:CreateTextBox(Parent)
    end))
    for _, Property in {"Enabled"} do
        local OriginalValue = (ScreenGui :: any)[Property]
        table.insert(self.TextBoxEvents, ScreenGui:GetPropertyChangedSignal(Property):Connect(function()
            (ScreenGui :: any)[Property] = OriginalValue
        end))
    end
    for _, Property in {"BackgroundTransparency", "Size", "ClipsDescendants", "Visible", "TextTransparency"} do
        local OriginalValue = (TextBox :: any)[Property]
        table.insert(self.TextBoxEvents, TextBox:GetPropertyChangedSignal(Property):Connect(function()
            (TextBox :: any)[Property] = OriginalValue
        end))
    end
end

--[[
Returns the current data in the TextBox.
Any data may be stored, including an empty string.
--]]
function TextBoxInput.GetCurrentText(self: TextBoxInput): string
    if not self.TextBox then return "" end
    return self.TextBox.Text
end

--[[
Tries to capture the focus of the TextBox.
--]]
function TextBoxInput.TryCaptureFocus(self: TextBoxInput): ()
    --Return if the TextBox should not be focused.
    if not self.WindowFocused or (tick() - self.LastHeartbeatTime) > HEARTBEAT_TIMEOUT_SECONDS then
        self:TryReleaseFocus()
        return
    end
    if not self.TextBox then return end
    if UserInputService:GetFocusedTextBox() == self.TextBox then return end

    --Capture the focus of the TextBox.
    self.TextBox:CaptureFocus()
end

--[[
Tries to release the focus of the TextBox.
--]]
function TextBoxInput.TryReleaseFocus(self: TextBoxInput): ()
    if not self.TextBox then return end
    if UserInputService:GetFocusedTextBox() ~= self.TextBox then return end
    self.TextBox:ReleaseFocus()
end

--[[
Destroys the TextBoxInput.
--]]
function TextBoxInput.Destroy(self: TextBoxInput): ()
    --Clear the events.
    for _, Event in self.Events do
        Event:Disconnect()
    end
    self.Events = {}
    for _, Event in self.TextBoxEvents do
        Event:Disconnect()
    end
    self.TextBoxEvents = {}

    --Clear the objects.
    if self.ScreenGui then
        self.ScreenGui:Destroy()
    end
    self.ScreenGui = nil
    if self.TextBox then
        self.TextBox:Destroy()
    end
    self.TextBox = nil
end



return TextBoxInput