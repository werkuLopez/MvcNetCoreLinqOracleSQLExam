using MvcNetCoreLinqOracleSQLExam.Models;
namespace MvcNetCoreLinqOracleSQLExam.Repositories
{
    public interface IComicRepository
    {
        List<Comic> GetComics();
        Comic GetComicById(int id);
        void InsertarComicProcedure(string nombre, string imagen, string descripcion);
        void InsertarComicTexto(string nombre, string imagen, string descripcion);
        void EliminarComic(int id);
    }
}
