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



    }
}
