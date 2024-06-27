--Deserializes strings from Enigma's StringSerializer format.
--Due to pasting sometimes appending instead of replacing, this method is preferred over JSON.
--!strict

local StringDeserializer = {}
StringDeserializer.__index = StringDeserializer

export type Quaternion = {
    X: number,
    Y: number,
    Z: number,
    W: number,
}
export type StringDeserializer = {
    Index: number,
    Data: {string},
} & typeof(setmetatable({}, StringDeserializer))



--[[
Creates a new StringDeserializer for a message.
--]]
function StringDeserializer.new(Data: string): StringDeserializer
    return setmetatable({
        Index = 1,
        Data = string.split(Data, "|")
    }, StringDeserializer) :: StringDeserializer
end

--[[
Returns the next string.
--]]
function StringDeserializer.ReadString(self: StringDeserializer): string
    local Index = self.Index
    if not self.Data[Index] then error("Reached end of serialized string.") end
    self.Index += 1
    return self.Data[Index]
end

--[[
Returns the next number.
--]]
function StringDeserializer.ReadNumber(self: StringDeserializer): number
    return tonumber(self:ReadString()) :: number
end

--[[
Returns the next Vector3.
--]]
function StringDeserializer.ReadVector3(self: StringDeserializer): Vector3
    return Vector3.new(self:ReadNumber(), self:ReadNumber(), self:ReadNumber())
end

--[[
Returns the next Quaternion in the format of X, Y, Z, W.
--]]
function StringDeserializer.ReadQuaternion(self: StringDeserializer): Quaternion
    return {
        X = self:ReadNumber(),
        Y = self:ReadNumber(),
        Z = self:ReadNumber(),
        W = self:ReadNumber(),
    }
end



return StringDeserializer