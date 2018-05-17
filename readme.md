# Branding Configuration
A Folder should be created in Assets/Branding for all the imagery and config required for the new version

## 1. Add New Sprites

#### [Area] Map
- A map of the local area. Use https://mapstyle.withgoogle.com/ for styling and use Assets/Branding/maps.json for current style
- 1024x1024

#### [Area] Police
- A badge/icon for the police force. 
- 1024x1024

#### [Area] PoliceName
- A badge/icon for the police force with name included
- 1024x1024

#### [Area] PoliceNameNPOT
- A badge/icon for the police force used during the splash screen
- No Locked Resolution, currently using 839x1024

#### [Area] PoliceWebsite
- A sprite that will be used as a button to direct to the website
- 256x256

## 2. Setup Config File
2.1. Create a config file for the new [Area], one can be copied from the other files in the project, rename the config to "[Area] BrandingConfig" and start to fill in the config as desired.  
2.2. Change the metadata, making sure that app name, bundle id and location are unique.   
2.3. References to logos and images must be set with the newly added images. External Links should be set to link to the social media/websites of the new force.   
2.4. Languages must be set, but be aware that currently (16.05.2018) the game will only handle English, Spanish, Greek and Dutch, additional languages will require further setup. 

## 3. Apply Config
In ResourceForce.unity navigate to the BrandingManager Object in the hierarchy and set the Branding Config object to be the new config you have just created, and press either of the apply buttons.

## 4. Update Bundle ID
Bundle IDs are not set through the config and must be incremented manually. 
This is dont through the build settings

## 5. Setup GameAnalytics
For analytics to be sent to the correct platform/game version. the game analytics settings must be updated, see Resources/GameAnalytics/Settings for setup config.
Use login details provided (dev@playgen) and continue to create 2 versions of the new game "[Area] (Android) and [Area] (iOS)". Once this is done they can be loaded in Unity.
In Settings make sure to remove any existing platforms from previous versions, and add both Android and iOS as platforms selecting "Resource Force" as the studio and select the new games created.

**IMPORTANT**

The analytics by default will send generic data, but it is possible to send data regarding clicks through to external links, this can be set in the config file via the External Links section,
simply check all links where you want to track clicks, press apply in the branding manager and save.

## 6. Update Authoring Tool
In the config file in the authoring tool, app.config.js, make sure that the Location specified in the config file is added to the regions available in the authoring tool.
Update the auth tool on files.playgen.com and make sure URL is correct

## 7. Deploying to Asset Atore
The deployment to asset store process is the normal process, for the keystore, if using rfkeystore as for the original version of Resource Force, the password for this can be found in last pass

## Additional Notes
The game will use the content created through the authoring tool, but if there are not enough scenarios to run the game efficiently (15 minimum) it will use a combination of the scenarios in 
Resources/BasicContent.txt and the ones downloaded from the authoring tool.
Once there are enough valid scenarios in the authoring tool, the game will no longer use the default scenarios.