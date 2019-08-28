using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSIGE.Modelo;
using DSIGE.Negocio;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DSIGE.Web.Controllers
{
    public class Importar_ExcelController : Controller
    {

        // GET: /Agregar_Excel/

        public ActionResult Inicio()
        {
            return View();
        }


        [HttpPost]
        public ActionResult ImportarExcel(HttpPostedFileBase file, int idlocal)
        {
            DateTime _fecha_actual = DateTime.Now;
            try
            {                
                string extension = System.IO.Path.GetExtension(file.FileName);
                string nomExcel = ((Sesion)Session["Session_Usuario_Acceso"]).usuario.usu_id + extension;
                string fileLocation = Server.MapPath("~/Upload") + "\\" + nomExcel;
                file.SaveAs(fileLocation);

                DataTable dt = new DataTable();
                List<LeerLecturas> listaLecturas = new List<LeerLecturas>();
                LeerLecturas objLectura;

                dt = new NLectura().ListaExcel(fileLocation);

                foreach (DataRow row in dt.Rows)
                {
                    objLectura = new LeerLecturas();
                    objLectura.lec_importacion_archivo = nomExcel;
                    objLectura.claseAviso_corte = row["Clase de aviso"].ToString();
                    objLectura.aviso_corte = row["Aviso"].ToString();
                    objLectura.fechaAviso_corte = Convert.ToDateTime(row["Fecha de aviso"].ToString());
                    objLectura.docBloqueo_corte = string.IsNullOrEmpty(row["Documento de bloqueo"].ToString()) ? "" : Convert.ToDateTime(row["Documento de bloqueo"]).ToString("dd/MM/yyyy");

                    objLectura.claseCuenta_corte = row["Cta.Contr."].ToString();
                    objLectura.nombreInterlocutor_corte = row["Nombre interlocutor"].ToString();
                    objLectura.claseCuenta_corte = row["Clase cuenta"].ToString();
                    objLectura.deudaSoles_corte = row["Deuda Soles"].ToString();
                    objLectura.cantidad_corte = row["Cantidad de recibos"].ToString();
                    objLectura.nroInstalacion_corte = row["Instalacion"].ToString();
                    objLectura.medidor_corte = row["N°Ser.Me."].ToString();
                    objLectura.direccion_corte = row["Dirección de Instal."].ToString();
                    objLectura.distrito_corte = row["Distrito de Instal."].ToString();

                    objLectura.unidad_corte = row["Unidad de Lectura"].ToString();
                    objLectura.ejecutante_corte = row["Ejecutante"].ToString();
                    objLectura.orden_corte = Convert.ToInt32(row["Orden"].ToString());
                    objLectura.creadoPor_corte = Convert.ToInt32(row["Creado por"].ToString());
                    objLectura.statusSistem_corte = row["Status de sistema"].ToString();
                    objLectura.codificacion_corte = row["Cód.codificación"].ToString();
                    objLectura.contador = Convert.ToInt32(row["Contador"].ToString());
                    objLectura.tecnico = Convert.ToInt32(row["Cód.codificación"].ToString());
                    objLectura.secuencia = Convert.ToInt32(row["Cód.codificación"].ToString());
                    listaLecturas.Add(objLectura);

                }
                return new ContentResult
                {
                    Content = MvcApplication._Serialize(new NLectura().NImportarArchivoExcel01(listaLecturas)),
                    ContentType = "application/json"
                };
            }
            catch (Exception e)
            {
                return new ContentResult
                {
                    Content = MvcApplication._Serialize(e.Message),
                    ContentType = "application/json"
                };
            }

        }


        /// <summary>
        /// LEE EL EXCEL Y LUEGO GUARDA EN LA TABLA TEMPORAL
        /// </summary>
        /// <param name="file"></param>
        /// <param name="idlocal"></param>
        /// <param name="idfechaAsignacion"></param>
        /// <returns></returns>
        [HttpPost]
        public string InsertaExcel(HttpPostedFileBase file, int idlocal, string idfechaAsignacion, int idServicio)
        {

            List<CorteTemporalCorte> oCortes = new List<CorteTemporalCorte>();
            DateTime _fecha_actual = DateTime.Now;

            try
            {
                //object loDatos = null;

                //string NombreArchivo = file.FileName;
                //string extension = System.IO.Path.GetExtension(file.FileName);
                //string nomExcel = ((Sesion)Session["Session_Usuario_Acceso"]).usuario.usu_id + extension;
                //string fileLocation = Server.MapPath("~/Upload") + "\\" + nomExcel;
                //file.SaveAs(fileLocation);


                object loDatos = null;
                string nomExcel = "";
                string extension = System.IO.Path.GetExtension(file.FileName);

                if (idServicio ==1)
                {
                   nomExcel = idServicio +  "_LECTURAS_" + ((Sesion)Session["Session_Usuario_Acceso"]).usuario.usu_id + extension;
                }
                else if (idServicio == 2)
                {
                    nomExcel = idServicio + "_RELECTURAS_" + ((Sesion)Session["Session_Usuario_Acceso"]).usuario.usu_id + extension;
                }
                else if (idServicio == 3)
                {
                    nomExcel = idServicio + "_CORTES_" + ((Sesion)Session["Session_Usuario_Acceso"]).usuario.usu_id + extension;
                }
                else if (idServicio == 4)
                {
                    nomExcel = idServicio + "_RECONEXIONES_" + ((Sesion)Session["Session_Usuario_Acceso"]).usuario.usu_id + extension;
                }
                else if (idServicio == 6)
                {
                    nomExcel = idServicio + "_REPARTO_" + ((Sesion)Session["Session_Usuario_Acceso"]).usuario.usu_id + extension;
                } 

                string NombreArchivo = file.FileName;
                string fileLocation = Server.MapPath("~/Upload") + "\\" + nomExcel;


                if (System.IO.File.Exists(fileLocation))
                {
                    System.IO.File.Delete(fileLocation);
                }

                file.SaveAs(fileLocation);

                if (idServicio == 1)
                {
                    Cls_Negocio_Importacion_Lecturas Objeto_Negocio = new Cls_Negocio_Importacion_Lecturas();
                    loDatos = Objeto_Negocio.Capa_Negocio_save_temporalLectura(fileLocation, ((Sesion)Session["Session_Usuario_Acceso"]).usuario.usu_id, idlocal, idServicio, idfechaAsignacion, NombreArchivo);

                    var res = _Serialize(loDatos, true);
                    JObject data = JObject.Parse(res.ToString());

                    if (data["ok"].ToString() == "True")
                    {
                        loDatos = null;
                        loDatos = Objeto_Negocio.Capa_Negocio_Agrupado_temporal_lectura(idfechaAsignacion, ((Sesion)Session["Session_Usuario_Acceso"]).usuario.usu_id);
                        return _Serialize(loDatos, true);
                    }
                    else
                    {
                        return res;
                    }
                }
                else if (idServicio == 2)
                {
                    Cls_Negocio_Importacion_Lecturas Objeto_Negocio = new Cls_Negocio_Importacion_Lecturas();
                    Objeto_Negocio.Capa_Negocio_TemporalLecturaCargar(fileLocation, ((Sesion)Session["Session_Usuario_Acceso"]).usuario.usu_id, idlocal, idServicio, idfechaAsignacion, NombreArchivo);

                    loDatos = Objeto_Negocio.Capa_Negocio_Agrupado_temporal_Relectura(idfechaAsignacion, ((Sesion)Session["Session_Usuario_Acceso"]).usuario.usu_id);
                }
                else if (idServicio == 3)
                {
                    NCorte Objeto_Negocio = new NCorte();
                    Objeto_Negocio.Capa_Negocio_ListaExcel(fileLocation, ((Sesion)Session["Session_Usuario_Acceso"]).usuario.usu_id, idlocal, idfechaAsignacion);

                    loDatos = Objeto_Negocio.Capa_Negocio_Listar_Temporal_Cortes_Agrupado(idfechaAsignacion, ((Sesion)Session["Session_Usuario_Acceso"]).usuario.usu_id, idServicio);
                }
                else if (idServicio == 4)
                {
                    NCorte Objeto_Negocio = new NCorte();
                    Objeto_Negocio.Capa_Negocio_ListaExcelReconexiones(fileLocation, ((Sesion)Session["Session_Usuario_Acceso"]).usuario.usu_id, idlocal, idfechaAsignacion);

                    loDatos = Objeto_Negocio.Capa_Negocio_Listar_Temporal_Reconexiones_Agrupado(idfechaAsignacion, ((Sesion)Session["Session_Usuario_Acceso"]).usuario.usu_id, idServicio);
                }
                else if (idServicio == 6)
                {
                    NCorte Objeto_Negocio = new NCorte();
                    Objeto_Negocio.Capa_Negocio_ListaExcelReparto(fileLocation, ((Sesion)Session["Session_Usuario_Acceso"]).usuario.usu_id, idlocal, idfechaAsignacion);

                    loDatos = Objeto_Negocio.Capa_Negocio_Listar_Temporal_Reparto_Agrupado(idfechaAsignacion, ((Sesion)Session["Session_Usuario_Acceso"]).usuario.usu_id, idServicio);
                }
                else
                {
                    return null;
                }
                //NLectura obj_Lectura_Importacion_Bl = new NLectura();

                return _Serialize(loDatos, true);


            }
            catch (Exception ex)
            {
                return _Serialize(ex.Message, true);
            }



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


        /// <summary>
        /// INSERTAR DE LA TABLA TEMPORAL A LA TABLA VERDADERA
        /// </summary>
        /// <param name="fechaAsignacion"></param>
        /// <param name="id_servicio"></param>
        /// <param name="nombre_archivo"></param>
        /// <returns></returns>
        [HttpPost]
        public string JsonRegistrarTemp_LecturasExcel(string fechaAsignacion, int id_servicio, string nombre_archivo)
        {
            object loDatos = null;
            try
            {
                if (id_servicio == 2)
                {
                    var usuario = ((Sesion)Session["Session_Usuario_Acceso"]).usuario.usu_id;

                    Cls_Negocio_Importacion_Lecturas obj_negocio = new Cls_Negocio_Importacion_Lecturas();
                    loDatos = obj_negocio.Capa_Negocio_MigracionTemporalRelectura(fechaAsignacion, id_servicio, nombre_archivo, usuario);
                }
                //corte
                else if (id_servicio == 3)
                {
                    var usuario = ((Sesion)Session["Session_Usuario_Acceso"]).usuario.usu_id;

                    NCorte obj_negocio = new NCorte();
                    loDatos = obj_negocio.Capa_Negocio_MigracionTablaTemporalCorte(fechaAsignacion, id_servicio, nombre_archivo, usuario);

                }
                // reconexion
                else if (id_servicio == 4)
                {
                    var usuario = ((Sesion)Session["Session_Usuario_Acceso"]).usuario.usu_id;

                    NCorte obj_negocio = new NCorte();
                    loDatos = obj_negocio.Capa_Negocio_MigracionTablaTemporalReconexiones(fechaAsignacion, id_servicio, nombre_archivo, usuario);

                }
                else if (id_servicio == 6)
                {
                    var usuario = ((Sesion)Session["Session_Usuario_Acceso"]).usuario.usu_id;

                    NCorte obj_negocio = new NCorte();
                    loDatos = obj_negocio.Capa_Negocio_MigracionTablaTemporalReparto(fechaAsignacion, id_servicio, nombre_archivo, usuario);
                }
                else { }
                return _Serialize(loDatos, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        [HttpPost]
        public string set_enviarMovil_lecturas(string fechaAsignacion, string fechaMovil, int id_servicio, string nombre_archivo)
        {
            object loDatos = null;
            try
            {
                var usuario = ((Sesion)Session["Session_Usuario_Acceso"]).usuario.usu_id;
                Cls_Negocio_Importacion_Lecturas obj_negocio = new Cls_Negocio_Importacion_Lecturas();
                loDatos = obj_negocio.Capa_Negocio_save_Lecturas(fechaAsignacion, fechaMovil, id_servicio, nombre_archivo, usuario);
                return _Serialize(loDatos, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost]
        public string Lecturas_Detallado(int idServ, int idtecnico, string distrito)
        {

            try
            {
                object obj_list_datos = "";
                if (idServ == 1)
                {
                    Cls_Negocio_Importacion_Lecturas Objeto_Negocio = new Cls_Negocio_Importacion_Lecturas();
                    List<Cls_Entidad_Importacion_Lecturas> objeto_List_Importacion_Lecturas = new List<Cls_Entidad_Importacion_Lecturas>();
                    obj_list_datos = Objeto_Negocio.Capa_Negocio_Listar_Detalle_Lectura_Relectura(((Sesion)Session["Session_Usuario_Acceso"]).usuario.usu_id, idtecnico, distrito);                   
                }
                else if (idServ == 2)
                {
                    Cls_Negocio_Importacion_Lecturas Objeto_Negocio = new Cls_Negocio_Importacion_Lecturas();
                    List<Cls_Entidad_Importacion_Lecturas> objeto_List_Importacion_Lecturas = new List<Cls_Entidad_Importacion_Lecturas>();

                    obj_list_datos = Objeto_Negocio.Capa_Negocio_Listar_Detalle_Lectura_Relectura(((Sesion)Session["Session_Usuario_Acceso"]).usuario.usu_id, idtecnico, distrito);
                }
                else if (idServ == 3)
                {
                    NCorte Objeto_Negocio = new NCorte();
                    List<CorteTemporalCorte> objeto_List_Importacion_Cortes = new List<CorteTemporalCorte>();
                    obj_list_datos = Objeto_Negocio.Capa_Negocio_Listar_TemporalCorte(((Sesion)Session["Session_Usuario_Acceso"]).usuario.usu_id, idtecnico, distrito);

                }
                else if (idServ == 4)
                {
                    NCorte Objeto_Negocio = new NCorte();
                    List<CorteTemporalCorte> objeto_List_Importacion_Cortes = new List<CorteTemporalCorte>();
                    obj_list_datos = Objeto_Negocio.Capa_Negocio_Listar_TemporalReconexion(((Sesion)Session["Session_Usuario_Acceso"]).usuario.usu_id, idtecnico, distrito);
                }
                

                return _Serialize(obj_list_datos, true);

            }
            catch (Exception ex)
            {
                return _Serialize(ex.Message, true);
            }


        }


        /// <summary>
        /// Lista los servicios
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string ListadoServicios()
        {

            object loDatos;
            try
            {
                Cls_Negocio_Importacion_Lecturas objeto_negocio = new Cls_Negocio_Importacion_Lecturas();

                loDatos = objeto_negocio.Capa_Negocio_PermisoListUsuarioServicio(((Sesion)Session["Session_Usuario_Acceso"]).usuario.usu_id);

                //loDatos = objeto_negocio.Capa_Negocio_Listado_Servicios(); ;

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return _Serialize(loDatos, true);
        }


        [HttpPost]
        public string set_EnviarCorreo(int servicio, string fecha_Asigna)
        {
            object loDatos;
            try
            {
                Cls_Negocio_Importacion_Lecturas Objeto_Negocio = new Cls_Negocio_Importacion_Lecturas();
                loDatos= Objeto_Negocio.Capa_Negocio_set_EnviarCorreo(servicio, fecha_Asigna, ((Sesion)Session["Session_Usuario_Acceso"]).usuario.usu_id);

                return _Serialize(loDatos, true);

            }
            catch (Exception ex)
            {
                return _Serialize(ex.Message, true);
            }


        }



        //(Sesion)Session["Session_Usuario_Acceso"]).usuario.usu_id
        // Capa_Negocio_PermisoListUsuarioServicio

    }
}
