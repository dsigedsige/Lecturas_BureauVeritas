using DSIGE.Dato;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSIGE.Negocio
{
   public class Programacion_BL
    {

        public object capa_negocio_get_Suministros(string FechaAsiga, int servicio, int estado)
        {
            try
            {
                return new Programacion_DAO().capa_dato_get_Suministros(FechaAsiga, servicio, estado);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public object capa_negocio_set_actualizarOperario(string obj_cortes, string fecha_asignacion, int servicio, int operario, string fecha_movil, int usuario)
        {
            try
            {
                return new Programacion_DAO().capa_dato_set_actualizarOperario(obj_cortes, fecha_asignacion, servicio, operario, fecha_movil, usuario);
            }
            catch (Exception e)
            {
                throw e;
            }
        }


    }
}
