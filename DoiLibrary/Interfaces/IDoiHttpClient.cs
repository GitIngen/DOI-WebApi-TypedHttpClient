using DoiLibrary.Domain;
using System.Threading.Tasks;

namespace DoiLibrary.Interfaces
{
	public interface IDoiHttpClient
	{
		Task<string> CreateDoi(Attributes doiAttributes);
	}
}
