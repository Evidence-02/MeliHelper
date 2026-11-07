local trigger = {}
trigger.name = "MeliHelper/RubikCancelDashTrigger"
trigger.placements = {
	{
		name = "Rubik Cancel Dash Trigger",
		data = {
			dirOnly = "All"
		}
	}
}

trigger.fieldInformation = {
	dirOnly = {
		options = {	"All", "Left", "Right", "Up", "Down" },
		editable = false
	}
}

return trigger
