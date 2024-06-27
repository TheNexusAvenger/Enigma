--Tests StringDeserializer.
--!strict

local ReplicatedStorage = game:GetService("ReplicatedStorage")
local StringDeserializer = require(ReplicatedStorage.Enigma.Serialization.StringDeserializer)

return function()
    describe("A StringDeserializer with a string input", function()
        it("should read single strings.", function()
            local TestStringDeserializer = StringDeserializer.new("Test")
            expect(TestStringDeserializer:ReadString()).to.equal("Test")
        end)
        
        it("should read multiple strings.", function()
            local TestStringDeserializer = StringDeserializer.new("Test1|Test2")
            expect(TestStringDeserializer:ReadString()).to.equal("Test1")
            expect(TestStringDeserializer:ReadString()).to.equal("Test2")
        end)

        it("should throw an error when reading too many strings.", function()
            local TestStringDeserializer = StringDeserializer.new("Test")
            expect(TestStringDeserializer:ReadString()).to.equal("Test")
            expect(function()
                TestStringDeserializer:ReadString()
            end).to.throw("Reached end of serialized string.")
        end)

        it("should read numbers.", function()
            local TestStringDeserializer = StringDeserializer.new("1|2")
            expect(TestStringDeserializer:ReadNumber()).to.equal(1)
            expect(TestStringDeserializer:ReadNumber()).to.equal(2)
        end)

        it("should read Vector3s.", function()
            local TestStringDeserializer = StringDeserializer.new("1|2|3")
            local Vector = TestStringDeserializer:ReadVector3()
            expect(Vector.X).to.equal(1)
            expect(Vector.Y).to.equal(2)
            expect(Vector.Z).to.equal(3)
        end)

        it("should read Quaternions.", function()
            local TestStringDeserializer = StringDeserializer.new("1|2|3|4")
            local Vector = TestStringDeserializer:ReadQuaternion()
            expect(Vector.X).to.equal(1)
            expect(Vector.Y).to.equal(2)
            expect(Vector.Z).to.equal(3)
            expect(Vector.W).to.equal(4)
        end)
    end)
end