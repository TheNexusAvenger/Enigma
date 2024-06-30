--Enum for a tracker.
--!strict

export type TrackerRole = "None" | "Handed" | "LeftFoot" | "RightFoot" | "LeftShoulder" |
    "RightShoulder" | "LeftElbow" | "RightElbow" | "LeftKnee" | "RightKnee" |
    "Waist" | "Chest" | "Camera" | "Keyboard" | "Unsupported"

return {
    "None",
    "Handed",
    "LeftFoot",
    "RightFoot",
    "LeftShoulder",
    "RightShoulder",
    "LeftElbow",
    "RightElbow",
    "LeftKnee",
    "RightKnee",
    "Waist",
    "Chest",
    "Camera",
    "Keyboard",
    "Unsupported",
} :: {TrackerRole}