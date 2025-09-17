--Companion plugin for Enigma (https://github.com/TheNexusAvenger/Enigma).
--Enigma relies on pasting into TextBoxes to transfer data. This is *very* destructive in Roblox Studio.
--This plugin relies on the HttpService on the local host to transfer data instead.
--!strict

local HEARTBEAT_DELAY_SECONDS = 3
local DATA_VALUE_NAME = "__EnigmaPluginData"
local ENIGMA_BASE_URL = "http://localhost:52821/enigma"
local ENIMGA_HEARTBEAT_URL = `{ENIGMA_BASE_URL}/heartbeat`
local ENIMGA_STREAM_URL = `{ENIGMA_BASE_URL}/stream`

local ReplicatedStorage = game:GetService("ReplicatedStorage")
local HttpService = game:GetService("HttpService")
local RunService = game:GetService("RunService")

local DataFetchActive = false



--[[
Sends an HTTP request and returns the text body.
Throws an error if it does not succeed.
--]]
local function SendHttpRequest(Url: string, Method: string): string
    local Response = HttpService:RequestAsync({
        Url = Url,
        Method = Method,
    })
    if not Response.Success then
        local Message = `[Enigma] Response was HTTP {Response.StatusCode} from {Method} {Url}: {Response.Body}`
        warn(Message)
        error(Message)
    end
    return Response.Body
end

--[[
Sends a heartbeat request. Returns if it was successful.
--]]
local function SendHeartbeatRequest(): boolean
    --Send the heartbeat request.
    local Worked, _ = pcall(function()
        SendHttpRequest(ENIMGA_HEARTBEAT_URL, "POST")
    end)

    --Return if the heartbeat worked.
    return Worked
end

--[[
Starts listening for data.
The listening only runs in a run mode.
--]]
local function StartDataFetch(): ()
    --Return if the loop is active.
    if DataFetchActive then return end
    if not RunService:IsRunning() then return end
    DataFetchActive = true

    --Listen to the stream.
    local EventClient = HttpService:CreateWebStreamClient(Enum.WebStreamClientType.SSE, {
        Url = ENIMGA_STREAM_URL,
        Method = "GET",
    })
    local EventConnection = EventClient.MessageReceived:Connect(function(Data: string)
        local ExistingDataValue = ReplicatedStorage:FindFirstChild(DATA_VALUE_NAME) :: StringValue
        if not ExistingDataValue then
            local NewDataValue = Instance.new("StringValue")
            NewDataValue.Name = DATA_VALUE_NAME
            NewDataValue.Value = Data
            NewDataValue.Archivable = false
            NewDataValue.Parent = ReplicatedStorage
        else
            ExistingDataValue.Value = Data
        end
    end)
    EventClient.Closed:Wait()
    EventConnection:Disconnect()
end



--Start the heartbeat loop.
while true do
    if SendHeartbeatRequest() then
        task.spawn(StartDataFetch)
    else
        DataFetchActive = false
    end
    task.wait(HEARTBEAT_DELAY_SECONDS)
end