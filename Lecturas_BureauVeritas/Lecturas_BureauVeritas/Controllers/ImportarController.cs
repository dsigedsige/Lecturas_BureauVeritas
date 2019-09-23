using DSIGE.Modelo;
using DSIGE.Negocio;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lecturas_BureauVeritas.Controllers
{
    public class ImportarController : Controller
    {
        //
        // GET: /Importar/

        public ActionResult importar_index()
        {
            return View();
        }


        /// <summary>
        /// SERIALIZACION DE JSONPROPERTY
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignore"></param>
        /// <returns></returns>
        public static string _Serialize(object value, bool ignore = false)
        {
            var SerializerSettings = new JsonSerializerSettings()
            {
                MaxDepth = Int32.MaxValue,
                NullValueHandling = (ignore == true ? NullValueHandling.Ignore : NullValueHandling.Include)
            };

            return JsonConvert.SerializeObject(value, Formatting.Indented, SerializerSettings);
        }


        [HttpPost]
        public string get_marcaMedidor()
        {

            object loDatos;
            try
            {
                Cls_Negocio_Importacion_Lecturas objeto_negocio = new Cls_Negocio_Importacion_Lecturas();
                //loDatos = objeto_negocio.Capa_Negocio_PermisoListUsuarioServicio(((Sesion)Session["Session_Usuario_Acceso"]).usuario.usu_id);
                loDatos = objeto_negocio.Capa_Negocio_get_marcaMedidor();

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return _Serialize(loDatos, true);
        }

        [HttpPost]
        public string get_buscarCodigoEmr(string codigo)
        {

            object loDatos;
            try
            {
                Cls_Negocio_Importacion_Lecturas objeto_negocio = new Cls_Negocio_Importacion_Lecturas();
                loDatos = objeto_negocio.Capa_Negocio_buscarCodigoEMr(codigo);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return _Serialize(loDatos, true);
        }


        [HttpPost]
        public string save_GrandesClientesfile(HttpPostedFileBase file, int Id_GrandeCliente, string CodigoEMR, int  id_marcaMedidor,  string fechaCarga)
        {
             List<CorteTemporalCorte> oCortes = new List<CorteTemporalCorte>();
            DateTime _fecha_actual = DateTime.Now;
            string fileLocation = "";
            try
            {
                object loDatos = null;
                int resFile = 0;

                Cls_Negocio_Importacion_Lecturas objeto_negocio = new Cls_Negocio_Importacion_Lecturas();
                resFile = objeto_negocio.Capa_Negocio_grabarGrandesClienteFile(Id_GrandeCliente, CodigoEMR, file.FileName, file.FileName, id_marcaMedidor, fechaCarga, ((Sesion)Session["Session_Usuario_Acceso"]).usuario.usu_id);
                
                if (resFile > 0)
                {
                    fileLocation = System.Web.Hosting.HostingEnvironment.MapPath("~/Files_GrandesClientes/" + resFile);
                    file.SaveAs(fileLocation);

                    if (!System.IO.File.Exists(fileLocation))
                    {
                        loDatos = "No se pudo guardar el archivo : ";
                    }
                    else {
                        loDatos = "OK";


                        //metodo para poder descargar el archivo...
                        //string rutaArchivo = "1";
                        //string nombre_File = "añoNuevo.jpg";
                        //string nombreArchivoReal = "";

                        //nombreArchivoReal = rutaArchivo.Replace(rutaArchivo, nombre_File);

                        //string rutaOrig = System.Web.Hosting.HostingEnvironment.MapPath("~/Files_GrandesClientes/" + rutaArchivo);
                        //string rutaDest = System.Web.Hosting.HostingEnvironment.MapPath("~/Files_GrandesClientes/Descargas/" + nombreArchivoReal);

                        //if (System.IO.File.Exists(rutaDest)) //--- restaurarlo
                        //{
                        //    System.IO.File.Delete(rutaDest);
                        //    System.IO.File.Copy(rutaOrig, rutaDest);
                        //}
                        //else {
                        //    System.IO.File.Copy(rutaOrig, rutaDest);
                        //}                      
                    }
                }                
                ////_rutaServer = ConfigurationManager.AppSettings["servidor-archivos"];
                return _Serialize(loDatos, true);

            }
            catch (Exception ex)
            {
                return _Serialize(ex.Message, true);
            }
        }




    }
}
