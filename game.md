# Adventure Tycoon

Adventure Tycoon is a DND-inspired, god-sim, map tycoon. Manage incoming adventurers, helping them level up by placing helpful quests & resting areas throughout the land. 

A roll of the die controls most game events and player actions. 
Roll to generate new quest spots to place. 
Adventurers will succeed/fail quests based on stats & rolls. 

## Game Loop

Time will pass & game will be pause-able to better manage structures & placements. 

Example game flow: 
- Initial placement
  - Pick 2 to place: village, town, or city
  - Generate a set of quest spots (5?) & place 2
- Adventurers spawn
  - Will locate closest available quest & attempt it
    - If they succeed, they'll gain items & lose some health
    - If they fail, they die :(
  - Will locate closest settlements to sell items & rest
    - Based on activities the 
- Based on available quests, adventurer death rate, and settlement activities, adventurers will have a certain happiness level
- Adventurers will get items from quests, which will then be "sold" to marketplaces, earning the player $$$

## Money

Players will need money to build locations on the map & add activities to locations.

Adventurers will sell items found on quests at **TRADE** activities.

V1: Items will be auto-cashed in to generate $$$ for the player.

~?insert more complex loop?~

### NEEDS MORE THOUGHT

Various ideas that increase marketplace complexity:

Could have adventurers **trade** items at marketplaces for single-use items?

Could have adventurers **sell** items to be able to have their own money, and buy items w/ that money? 

Could have items be auto-cashed in at marketplaces (current basic system)
OR
Could have items be added to player's global stash of items, which they can sell to ???? 
- Visiting regional traders?
- Some greater entity/force? A money-hungry god perhaps, that you are narratively "playing" this game FOR....
- A global marketplace?

Could have money generating activities in the `PassTime` category that have adventurers spend their $$ which is in turn given to the player?

## Adventurers

Adventurers will spawn on the map at semi-random intervals dependent on overall map happiness. 
They are the money-makers for the player: they will collect items on quests, return to towns and sell those items, which in turn gives the player $$. See [money section](#money).

`Adventurer` class contains data for each adventurer on the map.

### Adventuring

If they have full health, they will navigate to the closest available quest spot. 

If they cannot find a suitable quest after a given # of days, they will... (TO DO: Leave? Attempt higher-level quest & potentially die?)

### Resting & Trade

If they are missing health or have items to sell, they will navigate to the closest settlement. 

- If they are missing health they can rest at rest activities (inns, spa, etc.). 
- If they have items from quests, they can sell them at trade activities (market, apothecary, tavern, blacksmith, etc.)

### Happiness

Happiness is decreased over time, and increased by various activities: 
- resting
- lollygagging (taverns, festivals, etc.)
- gaining money or XP

## Map Locations

At the start of the game, a set # of map locations will be placed randomly for the player. This should include at least one 
 
## Settlements

Settlements come in different sizes and have slots (based on size) for activities.
Players can select new activities & remove them at a cost to make room for new ones.

## Quest Spots

Quest spots will be generated in groups. The players can select from the generated spots & place them wherever they like (with limits). 

### Difficulty

Quest spots have difficulty level ranges (ex: 5-12) that will determine which adventurers can attempt them (ex: minimum level 5) and which will be most likely to success (ex: level higher than 12 is 99% chance success, level 5 is 25% chance success). 

> Player strategy includes noting the range of adventurer levels on the map, the existing quest spots on the map, and the level ranges of generated quest spots. Players should pick quest spots so that lower-level adventurers can succeed/level up and so higher-level adventurers can continue their journeys & make money.

### Adventurer Attempts

Adventurers that attempt a quest spot will have a chance of success/survival based on their level & the quest spot's difficulty. 

#### Failure/Death

Adventurers that fail have died during their attempt. They will leave behind items to be collected by other successful adventurers.

#### Success/Survival

Adventurers that succeed have survived their attempt - note that this does not mean they have defeated all enemies or "completed" the quest.

Quest spots will have an amount of "health" that is decreased away by successful adventurer attempts. After enough successful attempts, the quest spot will be "completed" and will be removed from the map. Adventurers who "complete" the quest spot will get extra XP, items, etc. 

> The amount of "damage" that a quest spot will take from a successful attempt depends on the adventurer's level. 

Adventurers will lose at least 1 health point no matter their level. More health points will be taken based on lower level. 

Adventurers will gain some items based on the "damage" amount they generated.

# Project Timeline

[Trello](https://trello.com/b/8qGlRU7f/adventure-tycoon)

