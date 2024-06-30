--Tests DeserializerOpenVrInputs.
--!strict

local ReplicatedStorage = game:GetService("ReplicatedStorage")
local DeserializerOpenVrInputs = require(ReplicatedStorage.Enigma.Serialization.DeserializerOpenVrInputs)

return function()
    describe("The DeserializerOpenVrInputs function", function()
        it("should return 0 inputs.", function()
            expect(#DeserializerOpenVrInputs("1|0")).to.equal(0)
        end)

        it("should return multiple inputs.", function()
            local Inputs = DeserializerOpenVrInputs("1|2|0|1|2|3|4|5|6|7|8|9|10|4|11|12|13|14|15|16|17|18|19|20")
            expect(#Inputs).to.equal(2)
            expect(Inputs[1].TrackerRole).to.equal("None")
            expect(Inputs[1].HeadRelativeCFrame.X).to.equal(1)
            expect(Inputs[1].HeadRelativeCFrame.Y).to.equal(2)
            expect(Inputs[1].HeadRelativeCFrame.Z).to.equal(3)
            expect(Inputs[1].HeadRelativeVelocity.X).to.equal(8)
            expect(Inputs[1].HeadRelativeVelocity.Y).to.equal(9)
            expect(Inputs[1].HeadRelativeVelocity.Z).to.equal(10)
            expect(Inputs[2].TrackerRole).to.equal("LeftShoulder")
            expect(Inputs[2].HeadRelativeCFrame.X).to.equal(11)
            expect(Inputs[2].HeadRelativeCFrame.Y).to.equal(12)
            expect(Inputs[2].HeadRelativeCFrame.Z).to.equal(13)
            expect(Inputs[2].HeadRelativeVelocity.X).to.equal(18)
            expect(Inputs[2].HeadRelativeVelocity.Y).to.equal(19)
            expect(Inputs[2].HeadRelativeVelocity.Z).to.equal(20)
        end)
    end)
end