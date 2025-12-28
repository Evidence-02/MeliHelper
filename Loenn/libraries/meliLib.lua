
local meliLib = {}

meliLib.bcCellOptions = { 
	"Brick", "Steel", "Water", "Dirt", "Grass"
}

meliLib.bcBonusOptions = {
	["Grenade"] = "Grenade",
	["Shield"] = "Shield",
	["Shovel"] = "Shovel",
	["Star"] = "Star",
	["Extra Life"] = "ExtraLife",
	["Time Stop"] = "TimeStop",
	["EMI"] = "EMI",
	["Duality"] = "Duality",
	["Homing Bullets"] = "HomingBullets",
	["Unlimited Shooting"] = "UnlimitedShooting",
	["Move Through Water"] = "MoveThroughWater",
	["Dirt Ball (item)"] = "DirtBall",
	["Demolition Bomb (item)"] = "DemolitionBomb",
	["Mine (item)"] = "Mine"
}

meliLib.bcEnemyOptions = { 
	"Basic", "Fast", "Power", "Armored" 
}

--    !!! CAUTION !!!
-- synchronized with: EnemyTypesController.cs
meliLib.bcEnemyParams = { 
	Basic = {
		tankID = 'B',
		tankType = 'Basic',
		points = 100,
		health = 1,
		speedMove = 36,
		speedBullets = 100,
		shootFrequency = 2.4,
		canDamageSteel = false
	}, 
	Fast = {
		tankID = 'F',
		points = 200,
		health = 1,
		speedMove = 60,
		speedBullets = 150,
		shootFrequency = 2.4,
		canDamageSteel = false
	}, 
	Power = {
		tankID = 'P',
		points = 300,
		health = 1,
		speedMove = 48,
		speedBullets = 200,
		shootFrequency = 1.8,
		canDamageSteel = true
	}, 
	Armored = {
		tankID = 'A',
		tankType = 'Armored',
		points = 400,
		health = 4,
		speedMove = 48,
		speedBullets = 150,
		shootFrequency = 2.4,
		canDamageSteel = false
	}
}



return meliLib