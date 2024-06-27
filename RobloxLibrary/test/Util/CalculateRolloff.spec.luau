--Tests CalculateRolloff.
--!strict

local ReplicatedStorage = game:GetService("ReplicatedStorage")
local CalculateRolloff = require(ReplicatedStorage.Enigma.Util.CalculateRolloff)

return function()
    describe("The CalculateRolloff function", function()
        it("should the input when not past the rolloff.", function()
            expect(CalculateRolloff(1, 2, 0)).to.equal(0)
            expect(CalculateRolloff(1, 2, 0.5)).to.equal(0.5)
            expect(CalculateRolloff(1, 2, 1)).to.equal(1)
        end)

        it("should roll off as the input gets too high.", function()
            expect(CalculateRolloff(1, 2, 2)).to.equal(1.5)
            expect(CalculateRolloff(1, 2, 3)).to.equal(2)
            expect(CalculateRolloff(1, 2, 4)).to.equal(2)
        end)
    end)
end