
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSIGE.Negocio;
using Newtonsoft.Json;
using System.IO;

using Excel = OfficeOpenXml;
using Style = OfficeOpenXml.Style;
using DSIGE.Modelo;
using DSIGE.Negocio;
using System.Configuration;
using System.Drawing;
using System.Globalization;

namespace DSIGE.Web.Controllers
{
    public class ExportarTrabajosLecturasController : Controller
    {
        //
        // GET: /ExportarTrabajosLecturas/

        public ActionResult ExportarTrabajosLecturas()
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
      public string MostrarInformacion(string fechaAsignacion, int TipoServicio )
        {
            object loDatos;
            try
            {
                Cls_Negocio_Export_trabajos_lectura obj_negocio = new Cls_Negocio_Export_trabajos_lectura();
                loDatos = obj_negocio.Capa_Negocio_Get_ListaLecturas(fechaAsignacion, TipoServicio);
                return _Serialize(loDatos, true);
            }
            catch (Exception ex)
            {
                return _Serialize(ex.Message, true);
            }

        }

      [HttpPost]
      public string DescargaExcel(string fechaAsignacion, int TipoServicio)
      {
            int _fila = 2;
            string _ruta;
            string nombreArchivo = "";
            string ruta_descarga = ConfigurationManager.AppSettings["Archivos"];
            var usuario = ((Sesion)Session["Session_Usuario_Acceso"]).usuario.usu_id;

          try
          {           
              List<Cls_Entidad_Export_trabajos_lectura> _lista = new List<Cls_Entidad_Export_trabajos_lectura>();

              Cls_Negocio_Export_trabajos_lectura obj_negocio = new DSIGE.Negocio.Cls_Negocio_Export_trabajos_lectura();
              _lista = obj_negocio.Capa_Negocio_Get_ListaLecturas_Excel(fechaAsignacion, TipoServicio);
              
              if (_lista.Count == 0)
              {
                  return _Serialize("0|No hay informacion para mostrar.", true);
              }

              if (TipoServicio==1)
              {
                  nombreArchivo = "LECTURAS_EXPORTADO_" + usuario + ".xls";
              }
              else if (TipoServicio == 2)
              {
                  nombreArchivo = "RELECTURAS_EXPORTADO_" + usuario + ".xls";
              }
                else if (TipoServicio == 9)
                {
                    nombreArchivo = "RECLAMOS_EXPORTADO_" + usuario + ".xls";
                }

                _ruta = Path.Combine(Server.MapPath("~/Temp") + "\\" + nombreArchivo );

              FileInfo _file = new FileInfo(_ruta);
              if (_file.Exists)
              {
                  _file.Delete();
                  _file = new FileInfo(_ruta);
              }

              using (Excel.ExcelPackage oEx = new Excel.ExcelPackage(_file))
              {
                  Excel.ExcelWorksheet oWs = oEx.Workbook.Worksheets.Add("Importar");
                    oWs.Cells.Style.Font.SetFromFont(new Font("Tahoma", 8));
                    for (int i = 1; i <= 21; i++)
                    {
                        oWs.Cells[1, i].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        oWs.Cells[1, i].Style.Font.Size = 9; //letra tamaño   
                        oWs.Cells[1, i].Style.Font.Bold = true; //Letra negrita 
                    } 

                    oWs.Cells[1, 1].Value = "ITEM";
                    oWs.Cells[1, 2].Value = "NOBORAR";
                    oWs.Cells[1, 3].Value = "INSTALACIÓN";                    
                    oWs.Cells[1, 4].Value = "APARATO";

                    oWs.Cells[1, 5].Value = "TIPO CALLE";
                    oWs.Cells[1, 6].Value = "NOMBRE DE CALLE";
                    oWs.Cells[1, 7].Value = "ALTURA DE CALLE";
                    oWs.Cells[1, 8].Value = "NÚMERO DE EDIFICIO";
                    oWs.Cells[1, 9].Value = "NÚMERO DE DEPARTAMENTO";
 
                    oWs.Cells[1, 10].Value = "DETALLE CONSTRUCCIÓN (OBJETO DE CONEXIÓN)";
                    oWs.Cells[1, 11].Value = "CONJUNTO DE VIVIENDA (OBJETO DE CONEXIÓN)";
                    oWs.Cells[1, 12].Value = "MANZANA/LOTE";
                    oWs.Cells[1, 13].Value = "DISTRITO";

                    oWs.Cells[1, 14].Value = "CUENTA CONTRATO";
                    oWs.Cells[1, 15].Value = "SECUENCIA DE LECTURA";
                    oWs.Cells[1, 16].Value = "UNIDAD DE LECTURA";
                    oWs.Cells[1, 17].Value = "NÚMERO DE LECTURAS ESTIMADAS CONSECUTIVAS";
 
                    oWs.Cells[1, 18].Value = "EMPRESA LECTORA"; 
                    oWs.Cells[1, 19].Value = "NOTA 2 DE LA UBICACIÓN DEL APARATO";
                    oWs.Cells[1, 20].Value = "TECNICO";
                    oWs.Cells[1, 21].Value = "SECUENCIA"; 

                    int acu = 0;
                   foreach (Cls_Entidad_Export_trabajos_lectura oBj in _lista)
                    {
                        acu = acu + 1 ;

                        for (int i = 1; i <= 21; i++)
                        {
                            oWs.Cells[_fila, i].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        }

                        oWs.Cells[_fila,1].Value = acu;
                        oWs.Cells[_fila,2].Value = oBj.id_Lectura;
                        oWs.Cells[_fila,3].Value= oBj.Instalacion;

                        //oWs.Cells[_fila, 4].Style.Numberformat.Format = "#,##0";
                        //oWs.Cells[_fila, 4].Value = Convert.ToDouble(oBj.Aparato);
                        oWs.Cells[_fila, 4].Value =  oBj.Aparato;

                        oWs.Cells[_fila, 5].Value = oBj.Tipo_calle;
                        oWs.Cells[_fila, 6].Value = oBj.Nombre_Calle;
                        oWs.Cells[_fila, 7].Value = oBj.Altura_Calle;
                        oWs.Cells[_fila, 8].Value = oBj.Numero_Edificio;
                        oWs.Cells[_fila, 9].Value = oBj.Numero_Departamento; 
 
                        oWs.Cells[_fila, 10].Value = oBj.Detalle_Construccion;
                        oWs.Cells[_fila, 11].Value = oBj.Conjunto_Vivienda;
                        oWs.Cells[_fila, 12].Value = oBj.Manzana_Lote;
                        oWs.Cells[_fila, 13].Value = oBj.Distrito;

                        oWs.Cells[_fila, 14].Value = oBj.Cuenta_contrato;
                        oWs.Cells[_fila, 15].Value = oBj.Secuencia_lectura;
                        oWs.Cells[_fila, 16].Value = oBj.Unidad_lectura;
                        oWs.Cells[_fila, 17].Value = oBj.Numero_lecturas_estimadas_consecutivas; 
                        oWs.Cells[_fila, 18].Value = oBj.Empresa_Lectora ;
 
                        oWs.Cells[_fila, 19].Value = oBj.Nota_2_ubicacion_aparato;
                        oWs.Cells[_fila, 20].Value = oBj.Tecnico ;
                        oWs.Cells[_fila, 21].Value = oBj.Secuencia; 

                      _fila++;
                  }

                oWs.Row(1).Style.Font.Bold = true;
                oWs.Row(1).Style.HorizontalAlignment = Style.ExcelHorizontalAlignment.Center;
                oWs.Row(1).Style.VerticalAlignment = Style.ExcelVerticalAlignment.Center;

                oWs.Column(1).Style.Font.Bold = true;

                for (int i = 1; i <=21; i++)
                {
                    oWs.Column(i).AutoFit(); 
                }
            
                  oEx.Save();
              }


              return _Serialize("1|" + ruta_descarga+ nombreArchivo, true);


          }
          catch (Exception ex )
          {
          
            return _Serialize("0|" + ex.Message, true);
          }           

      }

      [HttpPost]
      public string ListandoServicios()
      {
          object loDatos;
          try
          {
              Cls_Negocio_AsignarOrdenTrabajo obj_negocio = new Cls_Negocio_AsignarOrdenTrabajo();
              loDatos = obj_negocio.Capa_Negocio_Get_ListaServicioXusuario_II(((Sesion)Session["Session_Usuario_Acceso"]).usuario.usu_id);
              return _Serialize(loDatos, true);
          }
          catch (Exception ex)
          {
              return _Serialize(ex.Message, true);
          }

      }


    }
}
