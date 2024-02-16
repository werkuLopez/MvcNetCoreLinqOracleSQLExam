using Microsoft.AspNetCore.Http.HttpResults;
using MvcNetCoreLinqOracleSQLExam.Models;
using System.Data;
using System.Data.SqlClient;

#region PROCEDIMIENTOS
// ---- INSERTAR ----
//CREATE PROCEDURE SP_INSERTAR_COMIC
//(@NOMBRE NVARCHAR(50), @IMAGEN NVARCHAR(100), @DESCRIPCION NVARCHAR(50))
//AS
//	DECLARE @nextId int 
//	SELECT @nextId = MAX(IDCOMIC) + 1 FROM COMICS
//	INSERT INTO COMICS VALUES (@nextId, @NOMBRE, @IMAGEN, @DESCRIPCION)
//GO

//---- ELIMINAR ---
//CREATE PROCEDURE SP_ELIMINAR_COMIC
//(@idcomic INT)
//AS
//	DELETE FROM COMICS WHERE IDCOMIC=@idcomic
//GO
#endregion
namespace MvcNetCoreLinqOracleSQLExam.Repositories
{
    public class RepositoryComicSql: IComicRepository
    {
        DataTable tablaComics;
        SqlConnection cn;
        SqlCommand cmd;
        SqlDataReader rdr;

        public RepositoryComicSql()
        {
            string connection = @"Data Source=LOCALHOST\SQLEXPRESS;Initial Catalog=HOSPITAL;Persist Security Info=True;User ID=sa;Password=MCSD2023;";
            this.cn = new SqlConnection(connection);
            this.cmd = new SqlCommand();
            this.cmd.Connection = cn;

            string sql = "SELECT * FROM COMICS";
            this.tablaComics = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(sql, this.cn);
            adapter.Fill(this.tablaComics);
        }

        public async void EliminarComic(int id)
        {
            this.cmd.Parameters.AddWithValue("@idcomic", id);
            this.cmd.CommandType = CommandType.StoredProcedure;
            this.cmd.CommandText = "SP_ELIMINAR_COMIC";
            await this.cn.OpenAsync();
            int result = await this.cmd.ExecuteNonQueryAsync();
            await this.cn.CloseAsync();
            this.cmd.Parameters.Clear();
        }

        public Comic GetComicById(int id)
        {
            var consulta = from datos in this.tablaComics.AsEnumerable()
                           where datos.Field<int>("IDCOMIC") == id
                           select datos;
            var row = consulta.First();

            Comic comic = new Comic
            {
                IdComic = row.Field<int>("IDCOMIC"),
                Nombre = row.Field<string>("NOMBRE"),
                Imagen = row.Field<string>("IMAGEN"),
                Descripcion = row.Field<string>("DESCRIPCION")
            };

            return comic;
        }

        public List<Comic> GetComics()
        {
            var consulta = from datos in this.tablaComics.AsEnumerable()
                           select datos;

            List<Comic> lista = new List<Comic>();

            if (consulta.Count() == 0)
            {
                return null;
            }
            else
            {

                foreach(var row in consulta)
                {
                    Comic cm = new Comic
                    {
                        IdComic = row.Field<int>("IDCOMIC"),
                        Nombre = row.Field<string>("NOMBRE"),
                        Imagen = row.Field<string>("IMAGEN"),
                        Descripcion = row.Field<string>("DESCRIPCION")
                    };
                    lista.Add(cm);
                }
                return lista;
            }
        }

        public  void InsertarComicProcedure(string nombre, string imagen, string descripcion)
        {
            this.cmd.Parameters.AddWithValue("@NOMBRE", nombre);
            this.cmd.Parameters.AddWithValue("@IMAGEN", imagen);
            this.cmd.Parameters.AddWithValue("@DESCRIPCION", descripcion);
            this.cmd.CommandType = CommandType.StoredProcedure;
            this.cmd.CommandText = "SP_INSERTAR_COMIC";
            this.cn.Open();
            int result = this.cmd.ExecuteNonQuery();
            this.cn.Close();
            this.cmd.Parameters.Clear();
        }

        public void InsertarComicTexto(string nombre, string imagen, string descripcion)
        {
            var consulta = from datos in this.tablaComics.AsEnumerable()
                           select datos;
            int nextId = (consulta.Max(x => x.Field<int>("IDCOMIC"))) + 1;

            string sql = "INSERT INTO COMICS VALUES(@id, @nombre, @imagen, @descripcion)";
            this.cmd.Parameters.AddWithValue("@id", nextId);
            this.cmd.Parameters.AddWithValue("@nombre", nombre);
            this.cmd.Parameters.AddWithValue("@imagen", imagen);
            this.cmd.Parameters.AddWithValue("@descripcion", descripcion);
            this.cmd.CommandType = CommandType.Text;
            this.cmd.CommandText = sql;
            this.cn.Open();
            int result = this.cmd.ExecuteNonQuery();
            this.cn.CloseAsync();
            this.cmd.Parameters.Clear();
        }
    }
}
