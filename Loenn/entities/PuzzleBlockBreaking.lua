local entity = {}
entity.name = "MeliHelper/PuzzleBlockBreaking"
entity.placements = {
    {
        name = "Puzzle Block Breaking",
        data = {
			textureGUI = "Evidence02/puzzle_sugoma",
			blocksNeedToBreak = 40,
			action = "UnlockStrawberry",
			flagOnSolve = "PuzzleBreakingSolved",
			textureX = 960,
			textureY = 60,
			textDX = 0,
			textDY = 0,
			textColor = "FFFFFF",
			textScale = 0.9
        }
    }
}
entity.texture = "Evidence02/objects_melihelper/loenn/puzzle"
entity.fieldOrder = { "x", "y", "textureGUI", "blocksNeedToBreak", "action", "flagOnSolve", "textureX", "textureY", "textDX", "textDY", "textScale" }

entity.fieldInformation = {
	textColor  = { fieldType = "color" },
	blocksNeedToBreak = { field_type = "integer" }
}

return entity
