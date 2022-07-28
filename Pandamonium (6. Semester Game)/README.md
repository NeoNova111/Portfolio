# Pandamonium  
(Please refer to the links below for further information on my personal contributions and the project in general)
----------
“Pandamonium” is a humorous 2.5D rogue-like first-person shooter game for PC. In it you play as a puny animal control officer whose task it is to subdue comically muscular zoo animals using non lethal weaponry like modified tranquilizer dart guns. Play sessions are divided into runs, with each successful run guiding the player through three unique areas, and fighting a boss at the end of each. However, if the player's life points ever fall below zero the player will black out and the run will have failed, resulting in the rampant animals wreaking havoc on the world. The absurd physical humor of this game, and its weapon and powerup variety is to encourage players to come back even after having completed their first successful run.

![PandaMonium](https://user-images.githubusercontent.com/59093470/180043078-99188f71-97e5-4fd7-80a5-b22a9184bb83.png)

Most Relevant C# Scripts Produced:  
Weapon System: WeaponManager.cs, BaseWeapon.cs, RangedWeapon.cs, MeleeWeapon.cs, BaseBullet.cs, Rocket.cs, ParticleClusterExplosion.cs  
Player: PlayerController.cs, CameraLook.cs, PlayerStats.cs, ViewBob.cs  
Procedural Level Generation: RandomLevelGenerator.cs, TerrainGenerator.cs  
Boss Enemy: PandaBossStateMachine.cs, PandaBossAttackState.cs, PandaBossMeteorState.cs, PandaBossTransitionState.cs, PandaBossWalkState.cs  
Shop System: ShopInventory.cs, ShopItem.cs, ShopItemHolder.cs, ShopKeeper.cs, ShopItemPool.cs  
Items: Item.cs, WeaponPickup.cs, GroundPowerUp,cs, AmmoPickup.cs, ItemPool.cs, MoneyPickup.cs  
Interfaces: IDamagable.cs, IInteractable.cs, IPetable.cs, IPickupable.cs  
Utility & Extension methods: OwnDelaunatorExtentions.cs, ExtensionMethods.cs  
Miscellaneous: FlyCam.cs, UIManager.cs, SmolEnemy.cs, PostProcessingControl, DeactivateRigidbodyOnCollision.cs, FadetoBlackUI.cs  

---------
Links:  
<a href="https://www.youtube.com/watch?v=8UDz6PhGyvI">Trailer</a> | Itch.io | <a href="https://www.youtube.com/watch?v=i9RFHk3O4r4">Showreel</a>  
