local entity = {}
entity.name = "MeliHelper/BattleCityCustomBonusChances"
entity.depth = -9999999
entity.placements = {
    {
        name = "Battle City Custom Bonus Chances (full)",
        data = {
			Grenade = 1,
			Shield = 1,
			Shovel = 1,
			Star = 1,
			ExtraLife = 1,
			TimeStop = 1,
			EMI = 1,
			Duality = 1,
			HomingBullets = 1,
			UnlimitedShooting = 1,
			MoveThroughWater = 1,
			DirtBall = 1,
			DemolitionBomb = 1,
			Mine = 1
		}
    },
    {
        name = "Battle City Custom Bonus Chances (vanilla)",
        data = {
			Grenade = 0.18,
			Shield = 0.18,
			Shovel = 0.18,
			Star = 0.23,
			ExtraLife = 0.09,
			TimeStop = 0.18,
			EMI = 0,
			Duality = 0,
			HomingBullets = 0,
			UnlimitedShooting = 0,
			MoveThroughWater = 0,
			DirtBall = 0,
			DemolitionBomb = 0,
			Mine = 0
		}
    },
    {
        name = "Battle City Custom Bonus Chances (items)",
        data = {
			Grenade = 0,
			Shield = 1,
			Shovel = 0,
			Star = 0,
			ExtraLife = 0,
			TimeStop = 0,
			EMI = 1,
			Duality = 0,
			HomingBullets = 0,
			UnlimitedShooting = 0,
			MoveThroughWater = 0,
			DirtBall = 1,
			DemolitionBomb = 1,
			Mine = 1
		}
    },
}

entity.fieldOrder = { "x", "y", "Grenade", "Shield", "Shovel", "Star", "ExtraLife", "TimeStop", "EMI", "Duality", "HomingBullets", "UnlimitedShooting", "MoveThroughWater", "DirtBall", "DemolitionBomb", "Mine" }

function entity.texture(room, entity)
    return "Evidence02/objects_bc/loenn/custom_bonuses"
end

return entity
