--Calculates the multiplier to use with a rolloff to prevent excessive extrapolation.
--!strict

return function(NoRolloff: number, Rolloff: number, Multiplier: number): number
    if Multiplier <= NoRolloff then
        return Multiplier
    end
    return NoRolloff + (math.min((Multiplier - NoRolloff), Rolloff) / 2)
end