using MvcNetCoreLinqOracleSQLExam.Models;
using Oracle.ManagedDataAccess.Client;
using System.Data;

#region PROCEDIMIENTOS
//---- INSERTAR ----
//create or replace procedure sp_insertar_comic
//(p_nombre COMICS.NOMBRE%TYPE, p_imagen COMICS.IMAGEN%TYPE, p_descripcion COMICS.DESCRIPCION%TYPE)
//as
//p_idcomic COMICS.IDCOMIC%TYPE;
//begin
//  select max(IDCOMIC) + 1 into p_idcomic
//  FROM COMICS;
//INSERT INTO COMICS VALUES
//  (p_idcomic, p_nombre, p_imagen, p_descripcion);
//COMMIT;
//end;
#endregion

namespace MvcNetCoreLinqOracleSQLExam.Repositories
{
    public class RepositoryComicsOracle : IComicRepository
    {
        DataTable tablaComics;
        OracleConnection cn; 
        OracleCommand cmd;
        public RepositoryComicsOracle()
        {
            string conn = @"Data Source=LOCALHOST:1521/XE; Persist Security Info=True; User Id=SYSTEM; Password=oracle ";
            this.cn = new OracleConnection(conn);
            this.cmd = new OracleCommand();
            this.cmd.Connection = cn;
            this.tablaComics = new DataTable();
            string sql = "SELECT * FROM COMICS";
            OracleDataAdapter adapter = new OracleDataAdapter(sql, this.cn);
            adapter.Fill(this.tablaComics);
        }
        public async void EliminarComic(int id)
        {
            string sql = "DELETE FROM COMICS WHERE IDCOMIC=:idcomic";
            OracleParameter paramId = new OracleParameter(":idcomic", id);
            this.cmd.Parameters.Add(paramId);
            this.cmd.CommandType = CommandType.Text;
            this.cmd.CommandText = sql;
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

                foreach (var row in consulta)
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

        public void InsertarComicProcedure(string nombre, string imagen, string descripcion)
        {
            this.cmd.CommandType = CommandType.StoredProcedure;
            this.cmd.CommandText = "sp_insertar_comic";
            OracleParameter paramNom = new OracleParameter(":p_nombre", nombre);
            this.cmd.Parameters.Add(paramNom);
            OracleParameter paramImg = new OracleParameter(":p_imagen", imagen);
            this.cmd.Parameters.Add(paramImg);
            OracleParameter paramDesc = new OracleParameter(":p_descripcion", descripcion);
            this.cmd.Parameters.Add(paramDesc);
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

            string sql = "INSERT INTO COMICS VALUES(:idcomic, :nombre, :imagen, :descripcion)";
            OracleParameter paramId = new OracleParameter(":idcomic", nextId);
            this.cmd.Parameters.Add(paramId);
            OracleParameter paramNom = new OracleParameter(":nombre", nombre);
            this.cmd.Parameters.Add(paramNom);
            OracleParameter paramImg = new OracleParameter(":imagen", imagen);
            this.cmd.Parameters.Add(paramImg);
            OracleParameter paramDesc = new OracleParameter(":descripcion", descripcion);
            this.cmd.Parameters.Add(paramDesc);
            this.cmd.CommandType = CommandType.Text;
            this.cmd.CommandText = sql;
            this.cn.Open();
            int result =  this.cmd.ExecuteNonQuery();
             this.cn.Close();
            this.cmd.Parameters.Clear();
        }
    }
}
