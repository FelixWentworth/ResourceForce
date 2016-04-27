#pragma strict

var portraitToUse : Texture2D;

private var go : GameObject;

var dialogID = 1;
var gameObjectName = "Default Title";
var playerPrefName : String;
public var incidentInfo : IncidentInformation;
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//	START
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

function ShowIncident () {
// ========================================================================================================================
//	STEP ONE NOTES:
//
//	When creating a Dialog Thread using scripting, the first thing you should do is use API_DialogCreate() to create the
//	basic template of the dialog. It will create the gameObject, DialogController, etc. You can also specifically define
//	the autoPlay flag and delay by adding those arguments (boolean, float). Leaving them blank will use (true, 1) by default.
//
// ========================================================================================================================
    // Create a new DialogObject with Autoplay enabled (true), starting after 1 second with a custom name.
    gameObjectName = incidentInfo.playerPrefName;
	if (go == null)
		go = DialogUI.API_DialogCreate(true, 1, gameObjectName);	
	//incidentInfo.SetName(gameObjectName);
	// NOTE:
	// var go : GameObject =  DialogUI.API_DialogCreate() // This will do the exact same thing as the above!


// ========================================================================================================================
//	STEP TWO NOTES:
//
//	At this point we must add each screen to the dialog thread one by one. We can use different functions for each Dialog
//	Style we want to add. For example:
//
//	DialogUI.API_DialogAddNextScreen						(Next Dialog Style)
//	DialogUI.API_DialogAddOneButtonScreen					(One Button Dialog Style)
//	DialogUI.API_DialogAddYesNoScreen						(Yes / No Dialog Style)
//	DialogUI.API_DialogAddTwoButtonScreen					(Two Button Dialog Style)
//	DialogUI.API_DialogAddMultipleButtonScreen				(Two Button Dialog Style)
//	DialogUI.API_DialogAddDataEntryScreen					(Data Entry Dialog Style)
//	DialogUI.API_DialogAddPasswordScreen					(Password Dialog Style)
//	DialogUI.API_DialogAddTitleScreen						(Title Dialog Style)
//	DialogUI.API_DialogAddPopupScreen						(Popup Dialog Style)
//	DialogUI.API_DialogAddIconGridScreen					(Icon Grid Dialog Style)
//
//	That's it! Imagine you're using it in the editor and add each screen in the same way! 
//	Pay special attention to the DialogID's so the thread flows properly!
//
// ========================================================================================================================

	dialogID = PlayerPrefs.GetInt(incidentInfo.playerPrefName + "DialogID");
	if (dialogID == 0)
	{
		dialogID = 1;
		PlayerPrefs.SetInt(incidentInfo.playerPrefName + "DialogID", 1);
	}
	if (dialogID == 1)
	{
		DialogUI.API_DialogAddTwoButtonScreen( 	go,			// <- Make sure you send the gameObject we are adding to here!
												1,											// Dialog ID to use for this screen
												null,		 						// Portrait
												gameObjectName, 							// Title / Actor Name
												incidentInfo.incidentText[dialogID-1] + "\nTurns To Resolve: " + incidentInfo.turnsToResolve + "\nOfficers Required: " + incidentInfo.officersRequired,// Dialog Text
												DIALOG_OVERRIDE_YESNO.UseDefault,			// Settings to use for Typewriter Effect
												DIALOG_OVERRIDE_SCROLLING.UseDefault,		// Settings to use for text scrolling
												"",											// Audio filepath 
												false,										// Hide the Background UI
												false,										// End after this 
												true,									 	// Destroy after this
												false,										// Don't Fade Portrait In On This Screen
												false,										// Don't Fade Portrait Out On This Screen

												// Transitions (NEW in LDC 4.6)
												DIALOG_OVERRIDE_TRANSITION.None,		// Transition to use when showing dialog screen
												DIALOG_OVERRIDE_TRANSITION.None,		// Transition to use when ending dialog screen

												"Resolve",										// Custom Button 1	
												"Ignore", 										// Custom Button 2
												2,											// Dialog ID to move to upon clicking Custom Button 1.
												2,											// Dialog ID to move to upon clicking Custom Button 2.
												null,										// Function Array to Callback At Start ( Type Function[] )
												null,										// Function Array to Callback At End ( Type Function[] )
												null,										// System.Action to Callback At Start ( Type System.Action )
												null,										// System.Action to Callback At End ( Type System.Action )
												[this.gameObject.name, "ButtonSelected", "Decision Made"]										// Navigation Callback - String[GameObjectName, FunctionName, userString]
											);
	}
	else
	{
		//only show a one button dialog, this will likely change later
		DialogUI.API_DialogAddOneButtonScreen( 	go,			// <- Make sure you send the gameObject we are adding to here!
											2,											// Dialog ID to use for this screen
											null,		 						// Portrait
											gameObjectName, 								// Title / Actor Name
											incidentInfo.incidentText[dialogID-1]+ "\nTurns To Resolve: " + incidentInfo.turnsToResolve, 		// Dialog Text
											DIALOG_OVERRIDE_YESNO.UseDefault,			// Settings to use for Typewriter Effect
											DIALOG_OVERRIDE_SCROLLING.UseDefault,		// Settings to use for text scrolling
											"",											// Audio filepath 
											100,											// Seconds to show
											false,										// Hide Next Button
											false,										// Hide the Background UI
											true,										// End after this 
											true,									 	// Destroy after this
											false,										// Don't Fade Portrait In On This Screen
											false,										// Don't Fade Portrait Out On This Screen

											// Transitions (NEW in LDC 4.6)
											DIALOG_OVERRIDE_TRANSITION.None,		// Transition to use when showing dialog screen
											DIALOG_OVERRIDE_TRANSITION.None,		// Transition to use when ending dialog screen

											"OK!",									// Custom Button Label
											3,											// Dialog ID to move to upon clicking Next.
											null,										// Function Array to Callback At Start ( Type Function[] )
											null,										// Function Array to Callback At End ( Type Function[] )
											null,										// System.Action to Callback At Start ( Type System.Action )
											null,										// System.Action to Callback At End ( Type System.Action )
											[this.gameObject.name, "DeactivateGameObject", "Accepted"]										// Navigation Callback - String[GameObjectName, FunctionName, userString]
										);
	}
}	

function ButtonSelected( args : String[] )
{
	// Make sure the arguments are valid
	if( args!=null && args.length == 5 ){
		// split up the array into seperate variables
		var ldcObjectName : String = args[0];		// The name of the LDC Dialog
		var dialogID : int = int.Parse(args[1]);	// The Dialog ID of the current screen
		var buttonID : int = int.Parse(args[2]);	// The button ID of the selected button
		var buttonName : String = args[3];			// The name of the selected button
		var customString : String = args[4];		// The custom string

		//left button ID = 1 - Ignore
		//Right Button ID = 0 - Resolve
		//if the user has chosen to resolve, remove officer from available
		if (buttonID == 0)
		    incidentInfo.AddOfficer(incidentInfo.officersRequired);
		dialogID += buttonID == 1 ? 0 : 1;
		PlayerPrefs.SetInt(incidentInfo.playerPrefName + "DialogID", dialogID);
		ShowIncident ();

	}
	//decision made, change the Dialog ID to change the text next time the dialog is activated

	//this.gameObject.SetActive(false);
}