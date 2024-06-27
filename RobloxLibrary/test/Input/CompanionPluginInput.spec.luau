--Tests CompanionPluginInput.
--!strict

local ReplicatedStorage = game:GetService("ReplicatedStorage")
local CompanionPluginInput = require(ReplicatedStorage.Enigma.Input.CompanionPluginInput)

return function()
    describe("A CompanionPluginInput instance", function()
        local TestCompanionPluginInput = nil
        beforeEach(function()
            TestCompanionPluginInput = CompanionPluginInput.new()
        end)

        afterEach(function()
            local Value = ReplicatedStorage:FindFirstChild("__EnigmaPluginData")
            if Value then
                Value:Destroy()
            end
        end)

        it("should return an empty string when no StringValue is present.", function()
            expect(TestCompanionPluginInput:GetCurrentText()).to.equal("")
        end)

        it("should return the string when from the StringValue.", function()
            local DataValue = Instance.new("StringValue")
            DataValue.Name = "__EnigmaPluginData"
            DataValue.Value = "TestData"
            DataValue.Parent = ReplicatedStorage

            expect(TestCompanionPluginInput:GetCurrentText()).to.equal("TestData")
        end)
    end)
end