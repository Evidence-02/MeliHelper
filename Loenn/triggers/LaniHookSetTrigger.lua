local trigger = {}
trigger.name = "MeliHelper/LaniHookSetTrigger"
trigger.placements = {
    {
        name = "Lani Hook Set Trigger",
        data = {
			hookDirection = "Horizontal",
			hookLength = 120,
			hookSpeed = 300,
			hookCooldown = 0,
			hookColor = "0000FF",
			hookOpacity = 0.5,
			oneUse = true,
			clearOnLeave = false
        }
    }
}

trigger.fieldOrder = { "x", "y", 
	"hookDirection", "hookLength", "hookSpeed", "hookCooldown", "hookColor", "hookOpacity",
	"oneUse", "clearOnLeave"
}

trigger.fieldInformation = {
    hookDirection = {
		options = { "Horizontal", "All directions" },
		editable = false
    },
	hookColor = { fieldType = "color" }
}

return trigger
