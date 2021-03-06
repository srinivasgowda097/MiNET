using MiNET;
using MiNET.Entities;
using MiNET.Net;

namespace TestPlugin.Pets
{
	public class PetHealthManager : HealthManager
	{
		public PetHealthManager(Entity entity) : base(entity)
		{
		}

		public override void TakeHit(Entity source, int damage = 1, DamageCause cause = DamageCause.Unknown)
		{
			if (!(source is Player)) return;

			// Pets must die in void or they get stuck forever :-(
			if (cause == DamageCause.Void)
			{
				base.TakeHit(source, damage, cause);
				return; // Love denied!
			}

			int size = Entity.Level.Random.Next(0, 3); // The size of the hearts

			Pet pet = Entity as Pet;
			if (pet != null)
			{
				if (pet.AttackTarget != null) return;

				// He is still angry, better not pet him right now.
				if (!pet.IsInRage && IsOnFire && pet.Level.Random.Next(10) == 0)
				{
					pet.AttackTarget = (Player) source;
					pet.RageTick = 20*3;
					return;
				}

				// Only owner do petting with my pet!
				if (pet.Owner == source)
				{
					// Don't trust animals!
					if (pet.Level.Random.Next(500) == 0)
					{
						pet.AttackTarget = (Player) source;
						pet.RageTick = 20*2;
						return;
					}

					var msg = McpeLevelEvent.CreateObject();
					msg.eventId = 0x4000 | 15;
					msg.x = Entity.KnownPosition.X;
					msg.y = (float) (Entity.KnownPosition.Y + Entity.Height + 0.85d);
					msg.z = Entity.KnownPosition.Z;
					msg.data = size;
					pet.Level.RelayBroadcast(msg);
				}
				else
				{
					// HAHA Steal IT!
					if (pet.Level.Random.Next(50) == 0)
					{
						pet.Owner = (Player) source;
						pet.AttackTarget = null;
						pet.RageTick = 0;
						return;
					}

					// Don't trust animals!
					if (pet.Level.Random.Next(30) == 0)
					{
						pet.AttackTarget = (Player) source;
						pet.RageTick = 20*3;
						return;
					}
				}
			}
		}

		public override void OnTick()
		{
			Health = 200;
			base.OnTick();
		}
	}
}