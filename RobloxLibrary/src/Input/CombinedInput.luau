--Combines multiple inputs together and returns the latest one that changes.
--When multiple inputs change, the first changed one is prioritized.
--!strict

local CombinedInput = {}
CombinedInput.__index = CombinedInput

export type GenericInput = {
    GetCurrentText: (self: GenericInput) -> (string),
} & any
export type CombinedInput = {
    Inputs: {GenericInput},
    LastText: string,
    LastTextValues: {[GenericInput]: string},
} & typeof(setmetatable({}, CombinedInput))


--[[
Creates a new CombinedInput.
--]]
function CombinedInput.new(Inputs: {GenericInput}): CombinedInput
    --Get the initial text values.
    local LastTextValues = {}
    for _, Input in Inputs do
        LastTextValues[Input] = Input:GetCurrentText()
    end

    --Create and return the value.
    return setmetatable({
        Inputs = Inputs,
        LastText = LastTextValues[Inputs[1]],
        LastTextValues = LastTextValues,
    }, CombinedInput) :: CombinedInput
end

--[[
Returns the current data in the TextBox.
Any data may be stored, including an empty string.
--]]
function CombinedInput.GetCurrentText(self: CombinedInput): string
    --Get the new values.
    local NewTextValues = {}
    for _, Input in self.Inputs do
        NewTextValues[Input] = Input:GetCurrentText()
    end

    --Update the last text if an input changed, starting at the first input.
    for _, Input in self.Inputs do
        if NewTextValues[Input] == self.LastTextValues[Input] then continue end
        self.LastText = NewTextValues[Input]
        break
    end

    --Return the new text.
    return self.LastText
end



return CombinedInput