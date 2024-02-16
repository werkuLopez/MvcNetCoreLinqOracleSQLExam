using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MvcNetCoreLinqOracleSQLExam.Models;
using MvcNetCoreLinqOracleSQLExam.Repositories;
using System.Collections.Generic;


namespace MvcNetCoreLinqOracleSQLExam.Controllers
{
    public class ComicsController : Controller
    {
        IComicRepository repo;
        public ComicsController( IComicRepository repo)
        {
            this.repo = repo;
        }
        public IActionResult Index()
        {
            List<Comic> comics = this.repo.GetComics();
            //if(comics == null )
            //{
            //    ViewData["Alerta"] = "No hay comics";
            //}
            return View(comics);
        }

        public IActionResult BuscarComic()
        {
            List<Comic> comics = this.repo.GetComics();
            ViewData["Comics"] = comics;                       

            return View();
        }

        [HttpPost]
        public IActionResult BuscarComic(int idcomic)
        {
            List<Comic> comics = this.repo.GetComics();
            ViewData["Comics"] = comics;

            Comic comic = this.repo.GetComicById(idcomic);
            return View(comic);
        }

        public IActionResult CreateProcedure()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateProcedure(Comic comic)
        {
            this.repo.InsertarComicProcedure(comic.Nombre, comic.Imagen, comic.Descripcion);
            return RedirectToAction("Index");
        }

        public IActionResult CreateTexto()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateTexto(Comic comic)
        {
            this.repo.InsertarComicTexto(comic.Nombre, comic.Imagen, comic.Descripcion);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int idcomic)
        {
            Comic comic = this.repo.GetComicById(idcomic);
            return View(comic);
        }

        [HttpPost]
        public IActionResult Delete(int? idcomic)
        {
            this.repo.EliminarComic(idcomic.Value);
            return RedirectToAction("Index");
        }
    }
}
