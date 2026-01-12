using Core.IoC;

namespace Core
{
	public interface IServiceRegistration
	{
		void RegisterServices(IIoC container);
	}
}
