--Companion plugin for Enigma (https://github.com/TheNexusAvenger/Enigma).
--Enigma relies on pasting into TextBoxes to transfer data. This is *very* destructive in Roblox Studio.
--This plugin relies on the HttpService on the local host to transfer data instead.
--!strict

local DATA_LOOP_DELAY = 1 / 30 --30hz (1800 requests per minute) is below the 2000 requests per minute max for plugins.
local HEARTBEAT_DELAY_SECONDS = 3
local DATA_VALUE_NAME = "__EnigmaPluginData"
local ENIGMA_BASE_URL = "http://localhost:52821/enigma"
local ENIMGA_HEARTBEAT_URL = `{ENIGMA_BASE_URL}/heartbeat`
local ENIMGA_DATA_URL = `{ENIGMA_BASE_URL}/data`

local ReplicatedStorage = game:GetService("ReplicatedStorage")
local HttpService = game:GetService("HttpService")
local RunService = game:GetService("RunService")

local DataFetchLoopActive = false
local SentRequests = 0



--[[
Sends an HTTP request and returns the text body.
Throws an error if it does not succeed.
--]]
local function SendHttpRequest(Url: string, Method: string): string
    local Response = HttpService:RequestAsync({
        Url = Url,
        Method = Method,
    })
    SentRequests += 1
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
    local Worked, Message = pcall(function()
        SendHttpRequest(ENIMGA_HEARTBEAT_URL, "POST")
    end)
    if not Worked and Message == "Number of requests exceeded limit" then
        warn(`[Enigma] Number of HTTP service requests exceeded ({SentRequests} requests completed). The refresh rate must be decreased.`)
    end
    return Worked
end

--[[
Sends a data request. Returns the response.
Throws an error if the request failed.
--]]
local function SendDataRequest(): string
    return SendHttpRequest(ENIMGA_DATA_URL, "GET")
end

--[[
Starts the data fetch loop.
The loop only runs in a run mode.
--]]
local function StartDataFetch(): ()
    --Return if the loop is active.
    if DataFetchLoopActive then return end
    if not RunService:IsRunMode() then return end
    DataFetchLoopActive = true

    --Continously fetch the data.
    local DataFetchLoopStepActive = false
    while DataFetchLoopActive do
        if not DataFetchLoopStepActive then
            DataFetchLoopStepActive = true
            task.spawn(function()
                xpcall(function()
                    local Data = SendDataRequest()
                    if not ReplicatedStorage:FindFirstChild(DATA_VALUE_NAME) then
                        local NewDataValue = Instance.new("StringValue")
                        NewDataValue.Name = DATA_VALUE_NAME
                        NewDataValue.Value = Data
                        NewDataValue.Archivable = false
                        NewDataValue.Parent = ReplicatedStorage
                    else
                        (ReplicatedStorage:FindFirstChild(DATA_VALUE_NAME) :: StringValue).Value = Data
                    end
                end, function()
                    DataFetchLoopActive = false
                end)
                DataFetchLoopStepActive = false
            end)
        end
        task.wait(DATA_LOOP_DELAY)
    end
end



--Clear total requests every minute.
task.spawn(function()
    while true do
        SentRequests = 0
        task.wait(60)
    end
end)

--Start the heartbeat loop.
while true do
    if SendHeartbeatRequest() then
        task.spawn(StartDataFetch)
    else
        DataFetchLoopActive = false
    end
    task.wait(HEARTBEAT_DELAY_SECONDS)
end