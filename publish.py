"""
TheNexusAvenger

Creates the binaries for distribution.
"""

import json
import os
import platform
import re
import shutil
import subprocess

PROJECTS = [
    {
        "project": "Enigma.Cli",
        "targetExecutable": "enigma-cli",
    },
]


# Get the release information.
gitCommit = subprocess.check_output(["git", "show", "--oneline", "-s"]).decode().split(" ")[0]
gitOwner = "TheNexusAvenger"
gitProject = "Enigma"
gitRemote = subprocess.check_output(["git", "config", "--get", "remote.origin.url"]).decode()
gitRemoteParts = re.findall(r"([^/:]+)/([^/:]+)\.git", gitRemote)
if len(gitRemoteParts) >= 1:
    gitOwner = gitRemoteParts[0][0]
    gitProject = gitRemoteParts[0][1]

print("Git commit: " + gitCommit)
print("Git owner: " + gitOwner)
print("Git project: " + gitProject)
gitTag = input("Enter the Git tag that will be used: ")
print("Git tag: " + gitTag)

# Write the version information.
with open(os.path.join(os.path.dirname(__file__), "Enigma.Core", "Version.json"), "w") as file:
    file.write(json.dumps({
        "Version": gitTag,
        "Commit": gitCommit,
        "GitHubUser": gitOwner,
        "GitHubProject": gitProject,
    }, indent = 4))

# Set the platforms.
# macOS and Linux currently aren't supported in the code.
# See the README about potential support.
if platform.system() == "Windows":
    print("Building for Windows.")
    platforms = [
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
for platform in platforms:
    platformName = platform["name"]
    platformRuntime = platform["runtime"]
    for project in PROJECTS:
        projectName = project["project"]
        sourceExecutable = projectName
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
        if os.path.exists(targetExecutablePath):
            os.remove(targetExecutablePath)
        if os.path.exists(sourceExecutablePath):
            os.rename(sourceExecutablePath, targetExecutablePath)

        # Create the archive.
        shutil.make_archive("bin/" + project["targetExecutable"] + "-" + platformName, "zip", projectName + "/bin/Release/" + dotNetVersion + "/" + platformRuntime + "/publish")