using System.Threading.Tasks;

namespace SonosSharp
{
    public class TestTask
    {
        /// <summary>
        /// Creates a task which never returns the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<T> NeverReturns<T>()
        {
            await Task.Delay(-1);
            return default(T);
        }
    }
}
