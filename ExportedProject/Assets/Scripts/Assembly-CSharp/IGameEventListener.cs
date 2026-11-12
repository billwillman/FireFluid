public interface IGameEventListener<T>
{
	int GetOrder();

	IGameEventResult OnGameEvent(T eventParameters);
}
