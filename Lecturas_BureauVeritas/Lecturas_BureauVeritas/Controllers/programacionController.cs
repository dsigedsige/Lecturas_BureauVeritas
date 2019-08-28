﻿using DSIGE.Modelo;
using DSIGE.Negocio;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lecturas_BureauVeritas.Controllers
{
    public class programacionController : Controller
    {
        //
        // GET: /programacion/

        public ActionResult inicio_index()
        {
            return View();
        }

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
        public string get_Suministros(string FechaAsiga, int servicio, int estado)
        {
            object loDatos;
            try
            {
                loDatos = new Programacion_BL().capa_negocio_get_Suministros(FechaAsiga, servicio, estado);
                return _Serialize(loDatos, true);
            }
            catch (Exception ex)
            {
                return _Serialize(ex.Message, true);
            }
        }
 

    [HttpPost]
        public string set_actualizarOperario(string obj_cortes, string fecha_asignacion, int servicio, int operario, string fecha_movil)
        {
            object loDatos;
            try
            {
                loDatos = new Programacion_BL().capa_negocio_set_actualizarOperario(obj_cortes, fecha_asignacion, servicio, operario, fecha_movil, ((Sesion)Session["Session_Usuario_Acceso"]).usuario.usu_id);
                return _Serialize(loDatos, true);
            }
            catch (Exception ex)
            {
                return _Serialize(ex.Message, true);
            }
        }


        



    }
}
