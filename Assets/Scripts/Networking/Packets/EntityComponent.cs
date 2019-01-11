public abstract class EntityComponent : BindableMonoBehavior {

	[BindComponent]
	protected Entity entity;

	public EntityData data
	{
		get { return entity.data; }
	}
}
