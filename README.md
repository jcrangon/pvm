# pvm #### for Windows
A PHP Version Manager
current version: 1.0.0

## Installation
Download the pvm-setup.msi from the release page.

Double-click on it to start the installer.

Then double-click on the icon created on your desktop to finish auto-configuration.


## Usage
list of commands:

`pvm list available`  
will list all available php version. you can choose which major version you want.

`pvm install [version name]`  
downloads then unzips the PHP archive onto your system.

`pvm ls`  
lists all installed versions

`pvm use [version name]`  
activates one of the installed version.

`pvm display ini`  
display the file php.ini. You can choose the editor to use amongst 
  - notepad
  - notepad++
  - VSCode

`pvm remove [version name]`  
removes an intalled version from your system.

`pvm -help | pvm -h`  
display pvm command help list.