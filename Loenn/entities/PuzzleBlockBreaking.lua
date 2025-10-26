local entity = {}
entity.name = "MeliHelper/PuzzleBlockBreaking"
entity.placements = {
    {
        name = "Puzzle Block Breaking",
        data = {
			textureGUI = "",
			blocksNeedToBreak = 10,
			action = "UnlockStrawberry",
			flagSetOnSolve = "PuzzleBreakingSolved",
			textureX = 280,
			textureY = 120,
			textDX = 0,
			textDY = 0,
			textColor = "FFFFFF",
			textScale = 0.9
        }
    }
}
entity.texture = "Evidence02/objects_melihelper/loenn/puzzle"
entity.fieldOrder = { "x", "y", "textureGUI", "blocksNeedToBreak", "action", "flagSetOnSolve", "textureX", "textureY", "textDX", "textDY", "textScale" }

entity.fieldInformation = {
	action  = { 
		options = { "UnlockStrawberry" }, 
		editable = false 
	},
	textColor  = { fieldType = "color" },
	blocksNeedToBreak = { field_type = "integer" }
}

return entity
