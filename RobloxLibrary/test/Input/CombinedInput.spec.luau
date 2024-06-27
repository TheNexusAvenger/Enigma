--Tests CombinedInput.
--!strict

local ReplicatedStorage = game:GetService("ReplicatedStorage")
local CombinedInput = require(ReplicatedStorage.Enigma.Input.CombinedInput)

return function()
    describe("A CombinedInput instance", function()
        local TestCombinedInput = nil
        local TestInputA, TestInputB = "Test1", "Test2"
        
        beforeEach(function()
            TestCombinedInput = CombinedInput.new({
                {
                    GetCurrentText = function(_)
                        return TestInputA
                    end,
                },
                {
                    GetCurrentText = function(_)
                        return TestInputB
                    end,
                },
            })
            TestInputA, TestInputB = "Test1", "Test2"
        end)

        afterEach(function()
            TestInputA, TestInputB = "Test1", "Test2"
        end)

        it("should return initial data.", function()
            expect(TestCombinedInput:GetCurrentText()).to.equal("Test1")
        end)

        it("should return the first input changing.", function()
            TestInputA = "Test3"
            expect(TestCombinedInput:GetCurrentText()).to.equal("Test3")
        end)

        it("should return the second input changing.", function()
            TestInputB = "Test4"
            expect(TestCombinedInput:GetCurrentText()).to.equal("Test4")
        end)

        it("should prioritize earlier inputs when changing.", function()
            TestInputA = "Test3"
            TestInputB = "Test4"
            expect(TestCombinedInput:GetCurrentText()).to.equal("Test3")
        end)

        it("should remember changes.", function()
            TestInputA = "Test3"
            expect(TestCombinedInput:GetCurrentText()).to.equal("Test3")
            expect(TestCombinedInput:GetCurrentText()).to.equal("Test3")
        end)
    end)
end