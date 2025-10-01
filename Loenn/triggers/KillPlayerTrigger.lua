local trigger = {}
trigger.name = "MeliHelper/KillPlayerTrigger"
trigger.placements = {
	{
		name = "Kill Player Trigger (OnEnter)",
		data = {
			action = "OnEnter",
			stayTime = 0
		}
	},
	{
		name = "Kill Player Trigger (OnLeave)",
		data = {
			action = "OnLeave",
			stayTime = 0
		}
	},
	{
		name = "Kill Player Trigger (OnStay)",
		data = {
			action = "OnStay",
			stayTime = 1.5
		}
	}
}

trigger.fieldOrder = { "x", "y", "width", "height", "trigger", "action", "param",  
	"PlayerInside", "PlayerOnLeft", "PlayerOnRight", "BerryCollected", 
	"oneUse"
	}

trigger.fieldInformation = {
    action = {
        options = { "OnEnter", "OnLeave", "OnStay" },
		editable = false
    }
}

return trigger
