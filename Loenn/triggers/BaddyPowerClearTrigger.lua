local trigger = {}
trigger.name = "MeliHelper/BaddyPowerClearTrigger"
trigger.placements = {
    {
        name = "Badeline Power Clear Trigger",
        data = {
			oneUse = true
        }
    }
}

trigger.fieldOrder = { "x", "y", "width", "height", "oneUse" }

return trigger
