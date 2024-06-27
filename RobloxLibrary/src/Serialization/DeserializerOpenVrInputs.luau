--Deserializes OpenVR input data from Enigma.
--!strict

local StringDeserializer = require(script.Parent:WaitForChild("StringDeserializer"))

export type RawOpenVRInput = {
    HeadRelativeCFrame: CFrame,
    HeadRelativeVelocity: Vector3,
}

return function(Data: string): {RawOpenVRInput}
    --Prepare the deserialziation and ignore the first input (version number).
    local Deserializer = StringDeserializer.new(Data)
    Deserializer:ReadNumber()

    --Read the inputs.
    local RawInputs = {}
    for i = 1, Deserializer:ReadNumber() do
        local Position = Deserializer:ReadVector3()
        local Rotation = Deserializer:ReadQuaternion()
        local Velocity = Deserializer:ReadVector3()
        table.insert(RawInputs, {
            HeadRelativeCFrame = CFrame.new(Position.X, Position.Y, Position.Z, Rotation.X, Rotation.Y, Rotation.Z, Rotation.W),
            HeadRelativeVelocity = Velocity,
        })
    end
    return RawInputs
end