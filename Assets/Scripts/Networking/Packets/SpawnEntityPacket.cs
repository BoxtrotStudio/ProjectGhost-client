public class SpawnEntityPacket : Packet {
	protected override void Handle()
	{
		var type = Consume(2).AsShort();
		var id = Consume(2).AsShort();

		var nameLength = Consume(4).AsInt();
		var name = Consume(nameLength).AsString();

		var x = Consume(4).AsFloat();
		var y = Consume(4).AsFloat();

		var angle = (float)Consume(8).AsDouble();

		var width = Consume(2).AsShort();
		var height = Consume(2).AsShort();

		var hasLighting = Consume(1).AsBool();

		EntityManager.SpawnEntity(type, id, x, y, width, height, angle, name, hasLighting);
	}
}
