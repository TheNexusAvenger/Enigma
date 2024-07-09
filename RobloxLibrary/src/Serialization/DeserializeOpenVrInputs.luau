--Deserializes OpenVR input data from Enigma.
--!strict

local BASE_QUATERNION = {X = 0, Y = 0, Z = 0, W = 1}

local TrackerRole = require(script.Parent.Parent:WaitForChild("Data"):WaitForChild("TrackerRole"))
local StringDeserializer = require(script.Parent:WaitForChild("StringDeserializer"))

local WarnedMessages = {}

export type RawOpenVRInput = {
    TrackerRole: TrackerRole.TrackerRole,
    HeadRelativeCFrame: CFrame,
    HeadRelativeVelocity: Vector3,
}



--[[
Displays a warning message once.
--]]
local function WarnOnce(Message: string): ()
    if WarnedMessages[Message] then return end
    WarnedMessages[Message] = true
    warn(Message)
end



return function(Data: string): {RawOpenVRInput}
    --Prepare the deserialziation and check the protocol version.
    local Deserializer = StringDeserializer.new(Data)
    local ProcotolVersion = Deserializer:ReadNumber()
    local TotalTrackers = Deserializer:ReadNumber()
    if ProcotolVersion ~= 1 and ProcotolVersion ~= 2 then
        WarnOnce(`Enigma protocol version {ProcotolVersion} is unsupported and might not work as expected. Supported versions: 1, 2`)
    end

    --Read the inputs.
    local RawInputs = {}
    if ProcotolVersion == 1 then
        --Protocol version 1 was deprecated due to having no way to be forwards-compatible with new versions.
        for _ = 1, TotalTrackers do
            local Role = TrackerRole[Deserializer:ReadNumber() + 1] or "Unsupported" --Protocol starts counting at 0, but Lua starts at 1.
            local Position = Deserializer:ReadVector3()
            local Rotation = Deserializer:ReadQuaternion()
            local Velocity = Deserializer:ReadVector3()
            table.insert(RawInputs, {
                TrackerRole = Role :: TrackerRole.TrackerRole,
                HeadRelativeCFrame = CFrame.new(Position.X, Position.Y, Position.Z, Rotation.X, Rotation.Y, Rotation.Z, Rotation.W),
                HeadRelativeVelocity = Velocity,
            })
        end
    else
        --Protocol version 2 and higher *in theory* should be compatible with future versions.
        for _ = 1, TotalTrackers do
            local Role = "Unsupported"
            local Position = Vector3.zero
            local Rotation = BASE_QUATERNION
            local Velocity = Vector3.zero

            --Read the properties.
            local TotalProperties = Deserializer:ReadNumber()
            for _ = 1, TotalProperties do
                local PropertyId = Deserializer:ReadNumber()
                local TotalValues = Deserializer:ReadNumber()
                if PropertyId == 0 then --Tracker role
                    Role = TrackerRole[Deserializer:ReadNumber() + 1] or "Unsupported" --Protocol starts counting at 0, but Lua starts at 1.
                elseif PropertyId == 1 then --Position
                    Position = Deserializer:ReadVector3()
                elseif PropertyId == 2 then --Rotation
                    Rotation = Deserializer:ReadQuaternion()
                elseif PropertyId == 3 then --Velocity
                    Velocity = Deserializer:ReadVector3()
                else
                    local UnknownValues = {}
                    for _ = 1, TotalValues do
                        table.insert(UnknownValues, Deserializer:ReadString())
                    end
                    WarnOnce(`Unsupported property id {PropertyId} sent with {TotalValues} values: {table.concat(UnknownValues, "|")}`)
                end
            end

            --Add the input.
            table.insert(RawInputs, {
                TrackerRole = Role :: TrackerRole.TrackerRole,
                HeadRelativeCFrame = CFrame.new(Position.X, Position.Y, Position.Z, Rotation.X, Rotation.Y, Rotation.Z, Rotation.W),
                HeadRelativeVelocity = Velocity,
            })
        end

    end
    return RawInputs
end