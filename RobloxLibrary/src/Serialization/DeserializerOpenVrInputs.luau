--Deserializes OpenVR input data from Enigma.
--!strict

local TrackerRole = require(script.Parent.Parent:WaitForChild("Data"):WaitForChild("TrackerRole"))
local StringDeserializer = require(script.Parent:WaitForChild("StringDeserializer"))

export type RawOpenVRInput = {
    TrackerRole: TrackerRole.TrackerRole,
    HeadRelativeCFrame: CFrame,
    HeadRelativeVelocity: Vector3,
}

return function(Data: string): {RawOpenVRInput}
    --Prepare the deserialziation and check the protocol version.
    local Deserializer = StringDeserializer.new(Data)
    local ProcotolVersion = Deserializer:ReadNumber()
    if ProcotolVersion ~= 1 then
        warn(`Enigma protocol version {ProcotolVersion} is unsupported and might not work as expected. Supported versions: 1`)
    end

    --Read the inputs.
    local RawInputs = {}
    for i = 1, Deserializer:ReadNumber() do
        local Role = TrackerRole[Deserializer:ReadNumber() + 1] or "Unknown" :: TrackerRole.TrackerRole --Protocol starts counting at 0, but Lua starts at 1.
        local Position = Deserializer:ReadVector3()
        local Rotation = Deserializer:ReadQuaternion()
        local Velocity = Deserializer:ReadVector3()
        table.insert(RawInputs, {
            TrackerRole = Role,
            HeadRelativeCFrame = CFrame.new(Position.X, Position.Y, Position.Z, Rotation.X, Rotation.Y, Rotation.Z, Rotation.W),
            HeadRelativeVelocity = Velocity,
        })
    end
    return RawInputs
end