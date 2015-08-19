[![Build status](https://ci.appveyor.com/api/projects/status/ciac125gj43w4yb9?svg=true)](https://ci.appveyor.com/project/Ruhrpottpatriot/skyrimcompilehelper)
[![Build status](https://ci.appveyor.com/api/projects/status/ciac125gj43w4yb9/branch/master?svg=true)](https://ci.appveyor.com/project/Ruhrpottpatriot/skyrimcompilehelper/branch/master)

# Skyrim Compile Helper
The _Skyrim Compiler Helper_ is s simple tool designed to help modders with their script compilation process. It features convention over configuration to make the whole process as smooth as possible.

At the centre of a compilation process stands a simple _Git_ repository which contains the source files, a Readme and a License as well as the compiled files. However the repository is not only intended for scripts it can be used for other modding files as well (such as textures, and models). The application will recognize that those files are present and then act accordingly.

## Installation
To install the programm, simply download the latest binary files from GitHub and extract them to a location of your choosing. The second step is equally as important as the first. Without the compiler wrapper the programm will not function and crash when starting the compilation process. To install the compiler head over to the [Compiler Wrapper GitHub Page](https://github.com/Ruhrpottpatriot/PapyrusCompilerWrapper) and download the latest binary files to a location of your choosing. Disregard the install instructions since you are a user, not a programmer. After downloading, extract the DLL file (leave the other two alone) into the _Papyrus Compiler_ folder inside your main Skyrim folder. If this folder does not exist, you are missing the CreationKit and need to install it now.
With this the installation is complete and you can start using the application.

## First Use
The interface is intended to be as simple as possible. To use the programm, just start it. If this is the first start, you will need to insert the correct Skyrim and ModOrganizer paths. To do that, open the settings tab and either insert the path by hand or select it via the "Change" button.
The porgramm will show you, if the necessary programms are installed. If any of the check marks are missing, you are missing one reference and need to install it, before you can use the application.
If these prerequisites are met, you can start adding solutions and configurations.

## Terminology
Before we can start using the application we need to make sure you are up to speed with the terminology and conventions of the application.
* **Solution**: A solution is the item that stores the meta information about the mod inside the program. These information are:
  * _Name_: The name of your Mod (preferrably, but can be anything you like). The name will be used as the name of the ModOrganizer folder.
  * _Version_: The version of your mod. The versioning scheme strictly follows [Semantic Versioning](http://semver.org/)
  * _Git Repository Path_: The path to the Git Repository the files are stored in.
  * _Compile Configurations_: See below
* **Compile Configuration**: A compile configuration is a simple set of switches, that determine how the Papyrus Compiler should handle your files and which files should be copied after the compile process.

## The Git Repository
At the centre of the application stands the _Git_ repository. These repositories offer version control and other neat features. As everything with this application the repository follows convention over configuration, too. There is the [Skyrim Mod Repository Blueprint](https://github.com/Ruhrpottpatriot/SkyrimModRepositoryBlueprint), that essentially is an empty repository for you to use. If you don't know how to use Git, go over there, and read the readme, where everything essential is explained to you.

## Usage
At first the application does not store any solutions, to change that. Click on the solution box and click on "<edit...>". A new window opens where you can add and remove solutions from the storage. Click on "Add" and fill out the fields. If you already have a version of your mod, greater than 0.1.0, double click on the version text and change the version.
Now close the windows and select the newly created configuration from the solution box. Some information should be filled out already. As always, changing the version is done by double clicking the version text. Keep in mind that a build persists over different versions and can't, not should be changed.
Adding a configuration is done the same way as adding a solution.

A click on _Compile_ will start the compiler and compile all papyrus script files inside the src folder and then copy the necessary files to their destinations. Each compilation Process will increase the _Build_ number by one.

### Cleaning
If you have problems with your compiled files, you can "clean" the solution. This will delete all compiled files. **ATTENTION: THIS WILL EMPTY THE ModOrganizer FOLDER AS WELL, IF YOU HAVE FILES THERE, THAT ARE NOT INSIDE THE src FOLDER OF THIS MOD, THEY WILL BE LOST. YOU HAVE BEEN WARNED!**
