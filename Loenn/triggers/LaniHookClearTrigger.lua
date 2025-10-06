local trigger = {}
trigger.name = "MeliHelper/LaniHookClearTrigger"
trigger.placements = {
    {
        name = "Lani Hook Clear Trigger",
        data = {
			oneUse = true
        }
    }
}

trigger.fieldOrder = { "x", "y", "oneUse" }

return trigger
