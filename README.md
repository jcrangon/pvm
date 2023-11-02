# __pvm__
A PHP Version Manager  
_current version: 1.0.1  
for Windows 10 & 11_
_(see change log below)_

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


## Uninstall
use pvm to remove all installed php versions one by one.
use windows control panel to completely uninstall pvm from you computer.


## Change Log

- `V1.0.1: `
Minor correction in the command line to display help.