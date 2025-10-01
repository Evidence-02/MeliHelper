local trigger = {}
trigger.name = "MeliHelper/MeliHelperConditionTrigger"
trigger.placements = {
	{
		name = "Meli Helper Condition Trigger",
		data = {
			trigger = "Every 1 sec",
			action = "Teleport",
			param = "",
			
			PlayerInside = false,
			PlayerOnLeft = false,
			PlayerOnRight = false,
			BerryCollected = false,
			
			oneUse = true
		}
	}
}

trigger.fieldOrder = { "x", "y", "width", "height", "trigger", "action", "param",  
	"PlayerInside", "PlayerOnLeft", "PlayerOnRight", "BerryCollected", 
	"oneUse"
	}

trigger.fieldInformation = {
    trigger = {
        options = { "Every 0.1 sec", "Every 1 sec", "Every 5 sec", "Every 15 sec", "Every 1 minute", "OnPlayerEnter", "OnPlayerLeave" },
		editable = false
    },
    action = {
        options = { "Teleport", "Dialogue" },
		editable = false
    }
}

return trigger
