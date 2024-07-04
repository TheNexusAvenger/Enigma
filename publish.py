"""
TheNexusAvenger

Creates the binaries for distribution.
"""

import os
import platform
import shutil
import subprocess

PROJECTS = [
    {
        "project": "Enigma.Cli",
        "targetExecutable": "enigma-cli",
    },
]

# Set the platforms.
# macOS and Linux currently aren't supported in the code.
# See the README about potential support.
if platform.system() == "Windows":
    print("Building for Windows.")
    buildMode = "Windows"
    PLATFORMS = [
        {
            "name": "Windows-x64",
            "runtime": "win-x64"
        },
    ]
else:
    print("Unsupported platform: " + platform.system())
    exit(1)
print("")


# Create the directory.
if os.path.exists("bin"):
    shutil.rmtree("bin")
os.mkdir("bin")

# Compile the Roblox projects.
subprocess.call(["rojo", "build", "./default.project.json", "--output", "./bin/Enigma.rbxmx"])
subprocess.call(["rojo", "build", "./plugin.project.json", "--output", "./bin/EnigmaCompanionPlugin.rbxmx"])

# Compile the .NET releases.
for platform in PLATFORMS:
    platformName = platform["name"]
    platformRuntime = platform["runtime"]
    for project in PROJECTS:
        projectName = project["project"]
        sourceExecutable = platformName
        targetExecutable = project["targetExecutable"]
        if "win" in platformRuntime:
            sourceExecutable += ".exe"
            targetExecutable += ".exe"

        # Compile the project for the platform.
        print("Exporting " + projectName + " for " + platformName)
        subprocess.call(["dotnet", "publish", "-r", platformRuntime, "-c", "Release", projectName + "/" + projectName + ".csproj"])

        # Clear the unwanted files of the compile.
        dotNetVersion = os.listdir(projectName + "/bin/Release/")[0]
        outputDirectory = projectName + "/bin/Release/" + dotNetVersion + "/" + platformRuntime + "/publish"
        for file in os.listdir(outputDirectory):
            if file.endswith(".pdb"):
                os.remove(outputDirectory + "/" + file)
        if len(os.listdir(outputDirectory)) == 0 or (not os.path.exists(outputDirectory + "/" + projectName) and not os.path.exists(outputDirectory + "/" + projectName + ".exe")):
            print("Build for " + projectName + " failed and will not be created.")
            continue
        
        # Copy the companion plugin.
        shutil.copyfile("./bin/EnigmaCompanionPlugin.rbxmx", outputDirectory + "/EnigmaCompanionPlugin.rbxmx")
        
        # Rename the executables.
        sourceExecutablePath = outputDirectory + "/" + sourceExecutable
        targetExecutablePath = outputDirectory + "/" + targetExecutable
        if os.path.exists(sourceExecutablePath):
            os.rename(sourceExecutablePath, targetExecutablePath)

        # Create the archive.
        print(projectName + "/bin/Release/" + dotNetVersion + "/" + platformName + "/publish")
        shutil.make_archive("bin/" + project["targetExecutable"] + "-" + platformName, "zip", projectName + "/bin/Release/" + dotNetVersion + "/" + platformRuntime + "/publish")