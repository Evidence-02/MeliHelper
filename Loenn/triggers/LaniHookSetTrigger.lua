local trigger = {}
trigger.name = "MeliHelper/LaniHookSetTrigger"
trigger.placements = {
    {
        name = "Lani Hook Set Trigger",
        data = {
			hookDirection = "Horizontal",
			hookLength = 120,
			hookSpeed = 450,
			hookSpeedMovePlayer = 300,
			hookCooldown = 0,
			hookColor = "0000FF",
			hookOpacity = 0.5,
			hookAllowHypers = true,
			oneUse = true,
			clearOnLeave = false
        }
    }
}

trigger.fieldOrder = { "x", "y", "width", "height",
	"hookDirection", "hookLength", "hookSpeed", "hookSpeedMovePlayer", "hookCooldown", "hookColor", "hookOpacity", "hookOpacity", "hookAllowHypers",
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
